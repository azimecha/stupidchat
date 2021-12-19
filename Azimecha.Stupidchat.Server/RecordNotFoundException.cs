using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Server {
    public class RecordNotFoundException : Exception {
        public RecordNotFoundException(string strMessage) : base(strMessage) { }
    }
}
