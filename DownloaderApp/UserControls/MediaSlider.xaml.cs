using DownloaderApp.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DownloaderApp.UserControls
{
    public partial class MediaSlider : UserControl
    {
        public MediaElement TargetAudioSource
        {
            get { return (MediaElement)GetValue(TargetAudioSourceProperty); }
            set { SetValue(TargetAudioSourceProperty, value); }
        }

        public static readonly DependencyProperty TargetAudioSourceProperty =
            DependencyProperty.Register("TargetAudioSource", typeof(MediaElement), typeof(MediaSlider));

        public MediaElement Target
        {
            get { return (MediaElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(MediaElement), typeof(MediaSlider), new((d, e) =>
            {
                MediaSlider mediaSlider = (MediaSlider)d;

                if (e.OldValue is MediaElement oldValue)
                    oldValue.MediaOpened -= mediaSlider.OnMediaOpened; // remove from old target OnMediaOpened event

                if (e.NewValue is MediaElement newValue)
                    newValue.MediaOpened += mediaSlider.OnMediaOpened; // add to new target OnMediaOpened event
            }));

        private const string TIME_FORMAT = "hh\\:mm\\:ss\\.f";
        private static readonly string Zero = TimeSpan.Zero.ToString(TIME_FORMAT);
        private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };

        public MediaSlider()
        {
            InitializeComponent();

            _slider.PreviewKeyDown += (sender, e) =>
            {
                if (Target is null || !Target.HasTimeSpan())
                    return;

                if (e.Key == System.Windows.Input.Key.Left || e.Key == System.Windows.Input.Key.Down
                    || e.Key == System.Windows.Input.Key.Right || e.Key == System.Windows.Input.Key.Up)
                {
                    dispatcherTimer.Stop();
                }
            };

            _slider.PreviewKeyUp += (sender, e) =>
            {
                if (Target is null || !Target.HasTimeSpan())
                    return;

                if (e.Key == System.Windows.Input.Key.Left || e.Key == System.Windows.Input.Key.Down
                    || e.Key == System.Windows.Input.Key.Right || e.Key == System.Windows.Input.Key.Up)
                {
                    var position = TimeSpan.FromSeconds(_slider.Value);
                    Target.Position = position;
                    TargetAudioSource.Position = position;

                    _currentTime.Text = position.ToString(TIME_FORMAT);
                    dispatcherTimer.Start();
                }
            };

            // initialize a tick event
            dispatcherTimer.Tick += (sender, e) =>
            {
                _slider.Value = Target.Position.TotalSeconds; // increase slider value per tick
                _currentTime.Text = Target.Position.ToString(TIME_FORMAT); // set a current time to target position
            };
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();

            if (Target is null || !Target.HasTimeSpan())
                return;

            _slider.Value = 0;
            _slider.Maximum = Target.NaturalDuration.TimeSpan.TotalSeconds;

            _endTime.Text = Target.NaturalDuration.TimeSpan.ToString(TIME_FORMAT);
            _currentTime.Text = Zero;

            dispatcherTimer.Start();

            _slider.Focus();
        }

        private void _slider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Target is null || !Target.HasTimeSpan())
                return;

            dispatcherTimer.Stop();
            Point mousePosition = e.GetPosition(_slider);
            _slider.Value = mousePosition.X / _slider.ActualWidth * (_slider.Maximum - _slider.Minimum);
        }

        private void _slider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Target is null || !Target.HasTimeSpan())
                return;

            dispatcherTimer.Start();

            var position = TimeSpan.FromSeconds(_slider.Value);
            Target.Position = position;
            TargetAudioSource.Position = position;

            _currentTime.Text = position.ToString(TIME_FORMAT);
        }
    }
}
