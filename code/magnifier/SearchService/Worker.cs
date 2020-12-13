using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using SearchCore;

namespace SearchService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

#if DEBUG
            string search_root = Path.GetFullPath(@".\prepspace");
#else
            // TODO: make this part configurable
            string search_root = Path.GetFullPath(@".");
#endif

            var core = new SearchCore.SearchCore();
            core.Launch(search_root);

#if DEBUG // crawl test data
            var crawler = core.Topology.GetNode_LoadBalanced<SearchCore.Crawler.ContentDiscoveryModule>(ISearchTopologyNode.NodeType.Crawler);
            crawler.StartLocalFileCrawl(new string[] { Path.GetFullPath(@"..") });
#endif
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/magnifier/search/query/");
            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);

                var ctx = await listener.GetContextAsync();
                await ProcessRequest(ctx);
            }
        }

        protected async Task ProcessRequest(HttpListenerContext context)
        {
        }
    }
}
