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
    public enum InfoSignState : byte { Neutral, Success, Bad, }

    public partial class InfoSign : UserControl
    {
        public const string INFO = "\u2139", CHECK = "\u2713", CROSS = "\u2715";

        public InfoSignState State
        {
            get { return (InfoSignState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(InfoSignState), typeof(InfoSign), new PropertyMetadata((d, e) =>
            {
                InfoSign owner = (InfoSign)d;
                (owner.Background, owner.Icon) = (InfoSignState)e.NewValue switch
                {
                    InfoSignState.Neutral => (owner.NeutralBackground, INFO),
                    InfoSignState.Success => (owner.SuccessBackground, CHECK),
                    InfoSignState.Bad => (owner.BadBackground, CROSS),
                    _ => (owner.NeutralBackground, INFO),
                };
            }));

        public Brush NeutralBackground
        {
            get { return (Brush)GetValue(NeutralBackgroundProperty); }
            set { SetValue(NeutralBackgroundProperty, value); }
        }

        public static readonly DependencyProperty NeutralBackgroundProperty =
            DependencyProperty.Register("NeutralBackground", typeof(Brush), typeof(InfoSign), new PropertyMetadata(Brushes.LightGray));

        public Brush SuccessBackground
        {
            get { return (Brush)GetValue(SuccessBackgroundProperty); }
            set { SetValue(SuccessBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SuccessBackgroundProperty =
            DependencyProperty.Register("SuccessBackground", typeof(Brush), typeof(InfoSign), new PropertyMetadata(Brushes.Lime));

        public Brush BadBackground
        {
            get { return (Brush)GetValue(BadBackgroundProperty); }
            set { SetValue(BadBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BadBackgroundProperty =
            DependencyProperty.Register("BadBackground", typeof(Brush), typeof(InfoSign), new PropertyMetadata(Brushes.Red));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(InfoSign), new PropertyMetadata(INFO));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(InfoSign), new PropertyMetadata(new CornerRadius(double.MaxValue)));

        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(InfoSign), new PropertyMetadata(Brushes.LightGray));

        public InfoSign()
        {
            InitializeComponent();

            MouseRightButtonDown += (sender, e) =>
            {
                if (ToolTip is string toolTip)
                {
                    Clipboard.SetText(toolTip);
                }
            };
        }
    }
}
