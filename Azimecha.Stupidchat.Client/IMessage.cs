using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client {
    public interface IMessage {
        IChannel Channel { get; }
        MessageData Data { get; }
    }
}
