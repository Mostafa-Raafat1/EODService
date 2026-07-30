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
                .Build();

            SymbolSettings symbolSettings = new SymbolSettings();

            var symbolSettingsSection = configuration
                                        .GetSection("SymbolSettings")
                                        .Get<SymbolSettings>();
            return symbolSettingsSection;
        }
    }
}
