using System;
using System.IO;
using System.Text;
using TilosFeedDownloader.BusinessLogic;

namespace TilosFeedDownloader.ConsoleApp
{

    // Artists: Tilos Talk | Tilos Music
    // Album: Musor (Csonka 2017)
    // A

    //    /TilosDownloaded
    //        /Tilos Talk - Csonka
    //            file1.mp3
    //            file2.mp3
    //        /Tilos Talk - Kolorlokal
    //            file1.mp3
    //            file2.mp3
    //        /Tilos Music - Hotel North Pole
    //            file1.mp3
    //            file2.mp3

    public class FileSystemManager
    {
        public TilosDownloaderConfig Config { get; }
        public FileSystemManager(TilosDownloaderConfig config)
        {
            Config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        public string GenerateFinishedFullPath(TilosShowItem showItem)
        {
            var path = GenerateFullPath(Config.FinishedFolder, showItem);
            return path;
        }

        public string GenerateInProgressFullPath(TilosShowItem showItem)
        {
            var path = GenerateFullPath(Config.DownloadingFolder, showItem);
            return path;
        }


        private string GenerateFullPath(string folder, TilosShowItem showItem)
        {
            var path = Path.Combine(folder, $"Tilos {showItem.Show.Type} - {showItem.Show.Title}", GenerateFilename(showItem));
            return path;
        }

        public string GenerateFilename(TilosShowItem showItem)
        {
            string filename = Sanitize(showItem.Title);
            filename += ".mp3";

            var datestamp = showItem.PublishDate.ToString("yyyy-MM-dd");
            if (!filename.StartsWith(datestamp))
                filename = datestamp + "-" + filename;

            return filename;
        }

        public string Sanitize(string original)
        {
            string sanitized = original.Trim();
            sanitized = sanitized.Replace(" ", "-");
            sanitized = sanitized.Replace(".", "-");
            sanitized = sanitized.Replace(",", "-");
            sanitized = sanitized.Replace("!", "-");
            sanitized = sanitized.Replace("&", "-");
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sanitized = sanitized.Replace(c, '-');
            }
            while (sanitized.Contains("--")) sanitized = sanitized.Replace("--", "-");
            sanitized = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(sanitized));
            sanitized = sanitized.ToLower();
            sanitized = sanitized.Trim('-');
            return sanitized;
        }

        public bool IsDownloaded(TilosShowItem tsi)
        {
            if (File.Exists(GenerateFinishedFullPath(tsi)))
                return true;
            return false;
        }
    }
}
