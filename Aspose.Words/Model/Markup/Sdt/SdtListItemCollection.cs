// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2011 by Denis Darkin
using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Provides access to <see cref="SdtListItem"/> elements of a structured document tag.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    public class SdtListItemCollection : IEnumerable<SdtListItem>
    {
        internal SdtListItemCollection()
        {
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<SdtListItem> GetEnumerator()
        {
            return mChildren.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to this collection.
        /// </summary>
        public void Add(SdtListItem item)
        {
            mChildren.Add(item);
        }

        /// <summary>
        /// Removes a list item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            SdtListItem selectedValue = (SelectedIndex == index) ? null: SelectedValue;

            mChildren.RemoveAt(index);

            SetSelectedValueInternal(selectedValue);

            UpdateParentContent();
        }

        /// <summary>
        /// Clears all items from this collection.
        /// </summary>
        public void Clear()
        {
            mChildren.Clear();

            SelectedValue = null;

            UpdateParentContent();
        }

        internal SdtListItemCollection Clone()
        {
            SdtListItemCollection lhs = (SdtListItemCollection)MemberwiseClone();

            lhs.mChildren = new List<SdtListItem>(mChildren.Count);
            for (int itemIdx = 0; itemIdx < mChildren.Count; itemIdx++)
                lhs.Add(this[itemIdx].Clone());

            return lhs;
        }

        /// <summary>
        /// Searches for item which the Value property equals to the specified value and returns the zero-based index
        /// of the first occurrence within the collection.
        /// </summary>
        internal int IndexOfItemValue(string value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Value == value)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Sets link to parent SDT to able properly update SDT content.
        /// </summary>
        internal void SetParentSdt(StructuredDocumentTag parentSdt)
        {
            mParentSdt = parentSdt;
        }

        internal void SetSelectedValueInternal(SdtListItem item)
        {
            if (item == null)
            {
                mLastValueIdx = -1;
            }
            else if (mChildren.Contains(item))
            {
                mLastValueIdx = mChildren.IndexOf(item);
            }
            else
            {
                throw new ArgumentException("Can not find such LastValue in this collection.");
            }
        }

        /// <summary>
        /// Specifies currently selected value in this list.
        /// Null value allowed, meaning that no currently selected entry is associated with this list item collection.
        /// </summary>
        public SdtListItem SelectedValue
        {
            get
            {
                if (mLastValueIdx != -1)
                    return this[mLastValueIdx];

                return null;
            }
            set
            {
                SetSelectedValueInternal(value);

                // WORDSNET-21505 Do not update parent control if display text is not actually changed.
                if ((mParentSdt == null) || (SelectedValue == null) || (mParentSdt.ContentValue != SelectedValue.DisplayText))
                    UpdateParentContent();
            }
        }

        /// <summary>
        /// Specifies a value associated with current display text of a drop down list structured document tag.
        /// It is used to determine whether current display text in a SDT should be retained when document is loaded.
        /// (See ISO 29500, §17.5.2.5, 15 for more info.)
        /// </summary>
        internal string LastItemValue
        {
            get { return mLastItemValue; }
            set
            {
                mLastItemValue = value;

                UpdateParentContent();
            }
        }

        /// <summary>
        /// Returns a <see cref="SdtListItem"/> object given its zero-based index in the collection.
        /// </summary>
        public SdtListItem this[int index]
        {
            get { return mChildren[index]; }
        }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return mChildren.Count; }
        }

        /// <summary>
        /// Returns index of the selected value in the list, or -1 if nothing is selected.
        /// This is used to generate active control in PDF.
        /// </summary>
        internal int SelectedIndex
        {
            get { return mLastValueIdx; }
        }

        private void UpdateParentContent()
        {
            SdtContentUpdater.UpdateNonBoundDataContent(mParentSdt);
        }

        private int mLastValueIdx = -1;
        private List<SdtListItem> mChildren = new List<SdtListItem>();
        private string mLastItemValue;
        private StructuredDocumentTag mParentSdt;
    }
}
