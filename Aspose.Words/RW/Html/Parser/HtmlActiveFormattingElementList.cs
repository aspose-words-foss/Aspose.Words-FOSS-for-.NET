// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The list of active formatting elements, which is used by the HTML tree constructor to handle mis-nested formatting
    /// element tags.
    /// </summary>
    /// <remarks>
    /// The most recently opened element comes last.
    /// The list contains not only <see cref="HtmlElementNode"/> instances, but also special objects called 'scope markers',
    /// which divide formatting elements into groups to prevent formatting from 'leaking' in case of bad HTML markup. The most
    /// recent scope is called 'the current scope'. 
    /// </remarks>
    internal class HtmlActiveFormattingElementList
    {
        /// <summary>
        /// Adds the specified element to the list.
        /// </summary>
        /// <param name="element">An element.</param>
        /// <remarks>
        /// HTML specification limits the number of equal formatting elements that can be placed into one formatting scope.
        /// If the current formatting scope already contains maximum number of elements equal to the one being added, the oldest
        /// such element will be removed from the list as described here:
        /// http://www.w3.org/TR/html5/syntax.html#push-onto-the-list-of-active-formatting-elements
        /// </remarks>
        internal void Add(HtmlElementNode element)
        {
            Debug.Assert(element != null);

            // Maximum number of equal formatting elements that can share one formatting context.
            const int maxSameElementCount = 3;

            // Search the current formatting context for formatting elements that are equal to the element being added.
            // Count such elements and find the oldest one.
            int sameElementCount = 0;
            HtmlElementNode oldestSameElement = null;
            int elementIndex = mElements.Count - 1;
            while ((elementIndex >= 0) && (mElements[elementIndex] != gScopeMarker) && (sameElementCount < maxSameElementCount))
            {
                HtmlElementNode existingElement = mElements[elementIndex];
                bool same = (existingElement.Name == element.Name) &&
                    (existingElement.Namespace == element.Namespace) &&
                    (existingElement.Attributes.Equals(element.Attributes));
                if (same)
                {
                    ++sameElementCount;
                    oldestSameElement = existingElement;
                }
                --elementIndex;
            }

            // If the current formatting context contains too many formatting elements that are equal to the element being added,
            // remove the oldest such element.
            if (sameElementCount >= maxSameElementCount)
            {
                Debug.Assert(oldestSameElement != null);
                mElements.Remove(oldestSameElement);
            }

            mElements.Add(element);
        }

        /// <summary>
        /// Removes the specified element from the list.
        /// </summary>
        /// <param name="element">An element.</param>
        internal void Remove(HtmlElementNode element)
        {
            Debug.Assert(element != null);

            mElements.Remove(element);
        }

        /// <summary>
        /// Inserts the specified element into the list at the specified index.
        /// </summary>
        /// <param name="index">An element's index.</param>
        /// <param name="element">An element.</param>
        /// <remarks>
        /// This method does not check the number of equal elements that share one formatting scope.
        /// </remarks>
        internal void Insert(int index, HtmlElementNode element)
        {
            Debug.Assert(element != null);

            mElements.Insert(index, element);
        }

        /// <summary>
        /// Replaces an element at the specified index with the specified element.
        /// </summary>
        /// <param name="index">An element's index.</param>
        /// <param name="element">An element.</param>
        /// <remarks>
        /// This method does not check the number of equal elements that share one formatting scope.
        /// </remarks>
        internal void Replace(int index, HtmlElementNode element)
        {
            Debug.Assert(element != null);

            mElements[index] = element;
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="element">An element.</param>
        /// <returns>
        /// The zero-based index of the element, if the element is in the list. Otherwise, <c>-1</c> is returned.
        /// </returns>
        internal int IndexOf(HtmlElementNode element)
        {
            return mElements.IndexOf(element);
        }

        /// <summary>
        /// Opens a new formatting scope.
        /// </summary>
        /// <remarks>
        /// Adds a scope marker to the list.
        /// </remarks>
        internal void OpenNewScope()
        {
            mElements.Add(gScopeMarker);
        }

        /// <summary>
        /// Closes the current formatting scope.
        /// </summary>
        /// <remarks>
        /// Removes all elements from the list up to the last context boundary marker, as described here:
        /// http://www.w3.org/TR/html5/syntax.html#clear-the-list-of-active-formatting-elements-up-to-the-last-marker
        /// If there are no context boundary markers in the list, the method removes all elements from it.
        /// </remarks>
        internal void CloseCurrentScope()
        {
            int lastMarkerIndex = mElements.LastIndexOf(gScopeMarker);
            if (lastMarkerIndex >= 0)
            {
                mElements.RemoveRange(lastMarkerIndex, mElements.Count - lastMarkerIndex);
            }
            else
            {
                mElements.Clear();
            }
        }

        /// <summary>
        /// Searches the current formatting scope for an element with the specified name.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>
        /// An element with the specified name that is most recently added to the current formatting scope.
        /// If no such element is found, <c>null</c> is returned.
        /// </returns>
        internal HtmlElementNode FindInCurrentScope(string elementName)
        {
            int i = mElements.Count - 1;
            while ((i >= 0) && (mElements[i] != gScopeMarker))
            {
                HtmlElementNode element = mElements[i];
                if (element.Name == elementName)
                {
                    return element;
                }
                --i;
            }
            return null;
        }

        /// <summary>
        /// Gets number of elements in the list (including scope markers).
        /// </summary>
        internal int Count
        {
            get { return mElements.Count; }
        }

        /// <summary>
        /// Gets an element at the specified index in the list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>
        /// The element at the specified index. If the list contains a scope marker at the specified index,
        /// <c>null</c> is returned.
        /// </returns>
        internal HtmlElementNode this[int index]
        {
            get
            {
                return (mElements[index] != gScopeMarker)
                    ? mElements[index]
                    : null;
            }
        }

        /// <summary>
        /// The low-level list that contains elements and scope markers.
        /// </summary>
        private readonly List<HtmlElementNode> mElements = new List<HtmlElementNode>();

        /// <summary>
        /// The special value that is added to the list to mark the boundary of formatting scope.
        /// </summary>
        private static readonly HtmlElementNode gScopeMarker = new HtmlElementNode("ScopeMarker");
    }
}
