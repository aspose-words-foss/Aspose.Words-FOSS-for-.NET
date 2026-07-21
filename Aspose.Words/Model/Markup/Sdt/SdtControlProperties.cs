// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Base class for all Structured Document Tags control-specific properties.
    /// Encapsulates all differences between Sdt controls, allowing to define additional
    /// properties and methods in descendant classes.
    /// </summary>
    /// <remarks>
    /// All common Sdt properties are handled in <see cref="StructuredDocumentTag"/>.
    /// </remarks>
    internal abstract class SdtControlProperties
    {
        internal abstract SdtType Type { get; }

        /// <summary>
        /// Deep clone. You need to override when needed.
        /// </summary>
        internal virtual SdtControlProperties Clone()
        {
            return (SdtControlProperties)MemberwiseClone();
        }
    }
}
