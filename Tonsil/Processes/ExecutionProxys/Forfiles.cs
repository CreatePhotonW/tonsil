using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; 

namespace Tonsil.Processes.ExecutionProxys
{
    public class Forfiles : ExecutionProxy
    {
        private static string SearchPath { get; } = @"c:\windows\";
        private static string SearchMatch { get; } = @"notepad.exe";
        public override bool DropsModuleFromCmdline { get; } = true;
        public override string ImagePath { get; } = @"c:\windows\system32\forfiles.exe";
        public override string[] ArgumentsTemplate { get; } = new string[] { "/p", SearchPath, "/m", SearchMatch, "/c", "{0}" };
        public override int CmdlineArgumentIndex { get; } = 5;
    }
}
