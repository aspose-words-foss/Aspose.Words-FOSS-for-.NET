// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/10/2014 by Andrey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.RW.Vml;

namespace Aspose.Words.Drawing.Core
{
    internal class VmlNode : Graphic
    {
        internal VmlNode(ShapeBase shape)
        {
            mShape = shape;
        }

        internal override void SetCoordSizeWidthSafe(int width)
        {
            SetCoordSizeSafeCore(ShapeAttr.CoordSizeWidth, width);
        }

        internal override void SetCoordSizeHeightSafe(int height)
        {
            SetCoordSizeSafeCore(ShapeAttr.CoordSizeHeight, height);
        }

        internal override void SetWidthCore(double value, bool isThrow)
        {
            mShape.SetShapeAttrInternal(ShapeAttr.Width,
                ShapeSizeValidationHelper.ValidateDimension(mShape, value, isThrow, "width"));
        }

        internal override void SetHeightCore(double value, bool isThrow)
        {
            mShape.SetShapeAttrInternal(ShapeAttr.Height,
                ShapeSizeValidationHelper.ValidateDimension(mShape, value, isThrow, "height"));
        }

        /// <summary>
        /// Removes shadow effect from a shape.
        /// </summary>
        internal override void RemoveShadow()
        {
            mShape.ShapePr.Remove(ShapeAttr.ShadowOn);
            mShape.ShapePr.Remove(ShapeAttr.ShadowType);
            mShape.ShapePr.Remove(ShapeAttr.ShadowColor);
            mShape.ShapePr.Remove(ShapeAttr.ShadowHighlight);
            mShape.ShapePr.Remove(ShapeAttr.ImageDblCrMod);
            mShape.ShapePr.Remove(ShapeAttr.ShadowOpacity);
            mShape.ShapePr.Remove(ShapeAttr.ShadowOffsetX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowOffsetY);
            mShape.ShapePr.Remove(ShapeAttr.ShadowOriginX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowOriginY);
            mShape.ShapePr.Remove(ShapeAttr.ShadowScaleXtoX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowScaleXtoY);
            mShape.ShapePr.Remove(ShapeAttr.ShadowScaleYtoX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowScaleYtoY);
            mShape.ShapePr.Remove(ShapeAttr.ShadowSecondOffsetX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowSecondOffsetY);
            mShape.ShapePr.Remove(ShapeAttr.ShadowPerspectiveX);
            mShape.ShapePr.Remove(ShapeAttr.ShadowPerspectiveY);
        }

        private void SetCoordSizeSafeCore(int key, int value)
        {
            if (value < 0)
                value = (int)mShape.FetchInheritedShapeAttrInternal(key);

            mShape.SetShapeAttrInternal(key, value);
        }

        /// <summary>
        /// Gets the collection of adjustment values that are applied to the shape.
        /// </summary>
        private AdjustmentCollection GetAdjustmentCollection()
        {
            ShapePr parentPr = ShapeTypeLibrary.GetShapeTypePr(ShapeType);

            AdjustmentCollection adjustments = new AdjustmentCollection();
            for (int adjustKey = ShapeAttr.GeometryAdjust1; adjustKey <= ShapeAttr.GeometryAdjust10; ++adjustKey)
            {
                object adjustObj = mShape.ShapePr[adjustKey];
                if ((adjustObj == null) && (parentPr != null))
                    adjustObj = parentPr[adjustKey];

                if (adjustObj == null)
                    continue;

                int index = adjustKey - ShapeAttr.GeometryAdjust1 + 1;
                int adjustValue = (int)adjustObj;
                adjustments.Add(new Adjustment(index, adjustValue, mShape));
            }

            return adjustments;
        }

        /// <summary>
        /// Sets value to the <see cref="ShapePr"/> collection if value is not null,
        /// or remove it from the collection otherwise.
        /// </summary>
        private void SetAttrSafe(int key, object value)
        {
            if (value == null)
                mShape.ShapePr.Remove(key);
            else
                mShape.ShapePr[key] = value;
        }

