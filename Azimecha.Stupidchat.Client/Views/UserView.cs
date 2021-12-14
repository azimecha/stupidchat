using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Client.Interfaces;

namespace Azimecha.Stupidchat.Client.Views {
    internal class UserView : IUser {
        private ChatClient _client;
        private byte[] _arrUserID;

        public UserView(ChatClient client, ReadOnlySpan<byte> spanUserID) {
            _client = client;
            _arrUserID = spanUserID.ToArray();
        }

        public ReadOnlySpan<byte> ID => _arrUserID;
    }
}
