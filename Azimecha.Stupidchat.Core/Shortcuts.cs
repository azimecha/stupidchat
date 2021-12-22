using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public static class Shortcuts {
        public static Structures.MessageSignedData GetSignedData(this Structures.MessageData data)
            => SignedStructSerializer.Deserialize<Structures.MessageSignedData>(data.SignedData, data.Signature, data.SenderPublicSigningKey);

        public static Structures.UserProfile GetProfile(this Structures.MemberInfo infMember)
            => SignedStructSerializer.Deserialize<Structures.UserProfile>(infMember.Profile, infMember.ProfileSignature, infMember.PublicKey);

        public static string GetName(this Structures.MemberInfo infMember) {
            if ((infMember.Nickname?.Length ?? 0) > 0)
                return infMember.Nickname;

            Structures.UserProfile profile = infMember.GetProfile();
            if ((profile.DisplayName?.Length ?? 0) > 0)
                return profile.DisplayName;
            if ((profile.Username?.Length ?? 0) > 0)
                return profile.Username;

            return "#" + Utils.ToHexString(infMember.PublicKey);
        }
    }
}
