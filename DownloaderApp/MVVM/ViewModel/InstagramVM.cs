using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.Settings;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using InstagramApiSharp.Classes;
using InstagramService.Classes.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class InstagramVM : InputOutputVM
    {
        public override ICommand TextChangedCommand { get; }
        public override ICommand ButtonClickCommand { get; }

        private readonly InstagramModel _instagramModel;

        public ICommand SelectedPathCommand { get; }

        private List<MediaElement> _selectableCollection = null!;
        public List<MediaElement> SelectableCollection
        {
            get { return _selectableCollection; }
            set { RaiseAndSetIfChanged(ref _selectableCollection, value); }
        }

        public InstagramVM()
        {
            InfoText = "Instagram";

            TextChangedCommand = new RelayCommand(TextChangedCommandExecute, TextChangedCommandCanExecute);
            ButtonClickCommand = new RelayCommand(ClickCommandExecute, ClickCommandCanExecute);

            _instagramModel = new InstagramModel();

            Mediator.InstaApiChanged += (sender, e) =>
            {
                if (sender != this)
                    _instagramModel.Api = e;
            };
            Mediator.NotifyInstaApiChanged(this, _instagramModel.Api);

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

            SelectedPathCommand = new RelayCommand(obj =>
            {
                if (!string.IsNullOrWhiteSpace(SelectedPath))
                    CommonSettingsManager.ChangeToCommonSettings("instagramPath", SelectedPath);
            });

            SelectedPath = CommonSettingsManager.ReadFromCommonSettings("instagramPath");
        }

        protected override async void TextChangedCommandExecute(object? parameter)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Loading", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            var infoResult = await _instagramModel.InstagramService.MediaProcessor.GetInfosAsync(InputText);

            if (await AcceptChallangeIfRequired(infoResult))
            {
                infoResult = await _instagramModel.InstagramService.MediaProcessor.GetInfosAsync(InputText);
            }

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
                source[i] = new MediaElement { Source = new Uri(infos.ElementAt(i).Uri) };
            }

            source.First().MediaOpened += (sender, e) => cancellationTokenSource.Cancel();
            MediaSource = source;
        }

        protected override bool TextChangedCommandCanExecute(object? parameter)
        {
            if (!_instagramModel.Api.IsUserAuthenticated)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "User not logged";
                return false;
            }

            InputText = InputText?.Trim();

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
            var cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Installing", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            IsDownloadButtonEnabled = false;

            var filteredInfos = new List<InstaMediaInfo>();
            foreach (var mediaElement in SelectableCollection)
            {
                foreach (var info in _instagramModel.Infos!)
                {
                    if (mediaElement is MediaElement media && media.Source.ToString().Contains(info.Uri))
                    {
                        filteredInfos.Add(info);
                    }
                }
            }

            IResult<IEnumerable<InstaMediaStream>> instaMediaStreamsResult =
                await _instagramModel.InstagramService.StreamProcessor.GetMediaStreamsAsync(filteredInfos);

            if (instaMediaStreamsResult is null || !instaMediaStreamsResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = instaMediaStreamsResult?.Info.ToString() ?? "No streams has been found";
                cancellationTokenSource.Cancel();
                return;
            }

            var finalResult = await _instagramModel.InstagramService.MediaProcessor
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

        private async Task<bool> AcceptChallangeIfRequired<T>(IResult<T> result)
        {
            if (result.Info.NeedsChallenge)
            {
                var acceptResult = await _instagramModel.InstagramService.Api.AcceptChallengeAsync();
                if (!acceptResult.Succeeded)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
