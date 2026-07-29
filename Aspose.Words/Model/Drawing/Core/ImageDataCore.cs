// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2011 by Alexey Titov

using System;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.IO;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Contains common logic for ImageData classes.
    /// </summary>
    internal class ImageDataCore
    {
        /// <summary>
        /// Ctor.
        /// Document is needed to resolve relative image URIs in <see cref="ToStream" />, <see cref="ToByteArray" />
        /// and <see cref="SetImage(string)" />.
        /// Document might be null if in glossary, let's think what to do with them.
        /// </summary>
        internal ImageDataCore(Document document,
            IImageDataSource imageDataSource)
        {
            mDocument = document;
            mImageDataSource = imageDataSource;
        }

        /// <summary>
        /// Extract bmp/png raw bytes with the given index from .ico file.
        /// See https://en.wikipedia.org/wiki/ICO_(file_format)
        /// </summary>
        public static byte[] GetIcoImageBytes(Stream stream, int imageIndex)
        {
            BinaryReader reader = new BinaryReader(stream);

            int reserved = reader.ReadUInt16();
            Debug.Assert(reserved == 0);
            int type = reader.ReadUInt16();
            Debug.Assert(type == 1);
            int nrEntries = reader.ReadUInt16();
            Debug.Assert(nrEntries > 0);

            if (imageIndex < 0 || imageIndex >= nrEntries)
            {
                throw new ArgumentException("Couldn't extract image with index " + imageIndex +
                                            " from .ico file, because it contains only " + nrEntries + " image(s).");
            }

            for (int i = 0; i < nrEntries; ++i)
            {
                int width = reader.ReadByte();
                // Specifies image width in pixels. Can be any number between 0 and 255. Value 0 means image width is 256 pixels.
                width = (width == 0 ? 256 : width);
                // Specifies image height in pixels. Can be any number between 0 and 255. Value 0 means image height is 256 pixels.
                int height = reader.ReadByte();
                height = (height == 0 ? 256 : height);

                // Skip unused nrColorPalette.
                reader.ReadByte();
                int reservedByte = reader.ReadByte();
                Debug.Assert(reservedByte == 0);
                int nrColorPanes = reader.ReadUInt16();
                Debug.Assert(nrColorPanes == 0 || nrColorPanes == 1);
                // Skip unused bitsPerPixel.
                reader.ReadUInt16();
                int sizeInBytes = (int)reader.ReadUInt32();
                long offset = reader.ReadUInt32();
                if (i == imageIndex)
                {
                    byte[] imageBytes = new byte[sizeInBytes];
                    long pos = stream.Position;
                    stream.Position = offset;
                    reader.Read(imageBytes, 0, sizeInBytes);
                    if (nrEntries > 1 && pos != offset)
                        stream.Position = offset;
                    else
                        stream.Position = pos;

                    // WORDSNET-18086 Added support for .ICO files containing PNG images.
                    return ImageUtil.IsPng(imageBytes)
                        ? imageBytes
                        : ImageUtil.PrependBmpHeader(reader, sizeInBytes, width, height);
                }
            }

            throw new InvalidOperationException("Couldn't extract BMP/PNG image from .ico file.");
        }

        /// <summary>
        /// Extract bmp/png raw bytes with the given index from .ico file.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/ICO_(file_format)
        /// </remarks>
        internal static byte[] GetIcoImageBytes(byte[] imageBytes, int imageIndex)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
                return GetIcoImageBytes(stream, imageIndex);
        }

        /// <summary>
        /// Returns number of images stored in the ICO file.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/ICO_(file_format)
        /// </remarks>
        internal static int GetIcoImagesCount(byte[] icoBytes)
        {
            Debug.Assert(ImageUtil.IsIco(icoBytes));

            // The number of images in the file is stored in two little-endian bytes started from offset 4.
            const int offest = 4;
            return icoBytes[offest] + (icoBytes[offest + 1] << 8);
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="image">The image object.</param>
        internal void SetImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (MemoryStream stream = new MemoryStream())
            {
                BitmapPal.SaveNativeImageForWord97(image, stream);
                SetImage(stream);
            }
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="stream">The stream that contains the image.
        /// <java> The stream will be read from the current position, so one should be careful about stream position.</java>
        /// </param>
        /// <javaName>void setImage(java.io.InputStream stream)</javaName>
        internal void SetImage(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            ImageBytes = StreamUtil.CopyStreamToByteArray(stream);
        }

        /// <summary>
        /// Sets the image that the shape displays.
        /// </summary>
        /// <param name="fileName">The image file. Can be a file name or a URL.</param>
        internal void SetImage(string fileName)
        {
            ArgumentUtil.CheckHasChars(fileName, "fileName");

            ImageBytes = ImageDataUtil.LoadImageBytes(fileName, mDocument);
        }

        /// <summary>
        /// Gets the image stored in the shape as a <ms>.NET Image</ms><java>java <tt>BufferedImage</tt></java><cpp>Image</cpp> object.
        /// </summary>
        internal Image ToImage()
        {
            // We don't close the stream here because .NET requires the stream to remain open while the Image exists.
            return BitmapPal.LoadNativeImage(ToStream());
        }

        /// <summary>
        /// For description of this method see comments in <see cref="ImageData"/>.
        /// </summary>
        internal Stream ToStream()
        {
            byte[] imageBytes = ImageBytes;
            if (ArrayUtil.HasData(imageBytes))
                return new MemoryStream(imageBytes);

            Stream imageStream;
            try
            {
                // Added condition to check for null for C++ porting. Do not use LoadImageBytesNoImageFallback here to avoid creating and disposing NoImage stream.
                imageBytes = LoadImageBytes();
                imageStream = (imageBytes != null) ? new MemoryStream(imageBytes) : ImageUtil.GetNoImageStream();
            }
            catch
            {
                imageStream = ImageUtil.GetNoImageStream();
            }

            return imageStream;
        }

        /// <summary>
        /// Returns image bytes for any image regardless whether the image is stored or linked.
        /// </summary>
        /// <remarks>
        /// <para>If the image is linked, downloads the image every time it is called.</para>
        ///
        /// <seealso cref="ImageBytes"/>
        /// </remarks>
        internal byte[] ToByteArray()
        {
            byte[] imageBytes = ImageBytes;
            return ArrayUtil.HasData(imageBytes) ? imageBytes : LoadImageBytesNoImageFallback();
        }

        /// <overloads>Saves the image of the shape.</overloads>
        /// <summary>
        /// Saves the image into the specified stream.
        /// </summary>
        /// <param name="stream">The stream where to save the image to.</param>
        /// <remarks>
        /// <p>Is it the responsibility of the caller to dispose the stream object.</p>
        /// </remarks>
        /// <javaName>void save(java.io.OutputStream stream)</javaName>
        internal void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (Stream srcStream = ToStream())
                StreamUtil.CopyStream(srcStream, stream);
        }

        /// <summary>
        /// Saves the image into a file.
        /// </summary>
        /// <param name="fileName">The file name where to save the image.</param>
        internal void Save(string fileName)
        {
            ArgumentUtil.CheckHasChars(fileName, "fileName");

            using (Stream stream = File.Create(fileName))
                Save(stream);
        }

        /// <summary>
        /// Call this during loading of a document instead SetImage because not showing
        /// an image is better than throwing exception that stops loading the document.
        ///
        /// Sets the image that the shape displays, but silences exceptions
        /// if the image data is invalid.
        /// </summary>
        /// <param name="imageBytes">Bytes of the image.</param>
        /// <returns> True, if setting imageBytes was successful, otherwise - False.</returns>
        internal bool SetImageSafe(byte[] imageBytes)
        {
            bool result;
            try
            {
                if (gExtractImagesToFolder != null)
                    SaveImageToFile(imageBytes);
                ImageBytes = imageBytes;
                result = true;
            }
            catch
            {
                // Silence all exceptions because this is what we want to do.
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Loads image bytes referenced by SourceFullName.
        /// Returns null if any problem occurs.
        /// </summary>
        internal byte[] LoadImageBytes()
        {
            return ImageDataUtil.LoadImageBytes(mImageDataSource.SourceFullName, mDocument);
        }

        /// <summary>
        /// If file format supported, returns the original image bytes, otherwise converts to PNG.
        /// </summary>
        internal static byte[] GetImageBytes(byte[] imageBytes)
        {
            if (imageBytes == null)
                return null;

            imageBytes = CompressedData.GetData(imageBytes);
            if (IsFormatSupported(imageBytes))
            {
                // When image type is natively supported by MS Word, just store the bytes.

                // We do not let metafiles without a placeable header to lurk in our model.
                if (ImageUtil.IsStandardMetafile(imageBytes))
                    imageBytes = ImageUtil.PrependWmfPlaceableHeader(imageBytes, ImageUtil.GetMetafileSize(imageBytes));
            }
            else
            {
                // When image type is not natively supported by MS Word,
                // we load into a native image using the codecs provided by the platform
                // (we are hoping the codecs can load gif and tiff and bmp, which is usually the case)
                // and convert to PNG and then store the bytes in the model.
                imageBytes = ImageUtil.ConvertToPng(imageBytes);
            }

            return imageBytes;
        }

        /// <summary>
        /// If this set by TestUtil all source images from a document should be extracted to the given folder
        /// (before any conversions) .
        /// </summary>
        internal static void SetExtractOriginalImages(string imageFolderName)
        {
            gExtractImagesToFolder = imageFolderName;
        }

        /// <summary>
        /// Gets or sets the path and name of the source file for the linked image.
        /// </summary>
        /// <remarks>
        /// <p>The default value is an empty string.</p>
        /// <p>If <see cref="SourceFullName"/> is not an empty string, the image is linked.</p>
        /// </remarks>
        internal string SourceFullName
        {
            get { return mImageDataSource.SourceFullName; }
            set { mImageDataSource.SourceFullName = value; }
        }

        /// <summary>
        /// Gets or sets the raw bytes of the image stored in the shape.
        /// Setting null or empty array will remove the image from the shape.
        /// </summary>
        internal byte[] ImageBytes
        {
            get { return mImageDataSource.ImageBytes; }
            set
            {
                mImageSizeCache = null;

                if (!ArrayUtil.HasData(value))
                {
                    mImageDataSource.ImageBytes = null;
                    return;
                }

                if (ImageUtil.IsIco(value)) // WORDSJAVA-1333 In case of ICO file, analyze ico header and extract internal bmp/png image
                    mImageDataSource.ImageBytes = GetIcoImageBytes(value, 0);
                else
                    mImageDataSource.ImageBytes = GetImageBytes(value);
            }
        }

        /// <summary>
        /// Returns true if the shape has image bytes or links an image.
        /// </summary>
        internal bool HasImage
        {
            get { return (HasImageBytes || IsLink); }
        }

        /// <summary>
        /// Gets the information about image size and resolution.
        /// </summary>
        /// <remarks>
        /// <p>If the image is linked only and not stored in the document, returns zero size.</p>
        /// </remarks>
        internal ImageSize ImageSize
        {
            get
            {
                if (mImageSizeCache == null)
                {
                    mImageSizeCache = HasImageBytes ?
                        new ImageSize(ImageUtil.GetImageSize(ImageBytes)) :
                        new ImageSize(0, 0);
                }

                return mImageSizeCache;
            }
        }

        /// <summary>
        /// Gets the type of the image.
        /// </summary>
        internal ImageType ImageType
        {
            get { return FileFormatUtil.ToImageType(FileFormat); }
        }

        /// <summary>
        /// Returns true if the image is linked to the shape (when <see cref="SourceFullName"/> is specified).
        /// </summary>
        internal bool IsLink
        {
            get { return StringUtil.HasChars(mImageDataSource.SourceFullName); }
        }

        /// <summary>
        /// Returns true if the image is linked and not stored in the document.
        /// </summary>
        internal bool IsLinkOnly
        {
            get { return (IsLink && !HasImageBytes); }
        }

        internal bool HasImageBytes
        {
            get { return mImageDataSource.HasImageBytes; }
        }

        /// <summary>
        /// Gets the type of the image.
        /// </summary>
        internal FileFormat FileFormat
        {
            get
            {
                if (HasImageBytes)
                    return ImageUtil.GetImageType(ImageBytes);
                else if (IsLink)
                    return FileFormatCore.FromExt(UriUtil.GetExtension(null, mImageDataSource.SourceFullName));
                else
                    return FileFormat.Unknown;
            }
        }

        /// <summary>
        /// Saves imageBytes into a file for debug purposes.
        /// </summary>
        private void SaveImageToFile(byte[] imageBytes)
        {
            try
            {
                if (!Directory.Exists(gExtractImagesToFolder))
                    Directory.CreateDirectory(gExtractImagesToFolder);

                string path = gExtractImagesToFolder + Path.DirectorySeparatorChar +
                              Path.GetFileName(mDocument.OriginalFileName) + "." +
                              GetNextImageIndex() + "." + FileFormatCore.ToExt(ImageUtil.GetImageType(imageBytes));

                using (Stream stream = File.Create(path))
                {
                    stream.Write(imageBytes, 0, imageBytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static int GetNextImageIndex()
        {
            return ++gSaveImageIdx;
        }

        /// <summary>
        /// Loads image bytes referenced by SourceFullName.
        /// Makes a fallback to "no image" if any problem occurs.
        /// </summary>
        private byte[] LoadImageBytesNoImageFallback()
        {
            byte[] imageBytes = LoadImageBytes();
            return (imageBytes != null) ? imageBytes : ImageUtil.GetNoImageBytes();
        }

        /// <summary>
        /// Returns true if given image is natively supported by MS Word.
        /// </summary>
        private static bool IsFormatSupported(byte[] imageData)
        {
            return IsFormatSupported(ImageUtil.GetImageType(imageData));
        }

        /// <summary>
        /// <para>Returns <c>true</c> if image of given type is stored 'as is'.</para>
        /// <para>Compare the method with see RW.MarkupLanguage.Writer.ImageResourceWriter.IsFormatSupported -
        /// they handle file types differently.</para>
        /// </summary>
        private static bool IsFormatSupported(FileFormat imageType)
        {
            switch (imageType)
            {
                case FileFormat.Wmf:
                case FileFormat.Emf:
                case FileFormat.Jpeg:
                case FileFormat.Png:
                case FileFormat.Pict:
                case FileFormat.Ico:
                    return true;
                // These image types are not natively supported by MS Word, therefore we load into a native image and
                // convert to PNG and then store the bytes in the model.
                case FileFormat.Gif:
                case FileFormat.Tiff:
                case FileFormat.WebP:
                // WORDSNET-3702 RK We no longer store BMP (DIB) in MS Word document directly because
                // MS Word fails on some DIB formats (32 bit color for example). This code converts it to PNG.
                case FileFormat.Bmp:
                // SVG is not directly supported in MS Word document, so return false here.
                // SVG image will be converted to format supported by MS Word document.
                case FileFormat.Svg:
                // If there is something we cannot recognize still lets try loading via a native image object.
                default:
                    return false;
            }
        }

        /// <summary>
        /// Document providing context for any local relative URIs.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDocument;

        private readonly IImageDataSource mImageDataSource;

        /// <summary>
        /// Image size.
        /// </summary>
        private ImageSize mImageSizeCache;

        // Used to extract and save original images to files for debug purposes.
        private static string gExtractImagesToFolder;
        private static int gSaveImageIdx;
    }
}
