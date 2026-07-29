// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2016 by Alexander Zhiltsov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// This element specifies that the parent SDT shall be an entity picker when displayed in the document.
    /// </summary>
    internal class SdtEntityPicker : SdtControlProperties
    {
        /// <summary>
        /// Returns SDT type whose properties this class represents.
        /// </summary>
        internal override SdtType Type
        {
            get { return SdtType.EntityPicker; }
        }
    }
}
