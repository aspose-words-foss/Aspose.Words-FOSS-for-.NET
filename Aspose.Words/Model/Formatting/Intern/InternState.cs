// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Alexey Morozov

namespace Aspose.Words.Formatting.Intern
{
    /// <summary>
    /// Specifies modes for attribute collection states.
    /// </summary>
    internal enum InternState
    {
        /// <summary>
        /// Collection is in normal state.
        /// </summary>
        None,

        /// <summary>
        /// Collection is being referred by <see cref="InternState.Interned" /> collections for actual attribute values.
        /// </summary>
        Pooled,

        /// <summary>
        /// Collection is empty itself and refers to <see cref="InternState.Pooled" /> collection for actual attribute values.
        /// </summary>
        Interned
    }
}
