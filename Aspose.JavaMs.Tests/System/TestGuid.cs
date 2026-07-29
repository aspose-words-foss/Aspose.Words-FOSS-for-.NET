// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/01/2007 by Konstantin Sidorenko
// 2015/12/31 by Anatoliy Sidorenko

using System;
using System.Collections.Generic;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestGuid
    {
        [Test]
        public void TestNewGuid()
        {
            Guid guid;
            Guid guid1;
            Guid guid2;

            guid1 = Guid.NewGuid();
            guid2 = Guid.NewGuid();
            Assert.That(guid1.Equals(guid2), Is.False);

            List<Guid> table = new List<Guid>();
            for (int i = 0; i < 1000; i++)
            {
                guid = Guid.NewGuid();
                table.Add(guid);
            }

            for (int i = 0; i < 100; i++)
            {
                guid1 = Guid.NewGuid();
                guid2 = new Guid(guid1.ToByteArray());
                Assert.That(guid1.Equals(guid2), Is.True);
            }
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void TestCtorByteArr1()
        {
            byte[] bytes = null;
            Guid guid = new Guid(bytes);
            Assert.That(guid, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void TestCtorByteArr2()
        {
            byte[] bytes = new byte[0];
            Guid guid = new Guid(bytes);
            Assert.That(guid, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void TestCtorByteArr3()
        {
            byte[] bytes = new byte[177];
            Guid guid = new Guid(bytes);
            Assert.That(guid, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test]
        public void TestCtorByteArr4()
        {
            byte[] bytes = new byte[16];
            Guid guid = new Guid(bytes);
            Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(guid.ToString()));
        }

        [Test]
        public void TestCtorByteArr5()
        {
            byte[] bytes = new byte[16];
            Random random = new Random(DateTime.Now.Millisecond);
            random.NextBytes(bytes);

            Guid guid = new Guid(bytes);
            byte[] bytResultArr = guid.ToByteArray();

            Assert.That(bytResultArr, Is.EqualTo(bytes));
        }

        [Test]
        public void TestCtorArr1()
        {
            int int1 = 1;
            short short2 = 2;
            short short3 = 3;

            byte[] bytes = new byte[8];
            bytes[0] = (byte)0;
            bytes[1] = (byte)1;
            bytes[2] = (byte)2;
            bytes[3] = (byte)3;
            bytes[4] = (byte)4;
            bytes[5] = (byte)5;
            bytes[6] = (byte)6;
            bytes[7] = (byte)7;

            Guid guid = new Guid(int1, short2, short3, bytes);
            string str = guid.ToString();
            Assert.That("00000001-0002-0003-0001-020304050607", Is.EqualTo(str));

            int1 = -1;
            short2 = -2;
            short3 = -3;
            bytes[0] = (byte)-0;
            bytes[1] = (byte)255;
            bytes[2] = (byte)254;
            bytes[3] = (byte)253;
            bytes[4] = (byte)252;
            bytes[5] = (byte)251;
            bytes[6] = (byte)250;
            bytes[7] = (byte)249;
            guid = new Guid(int1, short2, short3, bytes);
            str = guid.ToString();
            Assert.That("ffffffff-fffe-fffd-00ff-fefdfcfbfaf9", Is.EqualTo(str));
        }

        [Test]
        public void TestCtorDegaultDupl()
        {
            const string strGuid2 = "123456Ab-1629-11d2-8879-00c04fb990b0";

            Guid guid2 = Guid.NewGuid();
            Guid guid3 = new Guid(guid2.ToString());
            Assert.That(guid3, Is.EqualTo(guid2));
            Assert.That(guid3.GetHashCode(), Is.EqualTo(guid2.GetHashCode()));

            guid2 = new Guid(strGuid2);
            guid3 = new Guid(strGuid2);
            Assert.That(guid3, Is.EqualTo(guid2));
            Assert.That(StringUtil.EqualsIgnoreCase(strGuid2, guid2.ToString()), Is.True);
        }

        [Test]
        public void TestCtorISSUUU()
        {
            Random rand = new Random(new DateTime().Millisecond);

            //the "main" ctor: (int, short, short, unsigned byte, unsigned byte, ...)
            Guid guid = new Guid(10, (short)11, (short)12, (byte)0, (byte)1, (byte)2, (byte)3, (byte)4, (byte)5, (byte)6, (byte)7);
            Assert.That("0000000a-000b-000c-0001-020304050607", Is.EqualTo(guid.ToString()));
            //convinient ctor without castings.

            guid = new Guid(10, 11, 12, (int)0, (int)1, (int)2, (int)3, (int)4, (int)5, (int)6, (int)7);
            Assert.That("0000000a-000b-000c-0001-020304050607", Is.EqualTo(guid.ToString()));
            
            guid = new Guid(0, 0, 0, (int)0, (int)0, (int)0, (int)0, (int)0, (int)0, (int)0, (int)0);
            Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(guid.ToString()));

            int a;
            short b, c;
            byte d, e, f, g, h, i, j, k;
            for (int ii = 0; ii < 8000; ii++)
            {
                a = rand.Next(int.MaxValue);
                b = (short)rand.Next(int.MaxValue);
                c = (short)rand.Next(int.MaxValue);
                d = (byte)rand.Next(byte.MaxValue);
                e = (byte)rand.Next(byte.MaxValue);
                f = (byte)rand.Next(byte.MaxValue);
                g = (byte)rand.Next(byte.MaxValue);
                h = (byte)rand.Next(byte.MaxValue);
                i = (byte)rand.Next(byte.MaxValue);
                j = (byte)rand.Next(byte.MaxValue);
                k = (byte)rand.Next(byte.MaxValue);
                guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            }
        }

        [Test]
        public void TestCtorUInt32UInt16Etc()
        {
            Random rand = new Random(new DateTime().Millisecond);
            Guid gTest;

            gTest = new Guid((uint)0xa, (ushort)0xb, (ushort)0xc, (byte)0, (byte)1, (byte)2, (byte)3, (byte)4, (byte)5, (byte)6, (byte)7);
            Assert.That("0000000a-000b-000c-0001-020304050607", Is.EqualTo(gTest.ToString()));

            gTest = new Guid((uint)0, (ushort)0, (ushort)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0);
            Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(gTest.ToString()));

            byte byteMaxValue = 0xff;// hardcoded byte.MaxValue for java.
            gTest = new Guid(uint.MaxValue, ushort.MaxValue, ushort.MaxValue, byteMaxValue, byteMaxValue, byteMaxValue, byteMaxValue, byteMaxValue, byteMaxValue, byteMaxValue, byteMaxValue);
            Assert.That("ffffffff-ffff-ffff-ffff-ffffffffffff", Is.EqualTo(gTest.ToString()));

            long a;
            int b, c;
            short d, e, f, g, h, i, j, k;
            for (int count = 0; count < 1000; count++)
            {
                a = ((long)(rand.Next(int.MaxValue)) * 2);
                b = (int)rand.Next(int.MaxValue);
                c = (int)rand.Next(int.MaxValue);
                d = (short)rand.Next(byteMaxValue);
                e = (short)rand.Next(byteMaxValue);
                f = (short)rand.Next(byteMaxValue);
                g = (short)rand.Next(byteMaxValue);
                h = (short)rand.Next(byteMaxValue);
                i = (short)rand.Next(byteMaxValue);
                j = (short)rand.Next(byteMaxValue);
                k = (short)rand.Next(byteMaxValue);
                gTest = new Guid((uint)a, (ushort)b, (ushort)c, (byte)d, (byte)e, (byte)f, (byte)g, (byte)h, (byte)i, (byte)j, (byte)k);
                string expected = FormatterPal.Int64ToStrX8(a) + "-" + 
                                  FormatterPal.IntToStrX8(b).Substring(4) + "-" + 
                                  FormatterPal.IntToStrX8(c).Substring(4) + "-" +
                                  FormatterPal.IntToStrX8(d).Substring(6) +
                                  FormatterPal.IntToStrX8(e).Substring(6) + "-" +
                                  FormatterPal.IntToStrX8(f).Substring(6) +
                                  FormatterPal.IntToStrX8(g).Substring(6) +
                                  FormatterPal.IntToStrX8(h).Substring(6) +
                                  FormatterPal.IntToStrX8(i).Substring(6) +
                                  FormatterPal.IntToStrX8(j).Substring(6) +
                                  FormatterPal.IntToStrX8(k).Substring(6);
                Assert.That(expected.ToLower(), Is.EqualTo(gTest.ToString()));
            }
        }

        [Test]
        public void TestCtorString()
        {
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = new Guid(guid2.ToString());
            Assert.That(guid3, Is.EqualTo(guid2));
            Assert.That(guid3.GetHashCode(), Is.EqualTo(guid2.GetHashCode()));

            const string strGuid2 = "123456Ab-1629-11d2-8879-00c04fb990b0";
            guid2 = new Guid(strGuid2);
            guid3 = new Guid(strGuid2);
            Assert.That(guid3, Is.EqualTo(guid2));
            Assert.That(guid3.GetHashCode(), Is.EqualTo(guid2.GetHashCode()));
            Assert.That(strGuid2.ToLower(), Is.EqualTo(guid2.ToString()));
            Assert.That(0x41D47C9, Is.EqualTo(guid2.GetHashCode()));
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString2()
        {
            Guid guid2 = new Guid("123456Ab 1629-11d2-8879-00c04fb990b0");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString3()
        {
            Guid guid2 = new Guid("123456A-1629-11d2-8879-00c04fb990b0");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString4()
        {
            Guid guid2 = new Guid("123456Az-1629-11d2-8879-00c04fb990b0");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString5()
        {
            Guid guid2 = new Guid("123456123445");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString6()
        {
            Guid guid2 = new Guid("{00000000-0000-0000-0000-000000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString7()
        {
            Guid guid2 = new Guid("{00000000-0000-0000-0000-000000000000}0");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString8()
        {
            Guid guid2 = new Guid("00000000-0000-0000-0000-0000000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString9()
        {
            Guid guid2 = new Guid("0000000-00000-0000-0000-000000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString10()
        {
            Guid guid2 = new Guid("00000000-00000-000-0000-000000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString11()
        {
            Guid guid2 = new Guid("00000000-0000-00000-000-000000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test, ExpectedException(typeof (FormatException))]
        public void TestCtorString12()
        {
            Guid guid2 = new Guid("00000000-0000-0000-0000-00L000000000");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        //JAVA: other string formats do not supported yet:
        [Test]
#if JAVA
        [ExpectedException(typeof(FormatException))]
#endif
        public void TestCtorString13()
        {
            Guid guid2 = new Guid("{0x11111111,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x11}}");
            Assert.That(guid2, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test]
        // TODO: What does this test do?
        public void TestCtorStringStat()
        {
            Guid guid2;
            Guid guid3;

            for (int i = 0; i < 100; i++)
            {
                guid2 = Guid.NewGuid();
                guid3 = new Guid(guid2.ToString());
            }
        }

        [Test]
        public void TestToString1()
        {
            Guid guid = new Guid("a4434021-80db-4b65-aadd-f138e53e0e07");
            string[] formats = {"d", "D", "n", "N", "p", "P", "b", "B"};

            foreach (string format in formats)
            {
                string value = guid.ToString(format);
                switch (format[0])
                {
                    case 'B':
                        Assert.That("{a4434021-80db-4b65-aadd-f138e53e0e07}", Is.EqualTo(value));
                        break;
                    case 'b':
                        Assert.That("{a4434021-80db-4b65-aadd-f138e53e0e07}", Is.EqualTo(value));
                        break;
                    case 'D':
                        Assert.That("a4434021-80db-4b65-aadd-f138e53e0e07", Is.EqualTo(value));
                        break;
                    case 'd':
                        Assert.That("a4434021-80db-4b65-aadd-f138e53e0e07", Is.EqualTo(value));
                        break;
                    case 'N':
                        Assert.That("a443402180db4b65aaddf138e53e0e07", Is.EqualTo(value));
                        break;
                    case 'n':
                        Assert.That("a443402180db4b65aaddf138e53e0e07", Is.EqualTo(value));
                        break;
                    case 'P':
                        Assert.That("(a4434021-80db-4b65-aadd-f138e53e0e07)", Is.EqualTo(value));
                        break;
                    case 'p':
                        Assert.That("(a4434021-80db-4b65-aadd-f138e53e0e07)", Is.EqualTo(value));
                        break;
                    default:
                        break;
                }
            }
        }

        [Test]
        public void TestToString2()
        {
            Guid emptyGuid = Guid.Empty;
            string[] formats = {"d", "D", "n", "N", "p", "P", "b", "B"};

            foreach (string format in formats)
            {
                string guidStr = emptyGuid.ToString(format);
                switch (format[0])
                {
                    case 'B':
                        Assert.That("{00000000-0000-0000-0000-000000000000}", Is.EqualTo(guidStr));
                        break;
                    case 'b':
                        Assert.That("{00000000-0000-0000-0000-000000000000}", Is.EqualTo(guidStr));
                        break;
                    case 'D':
                        Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(guidStr));
                        break;
                    case 'd':
                        Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(guidStr));
                        break;
                    case 'N':
                        Assert.That("00000000000000000000000000000000", Is.EqualTo(guidStr));
                        break;
                    case 'n':
                        Assert.That("00000000000000000000000000000000", Is.EqualTo(guidStr));
                        break;
                    case 'P':
                        Assert.That("(00000000-0000-0000-0000-000000000000)", Is.EqualTo(guidStr));
                        break;
                    case 'p':
                        Assert.That("(00000000-0000-0000-0000-000000000000)", Is.EqualTo(guidStr));
                        break;
                    default:
                        break;
                }
            }
        }

        [Test]
        public void TestToString3()
        {
            Guid guid1 = Guid.Empty;
            string retValue;
            string[] formats = {"d", "D", "n", "N", "p", "P", "b", "B"};

            foreach (string format in formats)
            {
                retValue = guid1.ToString(format);
                switch (format[0])
                {
                    case 'B':
                        Assert.That("{00000000-0000-0000-0000-000000000000}", Is.EqualTo(retValue));
                        break;
                    case 'b':
                        Assert.That("{00000000-0000-0000-0000-000000000000}", Is.EqualTo(retValue));
                        break;
                    case 'D':
                        Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(retValue));
                        break;
                    case 'd':
                        Assert.That("00000000-0000-0000-0000-000000000000", Is.EqualTo(retValue));
                        break;
                    case 'N':
                        Assert.That("00000000000000000000000000000000", Is.EqualTo(retValue));
                        break;
                    case 'n':
                        Assert.That("00000000000000000000000000000000", Is.EqualTo(retValue));
                        break;
                    case 'P':
                        Assert.That("(00000000-0000-0000-0000-000000000000)", Is.EqualTo(retValue));
                        break;
                    case 'p':
                        Assert.That("(00000000-0000-0000-0000-000000000000)", Is.EqualTo(retValue));
                        break;
                    default:
                        break;
                }
            }
        }

        [Test]
        public void TestToString4()
        {
            Guid guid1 = Guid.NewGuid();
            string[] exceptionFormats = { "g", "G", "c", "C", "e", "E", "#", "0", "####.####", "000.000", "%###", "##E+0", "{{"};

            foreach (string exp in exceptionFormats)
            {
                try
                {
                    string retValue = guid1.ToString(exp);
                    Assert.Fail("exp= " + exp + "\r\nretval= " + retValue);
                }
                catch (FormatException)
                {
                    //catched
                }

            }
        }

        [Test]
        public void TestCompareTo2()
        {
            byte[] bytes = new byte[16];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte) i;

            Guid guid1 = new Guid(bytes);
            Guid guid2 = new Guid(bytes);
            Assert.That(0, Is.EqualTo(guid1.CompareTo(guid2)));
            Assert.That(0, Is.EqualTo(guid2.CompareTo(guid1)));
            Assert.That(0, Is.EqualTo(guid1.CompareTo(guid1)));

            bytes = new byte[16];
            for (int i = 0, j = 16; i < bytes.Length; i++, j--)
                bytes[i] = (byte) j;

            guid2 = new Guid(bytes);
            Assert.That(guid1.CompareTo(guid2) < 0, Is.True);
            Assert.That(guid2.CompareTo(guid1) > 0, Is.True);
            Assert.That(guid2.CompareTo(null) > 0, Is.True);
        }
    }
}
