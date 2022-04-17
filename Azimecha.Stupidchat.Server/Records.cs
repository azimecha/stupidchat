using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core;

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

        public override string ToString() => ToChannelInfo().ToDataString();
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

        public Flattened.Message Flatten() {
            Core.Structures.MessageSignedData dataSigned = ToMessageData().GetSignedData();
            return new Flattened.Message() {
                AttachmentURL = dataSigned.AttachmentURL,
                ChannelID = ChannelID,
                Index = MessageIndex,
                Posted = DatePosted,
                SenderPublicKey = SenderPublicKey,
                Sent = new DateTime(dataSigned.SendTime),
                Text = dataSigned.Text,
                UID = UniqueID
            };
        }

        public override string ToString() => Flatten().ToDataString();
    }

    public class MemberRecord {
        [PrimaryKey, AutoIncrement]
        public long MemberID { get; set; }
        [Indexed] public string PublicKeyString { get; set; }
        public string Nickname { get; set; }
        public Core.Structures.PowerLevel Power { get; set; }
        public byte[] SignedProfile { get; set; }
        public byte[] ProfileSignature { get; set; }
        public Core.Structures.OnlineStatus Status { get; set; }
        public Core.Structures.OnlineDevice Device { get; set; }
        public bool IsOnline { get; set; }

        public Core.Structures.MemberInfo ToMemberInfo() => new Core.Structures.MemberInfo {
            Nickname = Nickname,
            Power = Power,
            Profile = SignedProfile,
            ProfileSignature = ProfileSignature,
            PublicKey = Utils.HexStringToBytes(PublicKeyString),
            Status = IsOnline ? Status : Core.Structures.OnlineStatus.Offline,
            Device = IsOnline ? Device : Core.Structures.OnlineDevice.None
        };

        public Flattened.Member Flatten() {
            Core.Structures.UserProfile profile = ToMemberInfo().GetProfile();
            return new Flattened.Member() {
                AvatarURL = profile.AvatarURL,
                Bio = profile.Bio,
                DisplayName = profile.DisplayName,
                ID = MemberID,
                LastProfileUpdate = new DateTime(profile.UpdateTime),
                Nickname = Nickname,
                Power = Power,
                PublicKey = Utils.HexStringToBytes(PublicKeyString),
                Username = profile.Username
            };
        }

        public override string ToString() => Flatten().ToDataString();
    }
}
