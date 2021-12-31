using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ServerControl : UserControl {
        public static readonly DirectProperty<ServerControl, IServer> ServerProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IServer>(nameof(Server), w => w.Server, (w, v) => w.Server = v);

        public static readonly DirectProperty<ServerControl, IChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        public ServerControl() {
            InitializeComponent();
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
    }
}
