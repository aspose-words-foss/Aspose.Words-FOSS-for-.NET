// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2018 by Vyacheslav Durin

using System;
using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;

namespace Aspose.Crypto
{
    /// <summary>
    /// This class is a Facade which provides an simplified interface to BouncyCastle's IBufferedCipher and IBlockCipher.
    /// This class can be instantiated via BlockCipherFactory.
    /// Not all the methods from IBufferedCipher and IBlockCipher are implemented.
    /// Extend this class with new methods if needed.
    /// </summary>
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    public class BlockCipher
    {
        // Common methods

        public int GetBlockSize()
        {
            return (mBufferedCipher != null)
                ? mBufferedCipher.GetBlockSize()
                : mBlockCipher.GetBlockSize();
        }

        public void Reset()
        {
            if (mBufferedCipher != null)
                mBufferedCipher.Reset();

            if (mBlockCipher != null)
                mBlockCipher.Reset();
        }

        // IBlockCipher methods, add new if needed

        public void ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
        {
            EnsureExists(mBlockCipher);
            mBlockCipher.ProcessBlock(inBytes, inOff, outBytes, outOff);
        }

        public void ProcessData(byte[] inBytes, byte[] outBytes)
        {
            EnsureExists(mBlockCipher);
            if (inBytes.Length % mBlockCipher.GetBlockSize() != 0)
                throw new ArgumentException("Data size is not rounded to the cipher block size.");
            if (outBytes.Length < inBytes.Length)
                throw new ArgumentException("Not enough bytes in output array");

            int blockSize = mBlockCipher.GetBlockSize();
            for (int i = 0; i < inBytes.Length; i += blockSize)
                ProcessBlock(inBytes, i, outBytes, i);
        }

        // IBufferedCipher methods, add new if needed

        public void DoTransform(Stream srcStream, Stream dstStream)
        {
            EnsureExists(mBufferedCipher);
            CipherStream outCipherStream = new CipherStream(srcStream, mBufferedCipher, null);
            StreamUtil.CopyStream(outCipherStream, dstStream);
        }

        public int GetOutputSize(int inLen)
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.GetOutputSize(inLen);
        }

        public byte[] ProcessBytes(byte[] input, int inOff, int length)
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.ProcessBytes(input, inOff, length);
        }

        public int ProcessBytes(byte[] inputBytes, byte[] outputBytes, int v)
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.ProcessBytes(inputBytes, outputBytes, v);
        }

        public int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.ProcessBytes(input, inOff, length, output, outOff);
        }

        // TODO: check, perhaps this method could share the same behaviour
        public int ProcessBytesViaCipherStream(byte[] input, int inOff, int inLen, byte[] outBytes, int v)
        {
            EnsureExists(mBufferedCipher);
#if PLAIN_JAVA
            // JAVA-changed: Using BufferedBlockCipher (mAlgorithm) instead of CipherInputStream.
            try
            {
                mAlgorithm.processBytes(sourceBytes, 0, sourceBytes.length, decryptedBytes, 0);
                mAlgorithm.doFinal(decryptedBytes, 0);
            }
            catch (Exception ex)
            {
                ex.printStackTrace();
            }
#else
            using (MemoryStream memoryStream = new MemoryStream(input, 0, input.Length))
            {
                //  Define memory stream which will be used to hold encrypted data.
                using (CipherStream cryptoStream = new CipherStream(memoryStream, mBufferedCipher, null))
                {
                    StreamUtil.Read(cryptoStream, outBytes, 0, outBytes.Length);
                }
            }
#endif
            return outBytes.Length;
        }

        public byte[] DoFinal()
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.DoFinal();
        }

        public int DoFinal(byte[] outBytes, int pos)
        {
            EnsureExists(mBufferedCipher);
            return mBufferedCipher.DoFinal(outBytes, pos);
        }

        private static void EnsureExists(object cipher)
        {
            if (cipher == null)
                throw new ArgumentException("Cipher cannot be null");
        }

        internal BlockCipher(IBufferedCipher cipher)
        {
            EnsureExists(cipher);
            mBufferedCipher = cipher;
        }

        internal BlockCipher(IBlockCipher cipher)
        {
            EnsureExists(cipher);
            mBlockCipher = cipher;
        }

        private readonly IBufferedCipher mBufferedCipher;
        private readonly IBlockCipher mBlockCipher;
    }
}
