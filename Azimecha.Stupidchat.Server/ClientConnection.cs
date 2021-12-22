using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Protocol;
using System.Diagnostics;

namespace Azimecha.Stupidchat.Server {

    internal class ClientConnection : IDisposable, Core.ProtocolConnection.IErrorProcessor, Core.ProtocolConnection.IRequestProcessor,
        Core.IDisposalObserver<Core.ProtocolConnection>
    {
        private Server _server;
        private Core.ProtocolConnection _conn;
        private byte[] _arrPartnerPublicKey;

        public ClientConnection(Server server, TcpClient client) {
            _server = server;

            _conn = new Core.ProtocolConnection(client, true, server.PrivateKey);
            _conn.ErrorProcessor = this;
            _conn.RequestProcessor = this;
            _conn.DisposalObserver = this;
            _conn.Start();

            // make sure this survives disposal of the ProtocolConnection - we have to use it one last time afterwards
            _arrPartnerPublicKey = _conn.PartnerPublicKey.ToArray();
        }

        public Core.ProtocolConnection Connection => _conn;
        public ReadOnlySpan<byte> ClientPublicKey => _arrPartnerPublicKey;

        public void Dispose() {
            Interlocked.Exchange(ref _conn, null)?.Dispose();
            Interlocked.Exchange(ref _server, null)?.OnClientDisconnected(this);
        }

        void Core.IDisposalObserver<Core.ProtocolConnection>.OnObjectDisposed(Core.ProtocolConnection obj) {
            if (!(obj is null) && ReferenceEquals(obj, _conn))
                Dispose();
        }

        void Core.ProtocolConnection.IErrorProcessor.ProcessError(Exception ex) {
            Server.LogListener.LogMessage($"Client connection error: {ex}");
        }

        ResponseMessage Core.ProtocolConnection.IRequestProcessor.ProcessRequest(RequestMessage msgRequest) 
            => _server.HandleRequest(this, msgRequest);
    }
}
