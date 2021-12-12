using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Azimecha.Stupidchat.Core.Networking {
    public class StreamMessageAdaptor : IMessageConnection {
        private Stream _stmInner;
        private bool _bOwnsStream;
        private SemaphoreSlim _semReceiving, _semSending;

        public StreamMessageAdaptor(Stream stm, bool bDisposeStream) {
            _stmInner = stm;
            _bOwnsStream = bDisposeStream;
            _semReceiving = new SemaphoreSlim(1, 1);
            _semSending = new SemaphoreSlim(1, 1);
            MaximumMessageSize = long.MaxValue;
        }

        public void Initialize() { }
        public void Initialize(CancellationToken ct) { }

        public long MaximumMessageSize { get; set; }

        public void Dispose() {
            if (_bOwnsStream) {
                Stream stm = Interlocked.Exchange(ref _stmInner, null);
                stm?.Dispose();
            }
        }

        public unsafe byte[] ReceiveMesssage() {
            _semReceiving.Wait();
            try {
                byte[] arrMessageSize = new byte[sizeof(long)];
                _stmInner.ReadExactly(arrMessageSize, 0, arrMessageSize.Length);

                long nMessageSize = 0;
                fixed (byte* pMessageSize = arrMessageSize)
                    nMessageSize = *(long*)pMessageSize;

                if (nMessageSize > MaximumMessageSize)
                    throw new MessageFramingException($"Received message size {nMessageSize} exceeds maximum of {MaximumMessageSize}");

                byte[] arrMessage = new byte[nMessageSize];
                _stmInner.ReadExactly(arrMessage, 0, arrMessage.LongLength);

                return arrMessage;
            } finally {
                _semReceiving.Release();
            }
        }

        public byte[] ReceiveMesssage(CancellationToken ct) {
            _semReceiving.Wait(ct);
            try {
                byte[] arrMessageSize = new byte[sizeof(long)];
                _stmInner.ReadExactly(arrMessageSize, 0, arrMessageSize.Length, ct);

                long nMessageSize = BitConverter.ToInt64(arrMessageSize, 0);
                if (nMessageSize > MaximumMessageSize)
                    throw new MessageFramingException($"Received message size {nMessageSize} exceeds maximum of {MaximumMessageSize}");

                byte[] arrMessage = new byte[nMessageSize];
                try {
                    _stmInner.ReadExactly(arrMessage, 0, arrMessage.LongLength, ct);
                } catch (Exception ex) {
                    Debug.WriteLine($"{ex.GetType().FullName} in {nameof(ReceiveMesssage)} causes message stream to enter invalid state! Disposing.");
                    Dispose();
                    throw;
                }

                return arrMessage;
            } finally {
                _semReceiving.Release();
            }
        }

        public void SendMessage(ReadOnlySpan<byte> spanData) {
            if (spanData.Length > MaximumMessageSize)
                throw new MessageFramingException($"Sent message length {spanData.Length} exceeds maximum of {MaximumMessageSize}");

            _semSending.Wait();
            try {
                _stmInner.Write(BitConverter.GetBytes((long)spanData.Length), 0, sizeof(long));

                try {
                    _stmInner.Write(spanData.ToArray(), 0, spanData.Length);
                } catch (Exception ex) {
                    Debug.WriteLine($"{ex.GetType().FullName} in {nameof(SendMessage)} causes message stream to enter invalid state! Disposing.");
                    Dispose();
                    throw;
                }
            } finally {
                _semSending.Release();
            }
        }
    }

    public class MessageFramingException : Exception {
        public MessageFramingException(string strMessage) : base(strMessage) { }
        public MessageFramingException(string strMessage, Exception exInner) : base(strMessage, exInner) { }
    }
}
