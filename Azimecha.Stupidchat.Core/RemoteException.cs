using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public abstract class RemoteException : Exception {
        public RemoteException(string strSummary, string strDescription) : base($"Request failed - \"{strSummary}\"") {
            Summary = strSummary;
            Description = strDescription;
        }

        public string Summary { get; private set; }
        public string Description { get; private set; }

        public override string StackTrace => $"---- Description from server ----\r\n{Description}\r\n---- End of description ----\r\n{base.StackTrace}";
    }

    public class RequestFailedException : RemoteException {
        public RequestFailedException(Protocol.ErrorResponse msgError) : base(msgError.Summary, msgError.Description) { }
    }

    public class SpontaneousRemoteException : RemoteException {
        public SpontaneousRemoteException(Protocol.ErrorNotification msgError) : base(msgError.Summary, msgError.Description) { }
    }
}
