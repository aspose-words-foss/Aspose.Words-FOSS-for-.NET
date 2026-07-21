// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2019 by Edward Voronov

using Aspose.Words.Fields;

namespace Aspose.Words
{
    internal class FieldRemoverDefaultFilter : IFieldRemoverFilter
    {
        private FieldRemoverDefaultFilter()
        {
        }

        bool IFieldRemoverFilter.NeedReplaceFieldWithResult(FieldChar fieldChar)
        {
            return true;
        }

        internal static readonly IFieldRemoverFilter Instance = new FieldRemoverDefaultFilter();
    }
}
