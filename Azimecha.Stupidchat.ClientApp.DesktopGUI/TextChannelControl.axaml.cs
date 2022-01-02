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

        private ScrollViewer _ctlMessagesScrollViewer;
        private StackPanel _ctlMessagesStack;
        private BackgroundWorker _wkrDownloadMessages;
        private Avalonia.Threading.DispatcherTimer _tmrScrollToEnd;
        private Avalonia.Threading.DispatcherTimer _tmrScrollDown;

        private bool _bInitialLoadComplete, _bReachedMessageZero;
        private double _fScrollDownAmount = 0.0;

        private Action<IMessage> _procOnMessagePosted;
        private Action<IMessage, IMessage> _procOnMessageDeleted;

        public TextChannelControl() {
            InitializeComponent();

            _ctlMessagesScrollViewer = this.Find<ScrollViewer>("MessagesScrollViewer");
            _ctlMessagesStack = this.Find<StackPanel>("MessagesStack");

            _wkrDownloadMessages = new BackgroundWorker() {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _wkrDownloadMessages.DoWork += DownloadMessagesWorker_DoWork;
            _wkrDownloadMessages.RunWorkerCompleted += DownloadMessagesWorker_RunWorkerCompleted;
            _wkrDownloadMessages.ProgressChanged += DownloadMessagesWorker_ProgressChanged;

            _procOnMessagePosted = new Action<IMessage>(OnMessagePosted);
            _procOnMessageDeleted = new Action<IMessage, IMessage>(OnMessageDeleted);

            _tmrScrollToEnd = new Avalonia.Threading.DispatcherTimer(TimeSpan.FromMilliseconds(100), Avalonia.Threading.DispatcherPriority.ApplicationIdle,
                ScrollToEndTimer_Tick);

            _tmrScrollDown = new Avalonia.Threading.DispatcherTimer(TimeSpan.FromMilliseconds(100), Avalonia.Threading.DispatcherPriority.ApplicationIdle,
                ScrollDownTimer_Tick);
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
                    OnChannelChanged(change.OldValue.HasValue ? (IChannel)change.OldValue.Value : null);
                if (change.Property == SendBoxTextProperty)
                    UpdateSendAvailable();
            }

            base.OnPropertyChanged(change);
        }

        private void OnChannelChanged(IChannel chanOld) {
            _ctlMessagesStack.Children.Clear();
            _bInitialLoadComplete = false;
            _bReachedMessageZero = false;

            if (!(chanOld is null)) {
                chanOld.MessagePosted -= _procOnMessagePosted;
                chanOld.MessageDeleted -= _procOnMessageDeleted;
            }

            if (!(_channel is null)) {
                _channel.MessagePosted += _procOnMessagePosted;
                _channel.MessageDeleted += _procOnMessageDeleted;
                _wkrDownloadMessages.RunWorkerAsync(new MessagesDownloadParams() { Count = 10, StartingOffset = 0 });
            }

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

            if (!_bInitialLoadComplete)
                _tmrScrollToEnd.Start();
            else
                _ctlMessagesScrollViewer.Offset += new Vector(0.0, 100.0);
        }

        private void DownloadMessagesWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (e.UserState is IMessage msg)
                AddControlForMessage(msg);
        }

        private void AddControlForMessage(IMessage msg) {
            int nIndex = 0;

            foreach (IControl ctlCur in _ctlMessagesStack.Children) {
                long nMessageIndex;

                if ((ctlCur is MessageControl ctlCurMessage) && (ctlCurMessage.Message is IMessage msgCur))
                    nMessageIndex = msgCur.IndexInChannel;
                else if ((ctlCur is TextBlock ctlTombstone) && (ctlTombstone.Tag is long nTombstoneMessageIndex))
                    nMessageIndex = nTombstoneMessageIndex;
                else
                    nMessageIndex = -1;

                if (nMessageIndex > msg.IndexInChannel)
                    break;

                nIndex++;
            }

            Control ctlMessage = msg.IsDeletedMessageTombstone
                ? new TextBlock() { Text = "(deleted message)", Opacity = 0.5, Tag = msg.IndexInChannel }
                : new MessageControl() { Message = msg };

            _ctlMessagesStack.Children.Insert(nIndex, ctlMessage);

            if (msg.IndexInChannel == 0)
                _bReachedMessageZero = true;

            if (_bInitialLoadComplete) {
                _fScrollDownAmount += ctlMessage.Height;
                _tmrScrollDown.IsEnabled = true;
            }
        }

        private void RemoveControlForMessage(IMessage msg)
            => _ctlMessagesStack.Children.RemoveAll(_ctlMessagesStack.Children.OfType<MessageControl>()
                .Where(ctl => ctl.Message?.IndexInChannel == msg.IndexInChannel).ToArray());

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

        private void OnMessagePosted(IMessage msg) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            AddControlForMessage(msg);
            _tmrScrollToEnd.Start();
        });

        private void OnMessageDeleted(IMessage msgRemoved, IMessage msgTombstone) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            RemoveControlForMessage(msgRemoved);
            AddControlForMessage(msgTombstone);
        });

        // so far i haven't been able to find an event that fires _after_ the scrollviewer determines the size of its content
        // (it doesn't do this immediately after things are added, or even by the time Initialized fires)
        // this timer method is bad, but so is everything about the scrollviewer control
        private void MessagesScrollViewer_Initialized(object objSender, EventArgs e) { }

        private void ScrollToEndTimer_Tick(object objSender, EventArgs e) {
            _tmrScrollToEnd.Stop();
            _ctlMessagesScrollViewer.ScrollToEnd();

            // don't set initial load complete until we've scrolled to the end
            // otherwise ScrollChanged will try to load more old messages
            if (!_bInitialLoadComplete)
                _bInitialLoadComplete = true;
        }

        private void MessagesScrollViewer_ScrollChanged(object objSender, ScrollChangedEventArgs e) {
            if (_ctlMessagesScrollViewer.Offset.Y == 0.0) {
                if (_bInitialLoadComplete && !_bReachedMessageZero && !_wkrDownloadMessages.IsBusy) {
                    _wkrDownloadMessages.RunWorkerAsync(new MessagesDownloadParams() { 
                        Count = 10, 
                        StartingOffset = _ctlMessagesStack.Children.Count 
                    });
                }
            }
        }

        private void ScrollDownTimer_Tick(object objSender, EventArgs e) {
            _tmrScrollDown.Stop();

            // this line has been disabled because it causes a stack overflow
            // the stack overflow is entirely composed of frames inside Avalonia DLLs
            // it's almost certainly a bug in Avalonia
            //_ctlMessagesScrollViewer.Offset += new Vector(0, _fScrollDownAmount);

            _fScrollDownAmount = 0.0;
        }
    }
}
