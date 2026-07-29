// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/05/2007 by Vladimir Averkin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.RW.OfficeCrypto;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Main starter class for writing a document to OOXML.
    /// Also provides services that are global for the whole package, e.g. naming of images or embedded objects.
    /// </summary>
    internal class DocxWriter : IDocumentWriter
    {
        internal DocxWriter()
        {
            Package = new OpcPackage();
        }

        SaveOutputParameters IDocumentWriter.SaveToStream(SaveInfo saveInfo)
        {
            SaveInfo = saveInfo;

            CheckCanSaveMacros();

            switch (saveInfo.SaveFormat)
            {
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    Package = new OpcFlatPackage();
                    break;

                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                {
                    // WORDSNET-19478 Use a specific comparator.
                    OoxmlSaveOptions saveOptions = (OoxmlSaveOptions)saveInfo.SaveOptions;
                    Package = new OpcPackage(
                        new OpcPartsComparer(),
                        GetCorrespondingCompressionLevel(saveOptions.CompressionLevel),
                        GetCorrespondingZip64Option(saveOptions.Zip64Mode));
                    break;
                }

                default:
                    throw new InvalidOperationException(string.Format("Unexpected SaveFormat value {0}.", saveInfo.SaveFormat));
            }

            mTempFiler = new TempFiler(saveInfo.SaveOptions.TempFolder);

            DocxDocumentWriter documentWriter = WriteDocument();
            DocxTaskPaneAddinsWriter.Write(documentWriter);
            DocxExtentedPropertiesWriter.Write(this);
            WriteCoreProperties();
            DocxCustomPropertiesWriter.Write(this);

            List<string> nonCustomPartNames = new List<string>();
            foreach (OpcPackagePart part in Package.Parts)
                nonCustomPartNames.Add(part.Name);
            WritePackageCustomParts("/", nonCustomPartNames);

            OpcRelsWriter.Write(Package, IsPrettyFormat);

            if (IsWriteContentTypes(saveInfo.SaveFormat))
                OpcContentTypesWriter.Write((OpcPackage)Package, IsPrettyFormat);

            OoxmlSaveOptions so = saveInfo.SaveOptions as OoxmlSaveOptions;

            // WORDSNET-25896 Implemented SaveOptions.DigitalSignatureDetails option.
            if ((so != null) && (so.DigitalSignatureDetails != null) && IsDigitalSignatureSupported(saveInfo.SaveFormat))
            {
                DigitalSignature signature = so.DigitalSignatureDetails.GetDigitalSignature();
                if (signature != null)
                {
                    OpcSignatureWriterDocx signatureWriter = new OpcSignatureWriterDocx((OpcPackage)Package);
                    signatureWriter.Write(signature);
                }
            }

            if((so != null) && (StringUtil.HasChars(so.Password)))
            {
                // Write encrypted.
                MemoryStream tempStream = new MemoryStream();
                Package.Save(tempStream);

                FileSystem fs = DataSpaces.Encrypt(tempStream, so.Password);
                fs.Save(saveInfo.Stream);
            }
            else
            {
                // Write normal.
                Package.Save(saveInfo.Stream);
            }

            mTempFiler.Cleanup();

            return new SaveOutputParameters(GetDocumentContentType());
        }

        /// <summary>
        /// Gets a boolean value indicating either <see cref="DigitalSignature"/>
        /// writing is supported for a specified save format.
        /// </summary>
        private static bool IsDigitalSignatureSupported(SaveFormat saveFormat)
        {
            return (saveFormat == SaveFormat.Docx) ||
                   (saveFormat == SaveFormat.Dotx) ||
                   (saveFormat == SaveFormat.Docm) ||
                   (saveFormat == SaveFormat.Dotm);
        }

        /// <summary>
        /// Gets corresponding Zip.CompressionLevel according to Saving.CompressionLevel
        /// </summary>
        private static ZipCompressionLevel GetCorrespondingCompressionLevel(Saving.CompressionLevel compressionLevel)
        {
            switch (compressionLevel)
            {
                case Saving.CompressionLevel.SuperFast:
                    return ZipCompressionLevel.Level1;
                case Saving.CompressionLevel.Fast:
                    return ZipCompressionLevel.Level3;
                case Saving.CompressionLevel.Normal:
                    return ZipCompressionLevel.Level5;
                case Saving.CompressionLevel.Maximum:
                    return ZipCompressionLevel.Level9;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Zip64Option GetCorrespondingZip64Option(Zip64Mode zip64)
        {
            switch (zip64)
            {
                case Zip64Mode.Never:
                    return Zip64Option.Never;
                case Zip64Mode.IfNecessary:
                    return Zip64Option.AsNecessary;
                case Zip64Mode.Always:
                    return Zip64Option.Always;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsWriteContentTypes(SaveFormat saveFormat)
        {
            // For FlatOpc we do not need to write ContentTypes XML because content type is written as an attribute of each part.
            return
                (saveFormat == SaveFormat.Docx) ||
                (saveFormat == SaveFormat.Docm) ||
                (saveFormat == SaveFormat.Dotx) ||
                (saveFormat == SaveFormat.Dotm);
        }

        private void CheckCanSaveMacros()
        {
            if (!SaveInfo.Document.HasMacros)
                return;

            switch (SaveInfo.SaveFormat)
            {
                case SaveFormat.Docx:
                case SaveFormat.Dotx:
                case SaveFormat.FlatOpc:
                    throw new InvalidOperationException(
                        "This document contains macros (VBA project) and you are " +
                        "attempting to save it in a Macro-Free format. " +
                        "Such document will be invalid if created. " +
                        "You need to either save it in a Macro-Enabled format (.DOCM or .DOTM) or " +
                        "remove macros before saving using the Document.RemoveMacros method.");
                case SaveFormat.Docm:
                case SaveFormat.Dotm:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    // Do nothing, all ok.
                    break;
                default:
                    throw new InvalidOperationException("Unexpected save format.");
            }
        }

        private string GetDocumentContentType()
        {
            switch (SaveInfo.SaveFormat)
            {
                case SaveFormat.Docx:
                case SaveFormat.FlatOpc:
                    return DocxContentType.Docx;
                case SaveFormat.Docm:
                case SaveFormat.FlatOpcMacroEnabled:
                    return DocxContentType.Docm;
                case SaveFormat.Dotx:
                case SaveFormat.FlatOpcTemplate:
                    return DocxContentType.Dotx;
                case SaveFormat.Dotm:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    return DocxContentType.Dotm;
                default:
                    throw new InvalidOperationException("Unexpected save format.");
            }
        }

        private DocxDocumentWriter WriteDocument()
        {
            SaveInfo.MakeZIndexDocxStyle();

            DocxDocumentWriter docWriter = new DocxDocumentWriter(this, mTempFiler);
            docWriter.Write();

            return docWriter;
        }

        private void WriteCoreProperties()
        {
            DocxBuilder builder = CreatePartAndBuilder(@"/docProps/core.xml", DocxContentType.CoreProperties, RelTypes.CoreProperties);
            BuiltInDocumentProperties props = Document.BuiltInDocumentProperties;
            OpcCorePropertiesWriter.Write(
                builder,
                props.Title,
                props.Subject,
                props.Author,
                props.Keywords,
                props.Comments,
                props.LastSavedBy,
                props.RevisionNumber.ToString(),
                props.LastPrinted,
                props.CreatedTime,
                props.LastSavedTime,
                props.Category,
                props.ContentStatus,
                props[PropertyName.Language].Value.ToString(),
                props[PropertyName.DocVersion].Value.ToString());
        }

        private void WritePackageCustomParts(string parentName, ICollection<string> nonCustomPartNames)
        {
            foreach (CustomPart cPart in Document.PackageCustomParts)
            {
                if (cPart.ParentPartName != parentName)
                    continue;

                // WORDSNET-5682 Adds +1 to custom part name if it already exists, mimics MSWord behavior.
                // WORDSNET-22604 We need to check for matches only with non-custom parts.
                if (nonCustomPartNames.Contains(cPart.Name))
                {
                    int index = cPart.Name.LastIndexOf(".", StringComparison.Ordinal);
                    cPart.Name = cPart.Name.Insert(index, "1");
                }

                if (cPart.IsExternal)
                {
                    Package.Rels.Add(cPart.RelationshipType, cPart.Name, true);
                }
                else
                {
                    string pName = cPart.ParentPartName;
                    OpcPackagePart pPart = (pName == RootName)
                        ? null
                        : Package.FetchPartByName(pName);

                    // For nested custom parts apply preserved relation identifiers.
                    AddInnerRelationExplicitly(pPart, cPart);

                    // WORDSNET-22604 If OpcPackagePart already exists, just do nothing.
                    if (!Package.Parts.Contains(cPart.Name))
                    {
                        OpcPackagePart opcPart = Package.CreateChildPart(
                            pPart,
                            cPart.Name,
                            cPart.ContentType,
                            cPart.RelationshipType);

                        // It is necessary to use this constructor to avoid UnauthorizedAccessException while writing
                        // resources as base64. Some of constructors do not expose the underlying stream as the result
                        // "GetBuffer" throws UnauthorizedAccessException.
                        opcPart.Stream = new MemoryStream(cPart.Data, 0, cPart.Data.Length, true, true);

                        // WORDSNET-10389 Custom parts can has relationships with another parts recursively.
                        WritePackageCustomParts(opcPart.Name, nonCustomPartNames);
                    }
                }
            }
        }

        /// <summary>
        /// Add nested custom part relation data to relationship collection of the parent element.
        /// Method applied only for parts stored in package.
        /// </summary>
        /// <param name="pPart">Parent part which contains target relationship collection.</param>
        /// <param name="cPart">Custom nested part which related with parent.</param>
        private static void AddInnerRelationExplicitly(OpcPackagePart pPart, CustomPart cPart)
        {
            Debug.Assert(cPart != null);

            if ((pPart == null) || cPart.IsExternal)
                return;

            string keyName = OpcPackageBase.MakeRelative(pPart.Name, cPart.Name);
            pPart.Rels.Add(cPart.OriginalId, cPart.RelationshipType, keyName, cPart.IsExternal);
        }

        internal DocxRelationshipTypes RelTypes
        {
            get
            {
                return mRelTypes ??
                       (mRelTypes = new DocxRelationshipTypes(Compliance == OoxmlComplianceCore.IsoStrict));
            }
        }

        internal DocxNamespaces DocxNamespaces
        {
            get
            {
                return mDocxNamespaces ??
                       (mDocxNamespaces = new DocxNamespaces(Compliance == OoxmlComplianceCore.IsoStrict));
            }
        }

        /// <summary>
        /// Creates an XML part at the root of the package and creates a builder for it.
        /// </summary>
        internal DocxBuilder CreatePartAndBuilder(string partName, string contentType, string relType)
        {
            OpcPackagePart part = Package.CreateChildPart(null, partName, contentType, relType);
            return new DocxBuilder(part, IsPrettyFormat, Compliance, MsWordVersionCore.Unspecified, WarningCallback);
        }

        internal OpcPackageBase Package { get; private set; }

        internal Document Document
        {
            get { return SaveInfo.Document; }
        }

        internal SaveInfo SaveInfo { get; set; }

        private bool IsPrettyFormat
        {
            get { return SaveOptions.PrettyFormat; }
        }

        /// <summary>
        /// Generated names for embedded parts are sequentially numbered. There is one sequence per relationship type.
        /// One sequence goes across the main and glossary documents.
        /// </summary>
        internal int GetNextEmbeddedPartNumber(string relType)
        {
            int i = mEmbeddedPartNumbers[relType];
            if (StringToIntDictionary.IsNullSubstitute(i))
                i = 0;

            i++;
            mEmbeddedPartNumbers[relType] = i;
            return i;
        }

        /// <summary>
        /// If the given image was already written, returns its part name.
        /// Otherwise returns null.
        /// </summary>
        internal string GetImagePartName(byte[] imageBytes)
        {
            // andrnosk: we cannot use byte array as a key, because byte[]{1,2,3} != byte[]{1,2,3},
            // that is why use MD4Hash.
            return mImageBytesTable[HashUtil.GetSHA512Hash(imageBytes)];
        }

        /// <summary>
        /// Adds a mapping between an image that was already written and its part name
        /// so it can be reused if the same image occurs in the document again.
        /// </summary>
        internal void AddImageBytes(byte[] imageBytes, string imagePartName)
        {
            // andrnosk: we cannot use byte array as a key, because byte[]{1,2,3} != byte[]{1,2,3},
            // that is why use MD4Hash.
            mImageBytesTable.Add(HashUtil.GetSHA512Hash(imageBytes), imagePartName);
        }

        internal IWarningCallback WarningCallback
        {
            get { return SaveInfo.Document.WarningCallback; }
        }

        private OoxmlSaveOptions SaveOptions
        {
            get { return (OoxmlSaveOptions)SaveInfo.SaveOptions; }
        }

        /// <summary>
        /// Gets the OOXML version for the output document.
        /// </summary>
        private OoxmlComplianceCore Compliance
        {
            get { return OoxmlComplianceInfo.GetCompliance(Document.ComplianceInfo, SaveOptions); }
        }

        private readonly BytesHashToObjDictionary<string> mImageBytesTable = new BytesHashToObjDictionary<string>();
        /// <summary>
        /// Key is a string that is a relationship type.
        /// Value is an integer number last used to generate a name for an embedded part of that relationship type.
        /// Used to generate names like image1.png, image2.png and so on.
        /// </summary>
        private readonly StringToIntDictionary mEmbeddedPartNumbers = new StringToIntDictionary();

        private TempFiler mTempFiler;
        private DocxRelationshipTypes mRelTypes;
        private DocxNamespaces mDocxNamespaces;

        private const string RootName = "/";
    }
}
