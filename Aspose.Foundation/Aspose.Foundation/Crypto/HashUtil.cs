// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.IO;
using Aspose.Collections;

namespace Aspose.Crypto
{
    public static class HashUtil
    {
        /// <summary>
        /// Calculates hash using the specified digest algorithm.
        /// </summary>
        public static byte[] ComputeHash(DigestAlgorithm digestAlgorithm, byte[] input, int offset, int length)
        {
            return ComputeHash(CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm), input, offset, length);
        }

        /// <summary>
        /// Calculates hash using the specified digest algorithm.
        /// </summary>
        public static byte[] ComputeHash(DigestAlgorithm digestAlgorithm, byte[] input)
        {
            return ComputeHash(digestAlgorithm, input, 0, input.Length);
        }

        /// <summary>
        /// Calculates hash using given IDigest object.
        /// Resets the digest state before calculating.
        /// </summary>
        public static byte[] ComputeHash(IDigest digest, byte[] input, int offset, int length)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            digest.Reset();
            digest.BlockUpdate(input, offset, length);

            byte[] digestBytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(digestBytes, 0);
            return digestBytes;
        }

        /// <summary>
        /// Calculates hash using given IDigest object.
        /// Resets the digest state before calculating.
        /// </summary>
        public static byte[] ComputeHash(IDigest digest, byte[] input)
        {
            return ComputeHash(digest, input, 0, input.Length);
        }

        /// <summary>
        /// Calculates hash of stream using the specified digest algorithm.
        /// </summary>
        public static byte[] ComputeHash(DigestAlgorithm digestAlgorithm, Stream inputStream)
        {
            IDigest digest = CryptoUtilPal.CreateHashAlgorithm(digestAlgorithm);
            digest.Reset();

            byte[] buffer = new byte[4096];
            int bytesRead;
            do
            {
                bytesRead = inputStream.Read(buffer, 0, 4096);
                if (bytesRead > 0)
                    digest.BlockUpdate(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);

            byte[] digestBytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(digestBytes, 0);
            return digestBytes;
        }

        /// <summary>
        /// Calculates SHA512 hash of a byte array and returns it as a BytesHash object.
        /// </summary>
        public static BytesHash GetSHA512Hash(byte[] bytes)
        {
            return new BytesHash(bytes);
        }

        /// <summary>
        /// Implements .Net String.GetHashCode method. MS implementation of the method was unsafe, 
        /// here is "ugly port" to safe code. We want the same string hashcode value in .NET, Mono 
        /// and Java to generate GUID font resource names for XPS so the golds will pass.
        /// </summary>
        public static int NetGetHashCode(string str)
        {
            int num = 352654597;
            int num2 = num;

            int numPtr = 0;
            for (int i = str.Length; i > 0; i -= 4)
            {
                int int1 = str[numPtr];
                int int2 = (i > 1) ? str[numPtr + 1] : 0;
                int int3 = (i > 2) ? str[numPtr + 2] : 0;
                int int4 = (i > 3) ? str[numPtr + 3] : 0;

                int intch1 = int2;
                intch1 <<= 16;
                intch1 += int1;

                int intch2 = int4;
                intch2 <<= 16;
                intch2 += int3;

                num = (((num << 5) + num) + (num >> 27)) ^ intch1;
                if (i <= 2)
                {
                    break;
                }
                num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ intch2;
                numPtr += 4;
            }
            return (num + (num2 * 1566083941));
        }
    }
}
