using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.Settings;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class SettingsVM : BaseVM
    {
        private bool _loadPreview;
        public bool LoadPreview
        {
            get => _loadPreview;
            set => SettingsManager.SetIfChanged(ref _loadPreview, value, "loadPreview", RaiseAndSet);
        }

        public VideoSettings VideoSettings { get; }

        public SettingsVM()
        {
            VideoSettings = new VideoSettings();

            LoadPreview = SettingsManager.Get("loadPreview", false);
        }
    }
}
