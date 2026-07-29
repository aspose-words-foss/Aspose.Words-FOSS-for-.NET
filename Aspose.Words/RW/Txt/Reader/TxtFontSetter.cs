// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2016 by Alexander Zhiltsov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Sets fonts to runs in a manner like MS Word does on loading document from a text file.
    /// Font is changed from default (Courier New) for some East Asian languages to make text more readable.
    /// </summary>
    /// <remarks>
    /// Result is not exactly identical since sometimes MS Word sets font in a bit strange way: for two nearest
    /// chars in a same Unicode block it often assigns different fonts.
    /// </remarks>
    internal class TxtFontSetter : DocumentVisitor
    {
        /// <summary>
        /// Sets font to the passed run depending on language of its text. If necessary,
        /// the run is split to several ones.
        /// </summary>
        public override VisitorAction VisitRun(Run run)
        {
            string text = run.Text;
            if (!StringUtil.HasChars(text))
                return VisitorAction.Continue;

            string fontName = GetCharFont(text[0]);
            int textStart = 0;
            for (int i = 1; i < text.Length; i++)
            {
                string currentFontName = GetCharFont(text[i]);
                if (fontName != currentFontName)
                {
                    SplitRun(run, i - textStart, fontName);

                    fontName = currentFontName;
                    textStart = i;
                }
            }

            run.Font.Name = fontName;

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Gets preferred font for the specified char.
        /// </summary>
        internal static string GetCharFont(char ch)
        {
            const string defaultChineseFont = "Microsoft JhengHei";
            const string defaultKoreanFont = "Malgun Gothic";
            const string otherAsianFont = "MS Gothic";
            const string yaHeiFont = "Microsoft YaHei";
            const string yiBaitiFont = "Microsoft Yi Baiti";

            // Skip all checks if European language.
            if (ch < 0x1100)
                return DefaultFont;

            if (((ch >= 0x4E00) && (ch <= 0x9FFF)) || //CJK Unified Ideographs. Word uses Microsoft JhengHei and MS Gothic
                ((ch >= 0x2E80) && (ch <= 0x2FEF)) || //CJK Radicals Supplement, Kangxi Radicals
                ((ch >= 0x3100) && (ch <= 0x312F)) || //Bopomofo
                ((ch >= 0x3190) && (ch <= 0x31EF)) || //Kanbun, Bopomofo Extended, CJK Strokes,
                ((ch >= 0xFE30) && (ch <= 0xFE4F)))   //CJK Compatibility Forms
                return defaultChineseFont;

            if (((ch >= 0x3000) && (ch <= 0x30FF)) || //CJK Symbols and Punctuation, Hiragana, Katakana
                ((ch >= 0x31F0) && (ch <= 0x31FF)) || //Katakana Phonetic Extensions
                ((ch >= 0x3220) && (ch <= 0x325F)) || //Enclosed CJK Letters and Months
                ((ch >= 0x3280) && (ch <= 0x32FF)) || //Enclosed CJK Letters and Months
                ((ch >= 0x3300) && (ch <= 0x33FF)) || //CJK Compatibility
                ((ch >= 0xFA10) && (ch <= 0xFAFF)))   //CJK Compatibility Ideographs
                return otherAsianFont;

            // Hangul
            if (((ch >= 0x1100) && (ch <= 0x11FF)) || //Hangul Jamo
                ((ch >= 0x3130) && (ch <= 0x318F)) || //Hangul Compatibility Jamo
                ((ch >= 0xA960) && (ch <= 0xA97F)) || //Hangul Jamo Extended-A
                ((ch >= 0xAC00) && (ch <= 0xD7FF)))   //Hangul Syllables, Hangul Jamo Extended-B
                return defaultKoreanFont;

            // CJK
            if (((ch >= 0x3200) && (ch <= 0x321F)) || //Enclosed CJK Letters and Months
                ((ch >= 0x3260) && (ch <= 0x327F)) || //Enclosed CJK Letters and Months
                ((ch >= 0x3400) && (ch <= 0x4DBF)) || //CJK Unified Ideographs Extension A
                ((ch >= 0xF900) && (ch <= 0xFA0F)))   //CJK Compatibility Ideographs
                return defaultKoreanFont;

            if ((ch >= 0x2FF0) && (ch <= 0x2FFF))     //Ideographic Description Characters
                return yaHeiFont;

            if ((ch >= 0xA000) && (ch <= 0xA4CF))     //Yi Syllables, Yi Radicals
                return yiBaitiFont;

            return DefaultFont;
        }

        /// <summary>
        /// Splits the specified base run.
        /// </summary>
        /// <param name="baseRun">A run to split.</param>
        /// <param name="position">Position in text of the run, at which to split.</param>
        /// <param name="fontName">Font name for the left run.</param>
        private static void SplitRun(Run baseRun, int position, string fontName)
        {
            Run run = new Run(baseRun.Document, baseRun.Text.Substring(0, position), baseRun.RunPr.Clone());
            run.Font.Name = fontName;
            baseRun.InsertPrevious(run);

            baseRun.Text = baseRun.Text.Substring(position);
        }

        internal const string DefaultFont = "Courier New";
    }
}
