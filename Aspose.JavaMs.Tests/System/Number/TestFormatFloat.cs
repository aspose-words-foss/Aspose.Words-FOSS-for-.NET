// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/02/2019 by Vyacheslav Durin

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatFloat
    {
        [SetUp]
        public void SetStandardCulture()
        {
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void RevertToOldCulture()
        {
            SystemPal.RestoreCulture();
        }

        [Test]
        public void TestUnexpectedZeros()
        {
            float val = 123.5f;
            string pattern = "0#0,##0.##0";
            Assert.That(val.ToString(pattern), Is.EqualTo("000,123.500"));
        }

        [Test]
        public void TestFormatWithMixedZeroPositions()
        {
            string pattern = "[$-409]#,##0.0";
            float[] input = 
            {
                1f, 2f, 3f, 4f,
                -1.3f, -9.4f, -8.3f,
                -7.9f, -0.9f, -8.1f, -10.2f,
                -6.6f, -0.1f, -8.8f, -1.6f
            };

            string[] results = 
            {
                "[$-409]0,001.0", "[$-409]0,002.0", "[$-409]0,003.0", "[$-409]0,004.0",
                "-[$-409]0,001.3", "-[$-409]0,009.4", "-[$-409]0,008.3", "-[$-409]0,007.9",
                "-[$-409]0,000.9", "-[$-409]0,008.1", "-[$-409]0,010.2", "-[$-409]0,006.6",
                "-[$-409]0,000.1", "-[$-409]0,008.8", "-[$-409]0,001.6",
            };

            for (int i = 0; i < input.Length; i++)
                Assert.That(input[i].ToString(pattern), Is.EqualTo(results[i]));
        }
    }
}
