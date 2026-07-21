// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class TextFieldBuildingBlock : IFieldBuildingBlock
    {
        internal TextFieldBuildingBlock(string text)
        {
            mText = text;
        }

        void IFieldBuildingBlock.BuildBlock(DocumentBuilder documentBuilder)
        {
            documentBuilder.Write(mText);
        }

        private readonly string mText;
    }
}