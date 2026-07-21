// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    internal interface IFieldBuildingBlock
    {
        [JavaThrows(true)]
        void BuildBlock(DocumentBuilder documentBuilder);
    }
}