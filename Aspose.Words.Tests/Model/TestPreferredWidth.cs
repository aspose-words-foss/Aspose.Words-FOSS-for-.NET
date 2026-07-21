// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests the <see cref="Aspose.Words.Tables.PreferredWidth"/> class.
    /// </summary>
    [TestFixture]
    public class TestPreferredWidth
    {
        [Test]
        public void TestCreateValid()
        {
            PreferredWidth w1 = PreferredWidth.FromPercent(100);
            Assert.That(w1.Type, Is.EqualTo(PreferredWidthType.Percent));
            Assert.That(w1.Value, Is.EqualTo(100));

            PreferredWidth w2 = PreferredWidth.FromPoints(10);
            Assert.That(w2.Type, Is.EqualTo(PreferredWidthType.Points));
            Assert.That(w2.Value, Is.EqualTo(10));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPercentNegative()
        {
            PreferredWidth.FromPercent(-0.1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPercentTooBig()
        {
            // alexnosk: I changed value from -100.1 here to 600.1 to test whether exception is still thrown.
            // Old value here was negative, that I suppose was a mistake.
            PreferredWidth.FromPercent(600.1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPointsNegative()
        {
            PreferredWidth.FromPoints(-1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestPointsTooBig()
        {
            PreferredWidth.FromPoints(22 * 72 + 1);
        }

        [Test]
        public void TestEquality()
        {
            // Percent equality.
            PreferredWidth percent1 = PreferredWidth.FromPercent(28.8);    // This will create raw value 1440
            PreferredWidth percent2 = PreferredWidth.FromPercent(28.8);
            Assert.That(percent2, IsNot.SameAs(percent1));
            Assert.That(percent2, Is.EqualTo(percent1));
            Assert.That(percent1 == percent2, Is.False);

            PreferredWidth percent3 = PreferredWidth.FromPercent(28.9);
            Assert.That(percent3, IsNot.EqualTo(percent1));


            // Point equality
            PreferredWidth points1 = PreferredWidth.FromPoints(72); // This will create raw value 1440
            PreferredWidth points2 = PreferredWidth.FromPoints(72);
            Assert.That(points2, IsNot.SameAs(points1));
            Assert.That(points2, Is.EqualTo(points1));
            Assert.That(points1 == points2, Is.False);

            PreferredWidth points3 = PreferredWidth.FromPoints(73);
            Assert.That(points3, IsNot.EqualTo(points1));


            // When raw value is the same, the objects are not equal because type is different.
            Assert.That(percent1.ValueRaw, Is.EqualTo(1440));
            Assert.That(points1.ValueRaw, Is.EqualTo(1440));
            Assert.That(points1, IsNot.EqualTo(percent1));

            // Auto
            PreferredWidth auto1 = PreferredWidth.Auto;
            PreferredWidth auto2 = PreferredWidth.Auto;
            Assert.That(auto2, Is.SameAs(auto1));
            Assert.That(auto1 == auto2, Is.True);
        }

        /// <summary>
        /// WORDSNET-5317 Long hyperlinks broke tables layout upon rendering.
        /// </summary>
        [Test]
        public void TestJira5317()
        {
            DocumentBuilder docBuilder = new DocumentBuilder();

            // begin table
            Table table = docBuilder.StartTable();

            // The first row
            docBuilder.RowFormat.Height = 0;

            // The first cell
            docBuilder.InsertCell();

            table.AllowAutoFit = true;


            docBuilder.CellFormat.Borders.LineStyle = LineStyle.Single;
            docBuilder.CellFormat.Borders.LineWidth = 1;
            docBuilder.CellFormat.PreferredWidth = PreferredWidth.Auto;

            Style style = docBuilder.Document.Styles["Hyperlink"];
            Style oldStyle = docBuilder.Font.Style;
            if (style != null)
            {
                docBuilder.Font.Style = style;
            }

            docBuilder.InsertHyperlink("text",
                                       "https://www.aspose.com/community/forums/aspose.words-product-family/75/showforum.aspx?PageIndex=1&sb=0&d=1&df=11&uf=0&hrp=0&lf=0",
                                       false);

            if (oldStyle != null)
            {
                docBuilder.Font.Style = oldStyle;
            }


            docBuilder.InsertCell();

            docBuilder.CellFormat.Width = 300;
            docBuilder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(300);

            docBuilder.Write("Lorem ipsum dolor sit amet");

            // The third cell
            docBuilder.InsertCell();
            docBuilder.CellFormat.PreferredWidth = PreferredWidth.Auto;

            docBuilder.Write("Lorem ipsum dolor sit amet");

            // The fourth cell
            docBuilder.InsertCell();
            docBuilder.CellFormat.PreferredWidth = PreferredWidth.Auto;

            docBuilder.Write("Lorem ipsum dolor sit amet");


            // End the first row
            docBuilder.EndRow();

            // end table
            docBuilder.EndTable();

            TestUtil.Save(docBuilder.Document, @"Model\Nodes\TestJira5317.docx");
            // FOSS: Pdf rendering removed; the Docx save above is the no-crash check.
        }

        /// <summary>
        /// WORDSNET-5520 Exception on setting Table.PreferredWidth higher than 100 percent.
        /// </summary>
        [Test]
        public void TestDefect5520()
        {
            const double prefWidth = 120;

            DocumentBuilder builder = new DocumentBuilder();
            Table table = builder.StartTable();
            // Build a dummy table.
            for (int rowIdx = 0; rowIdx < 5; rowIdx++)
            {
                for (int cellIdx = 0; cellIdx < 5; cellIdx++)
                {
                    builder.InsertCell();
                    builder.Write("test table");
                }
                builder.EndRow();
            }
            builder.EndTable();

            // Set preferred width more that 100%.
            table.PreferredWidth = PreferredWidth.FromPercent(prefWidth);

            // FOSS: Doc writer removed; roundtrip through Docx (no committed gold).
            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\Nodes\TestDefect5520.docx", (Aspose.Words.Saving.SaveOptions)null, false);

            // Make sure preferred width is preserved.
            table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.PreferredWidth, Is.EqualTo(PreferredWidth.FromPercent(prefWidth)));
        }
    }
}
