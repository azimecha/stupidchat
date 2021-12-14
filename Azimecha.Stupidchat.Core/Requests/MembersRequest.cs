using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Requests {
    [DataContract]
    public class MembersRequest : Protocol.RequestMessage { }

    [DataContract]
    public class MembersResponse : Protocol.ResponseMessage {
        [DataMember] public MemberInfo[] Members;
    }
}
