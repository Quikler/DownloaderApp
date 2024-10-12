using System.IO;

namespace DownloaderApp.Utils.Helpers
{
    public static class PathHelper
    {
        public static string CreateValidFileName(string str)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            var output = new System.Text.StringBuilder();
            bool lastCharWasSpace = false;

            foreach (char c in str)
            {
                if (invalidChars.Contains(c))
                    continue;

                if (char.IsWhiteSpace(c))
                {
                    if (lastCharWasSpace)
                        continue;

                    lastCharWasSpace = true;
                }
                else
                {
                    lastCharWasSpace = false;
                }

                output.Append(c);
            }

            return output.ToString().Trim();
        }

        public static string CreateValidFilePath(string folderPath, string fileName, string extension)
            => Path.Combine(folderPath, CreateValidFileName(fileName)) + GetExtensionWithPeriod(extension);

        public static string CreateValidFilePath(string folderPath, string fileNameWithExtension)
            => Path.Combine(folderPath, CreateValidFileName(fileNameWithExtension));

        public static string ChangeDirectory(string path, string newDirectory)
            => Path.Combine(newDirectory, Path.GetFileName(path));

        private static string GetExtensionWithPeriod(string extension)
            => extension.Contains('.') ? extension : $".{extension}";
    }
}
