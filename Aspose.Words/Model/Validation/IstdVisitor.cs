// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2020 by Alexey Morozov

using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Visits all attribute collections that may contain Istd.
    /// </summary>
    internal abstract class IstdVisitor : DocumentVisitor
    {
        /// <summary>
        /// Runs visiting all attribute collections to process style Istds.
        /// </summary>
        protected void Run(DocumentBase doc)
        {
            OnRunPr(doc.Styles.DefaultRunPr);
            OnParaPr(doc.Styles.DefaultParaPr);

            doc.Accept(this);

            VisitFootnoteSeparators(doc);
            OnDocPr(doc.DocPr);
        }

        /// <summary>
        /// Visits run properties.
        /// </summary>
        protected abstract void OnRunPr(RunPr runPr);

        /// <summary>
        /// Visits paragraph properties.
        /// </summary>
        protected abstract void OnParaPr(ParaPr paraPr);

        /// <summary>
        /// Visits row properties.
        /// </summary>
        protected abstract void OnRowPr(TablePr cellPr);

        /// <summary>
        /// Visits document properties.
        /// </summary>
        protected abstract void OnDocPr(DocPr docPr);

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            OnParaPr(paragraph.ParaPr);
            OnRunPr(paragraph.ParagraphBreakRunPr);

            if (paragraph.ParaPr.HasFormatRevision)
                OnParaPr((ParaPr)paragraph.ParaPr.FormatRevision.RevPr);

            if (paragraph.ParagraphBreakRunPr.HasFormatRevision)
                OnRunPr((RunPr)paragraph.ParagraphBreakRunPr.FormatRevision.RevPr);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRun(Run run)
        {
            OnRunPr(run.RunPr);

            if (run.RunPr.HasFormatRevision)
                OnRunPr((RunPr)run.RunPr.FormatRevision.RevPr);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            OnRowPr(row.TablePr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitSpecialChar(SpecialChar specialChar)
        {
            // WORDSNET-15076 Collect styles from special character nodes to avoid loss of styles.
            OnRunPr(specialChar.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            OnRunPr(footnote.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            OnRunPr(fieldStart.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            OnRunPr(fieldSeparator.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            OnRunPr(fieldEnd.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFormField(FormField formField)
        {
            OnRunPr(formField.RunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            OnRunPr(sdt.ContentsRunPr);
            OnRunPr(sdt.EndCharacterRunPr);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitShapeStart(Shape shape)
        {
            OnRunPr(shape.RunPr);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Visits contents of footnote separators.
        /// </summary>
        private void VisitFootnoteSeparators(DocumentBase doc)
        {
            foreach (FootnoteSeparator separator in doc.FootnoteSeparators)
                separator.Accept(this);
        }
    }
}