        private void SetCropCoefficient(int key, double value)
        {
            if ((value < -1.0) || (value > 1.0))
                throw new ArgumentOutOfRangeException("value");

            mShape.SetShapeAttrInternal(key, ConvertUtilCore.DoubleToFixed(value));
        }

        public override bool IsHorizontalRule
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.HROn); }
        }

        public override bool IsWordArt
        {
            get
            {
                // WORDSNET-5500 It appears that not any shape can be treated as a WordArt object,
                // but only which have a special ShapeType, because of a WordArt requirements to the paths.
                return VmlUtil.IsWordArt(ShapeType, (bool)mShape.FetchShapeAttrInternal(ShapeAttr.GeoTextOn));
            }
        }

        public override string Name
        {
            get { return (string) mShape.ShapePr.FetchAttr(ShapeAttr.ShapeName); }
            set { mShape.ShapePr.SetAttr(ShapeAttr.ShapeName, value); }
        }

        public override string AlternativeText
        {
            get { return (string) mShape.ShapePr.FetchAttr(ShapeAttr.ShapeDescription); }
            set { mShape.ShapePr.SetAttr(ShapeAttr.ShapeDescription, value); }
        }

        public override bool Decorative
        {
            get
            {
                object value = mShape.ShapePr.FetchAttr(ShapeAttr.ShapeDescription);
                return ((string)value == "\"\"");
            }

            set
            {
                if (value)
                    mShape.ShapePr.SetAttr(ShapeAttr.ShapeDescription, "\"\"");
                else if ((string)mShape.ShapePr.FetchAttr(ShapeAttr.ShapeDescription) == "\"\"")
                    mShape.ShapePr.Remove(ShapeAttr.ShapeDescription);
            }
        }

        public override string Title
        {
            get { return (string)mShape.ShapePr.FetchAttr(ShapeAttr.ShapeTitle); }
            set { mShape.ShapePr.SetAttr(ShapeAttr.ShapeTitle, value); }
        }

        internal override bool Hidden
        {
            get { return (bool)mShape.ShapePr.FetchAttr(ShapeAttr.Hidden); }
            set { mShape.ShapePr.SetAttr(ShapeAttr.Hidden, value); }
        }

        /// <summary>
        /// Gets the shape type.
        /// </summary>
        public override ShapeType ShapeType
        {
            get
            {
                // Use shape properties directly to avoid endless recursion.
                return (ShapeType)mShape.ShapePr.FetchAttr(ShapeAttr.ShapeType);
            }
        }

        /// <summary>
        /// Provides access to the OLE data of a shape. For a shape that is not an OLE object or ActiveX control, returns <c>null</c>.
        /// </summary>
        public override OleFormat OleFormat
        {
            get
            {
                if (!mShape.IsOle)
                    return null;

                if (mOleFormatCache == null)
                    mOleFormatCache = new OleFormat(mShape);

                return mOleFormatCache;
            }
        }

        /// <summary>
        /// Switches the orientation of a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.FlipOrientation.None"/>.</p>
        /// </remarks>
        public override FlipOrientation FlipOrientation
        {
            get { return (FlipOrientation)mShape.FetchShapeAttrInternal(ShapeAttr.Flip); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.Flip, value); }
        }

        /// <summary>
        /// Defines the angle (in degrees) that a shape is rotated.
        /// Positive value corresponds to clockwise rotation angle.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public override double Rotation
        {
            get { return ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.TransformRotation)); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TransformRotation, ConvertUtilCore.DoubleToFixed(value)); }
        }

        /// <summary>
        /// Represents adjustment values that correspond to the positions of the adjust handles of the shape.
        /// </summary>
        public override AdjustmentCollection Adjustments
        {
            get
            {
                if (mAdjustments == null)
                    mAdjustments = GetAdjustmentCollection();

                return mAdjustments;
            }
        }

        /// <summary>
        /// Defines the text of the text path (of a WordArt object).
        /// </summary>
        public override TextPath TextPath
        {
            get
            {
                if (mTextPathCache == null)
                    mTextPathCache = new TextPath(mShape);
                return mTextPathCache;
            }
        }

        /// <summary>
        /// Returns true if an extrusion effect is enabled.
        /// </summary>
        public override bool ExtrusionEnabled
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.TDOn); }
        }

        /// <summary>
        /// Returns true if a shadow effect is enabled.
        /// </summary>
        public override bool ShadowEnabled
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.ShadowOn); }
        }

        #region ImageData properties.
        public override double Brightness
        {
            get
            {
                return ImageData.BrightnessToPercent(
                    ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageBrightness)));
            }
            set
            {
                mShape.SetShapeAttrInternal(ShapeAttr.ImageBrightness,
                    ConvertUtilCore.DoubleToFixed(ImageData.PercentToBrightness(value)));
            }
        }

        public override double Contrast
        {
            get
            {
                return ImageData.ContrastToPercent(
                        ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageContrast)));
            }
            set
            {
                mShape.SetShapeAttrInternal(ShapeAttr.ImageContrast, ConvertUtilCore.DoubleToFixed(ImageData.PercentToContrast(value)));
            }
        }

        public override bool BiLevel
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.ImageBiLevel); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.ImageBiLevel, value); }
        }

        public override bool GrayScale
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.ImageGrayScale); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.ImageGrayScale, value); }
        }

        public override double CropTop
        {
            get { return ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageCropTop)); }
            set { SetCropCoefficient(ShapeAttr.ImageCropTop, value); }
        }

        public override double CropBottom
        {
            get { return ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageCropBottom)); }
            set { SetCropCoefficient(ShapeAttr.ImageCropBottom, value); }
        }

        public override double CropLeft
        {
            get { return ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageCropLeft)); }
            set { SetCropCoefficient(ShapeAttr.ImageCropLeft, value); }
        }

        public override double CropRight
        {
            get { return ConvertUtilCore.FixedToDouble((int)mShape.FetchShapeAttrInternal(ShapeAttr.ImageCropRight)); }
            set { SetCropCoefficient(ShapeAttr.ImageCropRight, value); }
        }
        #endregion

        #region TextBox properties.
        public override double InternalMarginLeft
        {
            get { return ConvertUtilCore.EmuToPoint((int)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxLeft)); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxLeft, ConvertUtilCore.PointToEmu(value)); }
        }

        public override double InternalMarginRight
        {
            get { return ConvertUtilCore.EmuToPoint((int)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxRight)); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxRight, ConvertUtilCore.PointToEmu(value)); }
        }

        public override double InternalMarginTop
        {
            get { return ConvertUtilCore.EmuToPoint((int)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxTop)); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxTop, ConvertUtilCore.PointToEmu(value)); }
        }

        public override double InternalMarginBottom
        {
            get { return ConvertUtilCore.EmuToPoint((int)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxBottom)); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxBottom, ConvertUtilCore.PointToEmu(value)); }
        }

        public override bool FitShapeToText
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxFitShapeToText); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxFitShapeToText, value); }
        }

        public override LayoutFlow LayoutFlow
        {
            get { return (LayoutFlow)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxLayoutFlow); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxLayoutFlow, value); }
        }

        public override TextBoxWrapMode TextBoxWrapMode
        {
            get { return (TextBoxWrapMode)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxWrapMode); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxWrapMode, value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either text of the TextBox should not rotate when the shape is rotated.
        /// </summary>
        internal override bool TextBoxNoTextRotation
        {
            get { return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxRotateText); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxRotateText, value); }
        }

        internal override TextBoxAnchor TextBoxAnchor
        {
            get { return (TextBoxAnchor)mShape.FetchShapeAttrInternal(ShapeAttr.TextboxAnchor); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.TextboxAnchor, value); }
        }
        #endregion

        internal override int CoordSizeWidth
        {
            get { return (int)mShape.FetchShapeAttrInternal(ShapeAttr.CoordSizeWidth); }
        }

        internal override int CoordSizeHeight
        {
            get { return (int)mShape.FetchShapeAttrInternal(ShapeAttr.CoordSizeHeight); }
        }

        internal override int CoordOriginX
        {
            get { return (int)mShape.FetchShapeAttrInternal(ShapeAttr.CoordOriginX); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.CoordOriginX, value); }
        }

        internal override int CoordOriginY
        {
            get { return (int)mShape.FetchShapeAttrInternal(ShapeAttr.CoordOriginY); }
            set { mShape.SetShapeAttrInternal(ShapeAttr.CoordOriginY, value); }
        }

        internal override bool AspectRatioLocked
        {
            get
            {
                ShapeBase topLevelShape = mShape.GetTopLevelParentShape();

                // WORDSNET-13812 Returns direct or inherited from shape type LockAspectRatio value.
                return (bool)topLevelShape.FetchShapeAttrInternal(ShapeAttr.LockAspectRatio);
            }
            set { mShape.SetShapeAttrInternal(ShapeAttr.LockAspectRatio, value); }
        }

        /// <summary>
        /// Provides access to the properties of the horizontal rule shape.
        /// </summary>
        internal override HorizontalRule HorizontalRule
        {
            get
            {
                if (mHorizontalRuleCache == null)
                    mHorizontalRuleCache = new HorizontalRule(mShape);
                return mHorizontalRuleCache;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> object for a shape.
        /// </summary>
        internal override ShadowType ShadowType
        {
            get
            {
                ShapePr shapePr = mShape.ShapePr;
                if ((shapePr[ShapeAttr.ShadowOn] == null) || !(bool)shapePr[ShapeAttr.ShadowOn] ||
                    (shapePr[ShapeAttr.ShadowScaleXtoY] != null) || (shapePr[ShapeAttr.ShadowPerspectiveX] != null))
                    return ShadowType.ShadowMixed;

                foreach (ShadowType type in ShapeTypeVmlDefaults.Keys)
                {
                    object[] shapeTypeVmlDefaults = ShapeTypeVmlDefaults[type];
                    if (Equals(shapePr[ShapeAttr.ShadowType], shapeTypeVmlDefaults[0]) &&
                        Equals(shapePr[ShapeAttr.ShadowHighlight], shapeTypeVmlDefaults[1]) &&
                        Equals(shapePr[ShapeAttr.ImageDblCrMod], shapeTypeVmlDefaults[2]) &&
                        Equals(shapePr[ShapeAttr.ShadowOffsetX], shapeTypeVmlDefaults[4]) &&
                        Equals(shapePr[ShapeAttr.ShadowOffsetY], shapeTypeVmlDefaults[5]) &&
                        Equals(shapePr[ShapeAttr.ShadowScaleXtoX], shapeTypeVmlDefaults[6]) &&
                        Equals(shapePr[ShapeAttr.ShadowSecondOffsetX], shapeTypeVmlDefaults[7]) &&
                        Equals(shapePr[ShapeAttr.ShadowSecondOffsetY], shapeTypeVmlDefaults[8]) &&
                        Equals(shapePr[ShapeAttr.ShadowScaleYtoX], shapeTypeVmlDefaults[9]) &&
                        Equals(shapePr[ShapeAttr.ShadowScaleYtoY], shapeTypeVmlDefaults[10]) &&
                        Equals(shapePr[ShapeAttr.ShadowOriginX], shapeTypeVmlDefaults[11]) &&
                        Equals(shapePr[ShapeAttr.ShadowOriginY], shapeTypeVmlDefaults[12]) &&
                        Equals(shapePr[ShapeAttr.ShadowPerspectiveY], shapeTypeVmlDefaults[13]))
                        return type;
                }

                return ShadowType.ShadowMixed;
            }
            set
            {
                object[] shapeTypeVmlDefaults;
                if (!ShapeTypeVmlDefaults.TryGetValue(value, out shapeTypeVmlDefaults))
                    return;

                mShape.ShapePr[ShapeAttr.ShadowOn] = true;
                SetAttrSafe(ShapeAttr.ShadowType, shapeTypeVmlDefaults[0]);
                SetAttrSafe(ShapeAttr.ShadowHighlight, shapeTypeVmlDefaults[1]);
                SetAttrSafe(ShapeAttr.ImageDblCrMod, shapeTypeVmlDefaults[2]);
                SetAttrSafe(ShapeAttr.ShadowOpacity, shapeTypeVmlDefaults[3]);
                SetAttrSafe(ShapeAttr.ShadowOffsetX, shapeTypeVmlDefaults[4]);
                SetAttrSafe(ShapeAttr.ShadowOffsetY, shapeTypeVmlDefaults[5]);
                SetAttrSafe(ShapeAttr.ShadowScaleXtoX, shapeTypeVmlDefaults[6]);
                SetAttrSafe(ShapeAttr.ShadowSecondOffsetX, shapeTypeVmlDefaults[7]);
                SetAttrSafe(ShapeAttr.ShadowSecondOffsetY, shapeTypeVmlDefaults[8]);
                SetAttrSafe(ShapeAttr.ShadowScaleYtoX, shapeTypeVmlDefaults[9]);
                SetAttrSafe(ShapeAttr.ShadowScaleYtoY, shapeTypeVmlDefaults[10]);
                SetAttrSafe(ShapeAttr.ShadowOriginX, shapeTypeVmlDefaults[11]);
                SetAttrSafe(ShapeAttr.ShadowOriginY, shapeTypeVmlDefaults[12]);
                SetAttrSafe(ShapeAttr.ShadowPerspectiveY, shapeTypeVmlDefaults[13]);
            }
        }

        /// <summary>
        /// Gets or sets color of a shadow effect.
        /// </summary>
        internal override Color ShadowColor
        {
            get
            {
                object color = mShape.ShapePr[ShapeAttr.ShadowColor];
                if (color == null)
                    return Color.Black;

                DrColor drColor = (DrColor)color;
                object opacity = mShape.ShapePr[ShapeAttr.ShadowOpacity];
                if (opacity != null)
                {
                    int alpha = (int)System.Math.Round(((int)opacity * 255 / 65535.0));
                    drColor = DrColor.FromArgb(alpha, drColor.R, drColor.G, drColor.B);
                }

                return drColor.ToNativeColor();
            }
            set
            {
                mShape.SetShapeAttrInternal(ShapeAttr.ShadowColor, DrColor.FromNativeColor(value));
            }
        }

        /// <summary>
        /// Gets or sets transparency of a shadow effect.
        /// </summary>
        internal override double ShadowTransparency
        {
            get
            {
                object opacity = mShape.ShapePr[ShapeAttr.ShadowOpacity];
                if (opacity == null)
                    return 0.0;

                return 1.0 - (int)opacity / 65535.0;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Shadow.Transparency");

                int opacity = (int)System.Math.Round((1 - value) * 65535);
                mShape.SetShapeAttrInternal(ShapeAttr.ShadowOpacity, opacity);         
            }
        }

        /// <summary>
        /// Returns VML attribute default values for the corresponding shadow types.
        /// </summary>
        private static Dictionary<ShadowType, object[]> ShapeTypeVmlDefaults
        {
            get
            {
                if (gShapeTypeVmlPresets == null)
                {
                    gShapeTypeVmlPresets = new Dictionary<ShadowType, object[]>();

                    gShapeTypeVmlPresets.Add(ShadowType.Shadow1, new object[]
                        { null, null, null, 32768, -76200, -76200, null, null, null, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow2, new object[]
                        { null, null, null, 32768, 76200, -76200, null, null, null, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow3, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, 56756, 32768, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow4, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, -56756, 32768, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow5, new object[]
                        { null, null, null, 32768, -76200, 76200, null, null, null, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow6, new object[]
                        { null, null, null, 32768, 76200, 76200, null, null, null, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow7, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, 56756, -32768, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow8, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, -56756, -32768, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow9, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, -76200, -76200, 49152, null, null, null, 49152, -32768, -32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow10, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, -76200, -76200, 81920, null, null, null, 81920, 32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow11, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, 92680, null, -32768, 32768, -16 });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow12, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, -92680, null, 32768, 32768, -16 });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow13, new object[] { ShadowTypeCore.Double,
                        DrColor.FromArgb(0xEF, 0xF3, 0x03, 0x66), null, 32768, -38100, -38100, null, -76200, -76200, null,
                        null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow14, new object[]
                        { null, null, null, 32768, null, null, null, null, null, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow15, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, 92680, -65536, -32768, 32768, -16 });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow16, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, -92680, -65536, 32768, 32768, -16 });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow17, new object[] { ShadowTypeCore.EmbossOrEngrave, DrColor.FromArgb(0xEF,
                            0xF3, 0x03, 0x66), DrColor.FromArgb(0xEF, 0xF3, 0x03, 0x33), 32768, 12700, 12700, null,
                        -12700, -12700, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow18, new object[] { ShadowTypeCore.EmbossOrEngrave, DrColor.FromArgb(0xEF,
                            0xF3, 0x03, 0x66), DrColor.FromArgb(0xEF, 0xF3, 0x03, 0x33), 32768, -12700, -12700, null, 12700,
                        12700, null, null, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow19, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, null, 32768, null, 32768, -8 });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow20, new object[]
                        { ShadowTypeCore.Rich, null, null, 32768, 0, 0, null, null, null, null, -65536, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow21, new object[]
                        { null, null, null, 26214, 26941, 26941, null, null, null, null, null, -32768, -32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow22, new object[]
                        { null, null, null, 26214, 0, 38100, null, null, null, null, null, null, -32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow23, new object[]
                        { null, null, null, 26214, -26941, 26941, null, null, null, null, null, 32768, -32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow24, new object[]
                        { null, null, null, 26214, 38100, 0, null, null, null, null, null, -32768, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow25, new object[]
                        { null, null, null, 26214, 0, 0, 66847, null, null, null, 66847, null, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow26, new object[]
                        { null, null, null, 26214, -38100, 0, null, null, null, null, null, 32768, null, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow27, new object[]
                        { null, null, null, 26214, 26941, -26941, null, null, null, null, null, -32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow28, new object[]
                        { null, null, null, 26214, 0, -38100, null, null, null, null, null, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow29, new object[]
                        { null, null, null, 26214, -26941, -26941, null, null, null, null, null, 32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow39, new object[]
                        { ShadowTypeCore.Rich, null, null, 13107, 0, 0, null, null, null, 23853, 15073, 32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow40, new object[]
                        { ShadowTypeCore.Rich, null, null, 13107, 0, 0, null, null, null, -23853, 15073, -32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow41, new object[]
                        { ShadowTypeCore.Rich, null, null, 9830, 0, 317500, 58982, null, null, null, -12452, null, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow42, new object[]
                        { ShadowTypeCore.Rich, null, null, 13107, -8980, 8980, null, null, null, 15540, -15073, 32768, 32768, null });
                    gShapeTypeVmlPresets.Add(ShadowType.Shadow43, new object[]
                        { ShadowTypeCore.Rich, null, null, 13107, 8980, 8980, null, null, null, -15540, -15073, -32768, 32768, null });
                }

                return gShapeTypeVmlPresets;
            }
        }

        /// <summary>
        /// Default values for VML attributes for the respective shadow types.
        /// </summary>
        private static Dictionary<ShadowType, object[]> gShapeTypeVmlPresets;

        private OleFormat mOleFormatCache;
        private HorizontalRule mHorizontalRuleCache;
        private TextPath mTextPathCache;
        private AdjustmentCollection mAdjustments;
    }
}
