using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Azimecha.Stupidchat.Core.Structures {
    [DataContract]
    public struct ServerInfo {
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public string ImageURL;
    }

    [DataContract]
    public struct MessageData {
        [DataMember] public long ID;
        [DataMember] public byte[] SenderPublicSigningKey;
        [DataMember] public long PostedTime;
        [DataMember] public byte[] SignedData; // MessageSignedData object
        [DataMember] public byte[] Signature;
    }

    [DataContract]
    public struct MessageSignedData {
        [DataMember] public string Text;
        [DataMember] public long SendTime;
        [DataMember] public string AttachmentURL;
    }

    [DataContract]
    public struct UserProfile {
        [DataMember] public string Username;
        [DataMember] public string DisplayName;
        [DataMember] public string Bio;
        [DataMember] public string AvatarURL;
        [DataMember] public string StatusDescription;
        [DataMember] public long UpdateTime;
    }

    public enum ChannelType {
        Text, Voice
    }

    [DataContract]
    public struct ChannelInfo {
        [DataMember] public long ID;
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public ChannelType Type;
    }

    public enum PowerLevel {
        Reduced,
        Normal,
        Moderator,
        Administrator
    }

    [DataContract]
    public struct MemberInfo {
        [DataMember] public byte[] PublicKey;
        [DataMember] public string Nickname;
        [DataMember] public PowerLevel Power;
        [DataMember] public byte[] Profile; // UserProfile object
        [DataMember] public byte[] ProfileSignature;
        [DataMember] public OnlineStatus Status;
        [DataMember] public OnlineDevice Device;
    }

    [Flags]
    public enum VCSubchannelMask : ulong {
        None = 0,
        MicrophoneAudio = 1 << 0,
        CameraVideo = 1 << 1,
        DesktopAudio = 1 << 2,
        DesktopVideo = 1 << 3,
        MediaAudio = 1 << 4,
        MediaVideo = 1 << 5,
        NonpersistentText = 1 << 6
    }

    [DataContract]
    public struct VCParticpant {
        [DataMember] public byte[] PublicKey;
        [DataMember] public VCSubchannelMask TransmittingOn;
        [DataMember] public VCSubchannelMask ReceivingOn;
    }

    public enum OnlineStatus : int {
        Offline = 0,
        Away = 100,
        Online = 200,
        DoNotDisturb = 300
    }

    public enum OnlineDevice : int {
        None = 0,
        Mobile = 100,
        Browser = 200,
        Desktop = 300,
        VirtualReality = 400
    }
}
