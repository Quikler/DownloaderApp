using DownloaderApp.MVVM.Model;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderApp.Utils
{
    public static class Mediator
    {
        public static event EventHandler<InstaApiEventArgs>? IInstaApiChanged;
        public static void NotifyIInstaApiChanged(object sender, InstaApiEventArgs args) => IInstaApiChanged?.Invoke(sender, args);
    }

    public class InstaApiEventArgs : EventArgs
    {
        public IInstaApi Api { get; set; }
        public InstaApiEventArgs(IInstaApi api) => Api = api;
    }
}
