// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/04/2016 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Drawing;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Math;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Ole.Ole2;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Validation;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This class is invoked by <see cref="DocumentValidator"/> to correct any issues with shapes
    /// including DML->VML and VML->DML conversion.
    /// </summary>
    internal class ShapeValidator
    {
        internal ShapeValidator(SaveInfo saveInfo,
            DocumentValidatorActions saveActions,
            DocumentVisitor validator)
        {
            mSaveInfo = saveInfo;
            mSaveActions = saveActions;
            mValidator = validator;

            mShapeIdGenerator = new ShapeIdGenerator(saveInfo);
            mShapeImageOptimizer = new ShapeImageOptimizer();
        }

        /// <summary>
        /// Performs re-initialization for the main document and for the glossary document.
        /// </summary>
        internal void DocumentStart(DocumentBase doc, BookmarkValidator bookmarkValidator)
        {
            mBookmarkValidator = bookmarkValidator;
            mDrawingMLIdValidator = new DrawingMLIdValidator();

            ValidateBackgroundShape(doc);
        }

        /// <summary>
        /// Sets properties on the document background shape that we know MS Word sets on a background shape in a DOC file.
        /// Not setting this will not crash MS Word, but still do it for the sake of consistency.
        /// </summary>
        private void ValidateBackgroundShape(DocumentBase doc)
        {
            Shape shape = doc.BackgroundShape;
            if (shape == null)
                return;

            // FOSS

            shape.Id = 1025;  // The background shape has this id in MS Word.
            // It is needed to assign shape for DML fill.
            if ((shape.MarkupLanguage == ShapeMarkupLanguage.Dml) &&
                (((IDmlCommonShapePrSource)shape.DmlNode).Fill != null))
            {
                ((IDmlCommonShapePrSource)shape.DmlNode).Fill.Parent = shape;
            }

            shape.Filled = true;
            shape.StrokeWeight = 0;
            shape.Stroked = false;
            shape.FlipOrientation = FlipOrientation.None;
        }

        /// <summary>
        /// Performs finalization for the main document and for the glossary document.
        /// </summary>
        internal void DocumentEnd(DocumentBase doc)
        {
            // FOSS
            mDrawingMLIdValidator.Validate();
        }

        internal VisitorAction VisitShapeStart(Shape shape, StoryTypeStack storyTypeStack)
        {
            if (shape.IsOleControl)
                UpdateOleControlImage(shape);

            // WORDSNET-15796 Modify textbox, which was produced by a customer's code.
            FixTextBoxWithImageData(shape);
            // WORDSNET-12170 Update signature line image with proper signer name.
            SignatureLine signatureLine = shape.SignatureLine;
            if (signatureLine != null)
            {
                // We treat generated with Guid.NewGuid() method GUIDs as unique. So, we should check uniqueness only those Ids, which were set by the user
                // through SignatureLine.Id property. Therefore, we do not need to add new generated id to hash of used ids and also check their uniqueness.
                if (mUsedSignatureLineIds.Contains(signatureLine.Id))
                {
                    Guid oldId = signatureLine.Id;
                    signatureLine.Id = Guid.NewGuid();

                    WarningUtil.Warn(WarningCallback, WarningType.Hint, WarningSource.Shapes, WarningStrings.NonUniqueSignatureLineId,
                        oldId.ToString("B").ToUpper(), signatureLine.Id.ToString("B").ToUpper());
                }
                else
                {
                    mUsedSignatureLineIds.Add(signatureLine.Id);
                }

                if (!signatureLine.IsSigned)
                    signatureLine.UpdateShapeImage();
            }
            ShapeIdGenerator.ValidateFluently(shape);

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLStart(shape, storyTypeStack);

            ValidateEquationXML(shape);

            // WORDSNET-17909 If the shape is converted to office math then no further processing is required,
            // because the shape was removed in the "ValidateEquationXML" from the model.
            if (shape.ParentNode == null)
                return VisitorAction.SkipThisNode;

            // FOSS

            AddParagraphIfTableIsLastChild(shape);
            VisitShapeCore(shape, storyTypeStack);
            ValidateTextboxAnchor(shape);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified VML shape can be converted to DML depending on output format.
        /// </summary>
        private bool CanCovertToDml(Shape shape)
        {
            return
                shape.IsTopLevel &&
                (mSaveInfo.SaveOptions is OoxmlSaveOptions) &&
                (mSaveInfo.OoxmlCompliance != OoxmlComplianceCore.Ecma376);
        }

        /// <summary>
        /// Validates a shape and adds a warning if improper compatibility is set for a LayoutInCell property.
        /// This is needed for Dml shapes only.
        /// </summary>
        private void ValidateLayoutInCell(ShapeBase shape)
        {
            if (mSaveInfo.IsDocxFormat &&
                (mSaveInfo.Document.CompatibilityOptions.MswVersion > MsWordVersionCore.Word2010) &&
                (shape.MarkupLanguage == ShapeMarkupLanguage.Dml) &&
                IsShapeInCell(shape) &&
                !shape.IsLayoutInCell)
            {
                Warn(WarningType.Hint, WarningSource.Validator,
                    WarningStrings.InvalidCompatibilityForLayoutInCell);
            }
        }

        /// <summary>
        /// Updates presentation images for OLE controls.
        /// </summary>
        /// <remarks>
        /// So far just updates image for HTML OLE radio button controls using predefined images.
        /// </remarks>
        private static void UpdateOleControlImage(Shape shape)
        {
            IEmbeddedObject embeddedObject = shape.EmbeddedObject;
            // Filter out controls other than HtmlOptionControl.
            if ((embeddedObject == null) || (embeddedObject.ClsidInternal != OleControl.HtmlOptionClsid))
                return;

            HtmlOptionOleControl htmlOptionOleControl = embeddedObject as HtmlOptionOleControl;
            if (htmlOptionOleControl == null)
            {
                OleObject oleObject = embeddedObject as OleObject;
                if ((oleObject != null) && (oleObject.Data != null))
                    htmlOptionOleControl = (HtmlOptionOleControl)OleControl.Create(oleObject.Data);
            }

            if (htmlOptionOleControl == null)
                return;

            byte[] imageBytes =
                OleUtil.ImageLibrary[htmlOptionOleControl.Checked ? "RadioChecked" : "RadioUnchecked"];
            shape.ImageData.ImageBytes = imageBytes;
            shape.Width = 20.25;
            shape.Height = 18;
        }

        /// <summary>
        /// Tests if a shape is contained in a cell.
        /// </summary>
        private static bool IsShapeInCell(ShapeBase shape)
        {
            if (shape.ParentNode != null)
            {
                Paragraph para = shape.FirstNonMarkupParentNode as Paragraph;

                if ((para != null) && para.IsInCell)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes image from ImageData, and places it as a background image.
        /// </summary>
        private static void FixTextBoxWithImageData(Shape shape)
        {
            if ((shape.ShapeType == ShapeType.TextBox) && shape.ImageData.HasImageBytes && (shape.Fill.ImageBytes == null))
            {
                // Such shape is a result of customer's code. To prevent AW from creating output which is incorrect and crashes MSW2016,
                // We must remove image bytes from ImageData, and place them into FillImageBytes.
                shape.ReplaceVmlImageDataWithFill();
            }
        }

        /// <summary>
        /// Updates text box geometry path if it is needed.
        /// </summary>
        /// <param name="shape">Shape which holds text box.</param>
        private void UpdateTxbxPathRects(ShapeBase shape)
        {
            Debug.Assert(shape != null);

            if (shape.ShapeType != ShapeType.CustomShape)
                return;

            PathRectangle[] rects = (PathRectangle[])shape.FetchShapeAttrInternal(ShapeAttr.GeometryPathTextBoxRects);
            Document doc = shape.Document as Document;

            if ((rects != null) || (doc == null))
                return;

            if (FileFormatUtil.IsXmlBasedFormat(doc.OriginalLoadFormat) && !IsSaveFormatXml)
                shape.SetShapeAttrInternal(ShapeAttr.GeometryPathTextBoxRects, ((Shape)shape).TextBoxRects);
        }

        /// <summary>
        /// Updates start and end point of the line when it is necessary.
        /// </summary>
        /// <param name="shape">Shape to process.</param>
        private static void UpdateLineVertices(ShapeBase shape)
        {
            Debug.Assert(shape != null);

            if (shape.ShapeType != ShapeType.Line)
                return;

            double[] vertices = GetLineVertices(shape, 0, 0, shape.CoordSizeWidth, shape.CoordSizeHeight);

            if (vertices.Length == 0)
                return;

            PathPoint startPoint = new PathPoint(MathUtil.DoubleToInt(vertices[0]), MathUtil.DoubleToInt(vertices[1]));
            PathPoint endPoint = new PathPoint(MathUtil.DoubleToInt(vertices[2]), MathUtil.DoubleToInt(vertices[3]));
            shape.SetShapeAttrInternal(ShapeAttr.GeometryVertices, new PathPoint[] { startPoint, endPoint });
        }

        /// <summary>
        /// Calculates line vertices coordinates according to geometry data and specified
        /// shape size and positions.
        /// </summary>
        /// <param name="shape">Source shape.</param>
        /// <param name="t">Actual top position.</param>
        /// <param name="l">Actual left position.</param>
        /// <param name="w">Actual width.</param>
        /// <param name="h">Actual height.</param>
        /// <returns>Array with line vertices coordinates.</returns>
        private static double[] GetLineVertices(ShapeBase shape, double t, double l, double w, double h)
        {
            Debug.Assert(shape != null);

            PathPoint[] vertices = (PathPoint[])shape.FetchShapeAttrInternal(ShapeAttr.GeometryVertices);

            // Expected two PathPoint to process.
            if ((vertices == null) || (vertices.Length < 2))
                return new double[0];

            return new double[] { (vertices[0].X.Value > 0) ? l + w : l,
                (vertices[0].Y.Value > 0) ? t + h : t,
                (vertices[1].X.Value > 0) ? l + w : l,
                (vertices[1].Y.Value > 0) ? t + h : t};
        }

        private static void AddParagraphIfTableIsLastChild(CompositeNode story)
        {
            Node lastNode = story.LastNonAnnotationChild;
            if (lastNode != null)
            {
                if ((lastNode.NodeType != NodeType.Paragraph) &&
                    (lastNode.NodeType != NodeType.StructuredDocumentTag))
                {
                    story.AppendChild(new Paragraph(story.Document));
                }
            }
        }

        internal VisitorAction VisitShapeEnd(Shape shape)
        {
            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLEnd(shape);
            else
                return VisitorAction.Continue;
        }

        internal VisitorAction VisitGroupShapeStart(GroupShape groupShape, StoryTypeStack storyTypeStack)
        {
            // andrnosk: WORDSNET-11960 Do not remove empty Dml group shapes.
            if (groupShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLStart(groupShape, storyTypeStack);

            // FOSS

            if (groupShape.HasNonMarkupDescendants)
            {
                VisitShapeCore(groupShape, storyTypeStack);
                return VisitorAction.Continue;
            }
            else
            {
                // WORDSNET-3882 Remove empty VML group shapes.
                groupShape.Remove();
                return VisitorAction.SkipThisNode;
            }
        }

        internal VisitorAction VisitGroupShapeEnd(GroupShape groupShape)
        {
            if (groupShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLEnd(groupShape);

            if (groupShape.HasNonMarkupDescendants)
            {
                return VisitorAction.Continue;
            }
            else
            {
                // WORDSNET-3882 Remove empty VML group shapes.
                groupShape.Remove();
                return VisitorAction.SkipThisNode;
            }
        }

        /// <summary>
        /// Verifies the shape is allowed in the current story and updates the shape id to make it valid for MS Word.
        /// </summary>
        private void VisitShapeCore(ShapeBase shape, StoryTypeStack storyTypeStack)
        {
            shape.UpdateSizeAndPositionFromRelative();

            // WORDSNET-5000 Do not shrink custom shape.
            // WORDSNET-9471 Do not shrink group shape.
            if ((shape.ShapeType != ShapeType.CustomShape) && (shape.NodeType != NodeType.GroupShape))
            {
                // Resiliency: Shape size is limited in MS Word.
                // DS: Looks like a shape size is limited only for updating and few rare cases on reading "old" formats.
                // Old Word versions may also open and re-save document with huge shape sizes.

                // WORDSNET-19128 In some cases, Word doesn't shrink shape.
                // WORDSNET-24026 The Word does not shrink shape to lower size exclude some cases for "old" formats.
                // Also see related tests: TestDefect2749, TestDefect2749.
                shape.ValidateZeroSizeShape();
            }

            // This is important, give the shape an identifier that is valid for MS Word.
            mShapeIdGenerator.VisitShape(shape, storyTypeStack.IsInHeaderOrFooter);

            mSaveInfo.AddToZOrderList(shape, storyTypeStack);

            mShapeImageOptimizer.RemoveImageDuplication(shape);

            // Collect OLE objects for writing to DOC and WML.
            if (shape.IsOle)
                ProcessOleShape((Shape)shape);

            // Writing a custom shape to RTF without this attribute seems to produce incorrectly drawn shapes.
            // MS Word 2007 export to RTF itself has this problem. This code seems to solve the problem.
            if ((shape.ShapeType == ShapeType.CustomShape) &&
                (shape.ShapePr.GetDirectAttr(ShapeAttr.GeometrySegmentInfo) == null))
            {
                shape.ShapePr.SetAttr(ShapeAttr.GeometrySegmentInfo, new PathInfo[0]);
            }

            // dmatv: a fix for WORDSNET-7327 by Andrey Noskov was present here.
            // It did not cover all the cases so I updated the logic and moved it to AttributeConverter per WORDSNET-14632

            if (IsSaveAction(DocumentValidatorActions.PrepareShapesForRendering))
            {
                ValidateColor(shape.ShapePr, ShapeAttr.FillColor);
                ValidateColor(shape.ShapePr, ShapeAttr.LineColor);
            }

            ValidateImageBytes(shape);

            // WORDSNET-14744 Set value for attribute "GeometryPathTextBoxRects" of the custom shape
            // when document is saved from XML-format to non-XML format. Applies this update only for VML shapes,
            // because MSW can not save the problematic shape which was previously converted to DML.
            UpdateTxbxPathRects(shape);

            UpdateLineVertices(shape);
            ValidateVerticalAlignment(shape);

            ValidateFloatingStory(shape, storyTypeStack.Current);

            ValidateTextPlainTextDecoration(shape);
        }

        /// <summary>
        /// Validates vertical position for a shape.
        /// </summary>
        private void ValidateVerticalAlignment(ShapeBase shape)
        {
            if ((shape.MarkupLanguage == ShapeMarkupLanguage.Dml) || !shape.IsTopLevel)
                return;

            // Take in attention fixed formats to obtain valid output for WORDSNET-17541 issue.
            if (!IsSaveFormatXml && !IsSaveAction(DocumentValidatorActions.PrepareShapesForRendering))
                return;

            // WORDSNET-17541 Word resets top position of the shape when it has "top" value of the relative vertical alignment.
            VerticalAlignment vertAlignment = (VerticalAlignment)shape.FetchShapeAttrInternal(ShapeAttr.VerticalAlignment);
            if ((VerticalAlignment.Top == vertAlignment) && !MathUtil.IsZero(shape.Top))
                shape.SetShapeAttrInternal(ShapeAttr.Top, 0.0);
        }


        /// <summary>
        /// Validates shapes with TextBoxes.
        /// </summary>
        private void ValidateTextboxAnchor(ShapeBase shape)
        {
            if (shape.HasTextbox && !mHasNotDefaultBoxVerticalAnchor)
            {
                Shape castShape = shape as Shape;
                mHasNotDefaultBoxVerticalAnchor |= (castShape != null) && (castShape.TextBox.VerticalAnchor != TextBoxAnchor.Top);
            }
        }

        /// <summary>
        /// Validates shapes with "EquationXML".
        /// </summary>
        private void ValidateEquationXML(Shape shape)
        {
            // Word tries to convert shape with "EquationXML" into office math object, when destination format is DOCX or ODT.
            // Also OOXML version must be greater then "Ecma" for DOCX, otherwise the Word does not convert shapes
            // to math objects. Looks like Word does not take in attention compatibility options, while deciding to convert
            // shape to math object.
            if ((mSaveInfo.IsDocxFormat && mSaveInfo.OoxmlCompliance > OoxmlComplianceCore.Ecma376) ||
                (mSaveInfo.SaveFormat == SaveFormat.Odt) ||
                (mSaveInfo.SaveFormat == SaveFormat.Ott))
            {
                OfficeMathUtil.ConvertShapeToOfficeMath(shape, WarningCallback);
                return;
            }

            byte[] equationXmlVal = OfficeMathUtil.RetrieveEquationXML(shape);

            // There is some restrictions for "EquationXML", when output document in the DOC format.
            if (!mSaveInfo.IsDocFormat || (equationXmlVal == null) || (equationXmlVal.Length == 0))
                return;

            // "EquationXML" has to be terminated with "0x00" byte, while saving to DOC format.
            // Otherwise Word raises errors while re-saving produced document to DOCX (see TestJira16540DocIssue).
            if (equationXmlVal[equationXmlVal.Length - 1] != 0x00)
                shape.ShapePr[ShapeAttr.EquationXML] = ArrayUtil.ResizeArray(equationXmlVal, equationXmlVal.Length + 1);
        }

        /// <summary>
        /// andrnosk: WORDSNET-8761, WORDSNET-11741 It seems special color 'window' it is simple DrColor.White upon rendering.
        /// Previously, we wrote DrColor(0xef, 0x11, 0, 0) like MSWord shows in UI toolbar (but not real shape color).
        /// Also MSWord automation returns white color for this shape too.
        /// </summary>
        private static void ValidateColor(ShapePr shapePr, int shapeAttr)
        {
            // 'window' color.
            DrColor wColor = DrColor.FromArgb(239, 17, 0, 0);
            if (shapePr.Contains(shapeAttr) && ((DrColor)shapePr[shapeAttr] == wColor))
                shapePr.SetAttr(shapeAttr, DrColor.White);
        }

        /// <summary>
        /// Make sure that we write to the document only fully supported image formats.
        /// We need to check image bytes, fill image bytes and line image bytes.
        /// </summary>
        private void ValidateImageBytes(ShapeBase shape)
        {
            if (shape.NodeType == NodeType.Shape)
            {
                Shape imageShape = (Shape)shape;

                foreach (int key in new int[] { ShapeAttr.FillImageBytes, ShapeAttr.LineImageBytes })
                {
                    byte[] imageBytes = (byte[])shape.GetDirectShapeAttrInternal(key);
                    if (imageBytes != null)
                        shape.SetShapeAttrInternal(key, ImageDataCore.GetImageBytes(imageBytes));
                }

                if (imageShape.HasImageBytes)
                {
                    byte[] imageBytes = ImageDataCore.GetImageBytes(imageShape.ImageData.ImageBytes);
                    imageShape.ImageData.ImageBytes = imageBytes;

                    // WORDSNET-12169 Create new picture fill using ImageData.ImageBytes if shape with specified type
                    // is supposed to be filled with image. Use this mechanism only upon rendering.
                    if (IsSaveAction(DocumentValidatorActions.PrepareShapesForRendering))
                    {
                        // Although MS Word has a special shape type for images, it appears that in MS Word documents any single
                        // shape can have an image, but some of them is supposed to be filled with image.
                        if (imageShape.CanHaveFillImage && (imageShape.ShapeType != ShapeType.Rectangle))
                        {
                            imageShape.ReplaceVmlImageDataWithFill();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates relative shape positions.
        /// </summary>
        /// <remarks>
        /// Word behavior depends on compatibility mode setting.
        /// </remarks>
        private void ValidateRelativePositions(ShapeBase shape)
        {
            Section section = (Section)shape.GetAncestor(NodeType.Section);
            if (section == null)
                return;

            CustomCompatibilitySetting compatibilityMode =
                mSaveInfo.Document.CompatibilityOptions.CustomCompatibilitySettings["compatibilityMode"];

            // Word converts relative position 'OutsideMargin' to 'Page' when compatibility is '11' (Word2003).
            // Actually, Word do this for vertical position as well and also converts shape to VML in this case.
            // But it is enough to convert only horizontal position for a while and leave shape as DML.
            if ((compatibilityMode != null) && (compatibilityMode.Value == "11") &&
                (shape.RelativeHorizontalPosition == RelativeHorizontalPosition.OutsideMargin))
            {
                shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                shape.Left += ConvertUtilCore.TwipToPoint((section.SectPr.PageWidth - section.SectPr.RightMargin));
            }
        }

        /// <summary>
        /// Prepares OLE data to be written.
        /// </summary>
        private void ProcessOleShape(Shape oleShape)
        {
            if (mSaveInfo.SaveOptions.IsTestMode)
                UpdateOcxName(oleShape);

            // For speed reason do following conversion only for "old" document formats.
            if (!mSaveInfo.IsLegacyFormat)
                return;

            IEmbeddedObject embeddedObject = oleShape.EmbeddedObject;
            if (embeddedObject == null)
                return;

            string progId = oleShape.OleFormat.ProgId;
            Guid clsid = new Guid(OleRegistryInfo.GetClsId(progId));
            if (clsid == Guid.Empty)
                Warn(WarningType.UnexpectedContent, WarningSource.Shapes, string.Format(WarningStrings.UnknownProgId, progId));

            MemoryStorage oleData = null;

            OoxmlObject ooxmlObject = embeddedObject as OoxmlObject;
            if (ooxmlObject != null)
            {
                if (oleShape.IsOoxmlControl)
                {
                    Warn(WarningType.DataLoss, WarningSource.Doc, WarningStrings.OoxmlControlNotSupported);
                }
                else
                {
                    // Wrap OOXML package to OLE object (note that original OOXML embed left unchanged):
                    oleData = OleUtil.WrapOoxmlPackage(oleShape.OleFormat.ProgId, ooxmlObject, oleShape.OleFormat.OleIcon);
                }
            }
            else
            {
                OleObject oleObject = embeddedObject.GetOleObject();
                if (oleObject != null)
                {
                    oleData = oleObject.Data;

                    // If original document is DOCX the oleObject might miss Clsid but this is essential to be written to DOC/RTF/WML
                    // otherwise Word shows error when object is double-clicked.
                    if (oleData.Clsid == Guid.Empty)
                        oleData.Clsid = clsid;
                }
            }

            if (oleData != null)
            {
                mSaveInfo.AllOleObjects[OleUtil.GetMsWordId(embeddedObject)] = oleData;

                switch (oleShape.OleObjectType)
                {
                    case OleObjectType.Control:
                        mSaveInfo.HasOleControls = true;
                        break;
                    case OleObjectType.Embedded:
                        mSaveInfo.EmbeddedOleObjects[OleUtil.GetMsWordId(embeddedObject)] = oleData;
                        break;
                    case OleObjectType.Linked:
                        // Do nothing.
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected OLE object type.");
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the specified shape is a chart in ODT format.
        /// </summary>
        private static bool IsOdtChart(Shape shape)
        {
            return shape.IsOle && (shape.OleFormat.ProgId == "opendocument.ChartDocument.1");
        }

        /// <summary>
        /// Updates OCXNAME stream with control name.
        /// </summary>
        /// <remarks>
        /// AM. In TestDefect1117.doc I found that Word doesn't update OCXNAME i.e it does not shrink stream length if new name is shorter
        /// that previous so OCXNAME stream contains valid data at start of stream and some "garbage" at end of stream.
        /// This fails our testing because Doc2Docx conversion updates OCXNAME stream and Docx2Docx does not.
        /// So for testing purpose I update this stream always.
        /// </remarks>
        private static void UpdateOcxName(Shape oleShape)
        {
            if (oleShape.IsOleControl && (oleShape.OleFormat.OleObject != null))
            {
                MemoryStorage oleData = oleShape.OleFormat.OleObject.Data;

                // Update OCXNAMESTREAM.
                OcxNameStream oldOcxNameStream = OcxNameStream.Read(oleData);

                if (oldOcxNameStream != null)
                {
                    OcxNameStream newOcxNameStream = new OcxNameStream(oldOcxNameStream.Value);
                    newOcxNameStream.Write(oleData);
                }
            }
        }

        private void ValidateFloatingStory(ShapeBase shape, StoryType currentStoryType)
        {
            if (shape.WrapType == WrapType.Inline)
                return;

            // Allow or disallow a shape in a particular story.
            switch (currentStoryType)
            {
                case StoryType.Textbox:
                case StoryType.Comments:
                case StoryType.Endnotes:
                case StoryType.Footnotes:
                {
                    // It looks like only inline pictures, ole objects and word art are allowed in these stories.
                    // But I let all shape types, just force them to become inline.
                    Warn(WarningType.Hint, WarningSource.Validator, WarningStrings.UnexpectedStoryForShape);

                    bool anchoredToPage =
                        (shape.RelativeHorizontalPosition == RelativeHorizontalPosition.Page) ||
                        (shape.RelativeVerticalPosition == RelativeVerticalPosition.Page);
                    if ((shape.MarkupLanguage == ShapeMarkupLanguage.Vml) && anchoredToPage)
                    {
                        // WORDSNET-20143 Move the floating shape to the end of the body if it's anchored to a page.
                        Document doc = shape.Document as Document;
                        CompositeNode paragraph = doc.LastSection.Body.LastParagraph;
                        if (paragraph != null)
                            paragraph.AppendChild(shape);
                    }
                    else
                    {
                        shape.WrapType = WrapType.Inline;
                    }

                    break;
                }
                default:
                    // Do nothing.
                    break;
            }
        }

        private void ValidateTextPlainTextDecoration(ShapeBase shape)
        {
            if ((shape.ShapeType == ShapeType.TextPlainText) &&
                (shape.MarkupLanguage == ShapeMarkupLanguage.Vml) &&
                (shape.ShapePr[ShapeAttr.GeoTextUnderline] != null))
                Warn(WarningType.MajorFormattingLoss, WarningSource.Shapes, WarningStrings.TextPlainTextDecorationInVml);
        }

        private VisitorAction VisitDrawingMLStart(ShapeBase drawingML, StoryTypeStack storyTypeStack)
        {
            mBookmarkValidator.VisitDrawingMLStart(drawingML);

            ValidateFloatingStory(drawingML, storyTypeStack.Current);

            // WORDSNET-26420 Validate relative positions.
            ValidateRelativePositions(drawingML);

            // Need to update size and position of drawingML.
            drawingML.UpdateSizeAndPositionFromRelative();

            // WORDSNET-9493 Make size of DrawingML specified on top level and in Dml node the same.
            UpdateDmlNodeTransform(drawingML);

            mDrawingMLIdValidator.VisitDrawingML(drawingML);

            // Validate only top-level drawingMLs.
            if (!drawingML.IsTopLevel)
                return VisitorAction.Continue;

            // WORDSNET-23287 After clone/import, the FallBack node still references the
            // original document. Pin it to the current document's NullNode so its Document
            // accessor returns the right doc.
            drawingML.UpdateFallBackParent();

            // WORDSNET-12320 When saving to OOXML, a DML shape that uses wp14 drawing
            // extensions (RelativeWidth / RelativeHeight) requires the document to be at
            // least IsoTransitional so the writer emits the wp14 namespace.
            if (mSaveInfo.IsOoxmlFormat && drawingML.HasDrawingExtensions)
                OoxmlComplianceInfo.MarkAsHasDrawingExtensions(mSaveInfo.Document);

            // FOSS Mirror of master's DmlShapeValidator.ProcessFallback ->
            // ReplaceDmlWithFallBackIfNeeded path. Pure-model operations only:
            // - For Ecma376_2006 (no mc:AlternateContent in that spec): replace the DML
            //   with the loaded VML fallback if one exists. Without a loaded fallback we
            //   leave the DML in place (master would generate one via rendering, which
            //   FOSS cannot do).
            // - For IsoTransitional / IsoStrict: visit the fallback so DrawingMLIdValidator
            //   registers its ids and gives it a unique nvPr.Id distinct from the choice's.
            if (drawingML.FallbackShape != null && mSaveInfo.IsOoxmlFormat)
            {
                if (mSaveInfo.OoxmlCompliance == OoxmlComplianceCore.Ecma376)
                {
                    DmlToVml.DmlUtil.ReplaceDmlWithFallBack(drawingML, mValidator);
                }
                else
                {
                    drawingML.FallbackShape.Accept(mValidator);
                }
            }

            // Return if drawingML was removed from document.
            if (drawingML.ParentNode == null)
                return VisitorAction.SkipThisNode;

            mSaveInfo.AddToZOrderList(drawingML, storyTypeStack);

            ValidateLayoutInCell(drawingML);

            return VisitorAction.Continue;
        }

        private VisitorAction VisitDrawingMLEnd(ShapeBase drawingML)
        {
            mBookmarkValidator.VisitDrawingMLEnd(drawingML);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Makes sure size of DrawingML node set in its xfrm.ext is the same as specified in DrawingML extent.
        /// </summary>
        private static void UpdateDmlNodeTransform(ShapeBase dml)
        {
            // Update size of drawingMl only if it has textbox content.
            // Other drawings are properly scaled using DmlToApsConverterResult.ScaleToSize.
            if (!dml.HasTextbox || !dml.IsTopLevel)
                return;

            // For top level shapes Width is in points, for child shapes Width is in the parent coordinate system.
            double widthEmu = ConvertUtilCore.PointToEmu(dml.Width);
            double heightEmu = ConvertUtilCore.PointToEmu(dml.Height);

            DmlTransform transform = dml.DmlNode.Transform;

            if (!MathUtil.IsZero(widthEmu))
                transform.Width = widthEmu;

            if (!MathUtil.IsZero(heightEmu))
                transform.Height = heightEmu;
        }

        /// <summary>
        /// Validates linked textboxes chains.
        /// </summary>
        private void ValidateLinkedTextboxes()
        {
            Dictionary<string, ShapeBase> shapeNames = new Dictionary<string, ShapeBase>();

            IEnumerator<int> enumerator = mSaveInfo.LinkedShapeIds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ShapeBase shape = GetShape(enumerator.Current);

                // Check that there is no conflicting name in linked textboxes.
                if (StringUtil.HasChars(shape.Name))
                {
                    if (shapeNames.ContainsKey(shape.Name))
                    {
                        // Textboxes must be written linked by ShapeId.
                        mSaveInfo.LinkedShapeNameConflict = true;
                    }
                    else
                    {
                        shapeNames[shape.Name] = shape;
                    }
                }

                // First textbox in chain must contain paragraph.
                if (IsFirstInChain(shape))
                {
                    if (shape.GetChildNodes(NodeType.Any, false).Count == 0)
                        shape.AppendChildForLoad(new Paragraph(mSaveInfo.Document));

                    // AM. Word moves any content of linked textboxes into first textbox but it's not just add content.
                    // Instead it moves content into last composite node i.e paragraph in this case. We can be sure that at least
                    // one paragraph exists in first textbox after above validation.
                    ShapeBase nextShape = GetNextShape(shape);
                    while (nextShape != null)
                    {
                        if (nextShape.GetChildNodes(NodeType.Any, false).Count > 0)
                            MoveNodes(nextShape, GetLastCompositeChildren(shape));

                        nextShape = GetNextShape(nextShape);
                    }
                }
            }

            // Check if vertical anchor is lost on saving.
            if (mHasNotDefaultBoxVerticalAnchor)
            {
                switch (mSaveInfo.SaveFormat)
                {
                    case SaveFormat.Doc:
                    case SaveFormat.Dot:
                        Warn(WarningType.DataLoss, WarningSource.Shapes, WarningStrings.TextboxVerticalAnchor2007Only);
                        break;
                    case SaveFormat.WordML:
                        Warn(WarningType.DataLoss, WarningSource.Shapes,
                             string.Format(WarningStrings.TextboxVerticalAnchorNotSupported));
                        break;
                    default:
                        // Textbox vertical alignment will be saved as expected.
                        // Currently AW can't save textbox vertical alignment for RTF and ODT formats.
                        break;
                }
            }
        }

        /// <summary>
        /// Returns shape with given shapeId or null if not found.
        /// </summary>
        private ShapeBase GetShape(int shapeId)
        {
            return mSaveInfo.Shapes[shapeId];
        }

        /// <summary>
        /// Returns next linked shape.
        /// </summary>
        private ShapeBase GetNextShape(ShapeBase shape)
        {
            return GetShape(shape.TextboxNextShapeId);
        }

        /// <summary>
        /// Move nodes from one composite node to another composite node. Removes source composite node.
        /// </summary>
        private static void MoveNodes(CompositeNode srcNode, CompositeNode dstNode)
        {
            // Lock node collection.
            Node[] childNodes = srcNode.GetChildNodes(NodeType.Any, false).ToArray();

            foreach (Node node in childNodes)
            {
                if (node.IsComposite && !dstNode.CanInsert(node))
                {
                    MoveNodes((CompositeNode)node, dstNode);
                    node.Remove();
                }
                else
                {
                    // AM. Some nodes can be lost here.
                    // Will look how Word moves nodes if such situation is occurred.
                    if (dstNode.CanInsert(node))
                        dstNode.AppendChild(node);
                }
            }
        }

        /// <summary>
        /// Returns last composite child node.
        /// </summary>
        private static CompositeNode GetLastCompositeChildren(CompositeNode node)
        {
            Node lastCompositeChildNode = null;

            foreach (Node childNode in node.GetChildNodes(NodeType.Any, false))
            {
                if (childNode.IsComposite)
                    lastCompositeChildNode = childNode;
            }

            return (CompositeNode)lastCompositeChildNode;
        }

        /// <summary>
        /// Checks whether given Shape is first shape in linked textboxes chain.
        /// </summary>
        /// <remarks>
        /// AM. Think that count of linked textboxes is small so do it in straight way.
        /// </remarks>
        private bool IsFirstInChain(ShapeBase shape)
        {
            IEnumerator<int> enumerator = mSaveInfo.LinkedShapeIds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (GetShape(enumerator.Current).TextboxNextShapeId == shape.Id)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all bits of the specified save actions are set in the current save operation.
        /// </summary>
        private bool IsSaveAction(DocumentValidatorActions actions)
        {
            return ((mSaveActions & actions) == actions);
        }

        internal void Finish()
        {
            mShapeIdGenerator.EndDocument();

            // This should be executed after mShapeIdGenerator finished.
            ValidateLinkedTextboxes();
        }

        internal void Revert()
        {
            // FOSS
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType type, WarningSource source, string description)
        {
            if (WarningCallback != null)
                WarningCallback.Warning(new WarningInfo(type, source, description));
        }

        private IWarningCallback WarningCallback
        {
            get { return mSaveInfo.Document.WarningCallback; }
        }

        /// <summary>
        /// Returns "true" when destination document stores as XML.
        /// </summary>
        private bool IsSaveFormatXml
        {
            get { return mSaveInfo.IsOoxmlFormat || (mSaveInfo.SaveFormat == SaveFormat.WordML); }
        }

        /// <summary>
        /// Returns "true" when any shape has not default vertical alignment and can be lost on saving.
        /// </summary>
        private bool mHasNotDefaultBoxVerticalAnchor { get; set; }

        private readonly SaveInfo mSaveInfo;
        private readonly DocumentValidatorActions mSaveActions;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentVisitor mValidator;
        private BookmarkValidator mBookmarkValidator;

        private readonly ShapeIdGenerator mShapeIdGenerator;
        private readonly ShapeImageOptimizer mShapeImageOptimizer;
        private DrawingMLIdValidator mDrawingMLIdValidator;

        private readonly HashSetGeneric<Guid> mUsedSignatureLineIds = new HashSetGeneric<Guid>();
    }
}
