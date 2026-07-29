// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2017 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to paragraphs.
    /// </summary>
    internal abstract class ParagraphFormatter
    {
        internal void Format(ParagraphFormat paragraphFormat, CssStyleTracker cssStyleTracker)
        {
            // Text direction of the paragraph affect mapping of alignment and indentation, so it is important to determine
            // the direction before alignment and indentation are applied.
            if (cssStyleTracker.CurrentElementInfo.BlockLevelDirection != CssDirection.Unspecified)
            {
                paragraphFormat.Bidi = cssStyleTracker.CurrentElementInfo.BlockLevelDirection == CssDirection.Rtl;
            }

            CssDeclarationCollection declarations = cssStyleTracker.ElementDeclarations;
            CssBorderStyleConverter.ToParagraphFormat(declarations, paragraphFormat);
            CssTextAlignStyleConverter.ToParagraphFormat(declarations, paragraphFormat.Bidi, paragraphFormat);
            ApplyDeclarations(paragraphFormat, declarations);
            ApplyBackgroundColor(paragraphFormat, new CssBackgroundColor(cssStyleTracker.Background.GetParagraphBackgroundColor()));

            ApplySpecialFormatting(paragraphFormat, cssStyleTracker);
        }

        internal void Format(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            // Here we do no know the context in which the style will be used. We do not know parent and root elements.
            // Also, we do not know what will be the direction of elements to which the style will be applied, 
            // so we load style formatting for the default direction (LTR).
            CssBorderStyleConverter.ToParagraphFormat(declarations, paragraphFormat);
            CssTextAlignStyleConverter.ToParagraphFormat(declarations, false, paragraphFormat);
            ApplyDeclarations(paragraphFormat, declarations);
            ApplyBackgroundColor(paragraphFormat, new CssBackgroundColor(declarations));

            ApplySpecialFormatting(paragraphFormat, declarations);
        }

        [JavaAttributes.JavaThrows(true)]
        protected abstract void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssStyleTracker cssStyleTracker);

        [JavaAttributes.JavaThrows(true)]
        protected abstract void ApplySpecialFormatting(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations);

        protected static CssValue GetSingleValue(CssDeclarationCollection declarations, string propertyName)
        {
            CssDeclaration declaration = declarations[propertyName];
            if (declaration == null)
            {
                return null;
            }

            Debug.Assert(declaration.Value.Count == 1);
            return declaration.Value.FirstValue;
        }

        private static void ApplyDeclarations(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            ApplyPaddings(paragraphFormat, declarations);
            ApplyPageBreakInside(paragraphFormat, declarations);
            ApplyPageBreakBefore(paragraphFormat, declarations);
            ApplyPageBreakAfter(paragraphFormat, declarations);
            ApplyOrphans(paragraphFormat, declarations);
            ApplyWidows(paragraphFormat, declarations);
            ApplyWritingMode(paragraphFormat, declarations);
            ApplyTextIndent(paragraphFormat, declarations);
            ApplyVerticalAlign(paragraphFormat, declarations);
            // WORDSNET-22192 Preserve frame properties during roundtrip.
            ApplyFrameProperties(paragraphFormat, declarations);
        }

        private static void ApplyPaddings(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            ApplyPadding("padding-top", BorderType.Top, paragraphFormat, declarations);
            ApplyPadding("padding-right", BorderType.Right, paragraphFormat, declarations);
            ApplyPadding("padding-bottom", BorderType.Bottom, paragraphFormat, declarations);
            ApplyPadding("padding-left", BorderType.Left, paragraphFormat, declarations);
        }

        private static void ApplyPadding(
            string propertyName,
            BorderType borderType,
            ParagraphFormat paragraphFormat,
            CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, propertyName);
            if (cssValue == null)
            {
                return;
            }

            double length = CssUtil.LengthToPoint(cssValue);
            if (!MathUtil.AreEqual(length, double.MinValue))
            {
                Border border = paragraphFormat.Borders[borderType];
                border.SetDistanceFromTextSafe(length);
            }
        }

        private static void ApplyPageBreakInside(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "page-break-inside");
            if (cssValue == null)
            {
                return;
            }

            paragraphFormat.KeepTogether = cssValue.Equals(CssValue.Avoid);
        }

        private static void ApplyPageBreakBefore(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "page-break-before");
            if (cssValue == null)
            {
                return;
            }

            paragraphFormat.PageBreakBefore = cssValue.Equals(CssValue.Always);
        }

        private static void ApplyPageBreakAfter(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "page-break-after");
            if (cssValue == null)
            {
                return;
            }

            paragraphFormat.KeepWithNext = cssValue.Equals(CssValue.Avoid);
        }

        private static void ApplyOrphans(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "orphans");
            if (cssValue == null)
            {
                return;
            }

            if (cssValue.ValueType == CssValueType.Number)
            {
                paragraphFormat.WidowControl = cssValue.DoubleValue > 0;
            }
        }

        private static void ApplyWidows(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "widows");
            if (cssValue == null)
            {
                return;
            }

            if (cssValue.ValueType == CssValueType.Number)
            {
                paragraphFormat.WidowControl = cssValue.DoubleValue > 0;
            }
        }

        private static void ApplyWritingMode(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "writing-mode");
            if (cssValue == null)
            {
                return;
            }

            if (cssValue.ValueType == CssValueType.Identifier)
            {
                paragraphFormat.FrameTextOrientation = CssUtil.CssToTextOrientation(((CssIdentifierValue)cssValue).Value);
            }
        }

        private static void ApplyTextIndent(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            if (paragraphFormat.IsListItem)
            {
                return;
            }

            double textIndent = declarations.GetLength("text-indent");

            // WORDSNET-24212 Add support for percentage "text-indent" values.
            if (MathUtil.IsMinValue(textIndent))
            {
                double percentage = declarations.GetPercentage("text-indent");
                if (!MathUtil.IsMinValue(percentage))
                {
                    // Mimic MS Word's behavior and use a fixed base value to compute percentage.
                    textIndent = percentage * CssUtil.DefaultBlockWidth / 100;
                }
            }

            if (!MathUtil.IsMinValue(textIndent))
            {
                // Mimic MS Word's behavior for out-of-range values. Convert the result in twips to Int16 (short), possibly
                // causing an overflow.
                unchecked
                {
                    textIndent = ConvertUtilCore.TwipToPoint((short)ConvertUtilCore.PointToTwip(textIndent));
                }

                paragraphFormat.FirstLineIndent = textIndent;
            }
        }

        private static void ApplyBackgroundColor(ParagraphFormat paragraphFormat, CssBackgroundColor backgroundColor)
        {
            // Note that ParagraphFormat.Shading will create a Shading object and we want to avoid this
            // unless the background color is defined.
            if (backgroundColor.IsDefined)
            {
                backgroundColor.ToShading(paragraphFormat.Shading);
            }
        }

        private static void ApplyVerticalAlign(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssValue cssValue = GetSingleValue(declarations, "vertical-align");
            if (cssValue == null)
                return;

            if (cssValue.ValueType == CssValueType.Identifier)
            {
                paragraphFormat.BaselineAlignment =
                    CssUtil.CssToBaselineAlignment(((CssIdentifierValue)cssValue).Value);
            }
        }

        private static void ApplyFrameProperties(ParagraphFormat paragraphFormat, CssDeclarationCollection declarations)
        {
            CssFrameStyleConverter.ToParagraphFormat(declarations, paragraphFormat);
        }
    }
}
