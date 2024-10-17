namespace DownloaderApp.Settings
{
    public class AudioSettings
    {
        public string[] Formats { get; } = { "MP3", "WAV" };

        private int _formatIndex;
        public int FormatIndex
        {
            get => _formatIndex;
            set
            {
                _formatIndex = value;
                SettingsManager.Set("outputAudioFormat", Formats[value]);
            }
        }

        public AudioSettings()
        {
            _formatIndex = Array.IndexOf(Formats, SettingsManager.Get("outputAudioFormat", Formats[0]));
        }
    }

}
