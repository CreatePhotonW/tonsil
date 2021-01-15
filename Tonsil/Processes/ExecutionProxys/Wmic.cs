using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tonsil.Processes.ExecutionProxys
{
    public class Wmic : ExecutionProxy
    {
        public override string ImagePath { get; } = @"c:\windows\system32\wbem\wmic.exe";
        public override string[] ArgumentsTemplate { get; } = new string[] { "process", "call", "create", "{0}" };
        public override int CmdlineArgumentIndex { get; } = 3;
    }
}
