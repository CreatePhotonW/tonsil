using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public class Mshta : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\system32\mshta.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.HTTP, FilePathType.SMB, FilePathType.Local, FilePathType.ADS };
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.Hta };
    }
}
