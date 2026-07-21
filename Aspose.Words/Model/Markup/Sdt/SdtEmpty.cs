// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Empty SDT.
    /// </summary>
    /// <remarks>
    /// Specification allows cases when Sdt has no associated <see cref="SdtType"/>. In this case we store 
    /// instance of this class in the parent <see cref="StructuredDocumentTag"/>.</remarks>
    internal class SdtEmpty : SdtControlProperties
    {
        internal override SdtType Type
        {
            get { return SdtType.None; }
        }
    }
}
