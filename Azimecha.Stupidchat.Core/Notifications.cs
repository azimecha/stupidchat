using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Core.Notifications {
    [DataContract]
    public class MemberJoinedNotification : Protocol.NotificationMessage {
        [DataMember] public MemberInfo Member;
    }

    [DataContract]
    public class MemberInfoChangedNotification : Protocol.NotificationMessage {
        [DataMember] public MemberInfo Member;
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
}
