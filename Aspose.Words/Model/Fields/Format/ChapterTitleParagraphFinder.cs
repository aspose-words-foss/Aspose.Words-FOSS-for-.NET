// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2013 by Ivan Lyagin

using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements an algorithm selecting a chapter title paragraph used to generate a chapter number
    /// for the specified PAGE or PAGEREF field considering differences in behavior of MS Word 2013
    /// and earlier versions.
    /// </summary>
    internal class ChapterTitleParagraphFinder : FieldParagraphFinder
    {
        /// <summary>
        /// Returns a chapter title paragraph used to generate a chapter number for the specified PAGE or PAGEREF field
        /// in the context of the given section.
        /// </summary>
        internal static Paragraph FindChapterTitleParagraph(Field field, Section formatSection)
        {
            // Ensure that the field is being updated.
            Debug.Assert(field.IsUpdating);

            ChapterTitleParagraphFinder finder = new ChapterTitleParagraphFinder(field, formatSection);
            return finder.FindChapterTitleParagraph();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private ChapterTitleParagraphFinder(Field field, Section formatSection)
        {
            mField = field;
            mFormatSection = formatSection;
        }

        /// <summary>
        /// Returns a chapter title paragraph used to generate a chapter number for the specified PAGE or PAGEREF field.
        /// </summary>
        private Paragraph FindChapterTitleParagraph()
        {
            Node startNode = GetStartNode();
            if (startNode == null)
                return null;

            return (startNode.GetAncestor(NodeType.HeaderFooter) != null)
                ? FindChapterTitleParagraphHeaderFooter()
                : FindChapterTitleParagraphMainText(startNode);
        }

        /// <summary>
        /// Returns a node which is the starting point to seek for a chapter title paragraph according to the given field type.
        /// </summary>
        private Node GetStartNode()
        {
            switch (mField.Type)
            {
                case FieldType.FieldPage:
                    return mField.Start;
                case FieldType.FieldPageRef:
                    return ((FieldPageRef)mField).GetChapterTitleNode();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a chapter title paragraph for the given PAGE field located in header/footer.
        /// </summary>
        private Paragraph FindChapterTitleParagraphHeaderFooter()
        {
            // MS Word does not consider bookmarks in headers/footers while PAGEREF fields' updating.
            if (mField.Type == FieldType.FieldPageRef)
                return null;

            // MS Word 2013 and earlier versions use different algorithms of chapter title paragraph selecting
            // for headers/footers. Respect the difference.
            if (mField.FetchDocument().CompatibilityOptions.MswVersion < MsWordVersionCore.Word2013)
                return FindParagraphHeaderFooter(mField);

            Paragraph chapterTitleParagraph = null;

            // MS Word 2013 works here as follows.
            //
            // 1. Get the first paragraph in the parent section's body considering nested (i.e. inside tables, etc.)
            //    paragraphs. If it is a chapter title, return it.
            Body body = mFormatSection.Body;
            if ((body != null) && body.HasChildNodes)
            {
                Node candidateNode = body.GetChild(NodeType.Paragraph, 0, true);
                chapterTitleParagraph = ConfirmParagraphNode(candidateNode);
            }

            // 2. If the first paragraph of the parent section's body is not a chapter title, seek for a chapter title
            //    paragraph backward starting from the parent section exclusively (i.e. before its first body paragraph).

            // 3. If nothing is found, seek for a chapter title paragraph forward starting from the parent section
            //    inclusively, since its body may contain chapter title paragraphs after its first paragraph.

            return chapterTitleParagraph
                ?? FindParagraph(mFormatSection, false, false)
                ?? FindParagraph(mFormatSection, true, true);
        }

        /// <summary>
        /// Returns a chapter title paragraph for the given PAGE field located in body or PAGEREF field which bookmark
        /// is located in body.
        /// </summary>
        private Paragraph FindChapterTitleParagraphMainText(Node startNode)
        {
            // MS Word works here as follows.
            //
            // 1. Seek backward from the start node.
            // 2. If nothing is found, seek forward from the start node.
            return FindParagraph(startNode, false, true) ?? FindParagraph(startNode, true, false);
        }

        internal override Paragraph ConfirmParagraph(Paragraph candidateParagraph)
        {
            if (candidateParagraph == null)
                return null;

            return IsChapterTitle(candidateParagraph) ? candidateParagraph : null;
        }

        /// <summary>
        /// Returns a value indicating whether the given paragraph can be treated as a chapter title in the context of
        /// the specified section.
        /// </summary>
        private bool IsChapterTitle(Paragraph paragraph)
        {
            // MS Word considers a paragraph to be a chapter title in the context of a concrete section,
            // if it satisfies the following conditions.
            //
            // 1. The paragraph and the section should belong to the same document.
            Node document = paragraph.GetAncestor(NodeType.Document);
            if ((document == null) || (document != mFormatSection.GetAncestor(NodeType.Document)))
                return false;

            // 2. The paragraph should be located directly in a body story. Even paragraphs in body textboxes
            //    are not considered.
            if (paragraph.GetStoryAncestor(NodeType.Body) == null)
                return false;

            // 3. The paragraph should have a heading style applied.
            int istd = paragraph.ParaPr.Istd;
            if (!StyleIndex.IsHeadingIstd(istd))
                return false;

            // 4. The paragraph's style should correspond to the section's chapter heading level.
            if (mFormatSection.SectPr.HeadingLevelForChapterFinal != istd)
                return false;

            // 5. The paragraph should be a list item.
            if (!paragraph.IsListItemFinal)
                return false;

            // 6. The paragraph's style should correspond to a style defined for the paragraph's list level.
            //    It seems to be a useless check but TestIndexAndTables.TestJira6554 fails without it.
            //    Use ListLabel.ListLevel to handle revisions properly.
            if (paragraph.ListLabel.ListLevelFinal.ParaStyleIstd != istd)
                return false;

            // WORDSNET-11170
            // 7. List item should has numberstate. For example, in some cases numberstate can be null because paragraph contains only page break
            if (paragraph.ListLabel.NumberStateFinal == null)
                return false;

            // 8. The paragraph's list level number format should contain any level number placeholder.
            return NumberFormatHasAnyLevelNumber(paragraph);
        }

        /// <summary>
        /// Returns a value indicating whether the number format of the given paragraph's list label list level contains
        /// any level number placeholder.
        /// </summary>
        private static bool NumberFormatHasAnyLevelNumber(Paragraph paragraph)
        {
            // Use ListLabel.ListLevel to handle revisions properly.
            string numberFormat = paragraph.ListLabel.ListLevelFinal.NumberFormat;

            foreach (char c in numberFormat)
            {
                if (ListLevel.IsLevelNumberValid(c))
                    return true;
            }

            return false;
        }

        internal override bool IsForwardPageScan
        {
            get { return false; }
        }

        private readonly Field mField;
        private readonly Section mFormatSection;
    }
}
