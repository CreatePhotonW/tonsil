using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public class Wmic : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\system32\wbem\wmic.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local, FilePathType.HTTP, FilePathType.SMB };
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.Stylesheet };
        public override string[] ArgumentsTemplate { get; } = new string[] { "process", "LIST", @"/FORMAT:""{0}""" };
        public override int SourceArgumentIndex { get; } = 2;
    }
}
