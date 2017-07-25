using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using TilosFeedDownloader.BusinessLogic;

namespace TilosFeedDownloader.ConsoleApp
{

  //    /Downloaded
  //        /Tilos(Szoveges) - Csonka
  //            file1.mp3
  //            file2.mp3
  //        /Tilos (Szoveges) - Kolorlokal
  //            file1.mp3
  //            file2.mp3
  //        /Tilos (Zenes) - Hotel North Pole
  //            file1.mp3
  //            file2.mp3

    public class Processor
    {
        private WebClient _webClient = new WebClient();
        public bool IsProcessing { get; set; }
        public TilosDownloaderConfig Config { get; set; }

        public Processor(TilosDownloaderConfig config)
        {
            Config = config;
            _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
        }

        private static string GenerateFilename(string showTitle)
        {
            string filename = showTitle;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, ' ');
            }
            filename = filename.Replace('.', ' ');
            filename = filename.Trim();
            while (filename.Contains("  ")) filename = filename.Replace("  ", " ");
            filename += ".mp3";
            return filename;
        }

        public void DoCheck()
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

                    // Generate folder name
                    var folderName = $"Tilos ({feed.Show.Type}) - {feed.Show.Title}";
                    var downloadFolder = Path.Combine(Config.DownloadingFolder, folderName);
                    if (!Directory.Exists(downloadFolder))
                    {
                        Directory.CreateDirectory(downloadFolder);
                    }

                    string fileName = GenerateFilename(item.Title.Text);


                    if (!_webClient.IsBusy)
                    {
                        string targetFileName = Path.Combine(downloadFolder, fileName);
                        _webClient.DownloadFileAsync(mp3Uri, targetFileName, targetFileName); // Here pass artist, album, title...
                    }
                    Console.WriteLine("------------------------------------------------");
                }
            }


        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage);

        }

        private void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var tag = TagLib.File.Create((string)e.UserState);
            tag.Tag.AlbumArtists = new string[] { "Artist1" }; // "Tilos Radio (Zenes)"
            tag.Tag.Album = ""; // "Csonkamagyarorszag"
            tag.Tag.Title = "Title"; // 2017. adas.
            tag.Tag.Comment = "Hello bello"; // long desc
            tag.Save();
            Console.WriteLine(e.Error);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var configContent = File.ReadAllText("config.json");
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<TilosDownloaderConfig>(configContent);

            var p = new Processor(config);
            p.DoCheck();



            Console.ReadLine();

        }


    }
}
