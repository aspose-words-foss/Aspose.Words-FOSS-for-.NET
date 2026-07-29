// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Collections;

namespace Aspose.Bidi
{
    public class BidiParagraph
    {
        // 3.3.1.P1 - Split the text into separate paragraphs.
        // A paragraph separator is kept with the previous paragraph.
        // Within each paragraph, apply all the other rules of this algorithm.
        // Paragraph embedding levels are defined by P2 and P3 rules.
        public static IList<BidiParagraph> SplitStringToParagraphs(string logicalString)
        {
            return SplitStringToParagraphs(logicalString, null, null);
        }

        // 3.3.1.P1 - Split the text into separate paragraphs.
        // A paragraph separator is kept with the previous paragraph.
        // Within each paragraph, apply all the other rules of this algorithm.
        // Paragraph embedding levels are defined by P2 and P3 rules.
        public static IList<BidiParagraph> SplitStringToParagraphs(
            string logicalString,
            IBidiParagraphLevelOverride embeddingLevelOverride)
        {
            return SplitStringToParagraphs(logicalString, embeddingLevelOverride, null);
        }

        // 3.3.1.P1 - Split the text into separate paragraphs.
        // A paragraph separator is kept with the previous paragraph.
        // Within each paragraph, apply all the other rules of this algorithm.
        // Paragraph embedding levels are defined by IBidiParagraphLevelOverride
        // implementation. Additional paragraph text modification is performed by
        // IBidiParagraphCharDataModifier implementation.
        public static IList<BidiParagraph> SplitStringToParagraphs(
            string logicalString,
            IBidiParagraphLevelOverride embeddingLevelOverride,
            IBidiParagraphCharDataModifier calcRunsCharDataModifier)
        {
            IList<BidiParagraph> ret = new List<BidiParagraph>();
            StringBuilder sb = new StringBuilder();
            string separator = "";
            int i = 0;
            while (i < logicalString.Length)
            {
                char c = logicalString[i];
                BidiCharacterType cType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
                if (cType == BidiCharacterType.B)
                {
                    separator += c;
                    //here we check /r+/n combination of paragraph separator
                    if ((i + 1 < logicalString.Length) &&
                        (logicalString[i] == 0x000D) &&
                        (logicalString[i + 1] == 0x000A))
                        separator += logicalString[++i];
                }
                else
                {
                    sb.Append(c);
                }
                if ((separator != "") || (i + 1 == logicalString.Length))
                {
                    string paragraphText = sb.ToString();
                    int embeddingLevel = (embeddingLevelOverride != null)
                        ? embeddingLevelOverride.GetEmbeddingLevel(ret, paragraphText)
                        : -1;

                    BidiParagraph p = new BidiParagraph(paragraphText, embeddingLevel, calcRunsCharDataModifier);
                    p.ParagraphSeparator = separator;
                    ret.Add(p);
                    sb.Length = 0;
                    separator = "";
                }

                ++i;
            }

            return ret;
        }

