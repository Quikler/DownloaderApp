using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DownloaderApp.UserControls
{
    public partial class DirectoryDialog : UserControl
    {
        private readonly Microsoft.Win32.SaveFileDialog _dialog = new()
        {
            Title = "Select a Directory", // instead of default "Save As"
            Filter = "Directory|*.this.directory", // Prevents displaying files
            FileName = "select" // Filename will then be "select.this.directory"
        };

        public int PathMaxWidth
        {
            get { return (int)GetValue(PathMaxWidthProperty); }
            set { SetValue(PathMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty PathMaxWidthProperty =
            DependencyProperty.Register("PathMaxWidth", typeof(int), typeof(DirectoryDialog), new PropertyMetadata(100));

        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string), typeof(DirectoryDialog));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DirectoryDialog), new PropertyMetadata("Choose directory"));

        public new int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(DirectoryDialog), new PropertyMetadata(0));

        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(DirectoryDialog), new PropertyMetadata(Brushes.Transparent));

        public ICommand SelectedPathCommand
        {
            get { return (ICommand)GetValue(SelectedPathCommandProperty); }
            set { SetValue(SelectedPathCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectedPathCommandProperty =
            DependencyProperty.Register("SelectedPathCommand", typeof(ICommand), typeof(DirectoryDialog));

        public object SelectedPathCommandParameter
        {
            get { return GetValue(SelectedPathCommandParameterProperty); }
            set { SetValue(SelectedPathCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty SelectedPathCommandParameterProperty =
            DependencyProperty.Register("SelectedPathCommandParameter", typeof(object), typeof(DirectoryDialog));

        public DirectoryDialog()
        {
            InitializeComponent();
        }

        private void ChooseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog.ShowDialog() != true)
                return;

            // Remove fake filename from resulting path
            string path = _dialog.FileName.Replace("\\select.this.directory", "");
            path = path.Replace(".this.directory", "");

            // If user has changed the filename, create the new directory
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            // Our final value is in path
            SelectedPath = path;
            if (SelectedPathCommand.CanExecute(SelectedPathCommandParameter))
                SelectedPathCommand.Execute(SelectedPathCommandParameter);
        }
    }
}
