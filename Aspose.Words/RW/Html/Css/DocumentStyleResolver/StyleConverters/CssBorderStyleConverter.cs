// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS border line style, width and color styles to a model border.
    /// </summary>
    internal static class CssBorderStyleConverter
    {
        /// <summary>
        /// Applies CSS border line style, width and color styles to a model border.
        /// </summary>
        internal static void ToModelBorder(CssDeclarationCollection declarations, BorderType borderType, Border border)
        {
            Debug.Assert(border != null);

            CssBorder cssBorder = CssBorder.CreateBorder(declarations, borderType, false);
            if (!cssBorder.IsUndefined)
            {
                cssBorder.ToModelBorder(border);
            }
        }

        /// <summary>
        /// Applies CSS border line style, width and color styles to a model paragraph.
        /// </summary>
        internal static void ToParagraphFormat(CssDeclarationCollection declarations, ParagraphFormat paragraphFormat)
        {
            CssBorder leftBorder = CssBorder.CreateBorder(declarations, BorderType.Left, false);
            if (!leftBorder.IsUndefined)
            {
                leftBorder.ToModelBorder(paragraphFormat.Borders.Left);
            }

            CssBorder rightBorder = CssBorder.CreateBorder(declarations, BorderType.Right, false);
            if (!rightBorder.IsUndefined)
            {
                rightBorder.ToModelBorder(paragraphFormat.Borders.Right);
            }

            CssBorder topBorder = CssBorder.CreateBorder(declarations, BorderType.Top, false);
            if (!topBorder.IsUndefined)
            {
                topBorder.ToModelBorder(paragraphFormat.Borders.Top);
            }

            CssBorder bottomBorder = CssBorder.CreateBorder(declarations, BorderType.Bottom, false);
            if (!bottomBorder.IsUndefined)
            {
                bottomBorder.ToModelBorder(paragraphFormat.Borders.Bottom);
            }
        }

        /// <summary>
        /// Applies CSS border line style, width and color styles to a model shape.
        /// </summary>
        internal static void ToShape(CssDeclarationCollection declarations, Shape shape)
        {
            if (shape.MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                ToVmlShape(declarations, shape);
            }
            else
            {
                ToDmlShape(declarations, shape);
            }
        }

        /// <summary>
        /// Applies CSS border styles to a VML shape.
        /// </summary>
        private static void ToVmlShape(CssDeclarationCollection declarations, Shape shape)
        {
            Debug.Assert(shape != null);
            Debug.Assert(shape.MarkupLanguage == ShapeMarkupLanguage.Vml);

            ToVmlShapeBorder(declarations, shape, BorderType.Top, ShapeAttr.BorderTop);
            ToVmlShapeBorder(declarations, shape, BorderType.Right, ShapeAttr.BorderRight);
            ToVmlShapeBorder(declarations, shape, BorderType.Bottom, ShapeAttr.BorderBottom);
            ToVmlShapeBorder(declarations, shape, BorderType.Left, ShapeAttr.BorderLeft);
        }

        /// <summary>
        /// Applies CSS border styles to a VML shape border.
        /// </summary>
        private static void ToVmlShapeBorder(
            CssDeclarationCollection declarations,
            Shape shape,
            BorderType borderType,
            int shapeBorderAttr)
        {
            CssBorder cssBorder = CssBorder.CreateBorder(declarations, borderType, false);
            if (!cssBorder.IsUndefined)
            {
                Border modelBorder = new Border();
                cssBorder.ToModelBorder(modelBorder);
                
                shape.SetShapeAttrInternal(shapeBorderAttr, modelBorder);
            }
        }

        /// <summary>
        /// Applies CSS border styles to a DML shape.
        /// </summary>
        private static void ToDmlShape(CssDeclarationCollection declarations, Shape shape)
        {
            Debug.Assert(shape != null);
            Debug.Assert(shape.MarkupLanguage == ShapeMarkupLanguage.Dml);

            // Border properties have no effect on DML shapes, so we apply CSS borders to the shape's stroke.
            // CSS allows to define style of each of four borders individually, but stroke defines same line style for all sides
            // of a shape. We need to somehow collapse styles of four borders and apply them to single stroke. MS Word doesn't
            // apply CSS borders to DML images and we cannot use its behavior as a reference in this case.
            // We decided to implement here the same behavior that MS Word demonstrates when it applies CSS borders to run
            // properties. Same problem arises there, because four CSS borders need to be somehow converted to single run border.
            // In that scenario, MS Word imports run border properties from the corresponding top CSS border properties.
            CssBorder topBorder = CssBorder.CreateBorder(declarations, BorderType.Top, false);
            if (!topBorder.IsUndefined)
            {
                topBorder.ToStroke(shape.Stroke);

                // Need to correct effect extent in order to make all borders visible.
                if (shape.Stroke.On)
                {
                    ShapePr shapePr = shape.ShapePr;

                    int currentExtentLeft = (int)shapePr.FetchAttr(ShapeAttr.DmlEffectExtentLeft);
                    int currentExtentRight = (int)shapePr.FetchAttr(ShapeAttr.DmlEffectExtentRight);
                    int currentExtentTop = (int)shapePr.FetchAttr(ShapeAttr.DmlEffectExtentTop);
                    int currentExtentBottom = (int)shapePr.FetchAttr(ShapeAttr.DmlEffectExtentBottom);

                    int strokeExtent = ConvertUtilCore.PointToEmu(shape.Stroke.Weight);
                    shapePr.SetAttr(ShapeAttr.DmlEffectExtentLeft, currentExtentLeft + strokeExtent);
                    shapePr.SetAttr(ShapeAttr.DmlEffectExtentRight, currentExtentRight + strokeExtent);
                    shapePr.SetAttr(ShapeAttr.DmlEffectExtentTop, currentExtentTop + strokeExtent);
                    shapePr.SetAttr(ShapeAttr.DmlEffectExtentBottom, currentExtentBottom + strokeExtent);
                }
            }
        }
    }
}
