using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace SearchCore
{
    public class IndexDocPosition : EqualityComparer<IndexDocPosition>
    {
        public UInt32 DocID;
        public UInt32 WordBegPosition;
        public UInt32 WordEndPosition;

        public override bool Equals([AllowNull] IndexDocPosition x, [AllowNull] IndexDocPosition y)
        {
            return (x.DocID == y.DocID) &&
                (x.WordBegPosition == y.WordBegPosition) &&
                (x.WordEndPosition == y.WordEndPosition);
        }

        public override int GetHashCode([DisallowNull] IndexDocPosition obj)
        {
            return (obj.DocID ^ obj.WordBegPosition ^ obj.WordEndPosition).GetHashCode();
        }
    }

    public class IndexRecord
    {
        public string Keyword
        {
            get
            {
                return _keyword;
            }
        }

        public Dictionary<UInt32, SortedList<UInt32, IndexDocPosition>> Positions
        {
            get
            {
                return _positions;
            }
        }

        public IndexRecord(string keyword)
        {
            _keyword = keyword;
            _positions = new Dictionary<uint, SortedList<UInt32, IndexDocPosition>>();
        }

        public void InsertDocPosition(IndexDocPosition docpos)
        {
            if(!HasPosition(docpos))
            {
                SortedList<UInt32, IndexDocPosition> positions;
                if (_positions.TryGetValue(docpos.DocID, out positions))
                {
                    positions.Add(docpos.WordBegPosition, docpos);
                }
                else
                {
                    positions = new SortedList<uint, IndexDocPosition>();
                    positions.Add(docpos.WordBegPosition, docpos);
                    _positions.Add(docpos.DocID, positions);
                }
            }
        }

        public bool HasPosition(IndexDocPosition docpos)
        {
            SortedList<UInt32, IndexDocPosition> positions;
            if(_positions.TryGetValue(docpos.DocID, out positions))
            {
                IndexDocPosition pos;
                if(positions.TryGetValue(docpos.WordBegPosition, out pos))
                {
                    return pos.Equals(docpos);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private string _keyword;
        private Dictionary<UInt32, SortedList<UInt32, IndexDocPosition>> _positions;
    }

    public class IndexTable
    {
        public UInt32 Partition
        {
            get
            {
                return _partition;
            }
        }

        public bool IsDirty
        {
            get
            {
                return _dirty;
            }
        }

        public int RecordCount
        {
            get
            {
                return _records.Count;
            }
        }

        public Dictionary<string, IndexRecord> Records
        {
            get
            {
                return _records;
            }
        }

        public IndexTable(UInt32 partition)
        {
            _partition = partition;
        }

        public bool LoadFile(string filename)
        {
            bool retval = true;
            _filename = filename;

            return retval;
        }

        public void WriteFile(string filename)
        {
            IndexTableSerializer.WriteToFile(filename, this);
        }

        public void InsertRecord(string keyword, IndexDocPosition docpos)
        {
            IndexRecord record;
            if(_records.TryGetValue(keyword, out record))
            {
                record.InsertDocPosition(docpos);
                _dirty = true;
            }
            else
            {
                record = new IndexRecord(keyword);
                record.InsertDocPosition(docpos);
                _records.Add(keyword, record);
                _dirty = true;
            }
        }

        private UInt32 _partition;
        private string _filename;
        private bool _dirty = false;
        private Dictionary<string, IndexRecord> _records = new Dictionary<string, IndexRecord>();
    }

    static class IndexTableSerializer
    {
        public static void WriteToFile(string filename, IndexTable table)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(KeyValueString("Partition", Convert.ToString(table.Partition)));
                sw.WriteLine(KeyValueString("RecordCount", Convert.ToString(table.RecordCount)));

                foreach(var record in table.Records)
                {
                    sw.WriteLine("[Record]");
                    sw.WriteLine(KeyValueString("Keyword", record.Key));
                    sw.WriteLine(IndexRecordToString(record.Value));
                }
            }
        }

        static string KeyValueString(string key, string value)
        {
            return key + "=" + value;
        }

        static string IndexRecordToString(IndexRecord record)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(KeyValueString("PosSetCount", Convert.ToString(record.Positions.Count)));
            foreach(var posset in record.Positions)
            {
                sb.AppendLine("[PositionSet]");
                sb.AppendLine(IndexDocPosSetToString(posset.Value));
            }

            return sb.ToString();
        }

        static string IndexDocPosSetToString(SortedList<UInt32, IndexDocPosition> posset)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(KeyValueString("PosCount", Convert.ToString(posset.Count)));
            foreach(var pos in posset)
            {
                sb.AppendLine("[Position]");
                sb.AppendLine(KeyValueString("DocID", Convert.ToString(pos.Value.DocID)));
                sb.AppendLine(KeyValueString("WordBeg", Convert.ToString(pos.Value.WordBegPosition)));
                sb.AppendLine(KeyValueString("WordEnd", Convert.ToString(pos.Value.WordEndPosition)));
            }

            return sb.ToString();
        }

    }
}
