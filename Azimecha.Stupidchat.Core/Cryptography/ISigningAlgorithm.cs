using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface ISigningAlgorithm {
        int PublicKeySize { get; }
        int PrivateKeySize { get; }
        int SignatureSize { get; }

        // returns public key
        byte[] GetPublicKey(ReadOnlySpan<byte> spanPrivateKey);

        // returns signature
        byte[] Sign(ReadOnlySpan<byte> spanPrivateKey, ReadOnlySpan<byte> spanPublicKey, ReadOnlySpan<byte> spanMessage);

        // returns false if not valid
        bool TryCheck(ReadOnlySpan<byte> spanMessage, ReadOnlySpan<byte> spanSignature, ReadOnlySpan<byte> spanPublicKey);

        // throws if not valid
        void Check(ReadOnlySpan<byte> spanMessage, ReadOnlySpan<byte> spanSignature, ReadOnlySpan<byte> spanPublicKey);
    }
}
