// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2021 by Artem Tsetkhalin

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    internal class CssAwPaddingPropertyDef : CssPaddingIndividualPropertyDefBase
    {
        internal CssAwPaddingPropertyDef(string name)
            : base(name)
        {
            // Empty constructor.
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            double padding;
            if (ConvertValueToPadding(cssValue, out padding))
            {
                switch (Name)
                {
                    case HtmlConstants.AsposePaddingTop:
                        cellFormat.TopPadding = padding;
                        break;
                    case HtmlConstants.AsposePaddingRight:
                        cellFormat.RightPadding = padding;
                        break;
                    case HtmlConstants.AsposePaddingBottom:
                        cellFormat.BottomPadding = padding;
                        break;
                    case HtmlConstants.AsposePaddingLeft:
                        cellFormat.LeftPadding = padding;
                        break;
                    default:
                        Debug.Assert(false, "Unsupported padding type!");
                        break;
                }
            }
        }
    }
}
