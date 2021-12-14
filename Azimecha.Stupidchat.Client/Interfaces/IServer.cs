using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Client.Interfaces {
    public interface IServer {
        string Address { get; }
        int Port { get; }
        ReadOnlySpan<byte> ID { get; }

        ServerInfo GetInformation();
        IMember[] GetMembers();
        IChannel[] GetChannels();
    }
}
