// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2018 by Konstantin Sidorenko

using System;
using NUnit.Framework;
using Org.BouncyCastle.Math;

namespace Aspose.JavaMs.Tests.BouncyCastle
{
    [TestFixture]
    public class TestBigInteger
    {
        [Test]
        public void TestToUnsignedArray()
        {
            // 127 - 7 bits
            BigInteger b = new BigInteger("127");
            byte[] result = b.ToByteArrayUnsigned();

            Assert.That(1, Is.EqualTo(result.Length));
            Assert.That((byte)127, Is.EqualTo((byte)result[0]));

            // 255 - 8 bits
            b = new BigInteger("255");
            result = b.ToByteArrayUnsigned();

            Assert.That(1, Is.EqualTo(result.Length));
            Assert.That((byte)255, Is.EqualTo((byte)result[0]));

            // 511 - 9 bits
            b = new BigInteger("511");
            result = b.ToByteArrayUnsigned();

            Assert.That(2, Is.EqualTo(result.Length));
            Assert.That((byte)1, Is.EqualTo((byte)result[0]));
            Assert.That((byte)255, Is.EqualTo((byte)result[1]));
        }

        [Test]
        public void TestToUnsignedArrayNegative()
        {
            // 127 - 8 bits
            BigInteger b = new BigInteger("-127");
            byte[] result = b.ToByteArrayUnsigned();

            Assert.That(1, Is.EqualTo(result.Length));
            Assert.That((byte)129, Is.EqualTo((byte)result[0]));

            // 255 - 9 bits
            b = new BigInteger("-255");
            result = b.ToByteArrayUnsigned();

            Assert.That(2, Is.EqualTo(result.Length));
            Assert.That((byte)255, Is.EqualTo((byte)result[0]));
            Assert.That((byte)1, Is.EqualTo((byte)result[1]));
        }

        [Test]
        public void TestJiraJ1709()
        {
            BigInteger b = new BigInteger("96981933666541832967299512474322211898636188637190044990075663952721645838174035096765125560945848897035959112600126083370821638355193232683135101141544464166366450262604502324532818487998065203497617865813508716036838981643932262115143013314574725842132061901795478937829910612557843981167517029302449028359");
            byte[] result = b.ToByteArrayUnsigned();

            Assert.That(128, Is.EqualTo(result.Length));
            Assert.That("ihtbCMhh+ewGvzrzAkK7rAz/+e0rxmDwaSIjdDCVN51bU/qzwA6QtGTM0wQ3eOBybIeKBqI8psjausYssigHWZfgMIivPcoUTjg2HhhFlozFpjYGUGVHV6M8p6cB78l9yQqgR36yxq/9XIGfkw/SP1bUKs3YqIc1LFBzvIdNPQc=", Is.EqualTo(Convert.ToBase64String(result)));
        }
    }
}

