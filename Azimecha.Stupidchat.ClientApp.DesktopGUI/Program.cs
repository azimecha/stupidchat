using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Azimecha.Stupidchat.Client;
using System;

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    internal class Program {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            Environment.Exit(0);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        public static string DataFolder => _strDataFolder.Value;

        private static Lazy<string> _strDataFolder = new Lazy<string>(GetDataFolderPath, 
            System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static string GetDataFolderPath() {
            string strPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                typeof(Program).Namespace);
            System.IO.Directory.CreateDirectory(strPath);
            return strPath;
        }
    }
}
