// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies whether a field should have a field separator.
    /// </summary>
    internal enum FieldSeparatorPresence
    {
        /// <summary>
        /// The field never has a separator. Attempting to set its result throws an exception.
        /// </summary>
        Never,
        /// <summary>
        /// The field may or may not have a separator. Attempting to set its result inserts a separator if needed,
        /// but the format readers do not insert it if it is missing.
        /// </summary>
        Sometimes,
        /// <summary>
        /// The field always has a spearator. Attempting to set its result inserts a separator if needed,
        /// and the format readers insert it if it is missing.
        /// </summary>
        Always
    }
}
