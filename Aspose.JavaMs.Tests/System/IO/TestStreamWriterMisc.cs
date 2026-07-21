// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2008 by Konstantin Sidorenko
// 2016/06/23 by Anatoliy Sidorenko

using System;
using System.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    /// <summary>
    /// Tests adapted from Rotor from files:
    /// \Rotor\sscli20\tests\bcl\system\io\streamWriter\:
    /// co5591set_autoFlush.cs.
    /// </summary>
    [TestFixture]
    public class TestStreamWriterMisc
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            if (!Directory.Exists(GetTestDir(typeof(TestStreamWriterMisc))))
                Directory.CreateDirectory(GetTestDir(typeof(TestStreamWriterMisc)));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(GetTestDir(typeof(TestStreamWriterMisc)), true);
        }

        [Test]
        public void TestStreamWriterAutoFlush()
        {
            StreamWriter sw2 = new StreamWriter(new MemoryStream());
            sw2.AutoFlush = true;
            Assert.That(true, Is.EqualTo(sw2.AutoFlush));
            sw2.Close();

            sw2 = new StreamWriter(new MemoryStream());
            sw2.AutoFlush = false;
            Assert.That(false, Is.EqualTo(sw2.AutoFlush));
            sw2.Close();
        }

        //JAVA: .net throws ObjectDisposedException here, but i think IllegalStateException is more appropriate.
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestStreamWriterClose1()
        {
            StreamWriter sw2;
            sw2 = new StreamWriter(new MemoryStream());
            sw2.Close();
            sw2.Write('A');
        }

        //JAVA: .net throws ObjectDisposedException here, but i think IllegalStateException is more appropriate.
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestStreamWriterClose2()
        {
            StreamWriter sw2;
            sw2 = new StreamWriter(new MemoryStream());
            sw2.Close();
            sw2.WriteLine("hello");
        }

        //JAVA: .net throws ObjectDisposedException here, but i think IllegalStateException is more appropriate.
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestStreamWriterClose3()
        {
            StreamWriter sw2;
            sw2 = new StreamWriter(new MemoryStream());
            sw2.Close();
            sw2.Flush();
        }

        [Test]
        public void TestStreamWriterClose4()
        {
            StreamWriter sw2;
            sw2 = new StreamWriter(new MemoryStream());
            sw2.Close();
            Assert.That(sw2.BaseStream, Is.Null);
        }

        //JAVA: .net throws ObjectDisposedException here, but i think IllegalStateException is more appropriate.
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TestStreamWriterClose5()
        {
            StreamWriter sw2;
            sw2 = new StreamWriter(new MemoryStream());
            sw2.Close();
            sw2.AutoFlush = true;
        }

        [Test]
        public void TestStreamWriterClose6()
        {
            // Use TestOut directory for creating test directories to avoid UnautorizedAccessException when run with NUnit3.
            string strFileName = Path.Combine(GetTestDir(GetType()), "Co5595Test.tmp");
            File.Delete(strFileName);
            StreamWriter sw2 = new StreamWriter(strFileName);
            sw2.Write("hello");
            sw2.Close();
            StreamReader sr2 = new StreamReader(strFileName);
            string tmp = sr2.ReadToEnd();
            Assert.That("hello", Is.EqualTo(tmp));
            sr2.Close();
            File.Delete(strFileName);
        }

        [Test]
        public void TestStreamWriterFlush()
        {
            StreamWriter sw2;
            MemoryStream memstr2 = new MemoryStream();
            sw2 = new StreamWriter(memstr2);
            string strTemp = "HelloWorld";
            sw2.Write(strTemp);
            Assert.That(0, Is.EqualTo(memstr2.Length));

            sw2.Flush();
            Assert.That(strTemp.Length, Is.EqualTo(memstr2.Length));
            sw2.Close();
        }

        public static string  GetTestDir(Type testClass)
        {
            // Use TestOut directory for creating test directories to avoid UnautorizedAccessException when run with NUnit3.
            return TestFxUtil.BuildOutFileName(testClass.Name, "", "");
        }
    }
}
