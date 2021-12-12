using System;
using System.Threading.Tasks;
using Monocypher;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Core.Protocol;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace Azimecha.Stupidchat.Core.Protocol {
    public class ProtocolConnection : IDisposable {
        TcpClient _tcp;
        NetworkStream _stmClient;
        CancellationTokenSource _ctsStop;
        WeakReference<INotificationProcessor> _cbNotification;
        WeakReference<IRequestProcessor> _cbRequest;
        WeakReference<IErrorProcessor> _cbError;
        Exception _exFatal;
        byte[] _arrKey;
        long _nNonceCounter;
        object _objWriteMutex;
        Dictionary<long, TaskCompletionSource<ResponseMessage>> _dicOutstandingRequests;
        long _nRequestCounter;
        bool _bOwnsClient;
        int _bDisposed;

        public ProtocolConnection(TcpClient tcp, byte[] arrKey, bool bOwnsClient = true) {
            _tcp = tcp;
            _stmClient = tcp.GetStream();
            _arrKey = arrKey;
            _objWriteMutex = new object();
            _dicOutstandingRequests = new Dictionary<long, TaskCompletionSource<ResponseMessage>>();
            _bOwnsClient = bOwnsClient;
            _bDisposed = 0;
        }

        public INotificationProcessor NotificationProcessor {
            get => _cbNotification?.GetValue();
            set => _cbNotification = new WeakReference<INotificationProcessor>(value);
        }

        public IErrorProcessor ErrorProcessor {
            get => _cbError?.GetValue();
            set => _cbError = new WeakReference<IErrorProcessor>(value);
        }

        public IRequestProcessor RequestProcessor {
            get => _cbRequest?.GetValue();
            set => _cbRequest = new WeakReference<IRequestProcessor>(value);
        }

        public void PerformRequest(RequestMessage msgRequest)
            => IssueRequest(msgRequest).GetAwaiter().GetResult();

        public Task<ResponseMessage> IssueRequest(RequestMessage msgRequest) {
            TaskCompletionSource<ResponseMessage> tcsResponse = new TaskCompletionSource<ResponseMessage>();
            msgRequest.ID = Interlocked.Increment(ref _nRequestCounter);

            MessageContainer mc = new MessageContainer() { 
                Message = msgRequest, 
                SentAt = DateTime.Now.ToBinary() 
            };

            lock (_dicOutstandingRequests)
                _dicOutstandingRequests.Add(msgRequest.ID, tcsResponse);

            lock (_objWriteMutex)
                MessageCoding.WriteMessage(_stmClient, mc, _arrKey, ref _nNonceCounter);

            return tcsResponse.Task;
        }

        private static void RecvThreadProc(object objClient) {
            ProtocolConnection client = (ProtocolConnection)objClient;
            WeakReference<ProtocolConnection> weakClient = new WeakReference<ProtocolConnection>((ProtocolConnection)objClient);
            NetworkStream stmClient = client._stmClient;
            CancellationToken ct = client._ctsStop.Token;
            byte[] arrKey = client._arrKey;
            client = null; objClient = null;

            try {
                try {
                    MessageContainer mc = MessageCoding.ReadMessage(stmClient, arrKey, ct);
                    if (mc is null) 
                        return; // cancelled
                    else if (mc.Message is ResponseMessage)
                        weakClient.GetValue()?.AcceptResponse((ResponseMessage)mc.Message);
                    else if (mc.Message is NotificationMessage)
                        weakClient.GetValue()?.AcceptNotification((NotificationMessage)mc.Message);
                    else if (mc.Message is RequestMessage)
                        weakClient.GetValue()?.AcceptRequest((RequestMessage)mc.Message);
                    else
                        throw new InvalidOperationException($"Invalid message type {mc.Message.GetType().FullName}");
                } catch (Exception ex) {
                    bool bContinue = weakClient.GetValue()?._cbError?.GetValue()?.ProcessError(ex) ?? false;
                    if (!bContinue) throw;
                }
            } catch (Exception exFatal) {
                client = weakClient.GetValue();
                if (!(client is null))
                    client._exFatal = exFatal;
                client = null;
            }
        }

        private void AcceptResponse(ResponseMessage msg) {
            lock (_dicOutstandingRequests) {
                if (_dicOutstandingRequests.ContainsKey(msg.RequestID)) {
                    if (msg is ErrorResponse)
                        _dicOutstandingRequests[msg.RequestID].SetException(new RemoteErrorException((ErrorResponse)msg));
                    else
                        _dicOutstandingRequests[msg.RequestID].SetResult(msg);
                    _dicOutstandingRequests.Remove(msg.RequestID);
                } else
                    Debug.WriteLine($"[{nameof(ProtocolConnection)}] [{nameof(AcceptResponse)}] No outstanding request with ID {msg.RequestID}");
            }
        }

        private void AcceptRequest(RequestMessage msg) {

        }

        private void AcceptNotification(NotificationMessage msg) {
            INotificationProcessor np = _cbNotification?.GetValue();
            if (np is null)
                Debug.WriteLine($"[{nameof(ProtocolConnection)}] [{nameof(AcceptNotification)}] No notification handler, discarding {msg.GetType().FullName}");
            else
                np.ProcessNotification(msg);
        }

        private bool HandleError(Exception ex) {
            IErrorProcessor ep = _cbError?.GetValue();
            if (ep is null) {
                Debug.WriteLine($"[{nameof(ProtocolConnection)}] [{nameof(HandleError)}] No error handler, {ex.GetType().FullName} will be fatal");
                return false;
            } else
                return ep.ProcessError(ex);
        }

        public void Dispose() {
            if (Interlocked.CompareExchange(ref _bDisposed, 1, 0) == 0) {
                _ctsStop.Cancel();
                if (_bOwnsClient) {
                    _stmClient.Dispose();
                    _tcp.Dispose();
                }
            }
        }
    }

    public interface INotificationProcessor {
        void ProcessNotification(NotificationMessage msgNotif);
    }

    public interface IRequestProcessor {
        void ProcessRequest(RequestMessage msgReq);
    }

    public interface IErrorProcessor {
        bool ProcessError(Exception ex);
    }
}
