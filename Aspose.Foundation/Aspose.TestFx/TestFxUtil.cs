// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2019 by Alexey Butalov

using System;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.TestFx.Pal;
using NUnit.Framework;

namespace Aspose.TestFx
{
    /// <summary>
    /// Utility methods for unit tests.
    /// </summary>
    public static class TestFxUtil
    {
        /// <summary>
        /// Performs common setup of test environment.
        /// </summary>
        public static void SetUpTests()
        {
            Debug.SetTestDefaults();
            DateTimeUtil.SetTestMode();
            RandomUtil.SetTestMode();

            // Register encodings support if required.
            EncodingUtil.RegisterEncodings();
        }

        /// <summary>
        /// Compares two streams and returns the result of comparison.
        /// </summary>
        /// <param name="stream1">First stream to compare.</param>
        /// <param name="stream2">Second stream to compare.</param>
        /// <returns>True, if streams have the same content.</returns>
        public static bool CompareStreams(Stream stream1, Stream stream2)
        {
            if (stream1.Length != stream2.Length)
                return false;

            const int bufferSize = 8192;
            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = stream1.Read(buffer1, 0, bufferSize);
                int count2 = stream2.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                    return false;

                // This ends the loop.
                if (count1 == 0)
                    return true;

                for (int i = 0; i < count1; i++)
                {
                    if (buffer1[i] != buffer2[i])
                        return false;
                }
            }
        }

