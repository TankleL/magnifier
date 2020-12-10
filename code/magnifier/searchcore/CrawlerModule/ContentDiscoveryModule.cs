using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    namespace Crawler
    {
        public class ContentDiscoveryModule
        {
            public ContentDiscoveryModule()
            {
                _top_jobid = 0;
            }
                
            public void ConnectContentProcessModule(Parser.ContentProcessModule module)
            {
                _parser_mode = module;
            }

            public void StartLocalFileCrawl(IEnumerable<string> start_addrs)
            {
                var job = new CrawlJob(++_top_jobid);
                job.JobFinishedCallback = (state) => {
                    _jobs.Remove(state.ID);
                };

                foreach(var addr in start_addrs)
                {
                    job.AddTask(new LocalFileCrawlTask(addr));
                }

                job.Start();
            }

            private Dictionary<uint, CrawlJob> _jobs = new Dictionary<uint, CrawlJob>();
            private uint _top_jobid;
            private Parser.ContentProcessModule _parser_mode;
        }
    }
}
