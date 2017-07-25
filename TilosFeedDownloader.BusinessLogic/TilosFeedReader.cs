using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TilosFeedDownloader.BusinessLogic
{
    public class ShowFeed
    {
        public TilosShow Show { get; set; }
        public IEnumerable<SyndicationItem> FeedItems { get; set; }
        public string Title { get; internal set; }
        public DateTimeOffset LastUpdatedTime { get; internal set; }
    }

    public class TilosFeedReader
    {
        public TilosDownloaderConfig Config { get; private set; }
        public IEnumerable<ShowFeed> ShowFeeds { get { return _shows; } }
        private List<ShowFeed> _shows = new List<ShowFeed>();

        public TilosFeedReader(TilosDownloaderConfig config)
        {
            Config = config;
        }
        public void ReadFeeds()
        {
            foreach (var show in Config.Shows)
            {
                var formatter = new Atom10FeedFormatter();
                using (XmlReader reader = XmlReader.Create(show.FeedUrl))
                {
                    formatter.ReadFrom(reader);
                }
                var showFeed = new ShowFeed();
                showFeed.Show = show;
                showFeed.FeedItems = formatter.Feed.Items;
                showFeed.Title = formatter.Feed.Title?.Text;
                showFeed.LastUpdatedTime = formatter.Feed.LastUpdatedTime;
                _shows.Add(showFeed);
            }
        }

    }
}
