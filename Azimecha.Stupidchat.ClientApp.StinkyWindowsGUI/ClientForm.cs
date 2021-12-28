using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Azimecha.Stupidchat.Core;

namespace Azimecha.Stupidchat.ClientApp.StinkyWindowsGUI {
    public partial class ClientForm : Form {
        private Client.ChatClient _client;
        private Client.IServer _serverCurrent;
        private Client.IChannel _channelCurrent;
        private string _strNewServerAddress;

        private const int DEFAULT_PORT = 22001;
        private static readonly System.Text.RegularExpressions.Regex REGEX_ADDRESS 
            = new System.Text.RegularExpressions.Regex(@"^([a-zA-Z0-9\-\.]+)(:[0-9]+)?$");

        private const int INITIAL_MESSAGES_LOAD_COUNT = 10;
        private static readonly string APP_CODENAME = typeof(ClientForm).Namespace;

        public ClientForm() {
            InitializeComponent();

            string strUsername, strPassword;
            Meziantou.Framework.Win32.Credential credExisting = Meziantou.Framework.Win32.CredentialManager
                .ReadCredential(typeof(ClientForm).Namespace);

            if (credExisting is null) {
                Meziantou.Framework.Win32.CredentialResult credResult = Meziantou.Framework.Win32.CredentialManager.PromptForCredentials(
                    captionText: "Log in or create account", messageText: "Enter your username and password for Azimecha Stupidchat.\n"
                    + "You do not need to use your Windows credentials. Instead, choose a strong and unique password.",
                    userName: Environment.UserName, saveCredential: Meziantou.Framework.Win32.CredentialSaveOption.Selected);

                if (credResult is null)
                    Environment.Exit(0);

                if (credResult.CredentialSaved == Meziantou.Framework.Win32.CredentialSaveOption.Selected)
                    Meziantou.Framework.Win32.CredentialManager.WriteCredential(APP_CODENAME, credResult.UserName, credResult.Password,
                        Meziantou.Framework.Win32.CredentialPersistence.LocalMachine);

                strUsername = credResult.UserName;
                strPassword = credResult.Password;
            } else {
                strUsername = credExisting.UserName;
                strPassword = credExisting.Password;
            }

            Core.Cryptography.IKeyDerivationAlgorithm algoKeyDeriv = new Core.Cryptography.Argon2i();
            byte[] arrPrivateKey = new byte[NetworkConnection.CreateSigningAlgorithmInstance().PrivateKeySize];

            algoKeyDeriv.DeriveKey(arrPrivateKey, System.Text.Encoding.UTF8.GetBytes(strPassword),
                System.Text.Encoding.UTF8.GetBytes(strUsername));

            _client = new Client.ChatClient(arrPrivateKey);
            _client.DisconnectedFromServer += Client_DisconnectedFromServer;
            _client.ChannelAdded += Client_ChannelAdded;
            _client.ChannelInfoChanged += Client_ChannelInfoChanged;
            _client.ChannelRemoved += Client_ChannelRemoved;
            _client.MemberInfoChanged += Client_MemberInfoChanged;
            _client.MemberJoined += Client_MemberJoined;
            _client.MemberLeft += Client_MemberLeft;
            _client.MessageDeleted += Client_MessageDeleted;
            _client.MessagePosted += Client_MessagePosted;

            Core.Structures.UserProfile profile = new();

            if ((Properties.Settings.Default.Profile?.Length ?? 0) > 0)
                profile = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.Structures.UserProfile>
                    (Properties.Settings.Default.Profile);

            profile.Username = strUsername;
            _client.MyProfile = profile;

            if ((Properties.Settings.Default.Servers?.Count ?? 0) > 0) {
                MainStatusLabel.Text = "Connecting to servers...";
                SavedConnectionsWorker.RunWorkerAsync(Properties.Settings.Default.Servers);
            }

            if (Properties.Settings.Default.Servers is null)
                Properties.Settings.Default.Servers = new System.Collections.Specialized.StringCollection();

            ServerImageList.Images.Add(DEFAULT_IMAGE, Properties.Resources.server2_16);
        }

        public Client.IServer CurrentServer {
            get => _serverCurrent;
            set {
                _serverCurrent = value;
                MembersListView.Items.Clear();

                if (!(_serverCurrent is null))
                    foreach (Client.IMember memb in _serverCurrent.Members)
                        AddMemberItem(memb);

                ServerDisconnectButton.Enabled = ServerSetNickButton.Enabled = !(_serverCurrent is null);
            }
        }

