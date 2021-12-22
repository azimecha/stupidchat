﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Core.Structures;
using System.Threading;
using System.Collections;
using Azimecha.Stupidchat.Core.Requests;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

            userExisting?.UpdateProfile(profile);
            return userExisting ?? userNew;
        }

        private static string IDToString(ReadOnlySpan<byte> spanKey) => spanKey.ToHexString();

        public event Action<IMember> MemberJoined;
        public event Action<IMember> MemberInfoChanged;
        public event Action<IMember> MemberLeft;

        public event Action<IChannel> ChannelAdded;
        public event Action<IChannel> ChannelInfoChanged;
        public event Action<IChannel> ChannelRemoved;

        public event Action<IMessage> MessagePosted;
        public event Action<IMessage> MessageDeleted;

        private class Server : IServer, IDisposable, ProtocolConnection.IErrorProcessor, ProtocolConnection.INotificationProcessor {
            private ChatClient _cclient;
            private ProtocolConnection _conn;
            private IDictionary<string, Member> _dicMembers;
            private IDictionary<long, Channel> _dicChannels;
            private ServerInfo _info;
            private object _objInfoMutex;

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

                _info = _conn.PerformRequest<ServerInfoResponse>(new ServerInfoRequest()).Info;
                _objInfoMutex = new object();

                _dicMembers = new Dictionary<string, Member>();
                foreach (MemberInfo infoMember in _conn.PerformRequest<MembersResponse>(new MembersRequest()).Members)
                    _dicMembers.Add(IDToString(infoMember.PublicKey), new Member(this, infoMember));

                _dicChannels = new Dictionary<long, Channel>();
                foreach (ChannelInfo infoChannel in _conn.PerformRequest<ChannelsResponse>(new ChannelsRequest()).Channels)
                    _dicChannels.Add(infoChannel.ID, new Channel(this, infoChannel));

                _dicNotifProcessors = ProcessorAttribute.BindProcessorsList<Server, Core.Protocol.NotificationMessage>
                    (this, _dicNotifProcessorMethods);
            }

            public string Address { get; private set; }
            public int Port { get; private set; }
            public ReadOnlySpan<byte> ID => _conn.PartnerPublicKey;

            public ServerInfo Info {
                get {
                    lock (_objInfoMutex)
                        return _info;
                }
            }

            public IEnumerable<IMember> Members => _dicMembers.Values;
            public IEnumerable<IChannel> Channels => _dicChannels.Values;

            internal ChatClient AssociatedChatClient => _cclient;
            internal ProtocolConnection Connection => _conn;

            public void Dispose() {
                Interlocked.Exchange(ref _conn, null)?.Dispose();
                _dicMembers = null;
            }

            void ProtocolConnection.IErrorProcessor.ProcessError(Exception ex) {
                Debug.WriteLine($"Error on connection to {Address}:{Port}: \n{ex}");
            }

            private static readonly IDictionary<Type, System.Reflection.MethodInfo> _dicNotifProcessorMethods
                = ProcessorAttribute.BuildProcessorsList<Server, Core.Protocol.NotificationMessage>();

            private readonly IDictionary<Type, Action<Core.Protocol.NotificationMessage>> _dicNotifProcessors;

            void ProtocolConnection.INotificationProcessor.ProcessNotification(Core.Protocol.NotificationMessage msgNotification) {
                Action<Core.Protocol.NotificationMessage> procProcessor;
                if (_dicNotifProcessors.TryGetValue(msgNotification.GetType(), out procProcessor))
                    procProcessor(msgNotification);
            }

            [Processor(typeof(Core.Notifications.MemberJoinedNotification))]
            private void OnMemberJoined(Core.Notifications.MemberJoinedNotification notif) {
                Member membNew = new Member(this, notif.Member);

                lock (_dicMembers)
                    _dicMembers.Add(IDToString(notif.Member.PublicKey), membNew);

                _cclient.MemberJoined?.Invoke(membNew);
            }

            [Processor(typeof(Core.Notifications.MemberInfoChangedNotification))]
            private void OnMemberInfoChanged(Core.Notifications.MemberInfoChangedNotification notif) {
                Member membChanged = null;

                lock (_dicMembers) {
                    if (_dicMembers.TryGetValue(IDToString(notif.Member.PublicKey), out membChanged))
                        membChanged.UpdateInfo(notif.Member);
                }

                if (!(membChanged is null))
                    _cclient.MemberInfoChanged?.Invoke(membChanged);
            }

            [Processor(typeof(Core.Notifications.MemberLeftNotification))]
            private void OnMemberLeft(Core.Notifications.MemberLeftNotification notif) {
                Member membThatLeft = null;
                string strID = IDToString(notif.MemberPublicKey);

                lock (_dicMembers) {
                    if (_dicMembers.TryGetValue(strID, out membThatLeft))
                        _dicMembers.Remove(strID);
                }

                if (!(membThatLeft is null))
                    _cclient.MemberLeft?.Invoke(membThatLeft);
            }

            [Processor(typeof(Core.Notifications.ChannelAddedNotification))]
            private void OnChannelAdded(Core.Notifications.ChannelAddedNotification notif) {
                Channel chanNew = new Channel(this, notif.Channel);

                lock (_dicChannels)
                    _dicChannels.Add(notif.Channel.ID, chanNew);

                _cclient.ChannelAdded?.Invoke(chanNew);
            }

            [Processor(typeof(Core.Notifications.ChannelInfoChangedNotification))]
            private void OnChannelInfoChanged(Core.Notifications.ChannelInfoChangedNotification notif) {
                Channel chanChanged = null;

                lock (_dicChannels) {
                    if (_dicChannels.TryGetValue(notif.Channel.ID, out chanChanged))
                        _dicChannels[notif.Channel.ID].UpdateInfo(notif.Channel);
                }

                if (!(chanChanged is null))
                    _cclient.ChannelInfoChanged?.Invoke(chanChanged);
            }

            [Processor(typeof(Core.Notifications.ChannelRemovedNotification))]
            private void OnChannelRemoved(Core.Notifications.ChannelRemovedNotification notif) {
                Channel chanDeleted = null;

                lock (_dicChannels) {
                    if (_dicChannels.TryGetValue(notif.ChannelID, out chanDeleted))
                        _dicChannels.Remove(notif.ChannelID);
                }

                if (!(chanDeleted is null))
                    _cclient.ChannelRemoved?.Invoke(chanDeleted);
            }

            [Processor(typeof(Core.Notifications.MessagePostedNotification))]
            private void OnMessagePosted(Core.Notifications.MessagePostedNotification notif) {
                Channel chan = (Channel)FindChannel(notif.ChannelID);
                IMessage msg = CreateMessageObject(chan, notif.MessageIndex, notif.Message);
                chan.AddMessage(msg);
            }

            [Processor(typeof(Core.Notifications.MessageDeletedNotification))]
            private void OnMessageDeleted(Core.Notifications.MessageDeletedNotification notif) {
                Channel chan = (Channel)FindChannel(notif.ChannelID);
                chan.RemoveMessage(notif.MessageIndex);
            }

            public IChannel FindChannel(long nChannelID) {
                IChannel chan = TryFindChannel(nChannelID);

                if (chan is null)
                    throw new KeyNotFoundException($"No channel with ID {nChannelID}");

                return chan;
            }

            public IChannel TryFindChannel(long nChannelID) {
                Channel chan = null;

                lock (_dicChannels)
                    _dicChannels.TryGetValue(nChannelID, out chan);

                return chan;
            }

            public IMember FindMember(ReadOnlySpan<byte> spanPublicKey) {
                IMember memb = TryFindMember(spanPublicKey);

                if (memb is null)
                    throw new KeyNotFoundException($"No member with public key {IDToString(spanPublicKey)}");

                return memb;
            }

            public IMember TryFindMember(ReadOnlySpan<byte> spanPublicKey) {
                Member memb = null;
                string strID = IDToString(spanPublicKey);

                lock (_dicMembers)
                    _dicMembers.TryGetValue(strID, out memb);

                return memb;
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
                UserProfile profile = SignedStructSerializer.Deserialize<UserProfile>(info.Profile, info.ProfileSignature, info.PublicKey);

                lock (_objInfoMutex) {
                    _user.UpdateProfile(profile);
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
            private object _objChannelMutex, _objDownloadMessagesMutex;
            private ConcurrentDictionary<long, IMessage> _dicMessages;
            private long _nHighestMessageIndex;

            internal Channel(Server server, ChannelInfo info) {
                _server = server;
                _info = info;
                _objChannelMutex = new object();
                _objDownloadMessagesMutex = new object();
                _dicMessages = new ConcurrentDictionary<long, IMessage>();
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

            internal IMessage TryGetMessage(long nIndex) {
                IMessage msg;

                // check to see if it's already downloaded
                if (_dicMessages.TryGetValue(nIndex, out msg))
                    return msg;

                // acquire the lock.  not needed for the dictionary -
                // just prevents us from downloading twice concurrently
                lock (_objDownloadMessagesMutex) {
                    // check to see if the last locker got it
                    if (_dicMessages.TryGetValue(nIndex, out msg))
                        return msg;

                    // download it
                    SingleMessageResponse resp = _server.Connection.PerformRequest<SingleMessageResponse>(new SingleMessageRequest() { Index = nIndex });
                    msg = CreateMessageObject(this, nIndex, resp.Message);

                    // try to put it in
                    if (_dicMessages.TryAdd(nIndex, msg)) {
                        // update highest message index if needed
                        Utils.InterlockedExchangeIfGreater(ref _nHighestMessageIndex, nIndex, nIndex);
                        return msg;
                    }
                        
                    // lock will release on exiting this block
                }

                // someone else put it in? (shouldn't happen)
                if (_dicMessages.TryGetValue(nIndex, out msg))
                    return msg;

                // we really shouldn't be here
                throw new InvalidOperationException($"Message {nIndex} is both present and not present in the dictionary (could not add or get)");
            }

            public IEnumerable<IMessage> Messages => new MessageEnumerable(this);

            private class MessageEnumerable : IEnumerable<IMessage> {
                private Channel _chan;
                public MessageEnumerable(Channel chan) { _chan = chan; }

                public IEnumerator<IMessage> GetEnumerator() => new MessageEnumerator(_chan);
                IEnumerator IEnumerable.GetEnumerator() => new MessageEnumerator(_chan);
            }

            // note that messages are enumerated backwards in time, starting from the newest message
            private class MessageEnumerator : IEnumerator<IMessage> {
                private Channel _chan;
                private long _nCurIndex;
                private IMessage _msgCurrent;

                public MessageEnumerator(Channel chan) { _chan = chan; }

                public IMessage Current => throw new NotImplementedException();
                object IEnumerator.Current => throw new NotImplementedException();

                public void Dispose() {
                    _chan = null;
                    _nCurIndex = -1;
                    _msgCurrent = null;
                }

                public bool MoveNext() {
                    _nCurIndex = _nCurIndex < 0 ? _chan._nHighestMessageIndex : _nCurIndex - 1;
                    _msgCurrent = _chan.TryGetMessage(_nCurIndex);
                    return !(_msgCurrent is null);
                }

                public void Reset() {
                    _nCurIndex = -1;
                    _msgCurrent = null;
                }
            }

            public void PostMessage(MessageSignedData msg) {
                throw new NotImplementedException();
            }

            internal void AddMessage(IMessage msg) {
                _dicMessages.TryAdd(msg.IndexInChannel, msg);
            }

            internal void RemoveMessage(long nIndex) {
                IMessage msg;
                _dicMessages.TryRemove(nIndex, out msg);
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
                lock (_objProfileMutex) {
                    if (profile.UpdateTime < _profile.UpdateTime)
                        _profile = profile;
                }
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

        private static IMessage CreateMessageObject(Channel chan, long nIndex, MessageData? data)
            => data.HasValue ? (IMessage)new Message(chan, nIndex, data.Value) : (IMessage)new MissingMessage(chan, nIndex);

        private class Message : IMessage {
            private Channel _chan;
            private long _nIndex;
            private MessageData _dataUnsigned;
            private MessageSignedData _dataSigned;
            private Member _membSender;
            private DateTime _dateSent;

            public Message(Channel chan, long nIndex, MessageData data) {
                _chan = chan;
                _nIndex = nIndex;
                _dataUnsigned = data;

                _dataSigned = SignedStructSerializer.Deserialize<MessageSignedData>(data.SignedData, data.Signature, data.SenderPublicSigningKey);
                _membSender = (Member)chan.Server.Members.Where(m => m.User.PublicKey.SequenceEqual(data.SenderPublicSigningKey)).FirstOrDefault();

                DateTime dateSentAccordingToUser = new DateTime(_dataSigned.SendTime);
                DateTime datePostedAccordingToServer = new DateTime(_dataUnsigned.PostedTime);
                if ((dateSentAccordingToUser > datePostedAccordingToServer) || (datePostedAccordingToServer - dateSentAccordingToUser).TotalMinutes > 5.0)
                    throw new InvalidDateException($"User reports message sent at {dateSentAccordingToUser}, "
                        + $"but server reports it was posted at {datePostedAccordingToServer}");

                _dateSent = dateSentAccordingToUser;
            }

            public IChannel Channel => _chan;
            public IMember Sender => _membSender;
            public bool Deleted => false;
            public MessageData Data => _dataUnsigned;
            public MessageSignedData SignedData => _dataSigned;
            public DateTime SentAt => _dateSent;
            public long IndexInChannel => _nIndex;
        }

        private class MissingMessage : IMessage {
            private Channel _chan;
            private long _nIndex;

            public MissingMessage(Channel chan, long nIndex) {
                _chan = chan;
                _nIndex = nIndex;
            }

            public IChannel Channel => _chan;
            public IMember Sender => null;
            public bool Deleted => true;
            public MessageData Data => new MessageData();
            public MessageSignedData SignedData => new MessageSignedData();
            public DateTime SentAt => DateTime.MinValue;
            public long IndexInChannel => _nIndex;
        }
    }
}
