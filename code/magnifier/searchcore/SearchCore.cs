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

            _crawler_mod = new ContentDiscoveryModule();
            _parser_mod = new ContentProcessModule();
            _index_mod = new IndexModule(Path.Combine(_root_path, "index"), 1);
            _query_mod = new QueryModule();
        }

        public void Kill()
        { }

        public IndexModule TestGetIndexModule()
        {
            return _index_mod;
        }

        public ContentDiscoveryModule TestGetCrawlerModule()
        {
            return _crawler_mod;
        }

        public void TestWriteDisk()
        {
            _index_mod.WriteToDisk();
        }

        private string _root_path;
        private ContentDiscoveryModule _crawler_mod;
        private ContentProcessModule _parser_mod;
        private IndexModule _index_mod;
        private QueryModule _query_mod;
    }
}
