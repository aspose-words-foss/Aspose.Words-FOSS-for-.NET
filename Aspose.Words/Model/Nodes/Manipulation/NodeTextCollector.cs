// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2010 by Dmitry Vorobyev

using System.Text;

using Aspose.Words.Fields;
using Aspose.Words.Notes;

namespace Aspose.Words
{
    /// <summary>
    /// Collects text between two nodes. May skip or include field codes.
    /// </summary>
    internal class NodeTextCollector : NodeEnumerator
    {
        private NodeTextCollector(NodeRange range, NodeTextCollectorOptions options)
            : base(range)
        {
            mOptions = options;
        }

        /// <summary>
        /// Collects and returns text of the specified range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static string GetText(NodeRange range)
        {
            return GetText(range, new NodeTextCollectorOptions());
        }

        /// <summary>
        /// Collects and returns text of the specified range.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="isFieldResultMode"></param>
        /// <returns></returns>
        internal static string GetText(NodeRange range, bool isFieldResultMode)
        {
            NodeTextCollectorOptions options = new NodeTextCollectorOptions();
            options.IsFieldResultMode = isFieldResultMode;
            return GetText(range, options);
        }

        /// <summary>
        /// Collects and returns text between the two specified nodes inclusively.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        internal static string GetText(Node start, Node end)
        {
            return GetText(start, true, end, true);
        }

        /// <summary>
        /// Collects and returns text between the two specified nodes inclusively or exclusively.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="isIncludeStart"></param>
        /// <param name="end"></param>
        /// <param name="isIncludeEnd"></param>
        /// <returns></returns>
        internal static string GetText(Node start, bool isIncludeStart, Node end, bool isIncludeEnd)
        {
            return GetText(start, isIncludeStart, end, isIncludeEnd, new NodeTextCollectorOptions());
        }

        /// <summary>
        /// Collects and returns text between the two specified nodes inclusively or exclusively, also allows
        /// to control field results.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="isIncludeStart"></param>
        /// <param name="end"></param>
        /// <param name="isIncludeEnd"></param>
        /// <param name="isFieldResultMode">
        /// True to include field results only, false to include all field parts that includes field code and field result
        /// separated by field characters.
        /// </param>
        /// <returns></returns>
        internal static string GetText(Node start, bool isIncludeStart, Node end, bool isIncludeEnd, bool isFieldResultMode)
        {
            NodeTextCollectorOptions options = new NodeTextCollectorOptions();
            options.IsFieldResultMode = isFieldResultMode;
            return GetText(start, isIncludeStart, end, isIncludeEnd, options);
        }

        /// <summary>
        /// Collects and returns text between the two specified nodes inclusively or exclusively.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="isIncludeStart"></param>
        /// <param name="end"></param>
        /// <param name="isIncludeEnd"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static string GetText(Node start, bool isIncludeStart, Node end, bool isIncludeEnd, NodeTextCollectorOptions options)
        {
            return GetText(new NodeRange(start, isIncludeStart, end, isIncludeEnd), options);
        }

        /// <summary>
        /// Collects and returns text of the specified range.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static string GetText(NodeRange range, NodeTextCollectorOptions options)
        {
            using (NodeTextCollector collector = new NodeTextCollector(range, options))
                return collector.GetText();
        }

        private string GetText()
        {
            while (MoveToNextNode(!SkipCurrentNode()))
            {
                ProcessPossibleFieldChar();

                if (SkipCurrentNode())
                    continue;

                if (!IsLocked)
                {
                    if (CurrentNode.IsComposite)
                    {
                        CompositeNode node = (CompositeNode)CurrentNode;
                        if (!node.HasChildNodes && !IsSkippedStoryEnd(node))
                            mBuilder.Append(node.GetEndText());
                    }
                    else
                    {
                        if (NeedAppendCurrentNodeText())
                            mBuilder.Append(CurrentNode.GetText());
                    }
                }
            }

            return mBuilder.ToString();
        }