        public Client.IChannel CurrentChannel {
            get => _channelCurrent;
            set {
                _channelCurrent = value;
                ClearMemberItems();

                if (!(_channelCurrent is null))
                    foreach (Client.IMessage msg in _channelCurrent.Messages.Take(INITIAL_MESSAGES_LOAD_COUNT))
                        AddMessageControl(msg);

                UpdateSendEnabled();
            }
        }

        private const string DEFAULT_IMAGE = "DEFAULT";

        private void AddMemberItem(Client.IMember memb) {
            ListViewItem item = new ListViewItem() { Text = memb.Info.GetName(), Tag = memb, ImageKey = DEFAULT_IMAGE };

            System.IO.Stream stmAvatar = memb.User.OpenAvatar();
            if (!(stmAvatar is null)) {
                string strImageKey = memb.User.PublicKey.ToHexString();
                UserImageList.Images.Add(strImageKey, new System.Drawing.Bitmap(stmAvatar));
                item.ImageKey = strImageKey;
            }

            MembersListView.Items.Add(item);
        }

        private void ClearMemberItems() {
            MembersListView.Items.Clear();
            UserImageList.Images.Clear();
            UserImageList.Images.Add(DEFAULT_IMAGE, Properties.Resources.nft32);
        }

        private void AddMessageControl(Client.IMessage msg) {
            MessagesLayout.Controls.Add(new Label() { 
                Text = msg.SignedData.ToDataString("\r\n"), 
                AutoSize = true, 
                BorderStyle = BorderStyle.FixedSingle,
                Tag = msg
            });
        }

        private void SendingLayout_Click(object sender, EventArgs e) {
            MessageTextBox.Select();
        }

        private void ConnectButton_Click(object sender, EventArgs e) {
            string strAddress = Microsoft.VisualBasic.Interaction.InputBox("Enter the address of the server to connect to.",
                "Connect to Server");

            if (strAddress != "") {
                System.Net.IPEndPoint endpoint;

                try {
                    endpoint = ParseServerAddress(strAddress);
                } catch (FormatException exFormat) {
                    MessageBox.Show(this, $"Invalid server address: {exFormat.Message}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                } catch (Exception ex) {
                    MessageBox.Show(this, $"Error parsing server address:\n{ex}", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                NewConnectionWorker.RunWorkerAsync(endpoint);
                ServerConnectButton.Text = $"Connecting to {strAddress}";
                ServerConnectButton.Enabled = false;
            }
        }

        private System.Net.IPEndPoint ParseServerAddress(string strAddress) {
            System.Net.IPEndPoint endpoint;
            System.Net.IPAddress address;

            if (System.Net.IPEndPoint.TryParse(strAddress, out endpoint)) {
                if (endpoint.Port == 0)
                    endpoint.Port = DEFAULT_PORT;
                return endpoint;
            } else if (System.Net.IPAddress.TryParse(strAddress, out address))
                return new System.Net.IPEndPoint(address, DEFAULT_PORT);
            else if (REGEX_ADDRESS.IsMatch(strAddress)) {
                System.Text.RegularExpressions.Match match = REGEX_ADDRESS.Match(strAddress);
                string strHostname = match.Groups[1].Value;

                address = System.Net.Dns.GetHostAddresses(strHostname).FirstOrDefault();
                if (address is null)
                    throw new FormatException($"Host {strHostname} could not be found");

                int nPort;
                if (match.Groups[2].Success) {
                    if (!int.TryParse(match.Groups[2].Value, out nPort) || (nPort < 0) || (nPort > 0xFFFF))
                        throw new FormatException($"{match.Groups[2].Value} is not a valid port");
                } else
                    nPort = DEFAULT_PORT;

                return new System.Net.IPEndPoint(address, nPort);
            } else
                throw new FormatException($"Server address \"{strAddress}\" does not match any known format.");
        }

        private struct ConnectionResult {
            public Client.IServer Server;
            public Client.IChannel[] Channels;
        }

        private void NewConnectionWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            System.Net.IPEndPoint endpoint = (System.Net.IPEndPoint)e.Argument;
            ConnectionResult result = new();

            result.Server = _client.ConnectToServer(endpoint .Address.ToString(), endpoint.Port);
            result.Channels = result.Server.Channels.ToArray();

            e.Result = result;
        }

        private void NewConnectionWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            if (!(e.Error is null)) {
                MessageBox.Show(this, $"Error connecting to server:\n\n{e.Error}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                MainStatusLabel.Text = $"Error connecting to server: {e.Error.Message} ({e.Error.GetType().FullName})";
            } else {
                ConnectionResult result = (ConnectionResult)e.Result;
                MainStatusLabel.Text = $"Connected to {result.Server.ID.ToHexString()} on {result.Server.Address}:{result.Server.Port}";

                AddServer(result.Server, result.Channels);

                Properties.Settings.Default.Servers.Add(_strNewServerAddress);
            }

            ServerConnectButton.Text = "Connect";
            ServerConnectButton.Enabled = true;
        }

        private void ServersTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Node.Tag is Client.IServer) {
                CurrentChannel = null;
                CurrentServer = (Client.IServer)e.Node.Tag;
            } else if (e.Node.Tag is Client.IChannel) {
                CurrentChannel = (Client.IChannel)e.Node.Tag;
                CurrentServer = _channelCurrent.Server;
            }
        }

