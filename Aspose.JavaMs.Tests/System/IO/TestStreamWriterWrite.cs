// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/08/2008 by Konstantin Sidorenko
// 2016/06/24 by Anatoliy Sidorenko

using System;
using System.IO;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.IO
{
    /// <summary>
    /// Tests adapted from Rotor from files:
    /// \Rotor\sscli20\tests\bcl\system\io\streamWriter\:
    /// co5563Write_ch.cs, co5564Write_charr_ii.cs, co5565Write_str.cs.
    /// </summary>
    [TestFixture]
    public class TestStreamWriterWrite
    {
        [Test]
        public void TestStreamWriterWriteChar1()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr;
            for (int i = 0; i < chArr.Length; i++)
                sw.Write(chArr[i]);
            sw.Flush();
            ms.Position = 0;
            sr = new StreamReader(ms);
            int tmp = 0;
            for (int i = 0; i < chArr.Length; i++)
            {
                tmp = sr.Read();
                Assert.That(chArr[i], Is.EqualTo(tmp));
            }
            sw.Close();
            sr.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestStreamWriterWriteChar2()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(null, 0, 0);
            sw.Close();//never get
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestStreamWriterWriteChar3()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(chArr, -1, 0);
            sw.Close();//never get
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestStreamWriterWriteChar4()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(chArr, 0, -1);
            sw.Close();//never get
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestStreamWriterWriteChar5()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(chArr, 1, chArr.Length);
            sw.Close();//never get
        }

        [Test]
        public void TestStreamWriterWriteChar6()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr;
            sw.Write(chArr, 0, chArr.Length);
            sw.Flush();
            ms.Position = 0;
            sr = new StreamReader(ms);
            int tmp = 0;
            for (int i = 0; i < chArr.Length; i++)
            {
                tmp = sr.Read();
                Assert.That(chArr[i], Is.EqualTo(tmp));
            }
            sw.Close();
            sr.Close();
            ms.Close();
        }

        public void TestStreamWriterWriteChar7()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr;
            sw.Write(chArr, 2, 5);
            sw.Flush();
            ms.Position = 0;
            sr = new StreamReader(ms);
            int tmp = 0;
            for (int i = 2; i < 7; i++)
            {
                tmp = sr.Read();
                Assert.That(chArr[i], Is.EqualTo(tmp));
            }
            sw.Close();
            sr.Close();
            ms.Close();
        }

        [Test]
        public void TestStreamWriterWriteStr1()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write((String)null);
            sw.Flush();
            Assert.That(0, Is.EqualTo(ms.Length));
            sw.Close();
            ms.Close();
        }

        [Test]
        public void TestStreamWriterWriteStr2()
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            String str = new String(chArr);
            sw.Write(str);
            sw.Flush();
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            int tmp = 0;
            for (int i = 0; i < chArr.Length; i++)
            {
                tmp = sr.Read();
                Assert.That(chArr[i], Is.EqualTo(tmp));
            }
            sw.Close();
            sr.Close();
            ms.Close();
        }

        private static char[] chArr = new char[]{
        Char.MinValue
        ,Char.MaxValue
        ,'\t'
        ,' '
        ,'$'
        ,'@'
        ,'#'
        ,'\0'
        ,'\u000B'
        ,'\''
        ,'\u3190'
        ,'\uC3A0'
        ,'A'
        ,'5'
        ,'\uFE70'
        ,'-'
        ,';'
        ,'\u00E6'};

    }
}
