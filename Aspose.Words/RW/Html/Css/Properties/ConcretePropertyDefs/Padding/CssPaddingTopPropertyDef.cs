// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'padding-top' CSS property. 
    /// </summary>
    internal class CssPaddingTopPropertyDef : CssPaddingIndividualPropertyDefBase

    {
        internal CssPaddingTopPropertyDef()
            : base("padding-top")
        {
            // Empty constructor.
        }

        internal override void ToTable(CssPropertyValue propertyValue, Table table)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            double padding;
            if (ConvertValueToPadding(cssValue, out padding))
                table.TopPadding = padding;
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            double padding;
            if (ConvertValueToPadding(cssValue, out padding))
                cellFormat.TopPadding = padding;
        }
    }
}
