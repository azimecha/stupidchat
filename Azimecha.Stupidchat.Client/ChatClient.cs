using System;
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
        private UserProfile _profileMe;
        private object _objMyProfileMutex;

        public ChatClient(ReadOnlySpan<byte> spanPrivateKey) {
            _dicServers = new Dictionary<string, Server>();
            _dicUsers = new Dictionary<string, User>();
            _arrPrivateKey = spanPrivateKey.ToArray();
            _arrPublicKey = NetworkConnection.CreateSigningAlgorithmInstance().GetPublicKey(_arrPrivateKey);
            _objMyProfileMutex = new object();
        }

        public IEnumerable<IServer> Servers {
            get {
                lock (_dicServers)
                    return _dicServers.Values.ToArray();
            }
        }

        public UserProfile MyProfile {
            get {
                lock (_objMyProfileMutex)
                    return _profileMe;
            }
            set {
                lock (_objMyProfileMutex)
                    _profileMe = value;

                SignedStructSerializer.SignedData data = SignedStructSerializer.Serialize(_profileMe,
                    _arrPublicKey, _arrPrivateKey);

                Servers.AsParallel().ForAll(server => ((Server)server).Connection.PerformRequest(
                    new UpdateProfileRequest() { SignedData = data.Data, Signature = data.Signature }));
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

            ConnectedToServer?.Invoke(server);

            SignedStructSerializer.SignedData signedProfile = SignedStructSerializer.Serialize(MyProfile, _arrPublicKey, _arrPrivateKey);
            server.Connection.PerformRequest(new UpdateProfileRequest() {
                Signature = signedProfile.Signature,
                SignedData = signedProfile.Data
            });

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

        private void OnDisconnected(Server server) {
            string strID = IDToString(server.ID);
            bool bDidRemove = false;

            lock (_dicServers) {
                if (_dicServers.ContainsKey(strID)) {
                    _dicServers.Remove(strID);
                    bDidRemove = true;
                }
            }

            if (bDidRemove)
                DisconnectedFromServer?.Invoke(server);
        }

        public event Action<IServer> ConnectedToServer;
        public event Action<IServer> DisconnectedFromServer;

        public event Action<IMember> MemberJoined;
        public event Action<IMember> MemberInfoChanged;
        public event Action<IMember> MemberLeft;

        public event Action<IChannel> ChannelAdded;
        public event Action<IChannel> ChannelInfoChanged;
        public event Action<IChannel> ChannelRemoved;

        public event Action<IMessage> MessagePosted;
        public event Action<IMessage> MessageDeleted;

        private class Server : IServer, IDisposable, ProtocolConnection.IErrorProcessor, ProtocolConnection.INotificationProcessor, IDisposalObserver<ProtocolConnection> {
            private ChatClient _cclient;
            private ProtocolConnection _conn;
            private IDictionary<string, Member> _dicMembers;
            private IDictionary<long, Channel> _dicChannels;
            private ServerInfo _info;
            private object _objInfoMutex;
            private byte[] _arrServerPublicKey;
            private Lazy<TemporaryFile> _tfIcon;

            internal Server(ChatClient cclient, string strAddress, int nPort, ReadOnlySpan<byte> spanPrivateKey) {
                _cclient = cclient;
                Address = strAddress;
                Port = nPort;

                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(strAddress, nPort);

                _dicMembers = new Dictionary<string, Member>();
                _dicChannels = new Dictionary<long, Channel>();
                _objInfoMutex = new object();

                _dicNotifProcessors = ProcessorAttribute.BindProcessorsList<Server, Core.Protocol.NotificationMessage>
                    (this, _dicNotifProcessorMethods);

                try {
                    _conn = new ProtocolConnection(client, true, spanPrivateKey);
                    _conn.ErrorProcessor = this;
                    _conn.DisposalObserver = this;
                    _conn.Start();
                } catch (Exception) {
                    client.Dispose();
                    throw;
                }

                _tfIcon = new Lazy<TemporaryFile>(DownloadIcon, LazyThreadSafetyMode.ExecutionAndPublication);

                _arrServerPublicKey = _conn.PartnerPublicKey.ToArray();
                _info = _conn.PerformRequest<ServerInfoResponse>(new ServerInfoRequest()).Info;

                SignedStructSerializer.SignedData dataProfile = SignedStructSerializer.Serialize(_cclient._profileMe,
                    _cclient._arrPublicKey, _cclient._arrPrivateKey);
                _conn.PerformRequest(new UpdateProfileRequest() { SignedData = dataProfile.Data, Signature = dataProfile.Signature });

                foreach (MemberInfo infoMember in _conn.PerformRequest<MembersResponse>(new MembersRequest()).Members)
                    _dicMembers.Add(IDToString(infoMember.PublicKey), new Member(this, infoMember));

                foreach (ChannelInfo infoChannel in _conn.PerformRequest<ChannelsResponse>(new ChannelsRequest()).Channels)
                    _dicChannels.Add(infoChannel.ID, Channel.Create(this, infoChannel));

                _conn.NotificationProcessor = this;
            }

            public string Address { get; private set; }
            public int Port { get; private set; }
            public ReadOnlySpan<byte> ID => _arrServerPublicKey;

            public ServerInfo Info {
                get {
                    lock (_objInfoMutex)
                        return _info;
                }
            }

            public IEnumerable<IMember> Members => _dicMembers.Values;
            public IEnumerable<IChannel> Channels => _dicChannels.Values;

            public IMember Me => FindMember(_cclient._arrPublicKey);

            public void Disconnect() {
                Dispose();
            }

            public void SetNickname(string strNickname) {
                Connection.PerformRequest(new SetNicknameRequest() { Nickname = strNickname });
            }

            public void SetPresence(OnlineStatus status, OnlineDevice device) {
                Connection.PerformRequest(new UpdatePresenceRequest() { Status = status, Device = device });
            }

            internal ChatClient AssociatedChatClient => _cclient;
            internal ProtocolConnection Connection => _conn;

            public void Dispose() {
                Interlocked.Exchange(ref _conn, null)?.Dispose();
                _dicMembers = null;
                _dicChannels = null;

                MemberJoined = null;
                MemberLeft = null;

                ChannelAdded = null;
                ChannelRemoved = null;
            }

            void ProtocolConnection.IErrorProcessor.ProcessError(Exception ex) {
                Debug.WriteLine($"Error on connection to {Address}:{Port}: \n{ex}");
                ErrorOccurred?.Invoke(this, ex);
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
                MemberJoined?.Invoke(membNew);
            }

            [Processor(typeof(Core.Notifications.MemberInfoChangedNotification))]
            private void OnMemberInfoChanged(Core.Notifications.MemberInfoChangedNotification notif) {
                Member membChanged = null;

                lock (_dicMembers) {
                    if (_dicMembers.TryGetValue(IDToString(notif.Member.PublicKey), out membChanged))
                        membChanged.UpdateInfo(notif.Member, notif.ProfileChanged);
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

                if (!(membThatLeft is null)) {
                    _cclient.MemberLeft?.Invoke(membThatLeft);
                    MemberLeft?.Invoke(membThatLeft);
                }
            }

            [Processor(typeof(Core.Notifications.ChannelAddedNotification))]
            private void OnChannelAdded(Core.Notifications.ChannelAddedNotification notif) {
                Channel chanNew = Channel.Create(this, notif.Channel);

                lock (_dicChannels)
                    _dicChannels.Add(notif.Channel.ID, chanNew);

                _cclient.ChannelAdded?.Invoke(chanNew);
                ChannelAdded?.Invoke(chanNew);
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

                if (!(chanDeleted is null)) {
                    _cclient.ChannelRemoved?.Invoke(chanDeleted);
                    ChannelRemoved?.Invoke(chanDeleted);
                }
            }

            [Processor(typeof(Core.Notifications.MessagePostedNotification))]
            private void OnMessagePosted(Core.Notifications.MessagePostedNotification notif) {
                Channel chan = (Channel)FindChannel(notif.ChannelID);
                IMessage msg = CreateMessageObject(chan, notif.MessageIndex, notif.Message);
                chan.AcceptNewMessage(msg);
                _cclient.MessagePosted?.Invoke(msg);
            }

            [Processor(typeof(Core.Notifications.MessageDeletedNotification))]
            private void OnMessageDeleted(Core.Notifications.MessageDeletedNotification notif) {
                Channel chan = (Channel)FindChannel(notif.ChannelID);

                IMessage msg = chan.TryGetMessage(notif.MessageIndex, false);
                chan.RemoveMessage(notif.MessageIndex);

                if (msg is Message msgNonDeleted)
                    msgNonDeleted.OnDeleted();

                _cclient.MessageDeleted?.Invoke(msg);
            }

            [Processor(typeof(Core.Notifications.VCParticipantEnteredNotification))]
            private void OnVCParticipantEntered(Core.Notifications.VCParticipantEnteredNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnParticipantEntered(FindMember(notif.ParticipantPublicKey));

            [Processor(typeof(Core.Notifications.VCParticipantExitedNotification))]
            private void OnVCParticipantExited(Core.Notifications.VCParticipantExitedNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnParticipantExited(FindMember(notif.ParticipantPublicKey));

            [Processor(typeof(Core.Notifications.VCTransmitStartedNotification))]
            private void OnVCTransmitStarted(Core.Notifications.VCTransmitStartedNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnTransmitStarted(FindMember(notif.ParticipantPublicKey), notif.Subchannels);

            [Processor(typeof(Core.Notifications.VCTransmitStoppedNotification))]
            private void OnVCTransmitStopped(Core.Notifications.VCTransmitStoppedNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnTransmitStopped(FindMember(notif.ParticipantPublicKey), notif.Subchannels);

            [Processor(typeof(Core.Notifications.VCReceiveStartedNotification))]
            private void OnVCReceiveStarted(Core.Notifications.VCReceiveStartedNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnReceiveStarted(FindMember(notif.ParticipantPublicKey), notif.Subchannels);

            [Processor(typeof(Core.Notifications.VCReceiveStoppedNotification))]
            private void OnVCReceiveStopped(Core.Notifications.VCReceiveStoppedNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnReceiveStopped(FindMember(notif.ParticipantPublicKey), notif.Subchannels);

            [Processor(typeof(Core.Notifications.VCDataBlockNotification))]
            private void OnVCDataBlock(Core.Notifications.VCDataBlockNotification notif)
                => ((VoiceChannel)FindChannel(notif.ChannelID)).OnDataReceived(FindMember(notif.SenderPublicKey), notif.Subchannel, notif.Data);

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

            void IDisposalObserver<ProtocolConnection>.OnObjectDisposed(ProtocolConnection obj) {
                _cclient?.OnDisconnected(this);
                Disconnected?.Invoke(this);
                Dispose();
            }

            private TemporaryFile DownloadIcon()
                => DownloadFile(Info.ImageURL);

            public System.IO.Stream OpenIcon()
                => (_tfIcon.Value is null) ? null : System.IO.File.Open(_tfIcon.Value.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            public event Action<IMember> MemberJoined;
            public event Action<IMember> MemberLeft;

            public event Action<IChannel> ChannelAdded;
            public event Action<IChannel> ChannelRemoved;

            public event Action<IServer, Exception> ErrorOccurred;
            public event Action<IServer> Disconnected;
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

                _user?.EnsureMembershipExists(this);
            }

            internal void UpdateInfo(MemberInfo info, bool bUpdateProfile) {
                UserProfile? profile = bUpdateProfile
                    ? (UserProfile?)SignedStructSerializer.Deserialize<UserProfile>(info.Profile, info.ProfileSignature, info.PublicKey) 
                    : null;

                lock (_objInfoMutex) {
                    if (bUpdateProfile)
                        _user.UpdateProfile(profile.Value);

                    _info = info;
                }

                _user.OnStatusMaybeChanged();

                Changed?.Invoke(this);
            }

            public IServer Server => _server;

            public MemberInfo Info {
                get {
                    lock (_objInfoMutex)
                        return _info;
                }
            }

            public IUser User => _user;

            public string DisplayName {
                get {
                    MemberInfo info = Info;
                    return (info.Nickname?.Length ?? 0) > 0 ? info.Nickname : User.DisplayName;
                }
            }

            public event Action<IMember> Changed;
        }

        private class Channel : IChannel {
            private Server _server;
            private ChannelInfo _info;
            private object _objChannelMutex, _objDownloadMessagesMutex;
            private ConcurrentDictionary<long, IMessage> _dicMessages;
            private long _nHighestMessageIndex;

            public static Channel Create(Server server, ChannelInfo info)
                => (info.Type == ChannelType.Voice) ? new VoiceChannel(server, info) : new Channel(server, info);

            protected Channel(Server server, ChannelInfo info) {
                _server = server;
                _info = info;
                _objChannelMutex = new object();
                _objDownloadMessagesMutex = new object();
                _dicMessages = new ConcurrentDictionary<long, IMessage>();
                _nHighestMessageIndex = -1;

                // try to get the last message posted
                MessagesBeforeResponse resp = _server.Connection.PerformRequest<MessagesBeforeResponse>(new MessagesBeforeRequest() {
                    BeforeIndex = long.MaxValue,
                    InChannel = info.ID,
                    MaxCount = 1
                });

                if (resp.Messages.Length > 0) {
                    MessageData data = resp.Messages[0];
                    _nHighestMessageIndex = data.ID;
                    _dicMessages.TryAdd(_nHighestMessageIndex, new Message(this, data.ID, data));
                }
            }

            internal void UpdateInfo(ChannelInfo info) {
                lock (_objChannelMutex)
                    _info = info;

                InfoChanged?.Invoke(this);
            }

            public IServer Server => _server;

            public ChannelInfo Info {
                get {
                    lock (_objChannelMutex)
                        return _info;
                }
            }

            internal IMessage TryGetMessage(long nIndex, bool bDownloadIfNotPresent) {
                IMessage msg;

                // check to see if it's already downloaded or doesn't exist
                if (_dicMessages.TryGetValue(nIndex, out msg))
                    return msg;
                else if (!bDownloadIfNotPresent)
                    return null;
                else if ((nIndex < 0) || (nIndex > _nHighestMessageIndex))
                    return null;

                // acquire the lock.  not needed for the dictionary -
                // just prevents us from downloading twice concurrently
                lock (_objDownloadMessagesMutex) {
                    // check to see if the last locker got it
                    if (_dicMessages.TryGetValue(nIndex, out msg))
                        return msg;

                    // download it
                    SingleMessageResponse resp = _server.Connection.PerformRequest<SingleMessageResponse>(
                        new SingleMessageRequest() {
                            Channel = _info.ID,
                            Index = nIndex 
                        });

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

                public MessageEnumerator(Channel chan) { 
                    _chan = chan;
                    _nCurIndex = -1;
                }

                public IMessage Current => _msgCurrent;
                object IEnumerator.Current => _msgCurrent;

                public void Dispose() {
                    _chan = null;
                    _nCurIndex = -1;
                    _msgCurrent = null;
                }

                public bool MoveNext() {
                    _nCurIndex = (_nCurIndex < 0) ? _chan._nHighestMessageIndex : (_nCurIndex - 1);
                    _msgCurrent = _chan.TryGetMessage(_nCurIndex, true);
                    return !(_msgCurrent is null);
                }

                public void Reset() {
                    _nCurIndex = -1;
                    _msgCurrent = null;
                }
            }

            public void PostMessage(MessageSignedData msg) {
                SignedStructSerializer.SignedData signed = SignedStructSerializer.Serialize(msg,
                    _server.AssociatedChatClient._arrPublicKey, _server.AssociatedChatClient._arrPrivateKey);
                
                _server.Connection.PerformRequest(new PostMessageRequest() { 
                    ChannelID = _info.ID,
                    Signature = signed.Signature,
                    SignedData = signed.Data
                });
            }

            internal void AcceptNewMessage(IMessage msg) {
                AddMessage(msg);
                MessagePosted?.Invoke(msg);
            }

            private void AddMessage(IMessage msg) {
                _dicMessages.TryAdd(msg.IndexInChannel, msg);
                Utils.InterlockedExchangeIfGreater(ref _nHighestMessageIndex, msg.IndexInChannel, msg.IndexInChannel);
            }

            internal void RemoveMessage(long nIndex, bool bPlaceTombstone = true) {
                IMessage msg, msgTombstone;

                if (_dicMessages.TryRemove(nIndex, out msg)) {
                    if (bPlaceTombstone) {
                        msgTombstone = new MissingMessage(this, nIndex);
                        _dicMessages.TryAdd(nIndex, msgTombstone);
                    }

                    if (_dicMessages.TryGetValue(nIndex, out msgTombstone))
                        MessageDeleted?.Invoke(msg, msgTombstone);
                    else
                        MessageDeleted?.Invoke(msg, null); // shouldn't happen
                }
            }

            public IMessage GetMessage(long nIndex)
                => TryGetMessage(nIndex, true);

            public event Action<IMessage> MessagePosted;
            public event Action<IMessage, IMessage> MessageDeleted;
            public event Action<IChannel> InfoChanged;
        }

        private class VoiceChannel : Channel, IVoiceChannel {
            private object _objTransmitOnMutex = new object();
            private object _objRecvOnMutex = new object();
            private object _objJoinLeaveMutex = new object();
            private VCSubchannelMask _maskTransmit, _maskRecv;
            private bool _bInChannel;
            
            internal VoiceChannel(Server server, ChannelInfo info) : base(server, info) { }

            public VCSubchannelMask TransmittingOn => _maskTransmit;
            public VCSubchannelMask ReceivingOn => _maskRecv;
            public bool InChannel => _bInChannel;

            public IEnumerable<VoiceParticipantInfo> Participants
                => ((ChatClient.Server)Server).Connection.PerformRequest<VCParticipantsResponse>(new VCParticipantsRequest() { ChannelID = Info.ID })
                    .Participants.Select(p => GetParticipant(p));

            public event Action<IMember> ParticipantEntered;
            public event Action<IMember> ParticipantLeft;
            public event Action<IMember, VCSubchannelMask> ParticipantStartedTransmitting;
            public event Action<IMember, VCSubchannelMask> ParticipantStoppedTransmitting;
            public event Action<IMember, VCSubchannelMask> ParticipantStartedReceiving;
            public event Action<IMember, VCSubchannelMask> ParticipantStoppedReceiving;
            public event Action<IMember, VCSubchannelMask, Memory<byte>> ParticipantSentData;

            public void Join() {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCJoinRequest() { ChannelID = Info.ID });
                _bInChannel = true;
            }

            public void Leave() {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCLeaveRequest() { ChannelID = Info.ID });
                _bInChannel = false;
            }

            public void SendData(VCSubchannelMask maskOn, ReadOnlySpan<byte> spanData)
                => ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCSendDataRequest() {
                    ChannelID = Info.ID,
                    Data = spanData.ToArray(),
                    Subchannel = maskOn
                });

            public Task SendDataAsync(VCSubchannelMask maskOn, ReadOnlySpan<byte> spanData)
                => ((ChatClient.Server)Server).Connection.IssueRequest(new VCSendDataRequest() {
                    ChannelID = Info.ID,
                    Data = spanData.ToArray(),
                    Subchannel = maskOn
                });

            public void StartReceiving(VCSubchannelMask maskOn) {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCReceiveRequest() {
                    ChannelID = Info.ID,
                    StartReceiveOn = maskOn
                });

                lock (_objRecvOnMutex)
                    _maskRecv |= maskOn;
            }

            public void StartTransmitting(VCSubchannelMask maskOn) {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCTransmitRequest() {
                    ChannelID = Info.ID,
                    StartTransmitOn = maskOn
                });

                lock (_objTransmitOnMutex)
                    _maskTransmit |= maskOn;
            }

            public void StopReceiving(VCSubchannelMask maskOn) {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCReceiveRequest() {
                    ChannelID = Info.ID,
                    StopReceiveOn = maskOn
                });

                lock (_objRecvOnMutex)
                    _maskRecv &= ~maskOn;
            }

            public void StopTransmitting(VCSubchannelMask maskOn) {
                ((ChatClient.Server)Server).Connection.PerformRequest<GenericSuccessResponse>(new VCTransmitRequest() {
                    ChannelID = Info.ID,
                    StopTransmitOn = maskOn
                });

                lock (_objTransmitOnMutex)
                    _maskTransmit &= ~maskOn;
            }

            private VoiceParticipantInfo GetParticipant(VCParticpant p) => new VoiceParticipantInfo() {
                Member = ((ChatClient.Server)Server).FindMember(p.PublicKey),
                TransmittingOn = p.TransmittingOn,
                ReceivingOn = p.ReceivingOn
            };

            internal void OnParticipantEntered(IMember memb) => ParticipantEntered?.Invoke(memb);
            internal void OnParticipantExited(IMember memb) => ParticipantLeft?.Invoke(memb);
            internal void OnTransmitStarted(IMember memb, VCSubchannelMask mask) => ParticipantStartedTransmitting?.Invoke(memb, mask);
            internal void OnTransmitStopped(IMember memb, VCSubchannelMask mask) => ParticipantStoppedTransmitting?.Invoke(memb, mask);
            internal void OnReceiveStarted(IMember memb, VCSubchannelMask mask) => ParticipantStartedReceiving?.Invoke(memb, mask);
            internal void OnReceiveStopped(IMember memb, VCSubchannelMask mask) => ParticipantStoppedReceiving?.Invoke(memb, mask);

            internal void OnDataReceived(IMember membSender, VCSubchannelMask mask, byte[] arrData) 
                => ParticipantSentData?.Invoke(membSender, mask, arrData);
        }

        private class User : IUser {
            private IList<Member> _lstMemberships;
            private byte[] _arrPublicKey;
            private UserProfile _profile;
            private object _objProfileMutex, _objStatusMutex;
            private Lazy<TemporaryFile> _tfAvatar;

            internal User(ReadOnlySpan<byte> spanPublicKey, UserProfile profile) {
                _lstMemberships = new List<Member>();
                _arrPublicKey = spanPublicKey.ToArray();
                _profile = profile;
                _objProfileMutex = new object();
                _objStatusMutex = new object();
                _tfAvatar = new Lazy<TemporaryFile>(DownloadAvatar, LazyThreadSafetyMode.ExecutionAndPublication);
            }

            internal void EnsureMembershipExists(Member memb) {
                lock (_lstMemberships) {
                    if (!_lstMemberships.Contains(memb))
                        _lstMemberships.Add(memb);
                }
            }

            internal void AddMembership(Member memb) {
                lock (_lstMemberships)
                    _lstMemberships.Add(memb);

                OnStatusMaybeChanged();
            }

            internal void RemoveMembership(Member memb) {
                lock (_lstMemberships)
                    _lstMemberships.Remove(memb);

                OnStatusMaybeChanged();
            }

            internal void UpdateProfile(UserProfile profile) {
                bool bDidChange = false;

                lock (_objProfileMutex) {
                    if (profile.UpdateTime > _profile.UpdateTime) {
                        bDidChange = true;
                        _profile = profile;
                    }
                }

                if (bDidChange) 
                    ProfileChanged?.Invoke(this);
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

            private TemporaryFile DownloadAvatar()
                => DownloadFile(Profile.AvatarURL);

            public System.IO.Stream OpenAvatar()
                => (_tfAvatar.Value is null) ? null : System.IO.File.Open(_tfAvatar.Value.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            public string DisplayName {
                get {
                    UserProfile profile = Profile;
                    return (profile.DisplayName?.Length ?? 0) > 0 ? profile.DisplayName
                        : ((profile.Username?.Length ?? 0) > 0 ? profile.Username : _arrPublicKey.ToHexString());
                }
            }

            public OnlineStatus CurrentStatus { get; private set; }
            public OnlineDevice CurrentDevice { get; private set; }

            internal void OnStatusMaybeChanged() {
                bool bDidChange = false;

                lock (_objStatusMutex) {
                    Tuple<OnlineStatus, OnlineDevice> tupNewStatus = CalculateStatus();
                    if ((tupNewStatus.Item1 != CurrentStatus) || (tupNewStatus.Item2 != CurrentDevice)) {
                        CurrentStatus = tupNewStatus.Item1;
                        CurrentDevice = tupNewStatus.Item2;
                        bDidChange = true;
                    }
                }

                if (bDidChange)
                    StatusChanged?.Invoke(this);
            }

            private Tuple<OnlineStatus, OnlineDevice> CalculateStatus() {
                OnlineStatus statusHighest = OnlineStatus.Offline;
                OnlineDevice devHighest = OnlineDevice.None;

                foreach (Member memb in Memberships) {
                    OnlineStatus status = memb.Info.Status;
                    if (status > statusHighest)
                        statusHighest = status;

                    OnlineDevice dev = memb.Info.Device;
                    if (dev > devHighest)
                        devHighest = dev;
                }

                return new Tuple<OnlineStatus, OnlineDevice>(statusHighest, devHighest);
            }

            public event Action<IUser> ProfileChanged;
            public event Action<IUser> StatusChanged;
        }

        private static TemporaryFile DownloadFile(string strURL) {
            strURL = strURL?.Trim();
            if ((strURL?.Length ?? 0) <= 0)
                return null;

            TemporaryFile tf = new TemporaryFile();
            try {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                    wc.DownloadFile(strURL, tf.Path);
                return tf;
            } catch (Exception) {
                tf.Dispose();
                throw;
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
            public bool IsDeletedMessageTombstone => false;
            public MessageData Data => _dataUnsigned;
            public MessageSignedData SignedData => _dataSigned;
            public DateTime SentAt => _dateSent;
            public long IndexInChannel => _nIndex;

            internal void OnDeleted() {
                Deleted?.Invoke(this);
            }

            public event Action<IMessage> Deleted;
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
            public bool IsDeletedMessageTombstone => true;
            public MessageData Data => new MessageData();
            public MessageSignedData SignedData => new MessageSignedData();
            public DateTime SentAt => DateTime.MinValue;
            public long IndexInChannel => _nIndex;

#pragma warning disable CS0067 // a deleted message will not be deleted again
            public event Action<IMessage> Deleted;
#pragma warning restore CS0067
        }
    }
}
