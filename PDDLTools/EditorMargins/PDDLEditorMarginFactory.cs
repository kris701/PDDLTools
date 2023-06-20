using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using PDDLTools.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace PDDLTools.EditorMargins
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(PDDLEditorMargin.MarginName)]
    [Order(After = PredefinedMarginNames.HorizontalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType(Constants.PDDLLanguageName)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class PDDLEditorMarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            if (!OptionsManager.Instance.EnableEditorMargin)
                return null;

            return new PDDLEditorMargin(wpfTextViewHost.TextView);
        }
    }
}
