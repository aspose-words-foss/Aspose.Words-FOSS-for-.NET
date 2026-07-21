// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2014 by Alexey Butalov

using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS border object.
    /// </summary>
    /// <remarks>
    /// This class is immutable (read-only), don't change this behaviour.
    /// </remarks>
    internal class CssBorder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private CssBorder(bool isAwBorder)
        {
            mLineStyle = LineStyle.None;
            mCssLineStyle = string.Empty;
            mLineWidth = 0;
            mCssLineWidth = 0;
            mColorInternal = DrColor.Empty;
            mIsHidden = false;
            mIsUndefined = true;
            IsNil = false;
            IsAwBorder = isAwBorder;
        }

        /// <summary>
        /// Creates a CSS border object from CSS declarations.
        /// </summary>
        internal static CssBorder CreateBorder(
            CssDeclarationCollection declarations,
            BorderType borderType,
            bool invertInsetOutsetLineStyle)
        {
            return CreateBorder(declarations, borderType, false, invertInsetOutsetLineStyle);
        }

        /// <summary>
        /// Creates a CSS border object from CSS declarations from the HTML block.
        /// </summary>
        internal static CssBorder CreateHtmlBlockBorder(
            CssDeclarationCollection declarations,
            BorderType borderType)
        {
            // MS Word preserves the line style for the HTML block if the border width is zero.
            return CreateBorder(declarations, borderType, true, false);
        }

        /// <summary>
        /// Returns a border width value from CSS declarations.
        /// </summary>
        /// <remarks>
        /// This method takes into account that the minimum border width value that's used to calculate padding values during
        /// export to HTML is 0.75pt.
        /// </remarks>
        internal static double GetCssBorderWidth(CssDeclarationCollection declarations, BorderType borderType)
        {
            string styleProperty;
            string widthProperty;
            string colorProperty;
            switch (borderType)
            {
                case BorderType.Top:
                    styleProperty = "border-top-style";
                    widthProperty = "border-top-width";
                    colorProperty = "border-top-color";
                    break;
                case BorderType.Right:
                    styleProperty = "border-right-style";
                    widthProperty = "border-right-width";
                    colorProperty = "border-right-color";
                    break;
                case BorderType.Bottom:
                    styleProperty = "border-bottom-style";
                    widthProperty = "border-bottom-width";
                    colorProperty = "border-bottom-color";
                    break;
                case BorderType.Left:
                    styleProperty = "border-left-style";
                    widthProperty = "border-left-width";
                    colorProperty = "border-left-color";
                    break;
                default:
                    Debug.Assert(false, "Unsupported border type!");
                    return 0;
            }

            CssBorder cssBorder = CreateBorder(
                declarations,
                styleProperty,
                widthProperty,
                colorProperty,
                null,
                null,
                null,
                false,
                false);
            return (cssBorder.LineWidth > 0)
                ? cssBorder.LineWidth
                : 0;
        }

        /// <summary>
        /// Creates a CSS border object from CSS declarations in case all four border sides are specified.
        /// </summary>
        internal static CssBorder CreateBoxBorder(CssDeclarationCollection declarations)
        {
            CssBorder leftBorder = CreateBorder(declarations, BorderType.Left, false);
            CssBorder rightBorder = CreateBorder(declarations, BorderType.Right, false);
            CssBorder topBorder = CreateBorder(declarations, BorderType.Top, false);
            CssBorder bottomBorder = CreateBorder(declarations, BorderType.Bottom, false);

            bool allBordersSpecified =
                (!leftBorder.IsUndefined) &&
                (!rightBorder.IsUndefined) &&
                (!topBorder.IsUndefined) &&
                (!bottomBorder.IsUndefined);

            if (allBordersSpecified)
            {
                bool allBordersAreOfSameStyle =
                    (leftBorder.LineStyle == topBorder.LineStyle) &&
                    (rightBorder.LineStyle == topBorder.LineStyle) &&
                    (bottomBorder.LineStyle == topBorder.LineStyle);
                if (allBordersAreOfSameStyle)
                {
                    return topBorder;
                }
            }

            return Empty;
        }

        internal static CssBorder CreateInsideBorder(CssDeclarationCollection declarations, BorderType borderType)
        {
            string styleProperty;
            string widthProperty;
            string colorProperty;
            switch (borderType)
            {
                case BorderType.Horizontal:
                    styleProperty = HtmlConstants.AsposeBorderInsideHStyle;
                    widthProperty = HtmlConstants.AsposeBorderInsideHWidth;
                    colorProperty = HtmlConstants.AsposeBorderInsideHColor;
                    break;
                case BorderType.Vertical:
                    styleProperty = HtmlConstants.AsposeBorderInsideVStyle;
                    widthProperty = HtmlConstants.AsposeBorderInsideVWidth;
                    colorProperty = HtmlConstants.AsposeBorderInsideVColor;
                    break;
                default:
                    Debug.Assert(false, "Unsupported border type!");
                    return null;
            }
            return CreateBorder(
                declarations,
                null,
                null,
                null,
                styleProperty,
                widthProperty,
                colorProperty,
                false,
                false);
        }

        /// <summary>
        /// Represents an empty CSS border object.
        /// </summary>
        internal static CssBorder Empty
        {
            get { return gEmpty; }
        }

        /// <summary>
        /// Checks the border wins an another border in the border conflict.
        /// </summary>
        /// <remarks>
        /// This comparison algorithm is based on rules 1-3 of Border Conflict Resolution specification
        /// (http://www.w3.org/TR/CSS21/tables.html#border-conflict-resolution).
        /// </remarks>
        /// <param name="other">A <see cref="CssBorder"/> instance.</param>
        /// <returns>A signed number indicating whether the border wins.
        /// Less than zero - this border loses.
        /// Zero - there is no winner (border styles differ only in color).
        /// Greater than zero - this border wins.</returns>
        internal int Wins(CssBorder other)
        {
            // Rule 1: Borders with the 'border-style' of 'hidden' take precedence over all other conflicting borders.
            // Any border with this value suppresses all borders at this location.
            if (mIsHidden && !other.IsHidden)
                return 1;
            if (mIsHidden && other.IsHidden)
                return 0;
            if (!mIsHidden && other.IsHidden)
                return -1;

            // Rule 2: Borders with a style of 'none' have the lowest priority. Only if the border properties of all the elements meeting at this
            // edge are 'none' will the border be omitted (but note that 'none' is the default value for the border style.)
            if (!IsNone && other.IsNone)
                return 1;
            if (IsNone && other.IsNone)
                return 0;
            if (IsNone && !other.IsNone)
                return -1;

            // Rule 3: If none of the styles are 'hidden' and at least one of them is not 'none', then narrow borders are discarded in favor of wider ones.
            // If several have the same 'border-width' then styles are preferred in this order: 'double', 'solid', 'dashed', 'dotted', 'ridge', 'outset', 'groove',
            // and the lowest: 'inset'.
            if (mCssLineWidth > other.CssLineWidth)
                return 1;
            if (mCssLineWidth < other.CssLineWidth)
                return -1;
            if (mLineStyle != other.LineStyle)
            {
                int stylePriority = GetStylePriority(mCssLineStyle);
                int otherStylePriority = GetStylePriority(other.CssLineStyle);
                if (stylePriority != otherStylePriority)
                    return stylePriority.CompareTo(otherStylePriority);
            }

            // Rule 4 cannot be implemented in this class therefore we return zero.
            return 0;
        }

        /// <summary>
        /// Applies CSS border style, width and color styles to a model border.
        /// </summary>
        internal void ToModelBorder(Border modelBorder)
        {
            modelBorder.LineStyleInternal = mLineStyle;
            modelBorder.SetLineWidthSafe(mLineWidth);
            modelBorder.ColorInternal = mColorInternal;
            modelBorder.IsNil = IsNil;
        }

        /// <summary>
        /// Applies CSS border style, width and color to a <see cref="Stroke"/> instance.
        /// </summary>
        internal void ToStroke(Stroke stroke)
        {
            if (mIsHidden)
            {
                stroke.On = false;
                return;
            }

            switch (mCssLineStyle)
            {
                case "solid":
                case "groove":
                case "ridge":
                case "inset":
                case "outset":
                    // There are no stroke counterparts for "3D" border styles like "groove" or "inset", so we import such
                    // borders as solid-color strokes.
                    stroke.LineStyle = ShapeLineStyle.Single;
                    stroke.DashStyle = DashStyle.Solid;
                    break;
                case "double":
                    stroke.LineStyle = ShapeLineStyle.Double;
                    stroke.DashStyle = DashStyle.Solid;
                    break;
                case "dotted":
                    stroke.LineStyle = ShapeLineStyle.Single;
                    stroke.DashStyle = DashStyle.ShortDot;
                    break;
                case "dashed":
                    stroke.LineStyle = ShapeLineStyle.Single;
                    stroke.DashStyle = DashStyle.ShortDash;
                    break;
                default:
                    // Unknown line styles are not supported.
                    stroke.On = false;
                    return;
            }

            stroke.On = true;
            stroke.Weight = mCssLineWidth;
            stroke.ColorInternal = mColorInternal;
            stroke.JoinStyle = JoinStyle.Miter;
        }

        private static CssBorder CreateBorder(
            CssDeclarationCollection declarations,
            BorderType borderType,
            bool preserveZeroWidthBorderStyle,
            bool invertInsetOutsetLineStyle)
        {
            CssBorder cssBorder = null;
            switch (borderType)
            {
                case BorderType.Top:
                    cssBorder = CreateBorder(
                        declarations,
                        "border-top-style",
                        "border-top-width",
                        "border-top-color",
                        HtmlConstants.AsposeBorderTopStyle,
                        HtmlConstants.AsposeBorderTopWidth,
                        HtmlConstants.AsposeBorderTopColor,
                        preserveZeroWidthBorderStyle,
                        invertInsetOutsetLineStyle);
                    break;
                case BorderType.Right:
                    cssBorder = CreateBorder(
                        declarations,
                        "border-right-style",
                        "border-right-width",
                        "border-right-color",
                        HtmlConstants.AsposeBorderRightStyle,
                        HtmlConstants.AsposeBorderRightWidth,
                        HtmlConstants.AsposeBorderRightColor,
                        preserveZeroWidthBorderStyle,
                        invertInsetOutsetLineStyle);
                    break;
                case BorderType.Bottom:
                    cssBorder = CreateBorder(
                        declarations,
                        "border-bottom-style",
                        "border-bottom-width",
                        "border-bottom-color",
                        HtmlConstants.AsposeBorderBottomStyle,
                        HtmlConstants.AsposeBorderBottomWidth,
                        HtmlConstants.AsposeBorderBottomColor,
                        preserveZeroWidthBorderStyle,
                        invertInsetOutsetLineStyle);
                    break;
                case BorderType.Left:
                    cssBorder = CreateBorder(
                        declarations,
                        "border-left-style",
                        "border-left-width",
                        "border-left-color",
                        HtmlConstants.AsposeBorderLeftStyle,
                        HtmlConstants.AsposeBorderLeftWidth,
                        HtmlConstants.AsposeBorderLeftColor,
                        preserveZeroWidthBorderStyle,
                        invertInsetOutsetLineStyle);
                    break;
                default:
                    Debug.Assert(false, "Unsupported border type!");
                    break;
            }

            return cssBorder;
        }

        /// <summary>
        /// Gets priority of the specified CSS line style.
        /// </summary>
        /// <param name="lineStyleName">CSS line style.</param>
        /// <returns>A unsigned number indicating priority of the line style. Larger value corresponds to higher priority.</returns>
        private static int GetStylePriority(string lineStyleName)
        {
            // If several have the same 'border-width' then styles are preferred in this order: 'double', 'solid', 'dashed', 'dotted',
            // 'ridge', 'outset', 'groove', and the lowest: 'inset'.
            switch (lineStyleName.ToLowerInvariant())
            {
                case "double":
                    return 10;
                case "solid":
                    return 9;
                case "dashed":
                    return 8;
                case "dotted":
                    return 7;
                case "ridge":
                    return 6;
                case "outset":
                    return 5;
                case "groove":
                    return 4;
                case "inset":
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Creates a CSS border object from CSS declarations.
        /// </summary>
        private static CssBorder CreateBorder(
            CssDeclarationCollection declarations,
            string stylePropertyName,
            string widthPropertyName,
            string colorPropertyName,
            string awBorderStyleName,
            string awBorderWidthName,
            string awBorderColorName,
            bool preserveZeroWidthBorderStyle,
            bool invertInsetOutsetLineStyle)
        {
            CssDeclaration borderStyleDeclaration = StringUtil.HasChars(stylePropertyName)
                ? declarations[stylePropertyName]
                : null;
            CssDeclaration awBorderStyleDeclaration = StringUtil.HasChars(awBorderStyleName)
                ? declarations[awBorderStyleName]
                : null;

            if ((borderStyleDeclaration == null) && (awBorderStyleDeclaration == null))
                return Empty;

            Debug.Assert(StringUtil.HasChars(widthPropertyName) || StringUtil.HasChars(awBorderWidthName));
            Debug.Assert(StringUtil.HasChars(colorPropertyName) || StringUtil.HasChars(awBorderColorName));

            CssDeclaration borderWidthDeclaration = StringUtil.HasChars(widthPropertyName)
                ? declarations[widthPropertyName]
                : null;
            CssDeclaration awBorderWidthDeclaration = StringUtil.HasChars(awBorderWidthName)
                ? declarations[awBorderWidthName]
                : null;

            CssDeclaration borderColorDeclaration = StringUtil.HasChars(colorPropertyName)
                ? declarations[colorPropertyName]
                : null;
            CssDeclaration awBorderColorDeclaration = StringUtil.HasChars(awBorderColorName)
                ? declarations[awBorderColorName]
                : null;

            CssPropertyValue borderStyleValue = (awBorderStyleDeclaration != null)
                ? awBorderStyleDeclaration.Value
                : borderStyleDeclaration.Value;

            // WORDSNET-20289 Get border width and style from custom CSS properties. If a custom property is present,
            // it takes precedence over the corresponding standard CSS property.
            // CSS width property is used to calculate table preffered width, cell paddings and cell preffered width.
            // Custom border width property is used to restore the original border line width after round-trip.
            CssPropertyValue borderWidthValue = null;
            if (borderStyleDeclaration == null)
            {
                borderWidthValue = new CssPropertyValue(CssValue.ZeroPt);
            }
            else if (borderWidthDeclaration == null)
            {
                borderWidthValue = new CssPropertyValue(CssValue.Medium);
            }
            else
            {
                borderWidthValue = borderWidthDeclaration.Value;
            }

            Debug.Assert(borderWidthValue != null);

            CssPropertyValue borderLineWidthValue = (awBorderWidthDeclaration != null)
                ? awBorderWidthDeclaration.Value
                : borderWidthValue;

            DrColor color;
            if ((borderColorDeclaration == null) && (awBorderColorDeclaration == null))
            {
                color = DrColor.Empty;
            }
            else
            {
                CssPropertyValue borderColorValue = (awBorderColorDeclaration != null)
                    ? awBorderColorDeclaration.Value
                    : borderColorDeclaration.Value;

                if (borderColorValue.Equals(CssValue.Transparent))
                {
                    color = DrColor.Transparent;
                }
                else if (borderColorValue.Equals(CssValue.CurrentColor))
                {
                    // WORDSNET-27791 The used value of 'currentcolor' in 'border-xxx-color' properties is the used value
                    // of the 'color' property on the same element.
                    CssDeclaration colorDeclaration = declarations["color"];
                    color = (colorDeclaration != null)
                        ? colorDeclaration.Value.ParseAsColor()
                        : DrColor.Empty;
                }
                else
                {
                    color = borderColorValue.ParseAsColor();
                }
            }

            bool isAwBorder = awBorderStyleDeclaration != null;
            CssBorder cssBorder = new CssBorder(isAwBorder);
            // "-aw-border-width" stores the original line width value (not the whole border width) that doesn't require
            // any recalculation on import.
            bool convertBorderWidthToLineWidth = awBorderWidthDeclaration == null;
            cssBorder.SetProperties(
                borderStyleValue,
                borderLineWidthValue,
                color,
                borderWidthValue,
                convertBorderWidthToLineWidth,
                preserveZeroWidthBorderStyle,
                invertInsetOutsetLineStyle && !cssBorder.IsAwBorder);

            return cssBorder;
        }

        /// <summary>
        /// Sets CSS line style, width and color property values.
        /// </summary>
        private void SetProperties(
            CssPropertyValue lineStyleValue,
            CssPropertyValue lineWidthValue,
            DrColor color,
            CssPropertyValue borderWidthValue,
            bool convertBorderWidthToLineWidth,
            bool preserveZeroWidthBorderStyle,
            bool invertInsetOutsetLineStyle)
        {
            Debug.Assert(lineStyleValue != null);
            Debug.Assert(lineWidthValue != null);

            if ((lineStyleValue.Count != 1) || (lineWidthValue.Count != 1) || (color == null))
                return;

            if (lineStyleValue.FirstValue.Equals(CssValue.Hidden))
            {
                mIsHidden = true;
                mIsUndefined = false;
            }
            else if (lineStyleValue.FirstValue.Equals(CssValue.None))
            {
                mLineStyle = LineStyle.None;
                mIsUndefined = false;
            }
            else if (lineStyleValue.FirstValue.Equals(CssValue.Nil))
            {
                mLineStyle = LineStyle.None;
                IsNil = true;
                mIsUndefined = false;
            }
            else
            {
                SetBorderLineStyle(lineStyleValue.FirstValue, invertInsetOutsetLineStyle);
                SetBorderLineWidth(lineWidthValue.FirstValue);
                SetCssBorderWidth(borderWidthValue.FirstValue);
                if ((mLineStyle != LineStyle.None) &&
                    (!MathUtil.IsZero(mLineWidth) || preserveZeroWidthBorderStyle))
                {
                    // WORDSNET-15436 ExportImport: Roundtrip border width
                    if (convertBorderWidthToLineWidth)
                        ConvertBorderWidthToLineWidthAfterImport();

                    mIsUndefined = false;
                    mColorInternal = color;
                }
                else
                {
                    mLineStyle = LineStyle.None;
                    mLineWidth = 0;
                }
            }
        }

        /// <summary>
        /// Sets the border line width from CSS line width.
        /// </summary>
        /// <param name="cssValue">CSS width value or CSS extended width value.</param>
        /// <remarks>This value may be taken or from CSS border width property
        /// or from CSS extended border width property.</remarks>
        private void SetBorderLineWidth(CssValue cssValue)
        {
            double borderWidth = GetBorderWidthValue(cssValue);

            // We store the original border width in mCssLineWidth because mLineWidth can be corrected
            // in CorrectBorderWidthAfterImport() function.
            mCssLineWidth = borderWidth;
            mLineWidth = borderWidth;
        }

        /// <summary>
        /// Sets the border width from CSS border width.
        /// </summary>
        /// <param name="cssValue">CSS width value.</param>
        private void SetCssBorderWidth(CssValue cssValue)
        {
            mCssBorderWidth = GetBorderWidthValue(cssValue);
        }

        /// <summary>
        /// Corrects border width according to line style. Needed after import.
        /// Maybe other attributes also should be corrected.
        /// </summary>
        private void ConvertBorderWidthToLineWidthAfterImport()
        {
            float coeff = Border.GetActualWidth(mLineStyle, 1.0f);
            if (coeff > 1.0f)
                mLineWidth /= coeff;
        }

        /// <summary>
        /// Sets the border line style from CSS line style.
        /// </summary>
        private void SetBorderLineStyle(
            CssValue cssValue,
            bool invertInsetOutsetLineStyle)
        {
            int lineStyleValue = gCssValueToLineStyleMap[cssValue];
            if (ObjToIntDictionary<CssValue>.IsNullSubstitute(lineStyleValue))
            {
                lineStyleValue = (int)LineStyle.None;
                Debug.Fail("Unsupported line style.");
            }
            mLineStyle = (LineStyle)lineStyleValue;

            // WORDSNET-24112 MS Word supports "inset" and "outset" border styles but renders them inverted
            // on table cell elements. That is, "inset" is rendered by MS Word as "outset" on table cells, and vice versa.
            // In HTML, however, each of these line styles is always rendered in the same way on all elements. In order to
            // compensate for this difference, MS Word explicitly inverts "inset" and "outset" styles on cell borders
            // when saving to HTML (for example, it writes "outset" instead of "inset") and inverts them back upon loading.
            // We do the same.
            if (invertInsetOutsetLineStyle)
            {
                switch (mLineStyle)
                {
                    case LineStyle.Inset:
                        mLineStyle = LineStyle.Outset;
                        break;
                    case LineStyle.Outset:
                        mLineStyle = LineStyle.Inset;
                        break;
                    default:
                        // Keep other line styles unchanged.
                        break;
                }
            }

            mCssLineStyle = (mLineStyle != LineStyle.None)
                ? ((CssIdentifierValue)cssValue).Value.ToLowerInvariant()
                : "none";
        }

        /// <summary>
        /// Gets the border style.
        /// </summary>
        internal LineStyle LineStyle
        {
            get { return mLineStyle; }
        }

        /// <summary>
        /// Gets the border CSS style name.
        /// </summary>
        internal string CssLineStyle
        {
            get { return mCssLineStyle; }
        }

        /// <summary>
        /// Gets the border width in points.
        /// </summary>
        internal double LineWidth
        {
            get { return mLineWidth; }
        }

        /// <summary>
        /// Gets the border CSS line width in points.
        /// </summary>
        internal double CssLineWidth
        {
            get { return mCssLineWidth; }
        }

        /// <summary>
        /// Gets the border CSS width in points.
        /// </summary>
        internal double CssBorderWidth
        {
            get { return mCssBorderWidth; }
        }

        /// <summary>
        /// Gets the border color.
        /// </summary>
        internal DrColor ColorInternal
        {
            get { return mColorInternal; }
        }

        /// <summary>
        /// Indicates whether the border style is 'hidden'.
        /// </summary>
        internal bool IsHidden
        {
            get { return mIsHidden; }
        }

        /// <summary>
        /// Indicates whether the border style is 'none'.
        /// </summary>
        internal bool IsNone
        {
            get { return mLineStyle == LineStyle.None; }
        }

        internal bool IsNil { get; private set; }

        /// <summary>
        /// Indicates whether the border is undefined.
        /// </summary>
        internal bool IsUndefined
        {
            get { return mIsUndefined; }
        }

        /// <summary>
        /// Indicates whether the border is visible.
        /// </summary>
        internal bool IsVisible
        {
            get { return !(mIsUndefined || mIsHidden || IsNone); }
        }

        internal bool IsAwBorder { get; private set; }

        private static double GetBorderWidthValue(CssValue cssValue)
        {
            Debug.Assert(cssValue != null);

            double borderWidth;
            if (cssValue.Equals(CssValue.Thin))
            {
                borderWidth = 1.5; // Looks like IE uses this width.
            }
            else if (cssValue.Equals(CssValue.Medium))
            {
                borderWidth = 3.0; // Looks like IE uses this width.
            }
            else if (cssValue.Equals(CssValue.Thick))
            {
                borderWidth = 4.5; // Looks like IE uses this width.
            }
            else
            {
                double length = CssUtil.LengthToPoint(cssValue);
                borderWidth = (!MathUtil.IsMinValue(length)) ? length : 0;
            }
            return borderWidth;
        }

        static CssBorder()
        {
            gCssValueToLineStyleMap = new ObjToIntDictionary<CssValue>();

            gCssValueToLineStyleMap.Add(CssValue.None, (int)LineStyle.None);
            gCssValueToLineStyleMap.Add(CssValue.Hidden, (int)LineStyle.None);
            gCssValueToLineStyleMap.Add(CssValue.Dotted, (int)LineStyle.Dot);
            gCssValueToLineStyleMap.Add(CssValue.Dashed, (int)LineStyle.DashSmallGap);
            gCssValueToLineStyleMap.Add(CssValue.DoubleId, (int)LineStyle.Double);
            gCssValueToLineStyleMap.Add(CssValue.Groove, (int)LineStyle.Emboss3D);
            gCssValueToLineStyleMap.Add(CssValue.Ridge, (int)LineStyle.Engrave3D);
            gCssValueToLineStyleMap.Add(CssValue.Solid, (int)LineStyle.Single);
            gCssValueToLineStyleMap.Add(CssValue.Inset, (int)LineStyle.Inset);
            gCssValueToLineStyleMap.Add(CssValue.Outset, (int)LineStyle.Outset);

            // The values below are custom AW border styles that are used to preserve
            // MS Word-specific border styles during HTML round-trip.
            gCssValueToLineStyleMap.Add(CssValue.Single, (int)LineStyle.Single);
            gCssValueToLineStyleMap.Add(CssValue.Thick, (int)LineStyle.Thick);
            gCssValueToLineStyleMap.Add(CssValue.Hairline, (int)LineStyle.Hairline);
            gCssValueToLineStyleMap.Add(CssValue.Dot, (int)LineStyle.Dot);
            gCssValueToLineStyleMap.Add(CssValue.DashLargeGap, (int)LineStyle.DashLargeGap);
            gCssValueToLineStyleMap.Add(CssValue.DotDash, (int)LineStyle.DotDash);
            gCssValueToLineStyleMap.Add(CssValue.DotDotDash, (int)LineStyle.DotDotDash);
            gCssValueToLineStyleMap.Add(CssValue.Triple, (int)LineStyle.Triple);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickSmallGap, (int)LineStyle.ThinThickSmallGap);
            gCssValueToLineStyleMap.Add(CssValue.ThickThinSmallGap, (int)LineStyle.ThickThinSmallGap);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickThinSmallGap, (int)LineStyle.ThinThickThinSmallGap);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickMediumGap, (int)LineStyle.ThinThickMediumGap);
            gCssValueToLineStyleMap.Add(CssValue.ThickThinMediumGap, (int)LineStyle.ThickThinMediumGap);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickThinMediumGap, (int)LineStyle.ThinThickThinMediumGap);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickLargeGap, (int)LineStyle.ThinThickLargeGap);
            gCssValueToLineStyleMap.Add(CssValue.ThickThinLargeGap, (int)LineStyle.ThickThinLargeGap);
            gCssValueToLineStyleMap.Add(CssValue.ThinThickThinLargeGap, (int)LineStyle.ThinThickThinLargeGap);
            gCssValueToLineStyleMap.Add(CssValue.Wave, (int)LineStyle.Wave);
            gCssValueToLineStyleMap.Add(CssValue.DoubleWave, (int)LineStyle.DoubleWave);
            gCssValueToLineStyleMap.Add(CssValue.DashSmallGap, (int)LineStyle.DashSmallGap);
            gCssValueToLineStyleMap.Add(CssValue.DashDotStroker, (int)LineStyle.DashDotStroker);
            gCssValueToLineStyleMap.Add(CssValue.Emboss3D, (int)LineStyle.Emboss3D);
            gCssValueToLineStyleMap.Add(CssValue.Engrave3D, (int)LineStyle.Engrave3D);
        }

        private static readonly ObjToIntDictionary<CssValue> gCssValueToLineStyleMap;
        private static readonly CssBorder gEmpty = new CssBorder(false);

        private LineStyle mLineStyle;
        private double mLineWidth;
        private DrColor mColorInternal;
        private bool mIsHidden;
        private string mCssLineStyle;
        private bool mIsUndefined;
        private double mCssLineWidth;
        private double mCssBorderWidth;
    }
}
