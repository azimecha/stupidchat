using Azimecha.Stupidchat.Client.Interfaces;
using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client.Views {
    internal class MemberView : UserView, IMember {
        private IServer _server;

        public MemberView(ChatClient client, IServer server, ReadOnlySpan<byte> spanUserID) : base(client, spanUserID) { }
    }
}
