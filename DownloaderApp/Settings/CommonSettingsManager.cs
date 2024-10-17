using Newtonsoft.Json.Linq;
using System.IO;

namespace DownloaderApp.Settings
{
    public static class CommonSettingsManager
    {
        public static bool CreateIfNotExist()
        {
            string? settingsParent = Directory.GetParent(App.CommonSettings)?.FullName;
            if (settingsParent is not null && !Directory.Exists(settingsParent))
            {
                Directory.CreateDirectory(settingsParent);
            }

            if (!File.Exists(App.CommonSettings) || new FileInfo(App.CommonSettings).Length == 0)
            {
                File.WriteAllText(App.CommonSettings, "{ }");
                return true;
            }

            return false;
        }

        public static string? ReadFromCommonSettings(string key)
        {
            if (CreateIfNotExist())
                return null;

            string jsonString = File.ReadAllText(App.CommonSettings);
            JObject jsonObject = JObject.Parse(jsonString);

            return jsonObject[key]?.ToString();
        }

        public static void ChangeToCommonSettings(string key, string value)
        {
            CreateIfNotExist();

            string jsonString = File.ReadAllText(App.CommonSettings);
            JObject jsonObject = JObject.Parse(jsonString);

            jsonObject[key] = value;

            File.WriteAllText(App.CommonSettings, jsonObject.ToString());
        }
    }
}
