using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class MainWindow : Window {
        private ChatClient _client;
        private BackgroundWorker _wkrNewConnection, _wkrRestoreConnections;
        private StackPanel _ctlServersStack;
        private string _strNewConnectionURL;
        private ServerControl _ctlServer;
        private Border _ctlServerBorder;
        private Button _ctlMenuButton;
        private TextBlock _ctlSubtitleText;

        // init phase 0
        public MainWindow() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            _wkrNewConnection = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _wkrNewConnection.WorkerSupportsCancellation = true;
            _wkrNewConnection.DoWork += NewConnectionWorker_DoWork;
            _wkrNewConnection.RunWorkerCompleted += NewConnectionWorker_RunWorkerCompleted;

            _wkrRestoreConnections = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _wkrRestoreConnections.WorkerReportsProgress = true;
            _wkrRestoreConnections.WorkerSupportsCancellation = true;
            _wkrRestoreConnections.DoWork += RestoreConnectionsWorker_DoWork;
            _wkrRestoreConnections.RunWorkerCompleted += RestoreConnectionsWorker_RunWorkerCompleted;
            _wkrRestoreConnections.ProgressChanged += RestoreConnectionsWorker_ProgressChanged;

            _ctlServersStack = this.FindControl<StackPanel>("ServersStack");
            _ctlServerBorder = this.FindControl<Border>("ServerBorder");
            _ctlMenuButton = this.FindControl<Button>("MenuButton");
            _ctlSubtitleText = this.FindControl<TextBlock>("SubtitleText");

            _ctlServer = new ServerControl();
            _ctlServer.Disconnected += ServerControl_Disconnected;

            Settings.Instance.ProfileChanged += Settings_ProfileChanged;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        // init phase 1
        private void MainWindow_Opened(object sender, EventArgs e) {
            byte[] arrPrivateKey = SecureSettings.GetPrivateKey();
            if (arrPrivateKey is null)
                new UsernamePasswordDialog() { 
                    PromptText = "Enter your Stupidchat username and password.\n\n"
                        + "If you don't already have an account, come up with a username you like and choose a strong password.",
                    Username = Settings.Instance.Username,
                    Height = 280,
                    CanResize = false
                }.ShowDialog<UsernamePasswordDialog.LoginInformation?>(this, InitFromNewLoginInfo);
            else
                FinishInitialization(arrPrivateKey);
        }

        // init phase 2 (may be bypassed)
        private void InitFromNewLoginInfo(UsernamePasswordDialog.LoginInformation? info) {
            if (!info.HasValue) // cancelled
                Environment.Exit(0);

            Core.Cryptography.IKeyDerivationAlgorithm algoDeriv = new Core.Cryptography.Argon2i();

            byte[] arrPrivateKey = new byte[Core.NetworkConnection.CreateSigningAlgorithmInstance().PrivateKeySize];
            algoDeriv.DeriveKey(arrPrivateKey, System.Text.Encoding.UTF8.GetBytes(info.Value.Password),
                System.Text.Encoding.UTF8.GetBytes(info.Value.Username));

            SecureSettings.SetPrivateKey(arrPrivateKey);
            Settings.Instance.Username = info.Value.Username;
            Settings.Instance.Save();

            FinishInitialization(arrPrivateKey);
        }

        // init phase 3
        private void FinishInitialization(byte[] arrPrivateKey) {
            _client = new ChatClient(arrPrivateKey);
            _client.MyProfile = Settings.Instance.Profile;

            if (Settings.Instance.Servers.Count > 0)
                _wkrRestoreConnections.RunWorkerAsync(Settings.Instance.Servers.ToArray());
            else
                PostInitialization();
        }

        // init phase 4 (after restore if applicable)
        private void PostInitialization() {
            if (_ctlServer.Server is null)
                _ctlSubtitleText.Text = "Press \xFF0B to add a server";
        }

        private struct RestorationResult {
            public KnownServer ServerBasicInfo;
            public Exception Error;
            public IServer Result;
        }

        private class ServerKeyMismatchException : Exception {
            public ServerKeyMismatchException() : base("The server's current key does not match its known key.") { }
        }

        // started by FinishInitialization
        private void RestoreConnectionsWorker_DoWork(object sender, DoWorkEventArgs e) {
            KnownServer[] arrServers = (KnownServer[])e.Argument;

            foreach (KnownServer ks in arrServers) {
                if (_wkrRestoreConnections.CancellationPending)
                    throw new OperationCanceledException();

                try {
                    IServer server = _client.ConnectToServer(ks.Address, ks.Port);
                    if (!server.ID.SequenceEqual(ks.PublicKey)) {
                        server.Disconnect();
                        throw new ServerKeyMismatchException();
                    }
                    _wkrRestoreConnections.ReportProgress(0, new RestorationResult() { ServerBasicInfo = ks, Result = server });
                } catch (Exception ex) {
                    _wkrRestoreConnections.ReportProgress(0, new RestorationResult() { ServerBasicInfo = ks, Error = ex });
                }
            }
        }

        private void RestoreConnectionsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (!(e.Error is null))
                MessageDialog.ShowMessage(this, "Connection Restoration Error", $"Error reconnecting to known servers: {e.Error}");
            else
                Debug.WriteLine($"[{nameof(MainWindow)}] Connection restoration complete");

            PostInitialization();
        }

        private void RestoreConnectionsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            RestorationResult result = (RestorationResult)e.UserState;

            if (!(result.Error is null)) {
                Debug.WriteLine($"[{nameof(MainWindow)}] Error reconnecting to {result.ServerBasicInfo.PublicKey} on {result.ServerBasicInfo.Address}:"
                    + $"{result.ServerBasicInfo.Port}:\n{result.Error}");
                MessageDialog.ShowMessage(this, "Connection Error", $"Error reconnecting to {result.ServerBasicInfo.Address}:{result.ServerBasicInfo.Port}:\n"
                    + $"{result.Error.Message} ({result.Error.GetType().FullName})");
                Settings.Instance.Servers.Remove(result.ServerBasicInfo);
                Settings.Instance.Save();
            }

            if (!(result.Result is null))
                FinishAddingServer(result.Result);
        }

        private void AddServer_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            TextEntryDialog dlg = new();
            dlg.Title = "Connect to Server";
            dlg.PromptText = "Enter the address of the server you would like to connect to.";
            dlg.PlaceholderText = "chatt://server.com:1234";
            dlg.ShowDialog<string>(this, AddServerDialogCompleted);
        }

        private void AddServerDialogCompleted(string strAddress) {
            if (strAddress is null) return;

            Uri uriAddress;
            try {
                uriAddress = new Uri(strAddress);
                Debug.WriteLine($"Protocol: {uriAddress.Scheme}, Host: {uriAddress.Host}, Port: {uriAddress.Port}, Additional: {uriAddress.PathAndQuery}");
            } catch (Exception ex) {
                MessageDialog.ShowMessage(this, "Invalid URL", $"Error parsing URL {strAddress}.\n\n{ex.Message} ({ex.GetType().FullName})"
                    + "\n\nMake sure you entered or pasted the URL correctly.");
                return;
            }

            if ((uriAddress.Host?.Length ?? 0) == 0) {
                MessageDialog.ShowMessage(this, "Invalid URL", $"The URL \"{strAddress}\" does not contain a hostname."
                    + "\n\nMake sure you entered or pasted the URL correctly.");
                return;
            }

            switch (uriAddress.Scheme.ToLowerInvariant()) {
                case "chatt":
                    break;

                default:
                    MessageDialog.ShowMessage(this, "Invalid URL", $"The {uriAddress.Scheme.ToUpper()} protocol is not a supported chat protocol."
                        + "\n\nMake sure you entered or pasted the URL correctly.");
                    return;
            }

            _strNewConnectionURL = strAddress;
            _wkrNewConnection.RunWorkerAsync(uriAddress);
        }

        private void NewConnectionWorker_DoWork(object sender, DoWorkEventArgs e) {
            Uri uriAddress = (Uri)e.Argument;

            int nPort = uriAddress.Port;
            if (nPort < 0) nPort = Core.Protocol.ProtocolConstants.DEFAULT_PORT;

            e.Result = _client.ConnectToServer(uriAddress.Host, nPort);
        }

        private void NewConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error is Exception ex) {
                Debug.WriteLine($"[{nameof(MainWindow)}] Error connecting to \"{_strNewConnectionURL}\":\n{ex}");
                MessageDialog.ShowMessage(this, "Connection Error", $"Error connecting to {_strNewConnectionURL}:\n{ex.Message} ({ex.GetType().FullName})");
            } else if (e.Result is IServer server) {
                FinishAddingServer(server);
                Settings.Instance.Servers.Add(server.ToKnownServer());
                Settings.Instance.Save();
            }
        }

        private void FinishAddingServer(IServer server) {
            _ctlServersStack.Children.Add(new Button() { Content = server.Info.Name, Tag = server });
            _ctlServer.Server = server;
            _ctlServer.Tag = server.ToKnownServer();
            _ctlServerBorder.Child = _ctlServer;
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            _client?.Dispose();
            _wkrRestoreConnections.CancelAsync();
            _wkrRestoreConnections.Dispose();
            _wkrNewConnection.CancelAsync();
            _wkrNewConnection.Dispose();
        }

        private void Settings_ProfileChanged(Core.Structures.UserProfile profile) {
            if (_client is not null)
                _client.MyProfile = profile;
        }

        private void MenuButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            ContextMenu menu = _ctlMenuButton.ContextMenu;
            menu.PlacementTarget = _ctlMenuButton;
            menu.Open();
        }

        private void EditProfileItem_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) { }

        private void LogOutItem_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Settings.Discard();
            SecureSettings.Discard();
            Environment.Exit(0);
        }

        private void ServerControl_Disconnected(ServerControl ctlServer) {
            _ctlServerBorder.Child = new TextBlock() {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Text = "Disconnected"
            };
        }
    }
}
