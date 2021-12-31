using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public static class Extensions {
        // This still sucks compared to being able to just call ShowDialog() under Winforms and have it act like a normal function
        public static void ShowDialog<T>(this Avalonia.Controls.Window wndDialog, Avalonia.Controls.Window wndOwner, Action<T> procCallback) =>
            wndDialog.ShowDialog<T>(wndOwner).ContinueWith(result => Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => procCallback(result.Result)));
    }
}
