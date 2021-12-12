using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Azimecha.Stupidchat.Core.Cryptography {
    public static class CryptoUtils {
        public static void GenerateNonce(Span<byte> spanNonceOUT) {
            byte[] arrHash = GenerateNonce();

            while (spanNonceOUT.Length > 0) {
                Span<byte> spanDest = spanNonceOUT.Slice(0, spanNonceOUT.Length > arrHash.Length ? arrHash.Length : spanNonceOUT.Length);
                ReadOnlySpan<byte> spanSrc = new ReadOnlySpan<byte>(arrHash).Slice(0, spanDest.Length);
                spanSrc.CopyTo(spanDest);
                spanNonceOUT = spanNonceOUT.Slice(spanDest.Length);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Nonce {
            public long DateTicks;
            public int ProcessID;
            public int ManagedThreadID;
            public long Counter;
        }

        private static long _nNonceCount = 0;
        private static readonly IHashAlgorithm _hash = new Blake2BHash();
        private static int _nProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;

        public static unsafe byte[] GenerateNonce()
            => _hash.Hash(new Nonce() {
                DateTicks = DateTime.Now.Ticks,
                ProcessID = _nProcessID,
                ManagedThreadID = Thread.CurrentThread.ManagedThreadId,
                Counter = Interlocked.Increment(ref _nNonceCount)
            }.Blit());
    }
}
