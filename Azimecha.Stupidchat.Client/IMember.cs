using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client {
    public interface IMember {
        IServer Server { get; }
        MemberInfo Info { get; }
        IUser User { get; }
        string DisplayName { get; }
    }
}
