using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    namespace Crawler
    {
        public class ContentDiscoveryModule : ISearchTopologyNode
        {
            public ContentDiscoveryModule(SearchTopology topology)
            {
                _topology = topology;
                _top_jobid = 0;
            }

            public ISearchTopologyNode.NodeType GetNodeType()
            {
                return ISearchTopologyNode.NodeType.Crawler;
            }
                
            public void StartLocalFileCrawl(IEnumerable<string> start_addrs)
            {
                var job = new CrawlJob(++_top_jobid);
                job.JobFinishedCallback = (state) => {
                    _jobs.Remove(state.ID);
                };

                foreach(var addr in start_addrs)
                {
                    job.AddTask(new LocalFileCrawlTask(addr, _topology));
                }

                job.Start();
            }

            private Dictionary<uint, CrawlJob> _jobs = new Dictionary<uint, CrawlJob>();
            private uint _top_jobid;
            private SearchTopology _topology;
        }
    }
}
