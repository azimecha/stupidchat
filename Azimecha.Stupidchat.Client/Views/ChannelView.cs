using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Client.Interfaces;

namespace Azimecha.Stupidchat.Client.Views {
    internal class ChannelView : IChannel {
        private ChatClient _client;
        private IServer _server;
        private long _nChannelID;

        public ChannelView(ChatClient client, IServer server, long nChannelID) {
            _client = client;
            _server = server;
            _nChannelID = nChannelID;
        }
    }
}
