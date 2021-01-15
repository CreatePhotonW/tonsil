using System.Collections.Generic;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public class Certutil : Downloader
    {
        public override string ImagePath { get; } = @"c:\windows\system32\certutil.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.HTTP};
        public override List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>() { FilePathType.Local};
        public override string[] ArgumentsTemplate { get; } = new string[] { "-urlcache", "-split", "-f", "{0}", "{0}" };
        public override int SourceArgumentIndex { get; } = 3;
        public override int DestinationArgumentIndex { get; } = 4;
    }
}
