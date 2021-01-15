using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using McMaster.Extensions.CommandLineUtils;

namespace TonsilConsole
{
    class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option("-u | --Uri", Description = "The Uri of file to execute. e.g. http://1.2.3.4/someDir/someFile.vbs , \\\\5.6.7.8\\someShare\\someDir\\someFile.vbs")]
        [Required]
        public string UriString { get; }
        
        [Option("-f | --FileType", Description = "The FileType of the file to execute.")]
        public string FileTypeName { get; }

        public Tonsil.Files.FileType FileType => (Tonsil.Files.FileType) Enum.Parse(typeof(Tonsil.Files.FileType), FileTypeName);

        public Uri FileUri
        {
            get
            {
                return new Uri(UriString);
            }
        }

        public void OnExecute()
        {
            var fi = new FileInfo(FileUri.LocalPath);
            var fileName = fi.Name;
            var fileExt = fi.Extension.ToLower();

            var fileTypeMap = new Dictionary<string, Tonsil.Files.FileType>
            {
                { ".vbs", Tonsil.Files.FileType.VBScript },
                { ".js", Tonsil.Files.FileType.JavaScript },
                { ".elf", Tonsil.Files.FileType.Elf }
            };


            Tonsil.Files.FileType fileType;

            try
            {
                fileType = FileType;
            }
            catch (System.ArgumentNullException e)
            {
                if (!fileTypeMap.ContainsKey(fileExt))
                {
                    throw new Exception("Unsupported File type");
                }
                fileType = fileTypeMap[fileExt];
            }
            catch (System.ArgumentException e)
            {
                throw new Exception("Invalid FileType");
            }

            Tonsil.Files.FilePath filePath = null;

            if (FileUri.Scheme.StartsWith("http"))
            {

                filePath = new Tonsil.Files.HttpFilePath()
                {
                    Host = FileUri.Host,
                    Port = FileUri.Port,
                    ssl = FileUri.Scheme.EndsWith("s"),
                    Directory = string.Join("", FileUri.LocalPath.Split(FileUri.Segments[FileUri.Segments.Length - 1])[0]),
                    Filename = fileName
                };
            }
            else if (FileUri.IsUnc)
            {
                var shareName = FileUri.Segments[1].Split("/")[0];
                var directory = FileUri.LocalPath.Split(shareName)[1].Split(fileName)[0];
                filePath = new Tonsil.Files.SmbFilePath()
                {
                    Host = FileUri.Host,
                    Port = 445,
                    ShareName = shareName,
                    Directory = directory,
                    Filename = fileName
                };
            }
            else
            {
                throw new Exception();
            }
            
            Tonsil.Files.File source = new Tonsil.Files.File()
            {
                FileType = fileType,
                FilePath = filePath
            };

            var processLists = Tonsil.Utils.GetAllWaysToExecuteFile(source);
            foreach (var processList in processLists)
            {
                foreach (var process in processList)
                {
                    Console.WriteLine(process.CmdLine.ToString());
                }
                Console.WriteLine("");
            }
            Console.WriteLine("{0} total results", processLists.Count());
        }
    }
}
