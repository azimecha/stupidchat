using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class SignatureCheckFailedException : Exception {
        public SignatureCheckFailedException() : base("Signature check failed. The data has been corrupted or forged.") { }
    }
}
