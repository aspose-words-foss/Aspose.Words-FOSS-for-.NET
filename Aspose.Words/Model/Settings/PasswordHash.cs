// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2008 by Roman Korchagin

using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Crypto;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Contains data and algorithms for calculating an OOXML password hash.
    /// </summary>
    internal class PasswordHash
    {
        static PasswordHash()
        {
            gAlgorithmSidNameMap.AddEntry("MD2", 1);
            gAlgorithmSidNameMap.AddEntry("MD4", 2);
            gAlgorithmSidNameMap.AddEntry("MD5", 3);
            gAlgorithmSidNameMap.AddEntry("RIPEMD-128", 6);
            gAlgorithmSidNameMap.AddEntry("RIPEMD-160", 7);
            gAlgorithmSidNameMap.AddEntry("SHA-1", 4);
            gAlgorithmSidNameMap.AddEntry("SHA-256", 12);
            gAlgorithmSidNameMap.AddEntry("SHA-384", 13);
            gAlgorithmSidNameMap.AddEntry("SHA-512", 14);
            gAlgorithmSidNameMap.AddEntry("WHIRLPOOL", UnusedAlgorithmSid); // does not have corresponding ECMA-376 int, so map to unused now.
        }

        /// <summary>
        /// Calculates the OOXML hash code for a document protection password.
        /// </summary>
        internal void CalculateDocumentProtectionHash(int legacyHash, PasswordHash passwordHash)
        {
            // andrnosk: WORDSNET-9724 We have to use algorithm, spin count and salt values specified inside document 
            // to calculate correct hash.
            CryptAlgorithmSid = passwordHash.CryptAlgorithmSid > 0 ? passwordHash.CryptAlgorithmSid : DefaultAlgorithmSid;
            // WORDSNET-22989 AW should to use value from the document file even it is zero.
            CryptSpinCount = passwordHash.IsEmpty ? DefaultSpinCount : passwordHash.CryptSpinCount;
            Salt = (passwordHash.Salt != null) ? passwordHash.Salt : DefaultSalt;

            // We are going to have 16 bytes salt and 16 bytes of the legacy hash string.
            byte[] initialBytes = new byte[32];
            Salt.CopyTo(initialBytes, 0);

            // Write 16 bytes legacy hash as a Unicode hex string representation of the 4-byte legacy hash.
            string legacyHashString = FormatterPal.IntToStrX8(BitUtil.SwapInt32(legacyHash));
            byte[] legacyHashBytes = Encoding.Unicode.GetBytes(legacyHashString);
            legacyHashBytes.CopyTo(initialBytes, 16);

            // WORDSNET-10286 Cryptographic hashing algorithm SHA-512 (w:cryptAlgorithmSid="14") was defined for document
            // and hence should be used to compute a hash value.
            Hash = CalculateDocxHash(CryptAlgorithmSid, initialBytes, CryptSpinCount);
        }

        /// <summary>
        /// Calculates the OOXML hash code for a write protection password.
        /// </summary>
        internal void CalculateWriteProtectionHash(string password, PasswordHash passwordHash)
        {
            CryptAlgorithmSid = passwordHash.CryptAlgorithmSid > 0 ? passwordHash.CryptAlgorithmSid : DefaultAlgorithmSid;
            CryptSpinCount = passwordHash.CryptSpinCount > 0 ? passwordHash.CryptSpinCount : DefaultSpinCount;
            Salt = (passwordHash.Salt != null) ? passwordHash.Salt : DefaultSalt;

            // Build 16 bytes salt prepended to the password.
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] initialBytes = new byte[16 + passwordBytes.Length];
            Salt.CopyTo(initialBytes, 0);
            passwordBytes.CopyTo(initialBytes, 16);

            Hash = CalculateDocxHash(CryptAlgorithmSid, initialBytes, CryptSpinCount);
        }

        /// <summary>
        /// Deep clone.
        /// </summary>
        internal PasswordHash Clone()
        {
            // RK Memberwise is enough at the moment.
            return (PasswordHash)MemberwiseClone();
        }

        /// <summary>
        /// Maps hash algorithm names to int values as expected by old OOXML1.
        /// ISO 29500 quietly introduces new field algorithmName that seems to be equivalent to 
        /// cryptAlgorithmSid as defined in ECMA 376.
        /// </summary>
        internal static int CryptAlgorithmSidFromAlgorithmName(string name)
        {
            return gAlgorithmSidNameMap.GetValue(name, UnusedAlgorithmSid);
        }

        internal static string CryptAlgorithmNameFromSid(int sid)
        {
            return gAlgorithmSidNameMap.GetValue(sid);
        }

        /// <summary>
        /// Returns hash, calculated using the given algorithm.
        /// </summary>
        /// <param name="cryptAlgorithmSid">Hash algorithm ID.</param>
        /// <param name="initialBytes">Initial message for applying hash algorithm.</param>
        /// <param name="spinCount">How many times hashing algorithm will be applied.</param>
        private static byte[] CalculateDocxHash(int cryptAlgorithmSid, byte[] initialBytes, int spinCount)
        {
            DigestAlgorithm alg = (cryptAlgorithmSid == CryptAlgorithmSidFromAlgorithmName("SHA-512")) ? DigestAlgorithm.Sha512 : DigestAlgorithm.Sha1;
            Aspose.Crypto.IDigest digest = CryptoUtilPal.CreateHashAlgorithm(alg);

            int digestSize = digest.GetDigestSize();

            byte[] hash = HashUtil.ComputeHash(digest, initialBytes);

            // Hash length, followed by 4 bytes containing the number of the iteration
            byte[] iterationBytes = new byte[digestSize + BlockSize];
            for (int iterationNumber = 0; iterationNumber < spinCount; iterationNumber++)
            {
                hash.CopyTo(iterationBytes, 0);
                ArrayUtil.WriteUInt32ToByteArray(iterationNumber, iterationBytes, digestSize);
                hash = HashUtil.ComputeHash(digest, iterationBytes);
            }

            return hash;
        }


        /// <summary>
        /// Returns true if there is no password.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (Hash == null); }
        }

        /// <summary>
        /// The value of OOXML "secure" hash.
        /// Null if there is no hash.
        /// </summary>
        internal byte[] Hash;

        /// <summary>
        /// Salt added before calculating the OOXML hash.
        /// </summary>
        internal byte[] Salt;

        /// <summary>
        /// The type of the hash algorithm used for the OOXML hash.
        /// </summary>
        internal int CryptAlgorithmSid;

        /// <summary>
        /// The number of times OOXML hash is calculated.
        /// </summary>
        internal int CryptSpinCount;

        /// <summary>
        /// We use fixed salt when hashing so unit tests and gold files can pass.
        /// </summary>
        internal static readonly byte[] DefaultSalt = new byte[] { 132, 138, 142, 151, 223, 100, 34, 191, 254, 55, 72, 114, 162, 241, 42, 237 };
        /// <summary>
        /// This is what MS Word does.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultSpinCount = 50000;

        /// <summary>
        ///  SHA-1. This is what MS Word does.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultAlgorithmSid = 4;

        /// <summary>
        /// Per OOXML spec any value above 14 is undefined value, so this one is.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int UnusedAlgorithmSid = 15;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int BlockSize = 4;

        private static readonly StringToIntBidirectionalMap gAlgorithmSidNameMap = new StringToIntBidirectionalMap();
    }
}
