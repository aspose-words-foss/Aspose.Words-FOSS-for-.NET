// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2010 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Words.DigitalSignatures;
using Aspose.Words.Nrx;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements XmlDsig signature generation for OOXML documents.
    /// </summary>
    internal class XmlDsigSignerOoxml : XmlDsigSignerMsFormatBase
    {
        internal override MemoryStream GetSignature(ReferenceCollection refs, DigitalSignature signature)
        {
            return GetSignatureCore(refs, signature, true);
        }

        /// <summary>
        /// Gets array of Object XML elements that will be signed.
        /// </summary>
        protected override MemoryStream[] GetObjects(ReferenceCollection refs, DigitalSignature signature)
        {
            int objectsCount = DigitalSignature.GetObjectsCount(signature.XmlDsigLevel);
            MemoryStream[] objects = new MemoryStream[objectsCount];
            objects[0] = GetPackageObject(refs, signature);
            objects[1] = GetOfficeObject(signature);

            if (signature.XmlDsigLevel >= XmlDsigLevel.XAdEsEpes)
                objects[2] = GetSignedProperties(signature);

            return objects;
        }

        /// <summary>
        /// Gets PackageObject XML node.
        /// </summary>
        private static MemoryStream GetPackageObject(ReferenceCollection refs, DigitalSignature signature)
        {
            AnyXmlBuilder builder = new AnyXmlBuilder(new MemoryStream(), new UTF8Encoding(false), false);

            builder.StartElement("Object");
            builder.WriteAttributeString("Id", "idPackageObject");
            // IN. Actually Word writes this namespace into SignatureTime element below. However, it computes digest
            // when this namespace is written to this PackageObject. Before I changed this code per WORDSNET-10816,
            // we used system SignedXml class, which is writing this namespace to SignatureTime.
            // BUT then we INTENTIONALLY copied it to the PackageObject node! I've checked and seems both approaches
            // are working in Word and signatures are valid in both cases. So, I leave this decision as it already worked for us.
            builder.WriteAttributeString("xmlns:mdssi", DocxNamespaces.GetNamespace(DocxNamespace.DigitalSignature, false));

            builder.StartElement("Manifest");
            DigestAlgorithm digestAlgorithm = CryptoUtilPal.GetDigestAlgorithmByPrivateKey(signature.CertificateHolderInternal);
            foreach (Reference r in refs)
                r.Write(builder, digestAlgorithm);
            builder.EndElement("Manifest");

            builder.StartElement("SignatureProperties");
            builder.StartElement("SignatureProperty");
            builder.WriteAttributeString("Id", "idSignatureTime");
            builder.WriteAttributeString("Target", "#idPackageSignature");

            builder.StartElement("mdssi:SignatureTime");
            builder.WriteElement("mdssi:Format", "YYYY-MM-DDThh:mm:ssTZD");
            builder.WriteElement("mdssi:Value", FormatterPal.DateTimeToXmlUtc(signature.SignTime));
            builder.EndElement("mdssi:SignatureTime");

            builder.EndElement("SignatureProperty");
            builder.EndElement("SignatureProperties");

            builder.EndElement("Object");

            builder.Flush();
            return (MemoryStream)builder.BaseStream;
        }

        /// <summary>
        /// Gets OfficeObject XML node.
        /// </summary>
        private static MemoryStream GetOfficeObject(DigitalSignature signature)
        {
            AnyXmlBuilder builder = new AnyXmlBuilder(new MemoryStream(), new UTF8Encoding(false), false);

            builder.StartElement("Object");
            builder.WriteAttributeString("Id", "idOfficeObject");

            builder.StartElement("SignatureProperties");
            builder.StartElement("SignatureProperty");
            builder.WriteAttributeString("Id", "idOfficeV1Details");
            builder.WriteAttributeString("Target", "#idPackageSignature");

            builder.StartElement("SignatureInfoV1");
            builder.WriteAttributeString("xmlns", DocxNamespaces.GetNamespace(DocxNamespace.MicrosoftDigitalSignature, false));

            // See MS-OI29500, 3.7.2.5 ST_SignatureType (Signature Type)
            // https://msdn.microsoft.com/en-us/library/ff531759(v=office.12).aspx
            bool isVisibleSignatureLine = !signature.SetupId.Equals(Guid.Empty);
            if (isVisibleSignatureLine)
                builder.WriteElement("SetupID", signature.SetupId.ToString("B").ToUpper());
            else
                builder.WriteEmptyElement("SetupID");

            builder.WriteEmptyElement("SignatureText");

            if (isVisibleSignatureLine && ArrayUtil.HasData(signature.ImageBytes))
                builder.WriteElement("SignatureImage", Convert.ToBase64String(signature.ImageBytes));
            else
                builder.WriteEmptyElement("SignatureImage");

            builder.WriteElement("SignatureComments", signature.Comments);
            builder.WriteElement("WindowsVersion", "6.1");
            builder.WriteElement("OfficeVersion", "12.0");
            builder.WriteElement("ApplicationVersion", "12.0");
            builder.WriteElement("Monitors", "1");
            builder.WriteElement("HorizontalResolution", "1920");
            builder.WriteElement("VerticalResolution", "1200");
            builder.WriteElement("ColorDepth", "32");
            builder.WriteElement("SignatureProviderId", signature.ProviderId.Equals(Guid.Empty)
                ? "{00000000-0000-0000-0000-000000000000}"
                : signature.ProviderId.ToString("B").ToUpper());
            builder.WriteEmptyElement("SignatureProviderUrl");
            builder.WriteElement("SignatureProviderDetails", "9");
            builder.WriteElement(
                "ManifestHashAlgorithm",
                (signature.XmlDsigLevel == XmlDsigLevel.XmlDSig) ? @"http://www.w3.org/2000/09/xmldsig#sha1" : "");
            builder.WriteElement("SignatureType", isVisibleSignatureLine ? "2" : "1");

            builder.EndElement("SignatureInfoV1");
            builder.EndElement("SignatureProperty");
            builder.EndElement("SignatureProperties");
            builder.EndElement("Object");

            builder.Flush();
            return (MemoryStream)builder.BaseStream;
        }

        /// <summary>
        /// Gets SignedProperties XML node.
        /// </summary>
        private static MemoryStream GetSignedProperties(DigitalSignature signature)
        {
            AnyXmlBuilder builder = new AnyXmlBuilder(new MemoryStream(), new UTF8Encoding(false), false);

            builder.StartElement("Object");

            builder.StartElement("xd:QualifyingProperties");
            builder.WriteAttributeString("xmlns:xd", "http://uri.etsi.org/01903/v1.3.2#");
            builder.WriteAttributeString("Target", "#idPackageSignature");

            builder.StartElement("xd:SignedProperties");
            builder.WriteAttributeString("Id", "idSignedProperties");

            builder.StartElement("xd:SignedSignatureProperties");
            builder.WriteElement("xd:SigningTime", FormatterPal.DateTimeToXmlUtc(signature.SignTime));

            builder.StartElement("xd:SigningCertificate");
            builder.StartElement("xd:Cert");

            builder.StartElement("xd:CertDigest");
            builder.StartElement("DigestMethod");
            builder.WriteAttributeString("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256");
            builder.EndElement("DigestMethod");

            DigestAlgorithm digestAlgorithm = CryptoUtilPal.GetDigestAlgorithmByPrivateKey(signature.CertificateHolderInternal);
            byte[] certHash = HashUtil.ComputeHash(
                CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm),
                CryptoUtilPal.GetEncodedData(signature.CertificateHolderInternal));
            builder.WriteElement("DigestValue", Convert.ToBase64String(certHash));
            builder.EndElement("xd:CertDigest");

            builder.StartElement("xd:IssuerSerial");
            builder.WriteElement("X509IssuerName", CryptoUtilPal.GetIssuer(signature.CertificateHolderInternal));
            builder.WriteElement("X509SerialNumber", CryptoUtilPal.GetSerialNumber(signature.CertificateHolderInternal));
            builder.EndElement("xd:IssuerSerial");

            builder.EndElement("xd:Cert");
            builder.EndElement("xd:SigningCertificate");

            builder.StartElement("xd:SignaturePolicyIdentifier");
            builder.WriteEmptyElement("xd:SignaturePolicyImplied ");
            builder.EndElement("xd:SignaturePolicyIdentifier");

            builder.EndElement("xd:SignedSignatureProperties");

            builder.EndElement("xd:SignedProperties");
            builder.EndElement("xd:QualifyingProperties");
            builder.EndElement("Object");

            builder.Flush();
            return (MemoryStream)builder.BaseStream;
        }
    }
}
