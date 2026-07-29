// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2014 by Alexey Morozov

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Aspose.Crypto;
using Aspose.Words.DigitalSignatures;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Utility class for XmlDsig signature verification. See http://www.w3.org/TR/xmldsig-core/.
    /// Processes signedXml itself, verifies each reference and fills signatures collection.
    /// </summary>
    internal class XmlDsigReader
    {
        /// <summary>
        /// Avoid instantiate.
        /// </summary>
        private XmlDsigReader(IWarningCallback warningCallback)
        {
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Implements reading of XmlDsig document and verifies all external references.
        /// </summary>
        /// <param name="stream">Stream which contains XmlDsig signature</param>
        /// <param name="resolver">Reference resolver for certain document type.</param>
        /// <param name="signatures">Signature collection where signatures and verification result should be put.</param>
        /// <param name="warningCallback">Optional warning callback.</param>
        internal static void Read(Stream stream, IReferenceResolver resolver, DigitalSignatureCollection signatures, IWarningCallback warningCallback)
        {
            XmlDsigReader reader = new XmlDsigReader(warningCallback);
            reader.ReadCore(stream, null, resolver, signatures);
        }

        /// <summary>
        /// Implements reading of XmlDsig document and verifies all external references.
        /// </summary>
        /// <param name="stream">Stream which contains XmlDsig signature</param>
        /// <param name="certificate">Certificate used to sign document</param>
        /// <param name="resolver">Reference resolver for certain document type.</param>
        /// <param name="signatures">Signature collection where signatures and verification result should be put.</param>
        /// <param name="warningCallback">Optional warning callback.</param>
        internal static void Read(Stream stream, CertificateHolderInternal certificate, IReferenceResolver resolver, DigitalSignatureCollection signatures, IWarningCallback warningCallback)
        {
            XmlDsigReader reader = new XmlDsigReader(warningCallback);
            reader.ReadCore(stream, certificate, resolver, signatures);
        }

        private void ReadCore(Stream stream, CertificateHolderInternal certificate, IReferenceResolver resolver, DigitalSignatureCollection signatures)
        {
            // If no XmlDsig stream provided return.
            if (stream == null)
                return;

            AnyXmlReader xmlReader = new AnyXmlReader(stream);

            if (xmlReader.LocalName == "Signature")
            {
                // There is only one <Signature> element.
                signatures.Add(ReadAndVerifySignature(stream, certificate, resolver));
            }
            else
            {
                // ODT document holds signatures in one document so we should process them one by one.
                // Extract each <Signature> element.
                bool isReadChild = xmlReader.ReadChild("document-signatures");
                while (isReadChild && // Infinite loop without it if <document-signatures .../>.
                       !xmlReader.IsEndElement("document-signatures"))
                {
                    switch (xmlReader.LocalName)
                    {
                        case "Signature":
                            string signatureXml = xmlReader.ReadOuterXml();
                            MemoryStream signatureStream = new MemoryStream(Encoding.UTF8.GetBytes(signatureXml));
                            signatures.Add(ReadAndVerifySignature(signatureStream, certificate, resolver));
                            break;
                        default:
                            xmlReader.IgnoreElement();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads 'Signature' element. Returns verified DigitalSignature object.
        /// </summary>
        private DigitalSignature ReadAndVerifySignature(Stream stream, CertificateHolderInternal certificate, IReferenceResolver resolver)
        {
            DigitalSignature signature = new DigitalSignature(DigitalSignatureType.XmlDsig);
            ReferenceCollection referenceCollection = new ReferenceCollection(resolver);

            byte[] certificateBytes = null;

            string signedInfoRaw = GetSignedInfoRaw(stream);
            string signatureMethod = "";

            AnyXmlReader xmlReader = new AnyXmlReader(stream);
            while (xmlReader.ReadChild("Signature"))
            {
                switch (xmlReader.LocalName)
                {
                    case "SignatureMethod":
                        signatureMethod = xmlReader.ReadAttribute("Algorithm", "");
                        break;

                    case "Reference":
                    {
                        Reference reference = new Reference(xmlReader);
                        referenceCollection.Add(reference);

                        // WORDSNET-27192 Added ability to specify XML-DSig signature level.
                        if (reference.Uri == "#idSignedProperties")
                            signature.XmlDsigLevel = XmlDsigLevel.XAdEsEpes;

                        break;
                    }

                    case "X509Certificate":
                        certificateBytes = Convert.FromBase64String(xmlReader.ReadString());
                        break;

                    case "SignatureValue":
                        signature.SignatureValue = Convert.FromBase64String(xmlReader.ReadString());
                        break;

                    case "Object":
                        string id = xmlReader.ReadAttribute("Id", "");
                        if(id == "idValidSigLnImg")
                            signature.ImageBytesValid = Convert.FromBase64String(xmlReader.ReadString());
                        else if(id == "idInvalidSigLnImg")
                            signature.ImageBytesInvalid = Convert.FromBase64String(xmlReader.ReadString());
                        break;

                    case "SignatureProperty":
                        ReadSignatureProperty(xmlReader, signature);
                        break;

                    // WORDSNET-18890 When algorithm is ECDSA, Word stores signing time in this element.
                    case "SigningTime":
                        signature.SetSignTime(ParseSignTime(xmlReader.ReadString()));
                        break;

                    default:
                        // Do not ignore element, go deeper.
                        break;
                }
            }

            if (StringUtil.HasChars(signedInfoRaw))
            {
                certificate = (certificate == null)
                        ? CryptoUtilPal.CreateHolder(certificateBytes)
                        : certificate;

                signature.SetCertificate(certificate);

                // Verify SignedInfo element.
                bool totalResult = VerifySignedInfo(signedInfoRaw, signatureMethod, certificate, signature.SignatureValue);

                // Verification process has two stages:
                //  1) Verify signed XML itself i.e SignedInfo element is unchanged.
                //  2) Verify references i.e content of referenced files is unchanged.
                // If signed XML is not valid it means that signature is not valid and further processing is actually not needed
                // but lets continue to get each reference verification result.

                // Verify all references.
                foreach (Reference r in referenceCollection)
                    totalResult &= (r.IsExternal)
                        ? VerifyExternalReference(r, resolver)
                        : VerifyInternalReference(r, stream);

                signature.SetIsValid(totalResult);

                // Issue warning for invalid signature.
                if (!signature.IsValid)
                    Warn(WarningType.UnexpectedContent, WarningSource.Unknown, WarningStrings.InvalidDigitalSignature);
            }

            return signature;
        }

        /// <summary>
        /// Extracts 'SignedInfo' element as raw string.
        /// </summary>
        private static string GetSignedInfoRaw(Stream stream)
        {
            AnyXmlReader xmlReader = new AnyXmlReader(stream);

            // SignedInfo MUST be first child of Signature so don't loop.
            xmlReader.ReadChild("Signature");
            return (xmlReader.LocalName == "SignedInfo")
                ? xmlReader.ReadOuterXml()
                : null;
        }

        /// <summary>
        /// Verifies 'SignedInfo' element using given certificate and signature hash.
        /// </summary>
        private bool VerifySignedInfo(string signedInfoRaw, string signatureMethod, CertificateHolderInternal certificate, byte[] signature)
        {
            byte[] signedInfoBytes = Canonicalize(signedInfoRaw);

            DigestAlgorithm digestAlgorithm = GetDigestAlgorithm(signatureMethod);
            if (digestAlgorithm == DigestAlgorithm.Unknown)
                return false;

            return CryptoUtilPal.VerifySignature(certificate, digestAlgorithm, signedInfoBytes, signature);
        }

        /// <summary>
        /// Verifies hash using given algorithm.
        /// </summary>
        private bool VerifyHash(byte[] data, string algorithmId, byte[] hash)
        {
            DigestAlgorithm digestAlgorithm = GetDigestAlgorithm(algorithmId);
            if (digestAlgorithm == DigestAlgorithm.Unknown)
                return false;

            byte[] computedHash = HashUtil.ComputeHash(CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm), data);

            return ArrayUtil.CompareBytes(hash, computedHash, hash.Length);
        }

        /// <summary>
        /// Verifies external reference.
        /// </summary>
        private bool VerifyExternalReference(Reference r, IReferenceResolver resolver)
        {
            MemoryStream stream = r.TransformCollection.Apply(resolver.Resolve(r));
            r.IsValid = VerifyHash(stream.ToArray(), r.DigestMethod, Convert.FromBase64String(r.DigestValue));

            return r.IsValid;
        }

        /// <summary>
        /// Verifies internal reference.
        /// </summary>
        private bool VerifyInternalReference(Reference r, Stream xml)
        {
            string elementRaw = GetElementRaw(xml, r.Uri);

            // If element is not found reference is treated not valid.
            r.IsValid = StringUtil.HasChars(elementRaw)
                && VerifyHash(Canonicalize(elementRaw), r.DigestMethod, Convert.FromBase64String(r.DigestValue));

            return r.IsValid;
        }

        /// <summary>
        /// Canonicalize given XML. Returns byte array.
        /// </summary>
        private static byte[] Canonicalize(string xml)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

            C14Transform c14Transform = new C14Transform();
            c14Transform.PropagateNamespace(OfficeCryptoNames.DsigReferenceUri);

            return c14Transform.Apply(stream).ToArray();
        }

        /// <summary>
        /// Returns digest corresponding to given algorithm.
        /// Null, if algorithm is not supported.
        /// </summary>
        private DigestAlgorithm GetDigestAlgorithm(string algorithmId)
        {
            switch (algorithmId)
            {
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512":
                case "http://www.w3.org/2001/04/xmlenc#sha512":
                    return DigestAlgorithm.Sha512;

                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#sha256":
                case "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256":
                    return DigestAlgorithm.Sha256;

                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#sha1":
                    return DigestAlgorithm.Sha1;

                default:
                    // WORDSNET-12579
                    // Now we don't support all algorithms for digest and certificate (etc ECGost3410), but we need to read file without exception.
                    // As completed solution We need to support all algorithms and throw exception for unsupported algorithms.
                    Warn(WarningType.UnexpectedContent, WarningSource.Unknown, string.Format(WarningStrings.NotSupportedAlgorithm, algorithmId));
                    return DigestAlgorithm.Unknown;
            }
        }

        /// <summary>
        /// Extracts XML element with given 'Id' attribute as raw string.
        /// </summary>
        /// <returns>Element as raw string or null if missing.</returns>
        private static string GetElementRaw(Stream xmlStream, string elementId)
        {
            string stripId = elementId.Substring(1);

            AnyXmlReader xmlReader = new AnyXmlReader(xmlStream);

            string tagName = xmlReader.LocalName;
            while(xmlReader.ReadChild(tagName))
            {
                string id = xmlReader.ReadAttribute("Id", "");

                if (!StringUtil.HasChars(id) || (id != stripId))
                    continue;

                // Element is found, return raw string.
                return xmlReader.ReadOuterXml();
            }

            return null;
        }

        private void ReadSignatureProperty(AnyXmlReader xmlReader, DigitalSignature signature)
        {
            while (xmlReader.ReadChild("SignatureProperty"))
            {
                switch (xmlReader.LocalName)
                {
                    case "SignatureTime":
                        // OOXML stores signTime in this attribute.
                        ReadSignatureTime(xmlReader, signature);
                        break;
                    case "SignatureInfoV1":
                        ReadSignatureInfoV1(xmlReader, signature);
                        break;
                    case "date":
                        // ODT stores signTime in this element.
                        signature.SetSignTime(ParseSignTime(xmlReader.ReadString()));
                        break;
                    default:
                        WarnAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Process idSignatureTime property element and extract it's value.
        /// </summary>
        private void ReadSignatureTime(AnyXmlReader xmlReader, DigitalSignature signature)
        {
            // <SignatureProperty Id="idSignatureTime" Target="#idPackageSignature">
            //   <mdssi:SignatureTime>
            //     <mdssi:Format>YYYY-MM-DDThh:mm:ssTZD</mdssi:Format>
            //     <mdssi:Value>2010-08-30T04:50:01Z</mdssi:Value>
            //   </mdssi:SignatureTime>
            // </SignatureProperty>
            while (xmlReader.ReadChild("SignatureTime"))
            {
                switch (xmlReader.LocalName)
                {
                    case "Value":
                        signature.SetSignTime(ParseSignTime(xmlReader.ReadString()));
                        break;
                    default:
                        WarnAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        private static void ReadSignatureInfoV1(AnyXmlReader xmlReader, DigitalSignature signature)
        {
            while (xmlReader.ReadChild("SignatureInfoV1"))
            {
                switch (xmlReader.LocalName)
                {
                    case "SignatureComments":
                        signature.SetComments(xmlReader.ReadString());
                        break;
                    case "SetupID":
                        string id = xmlReader.ReadString();
                        signature.SetupId = StringUtil.HasChars(id) ? new Guid(id) : Guid.Empty;
                        break;
                    case "SignatureText":
                        signature.Text = xmlReader.ReadString();
                        break;
                    case "SignatureType":
                        signature.Visible = (xmlReader.ReadString() == "2");
                        break;
                    case "SignatureImage":
                        string image = xmlReader.ReadString();
                        signature.ImageBytes = StringUtil.HasChars(image) ? Convert.FromBase64String(image) : null;
                        break;
                    case "SignatureProviderId":
                        string providerId = xmlReader.ReadString();
                        signature.ProviderId = StringUtil.HasChars(providerId) ? new Guid(providerId) : Guid.Empty;
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static DateTime ParseSignTime(string value)
        {
            // AM. It seems I should use Format element to get datetime format but format specifiers are not always the same as for ParseExact().
            return DateTime.ParseExact(value, gInputDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private void Warn(WarningType type, WarningSource source, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(type, source, description));
        }

        /// <summary>
        /// Writes warning for ignored element and then ignores element.
        /// </summary>
        private void WarnAndIgnoreElement(AnyXmlReader xmlReader)
        {
            // Create warning message.
            string message = String.Format("Import of element '{0}' is not supported by Aspose.Words.", xmlReader.LocalName);
            Warn(WarningType.UnexpectedContent, WarningSource.Unknown, message);

            xmlReader.IgnoreElement();
        }

        private readonly IWarningCallback mWarningCallback;

        private static readonly string[] gInputDateTimeFormats =
            {
                "yyyy-MM-ddTHH:mm:ss,FF",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fzzz",
                "yyyy-MM-ddTHH:mm:ss.fffzzz"
            };
    }
}
