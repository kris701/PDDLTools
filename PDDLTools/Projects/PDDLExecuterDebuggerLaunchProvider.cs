namespace PDDLTools.Projects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.Debug;
    using Microsoft.VisualStudio.ProjectSystem.Properties;
    using Microsoft.VisualStudio.ProjectSystem.VS.Debug;
    using PDDLTools.Commands;
    using PDDLTools.Options;

    [ExportDebugger("PDDLExecuter")]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    public class PDDLExecuterDebuggerLaunchProvider : DebugLaunchProviderBase
    {
        [ImportingConstructor]
        public PDDLExecuterDebuggerLaunchProvider(ConfiguredProject configuredProject)
            : base(configuredProject)
        {
        }

        [ExportPropertyXamlRuleDefinition("PDDL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9be6e469bc4921f1", "XamlRuleToCode:PDDLExecuterDebugger.xaml", "Project")]
        [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
        private object DebuggerXaml { get { throw new NotImplementedException(); } }

        [Import]
        private ProjectProperties ProjectProperties { get; set; }

        public override async Task<bool> CanLaunchAsync(DebugLaunchOptions launchOptions)
        {
            return true;
        }

        public override async Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(DebugLaunchOptions launchOptions)
        {
            var settings = new DebugLaunchSettings(launchOptions);

            StringBuilder sb = new StringBuilder("-WindowStyle hidden {& ");
            sb.Append($"{OptionsManager.Instance.PythonPrefix} ");
            sb.Append($"'{Path.Combine(OptionsManager.Instance.FDPath, "fast-downward.py")}' ");
            sb.Append($"--plan-file '{Path.Combine(OptionsManager.Instance.FDPath, "sas_plan")}' ");
            sb.Append($"--sas-file '{Path.Combine(OptionsManager.Instance.FDPath, "output.sas")}' ");

            var engineArg = SelectEngineCommand.SelectedSearch.Replace("\"", "'");
            if (engineArg.ToLower().Contains("--alias"))
                sb.Append($"{engineArg} ");

            sb.Append($"'C:\\Users\\kris7\\source\\repos\\New Project5\\New Project5\\domain.pddl' ");
            sb.Append($"'C:\\Users\\kris7\\source\\repos\\New Project5\\New Project5\\problem.pddl' ");

            if (engineArg.ToLower().Contains("--search"))
                sb.Append($"{engineArg}");
            sb.Append("}");

            settings.Executable = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
            settings.Arguments = sb.ToString();

            settings.LaunchOperation = DebugLaunchOperation.CreateProcess;
            settings.LaunchOptions = DebugLaunchOptions.IntegratedConsole;
            settings.SendToOutputWindow = true;

            return new IDebugLaunchSettings[] { settings };
        }
    }
}
