﻿using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PDDLTools.Tagger
{
    // https://learn.microsoft.com/en-us/visualstudio/extensibility/walkthrough-highlighting-text?view=vs-2022&tabs=csharp
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/HighlightWordFormatDefinition")]
    [UserVisible(true)]
    internal class HighlightWordFormatDefinition : MarkerFormatDefinition
    {
        public HighlightWordFormatDefinition()
        {
            this.BackgroundColor = Colors.LightBlue;
            this.ForegroundColor = Colors.DarkBlue;
            this.DisplayName = "Highlight Word";
            this.ZOrder = 5;
        }
    }
}
