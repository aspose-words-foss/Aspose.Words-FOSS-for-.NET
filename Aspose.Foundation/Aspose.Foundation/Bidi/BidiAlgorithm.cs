// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2014 by Victor Chebotok

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Collections;

namespace Aspose.Bidi
{
    /// <summary>
    /// Implements the Unicode bidirectional algorithm.
    /// </summary>
    /// <remarks>
    /// The Unicode bidirectional algorithm examines implicit and explicit directionality of text characters and splits
    /// the text into left-to-right and right-to-left runs.
    /// For description of the algorithm see http://www.unicode.org/reports/tr9/
    /// Our implementation also reorders the resulting runs in some cases to convert the text from visual to logical order
    /// and to arrange runs as required by MS Word.
    /// </remarks>
    /// JAVA: mData and gFieldMasks arrays type is changed from unsigned long to (signed) long for autoportability to Java.
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class BidiAlgorithm
    {
        /// <summary>
        /// Processes text with the Unicode bidirectional algorithm.
        /// </summary>
        /// <param name="text">The text to process.</param>
        /// <param name="isRtlParagraph">
        /// Indicates the direction of the paragraph that the text belongs to.
        /// <c>false</c> - left-to-right (LTR), <c>true</c> - right-to-left (RTL).
        /// </param>
        /// <returns>The text split into an array of runs by bidirectional embedding level.</returns>
        public static BidiRun[] Apply(string text, bool isRtlParagraph)
        {
            return Apply(new BidiSourceRun[] {new BidiSourceRun(text)}, isRtlParagraph, false, false);
        }

        /// <summary>
        /// Processes text with the Unicode bidirectional algorithm.
        /// </summary>
        /// <param name="sourceRuns">Runs of text to process.</param>
        /// <param name="isRtlParagraph">
        /// Indicates the direction of the paragraph that the text belongs to.
        /// <c>false</c> - left-to-right (LTR), <c>true</c> - right-to-left (RTL).
        /// </param>
        /// <param name="isVisuallyOrdered"></param>
        /// <param name="convertToMsWordOrder">
        /// Indicates whether the resulting runs must be rearranged in order required by MS Word.
        /// If this flag is <c>false</c>, the algorithm will split the text into runs as described in the specification.
        /// If this flag is <c>true</c>, the resulting runs will be additionally processed to put them into the document model
        /// (embedded RTL fragments will be reversed and level of numeric runs will be decreased).
        /// </param>
        /// <returns>The text split into an array of runs by bidirectional embedding level.</returns>
        public static BidiRun[] Apply(BidiSourceRun[] sourceRuns, bool isRtlParagraph, bool isVisuallyOrdered,
            bool convertToMsWordOrder)
        {
            BidiAlgorithm algorithm = new BidiAlgorithm(sourceRuns, isRtlParagraph, isVisuallyOrdered, convertToMsWordOrder);
            algorithm.Apply();
            return algorithm.GetResult();
        }

        /// <summary>
        /// Determines implicit paragraph level of text.
        /// </summary>
        /// <remarks>
        /// Corresponds to the P2 rule of the Unicode bidirectional algorithm (see https://unicode.org/reports/tr9/#P2).
        /// </remarks>
        public static bool IsRtlParagraph(string text)
        {
            // Search for a strong character.
            foreach (char c in text)
            {
                BidiCharacterType bidiType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
                if ((bidiType == BidiCharacterType.R) || (bidiType == BidiCharacterType.AL))
                {
                    // Found an RTL strong character.
                    return true;
                }
                else if (bidiType == BidiCharacterType.L)
                {
                    // Found an LTR strong character.
                    return false;
                }
            }
            // No strong characters found.
            return false;
        }

        /// <summary>
        /// Reorders the runs so that they come in visual order and don't have nested LTR/RTL ranges.
        /// </summary>
        /// <remarks>
        /// Uses the reordering part of the Unicode bidirectional algorithm, described here:
        /// http://www.unicode.org/reports/tr9/#L2
        /// </remarks>
        public static void ArrangeInVisualOrder(BidiRun[] runs)
        {
            // We use zero paragraph embedding level that corresponds to LTR paragraph direction, because our renderers
            // move from left to right when they populate the APS model with glyphs.
            FlatternEmbeddingLevels(runs, 0);
        }

        /// <summary>
        /// Processes text with the Unicode bidirectional algorithm.
        /// </summary>
        /// <param name="sourceRuns">Runs of text to process.</param>
        /// <param name="isRtlParagraph">
        /// Indicates the direction of the paragraph that the text belongs to.
        /// <c>false</c> - left-to-right (LTR), <c>true</c> - right-to-left (RTL).
        /// </param>
        /// <param name="isVisuallyOrdered"></param>
        /// <param name="convertToMsWordOrder">
        /// Indicates whether the resulting runs must be rearranged in order required by MS Word.
        /// If this flag is <c>false</c>, the algorithm will split the text into runs as described in the specification.
        /// If this flag is <c>true</c>, the resulting runs will be additionaly processed to put them into the document model
        /// (embedded RTL fragments will be reversed and level of numeric runs will be decreased).
        /// </param>
        /// <returns>The text split into an array of runs by bidirectional embedding level.</returns>
        private BidiAlgorithm(BidiSourceRun[] sourceRuns, bool isRtlParagraph, bool isVisuallyOrdered, bool convertToMsWordOrder)
        {
            mConvertVisualToLogicalOrder = isVisuallyOrdered;
            mConvertToMsWordOrder = convertToMsWordOrder;
            mIsRtlParagraph = isRtlParagraph;

            // Set the paragraph embedding level.
            // WORDSNET-7347 Visually ordered text is an RTL text which includes some LTR fragments,
            // so it should be processed as an RTL text by the bidirectional algorithm.
            mParagraphEmbeddingLevel = (isRtlParagraph || mConvertVisualToLogicalOrder) ? 1 : 0;

            // If text is in visual order, its runs are also in visual order.
            // To convert visually ordered text of an RTL paragraph to logical order, we need to reverse the order of the runs.
            bool reverseRunOrder = (mConvertVisualToLogicalOrder && isRtlParagraph);
 
            Initialize(sourceRuns, reverseRunOrder);
        }

        /// <summary>
        /// Fills data structures used by the Unicode bidirectional algorithm.
        /// </summary>
        private void Initialize(BidiSourceRun[] sourceRuns, bool reverseRunOrder)
        {
            Debug.Assert(!mAlgorithmIsApplied);

            // Calculate the text length.
            BidiLevelList activeLevels = new BidiLevelList();
            int textLength = 0;
            for (int i = 0; i < sourceRuns.Length; i++)
            {
                BidiSourceRun run = sourceRuns[i];
                textLength += run.Text.Length;
                textLength += GetTransitionControlCharacters(activeLevels, run.Levels).Length;
                activeLevels = run.Levels;
            }
            textLength += GetTrailingControlCharacters(activeLevels).Length;

            // Allocate space for data structures.
            mData = new long[textLength];

            // Fill the data required by the algorithm.
            activeLevels = new BidiLevelList();
            int charIndex = 0;
            for (int i = 0; i < sourceRuns.Length; i++)
            {
                int runIndex = (reverseRunOrder)
                    ? sourceRuns.Length - i - 1
                    : i;
                BidiSourceRun run = sourceRuns[runIndex];

                // Write transition bidi control characters.
                foreach (char controlChar in GetTransitionControlCharacters(activeLevels, run.Levels))
                {
                    InitializeChar(charIndex, controlChar, runIndex);
                    ++charIndex;
                }
                activeLevels = run.Levels;

                // Write the run text.
                for (int j = 0; j < run.Text.Length; j++)
                {
                    InitializeChar(charIndex, run.Text[j], runIndex, run.UseHebrewNumberSeparators, run.IgnoreBidiTypeET);
                    ++charIndex;
                }
            }
            // Write trailing bidi control characters.
            foreach (char controlChar in GetTrailingControlCharacters(activeLevels))
            {
                InitializeChar(charIndex, controlChar, sourceRuns.Length - 1);
                ++charIndex;
            }
        }

        /// <summary>
        /// Gets Unicode control characters needed to change from one stack of bidi levels to another.
        /// </summary>
        private static char[] GetTransitionControlCharacters(BidiLevelList fromLevels, BidiLevelList toLevels)
        {
            Debug.Assert(fromLevels != null);
            Debug.Assert(toLevels != null);

            // Count the number of matched levels.
            int matchCount = 0;
            while ((matchCount < fromLevels.Length) &&
                (matchCount < toLevels.Length) &&
                (fromLevels[matchCount] == toLevels[matchCount]))
            {
                ++matchCount;
            }

            int resultLength = (fromLevels.Length - matchCount) + (toLevels.Length - matchCount);
            char[] result = new char[resultLength];
            int resultIndex = 0;

            // Pop all previous unmatched levels.
            for (int j = matchCount; j < fromLevels.Length; j++)
            {
                result[resultIndex] = BidiChars.PDF;
                ++resultIndex;
            }

            // Open new unmatched levels.
            for (int j = matchCount; j < toLevels.Length; j++)
            {
                result[resultIndex] = GetOpeningControlChar(toLevels[j]);
                ++resultIndex;
            }

            return result;
        }

        /// <summary>
        /// Gets a Unicode bidi control character that opens the specified bidi level.
        /// </summary>
        private static char GetOpeningControlChar(BidiLevel level)
        {
            switch (level)
            {
                case BidiLevel.EmbedLtr:
                    return BidiChars.LRE;
                case BidiLevel.EmbedRtl:
                    return BidiChars.RLE;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        /// <summary>
        /// Gets Unicode control characters needed to close all of the specified bidi levels.
        /// </summary>
        private static char[] GetTrailingControlCharacters(BidiLevelList levels)
        {
            Debug.Assert(levels != null);
            return GetTransitionControlCharacters(levels, new BidiLevelList());
        }

        private void InitializeChar(int index, char c, int sourceRunIndex)
        {
            InitializeChar(index, c, sourceRunIndex, true, false);
        }

        private void InitializeChar(int index, char c, int sourceRunIndex, bool useHebrewSeparators, bool ignoreBidiTypeET)
        {
            SetChar(index, c);

            BidiCharacterType bidiType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);

            if (mIsRtlParagraph && bidiType == BidiCharacterType.ET && ignoreBidiTypeET)
            {
                bidiType = BidiCharacterType.ON;
            }

            SetBidiType(index, bidiType);
            SetEmbeddingLevel(index, 0);
            SetSourceRunIndex(index, sourceRunIndex);
            SetUseHebrewSeparators(index, useHebrewSeparators);

            if ((bidiType == BidiCharacterType.AL) || (bidiType == BidiCharacterType.AN))
            {
                mHasArabic = true;
            }
            if (bidiType == BidiCharacterType.NSM)
            {
                mHasNsm = true;
            }
        }

        private char GetChar(int index)
        {
            return (char)GetFieldValue(index, CharFieldIndex);
        }

        private void SetChar(int index, char c)
        {
            SetFieldValue(index, CharFieldIndex, c);
        }

        private BidiCharacterType GetBidiType(int index)
        {
            return (BidiCharacterType)GetFieldValue(index, BidiTypeFieldIndex);
        }

        private void SetBidiType(int index, BidiCharacterType bidiCharacterType)
        {
            SetFieldValue(index, BidiTypeFieldIndex, (long)bidiCharacterType);
        }

        private int GetEmbeddingLevel(int index)
        {
            return (int)GetFieldValue(index, EmbeddingLevelFieldIndex);
        }

        private void SetEmbeddingLevel(int index, int embeddingLevel)
        {
            SetFieldValue(index, EmbeddingLevelFieldIndex, (long)embeddingLevel);
        }

        private bool GetRequiresHebrewLocale(int index)
        {
            return GetFieldValue(index, RequiresHebrewLocaleFieldIndex) != 0;
        }

        private void SetRequiresHebrewLocale(int index, bool value)
        {
            SetFieldValue(index, RequiresHebrewLocaleFieldIndex, (value) ? 1L : 0L);
        }

        private bool GetUseHebrewSeparators(int index)
        {
            return GetFieldValue(index, UseHebrewSeparatorsFieldIndex) != 0;
        }

        private void SetUseHebrewSeparators(int index, bool value)
        {
            SetFieldValue(index, UseHebrewSeparatorsFieldIndex, (value) ? 1L : 0L);
        }

        private int GetSourceRunIndex(int index)
        {
            return (int)GetFieldValue(index, SourceRunIndexFieldIndex);
        }

        private void SetSourceRunIndex(int index, int sourceRunIndex)
        {
            SetFieldValue(index, SourceRunIndexFieldIndex, (long)sourceRunIndex);
        }

        private long GetFieldValue(int index, int fieldIndex)
        {
#if JAVA
            //JAVA-changed:
            return (mData[index] & gFieldMasks[fieldIndex]) >>> G_FIELD_SHIFTS[fieldIndex];
#else
            // Emulation of unsigned right shift.
            return (long) (((ulong)(mData[index] & gFieldMasks[fieldIndex])) >> gFieldShifts[fieldIndex]);
#endif
        }

        private void SetFieldValue(int index, int fieldIndex, long value)
        {
            // We are packing a value into a limited-length field. Let's make sure the value actually fits the field and we
            // don't clear any significant value bits by applying the mask. All value bits outside the mask should be zero.
            // If the value don't fit the target field, the field should be resized.
            Debug.Assert((value & (~(gFieldMasks[fieldIndex] >> gFieldShifts[fieldIndex]))) == 0);

            long oldValue = mData[index];
            long newFieldValue = (value << gFieldShifts[fieldIndex]) & gFieldMasks[fieldIndex];
            // Re-pack data. Merge the masked value with other fields. The code is taken from here:
            // https://graphics.stanford.edu/~seander/bithacks.html#MaskedMerge
            mData[index] = oldValue ^ ((oldValue ^ newFieldValue) & gFieldMasks[fieldIndex]); 
        }

        /// <summary>
        /// Applies the Unicode bidirectional algorithm to the text.
        /// </summary>
        private void Apply()
        {
            // The bidirectional algorithm must be applied to the text only once.
            Debug.Assert(!mAlgorithmIsApplied);
            if (mAlgorithmIsApplied)
            {
                return;
            }

#region rules X1 - X9
            // X1
            int lastCharEmbeddingLevel = mParagraphEmbeddingLevel;
            int embeddingLevel = mParagraphEmbeddingLevel;
            DirectionalOverrideStatus dos = DirectionalOverrideStatus.Neutral;
            IntStack dosStack = new IntStack();
            IntList elStack = new IntList();
            for (int i = 0; i < mData.Length; ++i)
            {
                BidiCharacterType bidiType = GetBidiType(i);

#region rules X2 - X5
                // X2. With each RLE, compute the least greater odd embedding level.
                // X4. With each RLO, compute the least greater odd embedding level.
                if ((bidiType == BidiCharacterType.RLE) || (bidiType == BidiCharacterType.RLO))
                {
                    SetEmbeddingLevel(i, lastCharEmbeddingLevel);
                    if (embeddingLevel < 60)
                    {
                        elStack.Add(embeddingLevel);
                        dosStack.Push((int)dos);

                        ++embeddingLevel;
                        embeddingLevel |= 1;

                        dos = (bidiType == BidiCharacterType.RLE)
                            ? DirectionalOverrideStatus.Neutral
                            : DirectionalOverrideStatus.RTL;
                    }
                }
                // X3. With each LRE, compute the least greater even embedding level.
                // X5. With each LRO, compute the least greater even embedding level.
                else if ((bidiType == BidiCharacterType.LRE) || (bidiType == BidiCharacterType.LRO))
                {
                    SetEmbeddingLevel(i, lastCharEmbeddingLevel);
                    if (embeddingLevel < 59)
                    {
                        elStack.Add(embeddingLevel);
                        dosStack.Push((int)dos);

                        embeddingLevel |= 1;
                        ++embeddingLevel;

                        dos = bidiType == BidiCharacterType.LRE
                            ? DirectionalOverrideStatus.Neutral
                            : DirectionalOverrideStatus.LTR;
                    }
                }
#endregion

#region rule X6
                // X6. For all types besides RLE, LRE, RLO, LRO, and PDF: (...)
                // Added From Revision #23 
                // For all types besides BN, RLE, LRE, RLO, LRO, and PDF: (...)
                if (bidiType != BidiCharacterType.B)
                {
                    if (!IsControlChar(GetChar(i)) && (bidiType != BidiCharacterType.BN))
                    {
                        // a. Set the level of the current character to the current embedding level.
                        SetEmbeddingLevel(i, embeddingLevel);
                        lastCharEmbeddingLevel = embeddingLevel;

                        //b. Whenever the directional override status is not neutral,
                        //reset the current character type to the directional override status.
                        if (dos == DirectionalOverrideStatus.LTR)
                            SetBidiType(i, BidiCharacterType.L);
                        else if (dos == DirectionalOverrideStatus.RTL)
                            SetBidiType(i, BidiCharacterType.R);
                    }
                    else
                    {
                        // The bidi control characters are no longer needed for text processing. However, we do not remove
                        // them (removing would require reallocation of all data structures) but mark all the control characters
                        // with the boundary neutral (BN) bidi type.
                        SetEmbeddingLevel(i, lastCharEmbeddingLevel);
                        SetBidiType(i, BidiCharacterType.BN);
                    }
                }

#endregion

#region rule X7
                //Terminating Embeddings and Overrides
                // X7. With each PDF, determine the matching embedding or override code.
                // If there was a valid matching code, restore (pop) the last remembered (pushed)
                // embedding level and directional override.
                if (bidiType == BidiCharacterType.PDF && (elStack.Count > 0))
                {
                    embeddingLevel = elStack[elStack.Count - 1];
                    elStack.RemoveAt(elStack.Count - 1);
                    dos = (DirectionalOverrideStatus)(dosStack.Pop());
                }
#endregion

                // X8. All explicit directional embeddings and overrides are completely
                // terminated at the end of each paragraph. Paragraph separators are not
                // included in the embedding.
                if (bidiType == BidiCharacterType.B)
                {
                    SetEmbeddingLevel(i, mParagraphEmbeddingLevel);
                    embeddingLevel = mParagraphEmbeddingLevel;
                    dos = DirectionalOverrideStatus.Neutral;
                    elStack.Clear();
                    dosStack.Clear();
                }
            }
#endregion

            // X10. The remaining rules are applied to each sequence of characters at the same level.
            List<Sequence> sequences = new List<Sequence>();
            int start = 0;
            while (start < mData.Length)
            {
                int level = GetEmbeddingLevel(start);

                int limit = start + 1;
                while ((limit < mData.Length) && (GetEmbeddingLevel(limit) == level))
                {
                    ++limit;
                }

                int prevLevel = mParagraphEmbeddingLevel;
                for (int i = start - 1; i >= 0; i--)
                {
                    if (GetBidiType(i) != BidiCharacterType.BN)
                    {
                        prevLevel = GetEmbeddingLevel(i);
                        break;
                    }
                }

                int nextLevel = mParagraphEmbeddingLevel;
                for (int i = limit; i < mData.Length; i++)
                {
                    if (GetBidiType(i) != BidiCharacterType.BN)
                    {
                        nextLevel = GetEmbeddingLevel(i);
                        break;
                    }
                }

                BidiCharacterType sor = TypeForLevel(Math.Max(prevLevel, level));
                BidiCharacterType eor = TypeForLevel(Math.Max(nextLevel, level));

                sequences.Add(new Sequence(start, limit, sor, eor, level));
                start = limit;
            }
            foreach (Sequence sequence in sequences)
            {
                ResolveWeakTypes(sequence);
                ResolveNeutralTypes(sequence);
            }

            ResolveImplicitTypes();

            ResolveSeparators();

            if (mConvertVisualToLogicalOrder)
            {
                ConvertVisualToLogicalOrder();
            }

            if (mConvertToMsWordOrder)
            {
                AdjustLevelOfNumericRuns();
            }

            // Set the flag to prevent repeated application of bidirectional algorithm.
            mAlgorithmIsApplied = true;
        }

        /// <summary>
        /// 3.3.3 Resolving Weak Types
        /// </summary>
        private void ResolveWeakTypes(Sequence sequence)
        {
            int i;

#region rule W1
            // W1. Examine each nonspacing mark (NSM) in the level run, and change the type of the NSM
            // to the type of the previous character. If the NSM is at the start of the level run, it will get the type of sor.
            if (mHasNsm)
            {
                BidiCharacterType preceedingCharacterType = sequence.StartOfSequence;
                for (i = sequence.Start; i < sequence.Limit; ++i)
                {
                    BidiCharacterType t = GetBidiType(i);
                    if (t == BidiCharacterType.NSM)
                    {
                        SetBidiType(i, preceedingCharacterType);
                    }
                    else if (t != BidiCharacterType.BN)
                    {
                        preceedingCharacterType = t;
                    }
                }
            }
#endregion

#region rule W2
            // W2. Search backward from each instance of a European number until the first strong type (R, L, AL, or sor)
            // is found. If an AL is found, change the type of the European number to Arabic number.

            BidiCharacterType t_w2 = BidiCharacterType.EN;
            for (i = sequence.Start; i < sequence.Limit; ++i)
            {
                BidiCharacterType bidiType = GetBidiType(i);
                if ((bidiType == BidiCharacterType.L) || (bidiType == BidiCharacterType.R))
                    t_w2 = BidiCharacterType.EN;
                else if (bidiType == BidiCharacterType.AL)
                    t_w2 = BidiCharacterType.AN;
                else if (bidiType == BidiCharacterType.EN)
                    SetBidiType(i, t_w2);
            }
#endregion

#region rule #3
            // W3. Change all ALs to R.
            if (mHasArabic)
            {
                for (i = sequence.Start; i < sequence.Limit; ++i)
                {
                    if (GetBidiType(i) == BidiCharacterType.AL)
                        SetBidiType(i, BidiCharacterType.R);
                }
            }
#endregion

#region rule W4
            // W4. A single European separator between two European numbers changes to a European number.
            // A single common separator between two numbers of the same type changes to that type.

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

            for (i = sequence.Start + 1; i < sequence.Limit - 1; ++i)
            {
                BidiCharacterType separatorBidiType = GetBidiType(i);

                // Search for separator characters in the sequence.
                if ((separatorBidiType != BidiCharacterType.ES) && (separatorBidiType != BidiCharacterType.CS))
                {
                    continue;
                }

                // Found a separator (ES or CS). Look backward for a number (EN or AN) skipping BN characters.
                int runStart = i - 1;
                while ((runStart >= sequence.Start) && (GetBidiType(runStart) == BidiCharacterType.BN))
                {
                    --runStart;
                }
                if (runStart < sequence.Start)
                {
                    // Reached the start of the sequence and no number found. This separator should be left as is.
                    continue;
                }
                BidiCharacterType numberBidiType = GetBidiType(runStart);
                if ((numberBidiType != BidiCharacterType.EN) &&
                    ((numberBidiType != BidiCharacterType.AN) || (separatorBidiType != BidiCharacterType.CS)))
                {
                    continue;
                }

                // Found a number. Look forward for a number of the same type skipping BN characters.
                int runEnd = i + 1;
                while ((runEnd < sequence.Limit) && (GetBidiType(runEnd) == BidiCharacterType.BN))
                {
                    ++runEnd;
                }
                if ((runEnd >= sequence.Limit) || (GetBidiType(runEnd) != numberBidiType))
                {
                    // Reached the end of the sequence. No number of the same type found.
                    continue;
                }

                // Do not consider '+', '-', '/' as number separators in Rtl paragraph if current or one of around characters
                // do not require this.
                bool isBlockedHebrewSeparator = false;
                if (mIsRtlParagraph && IsHebrewNumberSeparator(GetChar(i)))
                {
                    for (int j = runStart; j <= runEnd; j++)
                    {
                        if (!GetUseHebrewSeparators(j))
                        {
                            isBlockedHebrewSeparator = true;
                            break;
                        }
                    }
                }

                if (!isBlockedHebrewSeparator)
                {
                    // The first and the last characters are numbers and have already been checked to have the expected bidi
                    // type (EN or AN).
                    for (int j = runStart + 1; j <= runEnd - 1; j++)
                    {
                        // Change the type of the separator and adjacent BN characters to match the type of surrounding numbers.
                        // In other words, make the separator a part of the number.
                        SetBidiType(j, numberBidiType);
                    }
                }
            }
#endregion

#region rule W5
            // W5. A sequence of European terminators adjacent to European numbers changes to all European numbers.
            i = sequence.Start;
            while (i < sequence.Limit)
            {
                if ((GetBidiType(i) == BidiCharacterType.ET) || (GetBidiType(i) == BidiCharacterType.BN))
                {
                    bool sequenceContainsOnlyBn = (GetBidiType(i) == BidiCharacterType.BN);

                    // locate end of sequence
                    int runstart = i;
                    int runlimit = i;
                    while ((runlimit < sequence.Limit) &&
                        ((GetBidiType(runlimit) == BidiCharacterType.ET) || (GetBidiType(runlimit) == BidiCharacterType.BN)))
                    {
                        if (GetBidiType(runlimit) != BidiCharacterType.BN)
                        {
                            sequenceContainsOnlyBn = false;
                        }
                        ++runlimit;
                    }

                    if (!sequenceContainsOnlyBn)
                    {
                        // check values at ends of sequence
                        BidiCharacterType t = (runstart == sequence.Start)
                            ? sequence.StartOfSequence
                            : GetBidiType(runstart - 1);

                        if (t != BidiCharacterType.EN)
                        {
                            t = (runlimit == sequence.Limit)
                                ? sequence.EndOfSequence
                                : GetBidiType(runlimit);
                        }

                        if (t == BidiCharacterType.EN)
                        {
                            for (int j = runstart; j < runlimit; ++j)
                                SetBidiType(j, BidiCharacterType.EN);
                        }
                    }

                    // continue at end of sequence
                    i = runlimit;
                }

                ++i;
            }
#endregion

#region rule W6
            // W6. Otherwise, separators and terminators change to Other Neutral.
            for (i = sequence.Start; i < sequence.Limit; ++i)
            {
                BidiCharacterType t = GetBidiType(i);
                if (IsSeparatorOrTerminator(GetBidiType(i)))
                {
                    bool runContainsOnlyBn = (t == BidiCharacterType.BN);

                    int runStart = i;
                    int runLimit = i + 1;
                    while ((runLimit < sequence.Limit) && IsSeparatorOrTerminator(GetBidiType(runLimit)))
                    {
                        if (GetBidiType(runLimit) != BidiCharacterType.BN)
                        {
                            runContainsOnlyBn = false;
                        }
                        ++runLimit;
                    }

                    if (!runContainsOnlyBn)
                    {
                        for (int j = runStart; j < runLimit; j++)
                        {
                            SetBidiType(j, BidiCharacterType.ON);
                        }
                    }
                }
            }
#endregion

#region rule W7
            // W7. Search backward from each instance of a European number until the first strong type (R, L, or sor) is found.
            //     If an L is found, then change the type of the European number to L.

            BidiCharacterType t_w7 = (sequence.StartOfSequence == BidiCharacterType.L)
                ? BidiCharacterType.L
                : BidiCharacterType.EN;
            for (i = sequence.Start; i < sequence.Limit; ++i)
            {
                BidiCharacterType bidiType = GetBidiType(i);
                if (bidiType == BidiCharacterType.R)
                    t_w7 = BidiCharacterType.EN;
                else if (bidiType == BidiCharacterType.L)
                    t_w7 = BidiCharacterType.L;
                else if (bidiType == BidiCharacterType.EN)
                    SetBidiType(i, t_w7);
            }
#endregion
        }

        /// <summary>
        /// 3.3.4 Resolving Neutral Types
        /// </summary>
        private void ResolveNeutralTypes(Sequence sequence)
        {
            // N1. A sequence of neutrals takes the direction of the surrounding strong text if the text on both sides has the same direction. 
            //     European and Arabic numbers act as if they were R in terms of their influence on neutrals.
            //     Start-of-level-run (sor) and end-of-level-run (eor) are used at level run boundaries.
            // N2. Any remaining neutrals take the embedding direction.

            int i = sequence.Start;
            while (i < sequence.Limit)
            {
                if (IsNeutral(GetBidiType(i)))
                {
                    // find bounds of run of neutrals
                    int runstart = i;
                    int runlimit = i;
                    while (runlimit < sequence.Limit)
                    {
                        if (!IsNeutral(GetBidiType(runlimit)))
                        {
                            break;
                        }
                        ++runlimit;
                    }

                    // determine effective types at ends of run
                    BidiCharacterType leadingType;
                    BidiCharacterType trailingType;

                    if (runstart == sequence.Start)
                        leadingType = sequence.StartOfSequence;
                    else
                    {
                        leadingType = GetBidiType(runstart - 1);
                        if ((leadingType == BidiCharacterType.AN) || (leadingType == BidiCharacterType.EN))
                            leadingType = BidiCharacterType.R;
                    }

                    if (runlimit == sequence.Limit)
                        trailingType = sequence.EndOfSequence;
                    else
                    {
                        trailingType = GetBidiType(runlimit);
                        if ((trailingType == BidiCharacterType.AN) || (trailingType == BidiCharacterType.EN))
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
                        resolvedType = TypeForLevel(sequence.EmbeddingLevel);
                    }

                    for (int j = runstart; j < runlimit; ++j)
                    {
                        SetBidiType(j, resolvedType);
                    }

                    // skip over run of (former) neutrals
                    i = runlimit;
                }

                ++i;
            }
        }

        /// <summary>
        /// 3.3.5 Resolving Implicit Levels
        /// </summary>
        private void ResolveImplicitTypes()
        {
            // I1. For all characters with an even (left-to-right) embedding direction, those of type R go up one level
            //     and those of type AN or EN go up two levels.
            // I2. For all characters with an odd (right-to-left) embedding direction, those of type L, EN or AN go up one level.

            for (int i = 0; i < mData.Length; i++)
            {
                BidiCharacterType bidiType = GetBidiType(i);
                int embeddingLevel = GetEmbeddingLevel(i);
                if ((embeddingLevel % 2) == 0) // even level
                {
                    // Rule I1.
                    if (bidiType == BidiCharacterType.R)
                    {
                        SetEmbeddingLevel(i, embeddingLevel + 1);
                    }
                    else if ((bidiType == BidiCharacterType.AN) || (bidiType == BidiCharacterType.EN))
                    {
                        SetEmbeddingLevel(i, embeddingLevel + 2);
                    }
                }
                else
                {
                    // Rule I2.
                    if ((bidiType == BidiCharacterType.L) ||
                        (bidiType == BidiCharacterType.AN) ||
                        (bidiType == BidiCharacterType.EN))
                    {
                        SetEmbeddingLevel(i, embeddingLevel + 1);
                    }
                }
            }
        }

        /// <summary>
        /// 3.4 Reset the embedding level of segment and paragraph separator characters (rule L1).
        /// </summary>
        /// <remarks>
        /// See  http://unicode.org/reports/tr9/#L1
        /// </remarks>
        private void ResolveSeparators()
        {
            // Reset the embedding level of any sequence of whitespace characters or control characters
            // preceding a segment separator or paragraph separator to the paragraph embedding level.
            int lastNonWhitespaceLevel = mParagraphEmbeddingLevel;
            for (int i = 0; i < mData.Length; i++)
            {
                BidiCharacterType originalBidiType = UnicodeCharacterDataResolver.GetBidiCharacterType(GetChar(i));
                if ((originalBidiType == BidiCharacterType.B) ||
                    (originalBidiType == BidiCharacterType.S))
                {
                    // Reset the embedding level of the separator character.
                    SetEmbeddingLevel(i, mParagraphEmbeddingLevel);

                    // Reset the embedding levels of preceding whitespace and control characters.
                    for (int j = i - 1; j >= 0; j--)
                    {
                        BidiCharacterType t = UnicodeCharacterDataResolver.GetBidiCharacterType(GetChar(j));
                        if ((t == BidiCharacterType.WS) ||
                            (t == BidiCharacterType.BN) ||
                            IsControlChar(GetChar(j)))
                        {
                            SetEmbeddingLevel(j, mParagraphEmbeddingLevel);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Resolve bidi control characters and BN to the level of the preceding character if there is one,
                // and otherwise to the base level. This step is needed because our implementation does not remove
                // bidi control characters after explicit level resolution.
                if (IsControlChar(GetChar(i)) || (originalBidiType == BidiCharacterType.BN))
                {
                    SetEmbeddingLevel(i, lastNonWhitespaceLevel);
                }
                else
                {
                    lastNonWhitespaceLevel = GetEmbeddingLevel(i);
                }
            }

            // Reset the embedding level of any sequence of whitespace characters or control characters at the end of the line
            // to the paragraph embedding level.
            for (int j = mData.Length - 1; j >= 0; j--)
            {
                BidiCharacterType t = UnicodeCharacterDataResolver.GetBidiCharacterType(GetChar(j));
                if ((t == BidiCharacterType.WS) ||
                    (t == BidiCharacterType.BN) ||
                    IsControlChar(GetChar(j)))
                {
                    SetEmbeddingLevel(j, mParagraphEmbeddingLevel);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Converts visually ordered text into logical order.
        /// </summary>
        /// <remarks>
        /// To convert the text we split it into LTR and RTL fragments and then reverse the order of the fragments
        /// and reverse text of RTL fragmens. We use the Unicode bidirectional algorithm (UBA) to split the text.
        /// </remarks>
        private void ConvertVisualToLogicalOrder()
        {
            // A temporary array used to store reversed text.
            long[] convertedData = new long[mData.Length];

            // The embedding levels of the text has already been calculated. We process runs of same embedding level.
            int runStartIndex = 0;
            int runLength = 0;
            int runEmbeddingLevel = -1;
            // An additional imaginary end-of-array character is used as a boundary of the last run.
            for (int i = 0; i < mData.Length + 1; i++)
            {
                int currentEmbeddingLevel = (i < mData.Length)
                    ? GetEmbeddingLevel(i)
                    : -1; // Embedding level of the last end-of-array imaginary character.

                if (currentEmbeddingLevel != runEmbeddingLevel)
                {
                    if (runLength > 0)
                    {
                        // If the current run has RTL direction, reverse its text. We will throw the original data array away,
                        // so we can reverse and mirror characters in place.
                        bool isRtlRun = ((runEmbeddingLevel % 2) == 1);
                        if (isRtlRun)
                        {
                            Array.Reverse(mData, runStartIndex, runLength);

                            // Reversing text also requires some of characters to be replaced with their mirrored counterparts
                            // (for example, brackets).
                            for (int j = runStartIndex; j < (runStartIndex + runLength); j++)
                            {
                                char originalChar = GetChar(j);
                                char mirroredChar = BidiCharacterMirrorResolver.GetBidiCharacterMirror(originalChar);
                                if (mirroredChar != originalChar)
                                {
                                    SetChar(j, mirroredChar);
                                }
                            }
                        }

                        // To reverse the order of all runs we write them from the end to the beginning of the temporary arrays.
                        int destinationIndex = mData.Length - i;
                        Array.Copy(mData, runStartIndex, convertedData, destinationIndex, runLength);
                    }
                    runEmbeddingLevel = currentEmbeddingLevel;
                    runStartIndex = i;
                    runLength = 0;
                }
                ++runLength;
            }

            // Replace the text with its reordered variant.
            mData = convertedData;
        }

        /// <summary>
        /// Adjust the embedding level of runs containing numbers as required by MS Word.
        /// </summary>
        /// <remarks>
        /// The Unicode bidirectional algorithm increases the embedding level of numeric runs in RTL context to make sure
        /// the numbers are rendered in left-to-right direction. However, this offset is not needed when we write RTL text
        /// with numbers to MS Word, because Word correctly processes numbers in RTL context. In this method, we put numeric
        /// runs into one of adjacent runs by decreasing the number's embedding level. Of two adjacent runs we choose the one
        /// with higher embedding level (however, the run's embedding level must not be higher than that
        /// of the numeric run itself).
        /// </remarks>
        private void AdjustLevelOfNumericRuns()
        {
            // Split the text into runs of same embedding level and examine each run to find numbers.
            int rangeStart = 0;
            int rangeLength = 0;
            int previousRunEmbeddingLevel = -1;
            int runEmbeddingLevel = -1;
            bool runContainsDigits = false;
            bool runContainsLetters = false;
            bool runContainsHebrewNumberSeparators = false;
            // Here we scan one character past the text. An imaginary end-of-array character bounds the last run and allows
            // to process it inside the main loop thus reducing the code duplication.
            for (int i = 0; i < mData.Length + 1; i++)
            {
                char currentChar;
                int currentEmbeddingLevel;
                if (i < mData.Length)
                {
                    currentChar = GetChar(i);
                    currentEmbeddingLevel = GetEmbeddingLevel(i);
                }
                else
                {
                    // Properties of the imaginary end-of-array character.
                    currentChar = BidiChars.NotAChar;
                    currentEmbeddingLevel = -1;
                }

                // Control characters are ignored.
                if (IsControlChar(currentChar))
                {
                    continue;
                }

                // The end of current run is reached. Let's process the run.
                if (currentEmbeddingLevel != runEmbeddingLevel)
                {
                    // If the current run contains a number, we need to correct its embedding level.
                    if (runContainsDigits && (!runContainsLetters))
                    {
                        // Calculate adjacent run levels.
                        int leadingLevel = Math.Min(runEmbeddingLevel, previousRunEmbeddingLevel);
                        int trailingLevel = Math.Min(runEmbeddingLevel, currentEmbeddingLevel);
                        int maxAdjacentLevel = Math.Max(leadingLevel, trailingLevel);

                        // Calculate the new level of the numeric run.
                        // If there are no adjacent runs, keep the number's embedding level unchanged.
                        int adjustedEmbeddingLevel = (maxAdjacentLevel >= 0)
                            ? Math.Min(maxAdjacentLevel, runEmbeddingLevel)
                            : runEmbeddingLevel;

                        // Correct the embedding level of the current run if it has changed.
                        if (adjustedEmbeddingLevel != runEmbeddingLevel)
                        {
                            for (int j = 0; j < rangeLength; j++)
                            {
                                SetEmbeddingLevel(rangeStart + j, adjustedEmbeddingLevel);
                            }
                        }

                        // If needed, mark run as containing Hebrew number separators.
                        bool isRtlRun = (adjustedEmbeddingLevel % 2) == 1;
                        if (isRtlRun && runContainsHebrewNumberSeparators)
                        {
                            for (int j = 0; j < rangeLength; j++)
                            {
                                SetRequiresHebrewLocale(rangeStart + j, true);
                            }
                        }
                    }

                    // Start a new run.
                    previousRunEmbeddingLevel = runEmbeddingLevel;
                    runEmbeddingLevel = currentEmbeddingLevel;
                    rangeStart = i;
                    rangeLength = 0;
                    runContainsDigits = false;
                    runContainsLetters = false;
                    runContainsHebrewNumberSeparators = false;
                }

                // Append the current character to the current run.
                ++rangeLength;

                // Numeric runs always have left-to-right direction, so we can skip checks for right-to-left runs.
                bool isLtrRun = ((runEmbeddingLevel % 2) == 0);
                if (isLtrRun)
                {
                    // The BiDi type of the character might have been changed by the Bidirectional Algorithm,
                    // so we get the original type of the character.
                    BidiCharacterType originalBidiType = UnicodeCharacterDataResolver.GetBidiCharacterType(currentChar);
                    if ((originalBidiType == BidiCharacterType.EN) || (originalBidiType == BidiCharacterType.AN))
                    {
                        runContainsDigits = true;
                    }
                    // When common number separators are inside a run with a number, they are treated as a part of that number,
                    // not as separate letters.
                    //
                    // The following characters are number separators from the Unicode point of view. MS Word, however, treat
                    // them as such only if run's bidi locale is set to 'he-IL' (see ISO 29500, Annex I, Section I.2, 
                    // 'lang' element description).
                    else if (IsHebrewNumberSeparator(currentChar))
                    {
                        runContainsHebrewNumberSeparators = true;
                    }
                    // Althought Unicode defines more number separator characters (CS and ES characters types),
                    // some of them are always treated as neutral direction characters (ON) in MS Word.
                    else if ((currentChar != ',') && (currentChar != '.') && (currentChar != ':') && (currentChar != '\u060C'))
                    {
                        // Here letters are all characters except digits and number separators (all characters that cannot be
                        // a part of a number).
                        runContainsLetters = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets runs reordered by the Unicode bidirectional algorithm.
        /// </summary>
        private BidiRun[] GetResult()
        {
            // Before getting the result, the text must be processed by the bidirectional algorithm.
            Debug.Assert(mAlgorithmIsApplied);
            if (!mAlgorithmIsApplied)
            {
                return new BidiRun[0];
            }

            List<BidiRun> resultRuns = new List<BidiRun>();

            // Create a mask that filters all character properties relevant to run splitting. By using this mask we can get
            // all the properties at once and store them together in order to simplify their comparison and copying.
            long propertiesMask = gFieldMasks[EmbeddingLevelFieldIndex] |
                gFieldMasks[SourceRunIndexFieldIndex] |
                gFieldMasks[RequiresHebrewLocaleFieldIndex];

            long rangeCharProperties = 0;
            int rangeStartIndex = 0;
            StringBuilder rangeText = new StringBuilder();
            // Here we scan one character past the end of text. The extra end-of-text character closes the last run and allows
            // to process it inside the loop thus reducing the code duplication.
            for (int i = 0; i <= mData.Length; i++)
            {
                char currentChar = (i < mData.Length)
                    ? GetChar(i)
                    : BidiChars.NotAChar; // An arbitrary non-control character. It won't be written to the result anyway.

                // Skip normal control characters.
                // The end-of-text character is guaranteed to be non-control and it is never skipped.
                if (IsControlChar(currentChar))
                {
                    continue;
                }

                // Get all relevant character properties.
                // Make sure properties of the end-of-text character differ from properties of the preceding character
                // in order to close the last run.
                long currentCharProperties = (i >= mData.Length)
                    ? ~rangeCharProperties
                    : mData[i] & propertiesMask;

                if (currentCharProperties != rangeCharProperties)
                {
                    if (rangeText.Length > 0)
                    {
                        BidiRun newRun = new BidiRun(rangeText.ToString(), GetEmbeddingLevel(rangeStartIndex), 
                            GetSourceRunIndex(rangeStartIndex));

                        newRun.BidiCharacterType = GetBidiType(rangeStartIndex);

                        if (GetRequiresHebrewLocale(rangeStartIndex))
                        {
                            newRun.IsNumber = true;
                            newRun.HasHebrewNumberSeparators = true;
                        }
                        resultRuns.Add(newRun);
                    }
                    rangeCharProperties = currentCharProperties;
                    rangeStartIndex = i;
                    rangeText.Length = 0;
                }
                rangeText.Append(currentChar);
            }

            BidiRun[] result = resultRuns.ToArray();
            if (mConvertToMsWordOrder)
            {
                FlatternEmbeddingLevels(result, mParagraphEmbeddingLevel);
            }
            return result;
        }

        /// <summary>
        /// Rearranges runs in order to flattern embedding levels as required by Microsoft Word.
        /// </summary>
        /// <remarks>
        /// In MS Word, the text should be a sequence of left-to-right and right-to-left runs, without bidirectional embeddings
        /// (one run cannot be nested inside another). To flattern bidirectional embeddings, we use the reordering part
        /// of the Unicode bidirectional algorithm, described here: http://www.unicode.org/reports/tr9/#L2
        /// </remarks>
        private static void FlatternEmbeddingLevels(BidiRun[] runs, int paragraphEmbeddingLevel)
        {
            Debug.Assert(paragraphEmbeddingLevel >= 0);

            int minEmbeddingLevel = int.MaxValue;
            int maxEmbeddingLevel = int.MinValue;
            foreach (BidiRun run in runs)
            {
                minEmbeddingLevel = Math.Min(minEmbeddingLevel, run.Level);
                maxEmbeddingLevel = Math.Max(maxEmbeddingLevel, run.Level);
            }

            Debug.Assert(minEmbeddingLevel >= 0);
            Debug.Assert(maxEmbeddingLevel >= minEmbeddingLevel);

            // Flattern nested embeddings one level at a time.
            while (maxEmbeddingLevel > minEmbeddingLevel)
            {
                int rangeStart = 0;
                int rangeLength = 0;
                // Find ranges of runs whose order should be reversed at this step.
                // An additional imaginary end-of-array run bounds the last range. This allows to process the last range
                // inside the loop and avoid code duplication.
                for (int i = 0; i <= runs.Length; i++)
                {
                    // At this step we reorder runs whose level is higher or equal the maximum allowed level.
                    bool reorderCurrentRun = (i < runs.Length) && (runs[i].Level >= maxEmbeddingLevel);

                    if (reorderCurrentRun)
                    {
                        // This run in inside a range that should be reordered.
                        if (rangeLength == 0)
                        {
                            rangeStart = i;
                        }
                        ++rangeLength;
                    }
                    else
                    {
                        // This run should not be reordered. If there is a preceding range of reorderable runs,
                        // we need to reorder them now.
                        ReverseRuns(runs, rangeStart, rangeLength);
                        rangeStart = i;
                        rangeLength = 0;
                    }
                }

                // One more level has been flatterned.
                --maxEmbeddingLevel;
            }

            // Reverse the order of the runs if implicit text direction does not match the paragraph direction.
            int textDirection = minEmbeddingLevel % 2;
            int paragraphDirection = paragraphEmbeddingLevel % 2;
            if (textDirection != paragraphDirection)
            {
                ReverseRuns(runs, 0, runs.Length);
            }
        }

        /// <summary>
        /// Reverses the specified subrange of an array of runs.
        /// </summary>
        /// <remarks>
        /// Run direction is taken into account during reordering: order of subsequent runs having same direction is not changed.
        /// For example, the array L0-R1-L2-R3 will be reordered as follows: R3-L2-R1-L0 (the letter stands for run direction
        /// and the number stands for original run index).
        /// However, the array L0-R1-R2-L3 will become L3-R1-R2-L0, because the second (R1) and the third (R2) runs have the
        /// same direction and will be processed as one.
        /// </remarks>
        private static void ReverseRuns(BidiRun[] runs, int start, int length)
        {
            Debug.Assert(start >= 0);
            Debug.Assert(start < runs.Length);
            
            if (length <= 1)
            {
                // Nothing to reverse.
                return;
            }

            // Reorder runs into a temporary array. We cannot just call Array.Reverse(),
            // because we have process adjacent runs having same direction in a special way.
            BidiRun[] reorderedRuns = new BidiRun[length];
            int rangeStart = start;
            int rangeLength = 0;
            int rangeDirection = -1; // Meaning of values: 0 - LTR; 1 - RTL; -1 (and any other) - none (not initialized).
            // Here we process an imaginary out-of-array element that bounds the last range.
            // This allows to process the last range inside the cycle and prevent code duplication.
            for (int i = 0; i <= length; i++)
            {
                bool sameDirection = (i < length) && ((runs[start + i].Level % 2) == rangeDirection);
                if (sameDirection)
                {
                    ++rangeLength;
                }
                else
                {
                    int destinationIndex = reorderedRuns.Length - (rangeStart - start) - rangeLength;
                    if (rangeLength == 1)
                    {
                        reorderedRuns[destinationIndex] = runs[rangeStart];
                    }
                    else if (rangeLength > 1)
                    {
                        Array.Copy(runs, rangeStart, reorderedRuns, destinationIndex, rangeLength);
                    }
                    rangeStart = start + i;
                    rangeLength = 1;
                    rangeDirection = (rangeStart < runs.Length)
                        ? runs[rangeStart].Level % 2
                        : -1;
                }
            }

            // Copy the reordered runs back to the main array.
            Array.Copy(reorderedRuns, 0, runs, start, length);
        }

        private static bool IsSeparatorOrTerminator(BidiCharacterType bidiType)
        {
            return (bidiType == BidiCharacterType.ES) ||
                (bidiType == BidiCharacterType.ET) ||
                (bidiType == BidiCharacterType.CS) ||
                (bidiType == BidiCharacterType.BN);
        }

        private static bool IsNeutral(BidiCharacterType bidiType)
        {
            return (bidiType == BidiCharacterType.WS) ||
                (bidiType == BidiCharacterType.ON) ||
                (bidiType == BidiCharacterType.B) ||
                (bidiType == BidiCharacterType.S) ||
                (bidiType == BidiCharacterType.BN);
        }

        public static bool IsControlChar(char c)
        {
            return (c == BidiChars.LRE) ||
                (c == BidiChars.RLE) ||
                (c == BidiChars.LRO) ||
                (c == BidiChars.RLO) ||
                (c == BidiChars.PDF);
        }

        private static bool IsHebrewNumberSeparator(char c)
        {
            return (c == '+') || (c == '-') || (c == '/');
        }

        /// <summary>
        /// Return the strong type (L or R) corresponding to the embedding level.
        /// </summary>
        /// <param name="level">The embedding level to check.</param>
        /// <returns></returns>
        private static BidiCharacterType TypeForLevel(int level)
        {
            return ((level % 2) == 0)
                ? BidiCharacterType.L
                : BidiCharacterType.R;
        }

        /// <summary>
        /// Represents a sequence of characters having same embedding level.
        /// </summary>
        private class Sequence
        {
            internal Sequence(int start, int limit, BidiCharacterType startOfSequence, BidiCharacterType endOfSequence,
                int embeddingLevel)
            {
                Debug.Assert(start >= 0);
                Debug.Assert(limit > start);
                Debug.Assert((startOfSequence == BidiCharacterType.L) || (startOfSequence == BidiCharacterType.R));
                Debug.Assert((endOfSequence == BidiCharacterType.L) || (endOfSequence == BidiCharacterType.R));
                Debug.Assert(embeddingLevel >= 0);

                mStart = start;
                mLimit = limit;
                mStartOfSequence = startOfSequence;
                mEndOfSequence = endOfSequence;
                mEmbeddingLevel = embeddingLevel;
            }

            internal int Start
            {
                get { return mStart; }
            }

            internal int Limit
            {
                get { return mLimit; }
            }

            internal BidiCharacterType StartOfSequence
            {
                get { return mStartOfSequence; }
            }

            internal BidiCharacterType EndOfSequence
            {
                get { return mEndOfSequence; }
            }

            internal int EmbeddingLevel
            {
                get { return mEmbeddingLevel; }
            }

            private readonly int mStart;
            private readonly int mLimit;
            private readonly BidiCharacterType mStartOfSequence;
            private readonly BidiCharacterType mEndOfSequence;
            private readonly int mEmbeddingLevel;
        }

        static BidiAlgorithm()
        {
            const int fieldCount = 6;

            int[,] fieldOffsetsAndLengths = new int[,]
            {
                {0, 16},
                {16, 1}, // 1-bit flag 'is a character of an RTL number with Hebrew separators'.
                {17, 7}, // Bidi character types. Our implementation supports 19 types and the latest spec declares 23.
                {24, 8}, // Embedding levels. We support up to 64 levels and the latest spec raises this limit to 128.
                {32, 1}, // UseHebrewSeparator
                {33, 31}
            };

            gFieldMasks = new long[fieldCount];
            gFieldShifts = new int[fieldCount];

            int freeBlockOffset = 0;
            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
            {
                int fieldOffset = fieldOffsetsAndLengths[fieldIndex, 0];
                int fieldLength = fieldOffsetsAndLengths[fieldIndex, 1];

                Debug.Assert(fieldOffset >= 0);
                Debug.Assert(fieldLength > 0);
                Debug.Assert(fieldOffset >= freeBlockOffset);

                int shift = 64/*sizeof(long) * 8*/ - (fieldOffset + fieldLength);
                Debug.Assert(shift >= 0);
                long mask = (((long)1 << fieldLength) - 1) << shift;

                gFieldMasks[fieldIndex] = mask;
                gFieldShifts[fieldIndex] = shift;

                freeBlockOffset = fieldOffset + fieldLength;
            }
        }

        // The following fields contain information used by getter and setter methods to access individual fields
        // of character-related data.
        /// JAVA-changed from unsigned long to (signed) long for autoportability to Java.
        private static readonly long[] gFieldMasks;
        private static readonly int[] gFieldShifts;
        private const int CharFieldIndex = 0;
        private const int RequiresHebrewLocaleFieldIndex = 1;
        private const int BidiTypeFieldIndex = 2;
        private const int EmbeddingLevelFieldIndex = 3;
        private const int UseHebrewSeparatorsFieldIndex = 4;
        private const int SourceRunIndexFieldIndex = 5;
        

        /// <summary>
        /// Characters and related information used by the algorithm. Use the corresponding getter and setter methods to access
        /// individual data fields.
        /// </summary>
        /// <remarks>
        /// Items of this array are in fact packed structures containing characters and related information such as their levels
        /// and bidi types. See the static constructor of this class for information on internal representation of data.
        /// It would be better to use an array of structures instead, but structures cannot be ported to Java,
        /// and classes are too heavy to create one per character.
        /// </remarks>
        /// JAVA-changed from unsigned long to (signed) long for autoportability to Java.
        private long[] mData;
        
        private readonly int mParagraphEmbeddingLevel;
        private readonly bool mConvertVisualToLogicalOrder;
        private readonly bool mConvertToMsWordOrder;

        private readonly bool mIsRtlParagraph;

        /// <summary>
        /// Indicates whether the Unicode bidirectional algorithm has been applied to the text.
        /// </summary>
        private bool mAlgorithmIsApplied;

        private bool mHasNsm;
        private bool mHasArabic;
    }
}
