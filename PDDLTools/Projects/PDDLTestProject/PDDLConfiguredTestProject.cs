namespace PDDLTools.Projects.PDDLTestProject
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
    using PDDLTools.Projects.BaseProject;

    [Export]
    [AppliesTo(PDDLUnconfiguredTestProject.UniqueCapability)]
    public class PDDLConfiguredTestProject
    {
        internal ConfiguredProject ConfiguredProject { get; private set; }

        internal ProjectProperties Properties { get; private set; }

        [ImportingConstructor]
        internal PDDLConfiguredTestProject(ConfiguredProject configuredProject, ProjectProperties properties)
        {
            ConfiguredProject = configuredProject;
            Properties = properties;
        }
    }
}
