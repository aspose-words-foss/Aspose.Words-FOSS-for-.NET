// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2023 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to document sections.
    /// Uses same formatting rules as MS Word.
    /// </summary>
    internal class SectionFormatterWordRules : SectionFormatter
    {
        internal SectionFormatterWordRules(
            Document document,
            CssStyleTracker cssStyleTracker)
            : base(document, cssStyleTracker)
        {
            // Empty constructor.
        }

        internal override void UpdateDefaultSectionProperties(Section section)
        {
            SectPr sectPr = section.SectPr;
            ClearSectionAttributes(sectPr);

            sectPr.SetDefaultPageSize();

            if (section.IsFirstSection)
            {
                sectPr[SectAttr.LinePitch] = 360;
            }

            int[] keys = new int[]
            {
                SectAttr.FooterDistance,
                SectAttr.HeaderDistance,
                SectAttr.Gutter,
                SectAttr.ColumnsSpacing,
                SectAttr.TopMargin,
                SectAttr.RightMargin,
                SectAttr.BottomMargin,
                SectAttr.LeftMargin
            };
            foreach (int key in keys)
            {
                sectPr[key] = SectPr.FetchDefaultAttr(key);
            }
        }

        protected override void ApplyPageSetupFormatting(
            Section section,
            CssDeclarationCollection pageDeclarations)
        {
            SectPr sectPr = section.SectPr;

            // The title page value is case-insensitive.
            if (pageDeclarations.ContainsIdentifier("mso-title-page", "yes"))
            {
                sectPr.DifferentFirstPageHeaderFooter = true;
            }

            // Max margin value allowed by MS Word. Note that although MS Word's document's model can store greater margin
            // values (up to 22 inches), when loading from MHTML, the max value is 15 inches.
            const double maxMarginInPoints = 1080;

            double headerMargin = pageDeclarations.GetLength("mso-header-margin");
            if (!MathUtil.IsMinValue(headerMargin))
            {
                sectPr.HeaderDistance = ConvertUtilCore.PointToTwip(MathUtil.FitToRange(headerMargin, 0, maxMarginInPoints));
            }
            double footerMargin = pageDeclarations.GetLength("mso-footer-margin");
            if (!MathUtil.IsMinValue(footerMargin))
            {
                sectPr.FooterDistance = ConvertUtilCore.PointToTwip(MathUtil.FitToRange(footerMargin, 0, maxMarginInPoints));
            }

            double gutterMargin = pageDeclarations.GetLength("mso-gutter-margin");
            if (!MathUtil.IsMinValue(gutterMargin))
            {
                sectPr.Gutter = ConvertUtilCore.PointToTwip(MathUtil.FitToRange(gutterMargin, 0, maxMarginInPoints));
            }

            section.PageSetup.OddAndEvenPagesHeaderFooter = pageDeclarations.ContainsIdentifier("mso-facing-pages", "yes");

            CssDeclaration sizeDeclaration = pageDeclarations["size"];
            if (sizeDeclaration != null)
            {
                if (sizeDeclaration.Value.Count == 1)
                {
                    CssValue value = sizeDeclaration.Value.FirstValue;

                    int size = ParsePageSize(value);
                    if (size > 0)
                    {
                        sectPr.PageHeight = size;
                        sectPr.PageWidth = size;
                    }
                    // Note that this comparison is case-insensitive.
                    else if (value.Equals(new CssIdentifierValue("landscape")))
                    {
                        sectPr.Orientation = Orientation.Landscape;

                        // Swap page width and height.
                        int width = sectPr.PageWidth;
                        sectPr.PageWidth = sectPr.PageHeight;
                        sectPr.PageHeight = width;
                    }
                }
                else if (sizeDeclaration.Value.Count == 2)
                {
                    int widthValue = ParsePageSize(sizeDeclaration.Value[0]);
                    int heightValue = ParsePageSize(sizeDeclaration.Value[1]);
                    if ((widthValue > 0) && (heightValue > 0))
                    {
                        sectPr.PageWidth = widthValue;
                        sectPr.PageHeight = heightValue;
                    }
                }
            }

            // Note that this comparison is case-insensitive.
            if (pageDeclarations.ContainsIdentifier("mso-page-orientation", "landscape"))
            {
                // Note that this declaration affects orientation only and doesn't swap page width and height.
                sectPr.Orientation = Orientation.Landscape;
            }

            ApplyTextColumnsFormatting(section.PageSetup.TextColumns, pageDeclarations);
        }

        private static void ApplyTextColumnsFormatting(
            TextColumnCollection textColumns,
            CssDeclarationCollection pageDeclarations)
        {
            Debug.Assert(pageDeclarations != null);

            CssDeclaration msoColumnsDeclaration = pageDeclarations["mso-columns"];
            if (msoColumnsDeclaration == null)
            {
                return;
            }

            int columnCount = GetColumnCount(msoColumnsDeclaration);
            if (columnCount == int.MinValue)
            {
                return;
            }

            textColumns.SetCount(columnCount);

            string evenlySpacedIdentifier = GetColumnEvenlySpacedIdentifier(msoColumnsDeclaration);
            if (!StringUtil.HasChars(evenlySpacedIdentifier))
            {
                return;
            }

            evenlySpacedIdentifier = evenlySpacedIdentifier.ToLowerInvariant();
            if ((evenlySpacedIdentifier != "even") &&
                (evenlySpacedIdentifier != "not-even"))
            {
                return;
            }

            switch (evenlySpacedIdentifier)
            {
                case "even":
                    // MS Word doesn't write 'w:equalWidth="1"' attribute for 'w:cols' element, but we need to set this
                    // property here, so AW model correctly handle this case.
                    textColumns.EvenlySpaced = true;
                    break;
                case "not-even":
                    textColumns.EvenlySpaced = false;
                    break;
                default:
                    Debug.Fail("Invalid value.");
                    return;
            }

            if (textColumns.EvenlySpaced)
            {
                double spacing = GetColumnSpacing(msoColumnsDeclaration, 2);
                if (MathUtil.IsMinValue(spacing))
                {
                    return;
                }

                textColumns.Spacing = spacing;
            }
            else
            {
                int valueIndex = 2;
                for (int i = 0; i < textColumns.Count; i++)
                {
                    TextColumn column = textColumns[i];

                    double width = GetColumnWidth(msoColumnsDeclaration, valueIndex);
                    if (MathUtil.IsMinValue(width))
                    {
                        return;
                    }

                    column.Width = width;
                    valueIndex++;

                    if (i < (columnCount - 1))
                    {
                        double spacing = GetColumnSpacing(msoColumnsDeclaration, valueIndex);
                        if (MathUtil.IsMinValue(spacing))
                        {
                            return;
                        }

                        column.SpaceAfter = spacing;
                        valueIndex++;
                    }
                }
            }

            // MS Word writes 'w:space="-1"' attribute for 'w:col' element for all columns except last two ones
            // regardless of the actual value.
            for (int i = 0; i < (textColumns.Count - 2); i++)
            {
                textColumns[i].RawSpaceAfter = -1;
            }

            CssDeclaration msoColumnSeparatorDeclaration = pageDeclarations["mso-column-separator"];
            if (msoColumnSeparatorDeclaration == null)
            {
                return;
            }

            string columnSeparatorIdentifier = GetColumnSeparatorIdentifier(msoColumnSeparatorDeclaration);
            if (!StringUtil.HasChars(evenlySpacedIdentifier))
            {
                return;
            }

            columnSeparatorIdentifier = columnSeparatorIdentifier.ToLowerInvariant();
            if ((columnSeparatorIdentifier != "none") &&
                (columnSeparatorIdentifier != "solid"))
            {
                return;
            }

            switch (columnSeparatorIdentifier)
            {
                case "none":
                    // MS Word doesn't write 'w:sep="0"' attribute for 'w:cols' element.
                    break;
                case "solid":
                    textColumns.LineBetween = true;
                    break;
                default:
                    Debug.Fail("Invalid value.");
                    return;
            }
        }

        protected override void ApplySpecialFormatting(Section section)
        {
            if (CssStyleTracker.ElementDeclarations["page"] == null)
            {
                // MS Word doesn't recognize DIV elements as sections if they have no 'page' declarations.
                return;
            }

            if (CssStyleTracker.ElementDeclarations.ContainsIdentifier("direction", "rtl"))
            {
                section.SectPr.Bidi = true;
            }
        }

        private static void ClearSectionAttributes(SectPr sectPr)
        {
            // DocumentBuilder creates a new section by cloning current section, but MS Word creates a new section
            // with default properties. So here we clear all section's attributes, but preserve SectAttr.SectionStart
            // attribute if any.
            object sectionStartAttribute = sectPr.GetDirectSectionAttr(SectAttr.SectionStart);

            sectPr.ClearSectionAttrs();

            if (sectionStartAttribute != null)
            {
                SectionStart sectionStart = (SectionStart)sectionStartAttribute;

                // MS Word doesn't write 'w:val="nextPage"' attribute for 'w:type' element.
                if (sectionStart != SectionStart.NewPage)
                {
                    sectPr.SectionStart = sectionStart;
                }
            }
        }

        private static int ParsePageSize(CssValue value)
        {
            int result = 0;
            if (value.IsLength(true))
            {
                double sizeInPoints = CssUtil.LengthToPoint(value);
                if (!MathUtil.IsMinValue(sizeInPoints))
                {
                    // Note that this conversion may overflow, but this happens in MS Word too.
                    int sizeInTwips = ConvertUtilCore.PointToTwip(sizeInPoints);
                    // Min page size that MS Word uses is 0.1 inches.
                    const int minSizeTwip = 144;
                    result = MathUtil.FitToRange(sizeInTwips, minSizeTwip, ConvertUtilCore.MaxSizeTwip);
                }
            }
            return result;
        }

        private static int GetColumnCount(CssDeclaration msoColumnsDeclaration)
        {
            if (msoColumnsDeclaration.Value.Count < 1)
            {
                return -1;
            }

            CssValue value = msoColumnsDeclaration.Value[0];
            if (value.ValueType != CssValueType.Number)
            {
                return int.MinValue;
            }

            CssNumberValue numberValue = (CssNumberValue)value;
            if (MathUtil.IsMinValue(numberValue.DoubleValue))
            {
                return int.MinValue;
            }

            return MathUtil.DoubleToInt(numberValue.DoubleValue);
        }

        private static string GetColumnEvenlySpacedIdentifier(CssDeclaration msoColumnsDeclaration)
        {
            if (msoColumnsDeclaration.Value.Count < 2)
            {
                return string.Empty;
            }

            CssValue value = msoColumnsDeclaration.Value[1];
            if (value.ValueType != CssValueType.Identifier)
            {
                return string.Empty;
            }

            return ((CssIdentifierValue)value).Value;
        }

        private static double GetColumnWidth(
            CssDeclaration msoColumnsDeclaration,
            int valueIndex)
        {
            if (msoColumnsDeclaration.Value.Count < (valueIndex + 1))
            {
                return double.MinValue;
            }

            CssValue value = msoColumnsDeclaration.Value[valueIndex];
            if (value.ValueType != CssValueType.Length)
            {
                return double.MinValue;
            }

            CssLengthValue widthValue = (CssLengthValue)value;
            return widthValue.GetLength(CssUnit.Pt);
        }

        private static double GetColumnSpacing(
            CssDeclaration msoColumnsDeclaration,
            int valueIndex)
        {
            if (msoColumnsDeclaration.Value.Count < (valueIndex + 1))
            {
                return double.MinValue;
            }

            CssValue value = msoColumnsDeclaration.Value[valueIndex];
            if (value.ValueType != CssValueType.Length)
            {
                return double.MinValue;
            }

            CssLengthValue spacingValue = (CssLengthValue)value;
            return spacingValue.GetLength(CssUnit.Pt);
        }

        private static string GetColumnSeparatorIdentifier(CssDeclaration msoColumnSeparatorDeclaration)
        {
            if (!msoColumnSeparatorDeclaration.HasSingleValueOfType(CssValueType.Identifier))
            {
                return string.Empty;
            }

            CssValue value = msoColumnSeparatorDeclaration.Value.FirstValue;
            return ((CssIdentifierValue)value).Value;
        }
    }
}
