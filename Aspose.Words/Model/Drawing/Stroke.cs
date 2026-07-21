// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2006 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Defines a stroke for a shape.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.Stroke"/> property to access stroke properties of a shape.
    /// You do not create instances of the <see cref="Stroke"/> class directly.</p>
    ///
    /// <seealso cref="Shape.Stroke"/>
    /// </remarks>
    public class Stroke : IFillable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal Stroke(IStrokable parent)
        {
            mParent = parent;
        }

        #region Properties of the old Stroke API for compatibility.

        /// <summary>
        /// Defines whether the path will be stroked.
        /// </summary>
        /// <remarks>
        /// <p>The default value for a <see cref="Shape"/> is <c>true</c>.</p>
        /// </remarks>
        public bool On
        {
            get { return Visible; }
            set { Visible = value; }
        }

        /// <summary>
        /// Defines the color of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value for a <see cref="Shape"/> is
        /// <see cref="System.Drawing.Color.Black"/>.
        /// </p>
        /// </remarks>
        public Color Color
        {
            get { return ForeColor; }
            set { ForeColor = value; }
        }

        /// <summary>
        /// Defines a second color for a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value for a <see cref="Shape"/> is
        /// <see cref="System.Drawing.Color.White"/>.
        /// </p>
        /// </remarks>
        public Color Color2
        {
            get { return BackColor; }
            set { BackColor = value; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the foreground color of the stroke.
        /// </summary>
        /// <remarks>
        /// The default value for a <see cref="Shape"/> is
        /// <see cref="System.Drawing.Color.Black"/>.
        /// </remarks>
        public Color ForeColor
        {
            get { return ColorInternal.ToNativeColor(); }
            set { ColorInternal = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets the base foreground color of the stroke without any modifiers.
        /// </summary>
        /// <remarks>
        /// The default value for a <see cref="Shape"/> is
        /// <see cref="System.Drawing.Color.Black"/>.
        /// </remarks>
        public Color BaseForeColor
        {
            get { return mParent.StrokeBaseForeColor.ToNativeColor(); }
        }

        /// <summary>
        /// Gets or sets the background color of the stroke.
        /// </summary>
        /// <remarks>
        /// The default value for a <see cref="Shape"/> is
        /// <see cref="System.Drawing.Color.White"/>.
        /// </remarks>
        public Color BackColor
        {
            get { return Color2Internal.ToNativeColor(); }
            set { Color2Internal = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke foreground color.
        /// </summary>
        public ThemeColor ForeThemeColor
        {
            get { return ((IFillable)this).FillableForeThemeColor; }
            set { ((IFillable)this).FillableForeThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke background color.
        /// </summary>
        public ThemeColor BackThemeColor
        {
            get { return ((IFillable)this).FillableBackThemeColor; }
            set { ((IFillable)this).FillableBackThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the stroke foreground color.
        /// </summary>
        /// <remarks>
        /// <para> The allowed values are within the range from -1 (the darkest) to 1 (the lightest) for this property.
        /// Zero (0) is neutral. Attempting to set this property to a value less than -1 or more than 1
        /// results in <see cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        public double ForeTintAndShade
        {
            get { return ((IFillable)this).FillableForeTintAndShade; }
            set { ((IFillable)this).FillableForeTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the stroke background color.
        /// </summary>
        /// <remarks>
        /// <para> The allowed values are within the range from -1 (the darkest) to 1 (the lightest) for this property.
        /// Zero (0) is neutral. Attempting to set this property to a value less than -1 or more than 1
        /// results in <see cref="ArgumentOutOfRangeException"/>.</para>
        /// </remarks>
        public double BackTintAndShade
        {
            get { return ((IFillable)this).FillableBackTintAndShade; }
            set { ((IFillable)this).FillableBackTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the stroke is visible.
        /// </summary>
        /// <remarks>
        /// The default value for a <see cref="Shape"/> is <c>true</c>.
        /// </remarks>
        public bool Visible
        {
            get { return mParent.StrokeVisible; }
            set { mParent.StrokeVisible = value; }
        }

        /// <summary>
        /// Gets or sets a value between 0.0 (opaque) and 1.0 (clear) representing the degree of transparency
        /// of the stroke.
        /// </summary>
        /// <remarks>
        /// The default value is 0.
        /// </remarks>
        public double Transparency
        {
            get { return mParent.StrokeTransparency; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Transparency");
                mParent.StrokeTransparency = value;
            }
        }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a shape in points.
        /// </summary>
        /// <remarks>
        /// <p>The default value for a <see cref="Shape"/> is 0.75.</p>
        /// </remarks>
        public double Weight
        {
            get { return mParent.Weight; }
            set { mParent.Weight = value; }
        }

        /// <summary>
        /// Specifies the dot and dash pattern for a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.DashStyle.Solid"/>.</p>
        /// </remarks>
        public DashStyle DashStyle
        {
            get { return mParent.DashStyle; }
            set { mParent.DashStyle = value; }
        }

        /// <summary>
        /// Defines the join style of a polyline.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.JoinStyle.Round"/>.</p>
        /// </remarks>
        public JoinStyle JoinStyle
        {
            get { return mParent.JoinStyle; }
            set { mParent.JoinStyle = value; }
        }

        /// <summary>
        /// Defines the cap style for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.EndCap.Flat"/>.</p>
        /// </remarks>
        public EndCap EndCap
        {
            get { return mParent.EndCap; }
            set { mParent.EndCap = value; }
        }

        /// <summary>
        /// Defines the line style of the stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ShapeLineStyle.Single"/>.</p>
        /// </remarks>
        public ShapeLineStyle LineStyle
        {
            get { return mParent.LineStyle; }
            set { mParent.LineStyle = value; }
        }

        /// <summary>
        /// Defines the arrowhead for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowType.None"/>.</p>
        /// </remarks>
        public ArrowType StartArrowType
        {
            get { return mParent.StartArrowType; }
            set { mParent.StartArrowType = value; }
        }

        /// <summary>
        /// Defines the arrowhead for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowType.None"/>.</p>
        /// </remarks>
        public ArrowType EndArrowType
        {
            get { return mParent.EndArrowType; }
            set { mParent.EndArrowType = value; }
        }

        /// <summary>
        /// Defines the arrowhead width for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowWidth.Medium"/>.</p>
        /// </remarks>
        public ArrowWidth StartArrowWidth
        {
            get { return mParent.StartArrowWidth; }
            set { mParent.StartArrowWidth = value; }
        }

        /// <summary>
        /// Defines the arrowhead length for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowLength.Medium"/>.</p>
        /// </remarks>
        public ArrowLength StartArrowLength
        {
            get { return mParent.StartArrowLength; }
            set { mParent.StartArrowLength = value; }
        }

        /// <summary>
        /// Defines the arrowhead width for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowWidth.Medium"/>.</p>
        /// </remarks>
        public ArrowWidth EndArrowWidth
        {
            get { return mParent.EndArrowWidth; }
            set { mParent.EndArrowWidth = value; }
        }

        /// <summary>
        /// Defines the arrowhead length for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowLength.Medium"/>.</p>
        /// </remarks>
        public ArrowLength EndArrowLength
        {
            get { return mParent.EndArrowLength; }
            set { mParent.EndArrowLength = value; }
        }

        /// <summary>
        /// Defines the amount of transparency of a stroke. Valid range is from 0 to 1.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        public double Opacity
        {
            get { return 1 - Transparency; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Opacity");
                Transparency = 1 - value;
            }
        }

        /// <summary>
        /// Defines the image for a stroke image or pattern fill.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] ImageBytes
        {
            get { return mParent.StrokeImageBytes; }
        }

        /// <summary>
        /// Gets fill formatting for the <see cref="Stroke"/>.
        /// </summary>
        public Fill Fill
        {
            get
            {
                if (mFill == null)
                    mFill = new Fill(this);

                return mFill;
            }
        }

        #region IFillable implementation

        /// <summary>
        /// Changes type of the fill to Solid.
        /// </summary>
        void IFillable.Solid()
        {
            DmlFill fill = GetFill(false);
            if (fill == null)
            {
                // Most probably, the fill is VmlOutline. Just set it visible.
                Visible = true;
                return;
            }

            if (fill.DmlFillType == DmlFillType.SolidFill)
                return;

            CreateSolidFill();
        }

        /// <summary>
        /// Gets a PresetTexture for the fill.
        /// </summary>
        PresetTexture IFillable.GetPresetTexture()
        {
            throw new InvalidOperationException(ErrorPresetTextured);
        }

        /// <summary>
        /// Gets a PatternType for the fill.
        /// </summary>
        PatternType IFillable.GetPatternType()
        {
            throw new InvalidOperationException(ErrorPatterned);
        }

        /// <summary>
        /// Changes type of the fill to preset texture.
        /// </summary>
        void IFillable.PresetTextured(PresetTexture presetTexture)
        {
            throw new InvalidOperationException(ErrorPresetTextured);
        }

        /// <summary>
        /// Changes type of the fill to a pattern.
        /// </summary>
        void IFillable.Patterned(PatternType patternType)
        {
            throw new InvalidOperationException(ErrorPatterned);
        }

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.TwoColorGradient(GradientStyle style, GradientVariant variant)
        {
            DmlFill curFill = GetFill(false);
            DmlColor color1 = ((curFill != null) && (curFill.DmlColorInternal != null))
                ? curFill.DmlColorInternal.Clone()
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlColor color2 = ((curFill != null) && (curFill.DmlColor2Internal != null))
                ? curFill.DmlColor2Internal.Clone()
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlFill gradientFill = new DmlGradientFill(color1, color2, style, variant, mParent.StrokeThemeProvider);
            ((IFillable)this).SetFill(gradientFill);
        }

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.OneColorGradient(GradientStyle style, GradientVariant variant, double degree)
        {
            DmlFill curFill = GetFill(false);
            DmlColor color1 = ((curFill != null) && (curFill.DmlColorInternal != null))
                ? curFill.DmlColorInternal.Clone()
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlColor color2 = color1.Clone();

            // Check either we need luminance modifier.
            if (!MathUtil.AreEqual(degree, 0.5))
            {
                DmlLuminanceModulation lumMod = new DmlLuminanceModulation();
                DmlLuminanceOffset lumOff = new DmlLuminanceOffset();
                if (MathUtil.IsLessOrEqual(degree, 0.5))
                {
                    lumMod.Value = degree * 2;
                    lumOff.Value = 0;
                }
                else
                {
                    lumMod.Value = (1 - degree) * 2;
                    lumOff.Value = 1 - lumMod.Value;
                }

                color2.ColorModifiers.Add(lumMod);
                color2.ColorModifiers.Add(lumOff);
            }

            DmlFill gradientFill = new DmlGradientFill(color1, color2, style, variant, mParent.StrokeThemeProvider);
            ((IFillable)this).SetFill(gradientFill);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        void IFillable.SetImage(byte[] imageBytes)
        {
            throw new InvalidOperationException("SetImage cannot be applied to a fill of the Stroke.");
        }

        /// <summary>
        /// Sets specified fill to this object.
        /// </summary>
        void IFillable.SetFill(IFill fill)
        {
            DmlFill dmlFill = fill as DmlFill;
            if ((dmlFill == null) ||
                ((dmlFill.DmlFillType != DmlFillType.SolidFill) &&
                 (dmlFill.DmlFillType != DmlFillType.GradientFill) &&
                 (dmlFill.DmlFillType != DmlFillType.NoFill)))
                throw new InvalidOperationException("Invalid fill type for this object.");

            dmlFill.Parent = this;
            mParent.StrokeFill = dmlFill;
        }

        /// <summary>
        /// Returns a double value representing transparency of a specified color.
        /// </summary>
        double IFillable.GetTransparency(DmlColor color)
        {
            if (color.Alpha == null)
                return 0.0;

            return (1 - color.Alpha.Value);
        }

        /// <summary>
        /// Sets a specified value to a transparency of specified color .
        /// </summary>
        void IFillable.SetTransparency(DmlColor color, double value)
        {
            color.UpdateAlpha(1 - value);
        }

        #region The public properties of old Fill object for compatibility.

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        Color IFillable.FilledColor
        {
            get { return ((IFillable)this).FillableForeColor; }
            set { ((IFillable)this).FillableForeColor = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.OldOn
        {
            get { return ((IFillable)this).FillableVisible; }
            set { ((IFillable)this).FillableVisible = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (clear) and 1.0 (opaque) representing the degree
        /// of opacity of the specified fill.
        /// </summary>
        double IFillable.OldOpacity
        {
            get { return 1 - ((IFillable)this).FillableTransparency; }
            set { ((IFillable)this).FillableTransparency = 1 - value; }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        byte[] IFillable.FillableImageBytes
        {
            // There can be only Solid, Gradient and NoFill fills in stroke, so there cannot be any image.
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the alignment for tile texture fill.
        /// </summary>
        TextureAlignment IFillable.TextureAlignment
        {
            get { return TextureAlignment.None; }
            set { throw new InvalidOperationException(ErrorTextureAlignment); }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        double IFillable.GradientAngle
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return 0;

                return fill.Angle;
            }
            set
            {
                DmlFill fill = GetFill(false);
                if (fill != null)
                    fill.Angle = value;
            }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills.
        /// </summary>
        GradientVariant IFillable.GradientVariant
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return GradientVariant.None;

                return fill.GradientVariant;
            }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// </summary>
        GradientStyle IFillable.GradientStyle
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return GradientStyle.None;

                return fill.GradientStyle;
            }
        }

        /// <summary>
        /// Gets the gradient stops collection for the fill.
        /// </summary>
        GradientStopCollection IFillable.GradientStops
        {
            get
            {
                DmlGradientFill fill = GetFill(false) as DmlGradientFill;
                if (fill == null)
                    throw new InvalidOperationException(InvalidAction);

                return fill.GradientStopCollection;
            }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        Color IFillable.FillableForeColor
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeForeColor.ToColorFixAlpha().ToNativeColor()
                    : Color.Empty;
            }
            set { mParent.StrokeForeColor = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets a Color object that represents the base foreground color for the fill.
        /// </summary>
        Color IFillable.FillableBaseForeColor
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeBaseForeColor.ToColorFixAlpha().ToNativeColor()
                    : Color.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the background color for the fill.
        /// </summary>
        Color IFillable.FillableBackColor
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeBackColor.ToColorFixAlpha().ToNativeColor()
                    : Color.Empty;
            }
            set { mParent.StrokeBackColor = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableForeThemeColor
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeForeThemeColor
                    : ThemeColor.None;
            }
            set { mParent.StrokeForeThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableBackThemeColor
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeBackThemeColor
                    : ThemeColor.None;
            }
            set { mParent.StrokeBackThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double IFillable.FillableForeTintAndShade
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeForeTintAndShade
                    : 0;
            }
            set { mParent.StrokeForeTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double IFillable.FillableBackTintAndShade
        {
            get
            {
                return ((IFillable)this).FillableVisible
                    ? mParent.StrokeBackTintAndShade
                    : 0;
            }
            set { mParent.StrokeBackTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.FillableVisible
        {
            get { return mParent.StrokeVisible; }
            set { mParent.StrokeVisible = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency of the specified fill.
        /// </summary>
        double IFillable.FillableTransparency
        {
            get { return mParent.StrokeTransparency; }
            set { mParent.StrokeTransparency = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the fill rotates with the specified object.
        /// </summary>
        bool IFillable.RotateWithObject
        {
            get { throw new InvalidOperationException(DmlFill.MsgInvalidAction); }
            set { throw new InvalidOperationException(DmlFill.MsgInvalidAction); }
        }

        /// <summary>
        /// Gets fill type.
        /// </summary>
        FillTypeCore IFillable.FillType
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return FillTypeCore.Solid;

                return fill.FillType;
            }
        }

        /// <summary>
        /// Gets IThemeProvider object.
        /// </summary>
        IThemeProvider IFillable.FillableThemeProvider
        {
            get { return mParent.StrokeThemeProvider; }
        }

#endregion

        /// <summary>
        /// Gets fill of this <see cref="Stroke"/> object.
        /// </summary>
        /// <param name="isAllowAutoCreate"> Indicates whether it is allowed to create
        /// default Solid fill when there is no fill set. </param>
        private DmlFill GetFill(bool isAllowAutoCreate)
        {
            DmlFill value = mParent.StrokeFill;
            if (value == null)
                return (isAllowAutoCreate) ? CreateSolidFill() : null;

            value.Parent = this;
            return value;
        }

        /// <summary>
        /// Creates a new Solid fill in this <see cref="Stroke"/> object.
        /// </summary>
        private DmlFill CreateSolidFill()
        {
            DmlFill solidFill = new DmlSolidFill();
            mParent.StrokeFill = solidFill;

            solidFill.Parent = this;

            return solidFill;
        }

        internal DrColor ColorInternal
        {
            get { return mParent.StrokeForeColor; }
            set { mParent.StrokeForeColor = value; }
        }

        internal DrColor Color2Internal
        {
            get { return mParent.StrokeBackColor; }
            set { mParent.StrokeBackColor = value; }
        }

        /// <summary>
        /// Defines the type of fill used for the background of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.Core.LineFillType.Solid"/>.</p>
        /// </remarks>
        internal LineFillType LineFillType
        {
            get { return mParent.LineFillType; }
            set { mParent.LineFillType = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IStrokable mParent;

        private Fill mFill;

        private const string ErrorPresetTextured = "Preset texture cannot be applied to Stroke.";
        private const string ErrorTextureAlignment = "TextureAlignment cannot be applied to Stroke.";
        private const string ErrorPatterned = "Pattern cannot be applied to Stroke.";
        private const string InvalidAction = "Object doesn't support this action.";
    }
}
