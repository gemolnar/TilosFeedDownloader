using System.Net;

namespace TilosFeedDownloader.ConsoleApp
{
    //    /Downloaded
    //        /Tilos (Szoveges) - Csonka
    //            file1.mp3
    //            file2.mp3
    //        /Tilos (Szoveges) - Kolorlokal
    //            file1.mp3
    //            file2.mp3
    //        /Tilos (Zenes) - Hotel North Pole
    //            file1.mp3
    //            file2.mp3

    /// <summary>
    /// Handles downloading and tagging MP3 files from a given location.
    /// </summary>
    class DownloadEngine
    {
        private WebClient _webClient;

        public bool IsBusy { get; internal set; }

        public DownloadEngine()
        {
            _webClient = new WebClient();
            _webClient.DownloadFileCompleted += OnDownloadFileCompleted;
            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Console.WriteLine(e.ProgressPercentage);

        }

        private void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //var tag = TagLib.File.Create((string)e.UserState);
            //tag.Tag.AlbumArtists = new string[] { "Artist1" }; // "Tilos Radio (Zenes)"
            //tag.Tag.Album = ""; // "Csonkamagyarorszag"
            //tag.Tag.Title = "Title"; // 2017. adas.
            //tag.Tag.Comment = "Hello bello"; // long desc
            //tag.Save();
            //Console.WriteLine(e.Error);
        }
    }
}
