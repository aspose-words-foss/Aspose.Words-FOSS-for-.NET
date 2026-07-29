// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2015 by Alexander Zhiltsov

using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Vml;
using NUnit.Framework;

namespace Aspose.Words.Tests.RW
{
    /// <summary>
    /// Tests related to conversion of Color values to/from VML string representation.
    /// </summary>
    [TestFixture]
    public class TestVmlColor
    {
        /// <summary>
        /// WORDSNET-11964 Aspose.Words.FileCorruptedException is thrown while loading Docx
        /// that has #0 as shape strokecolor value.
        /// </summary>
        [TestCase("#0", 0, 0, 0)]
        [TestCase("#f", 0, 0, 0xFF)]
        public void TestOneDigitColor(string vmlColor, int expectedR, int expectedG, int expectedB)
        {
            DrColor color = VmlColor.VmlToColor(vmlColor);
            Assert.That(color.R, Is.EqualTo(expectedR), "R");
            Assert.That(color.G, Is.EqualTo(expectedG), "G");
            Assert.That(color.B, Is.EqualTo(expectedB), "B");
        }

        /// <summary>
        /// WORDSNET-15983 The orange color was written as 'orange' string that is not present as
        /// predefined color constant in VML documentation at section 6.1.3.1 ST_ColorType (Color Type).
        /// </summary>
        [Test]
        public void TestOrangeColor()
        {
            Assert.That(VmlColor.ColorToVml(new DrColor(Color.Orange.ToArgb())), Is.EqualTo("#ffa500"));
        }

        /// <summary>
        /// Tests parsing the base color and the modifier from VML extended color string.
        /// </summary>
        [TestCase("#d5b9c7 [rgb(149,79,114) lighten(102)]", 149, 79, 114, 102)]
        [TestCase("#954f72 [rgb(0,1,114) lighten(102)]", 0, 1, 114, 102)]
        [TestCase("#3b1f2d [rgb(255,255,255) darken(1)]", 255, 255, 255, -1)]
        [TestCase("#683750 [rgb(52,5,1) darken(12)]", 52, 5, 1, -12)]
        [TestCase("black [rgb(149,79,114) darken(10)]", 149, 79, 114, -10)]
        [TestCase("#9bbb59 [3042]", 0, 0, 0, 0)]
        public void TestBaseColorAndModifier(string vmlColor, int expR, int expG, int expB, int expModifier)
        {
            DrColor expectedColor = (expR == 0 && expG == 0 && expB == 0)
                ? null
                : DrColor.FromArgb(expR, expG, expB);

            Assert.That(VmlColor.GetBaseColor(vmlColor), Is.EqualTo(expectedColor));
            Assert.That(VmlColor.GetColorModifier(vmlColor), Is.EqualTo(expModifier));
        }
    }
}
