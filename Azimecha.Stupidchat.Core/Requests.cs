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
}
