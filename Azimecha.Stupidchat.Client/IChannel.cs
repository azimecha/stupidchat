﻿using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Client {
    public interface IChannel {
        IServer Server { get; }
        ChannelInfo Info { get; }

        IEnumerable<IMessage> Messages { get; }

        void PostMessage(MessageSignedData msg);
    }
}