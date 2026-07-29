// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/09/2021 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing GradientStops of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillGradientStops : TestFillFormatBase
    {
        [SetUp]
        public void SetUp()
        {
#if CPLUSPLUS || JAVA
            TestUtil.SetUpTests();
#endif
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            SystemPal.SetStandardCulture();
            SystemPal.SetStandardUICulture();
        }

        [TearDown]
        public void TearDown()
        {
            SystemPal.RestoreCulture();
            SystemPal.RestoreUICulture();
        }

        /// <summary>
        /// Checks indexer of gradient stop collection.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestIndexer(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            GradientStop gradientStopA = new GradientStop(Color.Chartreuse, 0.0);
            //Setter.
            fill.GradientStops[0] = gradientStopA;
            // Getter.
            Assert.That(fill.GradientStops[0], Is.EqualTo(gradientStopA));

            GradientStop gradientStopB = new GradientStop(Color.Gold, 0.8);
            // Setter.
            fill.GradientStops[1] = gradientStopB;
            // Getter.
            Assert.That(fill.GradientStops[1], Is.EqualTo(gradientStopB));

            // Roundtrip and check.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Indexer.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);
            fill = GetFill(doc);
#if !JAVA
            Assert.That(fill.GradientStops[0], Is.EqualTo(gradientStopA));
            Assert.That(fill.GradientStops[1], Is.EqualTo(gradientStopB));
#endif
        }

        /// <summary>
        /// Checks insertion of gradient stops to a specified index.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestInsert(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            int originalCount = fill.GradientStops.Count;
            GradientStop existingGradientStopA = fill.GradientStops[0];
            GradientStop existingGradientStopB = fill.GradientStops[1];

            // Insert at index 1.
            GradientStop gradientStopA = new GradientStop(Color.Chartreuse, 0.3);
            fill.GradientStops.Insert(1, gradientStopA);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 1));
            Assert.That(fill.GradientStops[1], Is.EqualTo(gradientStopA));

            // Insert at index 0.
            GradientStop gradientStopB = new GradientStop(Color.Yellow, 0.7);
            fill.GradientStops.Insert(0, gradientStopB);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 2));
            Assert.That(fill.GradientStops[0], Is.EqualTo(gradientStopB));

            // Roundtrip and check.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Insert.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);
            fill = GetFill(doc);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 2));
#if !JAVA
            Assert.That(fill.GradientStops[0], Is.EqualTo(gradientStopB));
            Assert.That(fill.GradientStops[1], Is.EqualTo(existingGradientStopA));
            Assert.That(fill.GradientStops[2], Is.EqualTo(gradientStopA));
            Assert.That(fill.GradientStops[3], Is.EqualTo(existingGradientStopB));
#endif
        }

        /// <summary>
        /// Checks adding gradient stop to the end of the collection and removing it at a specified index.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestAddAndRemoveAt(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            int originalCount = fill.GradientStops.Count;
            GradientStop existingGradientStopA = fill.GradientStops[0];
            GradientStop existingGradientStopB = fill.GradientStops[1];

            // Check adding a new gradient stop.
            GradientStop addedGradientStop = fill.GradientStops.Add(new GradientStop(Color.Green, 0.5));
            Assert.That(fill.GradientStops[originalCount], Is.EqualTo(addedGradientStop));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 1));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_AddRemoveAt.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 1));

            // Check removing gradient stop at a specified index.
            GradientStop removedGradientStop = fill.GradientStops.RemoveAt(0);
#if !JAVA
            Assert.That(removedGradientStop, Is.EqualTo(existingGradientStopA));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));
#endif

            // Roundtrip.
            outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_RemoveAt.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));
#if !JAVA
            Assert.That(fill.GradientStops[0], Is.EqualTo(existingGradientStopB));
            Assert.That(fill.GradientStops[originalCount - 1], Is.EqualTo(addedGradientStop));
