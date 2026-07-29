// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Common ancestor for <see cref="SdtDropDownList"/> and <see cref="SdtComboBox"/>
    /// </summary>
    internal abstract class SdtDropDownListBase : SdtControlProperties
    {
        /// <summary>
        /// Provides access to all list items <see cref="SdtListItem"/> of this <b>Sdt</b>
        /// </summary>
        internal SdtListItemCollection ListItems
        {
            get { return mItems; }
        }

        internal override SdtControlProperties Clone()
        {
            SdtDropDownListBase lhs = (SdtDropDownListBase)MemberwiseClone();
            lhs.mItems = mItems.Clone();
            return lhs;
        }

        private SdtListItemCollection mItems = new SdtListItemCollection();
    }
}
