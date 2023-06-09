using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools
{
#pragma warning disable 649
    public class ContentTypeDefinition
    {
        [Export]
        [Name(Constants.PDDLLanguageName)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteTextMateBraceCompletionTypeName)]
        [BaseDefinition(CodeRemoteContentDefinition.CodeRemoteTextMateStructureTypeName)]
        internal static Microsoft.VisualStudio.Utilities.ContentTypeDefinition PDDLContentTypeDefinition;

        [Export]
        [FileExtension(Constants.PDDLExt)]
        [ContentType(Constants.PDDLLanguageName)]
        internal static FileExtensionToContentTypeDefinition PDDLFileExtensionDefinition;
    }
#pragma warning restore 649
}
