using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Networking;

namespace Azimecha.Stupidchat.Core {
    // facade for managing the pipeline
    public class NetworkConnection : IMessageConnection {
        private TcpClient _client;
        private bool _bDisposeTCPClient;
        private NetworkStream _stream;
        private StreamMessageAdaptor _mcBase;
        private AsymmetricKeyExchangeDecorator _mcAsymmetric;
        private SymmetricEncryptionDecorator _mcSymmetric;
        private SigningChallengeDecorator _mcSigning;
        private byte[] _arrConnectionKey;
        private Cryptography.ISigningAlgorithm _algoSigning;

        public NetworkConnection(TcpClient client, bool bDisposeTCPClient, ReadOnlySpan<byte> spanPrivateKey) {
            _client = client;
            _bDisposeTCPClient = bDisposeTCPClient;
            _algoSigning = new Cryptography.RFC8032Algorithm();
            _arrConnectionKey = new byte[_algoSigning.PrivateKeySize];
            new Cryptography.Dotnet6CryptoRNG().Fill(_arrConnectionKey);

            _stream = client.GetStream();
            _mcBase = new StreamMessageAdaptor(_stream, false);
            _mcAsymmetric = new AsymmetricKeyExchangeDecorator(_mcBase, false, new Cryptography.X25519Cipher(), spanPrivateKey);
        }

        public static Cryptography.ISigningAlgorithm CreateSigningAlgorithmInstance()
            => new Cryptography.RFC8032Algorithm();

        private IMessageConnection Frontend => (IMessageConnection)_mcSigning ?? (IMessageConnection)_mcAsymmetric;

        public ReadOnlySpan<byte> PartnerPublicKey => _mcSigning.TheirPublicKey;

        public long MaximumMessageSize { 
            get => Frontend.MaximumMessageSize;
            set => Frontend.MaximumMessageSize = value;
        }

        public void Dispose() {
            Interlocked.Exchange(ref _mcSymmetric, null)?.Dispose();
            Interlocked.Exchange(ref _mcAsymmetric, null)?.Dispose();
            Interlocked.Exchange(ref _mcBase, null)?.Dispose();

            if (_bDisposeTCPClient) {
                Interlocked.Exchange(ref _client, null)?.Dispose();
                Interlocked.Exchange(ref _stream, null)?.Dispose();
            }
        }

        public void Initialize() {
            _mcAsymmetric.Initialize();
            CreateAdditionalDecorators();
            _mcSymmetric.Initialize();
            _mcSigning.Initialize();
        }

        public void Initialize(CancellationToken ct) {
            _mcAsymmetric.Initialize(ct);
            CreateAdditionalDecorators();
            _mcSymmetric.Initialize(ct);
            _mcSigning.Initialize(ct);
        }

        private void CreateAdditionalDecorators() {
            _mcSymmetric = new SymmetricEncryptionDecorator(_mcAsymmetric, false, new Cryptography.XChaCha20Poly1305Cipher(), _mcAsymmetric.SymmetricKey);
            _mcSigning = new SigningChallengeDecorator(_mcSymmetric, false, new Cryptography.RFC8032Algorithm(), _arrConnectionKey);
        }

        public byte[] ReceiveMesssage() => Frontend.ReceiveMesssage();
        public byte[] ReceiveMesssage(CancellationToken ct) => Frontend.ReceiveMesssage(ct);
        public void SendMessage(ReadOnlySpan<byte> spanData) => Frontend.SendMessage(spanData);
    }
}
