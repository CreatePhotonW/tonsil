using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Tonsil.Files;

namespace Tonsil.Processes.Executers
{
    public class Wsl : Executer
    {
        public override string ImagePath { get; } = @"c:\windows\system32\wsl.exe";
        public override List<FilePathType> ValidSourcePathTypes { get; } = new List<FilePathType>() { FilePathType.Local};
        public override List<FileType> ValidSourceFileTypes { get; } = new List<FileType>() { FileType.Elf };
        public override CmdLine CmdLine
        {
            get
            {
                File source = FilesRead.FirstOrDefault();
                string[] args = (string[])ArgumentsTemplate.Clone();

                var sourceFilePath = source.FilePath.Path.Replace("c:", "/mnt/c").Replace("\\", "/");
                args[SourceArgumentIndex] = string.Format(args[SourceArgumentIndex], sourceFilePath);

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains(@"\"))
                    {
                        args[i] = string.Format("{0}", args[i].Replace(@"\", @"\\"));
                    }
                }
                
                return new CmdLine
                {
                    image = ImagePath,
                    arguments = args
                };
            }
        }
    }
}
