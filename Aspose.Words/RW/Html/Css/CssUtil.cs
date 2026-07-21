// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2013 by Alexey Butalov

using System.Text.RegularExpressions;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Utility functions to deal with CSS.
    /// </summary>
    internal static class CssUtil
    {
        /// <summary>
        /// Gets text direction of an HTML element from its CSS declarations.
        /// </summary>
        internal static CssDirection GetDirection(CssDeclarationCollection declarations)
        {
            CssDirection result = CssDirection.Unspecified;
            if (declarations != null)
            {
                CssDeclaration directionDeclaration = declarations["direction"];
                if (directionDeclaration != null)
                {
                    if (directionDeclaration.Value.Equals(CssValue.Ltr))
                    {
                        result = CssDirection.Ltr;
                    }
                    else if (directionDeclaration.Value.Equals(CssValue.Rtl))
                    {
                        result = CssDirection.Rtl;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Replaces '-aw-start' with either 'left' or 'right' according to the element's 'direction'.
        /// </summary>
        internal static CssPropertyValue GetEffectiveAwStartValue(CssDirection direction)
        {
            CssValue result = (direction == CssDirection.Rtl)
                ? CssValue.Right
                : CssValue.Left;
            return new CssPropertyValue(new CssValueList(result));
        }

        /// <summary>
        /// Gets the 'unicode-bidi' property value of an HTML element.
        /// </summary>
        internal static CssUnicodeBidi GetUnicodeBidi(CssDeclarationCollection declarations, bool isBlockLevelElement)
        {
            CssUnicodeBidi result = CssUnicodeBidi.Normal;
            if (!isBlockLevelElement && (declarations != null))
            {
                CssDeclaration directionDeclaration = declarations["unicode-bidi"];
                if ((directionDeclaration != null) &&
                    directionDeclaration.Value.Equals(CssValue.Embed))
                {
                    result = CssUnicodeBidi.Embed;
                }
            }
            return result;
        }

        /// <summary>
        /// Converts <see cref="HtmlDocumentMode"/> to <see cref="CssDocumentMode"/>
        /// </summary>
        internal static CssDocumentMode HtmlDocumentModeToCssMode(HtmlDocumentMode documentMode)
        {
            switch (documentMode)
            {
                case HtmlDocumentMode.Standards:
                    return CssDocumentMode.Standards;
                case HtmlDocumentMode.LimitedQuirks:
                    return CssDocumentMode.LimitedQuirks;
                case HtmlDocumentMode.Quirks:
                    return CssDocumentMode.Quirks;
                default:
                    Debug.Assert(false);
                    return CssDocumentMode.Standards;
            }
        }

        /// <summary>
        /// Computes length value measured in points for CSS length value.
        /// </summary>
        /// <param name="lengthValue">CSS length value.</param>
        /// <returns>Length value measured in points; double.MinValue if the CSS value is invalid.</returns>
        internal static double LengthToPoint(CssValue lengthValue)
        {
            double length = AbsoluteLengthToPoint(lengthValue);
            if (MathUtil.IsMinValue(length))
                length = RelativeLengthToPoint(lengthValue, null, DefaultFontSize);

            return length;
        }

        /// <summary>
        /// Computes length value measured in points for CSS length value.
        /// </summary>
        /// <param name="lengthValue">CSS length value.</param>
        /// <returns>Length value measured in points; double.MinValue if the CSS value is invalid.</returns>
        internal static double LengthToPoint(CssPropertyValue lengthValue)
        {
            return (lengthValue.Count == 1)
                       ? LengthToPoint(lengthValue.FirstValue)
                       : double.MinValue;
        }

        /// <summary>
        /// Computes length value measured in points for relative CSS length value.
        /// </summary>
        /// <param name="lengthValue">Relative CSS length value.</param>
        /// <param name="rootElementDeclarations">CSS declarations of the root element. Can be null.</param>
        /// <param name="elementFontSize">'font-size' of the current CSS element, in points.</param>
        /// <returns>Length value measured in points; double.MinValue if the CSS value is absolute or invalid.</returns>
        internal static double RelativeLengthToPoint(
            CssValue lengthValue,
            CssDeclarationCollection rootElementDeclarations,
            double elementFontSize)
        {
            Debug.Assert(elementFontSize >= 0);

            double length = double.MinValue;

            if (lengthValue.ValueType == CssValueType.Length)
            {
                CssLengthValue cssLength = (CssLengthValue)lengthValue;
                switch (cssLength.Unit)
                {
                    case CssUnit.Em:
                    {
                        length = elementFontSize * cssLength.DoubleValue;
                        break;
                    }
                    case CssUnit.Ex:
                    {
                        // 1ex is equivalent to the height of the letter ‘x’ in the current font. In the cases where it is impossible or
                        // impractical to determine the x-height, a value of 0.5em must be assumed.
                        length = elementFontSize * cssLength.DoubleValue * 0.5;
                        break;
                    }
                    case CssUnit.Rem:
                    {
                        double rootFontSize = ComputeFontSize(rootElementDeclarations);
                        length = rootFontSize * cssLength.DoubleValue;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }

            return length;
        }

        /// <summary>
        /// Gets Word line width measured in points from CSS line width.
        /// </summary>
        /// <param name="propertyValue">Property value.</param>
        /// <returns>Line width measured in points; zero, if the property value is invalid.</returns>
        internal static double GetBorderLineWidth(CssPropertyValue propertyValue)
        {
            Debug.Assert(propertyValue != null);

            double borderWidth;
            if (propertyValue.Equals(CssValue.Thin))
            {
                borderWidth = 1.5; // Looks like IE uses this width.
            }
            else if (propertyValue.Equals(CssValue.Medium))
            {
                borderWidth = 3.0; // Looks like IE uses this width.
            }
            else if (propertyValue.Equals(CssValue.Thick))
            {
                borderWidth = 4.5; // Looks like IE uses this width.
            }
            else
            {
                double length = LengthToPoint(propertyValue);
                borderWidth = (!MathUtil.IsMinValue(length)) ? length : 0;
            }

            return borderWidth;
        }

        /// <summary>
        /// Computes length value measured in points for absolute CSS length value.
        /// </summary>
        /// <param name="lengthValue">Absolute CSS length value.</param>
        /// <returns>Length value measured in points; double.MinValue if the CSS value is relative or invalid.</returns>
        private static double AbsoluteLengthToPoint(CssValue lengthValue)
        {
            double length = double.MinValue;

            switch (lengthValue.ValueType)
            {
                case CssValueType.Length:
                {
                    CssLengthValue cssLength = (CssLengthValue)lengthValue;
                    switch (cssLength.Unit)
                    {
                        case CssUnit.Em:
                        case CssUnit.Ex:
                        case CssUnit.Rem:
                            break;
                        case CssUnit.None:
                        case CssUnit.In:
                        case CssUnit.Cm:
                        case CssUnit.Mm:
                        case CssUnit.Pt:
                        case CssUnit.Pc:
                        case CssUnit.Px:
                            length = cssLength.GetLength(CssUnit.Pt);
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                    break;
                }
                case CssValueType.Number:
                {
                    length = ((CssNumberValue)lengthValue).QuirkyLengthToPoint();
                    break;
                }
                default:
                {
                    // It is an error if a length value is neither a CSS length nor a number. We just ignore such values.
                    break;
                }
            }

            return length;
        }

        /// <summary>
        /// Finds a 'font-size' CSS declaration in the CSS declarations and computes absolute font size value measured in points for this declaration.
        /// </summary>
        /// <param name="cssDeclarations">CSS declarations. Can be null.</param>
        /// <returns>Font size measured in points; initial font size value if the declarations argument is null, or if a font-size declaration isn't found,
        /// or if CSS value is relative or invalid.</returns>
        internal static double ComputeFontSize(CssDeclarationCollection cssDeclarations)
        {
            double fontSize = double.MinValue;
            if (cssDeclarations != null)
            {
                CssDeclaration parentFontSizeDeclaration = cssDeclarations["font-size"];
                if ((parentFontSizeDeclaration != null) && (parentFontSizeDeclaration.Value.Count == 1))
                    fontSize = AbsoluteLengthToPoint(parentFontSizeDeclaration.Value.FirstValue);
            }

            if (MathUtil.IsMinValue(fontSize))
            {
                fontSize = DefaultFontSize;
            }

            return fontSize;
        }

        /// <summary>
        /// Gets a value indicating whether the declarations specify italic font face.
        /// </summary>
        /// <returns>
        /// <see cref="NullableBool.NotDefined"/> if "font-style" is not present in the declarations.
        /// </returns>
        internal static NullableBool IsItalicFont(
            CssDeclarationCollection declarations,
            string propertyName)
        {
            CssDeclaration declaration = declarations[propertyName];
            if (declaration == null)
            {
                return NullableBool.NotDefined;
            }

            bool isItalic = declaration.Value.Equals(CssValue.Italic) || declaration.Value.Equals(CssValue.Oblique);
            return NullableBoolUtil.AsNullable(isItalic);
        }

        /// <summary>
        /// Gets a value indicating whether the declarations specify bold font face.
        /// </summary>
        /// <returns>
        /// <see cref="NullableBool.NotDefined"/> if "font-weight" is not specified.
        /// </returns>
        internal static NullableBool IsBoldFont(
            CssDeclarationCollection declarations,
            string propertyName)
        {
            CssDeclaration declaration = declarations[propertyName];
            if ((declaration == null) || (declaration.Value.Count != 1))
            {
                return NullableBool.NotDefined;
            }

            bool isBold;
            CssValue cssValue = declaration.Value.FirstValue;
            switch (cssValue.ValueType)
            {
                case CssValueType.Identifier:
                    isBold = cssValue.Equals(CssValue.Bold) || cssValue.Equals(CssValue.Bolder);
                    break;
                case CssValueType.Number:
                    isBold = cssValue.DoubleValue >= 600;
                    break;
                default:
                    return NullableBool.NotDefined;
            }
            return NullableBoolUtil.AsNullable(isBold);
        }

        /// <summary>
        /// Splits class attribute value to separate values.
        /// </summary>
        /// <param name="classAttribValue">Class attribute value.</param>
        /// <returns>String array of separate parts.</returns>
        internal static string[] SplitClassAttributeValue(string classAttribValue)
        {
            MatchCollection matches = gClassesRegex.Matches(classAttribValue);
            string[] result = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                result[i] = matches[i].Value;
            }
            return result;
        }

        /// <summary>
        /// Returns CSS block flow direction for a specified paragraph.
        /// </summary>
        internal static CssBlockFlowDirection GetParaCssBlockFlowDirection(Paragraph para)
        {
            return (para.ParentSection != null)
                       ? TextFlowToCssBlockFlowDirection(para.ParentSection.PageSetup.TextFlow)
                       : CssBlockFlowDirection.HorizontalTb;
        }

        /// <summary>
        /// Converts specified Word text flow to a CSS block flow direction.
        /// </summary>
        internal static CssBlockFlowDirection TextFlowToCssBlockFlowDirection(TextFlow textFlow)
        {
            switch (textFlow)
            {
                case TextFlow.Horizontal:
                case TextFlow.LrBt:
                case TextFlow.RlTb:
                case TextFlow.HorizontalRotatedFarEast:
                {
                    return CssBlockFlowDirection.HorizontalTb;
                }
                case TextFlow.Vertical:
                case TextFlow.VNonV:
                {
                    return CssBlockFlowDirection.VerticalRl;
                }
                default:
                {
                    Debug.Assert(false, "Unknown value!");
                    return CssBlockFlowDirection.HorizontalTb;
                }
            }
        }

        /// <summary>
        /// Converts specified a CSS block flow direction to Word text flow.
        /// </summary>
        internal static TextFlow CssBlockFlowDirectionToTextFlow(CssBlockFlowDirection blockFlowDirection)
        {
            switch (blockFlowDirection)
            {
                case CssBlockFlowDirection.HorizontalTb:
                    return TextFlow.Horizontal;
                case CssBlockFlowDirection.VerticalRl:
                case CssBlockFlowDirection.VerticalLr:
                    return TextFlow.Vertical;
                default:
                    Debug.Assert(false, "Unknown value of block flow direction");
                    return TextFlow.Horizontal;
            }
        }

        /// <summary>
        /// Converts a specified block flow direction to standard CSS writing mode.
        /// </summary>
        internal static string CssBlockFlowDirectionToWritingMode(CssBlockFlowDirection blockFlowDirection)
        {
            switch (blockFlowDirection)
            {
                case CssBlockFlowDirection.HorizontalTb:
                    return "horizontal-tb";
                case CssBlockFlowDirection.VerticalRl:
                    return "vertical-rl";
                case CssBlockFlowDirection.VerticalLr:
                    return "vertical-lr";
                default:
                    Debug.Assert(false, "Unknown text flow direction value!");
                    return "horizontal-tb";
            }
        }

        /// <summary>
        /// Converts a specified CSS writing mode to block flow direction.
        /// </summary>
        internal static CssBlockFlowDirection CssWritingModeToBlockFlowDirection(string writingMode)
        {
            switch (writingMode)
            {
                case "horizontal-tb":
                    return CssBlockFlowDirection.HorizontalTb;
                case "vertical-rl":
                    return CssBlockFlowDirection.VerticalRl;
                case "vertical-lr":
                    return CssBlockFlowDirection.VerticalLr;
                default:
                    return CssBlockFlowDirection.HorizontalTb;
            }
        }

        /// <summary>
        /// Converts a specified block flow direction to MS (used in IE) CSS writing mode.
        /// </summary>
        internal static string CssBlockFlowDirectionToMSWritingMode(CssBlockFlowDirection blockFlowDirection)
        {
            switch (blockFlowDirection)
            {
                case CssBlockFlowDirection.HorizontalTb:
                    return "lr-tb";
                case CssBlockFlowDirection.VerticalRl:
                    return "tb-rl";
                case CssBlockFlowDirection.VerticalLr:
                    return "tb-lr";
                default:
                    Debug.Assert(false, "Unknown text flow direction value!");
                    return "lr-tb";
            }
        }

        /// <summary>
        /// This article is with info about CSS writing-mode attribute.
        /// http://msdn.microsoft.com/library/default.asp?url=/workshop/author/dhtml/reference/properties/layoutflow.asp
        /// We can only do some orientations, not all as HTML does not match MS Word well.
        /// </summary>
        internal static string TextOrientationToCss(TextOrientation orientation)
        {
            switch (orientation)
            {
                case TextOrientation.Downward:
                case TextOrientation.Upward:
                case TextOrientation.VerticalFarEast:
                    return "tb-rl";
                default:
                    return "lr-tb";
            }
        }

        internal static TextOrientation CssToTextOrientation(string value)
        {
            return StringUtil.EqualsIgnoreCase(value, "tb-rl")
                ? TextOrientation.VerticalFarEast
                : TextOrientation.Horizontal;
        }

        /// <summary>
        /// Generates properties of a section break. Here we need to output "magic" mso-break-type and mso-column-break-before attribute
        /// so MS Word will consider this break a section break.
        /// For the first section MS Word 2007 doesn't roundtrip section start type. So I won't either. This can be saved
        /// in native Word formats but not in HTML.
        /// </summary>
        internal static void SectionBreakPropsToCss(SectionStart secStart, CssStyle cssStyle)
        {
            switch (secStart)
            {
                case SectionStart.Continuous:
                    cssStyle.SetIdentifier("page-break-before", "auto");
                    break;
                case SectionStart.NewColumn:
                    cssStyle.SetIdentifier("mso-column-break-before", "always");
                    break;
                case SectionStart.EvenPage:
                    cssStyle.SetIdentifier("page-break-before", "left");
                    break;
                case SectionStart.OddPage:
                    cssStyle.SetIdentifier("page-break-before", "right");
                    break;
                default:
                    // SectionStart.NewPage is the default
                    cssStyle.SetIdentifier("page-break-before", "always");
                    break;
            }

            cssStyle.SetIdentifier("mso-break-type", "section-break");
            cssStyle.SetIdentifier("clear", "both");
        }

        /// <summary>
        /// Converts page setup to CSS in a @page at-rule
        /// </summary>
        internal static void PageSetupToCssPageAtRule(PageSetup ps, CssStyle cssStyle)
        {
            cssStyle.SetPageSize(ps.PageWidth, ps.PageHeight, CssUnit.Pt);
            cssStyle.SetMargin(ps.TopMargin, ps.RightMargin, ps.BottomMargin, ps.LeftMargin);
        }

        internal static BaselineAlignment CssToBaselineAlignment(string aligmentValue)
        {
            switch (aligmentValue.ToLowerInvariant())
            {
                case "middle":
                    return BaselineAlignment.Center;
                case "top":
                    return BaselineAlignment.Top;
                case "bottom":
                    return BaselineAlignment.Bottom;
                default:
                    return BaselineAlignment.Baseline;
            }
        }

        internal static CssValue BaselineAlignmentToCss(BaselineAlignment alignment)
        {
            switch (alignment)
            {
                case BaselineAlignment.Center:
                    return CssValue.Middle;
                case BaselineAlignment.Top:
                    return CssValue.Top;
                case BaselineAlignment.Bottom:
                    return CssValue.Bottom;
                default:
                    return CssValue.Baseline;
            }
        }

        /// <summary>
        /// Indicates whether the HTML element creates a <see cref="HtmlBlock"/> instance in the document model.
        /// </summary>
        internal static bool CreatesHtmlBlock(
            string elementName,
            CssDeclarationCollection elementDeclarations,
            bool applyFormattingAsMsWord)
        {
            switch (elementName)
            {
                case "body":
                    // The HTML body element creates HTML blocks only if it has appropriate declarations
                    // or it is imported from the alt-chunk document.
                    return applyFormattingAsMsWord || HasHtmlBlockProperties(elementDeclarations);
                case "blockquote":
                    // These elements always create HTML blocks.
                    return true;
                case "div":
                    // In MS Word, div elements that specify document sections don't create HTML blocks.
                    // Neither do div elements that represent headers and footers.
                    return !applyFormattingAsMsWord ||
                        ((elementDeclarations["page"] == null) && (elementDeclarations["mso-element"] == null));
                default:
                    return false;
            }
        }

        /// <summary>
        /// Default font size in points, is equal to 'medium' value.
        /// </summary>
        internal const double DefaultFontSize = 12;

        /// <summary>
        /// Default block width (in points) used by MS Word as a base when computing horizontal widths specified in CSS
        /// as percentage.
        /// </summary>
        /// <remarks>
        /// According to the CSS specification, horizontal values of properties like "text-indent" that are specified
        /// as percentage refer to width of the containing block. MS Word doesn't follow that rule and always uses a fixed
        /// base value when computing percentage.
        /// </remarks>
        internal const int DefaultBlockWidth = 612; // 8.5 inches, width of the Letter paper size.

        /// <summary>
        /// Returns a value indicating whether any of CSS declarations are applicable to <see cref="HtmlBlock"/> nodes.
        /// </summary>
        private static bool HasHtmlBlockProperties(CssDeclarationCollection declarations)
        {
            BorderType[] borderTypes = new BorderType[]
            {
                BorderType.Top,
                BorderType.Right,
                BorderType.Bottom,
                BorderType.Left
            };
            foreach (BorderType borderType in borderTypes)
            {
                CssBorder cssBorder = CssBorder.CreateBorder(declarations, borderType, false);
                if ((cssBorder != null) &&
                    (cssBorder.LineStyle != LineStyle.None) &&
                    (cssBorder.CssLineWidth > 0))
                {
                    return true;
                }
            }

            string[] marginProperties = new string[]
            {
                "margin-left",
                "margin-right",
                "margin-top",
                "margin-bottom",
            };
            foreach (string property in marginProperties)
            {
                if (declarations.GetIdentifier(property) == "auto")
                    return true;

                if (IsValidMarginValue(declarations.GetLength(property)) ||
                    IsValidMarginValue(declarations.GetPercentage(property)))
                {
                    return true;
                }
            }

            string[] colorProperties = new string[]
            {
                "border-left-color",
                "border-right-color",
                "border-top-color",
                "border-bottom-color"
            };
            foreach (string property in colorProperties)
            {
                if (declarations.GetColor(property) != null)
                    return true;
            }

            return false;
        }

        private static bool IsValidMarginValue(double marginValue)
        {
            return !MathUtil.IsMinValue(marginValue) && !MathUtil.IsZero(marginValue);
        }

        private static readonly Regex gClassesRegex = new Regex(@"[^ \t\r\n\f]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
}
