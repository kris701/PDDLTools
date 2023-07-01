using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace PDDLTools.Tagger
{
    // https://learn.microsoft.com/en-us/visualstudio/extensibility/walkthrough-highlighting-text?view=vs-2022&tabs=csharp
    [Export(typeof(EditorFormatDefinition))]
    [Name("PDDLClassifier.Highlight")]
    [UserVisible(true)]
    internal class HighlightWordFormatDefinition : MarkerFormatDefinition
    {
        public HighlightWordFormatDefinition()
        {
            this.BackgroundColor = Color.FromArgb(255, 17, 61, 111);
            this.ForegroundColor = Colors.White;
            this.DisplayName = "Highlight Word";
            this.ZOrder = 5;
        }
    }
}
