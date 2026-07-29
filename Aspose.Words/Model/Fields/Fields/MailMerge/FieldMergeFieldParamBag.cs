// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2014 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents an result of the MERGEFIELD field code parsing.
    /// </summary>
    internal class FieldMergeFieldParamBag
    {
        internal string Prefix { get; set; }

        internal string FieldName { get; set; }

        internal FieldMergeField.MergeFieldType MergeFieldType { get; set; }

        internal MergeFieldImageDimension ImageWidth { get; set; }

        internal MergeFieldImageDimension ImageHeight { get; set; }

        internal string FieldNameWithPrefix
        {
            get
            {
                return FieldMergeField.GetMergeFieldName(Prefix, FieldName);
            }
        }
    }
}
