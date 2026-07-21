// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2023 by Alexander Zhiltsov

using System.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Provides methods to be used when testing chart features.
    /// </summary>
    internal class TestChartUtil
    {
        /// <summary>
        /// Sets the specified values to the properties of the <see cref="Font"/> instance.
        /// </summary>
        internal static void SetFontProperties(Font font, string name, double size, double kerning, double spacing,
            bool bold, bool italic, bool strikeThrough, bool doubleStrikeThrough, bool allCaps, bool smallCaps,
            Color color, Underline underline, RunVerticalAlignment verticalAlignment, int localeId)
        {
            font.Name = name;
            font.NameAscii = name;
            font.NameBi = name;
            font.NameFarEast = name;
            font.NameOther = name;
            font.Size = size;
            font.Kerning = kerning;
            font.Spacing = spacing;
            font.Bold = bold;
            font.Italic = italic;
            font.StrikeThrough = strikeThrough;
            font.DoubleStrikeThrough = doubleStrikeThrough;
            font.AllCaps = allCaps;
            font.SmallCaps = smallCaps;
            font.Color = color;
            font.Underline = underline;

            if ((int)verticalAlignment >= 0)
                font.VerticalAlignment = verticalAlignment;

            if (localeId > 0)
                font.LocaleId = localeId;
        }

        /// <summary>
        /// Checks properties of the specified <see cref="Font"/> instance.
        /// </summary>
        internal static void CheckFontProperties(Font font, string name, string nameBi, string nameFarEast,
            string nameOther, double size, double kerning, double spacing, bool bold, bool italic, bool strikeThrough,
            bool doubleStrikeThrough, bool allCaps, bool smallCaps, Color color, Color highlightColor,
            Underline underline, RunVerticalAlignment verticalAlignment, int localeId)
        {
            Assert.That(font.Name, Is.EqualTo(name));
            Assert.That(font.NameAscii, Is.EqualTo(name));

            if (nameBi != null)
                Assert.That(font.NameBi, Is.EqualTo(nameBi));

            if (nameFarEast != null)
                Assert.That(font.NameFarEast, Is.EqualTo(nameFarEast));

            if (nameOther != null)
                Assert.That(font.NameOther, Is.EqualTo(nameOther));

            Assert.That(font.Size, Is.EqualTo(size));
            Assert.That(font.Kerning, Is.EqualTo(kerning));
            Assert.That(font.Spacing, Is.EqualTo(spacing));
            Assert.That(font.Bold, Is.EqualTo(bold));
            Assert.That(font.Italic, Is.EqualTo(italic));
            Assert.That(font.StrikeThrough, Is.EqualTo(strikeThrough));
            Assert.That(font.DoubleStrikeThrough, Is.EqualTo(doubleStrikeThrough));
            Assert.That(font.AllCaps, Is.EqualTo(allCaps));
            Assert.That(font.SmallCaps, Is.EqualTo(smallCaps));
            Assert.That(font.Color, Is.EqualTo(color));
            Assert.That(font.HighlightColor, Is.EqualTo(highlightColor));
            Assert.That(font.Underline, Is.EqualTo(underline));
            Assert.That(font.VerticalAlignment, Is.EqualTo(verticalAlignment));

            if (localeId > 0)
                Assert.That(font.LocaleId, Is.EqualTo(localeId));
        }
    }
}
