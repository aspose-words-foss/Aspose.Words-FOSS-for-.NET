// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2005 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using Aspose.Bidi;
using Aspose.Collections.Generic;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Various unsorted Word document related utility functions.
    /// Try to keep this to minimum and move methods into more suitable classes.
    /// </summary>
    internal static class WordUtil
    {
        /// <summary>
        /// Checks that style index is valid for Word document.
        /// </summary>
        internal static bool IsValidIstd(int istd)
        {
            return istd < StyleLimit;
        }

        /// <summary>
        /// Checks that style index is valid for Word document, otherwise logs warning.
        /// </summary>
        internal static bool CheckValidAndWarn(Style style, IWarningCallback warningCallback, WarningSource warningSource)
        {
            if (IsValidIstd(style.Istd))
                return true;

            if(warningCallback != null)
                warningCallback.Warning(
                    new WarningInfo(WarningType.DataLoss, warningSource, string.Format(WarningStrings.InvalidStyleIstd, style.Name)));

            return false;
        }

        /// <summary>
        /// Converts style index read from Word document to Aspose style index.
        /// </summary>
        internal static int ToAsposeIstd(int istd)
        {
            return (istd == StyleNil)
                ? StyleIndex.Nil
                : istd;
        }

        /// <summary>
        /// Converts Aspose style index to style index can be saved to Word document.
        /// </summary>
        internal static int ToWordIstd(int istd)
        {
            return (istd == StyleIndex.Nil)
                ? StyleNil
                : istd;
        }

        /// <summary>
        /// If the node is empty or the last child is not a paragraph, adds one empty paragraph.
        /// Needed to make some stories valid from MS Word point of view (e.g. body, cell, footnote, comment).
        /// </summary>
        internal static void EnsureNonEmptyStory(CompositeNode story)
        {
            if (IsAddParagraphNeeded(story))
                story.AppendChild(new Paragraph(story.Document));
        }

        internal static bool IsAddParagraphNeeded(CompositeNode story)
        {
            Node lastChild = story.LastNonAnnotationChild;

            if (lastChild == null)
                return true;

            switch (lastChild.NodeType)
            {
                case NodeType.Paragraph:
                    // Last child is a paragraph already, don't need to add one.
                    return false;
                case NodeType.StructuredDocumentTag:
                {
                    ((StructuredDocumentTag)lastChild).EnsureCorrectLastChild();
                    // I think recursion is well applicable here, just peer inside the sdt/customXml to find if it contains a paragraph or not.
                    return IsAddParagraphNeeded((CompositeNode)lastChild);
                }
                default:
                    return true;
            }
        }

        internal static bool ParagraphCanBeRemoved(Paragraph paragraph)
        {
            CompositeNode previousNode = paragraph.PreviousNonAnnotationSibling as CompositeNode;
            if (previousNode != null && (previousNode.NodeType == NodeType.Paragraph || !IsAddParagraphNeeded(previousNode)))
                return true;

            Node parent = paragraph;
            while (parent != null &&
                   parent.NodeType != NodeType.Body &&
                   parent.NodeType != NodeType.Cell &&
                   parent.NodeType != NodeType.Comment &&
                   parent.NodeType != NodeType.Footnote)
                parent = (parent.NextNonAnnotationSibling == null) ? parent.ParentNode : null;

            if (parent == null)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the character category (that determines what font name to use) for a specified character.
        ///
        /// The ranges of Unicode characters here are taken from
        /// http://en.wikipedia.org/wiki/Mapping_of_Unicode_characters
        /// and might not be perfect.
        /// See this resource for Unicode ranges:
        /// http://www.alanwood.net/unicode/index.html
        /// </summary>
        internal static CharacterCategory GetCharacterCategory(int c)
        {
            if ((c >= 0) && (c <= 127))
                return CharacterCategory.Ascii;

            if (c <= 0xffff && CharacterClassifier.IsCjk(c)) // CJK Unicode blocks.
                return CharacterCategory.FarEast;

            if (c > 0xffff && CharacterClassifier.IsSurrogateCjk(c)) // Extended CJK Unicode blocks (surrogate pairs).
                return CharacterCategory.FarEast;

            if ((c >= 0x0590) && (c <= 0x0dff))            // Range from Hebrew to Sinhala.
                return CharacterCategory.ComplexScript;

            if ((c >= 0xFB1D) && (c <= 0xFB4F))            // Hebrew Presentation Forms
                return CharacterCategory.ComplexScript;

            if ((c >= 0xFB50 && c <= 0xFDFF) ||            // Arabic Presentation Forms
                (c >= 0xFE70 && c <= 0xFEFF))
                return CharacterCategory.ComplexScript;

            // WORDSNET-17222 Add detection of the Thai Unicode range.
            if (CharacterClassifier.IsThai((char)c))
                return CharacterCategory.ComplexScript;

            return CharacterCategory.Other;
        }

        /// <summary>
        /// Returns category of character considering character category hint and complex script property.
        ///
        /// This method doesn't check whether Font.ComplexScript is true or not.
        /// This method is used in TextSplitter along with Font.ComplexScript value caching
        /// Because frequent calls of Font.ComplexScript is too expensive.
        /// </summary>
        internal static CharacterCategory GetCharacterCategoryIgnoreComplexScript(int c, Font font)
        {
            CharacterCategory charCategory = WordUtil.GetCharacterCategory(c);

            // AM. According to binary doc specification hint presence forces Word to use specified category when set but
            // according to ECMA-376 hint affects only ambiguous character. There are few files with ascii text has FarEast hint set and
            // Word really ignores the hint. So I couldn't understand what the ambiguity means exactly and
            // try to apply hint only for CharacterCategory.Other characters.
            // So far it seems to be OK and some golds accepted are better looking now.
            if (charCategory != CharacterCategory.Other)
                return charCategory;

            object hintValue = font.Parent.GetDirectRunAttr(FontAttr.CharacterCategoryHint);

            if (hintValue == null)
                return charCategory;

            CharacterCategory hint = (CharacterCategory)hintValue;

            if (hint == CharacterCategory.FarEast)
            {
                // Word ignores the FarEast hint for some Unicode ranges depending on ThemeFontFarEast or/and NameFarEast.
                // See WORDSNET-18373, WORDSNET-25643, WORDSNET-27859 for details.

                // Latin Extended-A (U+0100 - U+017F), Latin Extended-B (U+0180 - U+024F), IPA Extensions (U+0250-U+02AF).
                if (0x0100 <= c && c <= 0x02AF &&
                    font.ThemeFontFarEast == ThemeFont.None)
                {
                    return CharacterCategory.Other;
                }

                // Spacing Modifier Letters (U+02B0 - U+02FF), Combining Diacritical Marks (U+0300 - U+036F),
                // Greek and Coptic (U+0370 - U+03FF), Cyrillic (U+0400 - U+04FF)
                if (0x02B0 <= c && c <= 0x4FF &&
                    (font.NameFarEast == TimesNewRoman || font.NameFarEast == TimesNewRomanCYR) &&
                    font.ThemeFontFarEast == ThemeFont.None)
                {
                    return CharacterCategory.Other;
                }

                // Cyrillic Supplementary (U+0500 - u+052F)
                // Armenian (U+0530 - U+058F)
                if (0x0500 <= c && c <= 0x058F)
                {
                    return CharacterCategory.Other;
                }

                // When East Asian hint is specified for characters with CharacterCategory.Other
                // the ultimate CharacterCategory is resolved based on the East Asian width of characters.
                return EastAsianWidthResolver.IsWideCharacter(c, font.LocaleIdFarEast)
                    ? CharacterCategory.FarEast
                    : CharacterCategory.Other;
            }

            // Ignore CharacterCategory.ComplexScript hint if ComplexScript is not set.
            if (hint == CharacterCategory.ComplexScript)
                return CharacterCategory.Other;

            return hint;
        }

        /// <summary>
        /// Returns category of character considering character category hint and complex script property.
        /// </summary>
        internal static CharacterCategory GetCharacterCategory(int c, Font font)
        {
            // WORDSNET-8660 Character category should be ComplexScript when
            // Font.ComplexScript is true regardless of other properties. See ISO_IEC_29500-1, 17.3.2.7.
            if (font.ComplexScript)
                return CharacterCategory.ComplexScript;

            return GetCharacterCategoryIgnoreComplexScript(c, font);
        }

        /// <summary>
        /// Returns the complex font name for the specified character category.
        /// </summary>
        internal static ComplexFontName GetComplexFontNameByCategory(CharacterCategory characterCategory,
            ComplexFontName nameAscii, ComplexFontName nameBi, ComplexFontName nameFarEast, ComplexFontName nameOther)
        {
            switch (characterCategory)
            {
                case CharacterCategory.Ascii:
                    return nameAscii;
                case CharacterCategory.ComplexScript:
                    return nameBi;
                case CharacterCategory.FarEast:
                    return nameFarEast;
                case CharacterCategory.Other:
                default:
                    return nameOther;
            }
        }

        /// <summary>
        /// Returns the font name for the specified character category.
        /// </summary>
        internal static string GetFontNameByCategory(CharacterCategory characterCategory,
            string nameAscii, string nameBi, string nameFarEast, string nameOther)
        {
            switch (characterCategory)
            {
                case CharacterCategory.Ascii:
                    return nameAscii;
                case CharacterCategory.ComplexScript:
                    return nameBi;
                case CharacterCategory.FarEast:
                    return nameFarEast;
                case CharacterCategory.Other:
                default:
                    return nameOther;
            }
        }

        internal static string TrimDoubleQuotes(string s)
        {
            return TrimDoubleQuotes(s, true);
        }

        internal static string TrimDoubleQuotes(string s, bool isAllOccurrences)
        {
            if (isAllOccurrences)
                return s.Trim('\"');

            if (s.StartsWith("\"", StringComparison.Ordinal))
                s = s.Substring(1);
            if (s.EndsWith("\"", StringComparison.Ordinal))
                s = s.Substring(0, s.Length - 1);

            return s;
        }

        // Check all chars that is treated as quotation marks by Word.
        internal static bool IsInQuotationMarks(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 2)
                return false;

            return (s.IndexOfAny(gQuotationMarks, 0, 1) == 0) && (s.IndexOfAny(gQuotationMarks, s.Length - 1, 1) == s.Length - 1);
        }

        // Trim all chars that is treated as quotation marks by Word.
        internal static string TrimQuotationMarks(string s)
        {
            s = s.TrimStart(gQuotationMarks);
            s = s.TrimEnd(gQuotationMarks);

            return s;
        }

        internal static string AddDoubleQuotes(string s)
        {
            return String.Format("\"{0}\"", s);
        }

        /// <summary>
        /// Removes enclosing double quotes and replaces double backslashes with single ones.
        /// </summary>
        internal static string DecodeMSWordUri(string wordUri)
        {
            Debug.Assert(wordUri != null);
            wordUri = TrimDoubleQuotes(wordUri);
            wordUri = wordUri.Replace("\\\\", "\\");
            return wordUri;
        }

        /// <summary>
        /// Replaces single backslashes with double backslashes and adds double quotes around the string.
        /// </summary>
        internal static string EncodeMSWordUri(string uri)
        {
            Debug.Assert(uri != null);
            uri = uri.Replace("\\", "\\\\");
            uri = AddDoubleQuotes(uri);
            return uri;
        }

        /// <summary>
        /// Replaces CrLf combinations with Cr, replaces Lf characters with Cr.
        /// </summary>
        internal static string NormalizeToWord(string text)
        {
            // Performance optimization.
            if (text.IndexOf(ControlChar.LineFeedChar) == -1)
                return text;

            StringBuilder builder = new StringBuilder(text);
            builder.Replace(ControlChar.CrLf, ControlChar.Cr);
            builder.Replace(ControlChar.Lf, ControlChar.Cr);
            return builder.ToString();
        }

        /// <summary>
        /// Replaces CrLf or Cr with Lf.
        /// </summary>
        internal static string NormalizeToWordArt(string text)
        {
            StringBuilder builder = new StringBuilder(text);
            builder.Replace(ControlChar.CrLf, ControlChar.Lf);
            builder.Replace(ControlChar.Cr, ControlChar.Lf);
            return builder.ToString();
        }

        /// <summary>
        /// Formats a DateTime value using an MS Word date/time format string.
        /// </summary>
        internal static string FormatDateTime(DateTime d, string format)
        {
            const int EastAsianLanguageNotSet = 0;
            return FormatDateTime(d, format, EastAsianLanguageNotSet);
        }

        /// <summary>
        /// Formats a DateTime value using an MS Word date/time format string.
        /// </summary>
        internal static string FormatDateTime(DateTime d, string format, int eastAsianLanguageId)
        {
            return FormatDateTime(d, format, eastAsianLanguageId, CalendarType.Gregorian);
        }

        /// <summary>
        /// Formats a DateTime value using an MS Word date/time format string.
        /// </summary>
        internal static string FormatDateTime(DateTime d, string format, int eastAsianLanguageId, CalendarType calendarType)
        {
            return DateTimeFormat.FormatDateTime(format, d, eastAsianLanguageId, calendarType);
        }

        internal static StoryType HeaderFooterTypeToStoryType(HeaderFooterType type)
        {
            switch (type)
            {
                case HeaderFooterType.FooterEven:
                    return StoryType.EvenPagesFooter;
                case HeaderFooterType.FooterFirst:
                    return StoryType.FirstPageFooter;
                case HeaderFooterType.FooterPrimary:
                    return StoryType.PrimaryFooter;
                case HeaderFooterType.HeaderEven:
                    return StoryType.EvenPagesHeader;
                case HeaderFooterType.HeaderFirst:
                    return StoryType.FirstPageHeader;
                case HeaderFooterType.HeaderPrimary:
                    return StoryType.PrimaryHeader;
                default:
                    throw new InvalidOperationException("Unknown header or footer type.");
            }
        }

        internal static StoryType FootnoteSeparatorTypeToStoryType(FootnoteSeparatorType type)
        {
            switch (type)
            {
                case FootnoteSeparatorType.FootnoteSeparator:
                    return StoryType.FootnoteSeparator;
                case FootnoteSeparatorType.FootnoteContinuationSeparator:
                    return StoryType.FootnoteContinuationSeparator;
                case FootnoteSeparatorType.FootnoteContinuationNotice:
                    return StoryType.FootnoteContinuationNotice;
                case FootnoteSeparatorType.EndnoteSeparator:
                    return StoryType.EndnoteSeparator;
                case FootnoteSeparatorType.EndnoteContinuationSeparator:
                    return StoryType.EndnoteContinuationSeparator;
                case FootnoteSeparatorType.EndnoteContinuationNotice:
                    return StoryType.EndnoteContinuationNotice;
                default:
                    throw new InvalidOperationException("Unknown footnote or endnote separator type.");
            }
        }

        internal static HeaderFooterType StoryTypeToHeaderFooterType(StoryType type)
        {
            switch (type)
            {
                case StoryType.EvenPagesFooter:
                    return HeaderFooterType.FooterEven;
                case StoryType.FirstPageFooter:
                    return HeaderFooterType.FooterFirst;
                case StoryType.PrimaryFooter:
                    return HeaderFooterType.FooterPrimary;
                case StoryType.EvenPagesHeader:
                    return HeaderFooterType.HeaderEven;
                case StoryType.FirstPageHeader:
                    return HeaderFooterType.HeaderFirst;
                case StoryType.PrimaryHeader:
                    return HeaderFooterType.HeaderPrimary;
                default:
                    throw new InvalidOperationException("This story is not a header or footer story.");
            }
        }

        /// <summary>
        /// Returns paper size in twips.
        /// </summary>
        internal static Size PaperSizeToSize(PaperSize paperSize)
        {
            Debug.Assert(paperSize != PaperSize.Custom);
            return gPaperSizes[(int)paperSize];
        }

        /// <summary>
        /// Converts size in twips into paper size enumeration. Width and height must match the specified orientation, otherwise it will return custom paper size.
        /// </summary>
        internal static PaperSize SizeToPaperSize(int width, int height, bool isLandscape)
        {
            for (int i = 0; i < gPaperSizes.Length; i++)
            {
                if (i == (int)PaperSize.Custom)
                    continue;

                Size refSize = gPaperSizes[i];

                if (GeometryUtil.EqualsPaperSize(new SizeF(refSize.Width, refSize.Height), new SizeF(width, height), isLandscape, gPaperSizeTolerance))
                    return (PaperSize)i;
            }
            return PaperSize.Custom;
        }

        /// <summary>
        /// Returns a floating value which represents the filling level of the specified
        /// texture type.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// A value from 0.0 to 1.0 where 0 corresponds to 0% filling and 1.0 corresponds
        /// to 100% filling level.
        /// </returns>
        internal static double GetFillingLevelByTextureIndex(TextureIndex index)
        {
            switch (index)
            {
                case TextureIndex.TextureNone:
                case TextureIndex.TextureNil:
                    return 0.0;
                case TextureIndex.Texture2Pt5Percent:
                    return 0.025;
                case TextureIndex.Texture5Percent:
                    return 0.05;
                case TextureIndex.Texture7Pt5Percent:
                    return 0.075;
                case TextureIndex.Texture10Percent:
                    return 0.1f;
                case TextureIndex.Texture12Pt5Percent:
                    return 0.125;
                case TextureIndex.Texture15Percent:
                    return 0.15;
                case TextureIndex.Texture17Pt5Percent:
                    return 0.175;
                case TextureIndex.Texture20Percent:
                    return 0.2;
                case TextureIndex.Texture22Pt5Percent:
                    return 0.225;
                case TextureIndex.Texture25Percent:
                case TextureIndex.TextureHorizontal:
                case TextureIndex.TextureVertical:
                case TextureIndex.TextureDiagonalDown:
                case TextureIndex.TextureDiagonalUp:
                    return 0.25;
                case TextureIndex.Texture27Pt5Percent:
                    return 0.275;
                case TextureIndex.Texture30Percent:
                case TextureIndex.TextureDiagonalCross:
                    return 0.3;
                case TextureIndex.Texture32Pt5Percent:
                    return 0.325;
                case TextureIndex.Texture35Percent:
                case TextureIndex.TextureCross:
                    return 0.35;
                case TextureIndex.Texture37Pt5Percent:
                    return 0.375;
                case TextureIndex.Texture40Percent:
                    return 0.4;
                case TextureIndex.Texture42Pt5Percent:
                    return 0.425;
                case TextureIndex.Texture45Percent:
                    return 0.45;
                case TextureIndex.Texture47Pt5Percent:
                    return 0.475;
                case TextureIndex.Texture50Percent:
                case TextureIndex.TextureDarkHorizontal:
                case TextureIndex.TextureDarkVertical:
                case TextureIndex.TextureDarkDiagonalDown:
                case TextureIndex.TextureDarkDiagonalUp:
                case TextureIndex.TextureDarkCross:
                    return 0.5;
                case TextureIndex.Texture52Pt5Percent:
                    return 0.525;
                case TextureIndex.Texture55Percent:
                    return 0.55;
                case TextureIndex.Texture57Pt5Percent:
                    return 0.575;
                case TextureIndex.Texture60Percent:
                    return 0.6;
                case TextureIndex.Texture62Pt5Percent:
                    return 0.625;
                case TextureIndex.Texture65Percent:
                    return 0.65;
                case TextureIndex.Texture67Pt5Percent:
                    return 0.675;
                case TextureIndex.Texture70Percent:
                case TextureIndex.TextureDarkDiagonalCross:
                    return 0.7;
                case TextureIndex.Texture72Pt5Percent:
                    return 0.725;
                case TextureIndex.Texture75Percent:
                    return 0.75;
                case TextureIndex.Texture77Pt5Percent:
                    return 0.775;
                case TextureIndex.Texture80Percent:
                    return 0.8;
                case TextureIndex.Texture82Pt5Percent:
                    return 0.825;
                case TextureIndex.Texture85Percent:
                    return 0.85;
                case TextureIndex.Texture87Pt5Percent:
                    return 0.875;
                case TextureIndex.Texture90Percent:
                    return 0.9;
                case TextureIndex.Texture92Pt5Percent:
                    return 0.925;
                case TextureIndex.Texture95Percent:
                    return 0.95;
                case TextureIndex.Texture97Pt5Percent:
                    return 0.975;
                case TextureIndex.TextureSolid:
                    return 1.0;
                default:
                    throw new InvalidOperationException("Unknown texture.");
            }
        }

        /// <summary>
        /// Validates the specified texture index against the available patterns and converts
        /// the invalid indices into <see cref="TextureIndex.TextureNone"/> just like MS Word does.
        /// </summary>
        internal static TextureIndex ValidateTextureIndex(TextureIndex index)
        {
            switch (index)
            {
                case TextureIndex.TextureNone:
                case TextureIndex.TextureNil:
                case TextureIndex.Texture2Pt5Percent:
                case TextureIndex.Texture5Percent:
                case TextureIndex.Texture7Pt5Percent:
                case TextureIndex.Texture10Percent:
                case TextureIndex.Texture12Pt5Percent:
                case TextureIndex.Texture15Percent:
                case TextureIndex.Texture17Pt5Percent:
                case TextureIndex.Texture20Percent:
                case TextureIndex.Texture22Pt5Percent:
                case TextureIndex.Texture25Percent:
                case TextureIndex.TextureHorizontal:
                case TextureIndex.TextureVertical:
                case TextureIndex.TextureDiagonalDown:
                case TextureIndex.TextureDiagonalUp:
                case TextureIndex.Texture27Pt5Percent:
                case TextureIndex.Texture30Percent:
                case TextureIndex.TextureDiagonalCross:
                case TextureIndex.Texture32Pt5Percent:
                case TextureIndex.Texture35Percent:
                case TextureIndex.TextureCross:
                case TextureIndex.Texture37Pt5Percent:
                case TextureIndex.Texture40Percent:
                case TextureIndex.Texture42Pt5Percent:
                case TextureIndex.Texture45Percent:
                case TextureIndex.Texture47Pt5Percent:
                case TextureIndex.Texture50Percent:
                case TextureIndex.TextureDarkHorizontal:
                case TextureIndex.TextureDarkVertical:
                case TextureIndex.TextureDarkDiagonalDown:
                case TextureIndex.TextureDarkDiagonalUp:
                case TextureIndex.TextureDarkCross:
                case TextureIndex.Texture52Pt5Percent:
                case TextureIndex.Texture55Percent:
                case TextureIndex.Texture57Pt5Percent:
                case TextureIndex.Texture60Percent:
                case TextureIndex.Texture62Pt5Percent:
                case TextureIndex.Texture65Percent:
                case TextureIndex.Texture67Pt5Percent:
                case TextureIndex.Texture70Percent:
                case TextureIndex.TextureDarkDiagonalCross:
                case TextureIndex.Texture72Pt5Percent:
                case TextureIndex.Texture75Percent:
                case TextureIndex.Texture77Pt5Percent:
                case TextureIndex.Texture80Percent:
                case TextureIndex.Texture82Pt5Percent:
                case TextureIndex.Texture85Percent:
                case TextureIndex.Texture87Pt5Percent:
                case TextureIndex.Texture90Percent:
                case TextureIndex.Texture92Pt5Percent:
                case TextureIndex.Texture95Percent:
                case TextureIndex.Texture97Pt5Percent:
                case TextureIndex.TextureSolid:
                    return index;
                default:
                    return TextureIndex.TextureNone;
            }
        }

        internal static bool IsComplexTexture(TextureIndex index)
        {
            switch (index)
            {
                case TextureIndex.TextureHorizontal:
                case TextureIndex.TextureVertical:
                case TextureIndex.TextureDiagonalDown:
                case TextureIndex.TextureDiagonalUp:
                case TextureIndex.TextureDiagonalCross:
                case TextureIndex.TextureCross:
                case TextureIndex.TextureDarkHorizontal:
                case TextureIndex.TextureDarkVertical:
                case TextureIndex.TextureDarkDiagonalDown:
                case TextureIndex.TextureDarkDiagonalUp:
                case TextureIndex.TextureDarkCross:
                case TextureIndex.TextureDarkDiagonalCross:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a TextureIndex enumeration member which corresponds to the specified filling level.
        /// </summary>
        /// <param name="value">
        /// A value from 0.0 to 1.0 where 0 corresponds to 0% filling and 1.0 corresponds
        /// to 100% filling level.
        /// </param>
        /// <returns></returns>
        internal static TextureIndex GetTextureIndexByFillingLevel(double value)
        {
            switch ((int)(40 * value))
            {
                case 0: return TextureIndex.TextureNone;
                case 1: return TextureIndex.Texture2Pt5Percent;
                case 2: return TextureIndex.Texture5Percent;
                case 3: return TextureIndex.Texture7Pt5Percent;
                case 4: return TextureIndex.Texture10Percent;
                case 5: return TextureIndex.Texture12Pt5Percent;
                case 6: return TextureIndex.Texture15Percent;
                case 7: return TextureIndex.Texture17Pt5Percent;
                case 8: return TextureIndex.Texture20Percent;
                case 9: return TextureIndex.Texture22Pt5Percent;
                case 10: return TextureIndex.Texture25Percent;
                case 11: return TextureIndex.Texture27Pt5Percent;
                case 12: return TextureIndex.Texture30Percent;
                case 13: return TextureIndex.Texture32Pt5Percent;
                case 14: return TextureIndex.Texture35Percent;
                case 15: return TextureIndex.Texture37Pt5Percent;
                case 16: return TextureIndex.Texture40Percent;
                case 17: return TextureIndex.Texture42Pt5Percent;
                case 18: return TextureIndex.Texture45Percent;
                case 19: return TextureIndex.Texture47Pt5Percent;
                case 20: return TextureIndex.Texture50Percent;
                case 21: return TextureIndex.Texture52Pt5Percent;
                case 22: return TextureIndex.Texture55Percent;
                case 23: return TextureIndex.Texture57Pt5Percent;
                case 24: return TextureIndex.Texture60Percent;
                case 25: return TextureIndex.Texture62Pt5Percent;
                case 26: return TextureIndex.Texture65Percent;
                case 27: return TextureIndex.Texture67Pt5Percent;
                case 28: return TextureIndex.Texture70Percent;
                case 29: return TextureIndex.Texture72Pt5Percent;
                case 30: return TextureIndex.Texture75Percent;
                case 31: return TextureIndex.Texture77Pt5Percent;
                case 32: return TextureIndex.Texture80Percent;
                case 33: return TextureIndex.Texture82Pt5Percent;
                case 34: return TextureIndex.Texture85Percent;
                case 35: return TextureIndex.Texture87Pt5Percent;
                case 36: return TextureIndex.Texture90Percent;
                case 37: return TextureIndex.Texture92Pt5Percent;
                case 38: return TextureIndex.Texture95Percent;
                case 39: return TextureIndex.Texture97Pt5Percent;
                default: return TextureIndex.TextureSolid;
            }
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static WordUtil()
        {
            //See http://www.cl.cam.ac.uk/~mgk25/iso-paper.html for more info on paper sizes.
            //Also http://en.wikipedia.org/wiki/Paper_size
            // WORDSNET-19250 Corrected values for Folio, Ledger and Quarto.
            int paperSizeCount = EnumUtilPal.GetEffectiveArrayLength(PaperSize.A3.GetType(), 20);
            gPaperSizes = new Size[paperSizeCount];
            gPaperSizes[(int)PaperSize.A3] = ConvertUtilCore.MmToTwip(297, 420);
            gPaperSizes[(int)PaperSize.A4] = ConvertUtilCore.MmToTwip(210, 297);
            gPaperSizes[(int)PaperSize.A5] = ConvertUtilCore.MmToTwip(148, 210);
            gPaperSizes[(int)PaperSize.B4] = ConvertUtilCore.MmToTwip(250, 353);
            gPaperSizes[(int)PaperSize.B5] = ConvertUtilCore.MmToTwip(176, 250);
            gPaperSizes[(int)PaperSize.Executive] = ConvertUtilCore.InchToTwip(7.25, 10.5);
            gPaperSizes[(int)PaperSize.Folio] = ConvertUtilCore.InchToTwip(8.5, 13);
            gPaperSizes[(int)PaperSize.Ledger] = ConvertUtilCore.InchToTwip(17, 11);
            gPaperSizes[(int)PaperSize.Legal] = ConvertUtilCore.InchToTwip(8.5, 14);
            gPaperSizes[(int)PaperSize.Letter] = ConvertUtilCore.InchToTwip(8.5, 11);
            gPaperSizes[(int)PaperSize.EnvelopeDL] = ConvertUtilCore.MmToTwip(110, 220);
            gPaperSizes[(int)PaperSize.Quarto] = ConvertUtilCore.InchToTwip(8.47, 10.83);
            gPaperSizes[(int)PaperSize.Statement] = ConvertUtilCore.InchToTwip(5.5, 8.5);
            gPaperSizes[(int)PaperSize.Tabloid] = ConvertUtilCore.InchToTwip(11, 17);
            gPaperSizes[(int)PaperSize.Paper10x14] = ConvertUtilCore.InchToTwip(10, 14);
            gPaperSizes[(int)PaperSize.Paper11x17] = ConvertUtilCore.InchToTwip(11, 17);
            gPaperSizes[(int)PaperSize.Number10Envelope] = ConvertUtilCore.InchToTwip(4.125, 9.5);
            gPaperSizes[(int)PaperSize.JisB4] = ConvertUtilCore.MmToTwip(257, 364);
            gPaperSizes[(int)PaperSize.JisB5] = ConvertUtilCore.MmToTwip(182, 257);
        }

        private static readonly Size[] gPaperSizes;

        //Paper size tolerance is said to be 1.5mm for dimensions up to 150mm and 2mm above. This is 2mm tolerance.
        private static readonly int gPaperSizeTolerance = ConvertUtilCore.MmToTwip(2);

        /// <summary>
        /// Maximum font size in points allowed by MS Word.
        /// </summary>
        public const double MaxFontSize = 1638.0;

        /// <summary>
        /// Maximal style index allowed by MS Word.
        /// </summary>
        /// <remarks>
        /// According to spec maximum style count written to DOC format MUST be less than this value.
        /// This gives us maximum style index to be this value - 1 (Istd is zero based).
        ///
        /// It is interesting that other than DOC Word document formats allow greater values for style index but
        /// it's better to have unified behavior here.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int StyleLimit = (0x0FFE - 1);

        /// <summary>
        /// Style index used as Nil by MS Word.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int StyleNil = 0x0FFF;

        /// <summary>
        /// HeadersFooters sort order.
        /// </summary>
        private static readonly int[] gHeaderSortOrder =
            new int[]
            {
                2, /* HeaderEven */
                1, /* HeaderPrimary */
                5, /* FooterEven */
                4, /* FooterPrimary */
                0, /* HeaderFirst */
                3, /* FooterFirst */
            };

        /// <summary>
        /// The hash of Aspose.Words.Resources.AsposeLogo.png, generated with SHA-256.
        /// </summary>
        private static readonly byte[] gAsposeLogoHash = new byte[]
        {
            0xFF, 0x0B, 0xA8, 0xE6, 0xF5, 0x6B, 0x06, 0x1F, 0xAC, 0x6F, 0x11, 0x12, 0xC4, 0x3C, 0x2C, 0x14,
            0xD9, 0xB5, 0x41, 0x53, 0x20, 0x4B, 0x14, 0x0A, 0xF9, 0xE1, 0xFA, 0x40, 0xC6, 0x10, 0x4B, 0x34
        };

        private static readonly char[] gQuotationMarks = new char[] { '\"', '«', '»', '“', '”', '„', '‟' };

        private const string TimesNewRoman = "Times New Roman";
        private const string TimesNewRomanCYR = "Times New Roman CYR";
    }
}
