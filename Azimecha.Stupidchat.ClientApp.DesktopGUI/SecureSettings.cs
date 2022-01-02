using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// apparently this warning can't read "Environment.OSVersion.Platform == PlatformID.Win32NT"
#pragma warning disable CA1416

namespace Azimecha.Stupidchat.ClientApp.DesktopGUI {
    public static class SecureSettings {
        private static string _strFilePath = System.IO.Path.Combine(Program.DataFolder, "key.bin");

        public static byte[] GetPrivateKey() {
            if (!System.IO.File.Exists(_strFilePath))
                return null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return System.Security.Cryptography.ProtectedData.Unprotect(System.IO.File.ReadAllBytes(_strFilePath), null,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
            else
                return System.IO.File.ReadAllBytes(_strFilePath);
        }

        public static void SetPrivateKey(byte[] arrKey) {
            System.IO.File.WriteAllBytes(_strFilePath, Environment.OSVersion.Platform == PlatformID.Win32NT 
                ? System.Security.Cryptography.ProtectedData.Protect(arrKey, null, System.Security.Cryptography.DataProtectionScope.CurrentUser)
                : arrKey);
        }

        public static void Discard() {
            System.IO.File.Delete(_strFilePath);
        }
    }
}
