// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/09/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a container for text of a comment.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-comments/">Working with Comments</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>A comment is an annotation which is anchored to a region of text or to a position in text.
    /// A comment can contain an arbitrary amount of block-level content.</para>
    ///
    /// <para>If a <see cref="Comment"/> object occurs on its own, the comment is anchored to
    /// the position of the <see cref="Comment"/> object.</para>
    ///
    /// <para>To anchor a comment to a region of text three objects are required: <see cref="Comment"/>,
    /// <see cref="CommentRangeStart"/> and <see cref="CommentRangeEnd"/>. All three objects need to share the same
    /// <see cref="Comment.Id"/> value.</para>
    ///
    /// <p><see cref="Comment"/> is an inline-level node and can only be a child of <see cref="Paragraph"/>.</p>
    ///
    /// <p><see cref="Comment"/> can contain <see cref="Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> child nodes.</p>
    ///
    /// <seealso cref="CommentRangeStart"/>
    /// <seealso cref="CommentRangeEnd"/>
    /// </remarks>
    public sealed class Comment : InlineStory, INodeWithAnnotationId, IMoveTrackableNode
    {
        /// <summary>
        /// Main ctor used internally.
        /// </summary>
        internal Comment(DocumentBase doc, RunPr runPr) :
            base(doc, runPr)
        {
            // We must attempt to generate a unique id because comments with duplicate ids will be discarded during save.
            mId = doc.GetNextAnnotationId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Comment"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        ///
        /// <p>To append <see cref="Comment"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the paragraph where you want the comment inserted.</p>
        ///
        /// <para>After creating a comment, don't forget to set its <see cref="Author"/>,
        /// <see cref="Initial"/> and <see cref="DateTime"/> properties.</para>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Comment(DocumentBase doc) :
            this(doc, "", "", DateTime.MinValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="author">The author name for the comment. Cannot be <c>null</c>.</param>
        /// <param name="initial">The author initials for the comment. Cannot be <c>null</c>.</param>
        /// <param name="dateTime">The date and time for the comment.</param>
        public Comment(DocumentBase doc, string author, string initial, DateTime dateTime) : this(doc, new RunPr())
        {
            Author = author;
            Initial = initial;
            LocalDateTime = dateTime;
            Font.StyleIdentifier = StyleIdentifier.CommentReference;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Comment"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Comment; }
        }

        /// <summary>
        /// Returns <see cref="StoryType.Comments"/>.
        /// </summary>
        public override StoryType StoryType
        {
            get { return StoryType.Comments; }
        }

        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <remarks>
        /// <para>The comment identifier allows to anchor a comment to a region of text in the document.
        /// The region must be demarcated using the <see cref="CommentRangeStart"/> and <see cref="CommentRangeEnd"/>
        /// object sharing the same identifier value as the <see cref="Comment"/> object.</para>
        ///
        /// <para>You would use this value when looking for the <see cref="CommentRangeStart"/> and
        /// <see cref="CommentRangeEnd"/> nodes that are linked to this comment.</para>
        ///
        /// <para>Comment identifiers are supposed to be unique across a document and Aspose.Words automatically
        /// maintains comment identifiers when loading, saving and combining documents.</para>
        /// </remarks>
        public int Id
        {
            get { return mId; }
            set
            {
                mId = value;
                if (Document != null)
                    Document.MarkChanged();
            }
        }

        /// <summary>
        /// INeedImportCommentId
        /// </summary>
        int INodeWithAnnotationId.IdInternal
        {
            get { return Id; }
            set { Id = value; }
        }

        int INodeWithAnnotationId.ParentIdInternal
        {
            get { return ParentId; }
            set { ParentId = value; }
        }

        /// <summary>
        /// Returns or sets the initials of the user associated with a specific comment.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Initial
        {
            get { return mInitial; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "Initial");
                mInitial = value;
            }
        }

        /// <summary>
        /// Gets the date and time that the comment was made.
        /// </summary>
        /// <remarks>
        /// Default is
        /// <ms><see cref="System.DateTime.MinValue"/></ms>
        /// <java>03.01.0001</java>
        /// <cpp><see cref="System.DateTime.MinValue"/></cpp>
        /// </remarks>
        public DateTime DateTime
        {
            get { return mLocalDateTime; }
            set
            {
                mLocalDateTime = value;

                // If mUtcDateTime is not defined, leave it as is, otherwise ComplianceInfo will be updated as having
                // Word 2019 extensions. If necessary, UtcDateTime will be set during validation.
                if (mDateTimeUtc != DateTime.MinValue)
                    mDateTimeUtc = (value != DateTime.MinValue) ? value.ToUniversalTime() : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the UTC date and time that the comment was made.
        /// </summary>
        /// <remarks>
        /// The default value is
        /// <ms><see cref="System.DateTime.MinValue"/></ms>
        /// <java>03.01.0001</java>
        /// <cpp><see cref="System.DateTime.MinValue"/></cpp>
        /// </remarks>
        public DateTime DateTimeUtc
        {
            get
            {
                if (mDateTimeUtc != DateTime.MinValue)
                    return mDateTimeUtc;

                return (mLocalDateTime != DateTime.MinValue)
                    ? mLocalDateTime.ToUniversalTime()
                    : DateTime.MinValue;
            }
            internal set { mDateTimeUtc = value; }
        }

        /// <summary>
        /// Returns or sets the author name for a comment.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Author
        {
            get { return mAuthor; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "Author");
                mAuthor = value;
            }
        }

        /// <summary>
        /// Gets or sets the local date and time that the comment was made.
        /// </summary>
        internal DateTime LocalDateTime
        {
            get { return mLocalDateTime; }
            set { mLocalDateTime = value; }
        }

        /// <summary>
        /// Gets a flag indicating whether the UTC date and time of the comment is defined.
        /// </summary>
        internal bool IsDateTimeUtcDefined
        {
            get { return (mDateTimeUtc != DateTime.MinValue); }
        }

        /// <summary>
        /// Gets or sets a collection of reactions to the comment.
        /// </summary>
        internal CommentReactionCollection Reactions { get; set; }

        /// <summary>
        /// Gets a flag indicating whether the comment has reactions.
        /// </summary>
        internal bool HasReactions
        {
            get { return (Reactions != null) && (Reactions.Count > 0); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitCommentStart"/>, then calls <see cref="Node.Accept"/> for all
        /// child nodes of the comment and calls <see cref="DocumentVisitor.VisitCommentEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Adds a reply to this comment.
        /// </summary>
        /// <param name="author">The author name for the reply.</param>
        /// <param name="initial">The author initials for the reply.</param>
        /// <param name="dateTime">The date and time for the reply.</param>
        /// <param name="text">The reply text.</param>
        /// <returns>The created <see cref="Comment"/> node for the reply.</returns>
        /// <remarks>
        /// <para>Due to the existing MS Office limitations only 1 level of replies is allowed in the document.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Throws if this method is called on the existing Reply comment.</exception>
        public Comment AddReply(string author, string initial, DateTime dateTime, string text)
        {
            if (ParentId != NoParent)
                throw new InvalidOperationException("Cannot add a reply to another reply.");

            if (ParentNode == null)
                throw new InvalidOperationException("No parent node.");

            Comment comment = new Comment(Document, author, initial, dateTime);
            ((INodeWithAnnotationId)comment).IdInternal = CommentIdGenerator.Generate(Document);
            comment.ParentId = mId;
            comment.SetText(text);

            // Insert the new reply after the last reply, or after the comment itself if there is none.
            Node insertAfterNode = this;
            Comment nextComment = NextNonAnnotationSibling as Comment;
            while ((nextComment != null) && (nextComment.ParentId == mId))
            {
                insertAfterNode = nextComment;
                nextComment = nextComment.NextNonAnnotationSibling as Comment;
            }

            ParentNode.InsertAfter(comment, insertAfterNode);

            CommentRangeFinder finder = new CommentRangeFinder();
            if (finder.Find(Document, mId)) // Comment may have no range nodes.
            {
                InsertReplyRangeStartNode(finder.RangeStart, new CommentRangeStart(Document, comment.Id));
                InsertReplyRangeEndNode(finder.RangeEnd, new CommentRangeEnd(Document, comment.Id));
            }

            return comment;
        }

        /// <summary>
        /// Finds a necessary place and inserts a rande start node of a reply.
        /// </summary>
        private void InsertReplyRangeStartNode(CommentRangeStart parentRangeStart, CommentRangeStart insertingRangeStart)
        {
            // A range start node of a reply is inserted after the corresponding node of the comment or after
            // the corresponding node of the last reply if any is present.
            // If commentRangeStart node of the comment has displacedByCustomXml attribute with the 'prev' value,
            // MS Word inserts commentRangeStart node of the reply before the corresponding node of the comment.
            // It is ignored now since we do not read the displacedByCustomXml attribute of the node.

            List<int> replyIds = new List<int>();
            foreach (Comment reply in Replies)
                replyIds.Add(reply.Id);

            Node start = parentRangeStart;
            while (start.NextSibling != null)
            {
                Node nextNode = start.NextSibling;
                if ((nextNode.NodeType == NodeType.CommentRangeStart) &&
                    replyIds.Contains(((CommentRangeStart)nextNode).Id))
                    start = nextNode;
                else
                    break;
            }

            start.InsertNext(insertingRangeStart);
        }

        /// <summary>
        /// Finds a necessary place and inserts a rande end node of a reply.
        /// </summary>
        private void InsertReplyRangeEndNode(CommentRangeEnd parentRangeEnd, CommentRangeEnd insertingRangeEnd)
        {
            // A range end node of a reply is inserted after the corresponding node of the comment or after
            // the comment node itself if it is the next sibling of the comment range end node. If the comment
            // already has other replies, a range end is inserted after the corresponding nodes of the replies.

            List<int> replyIds = new List<int>();
            foreach (Comment reply in Replies)
            {
                if (reply.Id != insertingRangeEnd.Id)
                    replyIds.Add(reply.Id);
            }

            Node end = parentRangeEnd;
            while (end.NextSibling != null)
            {
                Node nextNode = end.NextSibling;

                int id = -1;
                if (nextNode.NodeType == NodeType.CommentRangeEnd)
                    id = ((CommentRangeEnd)nextNode).Id;
                else if (nextNode.NodeType == NodeType.Comment)
                    id = ((Comment)nextNode).Id;
                else
                    break;

                if (replyIds.Contains(id) || (id == mId))
                    end = nextNode;
                else
                    break;
            }
            end.InsertNext(insertingRangeEnd);
        }

        /// <summary>
        /// Removes the specified reply to this comment.
        /// </summary>
        /// <remarks>All constituent nodes of the reply will be deleted from the document.</remarks>
        /// <param name="reply">The comment node of the deleting reply.</param>
        public void RemoveReply(Comment reply)
        {
            CommentRangeFinder finder = new CommentRangeFinder();
            if (finder.Find(Document, reply.Id))
            {
                finder.RangeStart.Remove();
                finder.RangeEnd.Remove();
            }
            reply.Remove();
        }

        /// <summary>
        /// Removes all replies to this comment.
        /// </summary>
        /// <remarks> All constituent nodes of the replies will be deleted from the document.</remarks>
        public void RemoveAllReplies()
        {
            foreach (Comment reply in Replies.ToList<Comment>())
                RemoveReply(reply);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the comment.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitCommentStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the comment.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitCommentEnd(this);
        }

        /// <summary>
        /// This is a convenience method that allows to easily set text of the comment.
        /// </summary>
        /// <remarks>
        /// <para>This method allows to quickly set text of a comment from a string. The string can contain
        /// paragraph breaks, this will create paragraphs of text in the comment accordingly.
        /// If you want to insert more complex elements into the comment, for example bookmarks
        /// or tables or apply rich formatting, then you need to use the appropriate node classes to
        /// build up the comment text.</para>
        /// </remarks>
        /// <param name="text">The new text of the comment.</param>
        public void SetText(string text)
        {
            RemoveAllChildren();

            Paragraph para = new Paragraph(Document);
            para.ParagraphFormat.StyleIdentifier = StyleIdentifier.CommentText;
            AppendChild(para);

            DocumentBuilder builder = new DocumentBuilder(FetchDocument());
            builder.MoveTo(para);

            // Create the annotation reference character. It is a position inside the comment text where MS Word
            // displays the comment author and number. It can be omitted, but let's create it as MS Word does.
            SpecialChar annotationRef = new SpecialChar(FetchDocument(), ControlChar.AnnotationRefChar, new RunPr());
            annotationRef.Font.StyleIdentifier = StyleIdentifier.CommentReference;
            builder.InsertNode(annotationRef);

            builder.Write(text);
        }

        /// <summary>
        /// Returns the parent <see cref="Comment"/> object. Returns <c>null</c> for top-level comments.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public Comment Ancestor
        {
            get
            {
                if ((Document == null) || (ParentId == NoParent))
                    return null;

                // Make simplest implementation for a while.
                foreach(Comment comment in Document.GetChildNodes(NodeType.Comment, true))
                    if (comment.Id == ParentId)
                        return comment;

                return null;
            }
        }

        /// <summary>
        /// Returns a collection of <see cref="Comment"/> objects that are immediate children of the specified comment.
        /// </summary>
        public CommentCollection Replies
        {
            get
            {
                if(mReplies == null)
                    mReplies = new CommentCollection(Document, this);

                return mReplies;
            }
        }

        /// <summary>
        /// Gets or sets flag indicating that the comment has been marked done.
        /// </summary>
        public bool Done
        {
            get { return mDone; }
            set { mDone = value; }
        }

        /// <summary>
        /// Gets or sets the parent comment ID. A value of <c>-1</c> means the comment has no parent.
        /// </summary>
        public int ParentId
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mParentId; }
            set
            {
                mParentId = value;
                if (Document != null)
                    Document.MarkChanged();
            }
        }

        /// <summary>
        /// Comment durable identifier.
        /// </summary>
        /// <dev>
        /// Introduced in MS Word 2016. Looks like it is comment identifier, not associated with paragraph ID. The ID
        /// is used to refer to a comment from a commentsExtensible document part (2.1.5 of [MS-DOCX]).
        /// It's not clear now whether it is used in MS shared document editing, so let's store it.
        /// </dev>
        internal int DurableId
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether the comment is a follow-up.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// The attribute MUST NOT be present on comments that are replies to another comment.
        /// When the property is <c>true</c>, the Office 365 version of Word ignores the content of the comment. Other
        /// versions of Word treat the comment as a regular comment.
        /// </remarks>
        internal bool IsIntelligentPlaceholder { get; set; }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return RunPr.MoveFromRevision; }
            set { RunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return RunPr.MoveToRevision; }
            set { RunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            RunPr.Remove(RevisionAttr.MoveFromRevision);
            RunPr.Remove(RevisionAttr.MoveToRevision);
        }

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} id:{1}, parent:{2}", base.ToString(), mId, (ParentId == NoParent) ? "None" : ParentId.ToString());
        }
#endif

        /// <summary>
        /// Value of <see cref="ParentId" /> indicating that comment has no parent.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int NoParent = -1;

        /// <summary>
        /// Indicates that comment as marked done.
        /// </summary>
        private bool mDone;

        private int mId;
        private int mParentId = NoParent;
        private string mInitial = "";
        private string mAuthor = "";
        private DateTime mLocalDateTime = DateTime.MinValue; // Init for Java.
        private DateTime mDateTimeUtc = DateTime.MinValue; // Init for Java.

        private CommentCollection mReplies;
    }
}
