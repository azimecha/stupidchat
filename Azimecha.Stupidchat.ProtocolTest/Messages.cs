using Azimecha.Stupidchat.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.ProtocolTest {

    public class SampleNotifMessage : NotificationMessage {
        public override string ToString() => "Sample notification message";
    }

    public class SampleRequestMessage : RequestMessage {
        public override string ToString() => "Sample request message";
    }

    public class SampleBadRequestMessage : RequestMessage {
        public override string ToString() => "Sample \"bad\" request message (should cause exception)";
    }

    public class SampleResponseMessage : ResponseMessage {
        public override string ToString() => "Sample response message";
    }
}
