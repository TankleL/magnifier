using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SearchCore
{
    namespace Parser
    {
        public class ContentProcessModule : ISearchTopologyNode
        {
            public ContentProcessModule(SearchTopology topology)
            {
                _topology = topology;
            }

            public ISearchTopologyNode.NodeType GetNodeType()
            {
                return ISearchTopologyNode.NodeType.Parser;
            }

            public void ParsePlaintextFile(string path, UInt32 docid)
            {
                if(File.Exists(path))
                {
                    string alltext = File.ReadAllText(path);

                    bool selecting = false;
                    int tbeg = 0;
                    int tend = 0;

                    for(int i = 0; i < alltext.Length; ++i)
                    {
                        if(IsValidCharacter(alltext[i]))
                        {
                            if(!selecting)
                            {
                                selecting = true;
                                tbeg = i;
                            }

                            tend = i + 1;
                            continue;
                        }
                        else
                        {
                            if(selecting)
                            {
                                selecting = false;

                                _topology.GetNode_LoadBalanced<Index.IndexModule>(ISearchTopologyNode.NodeType.Index)
                                    .Insert(
                                        alltext.Substring(tbeg, tend - tbeg),
                                        new Index.IndexDocPosition() {
                                            DocID = docid,
                                            WordBegPosition = (uint)tbeg,
                                            WordEndPosition = (uint)tend
                                });
                            }
                        }
                    }
                }
            }

            private bool IsValidCharacter(char ch)
            {
                return !_INVALID_CHARS.Contains(ch);
            }

            private static readonly SortedSet<char> _INVALID_CHARS = new SortedSet<char>() {
                ' ', '\r', '\n', '\t', ',', '.', '(', ')', '{', '}', '+', '-', '=',
                '`', '!', '@', '#', '$', '%', '^', '&', '*', '<', '>', '?', '/', ':', '\'',
                '\"', ';'
            };

            private SearchTopology _topology;
        }
    }
}
