// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2014 by Vyacheslav Durin

using System.Collections;
using System.IO;
using System.Security;
using Aspose.JavaAttributes;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using X509CertificateBc = Org.BouncyCastle.X509.X509Certificate;
using X509CertificateNative = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace Aspose.Crypto
{
    public class CertificateHolderInternal
    {
        public X509CertificateBc CertificateBc
        {
            set
            {
                mCertificateBc = value;
                CertificateNative = CryptoUtilPal.ToNativeCertificate(mCertificateBc);
            }
            get { return mCertificateBc; }
        }

        public AsymmetricKeyParameter PrivateKeyBc
        {
            set { mPrivateKeyBc = value; }
            get { return mPrivateKeyBc; }
        }

        public X509CertificateNative CertificateNative
        {
            internal set { mCertificateNative = value; }
            get { return mCertificateNative; }
        }

        public string GetSubjectX509Principal(DerObjectIdentifier name)
        {
#if JAVA
            org.bouncycastle.asn1.x500.RDN[] listValues = mCertificateBc.getSubject().getRDNs(name);
            return listValues != null && listValues.length > 0 && listValues[0] != null ? listValues[0].getFirst().getValue().toString() : null;
#else
            IList listValues = mCertificateBc.SubjectDN.GetValueList(name);
            return listValues != null && listValues.Count > 0 ? listValues[0].ToString() : null;
#endif
        }

        public string GetSubjectX509Principal(string name)
        {
            return GetSubjectX509Principal(new DerObjectIdentifier(name));
        }

        #region Static initializers

        [JavaDelete("Java hasn't SecureString analog: 1) it should be low-level-platform-dependent, but 2) can't be absolutely safe.")]
        public static CertificateHolderInternal Create(byte[] storeBytes, SecureString password)
        {
            using (Stream storeStream = new MemoryStream(storeBytes))
                return CryptoUtilPal.CreateHolderForPkcs12(storeStream, CryptoUtilPal.ConvertToUnsecureString(password));
        }

        public static CertificateHolderInternal Create(byte[] storeBytes, string password)
        {
            using (Stream storeStream = new MemoryStream(storeBytes))
                return CryptoUtilPal.CreateHolderForPkcs12(storeStream, password);
        }

        public static CertificateHolderInternal Create(string storefileName, string password)
        {
             // WORDSNET-25903 We should access certificate file in read-only mode.
            using (FileStream storeStream = File.Open(storefileName, FileMode.Open, FileAccess.Read))
                return CryptoUtilPal.CreateHolderForPkcs12(storeStream, password);
        }

        public static CertificateHolderInternal Create(string storefileName, string password, string alias)
        {
            // WORDSNET-25903 We should access certificate file in read-only mode.
            using (FileStream storeStream = File.Open(storefileName, FileMode.Open, FileAccess.Read))
                return CryptoUtilPal.CreateHolderForPkcs12(storeStream, password, alias);
        }

        public static CertificateHolderInternal Create()
        {
           return new CertificateHolderInternal();
        }

        #endregion

        internal CertificateHolderInternal()
        {
            //
        }

        private X509CertificateNative mCertificateNative;
        private X509CertificateBc mCertificateBc;
        private AsymmetricKeyParameter mPrivateKeyBc;
    }
}
