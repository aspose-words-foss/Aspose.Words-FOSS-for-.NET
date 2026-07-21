// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Alexey Titov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.46 path (Path Gradient)
    /// This element defines that a gradient fill follows a path vs. a linear line.
    /// </summary>
    internal class DmlPathGradient : IDmlGradient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DmlPathGradient" /> class.
        /// </summary>
        internal DmlPathGradient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmlPathGradient" /> class.
        /// </summary>
        internal DmlPathGradient(GradientStyle style, GradientVariant variant, IThemeProvider themeProvider)
        {
            mFillToRectangle = new DmlPercentageOffsetRectangle(GetFillToRectangle(style, variant));
            mPath = DmlPathShadeType.Rectangle;
            mThemeProvider = themeProvider;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlPathGradient value = (DmlPathGradient)obj;

            return (value.Path == Path) && object.Equals(value.FillToRectangle, FillToRectangle);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= Path.GetHashCode();
            hash ^= FillToRectangle.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Gets model FillType for the shape of the path.
        /// </summary>
        internal FillTypeCore GetPathFillType()
        {
            switch (mPath)
            {
                case DmlPathShadeType.Shape:
                case DmlPathShadeType.Circle:
                    return FillTypeCore.ShadeShape;
                case DmlPathShadeType.Rectangle:
                    return FillTypeCore.ShadeCenter;
                default:
                    return FillTypeCore.Shade;
            }
        }

        /// <summary>
        /// Creates collection of gradient stops for <see cref="Aspose.Words.Drawing.GradientStyle.FromCenter"/> gradient.
        /// </summary>
        /// <dev>
        /// The order of gradient stops is meaningful. In VBA Word determines gradient variant and
        /// style depending on this order. And as the result the ForeColor and BackColor,
        /// for example, also depends on it.
        /// </dev>
        internal static IList<DmlGradientStop> CreateFromCenterGradientStops(DmlColor color1, DmlColor color2,
            GradientVariant variant)
        {
            IList<DmlGradientStop> gradientStops = new List<DmlGradientStop>();
            switch (variant)
            {
                case GradientVariant.Variant1:
                    gradientStops.Add(new DmlGradientStop(0, color1));
                    gradientStops.Add(new DmlGradientStop(1, color2));
                    break;
                case GradientVariant.Variant2:
                    gradientStops.Add(new DmlGradientStop(1, color1));
                    gradientStops.Add(new DmlGradientStop(0, color2));
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
        /// Creates collection of gradient stops for <see cref="Aspose.Words.Drawing.GradientStyle.FromCorner"/> gradient.
        /// </summary>
        /// <dev>
        /// The order of gradient stops is meaningful. In VBA Word determines gradient variant and
        /// style depending on this order. And as the result the ForeColor and BackColor,
        /// for example, also depends on it.
        /// </dev>
        internal static IList<DmlGradientStop> CreateFromCornerGradientStops(DmlColor color1, DmlColor color2)
        {
            IList<DmlGradientStop> gradientStops = new List<DmlGradientStop>();
            gradientStops.Add(new DmlGradientStop(0, color1));
            gradientStops.Add(new DmlGradientStop(1, color2));

            return gradientStops;
        }

        /// <summary>
        /// Creates tile rectangle for <see cref="Aspose.Words.Drawing.GradientStyle.FromCorner"/> gradient.
        /// </summary>
        internal static DmlPercentageOffsetRectangle CreateFromCornerTileRect(GradientVariant variant)
        {
            switch (variant)
            {
                case GradientVariant.Variant1:
                    return new DmlPercentageOffsetRectangle(-1, -1, 0, 0);
                case GradientVariant.Variant2:
                    return new DmlPercentageOffsetRectangle(0, -1, -1, 0);
                case GradientVariant.Variant3:
                    return new DmlPercentageOffsetRectangle(-1, 0, 0, -1);
                case GradientVariant.Variant4:
                    return new DmlPercentageOffsetRectangle(0, 0, -1, -1);
                default:
                    throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
            }
        }

        private static DrMatrix CreateFitMatrix(RectangleF originalBoundingBox, RectangleF newBoundingBox)
        {
            DrMatrix matrix = new DrMatrix();
            matrix.Translate(-originalBoundingBox.X, -originalBoundingBox.Y, MatrixOrder.Append);

            float scaleX = originalBoundingBox.Width > 0 ? newBoundingBox.Width / originalBoundingBox.Width : 1f;
            float scaleY = originalBoundingBox.Height > 0 ? newBoundingBox.Height / originalBoundingBox.Height : 1f;

            matrix.Scale(scaleX, scaleY, MatrixOrder.Append);

            matrix.Translate(newBoundingBox.X, newBoundingBox.Y, MatrixOrder.Append);
            return matrix;
        }

        /// <summary>
        /// Gets gradient variant for 'FromCorner' gradient.
        /// </summary>
        private GradientVariant GetFromCornerGradientVariant()
        {
            if (mFillToRectangle.Equals(gCornerRect1))
                return GradientVariant.Variant1;

            if (mFillToRectangle.Equals(gCornerRect2))
                return GradientVariant.Variant2;

            if (mFillToRectangle.Equals(gCornerRect3))
                return GradientVariant.Variant3;

            if (mFillToRectangle.Equals(gCornerRect4))
                return GradientVariant.Variant4;

            return GradientVariant.None;
        }

        /// <summary>
        /// Gets gradient variant for 'FromCenter' gradient.
        /// </summary>
        /// <remarks>
        /// Word determines only variants 1 and 3 in VBA for 'FromCenter' gradient.
        /// The variant 3 is when gradient stops count is either equal to 3 and first and last colors are equal,
        /// or gradient stops more than 3 and they are all equal.
        /// </remarks>
        private GradientVariant GetFromCenterGradientVariant(IList<DmlGradientStop> gradientStops)
        {
            Debug.Assert(mFillToRectangle.Equals(gCenterRect));

            if ((gradientStops == null) || (gradientStops.Count < 3))
                return GradientVariant.Variant1;

            if (gradientStops.Count == 3)
            {
                // If there are three gradient stops, then this can be variant 3 or 4.
                // First check if this is variant 3.
                DmlGradientStop stopA = gradientStops[0];
                DmlGradientStop stopB = gradientStops[2];
                if (ArgumentUtil.BothAreNotNull(stopA, stopB))
                {
                    DrColor colorA = stopA.Color.CreateDrColor(mThemeProvider, null);
                    DrColor colorB = stopB.Color.CreateDrColor(mThemeProvider, null);
                    if (colorA.Equals(colorB))
                        return GradientVariant.Variant3;

                    // If we here, then this is actually can be variant 4.
                    // However, Word returns 3 and we should mimic such behavior.
                    stopA = gradientStops[1];
                    if (stopA != null)
                    {
                        colorA = stopA.Color.CreateDrColor(mThemeProvider, null);
                        if (colorA.Equals(colorB))
                            return GradientVariant.Variant3;
                    }
                }

                // There are no equal colors, so this is variant 1.
                return GradientVariant.Variant1;
            }

            // If we here, then gradient stops count greater 3.
            // If all colors are equal, then this is variant 3, otherwise this is variant 1.
            DrColor prevColor = gradientStops[0].Color.CreateDrColor(mThemeProvider, null);
            for (int i = 1; i < gradientStops.Count - 1; i++)
            {
                DrColor curColor = gradientStops[i].Color.CreateDrColor(mThemeProvider, null);
                if (!prevColor.Equals(curColor))
                    return GradientVariant.Variant1;

                prevColor = curColor;
            }

            return GradientVariant.Variant3;
        }

        /// <summary>
        /// Returns one of the predefined static <see cref="DmlPercentageOffsetRectangle"/> corresponded
        /// to a particular <see cref="GradientStyle"/> and Variant.
        /// </summary>
        /// <remarks>
        /// Note, this should be immutable.
        /// </remarks>
        private static DmlPercentageOffsetRectangle GetFillToRectangle(GradientStyle style, GradientVariant variant)
        {
            switch (style)
            {
                case GradientStyle.FromCorner:
                {
                    switch (variant)
                    {
                        case GradientVariant.Variant1:
                            return gCornerRect1;
                        case GradientVariant.Variant2:
                            return gCornerRect2;
                        case GradientVariant.Variant3:
                            return gCornerRect3;
                        case GradientVariant.Variant4:
                            return gCornerRect4;
                        default:
                            throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
                    }
                }
                case GradientStyle.FromCenter:
                    return gCenterRect;
                default:
                    throw new ArgumentOutOfRangeException("style", style, ValueOutOfRange);
            }
        }

        #region IDmlGradient implementation
        IDmlGradient IDmlGradient.Clone()
        {
            DmlPathGradient result = new DmlPathGradient();
            result.Path = Path;
            result.FillToRectangle = FillToRectangle.Clone();
            return result;
        }

        void IDmlGradient.Write(IDmlShapeWriterContext writer, bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string tagName = isTextEffect ? "w14:path" : "a:path";
            builder.StartElement(tagName);
            builder.WriteAttribute((isTextEffect ? "w14:path" : "path"), DmlEnum.PathShadeTypeToDml(Path));
            DmlWriterUtil.WritePercentageOffsetRectangle(FillToRectangle, (isTextEffect ? "w14" : "a"), "fillToRect",
                false, builder, isIsoStrict);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Returns the gradient variant for the specified fill as an integer value from 1 to 4 or 0 if not defined.
        /// </summary>
        GradientVariant IDmlGradient.GetGradientVariant(IList<DmlGradientStop> gradientStops)
        {
            if (mFillToRectangle != null)
            {
                return mFillToRectangle.Equals(gCenterRect)
                    ? GetFromCenterGradientVariant(gradientStops)
                    : GetFromCornerGradientVariant();
            }

            return GradientVariant.None;
        }

        bool IDmlGradient.AreColorsInReverseOrder
        {
            get { return true; }
        }

        /// <summary>
        /// Gets <see cref="GradientStyle"/> of the gradient.
        /// </summary>
        GradientStyle IDmlGradient.GradientStyle
        {
            get
            {
                if (mFillToRectangle != null)
                {
                    if (mFillToRectangle.Equals(gCenterRect))
                        return GradientStyle.FromCenter;

                    if (mFillToRectangle.Equals(gCornerRect1) ||
                        mFillToRectangle.Equals(gCornerRect2) ||
                        mFillToRectangle.Equals(gCornerRect3) ||
                        mFillToRectangle.Equals(gCornerRect4))
                        return GradientStyle.FromCorner;
                }

                return GradientStyle.None;
            }
        }
        #endregion

        /// <summary>
        /// 20.1.8.31 fillToRect (Fill To Rectangle)
        /// This element defines the "focus" rectangle for the center shade,
        /// specified relative to the fill tile rectangle. The center shade fills
        /// the entire tile except the margins specified by each attribute.
        /// Each edge of the center shade rectangle is defined by a percentage offset
        /// from the corresponding edge of the tile rectangle. A positive percentage
        /// specifies an inset, while a negative percentage specifies an outset.
        /// [Note: For example, a left offset of 25% specifies that the left edge
        /// of the center shade rectangle is located to the right of the tile rectangle's left
        /// edge by an amount equal to 25% of the tile rectangle's width. end note]
        /// </summary>
        internal DmlPercentageOffsetRectangle FillToRectangle
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mFillToRectangle == null)
                    mFillToRectangle = new DmlPercentageOffsetRectangle();
                return mFillToRectangle;
            }
            set { mFillToRectangle = value; }
        }

        /// <summary>
        /// Specifies the shape of the path to follow.
        /// </summary>
        internal DmlPathShadeType Path
        {
            get { return mPath; }
            set { mPath = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlPercentageOffsetRectangle mFillToRectangle;
        private DmlPathShadeType mPath;
        private readonly IThemeProvider mThemeProvider;
        private const string ValueOutOfRange = "The specified value is out of range.";

        /// <summary>
        /// Predefined <see cref="FillToRectangle"/> corresponded to a particular <see cref="GradientStyle"/> and Variant.
        /// </summary>
        private static readonly DmlPercentageOffsetRectangle gCornerRect1 = new DmlPercentageOffsetRectangle(0.0, 0.0, 1.0, 1.0);
        private static readonly DmlPercentageOffsetRectangle gCornerRect2 = new DmlPercentageOffsetRectangle(1.0, 0.0, 0.0, 1.0);
        private static readonly DmlPercentageOffsetRectangle gCornerRect3 = new DmlPercentageOffsetRectangle(0.0, 1.0, 1.0, 0.0);
        private static readonly DmlPercentageOffsetRectangle gCornerRect4 = new DmlPercentageOffsetRectangle(1.0, 1.0, 0.0, 0.0);
        private static readonly DmlPercentageOffsetRectangle gCenterRect = new DmlPercentageOffsetRectangle(0.5, 0.5, 0.5, 0.5);
    }
}
