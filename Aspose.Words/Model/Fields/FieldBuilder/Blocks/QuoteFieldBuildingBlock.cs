// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class QuoteFieldBuildingBlock : TextFieldBuildingBlock
    {
        private QuoteFieldBuildingBlock()
            : base("\"")
        {
        }

        internal static readonly IFieldBuildingBlock Instance = new QuoteFieldBuildingBlock();
    }
}