#endif
        }

        /// <summary>
        /// Checks adding gradient stop to the end of the collection and
        /// removing some <see cref="GradientStop"/> from the collection.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestAddAndRemove(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            int originalCount = fill.GradientStops.Count;
            GradientStop existingGradientStopA = fill.GradientStops[0];
            GradientStop existingGradientStopB = fill.GradientStops[1];

            // Check adding a new gradient stop.
            GradientStop addedGradientStop = fill.GradientStops.Add(new GradientStop(Color.SkyBlue, 0.6));
            Assert.That(fill.GradientStops[originalCount], Is.EqualTo(addedGradientStop));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount + 1));

            // Check removing gradient stop.
            Assert.That(fill.GradientStops.Remove(existingGradientStopB), Is.True);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Remove.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));
#if !JAVA
            Assert.That(fill.GradientStops[0], Is.EqualTo(existingGradientStopA));
            Assert.That(fill.GradientStops[fill.GradientStops.Count - 1], Is.EqualTo(addedGradientStop));
#endif
        }

        /// <summary>
        /// Checks <see cref="GradientStop.Color"/> property of gradient stop.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestColor(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            GradientStop gradientStop = fill.GradientStops[0];

            //Check getter.
            Assert.That(fill.GradientStops[0].Color, Is.EqualTo(fill.ForeColor));
            Assert.That(gradientStop.Color, Is.EqualTo(fill.ForeColor));

            // Check setter.
            gradientStop.Color = Color.Chartreuse;
            Assert.That(fill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(Color.Chartreuse.ToArgb())));
            Assert.That(gradientStop.Color, Is.EqualTo(Color.FromArgb(Color.Chartreuse.ToArgb())));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Color.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(Color.Chartreuse.ToArgb())));
        }

        /// <summary>
        /// Checks <see cref="GradientStop.Position"/> property of gradient stop.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestPosition(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            //Check getter.
            Assert.That(fill.GradientStops[0].Position, Is.EqualTo(0.0).Within(0.001));
            GradientStop gradientStop = fill.GradientStops[0];
            Assert.That(gradientStop.Position, Is.EqualTo(0.0).Within(0.001));

            // Check setter.
            gradientStop.Position = 0.7;
            Assert.That(fill.GradientStops[0].Position, Is.EqualTo(0.7).Within(0.001));
            Assert.That(gradientStop.Position, Is.EqualTo(0.7).Within(0.001));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Position.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops[0].Position, Is.EqualTo(0.7).Within(0.001));
        }

        /// <summary>
        /// Checks <see cref="GradientStop.Transparency"/> property of gradient stop.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestTransparency(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            GradientStop gradientStop = fill.GradientStops[0];

            //Check getter.
            Assert.That(fill.GradientStops[0].Transparency, Is.EqualTo(fill.Transparency).Within(0.001));
            Assert.That(gradientStop.Transparency, Is.EqualTo(fill.Transparency).Within(0.001));

            // Check setter.
            gradientStop.Transparency = 0.15;
            Assert.That(fill.GradientStops[0].Transparency, Is.EqualTo(0.15).Within(0.001));
            Assert.That(gradientStop.Transparency, Is.EqualTo(0.15).Within(0.001));

            // Check also ctor with transparency.
            gradientStop = new GradientStop(Color.Crimson, 0.65, 0.25);
            fill.GradientStops.Insert(1, gradientStop);
            Assert.That(fill.GradientStops[1].Transparency, Is.EqualTo(0.25).Within(0.001));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Transparency.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops[0].Transparency, Is.EqualTo(0.15).Within(0.001));
            Assert.That(fill.GradientStops[1].Transparency, Is.EqualTo(0.25).Within(0.001));
        }


        /// <summary>
        /// Checks <see cref="GradientStop.Remove"/> method of gradient stop.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestRemove(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);

            int originalCount = fill.GradientStops.Count;
            GradientStop existingGradientStopA = fill.GradientStops[0];
            GradientStop existingGradientStopB = fill.GradientStops[1];

            GradientStop addedGradientStop = fill.GradientStops.Add(new GradientStop(Color.Brown, 0.85));

            existingGradientStopA.Remove();

            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));
            Assert.That(fill.GradientStops[0], Is.EqualTo(existingGradientStopB));
            Assert.That(fill.GradientStops[originalCount - 1], Is.EqualTo(addedGradientStop));

            // Roundtrip.
            string outFileName = string.Format("Model\\FillFormat\\{0}_GradientStop_Remove.docx", fileName);
            doc = TestUtil.SaveOpen(doc, outFileName, UnifiedScenario.Docx2DocxNoGold);

            fill = GetFill(doc);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(originalCount));