        /// <summary>
        /// Returns a value indicating whether the specified string contains any paragraph separator characters.
        /// </summary>
        public static bool IsMultiParagraphString(string logicalString)
        {
            foreach (char c in logicalString)
            {
                if (UnicodeCharacterDataResolver.GetBidiCharacterType(c) == BidiCharacterType.B)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Original inputed text
        /// </summary>
        private readonly string mOriginalText;

        /// <summary>
        /// Text for modification (working copy of mOriginalText)
        /// </summary>
        private string mText;

        /// <summary>
        /// Text is ready for dispaly
        /// </summary>
        private string mBidiText;
        private string mParagraphSeparator = "";

        private byte mEmbeddingLevel;
        private readonly int mEmbeddingLevelToSet;
        private readonly IBidiParagraphCharDataModifier mCalcRunsCharDataModifier;
        private BidiCharData[] mTextData;

        private bool mHasArabic;
        private bool mHasNSMs;
        private IList<BidiRun> mBidiRuns;

        /// <summary>
        /// Ctor to use with a guaranteed single-paragraph text. Any checks for the input text are not performed.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="embeddingLevel"></param>
        /// <param name="calcRunsCharDataModifier"></param>
        public BidiParagraph(string text, int embeddingLevel, IBidiParagraphCharDataModifier calcRunsCharDataModifier)
        {
            mOriginalText = text;
            mText = text;
            mEmbeddingLevelToSet = embeddingLevel;
            mCalcRunsCharDataModifier = calcRunsCharDataModifier;
        }


        public string Text
        {
            get { return mOriginalText; }
        }

        public string ParagraphSeparator
        {
            get { return mParagraphSeparator; }
            set { mParagraphSeparator = value; }
        }

        public string BidiText
        {
            get
            {
                if (!StringUtil.HasChars(mBidiText))
                {
                    ProcessTextByUnicodeBidiAlgorithm(false, true);
                    RemoveBidiMarkers();
                }
                string ret = mBidiText;
                if (mParagraphSeparator != "")
                    ret += mParagraphSeparator;
                return ret;
            }
        }

        /// <summary>
        /// Return array of BidiRun, sptited text by embedding levels.
        /// </summary>
        public ICollection<BidiRun> BidiRuns
        {
            get
            {
                if (mBidiRuns == null)
                    ProcessTextByUnicodeBidiAlgorithm(false, false);
                return mBidiRuns;
            }
        }

        public byte EmbeddingLevel
        {
            get { return mEmbeddingLevel; }
        }

        /// <summary>
        /// Returns an array of <see cref="BidiRun"/>. The runs are ordered the way MS Word expects them to be typed.
        /// </summary>
        /// <param name="isRtlParagraph">
        /// <c>true</c>, if the runs are part of an RTL paragraph.
        /// <c>false</c>, if the runs are part of an LTR paragraph.
        /// </param>
        /// <returns>
        /// Collection of <see cref="BidiRun"/> in MS Word order.
        /// </returns>
        public IList<BidiRun> GetBidiRunsInMsWordOrder(bool isRtlParagraph)
        {
            // Use canonical normalization of the text, because this method is used to prepare the text for output to a document (not to a screen),
            // and we don't want to lose some special characters. For example, when compatibility normalization is used, the "no-break space" character
            // is replaced with usual space character.
            // As a result of this call the fields mBidiText and mBidiRuns are filled.
            ProcessTextByUnicodeBidiAlgorithm(true, false);

            // The Bidi Algorithm always marks runs with numbers as LTR. In MS Word, however, such runs must be embedded into
            // an adjacent non-numeric run and must have its direction. The following three-step algorithm finds out numeric
            // runs and embeds them into appropriate adjacent non-numeric runs.
            MarkNumbers();
            AdjustLevelOfNumbers();
            JoinSimilarRuns();

            // Reverse order of runs in cases when
            // an LTR text is placed inside an RTL paragraph
            // or and RTL text is placed inside an LTR paragraph. 
            if (mBidiRuns.Count > 1)
            {
                int minLevel = mBidiRuns[0].Level;
                for (int i = 1; i < mBidiRuns.Count; i++)
                {
                    minLevel = Math.Min(minLevel, mBidiRuns[i].Level);
                }

                bool textIsRtl = minLevel % 2 == 1;
                if (textIsRtl != isRtlParagraph)
                {
                    ReverseRuns();
                }
            }

            IList<BidiRun> resultRuns = mBidiRuns;

            // Clear cached values. This will force next call to BidiRuns or BidiText property to reprocess text 
            // using compatibility (not canonical) normalization.
            mBidiText = null;
            mBidiRuns = null;

            return resultRuns;
        }

        /// <summary>
        /// Finds out runs containing numbers and marks them for further processing.
        /// </summary>
        private void MarkNumbers()
        {
            Debug.Assert(mBidiRuns != null);

            foreach (BidiRun run in mBidiRuns)
            {
                bool hasDigits = false;
                bool hasLetters = false;
                bool hasHebrewNumberSeparators = false;
                for (int j = 0; j < run.Text.Length; j++)
                {
                    char c = run.Text[j];
                    BidiCharacterType charType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);

                    if ((charType == BidiCharacterType.EN) || (charType == BidiCharacterType.AN))
                    {
                        hasDigits = true;
                    }
                    // When common number separators are inside a run with a number, they are treated as a part of that number,
                    // not as separate letters.
                    //
                    // The following characters are number separators from the Unicode point of view. MS Word, however, treat
                    // them as such only if run's bidi locale is set to 'he-IL' (see ISO 29500, Annex I, Section I.2, 
                    // 'lang' element description).
                    else if ((c == '+') || (c == '-') || (c == '/'))
                    {
                        hasHebrewNumberSeparators = true;
                    }
                    // Althought Unicode defines more number separator characters (CS and ES characters types),
                    // some of them are always treated as neutral direction characters (ON) in MS Word.
                    else if ((c != ',') && (c != '.') && (c != ':') && (c != '\u060C'))
                    {
                        // Here letters are all characters except digits and number separators (all characters that cannot be
                        // a part of a number).
                        hasLetters = true;
                    }
                }

                run.IsNumber = hasDigits && !hasLetters;
                run.HasHebrewNumberSeparators = hasHebrewNumberSeparators;
            }
        }

        /// <summary>
        /// For each numeric run, finds an corresponding non-numeric run that the number should be embedded into. Adjust levels
        /// of numeric runs to match levels of corresponding non-numeric runs.
        /// </summary>
        private void AdjustLevelOfNumbers()
        {
            Debug.Assert(mBidiRuns != null);

            // Override embedding levels of runs containing numbers.
            for (int i = 0; i < mBidiRuns.Count; i++)
            {
                BidiRun run = mBidiRuns[i];
                if (!run.IsNumber)
                    continue;

                // Calculate adjacent run levels.
                int leadingLevel = (i > 0)
                    ? Math.Min(mBidiRuns[i - 1].Level, run.Level)
                    : -1;
                int trailingLevel = ((i + 1) < mBidiRuns.Count)
                    ? Math.Min(mBidiRuns[i + 1].Level, run.Level)
                    : -1;
                int adjustedLevel = Math.Max(leadingLevel, trailingLevel);

                // Correct the embedding level of the current run if it has changed.
                // If there are no adjacent runs, keep the number's embedding level unchanged.
                if ((adjustedLevel >= 0) && (adjustedLevel != run.Level))
                {
                    BidiRun adjustedRun = new BidiRun(run.Text, adjustedLevel);
                    adjustedRun.IsNumber = run.IsNumber;
                    adjustedRun.HasHebrewNumberSeparators = run.HasHebrewNumberSeparators;
                    mBidiRuns[i] = adjustedRun;
                }
            }
        }

        /// <summary>
        /// Joins adjacent runs having the same direction and other properties. Each range of joined runs is replaced with one
        /// run containing concatenated text.
        /// </summary>
        private void JoinSimilarRuns()
        {
            Debug.Assert(mBidiRuns != null);

            if (mBidiRuns.Count <= 1)
            {
                // Nothing to join.
                return;
            }

            IList<BidiRun> joinedRuns = new List<BidiRun>(mBidiRuns.Count);

            int rangeStart = 0;
            int rangeLength = 0;
            int rangeLevel = mBidiRuns[0].Level;
            bool rangeRequiresHebrewLocaleBi = mBidiRuns[0].RequiresHebrewLocaleBi;

            // Here we scan one run past the run count. An imaginary end-of-list run bounds the last run range and allows
            // to process it inside the main loop thus reducing the code duplication.
            for (int i = 0; i <= mBidiRuns.Count; i++)
            {
                int runLevel = (i < mBidiRuns.Count)
                    ? mBidiRuns[i].Level
                    : -1;
                bool runRequiresHebrewLocaleBi = (i < mBidiRuns.Count) && mBidiRuns[i].RequiresHebrewLocaleBi;

                if ((runLevel != rangeLevel) || (runRequiresHebrewLocaleBi != rangeRequiresHebrewLocaleBi))
                {
                    if (rangeLength == 1)
                    {
                        joinedRuns.Add(mBidiRuns[rangeStart]);
                    }
                    else
                    {
                        StringBuilder joinedText = new StringBuilder();
                        for (int j = rangeStart; j < rangeStart + rangeLength; j++)
                        {
                            joinedText.Append(mBidiRuns[j].Text);
                        }
                        joinedRuns.Add(new BidiRun(joinedText.ToString(), rangeLevel));
                    }
                    rangeStart = i;
                    rangeLength = 0;
                    rangeLevel = runLevel;
                    rangeRequiresHebrewLocaleBi = runRequiresHebrewLocaleBi;
                }
                ++rangeLength;
            }

            mBidiRuns = joinedRuns;
        }

        /// <summary>
        /// Reverses the order of bidi runs preserving continuous ranges of runs having same direction.
        /// </summary>
        private void ReverseRuns()
        {
            if (mBidiRuns.Count <= 1)
            {
                // Nothing to reverse.
                return;
            }

            List<BidiRun> reversedRuns = new List<BidiRun>(mBidiRuns.Count);

            // Split runs into ranges. Store ranges in direct order and runs inside each range in reversed order.
            int rangeStart = 0;
            int rangeLength = 0;
            bool rangeIsRtl = mBidiRuns[0].Rtl;
            // Here we scan one run past the run count. An imaginary end-of-list run bounds the last run range and allows
            // to process it inside the main loop thus reducing the code duplication.
            for (int i = 0; i <= mBidiRuns.Count; i++)
            {
                bool runIsRtl = (i < mBidiRuns.Count)
                    ? mBidiRuns[i].Rtl
                    : !rangeIsRtl; // The imaginary end-of-list run must close the last range.
                if (runIsRtl != rangeIsRtl)
                {
                    // Copy runs in each range in reversed order. They will be rearranged in direct order
                    // during the final reverse.
                    for (int rangeIndex = rangeStart + rangeLength - 1; rangeIndex >= rangeStart; rangeIndex--)
                    {
                        reversedRuns.Add(mBidiRuns[rangeIndex]);
                    }

                    // Start a new range from the current run.
                    rangeStart = i;
                    rangeLength = 1;
                    rangeIsRtl = runIsRtl;
                }
                else
                {
                    ++rangeLength;
                }
            }

            // The final reverse. Reverses the order of ranges. Additionally, rearranges individual runs in each range back
            // to direct order.
            reversedRuns.Reverse();

            mBidiRuns = reversedRuns;
        }

        /// <summary>
        /// Processes the text of the paragraph by Unicode Bidirectional Algorithm.
        /// </summary>
        /// <param name="canonicalNormalization">
        /// <c>true</c> if canonical normalization should be used. 
        /// <c>false</c> if compatibility normalization should be used.
        /// </param>
        /// <param name="convertToRenderingOrder">
        /// <c>true</c> if the text should be reordered from logical to rendering order.</param>
        /// <c>false</c> if no such reordering is requred.
        /// <remarks>
        /// Values of <see cref="mBidiText"/> and <see cref="mBidiRuns"/> are filled as a result of calling this method.
        /// </remarks>
        private void ProcessTextByUnicodeBidiAlgorithm(bool canonicalNormalization, bool convertToRenderingOrder)
        {
            NormalizeText(canonicalNormalization);
            RecalculateParagraphEmbeddingLevel();
            RecalculateCharactersEmbeddingLevels(convertToRenderingOrder);
        }

        private void RemoveBidiMarkers()
        {
            const string controlChars = "\u200F\u202B\u202E\u200E\u202A\u202D\u202C";

            StringBuilder sb = new StringBuilder(mBidiText);

            int i = 0;
            while (i < sb.Length)
            {
                if (controlChars.IndexOf(sb[i]) >= 0)
                {
                    sb.Remove(i, 1);
                }
                else
                {
                    ++i;
                }
            }

            mBidiText = sb.ToString();
        }

        // 3.3.1 The Paragraph Level
        // P2 - In each paragraph, find the first character of type L, AL, or R.
        // P3 - If a character is found in P2 and it is of type AL or R, then
        // set the paragraph embedding level to one; otherwise, set it to zero.
        private void RecalculateParagraphEmbeddingLevel()
        {
            #region rule HL1
            // HL1.    Override P3, and set the paragraph embedding level explicitly.
            // A higher-level protocol may set any paragraph level. 
            // This can be done on the basis of the context, such as on a table cell, paragraph, document, or system level. 
            // (P2 may be skipped if P3 is overridden). 
            // Note that this does not allow a higher-level protocol to override the limit specified in BD2.
            // A higher-level protocol may apply rules equivalent to P2 and P3 
            // but default to level 1 (RTL) rather than 0 (LTR) to match overall RTL context.
            // A higher-level protocol may use an entirely different algorithm that heuristically auto-detects 
            // the paragraph embedding level based on the paragraph text and its context. 
            // For example, it could base it on whether there are more RTL characters in the text than LTR. 
            // As another example, when the paragraph contains no strong characters, 
            // its direction could be determined by the levels of the paragraphs before and after.
            if (mEmbeddingLevelToSet != -1)
            {
                mEmbeddingLevel = (byte)mEmbeddingLevelToSet;
                return;
            }
            #endregion

            for (int i = 0; i < mText.Length; i++)
            {
                BidiCharacterType cType = UnicodeCharacterDataResolver.GetBidiCharacterType(mText[i]);
                if (cType == BidiCharacterType.R || cType == BidiCharacterType.AL)
                {
                    mEmbeddingLevel = 1;
                    break;
                }
                else if (cType == BidiCharacterType.L)
                    break;
            }
        }

        /// <summary>
        /// Normalizes text of the paragraph.
        /// </summary>
        /// <param name="canonicalNormalization">
        /// <c>true</c> if the text should be normalized using NFC.
        /// <c>false</c> if the text should be normalized using NFKC.
        /// </param>
        /// <remarks>
        /// For description of normalization forms see http://unicode.org/reports/tr15/
        /// </remarks>
        private void NormalizeText(bool canonicalNormalization)
        {
            char[] chars = InternalDecompose(canonicalNormalization);
            mText = InternalCompose(chars);
        }

        // 3.3.2 Explicit Levels and Directions
        private void RecalculateCharactersEmbeddingLevels(bool forVisible)
        {
            // This method is implemented in such a way it handles the string in logical order,
            // rather than visual order, so it is easier to handle complex layouts. That is why
            // it is placed BEFORE ReorderString rather than AFTER it, as its number suggests.

            if ((mHasArabic) && (forVisible))
                mText = PerformArabicShaping(mText);

            mTextData = new BidiCharData[mText.Length];

            #region rules X1 - X9
            // X1
            byte embeddingLevel = EmbeddingLevel;
            DirectionalOverrideStatus dos = DirectionalOverrideStatus.Neutral;
            IntStack dosStack = new IntStack();
            ByteList elStack = new ByteList();
            for (int i = 0; i < mText.Length; ++i)
            {
                char c = mText[i];
                BidiCharData charData = new BidiCharData();
                charData.CharType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
                charData.Char = c;
                mTextData[i] = charData;

                #region rules X2 - X5
                // X2. With each RLE, compute the least greater odd embedding level.
                // X4. With each RLO, compute the least greater odd embedding level.
                if (c == BidiChars.RLE || c == BidiChars.RLO)
                {
                    charData.EmbLevel = embeddingLevel;
                    if (embeddingLevel < 60)
                    {
                        elStack.Add(embeddingLevel);
                        dosStack.Push((int)dos);

                        ++embeddingLevel;
                        embeddingLevel |= 1;

                        if (c == BidiChars.RLE)
                            dos = DirectionalOverrideStatus.Neutral;
                        else
                            dos = DirectionalOverrideStatus.RTL;
                    }
                }
                // X3. With each LRE, compute the least greater even embedding level.
                // X5. With each LRO, compute the least greater even embedding level.
                else if (c == BidiChars.LRE || c == BidiChars.LRO)
                {
                    charData.EmbLevel = embeddingLevel;
                    if (embeddingLevel < 59)
                    {
                        elStack.Add(embeddingLevel);
                        dosStack.Push((int)dos);

                        embeddingLevel |= 1;
                        ++embeddingLevel;

                        if (c == BidiChars.LRE)
                            dos = DirectionalOverrideStatus.Neutral;
                        else
                            dos = DirectionalOverrideStatus.LTR;
                    }
                }
                #endregion

                #region rule X6
                // X6. For all types besides RLE, LRE, RLO, LRO, and PDF: (...)
                // Added From Revision #23 
                // For all types besides BN, RLE, LRE, RLO, LRO, and PDF: (...)
                else if ((c != BidiChars.PDF) && (charData.CharType != BidiCharacterType.BN))
                {
                    // a. Set the level of the current character to the current embedding level.
                    charData.EmbLevel = embeddingLevel;

                    //b. Whenever the directional override status is not neutral,
                    //reset the current character type to the directional override status.
                    if (dos == DirectionalOverrideStatus.LTR)
                        charData.CharType = BidiCharacterType.L;
                    else if (dos == DirectionalOverrideStatus.RTL)
                        charData.CharType = BidiCharacterType.R;
                }
                #endregion

                #region rule X7
                //Terminating Embeddings and Overrides
                // X7. With each PDF, determine the matching embedding or override code.
                // If there was a valid matching code, restore (pop) the last remembered (pushed)
                // embedding level and directional override.
                else if (c == BidiChars.PDF)
                {
                    charData.EmbLevel = embeddingLevel;
                    if (elStack.Count > 0)
                    {
                        embeddingLevel = elStack[elStack.Count - 1];
                        elStack.RemoveAt(elStack.Count - 1);
                        dos = (DirectionalOverrideStatus)(dosStack.Pop());
                    }
                }
                #endregion

                // X8. All explicit directional embeddings and overrides are completely
                // terminated at the end of each paragraph. Paragraph separators are not
                // included in the embedding.
                else if (charData.CharType == BidiCharacterType.BN)
                {
                    charData.EmbLevel = embeddingLevel;
                }
            }
            #endregion

            // X10. The remaining rules are applied to each run of characters at the same level.
            int prevLevel = EmbeddingLevel;
            int start = 0;
            while (start < mText.Length)
            {
                #region rule X10 - run level setup
                int level = mTextData[start].EmbLevel;
                BidiCharacterType sor = TypeForLevel(System.Math.Max(prevLevel, level));

                int limit = start + 1;
                while (limit < mText.Length && mTextData[limit].EmbLevel == level)
                    ++limit;

                int nextLevel = limit < mText.Length ? mTextData[limit].EmbLevel : EmbeddingLevel;
                BidiCharacterType eor = TypeForLevel(System.Math.Max(nextLevel, level));
                #endregion

                ResolveWeakTypes(start, limit, sor, eor);
                ResolveNeutralTypes(start, limit, sor, eor, level);
                ResolveImplicitTypes(start, limit, level);

                prevLevel = level;
                start = limit;
            }




            if (!forVisible)
            {
                CalcBidiRuns();
                return;
            }

            ReorderString();

            FixMirroredCharacters();

            StringBuilder sb = new StringBuilder(mTextData.Length);
            foreach (BidiCharData cd in mTextData)
            {
                sb.Append(cd.Char);
            }
            mBidiText = sb.ToString();
        }

        /// <summary>
        /// 3.3.3 Resolving Weak Types
        /// </summary>
        private void ResolveWeakTypes(int start, int limit, BidiCharacterType sor, BidiCharacterType eor)
        {
            int i;
            // TODO - all these repeating runs seems somewhat unefficient...
            // TODO - rules 2 and 7 are the same, except for minor parameter changes...

            #region rule W1
            // W1. Examine each nonspacing mark (NSM) in the level run, and change the type of the NSM to the type of the previous character. 
            // If the NSM is at the start of the level run, it will get the type of sor.
            if (mHasNSMs)
            {
                BidiCharacterType preceedingCharacterType = sor;
                for (i = start; i < limit; ++i)
                {
                    BidiCharacterType t = mTextData[i].CharType;
                    if (t == BidiCharacterType.NSM)
                        mTextData[i].CharType = preceedingCharacterType;
                    else
                        preceedingCharacterType = t;
                }
            }
            #endregion

            #region rule W2
            // W2. Search backward from each instance of a European number until the first strong type (R, L, AL, or sor) is found. 
            // If an AL is found, change the type of the European number to Arabic number.

            BidiCharacterType t_w2 = BidiCharacterType.EN;
            for (i = start; i < limit; ++i)
            {
                if (mTextData[i].CharType == BidiCharacterType.L || mTextData[i].CharType == BidiCharacterType.R)
                    t_w2 = BidiCharacterType.EN;
                else if (mTextData[i].CharType == BidiCharacterType.AL)
                    t_w2 = BidiCharacterType.AN;
                else if (mTextData[i].CharType == BidiCharacterType.EN)
                    mTextData[i].CharType = t_w2;
            }
            #endregion

            #region rule #3
            // W3. Change all ALs to R.
            if (mHasArabic)
            {
                for (i = start; i < limit; ++i)
                {
                    if (mTextData[i].CharType == BidiCharacterType.AL)
                        mTextData[i].CharType = BidiCharacterType.R;
                }
            }
            #endregion

            #region rule W4
            // W4. A single European separator between two European numbers changes to a European number. A single common separator between two numbers of the same type changes to that type.

            // Since there must be values on both sides for this rule to have an
            // effect, the scan skips the first and last value.
            //
            // Although the scan proceeds left to right, and changes the type values
            // in a way that would appear to affect the computations later in the scan,
            // there is actually no problem.  A change in the current value can only
            // affect the value to its immediate right, and only affect it if it is
            // ES or CS.  But the current value can only change if the value to its
            // right is not ES or CS.  Thus either the current value will not change,
            // or its change will have no effect on the remainder of the analysis.

            for (i = start + 1; i < limit - 1; ++i)
            {
                if (mTextData[i].CharType == BidiCharacterType.ES || mTextData[i].CharType == BidiCharacterType.CS)
                {
                    BidiCharacterType prevSepType = mTextData[i - 1].CharType;
                    BidiCharacterType succSepType = mTextData[i + 1].CharType;
                    if (prevSepType == BidiCharacterType.EN && succSepType == BidiCharacterType.EN)
                    {
                        mTextData[i].CharType = BidiCharacterType.EN;
                    }
                    else if (mTextData[i].CharType == BidiCharacterType.CS && prevSepType == BidiCharacterType.AN && succSepType == BidiCharacterType.AN)
                    {
                        mTextData[i].CharType = BidiCharacterType.AN;
                    }
                }
            }
            #endregion

            #region rule W5
            // W5. A sequence of European terminators adjacent to European numbers changes to all European numbers.
            i = start;
            while (i < limit)
            {
                if (mTextData[i].CharType == BidiCharacterType.ET)
                {
                    // locate end of sequence
                    int runstart = i;
                    int runlimit = FindRunLimit(runstart, limit, ValidEt);

                    // check values at ends of sequence
                    BidiCharacterType t = runstart == start ? sor : mTextData[runstart - 1].CharType;

                    if (t != BidiCharacterType.EN)
                        t = runlimit == limit ? eor : mTextData[runlimit].CharType;

                    if (t == BidiCharacterType.EN)
                        SetTypes(runstart, runlimit, BidiCharacterType.EN);

                    // continue at end of sequence
                    i = runlimit;
                }

                ++i;
            }
            #endregion

            #region rule W6
            // W6. Otherwise, separators and terminators change to Other Neutral.
            for (i = start; i < limit; ++i)
            {
                BidiCharacterType t = mTextData[i].CharType;
                if (t == BidiCharacterType.ES || t == BidiCharacterType.ET || t == BidiCharacterType.CS)
                {
                    mTextData[i].CharType = BidiCharacterType.ON;
                }
            }
            #endregion

            #region rule W7
            // W7. Search backward from each instance of a European number until the first strong type (R, L, or sor) is found.
            //     If an L is found, then change the type of the European number to L.

            BidiCharacterType t_w7 = sor == BidiCharacterType.L ? BidiCharacterType.L : BidiCharacterType.EN;
            for (i = start; i < limit; ++i)
            {
                if (mTextData[i].CharType == BidiCharacterType.R)
                    t_w7 = BidiCharacterType.EN;
                else if (mTextData[i].CharType == BidiCharacterType.L)
                    t_w7 = BidiCharacterType.L;
                else if (mTextData[i].CharType == BidiCharacterType.EN)
                    mTextData[i].CharType = t_w7;
            }
            #endregion
        }

        /// <summary>
        /// 3.3.4 Resolving Neutral Types
        /// </summary>
        private void ResolveNeutralTypes(int start, int limit, BidiCharacterType sor, BidiCharacterType eor, int level)
        {
            // N1. A sequence of neutrals takes the direction of the surrounding strong text if the text on both sides has the same direction. 
            //     European and Arabic numbers act as if they were R in terms of their influence on neutrals.
            //     Start-of-level-run (sor) and end-of-level-run (eor) are used at level run boundaries.
            // N2. Any remaining neutrals take the embedding direction.

            int i = start;
            while (i < limit)
            {
                BidiCharacterType t = mTextData[i].CharType;
                if (t == BidiCharacterType.WS || t == BidiCharacterType.ON || t == BidiCharacterType.B || t == BidiCharacterType.S)
                {
                    // find bounds of run of neutrals
                    int runstart = i;
                    int runlimit = FindRunLimit(runstart, limit, ValidBSWsOn);

                    // determine effective types at ends of run
                    BidiCharacterType leadingType;
                    BidiCharacterType trailingType;

                    if (runstart == start)
                        leadingType = sor;
                    else
                    {
                        leadingType = mTextData[runstart - 1].CharType;
                        if (leadingType == BidiCharacterType.AN || leadingType == BidiCharacterType.EN)
                            leadingType = BidiCharacterType.R;
                    }

                    if (runlimit == limit)
                        trailingType = eor;
                    else
                    {
                        trailingType = mTextData[runlimit].CharType;
                        if (trailingType == BidiCharacterType.AN || trailingType == BidiCharacterType.EN)
                            trailingType = BidiCharacterType.R;
                    }

                    BidiCharacterType resolvedType;
                    if (leadingType == trailingType)
                    {
                        // Rule N1.
                        resolvedType = leadingType;
                    }
                    else
                    {
                        // Rule N2.
                        // Notice the embedding level of the run is used, not
                        // the paragraph embedding level.
                        resolvedType = TypeForLevel(level);
                    }

                    SetTypes(runstart, runlimit, resolvedType);

                    // skip over run of (former) neutrals
                    i = runlimit;
                }

                ++i;
            }
        }

        /// <summary>
        /// 3.3.5 Resolving Implicit Levels
        /// </summary>
        private void ResolveImplicitTypes(int start, int limit, int level)
        {
            // I1. For all characters with an even (left-to-right) embedding direction, those of type R go up one level and those of type AN or EN go up two levels.
            // I2. For all characters with an odd (right-to-left) embedding direction, those of type L, EN or AN go up one level.

            if ((level & 1) == 0) // even level
            {
                for (int i = start; i < limit; ++i)
                {
                    BidiCharacterType t = mTextData[i].CharType;
                    // Rule I1.
                    if (t == BidiCharacterType.R)
                    {
                        // Cannot be += because of autoporting.
                        mTextData[i].EmbLevel = mTextData[i].EmbLevel + 1;
                    }
                    else if (t == BidiCharacterType.AN || t == BidiCharacterType.EN)
                    {
                        // Cannot be += because of autoporting.
                        mTextData[i].EmbLevel = mTextData[i].EmbLevel + 2;
                    }
                }
            }
            else // odd level
            {
                for (int i = start; i < limit; ++i)
                {
                    BidiCharacterType t = mTextData[i].CharType;
                    // Rule I2.
                    if (t == BidiCharacterType.L || t == BidiCharacterType.AN || t == BidiCharacterType.EN)
                    {
                        // Cannot be += because of autoporting.
                        mTextData[i].EmbLevel = mTextData[i].EmbLevel + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 3.4 Reordering Resolved Levels
        /// </summary>
        private void ReorderString()
        {
            //L1. On each line, reset the embedding level of the following characters to the paragraph embedding level:
            //    1. Segment separators,
            //    2. Paragraph separators,
            //    3. Any sequence of whitespace characters preceding a segment separator or paragraph separator, and
            //    4. Any sequence of white space characters at the end of the line.

            int l1_start = 0;
            for (int i = 0; i < mTextData.Length; ++i)
            {
                if (mTextData[i].CharType == BidiCharacterType.S || mTextData[i].CharType == BidiCharacterType.B)
                {
                    for (int j = l1_start; j <= i; ++j)
                        mTextData[j].EmbLevel = EmbeddingLevel;
                }

                if (mTextData[i].CharType != BidiCharacterType.WS)
                {
                    l1_start = i + 1;
                }
            }
            for (int j = l1_start; j < mTextData.Length; ++j)
                mTextData[j].EmbLevel = EmbeddingLevel;

            // L2. From the highest level found in the text to the lowest odd level on each
            //     line, including intermediate levels not actually present in the text,
            //     reverse any contiguous sequence of characters that are at that level or
            //     higher.
            int highest = 0;
            int lowest_odd = 63;
            foreach (BidiCharData cd in mTextData)
            {
                if (cd.EmbLevel > highest)
                    highest = cd.EmbLevel;
                if ((cd.EmbLevel & 1) == 1 && cd.EmbLevel < lowest_odd)
                    lowest_odd = cd.EmbLevel;
            }

            for (int el = highest; el >= lowest_odd; --el)
            {
                int i = 0;
                while (i < mTextData.Length)
                {
                    if (mTextData[i].EmbLevel >= el)
                    {
                        // find range of text at or above this level
                        int l2_start = i;
                        int limit = i + 1;
                        while (limit < mTextData.Length && mTextData[limit].EmbLevel >= el)
                        {
                            ++limit;
                        }

                        // reverse run
                        for (int j = l2_start, k = limit - 1; j < k; ++j, --k)
                        {
                            BidiCharData temp_cd = mTextData[j];
                            mTextData[j] = mTextData[k];
                            mTextData[k] = temp_cd;
                        }

                        // skip to end of level run
                        i = limit;
                    }

                    ++i;
                }
            }

            // TODO - L3. Combining marks applied to a right-to-left base character will at this point precede their base 
            // character. If the rendering engine expects them to follow the base characters in the final display process,
            // then the ordering of the marks and the base character must be reversed.
        }

        /// <summary>
        /// L4. A character is depicted by a mirrored glyph if and only if (a) the resolved directionality of that character is R, and (b) the Bidi_Mirrored property value of that character is true.
        /// </summary>
        private void FixMirroredCharacters()
        {
            for (int i = 0; i < mTextData.Length; ++i)
            {
                if ((mTextData[i].EmbLevel & 1) == 1)
                    mTextData[i].Char = BidiCharacterMirrorResolver.GetBidiCharacterMirror(mTextData[i].Char);
            }
        }

        /// <summary>
        /// 3.5 Shaping
        /// Implements rules R1-R7 and rules L1-L3 of section 8.2 (Arabic) of the Unicode standard.
        /// </summary>
        // TODO - this code is very special-cased.
        private static string PerformArabicShaping(string text)
        {
            ArabicShapeJoiningType last_jt = ArabicShapeJoiningType.U;
            LetterForm last_form = LetterForm.Isolated;
            int last_pos = 0;
            char last_char;
            LetterForm[] letterForms = new LetterForm[text.Length];

            for (int curr_pos = 0; curr_pos < text.Length; ++curr_pos)
            {
                char ch = text[curr_pos];

                ArabicShapeJoiningType jt = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(ch);
                if ((jt == ArabicShapeJoiningType.R ||
                     jt == ArabicShapeJoiningType.D ||
                     jt == ArabicShapeJoiningType.C) &&
                    (last_jt == ArabicShapeJoiningType.L ||
                     last_jt == ArabicShapeJoiningType.D ||
                     last_jt == ArabicShapeJoiningType.C))
                {
                    if (last_form == LetterForm.Isolated && (last_jt == ArabicShapeJoiningType.D ||
                                                                   last_jt == ArabicShapeJoiningType.L))
                    {
                        letterForms[last_pos] = LetterForm.Initial;
                    }
                    else if (last_form == LetterForm.Final && last_jt == ArabicShapeJoiningType.D)
                    {
                        letterForms[last_pos] = LetterForm.Medial;
                    }
                    letterForms[curr_pos] = LetterForm.Final;
                    last_form = LetterForm.Final;
                    last_jt = jt;
                    last_pos = curr_pos;
                }
                else if (jt != ArabicShapeJoiningType.T)
                {
                    letterForms[curr_pos] = LetterForm.Isolated;
                    last_form = LetterForm.Isolated;
                    last_jt = jt;
                    last_pos = curr_pos;
                }
                else
                    letterForms[curr_pos] = LetterForm.Isolated;
            }

            last_char = BidiChars.NotAChar;
            last_pos = 0;
            int insert_pos = 0;

            StringBuilder sb = new StringBuilder();

            for (int curr_pos = 0; curr_pos < text.Length; ++curr_pos)
            {
                char ch = text[curr_pos];
                ArabicShapeJoiningType jt = UnicodeArabicShapingResolver.GetArabicShapeJoiningType(ch);

                if (last_char == BidiChars.ARABIC_LAM &&
                    ch != BidiChars.ARABIC_ALEF &&
                    ch != BidiChars.ARABIC_ALEF_MADDA_ABOVE &&
                    ch != BidiChars.ARABIC_ALEF_HAMZA_ABOVE &&
                    ch != BidiChars.ARABIC_ALEF_HAMZA_BELOW &&
                    jt != ArabicShapeJoiningType.T)
                {
                    last_char = BidiChars.NotAChar;
                }
                else if (ch == BidiChars.ARABIC_LAM)
                {
                    last_char = ch;
                    last_pos = curr_pos;
                    insert_pos = sb.Length;
                }

                if (last_char == BidiChars.ARABIC_LAM)
                {
                    if (letterForms[last_pos] == LetterForm.Medial)
                    {
                        switch (ch)
                        {
                            case BidiChars.ARABIC_ALEF:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_FINAL;
                                continue;

                            case BidiChars.ARABIC_ALEF_MADDA_ABOVE:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_MADDA_ABOVE_FINAL;
                                continue;

                            case BidiChars.ARABIC_ALEF_HAMZA_ABOVE:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_HAMZA_ABOVE_FINAL;
                                continue;

                            case BidiChars.ARABIC_ALEF_HAMZA_BELOW:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_HAMZA_BELOW_FINAL;
                                continue;
                            default:
                                break;
                        }
                    }
                    else if (letterForms[last_pos] == LetterForm.Initial)
                    {
                        switch (ch)
                        {
                            case BidiChars.ARABIC_ALEF:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_ISOLATED;
                                continue;

                            case BidiChars.ARABIC_ALEF_MADDA_ABOVE:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_MADDA_ABOVE_ISOLATED;
                                continue;

                            case BidiChars.ARABIC_ALEF_HAMZA_ABOVE:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_HAMZA_ABOVE_ISOLATED;
                                continue;

                            case BidiChars.ARABIC_ALEF_HAMZA_BELOW:
                                sb[insert_pos] = BidiChars.ARABIC_LAM_ALEF_HAMZA_BELOW_ISOLATED;
                                continue;
                            default:
                                break;
                        }
                    }
                }

                sb.Append(UnicodeArabicShapingResolver.GetArabicCharacterByLetterForm(ch, letterForms[curr_pos]));
            }

            return sb.ToString();
        }

        private static string InternalCompose(char[] decomposedText)
        {
            // WORDSNET-10227 Aspose.Words takes long time to load a HTML
            // Type of 'target' was changed from StringBuilder to char[].
            // Performance problems occur only if application is assembled with .NET Framework 4 and higher. 
            // In StringBuilder for .Net Framework 2 and 4 indexer implementation is different. 
            // In .NET Framework 4 "target[compPos] = ch" operation consumed a lot of resources.

            if (decomposedText.Length == 0)
            {
                return string.Empty;
            }

            char[] result = new char[decomposedText.Length];

            int starterPos = 0;
            int resultLength = 1;
            result[0] = decomposedText[0];
            char starterCh = decomposedText[0];

            UnicodeCanonicalClass lastClass = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(starterCh);

            if (lastClass != UnicodeCanonicalClass.NR)
                lastClass = (UnicodeCanonicalClass)256; // fix for strings staring with a combining mark

            // Loop on the decomposed characters, combining where possible
            for (int i = 1; i < decomposedText.Length; i++)
            {
                char ch = decomposedText[i];
                UnicodeCanonicalClass chClass = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(ch);
                char composite = UnicodeCharacterDataResolver.Compose(starterCh, ch);
                UnicodeDecompositionType composeType = UnicodeCharacterDataResolver.GetUnicodeDecompositionType(composite);

                if ((composeType == UnicodeDecompositionType.None) &&
                    (composite != BidiChars.NotAChar) &&
                    ((lastClass < chClass) || (lastClass == UnicodeCanonicalClass.NR)))
                {
                    result[starterPos] = composite;
                    // we know that we will only be replacing non-supplementaries by non-supplementaries
                    // so we don't have to adjust the decompPos
                    starterCh = composite;
                }
                else
                {
                    if (chClass == UnicodeCanonicalClass.NR)
                    {
                        starterPos = resultLength;
                        starterCh = ch;
                    }
                    lastClass = chClass;

                    result[resultLength] = ch;
                    ++resultLength;
                }
            }

            return new string(result, 0, resultLength);
        }

        private static void GetRecursiveDecomposition(bool canonical, char ch, StringBuilder builder)
        {
            string decomp = UnicodeCharacterDataResolver.GetUnicodeDecompositionMapping(ch);
            if ((decomp != null) && !(canonical && UnicodeCharacterDataResolver.GetUnicodeDecompositionType(ch) != UnicodeDecompositionType.None))
            {
                for (int i = 0; i < decomp.Length; ++i)
                    GetRecursiveDecomposition(canonical, decomp[i], builder);
            }
            else
            {
                // if no decomp, append
                builder.Append(ch);
            }
        }

        private char[] InternalDecompose(bool canonicalDecomposition)
        {
            StringBuilder target = new StringBuilder();
            StringBuilder buffer = new StringBuilder();

            mHasArabic = false;
            mHasNSMs = false;

            for (int i = 0; i < mText.Length; ++i)
            {
                BidiCharacterType ct = UnicodeCharacterDataResolver.GetBidiCharacterType(mText[i]);
                mHasArabic |= ((ct == BidiCharacterType.AL) || (ct == BidiCharacterType.AN));
                mHasNSMs |= (ct == BidiCharacterType.NSM);

                buffer.Length = 0;
                GetRecursiveDecomposition(canonicalDecomposition, mText[i], buffer);
                // add all of the characters in the decomposition.
                // (may be just the original character, if there was
                // no decomposition mapping)

                for (int j = 0; j < buffer.Length; ++j)
                {
                    char ch = buffer[j];
                    UnicodeCanonicalClass chClass = UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(ch);
                    int k = target.Length; // insertion point
                    if (chClass != UnicodeCanonicalClass.NR)
                    {
                        // bubble-sort combining marks as necessary
                        for (; k > 0; --k)
                        {
                            char ch2 = target[k - 1];
                            if (UnicodeCharacterDataResolver.GetUnicodeCanonicalClass(ch2) <= chClass)
                                break;
                        }
                    }

                    // Performance optimization. sb.Append(ch) works faster than sb.Insert(sb.Length, ch).
                    if (target.Length == k)
                        target.Append(ch);
                    else
                        target.Insert(k, ch);
                }
            }
            return target.ToString().ToCharArray();
        }

        /// <summary>
        /// Return the strong type (L or R) corresponding to the embedding level.
        /// </summary>
        /// <param name="level">The embedding level to check.</param>
        /// <returns></returns>
        private static BidiCharacterType TypeForLevel(int level)
        {
            return ((level & 1) == 0) ? BidiCharacterType.L : BidiCharacterType.R;
        }

        private readonly BidiCharacterType[] ValidEt = new BidiCharacterType[] { BidiCharacterType.ET };
        private readonly BidiCharacterType[] ValidBSWsOn = new BidiCharacterType[] { BidiCharacterType.B, BidiCharacterType.S, BidiCharacterType.WS, BidiCharacterType.ON };

        /// <summary>
        /// Return the limit of the run, starting at index, that includes only resultTypes in validSet.
        /// This checks the value at index, and will return index if that value is not in validSet.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="limit"></param>
        /// <param name="validSet"></param>
        /// <returns></returns>
        private int FindRunLimit(int index, int limit, BidiCharacterType[] validSet)
        {
            --index;
            while (++index < limit)
            {
                BidiCharacterType t = mTextData[index].CharType;
                bool found = false;
                for (int i = 0; i < validSet.Length && !found; ++i)
                {
                    if (t == validSet[i])
                        found = true;
                }

                if (!found)
                    return index; // didn't find a match in validSet
            }
            return limit;
        }

        /// <summary>
        /// Set resultTypes from start up to (but not including) limit to newType.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="newType"></param>
        private void SetTypes(int start, int limit, BidiCharacterType newType)
        {
            for (int i = start; i < limit; ++i)
                mTextData[i].CharType = newType;
        }

        /// <summary>
        /// Split text to runs of diferent embeddable levels
        /// </summary>
        private void CalcBidiRuns()
        {
            if (mCalcRunsCharDataModifier != null)
                mCalcRunsCharDataModifier.Modify(this, mTextData);

            const string controlChars = "\u200F\u202B\u202E\u200E\u202A\u202D\u202C";
            int tempLevel = -1;
            StringBuilder tempText = new StringBuilder();
            mBidiRuns = new List<BidiRun>();
            for (int i = 0; i < mTextData.Length; i++)
            {
                if (controlChars.IndexOf(mTextData[i].Char) == -1)
                {
                    if ((mTextData[i].EmbLevel != tempLevel) || (tempLevel == -1))
                    {
                        if (tempLevel != -1)
                            mBidiRuns.Add(new BidiRun(tempText.ToString(), tempLevel));
                        tempText.Length = 0;
                        tempLevel = mTextData[i].EmbLevel;
                    }
                    tempText.Append(mTextData[i].Char);
                }
            }
            mBidiRuns.Add(new BidiRun(tempText.ToString(), tempLevel));
        }
    }
}
