// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2020 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Fields
{
    internal static class FieldIndexFormatApplier
    {
        private static FieldIndexFormatDefinition DefineClassicFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 18);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Bold()
                .Size(26)
                .Name(ThemeFontCore.MinorHAnsi);

            headingStyle.ParaPr.Alignment = ParagraphAlignment.Center;
            headingStyle.ParaPr.SpaceBefore = 240;
            headingStyle.ParaPr.SpaceAfter = 120;

            return format;
        }

        private static FieldIndexFormatDefinition DefineFancyFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 18);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Bold()
                .Name(ThemeFontCore.MajorHAnsi);

            headingStyle.ParaPr.Alignment = ParagraphAlignment.Center;
            headingStyle.ParaPr.SpaceBefore = 240;
            headingStyle.ParaPr.SpaceAfter = 120;

            Border border = new Border();
            border.LineStyle = LineStyle.Double;
            border.LineWidth = 0.75;
            border.Shadow = true;
            headingStyle.ParaPr.BorderTop = border;
            headingStyle.ParaPr.BorderRight = border;
            headingStyle.ParaPr.BorderBottom = border;
            headingStyle.ParaPr.BorderLeft = border;

            return format;
        }

        private static FieldIndexFormatDefinition DefineModernFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 18);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Bold()
                .Italic()
                .Size(26)
                .Name(ThemeFontCore.MinorHAnsi);

            headingStyle.ParaPr.SpaceBefore = 360;
            headingStyle.ParaPr.SpaceAfter = 240;

            Border border = new Border();
            border.LineStyle = LineStyle.Single;
            border.LineWidth = 1.5;
            headingStyle.ParaPr.BorderTop = border;

            return format;
        }

        private static FieldIndexFormatDefinition DefineBulletedFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 18);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Bold()
                .Size(28)
                .Name(ThemeFontCore.MajorHAnsi);

            headingStyle.ParaPr.LeftIndent = 140;
            headingStyle.ParaPr.SpaceBefore = 240;
            headingStyle.ParaPr.SpaceAfter = 120;

            return format;
        }

        private static FieldIndexFormatDefinition DefineFormalFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 20);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Bold()
                .Italic()
                .Size(20)
                .Name(ThemeFontCore.MinorHAnsi);

            headingStyle.ParaPr.SpaceBefore = 120;
            headingStyle.ParaPr.SpaceAfter = 120;

            return format;
        }

        private static FieldIndexFormatDefinition DefineSimpleFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonAttrs(format, 20);

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading]
                .Size(20)
                .Name(ThemeFontCore.MinorHAnsi);

            headingStyle.ParaPr.SpaceAfter = 0;

            return format;
        }

        private static FieldIndexFormatDefinition DefineTemplateFormat()
        {
            FieldIndexFormatDefinition format = new FieldIndexFormatDefinition();

            DefineCommonParaPrAttrs(format);

            format.SetEntriesParaPr(ParaAttr.LineSpacing, new LineSpacing(240, LineSpacingRule.Multiple));

            FieldIndexFormatStyleDefinition headingStyle = format.Styles[StyleIdentifier.IndexHeading].Bold();
            headingStyle.RunPr.ComplexNameAscii = ComplexFontName.FromTheme(ThemeFontCore.MajorHAnsi);
            headingStyle.RunPr.ComplexNameFarEast = ComplexFontName.FromTheme(ThemeFontCore.MajorEastAsia);
            headingStyle.RunPr.ComplexNameOther = ComplexFontName.FromTheme(ThemeFontCore.MajorHAnsi);
            headingStyle.RunPr.ComplexNameBi = ComplexFontName.FromTheme(ThemeFontCore.MajorBidi);

            return format;
        }

        private static void DefineCommonAttrs(FieldIndexFormatDefinition format, int fontSize)
        {
            DefineCommonRunPrAttrs(format, fontSize);
            DefineCommonParaPrAttrs(format);
        }

        private static void DefineCommonRunPrAttrs(FieldIndexFormatDefinition format, int fontSize)
        {
            format.SetEntriesRunPr(FontAttr.Size, fontSize);
            format.SetEntriesRunPr(FontAttr.SizeBi, fontSize);
            format.SetEntriesRunPr(FontAttr.NameAscii, ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi));
            format.SetEntriesRunPr(FontAttr.NameOther, ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi));
            format.SetEntriesRunPr(FontAttr.NameBi, ComplexFontName.FromTheme(ThemeFontCore.MinorHAnsi));
        }

        private static void DefineCommonParaPrAttrs(FieldIndexFormatDefinition format)
        {
            format.SetEntriesParaPr(ParaAttr.FirstLineIndent, -220);
            format.SetEntriesParaPr(ParaAttr.SpaceAfter, 0);

            format.Styles[StyleIdentifier.Index1].ParaPr.LeftIndent = 220;
            format.Styles[StyleIdentifier.Index2].ParaPr.LeftIndent = 440;
            format.Styles[StyleIdentifier.Index3].ParaPr.LeftIndent = 660;
            format.Styles[StyleIdentifier.Index4].ParaPr.LeftIndent = 880;
            format.Styles[StyleIdentifier.Index5].ParaPr.LeftIndent = 1100;
            format.Styles[StyleIdentifier.Index6].ParaPr.LeftIndent = 1320;
            format.Styles[StyleIdentifier.Index7].ParaPr.LeftIndent = 1540;
            format.Styles[StyleIdentifier.Index8].ParaPr.LeftIndent = 1760;
            format.Styles[StyleIdentifier.Index9].ParaPr.LeftIndent = 1980;
        }

        internal static void Apply(Document document, FieldIndexFormat format)
        {
            FieldIndexFormatDefinition definition = gDefinitions[format];
            foreach (KeyValuePair<StyleIdentifier, FieldIndexFormatStyleDefinition> pair in definition.Styles)
            {
                Style documentStyle = document.Styles[pair.Key];
                documentStyle.RunPr = pair.Value.RunPr.Clone();
                documentStyle.ParaPr = pair.Value.ParaPr.Clone();
            }
        }

        internal static FieldIndexFormat Identify(Document document)
        {
            foreach (KeyValuePair<FieldIndexFormat, FieldIndexFormatDefinition> pair in gDefinitions)
            {
                if (pair.Key == FieldIndexFormat.Template)
                    continue;

                if (IsFormatApplied(document, pair.Value))
                    return pair.Key;
            }

            return FieldIndexFormat.Template;
        }

        private static bool IsFormatApplied(DocumentBase document, FieldIndexFormatDefinition formatDefinition)
        {
            foreach (KeyValuePair<StyleIdentifier, FieldIndexFormatStyleDefinition> pair in formatDefinition.Styles)
            {
                Style documentStyle = document.Styles[pair.Key];

                if (!IsFormatApplied(
                    new StyleRunPrAttrSource(documentStyle),
                    pair.Value.RunPr,
                    FontAttr.NameAscii,
                    FontAttr.NameOther,
                    FontAttr.NameBi,
                    FontAttr.SizeBi,
                    FontAttr.BoldBi,
                    FontAttr.ItalicBi))
                {
                    return false;
                }

                if (!IsFormatApplied(
                    new StyleParaPrAttrSource(documentStyle),
                    pair.Value.ParaPr,
                    ParaAttr.LeftIndent,
                    ParaAttr.FirstLineIndent))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFormatApplied(IAttrSource style, AttrCollection format, params int[] ignoredKeys)
        {
            for (int i = 0; i < format.Count; i++)
            {
                int key = format.GetKey(i);
                if (Array.IndexOf(ignoredKeys, key) != -1)
                    continue;

                object expectedValue = format[key];
                object actualValue = style.FetchAttr(key);
                if (!expectedValue.Equals(actualValue))
                    return false;
            }

            return true;
        }

        private static readonly IDictionary<FieldIndexFormat, FieldIndexFormatDefinition> gDefinitions = new Dictionary<FieldIndexFormat, FieldIndexFormatDefinition>
        {
            { FieldIndexFormat.Classic, DefineClassicFormat() },
            { FieldIndexFormat.Fancy, DefineFancyFormat() },
            { FieldIndexFormat.Modern, DefineModernFormat() },
            { FieldIndexFormat.Bulleted, DefineBulletedFormat() },
            { FieldIndexFormat.Formal, DefineFormalFormat() },
            { FieldIndexFormat.Simple, DefineSimpleFormat() },
            { FieldIndexFormat.Template, DefineTemplateFormat() }
        };

        private class FieldIndexFormatDefinition
        {
            internal FieldIndexFormatDefinition()
            {
                Styles = new Dictionary<StyleIdentifier, FieldIndexFormatStyleDefinition>();
                Styles[StyleIdentifier.IndexHeading] = new FieldIndexFormatStyleDefinition();
                for (int i = (int)StyleIdentifier.Index1; i <= (int)StyleIdentifier.Index9; i++)
                    Styles[(StyleIdentifier)i] = new FieldIndexFormatStyleDefinition();
            }

            internal IDictionary<StyleIdentifier, FieldIndexFormatStyleDefinition> Styles { get; }

            internal void SetEntriesRunPr(int key, object value)
            {
                foreach (KeyValuePair<StyleIdentifier, FieldIndexFormatStyleDefinition> pair in Styles)
                {
                    if (pair.Key == StyleIdentifier.IndexHeading)
                        continue;

                    pair.Value.RunPr[key] = value;
                }
            }

            internal void SetEntriesParaPr(int key, object value)
            {
                foreach (KeyValuePair<StyleIdentifier, FieldIndexFormatStyleDefinition> pair in Styles)
                {
                    if (pair.Key == StyleIdentifier.IndexHeading)
                        continue;

                    pair.Value.ParaPr[key] = value;
                }
            }
        }

        private class FieldIndexFormatStyleDefinition
        {
            internal FieldIndexFormatStyleDefinition()
            {
                RunPr = new RunPr();
                ParaPr = new ParaPr();
            }

            internal RunPr RunPr { get; }
            internal ParaPr ParaPr { get; }

            internal FieldIndexFormatStyleDefinition Bold()
            {
                RunPr.Bold = AttrBoolEx.True;
                RunPr.BoldBi = AttrBoolEx.True;
                return this;
            }

            internal FieldIndexFormatStyleDefinition Italic()
            {
                RunPr.Italic = AttrBoolEx.True;
                RunPr.ItalicBi = AttrBoolEx.True;
                return this;
            }

            internal FieldIndexFormatStyleDefinition Size(int size)
            {
                RunPr.Size = size;
                RunPr.SizeBi = size;
                return this;
            }

            internal FieldIndexFormatStyleDefinition Name(ThemeFontCore font)
            {
                RunPr.ComplexNameAscii = ComplexFontName.FromTheme(font);
                RunPr.ComplexNameOther = ComplexFontName.FromTheme(font);
                RunPr.ComplexNameBi = ComplexFontName.FromTheme(font);
                return this;
            }
        }

        private interface IAttrSource
        {
            object FetchAttr(int key);
        }

        private class StyleRunPrAttrSource : IAttrSource
        {
            public StyleRunPrAttrSource(Style style)
            {
                mStyle = style;
            }

            public object FetchAttr(int key)
            {
                return mStyle.GetFontAttr(key, true);
            }

            private readonly Style mStyle;
        }

        private class StyleParaPrAttrSource : IAttrSource
        {
            public StyleParaPrAttrSource(Style style)
            {
                mStyle = style;
            }

            public object FetchAttr(int key)
            {
                return mStyle.GetParaAttr(key, RevisionsView.Final) ?? mStyle.Styles.DefaultParaPr.FetchAttr(key);
            }

            private readonly Style mStyle;
        }
    }
}
