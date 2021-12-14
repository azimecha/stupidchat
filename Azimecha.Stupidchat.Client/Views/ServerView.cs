using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azimecha.Stupidchat.Core.Structures;
using Azimecha.Stupidchat.Client.Interfaces;

namespace Azimecha.Stupidchat.Client.Views {
    internal class ServerView : IServer {
        private ChatClient _client;
        private byte[] _arrServerID;

        internal ServerView(ChatClient client, ReadOnlySpan<byte> spanServerID) {
            _client = client;
            _arrServerID = spanServerID.ToArray();
        }

        internal ConnectionToServer Connection => _client.GetConnection(_arrServerID);

        public string Address => Connection.ServerAddress;
        public int Port => Connection.ServerPort;
        public ReadOnlySpan<byte> ID => _arrServerID;

        public ServerInfo GetInformation() 
            => Connection.Connection.PerformRequest<Core.Requests.ServerInfoResponse>(new Core.Requests.ServerInfoRequest()).Info;

        public IMember[] GetMembers()
            => Connection.Connection.PerformRequest<Core.Requests.MembersResponse>(new Core.Requests.MembersRequest()).MemberIDs
                .Select(arrUserID => new MemberView(_client, this, arrUserID)).ToArray();

        public IChannel[] GetChannels()
            => Connection.Connection.PerformRequest<Core.Requests.ChannelsResponse>(new Core.Requests.ChannelsRequest()).ChannelIDs
                .Select(nChannelID => new ChannelView(_client, this, nChannelID)).ToArray();
    }
}
