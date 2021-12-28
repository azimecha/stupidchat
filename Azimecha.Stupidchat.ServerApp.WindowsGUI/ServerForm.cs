using Azimecha.Stupidchat.Server;
using Azimecha.Stupidchat.Core;
using System;
using System.IO;
using System.Windows.Forms;

namespace Azimecha.Stupidchat.ServerApp.WindowsGUI {
    public partial class ServerForm : Form {
        private Server.Server _server;
        private Server.Records.ChannelRecord _chanSelected;

        public ServerForm() {
            InitializeComponent();

            if (!Properties.Settings.Default.SettingsInitialized)
                InitSettings();

            Server.Server.LogListener = new LogListener(this);
        }

        private void InitSettings() {
            byte[] arrPrivateKey = new byte[NetworkConnection.CreateSigningAlgorithmInstance().PrivateKeySize];
            WindowsRandomNumberGenerator.Fill(arrPrivateKey);
            Properties.Settings.Default.PrivateKey = Convert.ToHexString(arrPrivateKey);

            ServerInfo = new Core.Structures.ServerInfo() {
                Name = "Test Server",
                Description = "For testing purposes"
            };

            string strDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StupidchatServer");
            if (!Directory.Exists(strDataFolder))
                Directory.CreateDirectory(strDataFolder);

            Properties.Settings.Default.DatabasePath = Path.Combine(strDataFolder, "Server.dat");

            Properties.Settings.Default.SettingsInitialized = true;
            Properties.Settings.Default.Save();
        }

