// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2023 by Ilya Navrotskiy

using Aspose.Words.DigitalSignatures;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Contains details for signing a document with a digital signature.
    /// </summary>
    public class DigitalSignatureDetails
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DigitalSignatureDetails"/> class.
        /// </summary>
        /// <param name="certificateHolder">A certificate holder which contains the certificate itself.</param>
        /// <param name="signOptions">Signature options to use for signing a document.</param>
        public DigitalSignatureDetails(CertificateHolder certificateHolder, SignOptions signOptions)
        {
            CertificateHolder = certificateHolder;
            SignOptions = signOptions;
        }

        /// <summary>
        /// Gets <see cref="DigitalSignature"/> object corresponded to this signature details.
        /// Can be <c>null</c>, in case of <see cref="CertificateHolder"/> is <c>null</c>.
        /// </summary>
        internal DigitalSignature GetDigitalSignature()
        {
            if (CertificateHolder == null)
                return null;

            DigitalSignature signature = new DigitalSignature(CertificateHolder);

            if (SignOptions != null)
                SignOptions.ApplyTo(signature);

            return signature;
        }

        /// <summary>
        /// Gets or sets a <see cref="CertificateHolder"/> object that contains the certificate used to sign a document.
        /// </summary>
        public CertificateHolder CertificateHolder { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SignOptions"/> object used to sign a document.
        /// </summary>
        public SignOptions SignOptions { get; set; }
    }
}
