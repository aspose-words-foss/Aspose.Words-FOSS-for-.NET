// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using NUnit.Framework;

namespace Aspose.Words.Tests.Defects
{
    /// <summary>
    /// This file contains tests made for problems reported on support forum.
    /// Some of them will also correspond to issues logged on DTTS
    /// </summary>
    [TestFixture]
    public class TestDefects
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }



        /// <summary>
        /// Tests cloning chart title.
        /// </summary>
        [Test, Ignore("TestDefects")]
        public void TestCloningChartTitle()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestJira16069.docx");
            Document clone = doc.Clone();
            ChartTitle title = clone.FirstSection.Body.Shapes[0].Chart.Title;
            Assert.Fail("The mDocument field of the 'title' object has wrong value, it should be equals to 'clone'.");
        }


        [Test, Ignore("Not a Test")]
        [JavaDelete]
        public void CheckCLRVersion()
        {
            // Test one
            Console.WriteLine("CLR Version: {0}", Environment.Version.ToString());
        }
    }
}
