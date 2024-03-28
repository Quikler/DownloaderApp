using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DownloaderApp.MVVM.Model
{
    internal class ComboBoxModel
    {
        public Geometry? ClipGeometry { get; set; }
        public Geometry? Geometry { get; set; }
        public string? Title { get; set; }
    }
}
