// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/08/2006 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Compares two table cells by their span values.
    /// </summary>
    internal class SpanComparer : IComparer<SpanCell>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal SpanComparer()
        {
        }

        public int Compare(SpanCell x, SpanCell y)
        {
            int span1 = x.Span;
            int span2 = y.Span;

            return span1.CompareTo(span2);
        }
    }
}
