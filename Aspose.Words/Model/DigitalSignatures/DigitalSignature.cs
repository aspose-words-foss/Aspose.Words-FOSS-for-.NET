// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2010 by Alexey Morozov

using System;
using Aspose.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Represents a digital signature on a document and the result of its verification.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    public class DigitalSignature
    {
        /// <summary>
        /// Creates DigitalSignature object which can be used to sign document. Certificate must have private keys.
        /// </summary>
        internal DigitalSignature(CertificateHolder holder) : this(holder.ToInternal())
        {
            mCertificateHolder = holder;
            mSignTime = DateTime.Now;
        }

        internal DigitalSignature(DigitalSignatureType sigType)
            : this(CertificateHolderInternal.Create())
        {
            mSignatureType = sigType;
            mSignTime = DateTime.MinValue;
            mComments = "";
        }

        private DigitalSignature(CertificateHolderInternal holder)
        {
            mCertificateHolderInternal = holder;
            mCertificateHolder = new CertificateHolder(mCertificateHolderInternal);
            SetXmlDsigLevel();
        }

        /// <summary>
        /// Gets the type of the digital signature.
        /// </summary>
        public DigitalSignatureType SignatureType
        {
            get { return mSignatureType; }
        }

        /// <summary>
        /// Gets the time the document was signed.
        /// </summary>
        public DateTime SignTime
        {
            get { return mSignTime; }
        }
        /// <summary>
        /// Internal setter for public property <see cref="SignTime" />.
        /// </summary>
        internal void SetSignTime(DateTime signTime)
        {
            mSignTime = signTime;
        }

        /// <summary>
        /// Gets the signing purpose comment.
        /// </summary>
        public string Comments
        {
            get { return mComments; }
        }
        /// <summary>
        /// Internal setter for public property <see cref="Comments" />.
        /// </summary>
        internal void SetComments(string comments)
        {
            mComments = comments;
        }

        /// <summary>
        /// Returns the subject distinguished name of the certificate that was used to sign the document.
        /// </summary>
        public string SubjectName
        {
            get { return mCertificateHolder.Certificate.Subject; }
        }

        /// <summary>
        /// Returns the subject distinguished name of the certificate isuuer.
        /// </summary>
        public string IssuerName
        {
            get { return mCertificateHolder.Certificate.Issuer; }
        }

        /// <summary>
        /// Returns <c>true</c> if this digital signature is valid and the document has not been tampered with.
        /// </summary>
        public bool IsValid
        {
            get { return mIsValid; }
        }

        /// <summary>
        /// Internal setter for public property <see cref="IsValid" />.
        /// </summary>
        internal void SetIsValid(bool result)
        {
            mIsValid = result;
        }

        /// <summary>
        /// Returns the certificate holder object that contains the certificate was used to sign the document.
        /// </summary>
        public CertificateHolder CertificateHolder
        {
            get { return mCertificateHolder; }
        }

        /// <summary>
        /// Gets an array of bytes representing a signature value.
        /// </summary>
        public byte[] SignatureValue { get; internal set; }

        /// <summary>
        /// Internal setter for public property <see cref="Words.DigitalSignatures.CertificateHolder.Certificate" />.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        internal void SetCertificate(CertificateHolderInternal certificate)
        {
            mCertificateHolderInternal.CertificateBc = certificate.CertificateBc;
            SetXmlDsigLevel();
        }

        /// <summary>
        /// Returns the certificate holder object that contains the certificate was used to sign the document.
        /// For internal use only.
        /// </summary>
        internal CertificateHolderInternal CertificateHolderInternal
        {
            get { return mCertificateHolderInternal; }
        }

        /// <summary>
        /// Specifies an image for the digital signature.
        /// </summary>
        /// <remarks>
        /// This is image that user selected to sign document.
        /// </remarks>
        internal byte[] ImageBytes
        {
            get { return mImage; }
            set { mImage = value; }
        }

        /// <summary>
        /// Specifies the image of a valid signature.
        /// </summary>
        /// <remarks>
        /// This is rendered image which consist of signature placeholder with
        /// signature image <see cref="DigitalSignature.ImageBytes" /> placed over.
        /// </remarks>
        internal byte[] ImageBytesValid
        {
            get { return mImageValid; }
            set { mImageValid = value; }
        }

        /// <summary>
        /// Specifies the image of an invalid signature.
        /// </summary>
        /// <remarks>
        /// This is the same as valid image <see cref="DigitalSignature.ImageBytesValid" /> but red inscription
        /// "Invalid signature" over it.
        /// </remarks>
        internal byte[] ImageBytesInvalid
        {
            get { return mImageInvalid; }
            set { mImageInvalid = value; }
        }

        /// <summary>
        /// Indicates that the digital signature MUST be printed.
        /// </summary>
        internal bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        /// <summary>
        /// Specifies the text of actual signature in the digital signature.
        /// </summary>
        internal string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        /// <summary>
        /// Specifies a GUID that can be cross-referenced with the identifier of the signature line
        /// stored in the document content.
        /// </summary>
        internal Guid SetupId
        {
            get { return mSetupId; }
            set { mSetupId = value; }
        }

        /// <summary>
        /// Specifies the class identifier of the signature provider.
        /// </summary>
        /// <remarks>
        /// Office 2010 and the 2007 Office system reserve the value of {00000000-0000-0000-0000-000000000000} for
        /// its default signature provider and {000CD6A4-0000-0000-C000-000000000046} for its East Asian signature provider.
        /// </remarks>
        internal Guid ProviderId
        {
            get { return mProviderId; }
            set { mProviderId = value; }
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        internal AsymmetricKeyParameter PrivateKey
        {
            get { return mCertificateHolderInternal.PrivateKeyBc; }
            set { mCertificateHolderInternal.PrivateKeyBc = value; }
        }

        /// <summary>
        /// Specifies the level of a digital signature based on XML-DSig standard.
        /// </summary>
        internal XmlDsigLevel XmlDsigLevel { get; set; }

        /// <summary>
        /// Utility method extracts name from distinguished name.
        /// Used in testing only.
        /// </summary>
        internal static string ExtractCn(string cn)
        {
            int idx1 = cn.IndexOf("CN=", StringComparison.Ordinal);

            if (idx1 == -1)
                return cn;

            int idx2 = cn.IndexOf(",", idx1, StringComparison.Ordinal);
            if (idx2 == -1)
                idx2 = cn.Length;
            return cn.Substring(idx1 + 3, idx2 - idx1 - 3);
        }

        /// <summary>
        /// Returns a user-friendly string that displays the value of this object.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        public override string ToString()
        {
            X509Certificate certificate = mCertificateHolderInternal.CertificateBc;
            return string.Format("Signature [{0}, {1}, {2}/{3}, {4}/{5}]",
                mSignatureType,
                mIsValid ? "Valid" : "Not valid",
                (certificate != null) ? ExtractCn(certificate.CertificateStructure.Subject.ToString()) : "?",
                (certificate != null) ? ExtractCn(certificate.CertificateStructure.Issuer.ToString()) : "?",
                mSignTime != DateTime.MinValue ? mSignTime.ToString() : "?",
                (certificate != null) ? certificate.NotAfter.ToString() : "?");
        }

        /// <summary>
        /// Specifies the number of Objects in XML-DSig signature level.
        /// </summary>
        /// <remarks>
        /// XML-DSig signature level has only 2 Objects. The next signature level XAdES-EPES
        /// will have additional object with reference URI="#idSignedProperties".
        /// </remarks>
        internal static int GetObjectsCount(XmlDsigLevel xmlDsigLevel)
        {
            return xmlDsigLevel == XmlDsigLevel.XmlDSig ? 2 : 3;
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        // WORDSNET-18890 For ECDSA Word writes one additional PackageSignature Object.
        private void SetXmlDsigLevel()
        {
            if (CryptoUtilPal.GetCryptoAlgorithmByPrivateKey(mCertificateHolderInternal) == CryptoAlgorithm.ECDSA)
                XmlDsigLevel = XmlDsigLevel.XAdEsEpes;
        }

        private readonly DigitalSignatureType mSignatureType;
        private readonly CertificateHolder mCertificateHolder;
        private readonly CertificateHolderInternal mCertificateHolderInternal;
        private DateTime mSignTime;
        private string mComments;
        private bool mIsValid;
        private byte[] mImage;
        private byte[] mImageValid;
        private byte[] mImageInvalid;
        private string mText;
        private Guid mSetupId = Guid.Empty;
        private Guid mProviderId = Guid.Empty;
        private bool mVisible;
    }
}
