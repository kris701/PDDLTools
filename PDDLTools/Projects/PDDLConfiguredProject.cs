namespace PDDLTools.Projects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.Shell;
    using PDDLParser.Helpers;
    using PDDLTools.Commands;
    using PDDLTools.FileMonitors;
    using PDDLTools.Helpers;

    [Export]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    public class PDDLConfiguredProject
    {
        public delegate void ProjectLoadedHandler();
        public event ProjectLoadedHandler ProjectLoaded;

        internal ConfiguredProject ConfiguredProject { get; private set; }

        internal ProjectProperties Properties { get; private set; }

        [ImportingConstructor]
        internal PDDLConfiguredProject(ConfiguredProject configuredProject, ProjectProperties properties)
        {
            ConfiguredProject = configuredProject;
            Properties = properties;
            PDDLProjectManager.PDDLProjects.Add(new FileInfo(configuredProject.UnconfiguredProject.FullPath).Name.ToUpper(), this);
            LastRefresh = DateTime.Now;

            ConfiguredProject.ProjectUnloading += DoUnloadAsync;
            ProjectLoaded += LoadAsync;
            ProjectLoaded.Invoke();
        }

        private async Task DoUnloadAsync(object sender, EventArgs e)
        {
            var key = new FileInfo(ConfiguredProject.UnconfiguredProject.FullPath).Name.ToUpper();
            if (PDDLProjectManager.PDDLProjects.ContainsKey(key))
                PDDLProjectManager.PDDLProjects.Remove(key);
            if (ProjectFileMonitorService.Instance != null)
                ProjectFileMonitorService.Instance.Uninitialise(new FileInfo(ConfiguredProject.UnconfiguredProject.FullPath).Directory.FullName);
        }

        private async void LoadAsync()
        {
            await SelectDomainCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await GetSelectedDomainAsync(), IntPtr.Zero));
            await SelectProblemCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await GetSelectedProblemAsync(), IntPtr.Zero));
            await SelectEngineCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await GetSelectedEngineAsync(), IntPtr.Zero));
            if (ProjectFileMonitorService.Instance != null)
                await ProjectFileMonitorService.Instance.InitialiseAsync(new FileInfo(ConfiguredProject.UnconfiguredProject.FullPath).Directory.FullName);
        }

        public DateTime LastRefresh { get; internal set; }

        public async Task<string> GetProjectPathAsync()
        {
            var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
            var dir = new FileInfo(await generalProps.FullPath.GetValueAsync() as string).Directory.FullName;
            return dir;
        }

        private bool _refreshSelectedDomain = true;
        private string _selectedDomain = "";
        public async Task<string> GetSelectedDomainAsync()
        {
            if (_refreshSelectedDomain)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                var domain = await generalProps.SelectedDomain.GetValueAsync();
                if (domain is string str)
                {
                    if (PDDLHelper.IsFileDomain(str))
                        _selectedDomain = str;
                    else
                        _selectedDomain = "";
                }
                else
                    _selectedDomain = "";
                _refreshSelectedDomain = false;
            }

            return _selectedDomain;
        }
        public async Task SetSelectedDomainAsync(string value)
        {
            if (value != _selectedDomain)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                await generalProps.SelectedDomain.SetValueAsync(value);
                _refreshSelectedDomain = true;
                LastRefresh = DateTime.Now;
            }
        }

        private bool _refreshSelectedProblem = true;
        private string _selectedProblem = "";
        public async Task<string> GetSelectedProblemAsync()
        {
            if (_refreshSelectedProblem)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                var problem = await generalProps.SelectedProblem.GetValueAsync();
                if (problem is string str)
                {
                    if (PDDLHelper.IsFileProblem(str))
                        _selectedProblem = str;
                    else
                        _selectedProblem = "";
                }
                else
                    _selectedProblem = "";
                _refreshSelectedDomain = false;
            }
            return _selectedProblem;
        }
        public async Task SetSelectedProblemAsync(string value)
        {
            if (value != _selectedProblem)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                await generalProps.SelectedProblem.SetValueAsync(value);
                _refreshSelectedProblem = true;
                LastRefresh = DateTime.Now;
            }
        }

        private bool _refreshSelectedEngine = true;
        private string _selectedEngine = "";
        public async Task<string> GetSelectedEngineAsync()
        {
            if (_refreshSelectedEngine)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                var engine = await generalProps.SelectedEngine.GetValueAsync();
                if (engine is string str)
                    _selectedEngine = str;
                else
                    _selectedEngine = "";
                _refreshSelectedEngine = false;
            }

            return _selectedEngine;
        }
        public async Task SetSelectedEngineAsync(string value)
        {
            if (value != _selectedEngine)
            {
                var generalProps = await Properties.GetConfigurationGeneralPropertiesAsync();
                await generalProps.SelectedEngine.SetValueAsync(value);
                _refreshSelectedEngine = true;
                LastRefresh = DateTime.Now;
            }
        }
    }
}
