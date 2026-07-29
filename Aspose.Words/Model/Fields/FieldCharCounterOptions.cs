// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2024 by Edward Voronov

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Defines options for the <see cref="FieldCharCounter"/>.
    /// </summary>
    [Flags]
    internal enum FieldCharCounterOptions
    {
        /// <summary>
        /// Default option.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Starts counter within parent field's result.
        /// </summary>
        InFieldResult = 1,

        /// <summary>
        /// Ignores invalid field chars sequence.
        /// </summary>
        SafeMode = 2
    }
}
