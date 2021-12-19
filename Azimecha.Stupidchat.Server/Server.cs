using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Protocol;
using System.Diagnostics;
using System.Net;
using System.Linq;
using Azimecha.Stupidchat.Core;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Server {
    public class Server : IDisposable {
        private byte[] _arrPrivateKey;
        private ConcurrentDictionary<string, ClientConnection> _dicConnections;
        private TcpListener _listener;
        private CancellationTokenSource _ctsDisposing;
        private TaskCompletionSource<int> _tcsServerStart;
        private Thread _thdServer;
        private Core.Structures.ServerInfo _info;
        private ThreadLocalLazy<SQLite.SQLiteConnection> _tllConnections;
        private string _strDatabasePath;

        public Server(ReadOnlySpan<byte> spanPrivateKey, IPEndPoint endpointListenOn, Core.Structures.ServerInfo info, string strDatabasePath) {
            _dicConnections = new ConcurrentDictionary<string, ClientConnection>();
            _listener = new TcpListener(endpointListenOn);
            _ctsDisposing = new CancellationTokenSource();
            _info = info;

            _dicRequestProcessors = ProcessorAttribute.BindProcessorsList<Server, RequestMessage, ResponseMessage>(this, _dicRequestProcessorMethods);

            _strDatabasePath = strDatabasePath;
            _tllConnections = new ThreadLocalLazy<SQLite.SQLiteConnection>(ConnectToDatabase);
            InitDatabase();

            _tcsServerStart = new TaskCompletionSource<int>();
            Task<int> taskServerStart = _tcsServerStart.Task;

            _thdServer = new Thread(ServerThread);
            _thdServer.Name = "Server Thread";
            _thdServer.Start(new WeakReference<Server>(this));

            taskServerStart.Wait();
        }

        public void Dispose() {
            CancellationTokenSource ctsDisposing = Interlocked.Exchange(ref _ctsDisposing, null);
            _ctsDisposing?.Cancel();
            _ctsDisposing?.Dispose();

            ConcurrentDictionary<string, ClientConnection> dicConnections = Interlocked.Exchange(ref _dicConnections, null);
            if (!(dicConnections is null)) {
                foreach (ClientConnection conn in dicConnections.Values)
                    conn.Dispose();
            }

            _arrPrivateKey = null;
        }

        internal ReadOnlySpan<byte> PrivateKey => _arrPrivateKey;
        internal SQLite.SQLiteConnection Database => _tllConnections.Value;

        private SQLite.SQLiteConnection ConnectToDatabase() => new SQLite.SQLiteConnection(_strDatabasePath);

        private void InitDatabase() {
            SQLite.SQLiteConnection db = Database;
            db.CreateTable<Records.ChannelRecord>();
            db.CreateTable<Records.MemberRecord>();
            db.CreateTable<Records.MessageRecord>();
        }

        private void AcceptConnection(TcpClient client) {
            ClientConnection conn = new ClientConnection(this, client);
            string strClientUserID = conn.Connection.PartnerPublicKey.ToHexString();

            try {
                if (!_dicConnections.TryAdd(strClientUserID, conn))
                    throw new InvalidOperationException("You are already connected");

                NewUserCheck(conn);
            } catch (Exception ex) {
                ClientConnection connDummy;
                _dicConnections.TryRemove(strClientUserID, out connDummy);
                conn.Connection.SendErrorNotification(ex);
                conn.Dispose();
            }
        }

        private void NewUserCheck(ClientConnection conn) {
            byte[] arrPartnerPublicKey = conn.Connection.PartnerPublicKey.ToArray();

            SQLite.SQLiteConnection db = Database;
            Records.MemberRecord member;

            db.RunInTransaction(() => {
                member = db.Table<Records.MemberRecord>().Where(m => m.PublicKey.SequenceEqual(arrPartnerPublicKey)).FirstOrDefault();
                if (member is null) {
                    member = new Records.MemberRecord() { PublicKey = arrPartnerPublicKey };
                    db.Insert(member);
                }
            });
        }

        private void BroadcastNotification(NotificationMessage msgNotification) {
            lock (_dicConnections)
                _dicConnections.Values.AsParallel().ForAll(conn => conn.Connection.SendNotification(msgNotification));
        }

        private void HandleServerThreadError(Exception ex) {
            Console.WriteLine($"Error in server thread: {ex}");
        }

        private static void ServerThread(object objServerWeak) {
            WeakReference<Server> weakServer = (WeakReference<Server>)objServerWeak;
            TcpListener listener;
            CancellationToken ctDisposing;

            try {
                Server server = weakServer.GetValue();
                if (server is null) return;

                listener = server._listener;
                ctDisposing = server._ctsDisposing.Token;
                TaskCompletionSource<int> tcsServerStart = server._tcsServerStart;

                listener.Start();
                tcsServerStart.SetResult(1);
            } catch (Exception ex) {
                try { 
                    weakServer.GetValue()._tcsServerStart.SetException(ex); 
                } catch (Exception ex2) {
                    Console.WriteLine($"Error initializing server thread: {ex}");
                    Console.WriteLine($"Error reporting server thread initialization error: {ex2}");
                }
                return;
            }

            while (true) {
                try {
                    Task<TcpClient> taskAccept = listener.AcceptTcpClientAsync();
                    taskAccept.Wait(ctDisposing);

                    if (ctDisposing.IsCancellationRequested) {
                        Console.WriteLine($"Server thread exiting");
                        return;
                    }

                    weakServer.GetValue()?.AcceptConnection(taskAccept.Result);
                } catch (Exception ex) {
                    try {
                        weakServer.GetValue().HandleServerThreadError(ex);
                    } catch (Exception ex2) {
                        Console.WriteLine($"Error in server thread: {ex}");
                        Console.WriteLine($"Error reporting server thread error: {ex2}");
                        return;
                    }
                }
            }
        }

        private static readonly IDictionary<Type, System.Reflection.MethodInfo> _dicRequestProcessorMethods
            = ProcessorAttribute.BuildProcessorsList<Server, RequestMessage>();

        private readonly IDictionary<Type, Func<RequestMessage, ResponseMessage>> _dicRequestProcessors;

        internal ResponseMessage HandleRequest(ClientConnection conn, RequestMessage msgRequest) {
            Type typeRequest = msgRequest.GetType();
            Func<RequestMessage, ResponseMessage> procHandler;

            if (_dicRequestProcessors.TryGetValue(typeRequest, out procHandler))
                return procHandler(msgRequest);

            throw new NoHandlerException($"No request handler for {typeRequest.FullName}");
        }

        [Processor(typeof(Core.Requests.ChannelsRequest))]
        private Core.Requests.ChannelsResponse HandleChannelsRequest(Core.Requests.ChannelsRequest req) => new Core.Requests.ChannelsResponse() { 
            Channels = Database.Table<Records.ChannelRecord>()
                .Select(chan => chan.ToChannelInfo())
                .ToArray() 
        };

        [Processor(typeof(Core.Requests.MembersRequest))]
        private Core.Requests.MembersResponse HandleMembersRequest(Core.Requests.MembersRequest req) => new Core.Requests.MembersResponse() {
            Members = Database.Table<Records.MemberRecord>()
                .Select(memb => memb.ToMemberInfo())
                .ToArray()
        };

        [Processor(typeof(Core.Requests.MessagesBeforeRequest))]
        private Core.Requests.MessagesBeforeResponse HandleMessagesBeforeRequest(Core.Requests.MessagesBeforeRequest req) => new Core.Requests.MessagesBeforeResponse() {
            Messages = Database.Table<Records.MessageRecord>()
                .Where(msg => msg.ChannelID == req.InChannel && msg.MessageIndex < req.BeforeIndex)
                .OrderByDescending(msg => -msg.MessageIndex)
                .Take(req.MaxCount)
                .Select(msg => msg.ToMessageData())
                .ToArray()
        };

        [Processor(typeof(Core.Requests.SingleMessageRequest))]
        private Core.Requests.SingleMessageResponse HandleSingleMessageRequest(Core.Requests.SingleMessageRequest req) => new Core.Requests.SingleMessageResponse() {
            Message = Database.Table<Records.MessageRecord>()
                .Where(msg => msg.ChannelID == req.Channel && msg.MessageIndex == req.Index)
                .FirstOrDefault()
                ?.ToMessageData()
        };
    }
}
