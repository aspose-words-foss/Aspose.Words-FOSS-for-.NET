// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/05/2014 by Alexey Noskov

using Aspose.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestTextEffects
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestTextEffectsMainBody()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectsMainBody.docx");
        }

        [Test]
        public void TestTextEffectFillMainBody()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectFillMainBody.docx");
        }

        [Test]
        public void TestTextEffectOutlineMainBody()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectOutlineMainBody.docx");
        }

        [Test]
        public void TestTextEffectsSmartArt()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectsSmartArt.docx");
        }

        [Test]
        public void TestTextEffectFillSmartArt()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectFillSmartArt.docx");
        }

        [Test]
        public void TestTextEffectOutlineSmartArt()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectOutlineSmartArt.docx");
        }

        /// <summary>
        /// Tests the opening and saving of a file with text with a zero-width outline. See WORDSNET-17103
        /// </summary>
        [Test]
        public void TestTextEffectsOutlineZeroWidth()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestTextEffectsOutlineZeroWidth.docx");
        }

        public void TestJira7723()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestJira7723.docx");
        }


        [Test]
        public void TestJira9175()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestJira9175.docx");
        }


        [Test]
        public void TestUnderline()
        {
            TestUtil.OpenSaveOpen(@"Model\TextEffects\TestUnderline.docx");
        }




        /// <summary>
        /// WORDSNET-14297 When DOCX is opened, text effect font properties remain unfilled.
        /// HasDmlEffect method have been added to check if particular Dml text effect is applied.
        /// </summary>
        [Test]
        public void Test14297()
        {
            Document doc = TestUtil.Open(@"Model\TextEffects\Test14297.docx");

            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;

            Font runFont = runs[23].Font;

            // One run might have several Dml text effects applied.
            Assert.That(runFont.HasDmlEffect(TextDmlEffect.Shadow), Is.True);
            Assert.That(runFont.HasDmlEffect(TextDmlEffect.Effect3D), Is.True);

            // Check the other affects are not applied.
            Assert.That(runFont.HasDmlEffect(TextDmlEffect.Outline), Is.False);
            Assert.That(runFont.HasDmlEffect(TextDmlEffect.Fill), Is.False);

            // Do not mix with TextEffect - it is animation effects like LasVegasLights etc.
            Assert.That(runFont.TextEffect, Is.EqualTo(TextEffect.None));

            Assert.That(runs[25].Font.HasDmlEffect(TextDmlEffect.Shadow), Is.True);
            Assert.That(runs[27].Font.HasDmlEffect(TextDmlEffect.Reflection), Is.True);
            Assert.That(runs[29].Font.HasDmlEffect(TextDmlEffect.Outline), Is.True);
            Assert.That(runs[35].Font.HasDmlEffect(TextDmlEffect.Effect3D), Is.True);
            Assert.That(runs[37].Font.HasDmlEffect(TextDmlEffect.Fill), Is.True);
        }
    }
}
