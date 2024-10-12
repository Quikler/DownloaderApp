using DownloaderApp.Exceptions;
using DownloaderApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderApp.Utils.Helpers
{
    public static class FFmpegHelper
    {
        private const string FFMPEG_EXE_FILE = "ffmpeg.exe";

        private const string VIDEO_TO_AUDIO =
            "-hide_banner -loglevel error -y -i \"{0}\" -i \"{1}\"" +
            " -map 0:a -c:a libmp3lame -id3v2_version 3 {2}" +
            " -map 1 -metadata:s:v comment=\"Cover (front)\" \"{3}\"";

        private const string METADATA =
            "-metadata artist=\"{0}\"" +
            " -metadata title=\"{1}\"" +
            " -metadata date=\"{2}\"" +
            " -metadata album=\"{3}\"" +
            " -metadata album_artist=\"{4}\"";

        private const string VIDEO =
            "-hide_banner -loglevel error -y -i \"{0}\" -c copy {1} \"{2}\"";

        private const string MERGE_VIDEO_AUDIO =
            "-hide_banner -loglevel error -y -i \"{0}\" -i \"{1}\" {2} -c:v copy -c:a aac \"{3}\"";

        private static string _ffmpegExeFilePath = $"{AppContext.BaseDirectory}{FFMPEG_EXE_FILE}";
        public static string FFmpegExeFilePath
        {
            get => _ffmpegExeFilePath;
            set
            {
                _ffmpegExeFilePath = value;
                ffmpegProcessStartInfo.FileName = _ffmpegExeFilePath;
            }
        }

        private static readonly ProcessStartInfo ffmpegProcessStartInfo = new()
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = _ffmpegExeFilePath,
        };

        public static async Task ExecuteAsync(string arguments, CancellationToken cancellationToken = default)
        {
            ffmpegProcessStartInfo.Arguments = arguments;

            using Process process = new();
            process.StartInfo = ffmpegProcessStartInfo;

            string? lastErrorLine = null;
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data is not null)
                    lastErrorLine += e.Data;
            };

            process.Start();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);
            FFmpegException.ThrowIf(process.ExitCode != 0, process.ExitCode, lastErrorLine);
        }

        public static async Task CreateAudioAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default)
        {
            string tempCoverFilePath = Path.GetTempFileName();

            try
            {
                // create valid front cover for audio
                if (options.Thumbnail is not null)
                    await File.WriteAllBytesAsync(tempCoverFilePath, options.Thumbnail, cancellationToken);

                string command = string.Format(VIDEO_TO_AUDIO, tempVideoFilePath, tempCoverFilePath,
                    string.Format(METADATA, new object?[]
                    {
                        options.Author,
                        options.Title,
                        options.Date,
                        options.Album,
                        options.AlbumArtist
                    }), destinationFilePath);

                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteTemp)
                {
                    File.Delete(tempVideoFilePath);
                    File.Delete(tempCoverFilePath);
                }
            }
        }

        public static async Task CreateVideoAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default)
        {
            try
            {
                string command = string.Format(VIDEO, tempVideoFilePath,
                    string.Format(METADATA, new object?[]
                    {
                        options.Author,
                        options.Title,
                        options.Date,
                        options.Album,
                        options.AlbumArtist
                    }), destinationFilePath);

                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteTemp)
                {
                    File.Delete(tempVideoFilePath);
                }
            }
        }

        public static async Task MergeVideoAndAudioAsync(string destinationFilePath, string videoFilePath,
            string audioFilePath, FFmpegOptions options, bool deleteVideoFile = false, bool deleteAudioFile = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string command = string.Format(MERGE_VIDEO_AUDIO, videoFilePath, audioFilePath,
                    string.Format(METADATA, new object?[]
                    {
                        options.Author,
                        options.Title,
                        options.Date,
                        options.Album,
                        options.AlbumArtist
                    }), destinationFilePath);

                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteAudioFile) { File.Delete(audioFilePath); }
                if (deleteVideoFile) { File.Delete(videoFilePath); }
            }
        }
    }
}
