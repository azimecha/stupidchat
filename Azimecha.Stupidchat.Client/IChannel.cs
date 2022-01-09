using System;
using System.Collections.Generic;
using System.Text;
using Azimecha.Stupidchat.Core.Structures;

namespace Azimecha.Stupidchat.Client {
    public interface IChannel {
        IServer Server { get; }
        ChannelInfo Info { get; }

        IEnumerable<IMessage> Messages { get; }
        IMessage GetMessage(long nIndex);

        void PostMessage(MessageSignedData msg);

        event Action<IMessage> MessagePosted;
        event Action<IMessage, IMessage> MessageDeleted; // first is old message object, second is new tombstone
        event Action<IChannel> InfoChanged;
    }

    public interface IVoiceChannel : IChannel {
        void Join();
        void Leave();

        VCSubchannelMask TransmittingOn { get; }
        VCSubchannelMask ReceivingOn { get; }
        bool InChannel { get; }

        IEnumerable<VoiceParticipantInfo> Participants { get; }

        void StartTransmitting(VCSubchannelMask maskOn);
        void StopTransmitting(VCSubchannelMask maskOn);

        void StartReceiving(VCSubchannelMask maskOn);
        void StopReceiving(VCSubchannelMask maskOn);

        void SendData(VCSubchannelMask maskOn, ReadOnlySpan<byte> spanData);
        System.Threading.Tasks.Task SendDataAsync(VCSubchannelMask maskOn, ReadOnlySpan<byte> spanData);

        event Action<IMember> ParticipantEntered;
        event Action<IMember> ParticipantLeft;
        event Action<IMember, VCSubchannelMask> ParticipantStartedTransmitting;
        event Action<IMember, VCSubchannelMask> ParticipantStoppedTransmitting;
        event Action<IMember, VCSubchannelMask> ParticipantStartedReceiving;
        event Action<IMember, VCSubchannelMask> ParticipantStoppedReceiving;
        event Action<IMember, VCSubchannelMask, Memory<byte>> ParticipantSentData;
    }

    public struct VoiceParticipantInfo {
        public IMember Member;
        public VCSubchannelMask TransmittingOn;
        public VCSubchannelMask ReceivingOn;
    }
}
