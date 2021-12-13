using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azimecha.Stupidchat.Core.Cryptography;
using Azimecha.Stupidchat.Core.Networking;

namespace Azimecha.Stupidchat.Core.Networking {
    public class AsymmetricKeyExchangeDecorator : MessageConnectionDecorator {
        private byte[] _arrMyPrivateKey;
        private byte[] _arrMyPublicKey;
        private byte[] _arrTheirPublicKey;
        private byte[] _arrSymmetricKey;
        private bool _bInitialized;
        private IAsymmetricCipher _cipher;

        public AsymmetricKeyExchangeDecorator(IMessageConnection conn, bool bDisposeConnection, IAsymmetricCipher cipherAsym, 
            ReadOnlySpan<byte> spanPrivateKey) : base(conn, bDisposeConnection)
        {
            _cipher = cipherAsym;
            _arrMyPrivateKey = spanPrivateKey.ToArray();
            _arrMyPublicKey = _cipher.GetPublicKey(_arrMyPrivateKey);
        }

        public ReadOnlySpan<byte> MyPrivateKey => new ReadOnlySpan<byte>(_arrMyPrivateKey);
        public ReadOnlySpan<byte> MyPublicKey => new ReadOnlySpan<byte>(_arrMyPublicKey);
        public ReadOnlySpan<byte> TheirPublicKey => new ReadOnlySpan<byte>(_arrTheirPublicKey);
        public ReadOnlySpan<byte> SymmetricKey => new ReadOnlySpan<byte>(_arrSymmetricKey);

        public override void Initialize() => Initialize(null);
        public override void Initialize(CancellationToken ct) => Initialize(ct);

        private const int INIT_TXRX_TIMEOUT = int.MaxValue;

        private void Initialize(CancellationToken? ct) {
            if (_bInitialized)
                throw new InvalidOperationException("Connection has already been initialized");

            base.Initialize();

            try {
                Task taskSendMyPubkey = Task.Run(() => base.SendMessage(MyPublicKey));
                Task<byte[]> taskRecvTheirPubkey = Task.Run(() => ct.HasValue ? base.ReceiveMesssage(ct.Value) : base.ReceiveMesssage());

                taskSendMyPubkey.Wait(INIT_TXRX_TIMEOUT);
                taskRecvTheirPubkey.Wait(INIT_TXRX_TIMEOUT);

                taskSendMyPubkey.CheckFinished();
                taskRecvTheirPubkey.CheckFinished();

                _arrTheirPublicKey = taskRecvTheirPubkey.Result;
                _arrSymmetricKey = _cipher.PerformKeyExchange(MyPrivateKey, TheirPublicKey);

            } catch (Exception ex) {
                Debug.WriteLine($"{ex.GetType().Name} while initializing asymmetric encrypted connection! Disposing.");
                Dispose();
                throw;
            }

            _bInitialized = true;
        }

        public override byte[] ReceiveMesssage() {
            CheckInitialized();
            return base.ReceiveMesssage();
        }

        public override byte[] ReceiveMesssage(CancellationToken ct) {
            CheckInitialized();
            return base.ReceiveMesssage(ct);
        }

        public override void SendMessage(ReadOnlySpan<byte> spanData) {
            CheckInitialized();
            base.SendMessage(spanData);
        }

        private void CheckInitialized() {
            if (!_bInitialized)
                throw new InvalidOperationException("Connection must be initialized before sending or receiving messages");
        }
    }
}
