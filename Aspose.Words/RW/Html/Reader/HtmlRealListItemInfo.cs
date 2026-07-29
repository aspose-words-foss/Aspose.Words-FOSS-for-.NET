// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2017 by Nikolay Sezganov

using Aspose.Words.Lists;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents real list item. 
    /// </summary>
    internal class HtmlRealListItemInfo : HtmlListItemInfo
    {
        internal HtmlRealListItemInfo(
            Paragraph paragraph,
            CssLength wholeListLeftIndent,
            CssLength leftIndent,
            CssLength padding,
            CssLength markerToTextDistance,
            bool isMarkerPositionInside,
            bool isListItemNoneListStyle)
            : base(
                  paragraph,
                  wholeListLeftIndent,
                  leftIndent,
                  padding,
                  markerToTextDistance,
                  isMarkerPositionInside,
                  isListItemNoneListStyle)
        {
        }

        internal override CssLength GetTabStop(
            CssLength leftIndent,
            CssLength textIndent,
            ListLevel listLevel)
        {
            if (IsMarkerPositionInside && (listLevel != null))
            {
                if (listLevel.TrailingCharacter == ListTrailingCharacter.Tab)
                {
                    return WholeListLeftIndent + leftIndent + MarkerToTextDistance + textIndent;
                }
            }
            else if (!MathUtil.IsZero(textIndent.Value))
            {
                return WholeListLeftIndent + leftIndent + textIndent;
            }

            return null;
        }
    }
}
