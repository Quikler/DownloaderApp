using System.Net.Http;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using YoutubeExplode;
using System.IO;
using YoutubeExplode.Common;

namespace DownloaderApp.Utils.Helpers.Youtube;

public static class YTHelper
{
    public static YoutubeClient Client { get; } = new YoutubeClient();

    public static async Task<(Video, StreamManifest)> GetInfoAsync(string url, CancellationToken cancellationToken = default)
    {
        Video video = await Client.Videos.GetAsync(url, cancellationToken);
        StreamManifest manifest = await Client.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
        return (video, manifest);
    }

    public static async Task<StreamManifest> GetManifestAsync(string url)
        => await Client.Videos.Streams.GetManifestAsync(url);

    public static async Task<Video> GetVideoAsync(string url)
        => await Client.Videos.GetAsync(url);

    public static async Task<byte[]> GetThumbnailBytesAsync(string thumbnailUrl)
    {
        using var hc = new HttpClient();
        return await hc.GetByteArrayAsync(thumbnailUrl);
    }

    public static Task<byte[]> GetThumbnailBytesAsync(Thumbnail thumbnail) => GetThumbnailBytesAsync(thumbnail.Url);

    public static async Task<string> DownloadToTempAsync(
        IStreamInfo streamInfo, CancellationToken cancellationToken = default)
    {
        string temp = Path.GetTempFileName();
        await Client.Videos.Streams.DownloadAsync(streamInfo, temp, cancellationToken: cancellationToken);
        return temp;
    }
}
