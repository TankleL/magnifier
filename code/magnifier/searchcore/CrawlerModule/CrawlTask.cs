using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SearchCore.Crawler
{
    public abstract class CrawlTask
    {
        public delegate bool ExecuteDelegate(CrawlJobWorker jobworker, CrawlTask state);
        public ExecuteDelegate Execute;
    }

    public sealed class LocalFileCrawlTask : CrawlTask
    {
        public LocalFileCrawlTask(string path)
        {
            _path = path;
            Execute = ExecuteLocalFileCrawl;
        }

        private bool ExecuteLocalFileCrawl(CrawlJobWorker worker, CrawlTask state)
        {
            try
            {
                if(File.Exists(_path))
                {
                    ProcessFile(_path);
                }
                else if(Directory.Exists(_path))
                {
                    DirectoryInfo di = new DirectoryInfo(_path);
                    foreach(var file in di.EnumerateFiles())
                    {
                        worker.AddTask(new LocalFileCrawlTask(file.FullName));
                    }

                    foreach(var dir in di.EnumerateDirectories())
                    {
                        worker.AddTask(new LocalFileCrawlTask(dir.FullName));
                    }
                }
            }
            catch
            {}

            return true;
        }

        private void ProcessFile(string path)
        {
            if(Path.GetExtension(path) == ".txt")
            {
                IngestPipeline.ParserModule.ParsePlaintextFile(path, ++IngestPipeline.TopDocID);
            }
        }

        private string _path;
    }
}
