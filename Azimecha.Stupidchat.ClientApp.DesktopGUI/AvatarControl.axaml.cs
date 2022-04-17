using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Azimecha.Stupidchat.Client;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class AvatarControl : UserControl {
        public static readonly DirectProperty<AvatarControl, IUser> UserProperty =
            AvaloniaProperty.RegisterDirect<AvatarControl, IUser>(nameof(User), w => w.User, (w, v) => w.User = v);

        public static readonly DirectProperty<AvatarControl, IImage> AvatarProperty =
            AvaloniaProperty.RegisterDirect<AvatarControl, IImage>(nameof(Avatar), w => w.Avatar);

        public static readonly DirectProperty<AvatarControl, bool> ClickableProperty =
            AvaloniaProperty.RegisterDirect<AvatarControl, bool>(nameof(User), w => w.Clickable, (w, v) => w.Clickable = v);

        public static readonly DirectProperty<AvatarControl, Core.Structures.OnlineStatus> UserStatusProperty =
            AvaloniaProperty.RegisterDirect<AvatarControl, Core.Structures.OnlineStatus>(nameof(UserStatus), w => w.UserStatus);

        public static readonly DirectProperty<AvatarControl, Core.Structures.OnlineDevice> UserDeviceProperty =
            AvaloniaProperty.RegisterDirect<AvatarControl, Core.Structures.OnlineDevice>(nameof(UserDevice), w => w.UserDevice);

        private Border _ctlImageBorder, _ctlStatusBorder;
        private Image _ctlImage;
        private Popup _ctlPopup;
        private ProfileControl _ctlProfile;
        private BackgroundWorker _wkrDownloadAvatar;
        private bool _bRerun;

        private Action<IUser> _procUserProfileChanged, _procUserStatusChanged;

        public AvatarControl() {
            InitializeComponent();

            _ctlImageBorder = this.FindControl<Border>("ImageBorder");
            _ctlStatusBorder = this.FindControl<Border>("StatusBorder");
            _ctlImage = this.FindControl<Image>("ImageControl");
            _ctlPopup = this.FindControl<Popup>("ProfilePopup");

            _wkrDownloadAvatar = new BackgroundWorker();
            _wkrDownloadAvatar.WorkerSupportsCancellation = true;
            _wkrDownloadAvatar.DoWork += DownloadAvatarWorker_DoWork;
            _wkrDownloadAvatar.RunWorkerCompleted += DownloadAvatarWorker_RunWorkerCompleted;

            _procUserProfileChanged = new Action<IUser>(User_ProfileChanged);
            _procUserStatusChanged = new Action<IUser>(User_StatusChanged);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private IUser _user;
        public IUser User {
            get => _user;
            set => SetAndRaise(UserProperty, ref _user, value);
        }

        private IImage _imgAvatar;
        public IImage Avatar {
            get => _imgAvatar;
            private set => SetAndRaise(AvatarProperty, ref _imgAvatar, value);
        }

        private bool _bClickable;
        public bool Clickable {
            get => _bClickable;
            set => SetAndRaise(ClickableProperty, ref _bClickable, value);
        }

        private Core.Structures.OnlineStatus _status;
        public Core.Structures.OnlineStatus UserStatus {
            get => _status;
            private set => SetAndRaise(UserStatusProperty, ref _status, value);
        }

        private Core.Structures.OnlineDevice _device;
        public Core.Structures.OnlineDevice UserDevice {
            get => _device;
            private set => SetAndRaise(UserDeviceProperty, ref _device, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if (change.IsEffectiveValueChange) {
                if (change.Property == UserProperty)
                    OnUserChanged(change.OldValue.HasValue ? (IUser)change.OldValue.Value : null);
                else if (change.Property == WidthProperty)
                    OnWidthChanged();
                else if (change.Property == ClickableProperty)
                    OnClickableChanged();
            }

            base.OnPropertyChanged(change);
        }

        private const string DEFAULT_AVATAR_URL = "avares://Azimecha.Stupidchat.ClientApp.DesktopGUI/Assets/DefaultUserIcon.png";
        private static readonly Lazy<IImage> DEFAULT_AVATAR_IMAGE = new Lazy<IImage>(LoadDefaultAvatarImage, 
            System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private void OnUserChanged(IUser userOld) {
            Avatar = DEFAULT_AVATAR_IMAGE.Value;
            _bRerun = false;

            if (userOld is not null) {
                userOld.ProfileChanged -= _procUserProfileChanged;
                userOld.StatusChanged -= _procUserStatusChanged;
            }

            if (_user is not null) {
                UserStatus = _user.CurrentStatus;
                UserDevice = _user.CurrentDevice;

                try {
                    _wkrDownloadAvatar.RunWorkerAsync(_user);
                } catch (InvalidOperationException) {
                    _bRerun = true;
                }

                _user.ProfileChanged += _procUserProfileChanged;
                _user.StatusChanged += _procUserStatusChanged;
            }

            if (_ctlProfile is not null)
                _ctlProfile.User = _user;
        }

        private void DownloadAvatarWorker_DoWork(object sender, DoWorkEventArgs e) {
            IUser user = (IUser)e.Argument;

            using (System.IO.Stream stmAvatar = user.OpenAvatar())
                if (stmAvatar is not null)
                    e.Result = new Avalonia.Media.Imaging.Bitmap(stmAvatar);
        }

        private void DownloadAvatarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (_bRerun) {
                _bRerun = false;

                if (_user is not null) {
                    try {
                        _wkrDownloadAvatar.RunWorkerAsync(_user);
                    } catch (InvalidOperationException) { }
                }

                return;
            }

            if (e.Result is IImage imgAvatar)
                Avatar = imgAvatar;

            if (e.Error is Exception ex)
                Debug.WriteLine($"[{nameof(AvatarControl)}] Error downloading avatar: {ex}");
        }

        private static IImage LoadDefaultAvatarImage() => new Avalonia.Media.Imaging.Bitmap(
            AvaloniaLocator.Current.GetService<Avalonia.Platform.IAssetLoader>().Open(new Uri(DEFAULT_AVATAR_URL)));

        private void OnWidthChanged() {
            _ctlImageBorder.CornerRadius = new CornerRadius(Width / 2);
        }

        private void ImageControl_PointerPressed(object objSender, Avalonia.Input.PointerPressedEventArgs e) {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && Clickable)
                _ctlPopup.Open();
        }

        private void OnClickableChanged() {
            _ctlImage.Cursor = Clickable ? new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
                : Avalonia.Input.Cursor.Default;
            
            if (Clickable) {
                _ctlProfile = new ProfileControl() { User = _user };
                _ctlPopup.Child = _ctlProfile;
            } else {
                _ctlProfile = null;
                _ctlPopup.Child = null;
            }
        }

        private void User_ProfileChanged(IUser user) {
            try {
                _wkrDownloadAvatar.RunWorkerAsync(_user);
            } catch (InvalidOperationException) {
                _bRerun = true;
            }
        }

        private void User_StatusChanged(IUser user) => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
            UserStatus = user.CurrentStatus;
            UserDevice = user.CurrentDevice;
        });

        private void OnStatusChanged() {
            Color clrStatus;

            switch (UserStatus) {
                case Core.Structures.OnlineStatus.Offline:
                    clrStatus = new Color(255, 128, 128, 128);
                    break;

                case Core.Structures.OnlineStatus.Away:
                    clrStatus = new Color(255, 255, 255, 0);
                    break;

                case Core.Structures.OnlineStatus.Online:
                    clrStatus = new Color(255, 0, 192, 0);
                    break;

                case Core.Structures.OnlineStatus.DoNotDisturb:
                    clrStatus = new Color(0, 192, 0, 0);
                    break;

                default:
                    clrStatus = Colors.Transparent;
                    break;
            }

            _ctlStatusBorder.Background = new SolidColorBrush(clrStatus);
        }

        private void StatusBorder_PropertyChanged(object objSender, AvaloniaPropertyChangedEventArgs e) {
            if (e.IsEffectiveValueChange && (e.Property == Border.WidthProperty))
                _ctlStatusBorder.CornerRadius = new CornerRadius(_ctlStatusBorder.Width / 2);
        }
    }
}
