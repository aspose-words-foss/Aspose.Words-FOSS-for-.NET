// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestTableGrid : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCeller(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestCeller", lf, sf);
        }

    
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellerNarrow(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestCellerNarrow", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellerTop(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestCellerTop", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellerTopBottom(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestCellerTopBottom", lf, sf);
        }



        private static void VerifyDefect1130(Document doc)
        {
            // The table that consists of three tables on page 1.
            Assert.That(GetAllowBreak(doc, 0), Is.True);
            Assert.That(GetAllowBreak(doc, 1), Is.True);
            Assert.That(GetAllowBreak(doc, 2), Is.True);

            // The outer Optionscheine table on page 3.
            Assert.That(GetAllowBreak(doc, 3), Is.True);
            // The inner table on page 3.
            Assert.That(GetAllowBreak(doc, 4), Is.False);

            // The outer Optionscheine table on page 4.
            Assert.That(GetAllowBreak(doc, 5), Is.True);
            // The inner table on page 4.
            Assert.That(GetAllowBreak(doc, 6), Is.False);

            // The outer WKN table on page 7.
            Assert.That(GetAllowBreak(doc, 7), Is.True);
            // The inner WKN table on page 7.
            Assert.That(GetAllowBreak(doc, 8), Is.False);

            // The outer Basiswert table on page 8.
            Assert.That(GetAllowBreak(doc, 9), Is.False);
            // The inner Basiswert table on page 8.
            Assert.That(GetAllowBreak(doc, 10), Is.False);
        }

        private static bool GetAllowBreak(Document doc, int tableIdx)
        {
            Table table = (Table)doc.GetChild(NodeType.Table, tableIdx, true);
            return table.Rows[0].RowFormat.AllowBreakAcrossPages;
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLeftIndent(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestLeftIndent", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLeftIndentDifferent(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestLeftIndentDifferent", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLeftIndentDifferent1(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestLeftIndentDifferent1", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMergedCells(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestMergedCells", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMergedCellsH(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestMergedCellsH", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMergedCellsV(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\Grid\TestMergedCellsV", lf, sf);
        }



        private static void CheckCellMerged(Table table, int rowIdx, int cellIdx, CellMerge hMerge, CellMerge vMerge)
        {
            CellPr cellPr = table.Rows[rowIdx].Cells[cellIdx].CellPr;
            Assert.That(cellPr.HorizontalMerge, Is.EqualTo(hMerge));
            Assert.That(cellPr.VerticalMerge, Is.EqualTo(vMerge));
        }
    }
}
