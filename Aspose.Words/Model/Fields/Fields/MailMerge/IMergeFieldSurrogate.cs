// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2009 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implemented by the fields that may act as merge fields, such as IF 0 = 0 and MACROBUTTON.
    /// </summary>
    internal interface IMergeFieldSurrogate
    {
        /// <summary>
        /// Returns field start.
        /// </summary>
        FieldStart Start { get; }

        /// <summary>
        /// Returns field separator.
        /// </summary>
        FieldSeparator Separator { get; }

        /// <summary>
        /// Returns field end.
        /// </summary>
        FieldEnd End { get; }

        /// <summary>
        /// Gets whether the field requires value to act as a merge field.
        /// </summary>
        bool IsMergeValueRequired();

        /// <summary>
        /// Gets the name of the merge field used to retrieve a value from the data source.
        /// </summary>
        [JavaThrows(true)]
        string GetMergeFieldName();

        /// <summary>
        /// Gets whether the field may act as a merge field.
        /// </summary>
        [JavaThrows(true)]
        bool CanWorkAsMergeField();
    }
}
