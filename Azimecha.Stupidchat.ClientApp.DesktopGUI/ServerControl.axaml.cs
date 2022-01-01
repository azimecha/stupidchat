using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Core;
using Azimecha.Stupidchat.Client;
using System.Collections.ObjectModel;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ServerControl : UserControl {
        public static readonly DirectProperty<ServerControl, IServer> ServerProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IServer>(nameof(Server), w => w.Server, (w, v) => w.Server = v);

        public static readonly DirectProperty<ServerControl, IChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        private TextBlock _ctlServerNameText, _ctlChannelNameText;
        private StackPanel _ctlChannelsStack, _ctlMembersStack;
        private Border _ctlChannelBorder;

        public ServerControl() {
            InitializeComponent();

            _ctlServerNameText = this.FindControl<TextBlock>("ServerNameText");
            _ctlChannelNameText = this.FindControl<TextBlock>("ChannelNameText");
            _ctlChannelsStack = this.FindControl<StackPanel>("ChannelsStack");
            _ctlMembersStack = this.FindControl<StackPanel>("MembersStack");
            _ctlChannelBorder = this.FindControl<Border>("ChannelBorder");
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

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == ServerProperty) && change.IsEffectiveValueChange)
                OnServerChanged();

            if ((change.Property == ChannelProperty) && change.IsEffectiveValueChange)
                OnChannelChanged();

            base.OnPropertyChanged(change);
        }

        private void OnServerChanged() {
            _ctlServerNameText.Text = _server?.Info.Name ?? "";

            _channel = null;
            _ctlChannelsStack.Children.Clear();
            _ctlMembersStack.Children.Clear();

            if (!(_server is null)) {
                foreach (IChannel chan in _server.Channels) {
                    Button btn = new Button() { Content = chan.Info.Name, Tag = chan };
                    btn.Click += ChannelButton_Click;
                    _ctlChannelsStack.Children.Add(btn);
                }

                foreach (IMember memb in _server.Members) {
                    Button btn = new Button() { Content = memb.DisplayName, Tag = memb };
                    btn.Click += MemberButton_Click;
                    _ctlMembersStack.Children.Add(btn);
                }
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
            if ((objSender is Button btn) && (btn.Tag is IMember memb))
                new MessageDialog() { Title = "User Info", MessageText = memb.User.Profile.ToDataString("\n", ": ") }.Show();
        }
    }
}
