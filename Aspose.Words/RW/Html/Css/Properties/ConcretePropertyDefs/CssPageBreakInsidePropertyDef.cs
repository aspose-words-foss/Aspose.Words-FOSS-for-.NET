// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'page-break-inside' CSS property.
    /// </summary>
    internal class CssPageBreakInsidePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssPageBreakInsidePropertyDef()
            : base(
                "page-break-inside",
                false,
                CssValue.Auto,
                CssValueFilter.Values(CssValue.Avoid, CssValue.Auto))
        {
            // Empty constructor.
        }

        internal override void ToRow(CssPropertyValue propertyValue, Row row)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            row.RowFormat.AllowBreakAcrossPages = cssValue.Equals(CssValue.Auto);
        }
    }
}
