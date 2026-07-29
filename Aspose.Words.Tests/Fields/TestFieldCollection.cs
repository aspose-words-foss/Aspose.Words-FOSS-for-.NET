// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2018 by Edward Voronov

using System.Collections.Generic;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldCollection : TestFieldsBase
    {
        /// <summary>
        /// Tests <see cref="FieldCollection"/> functionality.
        /// </summary>
        [Test]
        public void TestFieldCollectionCore()
        {
            Document doc = TestUtil.Open(@"Fields\Other\TestGetFieldByFieldChar.docx");

            // Check that a non-composite node contains no fields.
            Node run = doc.GetChild(NodeType.Run, 0, true);
            Assert.That(run.Range.Fields.Count, Is.EqualTo(0));

            // Check the initial field count in the document.
            FieldCollection fields = doc.Range.Fields;
            Assert.That(fields.Count, Is.EqualTo(45));

            // Access the first picked field by the indexer and check it.
            Field firstField = fields[2];
            Assert.That(firstField, IsNot.Null());
            Assert.That(firstField.Type, Is.EqualTo(FieldType.FieldSet));

            // Access the last field by the indexer and check it.
            Field lastField = fields[44];
            Assert.That(lastField, IsNot.Null());
            Assert.That(lastField.Type, Is.EqualTo(FieldType.FieldSymbol));

            // Check field access by negative indexes.
            AssertAreEqual(firstField, fields[-43]);
            AssertAreEqual(lastField, fields[-1]);

            // Check field access by out-of-bounds indexes.
            Assert.That(fields[45], Is.Null);
            Assert.That(fields[-46], Is.Null);

            // Check enumeration.
            // 1. Check that all of the enumerated fields belong to the range.
            // 2. Check that each of them is met once per enumeration.
            // 3. Check their total count.
            IList<Field> fieldsToCheck = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fieldsToCheck.Count, Is.EqualTo(45));

            // Check field removal.
            Paragraph firstFieldParagraph = firstField.Start.ParentParagraph;
            string firstFieldCode = firstField.GetFieldCode();
            fields.Remove(firstField);
            Assert.That(fields.Count, Is.EqualTo(44));
            AssertAreEqual(lastField, fields[43]);

            // Check field addition through the model and collection auto-update.
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveTo(firstFieldParagraph);
            firstField = builder.InsertField(firstFieldCode);
            Assert.That(fields.Count, Is.EqualTo(45));
            AssertAreEqual(firstField, fields[2]);
            AssertAreEqual(lastField, fields[44]);

            // Check field removal by index.
            fields.RemoveAt(2);
            Assert.That(fields.Count, Is.EqualTo(44));
            AssertAreEqual(lastField, fields[43]);

            // Check collection clearing.
            fields.Clear();
            Assert.That(fields.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestIndexerAndEnumeratorEquality()
        {
            Document document = TestUtil.Open(@"Fields\Other\FieldCollection.docx");
            FieldCollection fields = document.Range.Fields;

            int index = 0;
            foreach (Field field in fields)
            {
                Field fieldByIndex = fields[index];
                Assert.That(fieldByIndex.Start, Is.SameAs(field.Start));

                index++;
            }

            Assert.That(index, Is.EqualTo(fields.Count));
        }

        [Test]
        [TestCase(FieldType.FieldAuthor, 0)]
        [TestCase(FieldType.FieldIf, 6)]
        [TestCase(FieldType.FieldRef, 3)]
        public void TestLifeIndexer(FieldType fieldType, int childFieldCount)
        {
            Document document = TestUtil.Open(@"Fields\Other\FieldCollection.docx");
            FieldCollection fields = document.Range.Fields;
            int fieldsCount = fields.Count;

            Collections.Generic.HashSetGeneric<FieldStart> iteratedFields = new Collections.Generic.HashSetGeneric<FieldStart>();
            int i = 0;
            while(i < fields.Count)
            {
                Field field = fields[i];
                Assert.That(field, IsNot.Null());

                Assert.That(iteratedFields.Contains(field.Start), Is.False);
                iteratedFields.Add(field.Start);

                if (field.Type == fieldType)
                {
                    field.Remove();
                    continue;
                }

                i++;
            }

            Assert.That(iteratedFields.Count, Is.EqualTo(fieldsCount - childFieldCount));
            Assert.That(fields.Count, Is.EqualTo(fieldsCount - childFieldCount - 1));
        }

        [Test]
        [TestCase(FieldType.FieldAuthor, 0)]
        [TestCase(FieldType.FieldIf, 6)]
        [TestCase(FieldType.FieldRef, 3)]
        public void TestLifeEnumerator(FieldType fieldType, int childFieldCount)
        {
            Document document = TestUtil.Open(@"Fields\Other\FieldCollection.docx");
            FieldCollection fields = document.Range.Fields;
            int fieldsCount = fields.Count;

            Collections.Generic.HashSetGeneric<FieldStart> iteratedFields = new Collections.Generic.HashSetGeneric<FieldStart>();
            foreach (Field field in fields)
            {
                Assert.That(field, IsNot.Null());

                Assert.That(iteratedFields.Contains(field.Start), Is.False);
                iteratedFields.Add(field.Start);

                if (field.Type == fieldType)
                    field.Remove();
            }

            Assert.That(iteratedFields.Count, Is.EqualTo(fieldsCount - childFieldCount));
            Assert.That(fields.Count, Is.EqualTo(fieldsCount - childFieldCount - 1));
        }
    }
}
