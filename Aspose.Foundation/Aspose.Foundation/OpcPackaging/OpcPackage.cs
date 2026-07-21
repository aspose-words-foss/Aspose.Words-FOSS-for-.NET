// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2007 by Vladimir Averkin

using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.IO;
using Aspose.Warnings;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Reads and writes OPC packages.
    /// Keeps all parts of the package in memory at all times.
    /// </summary>
    public class OpcPackage : OpcPackageBase
    {
        /// <summary>
        /// Creates a new empty package.
        /// </summary>
        public OpcPackage() { }

        /// <summary>
        /// Creates a new empty package with a specific comparator and compression level.
        /// </summary>
        public OpcPackage(IComparer<string> comparer, ZipCompressionLevel compressionLevel, Zip64Option zip64)
            : base(comparer)
        {
            mCompressionLevel = compressionLevel;
            mZip64Option = zip64;
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        /// <param name="filename">Source file path.</param>
        public OpcPackage(string filename)
        {
           using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                LoadCore(stream);
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        /// <param name="stream">It is the responsibility of the caller to close the stream.</param>
        public OpcPackage(Stream stream)
            : this(stream, false, null)
        {
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// Allows skipping all non-text document parts that can be used for text extraction.
        /// </summary>
        /// <param name="stream">It is the responsibility of the caller to close the stream.</param>
        /// <param name="skipNonTextParts">Value indicating whether non-text parts should not be loaded.</param>
        /// <param name="warningCallback">Callback to register warnings upon loading.</param>
        public OpcPackage(Stream stream, bool skipNonTextParts, IWarningCallbackCore warningCallback)
            : this(stream, skipNonTextParts, warningCallback, null)
        {
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory or file stream, reads content types
        /// and relationships. Allows skipping all non-text document parts that can be used for text extraction.
        /// </summary>
        public OpcPackage(Stream stream, bool skipNonTextParts, IWarningCallbackCore warningCallback, TempFiler tempFiler)
        {
            mWarnCallback = warningCallback;
            mSkipNonTextParts = skipNonTextParts;
            mTempFiler = tempFiler;
            LoadCore(stream);
        }

        /// <summary>
        /// Saves the package into the stream.
        /// </summary>
        /// <param name="stream"></param>
        public override void Save(Stream stream)
        {
            ZipWriterPal zipWriter = new ZipWriterPal(stream, mCompressionLevel, mZip64Option);
            foreach (OpcPackagePart part in Parts)
            {
                part.Stream.Position = 0;
                Stream partStream = CompressedData.GetStream(part.Stream);

                zipWriter.AddEntry(PartNameToZipName(part.Name), StringUtil.TrimRightCrLfStream(partStream), part.ZipEntryLastModified);
            }
            zipWriter.Finish();
        }

        /// <summary>
        /// Updates content of Relationships and ContextTypes parts.
        /// </summary>
        public void UpdateRelationshipsAndContentTypes()
        {
            List<OpcPackagePart> partsToRemove = new List<OpcPackagePart>();

            // We need to rewrite Relationships and ContentTypes, remove them first.
            foreach (OpcPackagePart part in Parts)
                if ((part.ContentType == OpcContentType.Relationships) || (part.Name == ContentTypesPartName))
                    partsToRemove.Add(part);

            foreach (OpcPackagePart part in partsToRemove)
                Parts.Remove(part.Name);

            // Rewrite Relationships and ContentTypes.
            OpcRelsWriter.Write(this, false);
            OpcContentTypesWriter.Write(this, false);
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        private void LoadCore(Stream stream)
        {
            LoadParts(stream);

            StringToStringDictionary contentTypes = ReadContentTypes();
            AssignContentTypesToParts(contentTypes);

            LoadRelationships();
        }

        /// <summary>
        /// Just loads all parts into the parts collection.
        /// When quits, content types are not yet resolved and relationships are not read.
        /// </summary>
        private void LoadParts(Stream stream)
        {
            using (ZipReaderPal zipReader = new ZipReaderPal(stream))
            {
                while (zipReader.MoveToNextEntry())
                {
                    OpcPackagePart part = new OpcPackagePart(ZipNameToPartName(zipReader.EntryFileName), "");
                    part.ZipEntryLastModified = zipReader.EntryLastModified;
                    Parts.Add(part);

                    if (mSkipNonTextParts && !CanContainText(part))
                        continue;

                    try
                    {
                        part.Stream = GetPartStream(zipReader, part);
                    }

                    catch (ZipException ex)
                    {
                        const string msg = "Unable to extract {0} part.";

                        // WORDSNET-13791 Register information about skipped document part
                        // which cannot be read or throw the exception for debug mode when callback
                        // is not specified.
                        if (mWarnCallback != null)
                            mWarnCallback.Warn(WarningTypeCore.DataLoss, WarningSourceCore.Unknown, msg, part.Name);
                        else
                            Debug.Fail(ex.Message);
                    }
                }
            }

            stream.Position = stream.Length;
        }

        /// <summary>
        /// Gets data contained in one of MemoryStream, FileStream or ZipStream depending on settings and data content.
        /// </summary>
        /// <remarks>
        /// Concept overview:
        ///   MemoryStream - common way.
        ///   FileStream - temporary files feature.
        ///   ZipStream - large EMF image.
        /// </remarks>
        private Stream GetPartStream(ZipReaderPal zipReader, OpcPackagePart part)
        {
            FileFormat fileFormat = FileFormatCore.FromExt(part.Extension);
            const long Mb = 1 << 20;

            if ((fileFormat == FileFormat.Emf) && (zipReader.EntryLength > 1 * Mb))
            {
                // Do not decompress data, store it as special "compressed EMF" which is marked by header.
                byte[] compressedData = zipReader.LoadCompressedEntryToByteArray();
                return new MemoryStream(compressedData);
            }

            // If temp folder was specified, then we are extracting large (>50Mb) xml package
            // to file. This degrades speed but reduces memory usage.
            if ((mTempFiler != null) && (mTempFiler.IsTempFilesEnabled) &&
                (fileFormat == FileFormat.Xml) &&
                (zipReader.EntryLength > 50 * Mb))
            {
                Stream stream = mTempFiler.GetNewStream();
                zipReader.ExtractEntryToStream(stream);
                stream.Position = 0;
                return stream;
            }
            return zipReader.LoadCompressedEntryToMemory();
        }

        /// <summary>
        /// Returns true if the specified part can contain text to be extracted.
        /// </summary>
        private static bool CanContainText(OpcPackagePart part)
        {
            FileFormat format = FileFormatCore.FromExt(part.Extension);
            return !(
                        format == FileFormat.Bmp ||
                        format == FileFormat.Emf ||
                        format == FileFormat.Gif ||
                        format == FileFormat.Jpeg ||
                        format == FileFormat.Mov ||
                        format == FileFormat.Pict ||
                        format == FileFormat.Png ||
                        format == FileFormat.Svg ||
                        format == FileFormat.Tiff ||
                        format == FileFormat.Wmf ||
                        format == FileFormat.WebP ||
                        format == FileFormat.Odttf
                    );
        }

        /// <summary>
        /// Returns a hashtable where the key is either extension or explicit part name and
        /// the value is the content type.
        /// </summary>
        private StringToStringDictionary ReadContentTypes()
        {
            // WORDSNET-15883 and 17236. Content type has to be detected case insensitively.
            StringToStringDictionary contentTypes = new StringToStringDictionary(false);

            OpcPackagePart part = FetchPartByName(ContentTypesPartName);
            AnyXmlReader partReader = new AnyXmlReader(part.Stream);
            while (partReader.ReadChild("Types"))
            {
                switch (partReader.LocalName)
                {
                    case "Default":
                        ReadDefaultContentType(partReader, contentTypes);
                        break;
                    case "Override":
                        ReadOverrideContentType(partReader, contentTypes);
                        break;
                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }

            return contentTypes;
        }

        /// <summary>
        /// Reads a default content type entry and adds it to the hashtable.
        /// </summary>
        private static void ReadDefaultContentType(AnyXmlReader partReader, StringToStringDictionary contentTypes)
        {
            string contentType = null;
            string extension = null;

            while (partReader.MoveToNextAttribute())
            {
                switch (partReader.LocalName)
                {
                    case "Extension":
                        extension = partReader.Value;
                        break;
                    case "ContentType":
                        contentType = partReader.Value;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            if (StringUtil.HasChars(contentType) && StringUtil.HasChars(extension))
                contentTypes[extension] = contentType;
        }

        /// <summary>
        /// Reads an override content type entry and adds it to the hashtable.
        /// </summary>
        private static void ReadOverrideContentType(AnyXmlReader partReader, StringToStringDictionary contentTypes)
        {
            string contentType = null;
            string partName = null;

            while (partReader.MoveToNextAttribute())
            {
                switch (partReader.LocalName)
                {
                    case "PartName":
                        partName = partReader.Value;
                        break;
                    case "ContentType":
                        contentType = partReader.Value;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            if (StringUtil.HasChars(contentType) && StringUtil.HasChars(partName))
                contentTypes[partName] = contentType;
        }

        private void AssignContentTypesToParts(StringToStringDictionary contentTypes)
        {
            foreach (OpcPackagePart part in Parts)
            {
                // First try to get content type for the part name (if it was specified using an override).
                string contentType = contentTypes[part.Name];

                // Then try to get content type for the part extension (if it was specified using a default).
                if (!StringUtil.HasChars(contentType))
                    contentType = contentTypes[part.Extension];

                part.ContentType = contentType;
            }
        }

        /// <summary>
        /// According to the OPC specification there is a quite complex algorithm to map
        /// from a ZIP entry name to a part name. It is complex because ZIP entry names
        /// are ASCII characters, whereas part names can contain Unicode characters,
        /// some characters need escaping and so on.
        /// These methods map from part name to zip name and back.
        ///
        /// So far there is a very simplistic implementation.
        ///
        /// RK We have to test with image file names containing Unicode characters and finish these methods.
        /// </summary>
        private static string PartNameToZipName(string partName)
        {
            return partName.TrimStart('/');
        }

        private static string ZipNameToPartName(string zipName)
        {
            // WORDSNET-6582 The problem occurred because paths to parts are separated by back-slash.
            // Fixed by replacing back-slash with slash during loading OPC package.
            return "/" + zipName.Replace(@"\", @"/");
        }

        public const string ContentTypesPartName = "/[Content_Types].xml";
        private readonly bool mSkipNonTextParts;

        private readonly ZipCompressionLevel mCompressionLevel = ZipWriterPal.DefaultCompressionLevel;
        private readonly Zip64Option mZip64Option = ZipWriterPal.DefaultZip64Mode;

        /// <summary>
        /// Callback to notify user about errors and warnings.
        /// </summary>
        private readonly IWarningCallbackCore mWarnCallback;
        private readonly TempFiler mTempFiler;
    }
}
