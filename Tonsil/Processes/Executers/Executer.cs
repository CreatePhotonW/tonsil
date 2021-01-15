using System;
using System.Collections.Generic;

using System.Linq;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public abstract class Executer : LoLBin
    {
        public virtual List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>();
        public virtual List<FileType> ValidSourceFileTypes { get; } = new List<FileType>();
        public virtual string[] ArgumentsTemplate { get; } = new string[] { "{0}" };
        public virtual int SourceArgumentIndex { get; } = 0;
        public override CmdLine CmdLine
        {
            get
            {
                File source = FilesRead.FirstOrDefault();
                string[] args = (string[])ArgumentsTemplate.Clone();
                args[SourceArgumentIndex] = string.Format(args[SourceArgumentIndex], source.FilePath.Path);
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
        public virtual bool IsValidSource(File source)
        {
            return ValidSourcePathTypes.Contains(source.FilePath.FilePathType) && ValidSourceFileTypes.Contains(source.FileType);
        }
        public virtual void AddFileExecution(File source)
        {
            if (!IsValidSource(source))
            {
                throw new InvalidSourceException();
            }
            FilesRead.Clear();
            FilesRead.Add(source);
        }

        public virtual void AddExecution(Process process)
        {
            throw new NotImplementedException();
        }
    }
}
