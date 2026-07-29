// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2007 by Vladimir Averkin
using System;
using System.IO;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Warnings;
using Aspose.Words.Loading;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.RW.OfficeCrypto;
using Aspose.Words.Saving;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// The main starter class for reading DOCX documents.
    /// </summary>
    internal class DocxReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal DocxReader(Stream stream, Document doc, LoadOptions loadOptions)
        {
            Debug.Assert(loadOptions != null);
            mTempFiler = new TempFiler(loadOptions.TempFolder);

            switch (doc.OriginalLoadFormat)
            {
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcTemplate:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                    mPackage = new OpcFlatPackage(stream);
                    break;
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                    {
                        IWarningCallbackCore callback = null;

                        if (doc.WarningCallback != null)
                            callback = new WarningCallbackCoreAdapter(doc.WarningCallback);

                        mPackage = new OpcPackage(stream, loadOptions.SkipFormatting, callback, mTempFiler);
                        break;
                    }
                default:
                    throw new InvalidOperationException(string.Format("Unexpected LoadFormat value {0}.", doc.OriginalLoadFormat));
            }

            mDoc = doc;
            mLoadOptions = loadOptions;

            // WORDSNET-17739 Mimic Word and use original document size, when retrieving "Bytes" property from the model.
            // Max size of file MSW can open 512 Mb.
            mDoc.BuiltInDocumentProperties[PropertyName.Bytes].DefaultValueInternal = (int)stream.Length;

            ReadSignatures();
        }

        /// <summary>
        /// Read digital signatures if they exists.
        /// </summary>
        internal void ReadSignatures()
        {
            OpcPackagePart originPart = mPackage.GetPartByRelationshipType(null, OpcRelationshipType.DigitalSignatureOrigin);

            // there is no signature data.
            if (originPart == null)
                return;

            foreach (OpcRelationship rel in originPart.Rels)
            {
                string signaturePartName = originPart.GetRelatedPartName(rel);

                OpcPackagePart signaturePart = mPackage.FetchPartByName(signaturePartName);
                XmlDsigReader.Read(signaturePart.Stream, new OpcReferenceResolver(mPackage),
                    mDoc.DigitalSignatures, mLoadOptions.WarningCallback);
            }
        }

        /// <summary>
        /// Imports a DOCX document.
        /// </summary>
        internal void Read()
        {
            try
            {
                // We have to check if this Docx document is ISO/IEC 29500 Strict.
                CheckIsoStrict();
                DocxCorePropertiesReader.Read(this);
                DocxExtentedPropertiesReader.Read(this);
                DocxCustomPropertiesReader.Read(this);

                DocxDocumentReader documentReader = ReadDocument();
                DocxTaskPaneAddinsReader.Read(documentReader);
                DocxCustomPartReader.Read(mPackage.Rels, mPackage, mDoc.PackageCustomParts);
            }
            finally
            {
                if (mTempFiler != null)
                    mTempFiler.Cleanup();
            }
        }

        private void CheckIsoStrict()
        {
            if (mPackage.Exists(RelTypes.OfficeDocument))
            {
                // Get document.xml part by transition namespace.
                mPackage.FetchPartByRelationshipType(null, RelTypes.OfficeDocument);
            }
            else
            {
                mRelTypes = null;

                // Actually we can stop here, and just call MarkAsIsoStrict(), but according to specification
                // ISO29500-1 p17.2.3 document has attribute w:conformance,
                // which we can also check to make sure it is strict.
                // Suppose we read strict OOXML.
                mDoc.ComplianceInfo.MarkAsIsoStrict();

                // Try to get document.xml part by strict namespace.
                OpcPackagePart documentPart = mPackage.FetchPartByRelationshipType(null, RelTypes.OfficeDocument);
                if (documentPart != null)
                {
                    DocxDocumentReader documentReader = DocxReaderFactory.CreateDocumentReader(
                        mPackage, documentPart, mDoc, mLoadOptions, mDoc.ComplianceInfo);

                    // If we successfully read OOXML document as strict, set the appropriate ComplianceInfo.
                    // According to specification ISO29500-1, if this attribute is omitted, its default value is transitional.
                    if (documentReader.XmlReader.ReadAttribute("conformance", "") != "strict")
                        mDoc.ComplianceInfo.Compliance = OoxmlComplianceCore.IsoTransitional;
                }
            }
        }

        private DocxDocumentReader ReadDocument()
        {
            OpcPackagePart documentPart = mPackage.FetchPartByRelationshipType(null, RelTypes.OfficeDocument);
            DocxDocumentReader documentReader = DocxReaderFactory.CreateDocumentReader(
                mPackage, documentPart, mDoc, mLoadOptions, mDoc.ComplianceInfo);
            documentReader.Read();

            return documentReader;
        }

        /// <summary>
        /// Creates a DOCX reader for a part that is a child of the package and
        /// at the end of the specified relationship. Returns null if there is no such part.
        /// </summary>
        internal DocxXmlReader CreatePackageChildPartReader(string relType)
        {
            OpcPackagePart part = mPackage.GetPartByRelationshipType(null, relType);
            return (part != null) ? new DocxXmlReader(part, Document.ComplianceInfo) : null;
        }

        /// <summary>
        /// You need to pass the "[Content_Types].xml" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static LoadFormat DetectLoadFormat(Stream contentTypesStream)
        {
            AnyXmlReader reader = new AnyXmlReader(contentTypesStream);
            while (reader.ReadChild("Types"))
            {
                switch (reader.LocalName)
                {
                    case "Default":
                    case "Override":
                    {
                        string contentType = reader.ReadAttribute("ContentType", "");
                        switch (contentType)
                        {
                            case DocxContentType.Document:
                                return LoadFormat.Docx;
                            case DocxContentType.DocumentMacroEnabled:
                                return LoadFormat.Docm;
                            case DocxContentType.Template:
                                return LoadFormat.Dotx;
                            case DocxContentType.TemplateMacroEnabled:
                                return LoadFormat.Dotm;
                            default:
                                break;
                        }
                        break;
                    }
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }
            return LoadFormat.Unknown;
        }

        /// <summary>
        /// You need to pass the "_rels/.rels" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static bool IsDigitalSignaturePresent(Stream relsStream)
        {
            return ContainsRelationshipOfType(relsStream, OpcRelationshipType.DigitalSignatureOrigin);
        }

        /// <summary>
        /// You need to pass the "_rels/.rels" stream positioned at the beginning.
        /// Does NOT preserve the stream position.
        /// </summary>
        internal static bool IsVbaProjectPresent(Stream relsStream)
        {
            return ContainsRelationshipOfType(relsStream, DocxRelationshipTypes.GetType(DocxRelationshipType.Vba, false));
        }

        private  static bool ContainsRelationshipOfType(Stream relsStream, string relType)
        {
            AnyXmlReader reader = new AnyXmlReader(relsStream);
            while (reader.ReadChild("Relationships"))
            {
                switch (reader.LocalName)
                {
                    case "Relationship":
                    {
                        if (reader.ReadAttribute("Type", "") == relType)
                            return true;

                        break;
                    }
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }
            return false;
        }

        internal DocxRelationshipTypes RelTypes
        {
            get
            {
                if (mRelTypes == null)
                    mRelTypes = new DocxRelationshipTypes(mDoc.ComplianceInfo.IsIsoStrict);
                return mRelTypes;
            }
        }

        internal Document Document
        {
            get { return mDoc; }
        }

        private readonly OpcPackageBase mPackage;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDoc;
        private readonly LoadOptions mLoadOptions;
        private DocxRelationshipTypes mRelTypes;
        private readonly TempFiler mTempFiler;
    }
}
