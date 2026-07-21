// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/05/2009 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Images;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Doc;
using Aspose.Words.RW.Doc.Reader;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Html.Reader;
using Aspose.Words.RW.Markdown.FormatDetector;
using Aspose.Words.RW.Mhtml.Reader;
using Aspose.Words.RW.Odt.Reader;
using Aspose.Words.RW.Txt.Reader;
using Aspose.Xml;

namespace Aspose.Words.RW.Factories
{
    /// <summary>
    /// Detects the file format.
    /// Use this as a normal instance (rather than static methods) to avoid loading
    /// of data for some formats twice when you need to perform detect+load.
    /// E.g. detecting a DOC file loads some parts of the structured storage directory into memory
    /// and you can access the "preloaded" structured storage as a property on this object.
    /// </summary>
    internal class FileFormatDetector
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static FileFormatDetector()
        {
            // Register encodings supported in the desktop .NET Framework but not in .NET Standard.
            EncodingUtil.RegisterEncodings();
        }

        /// <summary>
        /// <para>Detects the document load format.</para>
        /// <para>If detected format is HTML then <see cref="Words.FileFormatInfo.Encoding"/>
        /// property will contain the detected encoding.</para>
        /// </summary>
        internal FileFormatInfo Detect(Stream stream)
        {
            return Detect(stream, null);
        }

        /// <summary>
        /// <para>Detects the document load format. Allows specifying encoding of the detecting stream.</para>
        /// <para>If detected format is HTML then <see cref="Words.FileFormatInfo.Encoding"/>
        /// property will contain the detected encoding.</para>
        /// </summary>
        internal FileFormatInfo Detect(Stream stream, Encoding encoding)
        {
            mTextReader = new CustomTextReader(stream, encoding);
            mFileFormatInfo = new FileFormatInfo();

            try
            {
                // Don't change the order unless you are sure this will work.

                // Each Try* method can return false to try other format
                // or throw an exception to show that a format has been detected but the stream is invalid.

                if (TryDoc() || TryOoxmlAndOpenDocument() || TryRtf() || TryEncodingAwareFormats())
                {
                    // Detected
                }
            }
            finally
            {
                mTextReader.RewindStream();
            }

            return mFileFormatInfo;
        }

        /// <summary>
        /// Returns true if the file is a valid PDF document.
        /// Rewinds stream to initial position.
        /// </summary>
        internal static bool IsPdf(Stream stream)
        {
            CustomTextReader textReader = new CustomTextReader(stream);

            try
            {
                return TryPdf(textReader, null);
            }
            finally
            {
                textReader.RewindStream();
            }
        }

