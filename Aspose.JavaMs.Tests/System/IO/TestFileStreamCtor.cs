// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2008 by Konstantin Sidorenko
// 2016/06/21 by Anatoliy Sidorenko

using System;
using System.IO;
using System.Threading;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    /// <summary>
    /// Tests adapted from Rotor from files:
    /// \Rotor\sscli20\tests\bcl\system\io\filestream\Co5575ctor_str_fm.cs
    /// </summary>
    [TestFixture]
    public class TestFileStreamCtor
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            if (!Directory.Exists(GetTestDir(typeof(TestFileStreamCtor))))
                Directory.CreateDirectory(GetTestDir(typeof(TestFileStreamCtor)));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Directory.Delete(GetTestDir(typeof(TestFileStreamCtor)), true);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestFileStreamCtor1()
        {
            new FileStream(null, FileMode.Open);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestFileStreamCtor2()
        {
            new FileStream("", FileMode.Open);
        }

        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void TestFileStreamCtor3()
        {
            new FileStream(".", FileMode.Open);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestFileStreamCtor4()
        {
            new FileStream(GetTestFileName(GetType()), ~FileMode.Open);
        }

        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void TestFileStreamCtor5()
        {
            string  filName = GetTestFileName(GetType());
            File.Delete(filName);
            new FileStream(filName, FileMode.Open);
        }

        [Test]
        public void TestFileStreamCtor6()
        {
            string  filName = GetTestFileName(GetType());
            using (FileStream fs2 = new FileStream(filName, FileMode.Create))
            {
                StreamWriter sw2 = new StreamWriter(fs2);
                sw2.AutoFlush = true;
                for (char c = 'A'; c <= 'Z'; c++)
                    sw2.Write(c);
                fs2.Position = 0;
                StreamReader sr2 = new StreamReader(fs2);
                int tmp;
                for (int c = 'A'; c <= 'Z'; c++)
                {
                    tmp = sr2.Read();
                    Assert.That(c, Is.EqualTo(tmp));
                }
            }

            using (FileStream fs2 = new FileStream(filName, FileMode.Create))
            {
                StreamWriter sw2 = new StreamWriter(fs2);
                sw2.AutoFlush = true;

                StreamReader sr2 = new StreamReader(fs2);
                for (int i = 0; i < 10; i++)
                    sw2.Write(i);
                fs2.Position = 0;
                int tmp;
                int i32 = '0';
                for (int i = 0; i < 10; i++)
                {
                    tmp = sr2.Read();
                    Assert.That(i32++, Is.EqualTo(tmp));
                }
            }

            using (FileStream fs2 = new FileStream(filName, FileMode.Truncate))
            {
                Assert.That(0, Is.EqualTo(fs2.Length));

                StreamReader sr2 = new StreamReader(fs2);
                Assert.That(-1, Is.EqualTo(sr2.Read()));

                StreamWriter sw2 = new StreamWriter(fs2);
                sw2.AutoFlush = true;
                for (char c = 'A'; c < 'K'; c++)
                    sw2.Write(c);
            }
        }

        [Test]
        public void TestFileStreamCtor7()
        {
            string strFileName = Path.Combine(GetTestDir(GetType()), "ETE_LOGFILE.txt");
            using (StreamWriter writer = new StreamWriter(strFileName))
            {
                writer.WriteLine("This is a test file");
                writer.Close();

                FileStream fs = new FileStream(strFileName, FileMode.Append);

                StreamWriter logStream = new StreamWriter(fs);
                string s = "User logged out at " + DateTime.Now;
                logStream.WriteLine(s);
                logStream.Close();

                StreamReader reader = new StreamReader(strFileName);
                string strFileContent = reader.ReadToEnd();
                Assert.That(strFileContent.Contains(s), Is.True);
                reader.Close();

                File.Delete(strFileName);
            }
        }

        [Test, ExpectedException(typeof(IOException))]
        public void TestFileStreamCtor8()
        {
            string filName = GetTestFileName(GetType());
            FileStream fs2 = new FileStream(filName, FileMode.Create);
            fs2.Close();
            new FileStream(filName, FileMode.CreateNew);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestFileStreamCtor9()
        {
            new FileStream(GetTestFileName(GetType()), FileMode.Create | FileMode.Truncate);
        }

        public static string  GetTestFileName(Type testClass)
        {
            return GetTestDir(testClass) + Path.DirectorySeparatorChar + testClass.Name + "." + DateTime.Now.Millisecond + "." + Thread.CurrentThread.ManagedThreadId + ".tmp";
        }

        public static string  GetTestDir(Type testClass)
        {
            // Use TestOut directory for creating test directories to avoid UnautorizedAccessException when run with NUnit3.
            return TestFxUtil.BuildOutFileName(testClass.Name, "", "");
        }
    }
}
