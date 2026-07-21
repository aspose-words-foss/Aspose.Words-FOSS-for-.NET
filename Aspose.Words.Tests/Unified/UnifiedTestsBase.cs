// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2022 by Edward Voronov

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public abstract class UnifiedTestsBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void TestSetUp()
        {
            RandomUtil.Reset();
        }

        public static readonly TestCaseData[] DefaultTestScenarios =
        {
            // Reduced to FOSS supported formats only.
            new TestCaseData(LoadFormat.Docx, SaveFormat.Docx)
        };
    }
}
