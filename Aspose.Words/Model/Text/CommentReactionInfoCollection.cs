// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2024 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a collection of <see cref="CommentReactionInfo"/> instances.
    /// </summary>
    internal class CommentReactionInfoCollection : IEnumerable<CommentReactionInfo>
    {
        /// <summary>
        /// Adds the specified comment reaction info to this collection.
        /// </summary>
        internal void Add(CommentReactionInfo reactionInfo)
        {
            mReactionInfos.Add(reactionInfo);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<CommentReactionInfo> GetEnumerator()
        {
            return mReactionInfos.GetEnumerator();
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
            get { return mReactionInfos.Count; }
        }

        /// <summary>
        /// Returns a comment reaction info by an index.
        /// </summary>
        internal CommentReactionInfo this[int index]
        {
            get { return mReactionInfos[index]; }
        }

        private readonly List<CommentReactionInfo> mReactionInfos = new List<CommentReactionInfo>();
    }
}
