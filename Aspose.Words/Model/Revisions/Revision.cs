// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2012 by Denis Darkin

using System;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a revision (tracked change) in a document node or style.
    /// Use <see cref="RevisionType"/> to check the type of this revision.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/track-changes-in-a-document/">Track Changes in a Document</a> documentation article.</para>
    /// </summary>
    public class Revision
    {
        internal Revision(
            RevisionType revType,
            RevisionBase revBase,
            Node parentNode,
            RevisionCollection parentCollection)
            : this(revType, revBase, parentCollection)
        {
            Debug.Assert(parentNode != null);
            mParentNode = parentNode;
            mIsInline = parentNode is IInline;
        }

        /// <summary>
        /// ctor for revisions of styles. Currently we support creation of Revisions for Character, Paragraph and List style types.
        /// </summary>
        internal Revision(
            RevisionType revType,
            RevisionBase revBase,
            Style parentStyle,
            RevisionCollection parentCollection)
            : this(revType, revBase, parentCollection)
        {
            Debug.Assert(parentStyle != null);
            mParentStyle = parentStyle;
        }

        private Revision(RevisionType revType, RevisionBase revBase, RevisionCollection parentCollection)
        {
            Debug.Assert(parentCollection != null);
            mParentCollection = parentCollection;

            RevisionType = revType;
            mRevisionBase = revBase;
        }

        /// <summary>
        /// Accepts this revision.
        /// </summary>
        public void Accept()
        {
            HandleRevisions(true, new RevisionHandlingContext(true, RevisionType));
        }

        /// <summary>
        /// Reject this revision.
        /// </summary>
        public void Reject()
        {
            HandleRevisions(true, new RevisionHandlingContext(false, RevisionType));
        }

        /// <summary>
        /// Common method for accept/reject revisions both for individual revisions and for whole collection.
        /// </summary>
        /// <param name="notifyCollection">True to notify parent collection so it can update its live status.
        /// Used for operations on individual revisions.</param>
        /// <param name="context">Object to store current state of revision handling including nodes for delayed
        /// deletion.</param>
        internal void HandleRevisions(bool notifyCollection, RevisionHandlingContext context)
        {
            int documentTreeChangeCount = Document.TreeChangeCount;

            if (mParentNode != null) // dealing with Node revision
            {
                RevisionUtil.HandleNodeRevision(mParentNode, context);
            }
            else // dealing with style revision
            {
                if (context.IsAcceptance)
                {
                    mParentStyle.RunPr.AcceptFormatRevision();
                    mParentStyle.ParaPr.AcceptFormatRevision();
                }
                else
                {
                    mParentStyle.RunPr.RejectFormatRevision();
                    mParentStyle.ParaPr.RejectFormatRevision();
                }
            }

            // If it's a revision that doesn't cause a change in Document.TreeChangeCount,
            // we shall make this change ourselves to keep all Range.Revisions collections up-to-date.
            if (Document.TreeChangeCount == documentTreeChangeCount)
                Document.MarkChanged();

            if (notifyCollection)
                mParentCollection.Invalidate(this);
        }

        /// <summary>
        /// Gets or sets the author of this revision. Can not be empty string or <c>null</c>.
        /// </summary>
        public string Author
        {
            get { return mRevisionBase.Author; }

            set
            {
                if (!StringUtil.HasChars(value))
                    throw new ArgumentException(value);

                mRevisionBase.Author = value;
            }
        }

        /// <summary>
        /// Gets or sets the date/time of this revision.
        /// </summary>
        public DateTime DateTime
        {
            get { return mRevisionBase.DateTime; }
            set { mRevisionBase.DateTime = value; }
        }

        /// <summary>
        /// Gets the type of this revision.
        /// </summary>
        public RevisionType RevisionType { get; }

        /// <summary>
        /// Gets the immediate parent node (owner) of this revision.
        /// This property will work for any revision type other than <see cref="Aspose.Words.RevisionType.StyleDefinitionChange"/>.
        /// </summary>
        /// <remarks>
        /// If this revision relates to change of Style formatting, use <see cref="ParentStyle"/> instead.
        /// </remarks>
        public Node ParentNode
        {
            get
            {
                if (mParentNode == null)
                    throw new InvalidOperationException("Can not access ParentNode for a style revision. Use ParentStyle instead.");

                return mParentNode;
            }
        }

        /// <summary>
        /// Gets the immediate parent style (owner) of this revision.
        /// This property will work for only for the <see cref="Aspose.Words.RevisionType.StyleDefinitionChange"/> revision type.
        /// </summary>
        /// <remarks>
        /// If this revision relates to changes on document nodes, use <see cref="ParentNode"/> instead.
        /// </remarks>
        public Style ParentStyle
        {
            get
            {
                if (mParentStyle == null)
                    throw new InvalidOperationException("Can not access ParentStyle for a node revision. Use ParentNode instead.");

                return mParentStyle;
            }
        }

        /// <summary>
        /// Gets the revision group. Returns <c>null</c> if the revision does not belong to any group.
        /// </summary>
        /// <remarks>
        /// Revision has no group if revision type is <see cref="RevisionType.StyleDefinitionChange"/> or
        /// if the revision is not longer exist in document context (accepted/rejected).
        /// </remarks>
        public RevisionGroup Group
        {
            // WORDSNET-18418 Added property Group.
            get
            {
                if (RevisionType == RevisionType.StyleDefinitionChange)
                    return null;

                return mParentCollection.Groups[mRevisionBase];
            }
        }

        internal bool IsInline
        {
            get { return (RevisionType != RevisionType.StyleDefinitionChange) && mIsInline; }
        }

        private DocumentBase Document
        {
            get
            {
                return mParentNode != null
                    ? mParentNode.Document
                    : mParentStyle.Document;
            }
        }

        private readonly RevisionBase mRevisionBase;
        private readonly Node mParentNode;
        private readonly Style mParentStyle;
        private readonly bool mIsInline;
        private readonly RevisionCollection mParentCollection;
    }
}
