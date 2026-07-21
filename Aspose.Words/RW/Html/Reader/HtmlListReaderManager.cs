// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2020 by Anton Savko

using System.Collections.Generic;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Organizes <see cref="HtmlListReader"/> instances into nested contexts.
    /// </summary>
    internal class HtmlListReaderManager
    {
        internal HtmlListReaderManager()
        {
            mListReaders = new Stack<HtmlListReader>();
            OpenNewContext();
        }

        internal void OpenNewContext()
        {
            // A new context starts with no list reader.
            // We'll create one just before we start processing a list.
            mListReaders.Push(null);
        }

        internal void CloseContext()
        {
            Debug.Assert(mListReaders.Count > 0);

            mListReaders.Pop();
        }

        internal void CreateListReader(
            DocumentBuilder builder,
            DocumentFormatter documentFormatter,
            HtmlResourceLoader htmlResourceLoader,
            string baseUri,
            double defaultWholeListLeftIndent,
            bool adjustMarkerPositions)
        {
            Debug.Assert(CurrentListReader == null);

            HtmlListReader newListReader = new HtmlListReader(
                builder,
                documentFormatter,
                htmlResourceLoader,
                baseUri,
                defaultWholeListLeftIndent,
                adjustMarkerPositions);

            mListReaders.Pop();
            mListReaders.Push(newListReader);
        }

        internal HtmlListReader CurrentListReader
        {
            get
            {
                Debug.Assert(mListReaders.Count > 0);

                return mListReaders.Peek();
            }
        }

        private readonly Stack<HtmlListReader> mListReaders;
    }
}
