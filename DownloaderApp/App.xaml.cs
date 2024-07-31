using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace DownloaderApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string CommonSettings { get; } = $"{AppContext.BaseDirectory}common_settings.json";
    }

}
