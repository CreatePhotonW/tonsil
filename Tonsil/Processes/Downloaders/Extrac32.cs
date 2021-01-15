using System.Collections.Generic;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public class Extrac32 : Downloader
    {
        public override string ImagePath { get; } = @"c:\windows\system32\extrac32.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.SMB };
        public override List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>() { FilePathType.Local};
        public override string[] ArgumentsTemplate { get; } = new string[] { "/Y", "/C", "{0}", "{0}" };
        public override int SourceArgumentIndex { get; } = 2;
        public override int DestinationArgumentIndex { get; } = 3;
    }
}
