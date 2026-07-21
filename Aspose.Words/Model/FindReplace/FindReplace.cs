// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2016 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Bidi;
using Aspose.Collections.Generic;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;

namespace Aspose.Words.Replacing
{
    /// <summary>
    /// Implements advanced find/replace operations.
    /// </summary>
    /// <remarks>
    /// Supports to match and replace with breaks. Customer can specify formatting of replacement.
    /// </remarks>
    internal class FindReplace : DocumentVisitor
    {
        internal FindReplace(Node root, string pattern, string replacement, FindReplaceOptions options)
            : this(root, TextToRegex(pattern, options), replacement, options)
        {
        }

        internal FindReplace(NodeRange nodeRange, string pattern, string replacement, FindReplaceOptions options)
            : this(nodeRange, TextToRegex(pattern, options), replacement, options)
        {
        }

        internal FindReplace(NodeRange nodeRange, Regex regex, string replacement, FindReplaceOptions options)
        {
            Debug.Assert(regex != null);

            mOptions = options;
            mNodeRange = nodeRange;
            mDoc = nodeRange.Document;

            string pattern = ReplaceMetaCharacters(regex.ToString());
            if (!StringUtil.HasChars(pattern))
                throw new ArgumentException("The search string cannot be null or empty.");
#if NET45
            mPattern = new Regex(pattern, regex.Options, regex.MatchTimeout);
#else
            mPattern = new Regex(pattern, regex.Options);
#endif
            mReplaceWith = ReplaceMetaCharacters(replacement);
        }

        internal FindReplace(Node root, Regex regex, string replacement, FindReplaceOptions options)
        {
            Debug.Assert(regex != null);

            mOptions = options;
            mRootNode = root;
            mDoc = root.Document;

            string pattern = ReplaceMetaCharacters(regex.ToString());
            if (!StringUtil.HasChars(pattern))
                throw new ArgumentException("The search string cannot be null or empty.");
#if NET45
            // WORDSNET-23428 We should carry over a MatchTimeout property from the original regex.
            mPattern = new Regex(pattern, regex.Options, regex.MatchTimeout);
#else
            mPattern = new Regex(pattern, regex.Options);
#endif
            mReplaceWith = ReplaceMetaCharacters(replacement);
        }

        internal int Replace()
        {
            using (new SuspendTrackRevisionsDocument(mDoc, SuspendedRevisionTypes.Move))
              using (new SuspendMappedCustomXmlUpdateDocument(mDoc))
                return ReplaceCore(mRootNode);
        }

        /// <summary>
        /// Sorts nodes of the pending list to be in order that they are represented in visual structure of document.
        /// </summary>
        /// <dev>
        /// Headers/footers are sorted to be in correct order. Note that now body nodes are processed before
        /// the pending list, for example, before headers.
        /// </dev>
        internal static void SortPendingList(List<Node> list)
        {
            // Headers/footers are only sorted. Let's keep order of other nodes unchanged.
            // Headers/footers of a section occupies continuous range of list items because of FindReplaceIndexer
            // behavior. Let's use this assumption to simplify sorting code.

            List<Node> oldOrder = new List<Node>();
            oldOrder.AddRange(list);
            list.Clear();

            List<Node> headerFooters = new List<Node>();
            Section section = null;
            int startIndex = -1;

            for (int i = 0; i < oldOrder.Count; i++)
            {
                Node node = oldOrder[i];
                if (node.NodeType == NodeType.HeaderFooter)
                {
                    HeaderFooter headerFooter = (HeaderFooter)node;
                    if (section != null && headerFooter.ParentSection != section)
                    {
                        headerFooters.Sort(new HeaderFooterComparer(section.PageSetup.DifferentFirstPageHeaderFooter));
                        list.AddRange(headerFooters);
                        AddNodes(oldOrder, startIndex + 1, i - 1, list, NodeType.HeaderFooter);

                        headerFooters.Clear();
                        startIndex = i;
                    }
                    else
                    {
                        if (startIndex < 0)
                            startIndex = i;
                    }

                    headerFooters.Add(headerFooter);
                    section = headerFooter.ParentSection;
                }

                if (section == null)
                    list.Add(node);
            }

            if (headerFooters.Count > 0)
            {
                Debug.Assert(section != null);

                headerFooters.Sort(new HeaderFooterComparer(section.PageSetup.DifferentFirstPageHeaderFooter));
                list.AddRange(headerFooters);
                AddNodes(oldOrder, startIndex + 1, oldOrder.Count - 1, list, NodeType.HeaderFooter);
            }
        }

