// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/19/2014 by Alexey Noskov

using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml
{
    [TestFixture]
    public class TestDmlGeometryFallback
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestFallbackGeometry()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestFallbackGeometry.docx");
            // Remove alternate content. Test checks how alternate content is generated using our code.
            RemoveFallback(doc);
            TestUtil.SaveOpen(doc, @"Model\DrawingML\TestFallbackGeometry.docx");
        }

        [Test]
        public void TestFallbackGroupGeometry()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlGroup.docx");
            // Remove alternate content. Test checks how alternate content is generated using our code.
            RemoveFallback(doc);
            TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlGroup.docx");
        }

        [Test]
        public void TestFallbackNestedGroupGeometry()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlNestedGroup.docx");
            // Remove alternate content. Test checks how alternate content is generated using our code.
            RemoveFallback(doc);
            TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlNestedGroup.docx");
        }

        private static void RemoveFallback(Document doc)
        {
            NodeCollection dmls = doc.GetChildNodes(NodeType.GroupShape, true);
            foreach (GroupShape dml in dmls)
                dml.RunPr.Remove(FontAttr.AlternateContent);

            dmls = doc.GetChildNodes(NodeType.Shape, true);
            foreach (Shape dml in dmls)
                dml.RunPr.Remove(FontAttr.AlternateContent);
        }
    }
}
