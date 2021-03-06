﻿using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace SearchCore
{
    namespace Index
    {
        public class IndexModule : ISearchTopologyNode
        {
            public ISearchTopologyNode.NodeType GetNodeType()
            {
                return ISearchTopologyNode.NodeType.Index;
            }

            public IndexModule(string root_index_path, UInt32 partition_count)
            {
                InitDirectory(root_index_path);
                InitIndexTables(partition_count);
            }

            public bool Lookup(ref IList<IndexDocPosition> resutls, string keyword)
            {
                var pt = GetPartition(keyword);
                return _tables[pt].LookupRecord(ref resutls, keyword);
            }

            public void Insert(string keyword, IndexDocPosition docpos)
            {
                var pt = GetPartition(keyword);
                _tables[pt].InsertRecord(keyword, docpos);
            }

            public void WriteToDisk()
            {
                foreach (var table in _tables)
                {
                    table.Value.WriteFile(Path.Combine(_root_index_path, string.Format("part_{0}.idxt", table.Key)));
                }
            }

            public bool LoadFromDisk()
            {
                bool retval = true;

                return retval;
            }

            private UInt32 GetPartition(string keyword)
            {
                return ((UInt32)(Math.Abs(keyword.GetHashCode()))) % _partition_count;
            }

            private void InitDirectory(string dirpath)
            {
                if (Directory.Exists(dirpath))
                {
                    _root_index_path = dirpath;
                }
                else
                {
                    throw new DirectoryNotFoundException(string.Format("path \"{0}\" not found", dirpath));
                }
            }

            private void InitIndexTables(UInt32 count)
            {
                _partition_count = count;
                for (UInt32 i = 0; i < count; ++i)
                {
                    var tbl = new IndexTable(i);
                    _tables.Add(i, tbl);
                }
            }

            private bool IsIndexTableFile(string filename)
            {
                return filename.EndsWith(".idxt");
            }

            private UInt32 _partition_count = 1;
            private string _root_index_path = string.Empty;
            private Dictionary<UInt32, IndexTable> _tables = new Dictionary<uint, IndexTable>();
        }
    }
}
