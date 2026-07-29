// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2024 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Represents the 'reactions' element of the CT_CommentReactions complex type defined in §2.1.1.1 of [MS-OREACTXML]
    /// that specifies information for the reactions to a comment. It is the root element in an extension within the
    /// extension list of a CommentsExtensible part.
    /// </summary>
    internal class CommentReactionCollection: IEnumerable<CommentReaction>
    {
        /// <summary>
        /// Adds the specified comment reaction to this collection.
        /// </summary>
        internal void Add(CommentReaction reaction)
        {
            mReactions.Add(reaction);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<CommentReaction> GetEnumerator()
        {
            return mReactions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        internal int Count
        {
            get { return mReactions.Count; }
        }

        /// <summary>
        /// Returns a comment reaction by an index.
        /// </summary>
        internal CommentReaction this[int index]
        {
            get { return mReactions[index]; }
        }

        private readonly List<CommentReaction> mReactions = new List<CommentReaction>();
    }
}
