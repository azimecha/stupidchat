using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Core {
    public static class Utils {
        public static T GetValue<T>(this WeakReference<T> weak) where T : class {
            T obj;
            return weak.TryGetTarget(out obj) ? obj : null;
        }

        public static void ReadExactly(this Stream stm, byte[] arrBuffer, long nOffset, long nCount, CancellationToken? ct = null) {
            if ((nOffset <= (long)int.MaxValue) && (nCount <= (long)int.MaxValue)) {
                stm.ReadExactly(arrBuffer, (int)nOffset, (int)nCount, ct);
                return;
            }

            while (nCount > 0) {
                int nCurCount = (int)(nCount % int.MaxValue);
                byte[] arrCurBuffer = new byte[nCurCount];

                stm.ReadExactly(arrCurBuffer, 0, nCurCount);
                Array.Copy(arrCurBuffer, 0, arrBuffer, nOffset, nCurCount);

                nOffset += nCurCount;
                nCount += nCurCount;
            }
        }

        public static void ReadExactly(this Stream stm, byte[] arrBuffer, int nOffset, int nCount, CancellationToken? ct = null) {
            while (nCount > 0) {
                Task<int> taskRead = (ct is null) ? stm.ReadAsync(arrBuffer, nOffset, nCount) : stm.ReadAsync(arrBuffer, nOffset, nCount, ct.Value);
                taskRead.Wait();

                switch (taskRead.Status) {
                    case TaskStatus.Faulted:
                        throw taskRead.Exception;

                    case TaskStatus.Canceled:
                        throw new OperationCanceledException();
                }

                if (taskRead.Result == 0)
                    throw new EndOfStreamException($"Encountered end of stream (read result 0) with {nCount} bytes not read");

                nCount -= taskRead.Result;
                nOffset += taskRead.Result;
            }
        }

        public static void CheckFinished(this Task taskCheck) {
            switch (taskCheck.Status) {
                case TaskStatus.Faulted:
                    throw taskCheck.Exception;

                case TaskStatus.Canceled:
                    throw new OperationCanceledException();

                case TaskStatus.Running:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.WaitingForChildrenToComplete:
                    throw new InvalidOperationException("Task still running");
            }
        }

        public static void CheckFinished<T>(this Task<T> taskCheck) {
            switch (taskCheck.Status) {
                case TaskStatus.Faulted:
                    throw taskCheck.Exception;

                case TaskStatus.Canceled:
                    throw new OperationCanceledException();

                case TaskStatus.Running:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.WaitingForChildrenToComplete:
                    throw new InvalidOperationException("Task still running");
            }
        }

        public static unsafe byte[] Blit<T>(this T obj) where T : unmanaged {
            byte[] arrData = new byte[sizeof(T)];

            fixed (byte* pData = arrData)
                *(T*)pData = obj;

            return arrData;
        }

        public static unsafe void CopyMemory(byte* pSrc, byte* pDest, ulong nSize) {
            while (nSize > sizeof(ulong)) {
                *(ulong*)pDest = *(ulong*)pSrc;
                pDest += sizeof(ulong);
                pSrc += sizeof(ulong);
                nSize -= sizeof(ulong);
            }

            while (nSize > 0) {
                *pDest = *pSrc;
                pDest++; pSrc++;
                nSize--;
            }
        }

        public static string ToHexString(this ReadOnlySpan<byte> spanBytes) {
            string str = "";

            foreach (byte nCurVal in spanBytes)
                str += nCurVal.ToString("X2");

            return str;
        }

        // https://stackoverflow.com/questions/13042045/interlocked-compareexchangeint-using-greaterthan-or-lessthan-instead-of-equali
        public static bool InterlockedExchangeIfGreater(ref int nValue, int nCompareTo, int nReplaceWith) {
            int nInitialValue;
            do {
                nInitialValue = nValue;
                if (nInitialValue >= nCompareTo) return false;
            } while (Interlocked.CompareExchange(ref nValue, nReplaceWith, nInitialValue) != nInitialValue);
            return true;
        }

        public static bool InterlockedExchangeIfGreater(ref long nValue, long nCompareTo, long nReplaceWith) {
            long nInitialValue;
            do {
                nInitialValue = nValue;
                if (nInitialValue >= nCompareTo) return false;
            } while (Interlocked.CompareExchange(ref nValue, nReplaceWith, nInitialValue) != nInitialValue);
            return true;
        }
    }
}
