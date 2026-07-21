// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/10/2023 by Alexander Zhiltsov

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Represents a class to calculate and update footnote reference indexes.
    /// </summary>
    /// <seealso cref="Footnote.Index"/>
    /// <seealso cref="Footnote.ActualReferenceMark"/>
    /// <seealso cref="Document.UpdateActualReferenceMarks"/>
    internal class FootnoteIndexUpdater : DocumentVisitor
    {
        /// <summary>
        /// Ctor. It is not possible to create instances of this class externally.
        /// </summary>
        private FootnoteIndexUpdater(Document doc)
        {
            // FOSS
            mFootnoteIndexGenerator = new IndexGenerator();
            mEndnoteIndexGenerator = new IndexGenerator();
        }

        /// <summary>
        /// Calculates actual footnote reference indexes and updates the <see cref="Footnote.Index"/> properties.
        /// </summary>
        internal static void Execute(Document doc)
        {
            doc.Accept(new FootnoteIndexUpdater(doc));
        }

        public override VisitorAction VisitSectionStart(Section section)
        {
            mFootnoteIndexGenerator.Options = section.PageSetup.FootnoteOptions;
            mFootnoteIndexGenerator.ResetIndexInSection();

            mEndnoteIndexGenerator.Options = section.PageSetup.EndnoteOptions;
            mEndnoteIndexGenerator.ResetIndexInSection();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            if (!footnote.IsAuto)
                return VisitorAction.Continue;

            footnote.Index = (footnote.FootnoteType == FootnoteType.Footnote)
                ? mFootnoteIndexGenerator.GetNextIndex(footnote)
                : mEndnoteIndexGenerator.GetNextIndex(footnote);

            return VisitorAction.Continue;
        }

        private readonly IndexGenerator mFootnoteIndexGenerator;
        private readonly IndexGenerator mEndnoteIndexGenerator;

        /// <summary>
        /// Stores the current state of the index generation process and generates the next index value.
        /// </summary>
        private class IndexGenerator
        {
            internal void ResetIndexInSection()
            {
                mIndexInSection = 0;
            }

            internal int GetNextIndex(Node footnote)
            {
                // Looks like MS Word uses Continuous numbering rule if start number is not 1.
                FootnoteNumberingRule rule = (Options.StartNumber == 1)
                    ? Options.RestartRule
                    : FootnoteNumberingRule.Continuous;

                int index;

                switch (rule)
                {
                    case FootnoteNumberingRule.RestartSection:
                        index = mIndexInSection;
                        break;
                    case FootnoteNumberingRule.RestartPage:
                        // FOSS
                        index = mIndexInPage;
                        break;
                    case FootnoteNumberingRule.Continuous:
                    default:
                        index = mIndexInDocument;
                        break;
                }

                mIndexInDocument++;
                mIndexInSection++;
                mIndexInPage++;

                return index + Options.StartNumber;
            }

            internal IFootnoteOptions Options { get; set; }

            private int mIndexInDocument;
            private int mIndexInSection;
            private int mIndexInPage;
        }
    }
}
