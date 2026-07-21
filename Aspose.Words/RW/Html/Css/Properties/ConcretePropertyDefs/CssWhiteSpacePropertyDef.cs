// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'white-space' CSS property.
    /// </summary>
    internal class CssWhiteSpacePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssWhiteSpacePropertyDef()
            : base(
                "white-space",
                true,
                CssValue.Normal,
                // normal | pre | nowrap | pre-wrap | pre-line
                CssValueFilter.Values(
                    CssValue.Normal,
                    CssValue.Pre,
                    CssValue.Nowrap,
                    CssValue.PreWrap,
                    CssValue.PreLine))
        {
            // Empty constructor.
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            cellFormat.WrapText = !propertyValue.FirstValue.Equals(CssValue.Nowrap);
        }
    }
}
