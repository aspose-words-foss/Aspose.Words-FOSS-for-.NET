// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/22/2020 by Artem Ptitsin.

using System;
using System.Drawing;
using System.IO;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words
{
    /// <summary>
    /// Represents class to work with document watermark.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-watermark/">Working with Watermark</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// WORDSNET-4879 This class has been added to working with a watermark.
    /// </dev>
    public sealed class Watermark
    {
        internal Watermark(Document doc, IWatermarkProvider watermarkProvider)
        {
            mDoc = doc;
            mWatermarkProvider = watermarkProvider;
        }

        /// <summary>
        /// Adds Text watermark into the document.
        /// </summary>
        /// <param name="text">Text that is displayed as a watermark.</param>
        /// <remarks>
        /// The text length must be in the range from 1 to 200 inclusive.
        /// The text cannot be <c>null</c> or contain only whitespaces.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the text length is out of range or the text contains only whitespaces.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Throws when the text is <c>null</c>.
        /// </exception>
        public void SetText(string text)
        {
            SetText(text, new TextWatermarkOptions());
        }

        /// <summary>
        /// Adds Text watermark into the document.
        /// </summary>
        /// <param name="text">Text that is displayed as a watermark.</param>
        /// <param name="options">Defines additional options for the text watermark.</param>
        /// <remarks>
        /// The text length must be in the range from 1 to 200 inclusive.
        /// The text cannot be <c>null</c> or contain only whitespaces.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the text length is out of range or the text contain only whitespaces.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Throws when the text is <c>null</c>.
        /// </exception>
        /// <remarks>If <see cref="TextWatermarkOptions"/> is <c>null</c>, the watermark will be set with default options.</remarks>
        public void SetText(string text, TextWatermarkOptions options)
        {
            ValidateText(text);
            TextWatermarkOptions localOptions = options == null ? new TextWatermarkOptions() : options;

            Shape watermark = CreateWatermark(text, localOptions, mDoc);
            Add(watermark);
        }

        /// <summary>
        /// Adds Image watermark into the document.
        /// </summary>
        /// <param name="image">Image that is displayed as a watermark.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when the image is <c>null</c>.
        /// </exception>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public void SetImage(Image image)
        {
            SetImage(image, new ImageWatermarkOptions());
        }

        /// <summary>
        /// Adds Image watermark into the document.
        /// </summary>
        /// <param name="image">Image that is displayed as a watermark.</param>
        /// <param name="options">Defines additional options for the image watermark.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when the image is <c>null</c>.
        /// </exception>
        /// <remarks>If <see cref="ImageWatermarkOptions"/> is <c>null</c>, the watermark will be set with default options.</remarks>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public void SetImage(Image image, ImageWatermarkOptions options)
        {
            ArgumentUtil.CheckNotNull(image, "image");

            Shape watermark = new Shape(mDoc, ShapeType.Image);
            watermark.ImageData.SetImage(image);

            AddWatermark(watermark, options, mDoc);
        }

        /// <summary>
        /// Adds Image watermark into the document.
        /// </summary>
        /// <param name="imagePath">Path to the image file that is displayed as a watermark.</param>
        /// <param name="options">Defines additional options for the image watermark.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when the path is <c>null</c>.
        /// </exception>
        /// <remarks>If <see cref="ImageWatermarkOptions"/> is <c>null</c>, the watermark will be set with default options.</remarks>
        public void SetImage(string imagePath, ImageWatermarkOptions options)
        {
            ArgumentUtil.CheckNotNull(imagePath, "imagePath");

            Shape watermark = new Shape(mDoc, ShapeType.Image);
            watermark.ImageData.SetImage(imagePath);

            AddWatermark(watermark, options, mDoc);
        }

        /// <summary>
        /// Adds Image watermark into the document.
        /// </summary>
        /// <param name="imageStream">The stream containing the image data that is displayed as a watermark.</param>
        /// <param name="options">Defines additional options for the image watermark.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws when the path is <c>null</c>.
        /// </exception>
        /// <remarks>If <see cref="ImageWatermarkOptions"/> is <c>null</c>, the watermark will be set with default options.</remarks>
        public void SetImage(Stream imageStream, ImageWatermarkOptions options)
        {
            ArgumentUtil.CheckNotNull(imageStream, "imageStream");

            Shape watermark = new Shape(mDoc, ShapeType.Image);
            watermark.ImageData.SetImage(imageStream);

            AddWatermark(watermark, options, mDoc);
        }

        /// <summary>
        /// Gets the watermark type.
        /// </summary>
        public WatermarkType Type
        {
            get
            {
                Shape watermark = mWatermarkProvider.Get();

                if (watermark == null)
                    return WatermarkType.None;

                // IWatermarkProvider.Get should return the watermark or null.
                Debug.Assert(watermark.IsWatermark);

                if (watermark.CanBeTextWatermark)
                    return WatermarkType.Text;

                if (watermark.CanBeImageWatermark)
                    return WatermarkType.Image;

                return WatermarkType.None;
            }
        }

        private void Add(Shape shape)
        {
            Remove();
            mWatermarkProvider.Add(shape);
        }

        /// <summary>
        /// Removes the watermark.
        /// </summary>
        public void Remove()
        {
            mWatermarkProvider.Remove();
        }

        private void ValidateText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (StringUtil.ContainsOnlyWhitespaces(text))
                throw new ArgumentOutOfRangeException("text");

            ArgumentUtil.ValidateRange(text.Length, 0, 0, 200, 200, true, "text");
        }

        private void AddWatermark(Shape watermark, ImageWatermarkOptions options, Document doc)
        {
            if (options == null)
                options = new ImageWatermarkOptions();

            SetImageSize(watermark, options, doc);

            if (options.IsWashout)
            {
                watermark.ImageData.Contrast = 0.15d;
                watermark.ImageData.Brightness = 0.85d;
            }

            SetGeneralAttributes(watermark, ImageNamePrefix);
            Add(watermark);
        }

        private void SetImageSize(Shape watermark, ImageWatermarkOptions options, Document doc)
        {
            double scale = options.Scale;
            double initialWidth = watermark.ImageData.ImageSize.WidthPoints;
            double initialHeight = watermark.ImageData.ImageSize.HeightPoints;

            if (options.IsScaleAuto)
            {
                PageSetup pageSetup = doc.FirstSection.PageSetup;
                float maxWidth = pageSetup.ContentWidth;
                float maxHeight = pageSetup.ContentHeight;

                scale = maxWidth / initialWidth;
                if ((initialHeight * scale) > maxHeight)
                    scale = maxHeight / initialHeight;
            }

            watermark.SetWidthSafe(System.Math.Round(initialWidth * scale, 2, MidpointRounding.AwayFromZero));
            watermark.SetHeightSafe(System.Math.Round(initialHeight * scale, 2, MidpointRounding.AwayFromZero));
        }

        private Shape CreateWatermark(string text, TextWatermarkOptions options, Document doc)
        {
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);

            watermark.TextPath.Text = text;
            watermark.TextPath.FontFamily = options.FontFamily;
            watermark.Rotation = (int)options.Layout;

            if (options.IsSemitrasparent)
                watermark.Fill.Opacity = 0.5;

            SetTextSize(watermark, options, doc);
            watermark.Fill.ForeColor = options.Color;
            watermark.StrokeColor = options.Color;
            watermark.Font.Size = 1;

            SetGeneralAttributes(watermark, TextNamePrefix);

            return watermark;
        }

        private void SetTextSize(Shape watermark, TextWatermarkOptions options, Document doc)
        {
            float fontSize = options.IsAutoSize ? 1 : options.FontSize;
            DrFont drFont = doc.FontProvider.FetchDrFont(watermark.TextPath.FontFamily, fontSize, FontStyle.Regular);
            SizeF size = drFont.GetTextSizePoints(watermark.TextPath.Text);

            if (options.IsAutoSize)
                size = GetTextAutoSize(size, options, doc);

            watermark.SetWidthSafe(System.Math.Round(size.Width, 2, MidpointRounding.AwayFromZero));
            watermark.SetHeightSafe(System.Math.Round(size.Height, 2, MidpointRounding.AwayFromZero));
        }

        private SizeF GetTextAutoSize(SizeF textSize, TextWatermarkOptions options, Document doc)
        {
            PageSetup pageSetup = doc.FirstSection.PageSetup;
            float maxWidth = pageSetup.ContentWidth;
            float maxHeight = pageSetup.ContentHeight;
            float ratio = textSize.Height / textSize.Width;

            if (options.Layout == WatermarkLayout.Diagonal)
            {
                // This calculation will only work for angles of 45, 135, 225, 315 degrees.
                float minSide = System.Math.Min(maxHeight, maxWidth);
                float width = (minSide * (float)System.Math.Sqrt(2)) / (1 + ratio);

                return new SizeF(width, width * ratio);
            }

            return (textSize.Height >= textSize.Width)
                ? new SizeF(maxHeight / ratio, maxHeight)
                : new SizeF(maxWidth, maxWidth * ratio);
        }

        private void SetGeneralAttributes(Shape watermark, string name)
        {
            watermark.Name = string.Format("{0}{1}", name, watermark.Id);

            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Margin;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Margin;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = VerticalAlignment.Center;
            watermark.HorizontalAlignment = HorizontalAlignment.Center;
            // WORDSNET-24431 MS Word sets this property for a watermark, otherwise the watermark is drawn over
            // header/footer text.
            watermark.BehindText = true;
        }

        internal const string TextNamePrefix = "PowerPlusWaterMarkObject";
        internal const string ImageNamePrefix = "WordPictureWatermark";

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDoc;
        private readonly IWatermarkProvider mWatermarkProvider;
    }
}