        /// <summary>
        /// Compares two byte arrays and returns the result of comparison.
        /// </summary>
        /// <param name="buffer1">First byte array to compare.</param>
        /// <param name="buffer2">Second byte array to compare.</param>
        /// <returns>True, if byte arrays have the same content.</returns>
        public static bool CompareBuffers(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                return false;

            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two files and returns the result of comparison.
        /// </summary>
        /// <remarks>
        /// First file should exist in the provided path. Otherwise, an exception is thrown.
        /// The second file may be absent. Returns false if the files are different or the second file is absent.
        /// </remarks>
        /// <param name="filename1">Name of the first file to compare.</param>
        /// <param name="filename2">Name of the second file to compare.</param>
        public static bool CompareFiles(string filename1, string filename2)
        {
            // First file must be present. Otherwise, throw an exception.
            if (!File.Exists(filename1))
                throw new InvalidOperationException(String.Format("File is not found: {0}", filename1));

            // The second file may be absent. Files are considered not equal then.
            if (!File.Exists(filename2))
                return false;

            using (FileStream stream1 = new FileStream(filename1, FileMode.Open, FileAccess.Read))
            {
                using (FileStream stream2 = new FileStream(filename2, FileMode.Open, FileAccess.Read))
                {
                    return CompareStreams(stream1, stream2);
                }
            }
        }

        public static int GetFileSize(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
                return (int)stream.Length;
        }

        /// <summary>
        /// Returns TestOutPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInTestOutPath(string relativePath)
        {
            return CorrectPath(Path.Combine(TestOutPath, relativePath));
        }

        /// <summary>
        /// If file name is rooted, returns as is. Otherwise prepends the TestData folder.
        /// </summary>
        public static string BuildTestFileName(string fileName)
        {
            if (Path.IsPathRooted(fileName))
                return fileName;
            else
                return CorrectPath(Path.Combine(TestDataPath, fileName));
        }

        /// <summary>
        /// Prepends root test data folder (i.e. "awnet/TestData") to the relative path.
        /// </summary>
        [JavaThrows(false)]
        public static string BuildRootTestFileName(string relativePath)
        {
                return CorrectPath(Path.Combine(TestEnvironment.GetTestData(), relativePath));
        }

        /// <summary>
        /// Creates a directory for the specified file if the directory does not exist.
        /// </summary>
        [JavaThrows(false)]
        public static void EnsureDirectoryForFileExists(string fileName)
        {
            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static string BuildOutFileName(string testName, string userSuffix, string extension)
        {
            string result = BuildDerivedFileName(testName, "TestOut", userSuffix + " Out", extension);
            return TestUtilPal.CorrectOutFileNameIfNeeded(result);
        }

        public static string BuildGoldFileName(string testName, string userSuffix, string extension)
        {
            string result = BuildDerivedFileName(testName, "TestGold", userSuffix + " Gold", extension);
            return TestUtilPal.CorrectGoldFileNameIfNeeded(result);
        }

        private static string BuildDerivedFileName(string testName, string dirReplacement, string fileNameSuffix,
            string extension)
        {
            // This makes a full path in case just a relative path was given to us.
            testName = BuildTestFileName(testName);

            string dirOriginal = Path.GetDirectoryName(testName);
            string dirNew = dirOriginal.Replace("TestData", dirReplacement);
            if (dirNew == dirOriginal)
                dirNew = dirOriginal.Replace("TestGold", dirReplacement);
            if (dirNew == dirOriginal)
                dirNew = dirOriginal.Replace("TestOut", dirReplacement);


            string result = Path.Combine(dirNew, Path.GetFileNameWithoutExtension(testName));
            result += fileNameSuffix;
            result += StringUtil.HasChars(extension) ? extension : Path.GetExtension(testName);

            return CorrectPath(result);
        }

        /// <summary>
        /// Accepts the specified OUT file over the existing GOLD file.
        /// This method is platform-aware and will create Java golds in a separate folder from dot net golds.
        /// </summary>
        public static void AcceptNewGold(string fileNameOut, string fileNameGold)
        {
            // Accepting on Java needs to create a Java gold to avoid damaging the dotnet gold.
            fileNameGold = TestUtilPal.GetMostActualGold(fileNameGold);
            EnsureDirectoryForFileExists(fileNameGold);
            File.Copy(fileNameOut, fileNameGold, true);
        }

        /// <summary>
        /// Makes the specified path correct for Windows or Linux both for .NET and Java.
        /// On Linux replaces all '\' with '/' and also replaces drive path with the "root" e.g. equivalent of "X:\".
        /// </summary>
        /// <param name="anyPath">Path string with '\' as delimiter.</param>
        [JavaThrows(false)]
        public static String CorrectPath(String anyPath)
        {
            switch (PlatformUtilPal.GetPlatform())
            {
                case Platform.Windows:
                    {
                        // The path is expected to be with '\' already, do nothing.
                        return anyPath;
                    }
                case Platform.Android:
                case Platform.Unix:
                case Platform.MacOS:
                case Platform.iOS:
                    {
                        // If we have something like this:
                        // X:\awnet\Aspose.Words\TestData\xxx.doc
                        if (UriUtil.IsAbsoluteLocalFilePathWindows(anyPath))
                        {
                            // We need to convert it to something like this:
                            // /media/sf_F_DRIVE/awnet/Aspose.Words/TestData/xxx.doc

                            // We achieve this by:
                            // 1. Drop the drive letter and the first slash.
                            anyPath = anyPath.Substring(3);
                            // 2. Prepend the "project parent" path so it can be any location on Linux.
                            anyPath = Path.Combine(TestEnvironment.GetRawRoot(), anyPath);
                        }

                        // Turn slashes into Linux style.
                        return anyPath.Replace('\\', Path.DirectorySeparatorChar);
                    }
                default:
                    throw new InvalidOperationException("Unexpected platform.");
            }
        }

        /// <summary>
        /// Checks if gold file path must be replaced with local or platform-specific gold.
        /// If not returns <paramref name="originalGold"/> as is, otherwise return properly adjusted path.
        /// </summary>
        public static string GetReplacementGoldIfExists(string originalGold)
        {
            // Assumed is that current environment is bound to a single platform, e.g. C++ & Linux
            // thus we may only have original, platform and local gold at most.
            if (TestSettings.UseLocalGolds)
            {
                string gold = originalGold;
                // This will handle both Aspose.Words and Aspose.Foundation golds.
                // Local golds will have them both in the same directory though.
                // It may lead to a conflict eventually but chances are low.
                const string testGold = @"\TestGold\";
                int testGoldPos = StringUtil.IndexOfIgnoreCase(gold, testGold);
                if (testGoldPos > 0)
                    gold = gold.Substring(testGoldPos + testGold.Length);

                // We first check if local golds should be used, and if one exists.
                // Otherwise default to platform golds if present.
                // Finally use original.

                string localGold = Path.Combine(TestEnvironment.GetTestGoldLocal(), gold);
                if (File.Exists(localGold))
                    return localGold;
            }

            return TestUtilPal.GetMostActualExistingGold(originalGold);
        }

        /// <summary>
        /// Checks color by ARGB value with diagnostic message.
        /// </summary>
        /// <remarks>
        ///  System.Drawing.Color's Equals() checks all structure fields including Name etc.
        ///  For instance, Color 0xFFFFFFFF has the same A, R, G, B values as Color.White
        ///  but different (null) Name -- so the Colors are not equal.
        ///  This utility checks only ARGB values.
        /// </remarks>
        public static void AreEqualArgb(Color expectedColor, Color actualColor)
        {
            int expectedValue = expectedColor.ToArgb();
            int actualValue = actualColor.ToArgb();
            Assert.That(actualValue, Is.EqualTo(expectedValue), string.Format("Expected: 0x{0:X8}, Actual: 0x{1:X8}", expectedValue, actualValue));
        }

        /// <summary>
        /// The root folder of all Aspose.Foundation test documents.
        /// </summary>
        internal static string TestDataPath = TestEnvironment.GetFoundationTestData();

        /// <summary>
        /// The root folder of all Aspose.Foundatioin output documents.
        /// </summary>
        internal static string TestOutPath = TestEnvironment.GetFoundationTestOut();
    }
}
