using System.Collections.Generic;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public class Expand : Downloader
    {
        public override string ImagePath { get; } = @"c:\windows\system32\expand.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local, FilePathType.ADS, FilePathType.SMB };
        public override List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>() { FilePathType.Local, FilePathType.ADS, FilePathType.SMB };
        public override string[] ArgumentsTemplate { get; } = new string[] { "{0}", "{0}" };
        public override int SourceArgumentIndex { get; } = 0;
        public override int DestinationArgumentIndex { get; } = 1;
        public override bool IsValidSource(File source)
        {
            // expand.exe parses its arguments as all lowercase so source files on case-sensitive servers will be a issue
            bool isValid = base.IsValidSource(source);
            // sharenames, hosts are case-insensitve
            isValid &= source.FilePath.Directory == source.FilePath.Directory.ToLower();
            isValid &= source.FilePath.Filename == source.FilePath.Filename.ToLower();
            return isValid;
        }
        public override bool IsValidDestination(File destination)
        {
            bool isValid = base.IsValidDestination(destination);
            isValid &= destination.FilePath.Directory == destination.FilePath.Directory.ToLower();
            isValid &= destination.FilePath.Filename == destination.FilePath.Filename.ToLower();
            return isValid;
        }
        public override File ConformSource(File source)
        {
            source = (File)source.Clone();
            if (IsValidSource(source))
            {
                return source;
            }
            else
            {
                source.FilePath.Directory = source.FilePath.Directory.ToLower();
                source.FilePath.Filename = source.FilePath.Filename.ToLower();
                if (!IsValidSource(source))
                {
                    throw new CannotConformException();
                }
                return source;
            }
        }
        public override File ConformDestination(File destination)
        {
            destination = (File)destination.Clone();
            if (IsValidDestination(destination))
            {
                return destination;
            }
            else
            {
                destination.FilePath.Directory = destination.FilePath.Directory.ToLower();
                destination.FilePath.Filename = destination.FilePath.Filename.ToLower();
                if (!IsValidDestination(destination))
                {
                    throw new CannotConformException();
                }
                return destination;
            }
        }
    }
}