using EODService.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODSettingsApp.AppSettingsConfig
{
    public static class ConnectionStringResolver
    {
        public static string Get()
        {
            var model = AppSettingsService.Load();
            if (!string.IsNullOrWhiteSpace(model.ConnectionStrings?.DefaultConnection))
                return model.ConnectionStrings.DefaultConnection;
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(PathsConfig.AppSettingsFileName, optional: true, reloadOnChange: false)
                .Build()
                .GetConnectionString("DefaultConnection") ?? string.Empty;
        }
    }
}
