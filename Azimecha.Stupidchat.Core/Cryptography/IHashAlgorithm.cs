using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface IHashAlgorithm {
        int HashSize { get; }
        byte[] Hash(ReadOnlySpan<byte> spanData);
    }
}
