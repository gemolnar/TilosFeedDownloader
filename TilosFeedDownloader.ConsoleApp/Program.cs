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



        public object GetNewItem()
        {
            var feedReader = new TilosFeedReader(Config);
            feedReader.ReadFeeds();
            foreach (var feed in feedReader.ShowFeeds)
            {
                foreach (SyndicationItem item in feed.FeedItems)
                {
                    Console.WriteLine("[{0}][{1}]", item.PublishDate, item.Title.Text);
                    Console.WriteLine($"{item.Summary.Text}");
                    var mp3Uri = item.Links.Where(l => l.MediaType == "audio/mpeg").First().Uri;
                    Console.WriteLine(mp3Uri);

                    //FileSystemManager.
                    //// Generate folder name
                    //var folderName = $"Tilos ({feed.Show.Type}) - {feed.Show.Title}";
                    //var downloadFolder = Path.Combine(Config.DownloadingFolder, folderName);
                    //if (!Directory.Exists(downloadFolder))
                    //{
                    //    Directory.CreateDirectory(downloadFolder);
                    //}

                    //string fileName = GenerateFilename(item.Title.Text);


                    //if (!_webClient.IsBusy)
                    //{
                    //    string targetFileName = Path.Combine(downloadFolder, fileName);
                    //    _webClient.DownloadFileAsync(mp3Uri, targetFileName, targetFileName); // Here pass artist, album, title...
                    //}
                    //Console.WriteLine("------------------------------------------------");
                }
            }

            return null;
        }


    }

    class Program
    {

        static void Main(string[] args)
        {

            // wake up every 5 minutes
            // {
            //      - is a download in progress ? if yes: go back to sleep
            //      - Download feed from Tilos
            //      - Check if there's a missing file
            //      - If no: go back to sleep
            //      - if yes: go download it, when finished, move it to the right place and tag it
            //}

            ///  1. downloader - handles downloads, file naming, moving, tagging
            ///  2. Feed man - handles feed processing
            ///  3. FileSystemManager

            // Read the current configuration
            var configContent = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<TilosDownloaderConfig>(configContent);

            // Create a FeedProcessor
            var fileSystemManager = new FileSystemManager(config);
            var feedProcessor = new TilosFeedProcessor(config, fileSystemManager);
            var downloadEngine = new DownloadEngine();

            while(true)
            {
                if (downloadEngine.IsBusy)
                {
                    // Log current status (what is downloading, etc...)
                    Thread.Sleep(1000);
                }
                else
                {
                    var pendingItem = feedProcessor.GetNewItem();
                    if (pendingItem != null)
                    {

                    }
                    Thread.Sleep(1000);
                }
            }

        }


    }
}
