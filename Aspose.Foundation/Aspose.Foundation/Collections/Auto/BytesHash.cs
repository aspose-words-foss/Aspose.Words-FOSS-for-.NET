// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2018 by Vyacheslav Durin

using System;
using Aspose.Crypto;

namespace Aspose.Collections
{
    /// <summary>
    /// This class generates and holds SHA512 hash of the given array. It overrides equals and hashCode so that its instances could be used in Dictionaries.
    /// </summary>
    public class BytesHash
    {
        /// <summary>
        /// Returns Guid's 16 bytes from the generated SHA512 hash.
        /// </summary>
        public byte[] ToGuidByteArray()
        {
            return mGuid.ToByteArray();
        }

        /// <returns>SHA512 Hash's hashCode</returns>
        public override int GetHashCode()
        {
            return mHashCode;
        }

        /// <summary>
        /// For performance sake this method checks hashCode from SHA512 hash.
        /// There could be very small collision probability from SHA512 hash function and hashCode() method.
        /// So we neglect this probability.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is BytesHash))
                return false;

            return mHashCode == obj.GetHashCode();
        }

        /// <summary>
        /// Initializes a new instance of BytesHash which generates and holds SHA512 hash of the given byte array.
        /// </summary>
        public BytesHash(byte[] bytes)
            : this(bytes, 0, (bytes != null) ? bytes.Length : 0)
        {
            // Everything is done by another constructor.
        }

        /// <summary>
        /// Initializes a new instance of BytesHash which generates and holds SHA512 hash of a portion the given byte array.
        /// </summary>
        public BytesHash(byte[] bytes, int offset, int length)
        {
            // SHA512 = 64 length mHash
            IDigest hashAlg = CryptoUtilPal.CreateHashAlgorithm(DigestAlgorithm.Sha512);
            mHash = (bytes != null)
                ? HashUtil.ComputeHash(hashAlg, bytes, offset, length)
                : Empty.mHash;
            mHashCode = CalculateHashCode(mHash);
            mGuid = CreateGuid(mHash);
        }

        public static bool operator ==(BytesHash left, BytesHash right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BytesHash left, BytesHash right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Creates Guid from each 4th byte of SHA512 hash.
        /// </summary>
        private static Guid CreateGuid(byte[] hash)
        {
            byte[] b = new byte[16];
            for (int i = 0, j = 0; i < hash.Length; i += 4, j++)
                b[j] = hash[i];

            return new Guid(b);
        }

        /// <summary>
        /// This method performs equality check.
        /// This method could be used to verify password hashes to make sure there is no collisions from hashCode() method.
        /// </summary>
        /// <returns> true if SHA512 hashes are equal.</returns>
        public static bool Equals(BytesHash k1, BytesHash k2)
        {
            // 1. check hashCode() value first
            bool eq = k1.Equals(k2);
            if (!eq)
                return false;

            // 2. then check the Hashes equality
            if (k1.mHash.Length != k2.mHash.Length)
                return false;

            for (int i = 0; i < k1.mHash.Length; i++)
            {
                if (k1.mHash[i] != k2.mHash[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates hash code from the given array. 
        /// This method is taken from JDK10
        /// </summary>
        private static int CalculateHashCode(byte[] a)
        {
            if (a == null)
                return 0;

            int result = 1;
            foreach (byte element in a)
                result = 31 * result + element;

            return result;
        }

        public static readonly BytesHash Empty = new BytesHash(new byte[0]);
        private readonly byte[] mHash;
        private readonly int mHashCode;
        private readonly Guid mGuid;
    }
}
