using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public interface IRandomNumberGenerator {
        void Fill(Span<byte> spanToRandomize);
    }
}
