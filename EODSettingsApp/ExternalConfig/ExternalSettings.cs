namespace EODSettingsApp.ExternalConfig
{
    /// <summary>
    /// Mirrors the structure of C:\EODConfig\settings.json.
    /// Only contains settings managed by the WinForms app.
    /// All other settings (API keys, symbols, URLs) remain in EODService\appsettings.json.
    /// </summary>
    public class ExternalSettings
    {
        public ProviderSettingsSection ProviderSettings { get; set; } = new();
    }

    public class ProviderSettingsSection
    {
        public int ActiveProvider { get; set; } = 1;
    }
}
