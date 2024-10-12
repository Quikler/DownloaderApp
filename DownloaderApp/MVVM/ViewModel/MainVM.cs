using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.View;
using DownloaderApp.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class MainVM : BaseVM
    {
        private Page _currentPage = null!;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { RaiseAndSetIfChanged(ref _currentPage, value); }
        }

        public YoutubeView YoutubeView { get; }
        public InstagramView InstagramView { get; }
        public LoginView LoginView { get; }

        public ICommand YoutubeMenuItemCommand { get; }
        public ICommand InstagramMenuItemCommand { get; }
        public ICommand LoginMenuItemCommand { get; set; }

        public MainVM()
        {
            CurrentPage = YoutubeView = new YoutubeView();
            LoginView = new LoginView();
            InstagramView = new InstagramView();

            YoutubeMenuItemCommand = new RelayCommand(obj => FadeAnimation(YoutubeView), obj => CurrentPage != YoutubeView);
            InstagramMenuItemCommand = new RelayCommand(obj => FadeAnimation(InstagramView), obj => CurrentPage != InstagramView);
            LoginMenuItemCommand = new RelayCommand(obj => FadeAnimation(LoginView), obj => CurrentPage != LoginView);
            
        }

        private void FadeAnimation(Page desiredPage) => BeginFadeOutPage(desiredPage, TimeSpan.FromMilliseconds(200));
        
        private void BeginFadeOutPage(Page desiredPage, TimeSpan duration)
        {
            var fadeOut = new DoubleAnimation(1d, 0d, duration);

            Storyboard.SetTarget(fadeOut, CurrentPage);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(UIElement.OpacityProperty));

            var fadeOutStoryboard = new Storyboard();
            fadeOutStoryboard.Children.Add(fadeOut);

            fadeOutStoryboard.Completed += (sender, e) =>
            {
                CurrentPage = desiredPage;

                BeginFadeInPage(duration);
            };
            fadeOutStoryboard.Begin();
        }

        private void BeginFadeInPage(TimeSpan duration)
        {
            var fadeIn = new DoubleAnimation(0d, 1d, duration);

            Storyboard.SetTarget(fadeIn, CurrentPage);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));

            var fadeInStoryboard = new Storyboard();
            fadeInStoryboard.Children.Add(fadeIn);

            fadeInStoryboard.Begin();
        }
    }
}
