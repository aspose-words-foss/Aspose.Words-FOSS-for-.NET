// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2010 by Konstantin Sidorenko

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Aspose.IO;
using Aspose.JavaAttributes;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
using X509CertificateBc = Org.BouncyCastle.X509.X509Certificate;
using X509CertificateNative = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace Aspose.Crypto
{
    /// <summary>
    /// Implement this class in Java manually.
    ///
    /// This class abstracts the Cryptography library.
    /// </summary>
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    public static class CryptoUtilPal
    {
        /// <summary>
        /// Converts X509 BouncyCastle certificate to dotNet native.
        /// </summary>
        public static X509CertificateNative ToNativeCertificate(X509CertificateBc certificate)
        {
            return new X509CertificateNative(DotNetUtilities.ToX509Certificate(certificate));
        }

        /// <summary>
        /// Creates X509 BouncyCastle certificate from a byte array.
        /// </summary>
        public static X509CertificateBc CreateFromByteArray(byte[] bytes)
        {
            byte[] decoded = IsBytesBase64(bytes) ? Base64.Decode(bytes) : bytes;
            return new X509CertificateParser().ReadCertificate(decoded);
        }

        /// <summary>
        /// Creates certificate holder from a byte array.
        /// </summary>
        public static CertificateHolderInternal CreateHolder(byte[] bytes)
        {
            X509CertificateBc certificate = CreateFromByteArray(bytes);

            CertificateHolderInternal holder = CertificateHolderInternal.Create();
            holder.CertificateBc = certificate;
            return holder;
        }

        /// <summary>
        /// Creates PAL wrapped hash algorithm.
        /// </summary>
        public static Aspose.Crypto.IDigest CreateHashAlgorithm(Aspose.Crypto.DigestAlgorithm digestAlgorithm)
        {
            return new DigestWrapperPal(CreateHashAlgorithmNative(digestAlgorithm));
        }

        /// <summary>
        /// Creates PAL wrapped hash algorithm.
        /// </summary>
        public static Aspose.Crypto.IDigest CreateHashAlgorithm(string algorithmName)
        {
            DigestAlgorithm digestAlgorithm = StringToDigestAlgorithm(algorithmName);
            return CreateHashAlgorithm(digestAlgorithm);
        }

        /// <summary>
        /// Returns digest algorithm by its name.
        /// </summary>
        public static Aspose.Crypto.DigestAlgorithm StringToDigestAlgorithm(string algorithmName)
        {
            switch (algorithmName)
            {
                case "SHA1":
                    return DigestAlgorithm.Sha1;
                case "SHA224":
                    return DigestAlgorithm.Sha224;
                case "SHA256":
                case "http://www.w3.org/2000/09/xmldsig#sha256":
                    return DigestAlgorithm.Sha256;
                case "SHA384":
                    return DigestAlgorithm.Sha384;
                case "SHA512":
                    return DigestAlgorithm.Sha512;
                case "MD2":
                    return DigestAlgorithm.MD2;
                case "MD5":
                    return DigestAlgorithm.MD5;
                default:
                    throw new InvalidOperationException("Unsupported hash algorithm.");
            }
        }

        /// <summary>
        /// Creates HMAC with the specified hash algorithm.
        /// </summary>
        public static Aspose.Crypto.IMac CreateHMac(DigestAlgorithm digestAlgorithm)
        {
            return new MacWrapperPal(new HMac(CreateHashAlgorithmNative(digestAlgorithm)));
        }

        /// <summary>
        /// Creates HMAC with the specified hash algorithm name.
        /// </summary>
        public static Aspose.Crypto.IMac CreateHMac(string algorithm)
        {
            DigestAlgorithm digestAlgorithm = StringToDigestAlgorithm(algorithm);
            return CreateHMac(digestAlgorithm);
        }

        /// <summary>
        /// Creates HMAC with the specified hash algorithm and key.
        /// </summary>
        public static Aspose.Crypto.IMac CreateHMac(DigestAlgorithm digestAlgorithm, byte[] key)
        {
            IMac mac = CreateHMac(digestAlgorithm);
            mac.Init(key);

            return mac;
        }

        /// <summary>
        /// Creates HMAC with the specified hash algorithm and key.
        /// </summary>
        public static Aspose.Crypto.IMac CreateHMac(DigestAlgorithm digestAlgorithm, string key)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);

            return CreateHMac(digestAlgorithm, keyBytes);
        }

        /// <summary>
        /// Creates random generator.
        /// </summary>
        public static Aspose.Crypto.IRandomGenerator CreateRandomGenerator(DigestAlgorithm digestAlgorithm)
        {
            return new RandomGeneratorWrapperPal(new DigestRandomGenerator(CreateHashAlgorithmNative(digestAlgorithm)));
        }

        /// <summary>
        /// Verifies digital signature by certificate.
        /// </summary>
        public static bool VerifySignature(CertificateHolderInternal certificateHolder,
            DigestAlgorithm digestAlgorithm, byte[] signedInfoBytes, byte[] signature)
        {
            AsymmetricKeyParameter publicKey = GetPublicKey(certificateHolder.CertificateBc);

            CryptoAlgorithm algorithm = GetCryptoAlgorithm(publicKey);

            switch (algorithm)
            {
                case CryptoAlgorithm.DSA:
                case CryptoAlgorithm.ECDSA:
                    return VerifyEcDsaSignature(publicKey, digestAlgorithm, signedInfoBytes, signature);
                case CryptoAlgorithm.RSA:
                    return VerifyRsaSignature((RsaKeyParameters)publicKey, digestAlgorithm,
                        signedInfoBytes, signature);
                default:
                    throw new InvalidOperationException("Unexpected CryptoAlgorithm value.");
            }
        }

        /// <summary>
        /// Generates digital signature.
        /// </summary>
        public static byte[] GenerateSignature(CertificateHolderInternal certificateHolder,
            byte[] digest, DigestAlgorithm digestAlgorithm)
        {
            AsymmetricKeyParameter privateKey = certificateHolder.PrivateKeyBc;
            CryptoAlgorithm algorithm = GetCryptoAlgorithm(privateKey);

            switch (algorithm)
            {
                case CryptoAlgorithm.DSA:
                    return GenerateDsaSignature((DsaPrivateKeyParameters)privateKey, digest, digestAlgorithm);
                case CryptoAlgorithm.RSA:
                    return GenerateRsaSignature((RsaPrivateCrtKeyParameters)privateKey, digest, digestAlgorithm);
                case CryptoAlgorithm.ECDSA:
                    return GenerateEcSignature((ECPrivateKeyParameters)privateKey, digest, digestAlgorithm);
                default:
                    throw new InvalidOperationException("Unexpected CryptoAlgorithm value.");
            }
        }

        /// <summary>
        /// Gets public key from X509 BouncyCastle certificate.
        /// </summary>
        public static AsymmetricKeyParameter GetPublicKey(X509CertificateBc cert)
        {
            return cert.GetPublicKey();
        }

        /// <summary>
        /// Creates Rsa with parameters from X509 public key.
        /// </summary>
        public static Rsa CreateRsaFromPublicKey(CertificateHolderInternal certificateHolder)
        {
            RsaKeyParameters pubKey = (RsaKeyParameters)GetPublicKey(certificateHolder.CertificateBc);
            return new Rsa(pubKey.Modulus.ToByteArrayUnsigned(), pubKey.Exponent.ToByteArrayUnsigned());
        }

        /// <summary>
        /// Returns Dsa parameters from X509 public key.
        /// </summary>
        public static DsaParameters GetDsaParametersFromPublicKey(CertificateHolderInternal certificateHolder)
        {
            DsaPublicKeyParameters pubKey = (DsaPublicKeyParameters)GetPublicKey(certificateHolder.CertificateBc);
            DsaParameters parameters = new DsaParameters();
            parameters.P = pubKey.Parameters.P.ToByteArrayUnsigned();
            parameters.Q = pubKey.Parameters.Q.ToByteArrayUnsigned();
            parameters.G = pubKey.Parameters.G.ToByteArrayUnsigned();
            parameters.Y = pubKey.Y.ToByteArrayUnsigned();
            return parameters;
        }

        /// <summary>
        /// Gets a X509 certificate serial number.
        /// </summary>
        public static string GetSerialNumber(CertificateHolderInternal certificateHolder)
        {
            return certificateHolder.CertificateBc.SerialNumber.ToString();
        }

        /// <summary>
        /// Gets a X509 certificate Issuer.
        /// </summary>
        public static string GetIssuer(CertificateHolderInternal certificateHolder)
        {
            // WORDSNET-27976 There can be scenarios when .Net cert is unavailable.
            // Let's then fallback to BC cert. The signature in this case will be
            // correct while there will be  the difference in the order of written
            // OIDs and sometimes in friendly names.
            return certificateHolder.CertificateBc.IssuerDN.ToString();
        }

        /// <summary>
        /// Gets a X509 certificate encoded data.
        /// </summary>
        public static byte[] GetEncodedData(CertificateHolderInternal certificateHolder)
        {
            return certificateHolder.CertificateBc.GetEncoded();
        }

        /// <summary>
        /// Converts specified secured string to unsecured one.
        /// http://blogs.msdn.com/b/fpintos/archive/2009/06/12/how-to-properly-convert-securestring-to-string.aspx
        /// </summary>
        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Creates certificate holder for PKCS12.
        /// </summary>
        public static CertificateHolderInternal CreateHolderForPkcs12(Stream storeStream, string password)
        {
            return CreateHolderForPkcs12(storeStream, password, null);
        }

        /// <summary>
        /// Creates certificate holder for PKCS12.
        /// </summary>
        public static CertificateHolderInternal CreateHolderForPkcs12(Stream storeStream, string password, string alias)
        {
            if (storeStream == null)
                throw new InvalidParameterException("StoreStream cannot be null");

            if (password == null)
                throw new InvalidParameterException("KeyStore password must exist");

            // load store
            Pkcs12Store pkcs12Store = new Pkcs12Store();
            pkcs12Store.Load(storeStream, password.ToCharArray());

            // if alias is null then find the first alias which can be used to retrieve private key.
            // otherwise use alias which was provided by user.
            string certAlias = alias;
            if (certAlias == null)
            {
                foreach (string al in pkcs12Store.Aliases)
                {
                    if (pkcs12Store.IsKeyEntry(al) && pkcs12Store.GetKey(al).Key.IsPrivate)
                    {
                        certAlias = al;
                        break;
                    }
                }
            }

            if (certAlias == null)
                throw new SecurityException("At least one certificate and its private key must be present.");

            AsymmetricKeyEntry privateKey = pkcs12Store.GetKey(certAlias);
            if (privateKey == null)
                throw new SecurityException("Private key was not found by alias '" + certAlias + "'");

            return CreateHolder(pkcs12Store, certAlias, privateKey, storeStream, password);
        }

        /// <summary>
        /// Returns BounsyCastle hash algorithm.
        /// </summary>
        private static Org.BouncyCastle.Crypto.IDigest CreateHashAlgorithmNative(DigestAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case DigestAlgorithm.Sha1:
                    return new Sha1Digest();
                case DigestAlgorithm.Sha224:
                    return new Sha224Digest();
                case DigestAlgorithm.Sha256:
                    return new Sha256Digest();
                case DigestAlgorithm.Sha384:
                    return new Sha384Digest();
                case DigestAlgorithm.Sha512:
                    return new Sha512Digest();
                case DigestAlgorithm.MD2:
                    return new MD2Digest();
                case DigestAlgorithm.MD5:
                    return new MD5Digest();
                default:
                    throw new InvalidOperationException("Unsupported hash algorithm.");
            }
        }

        private static CertificateHolderInternal CreateHolder(Pkcs12Store pkcs12Store, string certAlias,
            AsymmetricKeyEntry privateKey, Stream storeStream, string password)
        {
            CertificateHolderInternal holder = new CertificateHolderInternal();
            holder.CertificateBc = pkcs12Store.GetCertificate(certAlias).Certificate;
            holder.CertificateNative = new X509CertificateNative(StreamUtil.CopyStreamToByteArray(storeStream), password);
            holder.PrivateKeyBc = privateKey.Key;
            return holder;
        }

        /// <summary>
        /// Returns true, if a specified byte array contains only Base64 allowed characters.
        /// </summary>
        private static bool IsBytesBase64(byte[] ascii)
        {
            if (ascii == null)
                return true;

            for (int i = 0; i < ascii.Length; i++)
            {
                if (ascii[i] == Cr || ascii[i] == Nl)
                    continue;

                bool isBase64Char = ascii[i] == Pad || ArrayUtil.Contains(Base64Alphabet, ascii[i]);
                if (!isBase64Char)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns CryptoAlgorithm enumeration by a specified key.
        /// </summary>
        public static CryptoAlgorithm GetCryptoAlgorithm(AsymmetricKeyParameter key)
        {
            if (key is ECKeyParameters)
                return CryptoAlgorithm.ECDSA;

            return (key is DsaKeyParameters)
                ? CryptoAlgorithm.DSA
                : CryptoAlgorithm.RSA;
        }

        /// <summary>
        /// Returns CryptoAlgorithm enumeration by the public key from the specified certificate.
        /// </summary>
        public static CryptoAlgorithm GetCryptoAlgorithmByPublicKey(CertificateHolderInternal certificateHolder)
        {
            return GetCryptoAlgorithm(GetPublicKey(certificateHolder.CertificateBc));
        }

        /// <summary>
        /// Returns CryptoAlgorithm enumeration by the private key from the specified certificate.
        /// </summary>
        public static CryptoAlgorithm GetCryptoAlgorithmByPrivateKey(CertificateHolderInternal certificateHolder)
        {
            return GetCryptoAlgorithm(certificateHolder.PrivateKeyBc);
        }

        /// <summary>
        /// Returns DigestAlgorithm enumeration by a specified key.
        /// </summary>
        public static DigestAlgorithm GetDigestAlgorithm(AsymmetricKeyParameter key)
        {
            if (key is RsaKeyParameters)
            {
                switch (((RsaKeyParameters)key).Modulus.BitLength)
                {
                    case 1792:
                        return DigestAlgorithm.Sha224;

                    case 2048:
                        return DigestAlgorithm.Sha256;

                    case 3072:
                        return DigestAlgorithm.Sha384;

                    case 4096:
                        return DigestAlgorithm.Sha512;

                    default:
                        return DigestAlgorithm.Sha1;
                }
            }

            return (key is ECKeyParameters) ? DigestAlgorithm.Sha256 : DigestAlgorithm.Sha1;
        }

        /// <summary>
        /// Returns DigestAlgorithm enumeration by the private key from the specified certificate.
        /// </summary>
        public static DigestAlgorithm GetDigestAlgorithmByPrivateKey(CertificateHolderInternal certificateHolder)
        {
            return GetDigestAlgorithm(certificateHolder.PrivateKeyBc);
        }

        /// <summary>
        /// Verifies RSA signature.
        /// </summary>
        private static bool VerifyRsaSignature(RsaKeyParameters publicKey, DigestAlgorithm digestAlgorithm,
            byte[] signedInfoBytes, byte[] signature)
        {
            RsaDigestSigner digestSigner = new RsaDigestSigner(CreateHashAlgorithmNative(digestAlgorithm));
            digestSigner.Init(false, new RsaKeyParameters(false, publicKey.Modulus, publicKey.Exponent));
            digestSigner.BlockUpdate(signedInfoBytes, 0, signedInfoBytes.Length);
            return digestSigner.VerifySignature(signature);
        }

        /// <summary>
        /// Verifies ECDSA or DSA signature.
        /// </summary>
        private static bool VerifyEcDsaSignature(ICipherParameters publicKey, DigestAlgorithm digestAlgorithm,
            byte[] signedInfoBytes, byte[] signature)
        {
            IDsa signer = (publicKey is DsaPublicKeyParameters) ? (IDsa)new DsaSigner() : new ECDsaSigner();
            signer.Init(false, publicKey);

            byte[] hash = HashUtil.ComputeHash(CreateHashAlgorithm(digestAlgorithm), signedInfoBytes);

            int len = signature.Length / 2;
            byte[] rBytes = new byte[len];
            byte[] sBytes = new byte[len];

            Array.Copy(signature, 0, rBytes, 0, len);
            Array.Copy(signature, len, sBytes, 0, len);

            Org.BouncyCastle.Math.BigInteger r = new Org.BouncyCastle.Math.BigInteger(1, rBytes);
            Org.BouncyCastle.Math.BigInteger s = new Org.BouncyCastle.Math.BigInteger(1, sBytes);

            return signer.VerifySignature(hash, r, s);
        }

        /// <summary>
        /// Generates RSA signature.
        /// </summary>
        private static byte[] GenerateRsaSignature(RsaPrivateCrtKeyParameters privateKey, byte[] digest,
            DigestAlgorithm digestAlgorithm)
        {
            RsaDigestSigner digestSigner = new RsaDigestSigner(CreateHashAlgorithmNative(digestAlgorithm));
            digestSigner.Init(true, new RsaKeyParameters(true, privateKey.Modulus, privateKey.Exponent));
            digestSigner.BlockUpdate(digest, 0, digest.Length);
            return digestSigner.GenerateSignature();
        }

        /// <summary>
        /// Generates DSA signature.
        /// </summary>
        private static byte[] GenerateDsaSignature(DsaPrivateKeyParameters privateKey, byte[] digest,
            DigestAlgorithm digestAlgorithm)
        {
            DsaSigner dsa = new DsaSigner();
            DsaDigestSigner digestSigner = new DsaDigestSigner(dsa, CreateHashAlgorithmNative(digestAlgorithm));
            digestSigner.Init(true, privateKey);
            digestSigner.BlockUpdate(digest, 0, digest.Length);
            byte[] signature = digestSigner.GenerateSignature();

            return ConvertDerToIeee1363(signature);
        }

        /// <summary>
        /// Generates ECDSA signature.
        /// </summary>
        private static byte[] GenerateEcSignature(ECPrivateKeyParameters privateKey, byte[] digest,
            DigestAlgorithm digestAlgorithm)
        {
            ECDsaSigner signer = new ECDsaSigner();
            signer.Init(true, privateKey);

            byte[] hash = HashUtil.ComputeHash(CreateHashAlgorithm(digestAlgorithm), digest);
            Org.BouncyCastle.Math.BigInteger[] signatureSequence = signer.GenerateSignature(hash);

            byte[] r = signatureSequence[0].ToByteArray();
            byte[] s = signatureSequence[1].ToByteArray();

            // In ECDSA the R and S are integers presenting cartesian coordinates, and their value is not always filling
            // all bytes of the field. Not filled bytes in R and S for ASN.1 should be filled with zeroes or 0xFF.
            int length = privateKey.Parameters.Curve.FieldSize / 8 + 1;
            byte[] signature = new byte[length * 2];

            // Fill with R bytes.
            int offset = length - r.Length;
            r.CopyTo(signature, offset);

            // Fill with S bytes.
            offset = length + (length - s.Length);
            s.CopyTo(signature, offset);

            return signature;
        }

        /// <summary>
        /// Converts ANS.1 encoded DER sequence to IEEE1363 sequence used in XmlDsig.
        /// </summary>
        private static byte[] ConvertDerToIeee1363(byte[] signature)
        {
            Asn1InputStream input = new Asn1InputStream(new MemoryStream(signature));
            DerSequence derSequence = (DerSequence)input.ReadObject();

            byte[] rBytes = derSequence[0].GetEncoded();
            byte[] sBytes = derSequence[1].GetEncoded();

            //IEEE1363 sequence is 40 bytes length and consists of two numbers sticked together.
            const int len = 20;
            byte[] p1363 = new byte[2 * len];
            Array.Copy(rBytes, rBytes.Length - len, p1363, 0, len);
            Array.Copy(sBytes, sBytes.Length - len, p1363, len, len);

            return p1363;
        }

        private const byte Cr = (byte)'\r';
        private const byte Nl = (byte)'\n';
        private const byte Pad = (byte)'=';
        private static readonly byte[] Base64Alphabet = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx yz0123456789+/");
    }
}
