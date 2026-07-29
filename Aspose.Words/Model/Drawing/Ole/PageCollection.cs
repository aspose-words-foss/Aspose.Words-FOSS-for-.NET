// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2019 by Ilya Navrotskiy

using System.Collections.Generic;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Forms2;

namespace Aspose.Words.Drawing.Ole
{
    /// <summary>
    /// The collection of pages of a <see cref="MultiPageControl"/>.
    /// </summary>
    /// <remarks>
    /// The pages in the collection are represented as <see cref="FormControl"/> objects.
    /// </remarks>
    internal class PageCollection
    {
        /// <summary>
        /// Creates collection of pages based on a source collection of Forms 2.0 controls.
        /// </summary>
        internal PageCollection(Forms2OleControlCollection source)
        {
            mItems = source.Items;
        }

        /// <summary>
        /// Adds a page to the collection.
        /// </summary>
        internal FormControl Add(FormControl page)
        {
            // MultiPage should have TabStrip control, that organizes tabs on it.
            TabStripControl tabStripControl = GetTabStripControl();
            if (tabStripControl == null)
            {
                tabStripControl = new TabStripControl("");
                mItems.Add(tabStripControl);
            }

            // Add new tab.
            TabStripTab tab = new TabStripTab(((IEmbeddedObject)page).GetName());
            tabStripControl.Tabs.Add(tab);

            // Add new page.
            mItems.Add(page);
            return page;
        }

        /// <summary>
        /// Removes a page from the collection.
        /// </summary>
        internal void Remove(FormControl page)
        {
            int index = IndexOf(page);
            if (index != -1)
            {
                TabStripControl tabStripControl = GetTabStripControl();
                // If there are pages in the collection, then there must be a corresponding TabStripControl.
                Debug.Assert(tabStripControl != null);
                tabStripControl.Tabs.RemoveAt(index);

                mItems.Remove(page);
            }
        }

        /// <summary>
        /// Gets a TabStripControl object from the collection of child nodes.
        /// </summary>
        private TabStripControl GetTabStripControl()
        {
            foreach (Forms2OleControl item in mItems)
            {
                if (item.Type == Forms2OleControlType.TabStrip)
                    return (TabStripControl)item;
            }

            return null;
        }

        /// <summary>
        /// Returns index of a specified page in the collection, or -1 if not found.
        /// </summary>
        private int IndexOf(FormControl page)
        {
            int index = 0;
            foreach (Forms2OleControl control in mItems)
            {
                if (control.Type == Forms2OleControlType.Form)
                {
                    if (control.Equals(page))
                        return index;

                    index++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets page at a specified index.
        /// </summary>
        internal FormControl this[int index]
        {
            get
            {
                int curIndex = -1;
                foreach (Forms2OleControl control in mItems)
                {
                    if (control.Type == Forms2OleControlType.Form)
                    {
                        curIndex++;
                        if (curIndex == index)
                            return (FormControl)control;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a count of pages in the collection.
        /// </summary>
        internal int Count
        {
            get
            {
                int count = 0;
                foreach (Forms2OleControl control in mItems)
                {
                    if (control.Type == Forms2OleControlType.Form)
                        count++;
                }

                return count;
            }
        }

        private readonly List<Forms2OleControl> mItems;
    }
}
