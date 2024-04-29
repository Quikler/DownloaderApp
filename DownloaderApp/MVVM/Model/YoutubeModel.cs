using System.Text.RegularExpressions;
using YoutubeExplode.Videos.Streams;
using YTDownloader.CLasses;
using YTDownloader.CLasses.Models;

namespace DownloaderApp.MVVM.Model
{
    internal class YoutubeModel
    {
        public static  Regex Regex { get; } = new Regex(
                @"^(http(s)?://)?(www\.)?(youtube\.com/watch\?v=|youtu.be/|youtube.com/shorts/)([a-zA-Z0-9\-_]{11})((\?|&)\S*)?$",
                RegexOptions.IgnoreCase);
        public YTSimpleInfo? Info { get; set; }

        private static readonly Func<StreamManifest, IStreamInfo> s_audio =
            ms => ms.GetAudioOnlyStreams().GetWithHighestBitrate();

        private static readonly Func<StreamManifest, IStreamInfo> s_muxed =
            ms => ms.GetMuxedStreams().GetWithHighestVideoQuality();

        public async Task<DownloadedMediaInfo> DownloadMediaAsync(string destinationFolder, bool isAudio)
        {
            var selector = isAudio ? s_audio : s_muxed;
            return await YTMediaDownloader.DownloadAsync(Info!, destinationFolder, selector);
        }
    }
}
