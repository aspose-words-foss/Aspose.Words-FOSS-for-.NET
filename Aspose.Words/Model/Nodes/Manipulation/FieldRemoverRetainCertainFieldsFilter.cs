// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2019 by Edward Voronov

using Aspose.Collections.Generic;
using Aspose.Words.Fields;

namespace Aspose.Words
{
    internal class FieldRemoverRetainCertainFieldsFilter : IFieldRemoverFilter
    {
        internal  FieldRemoverRetainCertainFieldsFilter(params FieldType[] fieldTypes)
        {
            mFieldTypes = new HashSetGeneric<FieldType>();
            foreach (FieldType fieldType in fieldTypes)
                mFieldTypes.Add(fieldType);
        }

        bool IFieldRemoverFilter.NeedReplaceFieldWithResult(FieldChar fieldChar)
        {
            return !mFieldTypes.Contains(fieldChar.FieldType);
        }

        private readonly HashSetGeneric<FieldType> mFieldTypes;
    }
}
