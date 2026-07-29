// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2017 by Anatoliy Sidorenko

using Aspose.Common;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatDouble
    {
        [SetUp]
        public void SetStandardCulture()
        {
            SystemPal.SaveCulture();
            // Parsing of numbers and dates is performed using the current culture and this is by design.
            // So we have to select the standard culture before we run a test in order to pass on all
            // development machines that can have different cultures selected. 
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void RevertToOldCulture()
        {
            SystemPal.RestoreCulture();
        }

        [Test]
        public void TestBasic()
        {
            double doubleValue = 1234567.0;
            // As .Net: formats even doubles as integers.
            Assert.That(doubleValue.ToString(), Is.EqualTo("1234567"));

            doubleValue = 12345678.1;
            // English locale used as default at the moment.
            Assert.That(doubleValue.ToString(), Is.EqualTo("12345678.1"));
        }

        [Test]
        [JavaThrows(true)]
        public void TestEscapesInPatterns()
        {
            double doubleValue = -20.0d;
            // escape '-' and space character
            Assert.That("-20 ", Is.EqualTo(doubleValue.ToString("#,##0;\\-#,##0\\ ")));
            // remove the trailing '\'
            Assert.That("-20", Is.EqualTo(doubleValue.ToString("#,##0;-#,##0\\")));
            // escape '-' and remove the trailing '\'
            doubleValue = 15.0d;
            Assert.That("15", Is.EqualTo(doubleValue.ToString("#,##0;\\-#,##0\\")));
            // escape second '¤' and remove the trailing '\'
            Assert.That("¤15", Is.EqualTo(doubleValue.ToString("\u00A4#,##0;\\\u00A4-#,##0\\")));
            doubleValue = 50.0d;
            // double quotes, don't change the enclosed characters.
            Assert.That("50.00 percent", Is.EqualTo(doubleValue.ToString("0.00\" percent\"")));
            // more than one successive escaped characters
            Assert.That("50.00   %", Is.EqualTo(doubleValue.ToString("0.00\\ \\ \\ \"%\"")));
            Assert.That("50.00\\ \\ \\ ", Is.EqualTo(doubleValue.ToString("0.00\"\\ \\ \\ \"")));
            Assert.That("50.00   ", Is.EqualTo(doubleValue.ToString("0.00\\ \\ \\ ")));
            // two successive substrings enclosed in double quotes
            Assert.That("50.00 % percent", Is.EqualTo(doubleValue.ToString("0.00\" %\"\" percent\"")));
            // dummy mix
            Assert.That(" %0.000.00\\-", Is.EqualTo(doubleValue.ToString("\" %\"\"0.00\"\"0.00\\-")));
        }

        [Test]
        [JavaThrows(true)]
        public void TestJava2084()
        {
            double doubleValue = 9d;
            Assert.That("9x.00", Is.EqualTo(doubleValue.ToString("#0x.00")));
            Assert.That("9x.00", Is.EqualTo(doubleValue.ToString("#0x.00")));
            Assert.That("9formatAxis", Is.EqualTo(doubleValue.ToString("0formatAxis")));

            doubleValue = 15d;
            Assert.That("$ x", Is.EqualTo(doubleValue.ToString("$ x")));

            doubleValue = 0.184d;
            Assert.That("0.18x", Is.EqualTo(doubleValue.ToString("0.00x")));
            Assert.That("0x", Is.EqualTo(doubleValue.ToString("0.x")));

            doubleValue = 111492d;
            Assert.That("dropx111492", Is.EqualTo(doubleValue.ToString("dropx##")));
            Assert.That("111492x", Is.EqualTo(doubleValue.ToString("0'x'")));
            Assert.That("111492x", Is.EqualTo(doubleValue.ToString("0\"x\"")));

            doubleValue = 24d;
            Assert.That("axis", Is.EqualTo(doubleValue.ToString("axis")));

            doubleValue = 3.14d;
            Assert.That("3formatAxis", Is.EqualTo(doubleValue.ToString("0formatAxis")));
            Assert.That("3x", Is.EqualTo(doubleValue.ToString("0x")));

            doubleValue = 600d;
            Assert.That("600formatAxis", Is.EqualTo(doubleValue.ToString("0formatAxis")));
            Assert.That("600\"formatAxis\"", Is.EqualTo(doubleValue.ToString("0\\\"formatAxis\\\"")));
            Assert.That("600formatAxis", Is.EqualTo(doubleValue.ToString("0\"formatAxis\"")));
        }
    }
}
