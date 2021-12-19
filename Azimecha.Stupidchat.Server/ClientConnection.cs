using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Protocol;
using System.Diagnostics;

namespace Azimecha.Stupidchat.Server {

    internal class ClientConnection : IDisposable, Core.ProtocolConnection.IErrorProcessor, Core.ProtocolConnection.IRequestProcessor {
        private Server _server;
        private Core.ProtocolConnection _conn;

        public ClientConnection(Server server, TcpClient client) {
            _conn = new Core.ProtocolConnection(client, true, server.PrivateKey);
            _conn.Start();
        }

        public Core.ProtocolConnection Connection => _conn;

        public void Dispose() {
            _server = null;
            Interlocked.Exchange(ref _conn, null)?.Dispose();
        }

        void Core.ProtocolConnection.IErrorProcessor.ProcessError(Exception ex) {
            Console.WriteLine($"Client connection error: {ex}");
        }

        ResponseMessage Core.ProtocolConnection.IRequestProcessor.ProcessRequest(RequestMessage msgRequest) 
            => _server.HandleRequest(this, msgRequest);
    }
}
