using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class X25519Cipher : IAsymmetricCipher {
        public int SharedKeySize => 32;
        public int PrivateKeySize => 32;
        public int PublicKeySize => 32;

        public byte[] GetPublicKey(ReadOnlySpan<byte> spanMyPrivateKey) {
            if (spanMyPrivateKey.Length != PrivateKeySize)
                throw new ArgumentException($"Private key size {spanMyPrivateKey.Length} does not match required size {PrivateKeySize}");

            byte[] arrPublicKey = new byte[PublicKeySize];
            Monocypher.Monocypher.crypto_x25519_public_key(new Span<byte>(arrPublicKey), spanMyPrivateKey);
            return arrPublicKey;
        }

        public byte[] PerformKeyExchange(ReadOnlySpan<byte> spanMyPrivateKey, ReadOnlySpan<byte> spanTheirPublicKey) {
            if (spanMyPrivateKey.Length != PrivateKeySize)
                throw new ArgumentException($"Private key size {spanMyPrivateKey.Length} does not match required size {PrivateKeySize}");
            if (spanTheirPublicKey.Length != PublicKeySize)
                throw new ArgumentException($"Public key size {spanTheirPublicKey.Length} does not match required size {PublicKeySize}");

            byte[] arrSharedKey = new byte[SharedKeySize];
            Monocypher.Monocypher.crypto_x25519(new Span<byte>(arrSharedKey), spanMyPrivateKey, spanTheirPublicKey);
            return arrSharedKey;
        }
    }
}
