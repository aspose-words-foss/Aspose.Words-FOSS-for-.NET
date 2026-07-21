// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// Represents collection of <see cref="Forms2OleControl"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ole-objects/">Working with Ole Objects</a> documentation article.</para>
    /// </summary>
    public class Forms2OleControlCollection : IEnumerable<Forms2OleControl>
    {
        /// <summary>
        /// Gets enumerator.
        /// </summary>
        public IEnumerator<Forms2OleControl> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)mItems).GetEnumerator();
        }

        /// <summary>
        /// Adds a specified control to the collection.
        /// </summary>
        internal Forms2OleControl Add(Forms2OleControl control)
        {
            mItems.Add(control);
            return control;
        }

        /// <summary>
        /// Gets <see cref="Forms2OleControl" /> object at a specified index.
        /// </summary>
        public Forms2OleControl this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// Gets count of objects in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets an internal list of the items.
        /// </summary>
        internal List<Forms2OleControl> Items
        {
            get { return mItems; }
        }

        private readonly List<Forms2OleControl> mItems = new List<Forms2OleControl>();
    }
}
