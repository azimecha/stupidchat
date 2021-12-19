using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Server.Flattened {
    [DataContract]
    public struct Message {
        [DataMember] public long UID;
        [DataMember] public long ChannelID;
        [DataMember] public long Index;
        [DataMember] public byte[] SenderPublicKey;
        [DataMember] public DateTime Posted;
        [DataMember] public string Text;
        [DataMember] public DateTime Sent;
        [DataMember] public string AttachmentURL;
    }

    [DataContract]
    public struct Member {
        [DataMember] public long ID;
        [DataMember] public byte[] PublicKey;
        [DataMember] public string Nickname;
        [DataMember] public Core.Structures.PowerLevel Power;
        [DataMember] public string Username;
        [DataMember] public string DisplayName;
        [DataMember] public string Bio;
        [DataMember] public string AvatarURL;
        [DataMember] public DateTime LastProfileUpdate;
    }
}
