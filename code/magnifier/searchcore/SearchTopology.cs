using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore
{
    public interface ISearchTopologyNode
    {
        public enum NodeType
        {
            Unknown,
            Crawler,
            Parser,
            Index,
            Query,
            DocsMgnmt
        }

        public NodeType GetNodeType();
    }

    public class SearchTopology
    {
        public SearchTopology()
        {
            _nodes_map = new Dictionary<ISearchTopologyNode.NodeType, NodeList>();
        }

        public int AddNode(ISearchTopologyNode node)
        {
            int index = 0;
            NodeList nl;
            if(_nodes_map.TryGetValue(node.GetNodeType(), out nl))
            {
                index = nl.AddNode(node);
            }
            else
            {
                NodeList newnl = new NodeList();
                index = newnl.AddNode(node);
                _nodes_map.Add(node.GetNodeType(), newnl);
            }
            return index;
        }

        public ISearchTopologyNode GetNode_LoadBalanced(ISearchTopologyNode.NodeType node_type)
        {
            return _nodes_map[node_type].GetNode_LoadBalanced();
        }

        public STNodeT GetNode_LoadBalanced<STNodeT>(ISearchTopologyNode.NodeType node_type)
        {
            ISearchTopologyNode nd = GetNode_LoadBalanced(node_type);
            return (STNodeT)nd;
        }

        public void ForEachNode(ISearchTopologyNode.NodeType node_type, Action<ISearchTopologyNode> processor)
        {
            NodeList nl = _nodes_map[node_type];

            foreach(var node in nl.Nodes)
            {
                processor(node);
            }
        }

        private class NodeList
        {
            public NodeList()
            {
                _nodes = new List<ISearchTopologyNode>();
                _balanceIndex = 0;
            }

            public int AddNode(ISearchTopologyNode node)
            {
                int idx = _nodes.Count;
                _nodes.Add(node);
                return idx;
            }

            public ISearchTopologyNode GetNode_LoadBalanced() // with 
            {
                ISearchTopologyNode retnd = _nodes[_balanceIndex];
                _balanceIndex = (_balanceIndex + 1) % _nodes.Count;
                return retnd;
            }

            public List<ISearchTopologyNode> Nodes
            {
                get
                {
                    return _nodes;
                }
            }

            List<ISearchTopologyNode> _nodes;
            int _balanceIndex;
        }

        private Dictionary<ISearchTopologyNode.NodeType, NodeList> _nodes_map;
    }
}
