namespace EODSettingsApp.ExternalConfig
{
    /// <summary>
    /// Single source of truth for the location of the shared external config file.
    /// Both the WinForms app (read/write) and EODService (read-only) point here.
    /// </summary>
    public static class ExternalConfigPath
    {
        public const string FolderPath = @"C:\EODConfig";
        public const string FilePath   = @"C:\EODConfig\settings.json";
    }
}
