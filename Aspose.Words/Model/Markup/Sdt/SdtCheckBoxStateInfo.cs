// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2011 by Alexey Morozov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents state information for <see cref="SdtCheckBox" />.
    /// </summary>
    internal class SdtCheckBoxStateInfo
    {
        internal SdtCheckBoxStateInfo(string fontName, int characterCode)
        {
            mFontName = fontName;
            mCharacterCode = characterCode;
        }

        internal SdtCheckBoxStateInfo Clone()
        {
            SdtCheckBoxStateInfo lhs = (SdtCheckBoxStateInfo)MemberwiseClone();
            return lhs;
        }

        /// <summary>
        /// Font name used in checkbox for certain state.
        /// </summary>
        internal string FontName
        {
            get { return mFontName; }
        }

        /// <summary>
        /// Character code used for certain state.
        /// </summary>
        internal int CharacterCode
        {
            get { return mCharacterCode; }
        }

        /// <summary>
        /// Default checked state info when control created in Office2010.
        /// </summary>
        internal static readonly SdtCheckBoxStateInfo DefaultCheckedStateInfo = new SdtCheckBoxStateInfo("MS Gothic", 0x2612);
        /// <summary>
        /// Default unchecked state info when control created in Office2010.
        /// </summary>
        internal static readonly SdtCheckBoxStateInfo DefaultUncheckedStateInfo = new SdtCheckBoxStateInfo("MS Gothic", 0x2610);

        private readonly string mFontName;
        private readonly int mCharacterCode;
    }
}
