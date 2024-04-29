using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using InstagramApiSharp.Classes;
using InstagramService.Classes.Collections;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class InstagramVM : InputOutputVM
    {
        public override ICommand TextChangedCommand { get; }
        public override ICommand ButtonClickCommand { get; }

        private readonly InstagramModel _instagramModel;

        private List<MediaElement> _selectableCollection = null!;
        public List<MediaElement> SelectableCollection
        {
            get { return _selectableCollection; }
            set
            {
                if (_selectableCollection != value)
                {
                    _selectableCollection = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCheckBoxChecked;
        public bool IsCheckBoxChecked
        {
            get { return _isCheckBoxChecked; }
            set
            {
                if (_isCheckBoxChecked != value)
                {
                    _isCheckBoxChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public InstagramVM()
        {
            InfoText = "Instagram";

            TextChangedCommand = new RelayCommand(TextChangedCommandExecute, TextChangedCommandCanExecute);
            ButtonClickCommand = new RelayCommand(ClickCommandExecute, ClickCommandCanExecute);

            _instagramModel = new InstagramModel();

            Mediator.IInstaApiChanged += (sender, e) =>
            {
                if (sender != this)
                    _instagramModel.Api = e.Api;
            };
            Mediator.NotifyIInstaApiChanged(this, new InstaApiEventArgs(_instagramModel.Api));

            if (_instagramModel.Api.IsUserAuthenticated)
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = "User logged";
            }
            else
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "User not logged";
            }
        }

        protected override async void TextChangedCommandExecute(object? parameter)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Loading", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            var infoResult = await _instagramModel.InstagramService.MediaHelper.GetInfosAsync(InputText);
            if (!infoResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = infoResult.Info.ToString();
                cancellationTokenSource.Cancel();
                return;
            }

            var infos = _instagramModel.Infos = infoResult.Value;
            int infosCount = infos.Count();

            MediaElement[] source = new MediaElement[infosCount];
            for (int i = 0; i < infosCount; i++)
            {
                source[i] = new MediaElement { Source = new Uri(infos[i].Uri) };
            }

            source.First().MediaOpened += (sender, e) => cancellationTokenSource.Cancel();
            MediaSource = source;
        }

        protected override bool TextChangedCommandCanExecute(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(InputText) || !InstagramModel.Regex.IsMatch(InputText))
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "Input doesn't match a regex";
                return false;
            }

            if (_instagramModel.Infos is not null && InputText.Contains(_instagramModel.Infos.First().InitialUri))
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = "Specified media already in preview";
                return false;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "Input matches a regex";

            return IsDownloadButtonEnabled = true;
        }

        protected async override void ClickCommandExecute(object? parameter)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Installing", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            IsDownloadButtonEnabled = false;
            IResult<InstaMediaStreams> instaMediaStreamsResult = SelectableCollection.Count != MediaSource?.Length ?
                await _instagramModel.GetStreamsFromAsync(SelectableCollection) :
                await _instagramModel.InstagramService.StreamTaker.GetMediaStreamsAsync(_instagramModel.Infos);

            if (!instaMediaStreamsResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = instaMediaStreamsResult.Info.ToString();
                cancellationTokenSource.Cancel();
                return;
            }

            var finalResult = await _instagramModel.InstagramService.MediaHelper
                .DownloadMediasAsync(instaMediaStreamsResult.Value, SelectedPath);

            if (finalResult.Succeeded)
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = $"Instagram media/s successfully downloaded by path/s:\n" +
                    $"{string.Join("\n", finalResult.Value.Select(fi => $"\"{fi.FullName}\""))}";
            }
            else
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = finalResult.Info.ToString();
            }

            IsDownloadButtonEnabled = true;
            cancellationTokenSource.Cancel();
        }

        protected override bool ClickCommandCanExecute(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(SelectedPath) || _instagramModel.Infos is null || SelectableCollection.Count <= 0)
                return false;

            return true;
        }
    }
}
