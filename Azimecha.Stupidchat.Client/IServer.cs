using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Client {
    public interface IServer {
        string Address { get; }
        int Port { get; }
        ReadOnlySpan<byte> ID { get; }

        ServerInfo Info { get; }
        IEnumerable<IMember> Members { get; }
        IEnumerable<IChannel> Channels { get; }
    }
}
