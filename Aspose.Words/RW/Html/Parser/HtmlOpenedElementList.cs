// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// List of still opened HTML elements used by the HTML tree constructor.
    /// </summary>
    /// <remarks>
    /// This data structure is called a stack by the HTML Specification, but in some situations it is accessed as a list.
    /// The most recently added elements comes last.
    /// </remarks>
    internal class HtmlOpenedElementList
    {
        internal void Add(HtmlElementNode element)
        {
            Debug.Assert(element != null);
            mElements.Add(element);
        }

        internal HtmlElementNode GetLast()
        {
            Debug.Assert(mElements.Count > 0);
            return mElements[mElements.Count - 1];
        }

        internal void RemoveLast()
        {
            Debug.Assert(mElements.Count > 0);
            mElements.RemoveAt(mElements.Count - 1);
        }

        internal void RemoveRange(int startIndex)
        {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex < mElements.Count);
            mElements.RemoveRange(startIndex, mElements.Count - startIndex);
        }

        internal void Remove(HtmlElementNode element)
        {
            Debug.Assert(element != null);
            mElements.Remove(element);
        }

        internal void Insert(int index, HtmlElementNode element)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index <= mElements.Count);
            Debug.Assert(element != null);
            mElements.Insert(index, element);
        }

        internal void Clear()
        {
            mElements.Clear();
        }

        internal bool IsEmpty
        {
            get { return mElements.Count == 0; }
        }

        internal int IndexOf(HtmlElementNode element)
        {
            return mElements.IndexOf(element);
        }

        internal bool Contains(HtmlElementNode element)
        {
            Debug.Assert(element != null);
            return mElements.Contains(element);
        }

        internal int Count
        {
            get { return mElements.Count; }
        }

        internal HtmlElementNode this[int index]
        {
            get { return mElements[index]; }
            set
            {
                Debug.Assert(value != null);
                mElements[index] = value;
            }
        }

        private readonly List<HtmlElementNode> mElements = new List<HtmlElementNode>();
    }
}
