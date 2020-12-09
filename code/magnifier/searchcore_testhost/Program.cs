using System;
using System.IO;
using SearchCore;

namespace searchcore_testhost
{
    class Program
    {
        static void Main(string[] args)
        {
            string search_root = Path.GetFullPath(@"..\..\..\prepspace");
            SearchCore.SearchCore core = new SearchCore.SearchCore();
            core.Launch(search_root);

            test_index(core);
        }

        static void test_index(SearchCore.SearchCore core)
        {
            var index = core.TestGetIndexModule();

            {
                IndexDocPosition idp = new IndexDocPosition()
                { DocID = 1, WordBegPosition = 0, WordEndPosition = 2 }; // Hi
                index.Insert("hi", idp);
            }
            {
                IndexDocPosition idp = new IndexDocPosition()
                { DocID = 2, WordBegPosition = 6, WordEndPosition = 8 }; // Hi
                index.Insert("hi", idp);
            }

            core.TestWriteDisk();
        }
    }
}
