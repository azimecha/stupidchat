using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Azimecha.Stupidchat.Core.Networking {
    public interface IMessageConnection : IDisposable {
        long MaximumMessageSize { get; set; }

        void Initialize();
        void Initialize(CancellationToken ct);

        void SendMessage(ReadOnlySpan<byte> spanData);

        byte[] ReceiveMesssage();
        byte[] ReceiveMesssage(CancellationToken ct);
    }
}
