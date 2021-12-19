using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public class InvalidDateException : Exception {
        public InvalidDateException(string strMessage) : base(strMessage) { }
    }
}
