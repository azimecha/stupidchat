using Azimecha.Stupidchat.Core.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Core {
    public class ProtocolConnection : IDisposable, IDisposalObserver<NetworkConnection> {
        private NetworkConnection _conn;
        private WeakReference<IRequestProcessor> _cbRequest;
        private WeakReference<INotificationProcessor> _cbNotif;
        private WeakReference<IErrorProcessor> _cbError;
        private WeakReference<IDisposalObserver<ProtocolConnection>> _cbDisposed;
        private CancellationTokenSource _ctsDisposed;
        private IDictionary<long, TaskCompletionSource<Protocol.ResponseMessage>> _dicOutstandingRequests;
        private Thread _thdReceive;
        private long _nRequestID;
        private bool _bStarted;
        private object _objStartMutex;

        public ProtocolConnection(TcpClient client, bool bDisposeTCPClient, ReadOnlySpan<byte> spanPrivateKey) {
            _conn = new NetworkConnection(client, bDisposeTCPClient, spanPrivateKey);
            _conn.DisposalObserver = this;

            _dicOutstandingRequests = new Dictionary<long, TaskCompletionSource<Protocol.ResponseMessage>>();
            _ctsDisposed = new CancellationTokenSource();
            _objStartMutex = new object();

            _thdReceive = new Thread(ReceiveThreadProc);
            _thdReceive.Name = "Receive Thread";
        }

        public void Start() {
            lock (_objStartMutex) {
                if (_bStarted)
                    throw new InvalidOperationException("Connection has already been started");

                _conn.Initialize();
                _thdReceive.Start(new WeakReference<ProtocolConnection>(this));

                _bStarted = true;
            }
        }

        public ReadOnlySpan<byte> PartnerPublicKey => _conn.PartnerPublicKey;

        public Task<Protocol.ResponseMessage> IssueRequest(Protocol.RequestMessage msgRequest) {
            msgRequest.ID = Interlocked.Increment(ref _nRequestID);

            TaskCompletionSource<Protocol.ResponseMessage> tcsResponse = new TaskCompletionSource<Protocol.ResponseMessage>();
            lock (_dicOutstandingRequests)
                _dicOutstandingRequests.Add(msgRequest.ID, tcsResponse);

            try {
                _conn.SendMessage(Encoding.UTF8.GetBytes(msgRequest.Serialize()));
            } catch (Exception) {
                _dicOutstandingRequests.Remove(msgRequest.ID);
                throw;
            }

            return tcsResponse.Task;
        }

        public Protocol.ResponseMessage PerformRequest(Protocol.RequestMessage msgRequest) {
            Task<Protocol.ResponseMessage> taskResp = IssueRequest(msgRequest);
            taskResp.WaitAndDisaggregate();
            return taskResp.Result;
        }

        public TResp PerformRequest<TResp>(Protocol.RequestMessage msgRequest) where TResp : Protocol.ResponseMessage
            => (TResp)PerformRequest(msgRequest);

        public void SendNotification(Protocol.NotificationMessage msgNotif) {
            _conn.SendMessage(Encoding.UTF8.GetBytes(msgNotif.Serialize()));
        }

        public void SendErrorNotification(Exception ex)
            => SendNotification(new Protocol.ErrorNotification() { Summary = ex.Message, Description = ex.ToString() });

        public void Dispose() {
            _ctsDisposed.Cancel();
            Interlocked.Exchange(ref _conn, null)?.Dispose();
            Interlocked.Exchange(ref _cbDisposed, null)?.GetValue()?.OnObjectDisposed(this);
        }

        public interface IRequestProcessor {
            Protocol.ResponseMessage ProcessRequest(Protocol.RequestMessage msgRequest);
        }

        public interface INotificationProcessor {
            void ProcessNotification(Protocol.NotificationMessage msgNotification);
        }

        public interface IErrorProcessor {
            void ProcessError(Exception ex);
        }

        // note that these observers may be called on threads other than the main thread

        public IRequestProcessor RequestProcessor {
            get => _cbRequest?.GetValue();
            set => _cbRequest = new WeakReference<IRequestProcessor>(value);
        }

        public INotificationProcessor NotificationProcessor {
            get => _cbNotif?.GetValue();
            set => _cbNotif = new WeakReference<INotificationProcessor>(value);
        }

        public IErrorProcessor ErrorProcessor {
            get => _cbError?.GetValue();
            set => _cbError = new WeakReference<IErrorProcessor>(value);
        }

        public IDisposalObserver<ProtocolConnection> DisposalObserver {
            get => _cbDisposed?.GetValue();
            set => _cbDisposed = new WeakReference<IDisposalObserver<ProtocolConnection>>(value);
        }

        private static void ReceiveThreadProc(object objWeakConnection) {
            WeakReference<ProtocolConnection> conn = (WeakReference<ProtocolConnection>)objWeakConnection;

            try {
                CancellationToken? ctDisposed = conn.GetValue()?._ctsDisposed.Token;

                while (!(ctDisposed?.IsCancellationRequested ?? true)) {
                    try {
                        try {
                            byte[] arrMessage = conn.GetValue()?._conn.ReceiveMesssage(ctDisposed.Value);
                            if (arrMessage is null)
                                throw new OperationCanceledException();

                            Protocol.MessageContainer mc = ProtocolMessageSerializer.Deserialize(Encoding.UTF8.GetString(arrMessage));
                            conn.GetValue()?.ProcessMessage(mc.Message);

                        } catch (System.IO.IOException exIO) {
                            if (exIO.InnerException is SocketException)
                                throw exIO.InnerException;
                            else
                                throw;
                        }

                    } catch (OperationCanceledException) {
                        Debug.WriteLine($"[{nameof(ProtocolConnection)}/Thread{Thread.CurrentThread.ManagedThreadId}] Operation cancelled, exiting");
                        return;

                    } catch (SocketException exSocket) {
                        Debug.WriteLine($"[{nameof(ProtocolConnection)}/Thread{Thread.CurrentThread.ManagedThreadId}] Socket error, disposing: {exSocket}");
                        conn.GetValue()?.Dispose();
                        return;

                    } catch (System.IO.EndOfStreamException) {
                        Debug.WriteLine($"[{nameof(ProtocolConnection)}/Thread{Thread.CurrentThread.ManagedThreadId}] End of stream, disposing");
                        conn.GetValue()?.Dispose();
                        return;

                    } catch (Exception ex) {
                        IErrorProcessor cbError = conn.GetValue()?.ErrorProcessor;
                        if (cbError is null)
                            throw;
                        else
                            cbError.ProcessError(ex);
                    }
                }

            } catch (Exception ex) {
                Debug.WriteLine($"[{nameof(ProtocolConnection)}/Thread{Thread.CurrentThread.ManagedThreadId}] Fatal error in ReceiveThreadProc: {ex}");
                conn.GetValue()?.Dispose();
            }
        }

        private void ProcessMessage(Protocol.IMessage msg) {
            if (msg is Protocol.RequestMessage)
                ProcessRequest((Protocol.RequestMessage)msg);
            else if (msg is Protocol.ResponseMessage)
                ProcessResponse((Protocol.ResponseMessage)msg);
            else if (msg is Protocol.NotificationMessage)
                ProcessNotification((Protocol.NotificationMessage)msg);
            else
                throw new ProtocolMessageFormatException($"Message type {msg.GetType().FullName} is not understood by {nameof(ProtocolConnection)}");
        }

        private void ProcessRequest(Protocol.RequestMessage msgRequest) {
            try {
                IRequestProcessor cbRequest = RequestProcessor;

                if (cbRequest is null)
                    throw new NoHandlerException("Request could not be processed because there is no request processor");

                Protocol.ResponseMessage msgResponse = cbRequest.ProcessRequest(msgRequest);
                msgResponse.RequestID = msgRequest.ID;
                SendResponse(msgResponse);

            } catch (Exception ex) {
                SendResponse(new Protocol.ErrorResponse() { 
                    Description = ex.ToString(), 
                    RequestID = msgRequest.ID, 
                    Summary = ex.Message 
                });
            }
        }

        private void SendResponse(Protocol.ResponseMessage msgResponse)
            => _conn.SendMessage(Encoding.UTF8.GetBytes(msgResponse.Serialize()));

        private void ProcessResponse(Protocol.ResponseMessage msgResponse) {
            TaskCompletionSource<Protocol.ResponseMessage> tcsResponded;

            lock (_dicOutstandingRequests) {
                if (_dicOutstandingRequests.ContainsKey(msgResponse.RequestID)) {
                    tcsResponded = _dicOutstandingRequests[msgResponse.RequestID];
                    _dicOutstandingRequests.Remove(msgResponse.RequestID);
                } else
                    throw new NoHandlerException($"Request ID {msgResponse.RequestID} does not match any outstanding request");
            }

            if (msgResponse is Protocol.ErrorResponse)
                tcsResponded.SetException(new RequestFailedException((Protocol.ErrorResponse)msgResponse));
            else
                tcsResponded.SetResult(msgResponse);
        }

        private void ProcessNotification(Protocol.NotificationMessage msgNotification) {
            if (msgNotification is Protocol.ErrorNotification) {
                IErrorProcessor cbError = ErrorProcessor;

                if (cbError is null)
                    throw new NoHandlerException("No error processor for error notification");

                cbError.ProcessError(new SpontaneousRemoteException((Protocol.ErrorNotification)msgNotification));
                return;
            }

            INotificationProcessor cbNotif = NotificationProcessor;

            if (cbNotif is null)
                throw new NoHandlerException("Notification could not be processed because there is no notification processor");

            cbNotif.ProcessNotification(msgNotification);
        }

        void IDisposalObserver<NetworkConnection>.OnObjectDisposed(NetworkConnection obj) {
            Dispose();
        }
    }
}
