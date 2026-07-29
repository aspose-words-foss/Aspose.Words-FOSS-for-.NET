// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestBackgroundShape
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestNullBackground()
        {
            Document doc = new Document();
            Assert.That(doc.BackgroundShape, Is.EqualTo(null));
            TestUtil.SaveOpen(doc, @"Model\BackgroundShape\TestNullBackground.docx");
        }

        /// <summary>
        /// Checks it is possible to set the background shape to null.
        /// </summary>
        [Test]
        public void TestSetNullBackground()
        {
            Document doc = new Document();
            doc.BackgroundShape = new Shape(doc, ShapeType.Rectangle);
            Assert.That(doc.BackgroundShape, IsNot.Null());
            doc.BackgroundShape = null;
            Assert.That(doc.BackgroundShape, Is.Null);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage="The shape was created from a different document.")]
        public void TestSetOtherDocumentShapeAsBackground()
        {
            Document otherDoc = new Document();
            Document myDoc = new Document();
            myDoc.BackgroundShape = new Shape(otherDoc, ShapeType.Rectangle);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The shape is a child of another node.")]
        public void TestSetShapeWithParentAsBackground()
        {
            Document doc = new Document();
            Shape shape = new Shape(doc, ShapeType.Rectangle);
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            doc.BackgroundShape = shape;
        }

        [Test]
        public void TestRectangleBackground()
        {
            Document doc = new Document();
            doc.BackgroundShape = new Shape(doc, ShapeType.Rectangle);
            doc.BackgroundShape.FillColor = Color.Yellow;
            TestUtil.SaveOpen(doc, @"Model\BackgroundShape\TestRectangleBackground.docx");
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Only a rectangle shape can be set as a document background.")]
        public void TestTriangleBackground()
        {
            Document doc = new Document();
            doc.BackgroundShape = new Shape(doc, ShapeType.Triangle);
        }

        [Test]
        public void TestPageColor()
        {
            Document doc = new Document();
            doc.PageColor = DrColor.Green.ToNativeColor();  // Going via DrColor.Green because native green on Java and .NET are different color values.
            TestUtil.SaveOpen(doc, @"Model\BackgroundShape\TestPageColor.docx");
        }

        /// <summary>
        /// WORDSNET-20068 Document.PageColor sets the page color to black when it is set to Color.Empty.
        /// When PageColor is set to Color.Empty BackgroundShape should be removed.
        /// </summary>
        [Test]
        public void Test20068()
        {
            Document doc = new Document();

            doc.PageColor = Color.Empty;
            Assert.That(doc.BackgroundShape, Is.Null);
            Assert.That(doc.PageColor, Is.EqualTo(Color.Empty));

            doc.PageColor = Color.Red;
            Assert.That(doc.BackgroundShape, IsNot.Null());

            doc.PageColor = Color.Empty;
            Assert.That(doc.BackgroundShape, Is.Null);
            Assert.That(doc.PageColor, Is.EqualTo(Color.Empty));
        }
    }
}