        /// <summary>
        /// Finds and replaces in one story.
        /// </summary>
        private int ReplaceCore(Node root)
        {
            // WORDSNET-14691 Replace content of CustomXML/Core properties if SDT is data bound.
            // WORDSNET-24241 Added new option to control the behavior.
            if ((root != null) && !mOptions.IgnoreStructuredDocumentTags && (root.NodeType == NodeType.StructuredDocumentTag))
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)root;
                SdtContentHelper.ReplaceDataBoundContent(sdt, mPattern, mReplaceWith);
            }

            mIndexer = mNodeRange == null
                ? new FindReplaceIndexer(root, mOptions)
                : new FindReplaceIndexer(mNodeRange, root, mOptions);

            int replaceCount = 0;

            // WORDSNET-24783 There can be situation when Indexer text is empty, but PendingList is not. So, the check when
            // nothing to replace is separated for text and pending list to avoid possible exceptions.
            if (mIndexer.Text.Length != 0)
            {
                // WORDSNET-20212 Split text to lines manually when regex option Multiline is enabled to allow
                // anchors (^ and $) work correctly.
                // WORDSNET-21588 Split text only when there are any of the anchors (^ or $) specified in pattern.
                if (IsMultiline && (mPattern.ToString().IndexOfAny(gAnchors) != -1))
                {
                    int start = 0;
                    for (int i = 0; i < mIndexer.SpecialPositions.Count; i++)
                    {
                        int breakPosition = mIndexer.SpecialPositions[i];
                        string text = mIndexer.Text.Substring(start, breakPosition - start + 1);
                        replaceCount += ReplaceLine(text, start);

                        start = breakPosition + 1;
                    }
                }
                else
                {
                    replaceCount = ReplaceLine(mIndexer.Text.AsString, 0);
                }
            }

            // Process headers/footers/textboxes/comments/cells content separately from document content.
            // There are many reasons for it, for example, section breaks insertion/deletion causes headers to move
            // within textual presentation and it's hard to detect.
            // But most major problem is that regex can match partially in one part and partially in another part and we should avoid it.
            if (mIndexer.PendingList.Count != 0)
            {
                List<Node> pendingList = mIndexer.PendingList;
                SortPendingList(pendingList);
                foreach (Node node in pendingList)
                    replaceCount += ReplaceCore(node);
            }

