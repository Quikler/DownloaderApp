using DownloaderApp.MVVM.Abstractions.Interfaces;
using DownloaderApp.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DownloaderApp.MVVM.Model
{
    internal class InputOutputModel : IInputOutputModel
    {
        public InfoSignState InfoSignState { get; set; }
        public string? InfoSignToolTip { get; set; }
        public string? InfoText { get; set; }
        public string? InputText { get; set; }
        public string? SelectedPath { get; set; }
        public MediaElement[]? MediaSource { get; set; }
        public bool IsDownloadButtonEnabled { get; set; }
    }
}
