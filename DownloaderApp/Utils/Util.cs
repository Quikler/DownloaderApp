using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DownloaderApp.Utils
{
    internal static class Util
    {
        public static bool HasTimeSpan(this MediaElement element) => element.NaturalDuration.HasTimeSpan;

        public static MediaState GetCurrentState(this MediaElement mediaElement)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance)!;
            object helperObject = hlp.GetValue(mediaElement)!;
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance)!;
            MediaState state = (MediaState)stateField.GetValue(helperObject)!;
            return state;
        }

        public static void AnimatedWaiting(string? initialText, string tempText, int dotChangeDelay,
            CancellationToken cancellationToken, IProgress<string?> progress)
        {
            string? original = initialText;
            cancellationToken.Register(() => progress.Report(original));

            Task.Run(async () =>
            {
                const char DOT = '.';
                initialText = tempText;

                while (true)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        initialText += DOT;
                        progress.Report(initialText);
                        await Task.Delay(dotChangeDelay, cancellationToken);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        initialText = initialText.Remove(initialText.Length - 1);
                        progress.Report(initialText);
                        await Task.Delay(dotChangeDelay, cancellationToken);
                    }
                }
            }, cancellationToken);
        }
    }
}
