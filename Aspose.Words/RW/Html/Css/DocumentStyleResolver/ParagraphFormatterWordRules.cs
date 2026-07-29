// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2017 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to paragraphs. Uses same formatting rules as MS Word.
    /// </summary>
    internal class ParagraphFormatterWordRules : ParagraphFormatter
    {
        protected override void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssStyleTracker cssStyleTracker)
        {
            // MS Word applies the following default styles to paragraphs imported from <li> elements.
            IHtmlElementProvider currentElement = cssStyleTracker.CurrentElement;
            if (currentElement.ElementName == "li")
            {
                paragraphFormat.SpaceBefore = 5;
                paragraphFormat.SpaceBeforeAuto = true;
                paragraphFormat.SpaceAfter = 5;
                paragraphFormat.SpaceAfterAuto = true;
                paragraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                paragraphFormat.LineSpacing = 12;
            }

            // WORDSNET-17035 When HTML is imported by MS Word, the "MsoNormal" class name has special meaning:
            // it is an alias for the "Normal" paragraph style. for MS Word. This means that the paragraph imported for
            // the <p class="msonormal"> element will be formatted using the "Normal" style instead of "Normal (Web)".
            // In constrast to "Normal" paragraphs imported from plain text (from anonymous text blocks), paragraphs
            // with the "msonormal" class still get default (auto) vertical margins (paragraph spacing) in MS Word.
            if (currentElement.ElementName == "p")
            {
                string[] classes = currentElement.GetClasses();
                if ((classes.Length > 0) && string.Equals(classes[0], "msonormal", StringComparison.OrdinalIgnoreCase))
                {
                    paragraphFormat.SpaceBefore = 5;
                    paragraphFormat.SpaceBeforeAuto = true;
                    paragraphFormat.SpaceAfter = 5;
                    paragraphFormat.SpaceAfterAuto = true;
                }
            }

            // WORDSNET-23496 Now, we use HtmlBlock nodes to store borders and margins of block-level HTML elements,
            // we don't need the box model and we apply only the current element margins directly to the current paragraph.
            ApplyMargins(paragraphFormat, cssStyleTracker);

            CssLineHeightStyleConverter.ToParagraphFormat(cssStyleTracker.ElementDeclarations, paragraphFormat, true);
        }

        protected override void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssLineHeightStyleConverter.ToParagraphFormat(declarations, paragraphFormat, true);
            ApplyMarginTop(paragraphFormat, declarations);
            ApplyMarginRight(paragraphFormat, declarations);
            ApplyMarginBottom(paragraphFormat, declarations);
            ApplyMarginLeft(paragraphFormat, declarations);

            ApplyMsoHtmlFormatting(paragraphFormat, declarations);
        }

        private static void ApplyMargins(ParagraphFormat paragraphFormat, CssStyleTracker cssStyleTracker)
        {
            double left = cssStyleTracker.ElementDeclarations.GetLength("margin-left");
            double right = cssStyleTracker.ElementDeclarations.GetLength("margin-right");
            double top = cssStyleTracker.ElementDeclarations.GetLength("margin-top");
            double bottom = cssStyleTracker.ElementDeclarations.GetLength("margin-bottom");

            CssMargins physicalMargins = new CssMargins();
            if (!MathUtil.IsMinValue(left))
                physicalMargins.Left = left;
            if (!MathUtil.IsMinValue(right))
                physicalMargins.Right = right;
            if (!MathUtil.IsMinValue(top))
                physicalMargins.Top = top;
            if (!MathUtil.IsMinValue(bottom))
                physicalMargins.Bottom = bottom;

            CssMargins logicalMargins = physicalMargins.ToLogicalMargins(
                CssBlockFlowDirection.HorizontalTb,
                paragraphFormat.Bidi);

            if (logicalMargins.UseLeft)
            {
                // If the paragraph is a list item then MS Word sets the left indent
                // as the sum of the list item left indent and the paragraph left margin.
                if (paragraphFormat.IsListItem)
                {
                    paragraphFormat.LeftIndent += logicalMargins.Left;
                }
                else
                {
                    paragraphFormat.LeftIndent = logicalMargins.Left;
                }
            }
            if (logicalMargins.UseRight)
            {
                paragraphFormat.RightIndent = logicalMargins.Right;
            }
            if (logicalMargins.UseTop)
            {
                ValidateAndSetSpaceBefore(paragraphFormat, System.Math.Max(logicalMargins.Top, 0));
            }
            if (logicalMargins.UseBottom)
            {
                ValidateAndSetSpaceAfter(paragraphFormat, System.Math.Max(logicalMargins.Bottom, 0));
            }
        }

        private static void ApplyMarginTop(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            double length = declarations.GetLength("margin-top");
            if (!MathUtil.IsMinValue(length))
            {
                ValidateAndSetSpaceBefore(paragraphFormat, length);
            }
        }

        private static void ApplyMarginRight(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            double length = declarations.GetLength("margin-right");
            if (!MathUtil.IsMinValue(length))
            {
                paragraphFormat.RightIndent = length;
            }
        }

        private static void ApplyMarginBottom(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            double length = declarations.GetLength("margin-bottom");
            if (!MathUtil.IsMinValue(length))
            {
                ValidateAndSetSpaceAfter(paragraphFormat, length);
            }
        }

        private static void ApplyMarginLeft(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            double length = declarations.GetLength("margin-left");
            if (!MathUtil.IsMinValue(length))
            {
                paragraphFormat.LeftIndent = length;
            }
        }

        private static void ValidateAndSetSpaceBefore(ParagraphFormat paragraphFormat, double value)
        {
            // WORDSNET-20282 Validate the new space value and if it's too large, use auto-spacing instead.
            // This mimics the MS Word's behavior.
            if (value <= ConvertUtilCore.MaxSizePoint)
            {
                paragraphFormat.SpaceBefore = value;
                paragraphFormat.SpaceBeforeAuto = false;
            }
            else
            {
                paragraphFormat.SpaceBeforeAuto = true;
            }
        }

        private static void ValidateAndSetSpaceAfter(ParagraphFormat paragraphFormat, double value)
        {
            // WORDSNET-20282 Validate the new space value and if it's too large, use auto-spacing instead.
            // This mimics the MS Word's behavior.
            if (value <= ConvertUtilCore.MaxSizePoint)
            {
                paragraphFormat.SpaceAfter = value;
                paragraphFormat.SpaceAfterAuto = false;
            }
            else
            {
                paragraphFormat.SpaceAfterAuto = true;
            }
        }

        private static void ApplyMsoHtmlFormatting(ParagraphFormat paragraphFormat, CssDeclarationCollection elementDeclarations)
        {
            if (elementDeclarations.ContainsIdentifier("mso-add-space", "auto"))
            {
                paragraphFormat.NoSpaceBetweenParagraphsOfSameStyle = true;
            }
        }
    }
}
