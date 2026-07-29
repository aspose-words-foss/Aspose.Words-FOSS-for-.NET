// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2017 by Edward Voronov

using System;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    internal class FieldIncludePictureUpdater
    {
        internal static FieldUpdateAction Update(Field field)
        {
            FieldIncludePictureUpdater updater = new FieldIncludePictureUpdater(field, (IFieldIncludePictureCode)field);
            return updater.Update();
        }

        private FieldIncludePictureUpdater(Field field, IFieldIncludePictureCode fieldCode)
        {
            mField = field;
            mFieldCode = fieldCode;
        }

        private FieldUpdateAction Update()
        {
            if (!StringUtil.HasChars(mFieldCode.SourceFullName))
                return new FieldUpdateActionInsertErrorMessage(mField, InvalidFileNameErrorMessage);

            Shape shape = new Shape(mField.Document, ShapeType.Image);
            SetDefaultImageSize(shape);
            shape.WrapType = WrapType.Inline;
            shape.ImageData.SourceFullName = mFieldCode.SourceFullName;

            // And try to load it to know image dimensions.
            byte[] imageBytes = shape.ImageData.LoadImageBytes();
            if (imageBytes != null)
            {
                Size imageSize = GetImageSize(imageBytes);
                shape.SetWidthSafe(imageSize.Width);
                shape.SetHeightSafe(imageSize.Height);

                if (!mFieldCode.IsLinked)
                {
                    try
                    {
                        shape.ImageData.ImageBytes = imageBytes;
                    }
                    catch (CantCreateBitmapException)
                    {
                        if (mField.HasResult)
                            return new FieldUpdateActionDoNothing(mField);

                        SetDefaultImageSize(shape);
                    }
                }
            }
            else if (!mFieldCode.IsLinked)
            {
                return new FieldUpdateActionInsertErrorMessage(mField, InvalidFileNameErrorMessage);
            }

            return new FieldUpdateActionApplyResult(mField, new NodeRangeFieldResult(new NodeRange(shape, shape)));
        }

        private static void SetDefaultImageSize(Shape shape)
        {
            // Word shows empty 3in square picture if the linked image is not found.
            const double defaultImageDimension = 72 * 3;

            shape.SetWidthSafe(defaultImageDimension);
            shape.SetHeightSafe(defaultImageDimension);
        }

        private Size GetImageSize(byte[] imageBytes)
        {
            ImageSizeCore sizeCore = ImageUtil.GetImageSize(imageBytes);
            Size size = new Size(sizeCore.WidthPoints, sizeCore.HeightPoints);
            return AdjustImageSize(size);
        }

        private Size AdjustImageSize(Size imageSize)
        {
            imageSize = AdjustImageSizeToShape(imageSize);
            imageSize = AdjustImageSizeToFrame(imageSize);
            imageSize = AdjustImageSizeToTableCell(imageSize);
            imageSize = AdjustImageSizeToPage(imageSize);

            return imageSize;
        }

        private Size AdjustImageSizeToShape(Size imageSize)
        {
            Shape shape = mField.Start.ParentParagraph.ParentNode as Shape;
            if (shape == null)
                return imageSize;

            return FitWidthAndHeight(imageSize, shape.Width, shape.Height);
        }

        private Size AdjustImageSizeToFrame(Size imageSize)
        {
            Paragraph paragraph = mField.Start.ParentParagraph;
            ParaPr paraPr = paragraph.ParaPr;

            if (!paraPr.HasFrameAttributes)
                return imageSize;

            return paraPr.HasFrameWidth
                ? AdjustImageSizeToFrameWidth(paragraph.ParagraphFormat, imageSize)
                : AdjustImageSizeToFrameHeight(paragraph.ParagraphFormat, imageSize);
        }

        private static Size AdjustImageSizeToFrameWidth(ParagraphFormat format, Size imageSize)
        {
            switch (format.FrameHeightRule)
            {
                case HeightRule.Exactly:
                    return FitToFrameSize(format, imageSize);
                case HeightRule.AtLeast:
                case HeightRule.Auto:
                    return FitToFrameWidth(format, imageSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Size AdjustImageSizeToFrameHeight(ParagraphFormat format, Size imageSize)
        {
            // MS Word behaviour is more difficult. Picture size increases from very small
            // up to source image size or frame height (depend on frame height rule) each time field is updated.
            switch (format.FrameHeightRule)
            {
                case HeightRule.Exactly:
                    return FitToFrameHeight(format, imageSize);
                case HeightRule.AtLeast:
                case HeightRule.Auto:
                    return imageSize;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Size FitToFrameSize(ParagraphFormat paraPr, Size imageSize)
        {
            return FitWidthAndHeight(imageSize, paraPr.FrameWidth, paraPr.FrameHeight);
        }

        private static Size FitToFrameWidth(ParagraphFormat paraPr, Size imageSize)
        {
            double frameWidth = paraPr.FrameWidth;
            return FitWidth(imageSize, frameWidth);
        }

        private static Size FitToFrameHeight(ParagraphFormat paraPr, Size imageSize)
        {
            double frameHeight = paraPr.FrameHeight;
            return FitHeight(imageSize, frameHeight);
        }

        private Size AdjustImageSizeToTableCell(Size imageSize)
        {
            Cell cell = mField.Start.ParentParagraph.ParentCell;
            if (cell == null)
                return imageSize;

            Table table = cell.ParentRow.ParentTable;
            if (table.AllowAutoFit)
                return imageSize;

            CellFormat cellFormat = cell.CellFormat;
            double width = cellFormat.Width - cellFormat.LeftPadding - cellFormat.RightPadding;
            return FitWidth(imageSize, width);
        }

        private Size AdjustImageSizeToPage(Size imageSize)
        {
            Section section = (Section)mField.Start.GetAncestor(NodeType.Section);
            return FitWidthAndHeight(imageSize, section.PageSetup.ContentWidth, section.PageSetup.ContentHeight);
        }

        private static Size FitHeight(Size imageSize, double height)
        {
            if (imageSize.Height < height)
                return imageSize;

            double scale = height / imageSize.Height;

            return imageSize.Scale(scale);
        }

        private static Size FitWidth(Size imageSize, double width)
        {
            if (imageSize.Width < width)
                return imageSize;

            double scale = width / imageSize.Width;

            return imageSize.Scale(scale);
        }

        private static Size FitWidthAndHeight(Size imageSize, double width, double height)
        {
            // MS Word behaviour is more difficult. It calculates picture size in same way, but with small deviations (+/- 1 pt).
            if (imageSize.Width < width && imageSize.Height < height)
                return imageSize;

            double scale = System.Math.Min(width / imageSize.Width, height / imageSize.Height);

            return imageSize.Scale(scale);
        }

        private const string InvalidFileNameErrorMessage = "Error! Filename not specified.";

        private readonly Field mField;
        private readonly IFieldIncludePictureCode mFieldCode;

        private class Size
        {
            public Size(double width, double height)
            {
                Width = width;
                Height = height;
            }

            public double Width { get; }

            public double Height { get; }

            public Size Scale(double scale)
            {
                return new Size(Width * scale, Height * scale);
            }
        }
    }
}
