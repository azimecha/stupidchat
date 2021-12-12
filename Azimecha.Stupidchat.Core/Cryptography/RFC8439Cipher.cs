using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class RFC8439Cipher : ISymmetricCipher {
        public int AuthenticationCodeSize => 16;
        public int KeySize => 32;
        public int NonceSize => 24;

        public void Encrypt(Span<byte> spanMessage, ReadOnlySpan<byte> spanKey, ReadOnlySpan<byte> spanNonce, Span<byte> spanAuthCode) {
            CheckSizes(spanKey, spanNonce, spanAuthCode);
            Monocypher.Monocypher.crypto_lock(spanAuthCode, spanMessage, spanKey, spanNonce, spanMessage);
        }

        public void Decrypt(Span<byte> spanMessage, ReadOnlySpan<byte> spanKey, ReadOnlySpan<byte> spanNonce, ReadOnlySpan<byte> spanAuthCode) {
            CheckSizes(spanKey, spanNonce, spanAuthCode);
            if (Monocypher.Monocypher.crypto_unlock(spanMessage, spanKey, spanNonce, spanAuthCode, spanMessage) < 0)
                throw new DecryptionFailedException();
        }

        private void CheckSizes(ReadOnlySpan<byte> spanKey, ReadOnlySpan<byte> spanNonce, ReadOnlySpan<byte> spanAuthCode) {
            if (spanKey.Length != KeySize)
                throw new ArgumentException($"Key size {spanKey.Length} does not match required size {KeySize}");

            if (spanNonce.Length != NonceSize)
                throw new ArgumentException($"Nonce size {spanNonce.Length} does not match required size {NonceSize}");

            if (spanAuthCode.Length != AuthenticationCodeSize)
                throw new ArgumentException($"Message authentication code size {spanAuthCode.Length} does not match required size {AuthenticationCodeSize}");
        }
    }
}
