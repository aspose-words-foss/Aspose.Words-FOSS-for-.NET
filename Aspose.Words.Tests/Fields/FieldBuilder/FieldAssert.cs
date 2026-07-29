// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    public class FieldAssert
    {
        private FieldAssert(Field field)
        {
            Assert.That(field, IsNot.Null(), "field is null");
            mField = field;
        }

        public static FieldAssert Field(Field field)
        {
            return new FieldAssert(field);
        }

        public FieldAssert OfType(FieldType fieldType)
        {
            Assert.That(mField.Type, Is.EqualTo(fieldType));

            return this;
        }

        public FieldAssert Inside(CompositeNode parent)
        {
            return Inside(parent, parent);
        }

        public FieldAssert Inside(CompositeNode startParent, CompositeNode endParent)
        {
            Assert.That(mField.Start.ParentNode, Is.SameAs(startParent));
            Assert.That(mField.End.ParentNode, Is.SameAs(endParent));

            return this;
        }

        public FieldAssert Between(Node afterNode, Node beforeNode)
        {
            After(afterNode);
            Before(beforeNode);

            return this;
        }

        public FieldNodeRangeAssert Contains()
        {
            return new FieldNodeRangeAssert(mField, this);
        }

        private void After(Node node)
        {
            Assert.That(mField.Start.PreviousSibling, Is.SameAs(node));
        }

        private void Before(Node node)
        {
            Assert.That(mField.End.NextSibling, Is.SameAs(node));
        }

        private readonly Field mField;
    }
}
