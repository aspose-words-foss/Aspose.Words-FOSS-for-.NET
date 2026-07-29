// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2011 by Alexey Titov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Fills
{
    [TestFixture]
    public class TestDmlBlipFill
    {
        [Test]
        public void Clone()
        {
            DmlBlipFill fill = new DmlBlipFill();
            //Arrange
            fill.Blip.EmbedImage = new byte[] { 192 };
            DmlBlipFillStretch blipFillStretch = new DmlBlipFillStretch();
            fill.BlipFillMode = blipFillStretch;
            blipFillStretch.FillRectangle = new DmlPercentageOffsetRectangle(1,2,3,4);
            fill.SourceRectangle = new DmlPercentageOffsetRectangle(5, 6, 7, 8);
            //Act
            DmlBlipFill result = (DmlBlipFill)fill.Clone();
            //Assert
            Assert.That(result.Blip.EmbedImage, Is.EqualTo(new byte[] { 192 }));
            Assert.That(((DmlBlipFillStretch)result.BlipFillMode).FillRectangle.BottomOffset, Is.EqualTo(4));
            Assert.That(result.SourceRectangle.BottomOffset, Is.EqualTo(8));
        }

        /// <summary>
        /// Relates with WORDSNET-16359
        /// Checks obtaining and setting opacity for blip fill.
        /// </summary>
        [Test]
        public void TestOpacity()
        {
            DmlBlipFill blipFill = new DmlBlipFill();
            IList<DmlEffect> effects = blipFill.Blip.Effects;

            // 1. There is not alpha modulate fixed effect.
            Assert.That(effects.Count, Is.EqualTo(0));
            Assert.That(MathUtil.AreEqual(1.0, blipFill.Opacity), Is.True);
    
            // 2. Check, that setter appends alpha modulate fixed effect to effects collection.
            blipFill.Opacity = 0.7;
            Assert.That(effects.Count, Is.EqualTo(1));
            Assert.That(MathUtil.AreEqual(0.7, ((DmlAlphaModulateFixedEffect)effects[0]).Amount), Is.True);

            // 3. Setter changes last alpha modulate fixed effect in the effects sequence.
            effects.Add(new DmlBlurEffect());
            DmlAlphaModulateFixedEffect newAplhaFixed = new DmlAlphaModulateFixedEffect();
            newAplhaFixed.Amount = 0.5;
            effects.Add(newAplhaFixed);
            effects.Add(new DmlDuotoneEffect());

            blipFill.Opacity = 0.75;
            Assert.That(effects.Count, Is.EqualTo(4));
            Assert.That(MathUtil.AreEqual(0.7, ((DmlAlphaModulateFixedEffect)effects[0]).Amount), Is.True);
            Assert.That(MathUtil.AreEqual(0.75, ((DmlAlphaModulateFixedEffect)effects[2]).Amount), Is.True);

            // 4. Getter obtains last alpha modulate fixed effect in the effects sequence.
            Assert.That(MathUtil.AreEqual(0.75, blipFill.Opacity), Is.True);
            Assert.That(effects.Count, Is.EqualTo(4));
        }
    }
}