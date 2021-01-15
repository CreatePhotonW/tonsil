using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    class InstallUtil : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local };
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() {FileType.DllExportInstaller};
        public override string[] ArgumentsTemplate { get; } = new string[] { "/logtoconsole=false", "/logfile=", "/u" , "{0}" };
        public override int SourceArgumentIndex { get; } = 3;
    }
}