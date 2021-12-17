using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class MessagesBeforeRequest : Protocol.RequestMessage {
        [DataMember] public int MaxCount;
        [DataMember] public long Before;
    }

    [DataContract]
    public class MessagesBeforeResponse : Protocol.ResponseMessage {
        [DataMember] public MessageData[] Messages;
    }

    [DataContract]
    public class SingleMessageRequest : Protocol.RequestMessage {
        [DataMember] public long Index;
    }

    [DataContract]
    public class SingleMessageResponse : Protocol.ResponseMessage {
        [DataMember] public MessageData? Message;
    }
}
