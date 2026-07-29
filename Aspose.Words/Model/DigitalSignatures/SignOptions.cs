// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2017 by Ilya Navrotskiy

using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Words.Drawing;

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Allows to specify options for document signing.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    public class SignOptions
    {
        /// <summary>
        /// Applies all properties to the <paramref name="signature"/>.
        /// </summary>
        internal void ApplyTo(DigitalSignature signature)
        {
            signature.SetComments(Comments);
            signature.SetSignTime(SignTime);
            signature.SetupId = SignatureLineId;
            signature.ImageBytes = SignatureLineImage;
            signature.ProviderId = ProviderId;
            signature.XmlDsigLevel = XmlDsigLevel;
        }

        /// <summary>
        /// Specifies comments on the digital signature.
        /// Default value is <b>empty string</b><ms>(<see cref="string.Empty"/>)</ms>.
        /// </summary>
        public string Comments
        {
            get { return mComments; }
            set { mComments = value; }
        }

        /// <summary>
        /// The date of signing.
        /// Default value is <b>current time</b><ms> (<see cref="DateTime.Now"/>)</ms><cpp> (<see cref="DateTime.Now"/>)</cpp>
        /// </summary>
        public DateTime SignTime
        {
            get { return mSignTime; }
            set { mSignTime = value; }
        }

        /// <summary>
        /// Signature line identifier.
        /// Default value is <b>Empty (all zeroes) Guid</b>.
        /// </summary>
        /// <remarks>
        /// When set, it associates <see cref="SignatureLine"/> with corresponding <see cref="DigitalSignature"/>.
        /// </remarks>
        public Guid SignatureLineId
        {
            get { return mSignatureLineId; }
            set { mSignatureLineId = value; }
        }

        /// <summary>
        /// The image that will be shown in associated <see cref="SignatureLine"/>.
        /// Default value is <c>null</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Public API, as designed.")]
        public byte[] SignatureLineImage
        {
            get { return mSignatureLineImage; }
            set { mSignatureLineImage = value; }
        }

        /// <summary>
        /// The password to decrypt source document.
        /// Default value is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>.
        /// </summary>
        /// <remarks>
        /// If OOXML document is encrypted, you should provide decryption password
        /// to decrypt source document before it will be signed.
        /// This is not required for documents in binary DOC format.
        /// </remarks>
        public string DecryptionPassword
        {
            get { return mDecryptionPassword; }
            set { mDecryptionPassword = value; }
        }

        /// <summary>
        /// Specifies the class ID of the signature provider.
        /// Default value is <b>Empty (all zeroes) Guid</b>.
        /// </summary>
        /// <remarks>
        /// <para>The cryptographic service provider (CSP) is an independent software module that actually performs
        /// cryptography algorithms for authentication, encoding, and encryption. MS Office reserves the value
        /// of {00000000-0000-0000-0000-000000000000} for its default signature provider.</para>
        /// <para>The GUID of the additionally installed provider should be obtained from the documentation shipped with the provider.</para>
        /// <para>In addition, all the installed cryptographic providers are enumerated in windows registry.
        /// It can be found in the following path: HKLM\SOFTWARE\Microsoft\Cryptography\Defaults\Provider.
        /// There is a key name "CP Service UUID" which corresponds to a GUID of signature provider.</para>
        /// </remarks>
        public Guid ProviderId
        {
            get { return mProviderId; }
            set { mProviderId = value; }
        }

        /// <summary>
        /// Specifies the level of a digital signature based on XML-DSig standard.
        /// The default value is <see cref="DigitalSignatures.XmlDsigLevel.XmlDSig"/>.
        /// </summary>
        /// <remarks>
        /// Different levels of XAdES signatures can be created starting from Office 2010.
        /// </remarks>
        public XmlDsigLevel XmlDsigLevel { get; set; }

        private string mComments = string.Empty;
        private DateTime mSignTime = DateTime.Now;
        private Guid mSignatureLineId = Guid.Empty;
        private Guid mProviderId = Guid.Empty;
        private byte[] mSignatureLineImage;
        private string mDecryptionPassword = string.Empty;
    }
}
