using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Common;

namespace DownloaderApp.Settings.Parsers
{
    public readonly struct ResolutionParser : ISettingsParser
    {
        public T Parse<T>(string value)
        {
            if (value == "Highest") return (T)Convert.ChangeType(new Resolution(1, 1), typeof(T));
            if (value == "Lowest") return (T)Convert.ChangeType(new Resolution(-1, -1), typeof(T));

            var size = value.Split('x', StringSplitOptions.TrimEntries);

            int width = int.Parse(size[0]);
            int height = int.Parse(size[1]);

            return (T)Convert.ChangeType(new Resolution(width, height), typeof(T));
        }
    }
}
