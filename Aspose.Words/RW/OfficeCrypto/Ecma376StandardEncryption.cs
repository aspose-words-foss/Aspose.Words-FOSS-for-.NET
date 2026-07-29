// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2009 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.Crypto;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements ECMA-376 Standard Encryption algorithm. 
    /// </summary>
    internal class Ecma376StandardEncryption : IEcma376Encryption
    {
        /// <summary>
        /// Decrypts stream using ECMA-376 Standard Encryption algorithm.
        /// </summary>
        public void Decrypt(Stream encryptionInfoStream, Stream encryptedStream, Stream dstStream, string password)
        {
            // Skip EncryptionVersionInfo and copy of the Flags stored in the EncryptionHeader field.
            encryptionInfoStream.Position = 8;
            BinaryReader reader = new BinaryReader(encryptionInfoStream);

            EncryptionHeader header = new EncryptionHeader(reader);
            EncryptionVerifier verifier = new EncryptionVerifier(reader, true);

            CheckEncryptionHeader(header);

            Aspose.Crypto.IDigest digest = CryptoUtilPal.CreateHashAlgorithm(DigestAlgorithm.Sha1);

            BlockCipher decryptor =
                BlockCipherFactory.CreateRijndaelDecryptor(GenerateKey(header.KeySize, digest, password, verifier.Salt));

            CheckEncryptionVerifier(decryptor, verifier);

            DecryptStream(decryptor, encryptedStream, dstStream);
        }

        /// <summary>
        /// Encrypts stream using ECMA-376 Standard Encryption algorithm. Writes EncryptionInfo stream.
        /// </summary>
        public void Encrypt(Stream encryptionInfo, Stream srcStream, Stream encryptedStream, string password)
        {
            BinaryWriter writer = new BinaryWriter(encryptionInfo);

            WriteEncryptionHeader(writer);

            byte[] salt = GetRandomBytes(16);
            BlockCipher encryptor = BlockCipherFactory.CreateRijndaelEncryptor(GenerateKey(0x80, CryptoUtilPal.CreateHashAlgorithm(DigestAlgorithm.Sha1), password, salt));

            EncryptionVerifier verifier = ComputeEncryptionVerifier(encryptor, salt);
            verifier.Write(writer);

            EncryptStream(encryptor, srcStream, encryptedStream);
        }

        /// <summary>
        /// Verifies that encryption specified in header is supported.
        /// </summary>
        [JavaThrows(true)]
        private static void CheckEncryptionHeader(EncryptionHeader header)
        {
            if (header.Flags != (EncryptionHeaderFlags.CryptoApi | EncryptionHeaderFlags.Aes))
                throw new InvalidOperationException("Unexpected encryption flags.");

            if ((header.AlgId != AlgorithmId.Aes128) &&
                (header.AlgId != AlgorithmId.Aes192) &&
                (header.AlgId != AlgorithmId.Aes256))
                throw new InvalidOperationException("Unexpected encryption algorithm id.");

            if (header.AlgIdHash != AlgorithmIdHash.Sha1)
                throw new InvalidOperationException("Unexpected encryption algorithm hash id.");
        }

        /// <summary>
        /// Validates encryption password verifier.
        /// </summary>
        [JavaThrows(true)]
        private static void CheckEncryptionVerifier(BlockCipher cryptoTransform, EncryptionVerifier encryptionVerifier)
        {
            Debug.Assert(cryptoTransform != null);

            //2. Decrypt the EncryptedVerifier field of the EncryptionVerifier 
            // structure as specified in section 2.3.3, and generated as specified in section 2.3.4.8. 
            // to obtain the Verifier value. 
            byte[] verifier = DoFinal(cryptoTransform,
                encryptionVerifier.EncryptedVerifier, 0, encryptionVerifier.EncryptedVerifier.Length);

            //3. Decrypt the EncryptedVerifierHash field of the EncryptionVerifier structure to obtain 
            // the hash of the Verifier value. 
            byte[] verifierHash = DoFinal(cryptoTransform,
                encryptionVerifier.EncryptedVerifierHash, 0, encryptionVerifier.EncryptedVerifierHash.Length);

            //4. Calculate the SHA-1 hash value of the Verifier value calculated in step 2.
            byte[] expectedVerifierHash = HashUtil.ComputeHash(DigestAlgorithm.Sha1, verifier);

            //5. Compare the results of step 3 and step 4. If the two hash values do not match, the password is incorrect.            
            //The number of bytes used by the decrypted Verifier hash is given by the VerifierHashSize field.
            if (!ArrayUtil.CompareBytes(verifierHash, expectedVerifierHash, encryptionVerifier.VerifierHashSize))
                throw new IncorrectPasswordException("The document password is incorrect.");
        }

        /// <summary>
        /// Decrypts the specified stream.
        /// Must be called after the password was verified.
        /// Does not close streams.
        /// </summary>
        /// <param name="decryptor"></param>
        /// <param name="srcStream">Decrypted from position 0 to the end.</param>
        /// <param name="dstStream">Written from position 0. The position is set to 0 when the method returns.</param>
        private static void DecryptStream(BlockCipher decryptor, Stream srcStream, Stream dstStream)
        {
            Debug.Assert(decryptor != null);
            Debug.Assert(srcStream != null);
            Debug.Assert(dstStream != null);

            srcStream.Position = 0;
            dstStream.Position = 0;

            BinaryReader reader = new BinaryReader(srcStream);
            long actualLength = reader.ReadInt64();

            // SPEED We know what the decrypted stream size will be, so let's allocate upfront to
            // save multiple allocations. But allocate a bit more to accommodate for decryptor block size.
            dstStream.SetLength(MathUtil.RoundUp(actualLength, 512));

            decryptor.DoTransform(srcStream, dstStream);

            // Set the actual length because it can be smaller due to encryption padding at the end.
            dstStream.SetLength(actualLength);

            srcStream.Position = 0;
            dstStream.Position = 0;
        }

        [JavaThrows(true)]
        private EncryptionVerifier ComputeEncryptionVerifier(BlockCipher encryptor, byte[] salt) 
        {
            byte[] verifier = GetRandomBytes(16);

            byte[] temp = HashUtil.ComputeHash(DigestAlgorithm.Sha1, verifier);
            byte[] verifierHash = new byte[0x20];
            Array.Copy(temp, verifierHash, temp.Length);

            EncryptionVerifier ret = new EncryptionVerifier();
            ret.Salt = salt;
            ret.SaltSize = 0x10;
            ret.VerifierHashSize = 0x20;
            ret.EncryptedVerifier = DoFinal(encryptor, verifier, 0, verifier.Length);
            ret.EncryptedVerifierHash = DoFinal(encryptor, verifierHash, 0, verifierHash.Length);
            
            return ret;
        }

        /// <summary>
        /// Writes EncryptionHeader structure.
        /// </summary>
        private static void WriteEncryptionHeader(BinaryWriter writer)
        {
            writer.Write((int)EncryptionVersionInfo.Standard2010);
            writer.Write((Int32)(EncryptionHeaderFlags.Aes | EncryptionHeaderFlags.CryptoApi));
            writer.Write(0x8c);
            writer.Write((Int32)(EncryptionHeaderFlags.Aes | EncryptionHeaderFlags.CryptoApi));
            writer.Write(0);
            writer.Write(0x0000660E);
            writer.Write(0x00008004);
            writer.Write(0x00000080);
            writer.Write(0x00000018);
            writer.Write(0);
            writer.Write(0);
            OfficeCryptoUtil.WriteWChar("Microsoft Enhanced RSA and AES Cryptographic Provider", writer);
        }

        /// <summary>
        /// Decrypts the specified stream.
        /// Must be called after the password was verified.
        /// Does not close streams.
        /// </summary>
        /// <param name="srcStream">Decrypted from position 0 to the end.</param>
        /// <param name="dstStream">Written from position 0. The position is set to 0 when the method returns.</param>
        /// <param name="encryptor">Encryption algorithm.</param>
        private static void EncryptStream(BlockCipher encryptor, Stream srcStream, Stream dstStream)
        {
            Debug.Assert(encryptor != null);
            Debug.Assert(srcStream != null);
            Debug.Assert(dstStream != null);

            srcStream.Position = 0;
            dstStream.Position = 0;

            long actualLength = srcStream.Length;
            srcStream.SetLength(MathUtil.RoundUp(srcStream.Length, 16));

            BinaryWriter writer = new BinaryWriter(dstStream);
            writer.Write(actualLength);
            
            // SPEED We know what the decrypted stream size will be, so let's allocate upfront to
            // save multiple allocations. But allocate a bit more to accommodate for decryptor block size 
            // considering initial 8-byte size.
            dstStream.SetLength(MathUtil.RoundUp(actualLength, 16) + 8);

            encryptor.DoTransform(srcStream, dstStream);

            srcStream.Position = 0;
            dstStream.Position = 0;
        }

        /// <summary>
        /// Generate an encryption key as specified in section 2.3.4.7.
        /// </summary>
        private static byte[] GenerateKey(int keySize, Aspose.Crypto.IDigest digest, string password, byte[] salt)
        {
            byte[] hash = ComputeKeyHash(digest, password, salt);

            byte[] x1 = ComputeX(digest, 0x36, hash);
            byte[] x2 = ComputeX(digest, 0x5c, hash);

            // Concatenate X1 with X2 to form X3, which will yield a value twice the length of cbHash.
            byte[] x3 = new byte[x1.Length + x2.Length];
            x1.CopyTo(x3, 0);
            x2.CopyTo(x3, x1.Length);

            // Let keyDerived be equal to the first cbRequiredKeyLength bytes of X3.
            byte[] key = new byte[keySize / 8];
            Array.Copy(x3, 0, key, 0, key.Length);
            return key;
        }

        private static byte[] ComputeKeyHash(Aspose.Crypto.IDigest digest, string password, byte[] salt)
        {
            if (password == null)
                password = "";

            // The password MUST be provided as an array of Unicode characters.
            // The initial password hash is generated as: H0 = H(salt + password).
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] initialBytes = new byte[salt.Length + passwordBytes.Length];
            salt.CopyTo(initialBytes, 0);
            passwordBytes.CopyTo(initialBytes, salt.Length);
            byte[] hash = HashUtil.ComputeHash(digest, initialBytes);

            // The hash is then iterated using the following approach: Hn = H(iterator + Hn-1)
            byte[] iterationBytes = new byte[4 + hash.Length];
            for (int iterator = 0; iterator < 50000; iterator++)
            {
                ArrayUtil.WriteUInt32ToByteArray(iterator, iterationBytes, 0);
                hash.CopyTo(iterationBytes, 4);
                hash = HashUtil.ComputeHash(digest, iterationBytes);
            }

            // The method used to generate the hash data which is the input into the key 
            // derivation algorithm is: Hfinal = H(Hn + block) and the block number MUST be 0x00000000.
            hash.CopyTo(iterationBytes, 0);
            ArrayUtil.WriteUInt32ToByteArray(0, iterationBytes, hash.Length);
            return HashUtil.ComputeHash(digest, iterationBytes);
        }

        private static byte[] ComputeX(Aspose.Crypto.IDigest digest, byte value, byte[] hash)
        {
            // Form a 64-byte buffer by repeating the constant 0x36 64 times.
            byte[] buf = new byte[64];
            for (int i = 0; i < buf.Length; i++)
                buf[i] = value;

            // XOR Hfinal into the first cbHash bytes of this buffer.
            for (int i = 0; i < hash.Length; i++)
                buf[i] = (byte)(buf[i] ^ hash[i]);

            // Compute a hash of the resulting 64-byte buffer using hashing algorithm H. 
            // This will yield a hash value of length cbHash. Let the resultant value be called X1.
            return HashUtil.ComputeHash(digest, buf);
        }

        /// <summary>
        /// Gets array of random bytes.
        /// </summary>
        private byte[] GetRandomBytes(int count)
        {
            byte[] randomBytes = new byte[count];
            mRandom.NextBytes(randomBytes);
            return randomBytes;
        }

        private static byte[] DoFinal(BlockCipher cryptoTransform, byte[] input, int inOff,int inLen)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int length = cryptoTransform.GetOutputSize(inLen);
            byte[] outBytes = new byte[0];
            if (length > 0)
            {
                outBytes = new byte[length];
                int pos = (inLen > 0) ? cryptoTransform.ProcessBytes(input, inOff, inLen, outBytes, 0) : 0;
                try
                {
                    pos += cryptoTransform.DoFinal(outBytes, pos);
                }
                catch (Exception e)
                {
                    // Java throws DataLengthException, IllegalStateException, InvalidCipherTextException
                    // .NET doesn't throw anything.
                    // In order to make it autoportable I have to wrap DoFinal method here.
                    // @exception DataLengthException if there is insufficient space in out for
                    // the output, or the input is not block size aligned and should be.
                    // @exception IllegalStateException if the underlying cipher is not initialised.
                    // @exception InvalidCipherTextException if padding is expected and not found.
                    Debug.WriteLine("ERROR: Please check that the underlying cipher is initialised. " +
                                    "The input is block size aligned. Padding is correct." +
                                    "There is not insufficient space in out for the output. Message:" + e.Message);
                }

                if (pos < outBytes.Length)
                {
                    byte[] tmp = new byte[pos];
                    Array.Copy(outBytes, 0, tmp, 0, pos);
                    outBytes = tmp;
                }
            }
            else
            {
                cryptoTransform.Reset();
            }
            return outBytes;
        }

        private readonly Aspose.Crypto.IRandomGenerator mRandom = CryptoUtilPal.CreateRandomGenerator(DigestAlgorithm.Sha256);
    }
}
