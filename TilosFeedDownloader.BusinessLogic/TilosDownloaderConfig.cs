using System.Collections.Generic;

namespace TilosFeedDownloader.BusinessLogic
{
    public class TilosDownloaderConfig
    {
        public List<TilosShow> Shows { get; set; }
        public string DownloadingFolder { get; set; }
        public string FinishedFolder { get; set; }

        public TilosDownloaderConfig()
        {
            Shows = new List<TilosShow>();
        }
    }

}
