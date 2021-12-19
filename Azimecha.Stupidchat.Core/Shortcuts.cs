using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public static class Shortcuts {
        public static Structures.MessageSignedData GetSignedData(this Structures.MessageData data)
            => SignedStructSerializer.Deserialize<Structures.MessageSignedData>(data.SignedData, data.Signature, data.SenderPublicSigningKey);

        public static Structures.UserProfile GetProfile(this Structures.MemberInfo infMember)
            => SignedStructSerializer.Deserialize<Structures.UserProfile>(infMember.Profile, infMember.ProfileSignature, infMember.PublicKey);
    }
}
