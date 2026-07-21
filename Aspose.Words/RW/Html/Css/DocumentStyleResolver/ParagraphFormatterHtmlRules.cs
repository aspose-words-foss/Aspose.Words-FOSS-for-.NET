// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2017 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to paragraphs. Uses formatting rules that are different from what MS Word uses but produce
    /// better looking results and are more confomant to the CSS and HTML specifications.
    /// </summary>
    internal class ParagraphFormatterHtmlRules : ParagraphFormatter
    {
        internal ParagraphFormatterHtmlRules(bool useHtmlBlocks)
        {
            mUseHtmlBlocks = useHtmlBlocks;
        }

        protected override void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssStyleTracker cssStyleTracker)
        {
            CssLineHeightStyleConverter.ToParagraphFormat(cssStyleTracker.ElementDeclarations, paragraphFormat, false);
            CssMargins physicalMargins;

            // WORDSNET-16334 If we use HtmlBlock nodes to store borders and margins of block-level HTML elements,
            // we don't need the box model and we apply only the current element margins directly to the current paragraph.
            if (mUseHtmlBlocks)
            {
                double left = GetMargin(cssStyleTracker.ElementDeclarations, "margin-left");
                double right = GetMargin(cssStyleTracker.ElementDeclarations, "margin-right");
                double top = GetMargin(cssStyleTracker.ElementDeclarations, "margin-top");
                double bottom = GetMargin(cssStyleTracker.ElementDeclarations, "margin-bottom");
                physicalMargins = new CssMargins();

                if (!MathUtil.IsZero(left))
                    physicalMargins.Left = left;

                if (!MathUtil.IsZero(right))
                    physicalMargins.Right = right;

                if (!MathUtil.IsZero(top))
                    physicalMargins.Top = top;

                if (!MathUtil.IsZero(bottom))
                    physicalMargins.Bottom = bottom;
            }
            else
            {
                physicalMargins = cssStyleTracker.BoxModel.Margins;
            }
            ApplyMargins(physicalMargins, paragraphFormat, cssStyleTracker.GetBlockFlowDirection());
            
            ApplyOutlineLevel(paragraphFormat, cssStyleTracker.ElementDeclarations, cssStyleTracker.CurrentElement.ElementName);
        }

        private static void ApplyOutlineLevel(
            ParagraphFormat paragraphFormat,
            CssDeclarationCollection declarations,
            string elementName)
        {
            // WORDSNET-23183 Apply OutlineLevel to paragraphs created from HTML heading elements ('h1'..'h6') in order
            // to preserve the semantics of those elements and make their text appear on the navigation pane in MS Word.
            // This is needed for elements that have custom paragraph styles applied via CSS classes, because those custom
            // styles are created with the default outline level.
            if ((elementName.Length == 2) && (elementName[0] == 'h'))
            {
                char headingLevel = elementName[1];
                if ((headingLevel >= '1') && (headingLevel <= '6'))
                {
                    paragraphFormat.OutlineLevel = (OutlineLevel)(headingLevel - '1');
                }
            }

            double outlineLevelValue = declarations.GetNumber(HtmlConstants.OutlineLevel);
            if (MathUtil.IsMinValue(outlineLevelValue) || (outlineLevelValue < 1) || (outlineLevelValue > 9))
            {
                return;
            }
            paragraphFormat.OutlineLevel = (OutlineLevel)((int)outlineLevelValue - 1);
        }

        protected override void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssLineHeightStyleConverter.ToParagraphFormat(declarations, paragraphFormat, false);
            ApplyOutlineLevel(paragraphFormat, declarations, string.Empty);
        }

        private static double GetMargin(
            CssDeclarationCollection declarations,
            string propertyName)
        {
            double margin = declarations.GetLength(propertyName);
            return MathUtil.IsMinValue(margin)
                ? 0
                : margin;
        }

        private static void ApplyMargins(
            CssMargins physicalMargins,
            ParagraphFormat paragraphFormat,
            CssBlockFlowDirection blockFlowDirection)
        {
            CssMargins logicalMargins = physicalMargins.ToLogicalMargins(
                blockFlowDirection,
                paragraphFormat.Bidi);

            // Apply paragraph indents if they are specified in HTML or CSS. Note that left indent of list items is applied
            // by the list reader and we don't want to override its value.
            if (!paragraphFormat.IsListItem)
            {
                if (logicalMargins.UseLeft || (!MathUtil.IsZero(logicalMargins.Left)))
                {
                    paragraphFormat.LeftIndent = logicalMargins.Left;
                }
            }
            if (logicalMargins.UseRight || !MathUtil.IsZero(logicalMargins.Right))
            {
                paragraphFormat.RightIndent = logicalMargins.Right;
            }

            // Top spacing.
            if (logicalMargins.UseTop || !MathUtil.IsZero(logicalMargins.Top))
            {
                // WORDSNET-20282 Validate and set the top margin in the range.
                paragraphFormat.SpaceBefore = MathUtil.FitToRange(logicalMargins.Top, 0, ConvertUtilCore.MaxSizePoint);
            }

            // Bottom spacing.
            if (logicalMargins.UseBottom || !MathUtil.IsZero(logicalMargins.Bottom))
            {
                // WORDSNET-20282 Validate and set the bottom margin in the range.
                paragraphFormat.SpaceAfter = MathUtil.FitToRange(logicalMargins.Bottom, 0, ConvertUtilCore.MaxSizePoint);
            }
        }

        /// <summary>
        /// Indicates whether to import borders and the margins of parent block-level HTML elements as <see cref="HtmlBlock"/>
        /// nodes.
        /// </summary>
        private readonly bool mUseHtmlBlocks;
    }
}
