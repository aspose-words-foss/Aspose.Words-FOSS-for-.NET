// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2016 by Dmitry Matveenko

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// A simple class that handles DML/VML specifics when reading some properties from a shape.
    /// </summary>
    /// <remarks>
    /// It is used for text box width calculation by the new table grid algorithm.
    /// </remarks>
    internal class ShapeAdaptor
    {
        internal ShapeAdaptor(Shape shape)
        {
            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                DmlShape dmlShape = shape.DmlNode as DmlShape;
                if ((dmlShape == null) || (dmlShape.TextShape == null))
                {
                    Debug.Fail("Unexpected shape type.");
                    WrapMode = TextBoxWrapMode.None;
                    Margins = 0d;
                }
                else
                {
                    // Get from text body properties.
                    DmlTextBodyProperties bodyPr = dmlShape.TextShape.TextBody.Properties;
                    
                    WrapMode = bodyPr.TextWrappingType;
                    int marginsEmu = bodyPr.LeftInset + bodyPr.RightInset;
                    Margins = ConvertUtilCore.EmuToPoint(marginsEmu);
                }
            }
            else
            {
                // TextBox should work for a VML shape.
                TextBox textBox = shape.TextBox;
                
                WrapMode = textBox.TextBoxWrapMode;
                Margins = textBox.InternalMarginLeft + textBox.InternalMarginRight;
            }

            // I had an idea to use shape content rectangle computed in the layout here.
            // There is experimental code to access it from here via PageLayout and ShapeInfo @dmitry.matveenko/tbl_27620_textBox.
            // But eventually I decided against it as it is simpler just to compute the grid in the layout as well.
            ContentWidth = GetContentWidthFromProperties(shape);
            // A condition not to re-calculate fixed layout grids inside text boxes was introduced
            // to preserve grids computed during layout stage.
        }

        /// <summary>
        /// Gets the wrapping mode of the underlying shapes taking DML specifics into account.
        /// </summary>
        internal TextBoxWrapMode WrapMode { get; }

        /// <summary>
        /// Gets the combined left and right margins of the underlying shape in points,
        /// taking DML specifics into account.
        /// </summary>
        internal double Margins { get; }

        /// <summary>
        /// Gets content width computed from shape width, margins and outlines.
        /// </summary>
        /// <remarks>
        /// This is the original simplistic implementation that is used outside layout.
        /// It may be incorrect, especially for complex shapes.
        /// Correct computation is supposed to be done by Rendering.
        /// </remarks>
        internal double ContentWidth { get; }

        private double GetContentWidthFromProperties(Shape shape)
        {
            double contentWidth = shape.Width - Margins;
            Stroke stroke = shape.Stroke;
            if (stroke.On)
                contentWidth -= stroke.Weight;  // half-sum of the two border sides

            return contentWidth;
        }
    }
}
