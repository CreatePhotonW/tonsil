using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    class Control : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\system32\control.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.SMB, FilePathType.Local };
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.DllExportRegSvr };
        
        //public override string[] ArgumentsTemplate { get; } = new string[] { "{0}" };
        //public virtual int SourceArgumentIndex { get; } = 0;

    }
}