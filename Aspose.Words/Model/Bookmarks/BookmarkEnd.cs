// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2005 by Roman Korchagin

using System;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents an end of a bookmark in a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-bookmarks/">Working with Bookmarks</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A complete bookmark in a Word document consists of a <see cref="BookmarkStart"/>
    /// and a matching <see cref="BookmarkEnd"/> with the same bookmark name.</p>
    ///
    /// <p><see cref="BookmarkStart"/> and <see cref="BookmarkEnd"/> are just markers inside a document
    /// that specify where the bookmark starts and ends.</p>
    ///
    /// <p>Use the <see cref="Bookmark"/> class as a "facade" to work with a bookmark
    /// as a single object.</p>
    /// </remarks>
    public class BookmarkEnd : Node, IBookmarkNode, IDisplaceableByCustomXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkEnd"/> class.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="name">The name of the bookmark. Cannot be <c>null</c>.</param>
        public BookmarkEnd(DocumentBase doc, string name) : base(doc)
        {
            ArgumentUtil.CheckNotNull(name, "name");
            mName = name;
        }

        internal BookmarkEnd(DocumentBase doc) : base(doc)
        {
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.BookmarkEnd"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.BookmarkEnd; }
        }

        /// <summary>
        /// Gets the name of the bookmark.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// </remarks>
        internal string GetNameInternal()
        {
            return mName;
        }

        /// <summary>
        /// Sets name of the bookmark.
        /// </summary>
        internal void SetNameInternal(string name)
        {
            ArgumentUtil.CheckNotNull(name, "name");
            mName = name;
        }

        /// <summary>
        /// Specifies that placement of the bookmark node is directly linked with the location of the physical 
        /// presentation of a custom XML element in the document.
        /// </summary>
        internal DisplacedByType DisplacedBy
        {
            get { return mDisplacedBy; }
            set { mDisplacedBy = value; }
        }

        /// <summary>
        /// Specifies that placement of the bookmark node is directly linked with the location of the physical 
        /// presentation of a custom XML element in the document.
        /// </summary>
        DisplacedByType IDisplaceableByCustomXml.DisplacedByCustomXml
        {
            get { return mDisplacedBy; }
            set { mDisplacedBy = value; }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitBookmarkEnd"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><c>false</c> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitBookmarkEnd(this));
        }

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} <{1}>", base.ToString(), mName);
        }
#endif

        #region IBookmarkNode implementation.
        /// <summary>
        /// Gets or sets the bookmark name.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// </remarks>
        public string Name
        {
            get { return GetNameInternal(); }
            set { SetNameInternal(value); }
        }
        #endregion

        private string mName = "";
        private DisplacedByType mDisplacedBy = DisplacedByType.Unspecified;
    }
}
