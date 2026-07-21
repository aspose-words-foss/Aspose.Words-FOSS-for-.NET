// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2018 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Contains info about last paragraph search for fields with <see cref="FieldParagraphFinder"/>.
    /// </summary>
    internal class FieldParagraphFinderCache
    {
        /// <summary>
        /// Retrieves or creates a new search result cache object for a field.
        /// </summary>
        internal LastParagraphSearchResult EnsureLastFieldParagraphResult(FieldStart fieldStart)
        {
            LastParagraphSearchResult results = mCache.GetValueOrNull(fieldStart);
            if (results == null)
            {
                results = new LastParagraphSearchResult();
                mCache[fieldStart] = results;
            }

            return results;
        }

        private readonly Dictionary<FieldStart, LastParagraphSearchResult> mCache =
            new Dictionary<FieldStart, LastParagraphSearchResult>();
    }
}
