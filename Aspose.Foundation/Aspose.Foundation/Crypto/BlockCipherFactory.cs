// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2018 by Vyacheslav Durin

using Aspose.JavaAttributes;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Aspose.Crypto
{
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    public static class BlockCipherFactory
    {
        public static BlockCipher CreateRijndaelDecryptor(string chainingMode, int blockSize, byte[] keyBytes, byte[] ivBytes)
        {
            int blockBits = blockSize << 3;
            IBlockCipher rijndael = new RijndaelEngine(blockBits);
            IBlockCipher blockCipher = (chainingMode == "ChainingModeCFB"
                ? new CfbBlockCipher(rijndael, blockBits) as IBlockCipher
                : new CbcBlockCipher(rijndael));
            IBufferedCipher cipher = new BufferedBlockCipher(blockCipher);
            cipher.Init(false, new ParametersWithIV(new KeyParameter(keyBytes), ivBytes));

            return new BlockCipher(cipher);
        }

        /// <summary>
        /// Creates Rijndael cipher based on key's length.
        /// Initialized for decrypting.
        /// Supports lengths 128, 160, 192, 224 or 256.
        /// </summary>
        public static BlockCipher CreateRijndaelDecryptor(byte[] generateKey)
        {
            // .NET blocksize=16  AES/CBC
            IBufferedCipher cipher = new BufferedBlockCipher(new RijndaelEngine(generateKey.Length << 3));
            cipher.Init(false, new KeyParameter(generateKey));

            return new BlockCipher(cipher);
        }

        /// <summary>
        /// Creates Rijndael cipher based on key's length.
        /// Initialized for encrypting.
        /// Supports lengths 128, 160, 192, 224 or 256.
        /// </summary>
        public static BlockCipher CreateRijndaelEncryptor(byte[] generateKey)
        {
            IBufferedCipher cipher = new BufferedBlockCipher(new RijndaelEngine(generateKey.Length << 3));
            cipher.Init(true, new KeyParameter(generateKey));

            return new BlockCipher(cipher);
        }

        public static BlockCipher CreateBlowfish(bool isForEncryption, byte[] keyBytes, byte[] iv)
        {
            IBlockCipher cipher = new CfbBlockCipher(new BlowfishEngine(), iv.Length * 8); // Block size in bits.
            ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(keyBytes), iv);
            cipher.Init(isForEncryption, parameters);

            return new BlockCipher(cipher);
        }

        /// <summary>
        /// Creates AES cipher in CBC mode with ANSI X9.23 padding.
        /// </summary>
        public static BlockCipher CreateAesCbcX923Cipher(bool isForEncryption, byte[] keyBytes, byte[] iv)
        {
            IBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            IBufferedCipher paddedBlockCipher = new PaddedBufferedBlockCipher(blockCipher, new X923Padding());
            ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(keyBytes), iv);
            paddedBlockCipher.Init(isForEncryption, parameters);

            return new BlockCipher(paddedBlockCipher);
        }

        /// <summary>
        /// Creates AES cipher in CBC mode with PKCS#7 padding without initialization vector.
        /// </summary>
        public static BlockCipher CreateAesCbcPkcs7NoIvEncryptor(byte[] keyBytes)
        {
            IBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            IBufferedCipher paddedBlockCipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            paddedBlockCipher.Init(true, new KeyParameter(keyBytes));

            return new BlockCipher(paddedBlockCipher);
        }

        /// <summary>
        /// Creates AES cipher in CBC mode without padding.
        /// </summary>
        public static BlockCipher CreateAesCbcNoPaddingEncryptor(byte[] keyBytes, byte[] iv)
        {
            IBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            ParametersWithIV parameters = new ParametersWithIV(new KeyParameter(keyBytes), iv);
            blockCipher.Init(true, parameters);

            return new BlockCipher(blockCipher);
        }
    }
}
