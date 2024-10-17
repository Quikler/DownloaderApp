namespace DownloaderApp.Settings.Parsers
{
    public interface ISettingsParser
    {
        T Parse<T>(string value);
    }
}
