using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;
using Azimecha.Stupidchat.Core.Structures;
using System;
using System.Diagnostics;
using System.Linq;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class VoiceChannelControl : UserControl {
        public static readonly DirectProperty<VoiceChannelControl, IVoiceChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<VoiceChannelControl, IVoiceChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        public static readonly DirectProperty<VoiceChannelControl, string> SendBoxTextProperty =
            AvaloniaProperty.RegisterDirect<VoiceChannelControl, string>(nameof(SendBoxText), w => w.SendBoxText, (w, v) => w.SendBoxText = v);

        public static readonly DirectProperty<VoiceChannelControl, bool> SendButtonAvailableProperty =
            AvaloniaProperty.RegisterDirect<VoiceChannelControl, bool>(nameof(SendButtonAvailable), w => w.SendButtonAvailable);

        private WrapPanel _ctlParticipantsPanel;
        private ScrollViewer _ctlMessagesScrollViewer;
        private StackPanel _ctlMessagesStack;
        private Avalonia.Threading.DispatcherTimer _tmrScrollToEnd;

        private Action<IMember> _procParticipantEntered, _procParticipantLeft;
        private Action<IMember, VCSubchannelMask> _procStartTx, _procStopTx, _procStartRx, _procStopRx;
        private Action<IMember, VCSubchannelMask, Memory<byte>> _procDataRx;

        public VoiceChannelControl() {
            InitializeComponent();

            _procParticipantEntered = new Action<IMember>(Channel_ParticipantEntered);
            _procParticipantLeft = new Action<IMember>(Channel_ParticipantLeft);
            _procStartTx = new Action<IMember, VCSubchannelMask>(Channel_ParticipantStartedTransmitting);
            _procStopTx = new Action<IMember, VCSubchannelMask>(Channel_ParticipantStoppedTransmitting);
            _procStartRx = new Action<IMember, VCSubchannelMask>(Channel_ParticipantStartedReceiving);
            _procStopRx = new Action<IMember, VCSubchannelMask>(Channel_ParticipantStoppedReceiving);
            _procDataRx = new Action<IMember, VCSubchannelMask, Memory<byte>>(Channel_ParticipantSentData);

            _ctlParticipantsPanel = this.Find<WrapPanel>("ParticipantsPanel");
            _ctlMessagesScrollViewer = this.Find<ScrollViewer>("MessagesScrollViewer");
            _ctlMessagesStack = this.Find<StackPanel>("MessagesStack");

            _tmrScrollToEnd = new Avalonia.Threading.DispatcherTimer(TimeSpan.FromMilliseconds(100), Avalonia.Threading.DispatcherPriority.ApplicationIdle,
                ScrollToEndTimer_Tick);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IVoiceChannel _channel;
        public IVoiceChannel Channel {
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
                    OnChannelChanged(change.OldValue.HasValue ? (IVoiceChannel)change.OldValue.Value : null);
                if (change.Property == SendBoxTextProperty)
                    UpdateSendAvailable();
            }

            base.OnPropertyChanged(change);
        }

        private void OnChannelChanged(IVoiceChannel chanOld) {
            _ctlMessagesStack.Children.Clear();
            _ctlParticipantsPanel.Children.Clear();

            if (!(chanOld is null)) {
                chanOld.Leave();

                chanOld.ParticipantEntered -= _procParticipantEntered;
                chanOld.ParticipantLeft -= _procParticipantLeft;
                chanOld.ParticipantStartedTransmitting -= _procStartTx;
                chanOld.ParticipantStoppedTransmitting -= _procStopTx;
                chanOld.ParticipantStartedReceiving -= _procStartRx;
                chanOld.ParticipantStoppedReceiving -= _procStopRx;
                chanOld.ParticipantSentData -= _procDataRx;
            }

            if (!(Channel is null)) {
                foreach (VoiceParticipantInfo inf in Channel.Participants) {
                    VoiceParticipantControl ctlParticipant = new VoiceParticipantControl() { Member = inf.Member };
                    _ctlParticipantsPanel.Children.Add(ctlParticipant);
                }

                Channel.ParticipantEntered += _procParticipantEntered;
                Channel.ParticipantLeft += _procParticipantLeft;
                Channel.ParticipantStartedTransmitting += _procStartTx;
                Channel.ParticipantStoppedTransmitting += _procStopTx;
                Channel.ParticipantStartedReceiving += _procStartRx;
                Channel.ParticipantStoppedReceiving += _procStopRx;
                Channel.ParticipantSentData += _procDataRx;

                Channel.Join();
                Channel.StartReceiving(VCSubchannelMask.NonpersistentText);
                Channel.StartTransmitting(VCSubchannelMask.NonpersistentText);
            }

            UpdateSendAvailable();
        }

        private void UpdateSendAvailable() {
            SendButtonAvailable = ((SendBoxText?.Length ?? 0) > 0) && !(_channel is null);
        }

        private void ScrollToEndTimer_Tick(object objSender, EventArgs e) {
            _tmrScrollToEnd.Stop();
            _ctlMessagesScrollViewer.ScrollToEnd();
        }

        private void Channel_ParticipantEntered(IMember memb) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            _ctlParticipantsPanel.Children.Add(new VoiceParticipantControl() { Member = memb });
        });

        private void Channel_ParticipantLeft(IMember memb) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            _ctlParticipantsPanel.Children.RemoveAll(_ctlParticipantsPanel.Children.OfType<VoiceParticipantControl>().Where(c => c.Member == memb));
        });

        private void Channel_ParticipantStartedTransmitting(IMember memb, VCSubchannelMask maskSubchannels) {
            Debug.WriteLine($"[{nameof(VoiceChannelControl)}] TX start: {memb.User.Profile.Username} 0x{maskSubchannels:X}");
        }

        private void Channel_ParticipantStoppedTransmitting(IMember memb, VCSubchannelMask maskSubchannels) {
            Debug.WriteLine($"[{nameof(VoiceChannelControl)}] TX end: {memb.User.Profile.Username} 0x{maskSubchannels:X}");
        }

        private void Channel_ParticipantStartedReceiving(IMember memb, VCSubchannelMask maskSubchannels) {
            Debug.WriteLine($"[{nameof(VoiceChannelControl)}] RX start: {memb.User.Profile.Username} 0x{maskSubchannels:X}");
        }

        private void Channel_ParticipantStoppedReceiving(IMember memb, VCSubchannelMask maskSubchannels) {
            Debug.WriteLine($"[{nameof(VoiceChannelControl)}] RX end: {memb.User.Profile.Username} 0x{maskSubchannels:X}");
        }

        private void Channel_ParticipantSentData(IMember memb, VCSubchannelMask maskSubchannels, Memory<byte> memData) {
            if ((maskSubchannels & VCSubchannelMask.NonpersistentText) != 0) {
                string strText = System.Text.Encoding.UTF8.GetString(memData.ToArray());
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => AddMessage(memb, strText));
            }
        }

        private void SendButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e)
            => SendMessage();


        private void SendBox_KeyDown(object objSender, Avalonia.Input.KeyEventArgs e) {
            if (e.Key == Avalonia.Input.Key.Return)
                SendMessage();
        }

        private void SendMessage() {
            if ((SendBoxText?.Length ?? 0) == 0) return;
            _channel.SendData(VCSubchannelMask.NonpersistentText, System.Text.Encoding.UTF8.GetBytes(SendBoxText));
            AddMessage(_channel.Server.Me, SendBoxText);
            SendBoxText = "";
        }

        private void AddMessage(IMember memb, string strText) {
            _ctlMessagesStack.Children.Add(new Label() { Content = $"{memb.DisplayName}: {strText}" });
        }
    }
}
