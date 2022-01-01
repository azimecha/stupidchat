using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Azimecha.Stupidchat.Client;
using System.ComponentModel;
using System.Diagnostics;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class MessageControl : UserControl {
        public static readonly DirectProperty<MessageControl, IMessage> MessageProperty =
            AvaloniaProperty.RegisterDirect<MessageControl, IMessage>(nameof(Message), w => w.Message, (w, v) => w.Message = v);

        public static readonly DirectProperty<MessageControl, IImage> SenderAvatarProperty =
            AvaloniaProperty.RegisterDirect<MessageControl, IImage>(nameof(SenderAvatar), w => w.SenderAvatar);

        public static readonly DirectProperty<MessageControl, string> MessageTextProperty =
            AvaloniaProperty.RegisterDirect<MessageControl, string>(nameof(MessageText), w => w.MessageText);

        private BackgroundWorker _wkrDownloadAvatar;

        public MessageControl() {
            InitializeComponent();

            _wkrDownloadAvatar = new BackgroundWorker();
            _wkrDownloadAvatar.WorkerSupportsCancellation = true;
            _wkrDownloadAvatar.DoWork += DownloadAvatarWorker_DoWork;
            _wkrDownloadAvatar.RunWorkerCompleted += DownloadAvatarWorker_RunWorkerCompleted;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }


        private IMessage _message;
        public IMessage Message {
            get => _message;
            set => SetAndRaise(MessageProperty, ref _message, value);
        }

        private IImage _imgAvatar;
        public IImage SenderAvatar {
            get => _imgAvatar;
            private set => SetAndRaise(SenderAvatarProperty, ref _imgAvatar, value);
        }

        private string _strBodyText;
        public string MessageText {
            get => _strBodyText;
            private set => SetAndRaise(MessageTextProperty, ref _strBodyText, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == MessageProperty) && change.IsEffectiveValueChange)
                OnMessageChanged();

            base.OnPropertyChanged(change);
        }

        private const string DEFAULT_AVATAR_URL = "avares://Azimecha.Stupidchat.ClientApp.DesktopGUI/Assets/DefaultUserIcon.png";
        private static readonly IImage DEFAULT_AVATAR_IMAGE = new Avalonia.Media.Imaging.Bitmap(
            AvaloniaLocator.Current.GetService<Avalonia.Platform.IAssetLoader>().Open(new System.Uri(DEFAULT_AVATAR_URL)));

        private void OnMessageChanged() {
            SenderAvatar = DEFAULT_AVATAR_IMAGE;
            MessageText = (_message is null) ? "" : _message.SignedData.Text;

            // could have an issue here if the worker is already running...
            if (!(_message?.Sender?.User is null))
                _wkrDownloadAvatar.RunWorkerAsync(_message.Sender.User);
        }

        private void DownloadAvatarWorker_DoWork(object sender, DoWorkEventArgs e) {
            IUser user = (IUser)e.Argument;

            using (System.IO.Stream stmAvatar = user.OpenAvatar())
                if (!(stmAvatar is null))
                    e.Result = new Avalonia.Media.Imaging.Bitmap(stmAvatar);
        }

        private void DownloadAvatarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Result is IImage imgAvatar)
                SenderAvatar = imgAvatar;

            if (e.Error is System.Exception ex)
                Debug.WriteLine($"[{nameof(MessageControl)}] Error downloading avatar: {ex}");
        }
    }
}
