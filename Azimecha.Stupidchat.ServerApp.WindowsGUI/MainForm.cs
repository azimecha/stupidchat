using Azimecha.Stupidchat.Server;
using System;
using System.IO;
using System.Windows.Forms;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    public partial class MainForm : Form {
        private Server.Server _server;
        private Core.Structures.ChannelInfo _chanSelected;

        public MainForm() {
            InitializeComponent();

            if (!Properties.Settings.Default.SettingsInitialized)
                InitSettings();

            Server.Server.LogListener = new LogListener(this);
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

        private class LogListener : ILogListener {
            private WeakReference<MainForm> _weakForm;

            public LogListener(MainForm form) {
                _weakForm = new WeakReference<MainForm>(form);
            }

            public void LogMessage(string strMessage) {
                MainForm form;
                if (_weakForm.TryGetTarget(out form))
                    form.BeginInvoke(() => form.LogMessageList.Items.Add(strMessage));
            }
        }

        private void LogMessageList_MouseDoubleClick(object sender, MouseEventArgs e) {
            MessageBox.Show(this, LogMessageList.SelectedItem.ToString(), "Log message", MessageBoxButtons.OK);
        }

        private void RefreshButton_Click(object sender, EventArgs e) {
            MembersList.Items.Clear();
            foreach (Core.Structures.MemberInfo infMember in _server.Members)
                MembersList.Items.Add(infMember);

            ChannelsList.Items.Clear();
            foreach (Core.Structures.ChannelInfo infChannel in _server.Channels)
                ChannelsList.Items.Add(infChannel);

            RefreshMessagesList();
        }

        private void ChannelsList_SelectedValueChanged(object sender, EventArgs e) {
            if (!(ChannelsList.SelectedItem is null)) {
                _chanSelected = (Core.Structures.ChannelInfo)ChannelsList.SelectedItem;
                RefreshMessagesList();
            }
        }

        private void RefreshMessagesList() {
            ChannelMessagesList.Items.Clear();
            foreach (Core.Structures.MessageData data in _server.GetMessages(_chanSelected.ID))
                ChannelMessagesList.Items.Add(data);
        }

        private void MembersList_SelectedIndexChanged(object sender, EventArgs e) {
            MemberPowerDropdown.Enabled = MembersList.SelectedIndex >= 0;
        }

        private void PowerReducedItem_Click(object sender, EventArgs e)
            => SetCurrentMemberPower(Core.Structures.PowerLevel.Reduced);

        private void PowerNormalItem_Click(object sender, EventArgs e)
            => SetCurrentMemberPower(Core.Structures.PowerLevel.Normal);

        private void PowerModeratorItem_Click(object sender, EventArgs e)
            => SetCurrentMemberPower(Core.Structures.PowerLevel.Moderator);

        private void PowerAdministratorItem_Click(object sender, EventArgs e)
            => SetCurrentMemberPower(Core.Structures.PowerLevel.Administrator);

        private void SetCurrentMemberPower(Core.Structures.PowerLevel power) {
            Core.Structures.MemberInfo? infMember = MembersList.SelectedItem as Core.Structures.MemberInfo?;
            if (infMember.HasValue) _server.SetMemberPower(_server.GetMemberID(infMember.Value), power);
        }

        private void ChannelCreateButton_Click(object sender, EventArgs e) {
            string strChannelName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name of the channel to create:",
                "Create Channel");
            if (!(strChannelName is null))
                _server.AddChannel(new Core.Structures.ChannelInfo() { Name = strChannelName, Type = Core.Structures.ChannelType.Text });
        }

        private void ChannelsList_SelectedIndexChanged(object sender, EventArgs e) {
            Core.Structures.ChannelInfo? infChannel = ChannelsList.SelectedItem as Core.Structures.ChannelInfo?;
            ChannelDeleteButton.Enabled = ModifyChannelDropdown.Enabled = infChannel.HasValue;
            if (infChannel.HasValue) {
                _chanSelected = infChannel.Value;
                RefreshMessagesList();
            }
        }

        private void ChannelDeleteButton_Click(object sender, EventArgs e) {
            Core.Structures.ChannelInfo? infChannel = ChannelsList.SelectedItem as Core.Structures.ChannelInfo?;
            if (infChannel.HasValue) _server.RemoveChannel(infChannel.Value.ID);
        }

        private void LogClearButton_Click(object sender, EventArgs e) {
            LogMessageList.Items.Clear();
        }

        private void MessageDeleteButton_Click(object sender, EventArgs e) {
            Core.Structures.MessageData? infMessage = ChannelMessagesList.SelectedItem as Core.Structures.MessageData?;
            if (infMessage.HasValue) _server.RemoveMessage(_chanSelected.ID, infMessage.Value.ID);
        }

        private void ChannelMessagesList_SelectedIndexChanged(object sender, EventArgs e) {
        }

        private void ChannelMessagesList_MouseDoubleClick(object sender, MouseEventArgs e) {
            Core.Structures.MessageData? infMessage = ChannelMessagesList.SelectedItem as Core.Structures.MessageData?;
            if (infMessage.HasValue) {
                MessageBox.Show(this, $"Poster: {infMessage.Value.SenderPublicSigningKey}\nPost date: {infMessage.Value.PostedTime}\n"
                    + "Data: " + System.Text.Encoding.UTF8.GetString(infMessage.Value.SignedData));
            }
        }
    }
}