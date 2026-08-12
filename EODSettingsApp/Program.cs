using System;
using System.Windows.Forms;
using EODSettingsApp.Forms;

namespace EODSettingsApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// [STAThread] is required for WinForms OLE calls such as SaveFileDialog.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SettingsForm());
        }
    }
}
