// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2021 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Evaluates sequence values.
    /// </summary>
    /// <dev>
    /// Here is how value evaluation works.
    ///
    /// SEQ fields which modify sequence values (i.e. non-cross-reference) are extracted and stored in the
    /// extraction order in the sequence items list. Sequence values for each of sequence items are also stored.
    ///
    /// Bookmarks are considered to be sequence markers.
    /// A sequence marker points to the last sequence item met to the time of its detecting. When the value for
    /// the marker is requested, the evaluator looks for it starting from this sequence item and moving to the first
    /// sequence item until a sequence item for the requested sequence (defined by the sequence name) is found.
    ///
    /// These values are evaluated rather than stored because of the possibility of using different sequences
    /// in conjunction with the same markers.
    ///
    /// Note, that bookmarks are paired markers i.e. they have two values: corresponding to the starts and
    /// corresponding to the ends.
    /// </dev>
    internal class FieldSeqValueEvaluator : FieldExtractor
    {
        internal FieldSeqValueEvaluator(Document document, BookmarkCache bookmarkCache)
            : base(FieldType.FieldSequence)
        {
            mBookmarkCache = bookmarkCache;

            // Initializing of a value evaluator consists of two field extracting stages which can not be joined. They are:
            // 1. Collecting of cross-reference bookmarks for SEQ
            // 2. Evaluating of sequence values considering collected bookmark names.

            // Step 1.
            mBookmarkSequenceEnds = BookmarkNamesFinder.FindBookmarkNames(document, bookmarkCache);

            // Step 2.
            EvaluateSequenceItemValues(document);
        }

        private void EvaluateSequenceItemValues(Node document)
        {
            mSequenceStates = new StringToObjDictionary<SequenceState>(false);
            Extract(document);
            mSequenceStates = null;
        }

        protected override void OnFieldExtracted()
        {
            FieldSeq field = (FieldSeq)CurrentField;

            if (field.HasBookmarkName || field.IsError(mBookmarkCache))
                return;

            // 1. Retrieve current sequence state. Initialize if not found.
            SequenceState sequenceState = mSequenceStates[field.SequenceIdentifier];
            if (sequenceState == null)
            {
                sequenceState = new SequenceState();
                mSequenceStates[field.SequenceIdentifier] = sequenceState;
            }

            // 2. Change the state according to the field's switches.
            sequenceState.ProcessFieldEntry(field);

            // 3. Save current sequence value for the field.
            mSequenceItemValues[field.Start] = sequenceState.CurrentValue;

            // 4. Remember the field as a sequence item if needed.
            if (mSequenceItems != null)
                mSequenceItems.Add(field);
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            // Remember bookmark end if needed.
            if (mBookmarkSequenceEnds.ContainsKey(bookmarkEnd.Name))
                mBookmarkSequenceEnds[bookmarkEnd.Name] = LastSequenceItemIndex;

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            // Notify all of the initialized sequences about the paragraph entry met.
            foreach (SequenceState sequenceState in mSequenceStates.Values)
                sequenceState.ProcessParagraphEntry(paragraph);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Gets a value for the given SEQ field.
        /// </summary>
        internal Int32Constant GetValue(FieldSeq field)
        {
            if (field.IsError(mBookmarkCache))
                return null;

            return field.IsInHeaderFooter
                ? GetHeaderFooterFieldValue(field)
                : GetBodyFieldValue(field);
        }

        private Int32Constant GetBodyFieldValue(FieldSeq field)
        {
            int value;
            if (field.HasBookmarkName)
            {
                NullableInt32 nullableValue = GetBookmarkSequenceValue(field.BookmarkName, field.SequenceIdentifier);
                if (!nullableValue.HasValue)
                    return null;

                value = nullableValue.Value;
            }
            else
            {
                value = GetSequenceValue(field.Start);
            }

            return GetInt32Constant(value);
        }

        internal NullableInt32 GetBookmarkSequenceValue(string bookmarkName, string sequenceIdentifier)
        {
            int endIndex = mBookmarkSequenceEnds[bookmarkName];
            if (StringToIntDictionary.IsNullSubstitute(endIndex))
                return NullableInt32.Null;

            // SEQ fields use a sequence value corresponding to a bookmark end.
            int value = GetInt32Value(endIndex, sequenceIdentifier);
            return new NullableInt32(value);
        }

        private Int32Constant GetHeaderFooterFieldValue(FieldSeq field)
        {
            // FOSS: resolving a SEQ field located in a header/footer requires traversing the laid-out pages,
            // which the layout engine provided. Without it, fall back to 0 - the same value the original code
            // returned when no matching SEQ field was found on the page.
            return new Int32Constant(0);
        }

        /// <summary>
        /// Evaluates a sequence value starting from a sequence item with the specified index as per
        /// <see cref="FieldSeqValueEvaluator"/> remarks.
        /// </summary>
        private int GetInt32Value(int startIndex, string sequenceName)
        {
            for (int index = startIndex; index >= 0; index--)
            {
                FieldSeq field = mSequenceItems[index];
                if (StringUtil.EqualsIgnoreCase(field.SequenceIdentifier, sequenceName))
                {
                    int value = GetSequenceValue(field.Start);
                    Debug.Assert(value != InvalidInt32Value);

                    return value;
                }
            }

            // If we are here then a reference to a sequence is met before initialization of this sequence.
            // Return zero in this case as MS Word does.
            return 0;
        }

        /// <summary>
        /// Gets a sequence value of the specified field. Returns <see cref="InvalidInt32Value"/> if it is not found.
        /// </summary>
        private int GetSequenceValue(FieldStart fieldStart)
        {
            return mSequenceItemValues.GetValueOrDefault(fieldStart, InvalidInt32Value);
        }

        /// <summary>
        /// A special factory method for <see cref="Int32Constant"/>.
        /// </summary>
        private static Int32Constant GetInt32Constant(int value)
        {
            Debug.Assert(value != InvalidInt32Value);

            return new Int32Constant(value);
        }

        /// <summary>
        /// Gets an index of the last sequence item.
        /// </summary>
        private int LastSequenceItemIndex
        {
            get { return mSequenceItems.Count - 1; }
        }

        internal void AddSequenceBookmark(string bookmarkName)
        {
            int endIndex = mBookmarkSequenceEnds[bookmarkName];
            if (!StringToIntDictionary.IsNullSubstitute(endIndex))
                return;

            Bookmark bookmark = mBookmarkCache[bookmarkName];

            endIndex = GetClosestSequenceItemIndex(bookmark.BookmarkEnd, LastSequenceItemIndex);

            mBookmarkSequenceEnds[bookmarkName] = endIndex;
        }

        private int GetClosestSequenceItemIndex(Node node, int from)
        {
            for (int i = System.Math.Min(from, LastSequenceItemIndex); i >= 0; i--)
                if (Node.NodeAIsAboveNodeB(mSequenceItems[i].End, node))
                    return i;

            return -1;
        }

        private readonly BookmarkCache mBookmarkCache;

        private readonly List<FieldSeq> mSequenceItems = new List<FieldSeq>();
        private readonly StringToIntDictionary mBookmarkSequenceEnds;
        private readonly Dictionary<FieldStart, int> mSequenceItemValues = new Dictionary<FieldStart, int>();
        private StringToObjDictionary<SequenceState> mSequenceStates;

        private const int InvalidInt32Value = -1;

        /// <summary>
        /// Specifies paragraphs of which styles are contained between SEQ field entries.
        /// </summary>
        [Flags]
        private enum ParagraphStyles
        {
            None = 0,
            Heading1 = 1,
            Heading2 = 2,
            Heading3 = 4,
            Heading4 = 8,
            Heading5 = 16,
            Heading6 = 32,
            Heading7 = 64,
            Heading8 = 128,
            Heading9 = 256
        }

        /// <summary>
        /// Represents the state of a single named sequence.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppDeclareFriendClass("Aspose.Words.Paragraph")] // To make Paragraph.ParaPr property accessible in C++.
        private class SequenceState
        {
            /// <summary>
            /// Processes a single SEQ field entry.
            /// </summary>
            internal void ProcessFieldEntry(FieldSeq field)
            {
                CurrentValue = GetNextValue(field);
                mMetParagraphStyles = ParagraphStyles.None;
            }

            /// <summary>
            /// Gets the next sequence value to be applied.
            /// </summary>
            private int GetNextValue(FieldSeq field)
            {
                // Reset the sequence with the specified number, if switch presents.
                if (field.HasResetNumberSwitch)
                {
                    int resetNumber = field.ResetNumberAsInt32.GetValueOrDefault();
                    return System.Math.Abs(resetNumber);
                }

                // Reset the sequence depending on the specified paragraph heading level, if present.
                // MS Word looks upward from the paragraph where the field is located (inclusively) to the paragraph
                // where the previous field for the same sequence is located (exclusively). If a heading paragraph of
                // a level which is less or equal to the specified one is met, then the sequence is reset.
                if ((mMetParagraphStyles != ParagraphStyles.None) && field.HasResetHeadingLevelSwitch)
                {
                    int resetHeadingLevel = field.ResetHeadingLevelAsInt32.GetValueOrDefault();
                    ParagraphStyles resetParagraphStyle = GetParagraphStyle(resetHeadingLevel);
                    if (resetParagraphStyle != ParagraphStyles.None)
                    {
                        // The following expression evaluates combination of all paragraph styles,
                        // which are less or equal to the specified one.
                        ParagraphStyles resetParagraphStyles = (resetParagraphStyle - 1) | resetParagraphStyle;
                        if ((mMetParagraphStyles & resetParagraphStyles) != 0)
                            return 1;
                    }
                }

                return field.InsertClosestPrecedingNumber ? CurrentValue : CurrentValue + 1;
            }

            /// <summary>
            /// Processes a single paragraph entry.
            /// </summary>
            internal void ProcessParagraphEntry(Paragraph paragraph)
            {
                mMetParagraphStyles |= GetParagraphStyle(paragraph.ParaPr.Istd);
            }

            /// <summary>
            /// Gets the current sequence value.
            /// </summary>
            internal int CurrentValue { get; private set; }

            /// <summary>
            /// Returns <see cref="ParagraphStyles"/> value corresponding to the specified style index.
            /// </summary>
            private static ParagraphStyles GetParagraphStyle(int istd)
            {
                switch (istd)
                {
                    case StyleIndex.Heading1:
                        return ParagraphStyles.Heading1;
                    case StyleIndex.Heading2:
                        return ParagraphStyles.Heading2;
                    case StyleIndex.Heading3:
                        return ParagraphStyles.Heading3;
                    case StyleIndex.Heading4:
                        return ParagraphStyles.Heading4;
                    case StyleIndex.Heading5:
                        return ParagraphStyles.Heading5;
                    case StyleIndex.Heading6:
                        return ParagraphStyles.Heading6;
                    case StyleIndex.Heading7:
                        return ParagraphStyles.Heading7;
                    case StyleIndex.Heading8:
                        return ParagraphStyles.Heading8;
                    case StyleIndex.Heading9:
                        return ParagraphStyles.Heading9;
                    default:
                        return ParagraphStyles.None;
                }
            }

            private ParagraphStyles mMetParagraphStyles;
        }

        /// <summary>
        /// Searches for SEQ cross-reference bookmarks.
        /// </summary>
        private class BookmarkNamesFinder : FieldExtractor
        {
            private BookmarkNamesFinder(BookmarkCache bookmarkCache)
                : base(FieldType.FieldSequence)
            {
                mBookmarkNames = new StringToIntDictionary(false);
                mBookmarkCache = bookmarkCache;
            }

            internal static StringToIntDictionary FindBookmarkNames(Document document, BookmarkCache bookmarkCache)
            {
                BookmarkNamesFinder finder = new BookmarkNamesFinder(bookmarkCache);
                finder.Extract(document);
                return finder.mBookmarkNames;
            }

            protected override void OnFieldExtracted()
            {
                FieldSeq field = (FieldSeq)CurrentField;

                if (!field.HasBookmarkName)
                    return;

                if (field.IsError(mBookmarkCache))
                    return;

                mBookmarkNames[field.BookmarkName] = 0;
            }

            private readonly StringToIntDictionary mBookmarkNames;
            private readonly BookmarkCache mBookmarkCache;
        }
    }
}
