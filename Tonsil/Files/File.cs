using System;
using System.Collections.Generic;
using System.Text;

namespace Tonsil.Files
{
    public enum FileType
    {
        JavaScript,
        VBScript,
        Library,
        Exe,
        DotnetAssembly,
        Msi,
        Elf,
        Hta,
        MsbuildProject,
        PowershellScript,
        Stylesheet,
        DllExportRegSvr,
        DllExportCplApp,
        DllExportInstaller
    }

    public class File : ICloneable
    {
        public FilePath FilePath { get; set; }
        public FileType FileType { get; set; }
        public byte[] Content;
        public object Clone()
        {
            File clone = (File) MemberwiseClone();
            clone.FilePath = (FilePath) clone.FilePath.Clone();
            return clone;
        }
    }

    public enum FilePathType
    {
        Local,
        ADS,
        HTTP,
        SMB,
        //WebDav
    }

    public class FilePath : ICloneable
    {
        public virtual FilePathType FilePathType { get; }
        public virtual string Path { get { return Filename; } }
        public string Directory { get; set; }
        public string Filename { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class RemoteFilePath : FilePath
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class HttpFilePath : RemoteFilePath
    {
        public bool ssl { get; set; } = false;
        public override FilePathType FilePathType { get; } = FilePathType.HTTP;
        public override string Path
        {
            get
            {
                string path = "";
                path += ssl ? "https" : "http";
                path += "://";
                path += Host;
                if (Port != 80)
                {
                    path += ":" + Port.ToString();
                }
                path += Directory;
                path += Filename;
                return path;
            }
        }
    }
    public class SmbFilePath : RemoteFilePath
    {
        public string ShareName { get; set; }

        public override FilePathType FilePathType { get; } = FilePathType.SMB;
        public override string Path
        {
            get
            {
                string path = "";
                path += @"\\";
                path += Host;
                // Can you specify a port for SMB?
                path += @"\";
                path += ShareName;
                path += Directory;
                path += Filename;
                return path;
            }
        }
    }
    public class LocalFilePath : FilePath
    {
        public override FilePathType FilePathType { get; } = FilePathType.Local;
        public override string Path
        {
            get
            {
                string path = "";
                path += Directory;
                path += Filename;
                return path;
            }
        }
    }
    public class ADSFilePath : LocalFilePath
    {
        public override FilePathType FilePathType { get; } = FilePathType.ADS;
        public override string Path
        {
            get
            {
                string path = "";
                path += Directory;
                path += Filename;
                return path;
            }
        }
    }
}

