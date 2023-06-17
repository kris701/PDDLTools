using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PDDLTools.Classifiers
{
    public static class PDDLTypeDefinition
    {
        #region Declaration
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.Declaration)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        static ClassificationTypeDefinition PDDLClassifier_Declaration = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.Declaration)]
        [Name(PDDLTypes.Declaration)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_Declaration_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_Declaration_Declaration()
            {
                this.DisplayName = "PDDL Declaration";
                this.ForegroundColor = Color.FromArgb(255, 155, 155, 155);
            }
        }
        #endregion

        #region Type
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.Type)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        static ClassificationTypeDefinition PDDLClassifier_Type = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.Type)]
        [Name(PDDLTypes.Type)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_Type_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_Type_Declaration()
            {
                this.DisplayName = "PDDL Type";
                this.ForegroundColor = Color.FromArgb(255, 78, 201,176);
            }
        }
        #endregion

        #region Expression
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.Expression)]
        [BaseDefinition(PredefinedClassificationTypeNames.Keyword)]
        static ClassificationTypeDefinition PDDLClassifier_Expression = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.Expression)]
        [Name(PDDLTypes.Expression)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_Expression_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_Expression_Declaration()
            {
                this.DisplayName = "PDDL Expression";
                this.ForegroundColor = Color.FromArgb(255, 86, 156, 214);
            }
        }
        #endregion

        #region Parameter
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.Parameter)]
        [BaseDefinition(PredefinedClassificationTypeNames.Identifier)]
        static ClassificationTypeDefinition PDDLClassifier_Parameter = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.Parameter)]
        [Name(PDDLTypes.Parameter)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_Parameter_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_Parameter_Declaration()
            {
                this.DisplayName = "PDDL Parameter";
                this.ForegroundColor = Color.FromArgb(255, 214, 157, 133);
            }
        }
        #endregion

        #region Externals
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.Externals)]
        [BaseDefinition(PredefinedClassificationTypeNames.ExcludedCode)]
        static ClassificationTypeDefinition PDDLClassifier_Externals = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.Externals)]
        [Name(PDDLTypes.Externals)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_Externals_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_Externals_Declaration()
            {
                this.DisplayName = "PDDL Externals";
                this.ForegroundColor = Color.FromArgb(255, 94, 138, 117);
            }
        }
        #endregion

        #region MinorToken
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PDDLTypes.MinorToken)]
        [BaseDefinition(PredefinedClassificationTypeNames.Punctuation)]
        static ClassificationTypeDefinition PDDLClassifier_MinorToken = null;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = PDDLTypes.MinorToken)]
        [Name(PDDLTypes.MinorToken)]
        [Order(After = Priority.High)]
        [UserVisible(true)]
        sealed class PDDLClassifier_MinorToken_Declaration : ClassificationFormatDefinition
        {
            public PDDLClassifier_MinorToken_Declaration()
            {
                this.DisplayName = "PDDL Minor Tokens";
                this.ForegroundColor = Colors.Gray;
            }
        }
        #endregion
    }
}
