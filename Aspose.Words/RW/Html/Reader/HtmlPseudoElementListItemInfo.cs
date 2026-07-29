// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2017 by Nikolay Sezganov

using Aspose.Words.Lists;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents list item imported from :before pseudo-element. 
    /// </summary>
    internal class HtmlPseudoElementListItemInfo : HtmlListItemInfo
    {
        internal HtmlPseudoElementListItemInfo(
            Paragraph paragraph,
            CssLength wholeListLeftIndent,
            CssLength leftIndent,
            CssLength bulletPaddingFromPseudoElement,
            CssLength markerToTextDistance,
            bool isMarkerPositionInside,
            bool isListItemNoneListStyle)
            : base(
                  paragraph,
                  wholeListLeftIndent,
                  leftIndent,
                  bulletPaddingFromPseudoElement,
                  markerToTextDistance,
                  isMarkerPositionInside,
                  isListItemNoneListStyle)
        {
            // Empty constructor.
        }

        internal override CssLength GetTabStop(
            CssLength leftIndent,
            CssLength textIndent,
            ListLevel listLevel)
        {
            if (!IsMarkerPositionInside)
                return null;

            CssLength textLeftIndent = base.GetLeftIndent(leftIndent, textIndent);
            return textLeftIndent + Padding + MarkerToTextDistance + textIndent;
        }

        internal override CssLength GetLeftIndent(CssLength leftIndent, CssLength textIndent)
        {
            CssLength textLeftIndent = base.GetLeftIndent(leftIndent, textIndent);
            if (IsMarkerPositionInside)
                return textLeftIndent;

            return textLeftIndent + Padding + MarkerToTextDistance + textIndent;
        }
    }
}
