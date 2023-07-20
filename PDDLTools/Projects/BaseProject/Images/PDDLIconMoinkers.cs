using Microsoft.VisualStudio.Imaging.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Projects.BaseProject.Images
{
    public static class PDDLIconMoinkers
    {
        private static readonly Guid ManifestGuid = new Guid("BC795B8C-3B33-4FBD-AFBA-6817D9A28F01");

        private const int ProjectIcon = 0;

        public static ImageMoniker ProjectIconImageMoniker
        {
            get
            {
                return new ImageMoniker { Guid = ManifestGuid, Id = ProjectIcon };
            }
        }
    }
}
