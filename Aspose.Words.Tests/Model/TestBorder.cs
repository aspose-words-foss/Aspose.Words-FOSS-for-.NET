// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Drawing;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests Border type.
    /// </summary>
    [TestFixture]
    public class TestBorder
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests border number according to ECMA $17.4.67 with
        /// Normative Variations 2.1.169 Part 4 Section 2.4.63, tcBorders (Table Cell Borders).
        /// </summary>
        [Test]
        public void TestBorderNumber()
        {
            Assert.That(GetBorderNumber(LineStyle.Single), Is.EqualTo(1));
            Assert.That(GetBorderNumber(LineStyle.Thick), Is.EqualTo(2));
            Assert.That(GetBorderNumber(LineStyle.Double), Is.EqualTo(3));
            Assert.That(GetBorderNumber(LineStyle.Dot), Is.EqualTo(4));
            Assert.That(GetBorderNumber(LineStyle.DashLargeGap), Is.EqualTo(5));
            Assert.That(GetBorderNumber(LineStyle.DotDash), Is.EqualTo(8));
            Assert.That(GetBorderNumber(LineStyle.DotDotDash), Is.EqualTo(9));
            Assert.That(GetBorderNumber(LineStyle.Triple), Is.EqualTo(10));
            Assert.That(GetBorderNumber(LineStyle.ThinThickSmallGap), Is.EqualTo(11));
            Assert.That(GetBorderNumber(LineStyle.ThickThinSmallGap), Is.EqualTo(12));
            Assert.That(GetBorderNumber(LineStyle.ThinThickThinSmallGap), Is.EqualTo(13));
            Assert.That(GetBorderNumber(LineStyle.ThinThickMediumGap), Is.EqualTo(14));
            Assert.That(GetBorderNumber(LineStyle.ThickThinMediumGap), Is.EqualTo(15));
            Assert.That(GetBorderNumber(LineStyle.ThinThickThinMediumGap), Is.EqualTo(16));
            Assert.That(GetBorderNumber(LineStyle.ThinThickLargeGap), Is.EqualTo(17));
            Assert.That(GetBorderNumber(LineStyle.ThickThinLargeGap), Is.EqualTo(18));
            Assert.That(GetBorderNumber(LineStyle.ThinThickThinLargeGap), Is.EqualTo(19));
            Assert.That(GetBorderNumber(LineStyle.Wave), Is.EqualTo(20));
            Assert.That(GetBorderNumber(LineStyle.DoubleWave), Is.EqualTo(21));
            Assert.That(GetBorderNumber(LineStyle.DashSmallGap), Is.EqualTo(22));
            Assert.That(GetBorderNumber(LineStyle.DashDotStroker), Is.EqualTo(23));
            Assert.That(GetBorderNumber(LineStyle.Emboss3D), Is.EqualTo(24));
            Assert.That(GetBorderNumber(LineStyle.Engrave3D), Is.EqualTo(25));
            Assert.That(GetBorderNumber(LineStyle.Outset), Is.EqualTo(26));
            Assert.That(GetBorderNumber(LineStyle.Inset), Is.EqualTo(27));

            Assert.That(GetBorderNumber(LineStyle.None), Is.EqualTo(0));

            // This is not in the ECMA for some reason. Looks like 1 is OK for Hairline.
            Assert.That(GetBorderNumber(LineStyle.Hairline), Is.EqualTo(1));

            // Now a border that does not exist.
            Assert.That(GetBorderNumber((LineStyle)123456), Is.EqualTo(0));
        }

        private static int GetBorderNumber(LineStyle lineStyle)
        {
            Border border = new Border();
            border.LineStyle = lineStyle;
            return border.BorderNumber;
        }

        /// <summary>
        /// Tests border weight according to according to ECMA $17.4.67 with
        /// Normative Variations 2.1.169 Part 4 Section 2.4.63, tcBorders (Table Cell Borders).
        /// </summary>
        [Test]
        public void TestBorderConflictWeight()
        {
            Border a = new Border();
            a.LineStyle = LineStyle.Triple;
            Assert.That(a.Weight, Is.EqualTo(a.RawLineWidth * a.BorderNumber));

            Border b = new Border();
            b.LineStyle = LineStyle.Wave;
            b.LineWidth = a.LineWidth/2;
            // Wave border number is two times triple border number.
            Assert.That(b.Weight, Is.EqualTo(b.RawLineWidth * b.BorderNumber));

            // The winning border is taken by border number in case of equal weight.
            Assert.That(b.Weight, Is.EqualTo(a.Weight));
            Assert.That(Border.GetWinningBorder(a, b), Is.SameAs(b));
            Assert.That(Border.GetWinningBorder(b, a), Is.SameAs(b));
        }

        private static Border MakeBorderWithLineStyle(LineStyle lineStyle)
        {
            Border border = new Border();
            border.LineWidth = 1d;
            border.LineStyle = lineStyle;
            return border;
        }

        /// <summary>
        /// Tests that Hairline behaves like Single.
        /// </summary>
        [Test]
        public void TestBorderWeightHairline()
        {
            Border none = MakeBorderWithLineStyle(LineStyle.None);
            Border hairline = MakeBorderWithLineStyle(LineStyle.Hairline);
            Border single = MakeBorderWithLineStyle(LineStyle.Single);

            Assert.That(Border.GetWinningBorder(none, hairline), Is.SameAs(hairline));
            // We only differ Hairline from single by width. There is no Hairline in ECMA.
            Assert.That(Border.GetWinningBorder(single, hairline), Is.SameAs(single));
            Assert.That(Border.GetWinningBorder(hairline, single), Is.SameAs(hairline));
        }

        /// <summary>
        /// Test that Dashed and Dotted have weight == 1.
        /// </summary>
        [Test]
        public void TestBorderWeightDashedDotted()
        {
            Border none = MakeBorderWithLineStyle(LineStyle.None);
            Border dotted = MakeBorderWithLineStyle(LineStyle.Dot);
            Border dashed = MakeBorderWithLineStyle(LineStyle.DashLargeGap);
            Border single = MakeBorderWithLineStyle(LineStyle.Single);

            Assert.That(dotted.Weight, Is.EqualTo(1));
            Assert.That(dashed.Weight, Is.EqualTo(1));

            Assert.That(Border.GetWinningBorder(none, dotted), Is.SameAs(dotted));
            Assert.That(Border.GetWinningBorder(dotted, dashed), Is.SameAs(dashed));
            Assert.That(Border.GetWinningBorder(dashed, single), Is.SameAs(single));
        }

        /// <summary>
        /// Tests border conflict resolved via brightness.
        /// </summary>
        [Test]
        public void TestBorderConflictBrightness()
        {
            Border red = MakeBorderWithColor(Color.FromArgb(255, 0 , 0));
            Border green = MakeBorderWithColor(Color.FromArgb(0, 255, 0));
            Border blue = MakeBorderWithColor(Color.FromArgb(0, 0, 255));

            Assert.That(Border.GetWinningBorder(blue, green), Is.SameAs(blue));
            Assert.That(Border.GetWinningBorder(blue, red), Is.SameAs(red));

            Border redder = MakeBorderWithColor(Color.FromArgb(101, 100, 100));
            Border bluer = MakeBorderWithColor(Color.FromArgb(100, 100, 101));
            Assert.That(Border.GetWinningBorder(bluer, redder), Is.SameAs(redder));

            Border greenish = MakeBorderWithColor(Color.FromArgb(10, 200, 12));
            Border greener = MakeBorderWithColor(Color.FromArgb(10, 201, 10));
            Assert.That(Border.GetWinningBorder(greener, greenish), Is.SameAs(greenish));
        }

        private static Border MakeBorderWithColor(Color color)
        {
            Border border = new Border();
            border.LineStyle = LineStyle.Single;
            border.LineWidth = 1d;
            border.Color = color;
            return border;
        }

        /// <summary>
        /// Tests that first parameter wins for identical borders.
        /// </summary>
        [Test]
        public void TestBorderConflictOrder()
        {
            Border first = MakeBorderWithColor(Color.Red);
            Border second = MakeBorderWithColor(Color.Red);

            Assert.That(second, IsNot.SameAs(first));
            Assert.That(Border.GetWinningBorder(first, second), Is.SameAs(first));
            Assert.That(Border.GetWinningBorder(second, first), Is.SameAs(second));
        }

        /// <summary>
        /// Tests that null and None loose in case of conflict.
        /// </summary>
        [Test]
        public void TestBorderConflictNull()
        {
            Border borderNull = null;
            Border borderNone = MakeBorderWithLineStyle(LineStyle.None);
            Border borderNone2 = MakeBorderWithLineStyle(LineStyle.None);

            Assert.That(Border.GetWinningBorder(borderNull, borderNone), Is.SameAs(borderNone));
            Assert.That(Border.GetWinningBorder(borderNone, borderNull), Is.SameAs(borderNone));
            Assert.That(Border.GetWinningBorder(borderNone, borderNone2), Is.SameAs(borderNone));
            Assert.That(Border.GetWinningBorder(borderNone2, borderNone), Is.SameAs(borderNone2));
        }

        /// <summary>
        /// WORDSNET-7706 Paragraph/Text Border is missing when saved to PDF, TIFF, XPS.
        /// The problem occurs because there is border with zero width.
        /// MS Word silently fixes borders with line style specified and zero widths, by assigning them the minimal widths of 1/4 pt.
        /// andrnosk: Fixed by assigning minimal (1/4pt) width in case when border style is specified and line widths equals zero.
        /// </summary>
        [Test]
        public void TestJira7706()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.ParagraphFormat.Borders.Left.LineStyle = LineStyle.Single;
            builder.Writeln("Hello World!");
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Borders.Left.Weight, Is.EqualTo(2));
        }




        /// <summary>
        /// WORDSNET-11182 Table borders are missing after re-saving Docx
        /// Duplicates WORDSNET-11539
        /// </summary>
        [Test]
        public void TestJira11182()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira11182.docx");

            // Verify that none of cells have Nil border.
            Table table = doc.FirstSection.Body.Tables[0];
            foreach (Row row in table.Rows)
                foreach(Cell cell in row.Cells)
                    foreach (int borderKey in CellPr.PossibleBorderKeys.Values)
                    {
                        Border border = (Border)cell.CellPr[borderKey];
                        if (border != null)
                        {
                            Assert.That(border.IsNil, Is.False);
                        }
                    }
        }


        /// <summary>
        /// WORDSNET-4796 Table style cell borders override direct table formatting borders
        /// Duplicates WORDSNET-11539
        /// </summary>
        [Test]
        public void TestJira4796()
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestJira4796.docx");
        }

    }
}
