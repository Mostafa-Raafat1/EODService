using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.DTOs.SymbolSettings
{
    public class SymbolSettingsMapper
    {
        public static SymbolSettings? MapToSymbolSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                // External file overrides the internal one if it exists.
                // This is written by the EODSettingsApp WinForms tool.
                .AddJsonFile(@"C:\EODConfig\settings.json", optional: true, reloadOnChange: true)
                .Build();

            return configuration
                .GetSection("SymbolSettings")
                .Get<SymbolSettings>();
        }
    }
}
