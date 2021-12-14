using Azimecha.Stupidchat.Core.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.Core.Networking {
    public class SigningChallengeDecorator : MessageConnectionDecorator {
        private byte[] _arrTheirPublicKey;
        private byte[] _arrMyPrivateKey;
        private byte[] _arrMyPublicKey;
        private bool _bInitialized;
        private ISigningAlgorithm _algo;

        public SigningChallengeDecorator(IMessageConnection conn, bool bDisposeConnection, ISigningAlgorithm algo, ReadOnlySpan<byte> spanMyPrivateKey) 
            : base(conn, bDisposeConnection) 
        {
            _algo = algo;
            _arrMyPrivateKey = spanMyPrivateKey.ToArray();
            _arrMyPublicKey = _algo.GetPublicKey(spanMyPrivateKey);
        }

        public ReadOnlySpan<byte> TheirPublicKey => new ReadOnlySpan<byte>(_arrTheirPublicKey);

        public override void Initialize() => Initialize(null);
        public override void Initialize(CancellationToken ct) => Initialize(ct);

        private const int INIT_TXRX_TIMEOUT = int.MaxValue;

        private void Initialize(CancellationToken? ct) {
            if (_bInitialized) return;
            base.Initialize();

            try {
                _arrTheirPublicKey = Exchange(Connection, _arrMyPublicKey, INIT_TXRX_TIMEOUT, ct);

                byte[] arrChallengeForThem = CryptoUtils.GenerateNonce();
                byte[] arrChallengeForMe = Exchange(Connection, arrChallengeForThem, INIT_TXRX_TIMEOUT, ct);

                byte[] arrMySignature = _algo.Sign(_arrMyPrivateKey, _arrMyPublicKey, arrChallengeForMe);
                byte[] arrTheirSignature = Exchange(Connection, arrMySignature, INIT_TXRX_TIMEOUT, ct);
                _algo.Check(arrChallengeForThem, arrTheirSignature, _arrTheirPublicKey);

            } catch (Exception ex) {
                Debug.WriteLine($"{ex.GetType().FullName} while performing signature challenge! Disposing.");
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
