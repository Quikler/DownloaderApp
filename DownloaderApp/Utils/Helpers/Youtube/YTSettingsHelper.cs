using AngleSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace DownloaderApp.Utils.Helpers.Youtube;

public static class YTSettingsHelper
{
    public static Thumbnail GetByResolution(this IReadOnlyList<Thumbnail> thumbnails,
        Resolution resolution)
    {
        if (resolution.Width == 1 && resolution.Height == 1) return thumbnails.GetWithHighestResolution();
        if (resolution.Width == -1 && resolution.Height == -1) return thumbnails.MinBy(t => t.Resolution.Height)!;
    
        return thumbnails.First(t => t.Resolution == resolution);
    }

    public static IVideoStreamInfo GetByVideoQualityExtended(this StreamManifest manifest, int quality)
    {
        if (quality == 1) return manifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();
        if (quality == -1) return manifest.GetVideoOnlyStreams().OrderBy(s => s.VideoQuality.MaxHeight).First();

        return manifest.GetByVideoQuality(quality);
    }

    public static IVideoStreamInfo GetByVideoQuality(this StreamManifest manifest, int quality)
    {
        if (quality < 144 || quality > 4320)
        {
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 144p and 4320p.");
        }

        var streams = manifest.GetVideoOnlyStreams();

        foreach (var stream in streams)
        {
            if (stream.VideoQuality.MaxHeight == quality)
            {
                return stream;
            }

            if (stream.VideoQuality.MaxHeight < quality)
            {
                return stream;
            }
        }

        return null!;
    }
}