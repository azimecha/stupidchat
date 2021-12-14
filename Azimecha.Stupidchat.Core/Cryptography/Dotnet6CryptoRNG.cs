using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class Dotnet6CryptoRNG : IRandomNumberGenerator {
        private System.Security.Cryptography.RandomNumberGenerator _rng = System.Security.Cryptography.RandomNumberGenerator.Create();

        public void Fill(Span<byte> spanToRandomize) {
            byte[] arrBytes = new byte[spanToRandomize.Length];
            _rng.GetBytes(arrBytes);
            arrBytes.CopyTo(spanToRandomize);
        }
    }
}
