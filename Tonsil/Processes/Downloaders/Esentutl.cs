using System.Collections.Generic;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public class Esentutl : Downloader
    {
        public override string ImagePath { get; } = @"c:\windows\system32\esentutl.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local, FilePathType.ADS, FilePathType.SMB};
        public override List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>() { FilePathType.Local, FilePathType.ADS, FilePathType.SMB};
        public override string[] ArgumentsTemplate { get; } = new string[] { "/y","{0}", "/d", "{0}", "/o" };
        public override int SourceArgumentIndex { get; } = 1;
        public override int DestinationArgumentIndex { get; } = 3;
    }
}
