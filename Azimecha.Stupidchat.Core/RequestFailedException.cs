using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public class RequestFailedException : Exception {
        public RequestFailedException(Protocol.ErrorResponse msgError) : this(msgError.Summary, msgError.Description) { }

        public RequestFailedException(string strSummary, string strDescription) : base($"Request failed - \"{strDescription}\"") {
            Summary = strSummary;
            Description = strDescription;
        }

        public string Summary { get; private set; }
        public string Description { get; private set; }

        public override string ToString() 
            => $"{Message}\r\n---- Description ----\r\n{Description}\r\n---- End of description ----\r\n{StackTrace}";
    }
}