        public Core.Structures.ServerInfo ServerInfo {
            get => Newtonsoft.Json.JsonConvert.DeserializeObject<Core.Structures.ServerInfo>(Properties.Settings.Default.ServerInfo);
            set => Properties.Settings.Default.ServerInfo = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        private void StartStopButton_Click(object sender, EventArgs e) {
            if (_server is null) {
                byte[] arrPrivateKey = Convert.FromHexString(Properties.Settings.Default.PrivateKey);
                System.Net.IPEndPoint endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, Properties.Settings.Default.Port);

                try {
                    _server = new Server.Server(arrPrivateKey, endpoint, ServerInfo, Properties.Settings.Default.DatabasePath);
                } catch (Exception ex) {
                    MessageBox.Show(this, $"Error starting server:\n{ex}", "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _server.ChannelCreated += Server_ChannelCreated;
                _server.ChannelDeleted += Server_ChannelDeleted;
                _server.ChannelModified += Server_ChannelModified;
                _server.MemberJoined += Server_MemberJoined;
                _server.MemberLeft += Server_MemberLeft;
                _server.MemberOtherInfoUpdated += Server_MemberOtherInfoUpdated;
                _server.MemberProfileUpdated += Server_MemberProfileUpdated;
                _server.MessageDeleted += Server_MessageDeleted;
                _server.MessagePosted += Server_MessagePosted;

                StatusLabel.Text = "Running";
                StartStopButton.Text = "Stop Server";

                RefreshButton_Click(null, null);
            } else {
                _server.ChannelCreated -= Server_ChannelCreated;
                _server.ChannelDeleted -= Server_ChannelDeleted;
                _server.ChannelModified -= Server_ChannelModified;
                _server.MemberJoined -= Server_MemberJoined;
                _server.MemberLeft -= Server_MemberLeft;
                _server.MemberOtherInfoUpdated -= Server_MemberOtherInfoUpdated;
                _server.MemberProfileUpdated -= Server_MemberProfileUpdated;
                _server.MessageDeleted -= Server_MessageDeleted;
                _server.MessagePosted -= Server_MessagePosted;

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

            ServerInfoDropdown.Enabled = _server is null;
            ChannelCreateButton.Enabled = !(_server is null);
        }

        private void Server_MessagePosted(Server.Records.MessageRecord message) {
            if (InvokeRequired) { BeginInvoke(Server_MessagePosted, message); return; }

            if (message.ChannelID == _chanSelected?.ID)
                ChannelMessagesList.Items.Add(message);

            LogMessageList.Items.Add("Message posted: " + message.ToMessageData().ToDataString());
        }

        private void Server_MessageDeleted(long nChannel, long nMessageIndex) {
            if (InvokeRequired) { BeginInvoke(Server_MessageDeleted, nChannel, nMessageIndex); return; }

            if (nChannel == _chanSelected?.ID) {
                Server.Records.MessageRecord message = FindMessageItem(nMessageIndex);
                if (!(message is null))
                    ChannelMessagesList.Items.Remove(message);
            }

            LogMessageList.Items.Add($"Message #{nMessageIndex} in channel #{nChannel} was deleted");
        }

        private Server.Records.MessageRecord FindMessageItem(long nMessageIndex) {
            foreach (Server.Records.MessageRecord message in ChannelMessagesList.Items)
                if (message.MessageIndex == nMessageIndex)
                    return message;
            return null;
        }

        private void Server_MemberProfileUpdated(Server.Records.MemberRecord obj) {
            if (InvokeRequired) { BeginInvoke(Server_MemberProfileUpdated, obj); return; }

            int? nIndex = FindMemberItemIndex(obj.MemberID);
            if (nIndex.HasValue)
                MembersList.Items[nIndex.Value] = obj;

            LogMessageList.Items.Add("Member profile updated: " + obj.Flatten().ToDataString());
        }

        private void Server_MemberOtherInfoUpdated(Server.Records.MemberRecord obj) {
            if (InvokeRequired) { BeginInvoke(Server_MemberOtherInfoUpdated, obj); return; }

            int? nIndex = FindMemberItemIndex(obj.MemberID);
            if (nIndex.HasValue)
                MembersList.Items[nIndex.Value] = obj;

            LogMessageList.Items.Add("Member info updated: " + obj.Flatten().ToDataString());
        }

        private void Server_MemberLeft(long nMemberID) {
            if (InvokeRequired) { BeginInvoke(Server_MemberLeft, nMemberID); return; }

            int? nIndex = FindMemberItemIndex(nMemberID);
            if (nIndex.HasValue)
                MembersList.Items.RemoveAt(nIndex.Value);

            LogMessageList.Items.Add($"Member {nMemberID} left server");
        }

        private int? FindMemberItemIndex(long nMemberID) {
            for (int nIndex = 0; nIndex < MembersList.Items.Count; nIndex++)
                if ((MembersList.Items[nIndex] as Server.Records.MemberRecord)?.MemberID == nMemberID)
                    return nIndex;
            return null;
        }

        private void Server_MemberJoined(Server.Records.MemberRecord obj) {
            if (InvokeRequired) { BeginInvoke(Server_MemberJoined, obj); return; }

            MembersList.Items.Add(obj);
            LogMessageList.Items.Add("Member joined: " + obj.Flatten().ToDataString());
        }

        private void Server_ChannelModified(Server.Records.ChannelRecord obj) {
            if (InvokeRequired) { BeginInvoke(Server_ChannelModified, obj); return; }

            int? nIndex = FindChannelItemIndex(obj.ID);
            if (nIndex.HasValue)
                ChannelsList.Items[nIndex.Value] = obj;

            LogMessageList.Items.Add("Channel modified: " + obj.ToChannelInfo().ToDataString());
        }

        private void Server_ChannelDeleted(long nChannel) {
            if (InvokeRequired) { BeginInvoke(Server_ChannelDeleted, nChannel); return; }

            int? nIndex = FindChannelItemIndex(nChannel);
            if (nIndex.HasValue)
                ChannelsList.Items.RemoveAt(nIndex.Value);

            LogMessageList.Items.Add($"Channel #{nChannel} was deleted");
        }

        private void Server_ChannelCreated(Server.Records.ChannelRecord obj) {
            if (InvokeRequired) { BeginInvoke(Server_ChannelCreated, obj); return; }

            ChannelsList.Items.Add(obj);
            LogMessageList.Items.Add("Channel created: " + obj.ToChannelInfo().ToDataString());
        }

        private int? FindChannelItemIndex(long nChannelID) {
            for (int nIndex = 0; nIndex < ChannelsList.Items.Count; nIndex++)
                if ((ChannelsList.Items[nIndex] as Server.Records.ChannelRecord)?.ID == nChannelID)
                    return nIndex;
            return null;
        }

        private class LogListener : ILogListener {
            private WeakReference<ServerForm> _weakForm;

            public LogListener(ServerForm form) {
                _weakForm = new WeakReference<ServerForm>(form);
            }

            public void LogMessage(string strMessage) {
                ServerForm form;
                if (_weakForm.TryGetTarget(out form))
                    form.BeginInvoke(() => form.LogMessageList.Items.Add("[Server Log] " + strMessage));
            }
        }

        private void LogMessageList_MouseDoubleClick(object sender, MouseEventArgs e) {
            MessageBox.Show(this, LogMessageList.SelectedItem.ToString(), "Log message", MessageBoxButtons.OK);
        }

        private void RefreshButton_Click(object sender, EventArgs e) {
            MembersList.Items.Clear();
            ChannelsList.Items.Clear();

            if (!(_server is null)) {
                foreach (Server.Records.MemberRecord member in _server.Members)
                    MembersList.Items.Add(member);

                foreach (Server.Records.ChannelRecord channel in _server.Channels)
                    ChannelsList.Items.Add(channel);
            }

            RefreshMessagesList();
        }

        private void ChannelsList_SelectedValueChanged(object sender, EventArgs e) {
            if (!(ChannelsList.SelectedItem is null)) {
                _chanSelected = (Server.Records.ChannelRecord)ChannelsList.SelectedItem;
                RefreshMessagesList();
            }
        }

        private void RefreshMessagesList() {
            ChannelMessagesList.Items.Clear();

            if (!(_chanSelected is null))
                foreach (Server.Records.MessageRecord message in _server.GetMessages(_chanSelected.ID))
                    ChannelMessagesList.Items.Add(message);
        }

        private void MembersList_SelectedIndexChanged(object sender, EventArgs e) {
            MemberRemoveButton.Enabled = MemberPowerDropdown.Enabled = MembersList.SelectedIndex >= 0;
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
            Server.Records.MemberRecord member = MembersList.SelectedItem as Server.Records.MemberRecord;
            if (!(member is null)) _server.SetMemberPower(member.MemberID, power);
        }

        private void ChannelCreateButton_Click(object sender, EventArgs e) {
            string strChannelName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name of the channel to create:",
                "Create Channel");
            if ((strChannelName?.Length ?? 0) > 0)
                _server.AddChannel(new Core.Structures.ChannelInfo() { Name = strChannelName, Type = Core.Structures.ChannelType.Text });
        }

        private void ChannelsList_SelectedIndexChanged(object sender, EventArgs e) {
            Server.Records.ChannelRecord channel = ChannelsList.SelectedItem as Server.Records.ChannelRecord;
            ChannelDeleteButton.Enabled = ModifyChannelDropdown.Enabled = !(channel is null);
            if (!(channel is null)) {
                _chanSelected = channel;
                RefreshMessagesList();
            }
        }

        private void ChannelDeleteButton_Click(object sender, EventArgs e) {
            Server.Records.ChannelRecord channel = ChannelsList.SelectedItem as Server.Records.ChannelRecord;
            if (!(channel is null)) _server.RemoveChannel(channel.ID);
        }

        private void LogClearButton_Click(object sender, EventArgs e) {
            LogMessageList.Items.Clear();
        }

        private void MessageDeleteButton_Click(object sender, EventArgs e) {
            Server.Records.MessageRecord message = ChannelMessagesList.SelectedItem as Server.Records.MessageRecord;
            if (!(message is null)) _server.RemoveMessage(message.ChannelID, message.MessageIndex);
        }

        private void ChannelMessagesList_SelectedIndexChanged(object sender, EventArgs e) {
            MessageDeleteButton.Enabled = ChannelMessagesList.SelectedIndex >= 0;
        }

        private void ChannelMessagesList_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (ChannelMessagesList.SelectedItem is Server.Records.MessageRecord message)
                MessageBox.Show(this, message.Flatten().ToDataString("\r\n"));
        }

        private void ChannelNameItem_Click(object sender, EventArgs e) {
            if (_chanSelected is null) return;

            string strName = Microsoft.VisualBasic.Interaction.InputBox("Enter the new name for the channel.", "Set Channel Name",
                _chanSelected.Name);
            if ((strName?.Length ?? 0) > 0) {
                Core.Structures.ChannelInfo infChannel = _chanSelected.ToChannelInfo();
                infChannel.Name = strName;
                _server.UpdateChannel(infChannel);
            }
        }

        private void ChannelDescriptionItem_Click(object sender, EventArgs e) {
            if (_chanSelected is null) return;

            string strDescription = Microsoft.VisualBasic.Interaction.InputBox("Enter the new description for the channel.", 
                "Set Channel Description", _chanSelected.Description);
            if ((strDescription?.Length ?? 0) > 0) {
                Core.Structures.ChannelInfo infChannel = _chanSelected.ToChannelInfo();
                infChannel.Description = strDescription;
                _server.UpdateChannel(infChannel);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            Server.Server.LogListener = new Server.DebugLogListener();
            _server?.Dispose();
            _server = null;
        }

        private void ChannelsList_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (!(_chanSelected is null))
                MessageBox.Show(this, _chanSelected.ToChannelInfo().ToDataString(strBetweenValues: "\n"), "Channel Information");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (!(_server is null)) {
                if (MessageBox.Show(this, "Are you sure you want to exit and stop the server?", "Confirm Exit", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning) != DialogResult.OK)
                    e.Cancel = true;
            }
        }

        private void MemberRemoveButton_Click(object sender, EventArgs e) {
            if (MembersList.SelectedItem is Server.Records.MemberRecord memb)
                _server.KickMember(memb.MemberID);
        }

        private void MembersList_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (MembersList.SelectedItem is Server.Records.MemberRecord memb)
                MessageBox.Show(this, memb.Flatten().ToDataString("\n"), "Member Info");
        }

