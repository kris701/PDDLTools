using PDDLParser.Helpers;
using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.FileMonitors
{
    public class ProjectFileMonitorService
    {
        public static ProjectFileMonitorService Instance { get; internal set; }

        private Dictionary<string, HashSet<string>> _domainCache = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> _problemCache = new Dictionary<string, HashSet<string>>();
        public ProjectFileMonitorService()
        {
            Instance = this;
        }

        public async Task<HashSet<string>> GetDomainCacheAsync(string projectPath)
        {
            if (!_domainCache.ContainsKey(projectPath))
                await InitialiseAsync(projectPath);
            return _domainCache[projectPath];
        }

        public async Task<HashSet<string>> GetProblemCacheAsync(string projectPath)
        {
            if (!_problemCache.ContainsKey(projectPath))
                await InitialiseAsync(projectPath);
            return _problemCache[projectPath];
        }

        public async Task InitialiseAsync(string projectPath)
        {
            var statusBar = new StatusBarHelper();
            await statusBar.ShowTextAsync("Indexing project pddl files...");
            var allDocuments = await DTE2Helper.GetAllFilesInPDDLProjectsAsync(".pddl");

            _domainCache.Add(projectPath, new HashSet<string>());
            _problemCache.Add(projectPath, new HashSet<string>());

            foreach(var document in allDocuments)
            {
                if (PDDLHelper.IsFileDomain(document))
                    _domainCache[projectPath].Add(document);
                else if (PDDLHelper.IsFileProblem(document))
                    _problemCache[projectPath].Add(document);
            }

            await statusBar.ClearAsync();

            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = projectPath,
                Filter = ".pddl"
            };

            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (source is FileSystemWatcher watcher)
            {
                if (_domainCache.ContainsKey(watcher.Path))
                {
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        if (PDDLHelper.IsFileDomain(e.FullPath))
                            _domainCache[watcher.Path].Add(e.FullPath);
                        else if (PDDLHelper.IsFileProblem(e.FullPath))
                            _problemCache[watcher.Path].Add(e.FullPath);
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        if (PDDLHelper.IsFileDomain(e.FullPath))
                            _domainCache[watcher.Path].Add(e.FullPath);
                        else if (PDDLHelper.IsFileProblem(e.FullPath))
                            _problemCache[watcher.Path].Add(e.FullPath);
                    }
                }
            }   
        }
    }
}
