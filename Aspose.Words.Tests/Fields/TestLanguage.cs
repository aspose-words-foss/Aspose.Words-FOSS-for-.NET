// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/05/2017 by Konstantin Sidorenko

using System;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestLanguage
    {
        [Test]
        public void TestEnumIsDefined()
        {
            Language[] values = (Language[])Enum.GetValues(typeof(Language));
            foreach (Language language in values)
            {
                Assert.That(LocaleClassifier.IsDefined(language), Is.True, string.Format("Language enum {0} is not defined. Please rerun TestLanguage.PrintSwitchBody() " +
                                                                    "and insert the new body into LocaleClassifier.IsDefined()", language));
            }
        }

        [JavaDelete]
        [Test, Ignore("Not a test.")]
        public void PrintSwitchBody()
        {
            Language[] values = (Language[])Enum.GetValues(typeof(Language));
            foreach (Language language in values)
            {
                Console.Out.WriteLine("                case Language.{0}:", language);
            }
        }
    }
}
