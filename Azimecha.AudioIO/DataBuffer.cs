using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    // designed for one reader thread only!
    public class DataBuffer : IDisposable {
        private BlockingCollection<byte[]> _collQueue;
        private Memory<byte>? _memCurrent;

        public DataBuffer(int nMaxBlocksInQueue = int.MaxValue) {
            MaxBlocksInQueue = nMaxBlocksInQueue;
            _collQueue = new BlockingCollection<byte[]>(nMaxBlocksInQueue);
        }

        public int MaxBlocksInQueue { get; private set; }

        public unsafe void Dequeue(Span<byte> spanReadInto) {
            while (spanReadInto.Length > 0) {
                if (!_memCurrent.HasValue)
                    _memCurrent = new Memory<byte>(_collQueue.Take());

                int nBytesToCopy = (spanReadInto.Length < _memCurrent.Value.Length) ? spanReadInto.Length : _memCurrent.Value.Length;
                _memCurrent.Value.Span.Slice(0, nBytesToCopy).CopyTo(spanReadInto.Slice(0, nBytesToCopy));

                _memCurrent = _memCurrent.Value.Slice(nBytesToCopy);
                spanReadInto = spanReadInto.Slice(nBytesToCopy);

                if (_memCurrent.HasValue && (_memCurrent.Value.Length == 0))
                    _memCurrent = null;
            }
        }

        public void Enqueue(ReadOnlySpan<byte> spanData)
            => _collQueue.Add(spanData.ToArray());

        public void Dispose() {
            Interlocked.Exchange(ref _collQueue, null)?.Dispose();
            _memCurrent = null;
        }
    }
}
