﻿using DownloaderApp.Models;
using DownloaderApp.MVVM.Abstractions;
using DownloaderApp.MVVM.Model;
using DownloaderApp.Settings;
using DownloaderApp.Settings.Parsers;
using DownloaderApp.UserControls;
using DownloaderApp.Utils;
using DownloaderApp.Utils.Helpers;
using DownloaderApp.Utils.Helpers.Youtube;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeExplode.Common;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace DownloaderApp.MVVM.ViewModel
{
    internal class YoutubeVM : InputOutputVM
    {
        public List<ComboBoxModel> MediaFormats { get; }

        public override ICommand TextChangedCommand { get; }
        public override ICommand ButtonClickCommand { get; }

        private readonly YoutubeModel _youtubeModel;

        public ICommand SelectedPathCommand { get; }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { RaiseAndSetIfChanged(ref _selectedIndex, value); }
        }

        private string? _audioSource;
        public string? AudioSource
        {
            get { return _audioSource; }
            set { RaiseAndSetIfChanged(ref _audioSource, value); }
        }

        public YoutubeVM()
        {
            _youtubeModel = new YoutubeModel();

            MediaFormats = new List<ComboBoxModel>
            {
                new()
                {
                    Title = "Audio",
                    Geometry = Geometry.Parse("F0 M104.23,122.88z M0,0z M87.9,78.04C90.64,77.56,93.23,77.64,95.5,78.17L95.5,24.82 39.05,41.03 39.05,102.98C39.08,103.32 39.1,103.67 39.1,104.01 39.1,104.01 39.1,104.02 39.1,104.02 39.1,112.36 30.35,120.64 19.55,122.51 8.76,124.37 0,119.12 0,110.77 0,102.43 8.76,94.15 19.55,92.29 23.61,91.59 27.39,91.9 30.52,93L30.52,16.74 30.99,16.74 104.04,0 104.04,85.92C104.17,86.55 104.24,87.19 104.24,87.83 104.24,87.83 104.24,87.83 104.24,87.84 104.24,94.81 96.92,101.73 87.91,103.28 78.89,104.84 71.58,100.45 71.58,93.48 71.57,86.51 78.88,79.59 87.9,78.04L87.9,78.04 87.9,78.04z"),
                },
                new()
                {
                    Title = "Video",
                    Geometry = Geometry.Parse("F1 M122.88,95.8z M0,0z M12.14,0L32.8,0 62.23,0 91.03,0 110.74,0C114.08,0 117.12,1.37 119.32,3.56 121.52,5.76 122.88,8.8 122.88,12.14L122.88,19.27 122.88,76.52 122.88,83.61C122.88,86.95 121.51,89.99 119.32,92.19 117.12,94.39 114.08,95.75 110.74,95.75L91.54,95.75C91.38,95.78 91.21,95.79 91.03,95.79 90.86,95.79 90.69,95.78 90.52,95.75L62.74,95.75C62.58,95.78 62.41,95.79 62.23,95.79 62.06,95.79 61.89,95.78 61.72,95.75L33.31,95.75C33.15,95.78 32.98,95.79 32.8,95.79 32.63,95.79 32.46,95.78 32.29,95.75L12.14,95.75C8.8,95.75 5.76,94.38 3.56,92.19 1.36,90 0,86.95 0,83.61L0,76.52 0,19.27 0,12.14C0,8.8 1.37,5.76 3.56,3.56 5.76,1.37 8.8,0 12.14,0L12.14,0z M55.19,31.24L75.72,45.56C76.04,45.76 76.33,46.04 76.56,46.37 77.48,47.7 77.14,49.51 75.82,50.43L55.37,64.57C54.87,64.98 54.22,65.23 53.52,65.23 51.9,65.23 50.59,63.92 50.59,62.3L50.59,33.63 50.6,33.63C50.6,33.05 50.77,32.47 51.12,31.96 52.05,30.64 53.87,30.32 55.19,31.24L55.19,31.24z M93.95,79.45L93.95,89.9 110.73,89.9C112.46,89.9 114.03,89.19 115.17,88.05 116.31,86.91 117.02,85.34 117.02,83.61L117.02,79.45 93.95,79.45 93.95,79.45z M88.1,89.9L88.1,79.45 65.16,79.45 65.16,89.9 88.1,89.9 88.1,89.9z M59.31,89.9L59.31,79.45 35.73,79.45 35.73,89.9 59.31,89.9 59.31,89.9z M29.87,89.9L29.87,79.45 5.85,79.45 5.85,83.61C5.85,85.34 6.56,86.91 7.7,88.05 8.84,89.19 10.41,89.9 12.14,89.9L29.87,89.9 29.87,89.9z M5.85,73.6L32.8,73.6 62.23,73.6 91.03,73.6 117.03,73.6 117.03,22.2 91.03,22.2 62.23,22.2 32.8,22.2 5.85,22.2 5.85,73.6 5.85,73.6z M88.1,16.35L88.1,5.85 65.16,5.85 65.16,16.34 88.1,16.34 88.1,16.35z M93.95,5.85L93.95,16.34 117.02,16.34 117.02,12.14C117.02,10.41 116.31,8.84 115.17,7.7 114.03,6.56 112.46,5.85 110.73,5.85L93.95,5.85 93.95,5.85z M59.31,16.35L59.31,5.85 35.73,5.85 35.73,16.34 59.31,16.34 59.31,16.35z M29.87,16.35L29.87,5.85 12.14,5.85C10.41,5.85 8.84,6.56 7.7,7.7 6.56,8.84 5.85,10.41 5.85,12.14L5.85,16.34 29.87,16.34 29.87,16.35z")
                },
            };

            InfoText = "Youtube";

            TextChangedCommand = new RelayCommand(TextChangedCommandExecute, TextChangedCommandCanExecute);
            ButtonClickCommand = new RelayCommand(ClickCommandExecute, ClickCommandCanExecute);

            SelectedPathCommand = new RelayCommand(obj => SettingsManager.Set("youtubePath", SelectedPath));
            SelectedPath = SettingsManager.Get<string?>("youtubePath", null);
        }

        protected override async void TextChangedCommandExecute(object? parameter)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Loading", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            IsDownloadButtonEnabled = false;

            try
            {
                _youtubeModel.Info = await YTHelper.GetInfoAsync(InputText!);

                if (SettingsManager.Get("loadPreview", false))
                {
                    var quality = SettingsManager.Get("videoQuality", 1, new VideoQualityParser());

                    _youtubeModel.StreamInfo = _youtubeModel.Info.Value.manifest
                        .GetByVideoQualityExtended(quality);

                    if (_youtubeModel.StreamInfo is not null)
                    {
                        var media = new MediaElement { Source = new Uri(_youtubeModel.StreamInfo.Url) };
                        
                        var audioSource = _youtubeModel.Info.Value.manifest
                            .GetAudioOnlyStreams()
                            .GetWithHighestBitrate();
                        _youtubeModel.AudioStreamInfo = audioSource;

                        AudioSource = audioSource.Url;

                        media.MediaOpened += (sender, e) =>
                        {
                            cancellationTokenSource.Cancel();
                            IsDownloadButtonEnabled = true;
                        };

                        MediaSource = new[] { media };
                    }
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    IsDownloadButtonEnabled = true;
                }
            }
            catch (YoutubeExplodeException ex)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = ex.Message;

                cancellationTokenSource.Cancel();
                IsDownloadButtonEnabled = true;
                return;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "Preview video loaded";
        }

        protected override bool TextChangedCommandCanExecute(object? parameter)
        {
            InputText = InputText?.Trim();

            if (string.IsNullOrWhiteSpace(InputText) || !YoutubeModel.Regex.IsMatch(InputText))
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = "Input doesn't match a regex";
                return false;
            }

            if (_youtubeModel.Info.HasValue && InputText.Contains(_youtubeModel.Info.Value.video.Url))
            {
                InfoSignState = InfoSignState.Success;
                InfoSignToolTip = "Specified video already in preview";
                return false;
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = "Input matches a regex";

            return IsDownloadButtonEnabled = true;
        }

        protected override async void ClickCommandExecute(object? parameter)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Util.AnimatedWaiting(InfoText, "Installing", 100,
                cancellationTokenSource.Token, new Progress<string?>(str => InfoText = str));

            IsDownloadButtonEnabled = false;
            bool isAudio = SelectedIndex == 0;

            var audioStream = _youtubeModel.AudioStreamInfo ?? 
                _youtubeModel.Info?.manifest
                    .GetAudioOnlyStreams()
                    .GetWithHighestBitrate()!;

            var tempAudioFilePath = await YTHelper.DownloadToTempAsync(audioStream);

            var thumbnailResolution = SettingsManager.Get("thumbnailResolution", new Resolution(480, 360), new ResolutionParser());
            var thumbnail = _youtubeModel.Info?.video.Thumbnails.GetByResolution(thumbnailResolution);

            //(string mediaType, string extenstion, FFmpegOptions options) =
            //    await PrepareDownloadOptionsAsync(_youtubeModel.Info?.video!, isAudio, 
            //    t => t.Resolution.Width >= thumbnailResolution.Width && t.Resolution.Width <= thumbnailResolution.Height);

            (string mediaType, string extension, FFmpegOptions options)
                = PrepareDownloadOptions(_youtubeModel.Info?.video!, isAudio);

            options.Thumbnail = await YTHelper.GetThumbnailBytesAsync(thumbnail!);

            string destinationFilePath = $"{SelectedPath}\\{options.Title}{extension}";

            try
            {
                if (isAudio)
                {
                    await FFmpegHelper.CreateAudioAsync(destinationFilePath, tempAudioFilePath, options, true);
                }
                else
                {
                    var quality = SettingsManager.Get("videoQuality", 1, new VideoQualityParser());

                    if (_youtubeModel.StreamInfo is null || _youtubeModel.StreamInfo.VideoQuality.MaxHeight != quality)
                    {
                        _youtubeModel.StreamInfo = _youtubeModel.Info?.manifest
                            .GetByVideoQualityExtended(quality)!;
                    }

                    var tempVideoFilePath = await YTHelper.DownloadToTempAsync(_youtubeModel.StreamInfo);

                    await FFmpegHelper.MergeVideoAndAudioAsync(destinationFilePath, tempVideoFilePath,
                        tempAudioFilePath, options, true, true);
                }
            }
            catch (Exception ex)
            {
                InfoSignState = InfoSignState.Bad;
                InfoSignToolTip = ex.Message;
                return;
            }
            finally
            {
                IsDownloadButtonEnabled = true;
                cancellationTokenSource.Cancel();
            }

            InfoSignState = InfoSignState.Success;
            InfoSignToolTip = $"Youtube {(isAudio ? "audio" : "video")} <{_youtubeModel.Info?.video.Id}> " +
                $"has been downloaded by path \"{destinationFilePath}\"";
        }

        protected override bool ClickCommandCanExecute(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(SelectedPath) || _youtubeModel.Info is null)
                return false;

            return true;
        }

        private static (string mediaType, string extension, FFmpegOptions options) PrepareDownloadOptions(
            Video video, bool isAudio)
        {
            string mediaType = isAudio ? "Audio" : "Video";
            string extension = isAudio ? ".mp3" : ".mp4";

            var options = new FFmpegOptions
            {
                Author = PathHelper.CreateValidFileName(video.Author.ChannelTitle),
                Title = PathHelper.CreateValidFileName(video.Title),
                Date = video.UploadDate.Year,
                Album = "DownloaderApp",
                AlbumArtist = "Quikler",
            };

            return (mediaType, extension, options);
        }
    }
}