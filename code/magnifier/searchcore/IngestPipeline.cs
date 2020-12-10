using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    public static class IngestPipeline
    {
        public static Crawler.ContentDiscoveryModule CrawlerModule;
        public static Parser.ContentProcessModule ParserModule;
        public static Index.IndexModule IndexModule;
        public static Query.QueryModule QueryModule;

        public static UInt32 TopDocID = 0; 
    }
}
