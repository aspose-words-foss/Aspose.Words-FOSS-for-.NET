// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2015 by Alexey Morozov

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Words
{
    /// <summary>
    /// Represents collection of <see cref="RubyChunk"/> items.
    /// </summary>
    internal class RubyChunkCollection : IEnumerable<RubyChunk>
    {
        internal RubyChunk this[int index]
        {
            get { return mItems[index]; }
        }

        internal void Add(RubyChunk item)
        {
            mItems.Add(item);
        }

        internal RubyChunkCollection Clone()
        {
            RubyChunkCollection clonedCollection = new RubyChunkCollection();
            foreach (RubyChunk chunk in mItems)
            {
                RubyChunk clonedChunk = new RubyChunk();
                clonedChunk.Text = chunk.Text;
                clonedChunk.RunPr = chunk.RunPr.Clone();

                clonedCollection.Add(clonedChunk);
            }

            return clonedCollection;
        }

        internal void Clear()
        {
            mItems.Clear();
        }

        public IEnumerator<RubyChunk> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Text of all collected chunks.
        /// </summary>
        internal string Text 
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (RubyChunk rubyChunk in mItems)
                    sb.Append(rubyChunk.Text);

                return sb.ToString();
            }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        private readonly List<RubyChunk> mItems = new List<RubyChunk>();
    }
}
