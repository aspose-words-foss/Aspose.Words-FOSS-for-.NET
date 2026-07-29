// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'vertical-align' CSS property.
    /// </summary>
    internal class CssVerticalAlignPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssVerticalAlignPropertyDef()
            : base(
                  "vertical-align",
                  false,
                  CssValue.Baseline,
                  // baseline | sub | super | top | text-top | middle | bottom | text-bottom | <percentage> | <length>
                  CssValueFilter.AnyOf(
                      CssValueFilter.Values(
                            CssValue.Baseline,
                            CssValue.Sub,
                            CssValue.Super,
                            CssValue.Top,
                            CssValue.TextTop,
                            CssValue.Middle,
                            CssValue.Bottom,
                            CssValue.TextBottom),
                      CssValueFilter.Percentage,
                      CssValueFilter.Length))
        {
            // Empty constructor.
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            switch (cssValue.ValueType)
            {
                case CssValueType.Length:
                case CssValueType.Percentage:
                    break;
                case CssValueType.Identifier:
                {
                    // WORDSNET-11645 MS Word imports baseline as bottom though this doesn't provide proper look.
                    // Don't see how we can do better so here we duplicates MS Word's behavior.
                    if (cssValue.Equals(CssValue.Baseline))
                        cellFormat.VerticalAlignment = CellVerticalAlignment.Bottom;
                    else if (cssValue.Equals(CssValue.Top))
                        cellFormat.VerticalAlignment = CellVerticalAlignment.Top;
                    else if (cssValue.Equals(CssValue.Center))
                        cellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    else if (cssValue.Equals(CssValue.Bottom))
                        cellFormat.VerticalAlignment = CellVerticalAlignment.Bottom;
                    else
                        cellFormat.VerticalAlignment = CellVerticalAlignment.Center;
                    break;
                }
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }
}
