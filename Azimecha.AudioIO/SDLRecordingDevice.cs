using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    internal class SDLRecordingDevice : SDLAudioDevice, IRecordingDevice {
        private BlockingCollection<byte[]> _collQueue;

        private const int MAX_QUEUED_PACKETS = 1024;

        public SDLRecordingDevice(string strName) : base(strName) {
            _collQueue = new BlockingCollection<byte[]>(MAX_QUEUED_PACKETS);
        }

        protected override bool IsCaptureDevice => true;

        public Task<byte[]> ReadData(CancellationToken ct = default)
            => Task.Run(() => _collQueue.Take(ct));

        protected override void AudioCallback(Span<byte> spanBuffer) {
            byte[] arrBuffer = spanBuffer.ToArray();

            // if it fills up we discard from the front
            while (!_collQueue.TryAdd(arrBuffer)) {
                byte[] arrDiscard;
                _collQueue.TryTake(out arrDiscard);
            }
        }

        protected override void Dispose(bool bDisposing) {
            Interlocked.Exchange(ref _collQueue, null)?.Dispose();
            base.Dispose(bDisposing);
        }
    }
}
