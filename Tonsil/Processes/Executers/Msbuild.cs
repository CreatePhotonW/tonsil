using System;
using System.Collections.Generic;
using System.Text;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public class Msbuild : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\microsoft.net\framework64\v4.0.30319\msbuild.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.SMB, FilePathType.Local};
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.MsbuildProject };
    }
}
