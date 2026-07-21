// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2018 by Konstantin Kornilov

#if !NETSTANDARD
using System.Drawing;
using System.Globalization;
using System.Threading;
using Aspose.Common;
using Aspose.Drawing.Fonts;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    [TestFixture]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Implementation dependent tests, not compilated in C++")]
    public class TestPalFont
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
        }

        [Test]
        [JavaDelete("Just an additional check, not neded in Java.")]
        public void TestGetNativeFamilyLocalized()
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = SystemPal.GetCulture(ProblemFontCulture);
            try
            {
                FontFamily result = FontPal.GetNativeFamily(ProblemFontName);
                Assert.That(result.Name, Is.EqualTo(ProblemFontNameLozalized));
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        private const string ProblemFontName = "DFKai-SB";
        private const string ProblemFontNameLozalized = "標楷體";
        private const string ProblemFontCulture = "zh-TW";
    }
}
#endif
