using Azimecha.Stupidchat.Server;
using System;
using System.IO;
using System.Windows.Forms;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    public partial class MainForm : Form {
        private Server.Server _server;

        public MainForm() {
            InitializeComponent();

            if (!Properties.Settings.Default.SettingsInitialized)
                InitSettings();
        }

        private void InitSettings() {
            byte[] arrPrivateKey = new byte[Core.NetworkConnection.CreateSigningAlgorithmInstance().PrivateKeySize];
            WindowsRandomNumberGenerator.Fill(arrPrivateKey);
            Properties.Settings.Default.PrivateKey = Convert.ToHexString(arrPrivateKey);

            Core.Structures.ServerInfo infServer = new Core.Structures.ServerInfo() {
                Name = "Test Server",
                Description = "For testing purposes"
            };
            Properties.Settings.Default.ServerInfo = Newtonsoft.Json.JsonConvert.SerializeObject(infServer);

            string strDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StupidchatServer");
            if (!Directory.Exists(strDataFolder))
                Directory.CreateDirectory(strDataFolder);

            Properties.Settings.Default.DatabasePath = Path.Combine(strDataFolder, "Server.dat");

            Properties.Settings.Default.SettingsInitialized = true;
            Properties.Settings.Default.Save();
        }

        private void StartStopButton_Click(object sender, EventArgs e) {
            if (_server is null) {
                byte[] arrPrivateKey = Convert.FromHexString(Properties.Settings.Default.PrivateKey);
                Core.Structures.ServerInfo infServer = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.Structures.ServerInfo>
                    (Properties.Settings.Default.ServerInfo);
                System.Net.IPEndPoint endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, Properties.Settings.Default.Port);

                try {
                    _server = new Server.Server(arrPrivateKey, endpoint, infServer, Properties.Settings.Default.DatabasePath);
                } catch (Exception ex) {
                    MessageBox.Show(this, $"Error starting server:\n{ex}", "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                StatusLabel.Text = "Running";
                StartStopButton.Text = "Stop Server";
            } else {
                try {
                    _server.Dispose();
                    _server = null;
                } catch (Exception ex) {
                    MessageBox.Show(this, $"Error stopping server:\n{ex}", "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    StatusLabel.Text = "Errored";
                    return;
                }

                GC.Collect();

                StatusLabel.Text = "Stopped";
                StartStopButton.Text = "Start Server";
            }
        }
    }
}