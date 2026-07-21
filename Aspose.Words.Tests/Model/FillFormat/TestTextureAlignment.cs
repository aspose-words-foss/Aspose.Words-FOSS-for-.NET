// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2021 by Vadim Saltykov

using System;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing TextureAlignment property of Fill object.
    /// </summary>
    [TestFixture]
    public class TestTextureAlignment : TestFillFormatBase
    {
        /// <summary>
        /// Checks obtaining TextureAlignment from DML shapes.
        /// </summary>
        [Test]
        public void TestGetTextureAlignmentDml()
        {
            TextureAlignment[] expAlignments = new TextureAlignment[]
            {
                TextureAlignment.BottomLeft, TextureAlignment.Bottom, TextureAlignment.BottomRight,
                TextureAlignment.Right, TextureAlignment.Center, TextureAlignment.Left,
                TextureAlignment.TopRight, TextureAlignment.Top, TextureAlignment.TopLeft
            };

            Document doc = Open("TextureAlignments", ShapeMarkupLanguage.Dml);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            for (int i = 0; i < shapes.Count; i++)
            {
                Fill fill = shapes[i].Fill;

                Assert.That(fill.TextureAlignment, Is.EqualTo(expAlignments[i]));
            }
        }

        /// <summary>
        /// Checks applying TextureAlignment to DML shapes.
        /// </summary>
        [Test]
        public void TestSetTextureAlignmentDml()
        {
            Document doc = Open("UserTextured", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.TextureAlignment, Is.EqualTo(TextureAlignment.TopLeft));
            fill.TextureAlignment = TextureAlignment.Center;
            Assert.That(fill.TextureAlignment, Is.EqualTo(TextureAlignment.Center));
        }

        /// <summary>
        /// Checks obtaining TextureAlign from VML shape.
        /// </summary>
        [Test]
        public void TestGetTextureAlignmentVml()
        {
            Document doc = Open("UserTextured", ShapeMarkupLanguage.Vml);
            Fill fill = GetFill(doc);
            Assert.That(fill.TextureAlignment, Is.EqualTo(TextureAlignment.None));
        }

        /// <summary>
        /// Checks applying TextureAlignment to VML shape.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "TextureAlignment cannot be applied to VML shapes.")]
        public void TestSetTextureAlignmentVml()
        {
            Document doc = Open("UserTextured", ShapeMarkupLanguage.Vml);
            Fill fill = GetFill(doc);
            fill.TextureAlignment = TextureAlignment.Center;
        }

        /// <summary>
        /// Checks obtaining TextureAlignment from Font fill.
        /// </summary>
        [Test]
        public void TestGetTextureAlignmentText()
        {
            Document doc = Open("TextNoFill", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.TextureAlignment, Is.EqualTo(TextureAlignment.None));
        }

        /// <summary>
        /// Checks applying TextureAlignment to Font fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "TextureAlignment cannot be applied to Font.")]
        public void TestSetTextureAlignmentText()
        {
            Document doc = Open("TextNoFill", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.TextureAlignment = TextureAlignment.Center;
        }

        /// <summary>
        /// Checks applying TextureAlignment to non-texture fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "TextureAlignment can be applied to texture fill only.")]
        public void TestTextureAlignmentNotTextureFill()
        {
            Document doc = Open("Solid", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.TextureAlignment = TextureAlignment.Center;
        }

        /// <summary>
        /// Checks applying TextureAlignment.None.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "TextureNone cannot be applied directly.")]
        public void TestTextureAlignmentNone()
        {
            Document doc = Open("UserTextured", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.TextureAlignment = TextureAlignment.None;
        }

        /// <summary>
        /// Checks obtaining TextureAlignment from non-tiling Blip fill.
        /// </summary>
        [Test]
        public void TestGetPictureAlignment()
        {
            Document doc = Open("UserPicture", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.TextureAlignment, Is.EqualTo(TextureAlignment.None));
        }

        /// <summary>
        /// Checks applying TextureAlignment to non-tiling Blip fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "TextureAlignment can be applied to tile texture fill only.")]
        public void TestSetPictureAlignment()
        {
            Document doc = Open("UserPicture", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.TextureAlignment = TextureAlignment.Bottom;
        }
    }
}
