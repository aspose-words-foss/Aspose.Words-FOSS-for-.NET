// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/17/2015 by Andrey Noskov

using System;
using Aspose.Drawing;
using Aspose.Words.ApsBuilder.Dml.Text.Layout;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Math;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// Represents methods for conversion between <see cref="DmlRunProperties"/> and <see cref="RunPr"/>.
    /// </summary>
    internal class RunPrConverter
    {
        /// <summary>
        /// Converts <see cref="DmlRunProperties"/>'s properties to the corresponding attributes of <see cref="RunPr"/>.
        /// </summary>
        internal static RunPr ConvertToRunPr(DmlRunProperties dmlRunPr, Theme theme)
        {
            RunPr runPr = new RunPr();

            foreach (int fontAttribute in gConvertingFontAttributes)
            {
                object value = GetProperty(dmlRunPr, fontAttribute, theme, false);
                if (value != null)
                    runPr.SetAttr(fontAttribute, value);
            }

            return runPr;
        }

        /// <summary>
        /// Converts <see cref="RunPr"/>'s attributes to the corresponding properties of <see cref="DmlRunProperties"/>.
        /// </summary>
        internal static DmlRunProperties ConvertToDmlRunProperties(RunPr runPr, Theme theme)
        {
            DmlRunProperties dmlRunPr = new DmlRunProperties();

            foreach (int fontAttribute in gConvertingFontAttributes)
            {
                object value = runPr[fontAttribute];
                if (value != null)
                    SetProperty(dmlRunPr, fontAttribute, value, theme);
            }

            return dmlRunPr;
        }

        /// <summary>
        /// Returns a flag indicating whether <paramref name="properties"/> contain the specified font property that is
        /// set directly.
        /// </summary>
        internal static bool IsPropertySpecified(DmlRunProperties properties, int fontAttr)
        {
            switch (fontAttr)
            {
                case FontAttr.Size:
                case FontAttr.SizeBi:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.FontSize);
                case FontAttr.Kerning:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Kerning);
                case FontAttr.Spacing:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Spacing);
                case FontAttr.NameAscii:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.LatinFont);
                case FontAttr.NameBi:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.ComplexScriptFont);
                case FontAttr.NameFarEast:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.EastAsianFont);
                case FontAttr.NameOther:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.SymbolFont);
                case FontAttr.Bold:
                case FontAttr.BoldBi:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Bold);
                case FontAttr.Italic:
                case FontAttr.ItalicBi:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Italics);
                case FontAttr.Underline:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Underline);
                case FontAttr.DoubleStrikeThrough:
                case FontAttr.StrikeThrough:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Strikethrough);
                case FontAttr.AllCaps:
                case FontAttr.SmallCaps:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Capitalization);
                case FontAttr.VerticalAlignment:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Baseline);
                case FontAttr.Color:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Fill);
                case FontAttr.EffectFill:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Fill);
                case FontAttr.HighlightColor:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.HighlightColor);
                case FontAttr.EffectOutline:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Outline);
                case FontAttr.EffectGlow:
                    return (properties.Effects != null) && (properties.Effects[DmlShapeEffectType.Glow] != null);
                case FontAttr.EffectShadow:
                    return (properties.Effects != null) && (properties.Effects[DmlShapeEffectType.OuterShadow] != null);
                case FontAttr.EffectReflection:
                    return (properties.Effects != null) && (properties.Effects[DmlShapeEffectType.Reflection] != null);
                case FontAttr.LocaleId:
                case FontAttr.LocaleIdBi:
                case FontAttr.LocaleIdFarEast:
                    return properties.IsPropertySpecified(DmlRunPropertiesIds.Language);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <b>true</b> if the specified font attribute is supported when converting between
        /// <see cref="DmlRunProperties"/> and <see cref="RunPr"/>
        /// </summary>
        internal static bool IsPropertySupported(int fontAttr)
        {
            switch (fontAttr)
            {
                case FontAttr.Size:
                case FontAttr.SizeBi:
                case FontAttr.Kerning:
                case FontAttr.Spacing:
                case FontAttr.NameAscii:
                case FontAttr.NameBi:
                case FontAttr.NameFarEast:
                case FontAttr.NameOther:
                case FontAttr.Bold:
                case FontAttr.BoldBi:
                case FontAttr.Italic:
                case FontAttr.ItalicBi:
                case FontAttr.Underline:
                case FontAttr.DoubleStrikeThrough:
                case FontAttr.StrikeThrough:
                case FontAttr.AllCaps:
                case FontAttr.SmallCaps:
                case FontAttr.VerticalAlignment:
                case FontAttr.Color:
                case FontAttr.EffectFill:
                case FontAttr.HighlightColor:
                case FontAttr.EffectOutline:
                case FontAttr.EffectGlow:
                case FontAttr.EffectShadow:
                case FontAttr.EffectReflection:
                case FontAttr.LocaleId:
                case FontAttr.LocaleIdBi:
                case FontAttr.LocaleIdFarEast:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets value of a DML run property specified by a font attribute.
        /// </summary>
        internal static object GetProperty(DmlRunProperties properties, int fontAttr, Theme theme, bool isForApi)
        {
            switch (fontAttr)
            {
                case FontAttr.Size:
                case FontAttr.SizeBi:
                    // Multiply by 2 because half-points units must be used.
                    return (int)(properties.FontSize.ValueInPoints * 2);
                case FontAttr.Kerning:
                    // Multiply by 2 because half-points units must be used.
                    return (int)(properties.Kerning.ValueInPoints * 2);
                case FontAttr.Spacing:
                    // Get spacing between characters in twips.
                    return ConvertUtilCore.PointToTwip(properties.Spacing.ValueInPoints);
                case FontAttr.NameAscii:
                    return CreateComplexFontName(properties.LatinFont, ThemeFontCore.MinorHAnsi, theme);
                case FontAttr.NameBi:
                    return CreateComplexFontName(properties.ComplexScriptFont, ThemeFontCore.MinorBidi, theme);
                case FontAttr.NameFarEast:
                    return CreateComplexFontName(properties.EastAsianFont, ThemeFontCore.MinorEastAsia, theme);
                case FontAttr.NameOther:
                {
                    // If Symbol font is omitted, use Latin font instead.
                    DmlFont symbolFont = (properties.SymbolFont != null) ? properties.SymbolFont : properties.LatinFont;
                    return CreateComplexFontName(symbolFont, ThemeFontCore.MinorHAnsi, theme);
                }
                case FontAttr.Bold:
                case FontAttr.BoldBi:
                    return AttrBoolEx.FromBool(properties.Bold);
                case FontAttr.Italic:
                case FontAttr.ItalicBi:
                    return AttrBoolEx.FromBool(properties.Italics);
                case FontAttr.Underline:
                    return properties.Underline;
                case FontAttr.DoubleStrikeThrough:
                    return AttrBoolEx.FromBool(properties.Strikethrough == DmlTextStrike.Double);
                case FontAttr.StrikeThrough:
                    return AttrBoolEx.FromBool(properties.Strikethrough == DmlTextStrike.Single);
                case FontAttr.AllCaps:
                    return AttrBoolEx.FromBool(properties.Capitalization == DmlCapitalization.All);
                case FontAttr.SmallCaps:
                    return AttrBoolEx.FromBool(properties.Capitalization == DmlCapitalization.Small);
                case FontAttr.VerticalAlignment:
                    return GetVerticalAlignment(properties);
                case FontAttr.Color:
                    return GetSolidFillColor(properties, theme);
                case FontAttr.EffectFill:
                    // Outline and Fill of text are set through effects.
                    return (properties.Fill != null)
                        ? (isForApi) ? properties.Fill : null // FOSS: DmlTextEffectsHelper is gone
                        : null;
                case FontAttr.HighlightColor:
                    return (properties.HighlightColor != null)
                        ? properties.HighlightColor.CreateDrColor(theme, null)
                        : null;
                case FontAttr.EffectOutline:
                    return GetEffectOutline(properties);
                case FontAttr.EffectGlow:
                    return GetEffectGlow(properties);
                case FontAttr.EffectShadow:
                    return GetEffectShadow(properties);
                case FontAttr.EffectReflection:
                    return GetEffectReflection(properties);
                case FontAttr.LocaleId:
                case FontAttr.LocaleIdBi:
                case FontAttr.LocaleIdFarEast:
                {
                    if (isForApi || (GetFontAttrLocaleId(properties.Language) == fontAttr))
                        return (int)properties.Language;
                    else
                        return null;
                }
                case FontAttr.TextEffect:
                case FontAttr.Outline:
                case FontAttr.Shadow:
                case FontAttr.Engrave:
                case FontAttr.Scaling:
                case FontAttr.Emboss:
                case FontAttr.Hidden:
                    // These attributes cannot be applied to chart title.
                    return null;
                case FontAttr.Position:
                case FontAttr.Bidi:
                case FontAttr.ComplexScript:
                case FontAttr.NoProofing:
                    // Currently there is no way to set these attributes inside DmlRunProperties.
                    return null;
                default:
                    return null;
            }
        }

        private static object GetVerticalAlignment(DmlRunProperties properties)
        {
            if (MathUtil.AreEqual(properties.Baseline, 0))
                return RunVerticalAlignment.Baseline;

            // WORDSNET-16973 The text with the specified baseline should be rendered as "subscript" or
            // "superscript".
            return (properties.Baseline < 0) ? RunVerticalAlignment.Subscript : RunVerticalAlignment.Superscript;
        }

        private static object GetSolidFillColor(DmlRunProperties properties, Theme theme)
        {
            if ((properties.Fill == null) || (properties.Fill.DmlFillType != DmlFillType.SolidFill))
                return null;

            // If fill is solid set text color, required for proper color rendering in Math,
            // because DmlEffects in math is not yet supported.
            DmlSolidFill solidFill = (DmlSolidFill)properties.Fill;

            DrColor color = solidFill.Color.CreateDrColor(theme, null);
            if ((color.A == 0) && (solidFill.Color.Alpha == null))
                color = new DrColor(0xff, color);

            return color;
        }

        private static object GetEffectOutline(DmlRunProperties properties)
        {
            throw new NotSupportedException("FOSS");
        }

        private static object GetEffectGlow(DmlRunProperties properties)
        {
            if (properties.Effects == null)
                return null;

            DmlShapeEffect glowEffect = properties.Effects[DmlShapeEffectType.Glow];
            if (glowEffect == null)
                return null;

            return null;
            // FOSS
        }

        private static object GetEffectShadow(DmlRunProperties properties)
        {
            if (properties.Effects == null)
                return null;

            DmlShapeEffect shadowEffect = properties.Effects[DmlShapeEffectType.OuterShadow];
            if (shadowEffect == null)
                return null;

            return null;
            // FOSS
        }

        private static DmlShapeEffect GetEffectReflection(DmlRunProperties properties)
        {
            if (properties.Effects == null)
                return null;

            DmlShapeEffect reflectionEffect = properties.Effects[DmlShapeEffectType.Reflection];
            return (reflectionEffect != null) ? reflectionEffect.Clone() : null;
        }

        /// <summary>
        /// Sets value of a DML run property specified by a font attribute.
        /// </summary>
        internal static void SetProperty(DmlRunProperties properties, int fontAttr, object value, Theme theme)
        {
            switch (fontAttr)
            {
                case FontAttr.Size:
                case FontAttr.SizeBi:
                    properties.FontSize = new DmlTextPoints(((int)value / 2) * 100);
                    break;
                case FontAttr.Kerning:
                    properties.Kerning = new DmlTextPoints(((int)value / 2) * 100);
                    break;
                case FontAttr.Spacing:
                    properties.Spacing = new DmlTextPoints((int)(ConvertUtilCore.TwipToPoint((int)value) * 100));
                    break;
                case FontAttr.NameAscii:
                {
                    properties.LatinFont = new DmlFont();
                    properties.LatinFont.TextTypeface = ComplexFontName.Resolve(value, theme);
                    break;
                }
                case FontAttr.NameBi:
                {
                    properties.ComplexScriptFont = new DmlFont();
                    properties.ComplexScriptFont.TextTypeface = ComplexFontName.Resolve(value, theme);
                    break;
                }
                case FontAttr.NameFarEast:
                {
                    properties.EastAsianFont = new DmlFont();
                    properties.EastAsianFont.TextTypeface = ComplexFontName.Resolve(value, theme);
                    break;
                }
                case FontAttr.NameOther:
                {
                    properties.SymbolFont = new DmlFont();
                    properties.SymbolFont.TextTypeface = ComplexFontName.Resolve(value, theme);
                    break;
                }
                case FontAttr.Bold:
                case FontAttr.BoldBi:
                    properties.Bold = ((AttrBoolEx)value).ToBool();
                    break;
                case FontAttr.Italic:
                case FontAttr.ItalicBi:
                    properties.Italics = ((AttrBoolEx)value).ToBool();
                    break;
                case FontAttr.Underline:
                    properties.Underline = (Underline)value;
                    break;
                case FontAttr.MathStyle:
                {
                    switch ((MathStyle)value)
                    {
                        case MathStyle.Plain:
                        {
                            properties.Bold = false;
                            properties.Italics = false;
                            break;
                        }
                        case MathStyle.Bold:
                        {
                            properties.Bold = true;
                            properties.Italics = false;
                            break;
                        }
                        case MathStyle.Italic:
                        {
                            properties.Bold = false;
                            properties.Italics = true;
                            break;
                        }
                        case MathStyle.BoldItalic:
                        {
                            properties.Bold = true;
                            properties.Italics = true;
                            break;
                        }
                    }

                    break;
                }
                case FontAttr.DoubleStrikeThrough:
                {
                    bool isOn = ((AttrBoolEx)value).ToBool();
                    if (isOn != (properties.Strikethrough == DmlTextStrike.Double))
                        properties.Strikethrough = isOn ? DmlTextStrike.Double : DmlTextStrike.No;
                    break;
                }
                case FontAttr.StrikeThrough:
                {
                    bool isOn = ((AttrBoolEx)value).ToBool();
                    if (isOn != (properties.Strikethrough == DmlTextStrike.Single))
                        properties.Strikethrough = isOn ? DmlTextStrike.Single : DmlTextStrike.No;
                    break;
                }
                case FontAttr.AllCaps:
                {
                    bool isOn = ((AttrBoolEx)value).ToBool();
                    if (isOn != (properties.Capitalization == DmlCapitalization.All))
                        properties.Capitalization = isOn ? DmlCapitalization.All : DmlCapitalization.None;
                    break;
                }
                case FontAttr.SmallCaps:
                {
                    bool isOn = ((AttrBoolEx)value).ToBool();
                    if (isOn != (properties.Capitalization == DmlCapitalization.Small))
                        properties.Capitalization = isOn ? DmlCapitalization.Small : DmlCapitalization.None;
                    break;
                }
                case FontAttr.Color:
                    properties.Fill = new DmlSolidFill(DmlHexRgbColor.FromDrColor((DrColor)value));
                    break;
                case FontAttr.LocaleId:
                case FontAttr.LocaleIdBi:
                case FontAttr.LocaleIdFarEast:
                    properties.Language = (Language)value;
                    break;
                case FontAttr.VerticalAlignment:
                {
                    // MS Word uses these defaults in its Font dialog.
                    const double superscriptOffset = 0.3;
                    const double subscriptOffset = -0.25;
                    switch ((RunVerticalAlignment)value)
                    {
                        case RunVerticalAlignment.Superscript:
                            properties.Baseline = superscriptOffset;
                            break;
                        case RunVerticalAlignment.Subscript:
                            properties.Baseline = subscriptOffset;
                            break;
                        case RunVerticalAlignment.Baseline:
                        default:
                            properties.Baseline = 0;
                            break;
                    }
                    break;
                }
                case FontAttr.HighlightColor:
                    properties.HighlightColor = DmlColor.CreateFromDrColor((DrColor)value);
                    break;
                case FontAttr.EffectFill:
                    properties.Fill = (DmlFill)value;
                    break;
                case FontAttr.TextEffect:
                case FontAttr.Outline:
                case FontAttr.Shadow:
                case FontAttr.Engrave:
                case FontAttr.Scaling:
                case FontAttr.Emboss:
                case FontAttr.Hidden:
                    // These attributes cannot be applied to chart title.
                    break;
                case FontAttr.Position:
                case FontAttr.Bidi:
                case FontAttr.ComplexScript:
                case FontAttr.NoProofing:
                    // Currently there is no way to set these attributes inside DmlRunProperties.
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Clears a DML run property specified by a font attribute.
        /// </summary>
        internal static void ClearProperty(DmlRunProperties properties, int fontAttr)
        {
            DmlRunPropertiesIds propertyId;

            switch (fontAttr)
            {
                case FontAttr.Size:
                case FontAttr.SizeBi:
                    propertyId = DmlRunPropertiesIds.FontSize;
                    break;
                case FontAttr.Kerning:
                    propertyId = DmlRunPropertiesIds.Kerning;
                    break;
                case FontAttr.Spacing:
                    propertyId = DmlRunPropertiesIds.Spacing;
                    break;
                case FontAttr.NameAscii:
                    propertyId = DmlRunPropertiesIds.LatinFont;
                    break;
                case FontAttr.NameBi:
                    propertyId = DmlRunPropertiesIds.ComplexScriptFont;
                    break;
                case FontAttr.NameFarEast:
                    propertyId = DmlRunPropertiesIds.EastAsianFont;
                    break;
                case FontAttr.NameOther:
                    propertyId = DmlRunPropertiesIds.SymbolFont;
                    break;
                case FontAttr.Bold:
                case FontAttr.BoldBi:
                    propertyId = DmlRunPropertiesIds.Bold;
                    break;
                case FontAttr.Italic:
                case FontAttr.ItalicBi:
                    propertyId = DmlRunPropertiesIds.Italics;
                    break;
                case FontAttr.Underline:
                    propertyId = DmlRunPropertiesIds.Underline;
                    break;
                case FontAttr.DoubleStrikeThrough:
                {
                    if (properties.Strikethrough != DmlTextStrike.Double)
                        return;

                    propertyId = DmlRunPropertiesIds.Strikethrough;
                    break;
                }
                case FontAttr.StrikeThrough:
                {
                    if (properties.Strikethrough != DmlTextStrike.Single)
                        return;

                    propertyId = DmlRunPropertiesIds.Strikethrough;
                    break;
                }
                case FontAttr.AllCaps:
                {
                    if (properties.Capitalization != DmlCapitalization.All)
                        return;

                    propertyId = DmlRunPropertiesIds.Capitalization;
                    break;
                }
                case FontAttr.SmallCaps:
                {
                    if (properties.Capitalization != DmlCapitalization.Small)
                        return;

                    propertyId = DmlRunPropertiesIds.Capitalization;
                    break;
                }
                case FontAttr.Color:
                    propertyId = DmlRunPropertiesIds.Fill;
                    break;
                case FontAttr.HighlightColor:
                    propertyId = DmlRunPropertiesIds.HighlightColor;
                    break;
                case FontAttr.LocaleId:
                case FontAttr.LocaleIdBi:
                case FontAttr.LocaleIdFarEast:
                    propertyId = DmlRunPropertiesIds.Language;
                    break;
                case FontAttr.TextEffect:
                case FontAttr.Outline:
                case FontAttr.Shadow:
                case FontAttr.Engrave:
                case FontAttr.Scaling:
                case FontAttr.Emboss:
                case FontAttr.Hidden:
                    // These attributes cannot be applied to chart title.
                    return;
                case FontAttr.Position:
                case FontAttr.Bidi:
                case FontAttr.ComplexScript:
                case FontAttr.NoProofing:
                    // Currently there is no way to set these attributes inside DmlRunProperties.
                    return;
                default:
                    return;
            }

            properties.Remove(propertyId);
        }

        /// <summary>
        /// Returns font attribute id used for storing the specified language.
        /// </summary>
        private static int GetFontAttrLocaleId(Language language)
        {
            int locale = (int)language;

            if (LocaleClassifier.IsArabic(locale))
                return FontAttr.LocaleIdBi;
            if (LocaleClassifier.IsChineseOrJapanese(locale))
                return FontAttr.LocaleIdFarEast;

            return FontAttr.LocaleId;
        }

        /// <summary>
        /// Creates <see cref="ComplexFontName"/> for specified <see cref="DmlFont"/>.
        /// If font is null or typeface of the font is not set, default theme font is returned.
        /// </summary>
        private static ComplexFontName CreateComplexFontName(DmlFont font, ThemeFontCore defaultFont, Theme theme)
        {
            string typeface = (font != null) ? DmlTextFontRepository.GetTypeFace(font.TextTypeface, theme) : "";

            return StringUtil.HasChars(typeface)
                       ? ComplexFontName.FromName(typeface)
                       : ComplexFontName.FromTheme(defaultFont);
        }

        private static readonly int[] gConvertingFontAttributes = new[]
        {
            FontAttr.Size, FontAttr.Kerning, FontAttr.Spacing, FontAttr.NameAscii, FontAttr.NameBi,
            FontAttr.NameFarEast, FontAttr.NameOther, FontAttr.Bold, FontAttr.Italic, FontAttr.Underline,
            FontAttr.DoubleStrikeThrough, FontAttr.StrikeThrough, FontAttr.AllCaps, FontAttr.SmallCaps,
            FontAttr.VerticalAlignment, FontAttr.Color, FontAttr.EffectFill, FontAttr.HighlightColor,
            FontAttr.EffectOutline, FontAttr.EffectGlow, FontAttr.EffectShadow, FontAttr.EffectReflection,
            FontAttr.LocaleIdBi, FontAttr.LocaleIdFarEast, FontAttr.LocaleId, FontAttr.MathStyle
        };
    }
}
