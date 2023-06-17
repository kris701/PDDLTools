using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Classification;
using Newtonsoft.Json;
using PDDLTools.PDDLInfo.PDDLDefinitionElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.QuickInfo
{
    public static class PDDLQuickInfoData
    {
        public static Dictionary<string, ContainerElement> QuickInfoContent = new Dictionary<string, ContainerElement>();

        public static void InitializeInfo()
        {
            if (PDDLInfo.PDDLInfo.PDDLDefinition == null)
                PDDLInfo.PDDLInfo.InitializeInfo();

            foreach (var element in PDDLInfo.PDDLInfo.PDDLDefinition.Elements)
                QuickInfoContent.Add(element.Key, GenerateElement(element));

        }

        private static ContainerElement GenerateElement(InfoElement element)
        {
            List<ClassifiedTextRun> textElements = new List<ClassifiedTextRun>();
            foreach (var block in element.Blocks)
                textElements.Add(new ClassifiedTextRun(block.Type, block.Text + Environment.NewLine, ClassifiedTextRunStyle.UseClassificationFont));
            
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
