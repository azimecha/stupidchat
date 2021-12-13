using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Core.Protocol;

namespace Azimecha.Stupidchat.Client {
    internal class ConnectionToServer : ProtocolConnection.INotificationProcessor, ProtocolConnection.IErrorProcessor, IDisposable {
        private ProtocolConnection _conn;
        private ChatClient _cclient;

        public ConnectionToServer(string strAddress, int nPort, ChatClient cclient, ReadOnlySpan<byte> spanPrivateKey) {
            ServerAddress = strAddress;
            ServerPort = nPort;
            _cclient = cclient;

            TcpClient tclient = new TcpClient();
            try {
                tclient.Connect(strAddress, nPort);
                _conn = new ProtocolConnection(tclient, true, spanPrivateKey);
            } catch (Exception) {
                tclient.Dispose();
                throw;
            }

            _conn.NotificationProcessor = this;
            _conn.ErrorProcessor = this;
            _conn.Start();
        }

        public string ServerAddress { get; private set; }
        public int ServerPort { get; private set; }
        public ReadOnlySpan<byte> ServerPublicKey => _conn.PartnerPublicKey;
        public ProtocolConnection Connection => _conn;

        public void Dispose() {
            Interlocked.Exchange(ref _conn, null)?.Dispose();
            _cclient = null;
        }

        void ProtocolConnection.IErrorProcessor.ProcessError(Exception ex)
            => _cclient.OnConnectionError(this, ex);

        void ProtocolConnection.INotificationProcessor.ProcessNotification(NotificationMessage msgNotification)
            => _cclient.OnNotification(this, msgNotification);
    }
}
