using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Client {
    public interface IMessage {
        IChannel Channel { get; }
        IMember Sender { get; }
        bool IsDeletedMessageTombstone { get; }
        MessageData Data { get; }
        MessageSignedData SignedData { get; }
        DateTime SentAt { get; }
        long IndexInChannel { get; }

        event Action<IMessage> Deleted;
    }
}
