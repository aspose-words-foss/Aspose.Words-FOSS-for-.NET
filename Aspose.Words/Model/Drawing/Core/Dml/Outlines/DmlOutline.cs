// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using System;
using System.Drawing.Drawing2D;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// Represents 20.1.2.2.24 ln (Outline)
    /// This element specifies an outline style that can be applied to
    /// a number of different objects such as shapes and text.
    /// The line allows for the specifying of many different
    /// types of outlines including even line dashes and bevels.
    /// </summary>
    internal class DmlOutline : DmlExtensionListSource, IDmlColorable, IStroke
    {
        internal DmlOutline()
        {
            mPropertyBag.ParentBagProvider = gDefaultParentBagProvider;
        }

        public virtual DmlOutline Clone()
        {
            DmlOutline lhs = new DmlOutline();

            if (mPropertyBag != null)
                lhs.mPropertyBag = mPropertyBag.Clone();

            if (IsPropertySpecified(DmlOutlinePropertiesIds.Fill))
                lhs.Fill = Fill.Clone();

            if (IsPropertySpecified(DmlOutlinePropertiesIds.Dash))
                lhs.Dash = Dash.Clone();

            if (IsPropertySpecified(DmlOutlinePropertiesIds.HeadLineEndStyle))
                lhs.HeadLineEndStyle = (DmlHeadLineEndStyle)HeadLineEndStyle.Clone();

            if (IsPropertySpecified(DmlOutlinePropertiesIds.TailLineEndStyle))
                lhs.TailLineEndStyle = (DmlTailLineEndStyle)TailLineEndStyle.Clone();

            if (IsPropertySpecified(DmlOutlinePropertiesIds.Extensions))
                lhs.Extensions = CloneExtensions();

            return lhs;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlOutline value = (DmlOutline)obj;

            bool equals = true;

            DmlOutlinePropertiesIds[] values = (DmlOutlinePropertiesIds[])Enum.GetValues(typeof(DmlOutlinePropertiesIds));
            foreach (DmlOutlinePropertiesIds id in values)
            {
                object val1 = GetDirectProperty(id);
                object val2 = value.GetDirectProperty(id);
                equals &= object.Equals(val1, val2);
                if(!equals)
                    break;
            }

            return equals;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            DmlOutlinePropertiesIds[] values = (DmlOutlinePropertiesIds[])Enum.GetValues(typeof(DmlOutlinePropertiesIds));
            foreach (DmlOutlinePropertiesIds id in values)
            {
                object value = GetDirectProperty(id);
                if (value == null)
                    continue;

                hash ^= value.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Set style color in color placeholders used
        /// in the outline.
        /// </summary>
        public void ApplyStyleColor(DmlColor styleColor)
        {
            if (Fill != null)
                Fill.ApplyStyleColor(styleColor);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        internal object GetDirectProperty(DmlOutlinePropertiesIds propertyId)
        {
            return mPropertyBag.GetDirectProperty((int)propertyId);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="propertyId">the property id</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlOutlinePropertiesIds propertyId)
        {
            return mPropertyBag.IsPropertySpecified((int)propertyId);
        }

        /// <summary>
        /// Sets parent properties collection for current outline.
        /// </summary>
        /// <remarks>
        /// Internal access level for test purposes.
        /// </remarks>
        internal void SetParentProperties(DmlOutline dmlParentOutline)
        {
            // WORDSNET-25330 Release parent in the Fill before it is replaced with a new Fill
            // in this parent properties bag to prevent possible memory leak.
            IDmlHierarchicalPropertyBag parentBag = mPropertyBag.ParentBagProvider.ParentBag;
            if (parentBag != null)
            {
                DmlFill fill = parentBag.GetProperty((int)DmlOutlinePropertiesIds.Fill) as DmlFill;
                if (fill != null)
                    fill.Parent = null;
            }

            mPropertyBag.ParentBagProvider = new DmlHierarchicalPropertyBagParentContainer(dmlParentOutline.mPropertyBag);
        }

        private void SetProperty(DmlOutlinePropertiesIds propertyId, object value)
        {
            mPropertyBag.SetProperty((int)propertyId, value);
        }

        private object GetProperty(DmlOutlinePropertiesIds propertyId)
        {
            return mPropertyBag.GetProperty((int)propertyId);
        }

        /// <summary>
        /// Removes the specified property.
        /// </summary>
        internal void Remove(DmlOutlinePropertiesIds propertyId)
        {
            mPropertyBag.Remove((int)propertyId);
        }

        /// <summary>
        /// Creates and sets new DmlFill using style fill if direct fill is not specified.
        /// </summary>
        private void CreateDirectFill()
        {
            object fillObj = GetDirectProperty(DmlOutlinePropertiesIds.Fill);

            if (fillObj == null)
            {
                // If direct fill is not specified we have to create new DmlFill using style fill type.
                DmlFill newFill;

                switch (Fill.DmlFillType)
                {
                    case DmlFillType.SolidFill:
                        newFill = new DmlSolidFill();
                        break;
                    case DmlFillType.PatternFill:
                        newFill = new DmlPatternFill();
                        break;
                    case DmlFillType.BlipFill:
                        newFill = new DmlBlipFill();
                        break;
                    case DmlFillType.GradientFill:
                        newFill = new DmlGradientFill();
                        break;
                    default:
                        newFill = new DmlSolidFill();
                        break;
                }

                UpdateEffectExtent(0, ConvertUtilCore.EmuToPoint(WidthInEmus));

                // Set Shape for this new fill, we need this to get color from
                // Document Themes and to reset fill for current shape.
                newFill.Parent = Fill.Parent;
                Fill = newFill;
            }
        }

        #region IStroke interface implementation.

        public bool On
        {
            get
            {
                // Return false only when the FillType equals DmlFillType.NoFill, in all other cases return true even
                // if the fill is not specified (in this case the style fill will be used).
                if ((Fill != null) && (Fill.DmlFillType == DmlFillType.NoFill))
                    return false;

                return true;
            }
            set
            {
                IFillable parent = (Fill != null) ? Fill.Parent : null;

                if (value)
                {
                    if ((Fill != null) && (Fill.DmlFillType == DmlFillType.NoFill))
                    {
                        mPropertyBag.Remove((int)DmlOutlinePropertiesIds.Fill);
                        if (Fill != null)
                            Fill.Parent = parent;
                    }
                }
                else
                {
                    // Reduce effect extent according to outline weight.
                    if ((Fill != null) && (Fill.DmlFillType != DmlFillType.NoFill))
                        UpdateEffectExtent(ConvertUtilCore.EmuToPoint(WidthInEmus), 0);

                    Fill = new DmlNoFill();
                    Fill.Parent = parent;
                }
            }
        }

        public double Weight
        {
            get { return ConvertUtilCore.EmuToPoint(WidthInEmus); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                // WORDSNET-14095 Add required effect extent to enable display outline of the image.
                if (On)
                    UpdateEffectExtent(ConvertUtilCore.EmuToPoint(WidthInEmus), value);

                WidthInEmus = ConvertUtilCore.PointToEmu(value);
            }
        }

        /// <summary>
        /// Updates effect extents according to changes of the outline weight.
        /// </summary>
        internal void UpdateEffectExtent(double curWeight, double newWeight)
        {
            if (Fill == null)
                return;

            Shape parentShape = Fill.Parent as Shape;
            if (parentShape == null)
                return;

            DmlNoFill defaultOutlineFill =
                (DmlNoFill)DmlOutlinePropertiesDefaults.Instance.GetDirectProperty((int)DmlOutlinePropertiesIds.Fill);

            bool isOutlineFillFromShapeStyle = (GetDirectProperty(DmlOutlinePropertiesIds.Fill) == null) &&
                !ReferenceEquals(Fill, defaultOutlineFill);

            // There are not examples of pictures with shape styles to implement calculation of effect extents.
            // So, this condition was added to mark this limitation.
            if (!isOutlineFillFromShapeStyle || !parentShape.IsImage)
            {
                DmlOutlineEffectExtentCalculator.UpdateWeightRelatedEffectExtentPart(parentShape, curWeight, newWeight);
            }
            else
            {
                Debug.Fail("Unsupported scenario");
            }
        }

        /// <summary>
        /// Gets or sets a color.
        /// </summary>
        public DrColor ColorInternal
        {
            get { return Fill.ColorInternal; }
            set
            {
                CreateDirectFill();
                Fill.ColorInternal = value;
            }
        }

        /// <summary>
        /// Gets a base color without modifiers.
        /// </summary>
        public DrColor ColorInternalUnmodified
        {
            get { return Fill.ColorInternalUnmodified; }
        }

        public DrColor Color2Internal
        {
            get { return Fill.Color2Internal; }
            set
            {
                CreateDirectFill();

                // Color2 works only for PatternFill and GradientFill types.
                if ((Fill.DmlFillType == DmlFillType.PatternFill) || (Fill.DmlFillType == DmlFillType.GradientFill))
                    Fill.Color2Internal = value;
            }
        }

        public DashStyle DashStyle
        {
            get
            {
                // In case of DmlDashType.CustomDash return DashStyle.Default.
                if (Dash.DashType == DmlDashType.PresetDash)
                    return ((DmlPresetDash)Dash).Preset;

                return DashStyle.Default;
            }
            set
            {
                if (IsPropertySpecified(DmlOutlinePropertiesIds.Dash) && (Dash.DashType == DmlDashType.PresetDash))
                    ((DmlPresetDash)Dash).Preset = value;

                // Create and set new DmlPresetDash with specified Preset value.
                DmlPresetDash dmlPreset = new DmlPresetDash();
                dmlPreset.Preset = value;
                Dash = dmlPreset;
            }
        }

        public JoinStyle JoinStyle
        {
            get { return LineJoinStyle; }
            set { LineJoinStyle = value; }
        }

        public EndCap EndCap
        {
            get { return LineEndingCapType; }
            set { LineEndingCapType = value; }
        }

        public ShapeLineStyle LineStyle
        {
            get { return CompoundLineType; }
            set { CompoundLineType = value; }
        }

        public ArrowType StartArrowType
        {
            get { return HeadLineEndStyle.Type; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.HeadLineEndStyle);
                HeadLineEndStyle.Type = value;
            }
        }

        public ArrowWidth StartArrowWidth
        {
            get { return HeadLineEndStyle.Width; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.HeadLineEndStyle);
                HeadLineEndStyle.Width = value;
            }
        }

        public ArrowLength StartArrowLength
        {
            get { return HeadLineEndStyle.Length; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.HeadLineEndStyle);
                HeadLineEndStyle.Length = value;
            }
        }

        public ArrowType EndArrowType
        {
            get { return TailLineEndStyle.Type; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.TailLineEndStyle);
                TailLineEndStyle.Type = value;
            }
        }

        public ArrowWidth EndArrowWidth
        {
            get { return TailLineEndStyle.Width; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.TailLineEndStyle);
                TailLineEndStyle.Width = value;
            }
        }

        public ArrowLength EndArrowLength
        {
            get { return TailLineEndStyle.Length; }
            set
            {
                CloneIfRequired(DmlOutlinePropertiesIds.TailLineEndStyle);
                TailLineEndStyle.Length = value;
            }
        }

        /// <summary>
        /// If direct DmlHeadLineEndStyle/DmlTailLineEndStyle is not set it is need to clone these objects
        /// from defaults before set internal attributes.
        /// </summary>
        private void CloneIfRequired(DmlOutlinePropertiesIds propertiesId)
        {
            if (!IsPropertySpecified(propertiesId) )
            {
                switch (propertiesId)
                {
                    case DmlOutlinePropertiesIds.HeadLineEndStyle:
                        HeadLineEndStyle = (DmlHeadLineEndStyle)HeadLineEndStyle.Clone();
                        break;
                    case DmlOutlinePropertiesIds.TailLineEndStyle:
                        TailLineEndStyle = (DmlTailLineEndStyle)TailLineEndStyle.Clone();
                        break;
                    default:
                        break;
                }
            }
        }

        public double Opacity
        {
            get { return Fill.Opacity; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");

                CreateDirectFill();

                // If color is empty we have to get and set color from style, and then apply opacity to this color.
                if (Fill.ColorInternal == DrColor.Empty)
                {
                    Shape shape = Fill.Parent as Shape;
                    if (shape != null)
                    {
                        DmlShapeBase shapeBase = (DmlShapeBase)shape.DmlNode;
                        if (shapeBase.Style != null)
                            Fill.DmlColorInternal = shapeBase.Style.LineReference.Color;
                    }
                }

                Fill.Opacity = value;
            }
        }

        public byte[] ImageBytes
        {
            get { return Fill.ImageBytes; }
        }

        public LineFillType LineFillType
        {
            get
            {
                switch (Fill.DmlFillType)
                {
                    case DmlFillType.SolidFill:
                        return LineFillType.Solid;
                    case DmlFillType.PatternFill:
                        return LineFillType.Pattern;
                    case DmlFillType.BlipFill:
                        {
                            if (((DmlBlipFill)Fill).BlipFillMode is DmlBlipFillTile)
                                return LineFillType.Texture;

                            return LineFillType.Picture;
                        }
                    default:
                        return LineFillType.Solid;
                }
            }
            set
            {
                DmlFill newFill;
                switch (value)
                {
                    case LineFillType.Pattern:
                        newFill = new DmlPatternFill();
                        break;
                    case LineFillType.Picture:
                        {
                            newFill = new DmlBlipFill();
                            ((DmlBlipFill)newFill).BlipFillMode = new DmlBlipFillStretch();
                            break;
                        }
                    case LineFillType.Solid:
                        newFill = new DmlSolidFill();
                        break;
                    case LineFillType.Texture:
                        {
                            newFill = new DmlBlipFill();
                            ((DmlBlipFill)newFill).BlipFillMode = new DmlBlipFillTile();
                            break;
                        }
                    default:
                        newFill = new DmlSolidFill();
                        break;
                }

                Fill = newFill;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        DmlFill IStroke.StrokeFill
        {
            get { return Fill; }
            set { Fill = value; }
        }

        #endregion

        public override StringToObjDictionary<DmlExtension> Extensions
        {
            get { return (StringToObjDictionary<DmlExtension>)GetProperty(DmlOutlinePropertiesIds.Extensions); }
            set { SetProperty(DmlOutlinePropertiesIds.Extensions, value); }
        }

        /// <summary>
        /// Specifies the line fill properties to be used for the underline stroke.
        /// </summary>
        internal DmlFill Fill
        {
            get { return (DmlFill)GetProperty(DmlOutlinePropertiesIds.Fill); }
            set { SetProperty(DmlOutlinePropertiesIds.Fill, value); }
        }

        /// <summary>
        /// Specifies the width in EMUs to be used for the underline stroke.
        /// </summary>
        internal double WidthInEmus
        {
            get { return (double)GetProperty(DmlOutlinePropertiesIds.WidthInEmus); }
            set { SetProperty(DmlOutlinePropertiesIds.WidthInEmus, value); }
        }

        /// <summary>
        /// Specifies that a line join shall be mitered.
        /// </summary>
        internal double LineMiterLimit
        {
            get { return (double)GetProperty(DmlOutlinePropertiesIds.LineMiterLimit); }
            set { SetProperty(DmlOutlinePropertiesIds.LineMiterLimit, value); }
        }

        /// <summary>
        /// Specifies the compound line type to be used for the underline stroke.
        /// The default value is SingleLine.
        /// </summary>
        internal ShapeLineStyle CompoundLineType
        {
            get { return (ShapeLineStyle)GetProperty(DmlOutlinePropertiesIds.CompoundLineType); }
            set { SetProperty(DmlOutlinePropertiesIds.CompoundLineType, value); }
        }

        /// <summary>
        /// Specifies the ending caps that should be used for this line.
        /// The default value is Square.
        /// </summary>
        internal EndCap LineEndingCapType
        {
            get { return (EndCap)GetProperty(DmlOutlinePropertiesIds.LineEndingCapType); }
            set { SetProperty(DmlOutlinePropertiesIds.LineEndingCapType, value); }
        }

        /// <summary>
        /// Specifies the alignment to be used for the underline stroke.
        /// </summary>
        internal bool StrokeAlignment
        {
            get { return (bool)GetProperty(DmlOutlinePropertiesIds.StrokeAlignment); }
            set { SetProperty(DmlOutlinePropertiesIds.StrokeAlignment, value); }
        }

        /// <summary>
        /// Specifies the dash properties to be used for the underline stroke.
        /// </summary>
        internal DmlDash Dash
        {
            get { return (DmlDash)GetProperty(DmlOutlinePropertiesIds.Dash); }
            set { SetProperty(DmlOutlinePropertiesIds.Dash, value); }
        }

        internal JoinStyle LineJoinStyle
        {
            get { return (JoinStyle)GetProperty(DmlOutlinePropertiesIds.LineJoinStyle); }
            set { SetProperty(DmlOutlinePropertiesIds.LineJoinStyle, value); }
        }

        internal DmlHeadLineEndStyle HeadLineEndStyle
        {
            get { return (DmlHeadLineEndStyle)GetProperty(DmlOutlinePropertiesIds.HeadLineEndStyle); }
            set { SetProperty(DmlOutlinePropertiesIds.HeadLineEndStyle, value); }
        }

        internal DmlTailLineEndStyle TailLineEndStyle
        {
            get { return (DmlTailLineEndStyle)GetProperty(DmlOutlinePropertiesIds.TailLineEndStyle); }
            set { SetProperty(DmlOutlinePropertiesIds.TailLineEndStyle, value); }
        }

        internal int DirectPropertiesCount
        {
            get { return mPropertyBag.Count; }
        }

        internal bool IsWeightSpecified
        {
            get { return IsPropertySpecified(DmlOutlinePropertiesIds.WidthInEmus); }
        }

        internal bool IsLineJoinStyleSpecified
        {
            get { return IsPropertySpecified(DmlOutlinePropertiesIds.LineJoinStyle); }
        }

        /// <summary>
        /// Resolves theme colors to a concrete RGB colors using a specified theme provider.
        /// </summary>
        internal void ResolveThemeColors(IThemeProvider themeProvider, DmlShapeStyle dmlShapeStyle)
        {
            // So far, we only resolve theme color when Outline fill is missing.
            if (IsPropertySpecified(DmlOutlinePropertiesIds.Fill))
                return;

            // In this case, Word takes color from a corresponding DmlShapeStyle.
            if (dmlShapeStyle == null)
                return;

            DmlSchemeColor schemeColor = dmlShapeStyle.LineReference.Color as DmlSchemeColor;
            if (schemeColor == null)
                return;

            Fill = new DmlSolidFill(schemeColor.Resolve(themeProvider));
        }

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        private static readonly IDmlHierarchicalPropertyBagParentProvider gDefaultParentBagProvider =
            new DmlHierarchicalPropertyBagParentContainer(DmlOutlinePropertiesDefaults.Instance);
    }
}
