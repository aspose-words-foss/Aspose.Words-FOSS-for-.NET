// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2021 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents the formatting of a chart element.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartFormat : IFillable, IStrokable
    {
        internal ChartFormat(IChartFormatSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Resets the fill of the chart element to have the default value.
        /// </summary>
        public void SetDefaultFill()
        {
            if (!IsDefined)
                return;

            EnsureFillCanBeUpdated();

            // It is expected that setting to 'null' resets the fill, and then mParent.Fill will return a value
            // representing the default fill.
            mParent.Fill = null;
        }

        /// <summary>
        /// Changes type of the fill to an one-color gradient.
        /// </summary>
        private void OneColorGradient(DmlColor color, GradientStyle style, GradientVariant variant, double degree)
        {
            DmlColor color2 = color.Clone();

            if (!MathUtil.AreEqual(degree, 0.5))
            {
                // Add either a Tint or Shade modifier to the second color.
                IDmlColorModifier modifier = MathUtil.IsLessOrEqual(degree, 0.5)
                    ? (IDmlColorModifier)new DmlShade(degree * 2)
                    : (IDmlColorModifier)new DmlTint((1 - degree) * 2);

                color2.ColorModifiers.Add(modifier);
            }

            TwoColorGradient(color, color2, style, variant);
        }

        /// <summary>
        /// Changes type of the fill to a two-color gradient.
        /// </summary>
        private void TwoColorGradient(DmlColor color1, DmlColor color2, GradientStyle style, GradientVariant variant)
        {
            EnsureFillCanBeUpdated();

            IThemeProvider themeProvider = ((IFillable)this).FillableThemeProvider;
            FillCore = new DmlGradientFill(color1, color2, style, variant, themeProvider);

            // Word VBA sets this flag, do the same.
            FillCore.RotateWithShape = true;
        }

        /// <summary>
        /// Ensures that fill of the chart element can be updated. Throws an exception if this is not so.
        /// </summary>
        private void EnsureFillCanBeUpdated()
        {
            if (!mParent.IsFillSupported)
                throw new InvalidOperationException(NotSupportedFill);

            mParent.MaterializeSpPr();
        }

        #region IFillable implementation

        /// <summary>
        /// Changes type of the fill to Solid.
        /// </summary>
        void IFillable.Solid()
        {
            EnsureFillCanBeUpdated();

            if (FillCore.DmlFillType == DmlFillType.SolidFill)
                return;

            FillCore = new DmlSolidFill();
            // Word VBA resets back color to default in the new Solid fill.
            FillCore.Color2Internal = DrColor.White;
        }

        /// <summary>
        /// Changes type of the fill to preset texture.
        /// </summary>
        void IFillable.PresetTextured(PresetTexture presetTexture)
        {
            string textureName = FillUtil.GetTextureName(presetTexture);
            if (textureName == string.Empty)
                throw new ArgumentOutOfRangeException("The specified texture is out of range.");

            EnsureFillCanBeUpdated();

            DmlBlipFill blipFill = new DmlBlipFill();
            blipFill.BlipFillMode = new DmlBlipFillTile();
            blipFill.Blip.EmbedImage = FillUtil.GetTextureBytes(textureName);

            FillCore = blipFill;
        }

        /// <summary>
        /// Changes type of the fill to a pattern.
        /// </summary>
        void IFillable.Patterned(PatternType patternType)
        {
            EnsureFillCanBeUpdated();

            DmlPatternFill patternFill = new DmlPatternFill();
            patternFill.FillPresetPattern = patternType;

            patternFill.ForegroundColor = (FillCore.DmlColorInternal != null)
                ? FillCore.DmlColorInternal.Clone()
                : ((IFillable)this).FillableThemeProvider.GetThemeColor(ThemeColor.Accent1);

            patternFill.BackgroundColor = (FillCore.DmlColor2Internal != null)
                ? FillCore.DmlColor2Internal.Clone()
                : ((IFillable)this).FillableThemeProvider.GetThemeColor(ThemeColor.Background1);

            FillCore = patternFill;
        }

        /// <summary>
        /// Gets a <see cref="PresetTexture"/> for the fill.
        /// </summary>
        PresetTexture IFillable.GetPresetTexture()
        {
            int textureHash = HashUtil.GetSHA512Hash(FillCore.ImageBytes).GetHashCode();
            return FillUtil.GetTextureByHash(textureHash);
        }

        /// <summary>
        /// Gets a <see cref="PatternType"/> of the fill.
        /// </summary>
        PatternType IFillable.GetPatternType()
        {
            return (FillCore.FillType == FillTypeCore.Pattern)
                ? ((DmlPatternFill)FillCore).FillPresetPattern
                : PatternType.None;
        }

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.OneColorGradient(GradientStyle style, GradientVariant variant, double degree)
        {
            DmlColor color = (FillCore.DmlColorInternal != null)
                ? FillCore.DmlColorInternal.Clone()
                : ((IFillable)this).FillableThemeProvider.GetThemeColor(ThemeColor.Accent1);

            OneColorGradient(color, style, variant, degree);
        }

        /// <summary>
        /// Changes type of the fill to two-color gradient.
        /// </summary>
        void IFillable.TwoColorGradient(GradientStyle style, GradientVariant variant)
        {
            IThemeProvider themeProvider = ((IFillable)this).FillableThemeProvider;

            DmlColor color1 = (FillCore.DmlColorInternal != null)
                ? FillCore.DmlColorInternal.Clone()
                : themeProvider.GetThemeColor(ThemeColor.Accent1);

            DmlColor color2 = (FillCore.DmlColor2Internal != null)
                ? FillCore.DmlColor2Internal.Clone()
                : themeProvider.GetThemeColor(ThemeColor.Background1);

            TwoColorGradient(color1, color2, style, variant);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        void IFillable.SetImage(byte[] imageBytes)
        {
            EnsureFillCanBeUpdated();

            FillCore = new DmlBlipFill();
            FillCore.SetImageBytes(imageBytes);
        }

        /// <summary>
        /// Sets the specified fill to the parent chart element.
        /// </summary>
        void IFillable.SetFill(IFill fill)
        {
            Debug.Assert(fill != null);

            DmlFill dmlFill = fill as DmlFill;
            if (dmlFill == null)
                throw new InvalidOperationException("Invalid fill type for this object.");

            FillCore = dmlFill;
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

        #region The properties of the old Fill API for compatibility.

        /// <summary>
        /// Gets or sets a <see cref="Color"/> object that represents a color of the fill.
        /// </summary>
        Color IFillable.FilledColor
        {
            get { return ((IFillable)this).FillableForeColor; }
            set { ((IFillable)this).FillableForeColor = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether a fill applied to the parent chart element is visible.
        /// </summary>
        bool IFillable.OldOn
        {
            get { return ((IFillable)this).FillableVisible; }
            set { ((IFillable)this).FillableVisible = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (clear) and 1.0 (opaque) representing the degree of opacity
        /// of the fill.
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
        /// <p>The default value is null.</p>
        /// </remarks>
        byte[] IFillable.FillableImageBytes
        {
            get { return (FillCore.DmlFillType == DmlFillType.BlipFill) ? ((DmlBlipFill)FillCore).Blip.EmbedImage : null; }
        }

        #endregion

        /// <summary>
        /// Gets or sets a <see cref="Color"/> object that represents a color of the fill.
        /// </summary>
        Color IFillable.FillableForeColor
        {
            get
            {
                // Transparency/Opacity properties are stored as color alpha: return color with 0xff alpha here.
                return FillCore.ColorInternal.ToColorFixAlpha().ToNativeColor();
            }
            set
            {
                EnsureFillCanBeUpdated();

                if (FillCore.DmlFillType == DmlFillType.NoFill)
                    FillCore = new DmlSolidFill();

                FillCore.ColorInternal = DrColor.FromNativeColor(value);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Color"/> object that represents a base color of the fill without modifiers.
        /// </summary>
        Color IFillable.FillableBaseForeColor
        {
            get { return FillCore.ColorInternalUnmodified.ToColorFixAlpha().ToNativeColor(); }
        }

        /// <summary>
        /// Gets or sets a <see cref="Color"/> object that represents an additional color of the fill.
        /// </summary>
        /// <remarks>
        /// This is an end color of a gradient fill or a background color of a pattern fill.
        /// </remarks>
        Color IFillable.FillableBackColor
        {
            get { return FillCore.Color2Internal.ToColorFixAlpha().ToNativeColor(); }
            set
            {
                EnsureFillCanBeUpdated();

                if (FillCore.DmlFillType != DmlFillType.NoFill)
                    FillCore.Color2Internal = DrColor.FromNativeColor(value);
            }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableForeThemeColor
        {
            get { return ThemeColor.None; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableBackThemeColor
        {
            get { return ThemeColor.None; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double IFillable.FillableForeTintAndShade
        {
            get { return 0; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double IFillable.FillableBackTintAndShade
        {
            get { return 0; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether a fill applied to the parent chart element is visible.
        /// </summary>
        bool IFillable.FillableVisible
        {
            get { return FillCore.On; }
            set
            {
                EnsureFillCanBeUpdated();
                FillCore.On = value;
            }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree of transparency
        /// of the fill.
        /// </summary>
        double IFillable.FillableTransparency
        {
            get { return 1 - FillCore.Opacity; }
            set
            {
                EnsureFillCanBeUpdated();

                // Word VBA allows to set up opacity (transparency) for NoFill and changes the type of fill
                // to Solid implicitly.
                if (FillCore.DmlFillType == DmlFillType.NoFill)
                    FillCore = new DmlSolidFill();

                FillCore.Opacity = 1 - value;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the fill rotates with the specified object.
        /// </summary>
        /// <dev>
        /// Couldn't find a way to change this option in MS Word, it is always disabled. And cannot find a way to rotate
        /// a chart in MS Word, the corresponding button is always disabled too.
        /// </dev>
        bool IFillable.RotateWithObject
        {
            get { return FillCore.RotateWithShape; }
            set
            {
                EnsureFillCanBeUpdated();
                FillCore.RotateWithShape = value;
            }
        }

        /// <summary>
        /// Gets fill type.
        /// </summary>
        FillTypeCore IFillable.FillType
        {
            get { return FillCore.FillType; }
        }

        /// <summary>
        /// Gets or sets the alignment for tile texture fill.
        /// </summary>
        TextureAlignment IFillable.TextureAlignment
        {
            get
            {
                DmlBlipFill blipFill = FillCore as DmlBlipFill;
                if (blipFill == null)
                    return TextureAlignment.None;

                DmlBlipFillTile blipFillTile = blipFill.BlipFillMode as DmlBlipFillTile;
                if (blipFillTile == null)
                    return TextureAlignment.None;

                return FillUtil.DmlRectangleToTextureAlignment(blipFillTile.Alignment);
            }
            set
            {
                if (value == TextureAlignment.None)
                    throw new InvalidOperationException("TextureAlignment.None cannot be applied directly.");

                DmlBlipFill blipFill = FillCore as DmlBlipFill;
                if (blipFill == null)
                    throw new InvalidOperationException("Texture alignment can be applied to a texture fill only.");

                DmlBlipFillTile blipFillTile = blipFill.BlipFillMode as DmlBlipFillTile;
                if (blipFillTile == null)
                    throw new InvalidOperationException("Texture alignment can be applied to a tile texture fill only.");

                blipFillTile.Alignment = FillUtil.TextureToDmlRectangleAlignment(value);
            }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        double IFillable.GradientAngle
        {
            get { return FillCore.Angle; }
            set { FillCore.Angle = value; }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills.
        /// </summary>
        GradientVariant IFillable.GradientVariant
        {
            get { return FillCore.GradientVariant; }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// </summary>
        GradientStyle IFillable.GradientStyle
        {
            get { return FillCore.GradientStyle; }
        }

        /// <summary>
        /// Gets the gradient stops collection for the fill.
        /// </summary>
        GradientStopCollection IFillable.GradientStops
        {
            get
            {
                DmlGradientFill gradientFill = FillCore as DmlGradientFill;
                if (gradientFill == null)
                    throw new InvalidOperationException(InvalidAction);

                // Let's materialize SpPr here so that changing gradient stops will not affect format of the parent object.
                // This makes format of this object unlinked from format of the parent object, which is not good for a get
                // operation, but let's use this simple solution until customers complain.
                mParent.MaterializeSpPr();

                return gradientFill.GradientStopCollection;
            }
        }

        /// <summary>
        /// Gets a <see cref="IThemeProvider"/> object.
        /// </summary>
        IThemeProvider IFillable.FillableThemeProvider
        {
            get { return (mParent.ThemeProvider != null) ? mParent.ThemeProvider : Theme.BuiltInTheme; }
        }
        #endregion

        #region IStrokable implementation

        /// <summary>
        /// Gets or sets the foreground color for the stroke.
        /// </summary>
        DrColor IStrokable.StrokeForeColor
        {
            get
            {
                // Transparency/Opacity properties are stored as color alpha: return color with 0xff alpha here.
                return StrokeCore.ColorInternal.ToColorFixAlpha();
            }
            set
            {
                mParent.MaterializeSpPr();

                // DmlNoFill doesn't store color: let's convert to a solid fill to not lost the value. The same is done
                // in IFillable.ForeColor implementation. This makes the line visible.
                if (StrokeCore.Fill.DmlFillType == DmlFillType.NoFill)
                    StrokeCore.Fill = new DmlSolidFill();

                StrokeCore.ColorInternal = value;
            }
        }

        /// <summary>
        /// Gets a base foreground color for the stroke without modifiers.
        /// </summary>
        DrColor IStrokable.StrokeBaseForeColor
        {
            get { return StrokeCore.ColorInternalUnmodified.ToColorFixAlpha(); }
        }

        /// <summary>
        /// Gets or sets the background color for the stroke.
        /// </summary>
        DrColor IStrokable.StrokeBackColor
        {
            get { return StrokeCore.Color2Internal.ToColorFixAlpha(); }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.Color2Internal = value;
            }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the stroke.
        /// </summary>
        ThemeColor IStrokable.StrokeForeThemeColor
        {
            get { return ThemeColor.None; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the stroke.
        /// </summary>
        ThemeColor IStrokable.StrokeBackThemeColor
        {
            get { return ThemeColor.None; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double IStrokable.StrokeForeTintAndShade
        {
            get { return 0; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double IStrokable.StrokeBackTintAndShade
        {
            get { return 0; }
            set { throw new NotImplementedException(ErrorNotImplemented); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the stroke is visible.
        /// </summary>
        bool IStrokable.StrokeVisible
        {
            get { return StrokeCore.On; }
            set
            {
                mParent.MaterializeSpPr();

                StrokeCore.On = value;
                if (value && StrokeCore.Fill.DmlFillType == DmlFillType.NoFill)
                    StrokeCore.Fill = new DmlSolidFill();
            }
        }

        /// <summary>
        /// Gets or sets a value between 0.0 (opaque) and 1.0 (clear) representing the degree of transparency
        /// of the stroke.
        /// </summary>
        double IStrokable.StrokeTransparency
        {
            get { return 1 - StrokeCore.Opacity; }
            set
            {
                mParent.MaterializeSpPr();

                // Opacity/transparency is stored in DmlOutline.Fill.Opacity, but DmlNoFill cannot store it: let's
                // convert to a solid fill.
                if (StrokeCore.Fill.DmlFillType == DmlFillType.NoFill)
                    StrokeCore.Fill = new DmlSolidFill();

                StrokeCore.Opacity = 1 - value;
            }
        }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a chart element in points.
        /// </summary>
        double IStrokable.Weight
        {
            get
            {
                if (StrokeCore.On && !StrokeCore.IsWeightSpecified)
                {
                    // Actually weight default value depends on a chart element type. For example, it is different for
                    // a line chart series. Weight is explicitly defined in chart definitions, so skip this discrepancy
                    // for now.
                    return DefaultWeight;
                }

                return StrokeCore.Weight;
            }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.Weight = value;
            }
        }

        /// <summary>
        /// Specifies the dot and dash pattern for a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="DashStyle.Solid"/>.</p>
        /// </remarks>
        DashStyle IStrokable.DashStyle
        {
            get { return StrokeCore.DashStyle; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.DashStyle = value;
            }
        }

        /// <summary>
        /// Defines the join style of a polyline.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="JoinStyle.Round"/>.
        /// </remarks>
        JoinStyle IStrokable.JoinStyle
        {
            get { return StrokeCore.JoinStyle; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.JoinStyle = value;
            }
        }

        /// <summary>
        /// Defines the cap style for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="EndCap.Flat"/>.
        /// </remarks>
        EndCap IStrokable.EndCap
        {
            get { return StrokeCore.EndCap; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.EndCap = value;
            }
        }

        /// <summary>
        /// Defines the line style of the stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ShapeLineStyle.Single"/>.
        /// </remarks>
        ShapeLineStyle IStrokable.LineStyle
        {
            get { return StrokeCore.LineStyle; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.LineStyle = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowType.None"/>.
        /// </remarks>
        ArrowType IStrokable.StartArrowType
        {
            get { return StrokeCore.StartArrowType; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.StartArrowType = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowType.None"/>.
        /// </remarks>
        ArrowType IStrokable.EndArrowType
        {
            get { return StrokeCore.EndArrowType; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.EndArrowType = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead width for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowWidth.Medium"/>.
        /// </remarks>
        ArrowWidth IStrokable.StartArrowWidth
        {
            get { return StrokeCore.StartArrowWidth; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.StartArrowWidth = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead length for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowLength.Medium"/>.
        /// </remarks>
        ArrowLength IStrokable.StartArrowLength
        {
            get { return StrokeCore.StartArrowLength; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.StartArrowLength = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead width for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowWidth.Medium"/>.
        /// </remarks>
        ArrowWidth IStrokable.EndArrowWidth
        {
            get { return StrokeCore.EndArrowWidth; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.EndArrowWidth = value;
            }
        }

        /// <summary>
        /// Defines the arrowhead length for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ArrowLength.Medium"/>.
        /// </remarks>
        ArrowLength IStrokable.EndArrowLength
        {
            get { return StrokeCore.EndArrowLength; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.EndArrowLength = value;
            }
        }

        /// <summary>
        /// Defines the type of fill used for the background of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="LineFillType.Solid"/>.
        /// </remarks>
        LineFillType IStrokable.LineFillType
        {
            get { return StrokeCore.LineFillType; }
            set
            {
                mParent.MaterializeSpPr();
                StrokeCore.LineFillType = value;
            }
        }

        /// <summary>
        /// Gets the image for a stroke image or pattern fill.
        /// </summary>
        byte[] IStrokable.StrokeImageBytes
        {
            get { return StrokeCore.ImageBytes; }
        }

        /// <summary>
        /// Gets a <see cref="IThemeProvider"/> object.
        /// </summary>
        IThemeProvider IStrokable.StrokeThemeProvider
        {
            get { return mParent.ThemeProvider; }
        }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        DmlFill IStrokable.StrokeFill
        {
            get { return StrokeCore.Fill; }
            set { StrokeCore.Fill = value; }
        }

        #endregion

        /// <summary>
        /// Gets fill formatting for the parent chart element.
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

        /// <summary>
        /// Gets line formatting for the parent chart element.
        /// </summary>
        public Stroke Stroke
        {
            get
            {
                if (mLine == null)
                    mLine = new Stroke(this);

                return mLine;
            }
        }

        /// <summary>
        /// Gets or sets the shape type of the parent chart element.
        /// </summary>
        /// <remarks>
        /// Currently, the property can only be used for data labels.
        /// </remarks>
        public ChartShapeType ShapeType
        {
            get { return mParent.ShapeType; }
            set { mParent.ShapeType = value; }
        }

        /// <summary>
        /// Gets a flag indicating whether any format is defined.
        /// </summary>
        public bool IsDefined
        {
            get { return mParent.IsFormatDefined; }
        }

        /// <summary>
        /// Gets a <see cref="DmlFill"/> containing fill properties of the parent chart element.
        /// </summary>
        private DmlFill FillCore
        {
            get
            {
                DmlFill fillCore = mParent.Fill;
                fillCore.Parent = this;
                return fillCore;
            }
            set
            {
                mParent.Fill = value;
                if (value != null)
                    value.Parent = this;
            }
        }

        /// <summary>
        /// Gets a <see cref="DmlOutline"/> containing line formatting properties of the parent chart element.
        /// </summary>
        private DmlOutline StrokeCore
        {
            get
            {
                DmlOutline strokeCore = mParent.Outline;
                strokeCore.Fill.Parent = this;
                return strokeCore;
            }
            set
            {
                mParent.Outline = value;
                if (value != null)
                    value.Fill.Parent = this;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IChartFormatSource mParent;
        private Fill mFill;
        private Stroke mLine;

        /// <summary>
        /// Default weight.
        /// </summary>
        /// <remarks>
        /// Actually weight default value depends on a chart element type. For example, it is different for a line
        /// chart series. Weight is explicitly defined in chart definitions, so skip this discrepancy for now.
        /// </remarks>
        private const double DefaultWeight = 0.75;

        private const string ErrorNotImplemented = "Method or property is not implemented for ChartFormat.";
        private const string InvalidAction = "Object doesn't support this action.";
        private const string NotSupportedFill = "Fill is not supported by this chart element.";
    }
}
