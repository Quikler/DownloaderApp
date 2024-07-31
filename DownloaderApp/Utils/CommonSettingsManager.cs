using Newtonsoft.Json.Linq;
using System.IO;

namespace DownloaderApp.Utils
{
    public static class CommonSettingsManager
    {
        public static bool CreateIfNotExist()
        {
            if (!File.Exists(App.CommonSettings))
            {
                using StreamWriter w = File.AppendText(App.CommonSettings);
                w.Write("{ }");
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
