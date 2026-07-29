// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'float' CSS property.
    /// </summary>
    internal class CssFloatPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFloatPropertyDef()
            : base(
                "float",
                false,
                CssValue.None,
                // left | right | none
                CssValueFilter.Values(CssValue.Left, CssValue.Right, CssValue.None))
        {
            // Empty constructor.
        }

        internal override void ToTable(CssPropertyValue propertyValue, Table table)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            if (cssValue.Equals(CssValue.Right))
                table.Alignment = TableAlignment.Right;
            else if (cssValue.Equals(CssValue.Left))
                table.Alignment = TableAlignment.Left;

            // WORDSNET-8410 Tables that have non-default 'float' property value should be imported as floating tables.
            table.TextWrapping = TextWrapping.Around;
        }

        internal override void ToShape(CssPropertyValue propertyValue, Shape shape)
        {
            if (shape.IsHorizontalRule)
                return;

            if (propertyValue.Equals(CssValue.Left))
            {
                shape.WrapType = WrapType.Square;
                shape.SetShapeAttrInternal(ShapeAttr.HorizontalAlignment, HorizontalAlignment.Left);
                shape.RelativeVerticalPosition = RelativeVerticalPosition.Line;
                shape.AllowOverlap = false;
            }
            else if (propertyValue.Equals(CssValue.Right))
            {
                shape.WrapType = WrapType.Square;
                shape.SetShapeAttrInternal(ShapeAttr.HorizontalAlignment, HorizontalAlignment.Right);
                shape.RelativeVerticalPosition = RelativeVerticalPosition.Line;
                shape.AllowOverlap = false;
            }
        }
    }
}
