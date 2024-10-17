namespace DownloaderApp.Settings.Parsers
{
    public readonly struct VideoQualityParser : ISettingsParser
    {
        public readonly T Parse<T>(string value) => value switch
        {
            "Highest" => (T)Convert.ChangeType(1, typeof(T)),
            "Lowest" => (T)Convert.ChangeType(-1, typeof(T)),
            _ => (T)Convert.ChangeType(int.Parse(value.TrimEnd('p')), typeof(T))
        };
    }
}
