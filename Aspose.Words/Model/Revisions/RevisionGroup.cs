// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2016 by Alexey Morozov

using System.Collections.Generic;
using System.Text;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a group of sequential <see cref="Aspose.Words.Revision" /> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/track-changes-in-a-document/">Track Changes in a Document</a> documentation article.</para>
    /// </summary>
    public class RevisionGroup
    {
        internal RevisionGroup(List<Node> nodes, EditRevision revision)
        {
            mRevision = new EditRevision(revision.Type, revision.Author, revision.DateTime);
            mNodes = nodes;
        }

        internal RevisionGroup(List<Node> nodes, FormatRevision revision)
        {
            mRevision = new FormatRevision(revision.RevPr.Clone(), revision.Author, revision.DateTime);
            mNodes = nodes;
        }

        internal RevisionGroup(Style style)
        {
            // Look for any format revision in style.
            FormatRevision revision = style.RunPr.HasRevisions ? style.RunPr.FormatRevision : style.ParaPr.FormatRevision;
            Debug.Assert(revision != null);

            // Use mRevision to hold revision details.
            mRevision = new FormatRevision(new RunPr(), revision.Author, revision.DateTime);

            mStyle = style;
            mNodes = new List<Node> { style.Document };
        }

        /// <summary>
        /// Ctor for a move revision.
        /// </summary>
        internal RevisionGroup(List<Node> nodes, MoveRevision revision, string moveRangeName)
        {
            mRevision = new MoveRevision(revision.Type, revision.Author, revision.DateTime);
            mNodes = nodes;
            MoveRangeName = moveRangeName;
        }

        /// <summary>
        /// Returns inserted/deleted/moved text or description of format change.
        /// </summary>
        public string Text
        {
            get
            {
                // Table formatted
                if ((Nodes.Count == 1) && Nodes[0].NodeType == NodeType.Table)
                    return "Table";

                if (mStyle != null)
                {
                    string runText = mStyle.RunPr.HasRevisions
                        ? FormatRevisionText.GetText(mStyle.RunPr.FormatRevision, mStyle.Document)
                        : "";

                    string paraText = mStyle.ParaPr.HasRevisions
                        ? FormatRevisionText.GetText(mStyle.ParaPr.FormatRevision, mStyle.Document)
                        : "";

                    if (mStyle.Type == StyleType.Paragraph)
                        return string.Format("{0}: {1}{2}{3}",
                            mStyle.Name, runText, string.IsNullOrEmpty(paraText) ? "" : ", ", paraText);

                    if (mStyle.Type == StyleType.Character)
                        return string.Format("{0}: {1}",
                            mStyle.Name, runText);

                    // Return empty text for other style types for a while.
                    return "";
                }

                return FormatRevision != null ? FormatRevisionText.GetText(FormatRevision, Start.Document) : VisibleText;
            }
        }

        /// <summary>
        /// Gets the author of this revision group.
        /// </summary>
        public string Author
        {
            get { return mRevision.Author; }
        }

        /// <summary>
        /// Gets the type of revisions included in this group.
        /// </summary>
        public RevisionType RevisionType
        {
            get
            {
                if (mStyle != null)
                    return RevisionType.StyleDefinitionChange;

                if (mRevision is EditRevision)
                {
                    switch (((EditRevision)mRevision).Type)
                    {
                        case EditRevisionType.Insertion:
                            return RevisionType.Insertion;
                        case EditRevisionType.Deletion:
                            return RevisionType.Deletion;
                        default:
                            Debug.Assert(false, "Unknown editing revision type.");
                            return RevisionType.Insertion;
                    }
                }

                if (mRevision is FormatRevision)
                {
                    return RevisionType.FormatChange;
                }

                if (mRevision is MoveRevision)
                {
                    return RevisionType.Moving;
                }

                {
                    Debug.Assert(false, "Unknown revision type.");
                    return RevisionType.FormatChange;
                }
            }
        }

        /// <summary>
        /// First node of the revision group.
        /// </summary>
        internal Node Start
        {
            get { return mNodes[0]; }
        }

        /// <summary>
        /// Collection of visible nodes.
        /// </summary>
        /// <remarks>
        /// In some cases Word doesn't show all nodes in revision group.
        /// </remarks>
        internal List<Node> VisibleNodes
        {
            get
            {
                if (mVisibleNodes == null)
                {
                    mVisibleNodes = new List<Node>();

                    foreach(Node node in mNodes)
                    {
                        if (node.NodeType == NodeType.Paragraph)
                        {
                            Paragraph p = (Paragraph)node;

                            // Special handling for tables. Word stops collecting nodes at end of first cell.
                            if (p.IsEndOfCell)
                            {
                                // If cell is empty collect cell break node.
                                if (mNodes[0] == node)
                                    mVisibleNodes.Add(node);
                                break;
                            }
                        }

                        // WORDSNET-16209 Skip SDTs.
                        if (node.NodeType != NodeType.StructuredDocumentTag)
                            mVisibleNodes.Add(node);
                    }
                }

                return mVisibleNodes;
            }
        }

        internal string VisibleText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Node node in VisibleNodes)
                    sb.Append(node.IsComposite ? ((CompositeNode)node).GetEndText() : node.GetText());

                return sb.ToString();
            }
        }

        internal EditRevision EditRevision
        {
            get { return mRevision as EditRevision; }
        }

        /// <summary>
        /// Returns the move revision if this object represents a move group.
        /// </summary>
        internal MoveRevision MoveRevision
        {
            get { return mRevision as MoveRevision; }
        }

        internal FormatRevision FormatRevision
        {
            get { return mRevision as FormatRevision; }
        }

        internal RevisionBase Revision
        {
            get { return mRevision; }
        }

        internal List<Node> Nodes
        {
            get { return mNodes; }
        }

        /// <summary>
        /// Gets name of a move range if this object is a move revision group.
        /// </summary>
        /// <remarks>
        /// The property is used to find a corresponding move-from or move-to group.
        /// </remarks>
        internal string MoveRangeName { get; }

        internal bool IsFontNameRevision
        {
            get { return (FormatRevision != null) && FormatRevision.RevPr.ContainsAnyKey(gFontNameKeys); }
        }

        /// <summary>
        /// Returns an indication of which attribute classes attributes recorded (non-inherited) in format revision belong to.
        /// </summary>
        internal int GetFormatRevisionAttributeAffiliations()
        {
            int result = 0;
            if (FormatRevision != null)
            {
                int[] keys = FormatRevision.RevPr.GetKeys();
                foreach (int key in keys)
                {
                    if (result == (HasTableAttr | HasCellAttr | HasSectAttr | HasParaAttr | HasFontAttr))
                        break;
                    if (key < 1000)
                        result |= HasFontAttr;
                    else if (key < 2000)
                        result |= HasParaAttr;
                    else if (key < 3000)
                        result |= HasSectAttr;
                    else if (key < 4000)
                        result |= HasCellAttr;
                    else if (key < 5000)
                        result |= HasTableAttr;
                }
            }
            return result;
        }

        internal const int HasFontAttr = 1 << 0;
        private const int HasParaAttr = 1 << 1;
        private const int HasSectAttr = 1 << 2;
        private const int HasCellAttr = 1 << 3;
        private const int HasTableAttr = 1 << 4;

#if DEBUG && !CPLUSPLUS
        public override string ToString()
        {
            return string.Format("{0} '{1}'", SymbolicRevisionType, Text);
        }

        private string SymbolicRevisionType
        {
            get
            {
                if (mStyle != null)
                    return "#";

                if (mRevision is FormatRevision)
                    return "*";

                if(mRevision is EditRevision)
                    return (((EditRevision)mRevision).Type == EditRevisionType.Deletion) ? "-" : "+";

                return "?";
            }
        }

#endif

        private readonly Style mStyle;
        private readonly RevisionBase mRevision;
        private readonly List<Node> mNodes;
        private List<Node> mVisibleNodes;
        private static readonly int[] gFontNameKeys = {FontAttr.NameAscii, FontAttr.NameBi, FontAttr.NameFarEast, FontAttr.NameOther };
    }
}
