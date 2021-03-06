using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Core.Networking {
    public class MessageConnectionDecorator : IMessageConnection {
        private IMessageConnection _connInner;
        private bool _bDisposesInner;

        public MessageConnectionDecorator(IMessageConnection conn, bool bDisposeConnection) {
            _connInner = conn;
            _bDisposesInner = bDisposeConnection;
        }

        protected IMessageConnection Connection => _connInner;

        public virtual long MaximumMessageSize { 
            get => _connInner.MaximumMessageSize; 
            set => _connInner.MaximumMessageSize = value; 
        }

        public virtual void Dispose() {
            if (_bDisposesInner) {
                IMessageConnection connInner = Interlocked.Exchange(ref _connInner, null);
                connInner?.Dispose();
            }
        }

        public virtual byte[] ReceiveMesssage() => _connInner.ReceiveMesssage();
        public virtual byte[] ReceiveMesssage(CancellationToken ct) => _connInner.ReceiveMesssage(ct);
        public virtual void SendMessage(ReadOnlySpan<byte> spanData) => _connInner.SendMessage(spanData);

        public virtual void Initialize() => _connInner.Initialize();
        public virtual void Initialize(CancellationToken ct) => _connInner.Initialize(ct);

        // Sends and receives "at the same time"
        public static byte[] Exchange(IMessageConnection conn, byte[] arrSend, int nTimeout = int.MaxValue, CancellationToken? ct = null) {
            Task taskSend = Task.Run(() => conn.SendMessage(arrSend));
            Task<byte[]> taskRecv = Task.Run(() => ct.HasValue ? conn.ReceiveMesssage(ct.Value) : conn.ReceiveMesssage());

            taskSend.WaitAndDisaggregate(nTimeout: nTimeout);
            taskRecv.WaitAndDisaggregate(nTimeout: nTimeout);

            return taskRecv.Result;
        }
    }
}