        private bool NeedAppendCurrentNodeText()
        {
            Inline inlineNode = CurrentNode as Inline;
            if (inlineNode == null)
                return true;

            if (inlineNode.Font.Hidden && !mOptions.AllowHiddenText)
                return false;

            if (inlineNode.IsDeleteRevision && !mOptions.AllowDeletedText)
                return false;

            if (inlineNode.IsInsertRevision && !mOptions.AllowInsertedText)
                return false;

            return true;
        }

        private bool SkipCurrentNode()
        {
            if (CurrentNode is Comment)
                return mOptions.SkipCommentText;
            if (CurrentNode is Footnote)
                return mOptions.SkipFootnoteText;

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is a story end and its story break char should not be included
        /// into result text.
        /// </summary>
        /// <remarks>
        /// When cross-structure annotations are allowed at block level, the range end node (annotation) may be
        /// in the same story but after the final paragraph. At this case story end control char returned by
        /// <see cref="Paragraph.GetEndText"/> should not be included into the result text.
        /// The same behaviour is with inline annotations: at this case the annotation is moved into the previous
        /// paragraph and the paragraph end is outside of the range.
        /// </remarks>
        private bool IsSkippedStoryEnd(Node node)
        {
            Node endNode = Range.End.Node;
            if ((node.NodeType != NodeType.Paragraph) ||
                !NodeUtil.IsCrossStructureAnnotation(endNode) ||
                (endNode.PreviousNonMarkupCompositeLimited != node))
                return false;

            Paragraph para = (Paragraph)node;
            return para.IsEndOfCell || para.IsEndOfTextbox || para.IsEndOfNoteSeparator || para.IsEndOfFootnote ||
                para.IsEndOfComment || para.IsEndOfSection || para.IsEndOfHeaderFooter;
        }

        protected override void OnMoved(DocumentPositionMovement movement)
        {
            if ((movement == DocumentPositionMovement.Above) && !IsSkippedStoryEnd(CurrentNode))
                mBuilder.Append(((CompositeNode)CurrentNode).GetEndText());
        }

        private void ProcessPossibleFieldChar()
        {
            if (!mOptions.IsFieldResultMode && !IsHiddenField)
                return;

            switch (CurrentNode.NodeType)
            {
                case NodeType.FieldStart:
                    ProcessFieldCodeStart();
                    break;
                case NodeType.FieldSeparator:
                    // WORDSNET-8937 Do not unlock an invisible field result.
                    if (!FieldUtil.IsFieldResultInvisible((FieldSeparator)CurrentNode))
                        ProcessFieldCodeEnd();
                    break;
                case NodeType.FieldEnd:
                    // If the field has separator, the output was already unlocked.
                    // WORDSNET-8937 Unlock an invisible field result here.
                    FieldEnd fieldEnd = (FieldEnd)CurrentNode;
                    if (!fieldEnd.HasSeparator || FieldUtil.IsFieldResultInvisible(fieldEnd))
                        ProcessFieldCodeEnd();
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private void ProcessFieldCodeStart()
        {
            mLockCount++;
        }

        private void ProcessFieldCodeEnd()
        {
            if (mLockCount > 0)
            {
                // Do not unlock if nothing is locked.
                mLockCount--;
            }
            else
            {
                // HACK We may deal with a bookmark node range, which starts inside a field code,
                // so we need not to collect the field code's text in this case.
                mBuilder.Length = 0;
            }
        }

        private bool IsLocked
        {
            get
            {
                if (mLockCount > 0)
                    return true;

                switch (CurrentNode.NodeType)
                {
                    case NodeType.FieldStart:
                    case NodeType.FieldSeparator:
                    case NodeType.FieldEnd:
                        // When in field result mode, do not include field control characters.
                        return mOptions.IsFieldResultMode || IsHiddenField;
                    default:
                        return false;
                }
            }
        }

        private bool IsHiddenField
        {
            get
            {
                FieldChar fieldChar = CurrentNode as FieldChar;
                if (fieldChar == null)
                    return false;

                return FieldUtil.IsDead(fieldChar.FieldType);
            }
        }

        private readonly NodeTextCollectorOptions mOptions;
        private readonly StringBuilder mBuilder = new StringBuilder();
        /// <summary>
        /// Greater than zero indicates the document content is being skipped.
        /// </summary>
        private int mLockCount;
    }
}
