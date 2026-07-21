// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2015 by Ivan Lyagin

using System;
using System.Globalization;
using Aspose.Common;
using CodePorting.Translator.Cs2Cpp;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    /// <summary>
    /// Tests number formatting to make sure Java's output is the same as .NET's.
    /// http://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx
    /// http://msdn.microsoft.com/en-us/library/0c899ak8(v=vs.110).aspx
    /// </summary>
    [TestFixture]
    public class TestNumberFormatting
    {
        [Test]
        public void TestStandardNumericFormat()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                double value = 123.456;
                CultureInfo[] cultures =
                    {
                        new CultureInfo("ru-RU", false), 
                        new CultureInfo("en-US", false), 
                        new CultureInfo("de-DE", false), 
                        new CultureInfo("sv-SE", false), 
                        new CultureInfo("ja-JP", false) 
                    };

                // C
                string[] specifier = new string[] {"C", "C2", "C3", "C10"};
                string[] precisionResults = new string[] {"$123.46", "$123.46", "$123.456", "$123.4560000000"};
#if JAVA
                //JAVA specific results. No ruble currency symbol yet.
                String[] cultureSpecificResults = new String[] { "123,46 руб.", "$123.46", "123,46 €", "123,46 kr", "￥123" }; 
#elif NETSTANDARD
                string[] cultureSpecificResults = new string[] { "123,46₽", "$123.46", "123,46 €", "123,46 kr", "￥123" };
#else
                string[] cultureSpecificResults = new string[] {"123,46₽", "$123.46", "123,46 €", "123,46 kr", "¥123"};
#endif

#if !JAVA
                // For some reason when system region is set to ru-RU the default CurrencyPositivePattern value 
                // for ru-RU culture equals 3. But when system region is en-US then this value for ru-RU cultute
                // equals 1. Set this value explicitly.
                cultures[0].NumberFormat.CurrencyPositivePattern = 1;
#endif
                VerifyFormat(specifier, value, precisionResults, cultures, cultureSpecificResults);

                // D
                int valueInt = -123; //Supported by: Integral types only.
                specifier = new string[] {"D", "D5"};
#if NETSTANDARD
                precisionResults = new string[] { "-123", "-00123" };
                cultureSpecificResults = new string[] { "-123", "-123", "-123", "−123", "-123" };
#else
                precisionResults = new string[] {"-123", "-00123"};
                cultureSpecificResults = new string[] {"-123", "-123", "-123", "-123", "-123"};
#endif
                VerifyFormat(specifier, valueInt, precisionResults, cultures, cultureSpecificResults);

                // E
                specifier = new string[] {"e", "e2"};
                precisionResults = new string[] {"1.234560e+002", "1.23e+002"};
                cultureSpecificResults = new string[]
                    {"1,234560e+002", "1.234560e+002", "1,234560e+002", "1,234560e+002", "1.234560e+002"};
                VerifyFormat(specifier, value, precisionResults, cultures, cultureSpecificResults);

                // F
                specifier = new string[] {"F1", "F4"};
                precisionResults = new string[] {"123.5", "123.4560"};
                cultureSpecificResults = new string[] {"123,5", "123.5", "123,5", "123,5", "123.5"};
                VerifyFormat(specifier, value, precisionResults, cultures, cultureSpecificResults);

                // G
                specifier = new string[] {"G", "G4"};
#if JAVA
                //JAVA specific rounding. 
                precisionResults =       new String[] { "123.456", "123.456" }; 
#else
                precisionResults = new string[] {"123.456", "123.5"};
#endif
                cultureSpecificResults = new string[] {"123,456", "123.456", "123,456", "123,456", "123.456"};
                VerifyFormat(specifier, value, precisionResults, cultures, cultureSpecificResults);

                // N
                double valueDouble = 123456.789;
                specifier = new string[] {"N", "N3"};
#if NETSTANDARD
                precisionResults = new string[] { "123,456.789", "123,456.789" };
                cultureSpecificResults = new string[] { "123 456,789", "123,456.789", "123.456,789", "123 456,789", "123,456.789" };
#else
                precisionResults = new string[] {"123,456.79", "123,456.789"};
                cultureSpecificResults = new string[] {"123 456,79", "123,456.79", "123.456,79", "123 456,79", "123,456.79"};
#endif
                VerifyFormat(specifier, valueDouble, precisionResults, cultures, cultureSpecificResults);

                // P
                double persentValue = 1.789;
                specifier = new string[] {"P", "P2"};
#if JAVA
                // JAVA-changed
                precisionResults =       new String[] { "178.90%", "178.90%" }; 
                cultureSpecificResults = new String[] { "178,90%", "178.90%", "178,90%", "178,90 %", "178.90%" };
#elif NETSTANDARD
                precisionResults = new string[] { "178.900%", "178.90%" };
                cultureSpecificResults = new string[] { "178,900 %", "178.900%", "178,900 %", "178,900 %", "178.900%" };
#else
                precisionResults = new string[] { "178.90%", "178.90%" };
                cultureSpecificResults = new string[] { "178,90%", "178.90%", "178,90 %", "178,90 %", "178.90%" };
#endif
                VerifyFormat(specifier, persentValue, precisionResults, cultures, cultureSpecificResults);

                // R
                double roundValue = 3.14159265358979323846;
                //Supported by: Single, Double, and BigInteger.Precision specifier: Ignored.
                specifier = new string[] {"R"};

#if JAVA
                //JAVA specific rounding.
                precisionResults =       new String[] { "3.1415926535897930" }; 
                cultureSpecificResults = new String[] { "3,1415926535897930", "3.1415926535897930", "3,1415926535897930", "3,1415926535897930", "3.1415926535897930" };
#elif NETSTANDARD
                precisionResults = new String[] { "3.141592653589793" };
                cultureSpecificResults = new String[] { "3,141592653589793", "3.141592653589793", "3,141592653589793", "3,141592653589793", "3.141592653589793" };
#else
                precisionResults = new string[] {"3.1415926535897931"};
                cultureSpecificResults = new string[]
                    {
                        "3,1415926535897931", "3.1415926535897931", "3,1415926535897931", "3,1415926535897931",
                        "3.1415926535897931"
                    };
#endif
                VerifyFormat(specifier, roundValue, precisionResults, cultures, cultureSpecificResults);

                // X
                int hexValue = 1463; //Supported by: Integral types only.
                specifier = new string[] {"X", "X6"};
                precisionResults = new string[] {"5B7", "0005B7"};
                cultureSpecificResults = new string[] {"5B7", "5B7", "5B7", "5B7", "5B7"};
                VerifyFormat(specifier, hexValue, precisionResults, cultures, cultureSpecificResults);
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        [Test, Ignore("This functionality is very different in Java. Will be implemented when we need it")]
        public void TestCustomNumericFormat()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();
                double value = 123.456;

                // 0
                string format = "00000";
                VerifyCustomFormat(value, format, "00123");

                // #
                format = "#############";
                VerifyCustomFormat(value, format, "123");

                // .
                format = "0.0";
                VerifyCustomFormat(value, format, "123.5");

                // ,
                double value2 = 23456.129;
                format = "#,#.##";
                VerifyCustomFormat(value2, format, "23,456.13");

                // %
                format = "%#0.00";
                VerifyCustomFormat(value, format, "%12345.60");

                // ‰
                format = "#0.00‰";
                VerifyCustomFormat(value, format, "123456.00‰");

                // E0
                format = "#0.0e0";
                VerifyCustomFormat(value, format, "12.3e1");

                // E+0
                format = "0.0##e+00";
                VerifyCustomFormat(value, format, "1.235e+02");

                // 'string'
                format = "# ' degrees'";
                VerifyCustomFormat(value, format, "123  degrees");

                // \
                format = "#0.0#;(#0.0#);-\0-";
                VerifyCustomFormat(value, format, "123.46");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        [Test]
        public void TestExp()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();
                Assert.That(Format("e", 1.23456, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e+000"));
                Assert.That(Format("e", 12.3456, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e+001"));
                Assert.That(Format("e", 123.456, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e+002"));
                Assert.That(Format("e", 1234.56, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e+003"));
                Assert.That(Format("e", .123456, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e-001"));
                Assert.That(Format("e", .123456, CultureInfo.CurrentCulture), Is.EqualTo("1.234560e-001"));
                Assert.That(Format("e3", .123456, CultureInfo.CurrentCulture), Is.EqualTo("1.235e-001"));
                Assert.That(Format("e", 12341234123412341234D, CultureInfo.CurrentCulture), Is.EqualTo("1.234123e+019"));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }

        }

        private static void VerifyFormat<T>(string[] specifier, T value, string[] precisionResults,
                CultureInfo[] cultures, string[] cultureSpecificResults)
        {
            for (int i = 0; i < specifier.Length; i++)
                Assert.That(Format(specifier[i], value, CultureInfo.CurrentCulture), Is.EqualTo(precisionResults[i]));

            for (int i = 0; i < cultures.Length; i++)
                Assert.That(Format(specifier[0], value, cultures[i]), Is.EqualTo(cultureSpecificResults[i]));
        }

        private static void VerifyCustomFormat<T>(T value, string format, string result)
        {
            Assert.That(Format(format, value, CultureInfo.CurrentCulture), Is.EqualTo(result));
        }

        [CppSkipDefinition] // replaced with C++ template through porter configuration
        private static string Format<T>(string fmt, T val, CultureInfo culture)
        {
#if JAVA
            return com.aspose.ms.System.msNumber.toString(val, fmt, culture);
#else
            return ((IFormattable)val).ToString(fmt, culture);
#endif
        }
    }
}
