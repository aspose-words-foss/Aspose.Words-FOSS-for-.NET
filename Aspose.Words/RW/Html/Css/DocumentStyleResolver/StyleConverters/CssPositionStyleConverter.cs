// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/12/2013 by Alexey Butalov

using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS position style to a model.
    /// </summary>
    internal static class CssPositionStyleConverter
    {
        /// <summary>
        /// Applies CSS width style to a shape.
        /// </summary>
        internal static void ToShape(CssDeclarationCollection declarations, Shape shape, CssBoxModel boxModel)
        {
            CssDeclaration positionDeclaration = declarations["position"];
            if (positionDeclaration == null)
                return;

            if (positionDeclaration.Value.Equals(CssValue.Absolute))
            {
                // WORDSNET-9459 Image is placed incorrectly when imported from HTML.
                // To understand the following code please see how absolute positioned shapes are saved to HTML
                // in HtmlStyleWriter.AddFloatingStyle method.
                shape.WrapType = WrapType.None;
                double zIndexValue = declarations.GetNumber("z-index");
                if (!MathUtil.IsMinValue(zIndexValue))
                {
                    // The "z-index" value must be clamped to int32 limits in order to avoid integer overflow that would change
                    // the sign and thus the position of the image relative to text (behind or in front of text).
                    // The values that are close to the limits are also not supported for the same reason.
                    zIndexValue = MathUtil.FitToRange(zIndexValue, int.MinValue + 2, int.MaxValue - 1);
                    int zOrder = DoublePal.ToInt(zIndexValue);

                    // In CSS, the "z-index" of text is zero. Consequently, all objects with "z-index" values less than zero
                    // are "behind text", and all objects with other "z-index" values are "in front of text".
                    shape.BehindText = zOrder < 0;

                    // Since we have already correctly set the "BehindText" flag, we can now convert the "z-index" value
                    // into a positive "ZOrder" value.
                    if (zOrder < 0)
                    {
                        zOrder += int.MaxValue;
                    }
                    shape.ZOrder = zOrder;
                }

                // WORDSNET-10125 CSS 2.1 specification says that if the element has 'position: absolute', the containing block
                // is established by the nearest ancestor with a 'position' of 'absolute', 'relative' or 'fixed'.
                // If there is no such ancestor, the containing block is the initial containing block.
                // Here (for simplicity now) we act like the only ancestor that meets the criteria is the initial containing block.
                double left = declarations.GetLength("left");
                double shapeLeft = !MathUtil.IsMinValue(left) ? left : boxModel.Left.Value;

                double marginLeft = declarations.GetLength("margin-left");
                if (!MathUtil.IsMinValue(marginLeft))
                    shapeLeft += marginLeft;
                shape.Left = shapeLeft;

                double shapeTop = boxModel.Top.Value;
                double marginTop = declarations.GetLength("margin-top");
                if (!MathUtil.IsMinValue(marginTop))
                    shapeTop += marginTop;
                shape.Top = shapeTop;
            }
        }
    }
}
