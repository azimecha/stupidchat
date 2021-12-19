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
        private object _objInfoMutex;

        public Server(ReadOnlySpan<byte> spanPrivateKey, IPEndPoint endpointListenOn, Core.Structures.ServerInfo info, string strDatabasePath) {
            _dicConnections = new ConcurrentDictionary<string, ClientConnection>();
            _listener = new TcpListener(endpointListenOn);
            _ctsDisposing = new CancellationTokenSource();
            _info = info;
            _objInfoMutex = new object();

            //_dicRequestProcessors = ProcessorAttribute.BindProcessorsList<Server, RequestMessage, ResponseMessage>(this, _dicRequestProcessorMethods);

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

        public static ILogListener LogListener { get; set; }

        public Core.Structures.ServerInfo BasicInfo {
            get {
                lock (_objInfoMutex)
                    return _info;
            }
            set {
                lock (_objInfoMutex)
                    _info = value;
                BroadcastNotification(new Core.Notifications.ServerInfoChangedNotification() { Info = value });
            }
        }

        public long AddChannel(Core.Structures.ChannelInfo infChannel) {
            Records.ChannelRecord channel = new Records.ChannelRecord() {
                Description = infChannel.Description,
                Name = infChannel.Name,
                Type = infChannel.Type
            };

            SQLite.SQLiteConnection db = Database;
            db.RunInTransaction(() => {
                db.Insert(channel);
                channel.ID = db.Table<Records.ChannelRecord>().OrderByDescending(chan => chan.ID).First().ID;
            });

            BroadcastNotification(new Core.Notifications.ChannelAddedNotification() { Channel = channel.ToChannelInfo() });
            return channel.ID;
        }

        public void UpdateChannel(Core.Structures.ChannelInfo infChannel) {
            Records.ChannelRecord channel = null;

            SQLite.SQLiteConnection db = Database;
            db.RunInTransaction(() => {
                channel = db.Table<Records.ChannelRecord>().Where(chan => chan.ID == infChannel.ID).First();
                channel.Description = infChannel.Description;
                channel.Name = infChannel.Name;
                db.Update(channel);
            });

            BroadcastNotification(new Core.Notifications.ChannelInfoChangedNotification() { Channel = channel.ToChannelInfo() });
        }

        public void RemoveChannel(long nChannelID) {
            SQLite.SQLiteConnection db = Database;

            db.RunInTransaction(() => {
                if (db.Delete<Records.ChannelRecord>(nChannelID) == 0)
                    throw new RecordNotFoundException($"No channel with ID {nChannelID}");

                db.Table<Records.MessageRecord>().Where(msg => msg.ChannelID == nChannelID).Delete();
            });

            BroadcastNotification(new Core.Notifications.ChannelRemovedNotification() { ChannelID = nChannelID });
        }

        public void RemoveMessage(long nChannelID, long nMessageIndex) {
            if (Database.Table<Records.MessageRecord>().Where(msg => msg.ChannelID == nChannelID && msg.MessageIndex == nMessageIndex).Delete() == 0)
                throw new RecordNotFoundException($"No message with index {nMessageIndex} in channel {nChannelID}");

            BroadcastNotification(new Core.Notifications.MessageDeletedNotification() { ChannelID = nChannelID, MessageIndex = nMessageIndex });
        }

        public void SetMemberPower(long nMemberID, Core.Structures.PowerLevel power)
            => ModifyMember(nMemberID, memb => memb.Power = power);

        public void SetMemberNickname(long nMemberID, string strNickname)
            => ModifyMember(nMemberID, memb => memb.Nickname = strNickname);

        private void ModifyMember(long nMemberID, Action<Records.MemberRecord> procAction) {
            Records.MemberRecord member = null;
            SQLite.SQLiteConnection db = Database;

            db.RunInTransaction(() => {
                member = db.Table<Records.MemberRecord>().Where(memb => memb.MemberID == nMemberID).First();
                procAction(member);
                db.Update(member);
            });

            BroadcastNotification(new Core.Notifications.MemberInfoChangedNotification() { Member = member.ToMemberInfo() });
        }

        public IEnumerable<Core.Structures.ChannelInfo> Channels
            => Database.Table<Records.ChannelRecord>().Select(chan => chan.ToChannelInfo());

        public IEnumerable<Core.Structures.MessageData> GetMessages(long nChannelID)
            => Database.Table<Records.MessageRecord>().Where(msg => msg.ChannelID == nChannelID).Select(msg => msg.ToMessageData());

        public IEnumerable<Core.Structures.MemberInfo> Members
            => Database.Table<Records.MemberRecord>().Select(memb => memb.ToMemberInfo());

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
            SQLite.SQLiteConnection db = Database;
            Records.MemberRecord member;

            db.RunInTransaction(() => {
                member = TryGetMemberRecord(conn.Connection.PartnerPublicKey);
                if (member is null) {
                    member = new Records.MemberRecord() { PublicKey = conn.Connection.PartnerPublicKey.ToArray() };
                    db.Insert(member);
                }
            });
        }

        private void BroadcastNotification(NotificationMessage msgNotification) {
            ClientConnection[] arrConnections;

            lock (_dicConnections)
                arrConnections = _dicConnections.Values.ToArray();

            arrConnections.AsParallel().ForAll(conn => {
                try {
                    conn.Connection.SendNotification(msgNotification);
                } catch (Exception ex) {
                    LogListener.LogMessage($"Error sending {msgNotification.GetType().FullName} to client: {ex}");
                }
           });
        }

        private void HandleServerThreadError(Exception ex) {
            LogListener.LogMessage($"Error in server thread: {ex}");
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
                    LogListener.LogMessage($"Error initializing server thread: {ex}");
                    LogListener.LogMessage($"Error reporting server thread initialization error: {ex2}");
                }
                return;
            }

            while (true) {
                try {
                    Task<TcpClient> taskAccept = listener.AcceptTcpClientAsync();
                    taskAccept.Wait(ctDisposing);

                    if (ctDisposing.IsCancellationRequested) {
                        LogListener.LogMessage($"Server thread exiting");
                        return;
                    }

                    weakServer.GetValue()?.AcceptConnection(taskAccept.Result);
                } catch (Exception ex) {
                    try {
                        weakServer.GetValue().HandleServerThreadError(ex);
                    } catch (Exception ex2) {
                        LogListener.LogMessage($"Error in server thread: {ex}");
                        LogListener.LogMessage($"Error reporting server thread error: {ex2}");
                        return;
                    }
                }
            }
        }

        private static readonly IDictionary<Type, System.Reflection.MethodInfo> _dicRequestProcessorMethods
            = ProcessorAttribute.BuildProcessorsList<Server, RequestMessage>();

        internal ResponseMessage HandleRequest(ClientConnection conn, RequestMessage msgRequest) {
            Type typeRequest = msgRequest.GetType();
            System.Reflection.MethodInfo infHandler;

            if (_dicRequestProcessorMethods.TryGetValue(typeRequest, out infHandler))
                return (ResponseMessage)infHandler.Invoke(this, new object[] { conn, msgRequest });

            throw new NoHandlerException($"No request handler for {typeRequest.FullName}");
        }

        [Processor(typeof(Core.Requests.ChannelsRequest))]
        private Core.Requests.ChannelsResponse HandleChannelsRequest(ClientConnection conn, Core.Requests.ChannelsRequest req)
        => new Core.Requests.ChannelsResponse() { 
            Channels = Database.Table<Records.ChannelRecord>()
                .Select(chan => chan.ToChannelInfo())
                .ToArray() 
        };

        [Processor(typeof(Core.Requests.MembersRequest))]
        private Core.Requests.MembersResponse HandleMembersRequest(ClientConnection conn, Core.Requests.MembersRequest req) 
        => new Core.Requests.MembersResponse() {
            Members = Database.Table<Records.MemberRecord>()
                .Select(memb => memb.ToMemberInfo())
                .ToArray()
        };

        [Processor(typeof(Core.Requests.MessagesBeforeRequest))]
        private Core.Requests.MessagesBeforeResponse HandleMessagesBeforeRequest(ClientConnection conn, Core.Requests.MessagesBeforeRequest req)
        => new Core.Requests.MessagesBeforeResponse() {
            Messages = Database.Table<Records.MessageRecord>()
                .Where(msg => msg.ChannelID == req.InChannel && msg.MessageIndex < req.BeforeIndex)
                .OrderByDescending(msg => -msg.MessageIndex)
                .Take(req.MaxCount)
                .Select(msg => msg.ToMessageData())
                .ToArray()
        };

        [Processor(typeof(Core.Requests.SingleMessageRequest))]
        private Core.Requests.SingleMessageResponse HandleSingleMessageRequest(ClientConnection conn, Core.Requests.SingleMessageRequest req)
        => new Core.Requests.SingleMessageResponse() {
            Message = Database.Table<Records.MessageRecord>()
                .Where(msg => msg.ChannelID == req.Channel && msg.MessageIndex == req.Index)
                .FirstOrDefault()
                ?.ToMessageData()
        };

        [Processor(typeof(Core.Requests.ServerInfoRequest))]
        private Core.Requests.ServerInfoResponse HandleServerInfoRequest(ClientConnection conn, Core.Requests.ServerInfoRequest req)
            => new Core.Requests.ServerInfoResponse() { Info = BasicInfo };

        [Processor(typeof(Core.Requests.UpdateProfileRequest))]
        private Core.Requests.GenericSuccessResponse HandleProfileUpdateRequest(ClientConnection conn, Core.Requests.UpdateProfileRequest req) {
            // make sure it's valid before putting it in
            Core.Structures.UserProfile profile = SignedStructSerializer.Deserialize<Core.Structures.UserProfile>
                (req.SignedData, req.Signature, conn.Connection.PartnerPublicKey);

            Records.MemberRecord memb = null;

            SQLite.SQLiteConnection db = Database;
            db.RunInTransaction(() => {
                memb = GetMemberRecord(conn.Connection.PartnerPublicKey);
                memb.SignedProfile = req.SignedData;
                memb.ProfileSignature = req.Signature;
                db.Update(memb);
            });

            BroadcastNotification(new Core.Notifications.MemberInfoChangedNotification() { Member = memb.ToMemberInfo() });
            return new Core.Requests.GenericSuccessResponse();
        }

        [Processor(typeof(Core.Requests.SetNicknameRequest))]
        private Core.Requests.GenericSuccessResponse HandleSetNicknameRequest(ClientConnection conn, Core.Requests.SetNicknameRequest req) {
            Records.MemberRecord memb = null;

            SQLite.SQLiteConnection db = Database;
            db.RunInTransaction(() => {
                memb = GetMemberRecord(conn.Connection.PartnerPublicKey);
                memb.Nickname = req.Nickname;
                db.Update(memb);
            });

            BroadcastNotification(new Core.Notifications.MemberInfoChangedNotification() { Member = memb.ToMemberInfo() });
            return new Core.Requests.GenericSuccessResponse();
        }

        public Records.MemberRecord TryGetMemberRecord(ReadOnlySpan<byte> spanPublicKey) {
            byte[] arrPublicKey = spanPublicKey.ToArray();
            return Database.Table<Records.MemberRecord>().Where(memb => memb.PublicKey.SequenceEqual(arrPublicKey)).FirstOrDefault();
        }

        public Records.MemberRecord GetMemberRecord(ReadOnlySpan<byte> spanPublicKey) {
            Records.MemberRecord memb = TryGetMemberRecord(spanPublicKey);

            if (memb is null)
                throw new RecordNotFoundException($"No member with public key {spanPublicKey.ToHexString()}");

            return memb;
        }

        [Processor(typeof(Core.Requests.PostMessageRequest))]
        private Core.Requests.MessagePostedResponse HandleMessagePostRequest(ClientConnection conn, Core.Requests.PostMessageRequest req) {
            Core.Structures.MessageSignedData data = SignedStructSerializer.Deserialize<Core.Structures.MessageSignedData>
                (req.SignedData, req.Signature, conn.Connection.PartnerPublicKey);

            Records.MessageRecord message = null;

            SQLite.SQLiteConnection db = Database;
            db.RunInTransaction(() => {
                Records.ChannelRecord chanPostIn = db.Table<Records.ChannelRecord>().Where(chan => chan.ID == req.ChannelID).First();

                message = new Records.MessageRecord() {
                    ChannelID = chanPostIn.ID,
                    DatePosted = DateTime.Now,
                    MessageIndex = chanPostIn.NextMessageID,
                    SenderPublicKey = conn.Connection.PartnerPublicKey.ToArray(),
                    Signature = req.Signature,
                    SignedData = req.SignedData
                };

                db.Insert(message);

                chanPostIn.NextMessageID++;
                db.Update(chanPostIn);
            });

            BroadcastNotification(new Core.Notifications.MessagePostedNotification() {
                ChannelID = req.ChannelID,
                Message = message.ToMessageData(),
                MessageIndex = message.MessageIndex
            });

            return new Core.Requests.MessagePostedResponse() { 
                ChannelID = req.ChannelID, 
                MessageIndex = message.MessageIndex
            };
        }
    }
}
