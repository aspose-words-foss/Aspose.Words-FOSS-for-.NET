// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldBuilder : TestFieldBuilderBase
    {
        private static readonly List<FieldType> gUnsupportedFieldTypes = new List<FieldType>
        {
            FieldType.FieldCannotParse,
            FieldType.FieldHtmlActiveX,
            FieldType.FieldRefNoKeyword
        };

        private static List<FieldType> SupportedFieldTypes()
        {
            List<FieldType> result = new List<FieldType>();

            foreach (FieldType fieldType in Enum.GetValues(typeof (FieldType)))
            {
                if (gUnsupportedFieldTypes.Contains(fieldType))
                    continue;

                result.Add(fieldType);
            }

            return result;
        }

        private static List<FieldType> UnsupportedFieldTypes()
        {
            List<FieldType> result = new List<FieldType>();

            foreach (FieldType fieldType in gUnsupportedFieldTypes)
                result.Add(fieldType);

            return result;
        }

        [Test]
        [TestCaseSource("SupportedFieldTypes")]
        public void BuilderShouldReturnFieldForSupportedTypes(FieldType fieldType)
        {
            FieldBuilder fieldBuilder = new FieldBuilder(fieldType);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field).OfType(fieldType);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = @"^Field type '.*' is not supported\.$", MatchType = MessageMatch.Regex)]
        [TestCaseSource("UnsupportedFieldTypes")]
        public void BuilderShouldThrowExceptionForUnsupportedTypes(FieldType fieldType)
        {
            new FieldBuilder(fieldType);
        }

        [Test]
        public void BuilderShouldAllowMultipleBuilding()
        {
            Document document = new Document();
            Paragraph paragraph = document.FirstSection.Body.FirstParagraph;

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldPage);

            Field firstField = fieldBuilder.BuildAndInsert(paragraph);
            Field secondField = fieldBuilder.BuildAndInsert(paragraph);

            Assert.That(secondField, IsNot.SameAs(firstField));

            FieldAssert.Field(firstField)
                .Inside(paragraph)
                .Between(null, secondField.Start);

            FieldAssert.Field(secondField)
                .Inside(paragraph)
                .Between(firstField.End, null);
        }
    }
}
