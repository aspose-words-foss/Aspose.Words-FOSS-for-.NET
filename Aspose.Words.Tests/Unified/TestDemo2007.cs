// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for some cool-looking documents created in MS Word 2007.
    /// I cannot think of any reasonably unit-testing checking code for these files yet.
    /// Their main purpose is to be included on the website in some form to demonstrate
    /// how good we support DOC files comparing to other competitors.
    /// </summary>
    [TestFixture]
    public class TestDemo2007 : UnifiedTestsBase
    {

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBusinessInvoice(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\BusinessInvoice", lf, sf);
        }

        /// <summary>
        /// This contains imagedata inside a rectangle shape. What should we do?
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBusinessLetter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\BusinessLetter", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBusinessPurchaseOrder(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\BusinessPurchaseOrder", lf, sf);
        }




        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFaxBlackWhite(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\FaxBlackWhite", lf, sf);
        }




        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFlyerSaleWinter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\FlyerSaleWinter", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGiftCard(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\GiftCard", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGiftCoupons(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\GiftCoupons", lf, sf);
        }




        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPlanningProject(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\PlanningProject", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPlanningTodo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Demo2007\PlanningTodo", lf, sf);
        }
    }
}
