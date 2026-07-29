// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2011 by Alexey Morozov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Office2010 SDT checkbox control properties.
    /// </summary>
    internal class SdtCheckBox : SdtControlProperties
    {
        internal SdtCheckBox()
        {
            mCheckedStateInfo = SdtCheckBoxStateInfo.DefaultCheckedStateInfo;
            mUncheckedStateInfo = SdtCheckBoxStateInfo.DefaultUncheckedStateInfo;
        }

        internal override SdtType Type
        {
            get { return SdtType.Checkbox; }
        }

        internal override SdtControlProperties Clone()
        {
            SdtCheckBox lhs = (SdtCheckBox)MemberwiseClone();

            lhs.mCheckedStateInfo = mCheckedStateInfo.Clone();
            lhs.mUncheckedStateInfo = mUncheckedStateInfo.Clone();
            return lhs;
        }

        /// <summary>
        /// Indicates if checkbox is in checked state.
        /// </summary>
        internal bool Checked
        {
            get { return mChecked; }
            set { mChecked = value; }
        }

        /// <summary>
        /// Specifies parameters for checked state.
        /// </summary>
        internal SdtCheckBoxStateInfo CheckedStateInfo
        {
            get { return mCheckedStateInfo; }
            set { mCheckedStateInfo = value; }
        }

        /// <summary>
        /// Specifies parameters for unchecked state.
        /// </summary>
        internal SdtCheckBoxStateInfo UncheckedStateInfo
        {
            get { return mUncheckedStateInfo; }
            set { mUncheckedStateInfo = value; }
        }

        private bool mChecked;
        private SdtCheckBoxStateInfo mCheckedStateInfo;
        private SdtCheckBoxStateInfo mUncheckedStateInfo;
    }
}
