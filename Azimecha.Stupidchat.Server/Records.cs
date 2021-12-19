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
    }

    public class MessageRecord {
        [PrimaryKey, AutoIncrement] 
        public long UniqueID { get; set; }
        [Indexed] public long ChannelID { get; set; }
        [Indexed] public long MessageIndex { get; set; }
        [Indexed] public byte[] SenderPublicKey { get; set; }
        public DateTime DatePosted { get; set; }
        public byte[] SignedData { get; set; }
        public byte[] Signature;
    }

    public class MemberRecord {
        [PrimaryKey, AutoIncrement]
        public long MemberID { get; set; }
        [Indexed] public byte[] PublicKey { get; set; }
        public string Nickname;
        public Core.Structures.PowerLevel Power;
        public byte[] SignedProfile;
        public byte[] ProfileSignature;
    }
}
