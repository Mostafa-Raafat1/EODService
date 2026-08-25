using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EODSettingsApp
{
    public static class AppIconHelper
    {
        public static void ApplyAppIconAndTitle(Form form)
        {
            form.Text = "TICKR";

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string localIconPath = Path.Combine(baseDir, "Resources", "TICKR.ico");
                if (File.Exists(localIconPath))
                {
                    form.Icon = new Icon(localIconPath);
                    return;
                }

                string rootIconPath = Path.Combine(baseDir, "TICKR.ico");
                if (File.Exists(rootIconPath))
                {
                    form.Icon = new Icon(rootIconPath);
                    return;
                }

                string downloadIconPath = @"C:\Users\mostafa.raafat.inter\Downloads\TICKR.ico";
                if (File.Exists(downloadIconPath))
                {
                    form.Icon = new Icon(downloadIconPath);
                    return;
                }

                var exePath = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    Icon? extracted = Icon.ExtractAssociatedIcon(exePath);
                    if (extracted != null)
                    {
                        form.Icon = extracted;
                    }
                }
            }
            catch
            {
                // Fallback ignored
            }
        }
    }
}
