using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client {
    public interface IUser {
        UserProfile Profile { get; }
        ReadOnlySpan<byte> PublicKey { get; }
        IEnumerable<IMember> Memberships { get; }
        string DisplayName { get; }

        System.IO.Stream OpenAvatar();
    }
}
