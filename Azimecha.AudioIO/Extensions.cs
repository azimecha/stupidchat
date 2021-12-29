using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.AudioIO {
    public static class Extensions {
        public static unsafe void EnqueueData<T>(this IPlaybackDevice dev, ReadOnlySpan<T> spanData) where T : unmanaged {
            fixed (T* pData = spanData)
                dev.EnqueueData(new ReadOnlySpan<byte>((byte*)pData, spanData.Length * sizeof(T)));
        }

        public static void EnqueueData<T>(this IPlaybackDevice dev, Span<T> spanData) where T : unmanaged
            => EnqueueData(dev, (ReadOnlySpan<T>)spanData);

        public static void EnqueueData<T>(this IPlaybackDevice dev, T[] arrData) where T : unmanaged
            => EnqueueData(dev, new ReadOnlySpan<T>(arrData));
    }
}
