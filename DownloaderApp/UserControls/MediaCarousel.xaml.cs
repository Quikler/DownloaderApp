using DownloaderApp.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DownloaderApp.UserControls
{
    public partial class MediaCarousel : UserControl
    {
        public bool IsCheckBoxChecked
        {
            get { return (bool)GetValue(IsCheckBoxCheckedProperty); }
            set { SetValue(IsCheckBoxCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckBoxCheckedProperty =
            DependencyProperty.Register("IsCheckBoxChecked", typeof(bool), typeof(MediaCarousel));

        public Brush MenuHoverColor
        {
            get { return (Brush)GetValue(MenuHoverColorProperty); }
            set { SetValue(MenuHoverColorProperty, value); }
        }

        public static readonly DependencyProperty MenuHoverColorProperty =
            DependencyProperty.Register("MenuHoverColor", typeof(Brush), typeof(MediaCarousel), new PropertyMetadata(Brushes.LightGray));

        public Brush MenuForeground
        {
            get { return (Brush)GetValue(MenuForegroundProperty); }
            set { SetValue(MenuForegroundProperty, value); }
        }

        public static readonly DependencyProperty MenuForegroundProperty =
            DependencyProperty.Register("MenuForeground", typeof(Brush), typeof(MediaCarousel), new PropertyMetadata(Brushes.White));

        public Brush MenuBackground
        {
            get { return (Brush)GetValue(MenuBackgroundProperty); }
            set { SetValue(MenuBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MenuBackgroundProperty =
            DependencyProperty.Register("MenuBackground", typeof(Brush), typeof(MediaCarousel), new PropertyMetadata(Brushes.Black));

        public Brush ButtonHoverBackground
        {
            get { return (Brush)GetValue(ButtonHoverBackgroundProperty); }
            set { SetValue(ButtonHoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonHoverBackgroundProperty =
            DependencyProperty.Register("ButtonHoverBackground", typeof(Brush), typeof(MediaCarousel));

        public Brush ButtonDisableBackground
        {
            get { return (Brush)GetValue(ButtonDisableBackgroundProperty); }
            set { SetValue(ButtonDisableBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonDisableBackgroundProperty =
            DependencyProperty.Register("ButtonDisableBackground", typeof(Brush), typeof(MediaCarousel));

        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(MediaCarousel));

        public MediaElement SelectedMedia
        {
            get { return (MediaElement)GetValue(SelectedMediaProperty); }
            private set { SetValue(SelectedMediaProperty, value); }
        }

        public static readonly DependencyProperty SelectedMediaProperty =
            DependencyProperty.Register("SelectedMedia", typeof(MediaElement), typeof(MediaCarousel));

        public string AudioTrackSource
        {
            get { return (string)GetValue(AudioTrackSourceProperty); }
            set { SetValue(AudioTrackSourceProperty, value); }
        }

        public static readonly DependencyProperty AudioTrackSourceProperty =
            DependencyProperty.Register("AudioTrackSource", typeof(string), typeof(MediaCarousel));

        public bool IsSourceSelectable
        {
            get { return (bool)GetValue(IsSourceSelectableProperty); }
            set { SetValue(IsSourceSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSourceSelectableProperty =
            DependencyProperty.Register("IsSourceSelectable", typeof(bool), typeof(MediaCarousel));

        public MediaElement[] Source
        {
            get { return (MediaElement[])GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(MediaElement[]), typeof(MediaCarousel), new((d, e) =>
            {
                MediaCarousel owner = (MediaCarousel)d;
                owner._selectedIndex = 0;

                if ((MediaElement[])e.OldValue is MediaElement[] oldSource)
                {
                    owner.UnsetDefaultSourceBehavior(oldSource);
                }

                MediaElement[] newSource = (MediaElement[])e.NewValue;

                owner.SetDefaultSourceBehavior(newSource);
                owner.SetDefaultSelectableMenu();

                owner.SelectedMedia = newSource[owner._selectedIndex];
            }));

        public List<MediaElement> SelectableCollection
        {
            get { return (List<MediaElement>)GetValue(SelectableCollectionProperty); }
            set { SetValue(SelectableCollectionProperty, value); }
        }

        public static readonly DependencyProperty SelectableCollectionProperty =
            DependencyProperty.Register("SelectableCollection", typeof(List<MediaElement>),
                typeof(MediaCarousel), new PropertyMetadata(new List<MediaElement>()));

        private int _selectedIndex;

        public MediaCarousel()
        {
            InitializeComponent();
        }

        #region Default Behavoiurs
        private void SetDefaultSourceBehavior(MediaElement[] source)
        {
            if (source is null)
                return;

            foreach (MediaElement media in source)
            {
                media.Loaded += OnMediaLoaded;
                media.Unloaded += OnMediaUnloaded;
                media.MediaOpened += OnMediaOpened;

                media.LoadedBehavior = MediaState.Manual;
                media.UnloadedBehavior = MediaState.Manual;
                media.ScrubbingEnabled = true;
            }
        }

        private void UnsetDefaultSourceBehavior(MediaElement[] source)
        {
            if (source is null)
                return;

            foreach (MediaElement media in source)
            {
                media.Loaded -= OnMediaLoaded;
                media.Unloaded -= OnMediaUnloaded;
                media.MediaOpened -= OnMediaOpened;
                media.Stop();
                media.Source = null;

                _audioTrack.Stop();
                //_audioTrack.Source = null;
            }
        }

        private void SetDefaultSelectableMenu()
        {
            SelectableCollection.Clear();
            IsCheckBoxChecked = false;
        }
        #endregion

        #region Selected Media events
        private void OnMediaLoaded(object sender, RoutedEventArgs e)
        {
            SelectedMedia?.Play();
            _audioTrack?.Play();
            _mediaContentControl.Focus();
        }

        private void OnMediaUnloaded(object sender, RoutedEventArgs e)
        {
            SelectedMedia?.Pause();
            _audioTrack?.Pause();
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            MediaElement? selected = SelectedMedia;
            if (selected is null)
                return;

            _audioTrack?.Stop();
            _audioTrack?.Play();
            _slider.Visibility = selected.HasTimeSpan() ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Commands
        public ICommand ButtonPreviousCommand => new RelayCommand(obj =>
        {
            SelectedMedia?.Pause();
            _audioTrack?.Pause();
            _selectedIndex--;
            SelectedMedia = Source[_selectedIndex];

            IsCheckBoxChecked = IsSourceSelectable && SelectedMedia is not null && SelectableCollection.Contains(SelectedMedia);
        }, obj => _selectedIndex != 0);
        public ICommand ButtonNextCommand => new RelayCommand(obj =>
        {
            SelectedMedia?.Pause();
            _audioTrack?.Pause();
            _selectedIndex++;
            SelectedMedia = Source[_selectedIndex];

            IsCheckBoxChecked = IsSourceSelectable && SelectedMedia is not null && SelectableCollection.Contains(SelectedMedia);
        }, obj => Source is not null && _selectedIndex != Source.Length - 1);

        public ICommand ButtonSelectAllCommand => new RelayCommand(obj =>
        {
            SelectableCollection.Clear();
            SelectableCollection.AddRange(Source!);
            IsCheckBoxChecked = true;
        }, obj => Source is not null && SelectableCollection.Count != Source?.Length);
        public ICommand ButtonDeselectAllCommand => new RelayCommand(obj =>
        {
            SelectableCollection.Clear();
            IsCheckBoxChecked = false;
        });

        public ICommand CheckBoxSelectableCommand => new RelayCommand(obj =>
        {
            if (IsCheckBoxChecked == true)
                SelectableCollection.Add(SelectedMedia!);
            else
                SelectableCollection.Remove(SelectedMedia!);
        }, obj => SelectedMedia is not null);

        public ICommand PreviewLinkNavigateCommand => new RelayCommand(obj =>
        {
            Process.Start(new ProcessStartInfo 
            { 
                UseShellExecute = true, 
                FileName = (string)obj! 
            })?.Dispose();
        }, obj => Source is not null && SelectedMedia is not null && PreviewLink.CommandParameter is not null);
        #endregion

        #region Input
        // on mouse down selected media
        private void OnPausePlaySelectedMedia(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMedia is null || !SelectedMedia.HasTimeSpan())
                return;

            //if (SelectedMedia.GetCurrentState() == MediaState.Play) SelectedMedia.Pause();
            //else SelectedMedia.Play();
            if (SelectedMedia.GetCurrentState() == MediaState.Play)
            {
                SelectedMedia?.Pause();
                _audioTrack?.Pause();
            }
            else
            {
                SelectedMedia?.Play();
                _audioTrack?.Play();
            }
        }

        // tap space to pause/play SelectedMedia
        private void MediaGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space || SelectedMedia is null || !SelectedMedia.HasTimeSpan())
                return;

            if (SelectedMedia.GetCurrentState() == MediaState.Play)
            {
                SelectedMedia?.Pause();
                _audioTrack?.Pause();
            }
            else
            {
                SelectedMedia?.Play();
                _audioTrack?.Play();
            }
        }
        #endregion
    }
}
