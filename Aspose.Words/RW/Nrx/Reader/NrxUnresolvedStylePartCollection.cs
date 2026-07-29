// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2008 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// A collection of <see cref="NrxUnresolvedStylePart"/>.
    /// </summary>
    internal class NrxUnresolvedStylePartCollection : List<NrxUnresolvedStylePart>
    {
        /// <summary>
        /// Resolve the unresolved attributes for styles.
        /// </summary>
        internal void ResolveStyleLinks(INrxStyleIdToIstd resolver)
        {
            foreach (NrxUnresolvedStylePart unresolved in this)
            {
                Style style = unresolved.Style;

                // WORDSNET-22587, WORDSNET-26219 Word seems to remove `BasedOn` style in a Normal style.
                if ((unresolved.BasedOn != null) && (style.StyleIdentifier != StyleIdentifier.Normal))
                    style.BasedOnIstd = resolver.ResolveStyleIdToIstd(unresolved.BasedOn, StyleIndex.Nil);

                if (unresolved.Next != null)
                    style.NextIstd = resolver.ResolveStyleIdToIstd(unresolved.Next, style.Istd);

                if (unresolved.Link != null)
                    style.LinkedIstd = resolver.ResolveStyleIdToIstd(unresolved.Link, StyleIndex.Nil);
            }
        }
    }
}
