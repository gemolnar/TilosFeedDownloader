using System;
using System.IO;
using System.Net;

namespace TilosFeedDownloader.ConsoleApp
{
    // Music/ArtistName - AlbumName/TrackNumber - TrackName.ext
    



    /// <summary>
    /// Handles downloading and tagging MP3 files from a given location.
    /// </summary>
    class DownloadEngine
    {
        private WebClient _webClient;

        public bool IsBusy { get; internal set; }
        public FileSystemManager FileSystemManager { get; }
        public string CurrentStatus { get; private set; }

        public DownloadEngine(FileSystemManager fileSystemManager)
        {
            _webClient = new WebClient();
            _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            
            FileSystemManager = fileSystemManager ?? throw new ArgumentNullException(nameof(fileSystemManager));
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var show = (TilosShowItem)e.UserState;
            CurrentStatus = $"Downloading {show.Mp3Uri}: {e.ProgressPercentage}% ({e.BytesReceived / (1024 * 1024)} MB of {e.TotalBytesToReceive / (1024*1024)} MB)";
        }

        private void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                CurrentStatus = e.Error.Message;
            }
            else
            {
                var downloadeShowItem = (TilosShowItem)e.UserState;
                var targetPath = FileSystemManager.GenerateInProgressFullPath(downloadeShowItem);
                var tag = TagLib.File.Create(targetPath);
                string artist = $"Tilos {downloadeShowItem.Show.Type}";
                tag.Tag.AlbumArtists = new string[] { artist }; // "Tilos Music"
                tag.Tag.Artists = new string[] { artist };
                tag.Tag.Performers = new string[] { artist };
                tag.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(downloadeShowItem.Show.ImagePath) };
                tag.Tag.Year = (uint)downloadeShowItem.PublishDate.Year;
                tag.Tag.Album = downloadeShowItem.Show.Title; // "Csonkamagyarorszag"
                tag.Tag.Title = downloadeShowItem.Title; // 2017. adas.
                tag.Tag.Comment = downloadeShowItem.Summary; // long desc
                tag.Save();

                var finalPath = FileSystemManager.GenerateFinishedFullPath(downloadeShowItem);
                if (!Directory.Exists(Path.GetDirectoryName(finalPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath));

                File.Move(targetPath, finalPath);
            }
            IsBusy = false;
            //Console.WriteLine(e.Error);
        }

        internal void DownloadShowItem(TilosShowItem pendingItem)
        {
            IsBusy = true;
            var targetPath = FileSystemManager.GenerateInProgressFullPath(pendingItem);
            if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            _webClient.DownloadFileAsync(pendingItem.Mp3Uri, targetPath, pendingItem);
        }
    }
}
