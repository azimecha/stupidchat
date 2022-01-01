using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class TextChannelControl : UserControl {
        public static readonly DirectProperty<TextChannelControl, IChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<TextChannelControl, IChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        public static readonly DirectProperty<TextChannelControl, string> SendBoxTextProperty =
            AvaloniaProperty.RegisterDirect<TextChannelControl, string>(nameof(SendBoxText), w => w.SendBoxText, (w, v) => w.SendBoxText = v);

        public static readonly DirectProperty<TextChannelControl, bool> SendButtonAvailableProperty =
            AvaloniaProperty.RegisterDirect<TextChannelControl, bool>(nameof(SendButtonAvailable), w => w.SendButtonAvailable);

        private StackPanel _ctlMessagesStack;
        private BackgroundWorker _wkrDownloadMessages;

        public TextChannelControl() {
            InitializeComponent();

            _ctlMessagesStack = this.Find<StackPanel>("MessagesStack");

            _wkrDownloadMessages = new BackgroundWorker() {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _wkrDownloadMessages.DoWork += DownloadMessagesWorker_DoWork;
            _wkrDownloadMessages.RunWorkerCompleted += DownloadMessagesWorker_RunWorkerCompleted;
            _wkrDownloadMessages.ProgressChanged += DownloadMessagesWorker_ProgressChanged;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IChannel _channel;
        public IChannel Channel {
            get => _channel;
            set => SetAndRaise(ChannelProperty, ref _channel, value);
        }

        private string _strSendBoxText;
        public string SendBoxText {
            get => _strSendBoxText;
            set => SetAndRaise(SendBoxTextProperty, ref _strSendBoxText, value);
        }

        private bool _bSendButtonAvail;
        public bool SendButtonAvailable {
            get => _bSendButtonAvail;
            private set => SetAndRaise(SendButtonAvailableProperty, ref _bSendButtonAvail, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if (change.IsEffectiveValueChange) {
                if (change.Property == ChannelProperty)
                    OnChannelChanged();
                if (change.Property == SendBoxTextProperty)
                    UpdateSendAvailable();
            }

            base.OnPropertyChanged(change);
        }

        private void OnChannelChanged() {
            _ctlMessagesStack.Children.Clear();

            if (!(_channel is null))
                _wkrDownloadMessages.RunWorkerAsync(new MessagesDownloadParams() { Count = 10, StartingOffset = 0 });

            UpdateSendAvailable();
        }

        private void UpdateSendAvailable() {
            SendButtonAvailable = ((SendBoxText?.Length ?? 0) > 0) && !(_channel is null);
        }

        private struct MessagesDownloadParams {
            public int Count;
            public int StartingOffset;
        }

        private void DownloadMessagesWorker_DoWork(object sender, DoWorkEventArgs e) {
            MessagesDownloadParams args = (MessagesDownloadParams)e.Argument;
            foreach (IMessage msg in _channel.Messages.Skip(args.StartingOffset).Take(args.Count))
                _wkrDownloadMessages.ReportProgress(0, msg);
        }

        private void DownloadMessagesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error is Exception ex)
                new MessageDialog() { Title = "Error Downloading", MessageText = $"Error downloading messages:\n{ex}" }.Show();
        }

        private void DownloadMessagesWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.UserState is IMessage msg)
                _ctlMessagesStack.Children.Insert(0, msg.Deleted 
                    ? new TextBlock() { Text = "(deleted message)", Opacity = 0.5 } 
                    : new MessageControl() { Message = msg });
        }

        private void SendButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e)
            => SendMessage();


        private void SendBox_KeyDown(object objSender, Avalonia.Input.KeyEventArgs e) {
            if (e.Key == Avalonia.Input.Key.Return)
                SendMessage();
        }

        private void SendMessage() {
            if ((SendBoxText?.Length ?? 0) == 0) return;

            try {
                _channel.PostMessage(new Core.Structures.MessageSignedData() {
                    SendTime = DateTime.Now.Ticks,
                    Text = SendBoxText
                });
                SendBoxText = "";
            } catch (Exception ex) {
                Debug.WriteLine($"[{nameof(TextChannelControl)}] Error sending message \"{SendBoxText}\": {ex}");
                new MessageDialog() { Title = "Error Sending", MessageText = $"Error sending message:\n{ex.Message} ({ex.GetType().FullName})" }.Show();
            }
        }
    }
}
