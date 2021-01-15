using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tonsil.Processes.ExecutionProxys
{
    // Might move this else where since bash.exe is useful it suppose piping to a file and executing multiple commands in one line
    public class Bash : ExecutionProxy
    {
        public override string ImagePath { get; } = @"c:\windows\system32\bash.exe";
        public override string[] ArgumentsTemplate { get; } = new string[] { "-c", "{0}" };
        public override int CmdlineArgumentIndex { get; } = 1;
        public override CmdLine CmdLine
        {
            get
            {
                CmdLine nextCmdline = ChildProcesses.FirstOrDefault().CmdLine;
                nextCmdline.image = nextCmdline.image.Replace("c:", "/mnt/c").Replace("\\", "/");
                for (int i = 0; i < nextCmdline.arguments.Length; i++)
                {
                    if (nextCmdline.arguments[i].Contains(@"\"))
                    {
                        nextCmdline.arguments[i] = string.Format("'{0}'", nextCmdline.arguments[i].Replace(@"\", @"\\"));
                    }
                }
                string[] args = (string[])ArgumentsTemplate.Clone();
                args[CmdlineArgumentIndex] = string.Format(args[CmdlineArgumentIndex], nextCmdline.ToString());
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains(" "))
                    {
                        args[i] = string.Format(@"""{0}""", args[i]);
                    }
                }
                return new CmdLine
                {
                    image = ImagePath,
                    arguments = args
                };
            }
        }
    }
}
