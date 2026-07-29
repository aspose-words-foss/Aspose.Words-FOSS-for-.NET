// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Sdt shall be a combo box when displayed in the document.
    /// </summary>
    internal class SdtComboBox : SdtDropDownListBase
    {
        internal override SdtType Type
        {
            get { return SdtType.ComboBox; }
        }
    }
}
