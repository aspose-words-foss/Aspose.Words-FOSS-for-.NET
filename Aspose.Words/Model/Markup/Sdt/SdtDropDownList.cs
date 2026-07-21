// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies that the parent sdt shall be a drop-down list when displayed in the document.
    /// </summary>
    internal class SdtDropDownList : SdtDropDownListBase
    {
        internal override SdtType Type
        {
            get { return SdtType.DropDownList; }
        }
    }
}
