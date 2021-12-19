using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Azimecha.Stupidchat.Server {
    public class DebugLogListener : ILogListener {
        private object _objMutex = new object();

        public void LogMessage(string strMessage) {
            lock (_objMutex)
                Debug.WriteLine(strMessage);
        }
    }
}