        private void ServerNameItem_Click(object sender, EventArgs e) {
            Core.Structures.ServerInfo infServer = ServerInfo;
            string strName = Microsoft.VisualBasic.Interaction.InputBox("Enter a short, human readable name for the server.",
                "Set Server Name", infServer.Name);
            if ((strName?.Length ?? 0) > 0) {
                infServer.Name = strName;
                ServerInfo = infServer;
            }
        }

        private void ServerDescItem_Click(object sender, EventArgs e) {
            Core.Structures.ServerInfo infServer = ServerInfo;
            string strDesc = Microsoft.VisualBasic.Interaction.InputBox("Enter a the description of the server to be shown to users.",
                "Set Server Description", infServer.Description);
            if ((strDesc?.Length ?? 0) > 0) {
                infServer.Description = strDesc;
                ServerInfo = infServer;
            }
        }

        private void ServerIconItem_Click(object sender, EventArgs e) {
            Core.Structures.ServerInfo infServer = ServerInfo;
            string strURL = Microsoft.VisualBasic.Interaction.InputBox("Enter the URL of the image to use as the server's icon.",
                "Set Server Icon", infServer.ImageURL);
            if ((strURL?.Length ?? 0) > 0) {
                infServer.ImageURL = strURL;
                ServerInfo = infServer;
            }
        }
    }
}