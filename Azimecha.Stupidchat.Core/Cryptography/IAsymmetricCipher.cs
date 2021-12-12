using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface IAsymmetricCipher {
        int SharedKeySize { get; }
        int PrivateKeySize { get; }
        int PublicKeySize { get; }

        // returns symmetric key
        byte[] PerformKeyExchange(ReadOnlySpan<byte> spanMyPrivateKey, ReadOnlySpan<byte> spanTheirPublicKey);

        // returns public key
        byte[] GetPublicKey(ReadOnlySpan<byte> spanMyPrivateKey);
    }
}
