// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/09/2012 by Alexey Butalov

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Fields;

namespace Aspose.Words
{
    /// <summary>
    /// Provides methods to build new documents based on a specified <see cref="Range" />s.
    /// </summary>
    internal class RangeDocumentBuilder
    {
        /// <summary>
        /// Builds a new document that contains a specified range.
        /// Clones and inserts source range's nodes to a new document. Doesn’t change the source document.
        /// </summary>
        /// <returns>New document that contains a specified range.</returns>
        internal static Document Build(Range range)
        {
            Debug.Assert(range != null);

            Node node = range.Node;
            Document doc;
            switch (node.NodeType)
            {
                case NodeType.Document:
                    doc = (Document) node.Clone(true);
                    break;
                case NodeType.GlossaryDocument:
                    doc = CreateDocument((GlossaryDocument) node, true);
                    break;
                case NodeType.Any:
                case NodeType.Section:
                case NodeType.Body:
                case NodeType.HeaderFooter:
                case NodeType.Table:
                case NodeType.Row:
                case NodeType.Cell:
                case NodeType.Paragraph:
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.GroupShape:
                case NodeType.Shape:
                case NodeType.Comment:
                case NodeType.Footnote:
                case NodeType.Run:
                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                case NodeType.FieldEnd:
                case NodeType.FormField:
                case NodeType.SpecialChar:
                case NodeType.SmartTag:
                case NodeType.StructuredDocumentTag:
                case NodeType.BuildingBlock:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.OfficeMath:
                case NodeType.SubDocument:
                case NodeType.System:
                case NodeType.Null:
                    doc = CreateDocumentFromNode(node);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return doc;
        }

        /// <summary>
        /// Creates a new document from a GlossaryDocument object.
        /// </summary>
        /// <param name="glossaryDocument">Source GlossaryDocument object.</param>
        /// <param name="importGlossaryChildren">true if you need to import children nodes of a GlossaryDocument object to a destination document; otherwise, false.</param>
        /// <returns></returns>
        private static Document CreateDocument(GlossaryDocument glossaryDocument, bool importGlossaryChildren)
        {
            Debug.Assert(glossaryDocument != null);
            Document doc = new Document(DocumentCtorMode.BlankDocumentNode);
            doc.GlossaryDocument = (GlossaryDocument) glossaryDocument.Clone(importGlossaryChildren);
            return doc;
        }

        /// <summary>
        /// Creates a new document from a Node object.
        /// </summary>
        /// <param name="node">Source node.</param>
        /// <returns>New document.</returns>
        private static Document CreateDocumentFromNode(Node node)
        {
            Debug.Assert(node != null);
            Debug.Assert(node.Document != null);

            Document doc;
            switch (node.Document.NodeType)
            {
                case NodeType.Document:
                {
                    doc = (Document)node.Document.Clone(false);
                    ImportSourceNode(doc, node);
                    break;
                }
                case NodeType.GlossaryDocument:
                {
                    doc = CreateDocument((GlossaryDocument)node.Document, false);
                    ImportSourceNode(doc.GlossaryDocument, node);
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unsupported document type!");
                    doc = null;
                    break;
                }
            }
            return doc;
        }

        /// <summary>
        /// Imports a source document node to a destination document.
        /// </summary>
        private static void ImportSourceNode(DocumentBase destDoc, Node sourceNode)
        {
            Debug.Assert(destDoc != null);
            Debug.Assert(sourceNode != null);
            Debug.Assert(sourceNode.Document != null);

            HashSetGeneric<NodeType> referNodeTypes = new HashSetGeneric<NodeType>();
            referNodeTypes.Add(NodeType.HeaderFooter);
            Stack<Node> srcNodesHierarchy = GetNodesHierarchyStack(sourceNode, referNodeTypes);
            CompositeNode destParentNode = destDoc;
            while (srcNodesHierarchy.Count != 0)
            {
                Node srcNode = srcNodesHierarchy.Pop();
                Node destNode = ImportNodeToDocument(destDoc, srcNode, srcNode == sourceNode, destParentNode);
                destParentNode = (destNode.IsComposite) ? (CompositeNode) destNode : null;
            }
        }

        /// <summary>
        /// Imports a source node to a destination document.
        /// </summary>
        /// <param name="destDoc">Destination document for importing node to.</param>
        /// <param name="srcNode">Source node to import.</param>
        /// <param name="initialSrcNode">true if a source node is an initial node for import; otherwise, false.</param>
        /// <param name="parentDestNode">Destination document parent node. A new node is added to children nodes of this node.</param>
        /// <returns>Imported node.</returns>
        private static Node ImportNodeToDocument(DocumentBase destDoc, Node srcNode, bool initialSrcNode, CompositeNode parentDestNode)
        {
            Debug.Assert(parentDestNode != null);
            Node destNode;
            if (srcNode.NodeType == NodeType.FormField)
            {
                // Form field is a complex node. Import it in particular manner.
                destNode = ImportFormFieldNodeToDocument(destDoc, (FormField)srcNode, parentDestNode);
            }
            else
            {
                // Imports child nodes of the initial source node only.
                destNode = destDoc.ImportNode(srcNode, initialSrcNode, ImportFormatMode.KeepSourceFormatting);
                parentDestNode.AppendChild(destNode);
            }

            return destNode;
        }

        /// <summary>
        /// Imports a source form field to a destination document.
        /// </summary>
        /// <param name="destDoc">Destination document.</param>
        /// <param name="srcFieldNode">Source form field node.</param>
        /// <param name="parentDestNode">Destination document parent node. A new node is added to children nodes of this node.</param>
        /// <returns></returns>
        private static Node ImportFormFieldNodeToDocument(DocumentBase destDoc, FormField srcFieldNode, CompositeNode parentDestNode)
        {
            // FormField consists of FieldStart, FieldEnd, FieldSeparator, FormField, BoormarkStart, BoormarkEnd and Run nodes.
            Node srcStartNode = srcFieldNode.Field.Start;
            BookmarkStart bookmarkStart = srcFieldNode.BookmarkStart;
            BookmarkEnd bookmarkEnd = null;
            if (bookmarkStart != null)
            {
                Bookmark bookmark = new Bookmark(bookmarkStart);
                bookmarkEnd = bookmark.BookmarkEnd;
            }

            bool isBookmarkStartCopied = false;
            bool isBookmarkEndCopied = false;
            Node destFieldNode = null;
            Node srcNode = srcStartNode;

            while ((srcNode != null) && (srcNode.NodeType != NodeType.FieldEnd))
            {
                Node destNode = destDoc.ImportNode(srcNode, false);
                parentDestNode.AppendChild(destNode);

                if (srcNode == srcFieldNode)
                    destFieldNode = destNode;

                isBookmarkStartCopied = isBookmarkStartCopied || (srcNode == bookmarkStart);
                isBookmarkEndCopied = isBookmarkEndCopied || (srcNode == bookmarkEnd);

                srcNode = srcNode.NextSibling;
            }

            if (srcNode != null)
                parentDestNode.AppendChild(destDoc.ImportNode(srcNode, true));

            if ((bookmarkStart != null) && !isBookmarkStartCopied)
            {
                Node destBookmarkStart = destDoc.ImportNode(bookmarkStart, false);
                // Mimic MS Word in placing bookmark nodes located before a field.
                if (parentDestNode.ParentNode.CanInsert(destBookmarkStart))
                    parentDestNode.InsertPrevious(destBookmarkStart);
                else
                    parentDestNode.InsertBefore(destBookmarkStart, destFieldNode);
            }

            if ((bookmarkEnd != null) && !isBookmarkEndCopied)
                parentDestNode.AppendChild(destDoc.ImportNode(bookmarkEnd, false));

            Debug.Assert(destFieldNode != null);
            return destFieldNode;
        }

        /// <summary>
        /// Gets source document nodes hierarchy for import. Skips all unnecessary nodes.
        /// Can process Document and GlossaryDocument child nodes.
        /// </summary>
        /// <remarks>
        /// To build a correct document we use a minimal nodes hierarchy.
        /// The minimal hierarchy consist of:
        /// [BuildingBlock]-Section-{Body|HeaderFooter}-FirstBlockLevelNode-[....]-SourceNode
        ///   FirstBlockLevelNode - first source node's ancestor of a block level type.
        /// All intermediate nodes are skipped between {Body|HeaderFooter} and FirstBlockLevelNode nodes.
        /// </remarks>
        private static Stack<Node> GetNodesHierarchyStack(Node sourceNode, HashSetGeneric<NodeType> referNodeTypes)
        {
            Debug.Assert(sourceNode != null);

            Stack<Node> nodesHierarchy = new Stack<Node>();
            Node node = sourceNode;
            referNodeTypes.Add(NodeType.Body);
            while ((node != null) && (node.NodeType != NodeType.Document) && (node.NodeType != NodeType.GlossaryDocument))
            {
                nodesHierarchy.Push(node);
                if (node.NodeLevel == NodeLevel.Block)
                {
                    // Skip all intermediate nodes between the found block-level node and nodes with the specified NodeType.
                    do
                        node = node.ParentNode;
                    while ((node != null) && (!referNodeTypes.Contains(node.NodeType)));
                }
                else
                {
                    node = node.ParentNode;
                }
            }

            return nodesHierarchy;
        }
    }
}
