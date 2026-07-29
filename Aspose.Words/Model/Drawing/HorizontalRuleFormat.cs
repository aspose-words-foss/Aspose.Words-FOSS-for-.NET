// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2019 by Artem Ptitsin

using System;
using System.Drawing;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents horizontal rule formatting.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// WORDSNET-18182 This class has been added to provide public horizontal rule properties.
    /// </dev>
    public class HorizontalRuleFormat
    {
        internal HorizontalRuleFormat(Shape shape)
        {
            mShape = shape;
        }

        /// <summary>
        /// Sets width percent value to the horizontal rule.
        /// </summary>
        private void SetWidthPercentInternal(double value)
        {
            value = ArgumentUtil.ValidateRange(value, 1, 1, 100, 100, true, "WidthPercent");
            mShape.HorizontalRule.Percent = value;
        }

        /// <summary>
        /// Sets height value to the horizontal rule.
        /// </summary>
        private void SetHeightInternal(double value)
        {
            value = ArgumentUtil.ValidateRange(value, 0, 0, 1584, 1584, true, "Height");
            mShape.Height = value;
        }

        /// <summary>
        /// Gets or sets the length of the specified horizontal rule expressed as a percentage of the window width.
        /// </summary>
        /// <remarks>
        /// <p>Valid values ​​range from 1 to 100 inclusive.</p>
        /// <p>The default value is 100.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public double WidthPercent
        {
            get { return mShape.HorizontalRule.Percent; }
            set { SetWidthPercentInternal(value); }
        }

        /// <summary>
        /// Gets or sets the height of the horizontal rule.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Aspose.Words.Drawing.ShapeBase.Height"/> property.</p>
        /// <p>Valid values ​​range from 0 to 1584 inclusive.</p>
        /// <p>The default value is 1.5.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public double Height
        {
            get { return mShape.Height; }
            set { SetHeightInternal(value); }
        }

        /// <summary>
        /// Indicates the presence of 3D shading for the horizontal rule.
        /// If <c>true</c>, then the horizontal rule is without 3D shading and solid color is used.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool NoShade
        {
            get { return mShape.HorizontalRule.NoShade; }
            set { mShape.HorizontalRule.NoShade = value; }
        }

        /// <summary>
        /// Gets or sets the brush color that fills the horizontal rule.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Fill.Color"/> property.</p>
        /// <p>The default value is
        /// <see cref="System.Drawing.Color.Gray"/>.
        /// </p>
        /// </remarks>
        public Color Color
        {
            get { return mShape.FillColor; }
            set { mShape.FillColor = value; }
        }

        /// <summary>
        /// Gets or sets the alignment of the horizontal rule.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="HorizontalRuleAlignment.Left"/>.</p>
        /// </remarks>
        public HorizontalRuleAlignment Alignment
        {
            get { return mShape.HorizontalRule.Align; }
            set { mShape.HorizontalRule.Align = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Shape mShape;
    }
}
