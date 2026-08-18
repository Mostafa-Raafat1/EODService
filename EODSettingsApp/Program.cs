using System;
using System.Windows.Forms;
using EODService.Services;
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

            // Ensure runtime directories exist on machine startup
            EODService.Config.PathsConfig.EnsureDirectoriesExist();

            Application.Run(new SettingsForm());
        }
    }
}

