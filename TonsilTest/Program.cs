using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TonsilTest
{
    class Notepad : Tonsil.Processes.Process
    {
        public override string ImagePath { get; } = @"c:\windows\system32\notepad.exe"; //"c:\Program Files\WindowsApps\Microsoft.WindowsCalculator_10.1906.53.0_x64__8wekyb3d8bbwe\Calculator.exe";
        public override Tonsil.Processes.CmdLine CmdLine
        {
            get
            {
                return new Tonsil.Processes.CmdLine()
                {
                    image = ImagePath,
                    arguments = new string[] { }
                };
            }
        }
    }

    static class Test
    {
        private static System.Diagnostics.Process[] GetProcesses(Tonsil.Processes.Process targetProcess)
        {
            Tonsil.Processes.CmdLine cmdline = targetProcess.CmdLine;
            System.IO.FileInfo fi = new System.IO.FileInfo(cmdline.image);
            string processName = fi.Name;
            string friendlyName = System.IO.Path.GetFileNameWithoutExtension(processName);
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(friendlyName);
            return processes;
        }
        private static void KillProcesses(Tonsil.Processes.Process targetProcess)
        {
            System.Diagnostics.Process[] processes = GetProcesses(targetProcess);
            foreach (var proc in processes)
            {
                try
                {
                    proc.Kill();
                }
                catch
                {
                    Console.WriteLine("Failed to kill process.");
                }
            }
        }
        // Kill targetProcess; Executes processUnderTest; checks if targetProcess is descendent of processUnderTest; kill targetProcess
        public static bool TestExecutionProxy(Tonsil.Processes.Process processUnderTest, Tonsil.Processes.Process targetProcess)
        {
            KillProcesses(targetProcess);
            System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = processUnderTest.CmdLine.image,
                Arguments = string.Join(" ", processUnderTest.CmdLine.arguments),
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process rootProcess = System.Diagnostics.Process.Start(si);
            //System.Threading.Thread.Sleep(2 * 1000);
            rootProcess.WaitForExit(1000);
            var processes = GetProcesses(targetProcess);
            KillProcesses(targetProcess);
            return processes.Length > 0;
        }

        public static Dictionary<string, bool> TestExecutionProxys()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            Type parentType = typeof(Tonsil.Processes.ExecutionProxys.ExecutionProxy);
            Assembly assembly = parentType.Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

            foreach (Type type in subclasses)
            {
                Tonsil.Processes.Process targetProcess = new Notepad();
                Tonsil.Processes.ExecutionProxys.ExecutionProxy processUnderTest = (Tonsil.Processes.ExecutionProxys.ExecutionProxy)Activator.CreateInstance(type);
                processUnderTest.AddCmdlineExecution(targetProcess);
                bool result = TestExecutionProxy(processUnderTest, targetProcess);
                Console.WriteLine(string.Format("{1}: \t{0}", processUnderTest.CmdLine.ToString(), result ? "Pass" : "Fail"));
                results.Add(processUnderTest.CmdLine.ToString(), result);
            }
            return results;
        }
        public static bool TestDownloader(Tonsil.Processes.Process processUnderTest, Tonsil.Files.File sourceFile, Tonsil.Files.File destinationFile)
        {
            System.IO.File.Delete(destinationFile.FilePath.Path);
            System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = processUnderTest.CmdLine.image,
                Arguments = string.Join(" ", processUnderTest.CmdLine.arguments),
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process rootProcess = System.Diagnostics.Process.Start(si);

            System.Threading.Thread.Sleep(2 * 1000);
            rootProcess.WaitForExit(1000);

            bool result = System.IO.File.Exists(destinationFile.FilePath.Path);
            System.IO.File.Delete(destinationFile.FilePath.Path);
            return result;
        }
        public static Dictionary<string, bool> TestDownloaders()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            string serverHost = "10.141.41.1";

            Tonsil.Files.HttpFilePath httpFilePath = new Tonsil.Files.HttpFilePath()
            {
                Host = serverHost,
                Port = 80,
                Directory = "/somedir/",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.SmbFilePath smbFilePath = new Tonsil.Files.SmbFilePath()
            {
                Host = serverHost,
                Port = 445,
                ShareName = "someshare",
                Directory = @"\somedir\",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.LocalFilePath localFilePath = new Tonsil.Files.LocalFilePath()
            {
                Directory = @"c:\",
                Filename = "somefile.vbs"
            };

            List<Tonsil.Files.FilePath> filePaths = new List<Tonsil.Files.FilePath>() { httpFilePath, smbFilePath };

            Tonsil.Files.File destinationFile = new Tonsil.Files.File()
            {
                FileType = Tonsil.Files.FileType.VBScript,
                FilePath = localFilePath
            };

            Type parentType = typeof(Tonsil.Processes.Downloaders.Downloader);
            Assembly assembly = parentType.Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

            foreach (Type type in subclasses)
            {
                foreach (var filePath in filePaths)
                {
                    Tonsil.Files.File sourceFile = new Tonsil.Files.File()
                    {
                        FilePath = filePath
                    };
                    Tonsil.Processes.Downloaders.Downloader processUnderTest = (Tonsil.Processes.Downloaders.Downloader)Activator.CreateInstance(type);
                    if (processUnderTest.IsValidSource(sourceFile) && processUnderTest.IsValidDestination(sourceFile, destinationFile))
                    {
                        processUnderTest.AddDownload(sourceFile, destinationFile);
                        bool result = TestDownloader(processUnderTest, sourceFile, destinationFile);
                        Console.WriteLine(string.Format("{1}: \t{0}", processUnderTest.CmdLine.ToString(), result ? "Pass" : "Fail"));
                        results.Add(processUnderTest.CmdLine.ToString(), result);
                    }
                }
            }
            return results;
        }
        public static Dictionary<string, bool> TestExecuters()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            string serverHost = "10.141.41.1";

            System.IO.File.WriteAllText(@"c:\somefile.vbs", @"CreateObject(""WScript.Shell"").Run(""Notepad.exe"")");

            Tonsil.Files.HttpFilePath httpFilePath = new Tonsil.Files.HttpFilePath()
            {
                Host = serverHost,
                Port = 80,
                Directory = "/somedir/",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.SmbFilePath smbFilePath = new Tonsil.Files.SmbFilePath()
            {
                Host = serverHost,
                Port = 445,
                ShareName = "someshare",
                Directory = @"\somedir\",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.LocalFilePath localFilePath = new Tonsil.Files.LocalFilePath()
            {
                Directory = @"c:\",
                Filename = "somefile.vbs"
            };

            List<Tonsil.Files.FilePath> filePaths = new List<Tonsil.Files.FilePath>() { httpFilePath, smbFilePath, localFilePath };

            Type parentType = typeof(Tonsil.Processes.Executers.Executer);
            Assembly assembly = parentType.Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));

            foreach (Type type in subclasses)
            {
                foreach (var filePath in filePaths)
                {
                    Tonsil.Files.File sourceFile = new Tonsil.Files.File()
                    {
                        FilePath = filePath
                    };
                    var processUnderTest = (Tonsil.Processes.Executers.Executer)Activator.CreateInstance(type);
                    if (processUnderTest.IsValidSource(sourceFile))
                    {
                        Tonsil.Processes.Process targetProcess = new Notepad(); // The process that sourceFile should spawn
                        processUnderTest.AddFileExecution(sourceFile);
                        bool result = TestExecutionProxy(processUnderTest, targetProcess);
                        Console.WriteLine(string.Format("{1}: \t{0}", processUnderTest.CmdLine.ToString(), result ? "Pass" : "Fail"));
                        results.Add(processUnderTest.CmdLine.ToString(), result);
                    }
                }
            }
            return results;
        }
        public static Dictionary<string, bool> TestExecutionProxyDownloaders()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            string serverHost = "10.141.41.1";

            Tonsil.Files.HttpFilePath httpFilePath = new Tonsil.Files.HttpFilePath()
            {
                Host = serverHost,
                Port = 80,
                Directory = "/somedir/",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.SmbFilePath smbFilePath = new Tonsil.Files.SmbFilePath()
            {
                Host = serverHost,
                Port = 445,
                ShareName = "someshare",
                Directory = @"\somedir\",
                Filename = "somefile.vbs"
            };
            Tonsil.Files.LocalFilePath localFilePath = new Tonsil.Files.LocalFilePath()
            {
                Directory = @"c:\",
                Filename = "somefile.vbs"
            };

            List<Tonsil.Files.FilePath> filePaths = new List<Tonsil.Files.FilePath>() { httpFilePath, smbFilePath };

            Tonsil.Files.File destinationFile = new Tonsil.Files.File()
            {
                FileType = Tonsil.Files.FileType.VBScript,
                FilePath = localFilePath
            };

            Type downloaderParentType = typeof(Tonsil.Processes.Downloaders.Downloader);
            Assembly assembly = downloaderParentType.Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> downloaderSubclasses = types.Where(t => t.IsSubclassOf(downloaderParentType));
            Type executionProxyParentType = typeof(Tonsil.Processes.ExecutionProxys.ExecutionProxy);
            IEnumerable<Type> executionProxySubclasses = types.Where(t => t.IsSubclassOf(executionProxyParentType));

            foreach (Type executionProxyType in executionProxySubclasses)
            {
                foreach (Type downloaderType in downloaderSubclasses)
                {
                    foreach (var filePath in filePaths)
                    {
                        Tonsil.Files.File sourceFile = new Tonsil.Files.File()
                        {
                            FilePath = filePath
                        };
                        Tonsil.Processes.Downloaders.Downloader downloadProcessUnderTest = (Tonsil.Processes.Downloaders.Downloader)Activator.CreateInstance(downloaderType);
                        if (downloadProcessUnderTest.IsValidSource(sourceFile) && downloadProcessUnderTest.IsValidDestination(sourceFile, destinationFile))
                        {
                            downloadProcessUnderTest.AddDownload(sourceFile, destinationFile);

                            Tonsil.Processes.Process targetProcess = downloadProcessUnderTest;
                            Tonsil.Processes.ExecutionProxys.ExecutionProxy processUnderTest = (Tonsil.Processes.ExecutionProxys.ExecutionProxy)Activator.CreateInstance(executionProxyType);
                            processUnderTest.AddCmdlineExecution(targetProcess);
                            bool result = TestDownloader(processUnderTest, sourceFile, destinationFile);
                            Console.WriteLine(string.Format("{1}: \t{0}", processUnderTest.CmdLine.ToString(), result ? "Pass" : "Fail"));
                            results.Add(processUnderTest.CmdLine.ToString(), result);
                        }
                    }
                }
            }
            return results;
        }

        public static Dictionary<string, bool> RunTests()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (var result in TestExecutionProxys())
                results.Add(result.Key, result.Value);
            foreach (var result in TestDownloaders())
                results.Add(result.Key, result.Value);
            foreach (var result in TestExecutionProxyDownloaders())
                results.Add(result.Key, result.Value);
            foreach (var result in TestExecuters())
                results.Add(result.Key, result.Value);
            return results;
        }
    }

    class Program
    { 
        static void Main(string[] args)
        {
            Dictionary<string, bool> results = Test.RunTests();
            Console.WriteLine("\nResult summary:\n");
            foreach (var result in results)
            {
                Console.WriteLine(string.Format("\t{0}: \t{1}", result.Value ? "Pass" : "Fail", result.Key));
            }
        }
    }
}
