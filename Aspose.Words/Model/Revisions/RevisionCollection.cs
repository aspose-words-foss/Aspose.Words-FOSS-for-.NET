// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2012 by Denis Darkin

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="Revision"/> objects that represent revisions in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/track-changes-in-a-document/">Track Changes in a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class directly. Use the <see cref="Document.Revisions"/> property to get revisions present in a document.</para>
    /// </remarks>
    public class RevisionCollection : IEnumerable<Revision>
    {
        internal RevisionCollection(Node node)
        {
            if (node.NodeType != NodeType.Document)
            {
                mDocument = node.Document;
                mParentNode = node;
            }
            else
            {
                mDocument = (DocumentBase)node;
            }

            CollectRevisions();
        }

        internal RevisionCollection(NodeRange nodeRange)
        {
            mDocument = nodeRange.Document;
            mNodeRange = nodeRange;
            CollectRevisions();
        }

        /// <summary>
        /// Accepts all revisions in this collection.
        /// </summary>
        public void AcceptAll()
        {
            Accept(DefaultRevisionCriteria.Instance);
        }

        /// <summary>
        /// Rejects all revisions in this collection.
        /// </summary>
        public void RejectAll()
        {
            Reject(DefaultRevisionCriteria.Instance);
        }

        /// <summary>
        /// Accepts revisions that match specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// The <see cref="IRevisionCriteria"/> implementation.
        /// </param>
        /// <returns>
        /// The count of accepted revisions.
        /// </returns>
        public int Accept(IRevisionCriteria criteria)
        {
            ArgumentUtil.CheckNotNull(criteria, "filter");
            return HandleRevisions(criteria, true);
        }

        /// <summary>
        /// Rejects revisions that match specified criteria.
        /// </summary>
        /// <param name="criteria">
        /// The <see cref="IRevisionCriteria"/> implementation.
        /// </param>
        /// <returns>
        /// The count of rejected revisions.
        /// </returns>
        public int Reject(IRevisionCriteria criteria)
        {
            ArgumentUtil.CheckNotNull(criteria, "filter");
            return HandleRevisions(criteria, false);
        }

        private int HandleRevisions(IRevisionCriteria criteria, bool isAcceptance)
        {
            // Nodes should be actually inserted/deleted here so suspend tracking during this operation.
            try
            {
                using (new SuspendTrackRevisionsDocument(mDocument))
                {
                    // WORDSNET-8209 Use List of revisions to prevent changes of collection in the loop.
                    IList<Revision> revisionsToHandle = ToList(criteria);

                    // Accumulates runs, paragraphs, rows, cells to delete since we cannot delete them
                    // while visiting document nodes.
                    RevisionHandlingContext context = new RevisionHandlingContext(revisionsToHandle, isAcceptance);

                    foreach (Revision revision in revisionsToHandle)
                        revision.HandleRevisions(false, context);

                    BookmarkDeleter.DeleteBookmarksFromDeletingNodes(mDocument, context);

                    using (new SuspendMappedCustomXmlUpdateDocument(mDocument))
                        RevisionUtil.ProcessDelayedNodes(context, mDocument);
                    InvalidateAll();

                    return revisionsToHandle.Count;
                }
            }
            finally
            {
                ChangeCount++;
            }
        }

        /// <summary>
        /// Returns the number of revisions in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                CheckInvalidate();
                return mNodeRevisions.Count + mStyleRevisions.Count;
            }
        }

        /// <summary>
        /// Returns the number of empty revisions in the collection.
        /// If the document has changed since last time the collection was accessed then refreshing of the collection
        /// is not implementing.
        /// Used for testing purposes only.
        /// </summary>
        internal int EmptyFormatRevisionsCount
        {
            get { return mEmptyFormatRevisions.Count; }
        }

        /// <summary>
        /// Returns a <see cref="Revision"/> at the specified index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public Revision this[int index]
        {
            get
            {
                CheckInvalidate();

                if (index < mNodeRevisions.Count)
                    return mNodeRevisions[index];

                int styleIndex = index - mNodeRevisions.Count;
                if (styleIndex < mStyleRevisions.Count)
                    return mStyleRevisions[styleIndex];

                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>
        /// Invalidates collection of nodes after accept/reject procedures. Used to keep live state of revision collection.
        /// </summary>
        internal void Invalidate(Revision revision)
        {
            Debug.Assert(revision != null);

            if (revision.RevisionType != RevisionType.StyleDefinitionChange)
                mNodeRevisions.Remove(revision);
            else
                mStyleRevisions.Remove(revision);

            ChangeCount++;
        }

        private void InvalidateAll()
        {
            mNodeRevisions.Clear();
            mStyleRevisions.Clear();
            mEmptyFormatRevisions.Clear();

            mRevisionGroups = null;

            ChangeCount++;
        }

        /// <summary>
        /// Tries to find revision with given Istd from input style and removes style this revision from the collection if found.
        /// </summary>
        internal void Remove(Style style)
        {
            Revision foundStyleRevision = null;
            foreach (Revision styleRevision in mStyleRevisions)
            {
                if (styleRevision.ParentStyle.Istd == style.Istd)
                {
                    foundStyleRevision = styleRevision;
                    break;
                }
            }

            if (foundStyleRevision != null)
            {
                mStyleRevisions.Remove(foundStyleRevision);
                ChangeCount++;
            }
        }

        /// <summary>
        /// Adds all revisions applied to style to the style revision array.
        /// </summary>
        internal void AddStyleRevisions(Style style)
        {
            if (style.RunPr.HasFormatRevision)
                AddStyleRevision(mStyleRevisions, style.RunPr.FormatRevision, style, true);

            if (style.ParaPr.HasFormatRevision)
                AddStyleRevision(mStyleRevisions, style.ParaPr.FormatRevision, style, true);
        }

        /// <summary>
        /// Loops through all revision groups and finds thd revision group by revision.
        /// </summary>
        /// <returns>
        /// Returns the revision group or null if the revision group is not found.
        /// See also <see cref="Revision.Group"/>.
        /// </returns>
        internal RevisionGroup GetGroupByRevision(Revision revision)
        {
            Node revisionNode = revision.ParentNode;
            // WORDSNET-24018 There are no separate groups for cells/rows in Word, but there is a group of table revisions instead.
            if (revisionNode.NodeType == NodeType.Row)
                revisionNode = ((Row)revisionNode).ParentTable;
            if (revisionNode.NodeType == NodeType.Cell)
                revisionNode = ((Cell)revisionNode).ParentTable;

            foreach (RevisionGroup group in Groups)
                if ((group.RevisionType == revision.RevisionType) && group.Nodes.Contains(revisionNode))
                    return group;

            return null;
        }

        internal void InvalidateGroups()
        {
            mRevisionGroups = null;
        }

        /// <summary>
        /// Incremented each time the collection is changed.
        /// Used as a version to invalidate RevisionIterator objects built for the collections.
        /// </summary>
        internal int ChangeCount { get; private set; }

        /// <summary>
        /// Collection of revision groups.
        /// </summary>
        public RevisionGroupCollection Groups
        {
            get { return mRevisionGroups ?? (mRevisionGroups = new RevisionGroupCollection(mDocument)); }
        }

        /// <summary>
        /// Loop through all nodes and styles with revisions and create real collection of revisions.
        /// </summary>
        private void CollectRevisions()
        {
            mNodeRevisions.Clear();
            mStyleRevisions.Clear();
            mEmptyFormatRevisions.Clear();

            mDocumentChangeCount = DocumentChangeCount;
            mRevisionNodeMatcher.ParentNode = mParentNode;

            List<Node> nodesWithRevisions;
            CompositeNode parent;

            if (mNodeRange != null)
            {
                if (mNodeRange.IsEmpty || mNodeRange.IsVoid ||
                    mNodeRange.Start.Node.IsRemoved || mNodeRange.End.Node.IsRemoved)
                    return;

                nodesWithRevisions = new List<Node>();
                foreach (Node node in mNodeRange)
                    if (RevisionUtil.HasRevision(node))
                        nodesWithRevisions.Add(node);

                parent = null;
            }
            else if (mParentNode == null)
            {
                nodesWithRevisions = new NodeCollection(mDocument, mRevisionNodeMatcher, true).ToNodeList();
                parent = mDocument;
            }
            else
            {
                parent = mParentNode as CompositeNode;
                nodesWithRevisions = (parent != null)
                    ? new NodeCollection(parent, mRevisionNodeMatcher, true).ToNodeList()
                    : new List<Node>();

                nodesWithRevisions.Add(mParentNode);
            }

            if (parent != null)
            {
                // Get revisions from fallback shapes.
                foreach (ShapeBase shape in parent.GetChildNodes(NodeType.Shape, true))
                {
                    if (shape.FallbackShape != null)
                    {
                        NodeCollection nodes = new NodeCollection(shape.FallbackShape, mRevisionNodeMatcher, true);
                        nodesWithRevisions.AddRange(nodes.ToNodeList());
                    }
                }
            }

            // andrnosk: WORDSNET-9611 A node can have more then one revision,
            // that is why we have to go through all nodes with revisions and create real collection of revisions.
            foreach (Node node in nodesWithRevisions)
            {
                WordAttrCollection formatting = RevisionUtil.GetNodeFormatting(node);
                if (formatting != null)
                    AddNodeRevisions(formatting, node);

                // Paragraph and SDT have extra formatting that is checked for revisions separately.
                Paragraph paragraph = node as Paragraph;
                if (paragraph != null)
                    AddNodeRevisions(paragraph.ParagraphBreakRunPr, paragraph);

                StructuredDocumentTag sdt = node as StructuredDocumentTag;
                if (sdt != null)
                    AddNodeRevisions(sdt.EndCharacterRunPr, sdt);
            }

            foreach (Style style in mDocument.Styles)
            {
                if (style.HasRevisions)
                    AddStyleRevisions(style);
                if (style.HasEmptyFormatRevision)
                    AddEmptyStyleFormatRevision(style);
            }

            ChangeCount++;
        }

        private int DocumentChangeCount
        {
            get
            {
                // mDocument is null in case when mContainer is EmptyCompositeNode so let's handle this properly.
                return mDocument != null
                    ? mDocument.TreeChangeCount
                    : 0;
            }
        }

        /// <summary>
        /// Invalidates a collection if the document has changed since last time the collection was accessed.
        /// </summary>
        private void CheckInvalidate()
        {
            if (mDocumentChangeCount != DocumentChangeCount)
                CollectRevisions();
        }

        /// <summary>
        /// Creates and adds new Revision for style to the empty format revision array.
        /// </summary>
        /// <param name="style">Style with empty revision.</param>
        private void AddEmptyStyleFormatRevision(Style style)
        {
            if (style.RunPr.HasEmptyFormatRevision)
                AddStyleRevision(mEmptyFormatRevisions, style.RunPr.FormatRevision, style, false);
            if (style.ParaPr.HasEmptyFormatRevision)
                AddStyleRevision(mEmptyFormatRevisions, style.ParaPr.FormatRevision, style, false);
        }

        /// <summary>
        /// Adds all revisions applied to node to the node revision array.
        /// </summary>
        private void AddNodeRevisions(WordAttrCollection attrCollection, Node node)
        {
            if (attrCollection.HasInsertRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.Insertion, attrCollection.InsertRevision, node, true);
            if (attrCollection.HasDeleteRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.Deletion, attrCollection.DeleteRevision, node, true);
            if (attrCollection.HasFormatRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.FormatChange, attrCollection.FormatRevision, node, true);
            if (attrCollection.HasMoveFromRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.Moving, attrCollection.MoveFromRevision, node, true);
            if (attrCollection.HasMoveToRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.Moving, attrCollection.MoveToRevision, node, true);

            if (attrCollection.HasEmptyFormatRevision)
                AddNodeRevision(mEmptyFormatRevisions, RevisionType.FormatChange, attrCollection.FormatRevision, node, false);

            ParaPr paraPr = attrCollection as ParaPr;
            if ((paraPr != null) && paraPr.HasNumberRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.FormatChange, paraPr.NumberRevision, node, true);

            RunPr runPr = attrCollection as RunPr;
            if ((runPr != null) && runPr.HasNumberRevision)
                AddNodeRevision(mNodeRevisions, RevisionType.FormatChange, runPr.NumberRevision, node, true);
        }

        /// <summary>
        /// Creates and adds new Revision to the node revision array.
        /// </summary>
        private void AddNodeRevision(IList<Revision> revArr, RevisionType type, RevisionBase revision, Node nodeWithRevisions, bool incChangeCount)
        {
            revArr.Add(new Revision(type, revision, nodeWithRevisions, this));
            IncrementChangeCount(incChangeCount);
        }

        /// <summary>
        /// Creates and adds new Revision to the style revision array.
        /// </summary>
        private void AddStyleRevision(IList<Revision> revArr, RevisionBase revision, Style styleWithRevisions, bool incChangeCount)
        {
            revArr.Add(new Revision(RevisionType.StyleDefinitionChange, revision, styleWithRevisions, this));
            IncrementChangeCount(incChangeCount);
        }

        /// <summary>
        /// Check incoming flag and increment changes count if it is set.
        /// </summary>
        /// <param name="incChangeCount">When value is "True" changes count is incremented.</param>
        private void IncrementChangeCount(bool incChangeCount)
        {
            if (incChangeCount)
                ++ChangeCount;
        }

        /// <summary>
        /// Creates IList of revisions from this RevisionCollection.
        /// </summary>
        internal IList<Revision> ToList()
        {
            return ToList(DefaultRevisionCriteria.Instance);
        }

        private IList<Revision> ToList(IRevisionCriteria criteria)
        {
            List<Revision> result = new List<Revision>(Count + mEmptyFormatRevisions.Count);

            foreach (Revision revision in this)
                CollectRevision(result, revision, criteria);

            foreach (Revision revision in mEmptyFormatRevisions)
                CollectRevision(result, revision, criteria);

            return result;
        }

        private static void CollectRevision(ICollection<Revision> revisions, Revision revision, IRevisionCriteria criteria)
        {
            if (!criteria.IsMatch(revision))
                return;

            revisions.Add(revision);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<Revision> GetEnumerator()
        {
            return new RevisionIterator(this);
        }

        [CppSkipEntity("C++ doesn't support untyped collection interfaces")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int mDocumentChangeCount;
        [CppWeakPtr]
        private readonly DocumentBase mDocument;
        private readonly Node mParentNode;
        private readonly NodeRange mNodeRange;
        private readonly List<Revision> mNodeRevisions = new List<Revision>();
        private readonly List<Revision> mEmptyFormatRevisions = new List<Revision>();
        private readonly List<Revision> mStyleRevisions = new List<Revision>();
        private readonly RevisionNodeMatcher mRevisionNodeMatcher = new RevisionNodeMatcher();
        private RevisionGroupCollection mRevisionGroups;

        private sealed class RevisionIterator : IEnumerator<Revision>
        {
            internal RevisionIterator(RevisionCollection revisions)
            {
                revisions.CheckInvalidate();
                mCollection = revisions;
                mCollectionChangeCount = revisions.ChangeCount;
                mNodeRevisions = revisions.mNodeRevisions;
                mStyleRevisions = new EnumeratorWrapperPalGeneric<Revision>(revisions.mStyleRevisions.GetEnumerator());
                Reset();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public Revision Current
            {
                get
                {
                    if (mCollectionChangeCount != mCollection.ChangeCount)
                        throw new InvalidOperationException(ExceptionMessage);

                    return mCurNode;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (mCollectionChangeCount != mCollection.ChangeCount)
                    throw new InvalidOperationException(ExceptionMessage);

                bool result;
                if (mIndex < mNodeRevisions.Count)
                {
                    mCurNode = mNodeRevisions[mIndex];
                    mIndex++;
                    result = true;
                }
                else if (mStyleRevisions.MoveNext())
                {
                    mCurNode = mStyleRevisions.Current;
                    result = true;
                }
                else
                {
                    mCurNode = null;
                    result = false;
                }
                return result;
            }

            public void Reset()
            {
                mIndex = 0;
                mCurNode = null;
                mCollectionChangeCount = mCollection.ChangeCount;
            }

            [CppWeakPtr]
            private readonly RevisionCollection mCollection;
            private int mCollectionChangeCount;
            private readonly List<Revision> mNodeRevisions;
            private readonly EnumeratorWrapperPalGeneric<Revision> mStyleRevisions;
            private int mIndex;
            private Revision mCurNode;

            private const string ExceptionMessage = "RevisionCollection was modified by Accept/Reject operation; enumeration operation may not execute.";
        }

        private class DefaultRevisionCriteria : IRevisionCriteria
        {
            private DefaultRevisionCriteria()
            {
            }

            bool IRevisionCriteria.IsMatch(Revision revision)
            {
                return true;
            }

            internal static readonly IRevisionCriteria Instance = new DefaultRevisionCriteria();
        }
    }
}
