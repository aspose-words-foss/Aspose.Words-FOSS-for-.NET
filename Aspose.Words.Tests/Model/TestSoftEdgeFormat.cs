// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2024 by Ilya Navrotskiy

using System;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests soft edge formatting.
    /// </summary>
    [TestFixture]
    public class TestSoftEdgeFormat
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-26672 Add support to get Soft Edges effect of a Shape.
        /// Tests soft edge properties getters and setters in DML shape.
        /// </summary>
        [Test]
        public void Test26672Dml()
        {
            Document doc = TestUtil.Open(@"Model\Shape\SoftEdgeFormat\Test26672Dml.docx");

            SoftEdgeFormat softEdge = doc.FirstSection.Body.Shapes[0].SoftEdge;

            // Test getter.
            Assert.That(softEdge.Radius, Is.EqualTo(27));

            // Test setter.
            softEdge.Radius = 40.8;
            Assert.That(softEdge.Radius, Is.EqualTo(40.8));

            // Roundtrip and check again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\SoftEdgeFormat\Test26672Dml.docx");
            softEdge = doc.FirstSection.Body.Shapes[0].SoftEdge;
            Assert.That(softEdge.Radius, Is.EqualTo(40.8));
        }

        // FOSS: Test26670VmlRadius removed — loads a VML .doc to assert that SoftEdge is
        // unsupported on VML shapes. The Doc reader is removed, so the .doc can no longer load.


    }
}
