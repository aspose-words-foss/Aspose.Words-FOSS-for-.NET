// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2013 by Ivan Lyagin

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies results of a field update.
    /// </summary>
    [Flags]
    internal enum FieldUpdateResults
    {
        /// <summary>
        /// Specifies that nothing was changed during a field update.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that a field's result was changed during its update.
        /// </summary>
        FieldResultChanged = 1,
        /// <summary>
        /// Specifies that a field's code was changed during its update.
        /// </summary>
        FieldCodeChanged = 2
    }
}
