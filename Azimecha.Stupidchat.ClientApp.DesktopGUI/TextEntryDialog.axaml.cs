using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class TextEntryDialog : Window {
        public static readonly DirectProperty<TextEntryDialog, string> PromptTextProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(PromptText), w => w.PromptText, (w, v) => w.PromptText = v);

        public static readonly DirectProperty<TextEntryDialog, string> EnteredTextProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(EnteredText), w => w.EnteredText, (w, v) => w.EnteredText = v);

        public static readonly DirectProperty<TextEntryDialog, string> PlaceholderTextProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, string>(nameof(PlaceholderText), w => w.PlaceholderText, (w, v) => w.PlaceholderText = v);

        public static readonly DirectProperty<TextEntryDialog, bool> NonemptyStringDesiredProperty =
            AvaloniaProperty.RegisterDirect<TextEntryDialog, bool>(nameof(NonemptyStringDesired), w => w.NonemptyStringDesired, (w, v) => w.NonemptyStringDesired = v);

        public TextEntryDialog() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private string _strPromptText = "Prompt text";
        public string PromptText {
            get => _strPromptText;
            set => SetAndRaise(PromptTextProperty, ref _strPromptText, value);
        }

        private string _strEnteredText = "";
        public string EnteredText {
            get => _strEnteredText;
            set => SetAndRaise(EnteredTextProperty, ref _strEnteredText, value);
        }

        private string _strPlaceholderText = "";
        public string PlaceholderText {
            get => _strPlaceholderText;
            set => SetAndRaise(PlaceholderTextProperty, ref _strPlaceholderText, value);
        }

        public bool _bNonemptyDesired = true;
        public bool NonemptyStringDesired {
            get => _bNonemptyDesired;
            set => SetAndRaise(NonemptyStringDesiredProperty, ref _bNonemptyDesired, value);
        }

        private void OKButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Close(EnteredText);
        }

        private void CancelButton_Click(object objSender, Avalonia.Interactivity.RoutedEventArgs e) {
            Close();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            if ((change.Property == NonemptyStringDesiredProperty) || (change.Property == EnteredTextProperty))
                this.FindControl<Button>("OKButton").IsEnabled = !NonemptyStringDesired || ((EnteredText?.Length ?? 0) > 0);

            base.OnPropertyChanged(change);
        }
    }
}
