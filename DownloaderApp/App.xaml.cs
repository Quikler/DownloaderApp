using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace DownloaderApp
{
    public partial class App : Application
    {
        public static string CommonSettings { get; } = 
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{nameof(DownloaderApp)}\\common_settings.json";
    }
}