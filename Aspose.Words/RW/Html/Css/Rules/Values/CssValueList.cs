// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2013 by Alexey Butalov

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a list of <see cref="CssValue">CSS values</see>.
    /// </summary>
    internal class CssValueList : IEnumerable<CssValue>
    {
        internal CssValueList()
        {
            mValues = new List<CssValue>();
        }

        internal CssValueList(int capacity)
        {
            mValues = new List<CssValue>(capacity);
        }

        internal CssValueList(CssValueList other)
            : this(other.Count)
        {
            mValues.AddRange(other.mValues);
        }

        internal CssValueList(params CssValue[] values)
            : this()
        {
            mValues.AddRange(values);
        }

        public IEnumerator<CssValue> GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            int result = 0;
            unchecked
            {
                foreach (CssValue value in mValues)
                {
                    result = (result * 397) ^ value.GetHashCode();
                }
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            CssValueList valueList = obj as CssValueList;
            if (valueList == null)
                return false;

            return Equals(valueList);
        }

        /// <summary>
        /// Determines whether the specified CSS values are equal to the current values.
        /// </summary>
        internal bool Equals(CssValueList other)
        {
            // Standard reference comparisons.
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (Count != other.Count)
                return false;
            for (int i = 0; i < mValues.Count; i++)
            {
                if (!mValues[i].Equals(other[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified CSS values are equal to the current values.
        /// </summary>
        internal bool Equals(params CssValue[] values)
        {
            return Equals(new CssValueList(values));
        }

        internal bool Equals(CssValue value)
        {
            return (Count == 1) && mValues[0].Equals(value);
        }

        /// <summary>
        /// Appends a value to the list.
        /// </summary>
        /// <param name="value">CSS value.</param> 
        /// <returns>The index of the appended value.</returns>
        internal int Add(CssValue value)
        {
            mValues.Add(value);
            return mValues.Count - 1;
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        internal void RemoveAt(int index)
        {
            mValues.RemoveAt(index);
        }

        internal void Remove(CssValue value)
        {
            mValues.Remove(value);
        }

        internal bool Contains(CssValue value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Equals(value))
                    return true;
            }
            return false;
        }

        internal void Clear()
        {
            mValues.Clear();
        }

        /// <summary>
        /// Creates a shallow copy of a range of items of this list. 
        /// </summary>
        /// <param name="index">The zero-based index at which the range starts.</param>
        /// <param name="count">The number of items in the range. Use -1 to copy all values up to the end of list.</param>
        internal CssValueList GetRange(int index, int count)
        {
            if (count == -1)
            {
                count = Count - index;
            }
            CssValueList range = new CssValueList(count);
            for (int i = 0; i < count; i++)
            {
                range.Add(this[index + i]);
            }
            return range;
        }

        /// <summary>
        /// Gets a CSS representation of the values.
        /// </summary>
        internal string ToCss()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CssValue cssValue in mValues)
            {
                if ((sb.Length > 0) && !(cssValue is CssCommaValue))
                    sb.Append(' ');
                cssValue.ToCss(sb);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the number of items in the list.
        /// </summary>
        internal int Count
        {
            get { return mValues.Count; }
        }

        /// <summary>
        /// Gets or sets a CSS value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element is located in this list.</param>
        internal CssValue this[int index]
        {
            get { return mValues[index]; }
            set { mValues[index] = value; }
        }

        private readonly List<CssValue> mValues;
    }
}
