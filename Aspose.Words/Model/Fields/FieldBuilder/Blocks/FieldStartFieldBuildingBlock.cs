// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class FieldStartFieldBuildingBlock : IFieldBuildingBlock
    {
        internal FieldStartFieldBuildingBlock(FieldType fieldType)
        {
            mFieldType = fieldType;
        }

        internal FieldStart FieldStart { get; private set; }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            FieldStart = new FieldStart(documentBuilder.Document, new RunPr(), mFieldType);
            documentBuilder.InsertNode(FieldStart);
        }

        private readonly FieldType mFieldType;
    }
}