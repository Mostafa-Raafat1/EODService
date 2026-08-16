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

            // Prompt key setup on first launch if security.dat does not exist yet
            if (!KeyStoreService.KeyExists())
            {
                using var setupForm = new SecuritySetupForm();
                if (setupForm.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Encryption key setup is required to run EODService Manager.",
                                    "Security Setup Required",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
            }

            Application.Run(new SettingsForm());
        }
    }
}

