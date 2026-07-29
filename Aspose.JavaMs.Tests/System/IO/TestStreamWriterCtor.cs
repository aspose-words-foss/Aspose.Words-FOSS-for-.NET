// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2008 by Konstantin Sidorenko
// 2016/06/22 by Anatoliy Sidorenko

using System;
using System.IO;
using System.Text;
using System.Threading;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    /// <summary>
    /// Rotor's tests adapted for TestNG.
    /// 
    /// Tests taken form  Rotor\sscli20\tests\bcl\system\io\streamwriter directory.
    /// In Rotor each tests sits in its own file named something like co5560ctor_str.cs.
    /// Each the test code extracted from surounding support code and placed in its own
    /// test methods named like Test_ctor_str1(), Test_ctor_str2(), etc.
    /// //@todo 2 sk test network streams
    /// </summary>
    [TestFixture]
    public class TestStreamWriterCtor
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            if (!Directory.Exists(GetTestDir(typeof(TestStreamWriterCtor))))
                Directory.CreateDirectory(GetTestDir(typeof(TestStreamWriterCtor)));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(GetTestDir(typeof(TestStreamWriterCtor)), true);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_str1()
        {
            StreamWriter sw = new StreamWriter((string)null);
        }

        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void Test_ctor_str2()
        {
            StreamWriter sw = new StreamWriter("..");
        }

        [Test]
        public void Test_ctor_str3()
        {
            string testPath = Path.Combine(GetTestDir(GetType()), "TestFile.tmp");

            using (StreamWriter sw = new StreamWriter(testPath))
            {
                sw.Write(5);
                sw.Flush();
                sw.Close();
                StreamReader sr = new StreamReader(testPath);
                int tmp = sr.Read();
                Assert.That('5', Is.EqualTo(tmp));
                sr.Close();
                File.Delete(testPath);
            }
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_stream_enc1()
        {
            StreamReader sr = new StreamReader((Stream)null, Encoding.UTF8);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_stream_enc2()
        {
            StreamReader sr = new StreamReader(new MemoryStream(), null);
        }

        [Test]
        public void Test_ctor_stream_enc3()
        {
            string fileName = GetTestFileName(GetType());
            FileStream fs2 = new FileStream(fileName, FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.ASCII);
            sw2.Write("HelloThere\u00FF");
            sw2.Close();

            fs2 = new FileStream(fileName, FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2, Encoding.ASCII);
            string str2 = sr2.ReadToEnd();
            Assert.That("HelloThere?", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc4()
        {
            MemoryStream ms2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(ms2, Encoding.ASCII);
            sw2.Write("HelloThere\u00FF");
            sw2.Flush();
            ms2.Position = 0;
            StreamReader sr2 = new StreamReader(ms2, Encoding.ASCII);
            string str2 = sr2.ReadToEnd();
            Assert.That("HelloThere?", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc5()
        {
            string fileName = GetTestFileName(GetType());
            FileStream fs2 = new FileStream(fileName, FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.UTF8);
            sw2.Write("This is UTF8\u00FF");
            sw2.Close();

            fs2 = new FileStream(fileName, FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2, Encoding.UTF8);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is UTF8\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc6()
        {
            MemoryStream ms2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(ms2, Encoding.UTF8);
            sw2.Write("This is UTF8\u00FF");
            sw2.Flush();
            ms2.Position = 0;
            StreamReader sr2 = new StreamReader(ms2, Encoding.UTF8);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is UTF8\u00FF", Is.EqualTo(str2));

            //check bom
            ms2.Position = 0;
            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            ms2.Read(bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc7()
        {
            string fileName = GetTestFileName(GetType());
            FileStream fs2 = new FileStream(fileName, FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.UTF7);
            sw2.Write("This is UTF7\u00FF");
            sw2.Close();

            fs2 = new FileStream(fileName, FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2, Encoding.UTF7);
            string str1 = "This is UTF7\u00FF";
            string str2 = sr2.ReadToEnd();
            Assert.That(str1, Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc8()
        {
            MemoryStream ms2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(ms2, Encoding.UTF7);
            sw2.Write("This is UTF7\u00FF");
            sw2.Flush();
            ms2.Position = 0;

            //check bom - utf7 doesn't Write the bom as in .net
            ms2.Position = 0;
            byte[] preamble = Encoding.UTF7.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            ms2.Read(bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            ms2.Position = 0;
            StreamReader sr2 = new StreamReader(ms2, Encoding.UTF7);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is UTF7\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void TestStreamWriterEncodingUTF7()
        {
            Encoding encoding = Encoding.UTF7;
            MemoryStream stream = new MemoryStream(128);
            StreamWriter sw2 = new StreamWriter(stream, encoding);

            sw2.Write("111aAБ");
            sw2.Flush();

            stream.Position = 0;
            byte[] preamble = encoding.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            stream.Position = 0;
            stream.Read(bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            stream.Position = 0;
            StreamReader Reader = new StreamReader(stream, encoding);
            string declaration = Reader.ReadToEnd();
            Assert.That("111aAБ", Is.EqualTo(declaration));

            stream.Position = 0;
            Reader = new StreamReader(stream, Encoding.ASCII);
            declaration = Reader.ReadToEnd();
            Assert.That("111aA+BBE-", Is.EqualTo(declaration));
            Reader.Close();
        }

        [Test]
        public void Test_ctor_stream_enc9()
        {
            string fileName = GetTestFileName(GetType());
            string strValue = "This is BigEndianUnicode\u00FF";
            FileStream fs2 = new FileStream(fileName, FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.BigEndianUnicode);
            sw2.Write(strValue);
            sw2.Close();

            fs2 = new FileStream(fileName, FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2, Encoding.BigEndianUnicode);
            string str2 = sr2.ReadToEnd();
            Assert.That(strValue, Is.EqualTo(str2));
            sr2.Close();


            MemoryStream ms2 = new MemoryStream();
            sw2 = new StreamWriter(ms2, Encoding.BigEndianUnicode);
            sw2.Write(strValue);
            sw2.Flush();
            ms2.Position = 0;
            sr2 = new StreamReader(ms2, Encoding.BigEndianUnicode);
            str2 = sr2.ReadToEnd();
            Assert.That(strValue, Is.EqualTo(str2));

            //check bom
            ms2.Position = 0;
            byte[] preamble = Encoding.BigEndianUnicode.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            ms2.Read(bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            sr2.Close();
        }

        [Test]
        public void Test_ctor_stream_enc10()
        {
            string fileName = GetTestFileName(GetType());
            string strValue = "This is Unicode\u00FF";
            FileStream fs2 = new FileStream(fileName, FileMode.Create);
            StreamWriter sw2 = new StreamWriter(fs2, Encoding.Unicode);
            sw2.Write(strValue);
            sw2.Close();

            fs2 = new FileStream(fileName, FileMode.Open);
            StreamReader sr2 = new StreamReader(fs2, Encoding.Unicode);
            string str2 = sr2.ReadToEnd();
            Assert.That(strValue, Is.EqualTo(str2));
            sr2.Close();

            MemoryStream ms2 = new MemoryStream();
            sw2 = new StreamWriter(ms2, Encoding.Unicode);
            sw2.Write(strValue);
            sw2.Flush();
            ms2.Position = 0;
            sr2 = new StreamReader(ms2, Encoding.Unicode);
            str2 = sr2.ReadToEnd();
            Assert.That(strValue, Is.EqualTo(str2));

            //check bom
            ms2.Position = 0;
            byte[] preamble = Encoding.Unicode.GetPreamble();
            byte[] bom = new byte[preamble.Length];
            ms2.Read(bom, 0, bom.Length);
            Assert.That(bom, Is.EqualTo(preamble));

            sr2.Close();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_str_b_enc1()
        {
            new StreamWriter(null, false, Encoding.UTF8);
        }

        [Test]
        public void Test_ctor_str_b_enc2()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.ASCII);
            sw2.Write("HelloThere\u00FF");
            sw2.Close();
            StreamReader sr2 = new StreamReader(fileName, Encoding.ASCII);
            string str2 = sr2.ReadToEnd();
            Assert.That("HelloThere?", Is.EqualTo(str2));
            sr2.Close();

            sw2 = new StreamWriter(fileName, true, Encoding.ASCII);
            Assert.That(11, Is.EqualTo(sw2.BaseStream.Length));
            sw2.Close();
        }

        [Test]
        public void Test_ctor_str_b_enc3()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.ASCII);
            Assert.That(0, Is.EqualTo(sw2.BaseStream.Length));
            sw2.Close();
        }

        [Test]
        public void Test_ctor_str_b_enc4()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.UTF8);
            sw2.Write("This is UTF8\u00FF");
            sw2.Close();
            StreamReader sr2 = new StreamReader(fileName, Encoding.UTF8);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is UTF8\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_str_b_enc5()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.UTF7);
            sw2.Write("This is UTF7\u00FF");
            sw2.Close();

            StreamReader sr2 = new StreamReader(fileName, Encoding.UTF7);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is UTF7\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_str_b_enc6()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.BigEndianUnicode);
            sw2.Write("This is BigEndianUnicode\u00FF");
            sw2.Close();
            StreamReader sr2 = new StreamReader(fileName, Encoding.BigEndianUnicode);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is BigEndianUnicode\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test]
        public void Test_ctor_str_b_enc7()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.Unicode);
            sw2.Write("This is Unicode\u00FF");
            sw2.Close();
            StreamReader sr2 = new StreamReader(fileName, Encoding.Unicode);
            string str2 = sr2.ReadToEnd();
            Assert.That("This is Unicode\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_str_b_enc_i1()
        {
            new StreamWriter(null, false, Encoding.UTF8, 1);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_str_b_enc_i2()
        {
            new StreamWriter(GetTestFileName(GetType()), false, null, 1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_ctor_str_b_enc_i3()
        {
            new StreamWriter(GetTestFileName(GetType()), false, Encoding.UTF8, -2);
        }

        [Test]
        public void Test_ctor_str_b_enc_i4()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false, Encoding.ASCII, 9);
            sw2.Write("HelloThere\u00FF");
            sw2.Close();

            StreamReader sr2 = new StreamReader(fileName, Encoding.ASCII);
            string str2 = sr2.ReadToEnd();
            Assert.That("HelloThere?", Is.EqualTo(str2));
            sr2.Close();

            sw2 = new StreamWriter(fileName, false, Encoding.UTF8, 8888);
            sw2.Write("This is UTF8\u00FF");
            sw2.Close();

            sr2 = new StreamReader(fileName, Encoding.UTF8);
            str2 = sr2.ReadToEnd();
            Assert.That("This is UTF8\u00FF", Is.EqualTo(str2));
            sr2.Close();

            sw2 = new StreamWriter(fileName, false, Encoding.UTF7, 11);
            sw2.Write("This is UTF7\u00FF");
            sw2.Close();

            sr2 = new StreamReader(fileName, Encoding.UTF7);
            str2 = sr2.ReadToEnd();
            Assert.That("This is UTF7\u00FF", Is.EqualTo(str2));
            sr2.Close();

            sw2 = new StreamWriter(fileName, false, Encoding.BigEndianUnicode, 20);
            sw2.Write("This is BigEndianUnicode\u00FF");
            sw2.Close();

            sr2 = new StreamReader(fileName, Encoding.BigEndianUnicode);
            str2 = sr2.ReadToEnd();
            Assert.That("This is BigEndianUnicode\u00FF", Is.EqualTo(str2));
            sr2.Close();

            sw2 = new StreamWriter(fileName, false, Encoding.Unicode, 50000);
            sw2.Write("This is Unicode\u00FF");
            sw2.Close();

            sr2 = new StreamReader(fileName, Encoding.Unicode);
            str2 = sr2.ReadToEnd();
            Assert.That("This is Unicode\u00FF", Is.EqualTo(str2));
            sr2.Close();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test_ctor_str_b1()
        {
            new StreamWriter(null, false);
        }

        [Test]
        public void Test_ctor_str_b2()
        {
            string fileName = GetTestFileName(GetType());
            StreamWriter sw2 = new StreamWriter(fileName, false);
            string strValue = "123456";
            sw2.Write(strValue);
            sw2.Flush();
            Assert.That(strValue.Length, Is.EqualTo(sw2.BaseStream.Length));
            sw2.Close();

            sw2 = new StreamWriter(fileName, false);
            strValue = "1234";
            sw2.Write(strValue);
            sw2.Flush();
            Assert.That(strValue.Length, Is.EqualTo(sw2.BaseStream.Length));
            sw2.Close();

            sw2 = new StreamWriter(fileName, true);
            strValue = "ABCDE";
            sw2.Write(strValue);
            sw2.Flush();
            Assert.That((strValue.Length + "ABCD".Length), Is.EqualTo(sw2.BaseStream.Length));
            sw2.Close();

            StreamReader sr2 = new StreamReader(fileName);
            string tmp;
            tmp = sr2.ReadToEnd();
            sr2.Close();
            Assert.That("1234ABCDE", Is.EqualTo(tmp));
        }

        public static string GetTestFileName(Type testClass)
        {
            return GetTestDir(testClass) + Path.DirectorySeparatorChar + testClass.Name + "." +
                   DateTime.Now.Millisecond + "." + Thread.CurrentThread.ManagedThreadId + ".tmp";
        }

        public static string GetTestDir(Type testClass)
        {
            // Use TestOut directory for creating test directories to avoid UnautorizedAccessException when run with NUnit3.
            return TestFxUtil.BuildOutFileName(testClass.Name, "", ""); 
        }
    }
}
