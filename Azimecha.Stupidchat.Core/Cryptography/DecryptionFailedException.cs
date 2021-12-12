using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public class DecryptionFailedException : Exception {
        public DecryptionFailedException() : base("The data could not be decrypted. It was encrypted using a different key, or was corrupted.") { }
    }
}
