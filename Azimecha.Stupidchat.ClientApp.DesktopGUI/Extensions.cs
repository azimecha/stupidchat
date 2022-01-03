using Azimecha.Stupidchat.Client;
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

        // WPF ScrollViewer has public properties for scrollbar values but the Avalonia one makes all of them protected
        private static System.Reflection.PropertyInfo _infScrollbarValueProperty = typeof(Avalonia.Controls.ScrollViewer)
            .GetProperty("VerticalScrollBarValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy /* 4 PM */
                | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        private static System.Reflection.PropertyInfo _infScrollbarMaximumProperty = typeof(Avalonia.Controls.ScrollViewer)
            .GetProperty("VerticalScrollBarMaximum", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy
                | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

        public static bool IsAtBottom(this Avalonia.Controls.ScrollViewer ctlScrollViewer)
            => (double)_infScrollbarValueProperty.GetValue(ctlScrollViewer) == (double)_infScrollbarMaximumProperty.GetValue(ctlScrollViewer);

        public static bool IsAtTop(this Avalonia.Controls.ScrollViewer ctlScrollViewer)
            => (double)_infScrollbarValueProperty.GetValue(ctlScrollViewer) == 0.0;

        public static void ScrollToBottom(this Avalonia.Controls.ScrollViewer ctlScrollViewer)
            => _infScrollbarValueProperty.SetValue(ctlScrollViewer, _infScrollbarMaximumProperty.GetValue(ctlScrollViewer));

        public static void ScrollDown(this Avalonia.Controls.ScrollViewer ctlScrollViewer, double fAmount)
            => _infScrollbarValueProperty.SetValue(ctlScrollViewer, (double)_infScrollbarValueProperty.GetValue(ctlScrollViewer) + fAmount);

        public static Avalonia.Controls.Window GetWindow(this Avalonia.Controls.IControl ctl) {
            while (ctl is not null) {
                if (ctl is Avalonia.Controls.Window wnd)
                    return wnd;

                ctl = ctl.Parent;
            }

            return null;
        }

        public static KnownServer ToKnownServer(this IServer server) => new KnownServer() {
            Address = server.Address,
            Port = server.Port,
            PublicKey = server.ID.ToArray()
        };
    }
}
