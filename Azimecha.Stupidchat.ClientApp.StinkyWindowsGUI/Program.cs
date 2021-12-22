using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Azimecha.Stupidchat.ClientApp.StinkyWindowsGUI {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            ApplicationConfiguration.Initialize();
            Application.Run(new ClientForm());
        }

        public static Control[] GetChildren(this Control ctl) {
            List<Control> lstChildren = new List<Control>();
            foreach (Control ctlChild in ctl.Controls)
                lstChildren.Add(ctlChild);
            return lstChildren.ToArray();
        }
    }
}