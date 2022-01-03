using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Client;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Avalonia.Controls.Primitives;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ServerControl : UserControl {
        public static readonly DirectProperty<ServerControl, IServer> ServerProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IServer>(nameof(Server), w => w.Server, (w, v) => w.Server = v);

        public static readonly DirectProperty<ServerControl, IChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        private TextBlock _ctlServerNameText, _ctlChannelNameText;
        private StackPanel _ctlChannelsStack, _ctlMembersStack;
        private Border _ctlChannelBorder;
        private Popup _ctlMemberPopup;
        private ProfileControl _ctlMemberProfile;

        private Action<IServer, Exception> _procOnError;
        private Action<IServer> _procOnDisposed;
        private Action<IMember> _procOnMemberAdded, _procOnMemberChanged, _procOnMemberRemoved;
        private Action<IChannel> _procOnChannelAdded, _procOnChannelModified, _procOnChannelRemoved;

        public ServerControl() {
            InitializeComponent();

            _ctlServerNameText = this.FindControl<TextBlock>("ServerNameText");
            _ctlChannelNameText = this.FindControl<TextBlock>("ChannelNameText");
            _ctlChannelsStack = this.FindControl<StackPanel>("ChannelsStack");
            _ctlMembersStack = this.FindControl<StackPanel>("MembersStack");
            _ctlChannelBorder = this.FindControl<Border>("ChannelBorder");
            _ctlMemberPopup = this.FindControl<Popup>("MemberPopup");

            _ctlMemberProfile = new ProfileControl();
            _ctlMemberPopup.Child = _ctlMemberProfile;

            _procOnError = new Action<IServer, Exception>(OnServerErrored);
            _procOnDisposed = new Action<IServer>(OnServerDisposed);

            _procOnMemberAdded = new Action<IMember>(OnMemberAdded);
            _procOnMemberChanged = new Action<IMember>(OnMemberChanged);
            _procOnMemberRemoved = new Action<IMember>(OnMemberRemoved);

            _procOnChannelAdded = new Action<IChannel>(OnChannelAdded);
            _procOnChannelModified = new Action<IChannel>(OnChannelModified);
            _procOnChannelRemoved = new Action<IChannel>(OnChannelRemoved);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IServer _server;
        public IServer Server {
            get => _server;
            set => SetAndRaise(ServerProperty, ref _server, value);
        }

        private IChannel _channel;
        public IChannel Channel {
            get => _channel;
            set => SetAndRaise(ChannelProperty, ref _channel, value);
        }

        public event Action<ServerControl> Disconnected;

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == ServerProperty) && change.IsEffectiveValueChange)
                OnServerChanged(change.OldValue.HasValue ? (IServer)change.OldValue.Value : null);

            if ((change.Property == ChannelProperty) && change.IsEffectiveValueChange)
                OnChannelChanged();

            base.OnPropertyChanged(change);
        }

        private void OnServerChanged(IServer serverOld) {
            _ctlServerNameText.Text = _server?.Info.Name ?? "";

            _channel = null;
            _ctlChannelsStack.Children.Clear();
            _ctlMembersStack.Children.Clear();

            if (serverOld is not null) {
                serverOld.MemberJoined -= _procOnMemberAdded;
                serverOld.MemberLeft -= _procOnMemberRemoved;
                serverOld.ChannelAdded -= _procOnChannelAdded;
                serverOld.ChannelRemoved -= _procOnChannelRemoved;
            }

            if (_server is not null) {
                foreach (IChannel chan in _server.Channels)
                    AddChannelControl(chan);

                foreach (IMember memb in _server.Members)
                    AddMemberControl(memb);

                _server.MemberJoined += _procOnMemberAdded;
                _server.MemberLeft += _procOnMemberRemoved;

                _server.ChannelAdded += _procOnChannelAdded;
                _server.ChannelRemoved += _procOnChannelRemoved;

                if (_server.Channels.FirstOrDefault() is IChannel chanFirst)
                    Channel = chanFirst;
            }
        }

        private void OnChannelChanged() {
            _ctlChannelNameText.Text = _channel?.Info.Name ?? "";
            _ctlChannelBorder.Child = (_channel is null) ? null : new TextChannelControl() { Channel = _channel };
        }

        private void ChannelButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs args) {
            if ((objSender is Button btn) && (btn.Tag is IChannel chan))
                Channel = chan;
        }

        private void MemberButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs args) {
            if ((objSender is Button btn) && (btn.Tag is IMember memb)) {
                _ctlMemberPopup.PlacementTarget = btn;
                _ctlMemberProfile.User = memb.User;
                _ctlMemberPopup.Open();
            }
        }

        private void AddMemberControl(IMember memb) {
            Button btn = new Button() { Content = memb.DisplayName, Tag = memb };
            btn.Click += MemberButton_Click;
            _ctlMembersStack.Children.Add(btn);
            memb.Changed += _procOnMemberChanged;
        }

        private void AddChannelControl(IChannel chan) {
            Button btn = new Button() { Content = chan.Info.Name, Tag = chan };
            btn.Click += ChannelButton_Click;
            _ctlChannelsStack.Children.Add(btn);
            chan.InfoChanged += _procOnChannelModified;
        }

        private void OnMemberAdded(IMember memb) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            AddMemberControl(memb);
        }).Wait();

        private void OnMemberChanged(IMember memb) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            Button ctlMember = _ctlMembersStack.Children.OfType<Button>().Where(ctl => ReferenceEquals(ctl.Tag, memb)).FirstOrDefault();
            if (ctlMember is not null)
                ctlMember.Content = memb.DisplayName;
        });

        private void OnMemberRemoved(IMember memb) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            _ctlMembersStack.Children.RemoveAll(_ctlMembersStack.Children.OfType<Button>().Where(ctl => ReferenceEquals(ctl.Tag, memb)).ToArray());
            memb.Changed -= _procOnMemberChanged;
        });

        private void OnChannelAdded(IChannel chan) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            AddChannelControl(chan);
        }).Wait();

        private void OnChannelModified(IChannel chan) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            Button ctlChannel = _ctlChannelsStack.Children.OfType<Button>().Where(ctl => ReferenceEquals(ctl.Tag, chan)).FirstOrDefault();
            if (ctlChannel is not null)
                ctlChannel.Content = chan.Info.Name;
        });

        private void OnChannelRemoved(IChannel chan) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            _ctlChannelsStack.Children.RemoveAll(_ctlChannelsStack.Children.OfType<Button>()
                .Where(ctl => (ctl.Tag is IChannel chanCur) && chanCur.Info.ID == chan.Info.ID).ToArray());
            chan.InfoChanged -= _procOnChannelModified;
        });

        private void OnServerErrored(IServer server, Exception ex) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            MessageDialog.ShowMessage(this.GetWindow(), "Connection Error", $"Error in connection to {server.Info.Name}:\n"
                + $"{ex.Message} ({ex.GetType().FullName})");
        });

        private void OnServerDisposed(IServer server) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            Server = null;
            Disconnected?.Invoke(this);
        });
    }
}
