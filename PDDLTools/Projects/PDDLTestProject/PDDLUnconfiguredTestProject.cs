namespace PDDLTools.Projects.PDDLTestProject
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.VS;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    [Export]
    [AppliesTo(PDDLUnconfiguredTestProject.UniqueCapability)]
    [ProjectTypeRegistration(
        PDDLTools.Constants.PDDLTestProjectTypeID, 
        PDDLTools.Constants.PDDLLanguageName,
        PDDLTools.Constants.PDDLTestProjectExt, 
        PDDLTools.Constants.PDDLTestProjectExt, 
        PDDLTools.Constants.PDDLLanguageName, 
        resourcePackageGuid: PDDLTools.Constants.PackageGuidString, 
        PossibleProjectExtensions = PDDLTools.Constants.PDDLTestProjectExt, 
        ProjectTemplatesDir = @"PDDLTestProject")]
    [ProvideProjectItem(
        PDDLTools.Constants.PDDLTestProjectTypeID, 
        "My Items",
        @"PDDLTestProject", 
        500)]
    public class PDDLUnconfiguredTestProject
    {
        internal const string UniqueCapability = "PDDLTestSample";

        [ImportingConstructor]
        public PDDLUnconfiguredTestProject(UnconfiguredProject unconfiguredProject)
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
        internal ActiveConfiguredProject<PDDLConfiguredTestProject> MyActiveConfiguredProject { get; private set; }

        [ImportMany(ExportContractNames.VsTypes.IVsProject, typeof(IVsProject))]
        internal OrderPrecedenceImportCollection<IVsHierarchy> ProjectHierarchies { get; private set; }

        internal IVsHierarchy ProjectHierarchy
        {
            get { return this.ProjectHierarchies.Single().Value; }
        }
    }
}
