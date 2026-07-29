// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2015 by Anton Savko

using System.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader.CommonBorder
{
    /// <summary>
    /// An element that might be used to draw a common border in HTML.
    /// </summary>
    internal class CommonBorderContainer
    {
        internal CommonBorderContainer(
            CommonBorderContainer parentContainer,
            BordersInfo borders)
        {
            Debug.Assert(borders != null);

            mParent = parentContainer;
            mContainerBorders = borders;
            mParagraphBordersPairs = new List<ParagraphBordersPair>();
        }

        internal void Add(CompositeNode node, BordersInfo borders)
        {
            CommonBorderContainer currentContainer = this;

            // Find a parent with borders.
            while ((currentContainer != null) &&
                !currentContainer.mContainerBorders.HasPaddingOrVisibleBorders)
            {
                currentContainer = currentContainer.mParent;
            }

            // If there is a parent with borders, add the node there.
            if (currentContainer != null)
            {
                Paragraph paragraph = node as Paragraph;
                if (paragraph != null)
                {
                    currentContainer.AddParagraph(paragraph, borders);
                }
                else
                {
                    Table table = node as Table;
                    if (table != null)
                    {
                        currentContainer.AddTable(table, borders);
                    }
                }
            }
        }

        internal void ApplyCommonBorderIfNeeded()
        {
            // Nothing to apply if the current container doesn't have visible borders.
            if (!mContainerBorders.HasPaddingOrVisibleBorders)
            {
                return;
            }

            if ((mFirstTable == null) && (mParagraphBordersPairs.Count == 0))
                return;

            if ((mFirstTable != null) && !mIsMultipleTablesAdded)
                ApplyCommonBorderForSingleTableIfNeeded();
            else if (mParagraphBordersPairs.Count == 1)
                ApplyCommonBorderForSingleParagraphIfNeeded();
            else
                ApplyCommonBorderForMultipleParagraphsIfNeeded();
        }

        private void AddParagraph(Paragraph paragraph, BordersInfo borders)
        {
            Debug.Assert(borders != null);

            if (mFirstTable != null)
                return;

            ParagraphBordersPair paragraphBordersPair = new ParagraphBordersPair(paragraph, borders);

            int index = 0;
            while (index < mParagraphBordersPairs.Count)
            {
                ParagraphBordersPair currentPair = mParagraphBordersPairs[index];
                if (currentPair.Paragraph == paragraphBordersPair.Paragraph)
                    break;
                index++;
            }

            if (index < mParagraphBordersPairs.Count)
            {
                // Due to logic of DocumentBuilder and HtmlCommonBorderResolver
                // last added paragraph will have actual information about borders.
                mParagraphBordersPairs.RemoveAt(index);
            }
            mParagraphBordersPairs.Add(paragraphBordersPair);
        }

        private void AddTable(Table table, BordersInfo borders)
        {
            Debug.Assert(borders != null);

            if (mFirstTable == null)
            {
                mFirstTable = table;
                mFirstTableBorders = borders;
            }
            else if (!mIsMultipleTablesAdded && !table.IsAncestorNode(mFirstTable))
            {
                mIsMultipleTablesAdded = true;
            }
        }

        private void ApplyCommonBorderForSingleTableIfNeeded()
        {
            // Here, if container borders are invisible, the container element has only padding declarations. We don't apply
            // padding to child tables so we just exit.
            if (!mContainerBorders.IsVisible)
            {
                return;
            }

            // For proper output apply container's borders only to tables with "width:100%" style.
            if (!mFirstTable.PreferredWidth.Equals(PreferredWidth.FromPercent(100)))
                return;

            // Due to logic of DocumentBuilder and HtmlCommonBorderResolver
            // sometimes one paragraph is added even if container doesn't contain any.
            if ((ParagraphsCount > 0) && (GetParagraph(0).FirstRun != null))
                return;

            if (!mFirstTableBorders.IsVisible)
            {
                for (int i = 0; i < mFirstTable.Rows.Count; i++)
                    mContainerBorders.ApplyBorderPropertiesTo(mFirstTable.Rows[i].RowFormat.Borders);
            }
        }

        private void ApplyCommonBorderForSingleParagraphIfNeeded()
        {
            // We copy container borders to paragraph 'as is' if there is one inner paragraph in the container.
            Paragraph firstPara = GetParagraph(0);

            BordersInfo firstParaBorders = GetParaBorders(0);

            // WORDSNET-19178 Do not apply the common border to an empty paragraph after multiple tables.
            // However, we should apply borders single empty paragraphs inside conatiners with borders.
            if (!firstParaBorders.IsVisible && ((firstPara.GetChildNodes(NodeType.Any, false).Count > 0) || !mIsMultipleTablesAdded))
            {
                // Note that container borders can be invisible here. In that case we apply only padding values to the child
                // paragraph.
                mContainerBorders.ApplyBorderPropertiesTo(firstPara.ParagraphFormat.Borders);
                mContainerBorders.ApplyPaddingTo(firstPara.ParagraphFormat.Borders);
            }

            // WORDSNET-11086 A div element around a single paragraph may be a special div we have created during export
            // to hold paragraph's border and shading in case the paragraph has a hanging indent. Such paragraphs may also be
            // enclosed in a div with common paragraph borders so they should be reprocessed at a higher level.
            if (mParent != null)
            {
                mParent.Add(firstPara, mContainerBorders);
            }
        }

        private void ApplyCommonBorderForMultipleParagraphsIfNeeded()
        {
            // Here, if container borders are invisible, the container element has only padding declarations. The following
            // code is designed to work with visible common borders so we just exit.
            if (!mContainerBorders.IsVisible)
            {
                return;
            }

            Paragraph firstPara = GetParagraph(0);
            BordersInfo firstParaBorders = GetParaBorders(0);

            // AW paragraph border to text distance and after\before spacing are converted into CSS padding property during HTML export.
            // We need to find the common paragraph borders saved in HTML and to restore the original border properties during HTML import. 
            // We ignore common HTML borders that cannot be represented in AW.
            // Please see CssParagraphWriter.PaddingsToCss(...) function to find how common borders are exported into HTML.
            BordersInfo lastParaBorders = GetParaBorders(ParagraphsCount - 1);
            bool hasCommonHorizontalBorder = firstParaBorders.Bottom.IsVisible;
            double commonTopDistanceFromText = firstParaBorders.Top.Padding;
            double commonBottomDistanceFromText = lastParaBorders.Bottom.Padding;
            double commonSpaceAfter = (hasCommonHorizontalBorder)
                                          ? firstParaBorders.Bottom.Padding - commonTopDistanceFromText
                                          : firstParaBorders.Bottom.Padding;
            BorderInfo commonHorizontalBorder = (hasCommonHorizontalBorder)
                                                    ? firstParaBorders.Bottom
                                                    : null;

            bool isCommonBorderRepresentable = true;
            // There are should be no other nodes between the paragraphs.
            for (int i = 0; i < ParagraphsCount; i++)
            {
                if ((i != ParagraphsCount - 1) && (GetParagraph(i).NextNonAnnotationSibling != GetParagraph(i + 1)))
                {
                    isCommonBorderRepresentable = false;
                    break;
                }
            }

            if (!isCommonBorderRepresentable)
                return;

            // All paragraphs should belong to the same list and list level.
            for (int i = 1; i < ParagraphsCount; i++)
            {
                Paragraph para = GetParagraph(i);
                if ((para.ListFormat.ListId != firstPara.ListFormat.ListId) ||
                    (para.ListFormat.ListLevelNumberOriginal != firstPara.ListFormat.ListLevelNumberOriginal))
                {
                    isCommonBorderRepresentable = false;
                    break;
                }
            }

            for (int i = 0; i < ParagraphsCount; i++)
            {
                BordersInfo paraBorders = GetParaBorders(i);

                // Ignore common border if an inner paragraph has own border.
                if (paraBorders.Top.IsVisible || paraBorders.Right.IsVisible || paraBorders.Left.IsVisible)
                {
                    isCommonBorderRepresentable = false;
                    break;
                }

                // There is no reason to compare the first paragraph borders with itself.
                if (i == 0)
                    continue;

                // Ignore common border if we cannot emulate horizontal border correctly.
                if ((i != ParagraphsCount - 1) && hasCommonHorizontalBorder &&
                    (!paraBorders.Bottom.IsVisible ||
                     (commonHorizontalBorder.LineStyle != paraBorders.Bottom.LineStyle)))
                {
                    isCommonBorderRepresentable = false;
                    break;
                }

                double paraPaddingTop = paraBorders.Top.Padding;
                double paraPaddingBottom = paraBorders.Bottom.Padding;
                if (i == ParagraphsCount - 1)
                {
                    if (hasCommonHorizontalBorder && !MathUtil.AreEqual(paraPaddingTop, commonTopDistanceFromText))
                        isCommonBorderRepresentable = false;
                    if (!MathUtil.AreEqual(paraPaddingBottom, commonBottomDistanceFromText))
                        isCommonBorderRepresentable = false;
                }
                else
                {
                    if (!hasCommonHorizontalBorder)
                    {
                        if (!MathUtil.AreEqual(paraPaddingBottom, commonSpaceAfter))
                            isCommonBorderRepresentable = false;
                    }
                    else
                    {
                        if (!MathUtil.AreEqual(paraPaddingTop, commonTopDistanceFromText))
                            isCommonBorderRepresentable = false;
                        if (!MathUtil.AreEqual(paraPaddingBottom, commonSpaceAfter + commonTopDistanceFromText))
                            isCommonBorderRepresentable = false;
                    }
                }

                if (!isCommonBorderRepresentable)
                    break;
            }

            if (!isCommonBorderRepresentable)
                return;

            for (int i = 0; i < ParagraphsCount; i++)
            {
                ParagraphFormat pf = GetParagraph(i).ParagraphFormat;

                if (hasCommonHorizontalBorder)
                {
                    commonHorizontalBorder.ApplyBorderPropertiesTo(pf.Borders.Horizontal);
                    pf.Borders.Horizontal.SetDistanceFromTextSafe(commonTopDistanceFromText);
                }

                mContainerBorders.ApplyBorderPropertiesTo(pf.Borders);
                pf.Borders.Top.SetDistanceFromTextSafe(commonTopDistanceFromText);
                pf.Borders.Bottom.SetDistanceFromTextSafe(commonBottomDistanceFromText);
            }
        }

        private Paragraph GetParagraph(int index)
        {
            return mParagraphBordersPairs[index].Paragraph;
        }

        private BordersInfo GetParaBorders(int index)
        {
            return mParagraphBordersPairs[index].Borders;
        }

        private int ParagraphsCount
        {
            get { return mParagraphBordersPairs.Count; }
        }

        /// <summary>
        /// Parent container. <c>null</c> if this is the topmost container (has no parent).
        /// </summary>
        private readonly CommonBorderContainer mParent;

        private readonly BordersInfo mContainerBorders;
        /// <summary>
        /// Stores collection of ParagraphBordersPair objects.
        /// </summary>
        private readonly List<ParagraphBordersPair> mParagraphBordersPairs;
        private Table mFirstTable;
        private BordersInfo mFirstTableBorders;
        private bool mIsMultipleTablesAdded;
    }
}
