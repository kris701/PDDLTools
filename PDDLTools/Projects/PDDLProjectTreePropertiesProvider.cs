using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.ProjectSystem;
using PDDLTools.Projects.Images;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Projects
{
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    [Order(1000)]
    internal class PDDLProjectTreePropertiesProvider : IProjectTreePropertiesProvider
    {
        public void CalculatePropertyValues(
            IProjectTreeCustomizablePropertyContext propertyContext,
            IProjectTreeCustomizablePropertyValues propertyValues)
        {
            if (propertyValues.Flags.Contains(ProjectTreeFlags.Common.ProjectRoot))
            {
                propertyValues.Icon = PDDLProjectIconMoinkers.ProjectIconImageMoniker.ToProjectSystemType();
            }
        }
    }
}
