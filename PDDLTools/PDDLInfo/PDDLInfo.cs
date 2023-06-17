using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Adornments;
using Newtonsoft.Json;
using PDDLTools.PDDLInfo.PDDLDefinitionElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLTools.PDDLInfo
{
    public static class PDDLInfo
    {
        public static PDDLDefinition PDDLDefinition { get; internal set; }

        public static void InitializeInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PDDLTools.PDDLInfo.PDDLDefinition.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string defText = reader.ReadToEnd();
                PDDLDefinition = JsonConvert.DeserializeObject<PDDLDefinition>(defText);
            }

        }
    }
}
