// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2014 by Victor Chebotok

using System.Collections.Generic;
using System.Text;
using Aspose.Bidi;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Rearranges text of adjacent inline HTML elements using the Unicode bidirectional algorithm.
    /// </summary>
    /// <remarks>
    /// In HTML, text of inline elements inside a box is a subject to reordering with the bidirectional algorithm.
    /// This class is used to accumulate text of adjacent inline elements, reorder it and write runs of reordered text
    /// with corresponding font properties into document builder.
    /// </remarks>
    internal class HtmlBidiTextArranger
    {
        internal HtmlBidiTextArranger(
            DocumentBuilder builder,
            bool textIsVisuallyOrdered,
            bool joinRuns)
        {
            Debug.Assert(builder != null);
            mBuilder = builder;
            mTextIsVisuallyOrdered = textIsVisuallyOrdered;
            mJoinRuns = joinRuns;
        }

        /// <summary>
        /// Appends a fragment of text. Each fragment has its own font properties and bidi levels.
        /// </summary>
        internal void Append(
            string text,
            RunPr runPr,
            BidiLevelList activeBidiLevels,
            bool dontCollapseLastSpace,
            Bookmark bookmark)
        {
            Debug.Assert(StringUtil.HasChars(text));

            if ((mParagraph != null) && (mParagraph != mBuilder.CurrentParagraph))
            {
                // We changed the paragraph and forgot to write pending text first. It's a severe logical error,
                // and we should never get here. The pending text will be lost.
                Debug.Assert(false);
                mRuns.Clear();
            }

            // Try to join the new run with the last run in the buffer, if any exists and if formatting of the two runs
            // is equal. Note that the calling code may disable joining of runs.
            bool joined = false;
            if (mJoinRuns && (mRuns.Count > 0))
            {
                PendingRun lastRun = mRuns[mRuns.Count - 1];
                if (!lastRun.IsDirectionMark &&
                    lastRun.RunPr.Equals(runPr) &&
                    lastRun.Run.Levels.Equals(activeBidiLevels) &&
                    (lastRun.Bookmark == bookmark))
                {
                    mRuns[mRuns.Count - 1] = new PendingRun(
                        new BidiSourceRun(lastRun.Run.Text + text, activeBidiLevels),
                        runPr,
                        bookmark);
                    joined = true;
                }
            }

            if (!joined)
            {
                mRuns.Add(new PendingRun(new BidiSourceRun(text, activeBidiLevels), runPr, bookmark));
            }

            mDontCollapseWhitespaceAfterText = dontCollapseLastSpace;
            mParagraph = mBuilder.CurrentParagraph;
        }

        /// <summary>
        /// Appends a proper direction mark (LRM or RLM) according to the bidi levels.
        /// </summary>
        /// <remarks>
        /// Appended marks are used only to calculate bidi embedding levels and aren't written to the rearranged text.
        /// That is why we don't have to specify run properties for marks.
        /// </remarks>
        internal void AppendDirectionMark(BidiLevelList activeBidiLevels)
        {
            Debug.Assert(activeBidiLevels.Length > 0);
            if (activeBidiLevels.Length <= 0)
            {
                return;
            }

            string mark = (activeBidiLevels[activeBidiLevels.Length - 1] == BidiLevel.EmbedLtr)
                ? LeftToRightMark
                : RightToLeftMark;
            Append(mark, null, activeBidiLevels, false, null);
        }

        /// <summary>
        /// Rearranges text accumulated since the last write and writes it to the document builder.
        /// </summary>
        internal void RearrangeAndWriteText()
        {
            // If there is no pending text, just return.
            if (IsEmpty)
            {
                Debug.Assert(mParagraph == null);
                return;
            }

            // The pending text must belong to the current paragraph.
            if (mParagraph == mBuilder.CurrentParagraph)
            {
                // Copy source runs to array.
                BidiSourceRun[] sourceRuns = new BidiSourceRun[mRuns.Count];
                for (int i = 0; i < mRuns.Count; i++)
                {
                    PendingRun textRun = mRuns[i];
                    sourceRuns[i] = textRun.Run;
                }

                // Reorder source runs using the unicode bidirectional algorithm.
                BidiRun[] bidiRuns = BidiAlgorithm.Apply(
                    sourceRuns,
                    mBuilder.ParagraphFormat.Bidi,
                    mTextIsVisuallyOrdered,
                    true);

                // WORDSNET-13311 All text after STRONG tag contained in PRE tag becomes bold in output documents.
                RunPr oldRunPr = mBuilder.GetRunPrCopy();
                // Write reordered runs with corresponding font properties.
                foreach (BidiRun bidiRun in bidiRuns)
                {
                    PendingRun sourceTextRun = mRuns[bidiRun.SourceRunIndex];

                    // Mark runs have no textual representation in the source document. They are for internal use only
                    // and their content shouldn't be written to the result.
                    if (sourceTextRun.IsDirectionMark)
                    {
                        continue;
                    }

                    mBuilder.SetFont(sourceTextRun.RunPr, true);
                    if (mBuilder.Font.Bidi != bidiRun.Rtl)
                    {
                        mBuilder.Font.Bidi = bidiRun.Rtl;
                    }

                    // WORDSNET-16568 Start a bookmark for inline HTML element with "id" attribute.
                    if (sourceTextRun.Bookmark != null)
                        mBuilder.InsertNode(sourceTextRun.Bookmark.BookmarkStart);

                    // WORDSNET-18872 Split bidi text that contains intermixed HebrewIsrael and ArabicSaudiArabia parts. 
                    string[] textSplitByLocaleId = mBuilder.Font.Bidi
                        ? SplitBidiTextByLocaleId(bidiRun.Text)
                        : new string[] { bidiRun.Text };

                    WriteSplitText(textSplitByLocaleId, bidiRun);

                    // WORDSNET-16568 End a bookmark for inline HTML element with "id" attribute.
                    if (sourceTextRun.Bookmark != null)
                    {
                        mBuilder.InsertNode(sourceTextRun.Bookmark.BookmarkEnd);
                    }
                }
                mBuilder.SetFont(oldRunPr, false);
            }
            else
            {
                // The pending text does not belong to the current paragraph. We changed the paragraph 
                // and forgot to write pending text first. It is a severe logical error, and we should never get here.
                // The pending text will be lost.
                Debug.Assert(false);
            }

            mRuns.Clear();
            mParagraph = null;
        }

        private void WriteSplitText(string[] textSplitByLocaleId, BidiRun bidiRun)
        {
            int originalBidiLocaleId = mBuilder.Font.LocaleIdBi;
            foreach (string textPart in textSplitByLocaleId)
            {
                // WORDSNET-13787 We have to set bidi locale to 'he-IL' on RTL runs containing numbers with any of
                // the following number separator characters: '+', '-', and '/'. Otherwise, these separators will be treated
                // as neutral characters in MS Word and parts of the number will be reordered.
                if (bidiRun.RequiresHebrewLocaleBi)
                {
                    SetLocaleIdBi(mBuilder.Font, (int)Language.HebrewIsrael);
                }
                else
                {
                    Language autoDetectedBidiLocaleId = mBuilder.Font.Bidi
                        ? DetectBidiLocaleId(textPart)
                        : Language.LanguageNotSet;

                    // WORDSNET-18872 Always auto-detect and set bidi locale ID of bidi text.
                    if (autoDetectedBidiLocaleId != Language.LanguageNotSet)
                    {
                        SetLocaleIdBi(mBuilder.Font, (int)autoDetectedBidiLocaleId);
                    }
                    // Don't change bidi locale ID of other text.
                    else
                    {
                        SetLocaleIdBi(mBuilder.Font, originalBidiLocaleId);
                    }
                }

                Run newRunNode = new Run(mBuilder.Document, textPart, mBuilder.GetRunPrCopy());
                mBuilder.InsertNode(newRunNode);
            }
        }

        private static void SetLocaleIdBi(Font font, int localeId)
        {
            if (font.LocaleIdBi != localeId)
                font.LocaleIdBi = localeId;
        }

        internal bool IsEmpty
        {
            get { return mRuns.Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether leading whitespace must be trimmed after the text that has been already
        /// written into this buffer.
        /// </summary>
        internal bool CollapseWhitespaceAfterText()
        {
            if (mDontCollapseWhitespaceAfterText)
            {
                return false;
            }

            for (int runIndex = mRuns.Count - 1; runIndex >= 0; runIndex--)
            {
                PendingRun run = mRuns[runIndex];

                // Mark runs are created from HTML markup, not from text, and so they shouldn't affect methods that
                // work with the text. That's why we skip mark runs here.
                if (run.IsDirectionMark)
                {
                    continue;
                }

                string text = run.Run.Text;
                if (text.Length == 0)
                {
                    continue;
                }

                char lastChar = text[text.Length - 1];
                return (lastChar == ' ') ||
                    (lastChar == ControlChar.LineBreakChar) ||
                    (lastChar == ControlChar.PageBreakChar) ||
                    (lastChar == ControlChar.ColumnBreakChar);
            }

            return false;
        }

        /// <summary>
        /// Detects 
        /// </summary>
        private static Language DetectBidiLocaleId(string bidiText)
        {
            if (bidiText.Length > 0)
            {
                char firstChar = bidiText[0];
                BidiCharacterType bidiType = GetBidiCharacterType(firstChar);
                if (bidiType == BidiCharacterType.R)
                {
                    return Language.HebrewIsrael;
                }
                // In some cases Arabic characters may be mixed with Nonspacing Mark.
                // Such strings should have ArabicSaudiArabia language.
                foreach (char character in bidiText)
                {
                    bidiType = GetBidiCharacterType(character);
                    if (bidiType == BidiCharacterType.AL)
                    {
                        return Language.ArabicSaudiArabia;
                    }
                }
            }
            return Language.LanguageNotSet;
        }

        /// <summary>
        /// Splits bidi (RTL) text into parts having same bidi locale ID (HebrewIsrael or ArabicSaudiArabia).
        /// </summary>
        private static string[] SplitBidiTextByLocaleId(string bidiText)
        {
            bool isMixedLocaleBidiText = HasBothArabicAndHebrewCharacters(bidiText);
            if (!isMixedLocaleBidiText)
            {
                return new string[] { bidiText };
            }

            List<string> splitBidiText = new List<string>();

            BidiCharacterType currentRangeCharType = GetBidiCharacterType(bidiText[0]);
            StringBuilder rangeText = new StringBuilder(bidiText.Length);

            // It seems Word separates Hebrew to one range and other character types to another range.
            // For example: the following sequence "עבריתالسَّلَامُ" contains 
            // HebrewIsrael, ArabicSaudiArabia and Nonspacing Mark character types.
            foreach (char c in bidiText)
            {
                BidiCharacterType currentCharType = GetBidiCharacterType(c);
                if ((currentRangeCharType != currentCharType) &&
                    ((currentRangeCharType == BidiCharacterType.R) || (currentCharType == BidiCharacterType.R)))
                {
                    splitBidiText.Add(rangeText.ToString());
                    rangeText.Length = 0;
                    currentRangeCharType = currentCharType;
                }
                rangeText.Append(c);
            }
            if (rangeText.Length > 0)
            {
                splitBidiText.Add(rangeText.ToString());
            }

            return splitBidiText.ToArray();
        }

        /// <summary>
        /// Determines whether the specified text contains both HebrewIsrael and ArabicSaudiArabia content at the same time.
        /// </summary>
        private static bool HasBothArabicAndHebrewCharacters(string text)
        {
            bool hasRtlHebrew = false;
            bool hasRtlArabic = false;
            foreach (char c in text)
            {
                BidiCharacterType charType = GetBidiCharacterType(c);
                if (charType == BidiCharacterType.R)
                {
                    hasRtlHebrew = true;
                }
                else if (charType == BidiCharacterType.AL)
                {
                    hasRtlArabic = true;
                }
                if (hasRtlHebrew && hasRtlArabic)
                {
                    return true;
                }
            }
            return false;
        }

        private static BidiCharacterType GetBidiCharacterType(char c)
        {
            return UnicodeCharacterDataResolver.GetBidiCharacterType(c);
        }

        /// <summary>
        /// Represents a run of text with corresponding font properties.
        /// </summary>
        private class PendingRun
        {
            internal PendingRun(BidiSourceRun run, RunPr runPr, Bookmark bookmark)
            {
                Run = run;
                RunPr = runPr;
                Bookmark = bookmark;
            }

            internal BidiSourceRun Run { get; }

            internal RunPr RunPr { get; }

            internal bool IsDirectionMark
            {
                get { return (Run.Text == LeftToRightMark) || (Run.Text == RightToLeftMark); }
            }

            internal Bookmark Bookmark { get; }
        }

        private const string LeftToRightMark = "\u200e";
        private const string RightToLeftMark = "\u200f";

        /// <summary>
        /// The document builder the text is to be written to.
        /// </summary>
        private readonly DocumentBuilder mBuilder;

        /// <summary>
        /// Indicates that the text is in visual order (right-to-left text is reversed), 
        /// as apposed to much more common logical order.
        /// </summary>
        private readonly bool mTextIsVisuallyOrdered;

        /// <summary>
        /// Indicates whether adjacent runs that have equal formatting are allowed to join into one run.
        /// </summary>
        private readonly bool mJoinRuns;

        /// <summary>
        /// Runs of text to be reordered with the bidirectional algorithm. Element type is <see cref="PendingRun"/>.
        /// </summary>
        private readonly List<PendingRun> mRuns = new List<PendingRun>();

        /// <summary>
        /// The paragraph the text belongs to.
        /// </summary>
        private Paragraph mParagraph;

        /// <summary>
        /// Indicates that text ends with a space character that was imported from a non-breaking space
        /// and thus cannot be collapsed with spaces after it.
        /// </summary>
        private bool mDontCollapseWhitespaceAfterText;
    }
}
