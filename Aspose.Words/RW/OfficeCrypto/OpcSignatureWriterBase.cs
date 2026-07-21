// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2014 by Alexey Morozov

using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Words.DigitalSignatures;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Base class for OPC package base document signature writers.
    /// </summary>
    internal abstract class OpcSignatureWriterBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="package">OPC package to sign.</param>
        /// <param name="xmlKeyInfo">True if certificate data should be written as KeyInfo XML element rather than separate file.</param>
        protected OpcSignatureWriterBase(OpcPackage package, bool xmlKeyInfo)
        {
            mPackage = package;
            mXmlKeyInfo = xmlKeyInfo;
        }

        /// <summary>
        /// Signs OpcPackage.
        /// </summary>
        internal void Write(DigitalSignature signature)
        {
            // Collect References.
            ReferenceCollection refs = new ReferenceCollection(new OpcReferenceResolver(mPackage));

            // Add root relationships.
            AddRelationshipReference(mPackage, null, refs);

            // Add parts which are not excluded and their relationships.
            foreach (OpcPackagePart part in mPackage.Parts)
            {
                if (IsExcludedPart(part) || (part.ContentType == OpcContentType.Relationships))
                    continue;

                // Add reference to part relationships.
                if (part.Rels.Count > 0)
                    AddRelationshipReference(mPackage, part, refs);

                refs.Add(new Reference(part.Name, part.ContentType));
            }

            string signatureOriginPartName = GetSignatureOriginPartName();
            OpcPackagePart signatureOriginPart = FetchSignatureOriginPart(mPackage, signatureOriginPartName, OpcRelationshipType.DigitalSignatureOrigin);

            string signaturePartName = GetSignaturePartName(mPackage);

            // Append signature to package.
            OpcPackagePart signaturePart = mPackage.CreateChildPart(
                signatureOriginPart,
                signaturePartName,
                OpcContentType.DigitalSignature,
                OpcRelationshipType.DigitalSignature);

            // Get signature stream.
            MemoryStream signatureStream = GetSignatureStream(refs, signature);
            signatureStream.Position = 0;
            StreamUtil.CopyStream(signatureStream, signaturePart.Stream);

            if (!mXmlKeyInfo)
            {
                string certificatePartName = GetCertificatePartName(mPackage);

                OpcPackagePart certificatePart = mPackage.CreateChildPart(
                                signaturePart,
                                certificatePartName,
                                OpcContentType.DigitalSignatureCertificate,
                                OpcRelationshipType.DigitalSignatureCertificate);

                byte[] exportedCert = Crypto.CryptoUtilPal.GetEncodedData(signature.CertificateHolderInternal);
                certificatePart.Stream.Write(exportedCert, 0, exportedCert.Length);
            }

            mPackage.UpdateRelationshipsAndContentTypes();
        }

        protected abstract string GetSignatureOriginPartName();

        /// <summary>
        /// Get part name for new signature.
        /// </summary>
        protected abstract string GetSignaturePartName(OpcPackage package);

        /// <summary>
        /// Gets part name for new signature.
        /// </summary>
        [JavaThrows(true)]
        protected abstract MemoryStream GetSignatureStream(ReferenceCollection refs, DigitalSignature signature);

        /// <summary>
        /// Gets part name for certificate.
        /// </summary>
        protected abstract string GetCertificatePartName(OpcPackage package);

        /// <summary>
        /// Indicates that part should be excluded from signing.
        /// </summary>
        protected abstract bool IsExcludedPart(OpcPackagePart part);

        /// <summary>
        /// Adds reference for relationship collection.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void AddRelationshipReference(OpcPackage package, OpcPackagePart part, ReferenceCollection refs);

        /// <summary>
        /// Returns existing or creates new signature origin part.
        /// </summary>
        private static OpcPackagePart FetchSignatureOriginPart(OpcPackage package, string name, string type)
        {
            OpcPackagePart originPart = package.GetPartByRelationshipType(null, type);

            if (originPart == null)
                originPart = package.CreateChildPart(null, name, OpcContentType.DigitalSignatureOrigin, type);

            return originPart;
        }

        private readonly OpcPackage mPackage;
        private readonly bool mXmlKeyInfo;
    }
}
