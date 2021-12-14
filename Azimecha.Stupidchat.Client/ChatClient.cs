using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Azimecha.Stupidchat.Core;

namespace Azimecha.Stupidchat.Client {
    public class ChatClient : IDisposable {
        private IList<ConnectionToServer> _lstConnections;

        public ChatClient() {
            _lstConnections = new List<ConnectionToServer>();
        }

        public IEnumerable<Interfaces.IServer> Servers
            => _lstConnections.Select(conn => new Views.ServerView(this, conn.ServerPublicKey));

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

        internal ConnectionToServer GetConnection(ReadOnlySpan<byte> spanServerID) {
            foreach (ConnectionToServer conn in _lstConnections)
                if (conn.ServerPublicKey == spanServerID)
                    return conn;

            throw new KeyNotFoundException($"No server with public key " + spanServerID.ToHexString());
        }


    }
}
