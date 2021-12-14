using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class ChannelsRequest : Protocol.RequestMessage { }

    [DataContract]
    public class ChannelsResponse : Protocol.ResponseMessage {
        [DataMember] public ChannelInfo[] Channels;
    }
}
