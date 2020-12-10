using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore.Parser
{
    public class ParseJob
    {
        public delegate void JobFinishedCallbackDelegate(ParseJob job);
        public JobFinishedCallbackDelegate JobFinishedCallback;

        public uint ID
        {
            get
            {
                return _id;
            }
        }

        public ParseJob(uint id)
        {
            _id = id;
            _worker = new ParseJobWorker();
        }

        public void Start()
        {
            _worker.Start();
        }

        public void Stop()
        {
            _worker.Stop();
        }

        public void AddTask(ParseTask task)
        {
            _worker.AddTask(task);
        }

        private ParseJobWorker _worker;
        private uint _id;
    }
}
