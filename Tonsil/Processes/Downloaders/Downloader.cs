using System;
using System.Collections.Generic;

using System.Linq;

using Tonsil.Files;

namespace Tonsil.Processes.Downloaders
{
    public abstract class Downloader : LoLBin
    {
        public virtual List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>();
        public virtual List<FilePathType> ValidDestinationPathTypes { get; } = new List<FilePathType>();
        public virtual string[] ArgumentsTemplate { get; } = new string[] { "{0}", "{0}" };
        public virtual int SourceArgumentIndex { get; } = 0;
        public virtual int DestinationArgumentIndex { get; } = 1;
        public override CmdLine CmdLine
        {
            get
            {
                string[] args = (string[]) ArgumentsTemplate.Clone();
                args[SourceArgumentIndex] = string.Format(args[SourceArgumentIndex], Source.FilePath.Path);
                args[DestinationArgumentIndex] = string.Format(args[DestinationArgumentIndex], Destination.FilePath.Path);
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
        public virtual File Source
        {
            get => FilesRead.FirstOrDefault();
            set
            {
                FilesRead.Clear();
                FilesRead.Add(value);
            }
        }

        public virtual File Destination
        {
            get => FilesWritten.FirstOrDefault();
            set
            {
                FilesWritten.Clear();
                FilesWritten.Add(value);
            }
        }

        public virtual bool IsValidSource(File source)
        {
            return ValidSourcePathTypes.Contains(source.FilePath.FilePathType);
        }
        public virtual bool IsValidDestination(File destination)
        {
            return ValidDestinationPathTypes.Contains(destination.FilePath.FilePathType);
        }
        public virtual bool IsValidDestination(File source, File destination)
        {
            return IsValidDestination(destination);
        }
        public virtual File ConformSource(File source)
        {
            source = (File)source.Clone();
            if (IsValidSource(source))
            {
                return source;
            }
            else
            {
                throw new CannotConformException();
            }
        }
        public virtual File ConformDestination(File destination)
        {
            destination = (File)destination.Clone();
            if (IsValidDestination(destination))
            {
                return destination;
            }
            else
            {
                throw new CannotConformException();
            }
        }
        public virtual File ConformDestination(File source, File destination)
        {
            return ConformDestination(destination);
        }
        public virtual File GetValidDestination(File source, File destination = null)
        {
            throw new NotImplementedException();
        }
        public virtual void AddDownload(File source, File destination)
        {
            if (!IsValidSource(source))
            {
                throw new InvalidSourceException();
            }
            if (!IsValidDestination(source, destination))
            {
                throw new InvalidDestinationException();
            }
            FilesRead.Clear();
            FilesRead.Add(source);
            FilesWritten.Clear();
            FilesWritten.Add(destination);
        }
    }

}
