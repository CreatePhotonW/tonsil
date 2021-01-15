using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tonsil.Processes.ExecutionProxys
{
    // Might move this else where since cmd.exe is useful it suppose piping to a file and executing multiple commands in one line
    public class Cmd : ExecutionProxy
    {
        public override string ImagePath { get; } = @"c:\windows\system32\cmd.exe";
        public override string[] ArgumentsTemplate { get; } = new string[] { "/c", "{0}" };
        public override int CmdlineArgumentIndex { get; } = 1;
    }
}
