using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DownloaderApp.MVVM.Abstractions
{
    internal abstract class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}