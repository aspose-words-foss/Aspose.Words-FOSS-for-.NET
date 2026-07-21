// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Class factory for CSS property definitions <see cref="CssPropertyDef" />.
    /// </summary>
    internal static class CssPropertyDefFactory
    {
        /// <summary>
        /// Gets a CSS property definition class instance by the property name.
        /// </summary>
        /// <param name="propertyName">Property name. Must be lowercase.</param>
        /// <returns>An instance of the corresponding property class or a <see cref="CssUnknownPropertyDef" /> instance
        /// if the property is not supported.
        /// </returns>
        internal static CssPropertyDef GetPropertyDef(string propertyName)
        {
            Debug.Assert(StringUtil.HasChars(propertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(propertyName));

            CssPropertyDef propertyDef = gPropertyDefs.GetValueOrNull(propertyName);
            if (propertyDef == null)
                propertyDef = new CssUnknownPropertyDef(propertyName);
            return propertyDef;
        }

        /// <summary>
        /// Gets a CSS property definition class by the property name. Returns only individual (non-shorthand) properties.
        /// </summary>
        /// <param name="propertyName">Property name. Must be lowercase.</param>
        /// <returns>
        /// The property definition or <c>null</c> if the property is not supported.
        /// </returns>
        internal static CssIndividualPropertyDef GetIndividualPropertyDef(string propertyName)
        {
            Debug.Assert(StringUtil.HasChars(propertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(propertyName));

            return gPropertyDefs[propertyName] as CssIndividualPropertyDef;
        }

        /// <summary>
        /// Supported shorthand properties.
        /// </summary>
        internal static ICollection<CssShorthandPropertyDef> ShorthandProperties
        {
            get { return gShorthandPropertyDefs; }
        }

        static CssPropertyDefFactory()
        {
            gPropertyDefs = new Dictionary<string, CssPropertyDef>();
            gShorthandPropertyDefs = new List<CssShorthandPropertyDef>();
            FillPropertyDefs();
        }

        private static void FillPropertyDefs()
        {
            // background
            AddPropertyDef(new CssBackgroundAttachmentPropertyDef());
            AddPropertyDef(new CssBackgroundColorPropertyDef());
            AddPropertyDef(new CssBackgroundImagePropertyDef());
            AddPropertyDef(new CssBackgroundPositionPropertyDef());
            AddPropertyDef(new CssBackgroundRepeatPropertyDef());
            AddPropertyDef(new CssBackgroundPropertyDef());
            // margins
            AddPropertyDef(new CssMarginLeftPropertyDef());
            AddPropertyDef(new CssMarginRightPropertyDef());
            AddPropertyDef(new CssMarginTopPropertyDef());
            AddPropertyDef(new CssMarginBottomPropertyDef());
            // padding
            AddPropertyDef(new CssPaddingLeftPropertyDef());
            AddPropertyDef(new CssPaddingRightPropertyDef());
            AddPropertyDef(new CssPaddingTopPropertyDef());
            AddPropertyDef(new CssPaddingBottomPropertyDef());
            // -aw-padding-xxx properties
            AddPropertyDef(new CssAWPaddingTopPropertyDef());
            AddPropertyDef(new CssAWPaddingRightPropertyDef());
            AddPropertyDef(new CssAWPaddingBottomPropertyDef());
            AddPropertyDef(new CssAWPaddingLeftPropertyDef());
            // list-style
            AddPropertyDef(new CssListStyleTypePropertyDef());
            AddPropertyDef(new CssListStylePositionPropertyDef());
            AddPropertyDef(new CssListStyleImagePropertyDef());
            AddPropertyDef(new CssListStylePropertyDef());

            AddPropertyDef(new CssColorPropertyDef());
            AddPropertyDef(new CssDirectionPropertyDef());
            AddPropertyDef(new CssOrphansPropertyDef());
            AddPropertyDef(new CssPageBreakInsidePropertyDef());
            AddPropertyDef(new CssPageBreakAfterPropertyDef());
            AddPropertyDef(new CssPageBreakBeforePropertyDef());
            AddPropertyDef(new CssTextAlignPropertyDef());
            AddPropertyDef(new CssTextIndentPropertyDef());
            AddPropertyDef(new CssWidowsPropertyDef());
            AddPropertyDef(new CssWritingModePropertyDef());
            AddPropertyDef(new CssMarginPropertyDef());
            AddPropertyDef(new CssPaddingPropertyDef());
            AddPropertyDef(new CssUnicodeBidiPropertyDef());
            AddPropertyDef(new CssVerticalAlignPropertyDef());
            AddPropertyDef(new CssFloatPropertyDef());
            AddPropertyDef(new CssWidthPropertyDef());
            AddPropertyDef(new CssMinWidthPropertyDef());
            AddPropertyDef(new CssMaxWidthPropertyDef());
            AddPropertyDef(new CssClearPropertyDef());
            AddPropertyDef(new CssCaptionSidePropertyDef());
            AddPropertyDef(new CssBorderCollapsePropertyDef());
            AddPropertyDef(new CssHeightPropertyDef());
            AddPropertyDef(new CssMinHeightPropertyDef());
            AddPropertyDef(new CssMaxHeightPropertyDef());
            AddPropertyDef(new CssBorderSpacingPropertyDef());
            AddPropertyDef(new CssWhiteSpacePropertyDef());
            AddPropertyDef(new CssTransformPropertyDef());

            AddPropertyDef(new CssTextTransformPropertyDef());
            AddPropertyDef(new CssTextDecorationPropertyDef());
            AddPropertyDef(new CssDisplayPropertyDef());
            AddPropertyDef(new CssLetterSpacingPropertyDef());
            AddPropertyDef(new CssBoxSizingPropertyDef());
            AddPropertyDef(new CssVisibilityPropertyDef());
            AddPropertyDef(new CssRubyAlignPropertyDef());
            // @page
            AddPropertyDef(new CssPagePropertyDef());
            AddPropertyDef(new CssPageSizePropertyDef());
            // font
            AddPropertyDef(new CssFontSizePropertyDef());
            AddPropertyDef(new CssFontFamilyPropertyDef());
            AddPropertyDef(new CssFontWeightPropertyDef());
            AddPropertyDef(new CssFontStylePropertyDef());
            AddPropertyDef(new CssFontVariantPropertyDef());
            AddPropertyDef(new CssLineHeightPropertyDef());
            AddPropertyDef(new CssFontPropertyDef());
            // borders shorthand
            AddPropertyDef(new CssBorderTopPropertyDef());
            AddPropertyDef(new CssBorderRightPropertyDef());
            AddPropertyDef(new CssBorderBottomPropertyDef());
            AddPropertyDef(new CssBorderLeftPropertyDef());
            // 'border' shorthand property should be placed in mShorthandPropertyDefList after border-(top|right|bottom|left) properties.
            // See CssBorderPropertyDef.ReduceByShorthand for details.
            AddPropertyDef(new CssBorderPropertyDef());
            // border widths
            AddPropertyDef(new CssBorderWidthPropertyDef());
            AddPropertyDef(new CssBorderTopWidthPropertyDef());
            AddPropertyDef(new CssBorderRightWidthPropertyDef());
            AddPropertyDef(new CssBorderBottomWidthPropertyDef());
            AddPropertyDef(new CssBorderLeftWidthPropertyDef());
            // border colors
            AddPropertyDef(new CssBorderColorPropertyDef());
            AddPropertyDef(new CssBorderTopColorPropertyDef());
            AddPropertyDef(new CssBorderRightColorPropertyDef());
            AddPropertyDef(new CssBorderBottomColorPropertyDef());
            AddPropertyDef(new CssBorderLeftColorPropertyDef());
            // border styles
            AddPropertyDef(new CssBorderStylePropertyDef());
            AddPropertyDef(new CssBorderTopStylePropertyDef());
            AddPropertyDef(new CssBorderRightStylePropertyDef());
            AddPropertyDef(new CssBorderBottomStylePropertyDef());
            AddPropertyDef(new CssBorderLeftStylePropertyDef());
            // -aw-borders shorthand
            AddPropertyDef(new CssAWBorderTopPropertyDef());
            AddPropertyDef(new CssAWBorderRightPropertyDef());
            AddPropertyDef(new CssAWBorderBottomPropertyDef());
            AddPropertyDef(new CssAWBorderLeftPropertyDef());
            // '-aw-border' shorthand property should be placed in mShorthandPropertyDefList after -aw-border-(top|right|bottom|left) properties.
            // See CssAWBorderPropertyDef.ReduceByShorthand for details.
            AddPropertyDef(new CssAWBorderPropertyDef());
            // -aw-border widths
            AddPropertyDef(new CssAWBorderWidthPropertyDef());
            AddPropertyDef(new CssAWBorderTopWidthPropertyDef());
            AddPropertyDef(new CssAWBorderRightWidthPropertyDef());
            AddPropertyDef(new CssAWBorderBottomWidthPropertyDef());
            AddPropertyDef(new CssAWBorderLeftWidthPropertyDef());
            // -aw-border styles
            AddPropertyDef(new CssAWBorderStylePropertyDef());
            AddPropertyDef(new CssAWBorderTopStylePropertyDef());
            AddPropertyDef(new CssAWBorderRightStylePropertyDef());
            AddPropertyDef(new CssAWBorderBottomStylePropertyDef());
            AddPropertyDef(new CssAWBorderLeftStylePropertyDef());
            // -aw-border colors
            AddPropertyDef(new CssAWBorderColorPropertyDef());
            AddPropertyDef(new CssAWBorderTopColorPropertyDef());
            AddPropertyDef(new CssAWBorderRightColorPropertyDef());
            AddPropertyDef(new CssAWBorderBottomColorPropertyDef());
            AddPropertyDef(new CssAWBorderLeftColorPropertyDef());
            // -aw-border-insideH, -aw-border-insideV shorthand
            AddPropertyDef(new CssAWBorderInsideHPropertyDef());
            AddPropertyDef(new CssAWBorderInsideVPropertyDef());
            // -aw-border-inside widths
            AddPropertyDef(new CssAWBorderInsideHWidthPropertyDef());
            AddPropertyDef(new CssAWBorderInsideVWidthPropertyDef());
            // -aw-border-inside styles
            AddPropertyDef(new CssAWBorderInsideHStylePropertyDef());
            AddPropertyDef(new CssAWBorderInsideVStylePropertyDef());
            // -aw-border-inside colors
            AddPropertyDef(new CssAWBorderInsideHColorPropertyDef());
            AddPropertyDef(new CssAWBorderInsideVColorPropertyDef());

            AddPropertyDef(new CssAWOutlineLevelPropertyDef());

            AddPropertyDef(new CssContentPropertyDef());

            // The "src" property of the @font-face rule.
            AddPropertyDef(new CssSrcPropertyDef());

            // SVG
            AddPropertyDef(new CssStrokePropertyDef());
            AddPropertyDef(new CssStrokeWidthPropertyDef());
            AddPropertyDef(new CssStrokeDasharrayPropertyDef());
            AddPropertyDef(new CssStrokeMiterlimitPropertyDef());
            AddPropertyDef(new CssStrokeLineCapPropertyDef());
            AddPropertyDef(new CssStrokeLineJoinPropertyDef());
            AddPropertyDef(new CssFillPropertyDef());
            AddPropertyDef(new CssFillOpacityPropertyDef());
            AddPropertyDef(new CssFillRulePropertyDef());
            AddPropertyDef(new CssOpacityPropertyDef());
            AddPropertyDef(new CssTextAnchorPropertyDef());
            AddPropertyDef(new CssStrokeDashOffsetPropertyDef());
            AddPropertyDef(new CssBaseLineShiftPropertyDef());
            AddPropertyDef(new CssDominantBaseLinePropertyDef());

            // MSO
            AddPropertyDef(new CssMsoAnsiFontSizePropertyDef());
            AddPropertyDef(new CssMsoAnsiFontStylePropertyDef());
            AddPropertyDef(new CssMsoAnsiFontWeightPropertyDef());
            AddPropertyDef(new CssMsoAnsiLanguagePropertyDef());
            AddPropertyDef(new CssMsoAsciiFontFamilyPropertyDef());
            AddPropertyDef(new CssMsoAsciiThemeFontPropertyDef());
            AddPropertyDef(new CssMsoBidiFontFamilyPropertyDef());
            AddPropertyDef(new CssMsoBidiFontSizePropertyDef());
            AddPropertyDef(new CssMsoBidiFontStylePropertyDef());
            AddPropertyDef(new CssMsoBidiFontWeightPropertyDef());
            AddPropertyDef(new CssMsoBidiLanguagePropertyDef());
            AddPropertyDef(new CssMsoBidiThemeFontPropertyDef());
            AddPropertyDef(new CssMsoColorAltPropertyDef());
            AddPropertyDef(new CssMsoFareastFontFamilyPropertyDef());
            AddPropertyDef(new CssMsoFareastLanguagePropertyDef());
            AddPropertyDef(new CssMsoFareastThemeFontPropertyDef());
            AddPropertyDef(new CssMsoFontKerningPropertyDef());
            AddPropertyDef(new CssMsoHansiFontFamilyPropertyDef());
            AddPropertyDef(new CssMsoHansiThemeFontPropertyDef());
            AddPropertyDef(new CssMsoNoProofPropertyDef());
            AddPropertyDef(new CssMsoOutlineLevelPropertyDef());
            AddPropertyDef(new CssMsoPaginationPropertyDef());
            AddPropertyDef(new CssMsoParaMarginBottomPropertyDef());
            AddPropertyDef(new CssMsoParaMarginLeftPropertyDef());
            AddPropertyDef(new CssMsoParaMarginPropertyDef());
            AddPropertyDef(new CssMsoParaMarginRightPropertyDef());
            AddPropertyDef(new CssMsoParaMarginTopPropertyDef());
        }

        private static void AddPropertyDef(CssPropertyDef propertyDef)
        {
            gPropertyDefs.Add(propertyDef.Name, propertyDef);

            CssShorthandPropertyDef cssShorthand = propertyDef as CssShorthandPropertyDef;
            if (cssShorthand != null)
                gShorthandPropertyDefs.Add(cssShorthand);
        }

        /// <summary>
        /// All supported properties, including shorthands.
        /// </summary>
        private static readonly Dictionary<string, CssPropertyDef> gPropertyDefs;

        /// <summary>
        /// Supported shorthand properties.
        /// </summary>
        private static readonly List<CssShorthandPropertyDef> gShorthandPropertyDefs;
    }
}
