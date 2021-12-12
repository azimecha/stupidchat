using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public class RemoteErrorException : Exception {
        public RemoteErrorException(Protocol.ErrorResponse resp) : base($"Remote server reported error on request {resp.RequestID}: {resp.Summary}") {
            Error = resp;
        }

        public Protocol.ErrorResponse Error { get; private set; }

        public override string ToString()
            => $"{Message}\n---- Start of remote error information ----\n{Error.Description}\n---- End of remote error information ----\n{StackTrace}";
    }
}
