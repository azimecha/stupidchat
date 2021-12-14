using System;
using System.Collections.Generic;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    public static class SignedStructSerializer {
        public struct SignedData {
            public byte[] Data;
            public byte[] Signature;
        }

        private static readonly Cryptography.ISigningAlgorithm _algoSign = new Cryptography.RFC8032Algorithm();

        public static SignedData Serialize(object objStructure, ReadOnlySpan<byte> spanPublicKey, ReadOnlySpan<byte> spanPrivateKey) {
            SignedData data = new SignedData();
            data.Data = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(objStructure));
            data.Signature = _algoSign.Sign(spanPrivateKey, spanPublicKey, data.Data);
            return data;
        }

        public static unsafe T Deserialize<T>(ReadOnlySpan<byte> spanData, ReadOnlySpan<byte> spanSignature, ReadOnlySpan<byte> spanPublicKey) {
            _algoSign.Check(spanData, spanSignature, spanPublicKey);

            fixed (byte* pData = spanData)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(pData, spanData.Length));
        }
    }
}
