using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azimecha.Stupidchat.Client;
using System.Collections.Generic;
using System.Linq;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ProfileControl : UserControl {
        public static readonly DirectProperty<ProfileControl, IUser> UserProperty =
            AvaloniaProperty.RegisterDirect<ProfileControl, IUser>(nameof(User), w => w.User, (w, v) => w.User = v);

        public static readonly DirectProperty<ProfileControl, string> UsernameProperty =
            AvaloniaProperty.RegisterDirect<ProfileControl, string>(nameof(Username), w => w.Username);

        public static readonly DirectProperty<ProfileControl, Membership[]> MembershipsProperty =
            AvaloniaProperty.RegisterDirect<ProfileControl, Membership[]>(nameof(Memberships), w => w.Memberships);


        private Border _ctlAvatarBorder;
        private AvatarControl _ctlAvatar;

        public ProfileControl() {
            InitializeComponent();

            _ctlAvatarBorder = this.FindControl<Border>("AvatarBorder");
            _ctlAvatar = new AvatarControl();
            _ctlAvatarBorder.Child = _ctlAvatar;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IUser _user;
        public IUser User {
            get => _user;
            set => SetAndRaise(UserProperty, ref _user, value);
        }

        private string _strPrimaryName;
        public string Username {
            get => _strPrimaryName;
            private set => SetAndRaise(UsernameProperty, ref _strPrimaryName, value);
        }

        private Membership[] _arrMemberships;
        public Membership[] Memberships {
            get => _arrMemberships;
            private set => SetAndRaise(MembershipsProperty, ref _arrMemberships, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if (change.IsEffectiveValueChange && (change.Property == UserProperty))
                OnUserChanged();

            base.OnPropertyChanged(change);
        }

        private void OnUserChanged() {
            _ctlAvatar.User = User;
            Username = User.Profile.Username;
            Memberships = User.Memberships.Select(memb => new Membership(memb)).ToArray();
        }

        public class Membership {
            private IMember _member;
            public Membership(IMember memb) { _member = memb; }

            public string ServerIconURL => _member.Server.Info.ImageURL;
            public string ServerName => _member.Server.Info.Name;
            public string KnownAsName => _member.DisplayName;
            public string Power => _member.Info.Power.ToString();
        }
    }
}
