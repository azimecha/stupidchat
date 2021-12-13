using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Azimecha.Stupidchat.Core;

namespace Azimecha.Stupidchat.Client {
    public class ChatClient : IDisposable {
        private IList<ConnectionToServer> _lstConnections;

        public ChatClient() {
            _lstConnections = new List<ConnectionToServer>();
        }

        public void Dispose() {
            lock (_lstConnections) {
                foreach (ConnectionToServer conn in _lstConnections)
                    conn.Dispose();
                _lstConnections.Clear();
            }
        }

        internal void OnConnectionError(ConnectionToServer conn, Exception ex) {
            Debug.WriteLine($"Error from server {conn.ServerAddress}:{conn.ServerPort}: {ex}");
        }

        internal void OnNotification(ConnectionToServer conn, Core.Protocol.NotificationMessage msgNotif) {

        }
    }
}
