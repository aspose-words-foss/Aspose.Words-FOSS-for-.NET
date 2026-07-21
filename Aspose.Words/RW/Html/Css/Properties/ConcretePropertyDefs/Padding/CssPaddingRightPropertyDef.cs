// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'padding-right' CSS property. 
    /// </summary>
    internal class CssPaddingRightPropertyDef : CssPaddingIndividualPropertyDefBase
    {
        internal CssPaddingRightPropertyDef() :
            base("padding-right")
        {
            // Empty constructor.
        }

        internal override void ToTable(CssPropertyValue propertyValue, Table table)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            double padding;
            if (ConvertValueToPadding(cssValue, out padding))
                table.RightPadding = padding;
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            double padding;
            if (ConvertValueToPadding(cssValue, out padding))
                cellFormat.RightPadding = padding;
        }
    }
}
