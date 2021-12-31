using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;
using System.Collections.ObjectModel;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ServerControl : UserControl {
        public static readonly DirectProperty<ServerControl, IServer> ServerProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IServer>(nameof(Server), w => w.Server, (w, v) => w.Server = v);

        public static readonly DirectProperty<ServerControl, IChannel> ChannelProperty =
            AvaloniaProperty.RegisterDirect<ServerControl, IChannel>(nameof(Channel), w => w.Channel, (w, v) => w.Channel = v);

        private ObservableCollection<ChannelItem> _collChannels = new();
        private ObservableCollection<MemberItem> _collMembers = new();
        private TextBlock _ctlServerNameText, _ctlChannelNameText;

        public ServerControl() {
            InitializeComponent();

            this.FindControl<ItemsControl>("ChannelItems").Items = _collChannels;
            this.FindControl<ItemsControl>("MemberItems").Items = _collMembers;

            _ctlServerNameText = this.FindControl<TextBlock>("ServerNameText");
            _ctlChannelNameText = this.FindControl<TextBlock>("ChannelNameText");
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

        

        private class ChannelItem : AvaloniaObject {
            public static readonly DirectProperty<ChannelItem, string> NameProperty
                = AvaloniaProperty.RegisterDirect<ChannelItem, string>(nameof(Name), i => i.Name);

            public static readonly DirectProperty<ChannelItem, Core.Structures.ChannelType> ChannelTypeProperty
                = AvaloniaProperty.RegisterDirect<ChannelItem, Core.Structures.ChannelType>(nameof(ChannelType), i => i.ChannelType);

            public IChannel Channel;

            private string _strName;
            public string Name {
                get => _strName;
                set => SetAndRaise(NameProperty, ref _strName, value);
            }

            private Core.Structures.ChannelType _type;
            public Core.Structures.ChannelType ChannelType {
                get => _type;
                set => SetAndRaise(ChannelTypeProperty, ref _type, value);
            }
        }

        private class MemberItem : AvaloniaObject {
            public static readonly DirectProperty<MemberItem, string> NameProperty
                = AvaloniaProperty.RegisterDirect<MemberItem, string>(nameof(Name), i => i.Name);

            public static readonly DirectProperty<MemberItem, Avalonia.Media.Imaging.Bitmap> ProfileImageProperty
                = AvaloniaProperty.RegisterDirect<MemberItem, Avalonia.Media.Imaging.Bitmap>(nameof(ProfileImage), i => i.ProfileImage);

            public IMember Member;

            private string _strName;
            public string Name {
                get => _strName;
                set => SetAndRaise(NameProperty, ref _strName, value);
            }

            private Avalonia.Media.Imaging.Bitmap _bmProfile;
            public Avalonia.Media.Imaging.Bitmap ProfileImage {
                get => _bmProfile;
                set => SetAndRaise(ProfileImageProperty, ref _bmProfile, value);
            }
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == ServerProperty) && change.IsEffectiveValueChange)
                OnServerChanged();

            if ((change.Property == ChannelProperty) && change.IsEffectiveValueChange)
                OnChannelChanged();

            base.OnPropertyChanged(change);
        }

        private void OnServerChanged() {
            _collChannels.Clear();
            _collMembers.Clear();

            if (!(_server is null)) {
                _ctlServerNameText.Text = _server.Info.Name;

                foreach (IChannel channel in _server.Channels)
                    _collChannels.Add(new ChannelItem() { Channel = channel, ChannelType = channel.Info.Type, Name = channel.Info.Name });

                foreach (IMember member in _server.Members)
                    _collMembers.Add(new MemberItem() { Member = member, Name = member.DisplayName });
            }
        }

        private void OnChannelChanged() {
            _ctlChannelNameText.Text = _channel?.Info.Name ?? "";
        }
    }
}