        private struct ConnectionAttemptResult {
            public string Address;
            public Client.IServer ServerObject;
            public Client.IChannel[] Channels;
            public Exception Error;
        }

        private void SavedConnectionsWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            foreach (string strAddress in (System.Collections.Specialized.StringCollection)e.Argument) {
                if ((strAddress?.Length ?? 0) == 0) continue;
                ConnectionAttemptResult result = new ConnectionAttemptResult() { Address = strAddress };
                try {
                    BeginInvoke(() => MainStatusLabel.Text = $"Connecting to {strAddress}...");
                    System.Net.IPEndPoint endpoint = ParseServerAddress(strAddress);
                    result.ServerObject = _client.ConnectToServer(endpoint.Address.ToString(), endpoint.Port);
                    result.Channels = result.ServerObject.Channels.ToArray();
                } catch (Exception ex) {
                    result.ServerObject = null;
                    result.Channels = null;
                    result.Error = ex;
                }
                SavedConnectionsWorker.ReportProgress(0, result);
            }
        }

        private void SavedConnectionsWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            ConnectionAttemptResult result = (ConnectionAttemptResult)e.UserState;

            if (!(result.Error is null)) {
                MessageBox.Show(this, $"Unable to reconnect to {result.Address}:\n{result.Error}", "Error reconnecting to server",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MainStatusLabel.Text = $"Error connecting to {result.Address}: {result.Error.Message} ({result.Error.GetType().FullName})";
                Properties.Settings.Default.Servers.Remove(result.Address);
                return;
            }

            AddServer(result.ServerObject, result.Channels);
        }

        private void AddServer(Client.IServer server, IEnumerable<Client.IChannel> enuChannels) {
            TreeNode nodeServer = new TreeNode() {
                Text = server.Info.Name,
                Tag = server,
                ImageKey = DEFAULT_IMAGE
            };

            /*System.IO.Stream stmIcon = server.OpenIcon();
            if (!(stmIcon is null)) {
                string strIconKey = server.ID.ToHexString();
                ServerImageList.Images.Add(strIconKey, new System.Drawing.Bitmap(stmIcon));
                nodeServer.ImageKey = strIconKey;
            }*/

            foreach (Client.IChannel chan in enuChannels) {
                TreeNode nodeChannel = new TreeNode() {
                    Text = chan.Info.Name,
                    Tag = chan
                };

                nodeServer.Nodes.Add(nodeChannel);
            }

            ServersTreeView.Nodes.Add(nodeServer);
        }

        private TreeNode[] GetAllTreeNodes() {
            List<TreeNode> lstNodes = new List<TreeNode>();

            Queue<TreeNode> queueToProcess = new Queue<TreeNode>();
            foreach (TreeNode node in ServersTreeView.Nodes)
                queueToProcess.Enqueue(node);

            TreeNode nodeCur;
            while (queueToProcess.TryDequeue(out nodeCur)) {
                foreach (TreeNode node in nodeCur.Nodes)
                    queueToProcess.Enqueue(node);
                lstNodes.Add(nodeCur);
            }

            return lstNodes.ToArray();
        }

        private void Client_DisconnectedFromServer(Client.IServer server) {
            CurrentChannel = null;
            CurrentServer = null;

            foreach (TreeNode node in GetAllTreeNodes().Where(node => node.Tag == server))
                node.Remove();
        }

