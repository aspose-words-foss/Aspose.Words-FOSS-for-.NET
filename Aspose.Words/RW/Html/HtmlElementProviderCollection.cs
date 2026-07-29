// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/10/2013 by Alexey Butalov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Represents collection of <see cref="IHtmlElementProvider"/> elements.
    /// </summary>
    internal class HtmlElementProviderCollection : IEnumerable<IHtmlElementProvider>
    {
        internal HtmlElementProviderCollection()
        {
            mList = new List<IHtmlElementProvider>();
        }

        public IEnumerator<IHtmlElementProvider> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds item into the collection.
        /// </summary>
        internal int Add(IHtmlElementProvider item)
        {
            mList.Add(item);
            return mList.Count - 1;
        }

        internal void Insert(int index, IHtmlElementProvider item)
        {
            mList.Insert(index, item);
        }

        internal IHtmlElementProvider[] ToArray()
        {
            return mList.ToArray();
        }

        internal IHtmlElementProvider this[int index]
        {
            get { return mList[index]; }
        }

        internal int Count
        {
            get { return mList.Count; }
        }

        private readonly List<IHtmlElementProvider> mList;
    }
}