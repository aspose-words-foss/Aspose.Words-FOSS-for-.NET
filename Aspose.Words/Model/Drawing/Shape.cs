// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/07/2006 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.RW.Vml;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents an object in the drawing layer, such as an AutoShape, textbox, freeform, OLE object, ActiveX control, or picture.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-shapes/">Working with Shapes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Using the <see cref="Shape"/> class you can create or modify shapes in a Microsoft Word document.</p>
    ///
    /// <p>An important property of a shape is its <see cref="ShapeBase.ShapeType"/>. Shapes of different
    /// types can have different capabilities in a Word document. For example, only image and OLE shapes
    /// can have images inside them. Most of the shapes can have text, but not all.</p>
    ///
    /// <p>Shapes that can have text, can contain <see cref="Aspose.Words.Paragraph"/> and
    /// <see cref="Aspose.Words.Tables.Table"/> nodes as children.</p>
    ///
    /// <seealso cref="ShapeBase"/>
    /// <seealso cref="GroupShape"/>
    /// </remarks>
    public sealed class Shape : ShapeBase, ITextBox, IStrokable
    {
        /// <summary>
        /// Ctor. Use this internally only.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        internal Shape(DocumentBase doc)
            : this(doc, ShapeMarkupLanguage.Vml)
        {
        }

        /// <summary>
        /// Ctor. Use this internally only.
        /// WORDSNET-11493 Implement possibility to create DML shapes by default.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="markupLanguage">Shape markup language: DrawingML or Vml</param>
        internal Shape(DocumentBase doc, ShapeMarkupLanguage markupLanguage)
            : base(doc, markupLanguage)
        { }

        /// <summary>
        /// Creates a new shape object.
        /// </summary>
        /// <remarks>
        /// <p>You should specify desired shape properties after you created a shape.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="shapeType">The type of the shape to create.</param>
        public Shape(DocumentBase doc, ShapeType shapeType)
            : this(doc, ShapeMarkupLanguage.Vml)
        {
            if (!IsAllowedShapeTypes(shapeType) || !DmlEnum.HasVmlPresetGeometry(shapeType))
                throw new NotSupportedException("Cannot create shapes of this type.");

            SetShapeType(shapeType);
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitShapeStart"/>, then calls <see cref="Node.Accept"/>
        /// for all child nodes of the shape and calls <see cref="DocumentVisitor.VisitShapeEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the shape.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitShapeStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the shape.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitShapeEnd(this);
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Clone(bool,INodeCloningListener)"]/*'/>
        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Shape lhs = (Shape)base.Clone(isCloneChildren, cloningListener);
            lhs.mStroke = null;
            lhs.mStrokeCore = null;
            lhs.mTextBoxCache = null;
            lhs.mSignatureLineCache = null;
            lhs.mChart = null;
            lhs.mImageDataCache = null;
            ((IFillable)lhs).SetFill(null);

            return lhs;
        }

        internal override bool IsPercentWidthInapplicable
        {
            get
            {
                if (IsInline)
                    return true;

                if (!HasTextbox)
                    return false;

                LayoutFlow layoutFlow = TextBox.LayoutFlow;

                return (layoutFlow == LayoutFlow.Vertical)
                       || (layoutFlow == LayoutFlow.BottomToTop)
                       || (layoutFlow == LayoutFlow.TopToBottom)
                       || (layoutFlow == LayoutFlow.TopToBottomIdeographic);
            }
        }

        /// <summary>
        /// Can only insert paragraphs and tables into this node.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                return NodeUtil.IsBlockLevelNode(newChild);

            DmlNodeType dmlNodeType = DmlNode.DmlNodeType;

            // Only WordprocessingShape can contain other BlockLevel nodes.
            if ((dmlNodeType == DmlNodeType.WordprocessingShape) && NodeUtil.IsBlockLevelNode(newChild))
                return true;

            if ((newChild.NodeType != NodeType.Shape) && (newChild.NodeType != NodeType.GroupShape))
                return false;

            if (!(DmlNode is DmlCompositeNode) &&
                (dmlNodeType != DmlNodeType.Chart) &&
                (dmlNodeType != DmlNodeType.ChartEx))
                return false;

            // Current shape can not hold WordprocessingCanvas, WordprocessingGroupShape, LockedCanvas or GroupShape
            // DML node types in DmlNode property. DML nodes with this types have to be placed into GroupShape.
            // But actually code has not any restrictions for it, so the code below formally is valid.

            DmlNodeType newNodeType = ((ShapeBase)newChild).DmlNode.DmlNodeType;

            if (((dmlNodeType == DmlNodeType.WordprocessingCanvas) || (dmlNodeType == DmlNodeType.WordprocessingGroupShape)) &&
                CanInsertInWpCanvasAndWpGroupShape(newNodeType))
                return true;

            if (((dmlNodeType == DmlNodeType.LockedCanvas) || (dmlNodeType == DmlNodeType.GroupShape)) &&
                CanInsertInLockedCanvasAndGroupShape(newNodeType))
                return true;

            if (((dmlNodeType == DmlNodeType.Chart) || (dmlNodeType == DmlNodeType.ChartEx)) &&
                CanInsertInChart(newNodeType))
                return true;

            // If current shape has GraphicFrame then it can hold diagram or chart.
            // Generally content of the graphic frame expects object which is serialized out as xml.
            if ((dmlNodeType == DmlNodeType.GraphicFrame) &&
                ((newNodeType == DmlNodeType.Chart) || (newNodeType == DmlNodeType.ChartEx) ||
                    (newNodeType == DmlNodeType.Diagram)))
                return true;

            // Graphic frame can be inserted into group shape or user shapes of the chart.
            if (((dmlNodeType == DmlNodeType.GroupShape) || (dmlNodeType == DmlNodeType.WordprocessingGroupShape)) &&
                  (newNodeType == DmlNodeType.GraphicFrame))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the sum of horizontal margins.
        /// </summary>
        /// <remarks>
        /// It is used to calculate how much space is left for text box contents in a container of a certain size.
        /// </remarks>
        float ITextBox.GetHorizontalMargins_ITextBox()
        {
            if (HasVerticalTextFlow())
            {
                // Margins are not used for vertical text flow.
                return 0f;
            }

            // DM I considered using DmlRenderer,
            // but by looking at the code it may return other insets instead of LeftInset depending on rotation.
            // It would not work correctly for text boxes.
            ShapeAdaptor shapeAdaptor = new ShapeAdaptor(this);
            return (float)shapeAdaptor.Margins;

            // DM However, eventually, we need to get effective text rectangle size for complex shapes from rendering.
            // We will need to decide how to handle margins then.
        }

        /// <summary>
        /// WordprocessingCanvas and WordprocessingGroupShape can only contain WordprocessingGroupShape,
        /// WordprocessingShape, GroupShape and Picture.
        /// </summary>
        private static bool CanInsertInWpCanvasAndWpGroupShape(DmlNodeType dmlNodeType)
        {
            return ((dmlNodeType == DmlNodeType.WordprocessingGroupShape) ||
                   (dmlNodeType == DmlNodeType.WordprocessingShape) ||
                   CanInsertInCompositeShapeExcludeGrFrame(dmlNodeType));
        }

        /// <summary>
        /// LockedCanvas and GroupShape can only contain GroupShape, Shape, Picture, ConnectorShape and WordprocessingShape.
        /// </summary>
        private static bool CanInsertInLockedCanvasAndGroupShape(DmlNodeType dmlNodeType)
        {
            return ((dmlNodeType == DmlNodeType.Shape) ||
                    (dmlNodeType == DmlNodeType.ConnectorShape) ||
                    (dmlNodeType == DmlNodeType.WordprocessingShape) ||
                    CanInsertInCompositeShapeExcludeGrFrame(dmlNodeType));
        }

        private static bool CanInsertInCompositeShapeExcludeGrFrame(DmlNodeType dmlNodeType)
        {
            // Content of the graphic frame expects object which is serialized out as xml.
            return ((dmlNodeType == DmlNodeType.GroupShape) ||
                    (dmlNodeType == DmlNodeType.Picture));
        }

        /// <summary>
        /// WORDSNET-11246 Chart can only contain GroupShape, Shape, Picture, ConnectorShape or GraphicFrame.
        /// </summary>
        private static bool CanInsertInChart(DmlNodeType dmlNodeType)
        {
            return ((dmlNodeType == DmlNodeType.Shape) ||
                    (dmlNodeType == DmlNodeType.GraphicFrame) ||
                    (dmlNodeType == DmlNodeType.ConnectorShape) ||
                    CanInsertInCompositeShapeExcludeGrFrame(dmlNodeType));
        }

        /// <summary>
        /// Creates and returns a horizontal rule shape.
        /// </summary>
        /// <remarks>
        /// <p>After you create a shape, you need to insert it in the document where you want it.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <returns>A horizontal rule shape with default settings.</returns>
        internal static Shape CreateHorizontalRule(Document doc)
        {
            // This is how Microsoft Word creates a horizontal rule.
            Shape shape = new Shape(doc, ShapeType.Rectangle);
            shape.WrapType = WrapType.Inline;
            shape.Filled = true;
            shape.FillColor = HorizontalRule.DefaultColor.ToNativeColor();
            shape.Stroked = false;

            shape.HorizontalRule.On = true;
            shape.HorizontalRule.Standard = true;

            shape.Height = 1.5;
            shape.SetWidthSafe(doc.FirstSection.PageSetup.ContentWidth); // This is pretty simplistic, but okay.
            shape.HorizontalRule.Percent = 100;

            return shape;
        }

        /// <summary>
        /// True when the shape is not null and filled or has an image.
        /// Used one a document background shape to check if it is visible or dummy.
        /// </summary>
        internal static bool IsVisibleAsBackground(Shape shape)
        {
            return (shape != null) &&
                   (shape.Filled || shape.HasImage || shape.MarkupLanguage == ShapeMarkupLanguage.Dml);
        }

        /// <summary>
        /// Fixes zero size of shape. Gets it from the default or a corresponding image.
        /// </summary>
        internal void FixZeroSize(bool isFromDefault)
        {
            if (HasImage)
            {
                if (isFromDefault)
                {
                    if (MathUtil.IsZero(Width))
                        SetWidthSafe(DefaultShapeSize);
                    if (MathUtil.IsZero(Height))
                        SetHeightSafe(DefaultShapeSize);
                }
                else
                {
                    if (HasImageBytes && MathUtil.IsZero(Width) && MathUtil.IsZero(Height))
                    {
                        SetWidthSafe(ImageData.ImageSize.WidthPoints);
                        SetHeightSafe(ImageData.ImageSize.HeightPoints);
                    }
                }
            }
        }

        /// <summary>
        /// Removes image bytes from ImageData, and places them into FillImageBytes.
        /// </summary>
        /// <remarks>
        /// Can be executed for VML shape only.
        /// </remarks>
        internal void ReplaceVmlImageDataWithFill()
        {
            Debug.Assert(MarkupLanguage == ShapeMarkupLanguage.Vml);

            FillCore.SetImageBytes(ImageData.ImageBytes);
            FillCore.FillType = FillTypeCore.Picture;
            // WORDSNET-17052 Converted image data is not visible due to attribute "Filled" does not set.
            Filled = true;

            // Reset ImageBytes.
            ImageData.ImageBytes = null;
        }

        private DmlOutline GetThemeOutlineProperties()
        {
            Debug.Assert(MarkupLanguage == ShapeMarkupLanguage.Dml);

            DmlLineReference lineReference = ((DmlShapeBase)DmlNode).Style.LineReference;
            int styleMatrixIndex = lineReference.StyleMatrixIndex;
            IThemeProvider themeProvider = DocumentTheme;

            // It seems that 0 index disables outline and also if there are no Themes inside document return
            // default outline.
            DmlOutline outline = ((styleMatrixIndex == 0) || (themeProvider == null))
                ? new DmlOutline()
                : themeProvider.GetLineStyle(styleMatrixIndex - 1);

            outline.ApplyStyleColor(lineReference.Color);

            return outline;
        }

        #region IStrokable implementation

        /// <summary>
        /// Gets or sets the foreground color for the stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="DrColor.Black"/>.
        /// </remarks>
        DrColor IStrokable.StrokeForeColor
        {
            get { return StrokeCore.ColorInternal; }
            set { StrokeCore.ColorInternal = value; }
        }

        /// <summary>
        /// Gets the base foreground color without modifiers for the stroke.
        /// </summary>
        DrColor IStrokable.StrokeBaseForeColor
        {
            get { return StrokeCore.ColorInternalUnmodified; }
        }

        /// <summary>
        /// Gets or sets the background color for the stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="DrColor.White"/>.
        /// </remarks>
        DrColor IStrokable.StrokeBackColor
        {
            get { return StrokeCore.Color2Internal; }
            set { StrokeCore.Color2Internal = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke foreground color.
        /// </summary>
        ThemeColor IStrokable.StrokeForeThemeColor
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return FillUtil.NativeToThemeColor(StrokeCore.ColorInternal, DocumentTheme);

                DmlFill fill = StrokeCore.StrokeFill;
                if (fill == null)
                    return ThemeColor.None;

                DmlColor curDmlColor = fill.DmlColorInternal;
                return ((curDmlColor == null) || (curDmlColor.ColorType != DmlColorType.SchemeColor))
                    ? ThemeColor.None
                    : ((DmlSchemeColor)curDmlColor).Value;
            }
            set
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    StrokeCore.ColorInternal = (value != ThemeColor.None)
                        ? FillUtil.ThemeToNativeColor(value, DocumentTheme)
                        : StrokeCore.ColorInternal;
                }
                else
                {
                    if (value == ThemeColor.None)
                    {
                        // MS Word converts ThemeColor to RGB in this case.
                        DmlColor curDmlColor = StrokeCore.StrokeFill.DmlColorInternal;
                        if (curDmlColor.ColorType == DmlColorType.SchemeColor)
                        {
                            DmlHexRgbColor color = ((DmlSchemeColor)curDmlColor).Resolve(DocumentTheme);
                            StrokeCore.ColorInternal = color.CreateUnmodifiedDrColor(DocumentTheme);
                        }
                    }
                    else
                    {
                        StrokeCore.ColorInternal = DrColor.Empty;
                        StrokeCore.StrokeFill.DmlColorInternal = new DmlSchemeColor(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke background color.
        /// </summary>
        ThemeColor IStrokable.StrokeBackThemeColor
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    return FillUtil.NativeToThemeColor(StrokeCore.Color2Internal, DocumentTheme);

                DmlFill fill = StrokeCore.StrokeFill;
                if (fill == null)
                    return ThemeColor.None;

                DmlColor curDmlColor = fill.DmlColor2Internal;
                return ((curDmlColor == null) || (curDmlColor.ColorType != DmlColorType.SchemeColor))
                    ? ThemeColor.None
                    : ((DmlSchemeColor)curDmlColor).Value;
            }
            set
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    StrokeCore.Color2Internal = (value != ThemeColor.None)
                        ? FillUtil.ThemeToNativeColor(value, DocumentTheme)
                        : StrokeCore.Color2Internal;
                else
                {
                    if (value == ThemeColor.None)
                    {
                        // MS Word converts ThemeColor to RGB in this case.
                        DmlColor curDmlColor = StrokeCore.StrokeFill.DmlColor2Internal;
                        if ((curDmlColor != null) && (curDmlColor.ColorType == DmlColorType.SchemeColor))
                        {
                            DmlHexRgbColor color = ((DmlSchemeColor)curDmlColor).Resolve(DocumentTheme);
                            StrokeCore.Color2Internal = color.CreateUnmodifiedDrColor(DocumentTheme);
                        }
                    }
                    else
                    {
                        StrokeCore.Color2Internal = DrColor.Empty;
                        StrokeCore.StrokeFill.DmlColor2Internal = new DmlSchemeColor(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the stroke foreground color.
        /// </summary>
        double IStrokable.StrokeForeTintAndShade
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if ((ShapePr[ShapeAttr.LineColorExt] == null) || (ShapePr[ShapeAttr.LineColorExtMod] == null))
                        return 0.0d;

                    int modifier = (int)ShapePr[ShapeAttr.LineColorExtMod];
                    if ((modifier > -256) && (modifier < 256))
                        return modifier >= 0
                            ? 1.0d - ((double)modifier / 255)
                            : -1.0d - ((double)modifier / 255);

                    return 0.0d;
                }
                else
                {
                    DmlColor curColor = StrokeCore.StrokeFill.DmlColorInternal;
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
                    DrColor baseColor = (ShapePr[ShapeAttr.LineColorExt] != null)
                        ? (DrColor)ShapePr[ShapeAttr.LineColorExt]
                        : StrokeCore.ColorInternal;
                    ShapePr[ShapeAttr.LineColorExt] = baseColor;

                    int tintAndShadeModifier = FillUtil.VmlTintAndShadeModifier(value);
                    ShapePr[ShapeAttr.LineColorExtMod] = value >= 0
                        ? tintAndShadeModifier
                        : -tintAndShadeModifier;

                    if (value > 0)
                        StrokeCore.ColorInternal = FillUtil.VmlTint(baseColor, tintAndShadeModifier);
                    if (value < 0)
                        StrokeCore.ColorInternal = FillUtil.VmlShade(baseColor, tintAndShadeModifier);
                }
                else
                {
                    DmlFill curFill = StrokeCore.StrokeFill;
                    DmlColor curColor = curFill.DmlColorInternal;

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
        /// Gets or sets a double value that lightens or darkens the stroke background color.
        /// </summary>
        double IStrokable.StrokeBackTintAndShade
        {
            get
            {
                if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                {
                    if ((ShapePr[ShapeAttr.LineBackColorExt] == null) || (ShapePr[ShapeAttr.LineBackColorExtMod] == null))
                        return 0.0d;

                    int modifier = (int)ShapePr[ShapeAttr.LineBackColorExtMod];
                    if ((modifier > -256) && (modifier < 256))
                        return modifier >= 0
                            ? 1.0d - ((double)modifier / 255)
                            : -1.0d - ((double)modifier / 255);

                    return 0.0d;
                }
                else
                {
                    DmlColor curColor = StrokeCore.StrokeFill.DmlColor2Internal;
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
                    DrColor baseColor = (ShapePr[ShapeAttr.LineBackColorExt] != null)
                        ? (DrColor)ShapePr[ShapeAttr.LineBackColorExt]
                        : StrokeCore.Color2Internal;
                    ShapePr[ShapeAttr.LineBackColorExt] = baseColor;

                    int tintAndShadeModifier = FillUtil.VmlTintAndShadeModifier(value);
                    ShapePr[ShapeAttr.LineBackColorExtMod] = value >= 0
                        ? tintAndShadeModifier
                        : -tintAndShadeModifier;

                    if (value > 0)
                        StrokeCore.Color2Internal = FillUtil.VmlTint(baseColor, tintAndShadeModifier);
                    if (value < 0)
                        StrokeCore.Color2Internal = FillUtil.VmlShade(baseColor, tintAndShadeModifier);
                }
                else
                {

                    DmlFill curFill = StrokeCore.StrokeFill;
                    DmlColor curColor = curFill.DmlColor2Internal;

                    if (curColor != null)
                    {
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
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the stroke is visible.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        bool IStrokable.StrokeVisible
        {
            get { return StrokeCore.On; }
            set { StrokeCore.On = value; }
        }

        /// <summary>
        /// Gets or sets a value between 0.0 (opaque) and 1.0 (clear) representing the degree of transparency
        /// of the stroke.
        /// </summary>
        /// <remarks>
        /// The default value is 0.
        /// </remarks>
        double IStrokable.StrokeTransparency
        {
            get { return 1 - StrokeCore.Opacity; }
            set { StrokeCore.Opacity = 1 - value; }
        }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a shape in points.
        /// </summary>
        /// <remarks>
        /// The default value is 0.75.
        /// </remarks>
        double IStrokable.Weight
        {
            get { return StrokeCore.Weight; }
            set { StrokeCore.Weight = value; }
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
            set { StrokeCore.DashStyle = value; }
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
            set { StrokeCore.JoinStyle = value; }
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
            set { StrokeCore.EndCap = value; }
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
            set { StrokeCore.LineStyle = value; }
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
            set { StrokeCore.StartArrowType = value; }
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
            set { StrokeCore.EndArrowType = value; }
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
            set { StrokeCore.StartArrowWidth = value; }
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
            set { StrokeCore.StartArrowLength = value; }
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
            set { StrokeCore.EndArrowWidth = value; }
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
            set { StrokeCore.EndArrowLength = value; }
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
            set { StrokeCore.LineFillType = value; }
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
            get { return DocumentTheme; }
        }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        DmlFill IStrokable.StrokeFill
        {
            get { return StrokeCore.StrokeFill; }
            set { StrokeCore.StrokeFill = value; }
        }

        #endregion

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.Shape"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Shape; }
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.StoryType.Textbox"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Public API, as designed.")]
        public StoryType StoryType
        {
            get { return StoryType.Textbox; }
        }

        /// <summary>
        /// Returns <c>true</c> if an extrusion effect is enabled.
        /// </summary>
        public bool ExtrusionEnabled
        {
            get { return GraphicData.ExtrusionEnabled; }
        }

        /// <summary>
        /// Returns <c>true</c> if a shadow effect is enabled.
        /// </summary>
        public bool ShadowEnabled
        {
            get { return GraphicData.ShadowEnabled; }
        }

        /// <summary>
        /// Defines a stroke for a shape.
        /// </summary>
        public Stroke Stroke
        {
            get
            {
                if (mStroke == null)
                    mStroke = new Stroke(this);

                return mStroke;
            }
        }

        /// <summary>
        /// Defines whether the path will be stroked.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Aspose.Words.Drawing.Stroke.On"/> property.</p>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        public bool Stroked
        {
            get { return Stroke.On; }
            set { Stroke.On = value; }
        }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a shape in points.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Aspose.Words.Drawing.Stroke.Weight"/> property.</p>
        /// <p>The default value is 0.75.</p>
        /// </remarks>
        public double StrokeWeight
        {
            get { return Stroke.Weight; }
            set { Stroke.Weight = value; }
        }

        /// <summary>
        /// Defines the color of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Aspose.Words.Drawing.Stroke.Color"/> property.</p>
        /// <p>The default value is
        /// <see cref="System.Drawing.Color.Black"/>.
        /// </p>
        /// </remarks>
        public Color StrokeColor
        {
            get { return Stroke.Color; }
            set { Stroke.Color = value; }
        }

        /// <summary>
        /// Determines whether the closed path of the shape will be filled.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Fill.Visible"/> property.</p>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        public bool Filled
        {
            get { return FillCore.On; }
            set { FillCore.On = value; }
        }

        /// <summary>
        /// Defines the brush color that fills the closed path of the shape.
        /// </summary>
        /// <remarks>
        /// <p>This is a shortcut to the <see cref="Fill.Color"/> property.</p>
        /// <p>The default value is
        /// <see cref="System.Drawing.Color.White"/>.
        /// </p>
        /// </remarks>
        public Color FillColor
        {
            get { return ((IFillable)this).FilledColor; }
            set { ((IFillable)this).FilledColor = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if the shape has image bytes or links an image.
        /// </summary>
        public bool HasImage
        {
            get { return (CanHaveImage && ImageData.HasImage); }
        }

        /// <summary>
        /// Provides access to the image of the shape.
        /// Returns <c>null</c> if the shape cannot have an image.
        /// </summary>
        public ImageData ImageData
        {
            get
            {
                if (!CanHaveImage)
                    return null;

                if (mImageDataCache == null)
                {
                    // That's not good to call FetchDocument because it throws inside the glossary.
                    // Let's do the same "by hand".
                    mImageDataCache = new ImageData(this, (Document as Document));
                }

                return mImageDataCache;
            }
        }

        /// <summary>
        /// Provides access to the OLE data of a shape. For a shape that is not an OLE object or ActiveX control, returns <c>null</c>.
        /// </summary>
        public OleFormat OleFormat
        {
            get { return GraphicData.OleFormat; }
        }

        /// <summary>
        /// Defines attributes that specify how text is displayed in a shape.
        /// </summary>
        public TextBox TextBox
        {
            get
            {
                if (mTextBoxCache == null)
                    mTextBoxCache = new TextBox(this);
                return mTextBoxCache;
            }
        }

        /// <summary>
        /// Defines the text of the text path (of a WordArt object).
        /// </summary>
        public TextPath TextPath
        {
            get { return GraphicData.TextPath; }
        }

        /// <summary>
        /// Gets the first paragraph in the shape.
        /// </summary>
        public Paragraph FirstParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, 0, false); }
        }

        /// <summary>
        /// Gets the last paragraph in the shape.
        /// </summary>
        public Paragraph LastParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, -1, false); }
        }

        /// <summary>
        /// Provides access to the properties of the horizontal rule shape.
        /// For a shape that is not a horizontal rule, returns <c>null</c>.
        /// </summary>
        public HorizontalRuleFormat HorizontalRuleFormat
        {
            get
            {
                if (IsHorizontalRule)
                {
                    if (mHorizontalRuleFormatCache == null)
                        mHorizontalRuleFormatCache = new HorizontalRuleFormat(this);
                    return mHorizontalRuleFormatCache;
                }

                return null;
            }
        }

        /// <summary>
        /// Provides access to the adjustment raw values of a shape.
        /// For a shape that does not contain any adjustment raw values, it returns an empty collection.
        /// </summary>
        public AdjustmentCollection Adjustments
        {
            get { return GraphicData.Adjustments; }
        }

        /// <summary>
        /// Looks like JavaScript can be associated with a shape.
        /// </summary>
        internal string ScriptText
        {
            get { return (string)FetchShapeAttrInternal(ShapeAttr.ScriptText); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetShapeAttrInternal(ShapeAttr.ScriptText, value);
            }
        }

        /// <summary>
        /// Looks like " type="text/javascript"" type description of the script.
        /// </summary>
        internal string ScriptType
        {
            get { return (string)FetchShapeAttrInternal(ShapeAttr.ScriptType); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetShapeAttrInternal(ShapeAttr.ScriptType, value);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the shape has image bytes.
        /// </summary>
        internal bool HasImageBytes
        {
            get { return (CanHaveImage && ImageData.HasImageBytes); }
        }

        /// <summary>
        /// Returns <c>true</c> if shape is Ooxml control.
        /// </summary>
        internal bool IsOoxmlControl
        {
            get { return IsOleControl && (EmbeddedObject is OoxmlObject); }
        }

        /// <summary>
        /// Gets <see cref="Aspose.Words.Drawing.SignatureLine"/> object if the shape is a signature line. Returns <c>null</c> otherwise.
        /// </summary>
        public SignatureLine SignatureLine
        {
            get
            {
                if (GetDirectShapeAttrInternal(ShapeAttr.IsSignatureLine) == null)
                    return null;

                if (mSignatureLineCache == null)
                    mSignatureLineCache = new SignatureLine(this);
                return mSignatureLineCache;
            }
        }

        /// <summary>
        /// Provides access to the properties of the horizontal rule shape.
        /// </summary>
        internal HorizontalRule HorizontalRule
        {
            get { return GraphicData.HorizontalRule; }
        }

        /// <summary>
        /// Indicates the type of the connector if this shape is a connector.
        /// </summary>
        internal new ConnectorType ConnectorType
        {
            get { return base.ConnectorType; }
            set { base.ConnectorType = value; }
        }

        /// <summary>
        /// Gets the OLE object type as one enum (as opposed to querying several IsXXX properties).
        /// </summary>
        internal OleObjectType OleObjectType
        {
            get
            {
                if (IsOleControl)
                {
                    return OleObjectType.Control;
                }
                else if (IsOleObject)
                {
                    return (OleFormat.IsLink) ? OleObjectType.Linked : OleObjectType.Embedded;
                }
                else
                {
                    return OleObjectType.None;
                }
            }
        }

        internal PathInfo[] SegmentInfo
        {
            get { return (PathInfo[])FetchShapeAttrInternal(ShapeAttr.GeometrySegmentInfo); }
        }

        internal PathPoint[] Vertices
        {
            get { return (PathPoint[])FetchShapeAttrInternal(ShapeAttr.GeometryVertices); }
        }

        internal Formula[] Formulas
        {
            get { return (Formula[])FetchShapeAttrInternal(ShapeAttr.GeometryFormulas); }
        }

        /// <summary>
        /// Gets the adjustment value that is applied to the shape.
        /// </summary>
        internal int GetAdjust(int index)
        {
            int adjKey = VmlUtil.GetAdjustKey(index);
            return (int)FetchShapeAttrInternal(adjKey);
        }

        internal DrColor ImageRecolor
        {
            get
            {
                object imageRecolor = GetDirectShapeAttrInternal(ShapeAttr.ImageRecolor);
                // 0xffffffff - is the default value for this attribute.
                if ((imageRecolor == null) || ((int)imageRecolor) == unchecked((int)0xffffffff))
                    return DrColor.Empty;

                // Color as int number has reversed order of color components.
                // Use temporary color to create correct one.
                DrColor color = new DrColor((int)((int)imageRecolor | unchecked((int)0xff000000)));
                return new DrColor(color.B, color.G, color.R);
            }
        }

        internal int LimoX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.GeometryXLimo); }
            set { SetShapeAttrInternal(ShapeAttr.GeometryXLimo, value); }
        }

        internal int LimoY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.GeometryYLimo); }
            set { SetShapeAttrInternal(ShapeAttr.GeometryYLimo, value); }
        }

        internal bool IsFitTextboxToText
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.TextboxFitShapeToText); }
            set { SetShapeAttrInternal(ShapeAttr.TextboxFitShapeToText, value); }
        }

        #region Shadow attrs.

        internal int ShadowOpacity
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowOpacity); }
        }

        internal int ShadowOffsetX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowOffsetX); }
        }

        internal int ShadowOffsetY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowOffsetY); }
        }

        internal int ShadowSecondOffsetX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowSecondOffsetX); }
        }

        internal int ShadowSecondOffsetY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowSecondOffsetY); }
        }

        internal int ShadowScaleXtoX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowScaleXtoX); }
        }

        internal int ShadowScaleYtoY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowScaleYtoY); }
        }

        internal int ShadowScaleXtoY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowScaleXtoY); }
        }

        internal int ShadowScaleYtoX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowScaleYtoX); }
        }

        internal int ShadowOriginX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowOriginX); }
        }

        internal int ShadowOriginY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowOriginY); }
        }

        internal int ShadowPerspectiveX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowPerspectiveX); }
        }

        internal int ShadowPerspectiveY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.ShadowPerspectiveY); }
        }

        internal ShadowTypeCore ShadowTypeCore
        {
            get { return (ShadowTypeCore)FetchShapeAttrInternal(ShapeAttr.ShadowType); }
        }

        #endregion

        internal bool IsParallelExtrusion
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.TDParallel); }
        }

        internal int ExtrusionSkewAngle
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDSkewAngle); }
        }

        internal int ExtrusionForeDepth
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDExtrudeForward); }
        }

        internal int ExtrusionBackDepth
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDExtrudeBackward); }
        }

        internal DrColor ExtrusionColor
        {
            get { return (DrColor)FetchShapeAttrInternal(ShapeAttr.TDExtrusionColor); }
        }

        internal bool ShouldUpdateRelativeSizeConsideringExtrusion
        {
            get { return ((WidthPercent != null || HeightPercent != null) && ExtrusionEnabled); }
        }

        internal int ViewpointX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDViewpointX); }
        }

        internal int ViewpointY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDViewpointY); }
        }

        internal int ViewpointZ
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDViewpointZ); }
        }

        internal int ViewpointOriginX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDOriginX); }
        }

        internal int ViewpointOriginY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDOriginY); }
        }

        internal int TDRotationAngleX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDRotationAngleX); }
        }

        internal int TDRotationAngleY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDRotationAngleY); }
        }

        internal int TDLight1PositionX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDKeyX); }
        }

        internal int TDLight1PositionY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDKeyY); }
        }

        internal int TDLight1PositionZ
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDKeyZ); }
        }

        internal int TDLight1Intensity
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDKeyIntensity); }
        }

        internal bool TDLight1Harsh
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.TDKeyHarsh); }
        }

        internal int TDLight2PositionX
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDFillX); }
        }

        internal int TDLight2PositionY
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDFillY); }
        }

        internal int TDLight2PositionZ
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDFillZ); }
        }

        internal int TDLight2Intensity
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDFillIntensity); }
        }

        internal bool TDLight2Harsh
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.TDFillHarsh); }
        }

        internal int TDAmbientIntensity
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDAmbientIntensity); }
        }

        internal int TDSpecularAmount
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDSpecularAmount); }
        }

        internal int TDDiffuseAmount
        {
            get { return (int)FetchShapeAttrInternal(ShapeAttr.TDDiffuseAmount); }
        }

        internal bool TDMetallic
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.TDMetallic); }
        }

        internal ThreeDRenderMode RenderMode
        {
            get { return (ThreeDRenderMode)FetchShapeAttrInternal(ShapeAttr.TDRenderMode); }
        }

        /// <summary>
        /// Gets shape adjust handles.
        /// This can return <c>null</c> or an empty array.
        /// </summary>
        internal Handle[] Handles
        {
            get { return (Handle[])FetchShapeAttrInternal(ShapeAttr.GeometryHandles); }
        }

        /// <summary>
        /// This never returns <c>null</c> and never returns an empty array.
        /// Can return 1, 2, 3 or 6 rectangles. The algorithm is specified in MS-ODRAW.pdf in the 2.3.6.29 pInscribe_complex section.
        /// </summary>
        internal PathRectangle[] TextBoxRects
        {
            get
            {
                // Try to get text box rectangle from attributes if specified.
                PathRectangle[] rects = (PathRectangle[])FetchShapeAttrInternal(ShapeAttr.GeometryPathTextBoxRects);
                if ((rects != null) && (rects.Length > 0))
                    return rects;

                // Text box rectangle is not specified so return the bounding rectangle of shape
                PathRectangle pathRectangle = new PathRectangle();
                pathRectangle.Left = new PathValue(-CoordOriginX, false);
                pathRectangle.Top = new PathValue(-CoordOriginY, false);
                pathRectangle.Right = new PathValue(CoordSizeWidth - CoordOriginX, false);
                pathRectangle.Bottom = new PathValue(CoordSizeHeight - CoordOriginY, false);
                return new PathRectangle[] {pathRectangle};
            }
            set { SetShapeAttrInternal(ShapeAttr.GeometryPathTextBoxRects, value); }
        }

        internal bool IsTextureRotated
        {
            get { return (bool)FetchShapeAttrInternal(ShapeAttr.FillUseShapeAnchor); }
        }

        /// <summary>
        /// Shows if shape's fill should be ignored while rendering.
        /// </summary>
        /// <remarks>
        /// For now returns <c>true</c> only for connector shapes and lines.
        /// </remarks>
        internal bool IgnoreFill
        {
            get { return IsConnector || ShapeType == ShapeType.Line; }
        }

        /// <summary>
        /// Returns <c>true</c> if shape fill has image.
        /// </summary>
        internal bool HasFillImage
        {
            get { return (CanHaveFillImage && ArrayUtil.HasData(FillCore.ImageBytes)); }
        }

        TextBoxWrapMode ITextBox.TextBoxWrapMode_ITextBox
        {
            get { return TextBox.TextBoxWrapMode; }
        }

        LayoutFlow ITextBox.TextboxLayoutFlow_ITextBox
        {
            get { return LayoutFlow; }
        }

        bool ITextBox.HasVerticalTextFlow_ITextBox
        {
            get { return HasVerticalTextFlow(); }
        }

        private bool HasVerticalTextFlow()
        {
            bool hasVerticalTextFlow;
            switch (LayoutFlow)
            {
                case LayoutFlow.Vertical:
                case LayoutFlow.TopToBottom:
                case LayoutFlow.TopToBottomIdeographic:
                case LayoutFlow.BottomToTop:
                    hasVerticalTextFlow = true;
                    break;
                default:
                    hasVerticalTextFlow = false;
                    break;
            }

            return hasVerticalTextFlow;
        }

        ShapeMarkupLanguage ITextBox.MarkupLanguage_ITextBox
        {
            get { return MarkupLanguage; }
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="Shape"/> has a <see cref="Drawing.Charts.Chart"/>.
        /// </summary>
        public bool HasChart
        {
            get
            {
                return (DmlNode != null) &&
                    ((DmlNode.DmlNodeType == DmlNodeType.Chart) || (DmlNode.DmlNodeType == DmlNodeType.ChartEx));
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this <see cref="Shape"/> has a SmartArt object.
        /// </summary>
        public bool HasSmartArt
        {
            get
            {
                return ((DmlNode != null) && (DmlNode.DmlNodeType == DmlNodeType.Diagram));
            }
        }

        /// <summary>
        /// Provides access to the chart properties if this shape has a <see cref="Drawing.Charts.Chart"/>.
        /// </summary>
        /// <remarks>This property will return the <see cref="Drawing.Charts.Chart"/> object only if <see cref="HasChart"/>
        /// property is <c>true</c> for this <see cref="Shape"/>, and will throw an exception otherwise.</remarks>
        public Chart Chart
        {
            get
            {
                if (HasChart)
                {
                    if (mChart == null)
                        mChart = new Chart((DmlChartSpace)DmlNode);

                    return mChart;
                }

                throw new InvalidOperationException("This Shape does not have a Chart.");
            }
        }

        /// <summary>
        /// Gets <see cref="IStroke"/> object for the shape.
        /// </summary>
        private IStroke StrokeCore
        {
            get
            {
                if (mStrokeCore == null)
                {
                    if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                    {
                        mStrokeCore = new VmlOutline(this);
                    }
                    else
                    {
                        IDmlCommonShapePrSource dmlShape = (IDmlCommonShapePrSource)DmlNode;
                        DmlOutline dmlOutline = dmlShape.Outline;

                        if (dmlShape.Style != null)
                        {
                            // Set parent properties here, because previously we have not Document Themes to achieve this.
                            dmlOutline.SetParentProperties(GetThemeOutlineProperties());
                        }

                        // Set Shape for outline fill, we need this to get color from
                        // Document Themes and to reset fill, please see DmlFill class.
                        dmlOutline.Fill.Parent = this;

                        mStrokeCore = dmlOutline;
                    }
                }

                return mStrokeCore;
            }
        }

        private Stroke mStroke;
        private TextBox mTextBoxCache;
        private ImageData mImageDataCache;
        private Chart mChart;
        private SignatureLine mSignatureLineCache;
        private HorizontalRuleFormat mHorizontalRuleFormatCache;
        private IStroke mStrokeCore;
    }
}
