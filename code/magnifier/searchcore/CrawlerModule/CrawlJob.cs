using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore.Crawler
{
    public class CrawlJob
    {
        public delegate void JobFinishedCallbackDelegate(CrawlJob job);
        public JobFinishedCallbackDelegate JobFinishedCallback;

        public uint ID
        {
            get
            {
                return _id;
            }
        }

        public CrawlJob(uint id)
        {
            _id = id;
            _worker = new CrawlJobWorker();
        }

        public void Start()
        {
            _worker.Start();
        }

        public void Stop()
        {
            _worker.Stop();
        }

        public void AddTask(CrawlTask task)
        {
            _worker.AddTask(task);
        }

        private CrawlJobWorker _worker;
        private uint _id;
    }
}
