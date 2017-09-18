using Microsoft.VisualStudio.TestTools.UnitTesting;
using TilosFeedDownloader.ConsoleApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilosFeedDownloader.BusinessLogic;

namespace TilosFeedDownloader.ConsoleApp.Tests
{
    [TestClass()]
    public class FileSystemManagerTests
    {
        [TestMethod()]
        public void SanitizeTest()
        {
            var fsm = new FileSystemManager(new TilosDownloaderConfig());
            var fn = fsm.Sanitize("  \\ Árviztűrő Tükörfúrógép  ?");
            Assert.IsTrue(fn == "arvizturo-tukorfurogep");
        }
    }
}