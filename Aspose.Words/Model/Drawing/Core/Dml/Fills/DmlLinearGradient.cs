// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Alexey Titov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.41 lin (Linear Gradient Fill)
    /// This element specifies a linear gradient.
    /// </summary>
    internal class DmlLinearGradient : IDmlGradient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DmlLinearGradient" /> class.
        /// </summary>
        internal DmlLinearGradient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmlLinearGradient" /> class.
        /// </summary>
        internal DmlLinearGradient(GradientStyle style, GradientVariant variant, IThemeProvider themeProvider)
        {
            Angle = DmlAngle.FromDegrees(GetDegrees(style, variant));
            mIsScaled = true;
            mThemeProvider = themeProvider;
        }

        public IDmlGradient Clone()
        {
            DmlLinearGradient result = new DmlLinearGradient();
            result.Angle = Angle;
            result.IsScaled = IsScaled;
            return result;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlLinearGradient value = (DmlLinearGradient)obj;

            return (value.IsScaled == IsScaled) && object.Equals(value.Angle, Angle);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Angle.Value.GetHashCode();
            hash ^= IsScaled.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Creates collection of gradient stops for any linear style gradient (i.e. Horizontal,
        /// Vertical, DiagonalDown or DiagonalUp).
        /// </summary>
        /// <dev>
        /// The behavior mimics Word VBA.
        /// Note, all linear style gradient stops depend on variant of the gradient only.
        /// Also the order of gradient stops is meaningful. In VBA Word determines gradient variant
        /// and style depending on this order. And as the result the ForeColor and BackColor,
        /// for example, also depends on it.
        /// </dev>
        internal static IList<DmlGradientStop> CreateGradientStops(DmlColor color1, DmlColor color2, GradientVariant variant)
        {
            IList<DmlGradientStop> gradientStops = new List<DmlGradientStop>();
            switch (variant)
            {
                // Variants 1-2 are two-colored, and 3-4 are three-colored in VBA.
                case GradientVariant.Variant1:
                case GradientVariant.Variant2:
                    gradientStops.Add(new DmlGradientStop(0, color1));
                    gradientStops.Add(new DmlGradientStop(1, color2));
                    break;
                case GradientVariant.Variant3:
                    gradientStops.Add(new DmlGradientStop(0, color1));
                    gradientStops.Add(new DmlGradientStop(0.5, color2));
                    gradientStops.Add(new DmlGradientStop(1, color1));
                    break;
                case GradientVariant.Variant4:
                    gradientStops.Add(new DmlGradientStop(0.5, color1));
                    gradientStops.Add(new DmlGradientStop(0, color2));
                    gradientStops.Add(new DmlGradientStop(1, color2));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
            }

            return gradientStops;
        }

        /// <summary>
        /// Returns gradient degree for a specified gradient style and variant.
        /// </summary>
        /// <remarks>
        /// The values are taken from the Word "Format Shape" -> "Fill" dialog.
        /// </remarks>
        private static double GetDegrees(GradientStyle style, GradientVariant variant)
        {
            double degrees;
            switch (style)
            {
                case GradientStyle.Vertical:
                    degrees = 0;
                    break;
                case GradientStyle.DiagonalUp:
                    degrees = 45;
                    break;
                case GradientStyle.Horizontal:
                    degrees = 90;
                    break;
                case GradientStyle.DiagonalDown:
                    degrees = 135;
                    break;
                default:
                    degrees = 0;
                    break;
            }

            if ((variant == GradientVariant.Variant2) || (variant == GradientVariant.Variant4))
                degrees += 180;

            return degrees;
        }


        #region IDmlGradient implementation
        void IDmlGradient.Write(IDmlShapeWriterContext writer, bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;
            string tagName = isTextEffect ? "w14:lin" : "a:lin";
            builder.StartElement(tagName);
            builder.WriteAttribute((isTextEffect ? "w14:ang" : "ang"), Angle.Value);
            builder.WriteAttribute((isTextEffect ? "w14:scaled" : "scaled"), IsScaled);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Returns the gradient variant for the specified fill as an integer value from 1 to 4, or 0 if not defined.
        /// </summary>
        /// <remarks>
        /// See <see cref="GetDegrees"/> for details.
        /// </remarks>
        GradientVariant IDmlGradient.GetGradientVariant(IList<DmlGradientStop> gradientStops)
        {
            GradientVariant variant;

            double degrees = Angle.ValueInDegrees;
            if (MathUtil.AreEqual(degrees, 0.0) ||
                MathUtil.AreEqual(degrees, 45.0) ||
                MathUtil.AreEqual(degrees, 90.0) ||
                MathUtil.AreEqual(degrees, 135.0))
                variant = GradientVariant.Variant1;
            else if (MathUtil.AreEqual(degrees, 180.0) ||
                     MathUtil.AreEqual(degrees, 225.0) ||
                     MathUtil.AreEqual(degrees, 270.0) ||
                     MathUtil.AreEqual(degrees, 315.0))
                variant = GradientVariant.Variant2;
            else
                return GradientVariant.None;

            if (gradientStops != null)
            {
                // If gradientStops count is greater 2, then there can be variants 3 or 4.
                if (gradientStops.Count == 3)
                {
                    // If gradient stops count equals to 3, then VBA checks also corresponding colors(see CreateGradientStop()).
                    DmlGradientStop stopA = gradientStops[(int)variant - 1];
                    DmlGradientStop stopB = gradientStops[2];
                    if (ArgumentUtil.BothAreNotNull(stopA, stopB))
                    {
                        DrColor colorA = stopA.Color.CreateDrColor(mThemeProvider, null);
                        DrColor colorB = stopB.Color.CreateDrColor(mThemeProvider, null);
                        if (colorA.Equals(colorB))
                        {
                            // C++ workaround
                            variant = (GradientVariant)((int)variant + 2);
                        }

                    }
                }
                else if (gradientStops.Count > 3)
                {
                    // If gradient stops count is more than 3, then VBA returns variant 3 or 4 only if all colors are equal.
                    DrColor prevColor = gradientStops[0].Color.CreateDrColor(mThemeProvider, null);
                    for (int i = 1; i < gradientStops.Count - 1; i++)
                    {
                        DrColor curColor = gradientStops[i].Color.CreateDrColor(mThemeProvider, null);
                        if (!prevColor.Equals(curColor))
                            return variant;

                        prevColor = curColor;
                    }

                    // C++ workaround
                    variant = (GradientVariant)((int)variant + 2);
                }
            }

            return variant;
        }

        bool IDmlGradient.AreColorsInReverseOrder
        {
            get { return false; }
        }

        /// <summary>
        /// Gets <see cref="GradientStyle"/> of the gradient.
        /// </summary>
        GradientStyle IDmlGradient.GradientStyle
        {
            get
            {
                double degrees = Angle.ValueInDegrees;
                if (MathUtil.AreEqual(degrees, 0.0) || MathUtil.AreEqual(degrees, 180.0))
                    return GradientStyle.Vertical;

                if (MathUtil.AreEqual(degrees, 45.0) || MathUtil.AreEqual(degrees, 225.0))
                    return GradientStyle.DiagonalUp;

                if (MathUtil.AreEqual(degrees, 90.0) || MathUtil.AreEqual(degrees, 270.0))
                    return GradientStyle.Horizontal;

                if (MathUtil.AreEqual(degrees, 135.0) || MathUtil.AreEqual(degrees, 315.0))
                    return GradientStyle.DiagonalDown;

                return GradientStyle.None;
            }
        }
        #endregion

        /// <summary>
        /// Specifies the direction of color change for the gradient.
        /// To define this angle, let its value be x measured clockwise.
        /// Then ( -sin x, cos x ) is a vector parallel to the line of
        /// constant color in the gradient fill.
        /// </summary>
        internal DmlAngle Angle
        {
            get { return mAngle; }
            set { mAngle = value; }
        }

        /// <summary>
        /// Whether the gradient angle scales with the fill region.
        /// Mathematically, if this flag is true, then the gradient vector ( cos x , sin x )
        /// is scaled by the width (w) and height (h) of the fill region, so that the vector
        /// becomes ( w cos x, h sin x ) (before normalization). Observe that now if the
        /// gradient angle is 45 degrees, the gradient vector is ( w, h ), which goes
        /// from top-left to bottom-right of the fill region. If this flag is false,
        /// the gradient angle is independent of the fill region and is not scaled
        /// using the manipulation described above. So a 45-degree gradient angle always
        /// give a gradient band whose line of constant color is parallel to the vector (1, -1).
        /// </summary>
        internal bool IsScaled
        {
            get { return mIsScaled; }
            set { mIsScaled = value; }
        }

        private DmlAngle mAngle = new DmlAngle(); // For Java
        private bool mIsScaled;
        private readonly IThemeProvider mThemeProvider;
        private const string ValueOutOfRange = "The specified value is out of range.";
    }
}
