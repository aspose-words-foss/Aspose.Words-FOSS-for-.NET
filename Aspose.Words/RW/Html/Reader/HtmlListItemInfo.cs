// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2016 by Victor Chebotok

using Aspose.Words.Lists;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Superclass for storing and calculating information is required for correct indentation of list items.
    /// </summary>
    internal abstract class HtmlListItemInfo
    {
        protected HtmlListItemInfo(
            Paragraph paragraph,
            CssLength wholeListLeftIndent,
            CssLength leftIndent,
            CssLength padding,
            CssLength markerToTextDistance,
            bool isMarkerPositionInside,
            bool isListItemNoneListStyle)
        {
            Debug.Assert(paragraph != null);
            Debug.Assert(wholeListLeftIndent != null);
            Debug.Assert(leftIndent != null);
            Debug.Assert(padding != null);
            Debug.Assert(markerToTextDistance != null);

            Paragraph = paragraph;
            WholeListLeftIndent = wholeListLeftIndent;
            LeftIndent = leftIndent;
            Padding = padding;
            MarkerToTextDistance = markerToTextDistance;

            IsMarkerPositionInside = isMarkerPositionInside;
            IsListItemNoneListStyle = isListItemNoneListStyle;
        }

        /// <summary>
        /// Calculates first line indent of list item.
        /// </summary>
        internal CssLength GetFirstLineIndent(CssLength leftIndent, CssLength textIndent)
        {
            return (IsMarkerPositionInside)
                ? textIndent
                : GetFirstLineIndentForOutsidePosition(leftIndent);
        }

        /// <summary>
        /// Calculates left indent of list item.
        /// </summary>
        internal virtual CssLength GetLeftIndent(CssLength leftIndent, CssLength textIndent)
        {
            return leftIndent + WholeListLeftIndent;
        }

        /// <summary>
        /// Calculates first line indent for outside postion of list item marker.
        /// </summary>
        protected CssLength GetFirstLineIndentForOutsidePosition(CssLength leftIndent)
        {
            return -(MarkerToTextDistance + Padding + (leftIndent - LeftIndent));
        }
        
        internal Paragraph Paragraph { get; }

        internal CssLength LeftIndent { get; }

        internal bool IsListItemNoneListStyle { get; }

        protected CssLength WholeListLeftIndent { get; }

        protected CssLength Padding { get; }

        protected CssLength MarkerToTextDistance { get; }

        protected bool IsMarkerPositionInside { get; }

        /// <summary>
        /// Calculates size of tab stop between list item marker and text.
        /// </summary>
        /// <returns>
        /// Returns tab stop size as <see cref="CssLength"/> or <c>null</c> if tab stop is not needed.
        /// </returns>
        internal abstract CssLength GetTabStop(CssLength leftIndent, CssLength textIndent, ListLevel listLevel);
       
    }
}
