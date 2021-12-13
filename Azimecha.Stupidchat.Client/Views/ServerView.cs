using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;
using Azimecha.Stupidchat.Client.Interfaces;

namespace Azimecha.Stupidchat.Client.Views {
    internal class ServerView : IServer {
        private ConnectionToServer _conn;

        internal ServerView(ConnectionToServer conn) {
            _conn = conn;
        }

        public string Address => _conn.ServerAddress;
        public int Port => _conn.ServerPort;
        public ReadOnlySpan<byte> PublicKey => _conn.ServerPublicKey;

        public ServerInformation GetInformation() 
            => _conn.Connection.PerformRequest<Core.Requests.ServerInfoResponse>(new Core.Requests.ServerInfoRequest()).Info;
    }
}
