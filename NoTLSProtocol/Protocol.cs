using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Protocol {
    public interface IMessage { }

    [DataContract]
    public class RequestMessage : IMessage {
        [DataMember] public long ID;
    }

    [DataContract]
    public class ResponseMessage : IMessage {
        [DataMember] public long RequestID;
    }

    [DataContract]
    public class ErrorResponse : ResponseMessage {
        [DataMember] public string Summary;
        [DataMember] public string Description;
    }

    [DataContract]
    public class NotificationMessage : IMessage { }

    [DataContract]
    public class MessageContainer {
        [DataMember] public long SentAt;
        [DataMember] public IMessage Message;
    }

    [StructLayout(LayoutKind.Sequential, Size = ProtocolConstants.NONCE_SIZE)]
    public struct EncryptedMessageNonce {
        public long Time;
        public int Process;
        public int Thread;
        public long Counter;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EncryptedMessageHeader {
        public EncryptedMessageNonce Nonce;
        public fixed byte MAC[ProtocolConstants.MAC_SIZE];
        public int DataSize;
    }

    public static class ProtocolConstants {
        public const int NONCE_SIZE = 24;
        public const int MAC_SIZE = 16;
        public const int KEY_SIZE = 32;
    }
}
