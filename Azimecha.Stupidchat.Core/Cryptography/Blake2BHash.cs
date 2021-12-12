using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class Blake2BHash : IHashAlgorithm {
        public int HashSize => 64;

        public byte[] Hash(ReadOnlySpan<byte> spanData) {
            byte[] arrHash = new byte[HashSize];
            Monocypher.Monocypher.crypto_blake2b(arrHash, spanData);
            return arrHash;
        }
    }
}
