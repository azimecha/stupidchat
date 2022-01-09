using Azimecha.Stupidchat.Core.Requests;
using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Azimecha.Stupidchat.Core.Notifications;
using System.Threading;

namespace Azimecha.Stupidchat.Server {
    internal class VoiceChannel {
        private Server _server;
        private long _nChannelID;
        private ConcurrentDictionary<long, ParticipantInfo> _dicParticipants;

        public VoiceChannel(Server server, long nChannelID) {
            _server = server;
            _nChannelID = nChannelID;
            _dicParticipants = new ConcurrentDictionary<long, ParticipantInfo>();
        }

        public VCParticipantsResponse ProcessParticipantsRequest(ClientConnection conn, VCParticipantsRequest req) => new VCParticipantsResponse() {
            ChannelID = _nChannelID,
            Participants = _dicParticipants.Values.Select(p => p.ToStruct()).ToArray()
        };

        public GenericSuccessResponse ProcessJoinRequest(ClientConnection conn, VCJoinRequest req) {
            long nMemberID = CheckMemberID(conn);

            if (!_dicParticipants.TryAdd(nMemberID, new ParticipantInfo() { Connection = conn }))
                throw new InvalidOperationException("Already in this voice chat");

            _server.BroadcastNotification(new VCParticipantEnteredNotification() {
                ChannelID = _nChannelID,
                ParticipantPublicKey = conn.ClientPublicKey.ToArray()
            });

            return new GenericSuccessResponse();
        }

        public GenericSuccessResponse ProcessTransmitRequest(ClientConnection conn, VCTransmitRequest req) {
            ParticipantInfo info = GetParticipant(conn);

            VCSubchannelMask maskStartingOn, maskStoppingOn;
            lock (info.TransmitStartStopMutex) {
                maskStartingOn = ~info.TransmittingOn & req.StartTransmitOn;
                maskStoppingOn = info.TransmittingOn & req.StopTransmitOn;
                info.TransmittingOn = (info.TransmittingOn | req.StartTransmitOn) & ~req.StopTransmitOn;
            }

            if (maskStartingOn != 0)
                BroadcastNotification(new VCTransmitStartedNotification() {
                    ChannelID = _nChannelID,
                    ParticipantPublicKey = info.Connection.ClientPublicKey.ToArray(),
                    Subchannels = maskStartingOn
                });

            if (maskStoppingOn != 0)
                BroadcastNotification(new VCTransmitStoppedNotification() {
                    ChannelID = _nChannelID,
                    ParticipantPublicKey = info.Connection.ClientPublicKey.ToArray(),
                    Subchannels = maskStoppingOn
                });

            return new GenericSuccessResponse();
        }

        public GenericSuccessResponse ProcessReceiveRequest(ClientConnection conn, VCReceiveRequest req) {
            ParticipantInfo info = GetParticipant(conn);

            VCSubchannelMask maskStartingOn, maskStoppingOn;
            lock (info.ReceiveStartStopMutex) {
                maskStartingOn = ~info.ReceivingOn & req.StartReceiveOn;
                maskStoppingOn = info.ReceivingOn & req.StopReceiveOn;
                info.ReceivingOn = (info.ReceivingOn | req.StartReceiveOn) & ~req.StopReceiveOn;
            }

            if (maskStartingOn != 0)
                BroadcastNotification(new VCReceiveStartedNotification() {
                    ChannelID = _nChannelID,
                    ParticipantPublicKey = info.Connection.ClientPublicKey.ToArray(),
                    Subchannels = maskStartingOn
                });

            if (maskStoppingOn != 0)
                BroadcastNotification(new VCReceiveStoppedNotification() {
                    ChannelID = _nChannelID,
                    ParticipantPublicKey = info.Connection.ClientPublicKey.ToArray(),
                    Subchannels = maskStoppingOn
                });

            return new GenericSuccessResponse();
        }

        public GenericSuccessResponse ProcessSendDataRequest(ClientConnection conn, VCSendDataRequest req) {
            ParticipantInfo infSender = GetParticipant(conn);

            if (req.Subchannel == 0) return new GenericSuccessResponse();

            lock (infSender.TransmitStartStopMutex) {
                if ((req.Subchannel & ~infSender.TransmittingOn) != 0)
                    throw new InvalidOperationException("Cannot send data - not transmitting on subchannel");
            }

            foreach (ParticipantInfo infOther in _dicParticipants.Values) {
                if (infOther.Connection.ClientMemberID == conn.ClientMemberID)
                    continue;

                if ((infOther.ReceivingOn & req.Subchannel) == 0)
                    continue;

                infOther.Connection.Connection.SendNotification(new VCDataBlockNotification() {
                    ChannelID = _nChannelID,
                    Data = req.Data,
                    SenderPublicKey = infSender.Connection.ClientPublicKey.ToArray(),
                    Subchannel = infOther.ReceivingOn & req.Subchannel
                });
            }

            return new GenericSuccessResponse();
        }
        
        public GenericSuccessResponse ProcessLeaveRequest(ClientConnection conn, VCLeaveRequest req) {
            if (!_dicParticipants.TryRemove(conn.ClientMemberID, out ParticipantInfo info))
                throw new InvalidOperationException("Cannot leave - not in channel");

            BroadcastNotification(new VCParticipantExitedNotification() {
                ChannelID = _nChannelID,
                ParticipantPublicKey = conn.ClientPublicKey.ToArray()
            });

            return new GenericSuccessResponse();
        }

        private class ParticipantInfo {
            public VCSubchannelMask TransmittingOn;
            public VCSubchannelMask ReceivingOn;
            public ClientConnection Connection;
            public object TransmitStartStopMutex = new object();
            public object ReceiveStartStopMutex = new object();

            public VCParticpant ToStruct() => new VCParticpant() {
                PublicKey = Connection.ClientPublicKey.ToArray(),
                TransmittingOn = TransmittingOn,
                ReceivingOn = ReceivingOn
            };
        }

        private void BroadcastNotification(Core.Protocol.NotificationMessage msg)
            => _dicParticipants.Values.AsParallel().ForAll(p => p.Connection.Connection.SendNotification(msg));

        private long CheckMemberID(ClientConnection conn) {
            long nMemberID = conn.ClientMemberID;
            if (nMemberID == -1) throw new UnauthorizedAccessException();
            return nMemberID;
        }

        private ParticipantInfo GetParticipant(long nMemberID) {
            ParticipantInfo info;
            if (!_dicParticipants.TryGetValue(nMemberID, out info))
                throw new InvalidOperationException("Not in this voice channel");
            return info;
        }

        private ParticipantInfo GetParticipant(ClientConnection conn)
            => GetParticipant(CheckMemberID(conn));
    }
}
