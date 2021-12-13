using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface IAuthenticatedSymmetricCipher {
        int AuthenticationCodeSize { get; }
        int KeySize { get; }
        int NonceSize { get; }

        // encrypts in place
        void Encrypt(Span<byte> spanMessage, ReadOnlySpan<byte> spanKey, ReadOnlySpan<byte> spanNonce, Span<byte> spanAuthCode);

        // decrypts in place
        void Decrypt(Span<byte> spanMessage, ReadOnlySpan<byte> spanKey, ReadOnlySpan<byte> spanNonce, ReadOnlySpan<byte> spanAuthCode);
    }
}
