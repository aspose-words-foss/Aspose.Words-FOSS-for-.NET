// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies the meaning of display range required by layout engine.
    /// </summary>
    internal enum FieldDisplayRangeType
    {
        /// <summary>
        /// The display range designates field result.
        /// </summary>
        FieldResult,
        /// <summary>
        /// The display range designates the whole field.
        /// </summary>
        WholeField,
        /// <summary>
        /// The display range was created artificially.
        /// </summary>
        FakeResult
    }
}
