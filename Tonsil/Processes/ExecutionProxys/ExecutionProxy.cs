using System;
using System.Collections.Generic;

using System.Linq;

namespace Tonsil.Processes.ExecutionProxys
{
    public abstract class ExecutionProxy : LoLBin
    {
        //https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessa
        // e.g. lpApplicationName = "program.exe", lpCommandLine = "arg1 arg2 arg3" ->  DropsModuleFromCmdline true
        // e.g. lpApplicationName = "program.exe", lpCommandLine = "program.exe arg1 arg2 arg3" ->  DropsModuleFromCmdline false
        // The two ways to create a process above result in the same behavior for most processes but not all!
        public virtual bool DropsModuleFromCmdline { get; } = false;
        public virtual string[] ArgumentsTemplate { get; } = new string[] { "{0}" };
        public virtual int CmdlineArgumentIndex { get; } = 0;
        public override CmdLine CmdLine
        {
            get
            {
                CmdLine nextCmdline = ChildProcesses.FirstOrDefault().CmdLine;
                if (DropsModuleFromCmdline)
                {
                    string[] newArgs = new string[nextCmdline.arguments.Length + 1];
                    newArgs[0] = "A"; // This can be whatever, but by convention it is the application name what the process being created.
                    Array.Copy(nextCmdline.arguments, 0, newArgs, 1, nextCmdline.arguments.Length);
                    nextCmdline.arguments = newArgs;
                }
                string[] args = (string[]) ArgumentsTemplate.Clone();
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
        public virtual void AddCmdlineExecution(Process process)
        {
            ChildProcesses.Clear();
            ChildProcesses.Add(process);
            process.ParentProcess = this;
        }
    }
}
