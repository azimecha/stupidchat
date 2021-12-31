using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class TextChannelControl : UserControl {
        public TextChannelControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
