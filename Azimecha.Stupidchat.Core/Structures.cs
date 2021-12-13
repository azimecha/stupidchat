using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Structures {
    [DataContract]
    public struct ServerInformation {
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public string ImageURL;
    }
}
