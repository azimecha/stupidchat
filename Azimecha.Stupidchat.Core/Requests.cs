using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class ChannelsRequest : Protocol.RequestMessage { }

    [DataContract]
    public class ChannelsResponse : Protocol.ResponseMessage {
        [DataMember] public ChannelInfo[] Channels;
    }

    [DataContract]
    public class MembersRequest : Protocol.RequestMessage { }

    [DataContract]
    public class MembersResponse : Protocol.ResponseMessage {
        [DataMember] public MemberInfo[] Members;
    }

    [DataContract]
    public class MessagesBeforeRequest : Protocol.RequestMessage {
        [DataMember] public int MaxCount;
        [DataMember] public long BeforeIndex;
        [DataMember] public long InChannel;
    }

    [DataContract]
    public class MessagesBeforeResponse : Protocol.ResponseMessage {
        [DataMember] public MessageData[] Messages;
    }

    [DataContract]
    public class SingleMessageRequest : Protocol.RequestMessage {
        [DataMember] public long Channel;
        [DataMember] public long Index;
    }

    [DataContract]
    public class SingleMessageResponse : Protocol.ResponseMessage {
        [DataMember] public MessageData? Message;
    }

    [DataContract]
    public class ServerInfoRequest : Protocol.RequestMessage { }

    [DataContract]
    public class ServerInfoResponse : Protocol.ResponseMessage {
        [DataMember] public ServerInfo Info;
    }

    [DataContract]
    public class UpdateProfileRequest : Protocol.RequestMessage {
        [DataMember] public byte[] SignedData;
        [DataMember] public byte[] Signature;
    }

    [DataContract]
    public class GenericSuccessResponse : Protocol.ResponseMessage { }

    [DataContract]
    public class PostMessageRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] SignedData;
        [DataMember] public byte[] Signature;
    }

    [DataContract]
    public class MessagePostedResponse : Protocol.ResponseMessage {
        [DataMember] public long ChannelID;
        [DataMember] public long MessageIndex;
    }

    [DataContract]
    public class SetNicknameRequest : Protocol.RequestMessage {
        [DataMember] public string Nickname;
    }

    [DataContract]
    public class VCParticipantsRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
    }

    [DataContract]
    public class VCParticipantsResponse : Protocol.ResponseMessage {
        [DataMember] public long ChannelID;
        [DataMember] public VCParticpant[] Participants;
    }

    [DataContract]
    public class VCJoinRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
    }

    [DataContract]
    public class VCTransmitRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
        [DataMember] public VCSubchannelMask StartTransmitOn;
        [DataMember] public VCSubchannelMask StopTransmitOn;
    }

    [DataContract]
    public class VCReceiveRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
        [DataMember] public VCSubchannelMask StartReceiveOn;
        [DataMember] public VCSubchannelMask StopReceiveOn;
    }

    [DataContract]
    public class VCSendDataRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
        [DataMember] public VCSubchannelMask Subchannel;
        [DataMember] public byte[] Data;
    }

    [DataContract]
    public class VCLeaveRequest : Protocol.RequestMessage {
        [DataMember] public long ChannelID;
    }

    [DataContract]
    public class UpdatePresenceRequest : Protocol.RequestMessage {
        [DataMember] public OnlineStatus Status;
        [DataMember] public OnlineDevice Device;
    }
}
