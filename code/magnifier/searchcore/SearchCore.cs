using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    public class SearchCore
    {
        public void Launch(string root_path)
        {
            _root_path = root_path;

            IngestPipeline.CrawlerModule = new Crawler.ContentDiscoveryModule();
            IngestPipeline.ParserModule = new Parser.ContentProcessModule();
            IngestPipeline.IndexModule = new Index.IndexModule(Path.Combine(_root_path, "index"), 2);
            IngestPipeline.QueryModule = new Query.QueryModule();
        }

        public void Kill()
        { }

        public Index.IndexModule TestGetIndexModule()
        {
            return IngestPipeline.IndexModule;
        }

        public Crawler.ContentDiscoveryModule TestGetCrawlerModule()
        {
            return IngestPipeline.CrawlerModule;
        }

        public void TestWriteDisk()
        {
            IngestPipeline.IndexModule.WriteToDisk();
        }

        private string _root_path;
    }
}
