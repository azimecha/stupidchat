using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Core.Structures;
using System.Threading;

namespace Azimecha.Stupidchat.Client {
    public class ChatClient : IDisposable {
        private IDictionary<string, Server> _dicServers;
        private IDictionary<string, User> _dicUsers;
        private byte[] _arrPrivateKey;
        private byte[] _arrPublicKey;

        public ChatClient(ReadOnlySpan<byte> spanPrivateKey) {
            _dicServers = new Dictionary<string, Server>();
            _dicUsers = new Dictionary<string, User>();
            _arrPrivateKey = spanPrivateKey.ToArray();
            _arrPublicKey = NetworkConnection.CreateSigningAlgorithmInstance().GetPublicKey(_arrPrivateKey);
        }

        public IEnumerable<IServer> Servers {
            get {
                lock (_dicServers)
                    return _dicServers.Values.ToArray();
            }
        }

        public void Dispose() {
            lock (_dicServers) {
                foreach (Server server in _dicServers.Values)
                    server.Dispose();
                _dicServers.Clear();
            }

            lock (_dicUsers)
                _dicUsers.Clear();
        }

        public IServer ConnectToServer(string strAddress, int nPort) {
            Server server = new Server(this, strAddress, nPort, _arrPrivateKey);

            try {
                string strID = IDToString(server.ID);

                lock (_dicServers) {
                    if (_dicServers.ContainsKey(strID))
                        throw new InvalidOperationException($"Already connected to server with ID {strID}");

                    _dicServers.Add(strID, server);
                }
            } catch (Exception) {
                server.Dispose();
                throw;
            }

            return server;
        }

        private User MemberToUser(MemberInfo info) {
            string strID = IDToString(info.PublicKey);
            UserProfile profile = SignedStructSerializer.Deserialize<UserProfile>(info.Profile, info.ProfileSignature, info.PublicKey);
            User userExisting = null;
            User userNew = new User(info.PublicKey, profile);

            lock (_dicUsers) {
                if (_dicUsers.ContainsKey(strID))
                    userExisting = _dicUsers[strID];
                else
                    _dicUsers.Add(strID, userNew);
            }

            if (!(userExisting is null)) {
                if (userExisting.Profile.UpdateTime < profile.UpdateTime)
                    userExisting.UpdateProfile(profile);
            }

            return userExisting ?? userNew;
        }

        private string IDToString(ReadOnlySpan<byte> spanKey) => spanKey.ToHexString();

        private class Server : IServer, IDisposable, ProtocolConnection.IErrorProcessor, ProtocolConnection.INotificationProcessor {
            private ChatClient _cclient;
            private ProtocolConnection _conn;
            private IDictionary<long, Member> _dicMembers;

            internal Server(ChatClient cclient, string strAddress, int nPort, ReadOnlySpan<byte> spanPrivateKey) {
                _cclient = cclient;

                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(strAddress, nPort);

                try {
                    _conn = new ProtocolConnection(client, true, spanPrivateKey);
                    _conn.ErrorProcessor = this;
                    _conn.NotificationProcessor = this;
                    _conn.Start();
                } catch (Exception) {
                    client.Dispose();
                    throw;
                }

                _dicMembers = new Dictionary<long, Member>();
            }

            public string Address { get; private set; }
            public int Port { get; private set; }
            public ReadOnlySpan<byte> ID => _conn.PartnerPublicKey;
            public ServerInfo Info { get; private set; }
            public IEnumerable<IMember> Members => _dicMembers.Values;
            public IEnumerable<IChannel> Channels => throw new NotImplementedException();

            internal ChatClient AssociatedChatClient => _cclient;

            public void Dispose() {
                Interlocked.Exchange(ref _conn, null)?.Dispose();
                _dicMembers = null;
            }

            void ProtocolConnection.IErrorProcessor.ProcessError(Exception ex) {
                throw new NotImplementedException();
            }

            void ProtocolConnection.INotificationProcessor.ProcessNotification(Core.Protocol.NotificationMessage msgNotification) {
                throw new NotImplementedException();
            }
        }

        private class Member : IMember {
            private Server _server;
            private MemberInfo _info;
            private User _user;
            private object _objInfoMutex;

            internal Member(Server server, MemberInfo info) {
                _server = server;
                _user = server.AssociatedChatClient.MemberToUser(info);
                _info = info;
                _objInfoMutex = new object();
            }

            internal void UpdateInfo(MemberInfo info) {
                lock (_objInfoMutex) {
                    _user = _server.AssociatedChatClient.MemberToUser(info);
                    _info = info;
                }
            }

            public IServer Server => _server;

            public MemberInfo Info {
                get {
                    lock (_objInfoMutex)
                        return _info;
                }
            }

            public IUser User => _user;
        }

        private class Channel : IChannel {
            private Server _server;
            private ChannelInfo _info;
            private object _objChannelMutex;

            internal Channel(Server server, ChannelInfo info) {
                _server = server;
                _info = info;
                _objChannelMutex = new object();
            }

            internal void UpdateInfo(ChannelInfo info) {
                lock (_objChannelMutex)
                    _info = info;
            }

            public IServer Server => _server;

            public ChannelInfo Info {
                get {
                    lock (_objChannelMutex)
                        return _info;
                }
            }

            // TODO
            public IEnumerable<IMessage> Messages { get; }

            public void PostMessage(MessageSignedData msg) {
                throw new NotImplementedException();
            }
        }

        private class User : IUser {
            private IList<Member> _lstMemberships;
            private byte[] _arrPublicKey;
            private UserProfile _profile;
            private object _objProfileMutex;

            internal User(ReadOnlySpan<byte> spanPublicKey, UserProfile profile) {
                _lstMemberships = new List<Member>();
                _arrPublicKey = spanPublicKey.ToArray();
                _profile = profile;
                _objProfileMutex = new object();
            }

            internal void AddMembership(Member memb) {
                lock (_lstMemberships)
                    _lstMemberships.Add(memb);
            }

            internal void RemoveMembership(Member memb) {
                lock (_lstMemberships)
                    _lstMemberships.Remove(memb);
            }

            internal void UpdateProfile(UserProfile profile) {
                lock (_objProfileMutex)
                    _profile = profile;
            }

            public IEnumerable<IMember> Memberships {
                get {
                    lock (_lstMemberships)
                        return _lstMemberships.ToArray();
                }
            }

            public ReadOnlySpan<byte> PublicKey => _arrPublicKey;

            public UserProfile Profile {
                get {
                    lock (_objProfileMutex)
                        return _profile;
                }
            }
        }
    }
}