        private void Client_ChannelAdded(Client.IChannel chan) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_ChannelAdded(chan));
                return;
            }

            TreeNode nodeServer = GetAllTreeNodes().Where(node => node.Tag == chan.Server).First();
            TreeNode nodeChannel = new TreeNode() { Text = chan.Info.Name, Tag = chan };
            nodeServer.Nodes.Add(nodeChannel);
        }

        private void Client_ChannelInfoChanged(Client.IChannel chan) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_ChannelInfoChanged(chan));
                return;
            }

            TreeNode nodeChannel = GetAllTreeNodes().Where(node => node.Tag == chan).First();
            nodeChannel.Text = chan.Info.Name;
        }

        private void Client_ChannelRemoved(Client.IChannel chan) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_ChannelRemoved(chan));
                return;
            }

            foreach (TreeNode node in GetAllTreeNodes().Where(node => node.Tag == chan))
                node.Remove();
        }

        private ListViewItem[] GetAllMemberItems() {
            List<ListViewItem> lstItems = new List<ListViewItem>();
            foreach (ListViewItem item in MembersListView.Items)
                lstItems.Add(item);
            return lstItems.ToArray();
        }

        private void Client_MemberJoined(Client.IMember memb) {
            AddMemberItem(memb);
        }

        private void Client_MemberInfoChanged(Client.IMember memb) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_MemberInfoChanged(memb));
                return;
            }

            ListViewItem itemMember = GetAllMemberItems().Where(item => item.Tag == memb).FirstOrDefault();
            if (!(itemMember is null))
                itemMember.Text = memb.Info.GetName();
        }

        private void Client_MemberLeft(Client.IMember memb) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_MemberLeft(memb));
                return;
            }

            foreach (ListViewItem item in GetAllMemberItems().Where(item => item.Tag == memb))
                item.Remove();
        }

        private void Client_MessagePosted(Client.IMessage msg) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_MessagePosted(msg));
                return;
            }

            AddMessageControl(msg);
        }

        private void Client_MessageDeleted(Client.IMessage msg) {
            if (InvokeRequired) {
                BeginInvoke(() => Client_MessageDeleted(msg));
                return;
            }

            foreach (Control ctl in MessagesLayout.GetChildren().Where(ctl => ctl.Tag == msg))
                MessagesLayout.Controls.Remove(ctl);
        }

        private void SendMessageButton_Click(object sender, EventArgs e) {
            try {
                _channelCurrent.PostMessage(new Core.Structures.MessageSignedData() {
                    SendTime = DateTime.Now.Ticks,
                    Text = MessageTextBox.Text
                });
            } catch (Exception ex) {
                MessageBox.Show(this, $"Error sending message:\r\n{ex}", "Error sending",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageTextBox.Clear();
        }

        private void UpdateSendEnabled() {
            SendMessageButton.Enabled = (MessageTextBox.Text.Trim().Length > 0) && !(_channelCurrent is null);
        }

        private void MessageTextBox_TextChanged(object sender, EventArgs e) {
            UpdateSendEnabled();
        }

        private void ServerDisconnectButton_Click(object sender, EventArgs e) {
            _serverCurrent?.Disconnect();
        }

        private void ServerSetNickButton_Click(object sender, EventArgs e) {
            if (_serverCurrent is null) return;
            string strOldNickname = _serverCurrent.Me.Info.Nickname;

            string strNewNickname = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter the nickname you would like to use on {_serverCurrent.Info.Name}.",
                "Choose Nickname", "");

            if ((strNewNickname?.Length ?? 0) > 0)
                _serverCurrent.SetNickname(strNewNickname);
        }

        private void UserSetNameButton_Click(object sender, EventArgs e) {
            Core.Structures.UserProfile profile = _client.MyProfile;
            profile.DisplayName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the display name you would like to use.", "Choose Name",
                profile.DisplayName ?? "");
            UpdateUserProfile(profile);
        }

        private void UserSetBioButton_Click(object sender, EventArgs e) {
            Core.Structures.UserProfile profile = _client.MyProfile;
            profile.Bio = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the bio text you would like to have on your user profile.",
                "Choose Bio", profile.Bio ?? "");
            UpdateUserProfile(profile);
        }

        private void UserSetAvatarButton_Click(object sender, EventArgs e) {
            Core.Structures.UserProfile profile = _client.MyProfile;
            profile.AvatarURL = Microsoft.VisualBasic.Interaction.InputBox(
                "Paste or enter the URL to the avatar image you would like to use.",
                "Choose Avatar", profile.AvatarURL ?? "");
            UpdateUserProfile(profile);
        }

        private void UserLogOutButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show(this, "Are you sure you want to log out? " +
                "The application will exit and you will need to enter your " +
                "username and password when you open it again.",
                "Confirm Log Out", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                Meziantou.Framework.Win32.CredentialManager.DeleteCredential(APP_CODENAME);
                _client?.Dispose();
                Environment.Exit(0);
            }
        }

        private void UpdateUserProfile(Core.Structures.UserProfile profileNew) {
            _client.MyProfile = profileNew;
            Properties.Settings.Default.Profile = Newtonsoft.Json.JsonConvert.SerializeObject(profileNew);
            Properties.Settings.Default.Save();
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e) {
            _client?.Dispose();
            _client = null;
            GC.Collect();
        }
    }
}