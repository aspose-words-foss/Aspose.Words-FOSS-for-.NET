// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2011 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Crypto;
using Aspose.IO;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Decrypt document encrypted using agile encryption.
    /// Has few limitation - only one PasswordKeyEncryptor, only AES is supported as cipher algorithm and only SHA1 supported as hashing algorithm. 
    /// Seems that is enough to decrypt documents encrypted by Word2010.
    /// 
    /// Original source code can be found at http://s3downloads.lyquidity.com/OoXmlCrypto/OoXmlAgileCrypto3.cs - many thanks to author.
    /// </summary>
    internal class Ecma376AgileEncryption : IEcma376Encryption
    {
        public void Decrypt(Stream encryptionInfoStream, Stream encryptedStream, Stream stream, string password)
        {
            // Skip EncryptionVersionInfo and copy of the Flags stored in the EncryptionHeader field.
            encryptionInfoStream.Position = 8;
            BinaryReader reader = new BinaryReader(encryptionInfoStream);

            mEncryptionInfo = new EncryptionInfo(reader);

            if (PkeData.CipherAlgorithm != "AES")
                throw new InvalidOperationException("Unsupported cipher algorithm.");

            InitAlgorithms(password, PkeData.HashAlgorithm, KeyData.HashAlgorithm);

            if (!VerifyPassword())
                throw new IncorrectPasswordException("The document password is incorrect.");

            if (!VerifyIntegrity(encryptedStream))
                throw new FileCorruptedException("The document integrity is failed.");

            byte[] decryptedKeyValue = DecryptWithBlockKey(PkeData.SaltValue, PkeData.EncryptedKeyValue, gKeyValueBlockKey);

            DecryptStream(encryptedStream, stream, decryptedKeyValue);
        }

        public void Encrypt(Stream encryptionInfo, Stream stream, Stream encryptedStream, string password)
        {
            throw new NotImplementedException();
        }

        private void InitAlgorithms(string password, string pkeHashName, string hashName)
        {
            mPkeHashAlg = CryptoUtilPal.CreateHashAlgorithm(pkeHashName);
            mHashAlg = CryptoUtilPal.CreateHashAlgorithm(hashName);
            mAgileKey = CreateAgileKey(password, PkeData.SaltValue, PkeData.SpinCount);
        }

        /// <summary>
        /// Decrypts encrypted stream.
        /// </summary>
        private void DecryptStream(Stream srcStream, Stream dstStream, byte[] decryptedKeyValue)
        {
            Debug.Assert(srcStream != null);
            Debug.Assert(dstStream != null);

            srcStream.Position = 0;
            dstStream.Position = 0;

            BinaryReader reader = new BinaryReader(srcStream);
            long actualLength = reader.ReadInt64();

            // SPEED We know what the decrypted stream size will be, so let's allocate upfront to
            // save multiple allocations. But allocate a bit more to accommodate for decryptor block size.
            dstStream.SetLength(MathUtil.RoundUp(actualLength, 512));

            byte[] segment = new byte[4096];

            int segmentIndex = 0;
            while (srcStream.Position < srcStream.Length)
            {
                int segmentSize = System.Math.Min((int)(srcStream.Length - srcStream.Position), 4096);
                StreamUtil.Read(srcStream, segment, 0, segmentSize);

                byte[] iv = ComputeHash(mHashAlg, KeyData.SaltValue, BitConverter.GetBytes(segmentIndex), KeyData.BlockSize, 0x36);
                byte[] decryptedData = Decrypt(decryptedKeyValue, iv, segment);

                dstStream.Write(decryptedData, 0, segmentSize);
                segmentIndex++;
            }

            // Set the actual length because it can be smaller due to encryption padding at the end.
            dstStream.SetLength(actualLength);
            dstStream.Position = 0;
        }

        /// <summary>
        /// Verifies password.
        /// </summary>
        private bool VerifyPassword()
        {
            byte[] salt = PkeData.SaltValue;
            int hashSize = PkeData.HashSize;

            byte[] decryptedHash = Pad(DecryptWithBlockKey(salt, PkeData.EncryptedVerifierHashValue, gVerifierHashValueBlockKey), hashSize, 0x00);
            byte[] inputHash = ComputeHash(mPkeHashAlg, DecryptWithBlockKey(salt, PkeData.EncryptedVerifierHashInput, gVerifierHashInputBlockKey));
            return ArrayUtil.IsArrayEqual(decryptedHash, inputHash);
        }

        /// <summary>
        /// Verifies data integrity.
        /// </summary>
        private bool VerifyIntegrity(Stream srcStream)
        {
            byte[] salt = PkeData.SaltValue;
            byte[] keySalt = KeyData.SaltValue;
            int keyHashSize = KeyData.HashSize;
            int keyBlockSize = KeyData.BlockSize;

            byte[] decryptedKey = DecryptWithBlockKey(salt, PkeData.EncryptedKeyValue, gKeyValueBlockKey);
            byte[] ivDataIntegritySalt = ComputeHash(mHashAlg, keySalt, gDataIntegritySaltBlockKey, keyBlockSize, 0x00);

            Aspose.Crypto.IMac keyDataHashAlg = CryptoUtilPal.CreateHMac(KeyData.HashAlgorithm);
            keyDataHashAlg.Init(DecryptAndPad(decryptedKey, ivDataIntegritySalt, KeyData.EncryptedHmacKey, keyHashSize, 0x00));

            //TODO 0 RK new KeyParameter(DecryptAndPad(decryptedKey, ivDataIntegritySalt, KeyData.EncryptedHmacKey, keyHashSize, 0x00))
            byte[] dataIntegrityHash = ComputeHash(keyDataHashAlg, srcStream);

            // Decrypt the hash generated by the encryptor.
            byte[] ivDataIntegrityHash = ComputeHash(mHashAlg, keySalt, gDataIntegrityHmacValueBlockKey, keyBlockSize, 0x00);
            byte[] decryptedDataIntegrityHmac = DecryptAndPad(decryptedKey, ivDataIntegrityHash, KeyData.EncryptedHmacValue, keyHashSize, 0x00);

            return ArrayUtil.IsArrayEqual(dataIntegrityHash, decryptedDataIntegrityHmac);
        }

        private byte[] DecryptWithBlockKey(byte[] ivBytes, byte[] sourceBytes, byte[] block)
        {
            return Decrypt(ComputeBlockKey(block), ivBytes, sourceBytes);
        }

        private byte[] DecryptAndPad(byte[] keyBytes, byte[] ivBytes, byte[] sourceBytes, int size, byte padding)
        {
            return Pad(Decrypt(keyBytes, ivBytes, sourceBytes), size, padding);
        }

        /// <summary>
        /// Decrypts bytes using block key and iv.
        /// </summary>
        private byte[] Decrypt(byte[] keyBytes, byte[] ivBytes, byte[] sourceBytes)
        {
            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key bytes.
            mAlgorithm = BlockCipherFactory.CreateRijndaelDecryptor(PkeData.CipherChaining, PkeData.BlockSize, keyBytes, ivBytes);
            //  Since at this point we don't know what the size of decrypted data
            //  will be, allocate the buffer long enough to hold ciphertext;
            //  plaintext is never longer than ciphertext.
            byte[] decryptedBytes = new byte[sourceBytes.Length];
            int offset = mAlgorithm.ProcessBytesViaCipherStream(sourceBytes, 0, sourceBytes.Length, decryptedBytes, 0);
            mAlgorithm.DoFinal(decryptedBytes, offset);
        
            return decryptedBytes;
        }

        /// <summary>
        /// Creates base key.
        /// </summary>
        /// <remarks>Consider refactoring with Ecma376Decryptor.</remarks>
        private byte[] CreateAgileKey(string password, byte[] salt, int spinCount)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password != null ? password : "");

            // H(0) = H(salt, password);
            byte[] hash = ComputeHash(mPkeHashAlg, salt, passwordBytes);

            byte[] iterationBytes = new byte[hash.Length + 4];
            for (int i = 0; i < spinCount; i++)
            {
                // Generate each hash in turn
                // H(n) = H(i, H(n-1))
                ArrayUtil.WriteUInt32ToByteArray(i, iterationBytes, 0);
                hash.CopyTo(iterationBytes, 4);

                hash = ComputeHash(mPkeHashAlg, iterationBytes);
            }

            return (hash);
        }

        private byte[] ComputeBlockKey(byte[] blockKey)
        {
            return ComputeHash(mPkeHashAlg, mAgileKey, blockKey, PkeData.KeyBits >> 3, 0x36);
        }

        /// <summary>
        /// Hashes data + block bytes. Resize and pad.
        /// </summary>
        private static byte[] ComputeHash(Aspose.Crypto.IDigest hashAlg, byte[] data, byte[] block, int size, byte padding)
        {
            return Pad(ComputeHash(hashAlg, data, block), size, padding);
        }

        /// <summary>
        /// Hashes data + block bytes.
        /// </summary>
        private static byte[] ComputeHash(Aspose.Crypto.IDigest hashAlg, byte[] data, byte[] block)
        {
            byte[] array = new byte[data.Length + block.Length];

            data.CopyTo(array, 0);
            block.CopyTo(array, data.Length);

            return ComputeHash(hashAlg, array);
        }

        /// <summary>
        /// Resize array to given size and pads new array elements.
        /// </summary>
        private static byte[] Pad(byte[] data, int size, byte padding)
        {
            byte[] newData = new byte[size];

            for (int i = 0; i < size; i++)
                newData[i] = (i < System.Math.Min(size, data.Length)) ? data[i] : padding;

            return newData;
        }

        /// <summary>
        /// Compute hash with the given implementation of IDigest (SHA1, SHAXXX).
        /// </summary>
        private static byte[] ComputeHash(Aspose.Crypto.IDigest alg, byte[] data)
        {
            alg.BlockUpdate(data, 0, data.Length);
            byte[] digest = new byte[alg.GetDigestSize()];
            alg.DoFinal(digest, 0);
            return digest;
        }

        /// <summary>
        /// Compute hash with the given implementation of HMac.
        /// </summary>
        private static byte[] ComputeHash(Aspose.Crypto.IMac keyDataHashAlg, Stream encryptedStream)
        {
            byte[] source = StreamUtil.CopyStreamToByteArray(encryptedStream);
            return keyDataHashAlg.ComputeHash(source);
        }

        /// <summary>
        /// Provides access to base part of encryption info.
        /// </summary>
        private EncryptionInfo KeyData
        {
            get { return mEncryptionInfo; }
        }

        /// <summary>
        /// Provides access to PasswordKeyEncryptor part of encryption info.
        /// </summary>
        private PasswordKeyEncryptor PkeData
        {
            get { return mEncryptionInfo.PasswordKeyEncryptor; }
        }

        private EncryptionInfo mEncryptionInfo;
        private BlockCipher mAlgorithm;
        private Aspose.Crypto.IDigest mPkeHashAlg;
        private Aspose.Crypto.IDigest mHashAlg;
        private byte[] mAgileKey;

        // These block keys are defined in the specification document
        private static readonly byte[] gVerifierHashInputBlockKey = new byte[] { 0xfe, 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79 };
        private static readonly byte[] gVerifierHashValueBlockKey = new byte[] { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e };
        private static readonly byte[] gKeyValueBlockKey = new byte[] { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6 };
        private static readonly byte[] gDataIntegritySaltBlockKey = new byte[] { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6 };
        private static readonly byte[] gDataIntegrityHmacValueBlockKey = new byte[] { 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33 };
    }
}
