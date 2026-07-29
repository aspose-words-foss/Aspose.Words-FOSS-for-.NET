// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/11/2014 by Konstantin Sidorenko

using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Aspose.Crypto;
using Aspose.JavaAttributes;
using Aspose.Words.Saving;
using Org.BouncyCastle.Security;

namespace Aspose.Words.DigitalSignatures
{

    /// <summary>
    /// Represents a holder of <b>X509Certificate2</b> instance.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-digital-signatures/">Work with Digital Signatures</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="CertificateHolder"/> can be created by static factory methods only.
    /// It contains an instance of <b>X509Certificate2</b> which is used to introduce private, public keys and certificate chains into the system.
    /// <ms><see cref="X509Certificate2"/></ms><java><b>X509Certificate2</b></java><cpp><see cref="X509Certificate2"/></cpp> as parameters.</p>
    /// </remarks>
    public class CertificateHolder
    {

        /// <summary>
        /// Returns the instance of <java><b>X509Certificate2Wrapper</b> that holds </java><b>X509Certificate2</b> which holds private, public keys and certificate chain.
        /// </summary>
        /// <returns><see cref="X509Certificate2"/>
        /// <cpp><see cref="X509Certificate2"/></cpp> instance</returns>
        public X509Certificate2 Certificate
        {
            get { return mCertificateHolder != null ? mCertificateHolder.CertificateNative : null; }
        }

        /// <summary>
        /// Creates <see cref="CertificateHolder"/> object using byte array of PKCS12 store and its password.
        /// </summary>
        /// <param name="certBytes">A byte array that contains data from an X.509 certificate.</param>
        /// <param name="password">The password required to access the X.509 certificate data.</param>
        /// <returns>An instance of <see cref="CertificateHolder"/></returns>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="certBytes"/> is <c>null</c></exception>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="password"/> is <c>null</c></exception>
        /// <exception cref="SecurityException">Thrown if PKCS12 store contains no aliases</exception>
        /// <exception cref="IOException">Thrown if there is wrong password or corrupted file.</exception>
        /// <msonly>Remove this from Java public API.</msonly>
        [JavaDelete("Java hasn't SecureString analog: 1) it should be low-level-platform-dependent, but 2) can't be absolutely safe.")]
        public static CertificateHolder Create(byte[] certBytes, SecureString password)
        {
            return new CertificateHolder(CertificateHolderInternal.Create(certBytes, password));
        }

        /// <summary>
        /// Creates <see cref="CertificateHolder"/> object using byte array of PKCS12 store and its password.
        /// </summary>
        /// <param name="certBytes">A byte array that contains data from an X.509 certificate.</param>
        /// <param name="password">The password required to access the X.509 certificate data.</param>
        /// <returns>An instance of <see cref="CertificateHolder"/></returns>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="certBytes"/> is <c>null</c></exception>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="password"/> is <c>null</c></exception>
        /// <exception cref="SecurityException">Thrown if PKCS12 store contains no aliases</exception>
        /// <exception cref="IOException">Thrown if there is wrong password or corrupted file.</exception>
        public static CertificateHolder Create(byte[] certBytes, string password)
        {
            return new CertificateHolder(CertificateHolderInternal.Create(certBytes, password));
        }

        /// <summary>
        /// Creates <see cref="CertificateHolder"/> object using path to PKCS12 store and its password.
        /// </summary>
        /// <param name="fileName">The name of a certificate file.</param>
        /// <param name="password">The password required to access the X.509 certificate data.</param>
        /// <returns>An instance of <see cref="CertificateHolder"/></returns>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="password"/> is <c>null</c></exception>
        /// <exception cref="SecurityException">Thrown if PKCS12 store contains no aliases</exception>
        /// <exception cref="IOException">Thrown if there is wrong password or corrupted file.</exception>
        public static CertificateHolder Create(string fileName, string password)
        {
            return new CertificateHolder(CertificateHolderInternal.Create(fileName, password));
        }

        /// <summary>
        /// Creates <see cref="CertificateHolder"/> object using path to PKCS12 store, its password and the alias by using which private key and certificate will be found.
        /// </summary>
        /// <param name="fileName">The name of a certificate file.</param>
        /// <param name="password">The password required to access the X.509 certificate data.</param>
        /// <param name="alias">The associated alias for a certificate and its private key</param>
        /// <returns>An instance of <see cref="CertificateHolder"/></returns>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="fileName"/> is <c>null</c></exception>
        /// <exception cref="InvalidParameterException">Thrown if <paramref name="password"/> is <c>null</c></exception>
        /// <exception cref="SecurityException">Thrown if PKCS12 store contains no aliases</exception>
        /// <exception cref="IOException">Thrown if there is wrong password or corrupted file.</exception>
        /// <exception cref="SecurityException">Thrown if there is no private key with the given alias</exception>
        public static CertificateHolder Create(string fileName, string password, string alias)
        {
            return new CertificateHolder(CertificateHolderInternal.Create(fileName, password, alias));
        }

        /// <summary>
        /// Converts CertificateHolder to <see cref="CertificateHolderInternal"/> for internal use.
        /// </summary>
        internal CertificateHolderInternal ToInternal()
        {
            return mCertificateHolder;
        }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal CertificateHolder(CertificateHolderInternal holderInternal)
        {
            mCertificateHolder = holderInternal;
        }

        private readonly CertificateHolderInternal mCertificateHolder;
    }
}
