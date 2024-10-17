using DownloaderApp.Settings.Parsers;
using System.Runtime.CompilerServices;

namespace DownloaderApp.Settings
{
    public static class SettingsManager
    {
        public delegate void RaiseAndSet<T>(ref T t1, T t2, [CallerMemberName] string? propertyName = null);

        public static T Get<T>(string key, T defaultValue)
        {
            string? value = CommonSettingsManager.ReadFromCommonSettings(key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T Get<T>(string key, T defaultValue, ISettingsParser parser)
        {
            string? value = CommonSettingsManager.ReadFromCommonSettings(key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return parser.Parse<T>(value);
        }

        public static void Set<T>(string key, T value)
        {
            CommonSettingsManager.ChangeToCommonSettings(key, value?.ToString() ?? string.Empty);
        }

        public static void SetIfChanged<T, S>(ref T field, T value, S settingValue, string settingKey,
            RaiseAndSet<T>? raiseAndSet, Action<T>? additionalAction)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                Set(settingKey, settingValue);
                raiseAndSet?.Invoke(ref field, value);
                additionalAction?.Invoke(value);
            }
        }

        public static void SetIfChanged<T>(ref T field, T value, string settingKey,
            RaiseAndSet<T>? raiseAndSet, Action<T>? additionalAction)
            => SetIfChanged(ref field, value, value, settingKey, raiseAndSet, additionalAction);

        public static void SetIfChanged<T, S>(ref T field, T value, S settingValue,
            string settingKey, RaiseAndSet<T>? raiseAndSet)
            => SetIfChanged(ref field, value, settingValue, settingKey, raiseAndSet, null);

        public static void SetIfChanged<T, S>(ref T field, T value, S settingValue,
            string settingKey, Action<T>? additionalAction) 
            => SetIfChanged(ref field, value, settingValue, settingKey, null, additionalAction);

        public static void SetIfChanged<T>(ref T field, T value, string settingKey,
            RaiseAndSet<T>? raiseAndSet) => SetIfChanged(ref field, value, value, settingKey, raiseAndSet, null);

        public static void SetIfChanged<T>(ref T field, T value, string settingKey,
            Action<T>? additionalAction) => SetIfChanged(ref field, value, value, settingKey, additionalAction);
    }
}