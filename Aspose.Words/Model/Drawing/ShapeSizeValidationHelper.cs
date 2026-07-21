// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2015 by Alexander Zhiltsov
using System;
using Aspose.Common;
using Aspose.Images;
using Aspose.Words.Tables;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// This class provides implementation for shape sizing and size validation logic 
    /// to use in the <see cref="ShapeBase"/> and <see cref="Shape"/> classes.
    /// </summary>
    internal class ShapeSizeValidationHelper
    {
        /// <summary>
        /// Validates that the shape width or height is > 0 and less than the maximum value in points for top 
        /// level shapes. Zero width or height is somewhat ugly, but it is valid (for horizontal line shape height 
        /// for example) so we allow it.
        /// </summary>
        /// <param name="shape">The shape to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="isThrow">Whether to throw or return a maximum/minimum valid value.</param>
        /// <param name="dimension">The "width" or "height" word.</param>
        internal static double ValidateDimension(ShapeBase shape, double value, bool isThrow, string dimension)
        {
            if (value < 0)
            {
                if (isThrow)
                    throw new ArgumentOutOfRangeException("value", String.Format("Shape {0} cannot be less than 0.",
                        dimension));
                else
                    return 0;
            }
            else if ((value > MaxShapeSize) && shape.IsTopLevel)
            {
                // RK While the shape is not added to the model we cannot really validate properly, therefore 
                // we allow any values. It's okay, they will be validated by DocumentValidator on save.
                if (shape.ParentNode == null)
                    return value;

                if (isThrow)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        String.Format("Shape {0} cannot be greater than {1} points.", dimension,
                            FormatterPal.DoubleToStr(MaxShapeSize)));
                }
                else
                {
                    return MinShapeSize;
                }
            }
            else
                return value;
        }

        /// <summary>
        /// Returns size of image scaled to either parent container or <see cref="MaxShapeSize"/> if needed.
        /// </summary>
        internal static SizeD GetScaledImageSize(ShapeBase shape, double width, double height, SizeD imageSize, double rotationAng)
        {
           imageSize = GetShapeImageSize(shape, width, height, imageSize);

            bool isScaleRequired = IsScaleRequired(shape, width, height, imageSize);
            SizeD containerSize = (isScaleRequired)
                ? GetContainerSize(shape, (Paragraph)shape.GetAncestor(NodeType.Paragraph))
                : new SizeD(MaxShapeSize, MaxShapeSize);

            imageSize = ScaleToFitContainer(imageSize, containerSize, rotationAng);

            return imageSize;
        }

        /// <summary>
        /// Checks if specified width and height exceeds <see cref="MaxShapeSize"/>.
        /// </summary>
        private static bool IsExceedsMaxShapeSize(SizeD size)
        {
            return ((size.Width >= MaxShapeSize) || (size.Height >= MaxShapeSize));
        }

        /// <summary>
        /// Returns minimal size of page content. Considers page text columns.
        /// </summary>
        private static SizeD GetPageMinContentSize(ShapeBase shape, Paragraph parentParagraph)
        {
            Section parentSection = (Section)parentParagraph.GetAncestor(NodeType.Section);
            if (parentSection == null)
                return new SizeD();

            PageSetup ps = parentSection.PageSetup;

            double leftOnPage = GetLeftRelatedToPage(shape, ps);
            double topOnPage = GetTopRelatedToPage(shape, ps);

            double tolerance = ConvertUtilCore.MmToPoint(1); // 1 mm
            bool isOutsideMargins = ps.ContentLeft - leftOnPage > tolerance || ps.ContentTop - topOnPage > tolerance;

            // WORDSNET-12462 If a shape is placed inside page margins, scale it to be inside them,
            // if a shape is placed outside page margins, scale it to be inside page.
            double allowedWidth = isOutsideMargins ? ps.PageWidth - leftOnPage : ps.ContentRight - leftOnPage;
            double allowedHeight = isOutsideMargins ? ps.PageHeight - topOnPage : ps.ContentBottom - topOnPage;

            TextColumnCollectionInternal textColumns = (TextColumnCollectionInternal)parentSection.SectPr[SectAttr.Columns];
            if (textColumns != null)
                for (int i = 0; i < textColumns.Count; i++)
                {
                    double colWidth = textColumns[i].Width;
                    if (!MathUtil.IsZero(colWidth))
                        allowedWidth = System.Math.Min(allowedWidth, colWidth);
                }

            return new SizeD(allowedWidth, allowedHeight);
        }

        /// <summary>
        /// Returns value of the Left property that is related to the left bound of the page.
        /// </summary>
        private static double GetLeftRelatedToPage(ShapeBase shape, PageSetup pageSetup)
        {
            // Same behavior as in Layout dialog of MS Word
            switch (shape.RelativeHorizontalPosition)
            {
                case RelativeHorizontalPosition.Margin:
                    return shape.Left + pageSetup.ContentLeft;
                case RelativeHorizontalPosition.Page:
                case RelativeHorizontalPosition.LeftMargin:
                case RelativeHorizontalPosition.InsideMargin:
                    return shape.Left;
                case RelativeHorizontalPosition.Column:
                case RelativeHorizontalPosition.Character:
                    return shape.Left + pageSetup.ContentLeft; // unknown now
                case RelativeHorizontalPosition.RightMargin:
                case RelativeHorizontalPosition.OutsideMargin:
                    return shape.Left + pageSetup.ContentRight;
                default:
                    Debug.Assert(false);
                    return shape.Left + pageSetup.ContentLeft;
            }
        }

        /// <summary>
        /// Returns value of the Top property that is related to the top bound of the page.
        /// </summary>
        private static double GetTopRelatedToPage(ShapeBase shape, PageSetup pageSetup)
        {
            // Same behavior as in Layout dialog of MS Word
            switch (shape.RelativeVerticalPosition)
            {
                case RelativeVerticalPosition.Margin:
                    return shape.Top + pageSetup.ContentTop;
                case RelativeVerticalPosition.Page:
                case RelativeVerticalPosition.TopMargin:
                case RelativeVerticalPosition.InsideMargin:
                    return shape.Top;
                case RelativeVerticalPosition.Paragraph:
                case RelativeVerticalPosition.Line:
                    return shape.Top + pageSetup.ContentTop; // unknown now
                case RelativeVerticalPosition.BottomMargin:
                case RelativeVerticalPosition.OutsideMargin:
                    return shape.Top + pageSetup.ContentBottom;
                default:
                    Debug.Assert(false);
                    return shape.Top + pageSetup.ContentTop;
            }
        }

        /// <summary>
        /// Checks if we need to scale the shape. 
        /// Returns true if shape size is not set explicitly
        /// and it is not exceeds the maximum shape size or
        /// parent shape size or parent cell size or maximum page content size.
        /// </summary>
        private static bool IsScaleRequired(ShapeBase shape, double width, double height, SizeD imageSize)
        {
            // If width and height is more than zero, then size was requested explicitly
            // and we should use this explicitly specified size for image
            // and shouldn't scale it to fit in container.
            if ((width > 0) && (height > 0))
                return false;

            // WORDSNET-21405 Requires scaling if the container is a textbox.
            Shape parentShape = shape.GetAncestor(NodeType.Shape) as Shape;
            if ((parentShape != null) && parentShape.IsTextBox)
                return true;

            // Otherwise we should use size from the image data and check if scale is required for it.
            bool scaleRequired = false;

            // Since to scale the shape we need to know the container size, we should make sure shape has a parent.
            if (shape.ParentNode != null)
            {
                Cell parentCell = (Cell)shape.GetAncestor(NodeType.Cell);
                if (IsExceedsMaxShapeSize(imageSize))
                {
                    // WORDSNET-20267 If table width is Auto then there is no container width to scale.
                    if ((parentCell == null) || !parentCell.ParentTable.AllowAutoFit)
                        scaleRequired = true;
                }
                else
                {
                    // andrnosk: WORDSNET-10174 MS Word scales large images to parent cells.
                    // We do the same but only when PreferredWidth is set.
                    if (parentCell != null)
                    {
                        if (!parentCell.ParentTable.AllowAutoFit)
                        {
                            PreferredWidth preferredCellWidth = parentCell.CellFormat.PreferredWidth;
                            if (preferredCellWidth.IsFixed && (imageSize.Width > preferredCellWidth.Value))
                                scaleRequired = true;
                        }
                    }
                    else
                    {
                        // WORDSNET-10718 MS Word scales large images to fit on page.
                        // It scales to the narrowest column regardless in which column this shape resides. 
                        // WORDSNET-18016 Shape might be inside SDT.
                        SizeD pageMinContentSize = GetPageMinContentSize(shape, (Paragraph)shape.GetAncestor(NodeType.Paragraph));
                        if (imageSize.IsExceeds(pageMinContentSize) && !pageMinContentSize.IsEmpty)
                            scaleRequired = true;
                    }
                }
            }

            return scaleRequired;
        }

        /// <summary>
        /// Returns size of the shape to occupy entire container (cell, textbox or page) size. 
        /// Keeps width/height ratio. 
        /// </summary>
        /// <param name="imageSize">Size of the image inside shape, it can be empty (0,0) for chart which does not have image data.</param>
        /// <param name="containerSize">Size of the container.</param>
        /// <param name="rotationAng">Rotation angle.</param>
        /// <returns></returns>
        internal static SizeD ScaleToFitContainer(SizeD imageSize, SizeD containerSize, double rotationAng)
        {
            // Check container size is valid.
            if (containerSize.IsEmpty)
                return imageSize;

            // If imageSize is empty - it is Chart, lets take default chart size and scale it to fit container.
            if (imageSize.IsEmpty)
                imageSize = new SizeD(DefaultChartWidth, DefaultChartHeight);
            // If image dimensions do not exceed container dimensions, then nothing to scale.
            else if ((imageSize.Width <= containerSize.Width) && (imageSize.Height <= containerSize.Height))
                return imageSize;

            SizeD scaledSize = new SizeD();

            int angle = MathUtil.DoubleToInt(MathUtil.NormalizeAngle(rotationAng));
            double containerHeigth = containerSize.Height;
            double containerWidth = containerSize.Width;

            if ((angle == 90) || (angle == 270))
            {
                containerHeigth = containerSize.Width;
                containerWidth = containerSize.Height;
            }

            double ratioWidth = imageSize.Width / containerWidth;
            double ratioHeight = imageSize.Height / containerHeigth;

            if (ratioWidth > ratioHeight)
            {
                scaledSize.Height = (containerHeigth * (ratioHeight / ratioWidth));
                scaledSize.Width = containerWidth;
            }
            else
            {
                scaledSize.Width = (containerWidth * (ratioWidth / ratioHeight));
                scaledSize.Height = containerHeigth;
            }

            return scaledSize;
        }

        /// <summary>
        /// Returns container size for the specified shape.
        /// </summary>
        internal static SizeD GetContainerSize(ShapeBase shape, Paragraph parentParagraph)
        {
            SizeD targetSize = new SizeD();

            // MS Word scales large images to parent shape or page size.
            // So first try to get parent shape (TextBox).
            Shape parentShape = (Shape)parentParagraph.GetAncestor(NodeType.Shape);
            Cell parentCell = (Cell)parentParagraph.GetAncestor(NodeType.Cell);
            if (parentShape != null)
            {
                targetSize.Width = parentShape.Width;
                targetSize.Height = parentShape.Height;
            }
            else if (parentCell != null)
            {
                // andrnosk: WORDSNET-10174 MS Word scales large images to parent cells.
                // We do the same but only when PreferredWidth is set. See the IsScaleRequired() method.
                CellFormat cellFormat = parentCell.CellFormat;
                if (cellFormat.PreferredWidth.IsFixed)
                    targetSize.Width = cellFormat.PreferredWidth.Value - cellFormat.LeftPadding - cellFormat.RightPadding;
                else
                    targetSize.Width = parentCell.CellFormat.Width;

                targetSize.Height = MaxShapeSize;
            }
            else
            {
                // WORDSNET-10718 MS Word scales to the narrowest column regardless in which column this shape resides. 
                SizeD pageMinContentSize = GetPageMinContentSize(shape, parentParagraph);
                if (!pageMinContentSize.IsEmpty)
                    targetSize = pageMinContentSize;
            }

            return targetSize;
        }

        /// <summary>
        /// Returns size from the specified width and height,
        /// replacing it with dimensions from the shape's image, if it is less than zero or equals zero.
        /// </summary>
        private static SizeD GetShapeImageSize(ShapeBase shape, double width, double height, SizeD imageSize)
        {
            if ((width > 0) && (height > 0))
                return new SizeD(width, height);

            if (imageSize != null)
                return imageSize;

            ImageSizeCore imageSizeCore = shape.GetShapeImageSize();
            double imageWidth = (width <= 0) ? imageSizeCore.WidthPoints : width;
            double imageHeight = (height <= 0) ? imageSizeCore.HeightPoints : height;

            return new SizeD(imageWidth, imageHeight);
        }

        /// <summary>
        /// Word cannot accept shape sizes more than this value in points. But it is not easy to validate here.
        /// </summary>
        internal const double MaxShapeSize = ConvertUtilCore.MaxSizePoint;

        /// <summary>
        /// Word set this value to shape dimensional in case they are greater than <see cref="MaxShapeSize" />.
        /// </summary>
        internal const double MinShapeSize = ConvertUtilCore.MinSizePoint;

        private const double DefaultChartWidth = 432;
        private const double DefaultChartHeight = 252;
    }
}
