using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class MessagesRequest : Protocol.RequestMessage {
        [DataMember] public int MaxCount;
        [DataMember] public long Before;
    }

    [DataContract]
    public class MessagesResponse : Protocol.ResponseMessage {
        [DataMember] public MessageData[] Messages;
    }
}
