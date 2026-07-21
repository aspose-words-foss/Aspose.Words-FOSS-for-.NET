// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2013 by Ivan Lyagin

using System.Collections.Generic;
using System.Text;
using Aspose.Words.Lists;
using Aspose.Words.Math;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides utility methods shared between <see cref="FieldRef"/>, <see cref="FieldPageRef"/>,
    /// <see cref="FieldStyleRef"/> and <see cref="FieldUnknown"/>.
    /// </summary>
    internal static class FieldRefUtil
    {
        /// <summary>
        /// Returns the specified field's update stage.
        /// </summary>
        internal static FieldUpdateStage GetFieldUpdateStage(Field field, string bookmarkName, bool isParagraphNumber)
        {
            // No bookmark name means error. Do not defer error message inserting.
            if (StringUtil.ContainsOnlyWhitespaces(bookmarkName))
                return FieldUpdateStage.MainLoop;

            // No bookmark means error. Do not defer error message inserting.
            // SPEED Get a bookmark from a cache.
            Bookmark bookmark = FieldUtil.GetCachedBookmark(field, bookmarkName);
            if (bookmark == null)
                return FieldUpdateStage.MainLoop;

            // WORDSNET-8226 If the referenced bookmark is dynamic (i.e. contained within a SET or ASK field result),
            // then we should update the field right away, because the bookmark's value can be changed in further.
            if (FieldUtil.IsDynamicBookmark(bookmark.BookmarkStart))
                return FieldUpdateStage.MainLoop;

            // WORDSNET-6545 Update REF field later if it references a paragraph number.
            if (isParagraphNumber)
                return FieldUpdateStage.DeferredUpdateRef;

            bool containsAnyField = false;
            foreach (Node node in bookmark.GetNodeRange())
            {
                // A special case of bookmark self-reference. See EndFieldUpdate for details.
                if (node == field.Start)
                    return FieldUpdateStage.MainLoop;

                if (node.NodeType == NodeType.FieldStart)
                    containsAnyField = true;
            }

            // WORDSNET-6545 We need to update the field later if it references other fields (which may be updated by that time).
            return containsAnyField ? FieldUpdateStage.DeferredUpdateRef : FieldUpdateStage.MainLoop;
        }

        /// <summary>
        /// Begins update of the specified field. Returns a parent paragraph for the specified bookmark's start.
        /// </summary>
        internal static Paragraph BeginFieldUpdate(
            Field field, Bookmark bookmark,
            bool isParagraphNumber,
            out bool hasBookmarkInResult)
        {
            Debug.Assert(field != null);
            Debug.Assert(bookmark != null);

            // WORDSNET-19495 Below code removes filed result nodes as usual, but if there is a referenced bookmark, removes all but the bookmark.

            NodeRange resultRange = new NodeRange(field.Separator, false, GetFieldResultRangeEnd(field), false);
            BookmarkExclusiveNodeRemover nodeRemover = new BookmarkExclusiveNodeRemover(resultRange, bookmark.Name);

            nodeRemover.RemoveCore();
            hasBookmarkInResult = nodeRemover.HasBookmark;

            // List labels should be up-to-date before the final update of the field if it references a paragraph number.
            UpdateListLabelsIfNeeded(field, isParagraphNumber);

            return GetBookmarkParentParagraph(bookmark.BookmarkStart, true);
        }

        private static Node GetFieldResultRangeEnd(Field field)
        {
           return GetFieldResultRangeEndOverride(field) ?? field.End;
        }

        private static Node GetFieldResultRangeEndOverride(Field field)
        {
            // WORDSNET-24538 If field end is inside Office Math but field separator is not,
            // then MS Words retains Office Math nodes during field update.
            OfficeMath endParent = GetTopLevelOfficeMathAncestor(field.End);
            if (endParent == null)
                return null;

            OfficeMath separatorParent = GetTopLevelOfficeMathAncestor(field.Separator);
            if (separatorParent == null || separatorParent != endParent)
                return endParent;

            return null;
        }

        private static OfficeMath GetTopLevelOfficeMathAncestor(Node node)
        {
            OfficeMath officeMath = (OfficeMath)node.GetAncestor(NodeType.OfficeMath);
            return officeMath != null
                ? officeMath.GetTopLevelOfficeMath()
                : null;
        }

        /// <summary>
        /// Ends update of the specified field, returning a field update action to be applied to the field.
        /// </summary>
        internal static FieldUpdateAction EndFieldUpdate(
            Field field,
            Bookmark bookmark,
            NodeRange range,
            Paragraph bookmarkParagraph,
            bool hasBookmarkInResult,
            bool includeNoteOrComment)
        {
            Debug.Assert(field != null);
            Debug.Assert(bookmark != null);
            Debug.Assert(bookmarkParagraph != null);

            if (includeNoteOrComment)
            {
                // WORDSNET-26220 the \f switch (include note or comment)
                // is ignored for fields in footnote/endnote, comment or header/footer.
                IStory story = field.End.GetAncestorIStory();
                includeNoteOrComment = story.StoryType == StoryType.MainText;
            }

            string error = ValidateBookmarkRange(field, range, includeNoteOrComment);
            if (error != null)
                return new FieldUpdateActionInsertErrorMessage(field, error);

            bool needUpdateListLabels = NeedUpdateListLabels(range);
            UpdateListLabelsIfNeeded(field, needUpdateListLabels);

            Node rangeStart = GetRangeStartInTableRow(bookmark, range);
            if (rangeStart != null)
                range = new NodeRange(rangeStart, true, range.End.Node, range.IsEndIncluded);

            CompositeNodeCopierListener listener = new CompositeNodeCopierListener();
            listener.Add(new FieldFakeResultAppender(field));
            // WORDSNET-13026 Replace En spaces with regular spaces.
            listener.Add(new FormFieldSpecialCharReplacer(field.Separator, field.End));

            if (!hasBookmarkInResult)
            {
                const NodeCopierOptions options =
                    NodeCopierOptions.UseSourceStartAncestorPr |
                    NodeCopierOptions.ProcessBoundBlockAnnotationAsInline |
                    NodeCopierOptions.CloneNode;

                NodeCopier.CopyWithoutFields(
                    range,
                    GetFieldResultRangeEnd(field),
                    new CompositeModifier(
                        new FieldRefNodeModifier(includeNoteOrComment),
                        new CommentIdsRegeneratorNodeModifier()),
                    listener,
                    true,
                    options);

                if (bookmarkParagraph != GetBookmarkParentParagraph(bookmark.BookmarkEnd, false))
                    NodeCopier.CloneParaPrPreserveFrameAttrs(field.Start.ParentParagraph, bookmarkParagraph.ParaPr);
            }
            else
            {
                // If the referenced bookmark is contained within field's result, then all old extra result nodes have been removed in
                // BeginFieldUpdate. We need to left out the bookmark content but remove the bookmark start and end nodes.
                bookmark.Remove();
            }

            return new FieldUpdateActionFormatResult(field);
        }

        private static Paragraph GetBookmarkParentParagraph(Node bookmarkNode, bool isStart)
        {
            if (bookmarkNode.NodeLevel == NodeLevel.Inline)
                return (Paragraph)bookmarkNode.FirstMeaningfulParentNode;

            return isStart
                ? NodeUtil.FindNextParagraph(bookmarkNode)
                : NodeUtil.FindPreviousParagraph(bookmarkNode);
        }

        private static string ValidateBookmarkRange(Field field, NodeRange range, bool includeNoteOrComment)
        {
            bool fieldInsideTable = field.Start.GetStoryAncestor(NodeType.Table) != null;

            NodeEnumerator enumerator = new NodeEnumerator(range);
            bool isDeep = true;

            while (enumerator.MoveToNextNode(isDeep))
            {
                Node node = enumerator.CurrentNode;

                // WORDSNET-6503 We need to check whether the range contains no REF field itself.
                if (node == field.Start)
                    return "Error! Not a valid bookmark self-reference.";

                // WORDSNET-16134 REF field within a table can not refer to range with section break.
                if (fieldInsideTable && (node.NodeType == NodeType.Section))
                    return "Error! Not a valid result for table.";

                isDeep = includeNoteOrComment || !(node is IStory);
            }

            return null;
        }

        private static bool NeedUpdateListLabels(NodeRange range)
        {
            foreach (Node node in range)
            {
                // If the bookmark contains a FIELDNUM, then list labels should be built before the final update of the field.
                if ((node.NodeType == NodeType.FieldStart) && FieldNumUtil.IsFieldNum(((FieldStart)node).FieldType))
                    return true;
            }

            return false;
        }

        private static Node GetRangeStartInTableRow(Bookmark bookmark, NodeRange range)
        {
            CompositeNode bookmarkStartParentNode = bookmark.BookmarkStart.ParentNode;
            bool bookmarkStartInsideOrBetweenTableRows =
                bookmarkStartParentNode.NodeType == NodeType.Row ||
                bookmarkStartParentNode.NodeType == NodeType.Table;
            if (!bookmarkStartInsideOrBetweenTableRows)
                return null;

            foreach (Node node in range)
            {
                // WORDSNET-18470/WORDSNET-18630: If the bookmark start locates in or between table rows, move range start into first paragraph in table cell.
                if (node.NodeType == NodeType.Paragraph)
                    return ((CompositeNode)node).FirstChild;
            }

            return null;
        }

        internal static bool IsRelativePositionAvailable(Field field)
        {
            if (field.IsInHeaderFooter)
                return false;

            return field.Start.GetAncestor(NodeType.Footnote) == null;
        }

        /// <summary>
        /// Returns a string representing the relative bookmark position to place in field result.
        /// </summary>
        internal static string GetRelativeBookmarkPosition(Field field, Bookmark targetBookmark)
        {
            Debug.Assert(targetBookmark != null);

            return GetRelativeNodePosition(field, targetBookmark.BookmarkStart);
        }

        internal static string GetRelativeNodePosition(Field field, Node node)
        {
            if (!IsRelativePositionAvailable(field))
                return string.Empty;

            return RelativePositionTranslator.Translate(
                field.LocaleId,
                node.IsAbove(field.Start));
        }

        /// <summary>
        /// Returns a string representing the relative node position to place in field result.
        /// </summary>
        internal static string GetRelativePosition(Field field, bool isAbove)
        {
            if (!IsRelativePositionAvailable(field))
                return string.Empty;

            return RelativePositionTranslator.Translate(field.LocaleId, isAbove);
        }

        private static void UpdateListLabelsIfNeeded(Field field, bool needUpdateListLabels)
        {
            if (needUpdateListLabels)
                field.Updater.RequestExternalAction(new ExternalActionUpdateListLabels(field.FetchDocument()));
        }

        internal static string GetParagraphNumber(
            Field field,
            Bookmark bookmark,
            Paragraph sourceParagraph,
            bool suppressNonDelimiters,
            bool insertRelativePosition)
        {
            string result = GetLabelStringInParagraphNumberContext(bookmark, sourceParagraph, suppressNonDelimiters);
            return FinalizeParagraphNumberResult(field, bookmark, sourceParagraph, insertRelativePosition, result);
        }

        internal static string GetTrimmedParagraphNumber(
            Field field,
            Paragraph sourceParagraph,
            bool insertRelativePosition)
        {
            string result = GetTrimmedLabelStringInParagraphNumberContext(sourceParagraph);
            return FinalizeParagraphNumberResult(field, null, sourceParagraph, insertRelativePosition, result);
        }

        internal static string GetParagraphNumberInRelativeContext(
            Field field,
            Bookmark bookmark,
            Paragraph sourceParagraph,
            Paragraph relativeParagraph,
            bool suppressNonDelimiters,
            string numberSeparator,
            bool insertRelativePosition)
        {
            string result = GetLabelStringInRelativeContext(field, bookmark, sourceParagraph, relativeParagraph, suppressNonDelimiters, numberSeparator);
            return FinalizeParagraphNumberResult(field, bookmark, sourceParagraph, insertRelativePosition, result);
        }

        internal static string GetParagraphNumberInFullContext(
            Field field,
            Bookmark bookmark,
            Paragraph sourceParagraph,
            bool suppressNonDelimiters,
            string numberSeparator,
            bool insertRelativePosition)
        {
            string result = GetLabelStringInFullContext(bookmark, sourceParagraph, suppressNonDelimiters, numberSeparator);
            return FinalizeParagraphNumberResult(field, bookmark, sourceParagraph, insertRelativePosition, result);
        }

        private static string GetLabelStringInParagraphNumberContext(
            Bookmark bookmark,
            Paragraph sourceParagraph,
            bool suppressNonDelimiters)
        {
            string labelString = GetListLabel(bookmark, sourceParagraph, suppressNonDelimiters);
            return TrimTrailingDot(labelString);
        }

        /// <summary>
        /// Returns substring of a label string between first and last placeholders.
        /// </summary>
        private static string GetTrimmedLabelStringInParagraphNumberContext(Paragraph sourceParagraph)
        {
            if (!sourceParagraph.HasListLabelFinal)
                return TrimTrailingDot(string.Empty);

            ListLabel listLabel = sourceParagraph.ListLabel;
            string numberFormat = listLabel.ListLevelFinal.NumberFormat;
            int trimStart = -1;
            int trimEnd = -1;
            for (int i = 0; i < numberFormat.Length; i++)
            {
                char c = numberFormat[i];
                if (ListLevel.IsPlaceholder(c))
                {
                    trimStart = trimStart != -1 ? trimStart : i;
                    trimEnd = numberFormat.Length - i - 1;
                }
            }

            string labelString = listLabel.LabelStringFinal;
            if (trimStart > 0)
                labelString = labelString.Remove(0, trimStart);
            if (trimEnd > 0)
                labelString = labelString.Remove(labelString.Length - trimEnd);

            return TrimTrailingDot(labelString);
        }

        /// <summary>
        /// Returns a label string consisting of all or some levels' labels based on relative position
        /// of the specified paragraph.
        /// </summary>
        /// <remarks>
        /// [Quote 2.1.489 Part 1 Section 17.16.5.51, REF
        /// If the REF field is contained within paragraph 4.3.1 and the referenced paragraph is numbered 4.5.2, the result shall be 5.2. end Quote]
        /// DM: Actually, at least with Word 2007 I used for testing, the ref field can be in any place, not necessarily within a numbered list paragraph.
        /// What matters is the last instance of the referenced list formatting before the field.
        /// So in the example above any reference after 4.5, but before 4.6 would be just "2". After 4.6. that would be "5.2" etc.
        /// See TestRefRelative for more examples.
        /// </remarks>
        /// <param name="field"></param>
        /// <param name="bookmark">The referenced bookmark.</param>
        /// <param name="sourceParagraph">The paragraph where the bookmark is located (REF field target).</param>
        /// <param name="relativeParagraph">The paragraph where the field is located, used to calculate the relative context.</param>
        /// <param name="suppressNonDelimiters">
        ///     Causes the REF field to suppress non-delimiter or non-numerical text when used in conjunction with the \r switch.
        ///     With this flag, for example, you can reference "Section 1.01," and only "1.01" is displayed in the result.
        /// </param>
        /// <param name="numberSeparator">Specifies custom level labels separator.</param>
        /// <returns></returns>
        private static string GetLabelStringInRelativeContext(
            Field field,
            Bookmark bookmark,
            Paragraph sourceParagraph,
            Paragraph relativeParagraph,
            bool suppressNonDelimiters,
            string numberSeparator)
        {
            if (!IsRelativePositionAvailable(field))
                return GetLabelStringInFullContext(bookmark, sourceParagraph, suppressNonDelimiters, numberSeparator);

            ListNumberState sourceParaNumberState = GetListNumberState(bookmark, sourceParagraph);

            if (sourceParaNumberState == null)
                return TrimTrailingDot(null);

            // Find the last paragraph with the same numbering format that starts before the field.
            ListNumberState relativeNumberState = GetLastNumberStateBeforeParagraph(sourceParaNumberState, relativeParagraph);

            int lastDifferentLevel = ListLevel.MinLevel;
            if (relativeNumberState != null)
            {
                // Check how many levels match before the referenced paragraph level.
                for (int level = ListLevel.MinLevel; level < sourceParaNumberState.CurrentLevel; ++level)
                {
                    ListLevel relativeLevel = relativeNumberState.GetListLevel(level);
                    ListLevel bookmarkLevel = sourceParaNumberState.GetListLevel(level);

                    int relativeNumber = relativeNumberState.GetNumber(level);
                    int bookMarkNumber = sourceParaNumberState.GetNumber(level);

                    if ((relativeLevel == bookmarkLevel) && (relativeNumber == bookMarkNumber))
                        lastDifferentLevel++;
                    else
                        break;
                }
            }

            // Make a list label up to the last different level.
            string relativeParagraphLabel = ListLabelUtil.BuildFullListLabel(
                sourceParaNumberState,
                lastDifferentLevel,
                GetListLabelBuildBehaviour(suppressNonDelimiters),
                numberSeparator);

            return TrimTrailingDot(relativeParagraphLabel);
        }

        /// <summary>
        /// Finds the last paragraph with the given list formatting before the given paragraph.
        /// </summary>
        private static ListNumberState GetLastNumberStateBeforeParagraph(
            ListNumberState refNumberState,
            Paragraph fieldParagraph)
        {
            ListLevel refListLevel = refNumberState.GetListLevel(ListLevel.MinLevel);
            ListNumberState lastNumberState = null;

            // Traverse the paragraphs from the start of the document.
            NodeCollection paragraphs = fieldParagraph.FetchDocument().GetChildNodes(NodeType.Paragraph, true);
            foreach (Paragraph paragraph in paragraphs)
            {
                ListNumberState currentNumberState = GetParagraphListNumberState(paragraph);
                bool isTheSameList = (currentNumberState != null) &&
                                     (currentNumberState.GetListLevel(ListLevel.MinLevel) == refListLevel);

                if (isTheSameList)
                    lastNumberState = currentNumberState;

                // Stop after the paragraph containing the field is checked.
                if (paragraph == fieldParagraph)
                    break;
            }

            return lastNumberState;
        }

        /// <summary>
        /// Returns a label string consisting of all levels' labels down to the current.
        /// </summary>
        /// <returns></returns>
        private static string GetLabelStringInFullContext(
            Bookmark bookmark,
            Paragraph sourceParagraph,
            bool suppressNonDelimiters,
            string numberSeparator)
        {
            ListNumberState state = GetListNumberState(bookmark, sourceParagraph);

            string fullContextLabel = null;
            if (state != null)
                fullContextLabel = ListLabelUtil.BuildFullListLabel(state, GetListLabelBuildBehaviour(suppressNonDelimiters), numberSeparator);

            return TrimTrailingDot(fullContextLabel);
        }

        private static string FinalizeParagraphNumberResult(
            Field field,
            Bookmark bookmark,
            Paragraph sourceParagraph,
            bool insertRelativePosition,
            string fieldResult)
        {
            string relativeBookmarkPosition = insertRelativePosition
                ? GetRelativePosition(field, bookmark, sourceParagraph)
                : string.Empty;

            // Cannot combine string and capacity in one call because not available on Java.
            StringBuilder result = new StringBuilder(fieldResult.Length + relativeBookmarkPosition.Length + 2);

            result.Append(ControlChar.LeftToRightMarkChar);
            result.Append(fieldResult);

            if (!string.IsNullOrEmpty(relativeBookmarkPosition))
            {
                result.Append(' ');
                result.Append(relativeBookmarkPosition);
            }

            return result.ToString();
        }

        private static string GetRelativePosition(Field field, Bookmark bookmark, Paragraph sourceParagraph)
        {
            return bookmark != null
                ? GetRelativeBookmarkPosition(field, bookmark)
                : GetRelativeNodePosition(field, sourceParagraph);
        }

        /// <summary>
        /// <para>Trims the last '.' character from string. The string can be <c>null</c>.</para>
        /// <para>Used when paragraph list label is inserted in document as a field.</para>
        /// </summary>
        private static string TrimTrailingDot(string source)
        {
            if (!StringUtil.HasChars(source))
                return "0";

            return source[source.Length - 1] == '.' ? source.Remove(source.Length - 1, 1) : source;
        }

        private static ListNumberState GetParagraphListNumberState(Paragraph paragraph)
        {
            return GetParagraphListNumberStateFromListNumField(paragraph) ?? paragraph.ListLabel.NumberStateFinal;
        }

        private static string GetListLabel(Bookmark bookmark, Paragraph paragraph, bool suppressNonDelimiters)
        {
            ListNumberState state = GetListNumberState(bookmark, paragraph);
            if (state == null)
                return null;

            return ListLabelUtil.BuildListLabel(state, GetListLabelBuildBehaviour(suppressNonDelimiters));
        }

        private static ListNumberState GetListNumberState(Bookmark bookmark, Paragraph paragraph)
        {
            // WORDSNET-17447 MS Word extracts list number state in following order:
            // 1. LISTNUM field which contain referenced bookmark inside field code
            // 2. Paragraph list label
            // 3. First LISTNUM field within bookmark
            // 4. First LISTNUM field after bookmark (not implemented)
            // 5. First LISTNUM field within paragraph

            IList<Field> fields = ExtractListNumFields(paragraph);

            return GetFieldListNumberState(GetListNumFieldFromParagraphWithBookmark(bookmark, fields))
                ?? paragraph.ListLabel.NumberStateFinal
                ?? GetFieldListNumberState(GetListNumFieldFromParagraphInsideBookmark(bookmark, fields))
                ?? GetFieldListNumberState(GetFirstField(fields));
        }

        private static ListNumberState GetFieldListNumberState(Field field)
        {
            if (field == null)
                return null;

            return FieldNumUtil.GetFieldListNumberState(field);
        }

        private static ListNumberState GetParagraphListNumberStateFromListNumField(Paragraph paragraph)
        {
            // WORDSNET-11690 MS Word considers the LISTNUM fields in referenced paragraph.
            Field field = GetListNumFieldFromParagraph(paragraph);
            return GetFieldListNumberState(field);
        }

        private static Field GetListNumFieldFromParagraph(Paragraph paragraph)
        {
            IList<Field> fields = FieldExtractor.ExtractToCollection(paragraph, false, FieldType.FieldListNum);
            return GetFirstField(fields);
        }

        private static Field GetFirstField(IList<Field> fields)
        {
            if (fields.Count == 0)
                return null;

            return fields[0];
        }

        private static Field GetListNumFieldFromParagraphWithBookmark(Bookmark bookmark, IList<Field> fields)
        {
            if (fields.Count == 0)
                return null;

            if (bookmark == null)
                return null;

            foreach (Field field in fields)
            {
                IList<Node> fieldBookmarks = NodeFinder.FindNodes(field.GetFieldRange(), NodeType.BookmarkStart);
                if (fieldBookmarks.Contains(bookmark.BookmarkStart))
                    return field;
            }

            return null;
        }

        private static Field GetListNumFieldFromParagraphInsideBookmark(Bookmark bookmark, IList<Field> fields)
        {
            if (fields.Count == 0)
                return null;

            if (bookmark == null)
                return null;

            IList<Node> rangeFieldStarts = NodeFinder.FindNodes(bookmark.GetNodeRange(), NodeType.FieldStart);
            foreach (Field field in fields)
            {
                if (rangeFieldStarts.Contains(field.Start))
                    return field;
            }

            return null;
        }

        private static IList<Field> ExtractListNumFields(Paragraph paragraph)
        {
            return FieldExtractor.ExtractToCollection(paragraph, false, FieldType.FieldListNum);
        }

        private static IListLabelBuildBehaviour GetListLabelBuildBehaviour(bool suppressNonDelimiters)
        {
            return suppressNonDelimiters
                ? FieldRefListLabelBuildBehaviour.Instance
                : null;
        }

        private class FormFieldSpecialCharReplacer : NodeReplacer
        {
            internal FormFieldSpecialCharReplacer(FieldSeparator fieldRefSeparator, FieldEnd fieldRefEnd)
                : base(NodeType.FieldSeparator, NodeType.FieldEnd)
            {
                mFieldRefSeparator = fieldRefSeparator;
                mFieldRefEnd = fieldRefEnd;
            }

            protected override void CollectClone(Node source, Node clone)
            {
                FieldChar fieldChar = (FieldChar)source;
                if (fieldChar.FieldType != FieldType.FieldFormTextInput)
                    return;

                base.CollectClone(source, clone);
            }

            protected override void ReplaceCollectedNode(Node node)
            {
                FieldChar fieldChar = (FieldChar)node;

                Debug.Assert(fieldChar.FieldType == FieldType.FieldFormTextInput);

                switch (fieldChar.NodeType)
                {
                    case NodeType.FieldSeparator:
                        mLastSeparator = (FieldSeparator)fieldChar;
                        break;
                    case NodeType.FieldEnd:

                        if (mLastSeparator == null)
                            mLastSeparator = mFieldRefSeparator;

                        ReplaceSpecialCharsInRange(mLastSeparator, fieldChar);

                        mLastSeparator = null;

                        break;
                    default:
                        Debug.Fail("Unexpected node type");
                        break;
                }
            }

            protected override void FinalizeReplace()
            {
                base.FinalizeReplace();

                if (mLastSeparator != null)
                    ReplaceSpecialCharsInRange(mLastSeparator, mFieldRefEnd);
            }

            private static void ReplaceSpecialCharsInRange(Node start, Node end)
            {
                List<SpecialChar> specialChars = FindSpecialChars(start, end);
                if (specialChars == null)
                    return;

                foreach (SpecialChar specialChar in specialChars)
                {
                    specialChar.ParentNode.InsertBefore(new Run(specialChar.Document, ControlChar.Space, specialChar.RunPr.Clone()), specialChar);
                    specialChar.Remove();
                }
            }

            private static List<SpecialChar> FindSpecialChars(Node start, Node end)
            {
                NodeRange resultRange = new NodeRange(start, false, end, false);
                List<SpecialChar> result = null;
                using (NodeEnumerator nodeEnumerator = new NodeEnumerator(resultRange))
                {
                    while (nodeEnumerator.MoveToNextNode())
                    {
                        if (nodeEnumerator.CurrentNode.NodeType != NodeType.SpecialChar)
                            continue;

                        SpecialChar specialChar = (SpecialChar)nodeEnumerator.CurrentNode;
                        if (specialChar.GetText() != ControlChar.DefaultTextInput)
                            continue;

                        if (result == null)
                            result = new List<SpecialChar>();

                        result.Add(specialChar);
                    }
                }

                return result;
            }

            private FieldSeparator mLastSeparator;

            private readonly FieldSeparator mFieldRefSeparator;
            private readonly FieldEnd mFieldRefEnd;
        }

        private static class RelativePositionTranslator
        {
            internal static string Translate(int language, bool above)
            {
                switch (LanguageOnly.ToLanguageOnly(language))
                {
                    // ReSharper disable StringLiteralTypo
                    case LanguageOnly.Afrikaans: return above ? "hierbo" : "hieronder";
                    case LanguageOnly.Armenian: return above ? "վեռժում" : "նեռքժում";
                    case LanguageOnly.Azeri:
                        switch ((Language)language)
                        {
                            case Language.AzeriCyrillic: return above ? "јухары" : "ашағы";
                            default: return above ? "yuxarı" : "aşağı";
                        }
                    case LanguageOnly.Basque: return above ? "goian" : "behean";
                    case LanguageOnly.Belarusian: return above ? "вышэй" : "ніжэй";
                    case LanguageOnly.Bulgarian: return above ? "отгоре" : "отдолу";
                    case LanguageOnly.Catalan: return above ? "a dalt" : "més avail";
                    case LanguageOnly.Chinese:
                        switch ((Language)language)
                        {
                            case Language.ChineseChina:return above ? "见上方" : "见下方";
                            default: return above ? "如上" : "如下";
                        }
                    case LanguageOnly.Croatian: return above ? "gore" : "dolje";
                    case LanguageOnly.Czech: return above ? "nahoře" : "dole";
                    case LanguageOnly.Danish: return above ? "ovenfor" : "nedenfor";
                    case LanguageOnly.Dutch: return above ? "hierboven" : "hieronder";
                    case LanguageOnly.Estonian: return above ? "ülalpool" : "allpool";
                    case LanguageOnly.Finnish: return above ? "yllä" : "alla";
                    case LanguageOnly.French: return above ? "ci-dessus" : "ci-dessous";
                    case LanguageOnly.Georgian: return above ? "ზემოთ" : "ქვემოთ";
                    case LanguageOnly.German: return above ? "oben" : "unten";
                    case LanguageOnly.Greek: return above ? "παραπάνω" : "παρακάτω";
                    case LanguageOnly.Hungarian: return above ? "előbb" : "alább";
                    case LanguageOnly.Italian: return above ? "più sopra" : "più sotto";
                    case LanguageOnly.Japanese: return above ? "上の" : "下の";
                    case LanguageOnly.Kazakh: return above ? "жоғарғы" : "төменгі";
                    case LanguageOnly.Kiswahili: return above ? "juu ya" : "chini ya";
                    case LanguageOnly.Korean: return above ? "위참조" : "아래참조";
                    case LanguageOnly.Latvian: return above ? "virs" : "zem";
                    case LanguageOnly.Lithuanian: return above ? "aukščiau" : "žemiau";
                    case LanguageOnly.Malay: return above ? "Atas" : "Bawah";
                    case LanguageOnly.Norwegian: return above ? "ovenfor" : "nedenfor";
                    case LanguageOnly.Polish: return above ? "wyżej" : "niżej";
                    case LanguageOnly.Portuguese: return above ? "acima" : "abaixo";
                    case LanguageOnly.Romanian: return above ? "deasupra" : "dedesubt";
                    case LanguageOnly.Russian: return above ? "выше" : "ниже";
                    case LanguageOnly.Slovak: return above ? "vyššie" : "nižšie";
                    case LanguageOnly.Slovenian: return above ? "zgoraj" : "spodaj";
                    case LanguageOnly.Spanish: return above ? "más atrás" : "más adelante";
                    case LanguageOnly.Swedish: return above ? "ovan" : "nedan";
                    case LanguageOnly.Tatar: return above ? "өск" : "аска";
                    case LanguageOnly.Turkish: return above ? "üst" : "alt";
                    case LanguageOnly.Ukrainian: return above ? "вище" : "нижче";
                    default: return above ? "above" : "below";
                    // ReSharper restore StringLiteralTypo
                }
            }
        }

        private class FieldRefListLabelBuildBehaviour : IListLabelBuildBehaviour
        {
            private FieldRefListLabelBuildBehaviour()
            {
            }

            void IListLabelBuildBehaviour.NotifyListNumberAppended(int listLabelLength)
            {
                // Nothing to do.
            }

            bool IListLabelBuildBehaviour.ShouldAppendNotListNumberChar(char c)
            {
                return IsDelimiterChar(c);
            }

            int IListLabelBuildBehaviour.FinalizeListLabelLength(int listLabelLength)
            {
                return listLabelLength;
            }

            NumberStyle IListLabelBuildBehaviour.NormalizeNumberStyle(NumberStyle numberStyle)
            {
                return numberStyle;
            }

            private static bool IsDelimiterChar(char c)
            {
                switch (c)
                {
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '\\':
                    case '|':
                    case '/':
                    case '-':
                    case ':':
                    case ';':
                    case '.':
                    case ',':
                        return true;
                    default:
                        return false;
                }
            }

            internal static readonly IListLabelBuildBehaviour Instance = new FieldRefListLabelBuildBehaviour();
        }
    }
}
