// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Guides;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Base class for concrete Dml shape object.
    /// </summary>
    internal abstract class DmlNode : Graphic, IDmlExtensionListSource
    {
        /// <summary>
        /// Call this method immediately when set DmlNode property of DrawingML.
        /// </summary>
        internal void SetDrawingML(ShapeBase dml)
        {
            mShape = dml;
        }

        protected void SetDmlTransform(DmlTransform transform)
        {
            // If DrawingML is not top level we must set its dimensions from transform.
            // This is required to be able render nested dmls.
            if ((mShape != null) && !mShape.IsTopLevel)
            {
                ShapePr shapePr = mShape.ShapePr;
                shapePr.SetAttr(ShapeAttr.Left, transform.X);
                shapePr.SetAttr(ShapeAttr.Top, transform.Y);
                shapePr.SetAttr(ShapeAttr.Width, transform.Width);
                shapePr.SetAttr(ShapeAttr.Height, transform.Height);
            }
        }

        internal override void SetWidthCore(double value, bool isThrow)
        {
            double validWidth = ShapeSizeValidationHelper.ValidateDimension(mShape, value, isThrow, "width");
            mShape.SetShapeAttrInternal(ShapeAttr.Width, validWidth);
            mShape.SetShapeAttrInternal(ShapeAttr.GraphicFrameExtWidth, (double)ConvertUtilCore.PointToEmu(validWidth));

            // WORDSNET-16239 Convert to EMUs only if shape is not in group.
            if (Dml.IsTopLevel)
                validWidth = ConvertUtilCore.PointToEmu(validWidth);

            Transform.Width = MathUtil.DoubleToInt(validWidth);
        }

        internal override void SetHeightCore(double value, bool isThrow)
        {
            double validHeight = ShapeSizeValidationHelper.ValidateDimension(mShape, value, isThrow, "height");
            mShape.SetShapeAttrInternal(ShapeAttr.Height, validHeight);
            mShape.SetShapeAttrInternal(ShapeAttr.GraphicFrameExtHeight, (double)ConvertUtilCore.PointToEmu(validHeight));

            // WORDSNET-16239 Convert to EMUs only if shape is not in group.
            if (Dml.IsTopLevel)
                validHeight = ConvertUtilCore.PointToEmu(validHeight);

            Transform.Height = MathUtil.DoubleToInt(validHeight);
        }

        internal override void SetCoordSizeWidthSafe(int width)
        {
            DmlGroupTransform groupTransform = Transform as DmlGroupTransform;
            if (groupTransform != null)
                groupTransform.ChildWidth = width;
        }

        internal override void SetCoordSizeHeightSafe(int height)
        {
            DmlGroupTransform groupTransform = Transform as DmlGroupTransform;
            if (groupTransform != null)
                groupTransform.ChildHeight = height;
        }

        /// <summary>
        /// Removes shadow effect from a shape.
        /// </summary>
        internal override void RemoveShadow()
        {
            DmlShapeBase dmlShapeBase = this as DmlShapeBase;
            if ((dmlShapeBase != null) && (dmlShapeBase.Effects != null))
            {
                dmlShapeBase.Effects.RemoveEffect(DmlShapeEffectType.PresetShadow);
                dmlShapeBase.Effects.RemoveEffect(DmlShapeEffectType.InnerShadow);
                dmlShapeBase.Effects.RemoveEffect(DmlShapeEffectType.OuterShadow);

                if (dmlShapeBase.Effects.Count == 0)
                    dmlShapeBase.Effects = null;
            }
        }

        protected virtual void InitializeTransform()
        {
            mTransform = new DmlTransform();
        }

        /// <summary>
        /// Returns false if dimensions of the shape should not be updated from relative values.
        /// </summary>
        internal virtual bool IsUpdateDimensionsFromRelative
        {
            get { return true; }
        }

        internal virtual DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            // This creates a node of the correct class and bitwise copies all fields.
            DmlNode lhs = (DmlNode)MemberwiseClone();

            if (mNonVisualPr != null)
                lhs.NonVisualPr = mNonVisualPr.Clone();

            if (mTransform != null)
                lhs.mTransform = mTransform.Clone();

            lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            // The mModelId field is skipped since it is immutable.

            return lhs;
        }

        internal abstract DmlNodeType DmlNodeType { get; }

        private double GetInternalMargin(int key)
        {
            if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
            {
                DmlTextBodyProperties bodyPr = ((DmlShape)this).TextBodyPr;

                switch (key)
                {
                    case ShapeAttr.TextboxLeft:
                        return ConvertUtilCore.EmuToPoint(bodyPr.LeftInset);
                    case ShapeAttr.TextboxTop:
                        return ConvertUtilCore.EmuToPoint(bodyPr.TopInset);
                    case ShapeAttr.TextboxRight:
                        return ConvertUtilCore.EmuToPoint(bodyPr.RightInset);
                    case ShapeAttr.TextboxBottom:
                        return ConvertUtilCore.EmuToPoint(bodyPr.BottomInset);
                    default:
                        break;
                }
            }

            return 0;
        }

        private void SetInternalMargin(int key, double value)
        {
            if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
            {
                int intValue = ConvertUtilCore.PointToEmu(value);

                DmlShape dmlShape = (DmlShape)this;

                if (dmlShape.TextShape == null)
                    dmlShape.TextShape = new DmlTextShape();

                switch (key)
                {
                    case ShapeAttr.TextboxLeft:
                        dmlShape.TextBodyPr.LeftInset = intValue;
                        break;
                    case ShapeAttr.TextboxTop:
                        dmlShape.TextBodyPr.TopInset = intValue;
                        break;
                    case ShapeAttr.TextboxRight:
                        dmlShape.TextBodyPr.RightInset = intValue;
                        break;
                    case ShapeAttr.TextboxBottom:
                        dmlShape.TextBodyPr.BottomInset = intValue;
                        break;
                    default:
                        break;
                }
            }
        }

        private double GetDmlLuminanceEffectValue(int key)
        {
            if (DmlNodeType == DmlNodeType.Picture)
            {
                DmlBlip dmlBlip = ((DmlPicture)this).BlipFill.Blip;

                if (dmlBlip.Effects.Count > 0)
                {
                    foreach (DmlEffect effect in dmlBlip.Effects)
                    {
                        if (effect.Type == DmlEffectType.Luminance)
                        {
                            switch (key)
                            {
                                case ShapeAttr.ImageBrightness:
                                    return ((DmlLuminanceEffect)effect).Brightness;
                                case ShapeAttr.ImageContrast:
                                    return ((DmlLuminanceEffect)effect).Contrast;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            //The default value is 0.5.
            return 0.5;
        }

        private void SetDmlLuminanceEffect(int key, double value)
        {
            if (DmlNodeType == DmlNodeType.Picture)
            {
                DmlBlipFill fill = ((DmlPicture)this).BlipFill;
                if (fill != null)
                {
                    DmlBlip blip = fill.Blip;
                    DmlLuminanceEffect effect = null;

                    // Check if this effect is already applied. If yes, then just set new brightness/contrast.
                    for (int i = 0; i < blip.Effects.Count; i++)
                    {
                        DmlEffect dmlEffect = blip.Effects[i];
                        if (dmlEffect.Type == DmlEffectType.Luminance)
                        {
                            effect = (DmlLuminanceEffect)blip.Effects[i];
                            break;
                        }
                    }

                    if (effect == null)
                    {
                        // Create new DmlLuminanceEffect add to effects collection, and then set new brightness/contrast.
                        effect = new DmlLuminanceEffect();
                        blip.Effects.Add(effect);
                    }

                    // Set brightness/contrast.
                    switch (key)
                    {
                        case ShapeAttr.ImageBrightness:
                            effect.Brightness = value;
                            break;
                        case ShapeAttr.ImageContrast:
                            effect.Contrast = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private bool GetDmlImageEffectValue(DmlEffectType type)
        {
            if (DmlNodeType == DmlNodeType.Picture)
            {
                DmlBlip dmlBlip = ((DmlPicture)this).BlipFill.Blip;

                if (dmlBlip.Effects.Count > 0)
                {
                    foreach (DmlEffect effect in dmlBlip.Effects)
                    {
                        if (effect.Type == type)
                            return true;
                    }
                }
            }

            return false;
        }

        private void SetDmlImageEffects(DmlEffectType type, object value)
        {
            // andrnosk: WORDSNET-12025 Apply or remove DmlGrayScaleEffect.
            if (DmlNodeType == DmlNodeType.Picture)
            {
                DmlBlipFill fill = ((DmlPicture)this).BlipFill;
                if (fill != null)
                {
                    IList<DmlEffect> dmlBlipEffects = fill.Blip.Effects;

                    if ((bool)value)
                    {
                        DmlEffect dmlEffect = null;

                        switch (type)
                        {
                            case DmlEffectType.GrayScale:
                                dmlEffect = new DmlGrayScaleEffect();
                                break;
                            case DmlEffectType.BiLevel:
                                {
                                    dmlEffect = new DmlBiLevelEffect();

                                    // In MS Word you can set 25, 50, 75 percent, so i think 50 looks good as default value.
                                    ((DmlBiLevelEffect)dmlEffect).Threshold = 0.5;
                                    break;
                                }
                            default:
                                break;
                        }

                        if (dmlEffect != null)
                            dmlBlipEffects.Add(dmlEffect);
                    }
                    else
                    {
                        // If false, remove this effect.
                        for (int i = 0; i < dmlBlipEffects.Count; i++)
                        {
                            DmlEffect dmlEffect = dmlBlipEffects[i];
                            if (dmlEffect.Type == type)
                            {
                                dmlBlipEffects.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private double GetCrop(int key)
        {
            if (DmlNodeType == DmlNodeType.Picture)
            {
                DmlBlipFill dmlBlipFill = ((DmlPicture)this).BlipFill;
                DmlPercentageOffsetRectangle sRec = dmlBlipFill.SourceRectangle;

                switch (key)
                {
                    case ShapeAttr.ImageCropTop:
                        return ValidateValue(sRec.TopOffset);
                    case ShapeAttr.ImageCropBottom:
                        return ValidateValue(sRec.BottomOffset);
                    case ShapeAttr.ImageCropLeft:
                        return ValidateValue(sRec.LeftOffset);
                    case ShapeAttr.ImageCropRight:
                        return ValidateValue(sRec.RightOffset);
                    default:
                        return 0;
                }
            }

            return 0;
        }

        private void SetCropAttr(int key, double value)
        {
            DmlPercentageOffsetRectangle srOffsetRectangle = GetOffsetRectangle();
            if (srOffsetRectangle == null)
                return;

            switch (key)
            {
                case ShapeAttr.ImageCropTop:
                    srOffsetRectangle.TopOffset = ValidateValue(value);
                    break;
                case ShapeAttr.ImageCropBottom:
                    srOffsetRectangle.BottomOffset = ValidateValue(value);
                    break;
                case ShapeAttr.ImageCropLeft:
                    srOffsetRectangle.LeftOffset = ValidateValue(value);
                    break;
                case ShapeAttr.ImageCropRight:
                    srOffsetRectangle.RightOffset = ValidateValue(value);
                    break;
                default:
                    break;
            }
        }

        private DmlPercentageOffsetRectangle GetOffsetRectangle()
        {
            if (DmlNodeType != DmlNodeType.Picture)
                return null;

            DmlBlipFill fill = ((DmlPicture)this).BlipFill;
            return fill.SourceRectangle;
        }

        private static double ValidateValue(double value)
        {
            // Fix of issue WORDSNET-13552 uses crop instead of clipping canvas,
            // and rounding to 2 digits produces incorrect output. So increases number of digits after dot to 5 for proper output.
            return System.Math.Round(value, 5, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Update EffectExtent upon rotation. Taking existing effect extents in account.
        /// </summary>
        private void UpdateEffectExtent(double value)
        {
            // The code below is not really calculating EffectExtent.
            // It's just a stub to avoid issues for some rotation changing, but a lot of cases are broken.
            double normAngle = MathUtil.NormalizeAngle(value);
            RectangleF sizeBox = mShape.GetsSizeBoxForBounds(normAngle);

            // Bounds should be rotated by an angle equal to the difference between
            // the previous shape's rotation angle and new value.
            float rotate = (float)normAngle - (float)mShape.Rotation;
            RectangleF rotatedBounds = GeometryUtil.GetRotatedRectangleBounds(mShape.BoundsWithEffects, rotate);

            int left = ConvertUtilCore.PointToEmu(System.Math.Max((int)(sizeBox.Left - rotatedBounds.Left), 0));
            int top = ConvertUtilCore.PointToEmu(System.Math.Max((int)(sizeBox.Top - rotatedBounds.Top), 0));
            int right = ConvertUtilCore.PointToEmu(System.Math.Max((int)(rotatedBounds.Right - sizeBox.Right), 0));
            int bottom = ConvertUtilCore.PointToEmu(System.Math.Max((int)(rotatedBounds.Bottom - sizeBox.Bottom), 0));

            mShape.SetShapeAttrInternal(ShapeAttr.DmlEffectExtentLeft, left);
            mShape.SetShapeAttrInternal(ShapeAttr.DmlEffectExtentTop, top);
            mShape.SetShapeAttrInternal(ShapeAttr.DmlEffectExtentRight, right);
            mShape.SetShapeAttrInternal(ShapeAttr.DmlEffectExtentBottom, bottom);
        }

        /// <summary>
        /// Gets the collection of adjustment values that are applied to the shape.
        /// </summary>
        private AdjustmentCollection GetAdjustmentCollection()
        {
            AdjustmentCollection adjustments = new AdjustmentCollection();

            DmlGuides dmlGuides = Dml.DmlShape.Geometry.Guides;
            foreach (DmlGuide guide in dmlGuides.AdjustableValues)
            {
                DmlOneArgumentFormula formula = guide.Formula as DmlOneArgumentFormula;
                ArgumentUtil.CheckNotNull(formula, "formula");

                string name = guide.Name;
                int value = FormatterPal.TryParseInt(formula.X);

                // If there are no custom values available, we will use preset values to work
                // with in order to prevent any updates to the collection during the work.
                // However, if custom values are available, we will use those instead.
                if (!dmlGuides.HasCustomAdjustableValues())
                    adjustments.Add(new Adjustment(name, value, formula, mShape));
                else
                {
                    if (!guide.IsPreset)
                        adjustments.Add(new Adjustment(name, value, formula, mShape));
                }
            }

            return adjustments;
        }

        /// <summary>
        /// Tries set specified shadow type to a specified effect.
        /// </summary>
        private bool TrySetShadowType(DmlShapeEffectType effectType, ShadowType shadowType)
        {
            DmlShapeBase dmlShapeBase = this as DmlShapeBase;
            if (dmlShapeBase == null)
                return false;

            bool hasEffects = (dmlShapeBase.Effects != null);

            DmlShapeEffect effect = null;
            if (hasEffects)
                effect = dmlShapeBase.Effects[effectType];

            bool hasEffect = (effect != null);
            if (!hasEffect)
                effect = DmlShapeEffect.CreateEffect(effectType);

            if (!effect.TrySetShadowType(shadowType))
                return false;

            if (!hasEffects)
                dmlShapeBase.Effects = new DmlShapeEffectsCollection();

            if (!hasEffect)
                dmlShapeBase.Effects.AddEffect(effect);

            return true;
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined.
        /// </summary>
        public StringToObjDictionary<DmlExtension> Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        public StringToObjDictionary<DmlExtension> DocPrExtensions
        {
            get { return mDocPrExtensions; }
            set { mDocPrExtensions = value; }
        }

        public override string Name
        {
            get
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties != null)
                        return NonVisualPr.NvDrawingProperties.Name;

                    return string.Empty;
                }

                return (string)mShape.FetchShapeAttrInternal(ShapeAttr.ShapeName);
            }
            set
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties == null)
                        NonVisualPr.NvDrawingProperties = new DmlNvDrawingProperties();

                    NonVisualPr.NvDrawingProperties.Name = value;
                }

                mShape.SetShapeAttrInternal(ShapeAttr.ShapeName, value);
            }
        }

        public override string AlternativeText
        {
            get
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties != null)
                        return NonVisualPr.NvDrawingProperties.Description;

                    return string.Empty;
                }

                return (string)mShape.FetchShapeAttrInternal(ShapeAttr.ShapeDescription);
            }
            set
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties == null)
                        NonVisualPr.NvDrawingProperties = new DmlNvDrawingProperties();

                    NonVisualPr.NvDrawingProperties.Description = value;
                }

                mShape.SetShapeAttrInternal(ShapeAttr.ShapeDescription, value);
            }
        }

        public override bool Decorative
        {
            get
            {
                if ((string)Dml.FetchShapeAttrInternal(ShapeAttr.ShapeDescription) != string.Empty)
                    return false;

                return (DocPrExtensions != null) &&
                       (DocPrExtensions[DmlExtensionUri.Decorative] != null) &&
                       DocPrExtensions[DmlExtensionUri.Decorative].Decorative;
            }

            set
            {
                if (DocPrExtensions == null)
                    DocPrExtensions = new StringToObjDictionary<DmlExtension>();
                DmlExtension ext = DmlExtension.GetOrCreateExtension(DocPrExtensions, DmlExtensionUri.Decorative);
                ext.Decorative = value;
            }
        }

        public override string Title
        {
            get
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties != null)
                        return NonVisualPr.NvDrawingProperties.Title;

                    return string.Empty;
                }

                return (string)mShape.FetchShapeAttrInternal(ShapeAttr.ShapeTitle);
            }
            set
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties == null)
                        NonVisualPr.NvDrawingProperties = new DmlNvDrawingProperties();

                    NonVisualPr.NvDrawingProperties.Title = value;
                }

                mShape.SetShapeAttrInternal(ShapeAttr.ShapeTitle, value);
            }
        }

        internal override bool Hidden
        {
            get
            {
                // WORDSNET-16976 Take "Hidden" value from non-visual properties for nested shapes.
                if (!mShape.IsTopLevel && (NonVisualPr != null) && (NonVisualPr.NvDrawingProperties != null))
                        return NonVisualPr.NvDrawingProperties.Hidden;

                return (bool)mShape.FetchShapeAttrInternal(ShapeAttr.Hidden);
            }

            set
            {
                if (!mShape.IsTopLevel)
                {
                    if (NonVisualPr.NvDrawingProperties == null)
                        NonVisualPr.NvDrawingProperties = new DmlNvDrawingProperties();

                    NonVisualPr.NvDrawingProperties.Hidden = value;
                }
                // This attributes has to be set for nested and top level shapes, due to currently DML->VML
                // conversion does not take in attention the "Hidden" property of the non-visual properties,
                // it just copy attributes. Probably, after fixing this conversion, it will be possible to avoid
                // set of the attribute below.
                mShape.SetShapeAttrInternal(ShapeAttr.Hidden, value);
            }
        }

        /// <summary>
        /// Vml shape related only.
        /// </summary>
        public override bool IsHorizontalRule
        {
            get { return false; }
        }

        /// <summary>
        /// Vml shape related only.
        /// </summary>
        public override bool IsWordArt
        {
            get { return false; }
        }

        /// <summary>
        /// Vml shape related only.
        /// </summary>
        public override OleFormat OleFormat
        {
            get { return null; }
        }

        /// <summary>
        /// Switches the orientation of a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.FlipOrientation.None"/>.</p>
        /// </remarks>
        public override FlipOrientation FlipOrientation
        {
            get { return Transform.FlipOrientation; }
            set { Transform.FlipOrientation = value; }
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
            get { return Transform.Rotation.ValueInDegrees; }
            set
            {
                // Previous rotation angle should be taken into account.
                UpdateEffectExtent(value);

                Transform.Rotation = DmlAngle.FromDegrees(value);
            }
        }

        /// <summary>
        /// Represents the avLst element that specifies a list of shape adjust values ([ISO/IEC29500-1:2012] section 20.1.9.5).
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
        /// Vml shape related only.
        /// </summary>
        public override TextPath TextPath
        {
            get
            {
                // Dml shape cannot be WordArt, but to avoid exception we return
                // the new TextPath (the same we do for example for all other VML shapes).
                return new TextPath(mShape);
            }
        }

        /// <summary>
        /// Returns true if an extrusion effect is enabled.
        /// </summary>
        public override bool ExtrusionEnabled
        {
            get
            {
                IDmlShapePrSource source = this as IDmlShapePrSource;
                if (source != null)
                {
                    DmlShape3DProperties shape3DProperties = source.Shape3DProperties;
                    DmlScene3DProperties scene3DProperties = source.Scene3DProperties;

                    return ((shape3DProperties != null) || (scene3DProperties != null));
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true if a shadow effect is enabled.
        /// </summary>
        public override bool ShadowEnabled
        {
            get
            {
                DmlFillableNode fillableNode = this as DmlFillableNode;
                if ((fillableNode != null) && (fillableNode.Effects != null))
                {
                    foreach (DmlShapeEffect dmlShapeEffect in fillableNode.Effects)
                    {
                        if ((dmlShapeEffect.EffectType == DmlShapeEffectType.InnerShadow) ||
                            (dmlShapeEffect.EffectType == DmlShapeEffectType.OuterShadow) ||
                            (dmlShapeEffect.EffectType == DmlShapeEffectType.PresetShadow))
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the shape type.
        /// </summary>
        public override ShapeType ShapeType
        {
            get
            {
                ShapeType shapeType = ShapeType.NonPrimitive;

                if ((DmlNodeType == DmlNodeType.GroupShape) ||
                    (DmlNodeType == DmlNodeType.WordprocessingGroupShape) ||
                    (DmlNodeType == DmlNodeType.WordprocessingCanvas) ||
                    (DmlNodeType == DmlNodeType.SpTree) ||
                    (DmlNodeType == DmlNodeType.LockedCanvas) ||
                    (DmlNodeType == DmlNodeType.GraphicFrame))
                    return ShapeType.Group;

                if (DmlNodeType == DmlNodeType.Picture)
                    return ShapeType.Image;

                if ((DmlNodeType == DmlNodeType.Shape) ||
                    (DmlNodeType == DmlNodeType.WordprocessingShape) ||
                    (DmlNodeType == DmlNodeType.ConnectorShape))
                {
                    shapeType = DmlEnum.DmlPresetGeomToShapeType(((DmlShape)this).Geometry.PresetName);

                    if (mShape.HasTextbox && (shapeType == ShapeType.Rectangle))
                    {
                        DmlCnvPrShape nvPr = (mNonVisualPr != null) && (mNonVisualPr.CNvProperties != null) ?
                            NonVisualPr.CNvProperties as DmlCnvPrShape : null;

                        // WORDSNET-17530 Take in account “txBox” attribute of the "cNvSpPr", while determining DML textbox.
                        shapeType = (nvPr != null) && nvPr.TextBox ? ShapeType.TextBox : shapeType;
                    }

                }

                return shapeType;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppRenameEntity("NodeTransform")] // Rename to avoid C++ compilation error.
        public DmlTransform Transform
        {
            get
            {
                if (mTransform == null)
                    InitializeTransform();

                return mTransform;
            }
            set
            {
                mTransform = value;
                SetDmlTransform(mTransform);
            }
        }

        #region ImageData properties.
        /// <summary>
        /// Gets or sets the brightness of the Dml picture.
        /// The value for this property must be a number from 0.0 (dimmest) to 1.0 (brightest).
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.5.</p>
        /// </remarks>
        public override double Brightness
        {
            get { return GetDmlLuminanceEffectValue(ShapeAttr.ImageBrightness); }
            set { SetDmlLuminanceEffect(ShapeAttr.ImageBrightness, value); }
        }

        /// <summary>
        /// Gets or sets the contrast for the specified Dml picture. The value
        /// for this property must be a number from 0.0 (the least contrast) to 1.0 (the greatest contrast).
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.5.</p>
        /// </remarks>
        public override double Contrast
        {
            get { return GetDmlLuminanceEffectValue(ShapeAttr.ImageContrast); }
            set { SetDmlLuminanceEffect(ShapeAttr.ImageContrast, value); }
        }

        /// <summary>
        /// Determines whether a Dml picture image will be displayed in black and white.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public override bool BiLevel
        {
            get { return GetDmlImageEffectValue(DmlEffectType.BiLevel); }
            set { SetDmlImageEffects(DmlEffectType.BiLevel, value); }
        }

        /// <summary>
        /// Determines whether a Dml picture will display in grayscale mode.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public override bool GrayScale
        {
            get { return GetDmlImageEffectValue(DmlEffectType.GrayScale); }
            set { SetDmlImageEffects(DmlEffectType.GrayScale, value); }
        }

        /// <summary>
        /// Defines the fraction of Dml picture removal from the top side.
        /// </summary>
        public override double CropTop
        {
            get { return GetCrop(ShapeAttr.ImageCropTop); }
            set { SetCropAttr(ShapeAttr.ImageCropTop, value); }
        }

        /// <summary>
        /// Defines the fraction of Dml picture removal from the bottom side.
        /// </summary>
        public override double CropBottom
        {
            get { return GetCrop(ShapeAttr.ImageCropBottom); }
            set { SetCropAttr(ShapeAttr.ImageCropBottom, value); }
        }

        /// <summary>
        /// Defines the fraction of Dml picture removal from the left side.
        /// </summary>
        public override double CropLeft
        {
            get { return GetCrop(ShapeAttr.ImageCropLeft); }
            set { SetCropAttr(ShapeAttr.ImageCropLeft, value); }
        }

        /// <summary>
        /// Defines the fraction of Dml picture removal from the right side.
        /// </summary>
        public override double CropRight
        {
            get { return GetCrop(ShapeAttr.ImageCropRight); }
            set { SetCropAttr(ShapeAttr.ImageCropRight, value); }
        }
        #endregion

        #region TextBox properties.
        /// <summary>
        /// Specifies the inner left margin in points for a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/10 inch.</p>
        /// </remarks>
        public override double InternalMarginLeft
        {
            get { return GetInternalMargin(ShapeAttr.TextboxLeft); }
            set { SetInternalMargin(ShapeAttr.TextboxLeft, value); }
        }

        /// <summary>
        /// Specifies the inner right margin in points for a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/10 inch.</p>
        /// </remarks>
        public override double InternalMarginRight
        {
            get { return GetInternalMargin(ShapeAttr.TextboxRight); }
            set { SetInternalMargin(ShapeAttr.TextboxRight, value); }
        }

        /// <summary>
        /// Specifies the inner top margin in points for a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/20 inch.</p>
        /// </remarks>
        public override double InternalMarginTop
        {
            get { return GetInternalMargin(ShapeAttr.TextboxTop); }
            set { SetInternalMargin(ShapeAttr.TextboxTop, value); }
        }

        /// <summary>
        /// Specifies the inner bottom margin in points for a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/20 inch.</p>
        /// </remarks>
        public override double InternalMarginBottom
        {
            get { return GetInternalMargin(ShapeAttr.TextboxBottom); }
            set { SetInternalMargin(ShapeAttr.TextboxBottom, value); }
        }

        /// <summary>
        /// Determines whether Microsoft Word will grow the TextShape to fit text.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public override bool FitShapeToText
        {
            get
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShapeAutoFitMode dmlShapeAutoFitMode = ((DmlShape)this).TextBodyPr.AutoFitMode as DmlShapeAutoFitMode;
                    if (dmlShapeAutoFitMode != null)
                        return true;

                }

                return false;
            }
            set
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShape dmlShape = (DmlShape)this;

                    if (dmlShape.TextShape == null)
                        dmlShape.TextShape = new DmlTextShape();

                    DmlTextBodyProperties textBodyPr = dmlShape.TextBodyPr;
                    if (value)
                        textBodyPr.AutoFitMode = new DmlShapeAutoFitMode();
                    else
                        textBodyPr.AutoFitMode = new DmlNoAutoFitMode();
                }

            }
        }

        /// <summary>
        /// Determines the flow of the text layout in a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.LayoutFlow.Horizontal"/>.</p>
        /// </remarks>
        public override LayoutFlow LayoutFlow
        {
            get
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                    return DmlUtilCore.DmlToTextVerticalType(((DmlShape)this).TextBodyPr.TextOrientation,
                        ((DmlShape)this).NormalEastAsianFlow);

                return LayoutFlow.Horizontal;
            }
            set
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShape dmlShape = (DmlShape)this;

                    if (dmlShape.TextShape == null)
                        dmlShape.TextShape = new DmlTextShape();

                    dmlShape.TextBodyPr.TextOrientation = DmlUtilCore.TextVerticalTypeToDml(value);
                }
            }
        }

        /// <summary>
        /// Determines how text wraps inside a TextShape of Shape or WordprocessingShape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Words.Drawing.TextBoxWrapMode.Square"/>.</p>
        /// </remarks>
        public override TextBoxWrapMode TextBoxWrapMode
        {
            get
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                    return ((DmlShape)this).TextBodyPr.TextWrappingType;

                return TextBoxWrapMode.None;
            }
            set
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShape dmlShape = (DmlShape)this;

                    if (dmlShape.TextShape == null)
                        dmlShape.TextShape = new DmlTextShape();

                    dmlShape.TextBodyPr.TextWrappingType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either text of the TextBox should not rotate when the shape is rotated.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c></p>
        /// </remarks>
        internal override bool TextBoxNoTextRotation
        {
            get
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                    return ((DmlShape)this).TextBodyPr.IsTextUpright;

                return false;
            }
            set
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShape dmlShape = (DmlShape)this;

                    if (dmlShape.TextShape == null)
                        dmlShape.TextShape = new DmlTextShape();

                    dmlShape.TextBodyPr.IsTextUpright = value;
                }
            }
        }

        /// <summary>
        /// Defines the vertical anchoring of text in a TextShape. Word 2007 only.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Words.Drawing.TextBoxAnchor.Top"/>.</p>
        /// </remarks>
        internal override TextBoxAnchor TextBoxAnchor
        {
            get
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                    return DmlUtilCore.DmlToTextboxAnchor(((DmlShape)this).TextBodyPr.Anchor);

                return TextBoxAnchor.Top;
            }
            set
            {
                if ((DmlNodeType == DmlNodeType.WordprocessingShape) || (DmlNodeType == DmlNodeType.Shape))
                {
                    DmlShape dmlShape = (DmlShape)this;

                    if (dmlShape.TextShape == null)
                        dmlShape.TextShape = new DmlTextShape();

                    dmlShape.TextBodyPr.Anchor = DmlUtilCore.TextboxAnchorToDml(value);
                }
            }
        }
        #endregion

        internal override int CoordSizeWidth
        {
            get
            {
                DmlGroupTransform groupTransform = Transform as DmlGroupTransform;
                if (groupTransform != null)
                {
                    // andrnosk: WORDSNET-12565 WordprocessingCanvas does not have CoordSize,
                    // CoordSize is the same as original shape size.
                    if (DmlNodeType == DmlNodeType.WordprocessingCanvas)
                        return ConvertUtilCore.PointToEmu(mShape.Width);

                    return !MathUtil.IsZero(groupTransform.ChildWidth)
                        ? (int)groupTransform.ChildWidth
                        : (int)groupTransform.Width;
                }

                return Width;
            }
        }

        internal override int CoordSizeHeight
        {
            get
            {
                DmlGroupTransform groupTransform = Transform as DmlGroupTransform;
                if (groupTransform != null)
                {
                    if (DmlNodeType == DmlNodeType.WordprocessingCanvas)
                        return ConvertUtilCore.PointToEmu(mShape.Height);

                    return !MathUtil.IsZero(groupTransform.ChildHeight)
                        ? (int)groupTransform.ChildHeight
                        : (int)groupTransform.Height;
                }

                return Height;
            }
        }

        /// <summary>
        /// Returns the width of the extents rectangle in EMUs.
        /// </summary>
        internal int Width
        {
            get
            {
                DmlTransform dmlTransform = Transform;
                return (int)dmlTransform.Width;
            }
        }

        /// <summary>
        /// Returns the Height of the extents rectangle in EMUs.
        /// </summary>
        internal int Height
        {
            get
            {
                DmlTransform dmlTransform = Transform;
                return (int)dmlTransform.Height;
            }
        }

        internal override int CoordOriginX
        {
            get
            {
                if (Transform is DmlGroupTransform)
                    return (int)((DmlGroupTransform)Transform).ChildX;

                return (int)Transform.X;
            }
            set
            {
                if (Transform is DmlGroupTransform)
                    ((DmlGroupTransform)Transform).ChildX = value;
                else
                    Transform.X = value;
            }
        }

        internal override int CoordOriginY
        {
            get
            {
                if (Transform is DmlGroupTransform)
                    return (int)((DmlGroupTransform)Transform).ChildY;

                return (int)Transform.Y;
            }
            set
            {
                if (Transform is DmlGroupTransform)
                    ((DmlGroupTransform)Transform).ChildY = value;
                else
                    Transform.Y = value;
            }
        }

        internal override bool AspectRatioLocked
        {
            get
            {
                if (mShape.IsTopLevel)
                {
                    // For top level shapes we have to use LockAspectRatio read from cNvGraphicFramePr->graphicFrameLocks
                    // and stored inside ShapePr.
                    if (mShape.ShapePr.Contains(ShapeAttr.LockAspectRatio))
                        return (bool)mShape.ShapePr.GetDirectAttr(ShapeAttr.LockAspectRatio);
                }
                else
                {
                    // For child shapes we have to use LockAspectRatio read from cNvSpPr->spLocks
                    // and stored inside CNvProperties.Locks.
                    object dmlLock = mShape.DmlNode.NonVisualPr.CNvProperties.Locks.GetLock(DmlLock.NoChangeAspect);
                    if (dmlLock != null)
                        return (bool)dmlLock;
                }

                return false;
            }
            set
            {
                if (mShape.IsTopLevel)
                    mShape.ShapePr.SetAttr(ShapeAttr.LockAspectRatio, value);

                mShape.DmlNode.NonVisualPr.CNvProperties.Locks.AddLock(DmlLock.NoChangeAspect, value);
            }
        }

        /// <summary>
        /// Vml shape related only.
        /// </summary>
        internal override HorizontalRule HorizontalRule
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> object for a shape.
        /// </summary>
        internal override ShadowType ShadowType
        {
            get
            {
                DmlShapeEffectsCollection effects = (this is DmlShapeBase) ? ((DmlShapeBase)this).Effects : null;
                if (effects == null)
                    return ShadowType.ShadowMixed;

                foreach (DmlShapeEffect effect in effects)
                {
                    if (effect.IsShadowEffect)
                        return effect.ShadowType;
                }

                return ShadowType.ShadowMixed;
            }
            set
            {
                if (TrySetShadowType(DmlShapeEffectType.PresetShadow, value))
                    return;

                if (TrySetShadowType(DmlShapeEffectType.InnerShadow, value))
                    return;

                TrySetShadowType(DmlShapeEffectType.OuterShadow, value);
            }
        }

        /// <summary>
        /// Gets or sets color of a shadow effect.
        /// </summary>
        internal override Color ShadowColor
        {
            get
            {
                DmlShapeEffect shadowShapeEffect = GetShadowShapeEffect(false);
                if ((shadowShapeEffect == null) || (shadowShapeEffect.Color == null))
                    return Color.Black;

                return shadowShapeEffect.Color.CreateDrColor(mShape.DocumentTheme, null).ToNativeColor();               
            }
            set
            {
                DmlShapeEffect shadowShapeEffect = GetShadowShapeEffect(true);
                shadowShapeEffect.Color = DmlHexRgbColor.FromDrColor(DrColor.FromNativeColor(value));
                if (value.IsEmpty)
                    shadowShapeEffect.Color.UpdateAlpha(0.0);
            }
        }

        /// <summary>
        /// Gets or sets transparency of a shadow effect.
        /// </summary>
        internal override double ShadowTransparency
        {
            get
            {
                DmlShapeEffect shadowShapeEffect = GetShadowShapeEffect(false);
                if ((shadowShapeEffect == null) || (shadowShapeEffect.Color == null))
                    return 0.0;

                return 1.0 - shadowShapeEffect.Color.Alpha.Value;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Shadow.Transparency");

                DmlShapeEffect shadowShapeEffect = GetShadowShapeEffect(true);
                shadowShapeEffect.Color.UpdateAlpha(1.0 - value);
            }
        }

        internal Node Parent
        {
            get { return mShape.ParentNode; }
        }

        /// <summary>
        /// Specifies whether the text flow of the text contents of the shape ignores the text flow value specified by the
        /// "vert" attribute of the "bodyPr" element.
        /// </summary>
        internal bool NormalEastAsianFlow
        {
            get { return mNormalEastAsianFlow; }
            set { mNormalEastAsianFlow = value; }
        }

        /// <summary>
        /// Model Id is used in diagram drawing.
        /// </summary>
        internal DmlModelId ModelId
        {
            get { return mModelId; }
            set { mModelId = value; }
        }

        internal DmlNvPrBase NonVisualPr
        {
            get { return mNonVisualPr; }
            set { mNonVisualPr = value; }
        }

        internal ShapeBase Dml
        {
            get { return mShape; }
        }

        /// <summary>
        /// Returns shadow shape effect for current DmlShape.
        /// </summary>
        /// <remarks>
        /// If DmlShape does not have a shadow shape effect and isAllowAutoCreate value is True,
        /// then creation of a new shadow shape effect corresponding to ShadowType.Shadow1 template
        /// is initiated. This is MS Word behavior.
        /// </remarks>
        private DmlShapeEffect GetShadowShapeEffect(bool isAllowAutoCreate)
        {
            DmlShapeBase dmlShapeBase = this as DmlShapeBase;

            if (isAllowAutoCreate)
            {
                ArgumentUtil.CheckNotNull(dmlShapeBase, "Shadow.ShapeEffect");
                ArgumentUtil.CheckNotNull(dmlShapeBase.Effects, "Shadow.ShapeEffect");
            }

            if ((dmlShapeBase != null) && (dmlShapeBase.Effects != null))
            {
                foreach (DmlShapeEffect effect in dmlShapeBase.Effects)
                    if (effect.IsShadowEffect)
                        return effect;
            }

            if (isAllowAutoCreate)
            {
                TrySetShadowType(DmlShapeEffectType.PresetShadow, ShadowType.Shadow1);
                return GetShadowShapeEffect(false);
            }

            return null;
        }

        private DmlModelId mModelId;
        private DmlNvPrBase mNonVisualPr;
        private bool mNormalEastAsianFlow;
        protected DmlTransform mTransform;
        private StringToObjDictionary<DmlExtension> mExtensions;
        private StringToObjDictionary<DmlExtension> mDocPrExtensions;
        private AdjustmentCollection mAdjustments;
    }
}
