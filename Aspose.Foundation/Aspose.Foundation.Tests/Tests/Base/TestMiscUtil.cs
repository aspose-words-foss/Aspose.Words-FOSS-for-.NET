// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using Aspose.Zip;
using CodePorting.Translator.Cs2Cpp;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestMiscUtil
    {
        [Test]
        public void TestSortedListStringOrder()
        {
#if !CPLUSPLUS // Aspose.csPorter for C++ doesn't support untyped collection

            // Default .NET sorted list is case sensitive, but it sorts strings using word comparison.
            // The sorted result actually looks like case-insensitive.
            // On the other hand, Java compares Unicode character values only and this results in
            // different sort order and differences in golds that we want to avoid.
            SortedList list = new SortedList();
            list.Add("Marble", null);
            list.Add("Apple 2", null);
            list.Add("apple 1", null);
            list.Add("license", null);

#if !JAVA  // This part of the test just verifies the platform specific sorting.
            Assert.That(list.GetKey(0), Is.EqualTo("apple 1"));
            Assert.That(list.GetKey(1), Is.EqualTo("Apple 2"));
            Assert.That(list.GetKey(2), Is.EqualTo("license"));
            Assert.That(list.GetKey(3), Is.EqualTo("Marble"));
#else
            checkCorrectSorting(list);
#endif

#endif

            // This part of the test verifies sorting that is the same across both platforms.
            // The solution is to use this comparer in .NET for case-sensitive sorted lists
            // when we want to achieve same sorted order as in java.
            SortedStringListGeneric<object> list2 = new SortedStringListGeneric<object>(true);
            list2.Add("Marble", null);
            list2.Add("Apple 2", null);
            list2.Add("apple 1", null);
            list2.Add("license", null);

            CheckCorrectSorting(list2);
        }

        private static void CheckCorrectSorting(SortedStringListGeneric<object> list)
        {
            Assert.That(list.GetKey(0), Is.EqualTo("Apple 2"));
            Assert.That(list.GetKey(1), Is.EqualTo("Marble"));
            Assert.That(list.GetKey(2), Is.EqualTo("apple 1"));
            Assert.That(list.GetKey(3), Is.EqualTo("license"));
        }


        /// <summary>
        /// Test that our implementation of scanning subfolders gives the same result as .NET 2.0 implementation.
        ///
        /// If our code works correctly on .NET 2.0 then it should work on .NET 1.1 as well because it doesn't
        /// use any .NET 2.0 specific code. So there is no need to test it on .NET 1.1.
        /// </summary>
        [Test]
        [CppSkipEntity("WORDSCPP-322 - SystemPal.GetWindowsFontsFolder() doesn't work on Linux")]
        public void TestGetFilesFromFolder()
        {
            string folder = TestFxUtil.BuildTestFileName("Rendering");
            List<string> miscUtilResult = FsPathUtil.GetFilesFromFolder(folder, false);
            string[] systemResult = Directory.GetFiles(folder);
            Assert.That(miscUtilResult.Count, Is.EqualTo(systemResult.Length));

            miscUtilResult = FsPathUtil.GetFilesFromFolder(folder, true);
            systemResult = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            Assert.That(miscUtilResult.Count, Is.EqualTo(systemResult.Length));

#if !NETSTANDARD // SystemPal.GetWindowsFontsFolder() does not work on Android device
            folder = SystemPal.GetWindowsFontsFolder();
            miscUtilResult = FsPathUtil.GetFilesFromFolder(folder, true);
            systemResult = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            Assert.That(miscUtilResult.Count, Is.EqualTo(systemResult.Length));
#endif
        }

        /// <summary>
        /// Tests <see cref="StringUtil.TrimEndNonDigits(string)"/> method.
        /// </summary>
        [Test]
        public void TestTrimEndNonDigits()
        {
            Assert.That(StringUtil.TrimEndNonDigits("-1,234.57%"), Is.EqualTo("-1,234.57"));
            Assert.That(StringUtil.TrimEndNonDigits("1,234.57-"), Is.EqualTo("1,234.57"));
        }

        /// <summary>
        /// Tests <see cref="UriUtil.CreateUriSafely"/> method.
        /// </summary>
        [Test]
        public void TestCreateUriSafely()
        {
            Assert.That(UriUtil.CreateUriSafely(@"file://C://temp//test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"file:\\C:\\temp\\test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"file://hostname//temp//test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"file:\\hostname\\temp\\test.doc"), IsNot.EqualTo(null));
            // alexnosk: Currently code doesn't throw for such uri, just counts it as in current dir,
            // because UriKind.RelativeOrAbsolute is used.
            Assert.That(UriUtil.CreateUriSafely(@"test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"./test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"../test.doc"), IsNot.EqualTo(null));
            Assert.That(UriUtil.CreateUriSafely(@"abracadabra"), IsNot.EqualTo(null));
            // This is invalid one.
            Assert.That(UriUtil.CreateUriSafely(@"http://server:port/Identifier"), Is.EqualTo(null));
        }

        /// <summary>
        /// Tests the <see cref="UriUtil.GetExtension"/> method in scenarios where the base URI is absent.
        /// </summary>
        [Test]
        public void TestUriUtilGetExtensionNoBaseUri()
        {
            Assert.That(UriUtil.GetExtension(null, "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "file.ext1.ext2"), Is.EqualTo("ext2"));
            Assert.That(UriUtil.GetExtension(null, ".ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "file"), Is.EqualTo(""));

            Assert.That(UriUtil.GetExtension(null, "folder/file"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, @"folder\file"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "folder.ext/file"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, @"folder.ext\file"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "folder/file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"folder\file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "folder.ext1/file.ext2"), Is.EqualTo("ext2"));
            Assert.That(UriUtil.GetExtension(null, @"folder.ext1\file.ext2"), Is.EqualTo("ext2"));

            Assert.That(UriUtil.GetExtension(null, @"c:\file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"c:\folder\file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"\\server\file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"\\server\folder\file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://folder/file.ext"), Is.EqualTo("ext"));

            // URI with additional parts.
            Assert.That(UriUtil.GetExtension(null, "http://file.ext?query=value"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://file.ext?query=value&query2=value2"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://file.ext?query=value&query2=value2#id"), Is.EqualTo("ext"));

            // There is no such thing as query part in Windows and UNC file paths.
            Assert.That(UriUtil.GetExtension(null, @"c:\file.ext?query=value"), Is.EqualTo("ext?query=value"));
            Assert.That(UriUtil.GetExtension(null, @"\\server\file.ext?query=value"), Is.EqualTo("ext?query=value"));

            // Href contains characters that are not allowed in Windows file paths.
            Assert.That(UriUtil.GetExtension(null, "?"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "?file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"c:\file*name.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, @"c:\file?name.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://example.com/?query|value;"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "http://example.com/file.ext?query|value;"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http://example.com/file.ext?query=value"), Is.EqualTo("ext"));

            // Some extreme cases.
            Assert.That(UriUtil.GetExtension(null, "http:?query=value"), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "http:.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "http:."), Is.EqualTo(""));
            Assert.That(UriUtil.GetExtension(null, "ht.tp:"), Is.EqualTo("tp:"));
            Assert.That(UriUtil.GetExtension(null, "ht?.tp:"), Is.EqualTo("tp:"));
            Assert.That(UriUtil.GetExtension(null, "ht?tp:file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(null, "ht.ext?tp://file.ext2"), Is.EqualTo("ext2"));
        }

        /// <summary>
        /// Tests the <see cref="UriUtil.GetExtension"/> method in scenarios where the base URI is known.
        /// </summary>
        [Test]
        public void TestUriUtilGetExtensionWithBaseUri()
        {
            Assert.That(UriUtil.GetExtension("", "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension("folder", "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(@"c:\folder", "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension(@"\\server\", "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension("http://example.com", "file.ext"), Is.EqualTo("ext"));
            Assert.That(UriUtil.GetExtension("folder", "file.ext?query=value"), Is.EqualTo("ext?query=value"));
            Assert.That(UriUtil.GetExtension(@"c:\folder", "file.ext?query=value"), Is.EqualTo("ext?query=value"));
            Assert.That(UriUtil.GetExtension(@"\\server\", "file.ext?query=value"), Is.EqualTo("ext?query=value"));
            Assert.That(UriUtil.GetExtension("http://example.com", "file.ext?query=value"), Is.EqualTo("ext"));
        }

        [Test]
        public void TestRemoveSubstring()
        {
            string[] inputs = new string[] { "abcdef defde", "aaaa", "aqaqaqaq", "abababa" };
            string[] subs = new string[] { "def", "aa", "text", "aqa", "a" };
            for (int i = 0; i < inputs.Length; i++)
            {
                string str = inputs[i];
                string substr = subs[i];
                Assert.That(str.Replace(substr, string.Empty), Is.EqualTo(StringUtil.RemoveSubstring(str, substr)));
            }
        }

        /// <summary>
        /// Tests random number generator that is used in the Zip library.
        /// </summary>
        [Test]
        [JavaDelete("Java uses it's own Zip library.")]
        [CppSkipEntity("Cpp uses it's own Zip library.")]
        public void TestZipRandomGenerator()
        {

#if NETSTANDARD
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
            bool has0 = false;
            bool has1 = false;

            for (int i = 0; i < 30; i++)
            {
                int lessThan2 = SharedUtilities.GetRandomInt(2);
                Assert.That(lessThan2 >= 0 && lessThan2 < 2, Is.True);
                has0 = has0 || (lessThan2 == 0);
                has1 = has1 || (lessThan2 == 1);

                int lessThan10 = SharedUtilities.GetRandomInt(10);
                Assert.That(lessThan10 >= 0 && lessThan10 < 10, Is.True);
            }

            Assert.That(has0, Is.True);
            Assert.That(has1, Is.True);
        }

        /// <summary>
        /// Tests the <see cref="StringUtil.Truncate"/> method.
        /// </summary>
        [TestCase(null, 0, null)]
        [TestCase(null, 5, null)]
        [TestCase("", 0, "")]
        [TestCase("", 5, "")]
        [TestCase("abcdefg", 0, "")]
        [TestCase("abcdefg", 5, "abcde")]
        [TestCase("abcdefg", 10, "abcdefg")]
        public void TestTruncate(string value, int lenght, string expectedResult)
        {
            string actualResult = StringUtil.Truncate(value, lenght);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestMatrixDecomposition_ZeroDeterminant()
        {
            DrMatrix matrix = new DrMatrix(2, 0, 0, 0, 0, 0);
            Assert.That(matrix.Determinant, Is.EqualTo(0));
            Assert.That(matrix.HasZeroDeterminant, Is.True);
            // This call must throw InvalidOperationException, because matrixes with zero determinant aren't decomposable.
            float[] coefficients = matrix.DecomposeMatrix();
        }

        [Test]
        public void TestMatrixDecomposition()
        {
            Random random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < 1000000; i++)
                CheckRandomMatrixDecomposition(random);
        }

        /// <summary>
        /// Tests <see cref="UriUtil.FindUri(string,out string)"/>.
        /// </summary>
        [TestCase("", "", "", -1)]
        [TestCase("https://www.text.com/html/convert.php", "https://www.text.com/html/convert.php", "https", 0)]
        [TestCase("http://www.com/a.php\\http://www.com/b.php", "http://www.com/a.php", "http", 0)]
        [TestCase("text http://w.com/a", "http://w.com/a", "http", 5)]
        [TestCase("http://w.com/индекс", "http://w.com/индекс", "http", 0)]
        [TestCase("text:http://w.com/index", "text:http://w.com/index", "text", 0)]
        [TestCase("ht tp://w.com/index", "tp://w.com/index", "tp", 3)]
        [TestCase("text http://w.com/index other text", "http://w.com/index", "http", 5)]
        [TestCase("simple text", "", "", -1)]
        [TestCase("scheme:", "scheme:", "scheme", 0)]
        public void TestFindUri(string s, string expectedUri, string expectedSchemeName, int expectedUriIndex)
        {
            string uri;
            int uriIndex = UriUtil.FindUri(s, out uri);
            Assert.That(uri, Is.EqualTo(expectedUri));
            Assert.That(uriIndex, Is.EqualTo(expectedUriIndex));

            string schemeName;
            uriIndex = UriUtil.FindUriScheme(uri, out schemeName);
            Assert.That(schemeName, Is.EqualTo(expectedSchemeName));
            Assert.That(uriIndex, Is.EqualTo(expectedSchemeName=="" ? -1 : 0));
        }

        /// <summary>
        /// Tests <see cref="UriUtil.FindUri(string,out string)"/> when search not from start.
        /// </summary>
        [Test]
        public void TestFindUriA()
        {
            const string s = "https://www.text.com/html/a.php\\https://www.text.com/html/b.php";
            string uri;
            int uriIndex = UriUtil.FindUri(s, 5, out uri);
            Assert.That(uri, Is.EqualTo("https://www.text.com/html/b.php"));
            Assert.That(uriIndex, Is.EqualTo(32));
        }

        private static void CheckRandomMatrixDecomposition([CodePorting.Translator.Cs2Cpp.CppArgumentKind(ArgumentKind.Reference)] Random random)
        {
            DrMatrix matrix = new DrMatrix();
            matrix.Rotate(GetRandomSign(random) * (float)(random.NextDouble() * 360), MatrixOrder.Append);
            matrix.Scale(GetRandomFloat(random), GetRandomFloat(random), MatrixOrder.Append);
            matrix.Translate(GetRandomFloat(random), GetRandomFloat(random), MatrixOrder.Append);

            // Matrixes that have zero descriminant can't be decomposed. That case is tested elsewhere.
            if (matrix.HasZeroDeterminant)
            {
                return;
            }

            DrMatrix decomposedMatrix = new DrMatrix();
            float[] decomposedMatrixValues = matrix.DecomposeMatrix();
            decomposedMatrix.Scale(decomposedMatrixValues[0], decomposedMatrixValues[1], MatrixOrder.Append);
            decomposedMatrix.Multiply(new DrMatrix(1, decomposedMatrixValues[2], 0, 1, 0, 0), MatrixOrder.Append);
            decomposedMatrix.Rotate(decomposedMatrixValues[3], MatrixOrder.Append);
            decomposedMatrix.Translate(decomposedMatrixValues[4], decomposedMatrixValues[5], MatrixOrder.Append);

            PointF testPoint = new PointF(GetRandomFloat(random), GetRandomFloat(random));

            PointF transfromedPointBySourceMatrix = matrix.TransformPoint(testPoint);
            PointF transfromedPointByDecomposedMatrix = decomposedMatrix.TransformPoint(testPoint);

            //Debug.WriteLine(String.Format("{0} {1} {2}", testPoint.ToString(),
            //    transfromedPointBySourceMatrix.ToString(), transfromedPointByDecomposedMatrix.ToString()));

            Assert.That(transfromedPointByDecomposedMatrix.X, Is.EqualTo(transfromedPointBySourceMatrix.X).Within(1f));
            Assert.That(transfromedPointByDecomposedMatrix.Y, Is.EqualTo(transfromedPointBySourceMatrix.Y).Within(1f));
        }

        private static float GetRandomSign([CodePorting.Translator.Cs2Cpp.CppArgumentKind(ArgumentKind.Reference)] Random random)
        {
            return random.Next() % 2 == 0 ? (1) : (-1);
        }

        private static float GetRandomFloat([CodePorting.Translator.Cs2Cpp.CppArgumentKind(ArgumentKind.Reference)] Random random)
        {
            return (float)(GetRandomSign(random) * random.Next(1000) + random.NextDouble());
        }

        /// <summary>
        /// Tests the <see cref="UriUtil.ConstructAbsoluteUri(string, string)"/> method.
        /// </summary>
        [TestCase("http://example.org", "", "http://example.org/", "")]
        [TestCase("http://example.org/", "", "http://example.org/", "")]
        [TestCase(@"c:\test", "", @"c:\test\", "")]
        [TestCase(@"c:\test\", "", @"c:\test\", "")]
        [TestCase("", "relativePart", "relativePart", "")]
        [TestCase(@"c:\images", "picture.png", @"c:\images\picture.png", "")]
        [TestCase(@"c:\images\", "picture.png", @"c:\images\picture.png", "")]
        [TestCase(@"\\server\", "picture.png", @"\\server\picture.png", "")]
        // This case doesn't work in MS Word or any browser. The base URL must end with a slash in order to work.
        // Not sure if we must support this scenario but it's here for historical reasons.
        [TestCase("http://images", "picture.png", "http://images/picture.png", "")]
        [TestCase("http://images/", "picture.png", "http://images/picture.png", "")]
        // WORDSNET-17894 If relativePart starts with single slash and it has file name then resolve it as "absolute path".
        [TestCase("https://en.wikipedia.org/static/images/project-logos", "/enwiki.png", "https://en.wikipedia.org/enwiki.png", "")]
        [TestCase("https://google.com/foo/bar", "/test/enwiki.png", "https://google.com/test/enwiki.png", "")]
        [TestCase("http://images", "/picture.png", "http://images/picture.png", "")]
        [TestCase("http://images/", "/picture.png", "http://images/picture.png", "")]
        // WORDSNET-17894 If relativePart starts with double slash then resolve it as "network-path reference".
        [TestCase("http://images", "//picture.png", "http://picture.png", "")]
        [TestCase("http://images/", "//picture.png", "http://picture.png", "")]
        [TestCase("http://images/dir1", "//dir2/picture.png", "http://dir2/picture.png", "")]
        [TestCase("https://google.com/foo/bar", "//test/enwiki.png", "https://test/enwiki.png", "")]
        [TestCase("", "//picture.png", "//picture.png", "")]
        // If the relative part is an UNC path then returns relative part only.
        [TestCase("http://images", "\\\\picture.png", "\\\\picture.png", "")]
        // "https://" and "https:" are not valid base URLs but these cases work in IE, Edge, and MS Word.
        [TestCase("https://", "//picture.png", "https://picture.png", "")]
        [TestCase("https:/", "//picture.png", "https://picture.png", "")]
        [TestCase("https:", "//picture.png", "https://picture.png", "")]
        [TestCase("https:/", "/picture.png", "https://picture.png", "")]
        [TestCase("https://", "picture.png", "https://picture.png", "")]
        // We don't correct the following cases and process them as is.
        [TestCase("https:", "picture.png", "https:/picture.png", "")]
        [TestCase("https:/", "picture.png", "https:/picture.png", "")]
        [TestCase("https:", "/picture.png", "https:/picture.png", "")]
        [TestCase("https://", "/picture.png", "https:///picture.png", "")]
        // Not a "network-path reference", because the base URL doesn't start with a scheme. It's a "garbage in, garbage out" scenario.
        [TestCase("c://images", "//picture.png", "c://images//picture.png", "")]
        // URL contains invalid port number. We don't actually parse parts of URSs so we keep invalid values.
        [TestCase("https://google.com:100000/foo/bar", "//example.org/image.png", "https://example.org/image.png", "")]
        [TestCase("https://google.com:100000/foo/bar", "/test/enwiki.png", "https://google.com:100000/test/enwiki.png", "")]
        [TestCase("https://google.com:100000/foo/bar/", "test/enwiki.png", "https://google.com:100000/foo/bar/test/enwiki.png", "")]
        [TestCase("https://google.com:100000/foo/bar", "test/enwiki.png", "https://google.com:100000/foo/bar/test/enwiki.png", "")]
        // URL with query part. We keep percent-encoded characters unchanged.
        [TestCase("https://google.com/foo/bar", "/test/enwiki.png?query=abc%20def", "https://google.com/test/enwiki.png?query=abc%20def", "")]
        // File URI scheme.
        [TestCase("file://", "localhost/c$/WINDOWS/clock.avi", "file://localhost/c$/WINDOWS/clock.avi", "")]
        [TestCase("file://localhost", "etc/fstab", "file://localhost/etc/fstab", "")]
        [TestCase("file:///", "etc/fstab", "file:///etc/fstab", "")]
        [TestCase("file:///", "//localhost/image.png", "file://localhost/image.png", "")]
        // File URI scheme if relative part is an absolute path.
        [TestCase("file:///", "c:/WINDOWS/clock.avi", "c:/WINDOWS/clock.avi", "")]
        // URI with spaces (no pecent-encoding).
        [TestCase("file://locahost/", @"test/some file.png", @"file://locahost/test/some file.png", "")]
        // URIs with backslashes.
        [TestCase("/usr/local", @"foo\bar\image.png", "/usr/local/foo/bar/image.png", "")]
        [TestCase("http://google.com/", @"c:\foo\bar.png", @"c:\foo\bar.png", "")]
        [TestCase("http://google.com/image.png", @"\foo\bar.png", "http://google.com/foo/bar.png", "")]
        // The relative part is not recognized as absolute path in this case, because it starts with a backslash instead of forward slash.
        [TestCase(@"http:\\google.com\", @"\foo/bar.png", @"http:\\google.com\foo\bar.png", "")]
        [TestCase(@"http:\\google.com\", @"/foo/bar.png", @"http:\\google.com\foo\bar.png", "")]
        // More than two slashes after scheme.
        [TestCase("https:////example.com/foo/bar", "//images/picture.png", "https://images/picture.png", "")]
        // No correction for the following cases. We process them as is.
        [TestCase("https:////example.com/foo/bar", "images/picture.png", "https:////example.com/foo/bar/images/picture.png", "")]
        [TestCase("https:////example.com/foo/bar", "/images/picture.png", "https:////example.com/images/picture.png", "")]
        // WORDSNET-22235 Absolute Uri for Linux path was constructed incorrectly. The fix works only on Unix.
        // So the test case expects different results on Windows and on Unix.
        [TestCase("/tmp/generateWordFromHtml", "/tmp/generateWordFromHtml/images/image0.png", "/tmp/generateWordFromHtml/tmp/generateWordFromHtml/images/image0.png",
            "/tmp/generateWordFromHtml/images/image0.png")]
        public void TestConstructAbsoluteUri(string baseUrl, string relativePart, string expectedWindows, string expectedUnix)
        {
            string expected = (StringUtil.HasChars(expectedUnix) && PlatformUtilPal.IsUnixLike()) ? expectedUnix : expectedWindows;

            Assert.That(UriUtil.ConstructAbsoluteUri(baseUrl, relativePart), Is.EqualTo(expected));
        }
    }
}
