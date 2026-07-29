// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2021 by Mikhail Nepreteamov

using NUnit.Framework;

namespace Aspose.Tests.Base
{
    /// <summary>
    /// Class for testing locale and language issues.
    /// </summary>
    [TestFixture]
    public class TestLocale
    {
        /// <summary>
        /// WORDSNET-22405 LocaleConverter doesn't have mapping for Language.Breton and Language.Alsatian locales.
        /// Fixed by adding the mapping for Language.Breton and Language.Alsatian locales.
        /// </summary>
        [Test]
        public void Test22405()
        {
            // For DOCX dictionary.
            Assert.That((Language)LocaleConverter.DocxTagToLocale("gsw-FR"), Is.EqualTo(Language.Alsatian));
            Assert.That((Language)LocaleConverter.DocxTagToLocale("br-FR"), Is.EqualTo(Language.Breton));

            Assert.That(LocaleConverter.LocaleToDocxTag((int)Language.Alsatian), Is.EqualTo("gsw-FR"));
            Assert.That(LocaleConverter.LocaleToDocxTag((int)Language.Breton), Is.EqualTo("br-FR"));

            // For other dictionaries.
            Assert.That((Language)LocaleConverter.WmlTagToLocale("GSW-FR"), Is.EqualTo(Language.Alsatian));
            Assert.That((Language)LocaleConverter.WmlTagToLocale("BR-FR"), Is.EqualTo(Language.Breton));

            Assert.That(LocaleConverter.LocaleToWmlTag((int)Language.Alsatian), Is.EqualTo("GSW-FR"));
            Assert.That(LocaleConverter.LocaleToWmlTag((int)Language.Breton), Is.EqualTo("BR-FR"));
        }

        /// <summary>
        /// Related with WORDSNET-22434
        /// Checks that we also support old tags for backward compatibility.
        /// </summary>
        [Test]
        public void Test22434()
        {
            // For DOCX dictionary.
            Assert.That((Language)LocaleConverter.DocxTagToLocale("ru-MO"), Is.EqualTo(Language.RussianMoldova));
            Assert.That((Language)LocaleConverter.DocxTagToLocale("ru-MD"), Is.EqualTo(Language.RussianMoldova));
            Assert.That((Language)LocaleConverter.DocxTagToLocale("ro-MO"), Is.EqualTo(Language.RomanianMoldova));
            Assert.That((Language)LocaleConverter.DocxTagToLocale("ro-MD"), Is.EqualTo(Language.RomanianMoldova));

            // For other dictionaries.
            Assert.That((Language)LocaleConverter.WmlTagToLocale("ru-MO"), Is.EqualTo(Language.RussianMoldova));
            Assert.That((Language)LocaleConverter.WmlTagToLocale("ru-MD"), Is.EqualTo(Language.RussianMoldova));
            Assert.That((Language)LocaleConverter.WmlTagToLocale("ro-MO"), Is.EqualTo(Language.RomanianMoldova));
            Assert.That((Language)LocaleConverter.WmlTagToLocale("ro-MD"), Is.EqualTo(Language.RomanianMoldova));
        }
    }
}
