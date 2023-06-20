using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLTools.Options;

namespace PDDLTools.Classifiers
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(Constants.PDDLLanguageName)]
    class PDDLClassifierProvider : IClassifierProvider
    {
        [Import]
        IClassificationTypeRegistryService TypeRegistry = null;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (!OptionsManager.Instance.EnableSyntaxHighlighting)
                return null;

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new PDDLClassifier(textBuffer, TypeRegistry));
        }
    }
}
