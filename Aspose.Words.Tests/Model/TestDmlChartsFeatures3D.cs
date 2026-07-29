// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/25/2014 by Alexey Noskov

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Simple open/save/compare tests for DOCX document with charts.
    /// Used to make sure charts are written properly from the model.
    /// Use test documents from charts features 3D rendering tests.
    /// </summary>
    [TestFixture]
    public class TestDmlChartsFeatures3D
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void Test3DAxisDepth()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisDepth.docx");
        }

        [Test]
        public void Test3DAxisRotation()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisRotation.docx");
        }

        [Test]
        public void Test3DAxisPositionA()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisPositionA.docx");
        }

        [Test]
        public void Test3DAxisPositionB()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisPositionB.docx");
        }

        [Test]
        public void Test3DAxisPositionC()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisPositionC.docx");
        }

        [Test]
        public void Test3DAxisPositionD()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAxisPositionD.docx");
        }

        [Test]
        public void Test3DBarDirrection()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarDirrection.docx");
        }

        [Test]
        public void Test3DBarDirrectionAxisRotation()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarDirrectionAxisRotation.docx");
        }

        [Test]
        public void Test3DBarDirrectionAxisPosition()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarDirrectionAxisPosition.docx");
        }

        [Test]
        public void Test3DManualLayout()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DManualLayout.docx");
        }

        [Test]
        public void Test3DPieChartsRotation()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DPieChartsRotation.docx");
        }

        [Test]
        public void Test3DPieChartsBorders()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DPieChartsBorders.docx");
        }

        [Test]
        public void Test3DVerticalAxisLabels()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DVerticalAxisLabels.docx");
        }

        [Test]
        public void Test3DBandFmts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBandFmts.docx");
        }

        [Test]
        public void Test3DDepthGridLines()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DDepthGridLines.docx");
        }

        [Test]
        public void Test3DDataLabels()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DDataLabels.docx");
        }

        [Test]
        public void Test3DDataTable()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DDataTable.docx");
        }
    }
}
