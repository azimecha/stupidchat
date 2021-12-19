using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Server {
    public interface ILogListener {
        void LogMessage(string strMessage);
    }
}
