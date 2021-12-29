using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    internal class SDLPlaybackDevice : SDLAudioDevice, IPlaybackDevice {
        private DataBuffer _buffer;

        public SDLPlaybackDevice(string strName) : base(strName) {
            _buffer = new DataBuffer();
        }

        protected override bool IsCaptureDevice => false;

        public void EnqueueData(ReadOnlySpan<byte> spanData)
            => _buffer.Enqueue(spanData);

        protected override void AudioCallback(Span<byte> spanBuffer)
            => _buffer.Dequeue(spanBuffer);
    }
}
