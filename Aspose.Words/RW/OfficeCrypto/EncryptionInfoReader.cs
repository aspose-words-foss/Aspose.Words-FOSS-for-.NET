// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2011 by Alexey Morozov

using System;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Abstract class used to read and hold encryption parameters.
    /// </summary>
    /// <remarks>AM. I made separate class because this parameter block is read twice: first for EncryptionInfo and second for PasswordKeyEncryptors.</remarks>
    internal abstract class EncryptionInfoReader
    {
        /// <summary>
        /// Parses tags which relate to encryption parameters. 
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <returns>Returns true if tag is processed and consumed otherwise returns false.</returns>
        protected bool ProcessTag(NrxXmlReader xmlReader)
        {
            switch (xmlReader.LocalName)
            {
                case "saltSize":
                    mSaltSize = xmlReader.ValueAsInt;
                    break;
                case "blockSize":
                    mBlockSize = xmlReader.ValueAsInt;
                    break;
                case "keyBits":
                    mKeyBits = xmlReader.ValueAsInt;
                    break;
                case "hashSize":
                    mHashSize = xmlReader.ValueAsInt;
                    break;
                case "cipherAlgorithm":
                    mCipherAlgorithm = xmlReader.Value;
                    break;
                case "cipherChaining":
                    mCipherChaining = xmlReader.Value;
                    break;
                case "hashAlgorithm":
                    mHashAlgorithm = xmlReader.Value;
                    break;
                case "saltValue":
                    mSaltValue = Convert.FromBase64String(xmlReader.Value);
                    break;
                case "spinCount":
                    mSpinCount = xmlReader.ValueAsInt;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Specifies the size of the salt.
        /// </summary>
        internal int SaltSize
        {
            get { return mSaltSize; }
        }

        /// <summary>
        /// Specifies the block size.
        /// </summary>
        internal int BlockSize
        {
            get { return mBlockSize; }
        }

        /// <summary>
        /// Specifies the number of bits.
        /// </summary>
        internal int KeyBits
        {
            get { return mKeyBits; }
        }

        /// <summary>
        /// Specifies the number of bytes utilized by a hash value.
        /// </summary>
        internal int HashSize
        {
            get { return mHashSize; }
        }

        /// <summary>
        /// Specifies the number of times to iterate on a hash of a password.
        /// </summary>
        internal int SpinCount
        {
            get { return mSpinCount; }
        }

        /// <summary>
        /// Specifies the cipher algorithm.
        /// </summary>
        internal string CipherAlgorithm
        {
            get { return mCipherAlgorithm; }
        }

        /// <summary>
        /// Specifies the chaining mode used by the CipherAlgorithm.
        /// </summary>
        internal string CipherChaining
        {
            get { return mCipherChaining; }
        }

        /// <summary>
        /// Specifies a hashing algorithm.
        /// </summary>
        internal string HashAlgorithm
        {
            get { return mHashAlgorithm; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal byte[] SaltValue
        {
            get { return mSaltValue; }
        }

        private int mSaltSize;
        private int mBlockSize;
        private int mKeyBits;
        private int mHashSize;
        private int mSpinCount;
        private string mCipherAlgorithm;
        private string mCipherChaining;
        private string mHashAlgorithm;
        private byte[] mSaltValue;
    }
}
