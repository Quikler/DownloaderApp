using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using InstagramApiSharp.Classes;
using InstagramService.Classes.Collections;
using InstagramService.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class InstagramVM : InputOutputVM<InstagramModel>
    {
        public override ICommand TextChangedCommand { get; }
        public override ICommand ButtonClickCommand { get; }

        protected override InstagramModel Model { get; }

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

        public InstagramVM()
        {
            Model = new InstagramModel() { InfoText = "Instagram" };

            TextChangedCommand = new RelayCommand(TextChangedCommandExecute, TextChangedCommandCanExecute);
            ButtonClickCommand = new RelayCommand(ClickCommandExecute, ClickCommandCanExecute);
        }

        protected override async void TextChangedCommandExecute(object? parameter)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Loading", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            var infoResult = await Model.InstagramService.MediaHelper.GetInfosAsync(InputText);
            if (!infoResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = infoResult.Info.ToString();
                cancellationTokenSource.Cancel();
                return;
            }

            var infos = Model.Infos = infoResult.Value;
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
            if (string.IsNullOrWhiteSpace(InputText) || !Model.Regex.IsMatch(InputText))
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "Input doesn't match a regex";
                return false;
            }

            if (Model.Infos is not null && InputText.Contains(Model.Infos.First().InitialUri))
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = "Specified media already in preview";
                return false;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "Input matches a regex";
            IsDownloadButtonEnabled = true;
            return IsDownloadButtonEnabled = true;
        }

        protected async override void ClickCommandExecute(object? parameter)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Installing", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            IsDownloadButtonEnabled = false;
            IResult<InstaMediaStreams> instaMediaStreamsResult = SelectableCollection.Count != MediaSource?.Length ?
                await Model.GetStreamsFromAsync(SelectableCollection) :
                await Model.InstagramService.StreamTaker.GetMediaStreamsAsync(Model.Infos);

            if (!instaMediaStreamsResult.Succeeded)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = instaMediaStreamsResult.Info.ToString();
                cancellationTokenSource.Cancel();
                return;
            }

            var finalResult = await Model.InstagramService.MediaHelper
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
            if (string.IsNullOrWhiteSpace(SelectedPath) || Model.Infos is null)
                return false;

            return true;
        }
    }
}
