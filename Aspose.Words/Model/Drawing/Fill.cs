// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2019 by Ilya Navrotskiy

using System;
using System.Drawing;
using System.IO;
using Aspose.IO;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents fill formatting for an object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-graphic-elements/">Working with Graphic Elements</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="ShapeBase.Fill"/> or <see cref="Font.Fill"/> property to access fill properties of an object.
    /// You do not create instances of the <see cref="Fill"/> class directly.</p>
    /// </remarks>
    /// <dev>
    /// This class is proposed public facade for <see cref="IFillable"/>.
    /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat
    /// </dev>
    public class Fill
    {
        /// <summary>
        /// Creates FillFormat object with a specified parent.
        /// </summary>
        internal Fill(IFillable parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Sets the fill to a uniform color.
        /// </summary>
        /// <remarks>
        /// Use this method to convert any of the fills back to solid fill.
        /// </remarks>
        public void Solid()
        {
            mParent.Solid();
        }

        /// <summary>
        /// Sets the fill to a specified uniform color.
        /// </summary>
        /// <remarks>
        /// Use this method to convert any of the fills back to solid fill.
        /// </remarks>
        public void Solid(Color color)
        {
            mParent.Solid();
            mParent.FillableForeColor = color;
        }

        /// <summary>
        /// Sets the fill to a preset texture.
        /// </summary>
        /// <param name="presetTexture"><see cref="Drawing.PresetTexture"/></param>
        public void PresetTextured(PresetTexture presetTexture)
        {
            if (presetTexture == PresetTexture.None)
                throw new ArgumentException("PresetTexture.None cannot be applied directly.");
            mParent.PresetTextured(presetTexture);
        }

        /// <summary>
        /// Sets the specified fill to a pattern.
        /// </summary>
        /// <param name="patternType"><see cref="Drawing.PatternType"/></param>
        public void Patterned(PatternType patternType)
        {
            if (patternType == PatternType.None)
                throw new ArgumentException(ErrorPatterned);

            mParent.Patterned(patternType);
        }

        /// <summary>
        /// Sets the specified fill to a pattern.
        /// </summary>
        /// <param name="patternType"><see cref="Drawing.PatternType"/></param>
        /// <param name="foreColor">The color of the foreground fill.</param>
        /// <param name="backColor">The color of the background fill.</param>
        public void Patterned(PatternType patternType, Color foreColor, Color backColor)
        {
            Patterned(patternType);
            mParent.FillableForeColor = foreColor;
            mParent.FillableBackColor = backColor;
        }

        /// <summary>
        /// Sets the specified fill to a one-color gradient.
        /// </summary>
        /// <param name="style">The gradient style <see cref="Drawing.GradientStyle"/></param>
        /// <param name="variant">The gradient variant <see cref="Drawing.GradientVariant"/></param>
        /// <param name="degree">The gradient degree. Can be a value from 0.0 (dark) to 1.0 (light).</param>
        public void OneColorGradient(GradientStyle style, GradientVariant variant, double degree)
        {
            mParent.OneColorGradient(style, variant, degree);
        }

        /// <summary>
        /// Sets the specified fill to a one-color gradient using the specified color.
        /// </summary>
        /// <param name="color">The color to build the gradient.</param>
        /// <param name="style">The gradient style <see cref="Drawing.GradientStyle"/></param>
        /// <param name="variant">The gradient variant <see cref="Drawing.GradientVariant"/></param>
        /// <param name="degree">The gradient degree. Can be a value from 0.0 (dark) to 1.0 (light).</param>
        public void OneColorGradient(Color color, GradientStyle style, GradientVariant variant, double degree)
        {
            // Generate a simple gradient so that the mParent.FillableForeColor property can be set. Otherwise its value
            // may be not stored.
            mParent.OneColorGradient(style, GradientVariant.Variant1, degree);

            mParent.FillableForeColor = color;

            // Generate the final gradient.
            mParent.OneColorGradient(style, variant, degree);
        }

        /// <summary>
        /// Sets the specified fill to a two-color gradient.
        /// </summary>
        /// <param name="style">The gradient style <see cref="Drawing.GradientStyle"/>.</param>
        /// <param name="variant">The gradient variant <see cref="Drawing.GradientVariant"/></param>
        public void TwoColorGradient(GradientStyle style, GradientVariant variant)
        {
            mParent.TwoColorGradient(style, variant);
        }

        /// <summary>
        /// Sets the specified fill to a two-color gradient.
        /// </summary>
        /// <param name="color1">The first color to build the gradient.</param>
        /// <param name="color2">The second color to build the gradient.</param>
        /// <param name="style">The gradient style <see cref="Drawing.GradientStyle"/>.</param>
        /// <param name="variant">The gradient variant <see cref="Drawing.GradientVariant"/></param>
        public void TwoColorGradient(Color color1, Color color2, GradientStyle style, GradientVariant variant)
        {
            // Generate a simple gradient so that the mParent.FillableForeColor and mParent.FillableBackColor properties
            // can be set. Otherwise their values may be not stored.
            mParent.TwoColorGradient(style, GradientVariant.Variant1);

            mParent.FillableForeColor = color1;
            mParent.FillableBackColor = color2;

            // Generate the final gradient.
            mParent.TwoColorGradient(style, variant);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        /// <param name="fileName">The path to the image file.</param>
        public void SetImage(string fileName)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            mParent.SetImage(imageBytes);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        /// <param name="stream">The stream that contains the image bytes.</param>
        /// <javaName>void setImage(java.io.InputStream stream)</javaName>
        public void SetImage(Stream stream)
        {
            byte[] imageBytes = new byte[(int)stream.Length];
            StreamUtil.Read(stream, imageBytes, 0, (int)stream.Length);
            mParent.SetImage(imageBytes);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        /// <param name="imageBytes">The image bytes array.</param>
        public void SetImage(byte[] imageBytes)
        {
            mParent.SetImage(imageBytes);
        }

        /// <summary>
        /// Gets a <see cref="Drawing.PresetTexture"/> for the fill.
        /// </summary>
        public PresetTexture PresetTexture
        {
            get { return mParent.GetPresetTexture(); }
        }

        /// <summary>
        /// Gets a <see cref="Drawing.PatternType"/> for the fill.
        /// </summary>
        public PatternType Pattern
        {
            get { return mParent.GetPatternType(); }
        }

        /// <summary>
        /// Gets or sets the alignment for tile texture fill.
        /// </summary>
        public TextureAlignment TextureAlignment
        {
            get { return mParent.TextureAlignment; }
            set { mParent.TextureAlignment = value; }
        }

        #region The public properties of old Fill object for compatibility.
        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        /// <remarks>
        /// This property preserves the alpha component of the <see cref="System.Drawing.Color"/>,
        /// unlike the <see cref="ForeColor"/> property, which resets it to fully opaque color.
        /// </remarks>
        /// <dev> Word VBA gets and sets RGB color. </dev>
        public Color Color
        {
            get { return mParent.FilledColor; }
            set { mParent.FilledColor = value; }
        }

        /// <summary>
        /// Gets or sets the degree of opacity of the specified fill as a value between 0.0 (clear) and 1.0 (opaque).
        /// </summary>
        /// <remarks>This property is the opposite of property <see cref="Transparency"/>.</remarks>
        public double Opacity
        {
            get { return mParent.OldOpacity; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Opacity");
                mParent.OldOpacity = value;
            }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        public byte[] ImageBytes
        {
            get { return mParent.FillableImageBytes; }
        }

        #endregion

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        /// <remarks> This property resets the alpha component of the <see cref="System.Drawing.Color"/>
        /// to fully opaque color unlike the <see cref="Color"/> property, which preserves it.
        /// </remarks>
        /// <dev> Word VBA gets and sets RGB color. </dev>
        public Color ForeColor
        {
            get { return mParent.FillableForeColor; }
            set { mParent.FillableForeColor = value; }
        }

        /// <summary>
        /// Gets a Color object that represents the base foreground color for the fill
        /// without any modifiers.
        /// </summary>
        public Color BaseForeColor
        {
            get { return mParent.FillableBaseForeColor; }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the background color for the fill.
        /// </summary>
        /// <dev> Word VBA gets and sets RGB color. </dev>
        public Color BackColor
        {
            get { return mParent.FillableBackColor; }
            set { mParent.FillableBackColor = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        public ThemeColor ForeThemeColor
        {
            get { return mParent.FillableForeThemeColor; }
            set { mParent.FillableForeThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        public ThemeColor BackThemeColor
        {
            get { return mParent.FillableBackThemeColor; }
            set { mParent.FillableBackThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        /// <remarks>
        /// <para>The allowed values are in the range from -1 (the darkest) to 1 (the lightest) for this property.</para>
        /// <para>Zero (0) is neutral.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throw if set this property to a value less than -1 or more than 1.</exception>
        public double ForeTintAndShade
        {
            get { return mParent.FillableForeTintAndShade; }
            set { mParent.FillableForeTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        /// <remarks>
        /// <para>The allowed values are in the range from -1 (the darkest) to 1 (the lightest) for this property.</para>
        /// <para>Zero (0) is neutral.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throw if set this property to a value less than -1 or more than 1.</exception>
        public double BackTintAndShade
        {
            get { return mParent.FillableBackTintAndShade; }
            set { mParent.FillableBackTintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets value that is <c>true</c> if the formatting applied to this instance, is visible.
        /// </summary>
        public bool Visible
        {
            get { return mParent.FillableVisible; }
            set { mParent.FillableVisible = value; }
        }

        /// <summary>
        /// Gets or sets the degree of transparency of the specified fill as a value between 0.0 (opaque) and 1.0 (clear).
        /// </summary>
        /// <remarks>This property is the opposite of property <see cref="Opacity"/>.</remarks>
        /// <dev>See https://docs.microsoft.com/en-us/office/vba/api/word.fillformat for details.</dev>
        public double Transparency
        {
            get { return mParent.FillableTransparency; }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Transparency");
                mParent.FillableTransparency = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the fill rotates with the specified object.
        /// </summary>
        /// <dev>
        /// This property is applicable to shape objects only.
        /// When accessing this property in VBA for text an exception is thrown.
        /// </dev>
        public bool RotateWithObject
        {
            get { return mParent.RotateWithObject; }
            set { mParent.RotateWithObject = value; }
        }

        /// <summary>
        /// Gets a fill type.
        /// </summary>
        public FillType FillType
        {
            get
            {
                if (!mParent.FillableVisible)
                    return FillType.Solid;

                switch (mParent.FillType)
                {
                    case FillTypeCore.Solid:
                        return FillType.Solid;
                    case FillTypeCore.Pattern:
                        return FillType.Patterned;
                    case FillTypeCore.Texture:
                        return FillType.Textured;
                    case FillTypeCore.Picture:
                        return FillType.Picture;
                    case FillTypeCore.Shade:
                    case FillTypeCore.ShadeCenter:
                    case FillTypeCore.ShadeShape:
                    case FillTypeCore.ShadeScale:
                    case FillTypeCore.ShadeTitle:
                    case FillTypeCore.ShadeUnscale:
                        return FillType.Gradient;
                    case FillTypeCore.Background:
                        return FillType.Background;
                    default:
                        throw new ArgumentOutOfRangeException("FillType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        public double GradientAngle
        {
            get
            { return mParent.GradientAngle; }
            set { mParent.GradientAngle = value; }
        }

        /// <summary>
        /// Gets the gradient variant <see cref="Drawing.GradientVariant"/> for the fill.
        /// </summary>
        public GradientVariant GradientVariant
        {
            get { return mParent.GradientVariant; }
        }

        /// <summary>
        /// Gets the gradient style <see cref="Drawing.GradientStyle"/> for the fill.
        /// </summary>
        public GradientStyle GradientStyle
        {
            get { return mParent.GradientStyle; }
        }

        /// <summary>
        /// Gets a collection of <see cref="GradientStop"/> objects for the fill.
        /// </summary>
        public GradientStopCollection GradientStops
        {
            get { return mParent.GradientStops; }
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IFillable mParent;

        private const string ErrorPatterned = "PatternType.None cannot be applied directly.";
    }
}
