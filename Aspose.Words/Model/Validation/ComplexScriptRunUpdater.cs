// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2017 by Maxim Glazyrin
using System.Collections.Generic;
using Aspose.Bidi;
using Aspose.Fonts.TrueType;
using Aspose.Words.Fonts;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Extracts runs with certain characters and ensures font/locale/complex script is set.
    /// </summary>
    /// <remarks>
    /// AM. For unknown reason Word fails to detect characters from certain Unicode blocks without additional information. 
    /// Sometimes it requires proper locale to be set, for others requires both complex script and proper font name. 
    /// This class tries to mimic such behavior.
    /// </remarks>
    internal static class ComplexScriptRunUpdater
    {
        /// <summary>
        /// Extracts runs with certain characters and applies required formatting.
        /// </summary>
        /// <param name="run">Run to process.</param>
        internal static void DoProcess(Run run)
        {
            CharacterInfo lastInfo = null;

            int i = 0;
            while (i < run.GetTextLength())
            {
                string text = run.Text;

                // WORDSNET-19668 Try to get information about the surrogate char, which is an emoji.
                CharacterInfo thisInfo = TryGetSurrogateCharacterInfo(run, i);
                bool surrogate = thisInfo != null;

                if (!surrogate)
                    thisInfo = GetCharacterInfo(text[i]);

                if (lastInfo != thisInfo)
                {
                    Run newRun = run.SplitBefore(i);
                    i = 0;

                    if (lastInfo != null)
                        ForceRunAttributes(newRun, lastInfo);
                }

                // Increase because the surrogate char consists of two Unicode chars.
                if (surrogate)
                    i++;
                i++;

                lastInfo = thisInfo;
            }

            if (lastInfo != null)
                ForceRunAttributes(run, lastInfo);
        }

        /// <summary>
        /// Sets required attributes for run to be correctly detected by Word.
        /// </summary>
        private static void ForceRunAttributes(Run run, CharacterInfo charInfo)
        {
            bool isComplexScript = InlineHelper.GetBool(run, FontAttr.ComplexScript);

            if (charInfo.SetComplexScript)
            {
                // WORDSNET-16910 Update size and formatting, when the "ComplexScript" attribute is not set.
                if (!isComplexScript)
                    UpdateComplexScriptFormatting(run);

                run.RunPr.SetAttr(FontAttr.ComplexScript, AttrBoolEx.True);
            }

            if (charInfo.Language != Language.LanguageNotSet)
                run.RunPr.SetAttr(FontAttr.LocaleIdBi, (int)charInfo.Language);

            // Change font info in run
            if ((charInfo.FontName == run.Font.Name) || (run.GetTextLength() == 0))
            {
                // If required font is already set or text is empty ignore run.
                return;
            }

            // First try to get information from FontInfo.Signature to know whether font has this character.
            char ch = run.Text[0];
            // WORDSNET-17046 Use appropriate font name depending on original value of the "ComplexScript" flag.
            string effectiveFontName = isComplexScript ? run.Font.NameBi : run.Font.Name;
            FontInfo info = run.Document.FontInfos[effectiveFontName];

            bool setFont;
            if ((info != null) && (!info.GetUnicodeRanges().Equals(FontUnicodeRanges.Empty)))
            {
                // Document has font info, check if font contains glyph, if doesn't contain, change font to default
                setFont = !info.GetSubstitutionInfo().UnicodeRanges.IsCharInFunctionalRange(ch);
            }
            else
            {
                // Document doesn't contain font info, may be user want to override standard font, for example set Arial Unicode MS
                // In this case we need to load font and check if it contains glyph.

                // This case is rare and should not affect performance in common case.
                TTFont curFont = run.Document.FontProvider.GetTTFont(effectiveFontName, System.Drawing.FontStyle.Regular);

                // If unable to load selected font info or font doesn't contain glyph, change it to default for language.
                setFont = (curFont == null) || (!curFont.Glyphs.ContainsCharCode(ch, true));
            }

            if (setFont)
                run.Font.Name = charInfo.FontName;
        }

        /// <summary>
        /// Updates formatting for complex script chars.
        /// </summary>
        private static void UpdateComplexScriptFormatting(Run run)
        {      
            int size = (int)InlineHelper.FetchAttr(run, FontAttr.Size);
            int sizeBi = (int)InlineHelper.FetchAttr(run, FontAttr.SizeBi);
            if (size != sizeBi)
                run.RunPr[FontAttr.SizeBi] = size;

            bool isBold = InlineHelper.GetBool(run, FontAttr.Bold);
            if (isBold != InlineHelper.GetBool(run, FontAttr.BoldBi))
                run.RunPr[FontAttr.BoldBi] = AttrBoolEx.FromBool(isBold);

            bool isItalic = InlineHelper.GetBool(run, FontAttr.Italic);
            if (isItalic != InlineHelper.GetBool(run, FontAttr.ItalicBi))
                run.RunPr[FontAttr.ItalicBi] = AttrBoolEx.FromBool(isItalic);
        }

        /// <summary>
        /// Determine language info by character.
        /// </summary>
        private static CharacterInfo GetCharacterInfo(char ch)
        {
            foreach (CharacterInfo charInfo in gOverrideLocales)
            {
                if ((charInfo.FromCharCode <= ch) && (ch <= charInfo.ToCharCode))
                    return charInfo;
            }

            return null;
        }

        /// <summary>
        /// Tries determine character info by surrogate symbol.
        /// </summary>
        /// <returns>Character info or null if the character isn't a surrogate symbol.</returns>
        private static CharacterInfo TryGetSurrogateCharacterInfo(Run run, int indexChar)
        {
            string text = run.Text;

            if ((indexChar + 1 < run.GetTextLength()) && char.IsHighSurrogate(text[indexChar]) &&
                char.IsLowSurrogate(text[indexChar + 1]))
            {
                int charCode = UnicodeUtil.ConvertToUtf32(text[indexChar], text[indexChar + 1]);
                return GetSurrogateCharacterInfo(charCode);
            }

            return null;
        }

        /// <summary>
        /// Determine character info by surrogate symbol.
        /// </summary>
        private static CharacterInfo GetSurrogateCharacterInfo(int charCode)
        {
            foreach (CharacterInfo charInfo in gOverrideEmojis)
            {
                if ((charInfo.FromCharCode <= charCode) && (charCode <= charInfo.ToCharCode))
                    return charInfo;
            }

            return null;
        }

        /// <summary>
        /// List of languages to override in document locale and fonts.
        /// </summary>
        private static readonly List<CharacterInfo> gOverrideLocales = GetOverrideLocales();

        /// <summary>
        /// List of Unicode emoji code point ranges to override in document fonts.
        /// </summary>
        private static readonly List<CharacterInfo> gOverrideEmojis = GetOverrideEmojes();

        private static List<CharacterInfo> GetOverrideLocales()
        {
            return new List<CharacterInfo>
            {
                // For these languages MS Word set locale, complex script and font
                new CharacterInfo(0x0D80, 0x0DFF, Language.Sinhalese, "Iskoola Pota", true),
                new CharacterInfo(0x0E00, 0x0E7F, Language.ThaiThailand, "Angsana New", true),
                new CharacterInfo(0x1800, 0x18FF, Language.MongolianMongolian, "Mongolian Baiti", true),

                // For these languages MS Word sets only font but not locale or complex script flag
                new CharacterInfo(0x1200, 0x137F, Language.LanguageNotSet, "Nyala", false),
                new CharacterInfo(0x16A0, 0x16FF, Language.LanguageNotSet, "Segoe UI Historic", false), // Runic
                new CharacterInfo(0x1780, 0x17FF, Language.LanguageNotSet, "DaunPenh", false),

                // WORDSNET-16856 It's a range with a single symbol. Mimic MSW for Mac. 
                new CharacterInfo(0x2060, 0x2060, Language.LanguageNotSet, "Segoe UI Symbol", false),
            };
        }

        private static List<CharacterInfo> GetOverrideEmojes()
        {
            return new List<CharacterInfo>
            {
                // Emoticons.
                new CharacterInfo(0x1F600, 0x1F64F, Language.LanguageNotSet, "Segoe UI Emoji", false),
            };
        }

        /// <summary>
        /// Stores information about document locale and fonts.
        /// </summary>
        private class CharacterInfo
        {
            /// <summary>
            /// Constructor, initialize internal variables.
            /// </summary>
            /// <param name="fromCharCode">Start character of Unicode block.</param>
            /// <param name="toCharCode">End character of Unicode block.</param>
            /// <param name="language">Language should be forced for text. If equal to Language.LanguageNotSet then it is ignored.</param>
            /// <param name="fontName">Font name to set in document.</param>
            /// <param name="setComplexScript">Indicates that ComplexScript should be forced for text.</param>
            public  CharacterInfo(int fromCharCode, int toCharCode, Language language, string fontName, bool setComplexScript)
            {
                mFromCharCode = fromCharCode;
                mToCharCode = toCharCode;
                mLanguage = language;
                mFontName = fontName;
                mSetComplexScript = setComplexScript;
            }

            /// <summary>
            /// Start character of Unicode block.
            /// </summary>
            public int FromCharCode { get { return mFromCharCode; } }

            /// <summary>
            /// End character of Unicode block.
            /// </summary>
            public int ToCharCode { get { return mToCharCode; } }

            /// <summary>
            /// Locale to set in document.
            /// </summary>
            public Language Language { get { return mLanguage; } }

            /// <summary>
            /// Font name to set in document.
            /// </summary>
            public string FontName { get { return mFontName; } }

            /// <summary>
            /// Indicates that complex script should be set for flag.
            /// </summary>
            public bool SetComplexScript { get { return mSetComplexScript; } }
                 
            private readonly int mToCharCode;           
            private readonly string mFontName;
            private readonly int mFromCharCode;
            private readonly Language mLanguage;
            private readonly bool mSetComplexScript;
        }
    }
}
