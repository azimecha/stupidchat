using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Core.Notifications {
    [DataContract]
    public class ServerInfoChangedNotification : Protocol.NotificationMessage {
        [DataMember] public ServerInfo Info;
    }

    [DataContract]
    public class MemberJoinedNotification : Protocol.NotificationMessage {
        [DataMember] public MemberInfo Member;
    }

    [DataContract]
    public class MemberInfoChangedNotification : Protocol.NotificationMessage {
        [DataMember] public MemberInfo Member;
        [DataMember] public bool ProfileChanged;
    }

    [DataContract]
    public class MemberLeftNotification : Protocol.NotificationMessage {
        [DataMember] public byte[] MemberPublicKey;
    }

    [DataContract]
    public class ChannelAddedNotification : Protocol.NotificationMessage {
        [DataMember] public ChannelInfo Channel;
    }

    [DataContract]
    public class ChannelInfoChangedNotification : Protocol.NotificationMessage {
        [DataMember] public ChannelInfo Channel;
    }

    [DataContract]
    public class ChannelRemovedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
    }

    [DataContract]
    public class MessagePostedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public long MessageIndex;
        [DataMember] public MessageData Message;
    }

    [DataContract]
    public class MessageDeletedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public long MessageIndex;
    }

    [DataContract]
    public class VCParticipantEnteredNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
    }

    [DataContract]
    public class VCParticipantExitedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
    }

    [DataContract]
    public class VCTransmitStartedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
        [DataMember] public VCSubchannelMask Subchannels;
    }

    [DataContract]
    public class VCTransmitStoppedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
        [DataMember] public VCSubchannelMask Subchannels;
    }

    [DataContract]
    public class VCReceiveStartedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
        [DataMember] public VCSubchannelMask Subchannels;
    }

    [DataContract]
    public class VCReceiveStoppedNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] ParticipantPublicKey;
        [DataMember] public VCSubchannelMask Subchannels;
    }

    [DataContract]
    public class VCDataBlockNotification : Protocol.NotificationMessage {
        [DataMember] public long ChannelID;
        [DataMember] public byte[] SenderPublicKey;
        [DataMember] public VCSubchannelMask Subchannel;
        [DataMember] public byte[] Data;
    }
}
