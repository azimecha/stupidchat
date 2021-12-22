using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class Argon2i : IKeyDerivationAlgorithm {
        public void DeriveKey(Span<byte> spanKeyOUT, ReadOnlySpan<byte> spanInputData, ReadOnlySpan<byte> spanSalt, Speed speed = Speed.Normal) {
            uint nBlocks, nIterations;

            switch (speed) {
                case Speed.VeryFast:
                    nBlocks = 8192;
                    nIterations = 3;
                    break;

                case Speed.Fast:
                    nBlocks = 8192 * 2;
                    nIterations = 3;
                    break;

                case Speed.Normal:
                default:
                    nBlocks = 8192 * 4;
                    nIterations = 4;
                    break;

                case Speed.Slow:
                    nBlocks = 8192 * 8;
                    nIterations = 5;
                    break;

                case Speed.VerySlow:
                    nBlocks = 8192 * 16;
                    nIterations = 5;
                    break;
            }

            byte[] arrWorkArea = new byte[nBlocks * 1024];
            Monocypher.Monocypher.crypto_argon2i(spanKeyOUT, arrWorkArea, nBlocks, nIterations, spanInputData, spanSalt);
        }
    }
}
