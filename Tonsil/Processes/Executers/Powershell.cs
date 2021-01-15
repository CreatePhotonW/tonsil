using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public class Powershell : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\system32\windowspowershell\v1.0\powershell.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local };
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.PowershellScript };
        public override string[] ArgumentsTemplate { get; } = new string[] { "-F", "{0}" };
        public override int SourceArgumentIndex { get; } = 1;
    }
}
