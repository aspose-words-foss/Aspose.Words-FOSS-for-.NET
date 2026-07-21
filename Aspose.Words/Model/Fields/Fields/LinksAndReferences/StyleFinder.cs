// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2010 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Searches for a fragment of text in the document formatted with the specified style.
    /// The style may be either a paragraph or character style.
    /// </summary>
    internal class StyleFinder : FieldParagraphFinder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private StyleFinder(string styleName, StyleCollection styles)
        {
            Debug.Assert(StringUtil.HasChars(styleName));

            mStyles = styles;

            mStyleName = styleName;
            mInvariantStyleName = StyleNameTranslator.GetInvariantStyleName(styleName);

            string separator = FormatterPal.GetListSeparatorCurrent().ToString();
            mStyleNameParts = styleName.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            mInvariantStyleNameParts = new string[mStyleNameParts.Length];
            for (int i = 0; i < mStyleNameParts.Length; i++)
                mInvariantStyleNameParts[i] = StyleNameTranslator.GetInvariantStyleName(mStyleNameParts[i]);

            mIstd = FormatterPal.TryParseInt(styleName);
        }

        /// <summary>
        /// Searches for the specified style for the given field.
        /// </summary>
        internal static StyleSearchResult FindStyle(FieldStyleRef field, string styleName)
        {
            // Ensure that the field is being updated.
            Debug.Assert(field.IsUpdating);

            StyleFinder finder = new StyleFinder(styleName, field.Document.Styles);
            return finder.FindStyle(field);
        }

        private StyleSearchResult FindStyle(FieldStyleRef field)
        {
            if (field.IsInHeaderFooter)
            {
                FindStyleHeaderFooter(field);
            }
            else
            {
                FindStyleMainText(field);
            }

            return mResult;
        }

        private void FindStyleHeaderFooter(FieldStyleRef field)
        {
            // WORDSNET-12038 Searches paragraph in backward direction, if \l switch specified.
            mIsForwardDirection = !field.SearchFromBottom;

            FindParagraphHeaderFooter(field);
        }

        protected override void OnPageScanned()
        {
            // 2. MS Word scans every page before the field's one in backward direction.
            mIsForwardDirection = false;
        }

        protected override void OnAllPagesScanned()
        {
            // 3. MS Word scans contents after the field in forward direction.
            mIsForwardDirection = true;
        }

        private void FindStyleMainText(Field field)
        {
            // Go backward first. Go forward then.
            if (!FindStyleMainText(field, false))
                FindStyleMainText(field, true);
        }

        private bool FindStyleMainText(Field field, bool isForwardDirection)
        {
            mIsForwardDirection = isForwardDirection;

            return (FindParagraph(field.Start.ParentParagraph, isForwardDirection, false) != null);
        }

        internal override Paragraph ConfirmParagraph(Paragraph candidateParagraph)
        {
            if (candidateParagraph == null)
                return null;

            mResult = FindStyleInParagraph(candidateParagraph);

            return (mResult != null) ? mResult.Paragraph : null;
        }

        /// <summary>
        /// Searches for a style in the specified paragraph.
        /// </summary>
        private StyleSearchResult FindStyleInParagraph(Paragraph paragraph)
        {
            // MS Word considers only main text paragraphs.
            if (paragraph.GetAncestor(NodeType.Body) == null)
                return null;

            if (CompareParagraphStyle(paragraph))
            {
                return StyleSearchResult.CreateWholeParagraphResult(paragraph, mIsForwardDirection);
            }

            // Try to find runs with specified style.
            int startIndex = -1;
            int endIndex = -1;

            // We should start from a paragraph break, if we are moving backward.
            bool isParagraphBreakStyleApplied = (!mIsForwardDirection && CompareParagraphBreakStyle(paragraph));
            bool isParagraphBreakIncluded = isParagraphBreakStyleApplied && IsParagraphBreakIncluded(paragraph);

            // This will only enumerate over inlines that are immediate children of the paragraph.
            // But there could be more inlines, inside Paragraph/SmartTag/Run for example.
            // I could use paragraph.GetChildNodes(NodeType.Any, true) to get a deep collection of inlines,
            // but it will also have its problems - it will include inlines that are inside a textbox shape for example,
            // which I don't want to include.
            // So I leave the current approach as is.
            NodeCollection inlines = GetParagraphInlines(paragraph);
            int inlinesCount = inlines.Count;
            for (int i = 0; i < inlinesCount; i++)
            {
                int currentIndex = mIsForwardDirection ? i : (inlinesCount - 1 - i);
                IInline inline = (IInline)inlines[currentIndex];

                if (CompareInlineStyle(inline))
                {
                    endIndex = currentIndex;
                    if (startIndex < 0)
                        startIndex = currentIndex;
                }
                else if ((startIndex >= 0) || isParagraphBreakIncluded)
                {
                    // If the style of the current inline differs from the one we are looking for and we have captured something
                    // (i.e. a inline or a paragraph break), then it is time to complete the inline index range.
                    break;
                }
            }

            // We should consider a paragraph break after all of the paragraph inlines have been considered, if we are moving forward.
            if (mIsForwardDirection)
            {
                // We can include a paragraph break only if:
                //   - we have not captured any paragraph inline or
                //   - we have captured the last paragraph inline.
                int maxIndex = System.Math.Max(startIndex, endIndex);
                bool canIncludeParagraphBreak = (maxIndex < 0) || (maxIndex == inlinesCount - 1);
                isParagraphBreakStyleApplied = canIncludeParagraphBreak && CompareParagraphBreakStyle(paragraph);
                isParagraphBreakIncluded = isParagraphBreakStyleApplied && IsParagraphBreakIncluded(paragraph);
            }

            return StyleSearchResult.CreateIfNeeded(paragraph, startIndex, endIndex, mIsForwardDirection, isParagraphBreakIncluded, isParagraphBreakStyleApplied);
        }

        internal static NodeCollection GetParagraphInlines(Paragraph paragraph)
        {
            return paragraph.GetChildNodes(new NodeType[] { NodeType.Run, NodeType.Footnote, NodeType.FieldStart, NodeType.FieldEnd }, false);
        }

        /// <summary>
        /// Returns a value indicating whether the specified paragraph's style is the one we are searching for.
        /// </summary>
        private bool CompareParagraphStyle(Paragraph paragraph)
        {
            return IsParagraphAcceptable(paragraph) && CompareStyle(paragraph.ParaPr, true);
        }

        private bool IsParagraphAcceptable(Paragraph paragraph)
        {
            return !mIsForwardDirection || !IsEmptySectionBreakParagraph(paragraph);
        }

        private static bool IsEmptySectionBreakParagraph(Paragraph paragraph)
        {
            return paragraph.IsSectionBreakParagraph &&
                   paragraph.Runs.Count == 0;
        }

        /// <summary>
        /// Returns a value indicating whether the specified paragraph's break has a style we are searching for.
        /// </summary>
        private bool CompareParagraphBreakStyle(Paragraph paragraph)
        {
            return CompareStyle(paragraph.ParagraphBreakRunPr, false);
        }

        private static bool IsParagraphBreakIncluded(Paragraph paragraph)
        {
            if (paragraph.ParentNode.NodeType != NodeType.Cell)
                return true;

            if (paragraph.NextNonAnnotationSibling != null)
                return true;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the specified inline's style is the one we are searching for.
        /// </summary>
        private bool CompareInlineStyle(IInline inline)
        {
            return CompareStyle(inline.RunPr_IInline, false);
        }

        private bool CompareStyle(AttrCollection attrs, bool isParaPr)
        {
            // WORDSNET-7376 Look for style by name or by index.
            int key = isParaPr ? ParaAttr.Istd : FontAttr.Istd;
            int istd = (int)attrs.FetchAttr(key);
            if (mIstd != int.MinValue)
                return (mIstd == istd);

            int defaultIstd = isParaPr ? StyleIndex.Normal : StyleIndex.DefaultParagraphFont;
            Style style = mStyles.FetchByIstd(istd, defaultIstd);
            return CompareStyle(style);
        }

        private bool CompareStyle(Style style)
        {
            if (CompareWithStyleName(style.Name))
                return true;

            // WORDSNET-9885 Search for style by name that includes the alias.
            if (CompareWithStyleName(style.GetNameWithAliases()))
                return true;

            // WORDSNET-9953 Search for style by styles aliases.
            foreach (string alias in style.GetAliasesInternal())
            {
                if (CompareWithStyleName(alias))
                    return true;
            }

            return CompareStyleParts(style);
        }

        private bool CompareStyleParts(Style style)
        {
            string[] styleAliases = style.GetAliasesInternal();
            string styleName = style.Name;

            for (int i = 0; i < mStyleNameParts.Length; i++)
            {
                string styleNamePart = mStyleNameParts[i];
                string invariantStyleNamePart = mInvariantStyleNameParts[i];

                if (CompareWithStyleName(styleName, styleNamePart, invariantStyleNamePart))
                    continue;

                if (CompareWithStyleName(styleAliases, styleNamePart, invariantStyleNamePart))
                    continue;

                return false;
            }

            return true;
        }

        private bool CompareWithStyleName(string value)
        {
            return CompareWithStyleName(value, mStyleName, mInvariantStyleName);
        }

        private bool CompareWithStyleName(string value, string styleName, string invariantStyleName)
        {
            return CompareWithStyleNameCaseSensitiveAware(value, styleName) || CompareWithStyleNameCaseSensitiveAware(value, invariantStyleName);
        }

        private bool CompareWithStyleNameCaseSensitiveAware(string value, string styleName)
        {
            if (styleName == null)
                return false;

            return RequireCaseSensitiveComparison(styleName)
                ? value.Equals(styleName, StringComparison.Ordinal)
                : StringUtil.EqualsIgnoreCase(value, styleName);
        }

        private bool CompareWithStyleName(string[] values, string styleName, string invariantStyleName)
        {
            foreach (string value in values)
            {
                if (CompareWithStyleName(value, styleName, invariantStyleName))
                    return true;
            }

            return false;
        }

        internal override bool IsForwardPageScan
        {
            get { return mIsForwardDirection; }
        }

        internal override bool NeedCachePageScanResult(Paragraph paragraph)
        {
            return !IsForwardPageScan || paragraph == null;
        }

        private bool RequireCaseSensitiveComparison(string styleName)
        {
            bool caseSensitive;
            if (mCaseSensitiveCache.TryGetValue(styleName, out caseSensitive))
                return caseSensitive;

            bool result = RequireCaseSensitiveComparisonCore(styleName);
            mCaseSensitiveCache[styleName] = result;

            return result;
        }

        private bool RequireCaseSensitiveComparisonCore(string styleName)
        {
            int count = 0;
            foreach (Style style in mStyles)
            {
                if (IsStyleNameOrAnyAliasSame(style, styleName))
                    count++;

                if (count > 1)
                    return true;
            }

            return false;
        }

        private static bool IsStyleNameOrAnyAliasSame(Style style, string styleName)
        {
            if (StringUtil.EqualsOrdinalIgnoreCase(styleName, style.Name))
                return true;

            foreach (string alias in style.GetAliasesInternal())
            {
                if (StringUtil.EqualsOrdinalIgnoreCase(styleName, alias))
                    return true;
            }

            return false;
        }

        private readonly string mStyleName;
        private readonly StyleCollection mStyles;
        private readonly string mInvariantStyleName;
        private readonly string[] mStyleNameParts;
        private readonly string[] mInvariantStyleNameParts;
        private readonly int mIstd;
        private readonly Dictionary<string, bool> mCaseSensitiveCache = new Dictionary<string, bool>();

        private bool mIsForwardDirection;
        private StyleSearchResult mResult;
    }
}
