namespace DownloaderApp.Settings
{
    public class VideoSettings
    {
        public string[] Resolutions { get; } = { "Highest", "1920 x 1080", "480 x 360", "336 x 188", "320 x 180", "246 x 138", "196 x 110", "168 x 94", "120 x 90", "Lowest" };
        public string[] Qualities { get; } = { "Highest", "1080p", "720p", "480p", "360p", "240p", "144p", "Lowest" };

        private int _resolutionIndex;
        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                _resolutionIndex = value;
                SettingsManager.Set("thumbnailResolution", Resolutions[value]);
            }
        }

        private int _qualityIndex;
        public int QualityIndex
        {
            get => _qualityIndex;
            set
            {
                _qualityIndex = value;
                SettingsManager.Set("videoQuality", Qualities[value]);
            }
        }

        public VideoSettings()
        {
            var thumbnailResolution = SettingsManager.Get("thumbnailResolution", string.Empty);
            if (thumbnailResolution == string.Empty) SettingsManager.Set("thumbnailResolution", Resolutions[_resolutionIndex]);
            _resolutionIndex = Array.IndexOf(Resolutions, SettingsManager.Get("thumbnailResolution", Resolutions[0]));

            var videoQuality = SettingsManager.Get("videoQuality", string.Empty);
            if (videoQuality == string.Empty) SettingsManager.Set("videoQuality", Qualities[_qualityIndex]);
            _qualityIndex = Array.IndexOf(Qualities, SettingsManager.Get("videoQuality", Qualities[0]));
        }
    }
}