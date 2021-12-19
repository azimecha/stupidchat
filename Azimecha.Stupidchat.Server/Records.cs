using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Server.Records {
    public class ChannelRecord {
        [PrimaryKey, AutoIncrement]
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Core.Structures.ChannelType Type { get; set; }
        public long NextMessageID { get; set; }

        public Core.Structures.ChannelInfo ToChannelInfo() => new Core.Structures.ChannelInfo() {
            Description = Description,
            ID = ID,
            Name = Name,
            Type = Type
        };
    }

    public class MessageRecord {
        [PrimaryKey, AutoIncrement] 
        public long UniqueID { get; set; }
        [Indexed] public long ChannelID { get; set; }
        [Indexed] public long MessageIndex { get; set; }
        [Indexed] public byte[] SenderPublicKey { get; set; }
        public DateTime DatePosted { get; set; }
        public byte[] SignedData { get; set; }
        public byte[] Signature { get; set; }

        public Core.Structures.MessageData ToMessageData() => new Core.Structures.MessageData() {
            ID = MessageIndex,
            PostedTime = DatePosted.Ticks,
            Signature = Signature,
            SignedData = SignedData,
            SenderPublicSigningKey = SenderPublicKey
        };
    }

    public class MemberRecord {
        [PrimaryKey, AutoIncrement]
        public long MemberID { get; set; }
        [Indexed] public byte[] PublicKey { get; set; }
        public string Nickname { get; set; }
        public Core.Structures.PowerLevel Power { get; set; }
        public byte[] SignedProfile { get; set; }
        public byte[] ProfileSignature { get; set; }

        public Core.Structures.MemberInfo ToMemberInfo() => new Core.Structures.MemberInfo {
            Nickname = Nickname,
            Power = Power,
            Profile = SignedProfile,
            ProfileSignature = ProfileSignature,
            PublicKey = PublicKey
        };
    }
}
