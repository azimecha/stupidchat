using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    public class WindowsRandomNumberGenerator : Core.Cryptography.IRandomNumberGenerator {
        [DllImport("advapi32", SetLastError = true, EntryPoint = "SystemFunction036")]
        [return:MarshalAs(UnmanagedType.I1)]
        private static unsafe extern bool RtlGenRandom(void* pBuffer, uint nBufferSize);

        void Core.Cryptography.IRandomNumberGenerator.Fill(Span<byte> spanToRandomize)
            => Fill(spanToRandomize);

        public static unsafe void Fill(Span<byte> spanToRandomize) {
            fixed (byte* pBuffer = spanToRandomize) {
                if (!RtlGenRandom(pBuffer, (uint)spanToRandomize.Length))
                    throw new Win32Exception();
            }
        }
    }
}
