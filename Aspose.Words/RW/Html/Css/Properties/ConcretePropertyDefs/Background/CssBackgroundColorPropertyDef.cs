// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background-color' CSS property.
    /// </summary>
    internal class CssBackgroundColorPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBackgroundColorPropertyDef()
            : base(
                "background-color",
                false,
                CssValue.Transparent,
                // <color> | transparent
                CssValueFilter.AnyOf(
                    CssValueFilter.Color,
                    CssValueFilter.Value(CssValue.Transparent)))
        {
            // Empty constructor.
        }

        internal override void ToTable(CssPropertyValue propertyValue, Table table)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            Shading shading = new Shading();
            ToShading(cssValue, shading);
            if (shading.IsVisible)
            {
                // Word applies shading to every row and every cell.
                foreach (Row row in table.Rows)
                {
                    row.TablePr.Shading = shading.Clone();

                    foreach (Cell cell in row.Cells)
                    {
                        // Preserve cell shading processed before.
                        if (!cell.CellPr.Contains(CellAttr.Shading))
                            cell.CellPr.Shading = shading.Clone();
                    }
                }
            }
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            CssValue cssValue = propertyValue.FirstValue;
            ToShading(cssValue, cellFormat.Shading);
        }

        internal override void ToHorizontalRule(CssPropertyValue propertyValue, Shape horizontalRuleShape)
        {
            DrColor color = propertyValue.ParseAsColor();
            if (color != null)
            {
                horizontalRuleShape.FillCore.ColorInternal = color;
            }
        }

        internal override void ToDocument(CssPropertyValue propertyValue, Document document)
        {
            DrColor color = propertyValue.ParseAsColor();
            if (color != null)
            {
                document.PageColor = color.ToNativeColor();
            }
        }

        private static void ToShading(CssValue cssValue, Shading shading)
        {
            DrColor color = cssValue.ParseAsColor();
            if (color != null)
            {
                // Plain background color corresponds to BackgroundPatternColor and TextureNone in the model.
                shading.Texture = TextureIndex.TextureNone;
                shading.BackgroundPatternColorInternal = color;
            }
        }
    }
}
