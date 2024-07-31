using DownloaderApp.MVVM.Abstractions.Interfaces;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.Abstractions
{
    internal abstract class InputOutputVM : BaseVM, IInputOutputVM
    {
        public abstract ICommand TextChangedCommand { get; }
        public abstract ICommand ButtonClickCommand { get; }

        protected abstract void TextChangedCommandExecute(object? parameter);
        protected abstract bool TextChangedCommandCanExecute(object? parameter);

        protected abstract void ClickCommandExecute(object? parameter);
        protected abstract bool ClickCommandCanExecute(object? parameter);

        private string? _infoText;
        public virtual string? InfoText
        {
            get { return _infoText; }
            set { RaiseAndSetIfChanged(ref _infoText, value); }
        }

        private string? _inputText;
        public virtual string? InputText
        {
            get { return _inputText; }
            set { RaiseAndSetIfChanged(ref _inputText, value); }
        }

        private string? _selectedPath;
        public virtual string? SelectedPath
        {
            get { return _selectedPath; }
            set { RaiseAndSetIfChanged(ref _selectedPath, value); }
        }

        private MediaElement[]? _mediaSource;
        public virtual MediaElement[]? MediaSource
        {
            get { return _mediaSource; }
            set { RaiseAndSetIfChanged(ref _mediaSource, value); }
        }

        private InfoSignState _infoSignState;
        public virtual InfoSignState InfoSignState
        {
            get { return _infoSignState; }
            set { RaiseAndSetIfChanged(ref _infoSignState, value); }
        }

        private string? _infoSignToolTip;
        public virtual string? InfoSignToolTip
        {
            get { return _infoSignToolTip; }
            set { RaiseAndSetIfChanged(ref _infoSignToolTip, value); }
        }

        private bool _isDownloadButtonEnabled;
        public virtual bool IsDownloadButtonEnabled
        {
            get { return _isDownloadButtonEnabled; }
            set { RaiseAndSetIfChanged(ref _isDownloadButtonEnabled, value); }
        }
    }
}
