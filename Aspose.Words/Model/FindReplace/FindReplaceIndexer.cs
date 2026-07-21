// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2016 by Alexey Morozov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Replacing
{
    /// <summary>
    /// Indexes nodes for find/replace operations.
    /// </summary>
    /// <remarks>
    /// AM. Currently ignores few node types to simplify replace operations.
    /// </remarks>
    internal class FindReplaceIndexer : NodeIndexer
    {
        internal FindReplaceIndexer(Node rootNode, FindReplaceOptions options) : base(rootNode)
        {
            mOptions = options;
            Update();
        }

        internal FindReplaceIndexer(NodeRange nodeRange, Node rootNode, FindReplaceOptions options) : base(nodeRange, rootNode)
        {
            mOptions = options;
            Update();
        }

        internal override IndexerAction OnNode(Node node)
        {
            // WORDSNET-18986 Removed SmartTag from ignore.
            switch (node.NodeType)
            {
                case NodeType.Table:
                case NodeType.Row:
                    return IndexerAction.Skip;

                case NodeType.OfficeMath:
                {
                    // WORDSNET-28249 Added option to consider text in OfficeMath.
                    return ((mOptions == null) || mOptions.IgnoreOfficeMath) ? IndexerAction.Ignore : IndexerAction.None;
                }

                case NodeType.SpecialChar:
                case NodeType.FormField:
                    return IndexerAction.Ignore;

                case NodeType.Comment:
                case NodeType.HeaderFooter:
                {
                    if (node != RootNode)
                    {
                        PendingList.Add(node);
                        return IndexerAction.Ignore;
                    }
                    break;
                }

                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                {
                    mFieldNodes.Push(node);
                    return IndexerAction.Ignore;
                }

                case NodeType.FieldEnd:
                {
                    // WORDSNET-23658 There can be situation when customer wants to replace inside some isolated Paragraph
                    // and this paragraph contains only FieldEnd, and its FieldStart is located inside another Paragraph.
                    // In this case we are not actually inside a Field and should check it for resilience.
                    if (IsInField)
                    {
                        Node fieldNode = mFieldNodes.Pop();
                        if ((mFieldNodes.Count > 0) && (fieldNode.NodeType == NodeType.FieldSeparator))
                            mFieldNodes.Pop();
                    }

                    return IndexerAction.Ignore;
                }

                case NodeType.Shape:
                    // In the legacy Find/Replace behavior, text boxes should be processed
                    // in the context of the document content, and not separately from it.
                    if ((node != RootNode) && !(mOptions.UseLegacyOrder && ((Shape)node).HasTextbox))
                    {
                        PendingList.Add(node);
                        // WORDSNET-24701 Do not ignore shapes to mimic Word behavior.
                        // WORDSNET-25115 Introduced an option to control over the behavior.
                        return mOptions.IgnoreShapes ? IndexerAction.Ignore : IndexerAction.Collapse;
                    }

                    break;

                case NodeType.StructuredDocumentTag:
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                    if(!IsSupportedSdtLevel(sdt.Level))
                        return IndexerAction.Skip;

                    // WORDSNET-24241 Added new option to control the behavior.
                    if ((!mOptions.IgnoreStructuredDocumentTags) && (node != RootNode))
                    {
                        PendingList.Add(node);
                        return IndexerAction.Ignore;
                    }

                    break;
                }

                case NodeType.Footnote:
                    return mOptions.IgnoreFootnotes
                        ? IndexerAction.Ignore
                        : IndexerAction.None;

                default:
                    break;
            }

            // WORDSNET-20184 Check options after all node types are processed.
            // Otherwise, some data may be never collected (for example, mFieldCharsCount will be never adjusted).
            return VerifyOptions(node);
        }

        /// <summary>
        /// Returns text for a specified node.
        /// </summary>
        protected override string GetText(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Shape:
                case NodeType.GroupShape:
                    return gShape;

                default:
                    return base.GetText(node);
            }
        }

        /// <summary>
        /// Verifies underlying FindReplace options for a specified node.
        /// </summary>
        private IndexerAction VerifyOptions(Node node)
        {
            if (mOptions != null)
            {
                // WORDSNET-19912 New option IgnoreFields is introduced.
                // WORDSNET-22955 Changed action from 'Ignore' to 'Skip' to allow indexer find 'FieldEnd' correctly.
                if (mOptions.IgnoreFields && IsInField)
                    return IndexerAction.Skip;

                // WORDSNET-22686 New option IgnoreFieldCodes is introduced.
                if (mOptions.IgnoreFieldCodes && IsInFieldCode)
                    return IndexerAction.Skip;

                // WORDSNET-19912 New option IgnoreDeleted is introduced.
                // WORDSNET-22632 Lets do 'Skip' instead of 'Ignore' for this option
                // to process child nodes of deleted composite nodes properly.
                if (mOptions.IgnoreDeleted && (node is ITrackableNode) && (((ITrackableNode)node).DeleteRevision != null))
                    return IndexerAction.Skip;

                // WORDSNET-19912 New option IgnoreInserted is introduced.
                // WORDSNET-22632 Lets do 'Skip' instead of 'Ignore' for this option
                // to process child nodes of inserted composite nodes properly.
                if (mOptions.IgnoreInserted && (node is ITrackableNode) && (((ITrackableNode)node).InsertRevision != null))
                    return IndexerAction.Skip;
            }

            return IndexerAction.None;
        }

        /// <summary>
        /// Indicates that given markup level is supported for find/replace operation.
        /// </summary>
        private static bool IsSupportedSdtLevel(MarkupLevel level)
        {
            return (level == MarkupLevel.Inline) || (level == MarkupLevel.Block);
        }

        protected override string GetEndText(CompositeNode node)
        {
            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                {
                    Paragraph para = (Paragraph)node;

                    if (para.IsEndOfSection)
                    {
                        // Make section break for sections and eof for the last section.
                        return para.ParentSection.IsLastChild ? gParagraphBreak : gSectionBreak;
                    }

                    if (para.IsEndOfCell)
                    {
                        return gCellBreak;
                    }

                    return gParagraphBreak;
                }

                case NodeType.Shape:
                case NodeType.GroupShape:
                {
                    return gShape;
                }

                default:
                    return node.GetEndText();
            }
        }

        /// <summary>
        /// Adds index for a specified node.
        /// </summary>
        protected override void Add(int position, Node node, string text)
        {
            base.Add(position, node, text);

            // WORDSNET-22814 We now also collect positions of LineBreak('\v') and PageBreak('\f') characters.
            int specialIndex = text.IndexOfAny(gSpecialChars, 0);
            while (specialIndex != -1)
            {
                SpecialPositions.Add(specialIndex + position);
                specialIndex = text.IndexOfAny(gSpecialChars, specialIndex + 1);
            }
        }

        /// <summary>
        /// List of nodes that ignored and pended for second stage of find/replace operation.
        /// </summary>
        internal List<Node> PendingList
        {
            get { return mPendingList; }
        }

        /// <summary>
        /// Gets a boolean value indicating that current node is inside a field.
        /// </summary>
        private bool IsInField
        {
            get { return (mFieldNodes.Count > 0); }
        }

        /// <summary>
        /// Gets a boolean value indicating that current node is inside a field's Code.
        /// </summary>
        private bool IsInFieldCode
        {
            get { return (IsInField && (mFieldNodes.Peek().NodeType == NodeType.FieldStart)); }
        }

        /// <summary>
        /// A list of positions of special characters within indexed text.
        /// </summary>
        internal IntList SpecialPositions
        {
            get { return mSpecialPositions; }
        }

        /// <summary>
        /// Substitute char for paragraph break.
        /// </summary>
        [CppConstexpr]
        internal const char ParagraphBreakChar = (char)0xe00d;
        private static readonly string gParagraphBreak = ParagraphBreakChar.ToString();

        /// <summary>
        /// Substitute char for section break.
        /// </summary>
        [CppConstexpr]
        internal const char SectionBreakChar = (char)0xe00f;
        private static readonly string gSectionBreak = SectionBreakChar.ToString();

        /// <summary>
        /// Substitute char for cell break.
        /// </summary>
        [CppConstexpr]
        internal const char CellBreakChar = (char)0xe010;
        private static readonly string gCellBreak = CellBreakChar.ToString();

        /// <summary>
        /// Substitute char for Shape node.
        /// </summary>
        [CppConstexpr]
        internal const char ShapeChar = (char)0xe019;
        private static readonly string gShape = ShapeChar.ToString();

        private readonly List<Node> mPendingList = new List<Node>();
        private readonly FindReplaceOptions mOptions;
        private readonly Stack<Node> mFieldNodes = new Stack<Node>();
        private readonly IntList mSpecialPositions = new IntList();

        /// <summary>
        /// The array of all special characters.
        /// </summary>
        private static readonly char[] gSpecialChars = new[]
        {
            ParagraphBreakChar, SectionBreakChar, CellBreakChar, ControlChar.LineBreakChar, ControlChar.PageBreakChar
        };
    }
}
