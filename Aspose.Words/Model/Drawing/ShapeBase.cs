// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2006 by Roman Korchagin

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Revisions;
using Aspose.Words.Themes;
using AsposeHorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment;
using AsposeRelativeHorizontalPosition = Aspose.Words.Drawing.RelativeHorizontalPosition;
using AsposeRelativeVerticalPosition = Aspose.Words.Drawing.RelativeVerticalPosition;
using AsposeVerticalAlignment = Aspose.Words.Drawing.VerticalAlignment;
using AsposeWrapSide = Aspose.Words.Drawing.WrapSide;
using AsposeWrapType = Aspose.Words.Drawing.WrapType;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Base class for objects in the drawing layer, such as an AutoShape, freeform, OLE object, ActiveX control, or picture.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>This is an abstract class. The two derived classes that you can instantiate
    /// are <see cref="Shape"/> and <see cref="GroupShape"/>.</p>
    ///
    /// <p>A shape is a node in the document tree.</p>
    ///
    /// <p>If the shape is a child of a <see cref="Paragraph"/> object, then the shape is said to be "top-level".
    /// Top-level shapes are measured and positioned in points.</p>
    ///
    /// <p>A shape can also occur as a child of a <see cref="GroupShape"/> object when several shapes
    /// are grouped. Child shapes of a group shape are positioned in the coordinate space and units
    /// defined by the <see cref="CoordSize"/> and <see cref="CoordOrigin"/> properties of the parent
    /// group shape.</p>
    ///
    /// <p>A shape can be positioned inline with text or floating. The positioning method is controlled
    /// using the <see cref="WrapType"/> property.</p>
    ///
    /// <p>When a shape is floating, it is positioned relative to something (e.g the current paragraph,
    /// the margin or the page). The relative positioning of the shape is specified using the
    /// <see cref="RelativeHorizontalPosition"/> and <see cref="RelativeVerticalPosition"/> properties.</p>
    ///
    /// <p>A floating shape be positioned explicitly using the <see cref="Left"/> and <see cref="Top"/>
    /// properties or aligned relative to some other object using the <see cref="HorizontalAlignment"/>
    /// and <see cref="VerticalAlignment"/> properties.</p>
    ///
    /// <seealso cref="Shape"/>
    /// <seealso cref="GroupShape"/>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public abstract class ShapeBase :
        CompositeNode,
        IInline,
        IShape,
        IShapeAttrSource,
        ITrackableNode,
        IFillable,
        IGlow,
        IReflection,
        ISoftEdge,
        IShadow
    {
        /// <summary>
        /// Ctor, available to derived classes only.
        /// </summary>
        /// <param name="doc">The owner document. Will be <c>null</c> when loading VML shape
        /// type definitions from the embedded resource.</param>
        /// <param name="markupLanguage">Shape markup language: DrawingML or Vml</param>
        protected ShapeBase(DocumentBase doc, ShapeMarkupLanguage markupLanguage)
            : base(doc)
        {
            mMarkupLanguage = markupLanguage;

            if (doc != null)
                Id = doc.GetNextShapeId();
        }

        #region IInline interface implementation.

        Paragraph IInline.ParentParagraph_IInline
        {
            get { return ParentParagraph; }
        }

        DocumentBase IInline.Document_IInline
        {
            get { return Document; }
        }

        /// <summary>
        /// Fully expands font properties of the shape character into direct formatting.
        /// </summary>
        RunPr IInline.GetExpandedRunPr_IInline(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        /// <summary>
        /// Provides direct access to the font formatting of the shape anchor character.
        /// These attributes only have effect for top level inline shapes.
        /// </summary>
        RunPr IInline.RunPr_IInline
        {
            get { return mRunPr; }
            set { mRunPr = value; }
        }

        #endregion

        #region IShape interface implementation.

        int IShape.ZOrder_IShape
        {
            get { return ZOrder; }
            set { ZOrder = value; }
        }

        #endregion

        #region IShapeAttrSource interface implementation.

        object IShapeAttrSource.GetDirectShapeAttr(int key)
        {
            return mShapePr.GetDirectAttr(key);
        }

        object IShapeAttrSource.FetchInheritedShapeAttr(int key)
        {
            ShapePr parentPr = ShapeTypeLibrary.GetShapeTypePr(ShapeType);

            // For some shape types (such as rectangle, oval etc) I do not have VML shape type definitions.
            // Let's resolve to the default value for such shape types.
            if (parentPr != null)
                return parentPr.FetchAttr(key);
            else
                return ShapePr.FetchDefaultAttr(key);
        }

        object IShapeAttrSource.FetchShapeAttr(int key)
        {
            object value = GetDirectShapeAttrInternal(key);
            return (value != null) ? value : FetchInheritedShapeAttrInternal(key);
        }

        void IShapeAttrSource.SetShapeAttr(int key, object value)
        {
            mShapePr.SetAttr(key, value);
        }

        void IShapeAttrSource.RemoveShapeAttr(int key)
        {
            mShapePr.Remove(key);
        }

        #endregion

        #region IShapeAttrSource wrappers.

        internal object GetDirectShapeAttrInternal(int key)
        {
            return ((IShapeAttrSource)this).GetDirectShapeAttr(key);
        }

        internal void SetShapeAttrInternal(int key, object value)
        {
            ((IShapeAttrSource)this).SetShapeAttr(key, value);
        }

        internal object FetchShapeAttrInternal(int key)
        {
            return ((IShapeAttrSource)this).FetchShapeAttr(key);
        }

        internal object FetchInheritedShapeAttrInternal(int key)
        {
            return ((IShapeAttrSource)this).FetchInheritedShapeAttr(key);
        }

        internal void RemoveShapeAttrInternal(int key)
        {
            ((IShapeAttrSource)this).RemoveShapeAttr(key);
        }

        #endregion

        #region ITrackableNode interface implementation.

        EditRevision ITrackableNode.InsertRevision
        {
            get { return RunPr.InsertRevision; }
            set { RunPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return RunPr.DeleteRevision; }
            set { RunPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return RunPr.MoveFromRevision; }
            set { RunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return RunPr.MoveToRevision; }
            set { RunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            RunPr.Remove(RevisionAttr.MoveFromRevision);
            RunPr.Remove(RevisionAttr.MoveToRevision);
        }

        #endregion

        #region IRunAttrSource interface implementation.

        object IRunAttrSource.GetDirectRunAttr(int fontAttr)
        {
            return mRunPr.GetDirectAttr(fontAttr);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            return InlineHelper.FetchInheritedAttr(this, fontAttr);
        }

        void IRunAttrSource.SetRunAttr(int fontAttr, object value)
        {
            mRunPr.SetAttr(fontAttr, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mRunPr.Clear();
        }

        #endregion

        #region IFillable implementation

        /// <summary>
        /// Changes type of the fill to Solid.
        /// </summary>
        void IFillable.Solid()
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                FillCore.FillType = FillTypeCore.Solid;
            }
            else
            {
                DmlFill curFill = (DmlFill)FillCore;
                if ((curFill.DmlFillType != DmlFillType.SolidFill) && (curFill.DmlFillType != DmlFillType.NoFill))
                {
                    // Mimic Word VBA: use either existing color if any, or a new white color otherwise.
                    // Also, after color is set, Word VBA resets its transparency to a fully opaque color.
                    DmlColor dmlColor = (curFill.DmlColorInternal != null)
                        ? curFill.DmlColorInternal.Clone().UpdateAlpha(1.0)
                        : DmlColor.CreateFromDrColor(DrColor.White);
                    curFill = new DmlSolidFill(dmlColor);

                    ((IFillable)this).SetFill(curFill);
                }
            }
        }

        /// <summary>
        /// Gets a PresetTexture for the fill.
        /// </summary>
        PresetTexture IFillable.GetPresetTexture()
        {
            int textureHash = HashUtil.GetSHA512Hash(FillCore.ImageBytes).GetHashCode();
            return FillUtil.GetTextureByHash(textureHash);
        }

        /// <summary>
        /// Gets a PatternType for the fill.
        /// </summary>
        PatternType IFillable.GetPatternType()
        {
            if (FillCore.FillType != FillTypeCore.Pattern)
                return PatternType.None;

            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                PatternType pattern = FillUtil.GetPatternType(FillCore.ImageBytes);
                // MS Word returns PatternType.LargeGrid for PatternType.Cross.
                if (pattern == PatternType.Cross)
                    return PatternType.LargeGrid;

                return pattern;
            }
            else
            {
                if (FillCore is DmlPatternFill)
                    return ((DmlPatternFill)FillCore).FillPresetPattern;

                return PatternType.None;
            }
        }

        /// <summary>
        /// Changes type of the fill to preset texture.
        /// </summary>
        void IFillable.PresetTextured(PresetTexture presetTexture)
        {
            string textureName = FillUtil.GetTextureName(presetTexture);
            if (textureName == string.Empty)
                throw new ArgumentOutOfRangeException("The specified texture is out of range.");

            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                FillCore.FillType = FillTypeCore.Texture;
                ShapePr[ShapeAttr.Filled] = true;
                ShapePr[ShapeAttr.FillImageBytes] = FillUtil.GetTextureBytes(textureName);
                // Mimics MS Word behaviour.
                ShapePr[ShapeAttr.FillBlipName] = FillUtil.GetTextureTitle(presetTexture);
                DrColor fillColor = FillUtil.GetFillColor(presetTexture);
                if (fillColor != null)
                    ShapePr[ShapeAttr.FillColor] = fillColor;
                else
                    RemoveShapeAttrInternal(ShapeAttr.FillColor);
            }
            else
            {
                DmlBlipFill blipFill = new DmlBlipFill();
                blipFill.BlipFillMode = new DmlBlipFillTile();
                blipFill.Blip.EmbedImage = FillUtil.GetTextureBytes(textureName);

                ((IFillable)this).SetFill(blipFill);
            }
        }

        /// <summary>
        /// Changes type of the fill to a pattern.
        /// </summary>
        void IFillable.Patterned(PatternType patternType)
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                FillCore.FillType = FillTypeCore.Pattern;
                ShapePr[ShapeAttr.Filled] = true;
                if (ShapePr[ShapeAttr.FillColor] == null)
                    FillCore.ColorInternal = DrColor.Black;
                if (ShapePr[ShapeAttr.FillBackColor] == null)
                    FillCore.Color2Internal = DrColor.White;
                string patternName = FillUtil.GetPatternName(patternType);
                ShapePr[ShapeAttr.FillImageBytes] = FillUtil.GetPatternBytes(patternName);
            }
            else
            {
                DmlPatternFill patternFill = new DmlPatternFill();
                patternFill.FillPresetPattern = patternType;

                DmlFill curFill = (DmlFill)FillCore;

                DmlColor dmlColor = (curFill.DmlColorInternal != null)
                    ? curFill.DmlColorInternal.Clone().UpdateAlpha(1.0)
                    : DmlColor.CreateFromDrColor(DrColor.FromNativeColor(DocumentTheme.Colors.Accent1));
                patternFill.ForegroundColor = dmlColor;

                dmlColor = (curFill.DmlColor2Internal != null)
                    ? curFill.DmlColor2Internal.Clone().UpdateAlpha(1.0)
                    : DmlColor.CreateFromDrColor(DrColor.White);
                patternFill.BackgroundColor = dmlColor;

                ((IFillable)this).SetFill(patternFill);
            }
        }

        /// <summary>
        /// Changes type of the fill to two-color gradient.
        /// </summary>
        void IFillable.TwoColorGradient(GradientStyle style, GradientVariant variant)
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                if (ShapePr[ShapeAttr.FillColor] == null)
                    FillCore.ColorInternal = DrColor.Black;
                if (ShapePr[ShapeAttr.FillBackColor] == null)
                    FillCore.Color2Internal = DrColor.White;

                SetVmlGradientFill(style, variant);
            }
            else
            {
                DmlFill curFill = (DmlFill)FillCore;
                DmlColor color1 = (curFill.DmlColorInternal != null)
                    ? curFill.DmlColorInternal.Clone()
                    : DmlColor.CreateFromDrColor(DrColor.FromNativeColor(DocumentTheme.Colors.Accent1));

                DmlColor color2 = (curFill.DmlColor2Internal != null)
                    ? curFill.DmlColor2Internal.Clone()
                    : DmlColor.CreateFromDrColor(DrColor.White);

                DmlGradientFill gradientFill = new DmlGradientFill(color1, color2, style, variant, DocumentTheme);
                ((IFillable)this).SetFill(gradientFill);

                // Reset fallback shape.
                RunPr.Remove(FontAttr.AlternateContent);
            }

            FillCore.RotateWithShape = true;
        }

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.OneColorGradient(GradientStyle style, GradientVariant variant, double degree)
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                if (ShapePr[ShapeAttr.FillColor] == null)
                    FillCore.ColorInternal = DrColor.White;

                DrColor color2 = new DrColor(FillCore.ColorInternal.ToArgb());
                if (MathUtil.AreEqual(degree, 0.5))
                {
                    FillCore.Color2Internal = color2;
                }
                else
                {
                    // The max value for TintAndShade is equal to 255. But we should additionally divide it onto two parts,
                    // because when degree is less than 0.5, then this will be a Shade, otherwise this will be a Tint.
                    const int maxTintAndShade = (int)(255 / 0.5);
                    if (MathUtil.IsLessOrEqual(degree, 0.5))
                    {
                        int shade = (int)System.Math.Ceiling(degree * maxTintAndShade);
                        FillCore.Color2Internal = OfficeColor.Shade(color2, shade);
                    }
                    else
                    {
                        // The Tint is not perfect for a moment and this should be investigated additionally.
                        int tint = (int)System.Math.Ceiling((1 - degree) * maxTintAndShade);
                        FillCore.Color2Internal = OfficeColor.Tint(color2, tint);
                    }
                }

                SetVmlGradientFill(style, variant);
            }
            else
            {
                DmlFill curFill = (DmlFill)FillCore;
                DmlColor color1 = (curFill.DmlColorInternal != null)
                    ? curFill.DmlColorInternal.Clone()
                    : DmlColor.CreateFromDrColor(DrColor.FromNativeColor(DocumentTheme.Colors.Accent1));

                DmlColor color2 = color1.Clone();
                // Check either we need Tint or Shade modifier.
                if (!MathUtil.AreEqual(degree, 0.5))
                {
                    if (MathUtil.IsLessOrEqual(degree, 0.5))
                        color2.ColorModifiers.Add(new DmlShade(degree * 2));
                    else
                        color2.ColorModifiers.Add(new DmlTint((1 - degree) * 2));
                }

                DmlGradientFill gradientFill = new DmlGradientFill(color1, color2, style, variant, DocumentTheme);
                ((IFillable)this).SetFill(gradientFill);

                // Reset fallback shape.
                RunPr.Remove(FontAttr.AlternateContent);
            }

            FillCore.RotateWithShape = true;
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        void IFillable.SetImage(byte[] imageBytes)
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                DmlBlipFill blipFill = new DmlBlipFill();
                ((IFillable)this).SetFill(blipFill);
            }

            FillCore.SetImageBytes(imageBytes);
        }

        /// <summary>
        /// Sets specified fill to this object.
        /// </summary>
        void IFillable.SetFill(IFill fill)
        {
            if (fill == null)
            {
                mFill = null;
                return;
            }

            if (((fill is VmlFill) && (MarkupLanguage != ShapeMarkupLanguage.Vml)) ||
                ((fill is DmlFill) && (MarkupLanguage != ShapeMarkupLanguage.Dml)))
                throw new InvalidOperationException("Invalid fill type for this node.");

            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                mFillCore = fill;
            }
            else
            {
                ((IDmlCommonShapePrSource)DmlNode).Fill = (DmlFill)fill;
            }

            fill.Parent = this;
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
            get { return FillCore.ColorInternal.ToNativeColor(); }
            set
            {
                FillCore.ColorInternal = DrColor.FromNativeColor(value);
                // WORDSNET-5726 Alpha equal to 255 means color is fully opaque.
                // Hence, if it less then 255 we should convert it to the Opacity.
                if (value.A < 255)
                    FillCore.Opacity = value.A / 255d;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.OldOn
        {
            get { return FillCore.On; }
            set { FillCore.On = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (clear) and 1.0 (opaque) representing the degree
        /// of opacity of the specified fill.
        /// </summary>
        double IFillable.OldOpacity
        {
            get { return FillCore.Opacity; }
            set
            {
                // WORDSNET-12980 If color was not initialized yet, then let's initialize it
                // with the new empty color, otherwise opacity will not be set
                // (see DmlFill.Opacity for details).
                // This case is related to DML shapes with "DmlNodeType.Picture" type. Such shapes already have picture
                // fill and additional fill can be specified (there are two properties BlipFill and Fill). Word automation
                // sets opacity for second fill, so it is necessary to initialize color of additional fill. Other DML
                // fillable nodes with blip fill have to change opacity for the blip, but not for the color. In common case
                // VML picture can also contain image data and the fill definition in the markup.
                if (((IFillable)this).FilledColor.IsEmpty)
                    ((IFillable)this).FilledColor = Color.Empty;

                FillCore.Opacity = value;
            }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        byte[] IFillable.FillableImageBytes
        {
            get { return FillCore.ImageBytes; }
        }

        #endregion

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        /// <dev>
        /// Word VBA also changes transparency to fully opaque color only for gradient fills.
        /// However, we do it for some other fill types for a moment as well (Solid, for example).
        /// </dev>
        Color IFillable.FillableForeColor
        {
            get { return FillCore.ColorInternal.ToColorFixAlpha().ToNativeColor(); }
            set { FillCore.ColorInternal = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets a Color object that represents the base foreground color without modifiers for the fill.
        /// </summary>
        Color IFillable.FillableBaseForeColor
        {
            get { return FillCore.ColorInternalUnmodified.ToColorFixAlpha().ToNativeColor(); }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the background color for the fill.
        /// </summary>
        Color IFillable.FillableBackColor
        {
            get { return FillCore.Color2Internal.ToColorFixAlpha().ToNativeColor(); }
            set
            {
                // Word VBA allows to set up color for NoFill and changes the type of the fill to Patterned implicitly.
                if (FillCore is DmlNoFill)
                    ((IFillable)this).SetFill(new DmlPatternFill());

                FillCore.Color2Internal = DrColor.FromNativeColor(value);
            }
        }


        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableForeThemeColor
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return (CanBeFilled(FillCore.ColorInternal))
                        ? FillUtil.NativeToThemeColor(FillCore.ColorInternal, DocumentTheme)
                        : ThemeColor.None;

                DmlFill curFill = (DmlFill)FillCore;
                DmlColor curDmlColor = curFill.DmlColorInternal;
                return ((curDmlColor == null) || (curDmlColor.ColorType != DmlColorType.SchemeColor))
                    ? ThemeColor.None
                    : ((DmlSchemeColor)curDmlColor).Value;
            }
            set
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    // Implemented for uniformity with ShapeMarkupLanguage.Dml.
                    if ((FillCore.FillType == FillTypeCore.Picture) || (FillCore.FillType == FillTypeCore.Texture))
                        throw new InvalidOperationException("Cannot set ForeThemeColor to this Fill.");
                    if (value == ThemeColor.None)
                        ShapePr[ShapeAttr.Filled] = false;
                    else if ((FillCore.ColorInternal == null) || !FillCore.On)
                    {
                        FillCore.FillType = FillTypeCore.Solid;
                        ShapePr[ShapeAttr.Filled] = true;
                    }

                    FillCore.ColorInternal = FillUtil.ThemeToNativeColor(value, DocumentTheme);
                }
                else
                {
                    DmlFill curFill = (DmlFill)FillCore;
                    if (curFill.DmlFillType == DmlFillType.BlipFill)
                        throw new InvalidOperationException("Cannot set ForeThemeColor to this Fill.");

                    if (value == ThemeColor.None)
                    {
                        DrColor curDrColor = curFill.ColorInternal;
                        curFill.ColorInternal = DrColor.Empty;
                        curFill.ColorInternal = curDrColor;
                    }
                    else
                    {
                        curFill.DmlColorInternal = new DmlSchemeColor(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableBackThemeColor
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return (CanBeFilled(FillCore.Color2Internal))
                        ? FillUtil.NativeToThemeColor(FillCore.Color2Internal, DocumentTheme)
                        : ThemeColor.None;

                DmlFill curFill = (DmlFill)FillCore;
                DmlColor curDmlColor = curFill.DmlColor2Internal;
                return ((curDmlColor == null) || (curDmlColor.ColorType != DmlColorType.SchemeColor))
                    ? ThemeColor.None
                    : ((DmlSchemeColor)curDmlColor).Value;
            }
            set
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    // Implemented for uniformity with ShapeMarkupLanguage.Dml.
                    if ((FillCore.FillType == FillTypeCore.Picture) || (FillCore.FillType == FillTypeCore.Texture))
                        throw new InvalidOperationException("Cannot set BackThemeColor to this Fill.");
                    if (value == ThemeColor.None)
                        ShapePr[ShapeAttr.Filled] = false;
                    else if ((FillCore.Color2Internal == null) || !FillCore.On)
                    {
                        // For NoFill the pattern fill is set by default with the same ForeThemeColor and BackThemeColor.
                        FillCore.FillType = FillTypeCore.Pattern;
                        ShapePr[ShapeAttr.Filled] = true;
                        FillCore.ColorInternal = FillUtil.ThemeToNativeColor(value, DocumentTheme);
                    }

                    FillCore.Color2Internal = FillUtil.ThemeToNativeColor(value, DocumentTheme);
                }
                else
                {
                    DmlFill curFill = (DmlFill)FillCore;

                    if (value == ThemeColor.None)
                    {
                        DrColor curDrColor = curFill.Color2Internal;
                        curFill.Color2Internal = DrColor.Empty;
                        curFill.Color2Internal = curDrColor;
                        return;
                    }

                    DmlSchemeColor curThemeColor = new DmlSchemeColor(value);
                    switch (curFill.DmlFillType)
                    {
                        case DmlFillType.GradientFill:
                        case DmlFillType.PatternFill:
                            curFill.DmlColor2Internal = curThemeColor;
                            break;
                        case DmlFillType.GroupFill:
                        case DmlFillType.NoFill:
                        case DmlFillType.SolidFill:
                        case DmlFillType.StyleFill:
                            // For NoFill the pattern fill is set by default with the same ForeThemeColor and BackThemeColor.
                            // VBA doesn't apply BackThemeColor to Solid fill, it just does nothing. I think this behavior is wrong,
                            // so I made the behavior similar to assigning BackThemeColor for NoFill.
                            DmlPatternFill patternFill = new DmlPatternFill();
                            patternFill.ForegroundColor = curThemeColor;
                            patternFill.BackgroundColor = curThemeColor;

                            ((IFillable)this).SetFill(patternFill);
                            break;
                        case DmlFillType.BlipFill:
                        default:
                            throw new InvalidOperationException("Cannot set BackThemeColor to this Fill.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double IFillable.FillableForeTintAndShade
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if (!CanBeFilled(FillCore.ColorInternal) ||
                        (ShapePr[ShapeAttr.FillColorExt] == null) || (ShapePr[ShapeAttr.FillColorExtMod] == null))
                        return 0.0d;

                    int modifier = (int)ShapePr[ShapeAttr.FillColorExtMod];
                    if ((modifier > -256) && (modifier < 256))
                        return modifier >= 0
                            ? 1.0d - ((double)modifier / 255)
                            : -1.0d - ((double)modifier / 255);

                    return 0.0d;
                }
                else
                {
                    DmlColor curColor = ((DmlFill)FillCore).DmlColorInternal;
                    if (curColor == null)
                        return 0.0d;

                    return (curColor.GetColorModifier(DmlColorModifierType.Tint) != null)
                        ? 1.0d - ((DmlTint)curColor.GetColorModifier(DmlColorModifierType.Tint)).Value
                        : (curColor.GetColorModifier(DmlColorModifierType.Shade) != null)
                            ? -1.0d + ((DmlShade)curColor.GetColorModifier(DmlColorModifierType.Shade)).Value
                            : 0.0d;
                }
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, -1.0d, 1.0d, "ForeTintAndShade");

                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if (!CanBeFilled(FillCore.ColorInternal))
                        throw new InvalidOperationException("Cannot apply ForeTintAndShade to this Fill.");

                    DrColor baseColor = (ShapePr[ShapeAttr.FillColorExt] != null)
                        ? (DrColor)ShapePr[ShapeAttr.FillColorExt]
                        : FillCore.ColorInternal;
                    ShapePr[ShapeAttr.FillColorExt] = baseColor;

                    int tintAndShadeModifier = FillUtil.VmlTintAndShadeModifier(value);
                    ShapePr[ShapeAttr.FillColorExtMod] = value >= 0
                        ? tintAndShadeModifier
                        : -tintAndShadeModifier;

                    if (value > 0)
                        FillCore.ColorInternal = FillUtil.VmlTint(baseColor, tintAndShadeModifier);
                    if (value < 0)
                        FillCore.ColorInternal = FillUtil.VmlShade(baseColor, tintAndShadeModifier);
                }
                else
                {
                    DmlFill curFill = (DmlFill)FillCore;
                    DmlColor curColor = curFill.DmlColorInternal;
                    if (curColor == null)
                        throw new InvalidOperationException("Cannot apply ForeTintAndShade to this Fill.");

                    IDmlColorModifier dmlColorModifier = curColor.GetColorModifier(DmlColorModifierType.Shade);
                    if (dmlColorModifier != null)
                        curColor.ColorModifiers.Remove(dmlColorModifier);
                    dmlColorModifier = curColor.GetColorModifier(DmlColorModifierType.Tint);
                    if (dmlColorModifier != null)
                        curColor.ColorModifiers.Remove(dmlColorModifier);

                    if (value < 0)
                        curColor.ColorModifiers.Add(new DmlShade(1 + value));
                    if (value > 0)
                        curColor.ColorModifiers.Add(new DmlTint(1 - value));
                }
            }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double IFillable.FillableBackTintAndShade
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if (!CanBeFilled(FillCore.Color2Internal) ||
                        (ShapePr[ShapeAttr.FillBackColorExt] == null) || (ShapePr[ShapeAttr.FillBackColorExtMod] == null))
                        return 0.0d;

                    int modifier = (int)ShapePr[ShapeAttr.FillBackColorExtMod];
                    if ((modifier > -256) && (modifier < 256))
                        return modifier >= 0
                            ? 1.0d - ((double)modifier / 255)
                            : -1.0d - ((double)modifier / 255);

                    return 0.0d;
                }
                else
                {
                    DmlColor curColor = ((DmlFill)FillCore).DmlColor2Internal;
                    if (curColor == null)
                        return 0.0d;

                    return (curColor.GetColorModifier(DmlColorModifierType.Tint) != null)
                        ? 1.0d - ((DmlTint)curColor.GetColorModifier(DmlColorModifierType.Tint)).Value
                        : (curColor.GetColorModifier(DmlColorModifierType.Shade) != null)
                            ? -1.0d + ((DmlShade)curColor.GetColorModifier(DmlColorModifierType.Shade)).Value
                            : 0.0d;
                }
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, -1.0d, 1.0d, "BackTintAndShade");

                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if (!CanBeFilled(FillCore.Color2Internal))
                        throw new InvalidOperationException("Cannot apply BackTintAndShade to this Fill.");

                    DrColor baseColor = (ShapePr[ShapeAttr.FillBackColorExt] != null)
                        ? (DrColor)ShapePr[ShapeAttr.FillBackColorExt]
                        : FillCore.Color2Internal;
                    ShapePr[ShapeAttr.FillBackColorExt] = baseColor;

                    int tintAndShadeModifier = FillUtil.VmlTintAndShadeModifier(value);
                    ShapePr[ShapeAttr.FillBackColorExtMod] = (value > 0)
                        ? tintAndShadeModifier
                        : -tintAndShadeModifier;

                    if (value > 0)
                        FillCore.Color2Internal = FillUtil.VmlTint(baseColor, tintAndShadeModifier);
                    if (value < 0)
                        FillCore.Color2Internal = FillUtil.VmlShade(baseColor, tintAndShadeModifier);
                }
                else
                {
                    DmlFill curFill = (DmlFill)FillCore;
                    DmlColor curColor = curFill.DmlColor2Internal;
                    if (curColor == null)
                        throw new InvalidOperationException("Cannot apply BackTintAndShade to this Fill.");

                    IDmlColorModifier dmlColorModifier = curColor.GetColorModifier(DmlColorModifierType.Shade);
                    if (dmlColorModifier != null)
                        curColor.ColorModifiers.Remove(dmlColorModifier);
                    dmlColorModifier = curColor.GetColorModifier(DmlColorModifierType.Tint);
                    if (dmlColorModifier != null)
                        curColor.ColorModifiers.Remove(dmlColorModifier);

                    if (value < 0)
                        curColor.ColorModifiers.Add(new DmlShade(1 + value));
                    if (value > 0)
                        curColor.ColorModifiers.Add(new DmlTint(1 - value));
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.FillableVisible
        {
            get { return FillCore.On; }
            set { FillCore.On = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency of the specified fill.
        /// </summary>
        double IFillable.FillableTransparency
        {
            get { return (1.0 - FillCore.Opacity); }
            set
            {
                // Word VBA allows to set up opacity (transparency) for NoFill and
                // changes the type of fill to Solid implicitly.
                if (FillCore is DmlNoFill)
                    ((IFillable)this).SetFill(new DmlPatternFill());

                FillCore.Opacity = (1.0 - value);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the fill rotates with the specified object.
        /// </summary>
        bool IFillable.RotateWithObject
        {
            get { return FillCore.RotateWithShape; }
            set { FillCore.RotateWithShape = value; }
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
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return TextureAlignment.None;

                DmlBlipFill dmlBlipFill = FillCore as DmlBlipFill;
                if (dmlBlipFill == null)
                    return TextureAlignment.None;

                DmlBlipFillTile dmlBlipFillTile = dmlBlipFill.BlipFillMode as DmlBlipFillTile;

                return (dmlBlipFillTile == null)
                    ? TextureAlignment.None
                    : FillUtil.DmlRectangleToTextureAlignment(dmlBlipFillTile.Alignment);
            }
            set
            {
                if (value == TextureAlignment.None)
                    throw new InvalidOperationException("TextureNone cannot be applied directly.");

                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    throw new InvalidOperationException("TextureAlignment cannot be applied to VML shapes.");

                DmlBlipFill dmlBlipFill = FillCore as DmlBlipFill;
                if (dmlBlipFill == null)
                    throw new InvalidOperationException("TextureAlignment can be applied to texture fill only.");

                DmlBlipFillTile dmlBlipFillTile = dmlBlipFill.BlipFillMode as DmlBlipFillTile;
                if (dmlBlipFillTile == null)
                    throw new InvalidOperationException("TextureAlignment can be applied to tile texture fill only.");

                dmlBlipFillTile.Alignment = FillUtil.TextureToDmlRectangleAlignment(value);
            }
        }

        /// <summary>
        /// Gets IThemeProvider object.
        /// </summary>
        IThemeProvider IFillable.FillableThemeProvider
        {
           get { return DocumentTheme; }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        double IFillable.GradientAngle
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    throw new InvalidOperationException(InvalidAction);

                return FillCore.Angle;
            }
            set
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    throw new InvalidOperationException(InvalidAction);

                FillCore.Angle = value;
            }
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
        /// Gets a collection of <see cref="GradientStop"/> objects for the fill.
        /// </summary>
        /// <remarks>This is facade for <see cref="DmlGradientFill.GradientStops"/> collection.</remarks>
        GradientStopCollection IFillable.GradientStops
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Dml)
                {
                    DmlGradientFill fill = FillCore as DmlGradientFill;
                    if (fill != null)
                        return fill.GradientStopCollection;
                }

                throw new InvalidOperationException(InvalidAction);
            }
        }

        #endregion

        #region IGlow implementation

        /// <summary>
        /// Removes glow from the parent object;
        /// </summary>
        void IGlow.RemoveGlow()
        {
            if ((DmlShape == null) || (DmlShape.Effects == null))
                return;

            DmlShape.Effects.RemoveEffect(DmlShapeEffectType.Glow);
        }

        /// <summary>
        /// Gets or sets a Color object that represents the color for a glow effect.
        /// </summary>
        Color IGlow.Color
        {
            get
            {
                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(false);
                if (glow == null)
                    return Color.Black;

                return glow.Color.CreateDrColor(DocumentTheme, null).ToNativeColor();
            }

            set
            {
                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(true);
                glow.Color = DmlHexRgbColor.FromDrColor(DrColor.FromNativeColor(value));
            }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency for the glow effect.
        /// </summary>
        double IGlow.Transparency
        {
            get
            {
                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(false);
                if ((glow == null) || (glow.Color == null))
                    return 0.0;

                DmlAlpha alpha = glow.Color.Alpha;
                if (alpha == null)
                    return 0.0;

                return 1.0 - alpha.Value;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Glow.Transparency");

                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(true);
                if (glow.Color == null)
                    glow.Color = new DmlHexRgbColor();

                glow.Color.UpdateAlpha(1.0 - value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the radius for a glow effect.
        /// </summary>
        double IGlow.Radius
        {
            get
            {
                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(false);
                if (glow == null)
                    return 0.0;

                return ConvertUtilCore.EmuToPoint(glow.Radius);
            }
            set
            {
                DmlShapeGlowEffect glow = GetDmlShapeGlowEffect(true);
                glow.Radius = ConvertUtilCore.PointToEmu(value);
            }
        }

        #endregion

        #region IReflection implementation

        /// <summary>
        /// Removes reflection from the parent object;
        /// </summary>
        void IReflection.RemoveReflection()
        {
            if ((DmlShape == null) || (DmlShape.Effects == null))
                return;

            DmlShape.Effects.RemoveEffect(DmlShapeEffectType.Reflection);
        }

        /// <summary>
        /// Gets or sets a double value that specifies the degree of blur effect applied to the reflection effect in points.
        /// </summary>
        double IReflection.Blur
        {
            get
            {
                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(false);
                if (reflection == null)
                    return 0.0;

                return ConvertUtilCore.EmuToPoint(reflection.BlurRadius);
            }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "Reflection.Blur");

                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(true);
                reflection.BlurRadius = ConvertUtilCore.PointToEmu(value);
            }
        }

        /// <summary>
        /// Gets or sets the amount of separation of the reflected image from the object in points.
        /// </summary>
        double IReflection.Distance
        {
            get
            {
                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(false);
                if (reflection == null)
                    return 0.0;

                return ConvertUtilCore.EmuToPoint(reflection.Distance);
            }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "Reflection.Distance");

                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(true);
                reflection.Distance = ConvertUtilCore.PointToEmu(value);
            }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 and 1.0 representing the size of the reflection
        /// as a percentage of the reflected object.
        /// </summary>
        double IReflection.ReflectionSize
        {
            get
            {
                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(false);
                if (reflection == null)
                    return 0.0;

                return reflection.EndPosition;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Reflection.Size");

                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(true);
                reflection.EndPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency for the reflection effect.
        /// </summary>
        double IReflection.ReflectionTransparency
        {
            get
            {
                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(false);
                if (reflection == null)
                    return 0.0;

                return 1.0 - reflection.StartAlpha;
            }
            set
            {
                ArgumentUtil.CheckRangeInclusive(value, 0.0, 1.0, "Reflection.Transparency");

                DmlShapeReflectionEffect reflection = GetDmlShapeReflectionEffect(true);
                reflection.StartAlpha = 1.0 - value;
            }
        }

        #endregion

        #region ISoftEdge implementation

        /// <summary>
        /// Removes soft edge from the parent object;
        /// </summary>
        void ISoftEdge.RemoveSoftEdge()
        {
            if ((DmlShape == null) || (DmlShape.Effects == null))
                return;

            DmlShape.Effects.RemoveEffect(DmlShapeEffectType.SoftEdges);
        }

        /// <summary>
        /// Gets or sets the length of the radius for a soft edge effect.
        /// </summary>
        double ISoftEdge.EdgeRadius
        {
            get
            {
                DmlShapeSoftEdgeEffect softEdge = GetDmlShapeSoftEdgeEffect(false);
                if (softEdge == null)
                    return 0.0;

                return ConvertUtilCore.EmuToPoint(softEdge.Radius);
            }
            set
            {
                DmlShapeSoftEdgeEffect softEdge = GetDmlShapeSoftEdgeEffect(true);
                softEdge.Radius = ConvertUtilCore.PointToEmu(value);
            }
        }

        #endregion

        #region IShadow implementation

        /// <summary>
        /// Removes shadow from the parent object;
        /// </summary>
        void IShadow.RemoveShadow()
        {
            GraphicData.RemoveShadow();
        }

        /// <summary>
        /// Gets or sets a <see cref="ShadowType"/> value for the shadow effect.
        /// </summary>
        ShadowType IShadow.ShadowType
        {
            get
            {
                return GraphicData.ShadowType;
            }
            set
            {
                GraphicData.RemoveShadow();
                GraphicData.ShadowType = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether shadow effect is visible.
        /// </summary>
        bool IShadow.Visible
        {
            get { return GraphicData.ShadowEnabled; }
        }

        /// <summary>
        /// Gets or sets <see cref="System.Drawing.Color"/> object that represents the color for a shadow effect.
        /// </summary>
        Color IShadow.ShadowColors
        {
            get { return GraphicData.ShadowColor; }
            set { GraphicData.ShadowColor = value; }
        }

        /// <summary>
        /// Gets or sets the degree of transparency for a shadow effect.
        /// </summary>
        double IShadow.ShadowTransparency
        {
            get { return GraphicData.ShadowTransparency; }
            set { GraphicData.ShadowTransparency = value; }
        }
        #endregion

        /// <summary>
        /// Converts a value from the local coordinate space into the coordinate space of the parent shape.
        /// </summary>
        public PointF LocalToParent(PointF value)
        {
            // Offset the value by the inverse of coordorigin (subtract).
            // The value is still in local coordinates.
            float x = value.X - CoordOrigin.X;
            float y = value.Y - CoordOrigin.Y;

            double widthFactor = !MathUtil.IsZero(CoordSize.Width) ? Width / CoordSize.Width : 1;
            double heightFactor = !MathUtil.IsZero(CoordSize.Height) ? Height / CoordSize.Height : 1;

            // Scale the value from local coordinates into parent coordinates.
            x = (float)(x * widthFactor);
            y = (float)(y * heightFactor);

            // Offset the value by the position of the parent shape in parent coordinates.
            x += (float)Left;
            y += (float)Top;

            return new PointF(x, y);
        }

        /// <summary>
        /// Converts a rectangle that is in the coordinate space of the child shape into a value in points
        /// of the coordinate space of the topmost parent shape.
        /// </summary>
        internal RectangleF ConvertLocalToTopmostAnchor(RectangleF rect)
        {
            // WORDSNET-27303 If a child shape is rotated 90 degrees, parent Y scale is used to calculate X coordinated
            // and vice versa. Mimic Word's behavior when rotation is not a multiple of 90 degrees.
            // Just rotate the rect before scaling.
            bool swapXY = ((int)System.Math.Round(Rotation / 90, MidpointRounding.AwayFromZero) % 2 == 1);
            if (swapXY)
                rect = Rotate90Degrees(rect);

            CompositeNode parent = ParentNode;
            PointF topLeft = rect.Location;
            PointF bottomRight = new PointF(rect.Right, rect.Bottom);

            bool parentWidthZeroSize = false;
            bool parentHeightZeroSize = false;
            while (parent is ShapeBase)
            {
                ShapeBase parentShape = (ShapeBase)parent;

                topLeft = parentShape.LocalToParent(topLeft);
                bottomRight = parentShape.LocalToParent(bottomRight);

                parentWidthZeroSize = (parentShape.Width == 0) ||
                    ((parentShape.CoordSize.Width == 0) && (bottomRight.X - topLeft.X > parentShape.Width));
                parentHeightZeroSize = (parentShape.Height == 0) ||
                    ((parentShape.CoordSize.Height == 0) && (bottomRight.Y - topLeft.Y > parentShape.Height));

                parent = parent.ParentNode;
            }

            float width = bottomRight.X - topLeft.X;
            float height = bottomRight.Y - topLeft.Y;

            // WORDSNET-28254, WORDSNET-28627 In case of a parent group with zero dimensions we shall convert
            // Width and Height from the local coordinate system, EMU in case of DML and Twip in case of VML.
            if (parentWidthZeroSize)
                width = MarkupLanguage == ShapeMarkupLanguage.Dml
                    ? (float)ConvertUtilCore.EmuToPoint(bottomRight.X - topLeft.X)
                    : (float)ConvertUtilCore.TwipToPoint(bottomRight.X - topLeft.X);

            if (parentHeightZeroSize)
                height = MarkupLanguage == ShapeMarkupLanguage.Dml
                    ? (float)ConvertUtilCore.EmuToPoint(bottomRight.Y - topLeft.Y)
                    : (float)ConvertUtilCore.TwipToPoint(bottomRight.Y - topLeft.Y);

            RectangleF result = new RectangleF(topLeft.X, topLeft.Y, width, height);
            if (swapXY)
            {
                // Unrotate the result rect back.
                result = Rotate90Degrees(result);
            }

            return result;
        }

        /// <summary>
        /// Rotates the rectangle 90 degrees around its center.
        /// </summary>
        internal static RectangleF Rotate90Degrees(RectangleF rect)
        {
            double centerX = rect.Left + rect.Width / 2d;
            double centerY = rect.Top + rect.Height / 2d;
            float left = (float)(centerX - rect.Height / 2d);
            float top = (float)(centerY - rect.Width / 2d);
            return new RectangleF(left, top, rect.Height, rect.Width);
        }

        /// <summary>
        /// Returns <c>true</c> if this shape or one of it's children down the tree has the textbox.
        /// </summary>
        internal bool HasTextBoxes()
        {
            if (NodeType == NodeType.Shape)
                return HasChildNodes;

            foreach (Shape childShape in GetChildNodes(NodeType.Shape, true))
            {
                if (childShape.HasChildNodes)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if one of shape's children down the tree has an image.
        /// </summary>
        internal bool HasImages()
        {
            foreach (Shape childShape in GetChildNodes(NodeType.Shape, true))
                if (childShape.HasImage)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape or one of it's children down the tree has fields.
        /// </summary>
        internal bool HasFields()
        {
            return GetChildNodes(NodeType.FieldStart, true).Count > 0;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape or one of it's children down the tree has HRefs.
        /// </summary>
        internal bool HasHRefs()
        {
            if (NodeType == NodeType.Shape)
                return !string.IsNullOrEmpty(HRef);

            foreach (Shape childShape in GetChildNodes(NodeType.Shape, true))
            {
                if (!string.IsNullOrEmpty(HRef))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape or one of it's children down the tree has bookmarks.
        /// </summary>
        internal bool HasBookmarks()
        {
            return GetChildNodes(NodeType.BookmarkStart, true).Count > 0;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape placed inside header or footer.
        /// </summary>
        internal bool IsHeaderFooter()
        {
            return GetStoryAncestor(NodeType.HeaderFooter) != null;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape or one of it's children down the tree is of the specified DML node type.
        /// </summary>
        internal bool HasDmlNodes(DmlNodeType dmlNodeType)
        {
            if (DmlNode != null && DmlNode.DmlNodeType == dmlNodeType)
                return true;

            Node[] childShapes = GetChildNodes(NodeType.Shape, true).ToArray();
            foreach (Node childNode in childShapes)
            {
                Shape childShape = (Shape)childNode; // casting for java
                if (childShape.DmlNode != null && childShape.DmlNode.DmlNodeType == dmlNodeType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this shape has textbox with text or image.
        /// </summary>
        /// <remarks> Used on the rendering side for shadow creation.</remarks>
        internal bool HasNonEmptyTextbox()
        {
            if (!HasTextbox)
                return false;

            return !StringUtil.ContainsOnlyWhitespaces(GetText()) || HasImages();
        }

        /// <summary>
        /// Sets the shape type. Users are not to modify shape type directly.
        /// </summary>
        internal void SetShapeType(ShapeType shapeType)
        {
            // Some shape types can not be created with VML markup language.
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                Debug.Assert(DmlEnum.HasVmlPresetGeometry(shapeType) || !IsAllowedShapeTypes(shapeType));

            SetShapeAttrInternal(ShapeAttr.ShapeType, shapeType);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
#if CPLUSPLUS // WORDSCPP-628 Update fall-back parent just before cloning. in C++ source document from which shape was read might be "disposed" already and NullReferece exception occurs.
            UpdateFallBackParent();
#endif
            ShapeBase lhs = (ShapeBase)base.Clone(isCloneChildren, cloningListener);
            lhs.mShapePr = mShapePr.Clone();
            lhs.mRunPr = mRunPr.Clone();
            lhs.mLocksCache = null;
            lhs.mFontCache = null;
            lhs.mGraphicData = null;
            if (DmlNode != null)
            {
                lhs.DmlNode = DmlNode.Clone(isCloneChildren, cloningListener);
                lhs.DmlNode.SetDrawingML(lhs);
            }
            return lhs;
        }

        /// <summary>
        /// Fully expands font properties of the shape character into direct formatting.
        /// </summary>
        internal RunPr GetExpandedRunPr(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        internal void SetSizeSafe(double width, double height)
        {
            SetSizeSafe(width, height, null, 0);
        }

        /// <summary>
        /// Sets width and height of the shape, if width or height exceeded the maximum shape size,
        /// Scales shape to fit the container size (cell, textbox or page).
        /// Use this method only if image size rescaling is required. Normally upon reading Office file formats,
        /// <see cref="SetWidthSafe"/> and <see cref="SetHeightSafe"/> method must be used.
        /// </summary>
        internal void SetSizeSafe(double width, double height, SizeD imageSize, double rotationAng)
        {
            SizeD scaledImageSize = ShapeSizeValidationHelper.GetScaledImageSize(this, width, height, imageSize, rotationAng);

            SetWidthSafe(scaledImageSize.Width);
            SetHeightSafe(scaledImageSize.Height);
        }

        /// <summary>
        /// Sets width of the shape. Brings the value into the valid range.
        /// </summary>
        /// <remarks>
        /// Method processes shape width without processing <see cref="AspectRatioLocked"/> attribute.
        /// AspectRatioLocked have to be taken in account only when client is changing width programmatically
        /// through the object model.
        /// </remarks>
        internal void SetWidthSafe(double value)
        {
            SetWidthCore(value, false);
        }

        /// <summary>
        /// Sets height of the shape. Brings the value into the valid range.
        /// </summary>
        /// <remarks>
        /// Method processes shape height without processing <see cref="AspectRatioLocked"/> attribute.
        /// AspectRatioLocked have to be taken in account only when client is changing height programmatically
        /// through the object model.
        /// </remarks>
        internal void SetHeightSafe(double value)
        {
            SetHeightCore(value, false);
        }

        /// <summary>
        /// We normally validate shape size when width or height is set, but we also allow validation
        /// of the shape size before document save. This is needed in case some child-level
        /// shape with huge local coordinates was turned into a top-level shape by the user.
        /// </summary>
        internal void MakeSizeValid()
        {
            ValidateZeroSizeShape();

            SetWidthSafe(Width);
            SetHeightSafe(Height);
        }

        /// <summary>
        /// Gets the topmost parent shape (root) of the shape tree.
        /// </summary>
        internal ShapeBase GetTopLevelParentShape()
        {
            ShapeBase shape = this;
            while (!shape.IsTopLevel)
                shape = (ShapeBase)shape.ParentNode;

            return shape;
        }

        /// <summary>
        /// Verify whether shape Bounds need to be updated.
        /// </summary>
        /// <remarks>
        /// Currently update size only for <see cref="Aspose.Words.Drawing.ShapeType.NonPrimitive"/> shapes with zero size
        /// if there are <see cref="ShapeAttr.GeometryVertices"/>.
        /// </remarks>
        internal bool RequiresBoundsRecalculation()
        {
            PathPoint[] pathPoints = (PathPoint[])ShapePr.GetDirectAttr(ShapeAttr.GeometryVertices);
            return ((ShapeType == ShapeType.NonPrimitive) && (MathUtil.IsZero(Width)) && (MathUtil.IsZero(Height))
                    && (pathPoints != null) && (pathPoints.Length > 0));
        }

        /// <summary>
        /// Updates shape size and position.
        /// It is not enough just update shape size, we also need update position of content inside shape,
        /// because previously it was drawn from top left corner of page.
        /// </summary>
        internal void UpdateSizeAndPosition()
        {
            // Gets the bounding rectangle.
            RectangleF boundingRect = GetPathBoundingRect();
            if (boundingRect.Size.IsEmpty)
                return;

            // Set new size and margins.
            double width = ConvertUtilCore.TwipToPoint(boundingRect.Width);
            double height = ConvertUtilCore.TwipToPoint(boundingRect.Height);

            SetWidthSafe(width);
            SetHeightSafe(height);

            Left = ConvertUtilCore.TwipToPoint(boundingRect.X);
            Top = ConvertUtilCore.TwipToPoint(boundingRect.Y);

            // Position of path should be changed according to margins.
            PathPoint[] pathPoints = (PathPoint[])ShapePr[ShapeAttr.GeometryVertices];
            for (int i = 0; i < pathPoints.Length; i++)
            {
                pathPoints[i] = new PathPoint(pathPoints[i].X.Value - (int)boundingRect.X,
                                              pathPoints[i].Y.Value - (int)boundingRect.Y);
            }

            SetShapeAttrInternal(ShapeAttr.GeometryVertices, pathPoints);
        }

        /// <summary>
        /// Adjust shape height if horizontal rule is specified.
        /// </summary>
        internal void AdjustHorizontalRuleHeight()
        {
            if (IsHorizontalRule && ShapePr.Contains(ShapeAttr.HRHeight))
            {
                SetHeightSafe(ConvertUtilCore.TwipToPoint((int)GetDirectShapeAttrInternal(ShapeAttr.HRHeight)));
                RemoveShapeAttrInternal(ShapeAttr.HRHeight);
            }
        }

        /// <summary>
        /// If the size or position of the shape is specified using relative (percentage) then updates the
        /// actual size and position of the shape. But uses a very simplistic and crude algorithm for this.
        /// Only suitable for down converting when exporting to formats that do not support relative position and size.
        /// </summary>
        internal void UpdateSizeAndPositionFromRelative()
        {
            // I think only top level shapes have relative positioning.
            if (!IsTopLevel)
                return;

            // In DrawingMl there are cases when size is not updated from relative. Do the same.
            if ((MarkupLanguage == ShapeMarkupLanguage.Dml) && !DmlNode.IsUpdateDimensionsFromRelative)
                return;

            Section section = (Section)GetAncestor(NodeType.Section);
            if (section == null)
                return;

            PageSetup ps = section.PageSetup;
            UpdateLeftFromPercent(ps);
            UpdateTopFromPercent(ps);
            UpdateWidthFromPercent(ps);
            UpdateHeightFromPercent(ps);
        }

        internal void SetCoordSizeSafe(Size value)
        {
            GraphicData.SetCoordSizeSafe(value);
        }

        internal void SetCoordSizeWidthSafe(int width)
        {
            GraphicData.SetCoordSizeWidthSafe(width);
        }

        internal void SetCoordSizeHeightSafe(int height)
        {
            GraphicData.SetCoordSizeHeightSafe(height);
        }

        /// <summary>
        /// Returns <c>true</c> if Dml shape was changed since load.
        /// </summary>
        internal bool IsChangedSinceLoad()
        {
            return (mHashCodeCache != CalculateHashCode());
        }

        /// <summary>
        /// Stores original hash code, calculated right after
        /// document reading (please see <see cref="Aspose.Words.Validation.DocumentPostLoader.ProcessFallback(ShapeBase)"/>)
        /// </summary>
        internal void MarkLoaded()
        {
            mHashCodeCache = CalculateHashCode();
        }

        /// <summary>
        /// Sets correct parent for FallBack node.
        /// </summary>
        /// <remarks>
        /// andrnosk: After cloning/importing document DrawingML belongs to cloned/imported document, but FallBack
        /// still belongs to the original document because it is alternate node
        /// outside of the main tree.
        /// That is why we need to set parent directly to FallBack.
        /// DS: Null node must be set as parent. At this case the FallBack still outside of the main tree and it has valid
        /// reference to the document.
        /// </remarks>
        internal void UpdateFallBackParent()
        {
            if ((FallBack != null) && (FallBack.Document != Document))
                FallBack.SetParent(Document.NullNode);
        }

        internal ImageSizeCore GetShapeImageSize()
        {
            Shape shape = this as Shape;
            if ((shape != null) && shape.HasImage)
            {
                ImageData imageData = shape.ImageData;

                // If an image is linked, but not stored in the document, try to load ImageBytes which will be used to get size.
                byte[] imageBytes = shape.HasImageBytes ? imageData.ImageBytes : imageData.LoadImageBytes();

                if (imageBytes != null)
                    return ImageUtil.GetImageSize(imageBytes);
            }

            return null;
        }

        /// <summary>
        /// Sets a flag indicating that shape is a signature line.
        /// </summary>
        internal void SetIsSignatureLine(bool value)
        {
            if (IsSignatureLine != value)
            {
                SetShapeAttrInternal(ShapeAttr.IsSignatureLine, value);
                if (value)
                    SetShapeAttrInternal(ShapeAttr.SigSetupProvId, Guid.Empty.ToString("B").ToUpper());
            }
        }

        /// <summary>
        /// Generated alternative text.
        /// In case older format conversion MS Word formats following string "Title: {titleText} - Description: {descText}"
        /// If there is no description "Title: {titleText}"
        /// If there is no title "{descText}"
        /// </summary>
        internal static string GenerateAltText(string title, string description)
        {
            string altText;

            if (StringUtil.HasChars(title))
            {
                string titleText = string.Format("Title: {0}", title);

                if (StringUtil.HasChars(description))
                    altText = string.Format("{0} - Description: {1}", titleText, description);
                else
                    altText = titleText;
            }
            else
            {
                altText = description;
            }

            return altText;
        }

        /// <summary>
        /// True when specified shape type can be created from the customers code.
        /// </summary>
        internal static bool IsAllowedShapeTypes(ShapeType shapeType)
        {
            // Let's block some shapes from user creation until we provide full API for them.
            switch (shapeType)
            {
                case ShapeType.OleControl:
                case ShapeType.OleObject:
                case ShapeType.NonPrimitive:
                case ShapeType.CustomShape:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Indicates whether the shape contains an SVG image in a DML extension.
        /// </summary>
        internal bool HasSvgBlip()
        {
            DmlPicture picture = DmlNode as DmlPicture;

            if ((picture == null) || (picture.BlipFill == null) || !picture.BlipFill.Blip.HasExtensions)
                return false;

            DmlExtension svgExtension = picture.BlipFill.Blip.Extensions[DmlExtensionUri.SvgBlip];

            if ((svgExtension == null) || (svgExtension.SvgBlip == null))
                return false;

            return true;
        }

        /// <summary>
        /// Returns SVG image bytes from a DML extenstion, if the shape contains one.
        /// </summary>
        /// <returns>SVG image bytes or <c>null</c>.</returns>
        internal byte[] GetImageBytesFromSvgBlip()
        {
            if (!HasSvgBlip())
                return null;

            DmlExtension svgExtension = ((DmlPicture)DmlNode).BlipFill.Blip.Extensions[DmlExtensionUri.SvgBlip];
            return svgExtension.SvgBlip.GetImageBytes();
        }

        private void UpdateLeftFromPercent(PageSetup ps)
        {
            object objLeftPercent = ShapePr[ShapeAttr.LeftPercent];
            if ((objLeftPercent == null) || ((int)objLeftPercent == DefaultPercentValue))
                return;

            double percent = (int)objLeftPercent / 1000.0;
            switch (RelativeHorizontalPosition)
            {
                case RelativeHorizontalPosition.Margin:
                    Left = ps.ContentWidth * percent;
                    break;
                case RelativeHorizontalPosition.Page:
                    Left = ps.PageWidth * percent;
                    break;
                case RelativeHorizontalPosition.LeftMargin:
                case RelativeHorizontalPosition.OutsideMargin:  // In this simplistic algorithm outside margin is a left margin.
                    Left = ps.LeftMargin * percent;
                    break;
                case RelativeHorizontalPosition.RightMargin:
                case RelativeHorizontalPosition.InsideMargin:   // In this simplistic algorithm inside margin is a right margin.
                    Left = ps.RightMargin * percent;
                    break;
                default:
                    // All others are not valid for relative position.
                    break;
            }

            // WORDSNET-12639 Take effects size in account when calculate relative position of shape.
            if (MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                // Get left and right effect extent.
                double effectExtentLeft = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentLeft));

                // Left position must be calculated from shapes edge, if there is effect at the left side
                // it is required to add its extent to left position calculated from relative.
                if (!MathUtil.IsZero(effectExtentLeft))
                    Left += effectExtentLeft;
            }
        }

        private void UpdateTopFromPercent(PageSetup ps)
        {
            // WORDSNET-12671 Mimic MS Word behavior and ignore relative vertical position if shape is inside table.
            Node parentTable = GetAncestor(NodeType.Table);
            if (parentTable != null)
                return;

            object objTopPercent = ShapePr[ShapeAttr.TopPercent];
            if ((objTopPercent == null) || ((int)objTopPercent == DefaultPercentValue))
                return;

            double percent = (int)objTopPercent / 1000.0;
            switch (RelativeVerticalPosition)
            {
                case RelativeVerticalPosition.Margin:
                    Top = ps.ContentHeight * percent;
                    break;
                case RelativeVerticalPosition.Page:
                    Top = ps.PageHeight * percent;
                    break;
                case RelativeVerticalPosition.TopMargin:
                case RelativeVerticalPosition.OutsideMargin:    // We some reason MS Word calculates outside and inside margins as the top margin.
                case RelativeVerticalPosition.InsideMargin:
                    Top = ps.TopMargin * percent;
                    break;
                case RelativeVerticalPosition.BottomMargin:
                    Top = ps.BottomMargin * percent;
                    break;
                default:
                    // All others are not valid for relative position.
                    break;
            }

            // WORDSNET-12639 Take effects size in account when calculate relative position of shape.
            if (MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                // Get left and right effect extent.
                double effectExtentTop = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentTop));

                // Top position must be calculated from shapes edge, if there is effect at the top side
                // it is required to add its extent to top position calculated from relative.
                if (!MathUtil.IsZero(effectExtentTop))
                    Top += effectExtentTop;
            }
        }

        private void UpdateWidthFromPercent(PageSetup ps)
        {
            // andrnosk: WORDSNET-9684 Mimic MS Word behavior and do not update
            // shape width in case when TextBox shape is Square and text layout is vertical.
            // WORDSNET-13632 MSW for another shapes with vertical text layout and in-line shapes
            // does not update shape width.
            if (IsPercentWidthInapplicable)
                return;

            object objWidthPercent = WidthPercent;
            if (objWidthPercent == null)
                return;

            double percent = (int)objWidthPercent / 1000.0;
            // Width cannot be negative or zero.
            if (percent <= 0)
                return;

            double newWidth = Width;
            switch (RelativeHorizontalSize)
            {
                case RelativeHorizontalSize.Margin:
                    newWidth = ps.ContentWidth * percent;
                    break;
                case RelativeHorizontalSize.Page:
                    newWidth = ps.PageWidth * percent;
                    break;
                case RelativeHorizontalSize.LeftMargin:
                case RelativeHorizontalSize.OuterMargin: // In this simplistic algorithm outside margin is a left margin.
                    newWidth = ps.LeftMargin * percent;
                    break;
                case RelativeHorizontalSize.RightMargin:
                case RelativeHorizontalSize.InnerMargin: // In this simplistic algorithm inside margin is a right margin.
                    newWidth = ps.RightMargin * percent;
                    break;
                default:
                    // All others are not valid.
                    break;
            }

            mBoundsForRelativeSize = new SizeF((float)newWidth, mBoundsForRelativeSize.Height);

            if (MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                // Get left and right effect extent.
                double effectExtentLeft = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentLeft));
                double effectExtentRight = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentRight));

                // Effects extends are absolute and their values does not depend on relative width.
                if (!MathUtil.IsZero(effectExtentLeft) || !MathUtil.IsZero(effectExtentRight))
                {
                    // In case of 3D effects size might be negative, it seems min size is acceptable here.
                    newWidth = System.Math.Max(ShapeSizeValidationHelper.MinShapeSize, newWidth - effectExtentLeft - effectExtentRight);
                }
            }

            SetPercentWidth(System.Math.Abs(newWidth));
        }

        /// <summary>
        /// Check that width of the <see cref="ShapeBase"/> object does not need to be updated.
        /// </summary>
        /// <returns>"True" when need to skip width update.</returns>
        internal abstract bool IsPercentWidthInapplicable { get; }

        private void UpdateHeightFromPercent(PageSetup ps)
        {
            object objHeightPercent = HeightPercent;
            if (objHeightPercent == null)
                return;

            double percent = (int)objHeightPercent / 1000.0;

            // Height cannot be negative or zero.
            if (percent <= 0)
                return;

            double newHeight = Height;
            switch (RelativeVerticalSize)
            {
                case RelativeVerticalSize.Margin:
                    newHeight = ps.ContentHeight * percent;
                    break;
                case RelativeVerticalSize.Page:
                    newHeight = ps.PageHeight * percent;
                    break;
                case RelativeVerticalSize.TopMargin:
                case RelativeVerticalSize.OuterMargin:    // For some reason MS Word calculates outside and inside margins as the top margin.
                case RelativeVerticalSize.InnerMargin:
                    newHeight = ps.TopMargin * percent;
                    break;
                case RelativeVerticalSize.BottomMargin:
                    newHeight = ps.BottomMargin * percent;
                    break;
                default:
                    // All others are not valid.
                    break;
            }

            mBoundsForRelativeSize = new SizeF(mBoundsForRelativeSize.Width, (float)newHeight);

            if (MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                // Get left and right effect extent.
                double effectExtentTop = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentTop));
                double effectExtentBottom = ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentBottom));

                // ItEffects extends are absolute and their values does not depend on relative width.
                if (!MathUtil.IsZero(effectExtentTop) || !MathUtil.IsZero(effectExtentBottom))
                {
                    // In case of 3D effects size might be negative, it seems min size is acceptable here.
                    newHeight = System.Math.Max(ShapeSizeValidationHelper.MinShapeSize, newHeight - effectExtentTop - effectExtentBottom);
                }
            }

            SetPercentHeight(System.Math.Abs(newHeight));
        }

        private void SetPercentWidth(double value)
        {
            // When angle is between 45 and 135 degrees or between 225 and 315 degrees, MS Word switches relative dimensions.
            if (SwitchPercentDimensions)
               SetHeightSafe(value);
            else
               SetWidthSafe(value);
        }

        private void SetPercentHeight(double value)
        {
            // When angle is between 45 and 135 degrees or between 225 and 315 degrees, MS Word switches relative dimensions.
            if (SwitchPercentDimensions)
                SetWidthSafe(value);
            else
                SetHeightSafe(value);
        }

        private void SetWidthLockAspectRatioSensitive(double value, bool isThrow)
        {
            double validWidth = ShapeSizeValidationHelper.ValidateDimension(this, value, isThrow, "width");

            // If LockAspectRatio is set, we have to set width and height proportionally.
            if (AspectRatioLocked)
            {
                SizeF calcSize = CalculateNotZeroSize();
                double ratio = validWidth / calcSize.Width;
                SetHeightCore(ShapeSizeValidationHelper.ValidateDimension(this, calcSize.Height * ratio, isThrow, "height"),
                    isThrow);
            }

            SetWidthCore(validWidth, isThrow);
        }

        private void SetHeightLockAspectRatioSensitive(double value, bool isThrow)
        {
            double validHeight = ShapeSizeValidationHelper.ValidateDimension(this, value, isThrow, "height");

            // If LockAspectRatio is set, we have to set width and height proportionally.
            if (AspectRatioLocked)
            {
                SizeF calcSize = CalculateNotZeroSize();
                double ratio = validHeight / calcSize.Height;
                SetWidthCore(ShapeSizeValidationHelper.ValidateDimension(this, calcSize.Width * ratio, isThrow, "width"),
                    isThrow);
            }

            SetHeightCore(validHeight, isThrow);
        }

        private void ResetRelSizeValue(int key)
        {
            Debug.Assert((key == ShapeAttr.HeightPercent) || (key == ShapeAttr.WidthPercent));

            if (ShapePr[key] != null)
                SetShapeAttrInternal(key, 0);
        }

        private void SetWidthCore(double value, bool isThrow)
        {
            GraphicData.SetWidthCore(value, isThrow);
        }

        private void SetHeightCore(double value, bool isThrow)
        {
            GraphicData.SetHeightCore(value, isThrow);
        }

        /// <summary>
        /// Returns true if the current VML shape can be filled with the specified color.
        /// </summary>
        private bool CanBeFilled(DrColor color)
        {
            Debug.Assert(MarkupLanguage == ShapeMarkupLanguage.Vml, "Valid for VML only.");

            return ((color != null) && FillCore.On && (FillCore.FillType != FillTypeCore.Picture) &&
                    (FillCore.FillType != FillTypeCore.Texture));
        }

        /// <summary>
        /// Sets image size for corresponding zero size shape.
        /// </summary>
        internal void ValidateZeroSizeShape()
        {
            // andrnosk: WORDSNET-775 If shape size equals zero but shape has image,
            // we have to set shape size equals image size.

            if (!MathUtil.IsZero(Width) || !MathUtil.IsZero(Height))
                return;

            ImageSizeCore imageSize = GetShapeImageSize();
            if (imageSize != null)
            {
                SetWidthSafe(imageSize.WidthPoints);
                SetHeightSafe(imageSize.HeightPoints);
            }
        }

        private SizeF CalculateNotZeroSize()
        {
            double width = Width;
            double height = Height;

            if (MathUtil.IsZero(width) || MathUtil.IsZero(height))
            {
                ImageSizeCore imageSizeCore = GetShapeImageSize();
                if ((imageSizeCore != null) && (MathUtil.IsZero(width) && MathUtil.IsZero(height)))
                {
                    width = imageSizeCore.Width;
                    height = imageSizeCore.Height;
                }
                else
                {
                    // This seems to be the value that MS Word uses when image shape size is zero.
                    width = DefaultShapeSize;
                    height = DefaultShapeSize;
                }
            }

            return new SizeF((float)width, (float)height);
        }

        /// <summary>
        /// Calculates simplified hash code to catch changes made from the public API.
        /// This method is used to invalidate fallback.
        /// </summary>
        private long CalculateHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                // Get all text from the Dml and use it in hash code computation
                // (fixed WORDSNET-10669, WORDSNET-11322, WORDSNET-11344)
                hash = (hash * 16777619 ^ ToString(SaveFormat.Text).GetHashCode());
                // We have to add to hash code computation all fields which can be modified through the public API.
                // Currently not all of fields are added. The other will be added by request.
                hash = (hash * 16777619 ^ Name.GetHashCode());
                hash = (hash * 16777619 ^ HRef.GetHashCode());
                hash = (hash * 16777619 ^ Target.GetHashCode());
                hash = (hash * 16777619 ^ ScreenTip.GetHashCode());
                hash = (hash * 16777619 ^ AlternativeText.GetHashCode());
                hash = (hash * 16777619 ^ ZOrder.GetHashCode());
                hash = (hash * 16777619 ^ Width.GetHashCode());
                hash = (hash * 16777619 ^ Height.GetHashCode());

                hash = (hash * 16777619 ^ AnchorLocked.GetHashCode());
                hash = (hash * 16777619 ^ AllowOverlap.GetHashCode());
                hash = (hash * 16777619 ^ BehindText.GetHashCode());

                hash = (hash * 16777619 ^ Left.GetHashCode());
                hash = (hash * 16777619 ^ Top.GetHashCode());
                hash = (hash * 16777619 ^ Right.GetHashCode());
                hash = (hash * 16777619 ^ Bottom.GetHashCode());

                hash = (hash * 16777619 ^ DistanceTop.GetHashCode());
                hash = (hash * 16777619 ^ DistanceBottom.GetHashCode());
                hash = (hash * 16777619 ^ DistanceLeft.GetHashCode());
                hash = (hash * 16777619 ^ DistanceRight.GetHashCode());

                hash = (hash * 16777619 ^ ((int)WrapType).GetHashCode());//casting for Java that can't get hashcode of enum.

                return hash;
            }
        }

        /// <summary>
        /// Convert <see cref="PathPoint"/> array to <see cref="PointF"/> array.
        /// </summary>
        private static PointF[] ConvertPathPointToPointF(PathPoint[] pathPoints)
        {
            if ((pathPoints != null) && (pathPoints.Length > 0))
            {
                PointF[] pointF = new PointF[pathPoints.Length];
                for (int i = 0; i < pathPoints.Length; i++)
                {
                    PathPoint pathPoint = pathPoints[i];
                    pointF[i] = new PointF(pathPoint.X.Value, pathPoint.Y.Value);
                }
                return pointF;
            }
            return null;
        }

        /// <summary>
        /// Returns bounding rectangle for shape Path.
        /// </summary>
        private RectangleF GetPathBoundingRect()
        {
            PathPoint[] pathPoints = (PathPoint[])ShapePr.GetDirectAttr(ShapeAttr.GeometryVertices);

            PointF[] pointF = ConvertPathPointToPointF(pathPoints);

            if (pointF == null)
                return RectangleF.Empty;

            return GeometryUtil.GetBounds(pointF);
        }

        /// <summary>
        /// Gets the bounding box with applied effectExtents.
        /// </summary>
        private RectangleF GetsBoundsWithEffects()
        {
            float addToLeft = GetExtentEffectValue(ShapeAttr.DmlEffectExtentLeft);
            float addToRight = GetExtentEffectValue(ShapeAttr.DmlEffectExtentRight);
            float addToTop = GetExtentEffectValue(ShapeAttr.DmlEffectExtentTop);
            float addToBottom = GetExtentEffectValue(ShapeAttr.DmlEffectExtentBottom);

            RectangleF sizeBox = GetsSizeBoxForBounds(Rotation);

            return new RectangleF(
                sizeBox.X - addToLeft,
                sizeBox.Y - addToTop,
                sizeBox.Width + addToLeft + addToRight,
                sizeBox.Height + addToTop + addToBottom);
        }

        /// <summary>
        /// Gets the size box for bounding box calculation.
        /// </summary>
        internal RectangleF GetsSizeBoxForBounds(double rotation)
        {
            // WORDSNET-16661, WORDSNET-17515 MS Word uses height and width values
            // that are rounded to twips (see TestInlineDmlShapeWidth).
            float height = GetIntegerNumberOfTwips((float)Height);
            float width = GetIntegerNumberOfTwips((float)Width);

            RectangleF sizeBox = new RectangleF(0, 0, width, height);

            // FOSS

            return sizeBox;
        }

        /// <summary>
        /// Gets integer number of the twips in the specified value converts into target measurement units.
        /// </summary>
        /// <param name="value">The specified value</param>
        /// <returns>Integer number of the twips converted into target measurement units</returns>
        private float GetIntegerNumberOfTwips(float value)
        {
            if (!IsTopLevel || (DmlNode == null))
                return value;

            // Convert to EMU for greater accuracy.
            float valueInEmus = ConvertUtilCore.PointToEmu(value);
            double result = System.Math.Floor(valueInEmus / ConvertUtilCore.EmusPerTwip) * ConvertUtilCore.EmusPerTwip;
            return (float)ConvertUtilCore.EmuToPoint(result);
        }

        /// <summary>
        /// Gets the effectExtent value and checks it.
        /// </summary>
        /// <param name="attr">The specified shape attribute key</param>
        /// <returns>The checked effectExtent value</returns>
        private float GetExtentEffectValue(int attr)
        {
            float value = (float)ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(attr));

            if (IsInline)
                return value;

            // If the shape is not inline, is placed in a table cell, wrapType is not TopBottom and compatibility version
            // is less the Word2013, the bottom value is ignored (see TestJira13608NestingInAlignedCell).
            if (attr == ShapeAttr.DmlEffectExtentBottom)
            {
                bool isTopLevelCell = (ParentParagraph != null)
                                        && (ParentParagraph.ParentNode != null) &&
                                        (ParentParagraph.ParentNode.NodeType == NodeType.Cell);
                bool isWrapTopBottom = (WrapType == WrapType.TopBottom);
                bool compatVersion = (Document.DocPr.CompatibilityOptions.MswVersion != MsWordVersionCore.Unspecified)
                                        && (Document.DocPr.CompatibilityOptions.MswVersion < MsWordVersionCore.Word2013);
                value = (compatVersion && !isWrapTopBottom && isTopLevelCell) ? 0 : value;
            }

            // WORDSNET-9014 If shape is not inline, and value is less then 635 EMU (1 Twip) MS Word uses the zero value.
            // Checked for top and bottom values. Use the same logic for the left and right values.
            // This change in the right and left values does not affect the existing tests.
            return (value < ConvertUtilCore.PointsPerTwip) ? 0 : value;
        }

        /// <summary>
        /// Sets gradient fill in accordance with a specified style and variant.
        /// </summary>
        private void SetVmlGradientFill(GradientStyle style, GradientVariant variant)
        {
            // Clear gradient colors that might be in the current fill.
            ShapePr.Remove(ShapeAttr.FillShadeColors);
            // Clear filled flag.
            ShapePr.Remove(ShapeAttr.Filled);

            // Path gradient
            if ((style == GradientStyle.FromCenter) || (style == GradientStyle.FromCorner))
            {
                VmlFill.SetFocusPositions(ShapePr, style, variant);
                ShapePr.SetAttrIfNotDefault(
                    ShapeAttr.FillType,
                    (style == GradientStyle.FromCorner) ? FillTypeCore.ShadeCenter : FillTypeCore.ShadeShape);
            }
            else // Linear gradient
            {
                ShapePr.SetAttrIfNotDefault(
                    ShapeAttr.FillAngle,
                    ConvertUtilCore.DoubleToFixed(VmlFill.GetLinearGradientAngle(style)));
                ShapePr.SetAttrIfNotDefault(ShapeAttr.FillType, FillTypeCore.ShadeScale);
            }

            // Need to remove the attribute because if the new value equals to default, it will be not set by
            // SetAttrIfNotDefault and the old value will be preserved.
            ShapePr.Remove(ShapeAttr.FillFocus);
            ShapePr.SetAttrIfNotDefault(ShapeAttr.FillFocus, VmlFill.GetGradientFocus(style, variant));
        }

        /// <summary>
        /// Returns <see cref="DmlShapeEffectsCollection"/> object of the shape.
        /// </summary>
        /// <param name="isCreate">Indicates whether to create collection automatically, if it doesn't exist.</param>
        private DmlShapeEffectsCollection GetDmlShapeEffectsCollection(bool isCreate)
        {
            if (DmlShape == null)
                throw new InvalidOperationException(InvalidAction);

            if (DmlShape.Effects == null && isCreate)
                DmlShape.Effects = new DmlShapeEffectsCollection();

            return DmlShape.Effects;
        }

        /// <summary>
        /// Returns <see cref="DmlShapeGlowEffect"/> object of the shape.
        /// </summary>
        /// <param name="isCreate">Indicates whether to create glow effect automatically, if it doesn't exist.</param>
        private DmlShapeGlowEffect GetDmlShapeGlowEffect(bool isCreate)
        {
            if (DmlShape == null)
                throw new InvalidOperationException(InvalidAction);

            return (DmlShapeGlowEffect)GetDmlShapeEffect(DmlShapeEffectType.Glow, isCreate);
        }

        /// <summary>
        /// Returns <see cref="DmlShapeReflectionEffect"/> object of the shape.
        /// </summary>
        /// <param name="isCreate">Indicates whether to create reflection effect automatically, if it doesn't exist.</param>
        private DmlShapeReflectionEffect GetDmlShapeReflectionEffect(bool isCreate)
        {
            if (DmlShape == null)
                throw new InvalidOperationException(InvalidAction);

            return (DmlShapeReflectionEffect)GetDmlShapeEffect(DmlShapeEffectType.Reflection, isCreate);
        }

        /// <summary>
        /// Returns <see cref="DmlShapeSoftEdgeEffect"/> object of the shape.
        /// </summary>
        /// <param name="isCreate">Indicates whether to create soft edge effect automatically, if it doesn't exist.</param>
        private DmlShapeSoftEdgeEffect GetDmlShapeSoftEdgeEffect(bool isCreate)
        {
            if (DmlShape == null)
                throw new InvalidOperationException(InvalidAction);

            return (DmlShapeSoftEdgeEffect)GetDmlShapeEffect(DmlShapeEffectType.SoftEdges, isCreate);
        }

        /// <summary>
        /// Returns <see cref="DmlShapeEffect"/> object of a specified type.
        /// </summary>
        /// <param name="type">The type of the effect to get.</param>
        /// <param name="isCreate">Indicates whether to create effect automatically, if it doesn't exist.</param>
        private DmlShapeEffect GetDmlShapeEffect(DmlShapeEffectType type, bool isCreate)
        {
            DmlShapeEffectsCollection effects = GetDmlShapeEffectsCollection(isCreate);
            if (effects == null)
                return null;

            DmlShapeEffect effect = effects[type];
            if ((effect == null) && isCreate)
            {
                effect = DmlShapeEffect.CreateEffect(type);
                effects.AddEffect(effect);
            }

            return effect;
        }

        /// <summary>
        /// Gets fill formatting for the shape.
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
        /// Gets shadow formatting for the shape.
        /// </summary>
        public ShadowFormat ShadowFormat
        {
            get
            {
                if (mShadow == null)
                    mShadow = new ShadowFormat(this);

                return mShadow;
            }
        }

        /// <summary>
        /// Gets glow formatting for the shape.
        /// </summary>
        public GlowFormat Glow
        {
            get
            {
                if (mGlowFormat == null)
                    mGlowFormat = new GlowFormat(this);

                return mGlowFormat;
            }
        }

        /// <summary>
        /// Gets reflection formatting for the shape.
        /// </summary>
        public ReflectionFormat Reflection
        {
            get
            {
                if (mReflectionFormat == null)
                    mReflectionFormat = new ReflectionFormat(this);

                return mReflectionFormat;
            }
        }

        /// <summary>
        /// Gets soft edge formatting for the shape.
        /// </summary>
        public SoftEdgeFormat SoftEdge
        {
            get
            {
                if (mSoftEdgeFormat == null)
                    mSoftEdgeFormat = new SoftEdgeFormat(this);

                return mSoftEdgeFormat;
            }
        }

        /// <summary>
        /// Defines the text displayed when the mouse pointer moves over the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string ScreenTip
        {
            get { return (string)FetchShapeAttrInternal(ShapeAttr.ScreenTip); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetShapeAttrInternal(ShapeAttr.ScreenTip, value);
            }
        }

        /// <summary>
        /// Gets or sets the full hyperlink address for a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        ///
        /// <p>Below are examples of valid values for this property:</p>
        /// <p>Full URI: <c>https://www.aspose.com/</c>.</p>
        /// <p>Full file name: <c>C:\\My Documents\\SalesReport.doc</c>.</p>
        /// <para>Relative URI: <c>../../../resource.txt</c></para>
        /// <p>Relative file name: <c>..\\My Documents\\SalesReport.doc</c>.</p>
        /// <p>Bookmark within another document: <c>https://www.aspose.com/Products/Default.aspx#Suites</c></p>
        /// <p>Bookmark within this document: <c>#BookmakName</c>.</p>
        /// </remarks>
        public string HRef
        {
            get { return (string)FetchShapeAttrInternal(ShapeAttr.HyperlinkAddress); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                GraphicData.SetHRef(value);
            }
        }

        /// <summary>
        /// Gets or sets the target frame for the shape hyperlink.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string Target
        {
            get { return (string)FetchShapeAttrInternal(ShapeAttr.HyperlinkTarget); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetShapeAttrInternal(ShapeAttr.HyperlinkTarget, value);
            }
        }

        /// <summary>
        /// Defines alternative text to be displayed instead of a graphic.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string AlternativeText
        {
            get
            {
                string altText = GraphicData.AlternativeText;
                return StringUtil.HasChars(altText) ? altText : string.Empty;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                GraphicData.AlternativeText = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag that specifies whether the shape is decorative in the document.
        /// </summary>
        /// <remarks>
        /// Note that shape having not empty <see cref="ShapeBase.AlternativeText"/> cannot be decorative.
        /// </remarks>
        public bool IsDecorative
        {
            get { return GraphicData.Decorative; }
            set { GraphicData.Decorative = value; }
        }

        /// <summary>
        /// Gets or sets the title (caption) of the current shape object.
        /// </summary>
        /// <remarks>
        /// <para>Default is empty string.</para>
        /// <para>Cannot be <c>null</c>, but can be an empty string.</para>
        /// </remarks>
        public string Title
        {
            get
            {
                string title = GraphicData.Title;
                return StringUtil.HasChars(title) ? title : string.Empty;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                GraphicData.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional shape name.
        /// </summary>
        /// <remarks>
        /// <para>Default is empty string.</para>
        /// <para>Cannot be <c>null</c>, but can be an empty string.</para>
        /// </remarks>
        public string Name
        {
            get
            {
                string name = GraphicData.Name;
                return StringUtil.HasChars(name) ? name : string.Empty;
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                GraphicData.Name = value;
            }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsInsertRevision"]/*'/>
        public bool IsInsertRevision
        {
            get { return InlineHelper.IsInsertRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsDeleteRevision"]/*'/>
        public bool IsDeleteRevision
        {
            get { return InlineHelper.IsDeleteRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveFromRevision"]/*'/>
        public bool IsMoveFromRevision
        {
            get { return InlineHelper.IsMoveFromRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveToRevision"]/*'/>
        public bool IsMoveToRevision
        {
            get { return InlineHelper.IsMoveToRevision(this); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is not a child of a group shape.
        /// </summary>
        public bool IsTopLevel
        {
            // alexnosk: DmlChart is not a GroupShape, but still can contain child shapes.
            // So add one more condition for parent of shape type.
            get
            {
                return (ParentNode == null) ||
                    ((ParentNode.NodeType != NodeType.GroupShape) && (ParentNode.NodeType != NodeType.Shape));
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this is a group shape.
        /// </summary>
        public bool IsGroup
        {
            get { return (ShapeType == ShapeType.Group); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is an image shape.
        /// </summary>
        public bool IsImage
        {
            get { return (ShapeType == ShapeType.Image); }
        }

        /// <summary>
        /// Return <c>true</c> is this shape is a TextBox shape.
        /// </summary>
        internal bool IsTextBox
        {
            get { return ShapeType == ShapeType.TextBox; }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is a horizontal rule.
        /// </summary>
        public bool IsHorizontalRule
        {
            get { return GraphicData.IsHorizontalRule; }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is a WordArt object.
        /// </summary>
        /// <remarks>
        /// Works till 2007 compatibility mode.
        /// In 2010 and higher compatibility mode WordArt is just a TextBox with fancy fonts.
        /// </remarks>
        public bool IsWordArt
        {
            get { return GraphicData.IsWordArt; }
        }

        /// <summary>
        /// Returns <c>true</c> if the shape type allows the shape to have an image.
        /// </summary>
        /// <remarks>
        /// <para>Although Microsoft Word has a special shape type for images, it appears that in Microsoft Word documents any shape
        /// except a group shape can have an image, therefore this property returns <c>true</c> for all shapes except <see cref="GroupShape"/>.</para>
        /// </remarks>
        public bool CanHaveImage
        {
            get
            {
                // WORDSNET-19771 We used to think that only Image and Ole shapes can have an image, but it appears
                // more shape types, for example rectangle can have an image. So I changed to include all shape types
                // and exclude only well known ones that cannot have an image.
                switch (ShapeType)
                {
                    case ShapeType.Group:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// Specifies whether the shape's anchor is locked.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// <p>Has effect only for top level shapes.</p>
        ///
        /// <p>This property affects behavior of the shape's anchor in Microsoft Word.
        /// When the anchor is not locked, moving the shape in Microsoft Word can move
        /// the shape's anchor too.</p>
        /// </remarks>
        public bool AnchorLocked
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.AnchorLocked); }
            set { SetShapeAttrInternal(ShapeAttr.AnchorLocked, value); }
        }

        /// <summary>
        /// Specifies whether the shape's aspect ratio is locked.
        /// </summary>
        /// <remarks>
        /// <p>The default value depends on the <see cref="Drawing.ShapeType"/>, for the <see cref="Drawing.ShapeType.Image"/> it is <c>true</c>
        /// but for the other shape types it is <c>false</c>.</p>
        /// <p>Has effect for top level shapes only.</p>
        /// </remarks>
        public bool AspectRatioLocked
        {
            get { return GraphicData.AspectRatioLocked; }
            set { GraphicData.AspectRatioLocked = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether this shape can overlap other shapes.
        /// </summary>
        /// <remarks>
        /// <para>This property affects behavior of the shape in Microsoft Word.
        /// Aspose.Words ignores the value of this property.</para>
        /// <para>This property is applicable only to top level shapes.</para>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        public bool AllowOverlap
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.AllowOverlap); }
            set { SetShapeAttrInternal(ShapeAttr.AllowOverlap, value); }
        }

        /// <summary>
        /// Specifies whether the shape is below or above text.
        /// </summary>
        /// <remarks>
        /// <p>Has effect only for top level shapes.</p>
        /// <p>The default value is <c>false</c>.</p>
        ///
        /// <seealso cref="ZOrder"/>
        /// </remarks>
        public bool BehindText
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.BehindText); }
            set { SetShapeAttrInternal(ShapeAttr.BehindText, value); }
        }

        /// <summary>
        /// A quick way to determine if this shape is positioned inline with text.
        /// </summary>
        /// <remarks>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public bool IsInline
        {
            get { return (WrapType == WrapType.Inline); }
        }

        /// <summary>
        /// Gets or sets the position of the left edge of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="LocationUnits"]/*'/>
        /// <p>The default value is 0.</p>
        /// <p>Has effect only for floating shapes.</p>
        /// </remarks>
        public double Left
        {
            get { return (double)FetchShapeAttrInternal(ShapeAttr.Left); }
            set { SetShapeAttrInternal(ShapeAttr.Left, value); }
        }

        /// <summary>
        /// Gets or sets the position of the top edge of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="LocationUnits"]/*'/>
        /// <p>The default value is 0.</p>
        /// <p>Has effect only for floating shapes.</p>
        /// </remarks>
        public double Top
        {
            get { return (double)FetchShapeAttrInternal(ShapeAttr.Top); }
            set { SetShapeAttrInternal(ShapeAttr.Top, value); }
        }

        /// <summary>
        /// Gets the position of the right edge of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="LocationUnits"]/*'/>
        /// </remarks>
        public double Right
        {
            get { return Left + Width; }
        }

        /// <summary>
        /// Gets the position of the bottom edge of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="LocationUnits"]/*'/>
        /// </remarks>
        public double Bottom
        {
            get { return Top + Height; }
        }

        /// <summary>
        /// Gets or sets the width of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="SizeUnits"]/*'/>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public double Width
        {
            get { return (double)FetchShapeAttrInternal(ShapeAttr.Width); }
            set
            {
                SetWidthLockAspectRatioSensitive(value, true);
                ResetRelSizeValue(ShapeAttr.WidthPercent);
            }
        }

        /// <summary>
        /// Gets or sets the height of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="SizeUnits"]/*'/>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public double Height
        {
            get { return (double)FetchShapeAttrInternal(ShapeAttr.Height); }
            set
            {
                SetHeightLockAspectRatioSensitive(value, true);
                // WORDSNET-16362 MSW automation resets relative height while absolute height value changing.
                ResetRelSizeValue(ShapeAttr.HeightPercent);
            }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the document text and the top edge of the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public double DistanceTop
        {
            get { return ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DistanceTop)); }
            set { SetShapeAttrInternal(ShapeAttr.DistanceTop, ConvertUtilCore.PointToEmu(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the document text and the bottom edge of the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public double DistanceBottom
        {
            get { return ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DistanceBottom)); }
            set { SetShapeAttrInternal(ShapeAttr.DistanceBottom, ConvertUtilCore.PointToEmu(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the document text and the left edge of the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/8 inch.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public double DistanceLeft
        {
            get { return ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DistanceLeft)); }
            set { SetShapeAttrInternal(ShapeAttr.DistanceLeft, ConvertUtilCore.PointToEmu(value)); }
        }

        /// <summary>
        /// Returns or sets the distance (in points) between the document text and the right edge of the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1/8 inch.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public double DistanceRight
        {
            get { return ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(ShapeAttr.DistanceRight)); }
            set { SetShapeAttrInternal(ShapeAttr.DistanceRight, ConvertUtilCore.PointToEmu(value)); }
        }

        /// <summary>
        /// Defines the angle (in degrees) that a shape is rotated.
        /// Positive value corresponds to clockwise rotation angle.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public double Rotation
        {
            get { return GraphicData.Rotation; }
            set { GraphicData.Rotation = value; }
        }

        /// <summary>
        /// Determines the display order of overlapping shapes.
        /// </summary>
        /// <remarks>
        /// <p>Has effect only for top level shapes.</p>
        /// <p>The default value is 0.</p>
        ///
        /// <p>The number represents the stacking precedence. A shape with a higher number will be displayed
        /// as if it were overlapping (in "front" of) a shape with a lower number. </p>
        ///
        /// <p>The order of overlapping shapes is independent for shapes in the header and in the main
        /// text of the document.</p>
        ///
        /// <p>The display order of child shapes in a group shape is determined by their order
        /// inside the group shape.</p>
        ///
        /// <seealso cref="BehindText"/>
        /// </remarks>
        public int ZOrder
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ZOrder); }
            set { SetShapeAttrInternal(ShapeAttr.ZOrder, value); }
        }

        /// <summary>
        /// Returns the immediate parent paragraph.
        /// </summary>
        /// <remarks>For child shapes of a group shape and child shapes of an Office Math object always returns <c>null</c>.</remarks>
        public Paragraph ParentParagraph
        {
            get { return ParentNode as Paragraph; }
        }

        /// <summary>
        /// Gets or sets the location and size of the containing block of the shape.
        /// </summary>
        /// <remarks>
        /// Ignores aspect ratio lock upon setting.
        /// </remarks>
        /// <remarks>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="LocationUnits"]/*'/>
        /// </remarks>
        [ComVisible(false)] // RK RectangleF is non COM visible value type therefore we cannot expose this.
        public RectangleF Bounds
        {
            get { return new RectangleF((float)Left, (float)Top, (float)Width, (float)Height); }
            set
            {
                Left = value.Left;
                Top = value.Top;
                SetWidthCore(value.Width, false);
                SetHeightCore(value.Height, false);
            }
        }

        /// <summary>
        /// Gets the location and size of the containing block of the shape in points, relative to the anchor of the topmost shape.
        /// </summary>
        /// <remarks>
        /// The returned bounds do not include the rotation of this shape or the rotation of the parent group shape, if any.
        /// </remarks>
        [ComVisible(false)] // RK RectangleF is non COM visible value type therefore we cannot expose this.
        public RectangleF BoundsInPoints
        {
            get
            {
                // Technically speaking, bounds of the shape are coordinates in the parent's coordinate space.
                return ConvertLocalToTopmostAnchor(Bounds);
            }
        }

        /// <summary>
        /// Gets final extent that this shape object has after applying drawing effects.
        /// Value is measured in points.
        /// </summary>
        public RectangleF BoundsWithEffects
        {
            get { return GetsBoundsWithEffects(); }
        }

        /// <summary>
        /// Shows if effect extent is empty (all values are zero).
        /// </summary>
        internal bool IsEffectExtentEmpty
        {
            get
            {
                return ((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentLeft) == 0)
                       && ((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentRight) == 0)
                       && ((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentTop) == 0)
                       && ((int)FetchShapeAttrInternal(ShapeAttr.DmlEffectExtentBottom) == 0);
            }
        }

        /// <summary>
        /// Adds to the source rectangle values of the effect extent and returns the final rectangle.
        /// </summary>
        public RectangleF AdjustWithEffects(RectangleF source)
        {
            float left = source.Left - FetchEmuShapeAttrInPoints(ShapeAttr.DmlEffectExtentLeft);
            float right = source.Right + FetchEmuShapeAttrInPoints(ShapeAttr.DmlEffectExtentRight);
            float top = source.Top - FetchEmuShapeAttrInPoints(ShapeAttr.DmlEffectExtentTop);
            float bottom = source.Bottom + FetchEmuShapeAttrInPoints(ShapeAttr.DmlEffectExtentBottom);
            return RectangleF.FromLTRB(left, top, right, bottom);
        }

        private float FetchEmuShapeAttrInPoints(int attr)
        {
            return (float)ConvertUtilCore.EmuToPoint((int)FetchShapeAttrInternal(attr));
        }

        /// <summary>
        /// Gets the shape type.
        /// </summary>
        public ShapeType ShapeType
        {
            get { return GraphicData.ShapeType; }
        }

        /// <summary>
        /// Gets MarkupLanguage used for this graphic object.
        /// </summary>
        public ShapeMarkupLanguage MarkupLanguage
        {
            get { return mMarkupLanguage; }
        }

        /// <summary>
        /// Gets the size of the shape in points.
        /// </summary>
        ///
        /// <javaMember type="method" name="java.awt.geom.Point2D.Float com.aspose.words.ShapeBase.getSizeInPoints()">
        /// <summary>
        /// Gets the size of the shape in points.
        /// </summary>
        /// <remarks>
        /// <p>Point2D.Float is used as return type because we need in float dimension values here.
        /// One should to assume that Point2D's <i>x == width</i> and <i>y == height</i>.</p>
        /// </remarks>
        /// </javaMember>
        public SizeF SizeInPoints
        {
            get { return BoundsInPoints.Size; }
        }

        /// <summary>
        /// Switches the orientation of a shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.FlipOrientation.None"/>.</p>
        /// </remarks>
        public FlipOrientation FlipOrientation
        {
            get { return GraphicData.FlipOrientation; }
            set { GraphicData.FlipOrientation = value; }
        }

        /// <summary>
        /// Gets the orientation of parent (group) shape.
        /// </summary>
        /// <remarks>
        /// <p>If there is no parent shape, returns <see cref="Aspose.Words.Drawing.FlipOrientation.None"/>.</p>
        /// </remarks>
        internal FlipOrientation ParentFlipOrientation
        {
            get
            {
                GroupShape parentGroup = GetAncestor(NodeType.GroupShape) as GroupShape;
                return parentGroup == null ? FlipOrientation.None : parentGroup.FlipOrientation;
            }
        }

        /// <summary>
        /// Specifies relative to what the shape is positioned horizontally.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeRelativeHorizontalPosition.Column"/>.</p>
        /// <p>Has effect only for top level floating shapes.</p>
        /// </remarks>
        public RelativeHorizontalPosition RelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)FetchShapeAttrInternal(ShapeAttr.RelativeHorizontalPosition); }
            set { SetShapeAttrInternal(ShapeAttr.RelativeHorizontalPosition, value); }
        }

        /// <summary>
        /// Specifies relative to what the shape is positioned vertically.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeRelativeVerticalPosition.Paragraph"/>.</p>
        /// <p>Has effect only for top level floating shapes.</p>
        /// </remarks>
        public RelativeVerticalPosition RelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)FetchShapeAttrInternal(ShapeAttr.RelativeVerticalPosition); }
            set { SetShapeAttrInternal(ShapeAttr.RelativeVerticalPosition, value); }
        }

        /// <summary>
        /// Gets or sets the value of shape's relative size in horizontal direction.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.RelativeHorizontalSize"/>.</p>
        /// <p>Has effect only if <see cref="ShapeBase.WidthRelative"/> is set.</p>
        /// </remarks>
        public RelativeHorizontalSize RelativeHorizontalSize
        {
            get
            {
                return (RelativeHorizontalSize)FetchShapeAttrInternal(ShapeAttr.RelativeWidth);
            }
            set
            {
                SetShapeAttrInternal(ShapeAttr.RelativeWidth, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of shape's relative size in vertical direction.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.RelativeVerticalSize.Margin"/>.</p>
        /// <p>Has effect only if <see cref="ShapeBase.HeightRelative"/> is set.</p>
        /// </remarks>
        public RelativeVerticalSize RelativeVerticalSize
        {
            get
            {
                return (RelativeVerticalSize)FetchShapeAttrInternal(ShapeAttr.RelativeHeight);
            }
            set
            {
                SetShapeAttrInternal(ShapeAttr.RelativeHeight, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents shape's relative left position in percent.
        /// </summary>
        public float LeftRelative
        {
            get
            {
                return ShapePr[ShapeAttr.LeftPercent] == null ? 0 : ((float)(int)LeftPercent / 10);
            }
            set
            {
                if (value == 0)
                    ShapePr.Remove(ShapeAttr.LeftPercent);
                else
                    SetShapeAttrInternal(ShapeAttr.LeftPercent, (int)System.Math.Truncate(value * 10));
            }
        }

        /// <summary>
        /// Gets or sets the value that represents shape's relative top position in percent.
        /// </summary>
        public float TopRelative
        {
            get
            {
                return ShapePr[ShapeAttr.TopPercent] == null ? 0 : ((float)(int)TopPercent / 10);
            }
            set
            {
                if (value == 0)
                    ShapePr.Remove(ShapeAttr.TopPercent);
                else
                    SetShapeAttrInternal(ShapeAttr.TopPercent, (int)System.Math.Truncate(value * 10));
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the percentage of shape's relative height.
        /// </summary>
        public float HeightRelative
        {
            get
            {
                return ShapePr[ShapeAttr.HeightPercent] == null ? 0 : ((float)(int)HeightPercent / 10);
            }
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("HeightRelative cannot be set to a negative value.");

                if (value == 0)
                {
                    // MS Word VBA resets RelativeVerticalPosition when setting HeightRelative value to 0.
                    ShapePr.Remove(ShapeAttr.RelativeHeight);
                    ShapePr.Remove(ShapeAttr.HeightPercent);
                }
                else
                {
                    SetShapeAttrInternal(ShapeAttr.HeightPercent, (int)System.Math.Truncate(value * 10));
                    // MS Word VBA automatically sets RelativeVerticalSize to Margin value when setting HeightRelative to a valid value.
                    if (ShapePr[ShapeAttr.RelativeHeight] == null)
                        RelativeVerticalSize = RelativeVerticalSize.Margin;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the percentage of shape's relative width.
        /// </summary>
        public float WidthRelative
        {
            get
            {
                return ShapePr[ShapeAttr.WidthPercent] == null ? 0 : ((float)(int)WidthPercent / 10);
            }
            set
            {
                if (value < 0)
                    throw new InvalidOperationException("WidthRelative cannot be set to a negative value.");

                if (value == 0)
                {
                    // MS Word VBA resets RelativeHorizontalSize when setting WidthRelative value to 0.
                    ShapePr.Remove(ShapeAttr.RelativeWidth);
                    ShapePr.Remove(ShapeAttr.WidthPercent);
                }
                else
                {
                    SetShapeAttrInternal(ShapeAttr.WidthPercent, (int)System.Math.Truncate(value * 10));
                    // MS Word VBA automatically sets RelativeHorizontalSize to Margin value when setting WidthRelative to a valid value.
                    if (ShapePr[ShapeAttr.RelativeWidth] == null)
                        RelativeHorizontalSize = RelativeHorizontalSize.Margin;
                }
            }
        }

        /// <summary>
        /// Specifies how the shape is positioned horizontally.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeHorizontalAlignment.None"/>.</p>
        /// <p>Has effect only for top level floating shapes.</p>
        /// </remarks>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)FetchShapeAttrInternal(ShapeAttr.HorizontalAlignment); }
            set { SetShapeAttrInternal(ShapeAttr.HorizontalAlignment, value); }
        }

        /// <summary>
        /// Specifies how the shape is positioned vertically.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeVerticalAlignment.None"/>.</p>
        /// <p>Has effect only for top level floating shapes.</p>
        /// </remarks>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)FetchShapeAttrInternal(ShapeAttr.VerticalAlignment); }
            set { SetShapeAttrInternal(ShapeAttr.VerticalAlignment, value); }
        }

        /// <summary>
        /// Defines whether the shape is inline or floating. For floating shapes defines the wrapping mode for text around the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeWrapType.None"/>.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public WrapType WrapType
        {
            get { return (WrapType)FetchShapeAttrInternal(ShapeAttr.WrapType); }
            set { SetShapeAttrInternal(ShapeAttr.WrapType, value); }
        }

        /// <summary>
        /// Specifies how the text is wrapped around the shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="AsposeWrapSide.Both"/>.</p>
        /// <p>Has effect only for top level shapes.</p>
        /// </remarks>
        public WrapSide WrapSide
        {
            get { return (WrapSide)FetchShapeAttrInternal(ShapeAttr.WrapSide); }
            set { SetShapeAttrInternal(ShapeAttr.WrapSide, value); }
        }

        /// <summary>
        /// The coordinates at the top-left corner of the containing block of this shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is (0,0).</p>
        /// </remarks>
        public Point CoordOrigin
        {
            get { return new Point(CoordOriginX, CoordOriginY); }
            set
            {
                CoordOriginX = value.X;
                CoordOriginY = value.Y;
            }
        }

        /// <summary>
        /// The width and height of the coordinate space inside the containing block of this shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is (1000, 1000).</p>
        /// </remarks>
        public Size CoordSize
        {
            get { return new Size(CoordSizeWidth, CoordSizeHeight); }
            set
            {
                // WORDSNET-13775 According to MSW behavior coordsize width and height can equals zero
                // but cannot be less than zero (it is true for VML, DML and for ODT shapes).
                // Zero width or height means this shape looks like vertical or horizontal line.
                if ((value.Width < 0) || (value.Height < 0))
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Local coordinate space size cannot be less than zero.");
                }

                SetCoordSizeSafe(value);
            }
        }

        /// <summary>
        /// Provides access to the font formatting of this object.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFontCache == null)
                    mFontCache = new Font(this, Document);
                return mFontCache;
            }
        }

        /// <summary>
        /// Indicates that shape is a <see cref="SignatureLine"/>.
        /// </summary>
        public bool IsSignatureLine
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.IsSignatureLine); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the shape is displayed inside a table or outside of it.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>true</c>.</p>
        /// <p>Has effect only for top level shapes, the property <see cref="WrapType"/> of which is set to value
        /// other than <see cref="Inline"/>.</p>
        /// </remarks>
        public bool IsLayoutInCell
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.AllowInCell); }
            set { SetShapeAttrInternal(ShapeAttr.AllowInCell, value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the shape is visible.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        public bool Hidden
        {
            get
            {
                return GraphicData.Hidden;
            }
            set
            {
                GraphicData.Hidden = value;
            }
        }

        /// <summary>
        /// Gets IFill object for the shape.
        /// </summary>
        internal IFill FillCore
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if (mFillCore == null)
                        mFillCore = new VmlFill();
                }
                else
                {
                    mFillCore = ((IDmlCommonShapePrSource)DmlNode).Fill;
                }

                mFillCore.Parent = this;
                return mFillCore;
            }
        }

        /// <summary>
        /// Defines locks for a shape.
        /// </summary>
        internal Locks Locks
        {
            get
            {
                if (mLocksCache == null)
                    mLocksCache = new Locks(this);
                return mLocksCache;
            }
        }

        internal int CoordOriginX
        {
            get { return GraphicData.CoordOriginX; }
            set { GraphicData.CoordOriginX = value; }
        }

        internal int CoordOriginY
        {
            get { return GraphicData.CoordOriginY; }
            set { GraphicData.CoordOriginY = value; }
        }

        /// <summary>
        /// Gets the unique shape identifier.
        /// </summary>
        internal int Id
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShapeId); }
            set { SetShapeAttrInternal(ShapeAttr.ShapeId, value); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is an OLE object.
        /// </summary>
        internal bool IsOleObject
        {
            get { return (ShapeType == ShapeType.OleObject); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is an ActiveX control.
        /// </summary>
        internal bool IsOleControl
        {
            get { return (ShapeType == ShapeType.OleControl); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is an OLE object or ActiveX control.
        /// </summary>
        internal bool IsOle
        {
            get { return (IsOleObject || IsOleControl); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is a standard horizontal rule.
        /// </summary>
        internal bool IsStandardHorizontalRule
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.HRStandard); }
        }

        /// <summary>
        /// A low level property used by RTF and DOC writers to find out whether the shape
        /// needs to be enclosed inside a SHAPE field when written as an inline shape.
        /// Returns <c>true</c> if the shape is not a picture, ole object/control, horizontal rule or word art.
        /// </summary>
        internal bool IsNeedDummyWhenInline
        {
            get { return !(IsImage || IsOle || IsHorizontalRule || IsWordArt || IsCustomShape); }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape is a picture bullet.
        ///
        /// This property is important in DOC and RTF importers/exporters and it is automatically
        /// set when a shape is added to the list of picture bullets.
        /// In WML and DOCX this property does not seem to exist.
        /// In the model this property does not make much difference.
        /// </summary>
        internal bool IsPictureBullet
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.PictureBullet); }
            set { SetShapeAttrInternal(ShapeAttr.PictureBullet, value); }
        }

        /// <summary>
        /// Gets the size of the shape in its native units. Top level shapes are in points. Child shapes are in parent coordinate system.
        /// </summary>
        internal SizeF Size
        {
            get { return new SizeF((float)Width, (float)Height); }
        }

        /// <summary>
        /// When not null, the shape is horizontally positioned using percentage relative to some object.
        /// The value is integer 1/10th percent.
        /// </summary>
        internal object LeftPercent
        {
            get { return GetDirectShapeAttrInternal(ShapeAttr.LeftPercent); }
        }

        /// <summary>
        /// When not null, the shape is vertically positioned using percentage relative to some object.
        /// The value is integer 1/10th percent.
        /// </summary>
        internal object TopPercent
        {
            get { return GetDirectShapeAttrInternal(ShapeAttr.TopPercent); }
        }

        /// <summary>
        /// When not null, the shape width is in percentage relative to some object.
        /// The value is integer 1/10th percent.
        /// </summary>
        internal object WidthPercent
        {
            get { return GetDirectShapeAttrInternal(ShapeAttr.WidthPercent); }
        }

        /// <summary>
        /// When not null, the shape height is in percentage relative to some object.
        /// The value is integer 1/10th percent.
        /// </summary>
        internal object HeightPercent
        {
            get { return GetDirectShapeAttrInternal(ShapeAttr.HeightPercent); }
        }

        internal int CoordSizeWidth
        {
            get { return GraphicData.CoordSizeWidth; }
        }

        internal int CoordSizeHeight
        {
            get { return GraphicData.CoordSizeHeight; }
        }

        /// <summary>
        /// Gets the total rotation of this object up to the hierarchy root.
        /// </summary>
        internal double TotalRotation
        {
            get
            {
                double angle = 0;
                Node shapeNode = this;
                do
                {
                    angle += ((ShapeBase)shapeNode).Rotation;
                    shapeNode = shapeNode.ParentNode;
                }
                while ((shapeNode != null) && (shapeNode.NodeType == NodeType.GroupShape));

                return angle;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the shape has a hyperlink.
        /// </summary>
        internal bool HasHyperlink
        {
            get { return StringUtil.HasChars(HRef); }
        }

        /// <summary>
        /// Returns <c>true</c> if shape should be included into HYPERLINK field.
        /// </summary>
        internal bool IsHyperlinkedInline
        {
            get { return IsInline && HasHyperlink && (IsImage || IsOleObject); }
        }

        /// <summary>
        /// Gets the hyperlink part that is before the "#".
        /// </summary>
        internal string HyperlinkAddress
        {
            get { return UriUtil.GetAddress(HRef); }
        }

        /// <summary>
        /// Gets the hyperlink part that is after the "#" excluding the "#".
        /// </summary>
        internal string HyperlinkSubAddress
        {
            get { return UriUtil.GetSubAddress(HRef); }
        }

        /// <summary>
        /// Provides direct access to shape attributes.
        /// </summary>
        internal ShapePr ShapePr
        {
            get { return mShapePr; }
            set { mShapePr = value; }
        }

        /// <summary>
        /// Provides direct access to the font formatting of the shape anchor character.
        /// These attributes only have effect for top level inline shapes.
        /// </summary>
        internal RunPr RunPr
        {
            get { return mRunPr; }
            set { mRunPr = value; }
        }

        /// <summary>
        /// This should actually belong to Shape, but I have it on ShapeBase to avoid casting in some places.
        /// Indicates the type of the connector if this shape is a connector.
        /// </summary>
        internal ConnectorType ConnectorType
        {
            get { return (ConnectorType)FetchShapeAttrInternal(ShapeAttr.ConnectorType); }
            set { SetShapeAttrInternal(ShapeAttr.ConnectorType, value); }
        }

        internal bool IsConnector
        {
            get { return (ConnectorType != ConnectorType.None); }
        }

        /// <summary>
        /// This is a helper property for DOC file only.
        /// </summary>
        internal int Txid
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TextboxTxid); }
            set { SetShapeAttrInternal(ShapeAttr.TextboxTxid, value); }
        }

        /// <summary>
        /// Gets or sets the id of the next shape in the chain of linked textboxes.
        /// This value is correct after loading and it is "corrected" during saving.
        /// Cloning or removing shapes might break chains in the model and document validator just "corrects" this before saving.
        /// </summary>
        internal int TextboxNextShapeId
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TextboxNextShapeId); }
            set { SetShapeAttrInternal(ShapeAttr.TextboxNextShapeId, value); }
        }

        /// <summary>
        /// Gets InkData.
        /// </summary>
        internal byte[] InkData
        {
            get { return (byte[])GetDirectShapeAttrInternal(ShapeAttr.InkData); }
        }

        /// <summary>
        /// Indicates that this Ink is annotation.
        /// </summary>
        internal bool InkAnnotation
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.InkAnnotation); }
        }

        /// <summary>
        /// Gets <see cref="IEmbeddedObject"/> object specifies OLE/ActiveX/OOXML embedded object stored in a document.
        /// </summary>
        internal IEmbeddedObject EmbeddedObject
        {
            get { return (IEmbeddedObject)FetchShapeAttrInternal(ShapeAttr.OleObject); }
        }

        /// <summary>
        /// A low level property used by RTF and DOC writers.
        /// Returns <c>true</c> when the PseudoInline attribute must be set in the output document.
        /// It is some obscure flag. The RTF doc says:
        /// The shape is pseudo-inline, meaning it behaves like an inline image as far as positioning goes,
        /// but has the features of shapes.
        /// </summary>
        internal bool IsPseudoInline
        {
            get { return (IsInline && IsNeedDummyWhenInline); }
        }

        /// <summary>
        /// Shows if the shape is a callout shape.
        /// </summary>
        internal bool IsCalloutShapeType
        {
            get
            {
                switch (ShapeType)
                {
                    case ShapeType.AccentBorderCallout90:
                    case ShapeType.BorderCallout90:
                    case ShapeType.AccentCallout90:
                    case ShapeType.Callout90:
                    case ShapeType.CloudCallout:
                    case ShapeType.QuadArrowCallout:
                    case ShapeType.UpDownArrowCallout:
                    case ShapeType.LeftRightArrowCallout:
                    case ShapeType.DownArrowCallout:
                    case ShapeType.UpArrowCallout:
                    case ShapeType.RightArrowCallout:
                    case ShapeType.LeftArrowCallout:
                    case ShapeType.WedgeEllipseCallout:
                    case ShapeType.WedgeRRectCallout:
                    case ShapeType.WedgeRectCallout:
                    case ShapeType.AccentBorderCallout3:
                    case ShapeType.AccentBorderCallout2:
                    case ShapeType.AccentBorderCallout1:
                    case ShapeType.BorderCallout3:
                    case ShapeType.BorderCallout2:
                    case ShapeType.BorderCallout1:
                    case ShapeType.AccentCallout3:
                    case ShapeType.AccentCallout2:
                    case ShapeType.AccentCallout1:
                    case ShapeType.Callout3:
                    case ShapeType.Callout2:
                    case ShapeType.Callout1:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Shows if the shape is a connector shape.
        /// </summary>
        internal bool IsConnectorShapeType
        {
            get
            {
                switch (ShapeType)
                {
                    case ShapeType.BentConnector2:
                    case ShapeType.BentConnector3:
                    case ShapeType.BentConnector4:
                    case ShapeType.BentConnector5:
                    case ShapeType.CurvedConnector2:
                    case ShapeType.CurvedConnector3:
                    case ShapeType.CurvedConnector4:
                    case ShapeType.CurvedConnector5:
                    case ShapeType.FlowChartConnector:
                    case ShapeType.FlowChartOffpageConnector:
                    case ShapeType.StraightConnector1:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Shows if the shape is a bracket shape.
        /// </summary>
        internal bool IsBracket
        {
            get
            {
                switch (ShapeType)
                {
                    case ShapeType.LeftBracket:
                    case ShapeType.RightBracket:
                    case ShapeType.BracketPair:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Shows if the shape is a brace shape.
        /// </summary>
        internal bool IsBrace
        {
            get
            {
                switch (ShapeType)
                {
                    case ShapeType.LeftBrace:
                    case ShapeType.RightBrace:
                    case ShapeType.BracePair:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Checks if DmlNode is <see cref="DmlShape"/>.
        /// </summary>
        internal bool IsDmlShape
        {
            get
            {
                return (DmlShape != null);
            }
        }

        /// <summary>
        /// Gets the layout flow (i.e. text orientation) of the shape's textbox.
        /// </summary>
        internal LayoutFlow LayoutFlow
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Dml)
                {
                    return (DmlShape != null)
                        ? DmlUtilCore.DmlToTextVerticalType(DmlShape.TextBodyPr.TextOrientation,
                            DmlShape.NormalEastAsianFlow)
                        : LayoutFlow.Horizontal;
                }

                return (LayoutFlow)ShapePr.FetchAttr(ShapeAttr.TextboxLayoutFlow);
            }
        }

        #region Linked textboxes attributes.

        /// <summary>
        /// Integer min value means id is not set.
        /// </summary>
        internal int TextboxId
        {
            get { return mTextboxId; }
            set { mTextboxId = value; }
        }

        /// <summary>
        /// Integer min value means id is not set.
        /// </summary>
        internal int LinkedTextboxId
        {
            get { return mLinkedTextboxId; }
            set { mLinkedTextboxId = value; }
        }

        /// <summary>
        /// Ordinal number of textbox in linked textbox sequence.
        /// </summary>
        internal int LinkedTextboxSeq
        {
            get { return mLinkedTextboxSeq; }
            set { mLinkedTextboxSeq = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if this shape has textbox (including linked).
        /// </summary>
        internal bool HasTextbox
        {
            get
            {
                if ((mTextboxId > 0) || (mLinkedTextboxId > 0))
                    return true;

                Node child = FirstNonAnnotationChild;
                return (child != null) && (child.NodeLevel == NodeLevel.Block);
            }
        }


        #endregion Linked textboxes attributes.

        internal Theme DocumentTheme
        {
            get { return Document.GetThemeInternal(); }
        }

        /// <summary>
        /// Shortcut for AlternateContent.FallBack shape.
        /// </summary>
        internal ShapeBase FallbackShape
        {
            get
            {
                return (FallBack != null) ? FallBack.FirstChild as ShapeBase : null;
            }
        }

        /// <summary>
        /// Indicates that this shape's media can be rendered.
        /// </summary>
        /// <remarks>
        /// WORDSNET-8002 It appears that DrawingML object can contain not only images
        /// but any media file such as QuickTime movie in the problematic file.
        /// Avoid to pass such DrawingML into layout engine.
        /// </remarks>
        internal bool SupportRendering
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Dml && NodeType == NodeType.Shape)
                {
                    ImageData imageData = ((Shape)this).ImageData;
                    if (imageData.HasImageBytes)
                        return !ImageUtil.IsMov(imageData.ImageBytes);
                }

                return true;
            }
        }

        /// <summary>
        /// Indicates whether the shape bounds should be taken into account, when calculating the width of a table cell.
        /// </summary>
        /// <remarks>
        /// The shape sizes in AW and in MS Word for some shapes are different. If these shapes are placed in a table cell,
        /// and the AutoFit property is set for the cell, discrepancies in the shape size can cause incorrect calculation
        /// of the table column width. To avoid this, we use this method, which selects only those shapes whose size is the
        /// same in AW and in MS Word. As discrepancies in shape sizes are eliminated, this property will change.
        /// </remarks>
        /// <returns>"True" if shape bounds are taken into account, otherwise "false"</returns>
        internal bool IsGridCalculationSupported
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return true;

                DmlShapeBase dmlShapeBase = DmlNode as DmlShapeBase;
                return (dmlShapeBase != null) && (dmlShapeBase.Geometry.PresetName == "rect") && IsEffectExtentEmpty;
            }
        }

        /// <summary>
        /// Returns user shape anchor type. Depending on the type,
        /// <see cref="From"/> and <see cref="To"/> properties must be interpreted differently.
        /// </summary>
        internal DmlChartUserShapeAnchorType AnchorType
        {
            get { return mAnchorType; }
            set { mAnchorType = value; }
        }

        /// <summary>
        /// The from element specifies the top left corner of the shape bounding box
        /// in a RTL(right-to-left) implementation.
        /// </summary>
        internal PointF From
        {
            get { return mFrom; }
            set { mFrom = value; }
        }

        /// <summary>
        /// The to element then specifies the bottom right corner of the shape bounding box
        /// in a RTL(right-to-left) implementation and thus the size of the shape.
        /// </summary>
        internal PointF To
        {
            get { return mTo; }
            set { mTo = value; }
        }

        /// <summary>
        /// Dml node that represents graphics data of the Dml shape.
        /// </summary>
        internal DmlNode DmlNode
        {
            get { return mDmlNode; }
            set
            {
                if (value != null)
                    value.SetDrawingML(this);
                mDmlNode = value;
                mGraphicData = mDmlNode;
            }
        }

        /// <summary>
        /// Wrapper for all common public properties for Vml and Dml shapes which have different implementation.
        /// </summary>
        internal Graphic GraphicData
        {
            get
            {
                if (mGraphicData == null)
                    mGraphicData = new VmlNode(this);

                return mGraphicData;
            }
        }

        /// <summary>
        /// True if shape contains properties that conforms to
        /// [MS-ODRAWXML]: Office Drawing Extensions to Office Open XML Structure.
        /// </summary>
        internal bool HasDrawingExtensions
        {
            get
            {
                return ((ShapePr.Contains(ShapeAttr.RelativeWidth)) ||
                        (ShapePr.Contains(ShapeAttr.RelativeHeight)));
            }
        }

        /// <summary>
        /// True if this shape is <see cref="FallbackShape"/>.
        /// </summary>
        internal bool IsFallback
        {
            get { return mIsFallback; }
            set { mIsFallback = value; }
        }

        /// <summary>
        /// Effective <see cref="CoordSize"/> for rendering purposes that has zero dimensions
        /// replaced with bounding rectangle shape path dimensions. If, in turn, it has width
        /// or height equals to zero again, then it is replaced with 1.
        /// </summary>
        internal Size EffectiveCoordSize
        {
            get
            {
                if ((CoordSizeWidth > 0) && (CoordSizeHeight > 0))
                    return CoordSize;

                if (mEffectiveCoordSize.IsEmpty)
                {
                    // Gets the bounding rectangle.
                    RectangleF boundingRect = GetPathBoundingRect();

                    // If there is no Path in this shape or it has zero size in one of
                    // the bounding rectangle dimensions, then assume rectangle is 1x1
                    // to allow Layout work properly.
                    if (boundingRect.IsEmpty)
                        boundingRect = new RectangleF(0, 0, 1, 1);

                    int width = (CoordSizeWidth <= 0) ? (int)boundingRect.Width : CoordSizeWidth;
                    int height = (CoordSizeHeight <= 0) ? (int)boundingRect.Height : CoordSizeHeight;
                    mEffectiveCoordSize = new Size(width, height);
                }

                return mEffectiveCoordSize;
            }
        }

        /// <summary>
        /// Effective <see cref="CoordSizeWidth"/> for rendering purposes that has zero value
        /// replaced with bounding rectangle shape path width. If, in turn, it has zero value,
        /// then returns 1.
        /// </summary>
        internal int EffectiveCoordSizeWidth
        {
            get { return EffectiveCoordSize.Width; }
        }

        /// <summary>
        /// Effective <see cref="CoordSizeHeight"/> for rendering purposes that has zero value
        /// replaced with bounding rectangle shape path height. If, in turn, it has zero value,
        /// then returns 1.
        /// </summary>
        internal int EffectiveCoordSizeHeight
        {
            get { return EffectiveCoordSize.Height; }
        }

        internal DmlShape DmlShape
        {
            get
            {
                if (mDmlShape != null)
                    return mDmlShape;

                mDmlShape = DmlNode as DmlShape;
                return mDmlShape;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if shape can have fill image.
        /// </summary>
        internal bool CanHaveFillImage
        {
            get
            {
                return (ShapeType != ShapeType.Image) &&
                    (ShapeType != ShapeType.CustomShape) &&
                    (ShapeType != ShapeType.NonPrimitive) &&
                    !IsOle;
            }
        }

        internal bool IsWatermark
        {
            get
            {
                if (MarkupLanguage != ShapeMarkupLanguage.Vml)
                    return false;

                if (CanBeTextWatermark || CanBeImageWatermark)
                {
                    HeaderFooter headerFooter = GetStoryAncestor(NodeType.HeaderFooter) as HeaderFooter;
                    return (headerFooter != null) && headerFooter.IsHeader;
                }

                return false;
            }
        }

        internal bool CanBeTextWatermark
        {
            get { return Name.Contains("PowerPlusWaterMarkObject") && IsWordArt; }
        }

        internal bool CanBeImageWatermark
        {
            get { return (Name.Contains("WordPictureWatermark") && IsImage); }
        }

        /// <summary>
        /// Gets bounds for shapes with relative sizes. The value is used only for rendering.
        /// </summary>
        internal SizeF BoundsForRelativeSize
        {
            get { return mBoundsForRelativeSize; }
        }

        internal bool IsInLockedCanvas
        {
            get
            {
                ShapeBase topShape = GetTopLevelParentShape();
                return (topShape.DmlNode != null) && (topShape.DmlNode.DmlNodeType == DmlNodeType.LockedCanvas);
            }
        }

        private bool IsCustomShape
        {
            get { return (ShapeType == ShapeType.CustomShape); }
        }

        /// <summary>
        /// When angle is between 45 and 135 degrees or between 225 and 315 degrees, MS Word switches relative dimensions.
        /// </summary>
        private bool SwitchPercentDimensions
        {
            get
            {
                double rotation = MathUtil.NormalizeAngle(Rotation);
                return ((rotation >= 45 && rotation < 135) || (rotation >= 225 && rotation < 315));
            }
        }

        /// <summary>
        /// Get "Fallback" of the "AlternateContent" if it exists.
        /// </summary>
        private CompositeNode FallBack
        {
            get
            {
                AlternateContent ac = RunPr.AlternateContent;
                return (ac != null) && (ac.FallBack != null) ? ac.FallBack : null;
            }
        }

        /// <summary>
        /// 3" in points - used in case we cannot determine shape size for the image shape.
        /// </summary>
        internal static double DefaultShapeSize = 216;

        /// <summary>
        /// This is the starting shape number for shapes in the main text of a Word document.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MainDrawingPatriarchShapeId = 0x0400;

        /// <summary>
        /// MS Word allocates shape ids in clusters of 1024 shapes, we have to honor this during writing.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int ShapeIdClusterSize = 0x0400;

        /// <summary>
        /// This seems to be the value that MS Word uses when shape size is invalid.
        /// </summary>
        internal const double InvalidShapeSizeDefault = 50.25;

        private ShapePr mShapePr = new ShapePr();

        /// <summary>
        /// Since any shape can become a top-level shape, we have to have run attributes.
        /// </summary>
        private RunPr mRunPr = new RunPr();

        private DmlShape mDmlShape;
        private Locks mLocksCache;
        private Font mFontCache;
        private DmlNode mDmlNode;
        private Graphic mGraphicData;
        private DmlChartUserShapeAnchorType mAnchorType;
        private PointF mFrom;
        private PointF mTo;
        private readonly ShapeMarkupLanguage mMarkupLanguage;
        private int mTextboxId;
        private int mLinkedTextboxId;
        private int mLinkedTextboxSeq;
        private long mHashCodeCache;

        /// <summary>
        /// Default value which specifies that position of shape is not relative. See [MS-ODRAW] 2.3.5.3 pctHorizPos.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DefaultPercentValue = -10001;      /* 0xFFFFD8EF */

        private bool mIsFallback;

        private Size mEffectiveCoordSize = new Size(0, 0);
        private SizeF mBoundsForRelativeSize = new SizeF(0f, 0f);

        private Fill mFill;
        private IFill mFillCore;
        private ShadowFormat mShadow;
        private GlowFormat mGlowFormat;
        private ReflectionFormat mReflectionFormat;
        private SoftEdgeFormat mSoftEdgeFormat;

        private const string InvalidAction = "Object doesn't support this action.";
    }
}
