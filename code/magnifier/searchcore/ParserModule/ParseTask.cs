using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SearchCore.Parser
{
    public abstract class ParseTask
    {
        public delegate bool ExecuteDelegate(ParseJobWorker jobworker, ParseTask state);
        public ExecuteDelegate Execute;
    }

}
