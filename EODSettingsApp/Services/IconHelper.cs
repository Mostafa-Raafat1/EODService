using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EODSettingsApp.Services
{
    public static class IconHelper
    {
        public static Icon? AppIcon => null;

        public static void ApplyTo(Form form)
        {
            // Do nothing - user does not want custom icon
        }
    }
}
