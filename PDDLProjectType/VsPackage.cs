namespace PDDL
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [Description("A custom project type based on CPS")]
    [Guid(VsPackage.PackageGuid)]
    public sealed class VsPackage : Package
    {
        public const string PackageGuid = "30a1c541-83f6-4c50-8f07-50e37521a9d6";
        public const string ProjectTypeGuid = "9c8d8400-dc74-48ed-9670-053185552ffd";
        public const string ProjectExtension = "pddlproj";
        internal const string DefaultNamespace = "PDDL";
    }
}
