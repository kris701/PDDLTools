using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Adornments;
using Newtonsoft.Json;
using PDDLTools.QuickInfo.PDDLInfo.PDDLDefinitionElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLTools.QuickInfo.PDDLInfo
{
    public static class PDDLInfo
    {
        public static Dictionary<string, ContainerElement> QuickInfoContent = new Dictionary<string, ContainerElement>();

        public static void InitializeInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PDDLTools.QuickInfo.PDDLInfo.PDDLDefinition.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string defText = reader.ReadToEnd();
                var deserialised = JsonConvert.DeserializeObject<PDDLDefinition>(defText);
                foreach (var element in deserialised.Elements)
                    QuickInfoContent.Add(element.Key, GenerateElement(element));
            }

        }

        private static ContainerElement GenerateElement(InfoElement element)
        {
            List<ClassifiedTextRun> textElements = new List<ClassifiedTextRun>();
            foreach(var block in element.Blocks)
            {
                textElements.Add(new ClassifiedTextRun(block.Type, block.Text + Environment.NewLine, ClassifiedTextRunStyle.Bold));
            }

            var textElement = new ClassifiedTextElement(
                            textElements.ToArray()
                        );
            return new ContainerElement(
                ContainerElementStyle.Stacked,
                textElement
            );
        }
    }
}
