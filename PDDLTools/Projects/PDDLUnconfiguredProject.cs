namespace PDDLTools.Projects
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.VS;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    [Export]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    [ProjectTypeRegistration(PDDLTools.Constants.PDDLProjectTypeID, PDDLTools.Constants.PDDLLanguageName, "#2", PDDLTools.Constants.PDDLProjectExt, PDDLTools.Constants.PDDLLanguageName, resourcePackageGuid: PDDLTools.Constants.PackageGuidString, PossibleProjectExtensions = PDDLTools.Constants.PDDLProjectExt, ProjectTemplatesDir = @"..\..\Templates\Projects\MyCustomProject")]
    [ProvideProjectItem(PDDLTools.Constants.PDDLProjectTypeID, "My Items", @"..\..\Templates\ProjectItems\MyCustomProject", 500)]
    public class PDDLUnconfiguredProject
    {
        internal const string UniqueCapability = "PDDLSample";

        [ImportingConstructor]
        public PDDLUnconfiguredProject(UnconfiguredProject unconfiguredProject)
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
        internal ActiveConfiguredProject<PDDLConfiguredProject> MyActiveConfiguredProject { get; private set; }

        [ImportMany(ExportContractNames.VsTypes.IVsProject, typeof(IVsProject))]
        internal OrderPrecedenceImportCollection<IVsHierarchy> ProjectHierarchies { get; private set; }

        internal IVsHierarchy ProjectHierarchy
        {
            get { return this.ProjectHierarchies.Single().Value; }
        }
    }
}
