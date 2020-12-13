using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchCore.Crawler
{
    public class CrawlJobWorker
    {
        public delegate void JobWorkerStoppedCallbackDelegate(CrawlJobWorker state);
        public JobWorkerStoppedCallbackDelegate JobWorkerStoppedCallback;

        public CrawlJobWorker()
        {
            _abort = false;
            _workthrd = null;
            _taskque = new Queue<CrawlTask>();
        }

        public void AddTask(CrawlTask task)
        {
            lock(_lk_taskque)
            {
                _taskque.Enqueue(task);
            }
        }

        public void Start()
        {
            if(null == _workthrd)
            {
                _workthrd = new Thread(new ThreadStart(WorkThreadProc));
                _workthrd.Start();
            }
        }
        
        public void Stop()
        {
            // request the worker to stop.
            _abort = true;

            // wait for it stop.
            _workthrd.Join();

            // reset states
            _abort = false;
            _workthrd = null;
            _taskque = new Queue<CrawlTask>();
        }

        private void WorkThreadProc()
        {
            const int MAX_TASK_SLOTS = 16;
            CrawlTask[] tasks = new CrawlTask[MAX_TASK_SLOTS];

            int taskcount;
            do
            {
                taskcount = _taskque.Count;

                // get tasks batch
                lock (_lk_taskque)
                {
                    for (int i = 0;
                        i < taskcount &&
                        i < MAX_TASK_SLOTS;
                        ++i)
                    {
                        tasks[i] = _taskque.Dequeue();
                    }
                }

                for(int i = 0;
                    i < taskcount &&
                    i < MAX_TASK_SLOTS &&
                    !_abort;
                    ++i)
                {
                    tasks[i].Execute(this, tasks[i]);
                }

            } while (taskcount > 0 && !_abort);
        }

        private Queue<CrawlTask> _taskque;

        private object _lk_taskque = new object();
        private Thread _workthrd;
        private bool _abort;

    }
}
