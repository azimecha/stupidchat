using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public class NoHandlerException : Exception {
        public NoHandlerException(string strMessage) : base(strMessage) { }
    }
}
