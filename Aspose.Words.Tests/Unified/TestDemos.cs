// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for documents included in Aspose.Words.Demos to make sure they all work.
    /// </summary>
    [TestFixture]
    public class TestDemos : UnifiedTestsBase
    {
        [Test]
        [JavaDelete("#WORDSJAVA-2125: Random w:tblStylePr sorting breaks tests.")]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlternatingRowsDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\AlternatingRowsDemo", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBarCodeDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\BarCodeDemo", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDocumentBuilderDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\DocumentBuilderDemo", lf, sf);
        }



        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProductCatalogDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\ProductCatalogDemo", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSalesReportDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\SalesReportDemo", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStampAndWatermarkDemo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"DemoDocuments\StampAndWatermarkDemo", lf, sf);
        }
    }
}
