using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace DownloaderApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Import user32.dll

        /*
         * The reason is that if a window is maximized, it cannot be dragged back using the DragMove method.
         */

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private static void StartDrag(Window window)
        {
            var helper = new WindowInteropHelper(window);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        #endregion

        #region Title bar methods

        private void ChangeWindowState() => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void WindowPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
                ChangeWindowState();
            else
                StartDrag(this);
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeButton_Click(object sender, RoutedEventArgs e) => ChangeWindowState();

        #endregion
    }
}