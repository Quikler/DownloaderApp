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
            set
            {
                if (_infoText != value)
                {
                    _infoText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _inputText;
        public virtual string? InputText
        {
            get { return _inputText; }
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _selectedPath;
        public virtual string? SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private MediaElement[]? _mediaSource;
        public virtual MediaElement[]? MediaSource
        {
            get { return _mediaSource; }
            set
            {
                if (_mediaSource != value)
                {
                    _mediaSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private InfoSignState _infoSignState;
        public virtual InfoSignState InfoSignState
        {
            get { return _infoSignState; }
            set
            {
                if (_infoSignState != value)
                {
                    _infoSignState = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _infoSignToolTip;
        public virtual string? InfoSignToolTip
        {
            get { return _infoSignToolTip; }
            set
            {
                if (_infoSignToolTip != value)
                {
                    _infoSignToolTip = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isDownloadButtonEnabled;
        public virtual bool IsDownloadButtonEnabled
        {
            get { return _isDownloadButtonEnabled; }
            set
            {
                if (_isDownloadButtonEnabled != value)
                {
                    _isDownloadButtonEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
