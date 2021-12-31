using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public partial class ServerControl : UserControl {
        public ServerControl() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
