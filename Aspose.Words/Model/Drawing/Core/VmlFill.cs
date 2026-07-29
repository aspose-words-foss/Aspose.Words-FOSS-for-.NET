// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2014 by Andrey Noskov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing.Core
{
    internal class VmlFill : IFill
    {
        /// <summary>
        /// Sets one of the predefined static focus positions corresponded to a particular <see cref="GradientStyle"/>
        /// and Variant to a specified shape attributes.
        /// </summary>
        internal static void SetFocusPositions(ShapePr shapePr, GradientStyle style, GradientVariant variant)
        {
            double[] focusPositions = GetFocusPositions(style, variant);
            shapePr.SetAttrIfNotDefault(ShapeAttr.FillToLeft, ConvertUtilCore.DoubleToFixed(focusPositions[0]));
            shapePr.SetAttrIfNotDefault(ShapeAttr.FillToTop, ConvertUtilCore.DoubleToFixed(focusPositions[1]));
            shapePr.SetAttrIfNotDefault(ShapeAttr.FillToRight, ConvertUtilCore.DoubleToFixed(focusPositions[2]));
            shapePr.SetAttrIfNotDefault(ShapeAttr.FillToBottom, ConvertUtilCore.DoubleToFixed(focusPositions[3]));
        }

        /// <summary>
        /// Gets gradient focus corresponded to a specified <see cref="GradientStyle"/> and Variant.
        /// </summary>
        internal static int GetGradientFocus(GradientStyle style, GradientVariant variant)
        {
            switch (style)
            {
                case GradientStyle.Horizontal:
                case GradientStyle.Vertical:
                case GradientStyle.DiagonalUp:
                case GradientStyle.DiagonalDown:
                    return GetLinearGradientFocus(style, variant);
                case GradientStyle.FromCorner:
                    return GetFromCornerGradientFocus(variant);
                case GradientStyle.FromCenter:
                    return GetFromCenterGradientFocus(variant);
                default:
                    throw new ArgumentOutOfRangeException("style", style, ValueOutOfRange);
            }
        }

        /// <summary>
        /// Returns angle corresponding to a specified gradient style.
        /// </summary>
        internal static double GetLinearGradientAngle(GradientStyle style)
        {
            switch (style)
            {
                case GradientStyle.Horizontal:
                    return 0;
                case GradientStyle.Vertical:
                    return -90;
                case GradientStyle.DiagonalUp:
                    return -135;
                case GradientStyle.DiagonalDown:
                    return -45;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns style of a linear gradient by specified angle.
        /// </summary>
        private static GradientStyle GetLinearGradientStyle(double angle)
        {
            // Fit value to diapason from -360 to 360 degrees.
            if (System.Math.Abs(angle) > 360)
                angle %= 360;

            if (MathUtil.IsZero(angle))
                return GradientStyle.Horizontal;

            if (MathUtil.AreEqual(angle, -90.0))
                return GradientStyle.Vertical;

            if (MathUtil.AreEqual(angle, -135.0))
                return GradientStyle.DiagonalUp;

            if (MathUtil.AreEqual(angle, -45.0))
                return GradientStyle.DiagonalDown;

            return GradientStyle.None;
        }

        /// <summary>
        /// Gets the gradient variant for the 'FromCorner' fills.
        /// </summary>
        private GradientVariant GetFromCornerGradientVariant()
        {
            double[] focusPositios = new[] { FocusLeft, FocusTop, FocusRight, FocusBottom };
            if (ArrayUtil.IsArrayEqual(focusPositios, gCornerRect1))
                return GradientVariant.Variant1;

            if (ArrayUtil.IsArrayEqual(focusPositios, gCornerRect2))
                return GradientVariant.Variant2;

            if (ArrayUtil.IsArrayEqual(focusPositios, gCornerRect3))
                return GradientVariant.Variant3;

            if (ArrayUtil.IsArrayEqual(focusPositios, gCornerRect4))
                return GradientVariant.Variant4;

            return GradientVariant.None;
        }

        /// <summary>
        /// Gets the gradient variant for the 'FromCenter' fills.
        /// </summary>
        private GradientVariant GetFromCenterGradientVariant()
        {
            switch (Focus)
            {
                case 0:
                    return GradientVariant.Variant2;
                case 100:
                    return GradientVariant.Variant1;
                default:
                    return GradientVariant.None;
            }
        }

        /// <summary>
        /// Gets the gradient variant for the linear gradient fills.
        /// </summary>
        private GradientVariant GetLinearGradientVariant(GradientStyle style)
        {
            Debug.Assert(
                (style == GradientStyle.Horizontal) ||
                (style == GradientStyle.Vertical) ||
                (style == GradientStyle.DiagonalDown) ||
                (style == GradientStyle.DiagonalUp));

            switch (Focus)
            {
                case 50:
                    return (style == GradientStyle.Horizontal) ? GradientVariant.Variant3 : GradientVariant.Variant4;
                case -50:
                    return (style == GradientStyle.Horizontal) ? GradientVariant.Variant4 : GradientVariant.Variant3;
                case 100:
                    return (style == GradientStyle.DiagonalDown) ? GradientVariant.Variant2 : GradientVariant.Variant1;
                case 0:
                    return (style == GradientStyle.DiagonalDown) ? GradientVariant.Variant1 : GradientVariant.Variant2;
                default:
                    return GradientVariant.None;
            }
        }

        /// <summary>
        /// Gets the gradient focus for the linear gradient fills.
        /// </summary>
        private static int GetLinearGradientFocus(GradientStyle style, GradientVariant variant)
        {
            Debug.Assert(
                (style == GradientStyle.Horizontal) ||
                (style == GradientStyle.Vertical) ||
                (style == GradientStyle.DiagonalDown) ||
                (style == GradientStyle.DiagonalUp));

            switch (variant)
            {
                case GradientVariant.Variant4:
                    return (style == GradientStyle.Horizontal) ? -50 : 50;
                case GradientVariant.Variant3:
                    return (style == GradientStyle.Horizontal) ? 50 : -50;
                case GradientVariant.Variant2:
                    return (style == GradientStyle.DiagonalDown) ? 100 : 0;
                case GradientVariant.Variant1:
                    return (style == GradientStyle.DiagonalDown) ? 0 : 100;
                default:
                    throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
            }
        }

        /// <summary>
        /// Gets the gradient focus for the 'FromCenter' fills.
        /// </summary>
        private static int GetFromCenterGradientFocus(GradientVariant variant)
        {
            switch (variant)
            {
                case GradientVariant.Variant1:
                    return 100;
                case GradientVariant.Variant2:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
            }
        }

        /// <summary>
        /// Gets the gradient focus for the 'FromCorner' fills.
        /// </summary>
        private static int GetFromCornerGradientFocus(GradientVariant variant)
        {
            switch (variant)
            {
                case GradientVariant.Variant1:
                case GradientVariant.Variant2:
                case GradientVariant.Variant3:
                case GradientVariant.Variant4:
                    return 100;
                default:
                    throw new ArgumentOutOfRangeException("variant", variant, ValueOutOfRange);
            }
        }

        /// <summary>
        /// Returns one of the predefined static focus positions corresponded
        /// to a particular <see cref="GradientStyle"/> and Variant.
        /// </summary>
        /// <remarks>
        /// Note, this should be immutable.
        /// </remarks>
        private static double[] GetFocusPositions(GradientStyle style, GradientVariant variant)
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

        private void SetOpacityAttrCore(double value, int key)
        {
            if ((value < 0.0) || (value > 1.0))
                throw new ArgumentOutOfRangeException("value");
            SetAttr(key, ConvertUtilCore.DoubleToFixed(value));
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        private object FetchAttr(int key)
        {
            return Shape.FetchShapeAttrInternal(key);
        }

        private void SetAttr(int key, object value)
        {
            Shape.SetShapeAttrInternal(key, value);
        }

        private void RemoveAttr(int key)
        {
            Shape.RemoveShapeAttrInternal(key);
        }

        #region IFill interface implementation.
        public void SetImageBytes(byte[] imageBytes)
        {
            FillType = FillTypeCore.Picture;

            SetAttr(ShapeAttr.FillImageBytes, imageBytes);
            SetAttr(ShapeAttr.Filled, true);

            // WORDSNET-16357 Remove possible existing texture.
            RemoveAttr(ShapeAttr.FillPresetTexture);
        }

        /// <summary>
        /// Gets or sets a color.
        /// </summary>
        public DrColor ColorInternal
        {
            get { return (DrColor)FetchAttr(ShapeAttr.FillColor); }
            set { SetAttr(ShapeAttr.FillColor, value); }
        }

        /// <summary>
        /// Gets a base color without modifiers.
        /// </summary>
        public DrColor ColorInternalUnmodified
        {
            get { return ColorInternal; }
        }

        /// <summary>
        /// Gets or sets a background color.
        /// </summary>
        public DrColor Color2Internal
        {
            get { return (DrColor)FetchAttr(ShapeAttr.FillBackColor); }
            set { SetAttr(ShapeAttr.FillBackColor, value); }
        }

        /// <summary>Gets the array of custom gradient colors.</summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        public GradientColor[] GradientColors
        {
            get { return (GradientColor[])FetchAttr(ShapeAttr.FillShadeColors); }
        }

        public double Opacity
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillOpacity)); }
            set { SetOpacityAttrCore(value, ShapeAttr.FillOpacity); }
        }

        public double Opacity2
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillBackOpacity)); }
            set { SetOpacityAttrCore(value, ShapeAttr.FillBackOpacity); }
        }

        bool IFill.On
        {
            get { return (bool)FetchAttr(ShapeAttr.Filled); }
            set { SetAttr(ShapeAttr.Filled, value); }
        }

        public byte[] ImageBytes
        {
            get { return (byte[])FetchAttr(ShapeAttr.FillImageBytes); }
        }

        public double Angle
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillAngle)); }
            set { SetAttr(ShapeAttr.FillAngle, ConvertUtilCore.DoubleToFixed(value)); }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientstyle
        /// </summary>
        GradientStyle IFill.GradientStyle
        {
            get
            {
                if (Shape == null)
                    return GradientStyle.None;

                FillTypeCore fillType = (FillTypeCore)FetchAttr(ShapeAttr.FillType);
                switch (fillType)
                {
                    // Path gradient.
                    case FillTypeCore.ShadeCenter:
                        return GradientStyle.FromCorner;
                    case FillTypeCore.ShadeShape:
                        return GradientStyle.FromCenter;
                    // Linear gradient.
                    case FillTypeCore.Shade:
                    case FillTypeCore.ShadeTitle:
                    case FillTypeCore.ShadeUnscale:
                    case FillTypeCore.ShadeScale:
                        return GetLinearGradientStyle(Angle);
                    default:
                        return GradientStyle.None;
                }
            }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills or 0 if not defined.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientvariant
        /// </summary>
        GradientVariant IFill.GradientVariant
        {
            get
            {
                GradientStyle style = ((IFill)this).GradientStyle;
                switch (style)
                {
                    case GradientStyle.Horizontal:
                    case GradientStyle.Vertical:
                    case GradientStyle.DiagonalUp:
                    case GradientStyle.DiagonalDown:
                        return GetLinearGradientVariant(style);
                    case GradientStyle.FromCorner:
                        return GetFromCornerGradientVariant();
                    case GradientStyle.FromCenter:
                        return GetFromCenterGradientVariant();
                    default:
                        return GradientVariant.None;
                }
            }
        }

        /// <summary>
        /// Determines whether the fill rotates with the specified shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        /// <dev>
        /// IN. Word VBA does not allow access to this property even in getter. But GUI allows full access
        /// for the all fills except of Solid and Pattern. So, I think we should allow this property as well.
        /// </dev>
        public bool RotateWithShape
        {
            get { return (bool)FetchAttr(ShapeAttr.FillUseShapeAnchor); }
            set { SetAttr(ShapeAttr.FillUseShapeAnchor, value); }
        }

        public bool LockAspectRatio
        {
            get { return (FillDimensionType)FetchAttr(ShapeAttr.FillDimensionType) != FillDimensionType.Default; }
            set { SetAttr(ShapeAttr.FillDimensionType, value ? FillDimensionType.FixedAspect : FillDimensionType.Default); }
        }

        /// <summary>
        /// Defines the left position of the center of a radial gradient.
        /// </summary>
        /// <remarks>
        /// <p>The value is a fraction of the width of the shape.</p>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public double FocusLeft
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillToLeft)); }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.FillToLeft, ConvertUtilCore.DoubleToFixed(value));
            }
        }

        /// <summary>
        /// Defines the top position of the center of a radial gradient.
        /// </summary>
        /// <remarks>
        /// <p>The value is a fraction of the height of the shape.</p>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public double FocusTop
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillToTop)); }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.FillToTop, ConvertUtilCore.DoubleToFixed(value));
            }
        }

        /// <summary>
        /// Defines the right position of the center of a radial gradient.
        /// </summary>
        /// <remarks>
        /// <p>The value is a fraction of the width of the shape.</p>
        /// <p>The default value is 0.</p>
        /// </remarks>
        private double FocusRight
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillToRight)); }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.FillToRight, ConvertUtilCore.DoubleToFixed(value));
            }
        }

        /// <summary>
        /// Defines the bottom position of the center of a radial gradient.
        /// </summary>
        /// <remarks>
        /// <p>The value is a fraction of the height of the shape.</p>
        /// <p>The default value is 0.</p>
        /// </remarks>
        private double FocusBottom
        {
            get { return ConvertUtilCore.FixedToDouble((int)FetchAttr(ShapeAttr.FillToBottom)); }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");
                SetAttr(ShapeAttr.FillToBottom, ConvertUtilCore.DoubleToFixed(value));
            }
        }

        /// <summary>
        /// Defines the center of a linear gradient fill.
        /// </summary>
        /// <remarks>
        /// <p>Defines the center starting position of the blend. Values range from -100 to 100.
        /// The default value is 0.</p>
        ///
        /// <p>A value of 100 or -100 will shift the focus so that the direction of the blend will reverse
        /// (in effect reversing Color and Color2). A value of 50 will change the blend so that Color is at
        /// both ends and Color2 is in the middle. A value of -50 will change the blend so that Color2 is at
        /// both ends and Color is in the middle.</p>
        /// </remarks>
        public int Focus
        {
            get { return (int)FetchAttr(ShapeAttr.FillFocus); }
            set
            {
                if ((value < -100) || (value > 100))
                    throw new ArgumentOutOfRangeException("value");

                SetAttr(ShapeAttr.FillFocus, value);
            }
        }

        /// <summary>
        /// Defines the type of fill.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="FillTypeCore.Solid"/>.</p>
        /// </remarks>
        public FillTypeCore FillType
        {
            get { return (FillTypeCore)FetchAttr(ShapeAttr.FillType); }
            set { SetAttr(ShapeAttr.FillType, value); }
        }

        /// <summary>
        /// Gets or sets parent object.
        /// </summary>
        IFillable IFill.Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        #endregion

        /// <summary>
        /// Gets parent Shape object.
        /// </summary>
        private ShapeBase Shape
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mParent as ShapeBase; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private IFillable mParent;

        /// <summary>
        /// Predefined focus positions corresponded to a particular <see cref="GradientStyle"/> and Variant.
        /// </summary>
        private static readonly double[] gCornerRect1 = new double[] {0.0, 0.0, 0.0, 0.0};
        private static readonly double[] gCornerRect2 = new double[] { 1.0, 0.0, 1.0, 0.0 };
        private static readonly double[] gCornerRect3 = new double[] { 0.0, 1.0, 0.0, 1.0 };
        private static readonly double[] gCornerRect4 = new double[] { 1.0, 1.0, 1.0, 1.0 };
        private static readonly double[] gCenterRect = new double[] {0.5 , 0.5, 0.5, 0.5 };

        private const string ValueOutOfRange = "The specified value is out of range.";
    }
}
