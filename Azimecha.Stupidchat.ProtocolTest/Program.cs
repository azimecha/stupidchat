using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Azimecha.Stupidchat.Core.Protocol;
using Azimecha.Stupidchat.ProtocolTest;

namespace Azimecha.Stupidchat.NetworkingTest {
    public static class Program {
        public static void Main(string[] arrArgs) {
            CancellationTokenSource ctsStop = new CancellationTokenSource();
            _ctStop = ctsStop.Token;

            Thread thdServer = new Thread(ServerThread);
            Thread thdClient = new Thread(ClientThread);

            thdServer.Name = "Server Thread";
            thdClient.Name = "Client Thread";

            thdServer.Start();
            thdClient.Start();

            Console.ReadKey(true);
            ctsStop.Cancel();

            thdServer.Join();
            thdClient.Join();

            GC.Collect();
        }

        private static readonly object _objConsoleMutex = new object();
        private static CancellationToken _ctStop;

        private static void ServerThread() {
            Random rand = new Random(1);
            byte[] arrPrivateKey = new byte[Core.NetworkConnection.PrivateKeySize];
            rand.NextBytes(arrPrivateKey);

            Core.ProtocolConnection.IRequestProcessor rp = new ServerRequestProcessor();
            Core.ProtocolConnection.IErrorProcessor ep = new ErrorProcessor();

            TcpListener listener = new TcpListener(IPAddress.Loopback, 22200);
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();
            using (Core.ProtocolConnection conn = new Core.ProtocolConnection(client, true, arrPrivateKey)) {
                conn.RequestProcessor = rp;
                conn.ErrorProcessor = ep;
                conn.Start();

                conn.SendNotification(new SampleNotifMessage());
                _ctStop.WaitHandle.WaitOne();
            }

            GC.KeepAlive(rp);
            GC.KeepAlive(ep);
        }

        private static void ClientThread() {
            Random rand = new Random(2);
            byte[] arrPrivateKey = new byte[Core.NetworkConnection.PrivateKeySize];
            rand.NextBytes(arrPrivateKey);

            Core.ProtocolConnection.INotificationProcessor np = new ClientNotifProcessor();
            Core.ProtocolConnection.IErrorProcessor ep = new ErrorProcessor();

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 22200);

            using (Core.ProtocolConnection conn = new Core.ProtocolConnection(client, true, arrPrivateKey)) {
                conn.NotificationProcessor = np;
                conn.ErrorProcessor = ep;
                conn.Start();

                Console.WriteLine("Client received response from server: " + conn.PerformRequest(new SampleRequestMessage()));

                try {
                    conn.PerformRequest(new SampleBadRequestMessage());
                } catch (Exception ex) {
                    lock (_objConsoleMutex) 
                        Console.WriteLine($"Error (as expected) while performing \"bad\" request message: {ex}");
                }

                _ctStop.WaitHandle.WaitOne();
            }

            GC.KeepAlive(np);
            GC.KeepAlive(ep);
        }

        private class ClientNotifProcessor : Core.ProtocolConnection.INotificationProcessor {
            public void ProcessNotification(NotificationMessage msgNotification) {
                lock (_objConsoleMutex) Console.WriteLine($"Client received notification: {msgNotification}");
            }
        }

        private class ServerRequestProcessor : Core.ProtocolConnection.IRequestProcessor {
            public ResponseMessage ProcessRequest(RequestMessage msgRequest) {
                lock (_objConsoleMutex) Console.WriteLine($"Server received request: {msgRequest}");
                if (msgRequest is SampleBadRequestMessage)
                    throw new Exception("Sample exception (server recieved \"bad\" request message)");
                return new SampleResponseMessage();
            }
        }

        private class ErrorProcessor : Core.ProtocolConnection.IErrorProcessor {
            public void ProcessError(Exception ex) {
                lock (_objConsoleMutex) Console.WriteLine($"Error: {ex}");
            }
        }
    }
}
