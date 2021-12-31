using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class MessageDialog : Window {
        public static readonly DirectProperty<TextEntryDialog, string> MessageTextProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(MessageText), w => w.PromptText, (w, v) => w.PromptText = v);

        public MessageDialog() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private string _strMessageText = "Message text";
        public string MessageText {
            get => _strMessageText;
            set => SetAndRaise(MessageTextProperty, ref _strMessageText, value);
        }

        private void OKButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Close();
        }

        public static void ShowMessage(Window wndParent, string strTitle, string strMessage)
            => new MessageDialog() { Title = strTitle, MessageText = strMessage }.ShowDialog(wndParent);
    }
}
