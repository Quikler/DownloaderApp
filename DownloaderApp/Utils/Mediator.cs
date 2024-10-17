using InstagramApiSharp.API;

namespace DownloaderApp.Utils
{
    public static class Mediator
    {
        public static event EventHandler<IInstaApi>? InstaApiChanged;
        public static void NotifyInstaApiChanged(object sender, IInstaApi e) => InstaApiChanged?.Invoke(sender, e);
    }
}
