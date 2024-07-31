using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace DownloaderApp.UserControls
{
    public partial class CustomMbox : Window
    {
        public enum CustomMboxButtons
        {
            Ok, YesNo
        }

        public CustomMboxButtons Buttons
        {
            get { return (CustomMboxButtons)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(CustomMboxButtons), typeof(CustomMbox),
                new PropertyMetadata((d, e) =>
                {
                    CustomMbox customMbox = (CustomMbox)d;
                    customMbox.btnsContainer.Children.Clear();

                    var newValue = (CustomMboxButtons)e.NewValue;
                    customMbox.AddButtons(newValue);
                }));

        private void AddButtons(CustomMboxButtons buttons)
        {
            switch (buttons)
            {
                case CustomMboxButtons.Ok:
                    var okBtn = GenerateButton("Ok");
                    btnsContainer.Children.Add(okBtn);
                    break;
                case CustomMboxButtons.YesNo:
                    var yesBtn = GenerateButton("Yes");
                    var noBtn = GenerateButton("No");

                    btnsContainer.Children.Add(yesBtn);
                    btnsContainer.Children.Add(noBtn);
                    break;
            }
        }

        private Button GenerateButton(string content)
        {
            Button btn = new()
            {
                Width = 70,
                Margin = new Thickness(4, 12, 4, 12),
                Content = content,
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 16,
                Foreground = Brushes.White
            };

            btn.Click += Btn_Click;

            Style buttonStyle = new(typeof(Button));

            buttonStyle.Setters.Add(new Setter(BackgroundProperty, FindResource("DarkBlueBrush")));

            ControlTemplate template = new(typeof(Button));

            FrameworkElementFactory border = new(typeof(Border));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(BackgroundProperty));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));

            FrameworkElementFactory contentPresenter = new(typeof(ContentPresenter));
            contentPresenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            buttonStyle.Setters.Add(new Setter(TemplateProperty, template));

            Trigger isMouseOverTrigger = new()
            {
                Property = IsMouseOverProperty,
                Value = true
            };
            isMouseOverTrigger.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#201658"))));

            buttonStyle.Triggers.Add(isMouseOverTrigger);

            btn.Resources.Add(typeof(Button), buttonStyle);

            return btn;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomMbox));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(CustomMbox));

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

        private void WindowPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartDrag(this);
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion

        public static bool? ShowDialog(string text, string caption, CustomMboxButtons buttons = CustomMboxButtons.Ok)
        {
            var box = new CustomMbox(text, caption)
            {
                Owner = Application.Current.MainWindow,
                Buttons = buttons,
            };
            return box.ShowDialog();
        }

        public CustomMbox()
        {
            InitializeComponent();
            AddButtons(Buttons);
        }

        public CustomMbox(string text, string caption) : this()
        {
            Text = text;
            Caption = caption;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ((Button)sender).Content.ToString() == "Yes";
            Close();
        }
    }
}
