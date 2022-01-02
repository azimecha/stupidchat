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

        private Border _ctlImageBorder;
        private Image _ctlImage;
        private Popup _ctlPopup;
        private ProfileControl _ctlProfile;
        private BackgroundWorker _wkrDownloadAvatar;
        private bool _bRerun;

        public AvatarControl() {
            InitializeComponent();

            _ctlImageBorder = this.FindControl<Border>("ImageBorder");
            _ctlImage = this.FindControl<Image>("ImageControl");
            _ctlPopup = this.FindControl<Popup>("ProfilePopup");

            _wkrDownloadAvatar = new BackgroundWorker();
            _wkrDownloadAvatar.WorkerSupportsCancellation = true;
            _wkrDownloadAvatar.DoWork += DownloadAvatarWorker_DoWork;
            _wkrDownloadAvatar.RunWorkerCompleted += DownloadAvatarWorker_RunWorkerCompleted;
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

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if (change.IsEffectiveValueChange) {
                if (change.Property == UserProperty)
                    OnUserChanged();
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

        private void OnUserChanged() {
            Avatar = DEFAULT_AVATAR_IMAGE.Value;
            _bRerun = false;

            if (_user is not null) {
                if (_wkrDownloadAvatar.IsBusy)
                    _bRerun = true;
                else
                    _wkrDownloadAvatar.RunWorkerAsync(_user);
            }

            if (_ctlProfile is not null)
                _ctlProfile.User = _user;
        }

        private void DownloadAvatarWorker_DoWork(object sender, DoWorkEventArgs e) {
            IUser user = (IUser)e.Argument;

            using (System.IO.Stream stmAvatar = user.OpenAvatar())
                if (!(stmAvatar is null))
                    e.Result = new Avalonia.Media.Imaging.Bitmap(stmAvatar);
        }

        private void DownloadAvatarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (_bRerun) {
                _bRerun = false;

                if (_user is not null)
                    _wkrDownloadAvatar.RunWorkerAsync(_user);

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
    }
}
