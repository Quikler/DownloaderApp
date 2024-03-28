using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.View;
using DownloaderApp.Utils;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class MainVM : BaseVM
    {
        private Page _currentPage = null!;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        public YoutubeView YoutubeView { get; }
        public InstagramView InstagramView { get; }

        public ICommand YoutubeMenuButtonCommand { get; }
        public ICommand InstagramMenuButtonCommand { get; }

        public MainVM()
        {
            CurrentPage = YoutubeView = new YoutubeView();
            InstagramView = new InstagramView();

            YoutubeMenuButtonCommand = new RelayCommand(obj => CurrentPage = YoutubeView);
            InstagramMenuButtonCommand = new RelayCommand(obj => CurrentPage = InstagramView);
        }
    }
}
