// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2011 by Alexey Titov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// 20.1.8.33 gradFill (Gradient Fill)
    /// This element defines a gradient fill.
    /// A gradient fill is a fill which is characterized by a smooth gradual
    /// transition from one color to the next. At its simplest, it is a fill
    /// which transitions between two colors; or more generally, it can be a
    /// transition of any number of colors.
    /// The desired transition colors and locations are defined in the gradient
    /// stop list (gsLst) child element.
    /// </summary>
    internal class DmlGradientFill : DmlFill
    {
        /// <summary>
        /// Creates instance of a <see cref="DmlGradientFill"/> class by default.
        /// </summary>
        internal DmlGradientFill()
        {
        }

        /// <summary>
        /// Creates instance of a <see cref="DmlGradientFill"/> class with specified gradient colors, style and variant.
        /// </summary>
        /// <remarks>
        /// Note, there can be 2 or 3 gradient colors depending on gradient style and variant.
        /// If the gradient is 3-colored, then third color is the same as one of the specified color1 or color2.
        /// </remarks>
        internal DmlGradientFill(DmlColor color1, DmlColor color2, GradientStyle style, GradientVariant variant,
            IThemeProvider themeProvider)
        {
            switch (style)
            {
                case GradientStyle.Horizontal:
                case GradientStyle.Vertical:
                case GradientStyle.DiagonalDown:
                case GradientStyle.DiagonalUp:
                {
                    Gradient = new DmlLinearGradient(style, variant, themeProvider);
                    GradientStops = DmlLinearGradient.CreateGradientStops(color1, color2, variant);
                    break;
                }
                case GradientStyle.FromCorner:
                {
                    Gradient = new DmlPathGradient(style, variant, themeProvider);
                    GradientStops = DmlPathGradient.CreateFromCornerGradientStops(color1, color2);
                    TileRectangle = DmlPathGradient.CreateFromCornerTileRect(variant);
                    break;
                }
                case GradientStyle.FromCenter:
                    Gradient = new DmlPathGradient(style, variant, themeProvider);
                    GradientStops = DmlPathGradient.CreateFromCenterGradientStops(color1, color2, variant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("style", style, ValueOutOfRange);
            }
        }

        /// <summary>
        /// Set style color to any color placeholders used in the fill.
        /// </summary>
        public override void ApplyStyleColor(DmlColor styleColor)
        {
            foreach (DmlGradientStop gradientStop in GradientStops)
            {
                gradientStop.Color.ApplyStyleColor(styleColor);
            }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlGradientFill value = (DmlGradientFill)obj;

            return (value.RotateWithShape == RotateWithShape) &&
                   (value.TileFlipMode == TileFlipMode) &&
                   object.Equals(value.TileRectangle, TileRectangle) &&
                   object.Equals(value.Gradient, Gradient) &&
                   ListUtil.CheckAreEquals(value.GradientStops, GradientStops);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= TileRectangle.GetHashCode();
            hash ^= TileFlipMode.GetHashCode();
            hash ^= RotateWithShape.GetHashCode();
            hash ^= Gradient.GetHashCode();
            foreach (DmlGradientStop stop in GradientStops)
                hash ^= stop.GetHashCode();
            return hash;
        }

        internal override DmlFill Clone()
        {
            DmlGradientFill fill = new DmlGradientFill();
            fill.RotateWithShape = RotateWithShape;
            fill.TileFlipMode = TileFlipMode;
            foreach (DmlGradientStop gradientStop in GradientStops)
            {
                fill.GradientStops.Add(gradientStop.Clone());
            }

            fill.Gradient = Gradient.Clone();
            fill.TileRectangle = TileRectangle.Clone();
            return fill;
        }

        /// <summary>
        /// Resolves theme colors to a concrete RGB colors using a specified theme provider.
        /// </summary>
        internal override void ResolveThemeColors(IThemeProvider themeProvider)
        {
            if (mGradientStops != null)
            {
                foreach (DmlGradientStop gradientStop in mGradientStops)
                {
                    if (gradientStop.Color.ColorType == DmlColorType.SchemeColor)
                        gradientStop.Color = ((DmlSchemeColor)gradientStop.Color).Resolve(themeProvider);
                }
            }
        }

        /// <summary>Gets the array of custom gradient colors.</summary>
        /// <remarks>
        /// <p>The default value is null.</p>
        /// </remarks>
        public override GradientColor[] GradientColors
        {
            get { return DmlUtilCore.DmlToGradientStops(GradientStopsSortedByPosition, ThemeProvider); }
        }

        /// <summary>
        /// Gets or sets fill color opacity.
        /// </summary>
        public override double Opacity
        {
            get
            {
                // IN. When colors in gradient stops have different transparencies, Word VBA returns value 0x8000000
                // scaled to [0, 1] range. Looks like this is denoting that transparency is mixed. Not sure we need this,
                // so lets return transparency of the first gradient stop, for a while.
                return base.Opacity;
            }
            set
            {
                // Setting transparency in VBA updates colors in all gradient stops.
                foreach (DmlGradientStop gradientStop in GradientStops)
                {
                    if (gradientStop.Color != null)
                        gradientStop.Color.UpdateAlpha(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating the fill should be rotated with parent object.
        /// </summary>
        public override bool RotateWithShape
        {
            get { return mRotateWithShape; }
            set { mRotateWithShape = value; }
        }

        /// <summary>
        /// Gets or sets the type of fill.
        /// </summary>
        public override FillTypeCore FillType
        {
            get
            {
                DmlPathGradient pathGradient = Gradient as DmlPathGradient;
                if (pathGradient != null)
                    return pathGradient.GetPathFillType();

                DmlLinearGradient linearGradient = Gradient as DmlLinearGradient;
                if (linearGradient != null)
                    return (linearGradient.IsScaled) ? FillTypeCore.ShadeScale : FillTypeCore.ShadeUnscale;

                return FillTypeCore.Shade;
            }
        }

        public override double Angle
        {
            get
            {
                DmlLinearGradient linearGradient = Gradient as DmlLinearGradient;
                return (linearGradient != null)
                    ? linearGradient.Angle.ValueInDegrees
                    : 0;
            }
            set
            {
                DmlLinearGradient linearGradient = Gradient as DmlLinearGradient;
                if (linearGradient != null)
                    linearGradient.Angle = DmlAngle.FromDegrees(value);
            }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientstyle
        /// </summary>
        public override GradientStyle GradientStyle
        {
            get
            {
                if (mGradient == null)
                    return GradientStyle.None;

                return mGradient.GradientStyle;
            }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills or 0 if not defined.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientvariant
        /// </summary>
        public override GradientVariant GradientVariant
        {
            get
            {
                if ((mGradient == null) || (mGradientStops == null))
                    return GradientVariant.None;

                return mGradient.GetGradientVariant(mGradientStops);
            }
        }

        /// <summary>
        /// Gets or sets foreground color of the fill.
        /// </summary>
        internal override DmlColor DmlColorInternal
        {
            get
            {
                return (FirstGradientStop == null) ? null : FirstGradientStop.Color;
            }
            set
            {
                if (FirstGradientStop != null)
                    FirstGradientStop.Color = value;
            }
        }

        /// <summary>
        /// Gets or sets background color of the fill.
        /// </summary>
        internal override DmlColor DmlColor2Internal
        {
            get
            {
                return (LastGradientStop == null) ? null : LastGradientStop.Color;
            }
            set
            {
                if (LastGradientStop != null)
                    LastGradientStop.Color = value;
            }
        }

        internal override DmlFillType DmlFillType
        {
            get { return DmlFillType.GradientFill; }
        }

        internal DmlTileFlipMode TileFlipMode
        {
            get { return mTileFlipMode; }
            set { mTileFlipMode = value; }
        }

        internal IList<DmlGradientStop> GradientStops
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mGradientStops == null)
                    mGradientStops = new List<DmlGradientStop>();
                return mGradientStops;
            }
            set { mGradientStops = value; }
        }

        internal IDmlGradient Gradient
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mGradient == null)
                    mGradient = new DmlLinearGradient();
                return mGradient;
            }
            set
            {
                mGradient = value;
            }
        }

        /// <summary>
        /// 20.1.8.59 tileRect (Tile Rectangle)
        /// This element specifies a rectangular region of the shape to which
        /// the gradient is applied. This region is then tiled across the remaining
        /// area of the shape to complete the fill. The tile rectangle is defined by
        /// percentage offsets from the sides of the shape's bounding box.
        /// Each edge of the tile rectangle is defined by a percentage offset from the
        /// corresponding edge of the bounding box. A positive percentage specifies an inset,
        /// while a negative percentage specifies an outset. [Note: For example, a left
        /// offset of 25% specifies that the left edge of the tile rectangle is located
        /// to the right of the bounding box's left edge by an amount equal to 25%
        /// of the bounding box's width.
        /// </summary>
        internal DmlPercentageOffsetRectangle TileRectangle
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get
            {
                if (mTileRectangle == null)
                    mTileRectangle = new DmlPercentageOffsetRectangle();
                return mTileRectangle;
            }
            set { mTileRectangle = value; }
        }

        /// <summary>
        /// Gets gradient stop collection.
        /// </summary>
        /// <remarks>
        /// Used as facade in <see cref="Fill"/> object for internal <see cref="GradientStops"/> collection.
        /// </remarks>
        internal GradientStopCollection GradientStopCollection
        {
            get
            {
                if (mGradientStopCollection == null)
                    mGradientStopCollection = new GradientStopCollection(this, ThemeProvider);

                return mGradientStopCollection;
            }
        }

        private DmlGradientStop FirstGradientStop
        {
            get
            {
                if (GradientStops.Count == 0)
                    return null;
                return GradientStops[0];
            }
        }

        private DmlGradientStop LastGradientStop
        {
            get
            {
                if (GradientStops.Count == 0)
                    return null;
                return GradientStops[GradientStops.Count - 1];
            }
        }

        /// <summary>
        /// Gets gradient stops sorted by position.
        /// </summary>
        /// <remarks>
        /// The gradient stops sorted by position are needed for Rendering. They were introduced per WORDSNET-9594
        /// Initially, after the fix, all gradient stops were sorted. However, during implementation of WORDSNET-22003
        /// it was discovered that Word VBA uses original order to determine gradient styles and variants and as consequence
        /// the ForeColor and BackColor. In addition, Word seems always inserts new gradient stops at the very start of
        /// gradient stop collection (before any existing gradient stops). And manipulating a sorted collection is tougher
        /// than manipulating an unsorted one. As the result, it is more convenient and natural to keep in model gradient
        /// stops unsorted and use this sorted version only for Rendering.
        /// </remarks>
        private List<DmlGradientStop> GradientStopsSortedByPosition
        {
            get
            {
                if (mGradientStopsSortedByPosition == null)
                {
                    mGradientStopsSortedByPosition = new List<DmlGradientStop>(GradientStops);
                    mGradientStopsSortedByPosition.Sort();
                }

                return mGradientStopsSortedByPosition;
            }
        }

        /// <summary>
        /// Gets first gradient stop of gradient fill sorted by position.
        /// </summary>
        private DmlGradientStop FirstGradientStopSortedByPosition
        {
            get
            {
                if (GradientStopsSortedByPosition.Count == 0)
                    return null;

                return GradientStopsSortedByPosition[0];
            }
        }

        /// <summary>
        /// Gets last gradient stop of gradient fill sorted by position.
        /// </summary>
        private DmlGradientStop LastGradientStopSortedByPosition
        {
            get
            {
                if (GradientStopsSortedByPosition.Count == 0)
                    return null;

                return GradientStopsSortedByPosition[GradientStopsSortedByPosition.Count - 1];
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either <see cref="FirstGradientStopSortedByPosition"/> is shifted.
        /// </summary>
        private bool IsFirstGradientStopShifted
        {
            get
            {
                return (FirstGradientStopSortedByPosition != null) &&
                       !MathUtil.IsZero(FirstGradientStopSortedByPosition.Position);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either <see cref="LastGradientStopSortedByPosition"/> is shifted.
        /// </summary>
        private bool IsLastGradientStopShifted
        {
            get
            {
                return (LastGradientStopSortedByPosition != null) &&
                       !MathUtil.AreEqual(LastGradientStopSortedByPosition.Position, 1.0);
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private IDmlGradient mGradient;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private IList<DmlGradientStop> mGradientStops;
        private bool mRotateWithShape;
        private DmlTileFlipMode mTileFlipMode;
        [CodePorting.Translator.Cs2Cpp.CppMutable]
        private DmlPercentageOffsetRectangle mTileRectangle;
        private List<DmlGradientStop> mGradientStopsSortedByPosition;
        private GradientStopCollection mGradientStopCollection;
        private const string ValueOutOfRange = "The specified value is out of range.";
    }
}
