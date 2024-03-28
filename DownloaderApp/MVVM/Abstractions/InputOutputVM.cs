using DownloaderApp.MVVM.Abstractions.Interfaces;
using DownloaderApp.MVVM.Model;
using DownloaderApp.UserControls;
using System.Windows.Controls;
using System.Windows.Input;

namespace DownloaderApp.MVVM.Abstractions
{
    internal abstract class InputOutputVM<M> : BaseVM, IInputOutputVM where M : IInputOutputModel
    {
        public abstract ICommand TextChangedCommand { get; }
        public abstract ICommand ButtonClickCommand { get; }

        protected abstract M Model { get; }

        protected abstract void TextChangedCommandExecute(object? parameter);
        protected abstract bool TextChangedCommandCanExecute(object? parameter);

        protected abstract void ClickCommandExecute(object? parameter);
        protected abstract bool ClickCommandCanExecute(object? parameter);

        public virtual string? InfoText
        {
            get { return Model.InfoText; }
            set
            {
                if (Model.InfoText != value)
                {
                    Model.InfoText = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual string? InputText
        {
            get { return Model.InputText; }
            set
            {
                if (Model.InputText != value)
                {
                    Model.InputText = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual string? SelectedPath
        {
            get { return Model.SelectedPath; }
            set
            {
                if (Model.SelectedPath != value)
                {
                    Model.SelectedPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual MediaElement[]? MediaSource
        {
            get { return Model.MediaSource; }
            set
            {
                if (Model.MediaSource != value)
                {
                    Model.MediaSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual InfoSignState InfoSignState
        {
            get { return Model.InfoSignState; }
            set
            {
                if (Model.InfoSignState != value)
                {
                    Model.InfoSignState = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual string? InfoSignToolTip
        {
            get { return Model.InfoSignToolTip; }
            set
            {
                if (Model.InfoSignToolTip != value)
                {
                    Model.InfoSignToolTip = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual bool IsDownloadButtonEnabled
        {
            get { return Model.IsDownloadButtonEnabled; }
            set
            {
                if (Model.IsDownloadButtonEnabled != value)
                {
                    Model.IsDownloadButtonEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
