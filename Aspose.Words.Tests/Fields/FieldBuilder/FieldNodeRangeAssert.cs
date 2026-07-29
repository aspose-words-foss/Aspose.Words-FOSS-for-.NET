// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using System.Text;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    public class FieldNodeRangeAssert
    {
        public FieldNodeRangeAssert(Field field, FieldAssert fieldAssert)
        {
            mFieldAssert = fieldAssert;
            NodeRange range = new NodeRange(field.Start, field.End);
            mNodeEnumerator = new NodeEnumerator(range);
        }

        public FieldNodeRangeAssert Start()
        {
            MoveToNextNodeAndAssert(NodeType.FieldStart);
            return this;
        }

        public FieldNodeRangeAssert Separator()
        {
            MoveToNextNodeAndAssert(NodeType.FieldSeparator);
            return this;
        }

        public FieldNodeRangeAssert End()
        {
            MoveToNextNodeAndAssert(NodeType.FieldEnd);
            return this;
        }

        public FieldNodeRangeAssert Text(string expectedText)
        {
            StringBuilder actualText = new StringBuilder();
            while (actualText.Length < expectedText.Length)
            {
                Run run = MoveToNextNode() as Run;
                if (run == null)
                    break;

                actualText.Append(run.Text);
            }

            Assert.That(actualText.ToString(), Is.EqualTo(expectedText));

            return this;
        }

        public FieldNodeRangeAssert ParagraphBreak()
        {
            MoveToNextNodeAndAssert(NodeType.Paragraph);
            return this;
        }

        public FieldAssert AndNoMore()
        {
            bool nextNodeExists = mNodeEnumerator.MoveToNextNode();

            if (nextNodeExists)
                Assert.Fail(string.Format("Field contains extra node {0}", mNodeEnumerator.CurrentNode));

            return mFieldAssert;
        }

        private void MoveToNextNodeAndAssert(NodeType expectedNodetype)
        {
            Assert.That(mNodeEnumerator.MoveToNextNode(), Is.True, "No more nodes in the field");
            Assert.That(mNodeEnumerator.CurrentNode.NodeType, Is.EqualTo(expectedNodetype));
        }

        private Node MoveToNextNode()
        {
            if (!mNodeEnumerator.MoveToNextNode())
                return null;

            return mNodeEnumerator.CurrentNode;
        }

        private readonly FieldAssert mFieldAssert;
        private readonly NodeEnumerator mNodeEnumerator;
    }
}