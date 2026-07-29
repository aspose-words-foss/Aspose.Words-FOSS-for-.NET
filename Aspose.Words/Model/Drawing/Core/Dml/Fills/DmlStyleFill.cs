// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// Represents a fill from style.
    /// </summary>
    internal class DmlStyleFill : DmlFill
    {
        internal override DmlFill Clone()
        {
            return new DmlStyleFill();
        }

        /// <summary>
        /// Defines the transparency of a fill. Valid range from 0 to 1, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        public override double Opacity
        {
            get { return base.Opacity; }
            set
            {
                DmlSolidFill solidFill = new DmlSolidFill(DmlColorInternal);
                Parent.SetFill(solidFill);

                solidFill.Opacity = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a parent object is filled.
        /// </summary>
        public override bool On
        {
            get
            {
                ShapeBase shape = Parent as ShapeBase;
                if (shape != null)
                {
                    DmlShapeStyle style = ((IDmlCommonShapePrSource)shape.DmlNode).Style;
                    // WORDSNET-16732 When fill type is StyleFill, we need to check that style actually exists in a shape.
                    if (style == null)
                        return false;

                    // WORDSNET-25919 Check shapes that have no 'a:noFill' tags and have default style fill.
                    // A StyleMatrixIndex value of 0 or 1000 indicates no background for a Shape.
                    // See description in ISO29500 20.1.4.2.10 fillRef (Fill Reference).
                    int styleMatrixIndex = style.FillReference.StyleMatrixIndex;
                    return (styleMatrixIndex != 0) && (styleMatrixIndex != 1000);
                }

                // MS Word using the default fill style for a new charts,
                // which always has the value true for the Visible property.
                // We need to mimic this behavior.
                if (Parent is ChartFormat)
                    return true;

                return false;
            }
            set
            {
                if (Parent is ShapeBase)
                {
                    if (value)
                    {
                        if (!On)
                        {
                            // WORDSNET-25919 For a moment we just set a DmlSolidFill for a shape.
                            // It is not clear what value exactly should be set,
                            // as there is no such property analogue in Word and
                            // there is no any default filling in specification.
                            // So, for a moment, lets postpone this issue.
                            Parent.SetFill(new DmlSolidFill(DmlColorInternal));
                        }
                    }
                    else
                    {
                        Parent.SetFill(new DmlNoFill());
                    }
                }
                else
                {
                    base.On = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets foreground color of the fill.
        /// </summary>
        internal override DmlColor DmlColorInternal
        {
            get
            {
                Shape shape = Parent as Shape;

                if (shape != null)
                {
                    IDmlCommonShapePrSource shapePrSource = (IDmlCommonShapePrSource)shape.DmlNode;
                    if (shapePrSource.Style != null)
                        return shapePrSource.Style.FillReference.Color;
                }


                return null;
            }
            set
            {
                DmlSolidFill solidFill = new DmlSolidFill(value);
                Parent.SetFill(solidFill);
            }
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.StyleFill; }
        }
    }
}
