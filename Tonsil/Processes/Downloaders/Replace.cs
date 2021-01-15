using System;
using System.Collections.Generic;

using System.Linq;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public class Replace : Downloader
    {
        public override string ImagePath { get; } = @"c:\windows\system32\replace.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.SMB, FilePathType.Local };
        public override List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>() { FilePathType.Local };
        public override string[] ArgumentsTemplate { get; } = new string[] { "{0}", "{0}", "/A"};
        public override int SourceArgumentIndex { get; } = 0;
        public override int DestinationArgumentIndex { get; } = 1;
        public override CmdLine CmdLine
        {
            get
            {
                string[] args = (string[]) ArgumentsTemplate.Clone();
                args[SourceArgumentIndex] = string.Format(args[SourceArgumentIndex], Source.FilePath.Path);
                args[DestinationArgumentIndex] = string.Format(args[DestinationArgumentIndex], Destination.FilePath.Directory.Remove(Destination.FilePath.Directory.Length - 1));
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains(" "))
                    {
                        args[i] = string.Format(@"""{0}""", args[i]);
                    }
                }
                return new CmdLine
                {
                    image = ImagePath,
                    arguments = args
                };
            }
        }
        public override bool IsValidDestination(File destination)
        {
            throw new NotImplementedException();
        }
        public override bool IsValidDestination(File source, File destination)
        {
            // Replace.exe uses the source filename as the filename for the destination
            return ValidDestinationPathTypes.Contains(destination.FilePath.FilePathType) && (source.FilePath.Filename == destination.FilePath.Filename);
        }
        public override File ConformDestination(File destination)
        {
            throw new NotImplementedException();
        }
        public override File ConformDestination(File source, File destination)
        {
            destination = (File)destination.Clone();
            if (IsValidDestination(source, destination))
            {
                return destination;
            }
            else
            {
                destination.FilePath.Filename = source.FilePath.Filename;
                if (!IsValidDestination(source, destination))
                {
                    throw new CannotConformException();
                }
                return destination;
            }
        }
        public override File GetValidDestination(File source, File destination = null)
        {
            throw new NotImplementedException();
        }
    }
}
