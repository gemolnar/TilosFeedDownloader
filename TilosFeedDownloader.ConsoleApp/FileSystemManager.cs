using System.IO;
using System.Text;
using TilosFeedDownloader.BusinessLogic;

namespace TilosFeedDownloader.ConsoleApp
{
    public class FileSystemManager
    {
        public TilosDownloaderConfig Config { get; }
        public FileSystemManager(TilosDownloaderConfig config)
        {
            Config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        public string GenerateFilename(string showTitle)
        {
            string filename = showTitle;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, ' ');
            }
            filename = filename.Replace('.', ' ');
            filename = filename.Trim();
            while (filename.Contains("  ")) filename = filename.Replace("  ", " ");
            filename = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(filename));
            filename = filename.Replace(' ', '-');
            filename += ".mp3";
            return filename;
        }

    }
}
