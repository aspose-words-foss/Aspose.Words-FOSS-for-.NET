// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.RW.Factories;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Provides utility methods for working with file formats, such as detecting file format
    /// or converting file extensions to/from file format enums.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/detect-file-format-and-check-format-compatibility/">Detect File Format and Check Format Compatibility</a> documentation article.</para>
    /// </summary>
    public static class FileFormatUtil
    {
        /// <overloads>Detects and returns the information about a format of a document.</overloads>
        /// <summary>
        /// Detects and returns the information about a format of a document stored in a disk file.
        /// </summary>
        /// <remarks>
        /// <para>Even if this method detects the document format, it does not guarantee
        /// that the specified document is valid. This method only detects the document format by
        /// reading data that is sufficient for detection. To fully verify that a document is valid
        /// you need to load the document into a <see cref="Document"/> object.</para>
        /// <para>This method throws <see cref="FileCorruptedException"/> when the format is
        /// recognized, but the detection cannot complete because of corruption.</para>
        /// </remarks>
        ///
        /// <param name="fileName">The file name.</param>
        /// <returns>A <see cref="FileFormatInfo"/> object that contains the detected information.</returns>
        public static FileFormatInfo DetectFileFormat(string fileName)
        {
            ArgumentUtil.CheckHasChars(fileName, "fileName");

            using (Stream stream = File.OpenRead(fileName))
                return DetectFileFormat(stream);
        }

        /// <summary>
        /// Detects and returns the information about a format of a document stored in a stream.
        /// </summary>
        /// <remarks>
        /// <para>The stream must be positioned at the beginning of the document.</para>
        ///
        /// <ms><para>When this method returns, the position in the stream is restored to the original position.</para></ms>
        /// <cpp><para>When this method returns, the position in the stream is restored to the original position.</para></cpp>
        ///
        /// <java><para>Detecting a file format might require seeking to various positions in the stream. Because java.io.InputStream
        /// is not seekable, this method loads the whole stream into memory temporarily. When this method returns,
        /// the stream is positioned at the end of the document.</para></java>
        ///
        /// <para>Even if this method detects the document format, it does not guarantee
        /// that the specified document is valid. This method only detects the document format by
        /// reading data that is sufficient for detection. To fully verify that a document is valid
        /// you need to load the document into a <see cref="Document"/> object.</para>
        /// <para>This method throws <see cref="FileCorruptedException"/> when the format is
        /// recognized, but the detection cannot complete because of corruption.</para>
        /// </remarks>
        ///
        /// <param name="stream">The stream.</param>
        /// <returns>A <see cref="FileFormatInfo"/> object that contains the detected information.</returns>
        /// <javaName>com.aspose.words.FileFormatInfo detectFileFormat(java.io.InputStream stream)</javaName>
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first
#if PLAIN_JAVA
        //JAVA-added public wrapper for internalized method
        public static FileFormatInfo detectFileFormat(java.io.InputStream stream) throws Exception
        {
            return detectFileFormat(com.aspose.ms.java.IO.JavaOnlyStreamUtil.copyToMemoryStream(stream));
        }
#endif
        [JavaInternal]
        public static FileFormatInfo DetectFileFormat([CppIOStreamWrapper(IOStreamType.IStream)] Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            try
            {
                FileFormatDetector detector = new FileFormatDetector();
                return detector.Detect(stream);
            }
            catch (Exception e)
            {
                // Same approach as in the loading of a document.

#if CPLUSPLUS
                // Workaround for WORDSCPP-622
                ConvertAndRethrowLoadException(e);
#else
                throw ConvertLoadException(e);
#endif
            }
        }

        /// <summary>
        /// Converts IANA content type into a load format enumerated value.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static LoadFormat ContentTypeToLoadFormat(string contentType)
        {
            LoadFormat loadFormat = ToLoadFormat(FileFormatCore.FromContentType(contentType));
            if (loadFormat != LoadFormat.Unknown)
                return loadFormat;
            else
                throw new ArgumentException("Cannot convert this content type to a load format.");
        }

        /// <summary>
        /// Converts IANA content type into a save format enumerated value.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static SaveFormat ContentTypeToSaveFormat(string contentType)
        {
            SaveFormat saveFormat = ToSaveFormat(FileFormatCore.FromContentType(contentType));
            if (saveFormat != SaveFormat.Unknown)
                return saveFormat;
            else
                throw new ArgumentException("Cannot convert this content type to a save format.");
        }

        /// <summary>
        /// Converts a load format enumerated value into a file extension. The returned extension is a lower-case string with a leading dot.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="SaveFormat.WordML"/> value is converted to ".wml".</para>
        /// </remarks>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static string LoadFormatToExtension(LoadFormat loadFormat)
        {
            if (loadFormat == LoadFormat.DocPreWord60)
                loadFormat = LoadFormat.Doc;

            string ext = FileFormatCore.ToExt(FromLoadFormat(loadFormat));
            if (StringUtil.HasChars(ext))
                return "." + ext;
            else
                throw new ArgumentException("Cannot convert this load format to a file extension.");
        }

        /// <summary>
        /// Converts a <see cref="SaveFormat"/> value to a <see cref="LoadFormat"/> value if possible.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static LoadFormat SaveFormatToLoadFormat(SaveFormat saveFormat)
        {
            LoadFormat loadFormat = ToLoadFormat(FromSaveFormat(saveFormat));
            if (loadFormat != LoadFormat.Unknown)
                return loadFormat;
            else
                throw new ArgumentException("Cannot convert this save format to a load format.");
        }

        /// <summary>
        /// Converts a <see cref="LoadFormat"/> value to a <see cref="SaveFormat"/> value if possible.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static SaveFormat LoadFormatToSaveFormat(LoadFormat loadFormat)
        {
            SaveFormat saveFormat = ToSaveFormat(FromLoadFormat(loadFormat));
            if (saveFormat != SaveFormat.Unknown)
                return saveFormat;
            else
                throw new ArgumentException("Cannot convert this load format to a save format.");
        }

        /// <summary>
        /// Converts a save format enumerated value into a file extension. The returned extension is a lower-case string with a leading dot.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="SaveFormat.WordML"/> value is converted to ".wml".</para>
        /// <para>The <see cref="SaveFormat.FlatOpc"/> value is converted to ".fopc".</para>
        /// </remarks>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static string SaveFormatToExtension(SaveFormat saveFormat)
        {
            string ext = FileFormatCore.ToExt(FromSaveFormat(saveFormat));
            if (StringUtil.HasChars(ext))
                return "." + ext;
            else
                throw new ArgumentException("Cannot convert this save format to a file extension.");
        }

        /// <summary>
        /// Converts a file name extension into a <see cref="SaveFormat"/> value.
        /// </summary>
        /// <param name="extension">The file extension. Can be with or without a leading dot. Case-insensitive.</param>
        /// <remarks>
        /// <p>If the extension cannot be recognized, returns <see cref="SaveFormat.Unknown"/>.</p>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Throws if the parameter is <c>null</c>.</exception>
        public static SaveFormat ExtensionToSaveFormat(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            return ToSaveFormat(FileFormatCore.FromExt(extension));
        }

        /// <summary>
        /// Converts an Aspose.Words image type enumerated value into a file extension. The returned extension is a lower-case string with a leading dot.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when cannot convert.</exception>
        public static string ImageTypeToExtension(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Unknown:
                case ImageType.NoImage:
                    throw new ArgumentException("Cannot convert this image type to a file extension.");
                default:
                    return "." + FileFormatCore.ToExt(FromImageType(imageType));
            }
        }

        /// <summary>
        /// Below are most likely exceptions that could be thrown by our reader classes or the .NET Framework
        /// when attempting to load corrupted documents. We convert these to a more user friendly exception.
        /// </summary>
        internal static Exception ConvertLoadException(Exception e)
        {
            // This could be thrown by our readers when they detect invalid data and cannot recover.
            // But not all invalid data can be easily detected, therefore see all other exceptions below.
            if (e is InvalidOperationException)
                return new FileCorruptedException(e);

            // This could be thrown by in binary reader when pointer or length is causing
            // to read beyond end of file.
            if (e is EndOfStreamException)
                return new FileCorruptedException(e);

            // This could be thrown by .NET when binary reader parses a document and accesses
            // a buffer or PLCF items beyond valid range.
            if (e is IndexOutOfRangeException)
                return new FileCorruptedException(e);

            // This could be thrown by .NET or by readers when invalid data is caught at function boundaries.
            if (e is ArgumentException)
                return new FileCorruptedException(e);

            // The file must be so screwed so our reader fails with null reference exception somewhere.
            if (e is NullReferenceException)
                return new FileCorruptedException(e);

            // ODT throws when some date formatting pattern is specified incorrectly.
            if (e is FormatException)
                return new FileCorruptedException(e);

            // If zip date is corrupted inside a document, we could have this.
#if !JAVA
            if (e is ZipException)
#else
            //Aspose.Zip is fully deleted from java, standard java.util.zip is used instead.
            if (e instanceof java.util.zip.ZipException)
#endif
                return new FileCorruptedException(e);

            return e;
        }

        /// <summary>
        /// The same as <see cref="ConvertLoadException(Exception)"/> but throws an exception instead of returning it.
        /// Workaround for WORDSCPP-622. C++ can't properly nest exceptions. If we return exception we are losing the
        /// information about the actual expection type.
        ///
        /// </summary>
        [JavaThrows(true)]
        internal static void ConvertAndRethrowLoadException([CppArgumentKind(ArgumentKind.ConstReference)] Exception e)
        {
            // This could be thrown by our readers when they detect invalid data and cannot recover.
            // But not all invalid data can be easily detected, therefore see all other exceptions below.
            if (e is InvalidOperationException)
                throw new FileCorruptedException(e);

            // This could be thrown by in binary reader when pointer or length is causing
            // to read beyond end of file.
            if (e is EndOfStreamException)
                throw new FileCorruptedException(e);

            // This could be thrown by .NET when binary reader parses a document and accesses
            // a buffer or PLCF items beyond valid range.
            if (e is IndexOutOfRangeException)
                throw new FileCorruptedException(e);

            // This could be thrown by .NET or by readers when invalid data is caught at function boundaries.
            if (e is ArgumentException)
                throw new FileCorruptedException(e);

            // The file must be so screwed so our reader fails with null reference exception somewhere.
            if (e is NullReferenceException)
                throw new FileCorruptedException(e);

            // ODT throws when some date formatting pattern is specified incorrectly.
            if (e is FormatException)
                throw new FileCorruptedException(e);

            // If zip date is corrupted inside a document, we could have this.
#if !JAVA
            if (e is ZipException)
#else
            //Aspose.Zip is fully deleted from java, standard java.util.zip is used instead.
            if (e instanceof java.util.zip.ZipException)
#endif
                throw new FileCorruptedException(e);

            throw e;
        }

        #region Safe Conversions (No Exceptions)

        /// <summary>
        /// Returns true if <see cref="PageSetup"/> is applicable to the specified <see cref="LoadFormat"/>.
        /// </summary>
        /// <remarks>
        /// Used to check the applicability of PaperSize automatic setting by the specified DefaultEditingLanguage.
        /// So far done only for Html. In case of customers' requests other formats may be added.
        /// </remarks>
        internal static bool IsPageSetupApplicable(LoadFormat format)
        {
            return format == LoadFormat.Html;
        }

        /// <summary>
        /// Used instead of enum.ToString() that is prohibited by Aspose.Auckland practices.
        /// </summary>
        internal static string SaveFormatToString(SaveFormat saveFormat)
        {
            return FileFormatCore.ToString(FromSaveFormat(saveFormat));
        }

        internal static SaveFormat ToSaveFormat(FileFormat fileFormat)
        {
            SaveFormat saveFormat;
            if (gToSaveFormat.TryGetValue(fileFormat, out saveFormat))
                return saveFormat;
            else
                return SaveFormat.Unknown;
        }

        internal static ImageType ToImageType(FileFormat fileFormat)
        {
            ImageType imageType;
            if (gToImageType.TryGetValue(fileFormat, out imageType))
                return imageType;
            else
                return ImageType.Unknown;
        }

        internal static bool IsXmlBasedFormat(LoadFormat format)
        {
            return
                 (format == LoadFormat.Docx) ||
                 (format == LoadFormat.Docm) ||
                 (format == LoadFormat.Dotx) ||
                 (format == LoadFormat.Dotm) ||
                 (format == LoadFormat.FlatOpc) ||
                 (format == LoadFormat.FlatOpcMacroEnabled) ||
                 (format == LoadFormat.FlatOpcTemplate) ||
                 (format == LoadFormat.FlatOpcTemplateMacroEnabled) ||
                 (format == LoadFormat.WordML);
        }

        /// <summary>
        /// Check if the save format is a fixed format.
        /// </summary>
        /// <param name="saveFormat">The save format to check.</param>
        /// <returns>True if it is a fixed format; otherwise, false.</returns>
        internal static bool IsFixedFormat(SaveFormat saveFormat)
        {
            return saveFormat == SaveFormat.Pdf
#if NETSTANDARD || NET462_OR_GREATER || JAVA
                || saveFormat == SaveFormat.WebP
#endif
                || saveFormat == SaveFormat.Emf
                || saveFormat == SaveFormat.Tiff
                || saveFormat == SaveFormat.Png
                || saveFormat == SaveFormat.Bmp
                || saveFormat == SaveFormat.Jpeg
                || saveFormat == SaveFormat.Svg
                || saveFormat == SaveFormat.Gif
                || saveFormat == SaveFormat.Eps
                || saveFormat == SaveFormat.Xps
                || saveFormat == SaveFormat.Pcl
                || saveFormat == SaveFormat.OpenXps
                || saveFormat == SaveFormat.HtmlFixed;
        }

        /// <summary>
        /// Check if the save format is an image format.
        /// </summary>
        /// <param name="saveFormat">The save format to check.</param>
        /// <returns>True if it is an image format; otherwise, false.</returns>
        internal static bool IsImageFormat(SaveFormat saveFormat)
        {
            return saveFormat == SaveFormat.Emf
#if NETSTANDARD || NET462_OR_GREATER || JAVA
                || saveFormat == SaveFormat.WebP
#endif
                || saveFormat == SaveFormat.Tiff
                || saveFormat == SaveFormat.Png
                || saveFormat == SaveFormat.Bmp
                || saveFormat == SaveFormat.Jpeg
                || saveFormat == SaveFormat.Svg
                || saveFormat == SaveFormat.Gif
                || saveFormat == SaveFormat.Eps;
        }

        internal static FileFormat FromLoadFormat(LoadFormat loadFormat)
        {
            return gFromLoadFormat.GetValueOrDefault(loadFormat, FileFormat.Unknown);
        }

        internal static FileFormat FromSaveFormat(SaveFormat saveFormat)
        {
            return gFromSaveFormat.GetValueOrDefault(saveFormat, FileFormat.Unknown);
        }

        internal static FileFormat FromImageType(ImageType imageType)
        {
            return gFromImageType.GetValueOrDefault(imageType, FileFormat.Unknown);
        }

        private static LoadFormat ToLoadFormat(FileFormat fileFormat)
        {
            return gToLoadFormat.GetValueOrDefault(fileFormat, LoadFormat.Unknown);
        }

        private static void AddMap(FileFormat fileFormat, SaveFormat saveFormat, LoadFormat loadFormat, ImageType imageType)
        {
            if (saveFormat != SaveFormat.Unknown)
            {
                gToSaveFormat.Add(fileFormat, saveFormat);
                gFromSaveFormat.Add(saveFormat, fileFormat);
            }

            if (loadFormat != LoadFormat.Unknown)
            {
                if (!gToLoadFormat.ContainsKey(fileFormat))
                    gToLoadFormat.Add(fileFormat, loadFormat);
                gFromLoadFormat.Add(loadFormat, fileFormat);
            }

            if (imageType != ImageType.Unknown)
            {
                gToImageType.Add(fileFormat, imageType);
                gFromImageType.Add(imageType, fileFormat);
            }
        }

        static FileFormatUtil()
        {
            AddMap(FileFormat.Doc, SaveFormat.Doc, LoadFormat.Doc, ImageType.Unknown);
            AddMap(FileFormat.Doc, SaveFormat.Unknown, LoadFormat.Auto, ImageType.Unknown);
            AddMap(FileFormat.Dot, SaveFormat.Dot, LoadFormat.Dot, ImageType.Unknown);
            AddMap(FileFormat.Docx, SaveFormat.Docx, LoadFormat.Docx, ImageType.Unknown);
            AddMap(FileFormat.Docx, SaveFormat.Unknown, (LoadFormat)LoadFormatTest.TestDocxDml, ImageType.Unknown);
            AddMap(FileFormat.Docm, SaveFormat.Docm, LoadFormat.Docm, ImageType.Unknown);
            AddMap(FileFormat.Dotx, SaveFormat.Dotx, LoadFormat.Dotx, ImageType.Unknown);
            AddMap(FileFormat.Dotm, SaveFormat.Dotm, LoadFormat.Dotm, ImageType.Unknown);

            AddMap(FileFormat.FlatOpc, SaveFormat.FlatOpc, LoadFormat.FlatOpc, ImageType.Unknown);
            AddMap(FileFormat.FlatOpcMacroEnabled, SaveFormat.FlatOpcMacroEnabled, LoadFormat.FlatOpcMacroEnabled,
                ImageType.Unknown);
            AddMap(FileFormat.FlatOpcTemplate, SaveFormat.FlatOpcTemplate, LoadFormat.FlatOpcTemplate, ImageType.Unknown);
            AddMap(FileFormat.FlatOpcTemplateMacroEnabled, SaveFormat.FlatOpcTemplateMacroEnabled,
                LoadFormat.FlatOpcTemplateMacroEnabled, ImageType.Unknown);

            AddMap(FileFormat.Rtf, SaveFormat.Rtf, LoadFormat.Rtf, ImageType.Unknown);
            AddMap(FileFormat.WordML, SaveFormat.WordML, LoadFormat.WordML, ImageType.Unknown);

            AddMap(FileFormat.Html, SaveFormat.Html, LoadFormat.Html, ImageType.Unknown);
            AddMap(FileFormat.HtmlFixed, SaveFormat.HtmlFixed, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Mhtml, SaveFormat.Mhtml, LoadFormat.Mhtml, ImageType.Unknown);
            AddMap(FileFormat.Mobi, SaveFormat.Mobi, LoadFormat.Mobi, ImageType.Unknown);
            AddMap(FileFormat.Chm, SaveFormat.Unknown, LoadFormat.Chm, ImageType.Unknown);
            AddMap(FileFormat.Azw3, SaveFormat.Azw3, LoadFormat.Azw3, ImageType.Unknown);

            AddMap(FileFormat.Odt, SaveFormat.Odt, LoadFormat.Odt, ImageType.Unknown);
            AddMap(FileFormat.Ott, SaveFormat.Ott, LoadFormat.Ott, ImageType.Unknown);

            AddMap(FileFormat.Pdf, SaveFormat.Pdf, LoadFormat.Pdf, ImageType.Unknown);
            AddMap(FileFormat.Ps, SaveFormat.Ps, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Pcl, SaveFormat.Pcl, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Xps, SaveFormat.Xps, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.OpenXps, SaveFormat.OpenXps, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.XamlFixed, SaveFormat.XamlFixed, LoadFormat.Unknown, ImageType.Unknown);

            AddMap(FileFormat.Svg, SaveFormat.Svg, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Epub, SaveFormat.Epub, LoadFormat.Epub, ImageType.Unknown);
            AddMap(FileFormat.Txt, SaveFormat.Text, LoadFormat.Text, ImageType.Unknown);
            AddMap(FileFormat.Markdown, SaveFormat.Markdown, LoadFormat.Markdown, ImageType.Unknown);
            AddMap(FileFormat.XamlFlow, SaveFormat.XamlFlow, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Tiff, SaveFormat.Tiff, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Gif, SaveFormat.Gif, LoadFormat.Unknown, ImageType.Gif);
            AddMap(FileFormat.Png, SaveFormat.Png, LoadFormat.Unknown, ImageType.Png);
            AddMap(FileFormat.Bmp, SaveFormat.Bmp, LoadFormat.Unknown, ImageType.Bmp);
            AddMap(FileFormat.Jpeg, SaveFormat.Jpeg, LoadFormat.Unknown, ImageType.Jpeg);
            AddMap(FileFormat.Pict, SaveFormat.Unknown, LoadFormat.Unknown, ImageType.Pict);
            AddMap(FileFormat.Wmf, SaveFormat.Unknown, LoadFormat.Unknown, ImageType.Wmf);
            AddMap(FileFormat.XamlFlowPack, SaveFormat.XamlFlowPack, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Emf, SaveFormat.Emf, LoadFormat.Unknown, ImageType.Emf);
            AddMap(FileFormat.Eps, SaveFormat.Eps, LoadFormat.Unknown, ImageType.Eps);
            AddMap(FileFormat.Xlsx, SaveFormat.Xlsx, LoadFormat.Unknown, ImageType.Unknown);
            AddMap(FileFormat.Docling, SaveFormat.Docling, LoadFormat.Unknown, ImageType.Unknown);
#if NETSTANDARD || NET462_OR_GREATER || JAVA
            AddMap(FileFormat.WebP, SaveFormat.WebP, LoadFormat.Unknown, ImageType.WebP);
#endif
            AddMap(FileFormat.MsWorks, SaveFormat.Unknown, LoadFormat.MsWorks, ImageType.Unknown);
        }

        #endregion

        private static readonly Dictionary<LoadFormat, FileFormat> gFromLoadFormat = new Dictionary<LoadFormat, FileFormat>();
        private static readonly Dictionary<SaveFormat, FileFormat> gFromSaveFormat = new Dictionary<SaveFormat, FileFormat>();
        private static readonly Dictionary<ImageType, FileFormat> gFromImageType = new Dictionary<ImageType, FileFormat>();

        private static readonly Dictionary<FileFormat, LoadFormat> gToLoadFormat = new Dictionary<FileFormat, LoadFormat>();
        private static readonly Dictionary<FileFormat, SaveFormat> gToSaveFormat = new Dictionary<FileFormat, SaveFormat>();
        private static readonly Dictionary<FileFormat, ImageType> gToImageType = new Dictionary<FileFormat, ImageType>();
    }
}
