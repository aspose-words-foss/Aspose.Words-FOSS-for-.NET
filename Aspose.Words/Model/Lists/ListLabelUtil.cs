// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/12/2012 by Ivan Lyagin

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Aspose.Words.Lists
{
    internal interface IListLabelBuildBehaviour
    {
        /// <summary>
        /// This method is invoked when list number is appended.
        /// </summary>
        void NotifyListNumberAppended(int listLabelLength);

        /// <summary>
        /// Gets a value indicating whether not list number char should be appended to list label or not.
        /// </summary>
        bool ShouldAppendNotListNumberChar(char c);

        /// <summary>
        /// Truncates list label length.
        /// </summary>
        int FinalizeListLabelLength(int listLabelLength);

        /// <summary>
        /// Normalizes number style if necessary.
        /// </summary>
        NumberStyle NormalizeNumberStyle(NumberStyle numberStyle);
    }

    /// <summary>
    /// <para>
    /// For specified paragraph, updates <see cref="Paragraph.ListLabel"/> with textual and numeric representations
    /// of list label.
    /// </para>
    /// <para>
    /// Also forms representation of list label of a paragraph to use in document fields as this representation
    /// can be different from list label inside the paragraph itself.
    /// </para>
    /// </summary>
    /// <dev>
    /// This class is a product of a <see cref="ListNumberGenerator"/> class refactoring, i.e. moving of its members
    /// which are not relative to a number generation process. All of the source code was left untouched while moving.
    /// </dev>
    internal static class ListLabelUtil
    {
        internal static bool UpdateParagraphListLabel(Paragraph para, ListNumberGenerator generator, RevisionsView view)
        {
            Debug.Assert(para != null);
            Debug.Assert(generator != null);

            ListLabelAction listLabelAction = GetListLabelAction(para, view);
            Debug.Assert(listLabelAction != ListLabelAction.Undetermined);

            int originalListId = (int)para.FetchParaAttr(ParaAttr.ListId, RevisionsView.Original);
            int finalListId = (int)para.FetchParaAttr(ParaAttr.ListId, RevisionsView.Final);

            int listId = (view == RevisionsView.Original) ? originalListId : finalListId;

            // WORDSNET-26889 Ignore numbering revision inherited from style format revision.
            if ((originalListId != finalListId) && !para.ParaPr.HasFormatRevision)
                listId = finalListId;

            List list = para.Document.Lists.GetListByListId(listId);
            // WORDSNET-13576 Do not number paragraphs referred to not existent list.
            if (list == null)
                listLabelAction = ListLabelAction.Clear;

            bool needUpdate = listLabelAction != ListLabelAction.Clear;
            if (needUpdate)
            {
                int listLevel = (int)para.FetchParaAttr(ParaAttr.ListLevel, view);

                ListNumberState state = generator.GetNextListNumberState(list, listLevel);

                // WORDSNET-5554 Set LocaleId to be able to determine correct LocalizedOrdinalSuffix.
                state.LocaleId = para.ParagraphBreakFont.LocaleId;

                if (listLabelAction == ListLabelAction.UpdateAndSet)
                {
                    string[] labelFragments = BuildListLabelFragments(state, false);
                    string[] labelArabicNumbers = BuildListLabelFragments(state, true);

                    para.ListLabel.SetLabelStringAndValue(labelFragments, labelArabicNumbers, state.Snapshot(), view);

                    // Update both labels at first pass.
                    if (view == RevisionsView.Original)
                        para.ListLabel.SetLabelStringAndValue(labelFragments, labelArabicNumbers, state.Snapshot(), RevisionsView.Final);

                    return true;
                }
            }

            para.ResetListLabel(view);
            // Update both labels at first pass.
            if (view == RevisionsView.Original)
                para.ResetListLabel(RevisionsView.Final);

            return needUpdate;
        }

        /// <summary>
        /// Checks if the list label of the paragraph is visible in MS Word.
        /// </summary>
        internal static bool IsListLabelVisible(Paragraph para, RevisionsView view)
        {
            ListLabelAction listLabelAction = GetListLabelAction(para, view);
            return (listLabelAction != ListLabelAction.UpdateAndClear) && (listLabelAction != ListLabelAction.Clear);
        }

        /// <summary>
        /// Returns action needed to correctly update list label of paragraph.
        /// </summary>
        /// <remarks>
        /// <para>First check is whether the paragraph is list item.</para>
        /// <para>Second check is whether the paragraph is inside merged table cell.</para>
        /// <para>Third check is whether the paragraph contains any characters except page breaks.
        /// List labels for paragraphs with page breaks only are not written as is MS Word.</para>
        /// <para>The last check is whether the paragraph is section break.
        /// List label is not written for such a paragraph.</para>
        /// </remarks>
        private static ListLabelAction GetListLabelAction(Paragraph para, RevisionsView view)
        {
            // If paragraph is marked for deletion it is not finally numbered.
            if ((view == RevisionsView.Final) && para.IsDeleteRevision)
                return ListLabelAction.Clear;

            // If paragraph is marked for insertion it is not numbered originally.
            if ((view == RevisionsView.Original) && para.IsInsertRevision)
                return ListLabelAction.Clear;

            int listId = (int)para.FetchParaAttr(ParaAttr.ListId, view);
            if ((listId == 0) || (para.IsInCell && para.ParentCell.CellPr.IsMergedToPrevious))
            {
                return ListLabelAction.Clear;
            }

            ListLabelAction result = AnalyzeEmptinessAndPageBreaksInParagraph(para);

            if (result == ListLabelAction.Undetermined)
            {
                // Section breaks (breaks only, not the last paragraph in the document) seem to never update and display
                // list labels, unless they are in sdt (dd: customxml is probably also the case, so add generic markup check here
                // instead of just checking for sdt).
                result = (!para.IsEndOfSection || para.IsEndOfDocument || NodeUtil.IsMarkupNode(para.ParentNode))
                    ? ListLabelAction.UpdateAndSet : ListLabelAction.Clear;
            }

            return result;
        }

        /// <summary>
        /// <para>Analyzes characters of the paragraph.</para>
        /// <para>If the paragraph is not empty, list label must be written.</para>
        /// <para>There is exception from the rule. If the paragraph contains only page breaks, list label
        /// number must be incremented but list label itself must not be written.</para>
        /// </summary>
        /// <remarks>
        /// Not writing of list label for paragraph with page breaks only is MS Word behavior.
        /// </remarks>
        private static ListLabelAction AnalyzeEmptinessAndPageBreaksInParagraph(Paragraph para)
        {
            ListLabelAction result = ListLabelAction.Undetermined;

            foreach (Node node in para)
            {
                switch (node.NodeType)
                {
                    case NodeType.BookmarkStart:
                    case NodeType.BookmarkEnd:
                    // WORDSNET-25064 Skip editable range nodes as well.
                    case NodeType.EditableRangeStart:
                    case NodeType.EditableRangeEnd:
                    {
                        // Skip bookmark nodes.
                        continue;
                    }

                    case NodeType.Run:
                    {
                        string runText = ((Run)node).Text;

                        if (!StringUtil.HasChars(runText))
                            continue;

                        if (IsActionUpdateAndSet(para) || ContainsNonPageBreakChars(runText))
                            return ListLabelAction.UpdateAndSet;

                        result = ListLabelAction.UpdateAndClear;
                        break;
                    }

                    default:
                    {
                        // A shape or something? The label is visible then.
                        return ListLabelAction.UpdateAndSet;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true, if list label action for the specified paragraph is <see cref="ListLabelAction.UpdateAndSet"/>.
        /// </summary>
        private static bool IsActionUpdateAndSet(Paragraph para)
        {
            // andrnosk: WORDSNET-8481 If PageBreakChar is in cell we have to update and set its list label.
            if (para.IsInCell)
                return true;

            // WORDSNET-13523 MS Word displays list label for PageBreakChar,
            // if following compatibility option is set.
            if (para.Document.DocPr.CompatibilityOptions.SplitPgBreakAndParaMark)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true, if text contains characters other than <see cref="ControlChar.PageBreakChar"/>.
        /// </summary>
        private static bool ContainsNonPageBreakChars(string text)
        {
            foreach (char c in text)
            {
                if (c != ControlChar.PageBreakChar)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the complete label for the current list number for most cases.
        /// <returns>List label split to fragments</returns>
        /// </summary>
        private static string[] BuildListLabelFragments(ListNumberState state, bool isArabicNumbersWithoutSeparators)
        {
            int dummy;
            string[] buildLevelLabel = BuildLevelLabelFragments(state, state.GetListLevel(), null, isArabicNumbersWithoutSeparators, out dummy);
            return buildLevelLabel;
        }

        /// <summary>
        /// Gets the full list label (including the labels of the previous levels) for the specified list numbering state.
        /// </summary>
        /// <remarks>
        /// Previous level labels are not included, if the previous levels are present in a number format for the lower levels.
        /// </remarks>
        internal static string BuildFullListLabel(ListNumberState state, IListLabelBuildBehaviour behaviour, string separator)
        {
            return BuildFullListLabel(state, ListLevel.MinLevel, behaviour, separator);
        }

        /// <summary>
        /// Gets the full list label (including the labels of the previous levels) for the specified list numbering state, up to the specified level.
        /// </summary>
        /// <remarks>
        /// Previous level labels are not included, if the previous levels are present in the number format for the lower levels.
        /// </remarks>
        internal static string BuildFullListLabel(ListNumberState state, int minLevel, IListLabelBuildBehaviour behaviour, string separator)
        {
            StringBuilder fullLabel = new StringBuilder();

            // Go back from the current level.
            int minLevelUsed = state.CurrentLevel + 1;
            for (int level = state.CurrentLevel; level >= minLevel; --level)
            {
                // Do not add a label for the level if the level was already used via NumberFormat of the previous levels.
                if (level >= minLevelUsed)
                    continue;

                if (!IsAccountedForRef(state, level))
                    continue;

                ListLevel levelPr = state.GetListLevel(level);

                int minLevelUsedForThisLevel;
                string levelLabel = BuildLevelLabelString(state, levelPr, behaviour, out minLevelUsedForThisLevel);

                // WORDSNET-13141 MS Word misses a dot symbol, if it is the last symbol of list level and next list level starts with parentheses.
                if (levelLabel.EndsWith(".", StringComparison.Ordinal) &&
                    fullLabel.Length > 0 && fullLabel[0] == '(')
                    levelLabel = levelLabel.Remove(levelLabel.Length - 1);

                if (!string.IsNullOrEmpty(separator) && fullLabel.Length != 0)
                    fullLabel.Insert(0, separator);

                fullLabel.Insert(0, levelLabel);

                if (minLevelUsedForThisLevel < minLevelUsed)
                    minLevelUsed = minLevelUsedForThisLevel;
            }

            return fullLabel.ToString();
        }

        /// <summary>
        /// Checks that given level should be present in REF list label.
        /// </summary>
        /// <remarks>
        /// AM. It seems that Word does this way:
        /// If level was used during numbering then level is accounted.
        /// If level is less than first level used during numbering then level is accounted.
        /// Otherwise level is not accounted.
        /// </remarks>
        private static bool IsAccountedForRef(ListNumberState state, int level)
        {
            const int allLevels = 0x1ff;

            // Bit mask to test whether higher levels were used.
            int higherLevels = allLevels >> (ListLevel.MaxLevels - level);

            int thisLevel = 1 << level;

            // Level was used so account it.
            if (BitUtil.IsSetInt32(state.UsedLevels, thisLevel))
                return true;

            // Otherwise account level only if it before first used level.
            return (state.UsedLevels & higherLevels) == 0;
        }

        /// <summary>
        /// Returns a string representation of list label built by <see cref="BuildLevelLabelFragments"/>.
        /// </summary>
        private static string BuildLevelLabelString(ListNumberState state, ListLevel levelPr, IListLabelBuildBehaviour behaviour, out int minLevelUsed)
        {
            string[] fragments = BuildLevelLabelFragments(state, levelPr, behaviour, false, out minLevelUsed);
            return ArrayUtil.StringArrayToString(fragments);
        }

        /// <summary>
        /// Builds a list label for the given list numbering state and level.
        /// </summary>
        /// <remarks>
        /// If the upper level labels are included in the current level label (like A.1., where A is at upper level),
        /// they should not be included twice when building a full label. That's why this method returns the minimal level used.
        /// </remarks>
        /// <returns>Minimal level used in this level's label split to fragments.</returns>
        private static string[] BuildLevelLabelFragments(
            ListNumberState state,
            ListLevel levelPr,
            IListLabelBuildBehaviour behaviour,
            bool isArabicNumbersWithoutSeparators,
            out int minLevelUsed)
        {
            ListLabelFragmentsBuilder builder = CreateListLabelFragmentsBuilder();
            minLevelUsed = BuildLevelLabel(builder, state, levelPr, behaviour, isArabicNumbersWithoutSeparators);
            return builder.GetFragments();
        }

        /// <summary>
        /// Builds a list label for the given list numbering state and level using the specified <see cref="IListLabelBuilder"/>.
        /// </summary>
        private static int BuildLevelLabel(
            IListLabelBuilder builder,
            ListNumberState state,
            ListLevel levelPr,
            IListLabelBuildBehaviour behaviour,
            bool isArabicNumbersWithoutSeparators)
        {
            if (behaviour == null)
                behaviour = EmptyListLabelBuildBehaviour.Instance;

            int minLevelUsed = levelPr.LevelNumber;

            for (int i = 0; i < levelPr.NumberFormat.Length; i++)
            {
                char c = levelPr.NumberFormat[i];

                // WORDSNET-14314 MS Word does not show non-printable characters in list label
                // in fixed file formats. Some of them are characters from Unicode category 'Format'.
                //
                // WORDSJAVA-1908 '-' is acceptable symbol
                if (char.GetUnicodeCategory(c) == UnicodeCategory.Format && c != '\u00AD')
                    continue;

                if (ListLevel.IsLevelNumberValid(c))
                {
                    // WORDSNET-12589 Avoid to build label if NumberStyle is None.
                    // WORDSNET-17863 Check NumberStyle of list level of current placeholder instead of NumberStyle of list
                    // level of current paragraph. In addition, Word ignores bulleted list levels in placeholders.
                    // See [MS-OI29500], 2.1.285: If the numFmt is bullet, Word will use the literal text in lvlText,
                    // ignoring %x syntax in lvlText.
                    ListLevel placeHolderLevel = state.GetListLevel(c);
                    if (HasCounter(placeHolderLevel.NumberStyle))
                    {
                        string labelPart = BuildListLabelPart(state, levelPr, c, isArabicNumbersWithoutSeparators, behaviour);
                        if (labelPart.Length > 0)
                        {
                            builder.Append(labelPart);
                            behaviour.NotifyListNumberAppended(builder.Length);
                        }

                        if (minLevelUsed > c)
                            minLevelUsed = c;
                    }
                }
                else if (!isArabicNumbersWithoutSeparators && behaviour.ShouldAppendNotListNumberChar(c))
                {
                    builder.Append(c);
                }
            }

            builder.Length = behaviour.FinalizeListLabelLength(builder.Length);

            return minLevelUsed;
        }

        /// <summary>
        /// Gets the label for the specified level of the current list.
        /// </summary>
        private static string BuildListLabelPart(
            ListNumberState state,
            ListLevel currentLevelPr,
            int levelOfLabel,
            bool isArabicNumbersWithoutSeparators,
            IListLabelBuildBehaviour behaviour)
        {
            int number = state.GetNumber(levelOfLabel);

            ListLevel levelPrOfLabel = state.GetListLevel(levelOfLabel);

            // WORDSNET-9829 Skip level with picture number style
            if (levelPrOfLabel.HasPictureBullet)
                return "";

            NumberStyle numberStyle = behaviour.NormalizeNumberStyle(levelPrOfLabel.NumberStyle);

            if (numberStyle == NumberStyle.Bullet)
            {
                return levelPrOfLabel.NumberFormat;
            }

            // Most likely we don't need to check anything regarding levelOfLabel.
            // All levels output here are less or equal than current. And all they
            // should be output as Arabic for legal lists.
            // Several number styles are not affected by IsLegal.
            if (isArabicNumbersWithoutSeparators ||
                currentLevelPr.IsLegal && HasCounter(numberStyle) && (numberStyle != NumberStyle.LeadingZero))
            {
                numberStyle = NumberStyle.Arabic;
            }

            // WORDSNET-5554 We should take state.LocaleId into consideration, to handle language depended OrdinalSuffix.
            return NumberConverter.NumberToLocalizedString(number, numberStyle, levelPrOfLabel.CustomNumberStyle, true, state.LocaleId);
        }

        /// <summary>
        /// Gets a list label text for the given list numbering state with optionally trimmed non-level-number characters
        /// from start and end.
        /// </summary>
        internal static string BuildListLabel(ListNumberState state, IListLabelBuildBehaviour behaviour)
        {
            ListLabelBuilder builder = CreateListLabelBuilder();
            BuildLevelLabel(builder, state, state.GetListLevel(), behaviour, false);
            return builder.ToString();
        }

        /// <summary>
        /// A factory method to create a <see cref="ListLabelBuilder"/> instance.
        /// </summary>
        /// <remarks>
        /// This method should be an instance member the code to be autoportable to Java since
        /// instances of nested classes should be created within instance members of an outer class.
        /// </remarks>
        private static ListLabelBuilder CreateListLabelBuilder()
        {
            return new ListLabelBuilder();
        }

        /// <summary>
        /// A factory method to create a <see cref="ListLabelFragmentsBuilder"/> instance.
        /// </summary>
        /// <remarks>
        /// This method should be an instance member the code to be autoportable to Java since
        /// instances of nested classes should be created within instance members of an outer class.
        /// </remarks>
        private static ListLabelFragmentsBuilder CreateListLabelFragmentsBuilder()
        {
            return new ListLabelFragmentsBuilder();
        }

        internal static bool HasCounter(NumberStyle numberStyle)
        {
            return (numberStyle != NumberStyle.None) && (numberStyle != NumberStyle.Bullet);
        }

        /// <summary>
        /// Specifies an action that should be applied to a list label.
        /// </summary>
        private enum ListLabelAction
        {
            Undetermined,
            UpdateAndSet,
            UpdateAndClear,
            Clear
        }

        /// <summary>
        /// Represents a list label builder.
        /// </summary>
        private interface IListLabelBuilder
        {
            void Append(char c);
            void Append(string s);
            int Length { get; set; }
        }

        /// <summary>
        /// Implements <see cref="IListLabelBuilder"/> in the way to build a list label string.
        /// </summary>
        private class ListLabelBuilder : IListLabelBuilder
        {
            public override string ToString()
            {
                return mBuilder.ToString();
            }

            void IListLabelBuilder.Append(char c)
            {
                mBuilder.Append(c);
            }

            void IListLabelBuilder.Append(string s)
            {
                mBuilder.Append(s);
            }

            int IListLabelBuilder.Length
            {
                get { return mBuilder.Length; }
                set { mBuilder.Length = value; }
            }

            private readonly StringBuilder mBuilder = new StringBuilder();
        }

        /// <summary>
        /// Implements <see cref="IListLabelBuilder"/> in the way to build a list label fragment string array.
        /// </summary>
        private class ListLabelFragmentsBuilder : IListLabelBuilder
        {
            internal string[] GetFragments()
            {
                return mList.ToArray();
            }

            void IListLabelBuilder.Append(char c)
            {
                mList.Add(c.ToString());
            }

            void IListLabelBuilder.Append(string s)
            {
                mList.Add(s);
            }

            int IListLabelBuilder.Length
            {
                get { return mList.Count; }
                set
                {
                    // Setter of length is used for trim non level number chars.
                    // In case of list label fragment  we should not trim it
                }
            }

            private readonly List<string> mList = new List<string>();
        }

        private class EmptyListLabelBuildBehaviour : IListLabelBuildBehaviour
        {
            private EmptyListLabelBuildBehaviour()
            {
            }

            public void NotifyListNumberAppended(int listLabelLength)
            {
                // Nobody to notify.
            }

            public bool ShouldAppendNotListNumberChar(char c)
            {
                return true;
            }

            public int FinalizeListLabelLength(int listLabelLength)
            {
                return listLabelLength;
            }

            public NumberStyle NormalizeNumberStyle(NumberStyle numberStyle)
            {
                return numberStyle;
            }

            internal static readonly IListLabelBuildBehaviour Instance = new EmptyListLabelBuildBehaviour();
        }
    }
}
