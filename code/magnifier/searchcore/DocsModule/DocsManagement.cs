using System;
using System.Collections.Generic;
using System.Text;

namespace SearchCore.DocsManagement
{
    public class DocsModule : ISearchTopologyNode
    {
        public ISearchTopologyNode.NodeType GetNodeType()
        {
            return ISearchTopologyNode.NodeType.DocsMgnmt;
        }

        public DocsModule()
        {
            _top_docid = 1;  // make sure 0 is an invalid doc id.
            _path2doc_maps = new Dictionary<string, uint>();
            _doc2path_maps = new Dictionary<uint, string>();
        }

        public bool HasDocument(string path)
        {
            return _path2doc_maps.ContainsKey(path);
        }

        public bool HasDocument(UInt32 docid)
        {
            return _doc2path_maps.ContainsKey(docid);
        }

        public UInt32 AddDocument(string path)
        {
            UInt32 docid;
            if(_path2doc_maps.TryGetValue(path, out docid))
            {
                return docid;
            }
            else
            {
                docid = ++_top_docid;
                _path2doc_maps.Add(path, docid);
                _doc2path_maps.Add(docid, path);
                return docid;
            }
        }

        public UInt32 GetDocumentID(string path)
        {
            UInt32 docid;
            if(_path2doc_maps.TryGetValue(path, out docid))
            {
                return docid;
            }
            else
            {
                return 0; // 0 is an invalid doc id
            }
        }

        public string GetDocumentPath(UInt32 docid)
        {
            string path = string.Empty;
            _doc2path_maps.TryGetValue(docid, out path);
            return path;
        }


        private Dictionary<string, UInt32> _path2doc_maps;
        private Dictionary<UInt32, string> _doc2path_maps;
        private UInt32 _top_docid;
    }
}
