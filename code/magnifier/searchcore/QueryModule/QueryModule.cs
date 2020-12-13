using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    namespace Query
    {
        public class QueryModule : ISearchTopologyNode
        {
            public ISearchTopologyNode.NodeType GetNodeType()
            {
                return ISearchTopologyNode.NodeType.Query;
            }
        }
    }
}
