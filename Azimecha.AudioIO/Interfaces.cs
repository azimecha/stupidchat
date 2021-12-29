using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    public interface IAudioDevice : IDisposable {
        void Start(SampleFormat format, int nSampleRate, int nChannels, int nSamplesPerChunk, AllowedChanges allowed = AllowedChanges.None);
        bool Paused { get; set; }

        SampleFormat Format { get; }
        int SampleRate { get; }
        int Channels { get; }
        int SamplesPerChunk { get; }
    }

    public interface IPlaybackDevice : IAudioDevice {
        void EnqueueData(ReadOnlySpan<byte> spanData);
    }

    public interface IRecordingDevice : IAudioDevice {
        Task<byte[]> ReadData(CancellationToken ct = default);
    }

    public interface IDeviceFactory<D> where D : IAudioDevice {
        string Name { get; }
        D CreateDevice();
    }
}
