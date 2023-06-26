namespace PDDLTools.Projects
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.ProjectSystem;

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
