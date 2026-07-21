// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2011 by Konstantin Sidorenko
// 2016/01/22 by Anatoliy Sidorenko

using System;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestUri
    {
        [Test]
        [JavaThrows(true)]
        public void TestSlashes()
        {
            // .Net normalizes forward slash '/' to backward one, java doesn't.
            string expected =
#if JAVA
                "C:/Test.docx";
#else
                "C:\\Test.docx";
#endif
            Assert.That(expected, Is.EqualTo(new Uri("C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("C:\\Test.docx").LocalPath));

            // WORDSNET-785
            Assert.That(expected, Is.EqualTo(new Uri("   file:C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("   file:/C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("   file://C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("   file:///C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("   file:////////C:/Test.docx").LocalPath));

            // .Net accepts only two slashes after 'http:' protocol prefix, Java - any number:
#if JAVA
            Assert.That("http://localhost", Is.EqualTo(new Uri("   http:localhost").AbsoluteUri));
            Assert.That("http://localhost", Is.EqualTo(new Uri("   http:/localhost").AbsoluteUri));
            Assert.That("http://localhost", Is.EqualTo(new Uri("   http://localhost").AbsoluteUri));
            Assert.That("http://localhost", Is.EqualTo(new Uri("   http:///localhost").AbsoluteUri));
            Assert.That("http://localhost", Is.EqualTo(new Uri("   http:////////localhost").AbsoluteUri));
#else
            Assert.That("http://localhost/", Is.EqualTo(new Uri("   http://localhost").AbsoluteUri));
#endif

            // Underscores in hostname
            // The same thing with arbitrary number of slashes in Java:
#if JAVA
            Assert.That("http://local_host_", Is.EqualTo(new Uri("   http:local_host_").AbsoluteUri));
            Assert.That("http://local_host_", Is.EqualTo(new Uri("   http:/local_host_").AbsoluteUri));
            Assert.That("http://local_host_", Is.EqualTo(new Uri("   http://local_host_").AbsoluteUri));
            Assert.That("http://local_host_", Is.EqualTo(new Uri("   http:///local_host_").AbsoluteUri));
            Assert.That("http://local_host_", Is.EqualTo(new Uri("   http:////////local_host_").AbsoluteUri));
#else
            Assert.That("http://local_host_/", Is.EqualTo(new Uri("   http://locAl_host_").AbsoluteUri));
            Assert.That("http://local_host_/", Is.EqualTo(new Uri("   http://loCal_hOst_").GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped))); 

#endif
        }

        [Test]
        [JavaThrows(true)]
        public void TestSpaces()
        {
#if JAVA
            Assert.That("C:/Test with spaces.docx", Is.EqualTo(new Uri("C:/Test with spaces.docx").LocalPath));
            Assert.That("C:/Test with spaces.docx", Is.EqualTo(new Uri("C:/Test%20with%20spaces.docx").LocalPath));
#else
            Assert.That("C:\\Test with spaces.docx", Is.EqualTo(new Uri("C:/Test with spaces.docx").LocalPath));
            Assert.That("C:\\Test%20with%20spaces.docx", Is.EqualTo(new Uri("C:/Test%20with%20spaces.docx").LocalPath));
#endif
            //.Net tolerates spaces just before separator: "dir /file.txt".
            Uri uri = new Uri("file:///X:/awnet/TestData/Fields/LinksAndReferences /Hahaha.docx");
            Assert.That(uri.IsAbsoluteUri, Is.True); //java used to throw here before fix.
        }

        [Test]
        [JavaThrows(true)]
        public void TestScheme()
        {
            string expected =
#if JAVA
                "C:/Test.docx";
#else
                "C:\\Test.docx";
#endif
            Assert.That(expected, Is.EqualTo(new Uri("file:/C:/Test.docx").LocalPath));
            Assert.That(expected, Is.EqualTo(new Uri("file:///C:/Test.docx").LocalPath));
        }

        [Test]
        [JavaThrows(true)]
        public void TestI18N()
        {
#if JAVA
            Assert.That("C:/русский.txt", Is.EqualTo(new Uri("C:/русский.txt").LocalPath));
#else
            Assert.That("C:\\русский.txt", Is.EqualTo(new Uri("C:/русский.txt").LocalPath));
#endif
        }

    /**
    * See http://en.wikipedia.org/wiki/File_URI_scheme about number of slashes in URI.
    *
    * Short resume about slashes in Uri:
    * 1. 2 slashes: Basic Uri like 'file://host/c:/test.pdf' or 'file://host/etc/test.pdf'.
    * 2. 3 slashes: Special case when host is localhost and it is omitted: 'file:///c:/test.pdf' or 'file:///etc/test.pdf'.
    * 3. 4 slashes - "legacy" URLs. Should be converted to 2 slashes.
    * 4. 0, 1, 4+ slashes - errors that can exist in real files. Should be converted to 2 slashes.
    *
    * So.
    * 1. 2 slashes are leaved as is.
    * 2. file scheme + (dos-like path or 3 slashes) => 3 slashes (implicit localhost).
    * 3. all other => 2 slashes.
    *
    * Note: .Net Uri throws on 0 or 1 slash - we don't (as some browsers). It works (i believe) for uri with scheme
    * but result is undefined for absolute Uri without scheme (like '/HOST/c:/test.pdf') - it will be treated as
    * relative local Uri i.e. something like 'X:/HOST/c:/test.pdf' - so I can't even include the case in tests.
    */

        [Test]
        [JavaThrows(true)]
        public void TestLocalHostSlashes()
        {
#if JAVA
            TestLocalHostSlashesInternal("host", "/Users/Public/aspose exchange.txt");
            TestLocalHostSlashesInternal("host_1", "/Users/Public/aspose exchange.txt");
#else
            TestLocalHostSlashesInternal("host", "\\Users\\Public\\aspose exchange.txt");
            TestLocalHostSlashesInternal("host_1", "\\Users\\Public\\aspose exchange.txt");
#endif
            // WORDSJAVA-840 AWJ under linux doesn't read localhost image file with 3 slashes: "file:///tmp/documentum/09.png"
            Assert.That("/tmp/documentum/09.png", Is.EqualTo(new Uri("file:///tmp/documentum/09.png").LocalPath));
            // Detailed results for different platforms are presented in http://auckland.aspose.com/jira/browse/WORDSJAVA-1038
        }

        [JavaThrows(true)]
        private void TestLocalHostSlashesInternal(String host, String path)
        {
#if JAVA
            string path1 = "//" + host + path;
            string path2 = "/" + host + path;
#else
            string path1 = "\\\\" + host + path;
            string path2 = "/" + host + "/Users/Public/aspose exchange.txt";
#endif


            Assert.That(path1, Is.EqualTo(new Uri("//" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("///" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("////" + host + @"\Users\Public\aspose exchange.txt").LocalPath));

            Assert.That(path1, Is.EqualTo(new Uri("\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("\\\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("\\\\\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));

#if JAVA
            Assert.That(path1, Is.EqualTo(new Uri("file:" + host + "\\Users\\Public\\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\" + host + "\\Users\\Public\\aspose exchange.txt").LocalPath));
#endif
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path2, Is.EqualTo(new Uri("file:\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\\\\\\\" + host + @"\Users\Public\aspose exchange.txt").LocalPath));

#if JAVA
            Assert.That(path1, Is.EqualTo(new Uri("file:" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\" + host + path).LocalPath));
#endif
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\" + host + path).LocalPath));
            Assert.That(path2, Is.EqualTo(new Uri("file:\\\\\\" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\\\\\" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:\\\\\\\\\\" + host + path).LocalPath));

#if JAVA
            Assert.That(path1, Is.EqualTo(new Uri("file:" + host + "\\Users\\Public\\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:/" + host + "\\Users\\Public\\aspose exchange.txt").LocalPath));
#endif
            Assert.That(path1, Is.EqualTo(new Uri("file://" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path2, Is.EqualTo(new Uri("file:///" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:////" + host + @"\Users\Public\aspose exchange.txt").LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file://///" + host + @"\Users\Public\aspose exchange.txt").LocalPath));

#if JAVA
            Assert.That(path1, Is.EqualTo(new Uri("file:" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:/" + host + path).LocalPath));
#endif
            Assert.That(path1, Is.EqualTo(new Uri("file://" + host + path).LocalPath));
            Assert.That(path2, Is.EqualTo(new Uri("file:///" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file:////" + host + path).LocalPath));
            Assert.That(path1, Is.EqualTo(new Uri("file://///" + host + path).LocalPath));
        }

        [Test]
        [JavaThrows(true)]
        public void TestNet()
        {
            Assert.That("ftp://ftp.dynabic.com/Test.docx", Is.EqualTo(new Uri("ftp://ftp.dynabic.com/Test.docx").AbsoluteUri));
        }

        [Test]
        [JavaThrows(true)]
        public void TestMailTo()
        {
            Uri uri = new Uri("mailto:info@jsvest.net");
            Assert.That(uri.IsAbsoluteUri, Is.True);
            Assert.That("mailto:info@jsvest.net", Is.EqualTo(uri.AbsoluteUri));
        }

        [Test]
        [JavaThrows(true)]
        public void TestJiraJ1802()
        {
            string s1 = @"X:\awnet\TestData\AUT 70015.jpg";
            string s2 = @"X:\awnet\TestData\AUT% 70015.jpg";
            string s3 = @"X:\awnet\TestData\AUT%2070015.jpg";

            Assert.That(new Uri(s1).ToString(), Is.EqualTo(@"file:///X:/awnet/TestData/AUT 70015.jpg"));
            Assert.That(new Uri(s1).AbsoluteUri, Is.EqualTo(@"file:///X:/awnet/TestData/AUT%2070015.jpg"));

            Assert.That(new Uri(s2).ToString(), Is.EqualTo(@"file:///X:/awnet/TestData/AUT%25 70015.jpg"));
            Assert.That(new Uri(s2).AbsoluteUri, Is.EqualTo(@"file:///X:/awnet/TestData/AUT%25%2070015.jpg"));

            Assert.That(new Uri(s3).ToString(), Is.EqualTo(@"file:///X:/awnet/TestData/AUT%252070015.jpg"));
            Assert.That(new Uri(s3).AbsoluteUri, Is.EqualTo(@"file:///X:/awnet/TestData/AUT%252070015.jpg"));


            string http1 = "http://test.com/?a=a b";
            string http2 = "http://test.com/?a=a%20b";

            Assert.That(new Uri(http1).ToString(), Is.EqualTo(@"http://test.com/?a=a b"));
            Assert.That(new Uri(http1).AbsoluteUri, Is.EqualTo(@"http://test.com/?a=a%20b"));

            Assert.That(new Uri(http2).ToString(), Is.EqualTo(@"http://test.com/?a=a b"));
            Assert.That(new Uri(http2).AbsoluteUri, Is.EqualTo(@"http://test.com/?a=a%20b"));
        }
    }
}