#if !JAVA
            Assert.That(fill.GradientStops[0], Is.EqualTo(existingGradientStopB));
            Assert.That(fill.GradientStops[originalCount - 1], Is.EqualTo(addedGradientStop));
#endif
        }

        /// <summary>
        /// Checks that exception is raised when accessing gradient stops of a non-gradient fill.
        /// </summary>
        [TestCase("NoFill")]
        [TestCase("TextNoFill")]
        [TestCase("Patterned")]
        [TestCase("PresetTextured")]
        [TestCase("Solid")]
        [TestCase("TextSolid")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Object doesn't support this action.")]
        public void TestNonGradientFills(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Dml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);
            Assert.That(fill.GradientStops, Is.Null);
        }

        /// <summary>
        /// Checks that exception is raised when accessing gradient stops of fill in VML object.
        /// </summary>
        [TestCase("NoFill")]
        [TestCase("TextNoFill")]
        [TestCase("Patterned")]
        [TestCase("PresetTextured")]
        [TestCase("Solid")]
        [TestCase("TextSolid")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Object doesn't support this action.")]
        public void TestVmlFills(string fileName)
        {
            const ShapeMarkupLanguage markupLanguage = ShapeMarkupLanguage.Vml;
            Document doc = Open(fileName, markupLanguage);

            Fill fill = GetFill(doc);
            Assert.That(fill.GradientStops, Is.Null);
        }

        /// <summary>
        /// Checks that exception is raised when accessing gradient stops collection at the out of range index.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
#if NETSTANDARD
        ExpectedMessage = "The specified value is out of range. (Parameter 'index')")]
#else
        ExpectedMessage = "The specified value is out of range.\r\nParameter name: index")]
#endif
        public void TestIndexerGetterOutOfRange(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.GradientStops[fill.GradientStops.Count], Is.Null);
        }

        /// <summary>
        /// Checks that exception is raised when accessing gradient stops collection at the out of range index.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        [ExpectedException(typeof(ArgumentOutOfRangeException),
#if NETSTANDARD
        ExpectedMessage = "The specified value is out of range. (Parameter 'index')")]
#else
        ExpectedMessage = "The specified value is out of range.\r\nParameter name: index")]
