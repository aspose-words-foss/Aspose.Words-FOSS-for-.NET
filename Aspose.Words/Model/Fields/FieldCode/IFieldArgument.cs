// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2013 by Ivan Lyagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a field code part that is interpreted as a field argument in some scenarios.
    /// This is typically a real field argument or a string representation of a field type.
    /// </summary>
    internal interface IFieldArgument
    {
        /// <summary>
        /// Invalidates a cached string representation of the argument it to be re-read on the next demand if any.
        /// </summary>
        [JavaThrows(true)]
        void InvalidateText();
    }
}
