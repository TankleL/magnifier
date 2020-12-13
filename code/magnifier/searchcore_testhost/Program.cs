using System;
using System.Collections.Generic;
using System.IO;
using SearchCore;
using SearchCore.Index;

namespace searchcore_testhost
{
    class Program
    {
        static void Main(string[] args)
        {
            string search_root = Path.GetFullPath(@"..\..\..\prepspace");
            SearchCore.SearchCore core = new SearchCore.SearchCore();
            core.Launch(search_root);

            test_crawl(core);
            test_index(core);
        }

        static void test_crawl(SearchCore.SearchCore core)
        {
            var crawler = core.Topology.GetNode_LoadBalanced<SearchCore.Crawler.ContentDiscoveryModule>(ISearchTopologyNode.NodeType.Crawler);
            crawler.StartLocalFileCrawl(new string[] { Path.GetFullPath(@"..\..\..\prepspace2") });

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        static void test_index(SearchCore.SearchCore core)
        {
            var index = core.Topology.GetNode_LoadBalanced<SearchCore.Index.IndexModule>(ISearchTopologyNode.NodeType.Index);
            core.TestWriteDisk();

            IList<IndexDocPosition> res = new List<IndexDocPosition>();
            index.Lookup(ref res, "findme");

            res.Clear();
            index.Lookup(ref res, "b2");
        }
    }
}
