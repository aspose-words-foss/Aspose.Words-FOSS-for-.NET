// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2017 by Dmitry Belov

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
    /// The base class for XmlDsig signature generation for all MS formats (DOC, DOCX and XPS).
    /// </summary>
    internal abstract class XmlDsigSignerMsFormatBase : XmlDsigSignerBase
    {
        /// <summary>
        /// Writes Object XML nodes.
        /// </summary>
        protected override void WriteObjects(AnyXmlBuilder builder, MemoryStream[] objects)
        {
            foreach (MemoryStream objectNode in objects)
            {
                objectNode.Position = 0;
                builder.WriteRaw(objectNode, false);
            }
        }

        /// <summary>
        /// Writes KeyInfo XML element.
        /// </summary>
        [JavaThrows(true)]
        protected override void WriteKeyInfo(AnyXmlBuilder builder, DigitalSignature signature)
        {
            builder.StartElement("KeyInfo");

            WriteKeyValue(builder, signature.CertificateHolderInternal);

            builder.StartElement("X509Data");
            builder.WriteElement("X509Certificate", Convert.ToBase64String(CryptoUtilPal.GetEncodedData(signature.CertificateHolderInternal)));
            builder.EndElement();

            builder.EndElement("KeyInfo");
        }

        /// <summary>
        /// Writes KeyValue XML element.
        /// </summary>
        private static void WriteKeyValue(AnyXmlBuilder builder, CertificateHolderInternal cert)
        {
            CryptoAlgorithm cryptoAlgorithm = CryptoUtilPal.GetCryptoAlgorithmByPublicKey(cert);

            // Seems Word does not write 'KeyValue' element for ECDSA.
            if (cryptoAlgorithm == CryptoAlgorithm.ECDSA)
                return;

            builder.StartElement("KeyValue");

            if (cryptoAlgorithm == CryptoAlgorithm.DSA)
                WriteDsaKeyValue(builder, cert);
            else
                WriteRsaKeyValue(builder, cert);

            builder.EndElement("KeyValue");
        }

        private static void WriteDsaKeyValue(AnyXmlBuilder builder, CertificateHolderInternal cert)
        {
            DsaParameters parameters = CryptoUtilPal.GetDsaParametersFromPublicKey(cert);

            builder.StartElement("DSAKeyValue");
            builder.WriteElement("P", Convert.ToBase64String(parameters.P));
            builder.WriteElement("Q", Convert.ToBase64String(parameters.Q));
            builder.WriteElement("G", Convert.ToBase64String(parameters.G));
            builder.WriteElement("Y", Convert.ToBase64String(parameters.Y));
            builder.EndElement();
        }

        private static void WriteRsaKeyValue(AnyXmlBuilder builder, CertificateHolderInternal cert)
        {
            Rsa rsa = CryptoUtilPal.CreateRsaFromPublicKey(cert);

            builder.StartElement("RSAKeyValue");
            builder.WriteElement("Modulus", Convert.ToBase64String(rsa.Modulus.ToByteArray()));
            builder.WriteElement("Exponent", Convert.ToBase64String(rsa.Exponent.ToByteArray()));
            builder.EndElement();
        }

        /// <summary>
        /// Writes reference nodes with digest value for specified objects and references.
        /// </summary>
        protected override void WriteReferences(
            AnyXmlBuilder builder,
            MemoryStream[] objects,
            ReferenceCollection refs,
            DigestAlgorithm digestAlgorithm)
        {
            foreach (MemoryStream objectNode in objects)
                WriteReference(builder, objectNode, digestAlgorithm);
        }

        /// <summary>
        /// Gets SignatureId.
        /// </summary>
        protected override string GetSignatureId()
        {
            return "idPackageSignature";
        }


        /// <summary>
        /// Writes Reference XML element.
        /// </summary>
        private static void WriteReference(AnyXmlBuilder builder, MemoryStream objectStream, DigestAlgorithm digestAlgorithm)
        {
            string uri = string.Format("#{0}", GetObjectId(objectStream));

            // Check if the specified object stream contains a SignedProperties XML node,
            // then we need to write a reference to this inner node only.
            string signedPropertiesReference = GetSignedProperties(objectStream);
            MemoryStream stream = StringUtil.HasChars(signedPropertiesReference)
                ? new MemoryStream(Encoding.UTF8.GetBytes(signedPropertiesReference))
                : objectStream;

            C14Transform c14Transform = new C14Transform();
            c14Transform.PropagateNamespace(OfficeCryptoNames.DsigReferenceUri);
            string digest = ComputeBase64(c14Transform.Apply(stream), digestAlgorithm);

            string algorithm = DigitalSignatureUtil.GetDigestMethod(digestAlgorithm);

            WriteReference(builder, digest, uri, algorithm);
        }

        /// <summary>
        /// Returns a string value from an Id element of a specified Object stream.
        /// </summary>
        private static string GetObjectId(MemoryStream objectStream)
        {
            AnyXmlReader reader = new AnyXmlReader(objectStream);
            Debug.Assert(reader.LocalName == "Object");

            string id = reader.ReadAttribute("Id", null);
            if (id != null)
                return id;

            while (reader.ReadChild("Object"))
            {
                id = reader.ReadAttribute("Id", null);
                if (id != null)
                    return id;
            }

            return null;
        }

        /// <summary>
        /// Returns string value with a SignedProperties XML node of a specified object stream.
        /// </summary>
        private static string GetSignedProperties(MemoryStream objectStream)
        {
            AnyXmlReader reader = new AnyXmlReader(objectStream);
            Debug.Assert(reader.LocalName == "Object");

            while (reader.ReadChild("Object"))
            {
                if (reader.LocalName == "SignedProperties")
                    return reader.ReadOuterXml();
            }

            return null;
        }

        /// <summary>
        /// Writes Reference XML element.
        /// </summary>
        private static void WriteReference(AnyXmlBuilder builder, string digest, string uri, string digestMethodAlgorithm)
        {
            builder.StartElement("Reference");

            bool isSignedProperties = (uri == "#idSignedProperties");
            builder.WriteAttributeString("Type", (isSignedProperties) ? SignedPropertiesReferenceUri : ObjectReferenceUri);

            builder.WriteAttributeString("URI", uri);

            if (isSignedProperties)
                    WriteC14TransformElements(builder);

            builder.StartElement("DigestMethod");
            builder.WriteAttributeString("Algorithm", digestMethodAlgorithm);
            builder.EndElement();

            builder.WriteElement("DigestValue", digest);

            builder.EndElement("Reference");
        }

        private const string ObjectReferenceUri = "http://www.w3.org/2000/09/xmldsig#Object";
        private const string SignedPropertiesReferenceUri = "http://uri.etsi.org/01903#SignedProperties";
    }
}
