using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Azimecha.Stupidchat.NetworkingTest {
    public static class Program {
        public static void Main(string[] arrArgs) {
            Thread thdServer = new Thread(ServerThread);
            Thread thdClient = new Thread(ClientThread);

            thdServer.Name = "Server Thread";
            thdClient.Name = "Client Thread";

            thdServer.Start();
            thdClient.Start();

            thdServer.Join();
            thdClient.Join();
        }

        private static readonly object _objConsoleMutex = new object();

        private static void ServerThread() {
            Random rand = new Random(1);
            byte[] arrPrivateKey = new byte[Core.NetworkConnection.PrivateKeySize];
            rand.NextBytes(arrPrivateKey);

            TcpListener listener = new TcpListener(IPAddress.Loopback, 22200);
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();
            using (Core.NetworkConnection conn = new Core.NetworkConnection(client, true, arrPrivateKey)) {
                conn.Initialize();
                conn.SendMessage(Encoding.ASCII.GetBytes("Hello from server!"));
                string strResp = Encoding.ASCII.GetString(conn.ReceiveMesssage());
                lock (_objConsoleMutex) Console.WriteLine("Server recieved: " + strResp);
            }
        }

        private static void ClientThread() {
            Random rand = new Random(2);
            byte[] arrPrivateKey = new byte[Core.NetworkConnection.PrivateKeySize];
            rand.NextBytes(arrPrivateKey);

            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 22200);

            using (Core.NetworkConnection conn = new Core.NetworkConnection(client, true, arrPrivateKey)) {
                conn.Initialize();
                string strResp = Encoding.ASCII.GetString(conn.ReceiveMesssage());
                conn.SendMessage(Encoding.ASCII.GetBytes("Hello from client!"));
                lock (_objConsoleMutex) Console.WriteLine("Client recieved: " + strResp);
            }
        }
    }
}
