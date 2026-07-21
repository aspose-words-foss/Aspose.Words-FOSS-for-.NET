// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/09/2018 by Ilya Navrotskiy

using System;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    /// <summary>
    /// Tests RandomUtil class.
    /// </summary>
    [TestFixture]
    public class TestRandomUtil
    {
        /// <summary>
        /// WORDSNET-17450 RandomUtil.GetRandomBytesArray() always generates the same result.
        /// We should add a seed material to a random generator.
        /// </summary>
        /// <remarks>
        /// Tests random byte arrays generated in test mode.
        /// In this mode the sequence of random byte arrays must be always the same.
        /// </remarks>
        [Test]
        public void TestJira17450TestMode()
        {
            // Set test mode.
            RandomUtil.SetTestMode();

            // Check random bytes in test mode are always generated in determined sequence.
            CheckArraysEqual(gFirstTestRandomValue, RandomUtil.GetRandomBytesArray(16));
            CheckArraysEqual(gSecondTestRandomValue, RandomUtil.GetRandomBytesArray(16));

            // Reset and check values are generated from the beginning again.
            RandomUtil.Reset();
            CheckArraysEqual(gFirstTestRandomValue, RandomUtil.GetRandomBytesArray(16));
            CheckArraysEqual(gSecondTestRandomValue, RandomUtil.GetRandomBytesArray(16));
        }

        /// <summary>
        /// Relates to WORDSNET-17450
        /// </summary>
        /// <remarks>
        /// Tests random byte arrays generated in production mode.
        /// Checks that random generator produces different sequence of random bytes.
        /// </remarks>
        [Test]
        [NonParallelizable]
        public void TestJira17450ProductionModeA()
        {
            // Set production mode.
            RandomUtil.ResetTestMode();

            // Generate random bytes array of some arbitrary lengths.
            CheckArraysNotEqual(RandomUtil.GetRandomBytesArray(16), RandomUtil.GetRandomBytesArray(16));
            CheckArraysNotEqual(RandomUtil.GetRandomBytesArray(1), RandomUtil.GetRandomBytesArray(1));
            CheckArraysNotEqual(RandomUtil.GetRandomBytesArray(3), RandomUtil.GetRandomBytesArray(3));
            CheckArraysNotEqual(RandomUtil.GetRandomBytesArray(4), RandomUtil.GetRandomBytesArray(4));
        }

        /// <summary>
        /// Relates to WORDSNET-17450
        /// </summary>
        /// <remarks>
        /// Tests random byte arrays generated in production mode.
        /// Checks that even after generator is reset it produces different sequence of random bytes.
        /// </remarks>
        [Test]
        [NonParallelizable]
        public void TestJira17450ProductionModeB()
        {
            // Set production mode.
            RandomUtil.ResetTestMode();
            byte[][] randomBytesArrayA = GetArrayOfRandomByteArrays(16, 1, 2, 3, 4, 5, 8, 14, 17, 32);

            // Reset and generate another one array of random byte arrays.
            RandomUtil.Reset();
            byte[][] randomBytesArrayB = GetArrayOfRandomByteArrays(16, 1, 2, 3, 4, 5, 8, 14, 17, 32);

            // Check arrays different after random generator was reset.
            for (int i = 0; i < randomBytesArrayA.Length; i++)
                CheckArraysNotEqual(randomBytesArrayA[i], randomBytesArrayB[i]);
        }

        /// <summary>
        /// WORDSJAVA-1917 In order to verify encrypted data we have to have predictable vectors.
        /// So the PredictableRandomGeneratorPal class was introduced to achieve this goals.
        /// When testMode is on then the same sequence of byte arrays comes up.
        /// </summary>
        [Test]
        public void TestPredictableRandomInTestMode()
        {
            RandomUtil.SetTestMode();
#if JAVA
            string[] sequence = { "N0frUk0DaLES69wyD38C+A==", "b1EvVLRfboPl8h7m/GqfeQ==", "9g8f2o7Vx7tWhDxLIiVzRg==", };
#elif CPLUSPLUS
            string[] sequence = { "O/z8vOE75EX3owC7fJ/PdA==", "3by9atbfEJ7g0P9aHpAcLA==", "RX2nxCPFmYPgX34n65BSRA==", };
#else
            string[] sequence = { "Q9pYG1eESeStPAq3/5F7kA==", "XkXHWDGtZ37QbXRrv5RvFA==", "MGFLNkpeFOHNX6XGyIz/JA==", };
#endif
            foreach (string bytesArray in sequence)
            {
                byte[] bytes = RandomUtil.GetRandomBytesArray(16);
                Assert.That(Convert.ToBase64String(bytes), Is.EqualTo(bytesArray));
            }
        }

        /// <summary>
        /// WORDSJAVA-1917 When testMode is off then the random sequence of byte arrays comes up.
        /// </summary>
        [Test]
        [NonParallelizable]
        public void TestProductionRandom()
        {
            RandomUtil.ResetTestMode();
#if JAVA
            string[] sequence = { "N0frUk0DaLES69wyD38C+A==", "b1EvVLRfboPl8h7m/GqfeQ==", "9g8f2o7Vx7tWhDxLIiVzRg==", };
#elif CPLUSPLUS
            string[] sequence = { "O/z8vOE75EX3owC7fJ/PdA==", "3by9atbfEJ7g0P9aHpAcLA==", "RX2nxCPFmYPgX34n65BSRA==", };
#else
            string[] sequence = { "Q9pYG1eESeStPAq3/5F7kA==", "XkXHWDGtZ37QbXRrv5RvFA==", "MGFLNkpeFOHNX6XGyIz/JA==", };
#endif
            foreach (string bytesArray in sequence)
            {
                byte[] bytes = RandomUtil.GetRandomBytesArray(16);
                Assert.That(Convert.ToBase64String(bytes), IsNot.EqualTo(bytesArray));
            }
        }

        /// <summary>
        /// Gets array of random byte arrays.
        /// </summary>
        private static byte[][] GetArrayOfRandomByteArrays(params int[] arraysLengths)
        {
            byte[][] randomBytesArray = new byte[arraysLengths.Length][];
            for (int i = 0; i < arraysLengths.Length; i++)
                randomBytesArray[i] = RandomUtil.GetRandomBytesArray(arraysLengths[i]);

            return randomBytesArray;
        }

        /// <summary>
        /// Checks arrays are equal.
        /// </summary>
        private static void CheckArraysEqual(byte[] arrayA, byte[] arrayB)
        {
            Assert.That(ArrayUtil.IsArrayEqual(arrayA, arrayB), Is.True, string.Format("\nArrayA: {0}\nArrayB: {1}", ArrayUtil.DumpArray(arrayA), ArrayUtil.DumpArray(arrayB)));
        }

        /// <summary>
        /// Checks arrays are not equal.
        /// </summary>
        private static void CheckArraysNotEqual(byte[] arrayA, byte[] arrayB)
        {
            Assert.That(ArrayUtil.IsArrayEqual(arrayA, arrayB), Is.False, string.Format("Both arrays equal to: {0}", ArrayUtil.DumpArray(arrayA)));
        }

        /// <summary>
        /// Random array in test mode is generated using the same deterministic seed,
        /// so the sequence of random bytes in test mode always the same.
        /// </summary>
#if JAVA
        private static readonly byte[] gFirstTestRandomValue = new byte[] { 0x37, 0x47, 0xEB, 0x52, 0x4D, 0x03, 0x68, 0xB1, 0x12, 0xEB, 0xDC, 0x32, 0x0F, 0x7F, 0x02, 0xF8 };
        private static readonly byte[] gSecondTestRandomValue = new byte[] { 0x6F, 0x51, 0x2F, 0x54, 0xB4, 0x5F, 0x6E, 0x83, 0xE5, 0xF2, 0x1E, 0xE6, 0xFC, 0x6A, 0x9F, 0x79 };
#elif CPLUSPLUS
        private static readonly byte[] gFirstTestRandomValue = new byte[] { 0x3B, 0xFC, 0xFC, 0xBC, 0xE1, 0x3B, 0xE4, 0x45, 0xF7, 0xA3, 0x00, 0xBB, 0x7C, 0x9F, 0xCF, 0x74};
        private static readonly byte[] gSecondTestRandomValue = new byte[] { 0xDD, 0xBC, 0xBD, 0x6A, 0xD6, 0xDF, 0x10, 0x9E, 0xE0, 0xD0, 0xFF, 0x5A, 0x1E, 0x90, 0x1C, 0x2C };
#else

        private static readonly byte[] gFirstTestRandomValue = new byte[] { 0x43, 0xDA, 0x58, 0x1B, 0x57, 0x84, 0x49, 0xE4, 0xAD, 0x3C, 0x0A, 0xB7, 0xFF, 0x91, 0x7B, 0x90 };
        private static readonly byte[] gSecondTestRandomValue = new byte[] { 0x5E, 0x45, 0xC7, 0x58, 0x31, 0xAD, 0x67, 0x7E, 0xD0, 0x6D, 0x74, 0x6B, 0xBF, 0x94, 0x6F, 0x14 };
#endif
    }
}
