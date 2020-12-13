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
        public LocalFileCrawlTask(string path, SearchTopology st)
        {
            _path = path;
            _st = st;
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
                        worker.AddTask(new LocalFileCrawlTask(file.FullName, _st));
                    }

                    foreach(var dir in di.EnumerateDirectories())
                    {
                        worker.AddTask(new LocalFileCrawlTask(dir.FullName, _st));
                    }
                }
            }
            catch
            {
                throw;
            }

            return true;
        }

        private void ProcessFile(string path)
        {
            if(Path.GetExtension(path) == ".txt")
            {
                UInt32 docid = _st.GetNode_LoadBalanced<DocsManagement.DocsModule>(
                    ISearchTopologyNode.NodeType.DocsMgnmt)
                    .AddDocument(path);
                _st.GetNode_LoadBalanced<Parser.ContentProcessModule>(ISearchTopologyNode.NodeType.Parser)
                    .ParsePlaintextFile(path, docid);
            }
        }

        private string _path;
        private SearchTopology _st;
    }
}
