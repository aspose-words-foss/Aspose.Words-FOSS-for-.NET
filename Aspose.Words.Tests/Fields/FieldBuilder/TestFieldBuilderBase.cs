// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Words.Fields;

namespace Aspose.Words.Tests.Fields
{
    public abstract class TestFieldBuilderBase : TestFieldsBase
    {
        protected Field BuildAndInsert(FieldBuilder fieldBuilder)
        {
            Document document = new Document();

            return BuildAndInsert(fieldBuilder, document);
        }

        protected Field BuildAndInsert(FieldBuilder fieldBuilder, Document document)
        {
             return fieldBuilder.BuildAndInsert(document.FirstSection.Body.FirstParagraph);
        }
    }
}
