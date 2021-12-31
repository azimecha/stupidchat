using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class UsernamePasswordDialog : Window {
        public static readonly DirectProperty<TextEntryDialog, string> PromptTextProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(PromptText), w => w.EnteredText, (w, v) => w.EnteredText = v);

        public static readonly DirectProperty<TextEntryDialog, string> UsernameProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(Username), w => w.EnteredText, (w, v) => w.EnteredText = v);

        public static readonly DirectProperty<TextEntryDialog, string> PasswordProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(Password), w => w.EnteredText, (w, v) => w.EnteredText = v);

        public UsernamePasswordDialog() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private string _strPrompt = "Enter your username and password";
        public string PromptText {
            get => _strPrompt;
            set => SetAndRaise(PromptTextProperty, ref _strPrompt, value);
        }

        private string _strUsername;
        public string Username {
            get => _strUsername;
            set => SetAndRaise(UsernameProperty, ref _strUsername, value);
        }

        private string _strPassword;
        public string Password {
            get => _strPassword;
            set => SetAndRaise(PasswordProperty, ref _strPassword, value);
        }

        public struct LoginInformation {
            public string Username;
            public string Password;
        }


        private void OKButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Close(new LoginInformation() { Username = this.Username, Password = this.Password });
        }

        private void CancelButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Close();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == UsernameProperty) || (change.Property == PasswordProperty))
                this.FindControl<Button>("OKButton").IsEnabled = ((Username?.Length ?? 0) > 0) && ((Password?.Length ?? 0) > 0);

            base.OnPropertyChanged(change);
        }
    }
}
