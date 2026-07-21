// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/12/2018 by Alexey Butalov

using System;
using System.Drawing;
using Aspose.Crypto;
using Aspose.Fonts;
using Aspose.Words.Fonts;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fonts
{
    public static class EmbeddedFontTestUtil
    {
        /// <summary>
        /// Checks that embedded fonts are absent for all fonts in <paramref name="fontInfoColection"/>.
        /// </summary>
        public static void CheckEmbeddedFontsAreNull(FontInfoCollection fontInfoColection)
        {
            foreach (FontInfo fontInfo in fontInfoColection)
                CheckEmbeddedFontsAreNull(fontInfo);
        }

        /// <summary>
        /// Checks that embedded fonts are absent for <paramref name="fontInfo"/>.
        /// </summary>
        public static void CheckEmbeddedFontsAreNull(FontInfo fontInfo)
        {
            CheckEmbeddedFontsAreNull(fontInfo, EmbeddedFontFormat.EmbeddedOpenType);
            CheckEmbeddedFontsAreNull(fontInfo, EmbeddedFontFormat.OpenType);
        }

        /// <summary>
        /// Checks that embedded fonts of specified format are absent for <paramref name="fontInfo"/>.
        /// </summary>
        private static void CheckEmbeddedFontsAreNull(FontInfo fontInfo, EmbeddedFontFormat embeddedFontFormat)
        {
            Assert.That(fontInfo.GetEmbeddedFont(embeddedFontFormat, EmbeddedFontStyle.Regular), Is.Null);
            Assert.That(fontInfo.GetEmbeddedFont(embeddedFontFormat, EmbeddedFontStyle.Bold), Is.Null);
            Assert.That(fontInfo.GetEmbeddedFont(embeddedFontFormat, EmbeddedFontStyle.Italic), Is.Null);
            Assert.That(fontInfo.GetEmbeddedFont(embeddedFontFormat, EmbeddedFontStyle.BoldItalic), Is.Null);
        }
    }
}
