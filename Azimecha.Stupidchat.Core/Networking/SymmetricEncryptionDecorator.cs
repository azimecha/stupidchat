using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Azimecha.Stupidchat.Core.Cryptography;

namespace Azimecha.Stupidchat.Core.Networking {
    public class SymmetricEncryptionDecorator : MessageConnectionDecorator {
        private ISymmetricCipher _cipher;
        private byte[] _arrKey;

        public SymmetricEncryptionDecorator(IMessageConnection conn, bool bDisposeConnection, 
            ISymmetricCipher cipher, ReadOnlySpan<byte> spanKey) : base(conn, bDisposeConnection)
        {
            _cipher = cipher;
            _arrKey = spanKey.ToArray();
        }

        public byte[] Key => _arrKey;

        public override byte[] ReceiveMesssage()
            => DecryptMessage(base.ReceiveMesssage()).ToArray();

        public override byte[] ReceiveMesssage(CancellationToken ct) 
            => DecryptMessage(base.ReceiveMesssage(ct)).ToArray();

        public override void SendMessage(ReadOnlySpan<byte> spanData)
            => base.SendMessage(EncryptMessage(spanData));

        // header format: mac, nonce, ciphertext

        private byte[] EncryptMessage(ReadOnlySpan<byte> spanPlaintext) {
            byte[] arrEncrypted = new byte[_cipher.AuthenticationCodeSize + _cipher.NonceSize + spanPlaintext.Length];
            Span<byte> spanEncryptedMessage = new Span<byte>(arrEncrypted);

            Span<byte> spanAuthCode = spanEncryptedMessage.Slice(0, _cipher.AuthenticationCodeSize);
            Span<byte> spanNonce = spanEncryptedMessage.Slice(spanAuthCode.Length, _cipher.NonceSize);
            Span<byte> spanCiphertext = spanEncryptedMessage.Slice(spanAuthCode.Length + spanNonce.Length);

            CryptoUtils.GenerateNonce(spanNonce);

            spanPlaintext.CopyTo(spanCiphertext);
            _cipher.Encrypt(spanCiphertext, _arrKey, spanNonce, spanAuthCode);

            return arrEncrypted;
        }

        private Span<byte> DecryptMessage(Span<byte> spanEncryptedMessage) {
            Span<byte> spanAuthCode = spanEncryptedMessage.Slice(0, _cipher.AuthenticationCodeSize);
            Span<byte> spanNonce = spanEncryptedMessage.Slice(spanAuthCode.Length, _cipher.NonceSize);
            Span<byte> spanCiphertext = spanEncryptedMessage.Slice(spanAuthCode.Length + spanNonce.Length);

            _cipher.Decrypt(spanCiphertext, _arrKey, spanNonce, spanAuthCode);
            return spanCiphertext;
        }
    }
}
