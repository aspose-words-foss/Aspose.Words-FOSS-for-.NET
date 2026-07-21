// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'writing-mode' CSS property.
    /// </summary>
    internal class CssWritingModePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssWritingModePropertyDef()
            : base(
                "writing-mode",
                true,
                CssValue.HorizontalTb,
                // lr-tb | rl-tb | tb-rl | bt-rl | tb-lr | bt-lr | horizontal-tb | vertical-rl | vertical-lr
                CssValueFilter.Values(
                    CssValue.HorizontalTb,
                    CssValue.VerticalRl,
                    CssValue.VerticalLr,
                    CssValue.LrTb,
                    CssValue.RlTb,
                    CssValue.TbRl,
                    CssValue.BtRl,
                    CssValue.TbLr,
                    CssValue.BtLr))
        {
            // Empty constructor.
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            if (cssValue.ValueType == CssValueType.Identifier)
                cellFormat.Orientation = CssUtil.CssToTextOrientation(((CssIdentifierValue)cssValue).Value);
            else
                Debug.Assert(false);
        }
    }
}
