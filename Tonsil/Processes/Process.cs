using System;
using System.Collections.Generic;

using System.Linq;

using Tonsil.Files;

namespace Tonsil.Processes
{
    public struct CmdLine
    {
        public string image;
        public string[] arguments;
        public override string ToString()
        {
            string output = "";
            if (image != null)
            {
                if (image.Contains(" "))
                {
                    output += string.Format(@"""{0}""", image);
                }
                else
                {
                    output += image;
                }
                if (arguments != null & arguments.Length > 0)
                {
                    if (image != "")
                        output += " ";
                    output += string.Join(" ", arguments);
                }
            }
            return output;
        }
    }

    public enum Architecture
    {
        i686,
        amd64
    }

    public class Process : ICloneable
    {
        public Process() { }
        public Process(CmdLine cmdLine) => CmdLine = cmdLine;
        private List<string> PATH = new List<string>(){ @"c:\windows\system32"};
        //public Architecture architecture;
        public bool IsInPath
        {
            get
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(ImagePath);
                System.IO.DirectoryInfo di = fi.Directory;
                return PATH.Contains(di.FullName);
            }
        }
        public virtual string ImagePath { get; } = "";
        public virtual CmdLine CmdLine { get; }
        public Process ParentProcess { get; set; }
        public List<Process> ChildProcesses { get; set; } = new List<Process>();
        // Processes that are not in the process tree of this process but have a connection to this process in some shape or form
        public List<Process> RelatedProcesses { get; set; } = new List<Process>();
        public List<File> FilesRead { get; set; } = new List<File>();
        public List<File> FilesWritten { get; set; } = new List<File>();
        public object Clone()
        {
            Process clone = (Process)MemberwiseClone();
            /*
            if (clone.ParentProcess != null)
            {
                clone.ParentProcess = (Process)ParentProcess.Clone();
            }
            */
            clone.ChildProcesses = ChildProcesses.Select(item => (Process)item.Clone()).ToList();
            clone.RelatedProcesses = RelatedProcesses.Select(item => (Process)item.Clone()).ToList();
            clone.FilesRead = FilesRead.Select(item => (File)item.Clone()).ToList();
            clone.FilesWritten = FilesWritten.Select(item => (File)item.Clone()).ToList();
            return clone;
        }
    }
    public class LoLBin : Process
    {
        public bool RequiresUserInteraction { get; } = false;
    }
    public class InvalidSourceException : Exception { }
    public class InvalidDestinationException : Exception { }
    public class CannotConformException : Exception { }
}
