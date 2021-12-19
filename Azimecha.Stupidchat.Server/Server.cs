using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Protocol;
using System.Diagnostics;
using System.Net;
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
        private ConcurrentDictionary<Thread, SQLite.SQLiteConnection> _dicDBConnections;

        public Server(ReadOnlySpan<byte> spanPrivateKey, IPEndPoint endpointListenOn, Core.Structures.ServerInfo info, string strDatabasePath) {
            _dicConnections = new ConcurrentDictionary<string, ClientConnection>();
            _listener = new TcpListener(endpointListenOn);
            _ctsDisposing = new CancellationTokenSource();
            _info = info;

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

        internal SQLite.SQLiteConnection Database {
            get {

            }
        }

        private void InitDatabase() {
            _db.CreateTable<Records.ChannelRecord>();
            _db.CreateTable<Records.MemberRecord>();
            _db.CreateTable<Records.MessageRecord>();
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
            _db.b
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
    }
}
