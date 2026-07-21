// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2006 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Factories;
using CodePorting.Translator.Cs2Cpp;

#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words.Drawing
{
#if NETSTANDARD
    /// <summary>
    /// Defines an image for a shape.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-images/">Working with Images</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.ImageData"/> property to access and modify the image inside a shape.
    /// You do not create instances of the <see cref="ImageData"/> class directly.</p>
    ///
    /// <p>An image can be stored inside a shape, linked to external file or both(linked and stored in the document).</p>
    /// <p>Regardless of whether the image is stored inside the shape or linked, you can always access the actual
    /// image using the <see cref="ToByteArray"/>, <ms><see cref = "ToStream" /></ms><cpp><see cref="ToStream"/></cpp> or <see cref ="Save(string)"/> methods.
    /// If the image is stored inside the shape, you can also directly access it using the <see cref="ImageBytes" /> property.</p>
    /// <p>To store an image inside a shape use the<see cref = "SetImage(string)"/> method.To link an image to a shape, set the<see cref = "SourceFullName" /> property.</p>
    /// </remarks>
#else
    /// <summary>
    /// Defines an image for a shape.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-images/">Working with Images</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Use the <see cref="Shape.ImageData"/> property to access and modify the image inside a shape.
    /// You do not create instances of the <see cref="ImageData"/> class directly.</p>
    ///
    /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="IImageData.Ctor"]/*'/>
    /// </remarks>
#endif
    public class ImageData : IBorderAttrSource
    {
        /// <summary>
        /// Ctor.
        /// Document is needed to resolve relative image URIs by some of the methods.
        /// Document might be null if in glossary, let's think what to do with them.
        /// </summary>
        internal ImageData(ShapeBase parent, Document doc)
        {
            Debug.Assert(parent != null);
            mParent = parent;

            mImageDataCore = (parent.MarkupLanguage == ShapeMarkupLanguage.Vml)
                ? new ImageDataCore(doc, new ImageDataSource(parent))
                : new ImageDataCore(doc, new DrawingMLImageDataSource(parent));
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="image">The image object.</param>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public void SetImage(Image image)
        {
            mImageDataCore.SetImage(image);
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="stream">The stream that contains the image.
        /// <java> The stream will be read from the current position, so one should be careful about stream position.</java></param>
        /// <javaName>void setImage(java.io.InputStream stream)</javaName>
#if PLAIN_JAVA // WORDSJAVA-25686 - Loading from InputStream always load into memory first
        //JAVA-added public wrapper for internalized member
        public void setImage(java.io.InputStream stream) throws Exception
        {
            setImage(com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyToMemoryStream(stream));
        }
#endif
        [JavaInternal]
        public void SetImage([CppIOStreamWrapper(IOStreamType.IStream)] Stream stream)
        {
            mImageDataCore.SetImage(stream);
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="fileName">The image file. Can be a file name or a URL.</param>
        public void SetImage(string fileName)
        {
            mImageDataCore.SetImage(fileName);
        }

        /// <summary>
        /// Gets the image stored in the shape as a <ms><see cref="Image"/></ms><java>java <tt>BufferedImage</tt></java><cpp><see cref="Image"/></cpp> object.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     <ms><p>A new <see cref="Image"/> object is created every time this method is called.</p></ms>
        ///     <cpp><p>A new <see cref="Image"/> object is created every time this method is called.</p></cpp>
        ///     <java>
        ///         <p>Tries to create a new <tt>java.awt.image.BufferedImage</tt> object from image bytes every time this method is called.
        /// If <tt>javax.imageio.ImageReader</tt> can't read image bytes (emf, wmf, tiff, etc.) the method returns <c>null</c>.</p>
        ///     </java>
        ///     <p>It is the responsibility of the caller to dispose the image object.</p>
        /// </remarks>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public Image ToImage()
        {
            return mImageDataCore.ToImage();
        }

        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="IImageData.ToStream"]/*'/>
        public Stream ToStream()
        {
            return mImageDataCore.ToStream();
        }

        /// <summary>
        /// Returns image bytes for any image regardless whether the image is stored or linked.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     <para>If the image is linked, downloads the image every time it is called.</para>
        ///     <seealso cref="ImageBytes"/>
        /// </remarks>
        public byte[] ToByteArray()
        {
            return mImageDataCore.ToByteArray();
        }

        /// <summary>
        /// Saves the image into the specified stream.
        /// </summary>
        /// <param name="stream">The stream where to save the image to.</param>
        /// <overloads>Saves the image of the shape.</overloads>
        /// <remarks>
        ///     <p>Is it the responsibility of the caller to dispose the stream object.</p>
        /// </remarks>
        /// <javaName>void save(java.io.OutputStream stream)</javaName>
#if PLAIN_JAVA    // WORDSJAVA-25685 - Saving to OutputStream always writes to memory first
        //JAVA-added public wrapper for internalized member
        public void save(java.io.OutputStream stream) throws Exception
        {
            com.aspose.ms.System.IO.MemoryStream tempStream = new com.aspose.ms.System.IO.MemoryStream();
            save(tempStream);
            tempStream.setPosition(0);
            com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyStream(tempStream, stream);
        }
#endif
        [JavaInternal]
        public void Save([CppIOStreamWrapper(IOStreamType.OStream)] Stream stream)
        {
            mImageDataCore.Save(stream);
        }

        /// <summary>
        /// Saves the image into a file.
        /// </summary>
        /// <param name="fileName">The file name where to save the image.</param>
        public void Save(string fileName)
        {
            mImageDataCore.Save(fileName);
        }

        /// <summary>
        /// Fits the image data to Shape frame so that the aspect ratio of the image data matches the aspect ratio of Shape frame.
        /// </summary>
        public void FitImageToShape()
        {
            if (mParent.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                DmlBlipFill blipFill;
                DmlNode dmlNode = mParent.DmlNode;
                if (dmlNode == null)
                    return;

                switch (dmlNode.DmlNodeType)
                {
                    case DmlNodeType.Picture:
                        DmlPicture dmlPicture = (DmlPicture)dmlNode;
                        blipFill = dmlPicture.BlipFill;
                        break;
                    case DmlNodeType.WordprocessingShape:
                        DmlShape dmlShape = (DmlShape)dmlNode;
                        blipFill = dmlShape.Fill as DmlBlipFill;
                        break;
                    default:
                        return;
                }

                if (blipFill == null)
                    return;

                ImageSize imageSize = ImageSize;
                if (MathUtil.AreEqual(mParent.Width, 0) || MathUtil.AreEqual(mParent.Height, 0) || !imageSize.IsValid)
                    return;

                double shapeRatio = mParent.Width / mParent.Height;
                double imageRatio = (double)imageSize.WidthPixels / imageSize.HeightPixels;
                if (MathUtil.AreEqual(shapeRatio, imageRatio))
                    return;

                DmlBlipFillStretch stretch = new DmlBlipFillStretch();
                if (shapeRatio > imageRatio)
                {
                    // Offsets are at left and right.
                    double imageRelativeWidth = imageRatio / shapeRatio;
                    double offset = (1 - imageRelativeWidth) / 2;
                    stretch.FillRectangle = new DmlPercentageOffsetRectangle(offset, 0, offset, 0);
                }
                else
                {
                    // Offsets are at top and bottom.
                    double imageRelativeHeight = shapeRatio / imageRatio;
                    double offset = (1 - imageRelativeHeight) / 2;
                    stretch.FillRectangle = new DmlPercentageOffsetRectangle(0, offset, 0, offset);
                }

                blipFill.BlipFillMode = stretch;
            }
            else
            {
                // A VML shape can have two defined images: a shape main image and an image specified as Fill. A fill
                // image can be displayed with keeping aspect ratio; it is drawn behind a shape image.
                // So, let's convert the shape main image into a fill image.
                // (Although we can simply remove the fill image, i.e. remove value of the ShapeAttr.FillImageBytes
                // attribute here, and Shape.ReplaceVmlImageDataWithFill will be automatically run on validation,
                // explicit calling the method looks more descriptive.)

                Shape shape = (Shape)mParent;
                if ((ImageBytes != null) && (ImageBytes.Length > 0))
                    shape.ReplaceVmlImageDataWithFill();
                shape.FillCore.LockAspectRatio = true;
            }
        }

#if NETSTANDARD
        /// <summary>
        /// Gets or sets the raw bytes of the image stored in the shape.
        /// </summary>
        /// <remarks>
        /// <para>Setting the value to <c>null</c> or an empty array will remove the image from the shape.</para>
        /// <p>Returns<c>null</c> if the image is not stored in the document (e.g the image is probably linked in this case).</p>
        /// <seealso cref = "SetImage(string)"/>
        /// <seealso cref="ToByteArray"/>
        /// <ms><seealso cref = "ToStream"/></ms>
        /// <cpp><seealso cref="ToStream"/></cpp>
        /// <seealso cref = "Save(string)"></seealso>
        /// </remarks>
#else
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="IImageData.ImageBytes"]/*'/>
#endif
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] ImageBytes
        {
            get { return mImageDataCore.ImageBytes; }
            set { mImageDataCore.ImageBytes = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if the shape has image bytes or links an image.
        /// </summary>
        /// <value></value>
        public bool HasImage
        {
            get { return mImageDataCore.HasImage; }
        }

        /// <summary>
        /// Gets the information about image size and resolution.
        /// </summary>
        /// <value></value>
        /// <remarks>
        ///     <p>If the image is linked only and not stored in the document, returns zero size.</p>
        /// </remarks>
        public ImageSize ImageSize
        {
            get { return mImageDataCore.ImageSize; }
        }

        /// <summary>
        /// Gets the type of the image.
        /// </summary>
        /// <value></value>
        public ImageType ImageType
        {
            get { return mImageDataCore.ImageType; }
        }

        /// <summary>
        /// Sets the image data and silences exceptions if any.
        /// </summary>
        /// <param name="bytes">Bytes of the image.</param>
        /// <returns>True, if setting was successful, otherwise - False.</returns>
        internal bool SetImageSafe(byte[] bytes)
        {
            return mImageDataCore.SetImageSafe(bytes);
        }

        /// <summary>
        /// Returns <c>true</c> if the image is linked to the shape (when <see cref="SourceFullName"/> is specified).
        /// </summary>
        /// <value></value>
        public bool IsLink
        {
            get { return mImageDataCore.IsLink; }
        }

        /// <summary>
        /// Returns <c>true</c> if the image is linked and not stored in the document.
        /// </summary>
        /// <value></value>
        public bool IsLinkOnly
        {
            get { return mImageDataCore.IsLinkOnly; }
        }

        /// <summary>
        /// Gets or sets the path and name of the source file for the linked image.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// <p>If <see cref="SourceFullName"/> is not an empty string, the image is linked.</p>
        /// </remarks>
        public string SourceFullName
        {
            get { return mImageDataCore.SourceFullName; }
            set { mImageDataCore.SourceFullName = value; }
        }

        /// <summary>
        /// Defines the title of an image.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// </remarks>
        public string Title
        {
            get
            {
                return (string)FetchAttr(ShapeAttr.ImageTitle);
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                SetAttr(ShapeAttr.ImageTitle, value);
            }
        }

        /// <summary>
        /// Defines the fraction of picture removal from the top side.
        /// </summary>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="Cropping"]/*'/>
        public double CropTop
        {
            get { return mParent.GraphicData.CropTop; }
            set { mParent.GraphicData.CropTop = value; }
        }

        /// <summary>
        /// Defines the fraction of picture removal from the bottom side.
        /// </summary>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="Cropping"]/*'/>
        public double CropBottom
        {
            get { return mParent.GraphicData.CropBottom; }
            set { mParent.GraphicData.CropBottom = value; }
        }

        /// <summary>
        /// Defines the fraction of picture removal from the left side.
        /// </summary>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="Cropping"]/*'/>
        public double CropLeft
        {
            get { return mParent.GraphicData.CropLeft; }
            set { mParent.GraphicData.CropLeft = value; }
        }

        /// <summary>
        /// Defines the fraction of picture removal from the right side.
        /// </summary>
        /// <include file='..\Docs\Drawing.xml' path='Topics/Topic[@name="Cropping"]/*'/>
        public double CropRight
        {
            get { return mParent.GraphicData.CropRight; }
            set { mParent.GraphicData.CropRight = value; }
        }

        /// <summary>
        /// Gets all crop info in one call.
        /// </summary>
        internal ImageCrop GetCrop()
        {
            return new ImageCrop(CropLeft, CropRight, CropTop, CropBottom);
        }

        /// <summary>
        /// Gets the collection of borders of the image. Borders only have effect for inline images.
        /// </summary>
        public BorderCollection Borders
        {
            get
            {
                if (mBordersCache == null)
                    mBordersCache = new BorderCollection(this);
                return mBordersCache;
            }
        }

        /// <summary>
        /// Defines the color value of the image that will be treated as transparent.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.</p>
        /// </remarks>
        public Color ChromaKey
        {
            get { return ChromaKeyInternal.ToNativeColor(); }
            set { ChromaKeyInternal = DrColor.FromNativeColor(value); }
        }

        internal DrColor ChromaKeyInternal
        {
            get { return (DrColor)FetchAttr(ShapeAttr.ImageTransparent); }
            set { SetAttr(ShapeAttr.ImageTransparent, value); }
        }

        /// <summary>
        /// Gets or sets the brightness of the picture.
        /// The value for this property must be a number from 0.0 (dimmest) to 1.0 (brightest).
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.5.</p>
        /// </remarks>
        public double Brightness
        {
            get { return mParent.GraphicData.Brightness; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");

                mParent.GraphicData.Brightness = value;
            }
        }

        /// <summary>
        /// Gets or sets the contrast for the specified picture. The value
        /// for this property must be a number from 0.0 (the least contrast) to 1.0 (the greatest contrast).
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.5.</p>
        /// </remarks>
        public double Contrast
        {
            get { return mParent.GraphicData.Contrast; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");

                mParent.GraphicData.Contrast = value;
            }
        }

        /// <summary>
        /// Determines whether an image will be displayed in black and white.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool BiLevel
        {
            get { return mParent.GraphicData.BiLevel; }
            set { mParent.GraphicData.BiLevel = value; }
        }

        /// <summary>
        /// Determines whether a picture will display in grayscale mode.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        public bool GrayScale
        {
            get { return mParent.GraphicData.GrayScale; }
            set { mParent.GraphicData.GrayScale = value; }
        }

        /// <summary>
        /// Converts escher brightness to percent brightness in -0.5 to 0.5 range to 0.0 to 1.0 range.
        /// </summary>
        internal static double BrightnessToPercent(double escherBrightness)
        {
            if ((escherBrightness < BrightnessMin) || (escherBrightness > BrightnessMax))
                throw new ArgumentOutOfRangeException("escherBrightness");

            return escherBrightness + 0.5;
        }

        /// <summary>
        /// Converts percent brightness to escher brightness from 0.0 to 1.0 range to -0.5 to 0.5 range.
        /// </summary>
        internal static double PercentToBrightness(double percent)
        {
            if ((percent < 0.0) || (percent > 1.0))
                throw new ArgumentOutOfRangeException("percent");

            return percent - 0.5;
        }

        /// <summary>
        /// Uses somewhat complicated formula that I came up myself while observing values stored by escher.
        /// </summary>
        internal static double ContrastToPercent(double escherContrast)
        {
            // WORDSNET-18766 Although MS Word doesn't allow to enter contrast values less then 0% and greater then 100% via GUI,
            // it still processes them normally if they're already specified in the document, and it doesn't skip such
            // out-of-range contrast values while rendering, so we shouldn't check for ContrastMin value here either.

            if (escherContrast <= 1.0)
                return escherContrast / 2;
            else
                return (escherContrast - 0.5) / escherContrast;
        }

        /// <summary>
        /// Converts percent contrast to escher contrast.
        /// Uses somewhat complicated formula that I came up myself while observing values stored by escher.
        /// </summary>
        internal static double PercentToContrast(double percent)
        {
            if ((percent < 0.0) || (percent > 1.0))
                throw new ArgumentOutOfRangeException("percent");

            if (percent <= 0.5)
            {
                return percent * 2;
            }
            else
            {
                if (percent < 1.0)
                    return 0.5 / (1.0 - percent);
                else
                    return double.MaxValue;
            }
        }

        internal byte[] LoadImageBytes()
        {
            return mImageDataCore.LoadImageBytes();
        }

        /// <summary>
        /// True if <see cref="ChromaKey"/> is specified.
        /// </summary>
        internal bool IsChromaKeySpecified
        {
            get { return mParent.GetDirectShapeAttrInternal(ShapeAttr.ImageTransparent) != null; }
        }

        internal bool HasImageBytes
        {
            get { return mImageDataCore.HasImageBytes; }
        }

        /// <summary>
        /// Gets the type of the image.
        /// </summary>
        internal FileFormat FileFormat
        {
            get { return mImageDataCore.FileFormat; }
        }

        private object FetchAttr(int key)
        {
            return mParent.FetchShapeAttrInternal(key);
        }

        private void SetAttr(int key, object value)
        {
            mParent.SetShapeAttrInternal(key, value);
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectShapeAttrInternal(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedShapeAttrInternal(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            SetAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return gImageBorders; }
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static ImageData()
        {
            gImageBorders.Add(BorderType.Top, ShapeAttr.BorderTop);
            gImageBorders.Add(BorderType.Left, ShapeAttr.BorderLeft);
            gImageBorders.Add(BorderType.Bottom, ShapeAttr.BorderBottom);
            gImageBorders.Add(BorderType.Right, ShapeAttr.BorderRight);
        }

        /// <summary>
        /// Parent attribute source.
        /// Typical implementer of <see cref="IShapeAttrSource" /> is <see cref="Shape" />.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly ShapeBase mParent;

        /// <summary>
        /// Associated border collection.
        /// </summary>
        private BorderCollection mBordersCache;

        private static readonly SortedList<BorderType, int> gImageBorders = new SortedList<BorderType, int>();
        private readonly ImageDataCore mImageDataCore;

        internal const double ContrastMin = 0.0;
        internal const double ContrastMax = 1.0;
        internal const double BrightnessMin = -0.5;
        internal const double BrightnessMax = 0.5;
    }
}
