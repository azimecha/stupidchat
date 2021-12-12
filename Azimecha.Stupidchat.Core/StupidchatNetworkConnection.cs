using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Azimecha.Stupidchat.Core.Networking;

namespace Azimecha.Stupidchat.Core {
    // facade for managing the pipeline
    public class StupidchatNetworkConnection : IMessageConnection {
        private TcpClient _client;
        private bool _bDisposeTCPClient;
        private NetworkStream _stream;
        private StreamMessageAdaptor _mcBase;
        private AsymmetricKeyExchangeDecorator _mcAsymmetric;
        private SymmetricEncryptionDecorator _mcSymmetric;

        public StupidchatNetworkConnection(TcpClient client, bool bDisposeTCPClient, ReadOnlySpan<byte> spanPrivateKey) {
            _client = client;
            _bDisposeTCPClient = bDisposeTCPClient;

            _stream = client.GetStream();
            _mcBase = new StreamMessageAdaptor(_stream, false);
            _mcAsymmetric = new AsymmetricKeyExchangeDecorator(_mcBase, false, new Cryptography.X25519Cipher(), spanPrivateKey);
        }

        public static int PrivateKeySize => new Cryptography.X25519Cipher().PrivateKeySize;

        private IMessageConnection Frontend => (IMessageConnection)_mcSymmetric ?? (IMessageConnection)_mcAsymmetric;

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
            CreateSymmetricDecorator();
        }

        public void Initialize(CancellationToken ct) {
            _mcAsymmetric.Initialize(ct);
            CreateSymmetricDecorator();
        }

        private void CreateSymmetricDecorator() {
            _mcSymmetric = new SymmetricEncryptionDecorator(_mcAsymmetric, false, new Cryptography.RFC8439Cipher(), _mcAsymmetric.SymmetricKey);
        }

        public byte[] ReceiveMesssage() => Frontend.ReceiveMesssage();
        public byte[] ReceiveMesssage(CancellationToken ct) => Frontend.ReceiveMesssage(ct);
        public void SendMessage(ReadOnlySpan<byte> spanData) => Frontend.SendMessage(spanData);
    }
}
