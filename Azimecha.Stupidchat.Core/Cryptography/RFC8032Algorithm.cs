using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class RFC8032Algorithm : ISigningAlgorithm {
        public int PublicKeySize => 32;
        public int PrivateKeySize => 32;
        public int SignatureSize => 64;

        public byte[] GetPublicKey(ReadOnlySpan<byte> spanPrivateKey) {
            byte[] arrPublicKey = new byte[PublicKeySize];
            Monocypher.Monocypher.crypto_sign_public_key(arrPublicKey, spanPrivateKey);
            return arrPublicKey;
        }

        public byte[] Sign(ReadOnlySpan<byte> spanPrivateKey, ReadOnlySpan<byte> spanPublicKey, ReadOnlySpan<byte> spanMessage) {
            byte[] arrSignature = new byte[SignatureSize];
            Monocypher.Monocypher.crypto_sign(arrSignature, spanPrivateKey, spanPublicKey, spanMessage);
            return arrSignature;
        }

        public bool TryCheck(ReadOnlySpan<byte> spanMessage, ReadOnlySpan<byte> spanSignature, ReadOnlySpan<byte> spanPublicKey)
            => Monocypher.Monocypher.crypto_check(spanSignature, spanPublicKey, spanMessage) == 0;

        public void Check(ReadOnlySpan<byte> spanMessage, ReadOnlySpan<byte> spanSignature, ReadOnlySpan<byte> spanPublicKey) {
            if (!TryCheck(spanMessage, spanSignature, spanPublicKey))
                throw new SignatureCheckFailedException();
        }
    }
}