            return replaceCount;
        }

        /// <summary>
        /// Finds and replaces text in a specified line considering its offset within whole text.
        /// </summary>
        private int ReplaceLine(string line, int lineStart)
        {
            MatchCollection matches = mPattern.Matches(line);

            int step = (IsForward) ? 1 : -1;
            int i = (IsForward) ? 0 : matches.Count - 1;
            int remainingMatches = matches.Count;

            int replaceCount = 0;

            while (remainingMatches > 0)
            {
                ReplaceAction action = ReplaceAction.Replace;
                string replaceWith = mReplaceWith;

                // WORDSNET-21588 We have to use match offset relative to the whole text instead of the current line.
                int curMatchIndex = matches[i].Index + lineStart;

                // WORDSNET-23258 Check option before call to ReplacingCallback to avoid calling it for matches
                // that are not permit 'WholeWordsOnly' option restriction.
                if (IsFindWholeWordsOnly && !IsWordBoundary(matches[i], lineStart))
                    action = ReplaceAction.Skip;

                if ((ReplacingCallback != null) && (action == ReplaceAction.Replace))
                {
                    Node matchNode = mIndexer.GetNodeByPosition(curMatchIndex);
                    int matchOffset = curMatchIndex - mIndexer.GetNodeStartByPosition(curMatchIndex);

                    // WORDSNET-28070 Added a new replacing arg to get match end node.
                    int endIndex = curMatchIndex + matches[i].Length;
                    Node matchEndNode = mIndexer.GetNodeByPosition(endIndex);

                    ReplacingArgs args = new ReplacingArgs(matches[i], matchOffset, matchNode, matchEndNode, mReplaceWith);
                    action = ReplacingCallback.Replacing(args);
                    replaceWith = ReplaceMetaCharacters(args.Replacement);
                }

                if (action == ReplaceAction.Stop)
                    break;

                if (action != ReplaceAction.Skip)
                {
                    // WORDSNET-19913 The new option is introduced to recognize substitutions within replacing string.
                    if (IsUseSubstitutions)
                        replaceWith = matches[i].Result(replaceWith);

                    if (ReplaceSubstring(replaceWith, curMatchIndex, matches[i].Length))
                        replaceCount++;
                }

                remainingMatches--;
                i += step;
            }

            return replaceCount;
        }

        /// <summary>
        /// Checks left and right word boundary for FindWholeWordsOnly option.
        /// </summary>
        private bool IsWordBoundary(Match match, int lineStart)
        {
            int pos = match.Index + lineStart;

            if ((pos > 0) && (!IsWordBoundary(mIndexer.Text[pos - 1])))
                return false;

            // WORDSNET-21639 Check the position is less than length of text.
            pos += match.Length;
            if ((pos < mIndexer.Text.Length) && !IsWordBoundary(mIndexer.Text[pos]))
                return false;

            return true;
        }

        /// <summary>
        /// Checks that symbol can be a word boundary.
        /// </summary>
        /// <remarks>
        /// Tab, Space, Paragraph Break, Section Break, Cell Break should exist regardless of other defined word boundary symbols.
        /// </remarks>
        private static bool IsWordBoundary(char c)
        {
            return (c == ' ') ||
                   (c == '\t') ||
                   (c == FindReplaceIndexer.ParagraphBreakChar) ||
                   (c == FindReplaceIndexer.SectionBreakChar) ||
                   (c == FindReplaceIndexer.CellBreakChar) ||
                   // WORDSNET-25588 Consider column break as word boundary.
                   (c == ControlChar.ColumnBreakChar) ||
                   (WordBoundarySymbols.Contains(c.ToString()));
        }

        /// <summary>
        /// Adds nodes from the source list to the destination list except nodes of the filteredNodeType type.
        /// </summary>
        private static void AddNodes(List<Node> sourceList, int fromIndex, int toIndex,
            List<Node> destinationList, NodeType filteredNodeType)
        {
            for (int i = fromIndex; i <= toIndex; i++)
            {
                if (sourceList[i].NodeType != filteredNodeType)
                    destinationList.Add(sourceList[i]);
            }
        }

        /// <summary>
        /// Replaces substring of the indexer text at a specified offset and length.
        /// </summary>
        private bool ReplaceSubstring(string replacement, int offset, int length)
        {
            // This is first node which will be deleted during the replacing process.
            Node startNode = mIndexer.GetNodeByPosition(offset);

            // We mimic Word and do not replace shapes.
            if (startNode.NodeType == NodeType.Shape)
                return false;

            // Word expands SmartTags if any text within it is replaced.
            if (startNode.ParentNode is SmartTag)
                ExpandSmartTagToPara(startNode);

            // The text of the run may be longer than the text we have to replace.
            if (startNode.NodeType == NodeType.Run)
                SplitRun(offset, (Run)startNode);

            // Word applies formatting to the inserted content from the node where the new content will be inserted.
            // This is actually first node that will be deleted upon replace process.
            // Also customer can specify additional formatting to apply to the inserted content.
            UpdateRefFormatting(offset);

            string textToDelete = mIndexer.Text.Substring(offset, length);

            // WORDSNET-26924, 23740 Changed the order of deletion/insertion.
            // This is the index in text where a new content will be inserted.
            int insertIndex = offset + textToDelete.Length;

            // This is last node which will be deleted during the replacing process.
            Node endNode = null;
            // WORDSNET-28047 Check index greater 0 to avoid possible exception.
            if (insertIndex > 0)
                endNode = mIndexer.GetNodeByPosition(insertIndex - 1);

            // WORDSNET-28047 There can be previously removed nodes after which we are going
            // to insert a new replacement string. Check here Parent instead of IsRemoved,
            // as it doesn't work correctly in SDTs.
            if ((endNode == null) || (endNode.ParentNode == null))
            {
                // This is a node before we insert replacement.
                Node refNode = mIndexer.GetNodeByPosition(insertIndex);
                if ((refNode == null) || (refNode.ParentNode == null))
                    return false;

                if (refNode.NodeLevel == NodeLevel.Inline)
                {
                    Run run = new Run(mDoc, replacement, ((Inline)refNode).RunPr);
                    refNode.InsertPrevious(run);
                    return true;
                }

                return false;
            }

            // And this should be split the same as first replacing node above.
            if (endNode.NodeType == NodeType.Run)
                SplitRun(insertIndex, (Run)endNode);

            // Word expands SmartTags if any text within it is replaced.
            if (endNode.ParentNode is SmartTag)
                ExpandSmartTagToPara(endNode);

            // First we insert new text.
            SplitAndInsert(insertIndex, replacement);

            // Then delete old text.
            SplitAndDelete(offset, textToDelete);

            return true;
        }

        /// <summary>
        /// Splits a specified Run at a specified offset.
        /// </summary>
        /// <remarks>
        /// The specified Run becomes right part of the split Run.
        /// The corresponding indexes in <see cref="mIndexer"/> will be also updated.
        /// </remarks>
        private void SplitRun(int offset, Run run)
        {
            // In case we are in the middle of run split it.
            int nodeStart = mIndexer.GetNodeStartByPosition(offset);
            int localStart = offset - nodeStart;

            if ((localStart <= 0) || (localStart >= run.Text.Length))
                return;

            Run newRun;

            // See "TestFindReplaceB".
            using (new SuspendTrackRevisionsDocument(mDoc))
                newRun = run.SplitBefore(localStart);

            // Update modified entry without rebuilding whole index.
            // This greatly increases performance and helps to isolate from callback changes.
            mIndexer.Replace(nodeStart, nodeStart, newRun);
            mIndexer.Add(nodeStart + localStart, run);
        }

        /// <summary>
        /// Updates formatting of the reference <see cref="mRunPr"/> and <see cref="mParaPr"/>
        /// taking it from the node at a specified <see cref="mIndexer"/> offset considering
        /// <see cref="FindReplaceOptions.ApplyRunPr"/> and <see cref="FindReplaceOptions.ApplyParaPr"/>.
        /// </summary>
        private void UpdateRefFormatting(int offset)
        {
            mRunPr.Clear();
            mParaPr.Clear();

            Node node = mIndexer.GetNodeByPosition(offset);
            IInline inline = node as IInline;
            if (inline != null)
            {
                inline.RunPr_IInline.ExpandTo(mRunPr);
                // WORDSNET-28047 The node might have been removed on previous replacement.
                // In this case we cannot get its parent formatting.
                if (inline.ParentParagraph_IInline != null)
                    inline.ParentParagraph_IInline.ParaPr.ExpandTo(mParaPr);
            }
            else if (NodeUtil.IsInlineLevelNode(node))
            {
                Paragraph parentPara = (Paragraph)node.GetAncestor(NodeType.Paragraph);
                if (parentPara != null)
                {
                    parentPara.ParagraphBreakRunPr.ExpandTo(mRunPr);
                    parentPara.ParaPr.ExpandTo(mParaPr);
                }
            }
            else if (node.NodeType == NodeType.Paragraph)
            {
                ((Paragraph)node).ParagraphBreakRunPr.ExpandTo(mRunPr);
                ((Paragraph)node).ParaPr.ExpandTo(mParaPr);
            }

            // Consider also optional formatting.
            if (mOptions == null)
                return;

            if (mOptions.ApplyRunPr != null)
            {
                mOptions.ApplyRunPr.ExpandTo(mRunPr);

                // WORDSNET-25418 Remove theme color, when customer sets only Color,
                // as ThemeColor has precedence over the Color.
                if (mOptions.ApplyRunPr.Contains(FontAttr.Color) && !mOptions.ApplyRunPr.Contains(FontAttr.ThemeColor))
                    mRunPr.Remove(FontAttr.ThemeColor);
            }

            if (mOptions.ApplyParaPr != null)
                mOptions.ApplyParaPr.ExpandTo(mParaPr);
        }

        /// <summary>
        /// Processes text insertion while breaks it at special chars.
        /// </summary>
        /// <param name="pos">The position in <see cref="mIndexer"/> where to insert a specified <paramref name="text"/>.</param>
        /// <param name="text">The text to insert at a specified <paramref name="pos"/>.</param>
        private void SplitAndInsert(int pos, string text)
        {
            // Reference node to insert content after.
            Node refNode = mIndexer.GetNodeByPosition(pos - 1);

            StringBuilder chunk = new StringBuilder(text.Length);

            foreach (char ch in text)
            {
                if (IsSpecialChar(ch))
                {
                    if (chunk.Length > 0)
                    {
                        refNode = InsertText(refNode, chunk.ToString(), pos);
                        chunk.Length = 0;
                    }

                    refNode = InsertSpecialChar(refNode, ch);
                }
                else
                {
                    chunk.Append(ch);
                }
            }

            if (chunk.Length > 0)
                InsertText(refNode, chunk.ToString(), pos);
        }

        /// <summary>
        /// Inserts text after given node.
        /// </summary>
        private Run InsertText(Node refNode, string text, int pos)
        {
            RunPr runPr = mRunPr.Clone();

           // WORDSNET-15621 It seems that we need to set BiDi for strong RTL replacement text.
            if (IsStrongRtlOrWhiteSpaces(text))
                runPr.SetAttr(FontAttr.Bidi, AttrBoolEx.True);

            // WORDSNET-20007 Preserve existing run on replacement.
            if ((refNode.NodeType == NodeType.Run) && (refNode == mRootNode))
            {
                Run prevRun;
                Run curRun;

                using (new SuspendTrackRevisionsDocument(mDoc))
                {
                    prevRun = (Run)refNode.InsertPrevious(refNode.Clone(false));
                    curRun = (Run)refNode;
                    curRun.Text = text;
                    curRun.RunPr = runPr;
                }

                // We need to manually mark existing run as inserted if tracking is on.
                if (mDoc.IsTrackRevisionsEnabled)
                {
                    EditSession editSession = mDoc.FetchDocumentOrGlossaryMain().EditSession;
                    RevisionTrackingUtil.AddInsertRevision(curRun, editSession);
                }

                int prevRunPos = pos - prevRun.Text.Length;
                mIndexer.Replace(prevRunPos, prevRunPos, prevRun);

                return curRun;
            }
            else
            {
                Run newRun = new Run(mDoc, text, runPr);

                if (refNode.NodeLevel == NodeLevel.Inline)
                {
                    // Just insert after inline node.
                    refNode.InsertNext(newRun);
                }
                else if (refNode.NodeType == NodeType.Paragraph)
                {
                    // Insertion after paragraph break means that we need to insert into beginning of next para.
                    Paragraph para = (Paragraph)refNode;
                    Paragraph nextPara = (Paragraph)para.NextPreOrderOfType(mDoc, NodeType.Paragraph);
                    if (nextPara == null)
                        nextPara = (Paragraph)para.InsertNext(new Paragraph(mDoc, mParaPr.Clone(), mRunPr.Clone()));

                    nextPara.InsertBefore(newRun, nextPara.FirstChild);
                }
                else
                {
                    // Unexpected node type.
                    Debug.Assert(false);
                }

                return newRun;
            }
        }

        private static bool IsStrongRtlOrWhiteSpaces(string text)
        {
            bool hasRTLChar = false;

            foreach (char c in text)
            {
                BidiCharacterType characterType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
                bool isStrongRtlCharacter = (characterType == BidiCharacterType.R) || (characterType == BidiCharacterType.AL);
                hasRTLChar |= isStrongRtlCharacter;

                if (isStrongRtlCharacter)
                    continue;

                if (char.IsWhiteSpace(c))
                    continue;

                return false;
            }

            // WORDSNET-16031 Set "BiDi" attribute forcibly, when text of the run contains only RTL chars or
            // RTL chars with spaces.
            return hasRTLChar;
        }

        /// <summary>
        /// Inserts a special char after given node.
        /// </summary>
        private Node InsertSpecialChar(Node node, char ch)
        {
            switch (ch)
            {
                case FindReplaceIndexer.ParagraphBreakChar:
                    return InsertParagraphBreak(node);

                case FindReplaceIndexer.SectionBreakChar:
                    return InsertSectionBreak(node);

                default:
                    // Unexpected special character.
                    Debug.Assert(false);
                    break;
            }

            return node;
        }

        /// <summary>
        /// Inserts paragraph break after given node.
        /// </summary>
        private Node InsertParagraphBreak(Node nodeBefore)
        {
            if (!CanInsertParagraphBreak(nodeBefore))
                return nodeBefore;

            Paragraph newPara = new Paragraph(mDoc, mParaPr.Clone(), mRunPr.Clone());

            switch (nodeBefore.NodeLevel)
            {
                case NodeLevel.Inline:
                {
                    Paragraph paraAfter = ((Run)nodeBefore).ParentParagraph;
                    paraAfter.InsertPrevious(newPara);
                    using (new SuspendTrackRevisionsDocument(mDoc))
                        newPara.InsertBefore(paraAfter.FirstChild, nodeBefore.NextSibling, null);

                    break;
                }

                case NodeLevel.Block:
                {
                    Paragraph paraBefore = (Paragraph)nodeBefore;
                    paraBefore.InsertNext(newPara);

                    break;
                }

                default:
                    // Unexpected node level.
                    Debug.Assert(false);
                    return nodeBefore;
            }

            return newPara;
        }

        /// <summary>
        /// Expand the SmartTag into the parent paragraph.
        /// </summary>
        /// <remarks>
        /// Once expanded, the all contents of the SmartTag will be moved to the parent paragraph,
        /// and the SmartTag will be removed. Supports nesting.
        /// </remarks>
        private static void ExpandSmartTagToPara(Node childSmartTag)
        {
            while (childSmartTag.ParentNode.NodeType != NodeType.Paragraph)
            {
                CompositeNode smartTag = childSmartTag.ParentNode;
                CompositeNode parentSmartTag = smartTag.ParentNode;

                parentSmartTag.InsertBefore(smartTag.FirstChild, null, smartTag.NextSibling);
                smartTag.Remove();
            }
        }

        /// <summary>
        /// Inserts section break before given node.
        /// </summary>
        private Node InsertSectionBreak(Node node)
        {
            if (!CanInsertSectionBreak(node))
                return node;

            Section newSect = new Section(mDoc);
            newSect.AppendChild(new Body(mDoc));

            Node newPara = InsertParagraphBreak(node);
            // Check new paragraph has been actually inserted.
            if (newPara == node)
                return node;

            Section curSect = (Section)node.GetAncestor(NodeType.Section);

            // WORDSNET-22353 Make Word-like section break insertion.
            newSect.SectPr = curSect.SectPr.Clone();
            while (curSect.HeadersFooters.Count > 0)
                newSect.AppendChild(curSect.HeadersFooters[0]);

            // If we are at the very end of section, then just append a new section.
            if (newPara.IsLastChild)
            {
                curSect.InsertNext(newSect);
                newSect.Body.AppendChild(newPara);
                return newPara;
            }

            // Otherwise, we should leave the rest of the section content (after the
            // new inserted paragraph break) inside the current old section.
            curSect.InsertPrevious(newSect);
            newSect.Body.InsertBefore(curSect.Body.FirstChild, newPara.NextSibling, null);
            return ((Paragraph)newPara).Count == 0 ? newPara : ((Paragraph)newPara).LastChild;
        }

        /// <summary>
        /// Returns true if paragraph break can be inserted before or after the node.
        /// </summary>
        /// <remarks>
        /// AM. So far only inline SDTs are prevented to insert paragraph break into.
        /// </remarks>
        private static bool CanInsertParagraphBreak(Node node)
        {
            Node parentNode = node.ParentNode;

            while (parentNode != null)
            {
                switch (parentNode.NodeType)
                {
                    case NodeType.StructuredDocumentTag:
                        StructuredDocumentTag sdt = (StructuredDocumentTag)parentNode;
                        if (sdt.Level == MarkupLevel.Inline)
                            return false;
                        parentNode = parentNode.ParentNode;
                        break;

                    default:
                        parentNode = parentNode.ParentNode;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if section break can be inserted before the node.
        /// </summary>
        private static bool CanInsertSectionBreak(Node before)
        {
            Node parentNode = before.ParentNode;

            while (parentNode != null)
            {
                switch (parentNode.NodeType)
                {
                    case NodeType.Cell:
                    case NodeType.HeaderFooter:
                    case NodeType.Shape:
                    case NodeType.GroupShape:
                    case NodeType.Comment:
                    case NodeType.StructuredDocumentTag:
                        return false;

                    default:
                        parentNode = parentNode.ParentNode;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Processes text deletion while breaks it at special chars.
        /// </summary>
        private void SplitAndDelete(int start, string text)
        {
            StringBuilder chunk = new StringBuilder(text.Length);

            int chunkStart = start;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];

                if (IsSpecialChar(ch))
                {
                    if (chunk.Length > 0)
                    {
                        DeleteText(chunkStart, chunk.Length);
                    }

                    DeleteSpecialChar(chunkStart + chunk.Length, ch);
                    chunkStart += chunk.Length + 1;
                    chunk.Length = 0;
                }
                else
                {
                    chunk.Append(ch);
                }
            }

            if (chunk.Length > 0)
            {
                DeleteText(chunkStart, chunk.Length);
            }
        }

        /// <summary>
        /// Deletes text portion from given position.
        /// </summary>
        private void DeleteText(int start, int len)
        {
            // The collection of bound StructuredDocumentTags, whose content will be deleted.
            HashSetGeneric<StructuredDocumentTag> boundSdts = new HashSetGeneric<StructuredDocumentTag>();

            int remains = len;
            while (remains > 0)
            {
                Run run = (Run)mIndexer.GetNodeByPosition(start);

                // WORDSNET-24345 Collect affected parent bound StructuredDocumentTags to update them accordingly.
                if (mOptions.IgnoreStructuredDocumentTags)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)run.GetAncestor(NodeType.StructuredDocumentTag);
                    if ((sdt != null) && sdt.XmlMapping.IsMapped)
                        boundSdts.Add(sdt);
                }

                if (run.GetTextLength() <= remains)
                {
                    remains -= run.GetTextLength();
                    start += run.GetTextLength();
                    run.Text = "";

                    SmartTag parentSmartTag = run.ParentNode as SmartTag;
                    run.Remove();

                    if (parentSmartTag != null && parentSmartTag.Count == 0)
                        parentSmartTag.Remove();
                }
                else
                {
                    Run newRun;

                    if (run.ParentNode is SmartTag)
                        ExpandSmartTagToPara(run);

                    using (new SuspendTrackRevisionsDocument(mDoc))
                        newRun = run.SplitBefore(remains);

                    // Update modified entry without rebuilding whole index.
                    // This greatly increases performance and helps to isolate from callback changes.
                    mIndexer.Replace(start, start, newRun);
                    mIndexer.Add(start + remains, run);

                    newRun.Remove();
                    remains = 0;
                }
            }

            // WORDSNET-24345 Update bound StructuredDocumentTags by its remained content.
            foreach (StructuredDocumentTag boundSdt in boundSdts)
            {
                string content = boundSdt.GetText();

                if (content.Length == 0)
                    boundSdt.Remove();
                else
                    boundSdt.XmlMapping.SetValue(content);
            }
        }

        /// <summary>
        /// Deletes one of special chars.
        /// </summary>
        private void DeleteSpecialChar(int start, char ch)
        {
            Node node = mIndexer.GetNodeByPosition(start);

            switch (ch)
            {
                case FindReplaceIndexer.ParagraphBreakChar:
                    DeleteParagraphBreak(node);
                    break;

                case FindReplaceIndexer.SectionBreakChar:
                    DeleteSectionBreak(node);
                    break;

                case FindReplaceIndexer.CellBreakChar:
                    // Do nothing.
                    break;

                case FindReplaceIndexer.ShapeChar:
                    // Do nothing.
                break;

                default:
                    // Unexpected special character.
                    Debug.Assert(false);
                    break;
            }
        }

        /// <summary>
        /// Deletes paragraph break.
        /// </summary>
        private void DeleteParagraphBreak(Node node)
        {
            // We need to get next paragraph and move all child
            Paragraph current = (Paragraph)node;

            // Just mark the paragraph as deleted if tracking is on.
            if (mDoc.IsTrackRevisionsEnabled)
            {
                // If there is a table after the deleting paragraph, when accepting revisions, MS Word places
                // not deleted text of the paragraph into the table. So, when revision tracking is enabled, the
                // mOptions.SmartParagraphBreakReplacement mode is implicitly used.
                EditSession editSession = mDoc.FetchDocumentOrGlossaryMain().EditSession;
                RevisionTrackingUtil.AddDeleteRevision(current, editSession);
                return;
            }

            // WORDSNET-21329 We introduced a new option that allows to move child nodes into the NextPreOrder paragraph.
            Paragraph nextParagraph = (mOptions.SmartParagraphBreakReplacement)
                ? (Paragraph)current.NextPreOrderOfType(current.Document, NodeType.Paragraph)
                : node.NextNonAnnotationSibling as Paragraph;

            // See Test18. Word also leaves this paragraph break but reports that replace operation is successful.
            if (nextParagraph == null)
                return;

            // Move annotations between paragraphs into the second paragraph.
            Node nextNonAnnotation = node.NextNonAnnotationSibling;
            if (current.NextSibling != nextNonAnnotation)
                nextParagraph.InsertBefore(current.NextSibling, nextNonAnnotation.PreviousSibling, nextParagraph.FirstChild);

            // Move child nodes from the current paragraph to the next one.
            nextParagraph.InsertBefore(current.FirstChild, null, nextParagraph.FirstChild);

            current.Remove();
        }

        /// <summary>
        /// Deletes section break.
        /// </summary>
        private void DeleteSectionBreak(Node node)
        {
            Paragraph sectBreak = (Paragraph)node;

            Section curSect = (Section)sectBreak.GetAncestor(NodeType.Section);
            Section nextSect = (Section)curSect.NextSibling;

            nextSect.Body.InsertBefore(curSect.Body.FirstChild, null, nextSect.Body.FirstChild);
            curSect.Remove();

            // Delete section break paragraph.
            DeleteParagraphBreak(sectBreak);
        }

        private static bool IsSpecialChar(char ch)
        {
            switch (ch)
            {
                case FindReplaceIndexer.ParagraphBreakChar:
                case FindReplaceIndexer.SectionBreakChar:
                case FindReplaceIndexer.CellBreakChar:
                case FindReplaceIndexer.ShapeChar:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates a regex pattern for a simple text search string.
        /// </summary>
        private static Regex TextToRegex(string what, FindReplaceOptions options)
        {
            string pattern = what;

            pattern = Regex.Escape(pattern);

            // WORDSNET-22269 Consider curly and double quotes as equal characters to mimic MS Word.
            pattern = pattern.Replace("\"", "[\"“”]");

            RegexOptions regexOptions = (options.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;
            return new Regex(pattern, regexOptions);
        }

        private static string ReplaceMetaCharacters(string text)
        {
            bool amp = false;
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    case '&':
                        if (amp) sb.Append('&');
                        amp = !amp;
                        break;

                    case 'b':
                    case 'p':
                    case 'm':
                    case 'l':
                        sb.Append(amp ? GetMetaChar(c) : c);
                        amp = false;
                        break;

                    default:
                        if (amp) sb.Append('&');
                        sb.Append(c);
                        amp = false;
                        break;
                }
            }

            // Add trailing ampersand.
            if (amp) sb.Append('&');

            return sb.ToString();
        }

        /// <summary>
        /// Returns meta character replacement.
        /// </summary>
        private static char GetMetaChar(char c)
        {
            switch (c)
            {
                case 'b':
                    return FindReplaceIndexer.SectionBreakChar;
                case 'p':
                    return FindReplaceIndexer.ParagraphBreakChar;
                case 'm':
                    return ControlChar.PageBreakChar;
                case 'l':
                    return ControlChar.LineBreakChar;
                case '&':
                    return '&'; // & is meta for itself
                default:
                    return c;
            }
        }

        /// <summary>
        /// Represents a comparer to sort headers/footers in order of their visual representation in a document.
        /// </summary>
        private class HeaderFooterComparer : IComparer<Node>
        {
            /// <summary>
            /// Ctor with an option defined whether separate first header and footer are present.
            /// </summary>
            internal HeaderFooterComparer(bool differentFirstPageHeaderFooter)
            {
                mDifferentFirstPageHeaderFooter = differentFirstPageHeaderFooter;
            }

            /// <summary>
            /// Compares two headers/footers.
            /// </summary>
            public int Compare(Node x, Node y)
            {
                HeaderFooter headerFooterX = x as HeaderFooter;
                HeaderFooter headerFooterY = y as HeaderFooter;
                if ((headerFooterX == null) || (headerFooterY == null))
                    throw new InvalidOperationException("Wrong object type.");

                return GetOrder(headerFooterX) - GetOrder(headerFooterY);
            }

            /// <summary>
            /// Gets order index for the specified header/footer.
            /// </summary>
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            private int GetOrder(HeaderFooter headerFooter)
            {
                switch (headerFooter.HeaderFooterType)
                {
                    case HeaderFooterType.HeaderFirst:
                        return 0;
                    case HeaderFooterType.FooterFirst:
                        return 1;
                    case HeaderFooterType.HeaderEven:
                        return 2;
                    case HeaderFooterType.FooterEven:
                        return 3;
                    case HeaderFooterType.HeaderPrimary:
                        return mDifferentFirstPageHeaderFooter ? 4 : 0;
                    case HeaderFooterType.FooterPrimary:
                        return mDifferentFirstPageHeaderFooter ? 5 : 1;
                    default:
                        throw new InvalidOperationException("Unknown type of the header or footer.");
                }
            }

            private readonly bool mDifferentFirstPageHeaderFooter;
        }

        /// <summary>
        /// Gets a replacing callback method.
        /// </summary>
        private IReplacingCallback ReplacingCallback
        {
            get { return (mOptions != null) ? mOptions.ReplacingCallback : null; }
        }

        /// <summary>
        /// Gets a boolean value indicating either to move forward.
        /// </summary>
        private bool  IsForward
        {
            get { return ((mOptions == null) || (mOptions.Direction == FindReplaceDirection.Forward)); }
        }

        /// <summary>
        /// Gets a boolean value indicating either to find a whole words only.
        /// </summary>
        private bool IsFindWholeWordsOnly
        {
            get { return ((mOptions != null) && (mOptions.FindWholeWordsOnly)); }
        }

        /// <summary>
        /// Gets a boolean value indicating either to recognize and use substitutions within replacement patterns.
        /// </summary>
        private bool IsUseSubstitutions
        {
            get { return ((mOptions != null) && (mOptions.UseSubstitutions)); }
        }

        /// <summary>
        /// Gets a boolean value indicating the regex option Multiline is enabled.
        /// </summary>
        private bool IsMultiline
        {
            get { return (((mPattern != null) && (mPattern.Options & RegexOptions.Multiline) != 0)); }
        }

        private FindReplaceIndexer mIndexer;

        private readonly DocumentBase mDoc;
        private readonly Node mRootNode;
        private readonly NodeRange mNodeRange;
        private readonly string mReplaceWith;
        private readonly Regex mPattern;

        private readonly FindReplaceOptions mOptions;
        private const string WordBoundarySymbols = "`~!@#$%^&*()-=+[{]}\\|;:'\",.<>/?\v";
        private static readonly char[] gAnchors = new[] { '^', '$' };

        internal static bool LogSwitch = false;

        /// <summary>
        /// The reference formatting to apply to the inserted content.
        /// </summary>
        private readonly RunPr mRunPr = new RunPr();

        /// <summary>
        /// The reference formatting to apply to the inserted content.
        /// </summary>
        private readonly ParaPr mParaPr = new ParaPr();
    }
}
