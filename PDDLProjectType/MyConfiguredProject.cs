namespace PDDL
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using PDDL;
    using VSLangProj;
    using Task = System.Threading.Tasks.Task;

    [Export]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    internal class MyConfiguredProject
    {
        [Import]
        internal ConfiguredProject ConfiguredProject { get; private set; }

        [Import]
        internal ProjectProperties Properties { get; private set; }
    }
}
