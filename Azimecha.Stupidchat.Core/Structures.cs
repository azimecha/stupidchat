using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Structures {
    [DataContract]
    public struct ServerInfo {
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public string ImageURL;
    }

    [DataContract]
    public struct Message {
        [DataMember] public long ID;
        [DataMember] public byte[] SenderID;
        [DataMember] public string Text;
        [DataMember] public long SendTime;
        [DataMember] public string AttachmentURL;
    }

    [DataContract]
    public struct UserProfile {
        [DataMember] public string Username;
        [DataMember] public string DisplayName;
        [DataMember] public string Bio;
        [DataMember] public string AvatarURL;
    }

    public enum ChannelType {
        Text, Voice
    }

    [DataContract]
    public struct ChannelInfo {
        [DataMember] public long ID;
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public ChannelType Type;
    }

    public enum PowerLevel {
        Reduced,
        Normal,
        Moderator,
        Administrator
    }

    [DataContract]
    public struct MemberInfo {
        [DataMember] public byte[] UserID;
        [DataMember] public string Nickname;
        [DataMember] public PowerLevel Power;
        [DataMember] public string Profile; // Serialized UserProfile object
        [DataMember] public byte[] ProfileSignature;
    }
}
