﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
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

        private HashSet<string> _currentlyIndexing = new HashSet<string>();
        private Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
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
            if (!_domainCache.ContainsKey(projectPath))
                return new HashSet<string>();
            return _domainCache[projectPath];
        }

        public async Task<HashSet<string>> GetProblemCacheAsync(string projectPath)
        {
            if (!_problemCache.ContainsKey(projectPath))
                await InitialiseAsync(projectPath);
            if (!_problemCache.ContainsKey(projectPath))
                return new HashSet<string>();
            return _problemCache[projectPath];
        }

        public void Uninitialise(string projectPath)
        {
            if (_domainCache.ContainsKey(projectPath))
                _domainCache.Remove(projectPath);
            if (_problemCache.ContainsKey(projectPath))
                _problemCache.Remove(projectPath);
            if (_watchers.ContainsKey(projectPath))
                _watchers.Remove(projectPath);
        }

        public async Task InitialiseAsync(string projectPath)
        {
            if (_currentlyIndexing.Contains(projectPath))
                return;

            _currentlyIndexing.Add(projectPath);
            Uninitialise(projectPath);
            _domainCache.Add(projectPath, new HashSet<string>());
            _problemCache.Add(projectPath, new HashSet<string>());

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                var statusBar = new StatusBarHelper();
                await statusBar.ShowTextAsync("Indexing project pddl files...");

                await TaskScheduler.Default;
                // Find all pddl files in project dir
                var docs = new DirectoryInfo(projectPath).GetFiles("*.pddl", SearchOption.AllDirectories);

                // Categorise them
                int current = 0;
                foreach (var document in docs)
                {
                    if (current % 100 == 0)
                        await statusBar.ShowProgressAsync("Indexing project pddl files...", (uint)current, (uint)docs.Length);
                    current++;
                    if (PDDLHelper.IsFileDomain(document.FullName))
                        _domainCache[projectPath].Add(document.FullName);
                    else if (PDDLHelper.IsFileProblem(document.FullName))
                        _problemCache[projectPath].Add(document.FullName);
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                await statusBar.ClearProgressAsync();

                // Setup listener
                _watchers.Add(projectPath, new FileSystemWatcher()
                {
                    Path = projectPath,
                    Filter = "*.pddl",
                    IncludeSubdirectories = true,
                });

                _watchers[projectPath].Created += OnChanged;
                _watchers[projectPath].Deleted += OnChanged;
                _watchers[projectPath].EnableRaisingEvents = true;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _currentlyIndexing.Remove(projectPath);
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
