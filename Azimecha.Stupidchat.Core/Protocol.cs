using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Protocol {
    public interface IMessage {
        long? AssociatedID { get; }
    }

    [DataContract]
    public class RequestMessage : IMessage {
        [DataMember] public long ID;
        public long? AssociatedID => ID;
    }

    [DataContract]
    public class ResponseMessage : IMessage {
        [DataMember] public long RequestID;
        public long? AssociatedID => RequestID;
    }

    [DataContract]
    public class ErrorResponse : ResponseMessage {
        [DataMember] public string Summary;
        [DataMember] public string Description;
    }

    [DataContract]
    public class NotificationMessage : IMessage {
        public long? AssociatedID => null;
    }

    [DataContract]
    public class ErrorNotification : NotificationMessage {
        [DataMember] public string Summary;
        [DataMember] public string Description;
    }

    [DataContract]
    public class MessageContainer {
        [DataMember] public long SentAt;
        [DataMember] public string MessageType;
        [DataMember] public IMessage Message;
    }

    public static class ProtocolConstants {

        public const int DEFAULT_PORT = 22001;
    }
}
