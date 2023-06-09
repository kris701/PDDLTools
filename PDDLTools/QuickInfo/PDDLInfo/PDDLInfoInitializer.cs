using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Adornments;

namespace PDDLTools.QuickInfo.PDDLInfo
{
    public class PDDLInfoInitializer
    {
        public void InitializeQuickInfo()
        {
            PDDLInfo.QuickInfoContent.Add("predicates", GenerateElement("test info"));
        }

        private ContainerElement GenerateElement(string text)
        {
            var textElement = new ClassifiedTextElement(
                            new ClassifiedTextRun(PredefinedClassificationTypeNames.Type, text)
                        );
            return new ContainerElement(
                ContainerElementStyle.Stacked,
                textElement
            );
        }
    }
}
