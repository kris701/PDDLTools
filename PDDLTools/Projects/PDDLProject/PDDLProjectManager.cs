using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Projects.PDDLProject
{
    internal static class PDDLProjectManager
    {
        public static Dictionary<string, PDDLConfiguredProject> PDDLProjects = new Dictionary<string, PDDLConfiguredProject>();
        public static async Task<PDDLConfiguredProject> GetCurrentProjectAsync()
        {
            var proj = (await DTE2Helper.GetActiveProjectNameAsync()).ToUpper();
            if (PDDLProjects.ContainsKey(proj))
                return PDDLProjects[proj];
            return null;
        }
    }
}
