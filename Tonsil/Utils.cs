using System;
using System.Collections.Generic;
using System.Reflection;

using System.Linq;
using Tonsil.Processes;

namespace Tonsil
{
    public static class Utils
    {
        public static IEnumerable<Processes.Downloaders.Downloader> GetValidDownloaders(Files.File source, Files.File destination)
        {
            List<Processes.Downloaders.Downloader> waysToDownload = new List<Processes.Downloaders.Downloader>();
            Type parentType = typeof(Processes.Downloaders.Downloader);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
            foreach (var subclass in subclasses)
            {
                Processes.Downloaders.Downloader downloader = (Processes.Downloaders.Downloader)Activator.CreateInstance(subclass);
                try
                {
                    Files.File conformedSource = downloader.ConformSource(source);
                    Files.File conformedDestination = downloader.ConformDestination(conformedSource, destination);
                    downloader.AddDownload(conformedSource, conformedDestination);
                    waysToDownload.Add(downloader);
                }
                catch (CannotConformException e)
                {
                    continue;
                }
            }
            return waysToDownload;
        }
        public static IEnumerable<Processes.Executers.Executer> GetValidExecuters(Files.File source)
        {
            List<Processes.Executers.Executer> waysToExecute = new List<Processes.Executers.Executer>();
            Type parentType = typeof(Processes.Executers.Executer);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
            foreach (var subclass in subclasses)
            {
                Processes.Executers.Executer executer = (Processes.Executers.Executer)Activator.CreateInstance(subclass);
                Files.File conformedSource = (Files.File)source.Clone();
                if (executer.IsValidSource(conformedSource))
                {
                    executer.AddFileExecution(conformedSource);
                    waysToExecute.Add(executer);
                }
            }
            return waysToExecute;
        }
        public static IEnumerable<Processes.ExecutionProxys.ExecutionProxy> GetValidExecutionProxys(Process process)
        {
            List<Processes.ExecutionProxys.ExecutionProxy> waysToExecute = new List<Processes.ExecutionProxys.ExecutionProxy>();
            Type parentType = typeof(Processes.ExecutionProxys.ExecutionProxy);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(parentType));
            foreach (var subclass in subclasses)
            {
                Processes.Process conformedProcess = (Process)process.Clone();
                Processes.ExecutionProxys.ExecutionProxy proxy = (Processes.ExecutionProxys.ExecutionProxy)Activator.CreateInstance(subclass);
                proxy.AddCmdlineExecution(conformedProcess);
                waysToExecute.Add(proxy);
            }
            return waysToExecute;
        }
        public static IEnumerable<IEnumerable<Processes.Process>> GetAllWaysToExecuteFile(Files.File source)
        {
            List<IEnumerable<Processes.Process>> allWaysToExecute = new List<IEnumerable<Processes.Process>>();

            Files.File destination = new Files.File()
            {
                FileType = source.FileType,
                FilePath = new Files.LocalFilePath()
                {
                    Directory = @"c:\users\public\",
                    Filename = source.FilePath.Filename
                }
            };

            // Executers without ExecutionProxys
            var executers = GetValidExecuters(source);
            foreach (var executer in executers)
            {
                IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)executer.Clone()};
                allWaysToExecute.Add(processList);
            }
            //

            // StackOverFlowException because of ParentProcess.Clone() so not cloning it for now
            // Executers with N=1 ExecutionProxys
            // How can this be generalized for int N >= 0 ?
            ///*
            foreach (var executer in executers)
            {
                foreach (var proxy in GetValidExecutionProxys(executer))
                {
                    IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)proxy.Clone() };
                    allWaysToExecute.Add(processList);
                }
            }
            //*/
            //


            // remote files only
            bool isRemoteSource = typeof(Files.RemoteFilePath).IsInstanceOfType(source.FilePath);
            if (isRemoteSource)
            {
                // Downloader + Executer
                var downloaders = GetValidDownloaders(source, destination);
                foreach (var downloader in downloaders)
                {
                    foreach (var executer in GetValidExecuters(downloader.Destination))
                    {
                        IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)downloader.Clone(), (Process)executer.Clone()};
                        allWaysToExecute.Add(processList);
                    }
                }
                //

                // How can these be generalized for int N >= 0, M >= 0 ?

                // Downloader with N=0 ExecutionProxys + Executer with M=1 ExecutionProxys
                foreach (var downloader in downloaders)
                {
                    foreach (var executer in GetValidExecuters(downloader.Destination))
                    {
                        foreach (var proxy in GetValidExecutionProxys(executer))
                        {
                            IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)downloader.Clone(), (Process)proxy.Clone() };
                            allWaysToExecute.Add(processList);
                        }
                    }
                }
                //

                // Downloader with N=1 ExecutionProxys + Executer with M=0 ExecutionProxys
                foreach (var downloader in downloaders)
                {
                    foreach (var proxy in GetValidExecutionProxys(downloader))
                    {
                        foreach (var executer in GetValidExecuters(downloader.Destination))
                        {
                            IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)proxy.Clone(), (Process)executer.Clone() };
                            allWaysToExecute.Add(processList);
                        }
                    }
                }
                //

                // Downloader with N=1 ExecutionProxys + Executer with M=1 ExecutionProxys
                foreach (var downloader in downloaders)
                {
                    foreach (var downloaderProxy in GetValidExecutionProxys(downloader))
                    {
                        foreach (var executer in GetValidExecuters(downloader.Destination))
                        {
                            foreach (var executerProxy in GetValidExecutionProxys(executer))
                            {
                                IEnumerable<Processes.Process> processList = new List<Processes.Process>() { (Process)downloaderProxy.Clone(), (Process)executerProxy.Clone() };
                                allWaysToExecute.Add(processList);
                            }
                        }
                    }
                }
                //
            }

            return allWaysToExecute;
        }
    }
}
