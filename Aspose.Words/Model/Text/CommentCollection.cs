// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2017 by Alexey Morozov

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to a collection of <see cref="Comment"/> nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-comments/">Working with Comments</a> documentation article.</para>
    /// </summary>
    [JavaGenericArguments("NodeCollection<Comment>")]
    public class CommentCollection : NodeCollection
    {
        /// <summary>
        /// Implements matching of comments that are children of given comment.
        /// </summary>
        private class CommentReplyMatcher : NodeMatcher
        {
            [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
            private readonly Comment mParentComment;
            internal CommentReplyMatcher(Comment parentComment)
            {
                mParentComment = parentComment;
            }

            internal override bool IsMatch(Node node)
            {
                if (node.NodeType != NodeType.Comment)
                    return false;

                return (((Comment)node).ParentId == mParentComment.Id);
            }

            internal override bool IsSkipMarkupNodes
            {
                get { return true; }
            }
        }

        internal CommentCollection(DocumentBase doc, Comment parentComment)
            : base(doc, new CommentReplyMatcher(parentComment), true)
        {
        }

        /// <summary>
        /// Retrieves a <see cref="Comment"/> at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public new Comment this[int index]
        {
            get { return (Comment)base[index]; }
        }
    }
}
