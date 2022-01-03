using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
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
                taskRead.WaitAndDisaggregate();

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

        public static void WaitAndDisaggregate(this Task task, CancellationToken? ct = null, int nTimeout = -1) {
            try {
                if (ct.HasValue) {
                    if (nTimeout >= 0)
                        task.Wait(nTimeout, ct.Value);
                    else
                        task.Wait(ct.Value);
                } else
                    task.Wait();
            } catch (AggregateException ex) {
                if (ex.InnerExceptions.Count == 1)
                    throw ex.InnerExceptions[0];
                else
                    throw;
            }
        }

        public static void WaitAndDisaggregate<T>(this Task<T> task, CancellationToken? ct = null, int nTimeout = -1) {
            try {
                if (ct.HasValue) {
                    if (nTimeout >= 0)
                        task.Wait(nTimeout, ct.Value);
                    else
                        task.Wait(ct.Value);
                } else {
                    if (nTimeout >= 0)
                        task.Wait(nTimeout);
                    else
                        task.Wait();
                }
            } catch (Exception ex) {
                while ((ex is AggregateException exAggregate) && (exAggregate.InnerExceptions.Count == 1))
                    ex = exAggregate.InnerExceptions[0];
                throw ex;
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

        public static string ToHexString(this ReadOnlySpan<byte> spanBytes)
            => ToHexString(spanBytes.ToArray());

        public static string ToHexString(this byte[] arrBytes)
            => ToHexString((IEnumerable<byte>)arrBytes);

        public static string ToHexString(this IEnumerable<byte> enuBytes) {
            string str = "";

            foreach (byte nCurVal in enuBytes)
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

        public static string ToDataString(this object objDataContract, string strBetweenValues = ", ", string strBetweenKeyAndValue = " = ") {
            string strCombined = "";
            bool bFirst = true;
            Type typeObj = objDataContract.GetType();

            if (typeObj.GetCustomAttribute<DataContractAttribute>() is null)
                throw new NotSupportedException($"Type {typeObj.FullName} is not a data contract");

            foreach (MemberInfo memb in typeObj.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.FlattenHierarchy)) {
                DataMemberAttribute attribDataMember = memb.GetCustomAttribute<DataMemberAttribute>();
                if (attribDataMember is null) continue;

                string strName = attribDataMember.Name ?? memb.Name;

                object objValue;
                switch (memb.MemberType) {
                    case MemberTypes.Field:
                        objValue = ((FieldInfo)memb).GetValue(objDataContract);
                        break;

                    case MemberTypes.Property:
                        objValue = ((PropertyInfo)memb).GetValue(objDataContract);
                        break;

                    default:
                        objValue = memb.MemberType;
                        break;
                }

                string strValue;
                if (objValue is null)
                    strValue = "(null)";
                else if (!(objValue.GetType().GetCustomAttribute<DataContractAttribute>() is null))
                    strValue = "[" + objValue.ToDataString() + "]";
                else if (objValue is IEnumerable<byte> enuValue)
                    strValue = enuValue.ToHexString();
                else
                    strValue = objValue.ToString();

                if (bFirst)
                    bFirst = false;
                else
                    strCombined += strBetweenValues;

                strCombined += strName;
                strCombined += strBetweenKeyAndValue;
                strCombined += strValue;
            }

            return strCombined;
        }

        // https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array/321404
        public static byte[] HexStringToBytes(string strHexDigits) {
            if ((strHexDigits.Length & 1) != 0)
                throw new ArgumentException("Hex strings must contain an even number of digits, "
                    + $"but this one contains {strHexDigits.Length}");

            byte[] arrData = new byte[strHexDigits.Length >> 1];

            for (int i = 0; i < strHexDigits.Length >> 1; ++i) {
                arrData[i] = (byte)((HexDigitToValue(strHexDigits[i << 1]) << 4) + (HexDigitToValue(strHexDigits[(i << 1) + 1])));
            }

            return arrData;
        }

        public static int HexDigitToValue(char cHexDigit) {
            int nHexVal = (int)cHexDigit;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return nHexVal - (nHexVal < 58 ? 48 : (nHexVal < 97 ? 55 : 87));
        }

        // note that this could take forever if something is adding items extremely fast
        public static void Clear<T>(this System.Collections.Concurrent.BlockingCollection<T> coll) {
            T temp;
            while (coll.TryTake(out temp)) ;
        }

        public static WeakReference<T> Weaken<T>(this T obj) where T : class
            => new WeakReference<T>(obj);
    }
}