        /// <summary>
        /// Returns true if the file is a valid PDF document.
        /// </summary>
        private static bool TryPdf(CustomTextReader textReader, FileFormatInfo fileFormatInfo)
        {
            if (textReader.Stream.Length < gPdfHeader.Length + gPdfTrailer.Length)
                return false;

            // WORDSNET-19937 Experiments shows that Acrobat Reader handles files where %PDF-1.x header lies within the first 1024 bytes of the file.
            // CustomTextReader.Contains checks whether prefix of the stream contains given value.
            // Uses a part of the stream which is already read to internal buffer of the reader but
            // not more than 512 characters (it is enough for now but can be extended to 1024 if such document will be find out)
            // WORDSNET-25454 Check also PDF trailers.
            if (textReader.ContainsBinary(gPdfHeader) && HasPdfTrailer(textReader.Stream))
            {
                if (fileFormatInfo != null)
                    fileFormatInfo.SetLoadFormat(LoadFormat.Pdf);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true, if a specified stream has PDF format trailer in last 1024 bytes.
        /// </summary>
        private static bool HasPdfTrailer(Stream stream)
        {
            long initialStreamPosition = stream.Position;
            // Acrobat viewers require only that the %%EOF marker appear somewhere within the last 1024 bytes of the file.
            // Adobe Portable Document Format, implementation note 18 in Appendix H.
            long startPosition = stream.Length > PdfTrailerOffset ? stream.Length - PdfTrailerOffset : stream.Length - gPdfHeader.Length;
            stream.Position = startPosition;
            byte[] buffer = new byte[(int)(stream.Length - startPosition)];
            int readCount = stream.Read(buffer, 0, buffer.Length);
            int bytesIndex = 0;
            for (int bufferIndex = readCount - gPdfTrailer.Length; bufferIndex >= 0; bufferIndex--)
            {
                bytesIndex = 0;
                while ((bytesIndex < gPdfTrailer.Length) && (buffer[bufferIndex + bytesIndex] == gPdfTrailer[bytesIndex]))
                    bytesIndex++;

                if (bytesIndex == gPdfTrailer.Length)
                {
                    break;
                }
            }

            stream.Position = initialStreamPosition;
            return bytesIndex == gPdfTrailer.Length;
        }

        /// <summary>
        /// At the moment both DOC and DOT documents are recognized as <see cref="LoadFormat.Doc"/>.
        /// Rewinds the stream to initial position.
        /// </summary>
        /// <remarks>
        /// Actually this method tries all binary document formats.
        /// </remarks>>
        private bool TryDoc()
        {
            FileSystem fileSystem = GetFileSystemFromStream();

            if (fileSystem == null)
            {
                // Detect Word20/WordForMac50 document.
                mTextReader.RewindStream();
                if (mTextReader.HasBinaryPrefix(LegacyDocumentReader.FilePrefixWord20) ||
                    mTextReader.HasBinaryPrefix(LegacyDocumentReader.FilePrefixWordForMac50))
                {
                    mFileFormatInfo.SetLoadFormat(LoadFormat.Doc);
                    mFileFormatInfo.SetIsEncrypted(false);
                    return true;
                }

                return false;
            }

            // WORDSNET-4800 XLS or PPT files are recognized as DOC. We have to read a bit into the structured
            // storage to understand whether this is a Word document.
            if (fileSystem.ContainsInRootStorage("\x0006DataSpaces") ||
                (fileSystem.ContainsInRootStorage("EncryptedPackage") && fileSystem.ContainsInRootStorage("EncryptionInfo")))
            {
                // When an OOXML document is encrypted, it is not possible to ascertained whether it is
                // an Excel, Word or PowerPoint document without decrypting it first so we just return DOCX.
                mFileFormatInfo.SetLoadFormat(LoadFormat.Docx);
                mFileFormatInfo.SetIsEncrypted(true);
            }
            else if (fileSystem.ContainsInRootStorage("WordDocument"))
            {

                // SPEED Read just a few bytes from one stream into memory.
                // No warning is needed at this moment.
                Fib fib = new Fib(new BinaryReader(fileSystem.ReadOneSectorFromStreamInRootStorage("WordDocument")), null, 0);

                // WORDSNET-26415 Try to detect document embedded into WordDocument stream.
                if (!fib.IsValid)
                {
                    MemoryStream wordDocument = (MemoryStream)fileSystem.Root["WordDocument"];
                    FileFormatDetector innerDetector = new FileFormatDetector();

                    FileFormatInfo fi = innerDetector.Detect(wordDocument);

                    if (fi.LoadFormat != LoadFormat.Unknown)
                    {
                        mFileFormatInfo.EmbeddedInWordDocument = true;
                        mFileFormatInfo.SetLoadFormat(fi.LoadFormat);
                        mFileFormatInfo.SetEncoding(fi.Encoding);
                        return true;
                    }
                }

                if (fib.IsPreWord60)
                    mFileFormatInfo.SetLoadFormat(LoadFormat.DocPreWord60);
                else
                    mFileFormatInfo.SetLoadFormat((fib.fDot) ? LoadFormat.Dot : LoadFormat.Doc);

                mFileFormatInfo.SetIsEncrypted(fib.fEncrypted);

                // WORDSNET-17672 Check if document is digitally signed.
                // As far as I understood presence of those streams indicates that document is signed.
                bool isXmlDsig = fileSystem.ContainsInRootStorage("_xmlsignatures");
                bool isCryptoApi = fileSystem.ContainsInRootStorage("_signatures");

                mFileFormatInfo.SetIsDigitalSignaturePresent(isXmlDsig || isCryptoApi);

                if (isXmlDsig)
                    mFileFormatInfo.DigitalSignatureType = DigitalSignatureType.XmlDsig;
                else if (isCryptoApi)
                    mFileFormatInfo.DigitalSignatureType = DigitalSignatureType.CryptoApi;
                else
                    mFileFormatInfo.DigitalSignatureType = DigitalSignatureType.Unknown;

                mFileFormatInfo.HasMacros = fileSystem.Root.GetStorageSafe("Macros") != null;

                // Default value for DOC format.
                mFileFormatInfo.Application = ApplicationType.MsWord;
            }
            else if (fileSystem.ContainsInRootStorage("CONTENTS"))
            {
                BinaryReader reader = new BinaryReader((MemoryStream)fileSystem.Root["CONTENTS"]);
                string signature = Encoding.ASCII.GetString(reader.ReadBytes(8));
            }

            if (fileSystem.ContainsInRootStorage("WpsCustomData"))
                mFileFormatInfo.Application = ApplicationType.WpsOffice;

            bool result = (mFileFormatInfo.LoadFormat != LoadFormat.Unknown);

            mFileFormatInfo.FileSystem = (result) ? fileSystem : null;
            return (mFileFormatInfo.LoadFormat != LoadFormat.Unknown);
        }

        /// <summary>
        /// Returns <see cref="FileSystem"/> read from the stream or returns <c>null</c> if the stream is not FileSystem.
        /// May throw an exception if the stream is invalid FileSystem.
        /// </summary>
        private FileSystem GetFileSystemFromStream()
        {
            mTextReader.ResetState();
            return (mTextReader.HasBinaryPrefix(FileSystem.StructuredStorageSignature))
                ? new FileSystem(mTextReader.Stream)
                : null;
        }

        /// <summary>
        /// Detects OOXML or OpenDocument document. Both formats are ZIP archives and detection
        /// is combined into one method to avoid unzipping twice during detection.
        /// Returns <see cref="LoadFormat.Unknown"/> if not a recognized document type.
        ///
        /// Rewinds the stream to initial position.
        ///
        /// RK According to the ODT specification, it is possible to detect ODT documents without unzipping
        /// because the mimetype entry is first and not zipped and occurs at a fixed location, but our
        /// early ODT export versions exported mimetype zipped and this type of identification will fail.
        /// Therefore we use the unzip library to find the mimetype entry in any location.
        /// </summary>
        private bool TryOoxmlAndOpenDocument()
        {
            ZipReaderPal zipReader = null;

            try
            {
                // Safe try to get zip reader.
                zipReader = GetPalZipReader(mTextReader);
            }
            // WORDSNET-16557 Word generates "file corrupted error" if source file is zip but it has corrupted content.
            catch (ZipException ex)
            {
                throw new FileCorruptedException(ex);
            }
            catch
            {
                // Do nothing.
            }

            if (zipReader == null)
                return false;

            try
            {
                while (zipReader.MoveToNextEntry())
                {
                    // This code loops through all zip entries, it can be improved if really needed, but leave till later.

                    // WORDSNET-8722 Compare ignoring case because seen "mimetype" in upper case when ODT document created by Windows 7 WordPad.

                    // OOXML
                    if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "[Content_Types].xml"))
                    {
                        mFileFormatInfo.SetLoadFormat(DocxReader.DetectLoadFormat(zipReader.LoadEntryToMemory()));
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "_rels/.rels"))
                    {
                        mFileFormatInfo.SetIsDigitalSignaturePresent(
                            DocxReader.IsDigitalSignaturePresent(zipReader.LoadEntryToMemory()));
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, @"word/_rels/document.xml.rels"))
                    {
                        mFileFormatInfo.HasMacros =
                            DocxReader.IsVbaProjectPresent(zipReader.LoadEntryToMemory());
                    }
                    // ODT
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "mimetype"))
                    {
                        mFileFormatInfo.SetLoadFormat(OdtReader.DetectLoadFormat(zipReader.LoadEntryToMemory()));
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, @"META-INF/manifest.xml"))
                    {
                        MemoryStream manifestStream = zipReader.LoadEntryToMemory();

                        bool isEncrypted = OdtReader.IsEncrypted(manifestStream);
                        mFileFormatInfo.SetIsEncrypted(isEncrypted);

                        if (!isEncrypted)
                            mFileFormatInfo.SetLoadFormat(OdtReader.DetectLoadFormatByManifest(manifestStream));
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "META-INF/documentsignatures.xml"))
                    {
                        mFileFormatInfo.SetIsDigitalSignaturePresent(
                            OdtReader.IsDigitalSignaturePresent(zipReader.LoadEntryToMemory()));
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "word/settings.xml"))
                    {
                        if (HasWpsCustomDataScheme(zipReader.LoadEntryToMemory()))
                            mFileFormatInfo.Application = ApplicationType.WpsOffice;
                    }
                    else if (StringUtil.EqualsIgnoreCase(zipReader.EntryFileName, "docProps/app.xml"))
                    {
                        if (mFileFormatInfo.Application == ApplicationType.Unknown)
                        {
                            mFileFormatInfo.Application = GetApplicationType(
                                GetNameOfApplication(zipReader.LoadEntryToMemory()));
                        }
                    }
                }
            }
            finally
            {
                zipReader.Dispose();
            }

            return (mFileFormatInfo.LoadFormat != LoadFormat.Unknown);
        }

        /// <summary>
        /// Returns ApplicationType according to NameOfApplication attribute value.
        /// Shall be used as a low priority check since this attribute value is easy to correct manually.
        /// </summary>
        private static ApplicationType GetApplicationType(string applicationAttr)
        {
            // We cannot check equivalence to "Microsoft Office Word" because Word 2003 with CompatibilityPack2007
            // sets NameOfApplication as "Microsoft Office Outlook".
            if (applicationAttr.StartsWith("Microsoft Office", StringComparison.Ordinal))
                return ApplicationType.MsWord;
            if (applicationAttr.StartsWith("OpenOffice", StringComparison.Ordinal))
                return ApplicationType.OpenOfficeWriter;
            if (applicationAttr.StartsWith("WPS", StringComparison.Ordinal))
                return ApplicationType.WpsOffice;

            return ApplicationType.Unknown;
        }

        /// <summary>
        /// You need to pass the "word/settings.xml" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        private static bool HasWpsCustomDataScheme(Stream relsStream)
        {
            AnyXmlReader reader = new AnyXmlReader(relsStream);
            if (reader.LocalName != "settings")
                return false;
            string value = reader.ReadAttribute("wpsCustomData", "");

            return (value == "http://www.wps.cn/officeDocument/2013/wpsCustomData");
        }

        /// <summary>
        /// You need to pass the "word/settings.xml" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        private static string GetNameOfApplication(Stream relsStream)
        {
            if (relsStream.Length > 0)
            {
                AnyXmlReader reader = new AnyXmlReader(relsStream);

                while (reader.ReadChild("Properties"))
                {
                    switch (reader.LocalName)
                    {
                        case "Application":
                            return reader.ReadString();
                        default:
                            reader.IgnoreElement();
                            break;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks for the @"{\rtf" signature.
        /// RTF is a text format. Can it potentially be in different encodings?
        /// </summary>
        private bool TryRtf()
        {
            bool result = mTextReader.HasBinaryPrefix(@"{\rtf");

            if (result)
                mFileFormatInfo.SetLoadFormat(LoadFormat.Rtf);

            return result;
        }

        /// <summary>
        /// Returns true if any encoding-aware format is detected. The document in these formats can have
        /// various encodings and they will be recognized. Remembers the detected encoding for HTML only.
        ///
        /// Preserves the stream position, but positions the stream to skip UTF-7 BOM
        /// if one is found because .NET cannot read it.
        /// </summary>
        private bool TryEncodingAwareFormats()
        {
            if (TryPdf(mTextReader, mFileFormatInfo))
                return true;

            // BOM is found on CustomTextReader creation and will be ignored during detection MHTML.
            // Although MHTML must be in ASCII we ignore erroneous BOM at the beginning if any.

            if (TryMhtml())
                return true;

            mTextReader.SetEncodingByBom();

            if (TryEncodingAwareFormatsOneEncoding())
                return true;

            FileFormatInfo fileFormatInfo = DetectMarkdown();
            if (fileFormatInfo != null)
            {
                mFileFormatInfo = fileFormatInfo;
                return true;
            }

            // WORDSNET-23492 Check known image formats explicitly to filter out.
            FileFormat fileFormat = ImageUtil.GetImageType(mTextReader.Stream);
            if (fileFormat != FileFormat.Unknown)
                return false;

            // If all format detection attempts, except plain text, are failed, use plain text detection result
            fileFormatInfo = DetectPlainText();
            if (fileFormatInfo != null)
            {
                mFileFormatInfo = fileFormatInfo;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries detecting encoding-aware formats on one encoding.
        /// </summary>
        private bool TryEncodingAwareFormatsOneEncoding()
        {
            return TryWml() || TryFlatOpc() || TryHtml();
        }

        /// <summary>
        /// Returns true and updates FileFormatInfo accordingly if the file is a valid MHTML document.
        /// </summary>
        private bool TryMhtml()
        {
            FileFormatInfo fileFormatInfo = MhtmlFormatDetector.Detect(mTextReader);

            if (fileFormatInfo != null)
            {
                mFileFormatInfo = fileFormatInfo;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true and updates FileFormatInfo accordingly if the file is a valid HTML document.
        /// </summary>
        private bool TryHtml()
        {
            mTextReader.SetEncodingByBom();

            FileFormatInfo fileFormatInfo = HtmlFormatDetector.Detect(mTextReader);

            if (fileFormatInfo.LoadFormat == LoadFormat.Html)
            {
                mFileFormatInfo = fileFormatInfo;
                return true;
            }

            return false;
        }

        /// <summary>
        /// If the file is a valid plain text document, records its load format and encoding in returned FileFormatInfo object.
        /// </summary>
        private FileFormatInfo DetectPlainText()
        {
            mTextReader.StepFirst();
            return TxtFormatDetector.Detect(mTextReader);
        }

        /// <summary>
        /// Returns FileFormatInfo object if the file is a markdown document, or <c>null</c> otherwise.
        /// </summary>
        private FileFormatInfo DetectMarkdown()
        {
            mTextReader.StepFirst();
            return MarkdownFormatDetector.Detect(mTextReader);
        }

        /// <summary>
        /// Returns true if this is a WordML document. We do not return the encoding because our importer
        /// reads it using XmlReader that automatically detects encoding of an XML document.
        /// </summary>
        private bool TryWml()
        {
            bool isDetected = mTextReader.Contains("<w:wordDocument");

            if (isDetected)
                mFileFormatInfo.SetLoadFormat(LoadFormat.WordML);

            return isDetected;
        }

        /// <summary>
        /// Returns true if this is a FlatOpc (Word 2007 XML) document. We do not return the encoding because our importer
        /// reads it using XmlReader that automatically detects encoding of an XML document.
        /// </summary>
        private bool TryFlatOpc()
        {
            // AN: The first condition detects if it is Flat Opc package.
            // The second detects if it is Word document. This is needed because any office documents can be saved as Flat Opc.
            // I think it is enough to detect Word 2007 XML documents.
            // AM. FlatOpc alt chunk does not contain mso-application element.
            bool isDetected = mTextReader.Contains("<pkg:package");

            if (isDetected)
            {
                mTextReader.ResetState();
                OpcFlatPackage package = new OpcFlatPackage(mTextReader.Stream);

                // MS Word does not allow saving Word XML as strict, that is why try to get the part only by transitional namespace (isStrict=false).
                OpcPackagePart documentPart = package.GetPartByRelationshipType(null, DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, false));
                if (documentPart == null)
                    // Try to get a workbookPart by the strict DocxRelationshipType.
                    documentPart = package.GetPartByRelationshipType(null, DocxRelationshipTypes.GetType(DocxRelationshipType.OfficeDocument, true));

                switch (documentPart.ContentType)
                {
                    case DocxContentType.Document:
                        mFileFormatInfo.SetLoadFormat(LoadFormat.FlatOpc);
                        break;
                    case DocxContentType.DocumentMacroEnabled:
                        mFileFormatInfo.SetLoadFormat(LoadFormat.FlatOpcMacroEnabled);
                        break;
                    case DocxContentType.Template:
                        mFileFormatInfo.SetLoadFormat(LoadFormat.FlatOpcTemplate);
                        break;
                    case DocxContentType.TemplateMacroEnabled:
                        mFileFormatInfo.SetLoadFormat(LoadFormat.FlatOpcTemplateMacroEnabled);
                        break;
                    default:
                        // Unknown content type.
                        isDetected = false;
                        break;
                }
            }

            return isDetected;
        }

        /// <summary>
        /// Returns <see cref="ZipReaderPal"/> built over the stream or returns <c>null</c> if the stream is not Zip package.
        /// May throw an exception if the stream is invalid Zip package.
        /// </summary>
        private static ZipReaderPal GetPalZipReader(CustomTextReader textReader)
        {
            textReader.ResetState();
            return (textReader.HasBinaryPrefix(ZipReaderPal.FileHeaderSignature))
                ? new ZipReaderPal(textReader.Stream)
                : null;
        }

        /// <summary>
        /// Gets detected <see cref="System.Text.Encoding"/>.
        /// </summary>
        internal Encoding Encoding
        {
            get { return mTextReader.Encoding; }
        }

        private static readonly byte[] gPdfHeader = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }; // %PDF-
        private static readonly byte[] gPdfTrailer = new byte[] { 0x25, 0x25, 0x45, 0x4F, 0x46 }; // %%EOF
        private const int PdfTrailerOffset = 1024;

        private CustomTextReader mTextReader;
        private FileFormatInfo mFileFormatInfo;
    }
}
