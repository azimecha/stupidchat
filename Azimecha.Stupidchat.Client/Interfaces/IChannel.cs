using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Client.Interfaces {
    public interface IChannel {
        IServer Server { get; }
        long ID { get; }

        ChannelInfo GetInfo();

        Message[] GetMessagesAfter(DateTime time);
        Message[] GetMessagesAfter(Message msg);
        Message[] GetMessagesBefore(DateTime time, int nMaxCount);
        Message[] GetMessagesBefore(Message msg, int nMaxCount);

        void PostMessage(Message msg);
        void UpdateMessage(Message msg);
        void DeleteMessage(Message msg);
    }
}
