// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2016 by Alexey Morozov

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Aspose.Drawing;
using Aspose.Fonts;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Lists;
using Aspose.Words.Revisions;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Formats human readable text for <see cref="FormatRevision" /> object.
    /// </summary>
    /// <remarks>
    /// AM. Output is not perfect for some complex cases yet (lists, tabs, etc) but all customer documents is OK and
    /// I think we can postpone complex cases for a while.
    /// </remarks>
    internal static class FormatRevisionText
    {
        internal static string GetText(FormatRevision revision, DocumentBase doc)
        {
            StringBuilder sb = new StringBuilder();
            WordAttrCollection pr = revision.RevPr;

            if (pr.Contains(ParaAttr.Istd))
            {
                Style style = doc.Styles.GetByIstd((int)pr[ParaAttr.Istd], false);
                Concat(sb, style.Name);
            }

            // Font.
            Concat(sb, FormatFont(pr, doc));

            // Paragraph formatting.
            Concat(sb, FormatParagraph(pr, doc));

            // Section formatting.
            SectPr revSectPr = pr as SectPr;
            if(revSectPr != null)
                Concat(sb, FormatSection(revSectPr));

            return sb.ToString();
        }

        /// <summary>
        /// Formats description for font format revision.
        /// </summary>
        private static string FormatFont(AttrCollection pr, DocumentBase doc)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTemp = new StringBuilder();

            if (pr.Contains(FontAttr.NameAscii))
            {
                ComplexFontName officeFont = (ComplexFontName) pr[FontAttr.NameAscii];

                Theme theme = doc.GetThemeInternal();
                string fontName = officeFont.Resolve(theme);
                if (officeFont.IsThemeFont)
                    fontName = string.Format((officeFont.ThemeFontCore & ThemeFontCore.GroupMask) == ThemeFontCore.Major ? "+Headings ({0})" : "+Body ({0})", fontName);

                // See "Test24237GroupText" for details.
                string currentOtherFont = GetOtherFontName(pr, theme);
                string defaultOtherFontName = GetOtherFontName(doc.Styles.DefaultRunPr, theme);

                if (pr.Contains(FontAttr.NameBi) ||
                    (currentOtherFont == defaultOtherFontName) ||
                    (FontUtil.FontSubstitutesCache[fontName] == defaultOtherFontName))
                {
                    fontName = string.Format("(Default) {0}", fontName);
                }

                Concat(sbTemp, fontName);
            }

            if (pr.Contains(FontAttr.Size))
                Concat(sbTemp, string.Format("{0} pt", ((int)pr[FontAttr.Size]) / 2));

            ConcatBoolEx(sbTemp, pr, FontAttr.Bold, "Bold");
            ConcatBoolEx(sbTemp, pr, FontAttr.Italic, "Italic");

            if (sbTemp.Length > 0)
                Concat(sb, string.Format("Font: {0}", sbTemp));

            if (pr.Contains(FontAttr.Underline))
                Concat(sb, FormatUnderline((Underline)pr[FontAttr.Underline]));

            if (pr.Contains(FontAttr.UnderlineColor))
                Concat(sb, string.Format("Underline color: {0}", FormatColor(pr[FontAttr.UnderlineColor], pr[FontAttr.UnderlineThemeColor])));

            if (pr.Contains(FontAttr.Color))
                Concat(sb, string.Format("Font color: {0}", FormatColor(pr[FontAttr.Color], pr[FontAttr.ThemeColor])));

            // Complex script.
            sbTemp.Length = 0;
            if (pr.Contains(FontAttr.NameBi))
            {
                ComplexFontName officeFont = (ComplexFontName)pr[FontAttr.NameBi];
                string fontName = officeFont.Resolve(doc.GetThemeInternal());
                if (officeFont.IsThemeFont)
                    fontName = string.Format((officeFont.ThemeFontCore & ThemeFontCore.GroupMask) == ThemeFontCore.Major ? "+Headings ({0})" : "+Body ({0})", fontName);

                Concat(sbTemp, fontName);
            }

            if (pr.Contains(FontAttr.SizeBi))
                Concat(sbTemp, string.Format("{0} pt", ((int)pr[FontAttr.SizeBi]) / 2));

            ConcatBoolEx(sbTemp, pr, FontAttr.BoldBi, "Bold");
            ConcatBoolEx(sbTemp, pr, FontAttr.ItalicBi, "Italic");

            if (sbTemp.Length > 0)
                Concat(sb, string.Format("Complex Script Font: {0}", sbTemp));

            ConcatBoolEx(sb, pr, FontAttr.StrikeThrough, "Strikethrough");
            ConcatBoolEx(sb, pr, FontAttr.DoubleStrikeThrough, "Double strikethrough");

            if (pr.Contains(FontAttr.VerticalAlignment))
                Concat(sb, FormatVerticalAlignment((RunVerticalAlignment)pr[FontAttr.VerticalAlignment]));

            ConcatBoolEx(sb, pr, FontAttr.SmallCaps, "Small caps");

            if (pr.Contains(FontAttr.Scaling))
                Concat(sb, string.Format("Character scale: {0}%", (int)pr[FontAttr.Scaling]));

            if (pr.Contains(FontAttr.Spacing))
                Concat(sb, string.Format("Expanded by {0} pt", ((int)pr[FontAttr.Spacing]) / 20));

            if (pr.Contains(FontAttr.Position))
                Concat(sb, string.Format("Raised by {0} pt", ((int)pr[FontAttr.Position]) / 2));

            if (pr.Contains(FontAttr.Kerning))
                Concat(sb, string.Format("Kern at {0} pt", ((int)pr[FontAttr.Kerning]) / 2));

            if (pr.Contains(FontAttr.Border))
                Concat(sb, string.Format("Border: {0}", FormatBorder("", (Border)pr[FontAttr.Border])));

            ConcatPropertyName(sb, pr, FontAttr.HighlightColor, "Highlight");

            // Text effects.
            StringBuilder sbEffects = new StringBuilder();
            ConcatPropertyName(sbEffects, pr, FontAttr.EffectOutline, "Outline");
            ConcatPropertyName(sbEffects, pr, FontAttr.EffectGlow, "Glow");
            ConcatPropertyName(sbEffects, pr, FontAttr.EffectReflection, "Reflection");
            ConcatPropertyName(sbEffects, pr, FontAttr.EffectFill, "Fill");
            ConcatPropertyName(sbEffects, pr, FontAttr.EffectProps3D, "Bevel");
            if (sbEffects.Length > 0)
                Concat(sb, string.Format("Text {0}", sbEffects));

            // Locale
            if (pr.Contains(FontAttr.LocaleId))
            {
                int localeId = (int)pr[FontAttr.LocaleId];

                CultureInfo cultureInfo = GetCultureInfoSafe(localeId);

                if(cultureInfo != null)
                    Concat(sb, cultureInfo.DisplayName);
            }

            return sb.ToString();
        }

        private static CultureInfo GetCultureInfoSafe(int localeId)
        {
            CultureInfo cultureInfo = null;
            try
            {
                cultureInfo = new CultureInfo(localeId, false);
            }
            catch
            {
            }

            return cultureInfo;
        }

        private static string GetOtherFontName(AttrCollection attrs, Theme theme)
        {
            if (!attrs.Contains(FontAttr.NameOther))
                return null;

            ComplexFontName fontNameComplex = (ComplexFontName)attrs[FontAttr.NameOther];
            string fontName = fontNameComplex.IsThemeFont ? fontNameComplex.Resolve(theme) : fontNameComplex.Name;

            return fontName;
        }

        /// <summary>
        /// Formats description for paragraph format revision.
        /// </summary>
        private static string FormatParagraph(AttrCollection pr, DocumentBase doc)
        {
            // FOSS: LayoutOptions/RevisionOptions removed; use the documented default (Centimeters)
            // that master's doc.FetchDocument().LayoutOptions.RevisionOptions.MeasurementUnit defaulted to.
            MeasurementUnits unit = MeasurementUnits.Centimeters;
            StringBuilder sb = new StringBuilder();

            // Paragraph formatting
            if (pr.Contains(ParaAttr.Alignment))
                Concat(sb, FormatAlignment((ParagraphAlignment)pr[ParaAttr.Alignment]));

            string indents = FormatIndent(pr, unit);
            if (StringUtil.HasChars(indents))
                Concat(sb, string.Format("Indent: {0}", indents));

            if (pr.Contains(ParaAttr.ListId))
                Concat(sb, FormatNumbering(pr, doc));

            StringBuilder sbTemp = new StringBuilder();
            if (pr.Contains(ParaAttr.TabStops))
            {
                TabStopCollection tabStops = (TabStopCollection)pr[ParaAttr.TabStops];

                for (int i = 0; i < tabStops.Count; i++)
                {
                    TabStop tab = tabStops[i];
                    Concat(sbTemp, FormatTabStop(tab, unit));
                }
                if (sbTemp.Length > 0)
                    Concat(sb, string.Format("Tab stops: {0}", sbTemp));
            }

            sbTemp.Length = 0;
            if (pr.Contains(ParaAttr.SpaceBefore))
                Concat(sbTemp, string.Format("Before: {0} pt", (int)pr[ParaAttr.SpaceBefore] / 20));

            if (pr.Contains(ParaAttr.SpaceAfter))
                Concat(sbTemp, string.Format("After: {0} pt", (int)pr[ParaAttr.SpaceAfter] / 20));

            if (sbTemp.Length > 0)
                Concat(sb, string.Format("Space {0}", sbTemp));

            if (pr.Contains(ParaAttr.LineSpacing))
                Concat(sb, string.Format("Line spacing: {0}", FormatLineSpacing((LineSpacing)pr[ParaAttr.LineSpacing])));

            ConcatBool(sb, pr, ParaAttr.AutoAdjustRightIndent, "Automatically adjust right indent when grid is defined", "Don't automatically adjust right indent when grid is defined");
            ConcatBool(sb, pr, ParaAttr.NoSpaceBetweenSameStyle, "Don't add space between paragraphs of the same style", "Add space between paragraphs of the same style");
            ConcatBool(sb, pr, ParaAttr.WidowControl, "Widow/orphan control", "No widow/orphan control");
            ConcatBool(sb, pr, ParaAttr.PageBreakBefore, "Page break before", "No page break before");
            ConcatBool(sb, pr, ParaAttr.KeepWithNext, "Keep with next", "Don't keep with next");
            ConcatBool(sb, pr, ParaAttr.KeepTogether, "Keep lines together", "Don't keep lines together");
            ConcatBool(sb, pr, ParaAttr.SuppressLineNumbers, "Suppress line numbers", "Don't suppress line numbers");
            ConcatBool(sb, pr, ParaAttr.SuppressAutoHyphens, "Don't hyphenate", "Hyphenate");
            ConcatBool(sb, pr, ParaAttr.FarEastLineBreakControl, "Use Asian rules to control first and last character", "Don't use Asian rules to control first and last character");
            ConcatBool(sb, pr, ParaAttr.WordWrap, "Don't allow text to wrap in the middle of a word", "Allow text to wrap in the middle of a word");
            ConcatBool(sb, pr, ParaAttr.HangingPunctuation, "Allow hanging punctuation", "Don't allow hanging punctuation");
            ConcatBool(sb, pr, ParaAttr.AddSpaceBetweenFarEastAndAlpha, "Adjust space between Latin and Asian text", "Don't adjust space between Latin and Asian text");
            ConcatBool(sb, pr, ParaAttr.AddSpaceBetweenFarEastAndDigit, "Adjust space between Asian text and numbers", "Don't adjust space between Asian text and numbers");
            ConcatBool(sb, pr, ParaAttr.SnapToGrid, "Snap to grid", "Don't snap to grid");
            ConcatBool(sb, pr, ParaAttr.MirrorIndents, "Don't swap indents on facing pages", "Not Don't swap indents on facing pages");
            ConcatBool(sb, pr, ParaAttr.TopLinePunctuation, "Compress initial punctuation", "Don't compress initial punctuation");

            sbTemp.Length = 0;
            if (pr.Contains(ParaAttr.BorderLeft) ||
                pr.Contains(ParaAttr.BorderRight) ||
                pr.Contains(ParaAttr.BorderTop) ||
                pr.Contains(ParaAttr.BorderBar) ||
                pr.Contains(ParaAttr.BorderBetween) ||
                pr.Contains(ParaAttr.BorderBottom))
            {
                Concat(sbTemp, FormatParaBorder(pr));
            }
            if (sbTemp.Length > 0)
                Concat(sb, string.Format("Border: {0}", sbTemp));

            if (pr.Contains(ParaAttr.Shading))
                Concat(sb, FormatShading((Shading)pr[ParaAttr.Shading]));


            return sb.ToString();
        }

        /// <summary>
        /// Formats description for section format revision.
        /// </summary>
        private static string FormatSection(SectPr revPr)
        {
            StringBuilder sb = new StringBuilder();

            if (revPr.Contains(SectAttr.LeftMargin))
                Concat(sb, string.Format("Left: {0} cm", TwipToCm(revPr.LeftMargin)));

            if (revPr.Contains(SectAttr.RightMargin))
                Concat(sb, string.Format("Right: {0} cm", TwipToCm(revPr.RightMargin)));

            if (revPr.Contains(SectAttr.TopMargin))
                Concat(sb, string.Format("Top: {0} cm", TwipToCm(revPr.TopMargin)));

            if (revPr.Contains(SectAttr.BottomMargin))
                Concat(sb, string.Format("Bottom: {0} cm", TwipToCm(revPr.BottomMargin)));

            if (revPr.Contains(SectAttr.PageWidth))
                Concat(sb, string.Format("Width: {0} cm", TwipToCm(revPr.PageWidth)));

            if (revPr.Contains(SectAttr.PageHeight))
                Concat(sb, string.Format("Height: {0} cm", TwipToCm(revPr.PageHeight)));

            if (revPr.Contains(SectAttr.ColumnsCount))
                Concat(sb, string.Format("Number of columns: {0}", revPr.ColumnsCount));

            if (revPr.Contains(SectAttr.SectionStart))
                Concat(sb, string.Format("Section start: {0}", FormatSectionStart(revPr.SectionStart)));

            return sb.ToString();
        }

        private static string FormatSectionStart(SectionStart sectionStart)
        {
            switch (sectionStart)
            {
                case SectionStart.NewPage: return "New page";
                case SectionStart.Continuous: return "Continuous";
                case SectionStart.EvenPage: return "Even page";
                case SectionStart.OddPage: return "Odd page";
                default: return "";
            }
        }

        private static string FormatNumbering(AttrCollection pr, DocumentBase doc)
        {
            int listId = (int)pr[ParaAttr.ListId];
            int listLevel = (int)pr.FetchAttr(ParaAttr.ListLevel);

            if (listId == 0)
            {
                return "No bullets or numbering";
            }

            List list = doc.Lists.GetListByListId(listId);
            ListLevel level = list.ListLevels[listLevel];

            // FOSS: LayoutOptions/RevisionOptions removed; use the documented default (Centimeters).
            MeasurementUnits unit = MeasurementUnits.Centimeters;

            int alignAt = (int)level.ParaPr.FetchAttr(ParaAttr.LeftIndent) +
                            (int)level.ParaPr.FetchAttr(ParaAttr.FirstLineIndent);

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Numbered + Level: {0} + ", listLevel + 1));
            sb.Append(string.Format("Numbering Style: {0} + ", FormatNumberStyle(level.NumberStyle)));
            sb.Append(string.Format("Aligned at: {0} + ", FormatMeasurementValue(unit, alignAt)));
            if ((level.ParaPr.TabStops!=null) && (level.ParaPr.TabStops.Count == 1))
                sb.Append(string.Format("Tab after: {0} cm + ", TwipToCm(level.ParaPr.TabStops[0].PositionTwips)));
            int leftIndent = (int)level.ParaPr.FetchAttr(ParaAttr.LeftIndent);
            sb.Append(string.Format("Indent at: {0}", FormatMeasurementValue(unit, leftIndent)));

            return sb.ToString();
        }

        private static string FormatNumberStyle(NumberStyle numberStyle)
        {
            switch (numberStyle)
            {
                case NumberStyle.UppercaseRoman: return "I, II, III, ...";
                case NumberStyle.Arabic: return "1, 2, 3, ...";
                case NumberStyle.Bullet: return "Bullet";

                default:
                    return "";
            }
        }

        private static void ConcatBoolEx(StringBuilder sb, AttrCollection pr, int key, string name)
        {
            object val = pr[key];
            if (val != null)
                Concat(sb, FormatBoolEx((AttrBoolEx)val, name));
        }

        private static void ConcatPropertyName(StringBuilder sb, AttrCollection pr, int key, string name)
        {
            if (pr.Contains(key))
                Concat(sb, name);
        }

        private static void ConcatBool(StringBuilder sb, AttrCollection pr, int key, string name, string notName)
        {
            object val = pr[key];
            if (val != null)
                Concat(sb, FormatBool((bool)val, name, notName));
        }

        private static string FormatIndent(AttrCollection pr, MeasurementUnits unit)
        {
            StringBuilder sb = new StringBuilder();
            if (pr.Contains(ParaAttr.LeftIndent))
            {
                int left = (int)pr[ParaAttr.LeftIndent] + (int)pr.FetchAttr(ParaAttr.FirstLineIndent);
                Concat(sb, string.Format("Left: {0}", FormatMeasurementValue(unit, left)));
            }
            if (pr.Contains(ParaAttr.RightIndent))
                Concat(sb, string.Format("Right: {0}", FormatMeasurementValue(unit, (int)pr[ParaAttr.RightIndent])));
            if (pr.Contains(ParaAttr.FirstLineIndent))
            {
                int firstLine = (int)pr[ParaAttr.FirstLineIndent];
                if (firstLine != 0)
                    Concat(sb, string.Format("{0}: {1} cm", firstLine > 0 ? "First Line" : "Hanging", TwipToCm(System.Math.Abs(firstLine))));
            }

            return sb.ToString();
        }

        private static string FormatTabAlignment(TabAlignment alignment)
        {
            switch (alignment)
            {
                case TabAlignment.Right: return "Right";
                case TabAlignment.List: return "List";

                default:
                    return "Left";
            }
        }

        private static string FormatTabStop(TabStop tab, MeasurementUnits unit)
        {
            return tab.IsClear
                ? string.Format("Not at {0}", FormatMeasurementValue(unit, tab.PositionTwips))
                : string.Format("{0}, {1}", FormatMeasurementValue(unit, tab.PositionTwips), FormatTabAlignment(tab.Alignment));
        }

        private static string FormatShading(Shading shading)
        {
            StringBuilder sb = new StringBuilder();

            if (shading.Texture == TextureIndex.TextureSolid)
            {
                Concat(sb, string.Format("Pattern: {0} ({1})",
                    FormatTextureIndex(shading.Texture),
                    FormatColor(shading.ForegroundPatternColorInternal, shading.ThemeColor)
                    ));

            }
            else if (shading.Texture == TextureIndex.TextureNone)
            {
                Concat(sb, string.Format("Pattern: {0} ({1})",
                    FormatTextureIndex(shading.Texture),
                    FormatColor(shading.BackgroundPatternColorInternal, shading.ThemeFill)
                    ));

            }
            else
            {
                Concat(sb, string.Format("Pattern: {0} ({1} Foreground, {2} Background)",
                    FormatTextureIndex(shading.Texture),
                    FormatColor(shading.ForegroundPatternColorInternal, shading.ThemeColor),
                    FormatColor(shading.BackgroundPatternColorInternal, shading.ThemeFill)
                    ));
            }

            return sb.ToString();
        }

        private static string FormatAlignment(ParagraphAlignment alignment)
        {
            switch (alignment)
            {
                case ParagraphAlignment.Left: return "Left";
                case ParagraphAlignment.Center: return "Centered";
                case ParagraphAlignment.Right: return "Right";

                default:
                    return "?";
            }
        }

        private static string FormatLineSpacing(LineSpacing lineSpacing)
        {
            switch (lineSpacing.Rule)
            {
                case LineSpacingRule.Multiple:
                    if (lineSpacing.Value == 240)
                        return "Single";

                    if (lineSpacing.Value == 480)
                        return "Double";

                    return string.Format("{0} lines", lineSpacing.Value / 240.0);

                default:
                    return string.Format("{0} {1} pt", (lineSpacing.Rule == LineSpacingRule.Exactly ? "Exactly" : "At least"), lineSpacing.Value / 20.0);
            }
        }

        private static string FormatVerticalAlignment(RunVerticalAlignment vertAlignment)
        {
            switch (vertAlignment)
            {
                case RunVerticalAlignment.Subscript: return "Subscript";
                case RunVerticalAlignment.Superscript: return "Superscript";
                case RunVerticalAlignment.Baseline: return "Baseline";

                default:
                    return "";
            }
        }

        private static string FormatBoolEx(AttrBoolEx val, string name)
        {
            return val.ToBool() ? name : string.Format("Not {0}", name);
        }

        private static string FormatBool(bool val, string name, string notName)
        {
            return val ? name : notName;
        }

        private static string FormatTextureIndex(TextureIndex textureIndex)
        {
            switch (textureIndex)
            {
                case TextureIndex.TextureSolid: return "Solid (100%)";
                case TextureIndex.TextureNone: return "Clear";
                case TextureIndex.Texture20Percent: return "20%";
                case TextureIndex.Texture62Pt5Percent: return "62.5%";
                case TextureIndex.Texture12Pt5Percent: return "12.5%";
                case TextureIndex.TextureDarkCross: return "Dk Grid";
                case TextureIndex.TextureCross: return "Lt Grid";
                case TextureIndex.TextureDarkHorizontal: return "Dk Horizontal";
                case TextureIndex.TextureDiagonalCross: return "Lt Trellis";

                default:
                    return "";
            }
        }

        private static string FormatLineStyle(LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.Single: return "Single solid line";
                case LineStyle.DashLargeGap: return "Dashed (large gap)";
                case LineStyle.Dot: return "Dotted";
                case LineStyle.DashSmallGap: return "Dashed (small gap)";
                case LineStyle.Triple:return "Triple solid lines";
                case LineStyle.DashDotStroker: return "Dash dot (stroked)";
                case LineStyle.DotDotDash: return "Dash dot dot";
                case LineStyle.DoubleWave: return "Double wavy";

                default:
                    return "";
            }
        }

        private static string FormatParaBorder(AttrCollection pr)
        {
            StringBuilder sb = new StringBuilder();

            Border between = (Border)pr[ParaAttr.BorderBetween];
            Border bar = (Border)pr[ParaAttr.BorderBar];

            Concat(sb, FormatBorder("Between", between));
            Concat(sb, FormatBorder("Bar", bar));

            Border left = (Border)pr[ParaAttr.BorderLeft];
            Border right = (Border)pr[ParaAttr.BorderRight];
            Border top = (Border)pr[ParaAttr.BorderTop];
            Border bottom = (Border)pr[ParaAttr.BorderBottom];

            if ((left != null) && AreEqual(left, right) && AreEqual(left, top) && AreEqual(left, bottom))
            {
                Concat(sb, FormatBorder("Box", left));
            }
            else
            {
                Concat(sb, FormatBorder("Top", top));
                Concat(sb, FormatBorder("Bottom", bottom));
                Concat(sb, FormatBorder("Left", left));
                Concat(sb, FormatBorder("Right", right));
            }

            return sb.ToString();
        }

        private static string FormatBorder(string name, Border border)
        {
            if (border == null)
                return "";

            return string.Format("{0}: ({1}{2}, {3}, {4} pt Line width)",
                name,
                border.Shadow ? "Shadowed " : "",
                FormatLineStyle(border.LineStyle),
                FormatColor(border.ColorInternal, border.ThemeColorInternal),
                border.LineWidth);
        }

        private static string FormatColor(object color, object themeColor)
        {
            if (themeColor == null)
            {
                DrColor drColor = (DrColor) color;
                if (drColor.IsEmpty)
                    return "Auto";

                string knownColorText = gKnownColors.GetValueOrNull((DrColor)color);

                return (StringUtil.HasChars(knownColorText))
                    ? knownColorText
                    : string.Format("Custom Color(RGB({0},{1},{2}))", drColor.R, drColor.G, drColor.B);
            }

            // Color is theme color.
            switch ((string) themeColor)
            {
                case "accent1": return "Accent 1";
                case "accent2": return "Accent 2";
                case "accent3": return "Accent 3";
                case "accent4": return "Accent 4";
                case "accent5": return "Accent 5";
                case "accent6": return "Accent 6";
                case "text1": return "Text 1";
                case "text2": return "Text 2";
                case "background1": return "Background 1";
                case "background2": return "Background 2";

                default:
                    return "";
            }
        }

        private static string FormatUnderline(Underline underline)
        {
            switch (underline)
            {
                case Underline.Single: return "Underline";
                case Underline.Double: return "Double underline";
                case Underline.Dash: return "Dashed underline";
                case Underline.Thick: return "Thick underline";
                case Underline.Dotted: return "Dotted underline";
                case Underline.DotDash: return "Dot-dash underline";
                case Underline.DotDotDash: return "Dot-dot-dash underline";
                case Underline.Wavy: return "Wave underline";
                case Underline.WavyDouble: return "Wave double underline";

                default:
                    return "";
            }
        }

        /// <summary>
        /// Concatenates text to list using comma as separator.
        /// </summary>
        private static void Concat(StringBuilder sb, string text)
        {
            if (!StringUtil.HasChars(text))
                return;

            if (sb.Length > 0)
                sb.Append(", ");

            sb.Append(text);
        }

        /// <summary>
        /// Custom equality method for borders. Ignores distance from text values.
        /// </summary>
        private static bool AreEqual(Border border1, Border border2)
        {
            if (border1 == null || border2 == null)
                return false;

            return
                (border1.LineStyle == border2.LineStyle) &&
                MathUtil.AreEqual(border1.LineWidth, border2.LineWidth) &&
                (border1.ColorInternal.Equals(border2.ColorInternal)) &&
                (border1.Frame == border2.Frame) &&
                (border1.Shadow == border2.Shadow) &&
                (border1.ThemeColorInternal == border2.ThemeColorInternal) &&
                (border1.ThemeShade == border2.ThemeShade) &&
                (border1.ThemeTint == border2.ThemeTint);
        }

        private static double TwipToCm(int value)
        {
            return System.Math.Round(ConvertUtilCore.TwipToMm(value) / 10, 2);
        }

        private static double TwipToMeasurementUnits(MeasurementUnits measurementUnit, int value)
        {
            switch (measurementUnit)
            {
                case MeasurementUnits.Centimeters:
                    return ConvertUtilCore.TwipToCm(value);
                case MeasurementUnits.Inches:
                    return ConvertUtilCore.TwipToInch(value);
                case MeasurementUnits.Millimeters:
                    return ConvertUtilCore.TwipToMm(value);
                case MeasurementUnits.Picas:
                    return ConvertUtilCore.TwipToPicas(value);
                case MeasurementUnits.Points:
                    return ConvertUtilCore.TwipToPoint(value);

                default:
                    Debug.Assert(false);
                    return double.NaN;
            }
        }

        private static string FormatMeasurementValue(MeasurementUnits measurementUnit, int value)
        {
            double convertedValue = TwipToMeasurementUnits(measurementUnit, value);

            switch (measurementUnit)
            {
                case MeasurementUnits.Centimeters:
                    return string.Format("{0} cm", System.Math.Round(convertedValue, 2));
                case MeasurementUnits.Inches:
                    return string.Format("{0}\"", System.Math.Round(convertedValue, 2));
                case MeasurementUnits.Millimeters:
                    return string.Format("{0} mm", System.Math.Round(convertedValue, 1));
                case MeasurementUnits.Picas:
                    return string.Format("{0} pi", System.Math.Round(convertedValue, 1));
                case MeasurementUnits.Points:
                    return string.Format("{0} pt", System.Math.Round(convertedValue, 0));

                default:
                    Debug.Assert(false);
                    return string.Empty;
            }
        }

        static FormatRevisionText()
        {
            gKnownColors.Add(DrColor.FromArgb(0xC0, 0x00, 0x00), "Dark Red");
            gKnownColors.Add(DrColor.FromArgb(0xFF, 0xFF, 0x00), "Yellow");
            gKnownColors.Add(DrColor.FromArgb(0xFF, 0x00, 0x00), "Red");
            gKnownColors.Add(DrColor.FromArgb(0x00, 0xB0, 0x50), "Green");
            gKnownColors.Add(DrColor.FromArgb(0x92, 0xD0, 0x50), "Light Green");
            gKnownColors.Add(DrColor.FromArgb(0x70, 0x30, 0xA0), "Purple");
            gKnownColors.Add(DrColor.FromArgb(0x00, 0x20, 0x60), "Dark Blue");
            gKnownColors.Add(DrColor.FromArgb(0x00, 0x70, 0xC0), "Blue");
            gKnownColors.Add(DrColor.FromArgb(0xff, 0xc0, 0x00), "Orange");
            gKnownColors.Add(DrColor.FromArgb(0x00, 0x00, 0x00), "Black");
        }

        private static readonly Dictionary<DrColor, string> gKnownColors = new Dictionary<DrColor, string>();
    }
}
