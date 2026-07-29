// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2014 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml
{
    [TestFixture]
    public class TestNestedDmls
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }


        [Test]
        public void TestJira10790()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\DrawingML\TestJira10790.docx");
            GroupShape dml = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(dml.DmlNode.DmlNodeType == DmlNodeType.WordprocessingCanvas, Is.True);
            DmlLockedCanvas canvas = (DmlLockedCanvas)dml.DmlNode;
            Assert.That(canvas.Fill, IsNot.Null());
            Assert.That(canvas.Fill.DmlFillType == DmlFillType.SolidFill, Is.True);
            Assert.That(canvas.Outline, IsNot.Null());
            Assert.That(canvas.Outline.DirectPropertiesCount <= 0, Is.False);
        }
    }
}