#endif
        public void TestIndexerSetterOutOfRange(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.GradientStops[fill.GradientStops.Count] = new GradientStop(Color.Aqua, 0.2);
        }

        /// <summary>
        /// Checks how facade and internal gradient stops are synchronizing.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextOneColorGradient")]
        [TestCase("TextPresetGradient")]
        public void TestSync(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            // Check original first gradient stop color.
            GradientStop gradientStop = fill.GradientStops[0];
            Assert.That(gradientStop.Color, Is.EqualTo(fill.ForeColor));

            // Change first gradient stop color and check foreground color of fill is altered as well.
            gradientStop.Color = Color.Chartreuse;
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(Color.Chartreuse.ToArgb())));

            // Change foreground color of fill and check first gradient stop color is altered as well.
            fill.ForeColor = Color.Chocolate;
            Assert.That(gradientStop.Color, Is.EqualTo(Color.FromArgb(Color.Chocolate.ToArgb())));
            Assert.That(fill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(Color.Chocolate.ToArgb())));
        }

        /// <summary>
        /// Checks that exception is raised when applying gradient stop that belongs to another collection already.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The gradient stop belongs to another collection already.")]
        public void TestInsertIntoOtherCollection()
        {
            Document docA = Open("OneColorGradient", ShapeMarkupLanguage.Dml);
            Fill fillA = GetFill(docA);

            Document docB = Open("TwoColorGradient", ShapeMarkupLanguage.Dml);
            Fill fillB = GetFill(docB);

            // Applying gradient stop from foreign collection.
            fillA.GradientStops[0] = fillB.GradientStops[0];
        }

        /// <summary>
        /// Checks that exception is raised when applying gradient stop
        /// that already set to another gradient index of the same collection.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The gradient stop is set to another index already.")]
        public void TestInsertIntoSameCollection()
        {
            Document doc = Open("OneColorGradient", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            // Applying gradient stop from the same collection.
            fill.GradientStops[0] = fill.GradientStops[1];
        }

        /// <summary>
        /// Checks that gradient stop can be applied to the same index of the same collection
        /// (this is meaningless, but is possible).
        /// </summary>
        [Test]
        public void TestInsertAtSameIndex()
        {
            Document doc = Open("OneColorGradient", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            // Applying gradient stop to itself.
            fill.GradientStops[0] = fill.GradientStops[0];
        }

        /// <summary>
        /// Checks that removed gradient stop can be set to another index or even another collection.
        /// </summary>
        [Test]
        public void TestInsertRemoved()
        {
            Document doc = Open("PresetGradient", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            Assert.That(fill.GradientStops.Count, Is.EqualTo(6));
            GradientStop gradientStop = fill.GradientStops[4];
            // Remove gradient stop from the collection.
            gradientStop.Remove();
            Assert.That(fill.GradientStops.Count, Is.EqualTo(5));

            Assert.That(fill.GradientStops[0], IsNot.EqualTo(gradientStop));
            // Set removed gradient stop to another index of the same collection.
            fill.GradientStops[0] = gradientStop;
            Assert.That(fill.GradientStops[0], Is.EqualTo(gradientStop));

            // Remove it again to allow it to be set into another collection.
            fill.GradientStops.Remove(gradientStop);
            Assert.That(fill.GradientStops.Count, Is.EqualTo(4));

            Document docOther = Open("OneColorGradient", ShapeMarkupLanguage.Dml);
            Fill fillOther = GetFill(docOther);

            Assert.That(fillOther.GradientStops[1], IsNot.EqualTo(gradientStop));
            // Set removed gradient stop to another collection.
            fillOther.GradientStops[1] = gradientStop;
            Assert.That(fillOther.GradientStops[1], Is.EqualTo(gradientStop));
        }

        /// <summary>
        /// Checks that exception is raised when trying to remove gradient stop
        /// that doesn't belong to any collection yet.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The gradient stop doesn't belong to any collection.")]
        public void TestRemoveNonAttached()
        {
            GradientStop gradientStop = new GradientStop(Color.Aqua, 0.4);
            gradientStop.Remove();
        }

        /// <summary>
        /// Checks that exception is raised when trying to remove gradient stop from the collection
        /// with less or equal than two gradient stops.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "There can not be less than two gradient stops in gradient fill.")]
        public void TestRemoveInsufficientNumberOfStops()
        {
            Document doc = Open("TwoColorGradient", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            fill.GradientStops.RemoveAt(0);
        }

        /// <summary>
        /// Checks <see cref="GradientStop.BaseColor"/> property of gradient stop.
        /// </summary>
        [Test]
        public void TestBaseColor()
        {
            Chart chart = CreateChart(new DocumentBuilder());

            Fill fill = chart.Series[0].DataPoints[0].Format.Fill;
            fill.OneColorGradient(Color.Red, GradientStyle.Horizontal, GradientVariant.Variant1, 0.1);

            // In OneColorGradient the second color is set automatically by adding to the first color either tint or shade.
            // So, we can use this to check unmodified color is actually those we set to the first color.
            GradientStop gradientStop = fill.GradientStops[1];
            Assert.That(gradientStop.BaseColor, Is.EqualTo(Color.FromArgb(0xFF, 0x00, 0x00)));
            Assert.That(gradientStop.Color, Is.EqualTo(Color.FromArgb(0x7C, 0x00, 0x00)));
        }
    }
}
