// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2009 by Roman Korchagin
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Notes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This object finds all <see cref="RunPr"/> objects in a document (including in all types of runs, styles, lists,
    /// defaults etc) and sends them to the template method so the derived class can handle them in a certain way.
    /// </summary>
    internal abstract class RunPrCollectorBase : DocumentVisitor
    {
        public override VisitorAction VisitDocumentEnd(Document doc)
        {
            CollectFromDocument(doc);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGlossaryDocumentEnd(GlossaryDocument glossary)
        {
            CollectFromDocument(glossary);
            return VisitorAction.Continue;
        }

        private void CollectFromDocument(DocumentBase doc)
        {
            CollectFromStyles(doc.Styles);
            CollectFromLists(doc.Lists);
        }

        private void CollectFromStyles(StyleCollection styles)
        {
            DoHandleRunPr(styles.DefaultRunPr);

            foreach (Style style in styles)
            {
                DoHandleRunPr(style.RunPr);

                switch (style.Type)
                {
                    case StyleType.Table:
                        CollectFromTableStyle((TableStyle)style);
                        break;
                    default:
                        // Do nothing. Already collected all runprs.
                        break;
                }
            }
        }

        private void CollectFromTableStyle(TableStyle style)
        {
            foreach (ConditionalStyle conditionalStyle in style.ConditionalStyles.DefinedStyles)
                DoHandleRunPr(conditionalStyle.RunPr);
        }

        private void CollectFromLists(ListCollection lists)
        {
            for (int i = 0; i < lists.ListDefCount; i++)
            {
                ListDef listDef = lists.GetListDefByIndex(i);
                foreach (ListLevel level in listDef.Levels)
                    DoHandleRunPr(level.RunPr);
            }

            foreach (List list in lists)
            {
                foreach (ListLevelOverride levelOverride in list.Overrides)
                {
                    if (levelOverride.IsFormatting)
                        DoHandleRunPr(levelOverride.ListLevel.RunPr);
                }
            }
        }

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            DoHandleRunPr(paragraph.ParagraphBreakRunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentStart(Comment comment)
        {
            DoHandleRunPr(comment.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            DoHandleRunPr(footnote.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitShapeStart(Shape shape)
        {
            DoHandleRunPr(shape.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGroupShapeStart(GroupShape groupShape)
        {
            DoHandleRunPr(groupShape.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFormField(FormField formField)
        {
            DoHandleRunPr(formField.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRun(Run run)
        {
            DoHandleRunPr(run.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            DoHandleRunPr(fieldStart.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            DoHandleRunPr(fieldSeparator.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            DoHandleRunPr(fieldEnd.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitSpecialChar(SpecialChar specialChar)
        {
            DoHandleRunPr(specialChar.RunPr);
            return VisitorAction.Continue;
        }

        protected abstract void DoHandleRunPr(RunPr runPr);
    }
}
