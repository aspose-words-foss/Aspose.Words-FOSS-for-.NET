// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2010 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Crypto;
using Aspose.JavaAttributes;
using Aspose.Words.DigitalSignatures;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Base class for XmlDsig signers.
    /// </summary>
    internal abstract class XmlDsigSignerBase
    {
        [JavaThrows(true)]
        internal abstract MemoryStream GetSignature(ReferenceCollection refs, DigitalSignature signature);

        /// <summary>
        /// Base method to write signatures.
        /// </summary>
        protected MemoryStream GetSignatureCore(ReferenceCollection refs, DigitalSignature signature, bool xmlKeyInfo)
        {
            // Get Object nodes that will be signed.
            MemoryStream[] objects = GetObjects(refs, signature);

            CryptoAlgorithm cryptoAlgorithm = CryptoUtilPal.GetCryptoAlgorithmByPrivateKey(signature.CertificateHolderInternal);
            DigestAlgorithm digestAlgorithm = CryptoUtilPal.GetDigestAlgorithmByPrivateKey(signature.CertificateHolderInternal);

            // Get SignedInfo node with signatures for Object nodes.
            MemoryStream signedInfo = GetSignedInfo(objects, refs, cryptoAlgorithm, digestAlgorithm);

            // Make initial XML file that will be signed.
            AnyXmlBuilder builder = new AnyXmlBuilder(new MemoryStream(), new UTF8Encoding(false), false);

            // Start document to write XML declaration.
            builder.StartDocument("Signature");
            builder.WriteAttributeString("xmlns", OfficeCryptoNames.DsigReferenceUri);
            builder.WriteAttributeString("Id", GetSignatureId());

            WriteSignedInfo(builder, signedInfo);
            WriteSignatureValue(builder, signedInfo, signature);
            if (xmlKeyInfo)
                WriteKeyInfo(builder, signature);
            WriteObjects(builder, objects);

            builder.FullEndElement("Signature");

            builder.Flush();
            return (MemoryStream)builder.BaseStream;
        }

        /// <summary>
        /// Returns Base64 string of a hash calculated for a specified stream and crypto algorithm.
        /// </summary>
        protected static string ComputeBase64(MemoryStream stream, DigestAlgorithm digestAlgorithm)
        {
            byte[] hash = HashUtil.ComputeHash(CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm), stream.ToArray());
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Writes C14Transform elements.
        /// </summary>
        protected static void WriteC14TransformElements(AnyXmlBuilder builder)
        {
            builder.StartElement("Transforms");

            C14Transform c14Transform = new C14Transform();
            c14Transform.Write(builder);

            builder.EndElement("Transforms");
        }

        /// <summary>
        /// Gets array of Object XML elements that will be signed.
        /// </summary>
        [JavaThrows(true)]
        protected abstract MemoryStream[] GetObjects(ReferenceCollection refs, DigitalSignature signature);

        /// <summary>
        /// Writes reference nodes with digest value for specified objects and references.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteReferences(
            AnyXmlBuilder builder,
            MemoryStream[] objects,
            ReferenceCollection refs,
            DigestAlgorithm digestAlgorithm);

        /// <summary>
        /// Writes KeyInfo XML element.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteKeyInfo(AnyXmlBuilder builder, DigitalSignature signature);

        /// <summary>
        /// Writes array of Object XML elements.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteObjects(AnyXmlBuilder builder, MemoryStream[] objects);

        /// <summary>
        /// Returns signature id.
        /// </summary>
        protected abstract string GetSignatureId();

        /// <summary>
        /// Gets SignedInfo XML element.
        /// </summary>
        private MemoryStream GetSignedInfo(
            MemoryStream[] objects,
            ReferenceCollection refs,
            CryptoAlgorithm cryptoAlgorithm,
            DigestAlgorithm digestAlgorithm)
        {
            AnyXmlBuilder builder = new AnyXmlBuilder(new MemoryStream(), new UTF8Encoding(false), false);

            builder.StartElement("SignedInfo");

            builder.StartElement("CanonicalizationMethod");
            builder.WriteAttributeString("Algorithm", C14Transform.Algorithm);
            builder.EndElement();

            builder.StartElement("SignatureMethod");
            builder.WriteAttributeString("Algorithm", GetAlgorithmUri(cryptoAlgorithm, digestAlgorithm));
            builder.EndElement();

            WriteReferences(builder, objects, refs, digestAlgorithm);

            builder.EndElement("SignedInfo");

            builder.Flush();
            return (MemoryStream)builder.BaseStream;
        }

        /// <summary>
        /// Returns algorithm URI string by a specified crypto and digest algorithms.
        /// </summary>
        private static string GetAlgorithmUri(CryptoAlgorithm algorithm, DigestAlgorithm digestAlgorithm)
        {
            switch (algorithm)
            {
                case CryptoAlgorithm.RSA:
                    return GetRsaUri(digestAlgorithm);

                case CryptoAlgorithm.DSA:
                    return "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

                case CryptoAlgorithm.ECDSA:
                    return "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256";

                default:
                    throw new InvalidOperationException("Unexpected CryptoAlgorithm value.");
            }
        }

        /// <summary>
        /// Returns RSA URI string by a specified digest algorithm.
        /// </summary>
        /// <remarks>
        /// See https://learn.microsoft.com/en-us/windows/win32/seccrypto/xml-digital-signature-cryptographic-algorithms
        /// </remarks>
        private static string GetRsaUri(DigestAlgorithm digestAlgorithm)
        {
            switch (digestAlgorithm)
            {
                case DigestAlgorithm.Sha256:
                    return "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

                case DigestAlgorithm.Sha384:
                    return "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

                case DigestAlgorithm.Sha512:
                    return "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

                default:
                    return "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
            }
        }

        /// <summary>
        /// Writes SignedInfo XML element.
        /// </summary>
        private static void WriteSignedInfo(AnyXmlBuilder builder, MemoryStream signedInfo)
        {
            signedInfo.Position = 0;
            builder.WriteRaw(signedInfo, false);
        }

        /// <summary>
        /// Writes SignatureValue XML element.
        /// </summary>
        private static void WriteSignatureValue(AnyXmlBuilder builder, MemoryStream signedInfo, DigitalSignature signature)
        {
            builder.StartElement("SignatureValue");
            builder.WriteRaw(ComputeSignatureValue(signedInfo, signature));
            builder.EndElement();
        }

        /// <summary>
        /// Returns signature value for the specified SignedInfo XML element.
        /// </summary>
        private static string ComputeSignatureValue(MemoryStream signedInfo, DigitalSignature signature)
        {
            DigestAlgorithm digestAlgorithm = CryptoUtilPal.GetDigestAlgorithmByPrivateKey(signature.CertificateHolderInternal);

            C14Transform c14Transform = new C14Transform();
            c14Transform.PropagateNamespace(OfficeCryptoNames.DsigReferenceUri);
            byte[] signedInfoBytes = c14Transform.Apply(signedInfo).ToArray();
            byte[] signatureBytes = CryptoUtilPal.GenerateSignature(signature.CertificateHolderInternal, signedInfoBytes, digestAlgorithm);

            return Convert.ToBase64String(signatureBytes);
        }
    }
}
