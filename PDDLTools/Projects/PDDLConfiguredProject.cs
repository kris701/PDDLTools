using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;

namespace PDDLTools.Projects
{
    [Export]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    internal class PDDLConfiguredProject
    {
        [Import]
        internal ConfiguredProject ConfiguredProject { get; private set; }

        [Import]
        internal ProjectProperties Properties { get; private set; }
    }
}
