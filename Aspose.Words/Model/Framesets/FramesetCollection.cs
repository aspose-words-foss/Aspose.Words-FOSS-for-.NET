// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2010 by Alexey Morozov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Represents a collection of instances of the <see cref="Frameset"/> class.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    public class FramesetCollection : IEnumerable<Frameset>
    {
        /// <summary>
        /// Gets the number of frames or frames pages contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets a frame or frames page at the specified index.
        /// </summary>
        public Frameset this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<Frameset> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        /// <summary>
        /// Returns a dictionary <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds Frame object to collection.
        /// </summary>
        /// <param name="frame"></param>
        internal void Add(Frameset frame)
        {
            mItems.Add(frame);
        }

        private readonly List<Frameset> mItems = new List<Frameset>();
    }
}
