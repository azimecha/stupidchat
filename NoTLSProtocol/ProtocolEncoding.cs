using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Core.Protocol {
    public static class MessageCoding {
        private static readonly Newtonsoft.Json.JsonSerializer SERIALIZER 
            = new Newtonsoft.Json.JsonSerializer() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto };

        public static string EncodeObject(object obj) {
            using (StringWriter writerString = new System.IO.StringWriter()) {
                using (Newtonsoft.Json.JsonTextWriter writerJSON = new Newtonsoft.Json.JsonTextWriter(writerString))
                    SERIALIZER.Serialize(writerJSON, obj);
                return writerString.ToString();
            }
        }

        public static T DecodeObject<T>(string str) {
            using (StringReader readerString = new System.IO.StringReader(str))
            using (Newtonsoft.Json.JsonTextReader readerJSON = new Newtonsoft.Json.JsonTextReader(readerString))
                return SERIALIZER.Deserialize<T>(readerJSON);
        }

        private static readonly int PROCESS_ID = System.Diagnostics.Process.GetCurrentProcess().Id;

        public static unsafe byte[] EncryptString(string str, ReadOnlySpan<byte> spanKey, ref long nNonceCounter) {
            byte[] arrPlaintext = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] arrEncryptedMessage = new byte[sizeof(EncryptedMessageHeader) + arrPlaintext.Length];

            fixed (byte* pEncryptedMessage = arrEncryptedMessage) {
                EncryptedMessageHeader* pHeader = (EncryptedMessageHeader*)pEncryptedMessage;
                byte* pCiphertext = pEncryptedMessage + sizeof(EncryptedMessageHeader);

                pHeader->DataSize = arrPlaintext.Length;
                pHeader->Nonce.Time = DateTime.Now.ToBinary();
                pHeader->Nonce.Process = PROCESS_ID;
                pHeader->Nonce.Thread = Thread.CurrentThread.ManagedThreadId;
                pHeader->Nonce.Counter = Interlocked.Increment(ref nNonceCounter);

                Monocypher.Monocypher.crypto_lock(
                    new Span<byte>(pHeader->MAC, ProtocolConstants.MAC_SIZE),
                    new Span<byte>(pCiphertext, arrPlaintext.Length),
                    spanKey,
                    new Span<byte>((byte*)&pHeader->Nonce, ProtocolConstants.NONCE_SIZE),
                    arrPlaintext);
            }

            return arrEncryptedMessage;
        }

        public static unsafe string DecryptString(EncryptedMessageHeader header, ReadOnlySpan<byte> spanCiphertext, ReadOnlySpan<byte> spanKey) {
            int nResult;
            byte[] arrPlaintext;

            fixed (byte* pCiphertext = spanCiphertext) {

                if ((header.DataSize + sizeof(EncryptedMessageHeader)) > spanCiphertext.Length)
                    throw new FormatException($"Invalid message: data length {header.DataSize} exceeds span size {spanCiphertext.Length}");
                arrPlaintext = new byte[header.DataSize];

                nResult = Monocypher.Monocypher.crypto_unlock(arrPlaintext, spanKey,
                    new Span<byte>((byte*)&header.Nonce, ProtocolConstants.NONCE_SIZE),
                    new Span<byte>(header.MAC, ProtocolConstants.MAC_SIZE),
                    spanCiphertext);
            }

            if (nResult == -1)
                throw new FormatException("Unable to decrypt data");

            return Encoding.UTF8.GetString(arrPlaintext);
        }

        public static void WriteMessage(Stream stm, MessageContainer mc, ReadOnlySpan<byte> spanKey, ref long nNonceCounter) {
            string strEncoded = EncodeObject(mc);
            byte[] arrEncrypted = EncryptString(strEncoded, spanKey, ref nNonceCounter);
            stm.Write(arrEncrypted, 0, arrEncrypted.Length);
        }

        public static unsafe MessageContainer ReadMessage(Stream stm, ReadOnlySpan<byte> spanKey, CancellationToken? ct = null) {
            try {
                byte[] arrHeader = new byte[sizeof(EncryptedMessageHeader)];
                EncryptedMessageHeader header;
                byte[] arrCiphertext;

                stm.ReadAsync(arrHeader, 0, arrHeader.Length)?.WaitOrError(ct);
                fixed (byte* pHeader = arrHeader)
                    header = *(EncryptedMessageHeader*)pHeader;

                arrCiphertext = new byte[header.DataSize];
                stm.ReadAsync(arrCiphertext, 0, arrCiphertext.Length)?.WaitOrError(ct);

                string strEncoded = DecryptString(header, arrCiphertext, spanKey);
                return DecodeObject<MessageContainer>(strEncoded);
            } catch (TimeoutException) {
                return null;
            } catch (OperationCanceledException) {
                return null;
            }
        }

        private static T WaitOrError<T>(this Task<T> task, CancellationToken? ct = null) {
            if (ct is null)
                task.Wait();
            else
                task.Wait(ct.Value);
            
            switch (task.Status) {
                case TaskStatus.RanToCompletion:
                    return task.Result;

                case TaskStatus.Canceled:
                    throw new OperationCanceledException("Task cancelled");

                case TaskStatus.Faulted:
                    throw task.Exception;

                default:
                    throw new TimeoutException("Cancelled by token");
            }
        }
    }
}
