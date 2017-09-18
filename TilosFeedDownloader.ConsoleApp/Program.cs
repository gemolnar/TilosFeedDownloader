using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TilosFeedDownloader.BusinessLogic;

namespace TilosFeedDownloader.ConsoleApp
{

    public class TilosShowItem
    {
        public DateTimeOffset PublishDate { get; internal set; }
        public string Title { get; internal set; }
        public string Summary { get; internal set; }
        public Uri Mp3Uri { get; internal set; }
        public TilosShow Show { get; internal set; }

        public override string ToString()
        {
            return $"[{PublishDate.ToString("yyyy-MM-dd")}] {Show.Title} {Title}";
        }
    }

    public class TilosFeedProcessor
    {
        public bool IsProcessing { get; set; }
        public TilosDownloaderConfig Config { get; set; }
        private FileSystemManager FileSystemManager { get; }

        public TilosFeedProcessor(TilosDownloaderConfig config, FileSystemManager fileSystemManager)
        {
            Config = config;
            FileSystemManager = fileSystemManager ?? throw new ArgumentNullException(nameof(fileSystemManager));
        }



        public TilosShowItem GetFirstMissingItem()
        {
            var feedReader = new TilosFeedReader(Config);
            feedReader.ReadFeeds();
            foreach (var feed in feedReader.ShowFeeds)
            {
                foreach (SyndicationItem item in feed.FeedItems)
                {
                    var tsi = new TilosShowItem();
                    tsi.PublishDate = item.PublishDate;
                    tsi.Title = item.Title.Text;
                    tsi.Summary = item.Summary.Text;
                    tsi.Mp3Uri = item.Links.Where(l => l.MediaType == "audio/mpeg").First().Uri;
                    tsi.Show = feed.Show;

                    //Console.WriteLine(tsi);
                    if (DateTimeOffset.Now.Subtract(tsi.PublishDate).TotalDays > 21)
                        continue;
                    // Check if the item is missing
                    if (!FileSystemManager.IsDownloaded(tsi))
                        return tsi;
                }
            }
            return null;
        }


    }

    class Program
    {

        static void Main(string[] args)
        {
            // Read the current configuration
            var configContent = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<TilosDownloaderConfig>(configContent);

            // Create a FeedProcessor
            var fileSystemManager = new FileSystemManager(config);
            var feedProcessor = new TilosFeedProcessor(config, fileSystemManager);
            var downloadEngine = new DownloadEngine(fileSystemManager);

            while(true)
            {
                if (downloadEngine.IsBusy)
                {
                    // Log current status (what is downloading, etc...)
                    Console.WriteLine(downloadEngine.CurrentStatus);
                    Thread.Sleep(5000);
                }
                else
                {
                    var pendingItem = feedProcessor.GetFirstMissingItem();
                    if (pendingItem != null)
                    {
                        downloadEngine.DownloadShowItem(pendingItem);
                        // download it
                    }
                    Thread.Sleep(5000);
                }
            }

        }


    }
}
