namespace PDDL
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.VS;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Task = System.Threading.Tasks.Task;

    [Export]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    [ProjectTypeRegistration(VsPackage.ProjectTypeGuid, "PDDL", "#2", ProjectExtension, Language, resourcePackageGuid: VsPackage.PackageGuid, PossibleProjectExtensions = ProjectExtension, ProjectTemplatesDir = @"..\..\Templates\Projects\MyCustomProject")]
    [ProvideProjectItem(VsPackage.ProjectTypeGuid, "My Items", @"..\..\Templates\ProjectItems\MyCustomProject", 500)]
    internal class MyUnconfiguredProject
    {
        internal const string ProjectExtension = "pddlproj";
        internal const string UniqueCapability = "PDDLSample";
        internal const string Language = "PDDL";

        [ImportingConstructor]
        public MyUnconfiguredProject(UnconfiguredProject unconfiguredProject)
        {
            this.ProjectHierarchies = new OrderPrecedenceImportCollection<IVsHierarchy>(projectCapabilityCheckProvider: unconfiguredProject);
        }

        [Import]
        internal UnconfiguredProject UnconfiguredProject { get; private set; }

        [Import]
        internal IActiveConfiguredProjectSubscriptionService SubscriptionService { get; private set; }

        [Import]
        internal IProjectThreadingService ProjectThreadingService { get; private set; }

        [Import]
        internal ActiveConfiguredProject<ConfiguredProject> ActiveConfiguredProject { get; private set; }

        [Import]
        internal ActiveConfiguredProject<MyConfiguredProject> MyActiveConfiguredProject { get; private set; }

        [ImportMany(ExportContractNames.VsTypes.IVsProject, typeof(IVsProject))]
        internal OrderPrecedenceImportCollection<IVsHierarchy> ProjectHierarchies { get; private set; }

        internal IVsHierarchy ProjectHierarchy
        {
            get { return this.ProjectHierarchies.Single().Value; }
        }
    }
}
