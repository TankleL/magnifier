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

            // TODO: make below initialization works to be configurable 
            _topology = new SearchTopology();
            _topology.AddNode(new Crawler.ContentDiscoveryModule(_topology));
            _topology.AddNode(new Parser.ContentProcessModule(_topology));
            _topology.AddNode(new Index.IndexModule(Path.Combine(_root_path, "index"), 1));
            _topology.AddNode(new Query.QueryModule());
            _topology.AddNode(new DocsManagement.DocsModule());
        }

        public SearchTopology Topology
        {
            get
            {
                return _topology;
            }
        }

        public void Kill()
        { }

        public void TestWriteDisk()
        {
            _topology.ForEachNode(ISearchTopologyNode.NodeType.Index, node => {
                (node as Index.IndexModule).WriteToDisk();
            });
        }

        private SearchTopology _topology;
        private string _root_path;
    }
}
