// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using System;

namespace Aspose.Words.Fields
{
    internal class FieldEndFieldBuildingBlock : IFieldBuildingBlock
    {
        internal FieldEndFieldBuildingBlock(FieldType fieldType)
        {
            mFieldType = fieldType;
        }

        internal FieldSeparator FieldSeparator { get; private set; }

        internal FieldEnd FieldEnd { get; private set; }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            BuildSeparator(documentBuilder);
            BuildEnd(documentBuilder);
        }

        private void BuildSeparator(DocumentBuilder documentBuilder)
        {
            switch (FieldUtil.GetSeparatorPresence(mFieldType))
            {
                case FieldSeparatorPresence.Never:
                case FieldSeparatorPresence.Sometimes:
                    FieldSeparator = null;
                    break;
                case FieldSeparatorPresence.Always:
                    FieldSeparator = new FieldSeparator(documentBuilder.Document, new RunPr(), mFieldType);
                    documentBuilder.InsertNode(FieldSeparator);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void BuildEnd(DocumentBuilder documentBuilder)
        {
            FieldEnd = new FieldEnd(documentBuilder.Document, new RunPr(), mFieldType, FieldSeparator != null);
            documentBuilder.InsertNode(FieldEnd);
        }

        private readonly FieldType mFieldType;
    }
}