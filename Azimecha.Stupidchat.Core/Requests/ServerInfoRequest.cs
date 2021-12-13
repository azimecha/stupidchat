using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class ServerInfoRequest : Protocol.RequestMessage { }

    [DataContract]
    public class ServerInfoResponse : Protocol.ResponseMessage {
        [DataMember] public ServerInformation Info;
    }
}
