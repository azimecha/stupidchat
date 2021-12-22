using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface IKeyDerivationAlgorithm {
        void DeriveKey(Span<byte> spanKeyOUT, ReadOnlySpan<byte> spanInputData, ReadOnlySpan<byte> spanSalt,
            Speed speed = Speed.Normal);
    }

    public enum Speed {
        VeryFast,
        Fast,
        Normal,
        Slow,
        VerySlow
    }
}
