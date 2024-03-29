﻿using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using PDDLTools.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Completers
{
    // https://learn.microsoft.com/en-us/visualstudio/extensibility/walkthrough-displaying-statement-completion?view=vs-2022&tabs=csharp
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(Constants.PDDLLanguageName)]
    [Name("PDDL Completion")]
    internal class PDDLCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            if (OptionsManager.Instance == null)
                return null;
            if (!OptionsManager.Instance.EnableAutoCompleteOfStatements)
                return null;

            return new PDDLCompletionSource(this, textBuffer);
        }
    }
}
