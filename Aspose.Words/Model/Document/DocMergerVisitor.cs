// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/10/2023 by Ilya Navrotskiy

using Aspose.Words.Fields;
using Aspose.Words.Notes;

namespace Aspose.Words
{
    /// <summary>
    /// The visitor for <see cref="DocumentMerger"/> that collects some helper information about the document.
    /// </summary>
    internal class DocMergerVisitor : DocumentVisitor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DocMergerVisitor"/> class.
        /// </summary>
        /// <param name="stopSection">The section after which we stop the visitor.
        /// This is need as we are going to collect data when the source document already has been merged
        /// with the destination one and we need to distinguish the documents in their original state.
        /// </param>
        internal DocMergerVisitor(Section stopSection)
        {
            mStopSection = stopSection;
        }

        /// <summary>
        /// Called when enumeration of a section has ended.
        /// </summary>
        public override VisitorAction VisitSectionEnd(Section section)
        {
            if (section == mStopSection)
                return VisitorAction.Stop;

            return base.VisitSectionEnd(section);
        }

        /// <summary>
        /// Called when enumeration of a footnote or endnote text has started.
        /// </summary>
        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            if (footnote.FootnoteType == FootnoteType.Footnote)
                HasFootnotes = true;

            return base.VisitFootnoteStart(footnote);
        }

        /// <summary>
        /// Called when a form field is encountered in the document.
        /// </summary>
        public override VisitorAction VisitFormField(FormField formField)
        {
            HasFormFields = true;
            return base.VisitFormField(formField);
        }

        /// <summary>
        /// Gets a boolean value indicating if there are any footnotes in the visiting document.
        /// </summary>
        internal bool HasFootnotes { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating if there are any form fields in the visiting document.
        /// </summary>
        internal bool HasFormFields { get; private set; }

        /// <summary>
        /// Specifies the last section after which we stop the visitor work.
        /// </summary>
        private readonly Section mStopSection;
    }
}
