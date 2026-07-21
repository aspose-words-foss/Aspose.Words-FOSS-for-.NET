// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2011 by Andrey Soldatov

using System;
using Aspose.Crypto;
using Aspose.JavaAttributes;
using Org.BouncyCastle.Security;

namespace Aspose.Common
{
    /// <summary>
    /// Random numbers generation related methods.
    ///
    /// AW uses test system which requires byte level comparisons between output files and template ("Gold") files.
    /// This utility class is introduced to make output files independent on random number generation.
    ///
    /// Note: Call <see cref="SetTestMode"/> before executing any tests which can be connected to methods of this class.
    ///
    /// As this class violates equality between output files in Release and Test modes, keep it as simple as possible.
    /// </summary>
    [JavaManual("Too many ThreadStatic properties")]
    public static class RandomUtil
    {
        /// <summary>
        /// <para>Sets Test flag. This flag is used to make behavior of some methods deterministic for test purposes.
        /// Call this method before running any tests which can be connected to methods of this class.</para>
        /// </summary>
        public static void SetTestMode()
        {
            gTestMode = true;
            Reset();
        }

        /// <summary>
        /// Resets the Test flag.
        /// </summary>
        public static void ResetTestMode()
        {
            gTestMode = false;
        }

        /// <summary>
        /// Reset static initializers, so they don't create inter-test dependencies.
        /// </summary>
        /// <remarks>Remarks this method is useful in nunit functions marked with [SetUp] attribute.</remarks>
        public static void Reset()
        {
            gLastPseudorandomInitializer = 0;
            gRandomGenerator = null;
        }

        /// <summary>
        /// <para>
        /// Returns random <see cref="Guid"/>.
        /// </para>
        /// <para>
        /// If <see cref="SetTestMode"/> has not been called before,
        /// it's the same as <see cref="Guid.NewGuid"/> (Parameters are ignored).
        /// </para>
        /// <para>
        /// In test mode returns Guid built as hash from <paramref name="initializers"/>,
        /// so it's constant for the same set of initializers. Provide different initializers to get different Guids.
        /// </para>
        /// <para>If no initializers provided, then this methods will use increasing int values from 0 up to generate Guids.
        /// This is useful when caller does not want to keep track of any initializers but wants to generate new ones all the time.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Each initializer must be of one of following types: string, byte[], int, Enum.
        /// Throws <see cref="NotImplementedException"/> if an initializer has other type.
        /// Implement support of other types if needed.
        /// Note: see description of gTestMode for explanation of using TEST constant.
        /// </remarks>
        public static Guid NewGuid(params object[] initializers)
        {
            if (!gTestMode)
                return Guid.NewGuid();

            if (initializers.Length == 0)
            {
                initializers = new object[] {gLastPseudorandomInitializer++};
            }

            byte[] guid = new byte[16];
            int offset = 0;

            foreach (object obj in initializers)
            {
                int newHash = GetTestHashCode(obj) ^ BitConverter.ToInt32(guid, offset);
                ArrayUtil.WriteUInt32ToByteArray(newHash, guid, offset);
                offset += 4;
                offset %= 16;
            }

            return new Guid(guid);
        }

        // WORDSCPP-564
#if CPLUSPLUS
        public static Guid NewGuid()
        {
            return NewGuid(new object[] {});
        }

        public static Guid NewGuid(string initializer)
        {
            return NewGuild(new object[] {initializer});
        }
#endif


        public static byte[] GetRandomBytesArray(int length)
        {
            if (gRandomGenerator == null)
            {
                gRandomGenerator = CryptoUtilPal.CreateRandomGenerator(DigestAlgorithm.Sha256);
                if (!gTestMode)
                {
                    // WORDSNET-17450 Add seed to ensure random bytes are different every time it is generated.
                    byte[] seed = SecureRandom.GetSeed(length);
                    gRandomGenerator.AddSeedMaterial(seed);
                }
            }

            byte[] bytes = new byte[length];
            gRandomGenerator.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// <para>Returns 4-byte hash for string, byte[], int and, Enum types.</para>
        /// <para>Throws <see cref="NotImplementedException"/> if <paramref name="value"/> has other type.</para>
        /// <para>Use it for test purposes only, as there was no goal to provide good hash algorithm.</para>
        /// </summary>
        /// <remarks>
        /// Implement support of other types if needed.
        /// </remarks>
        private static int GetTestHashCode(object value)
        {
            byte[] valueBytes = value as byte[];
            if (valueBytes != null)
                value = ArrayUtil.DumpArray(valueBytes, 0, Math.Min(256, valueBytes.Length));

            string valueStr = value as string;
            if (valueStr != null)
                return HashUtil.NetGetHashCode(valueStr);

            if (value is int)
                return (int)value;

            if (value is Enum)
                return (int)value;

            throw new InvalidOperationException("Type of object is not supported.");
        }

        /// <summary>
        /// This static member is used to switch between generating of convenient
        /// random numbers and stub (test) random numbers which CAN and SHOULD be different in different tests
        /// but MUST be equal for one test from run to run.
        ///
        /// All tests which can be connected to methods of this class must call <see cref="SetTestMode"/>
        /// to set this flag to <c>true</c>.
        /// </summary>
        [ThreadStatic]
        private static bool gTestMode = false;

        /// <summary>
        /// Used in TEST builds to enumerate initializer seeds passed inside <see cref="NewGuid()"/>,
        /// so caller does not have to worry about a problem of storing
        /// initializer values.
        /// </summary>
        [ThreadStatic]
        private static int gLastPseudorandomInitializer;

        [ThreadStatic]
        private static IRandomGenerator gRandomGenerator;
    }
}
