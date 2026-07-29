// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2021 by Alexey Maslov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Lowercase Roman style numbering like i. ii. iii.
    /// </summary>
    internal class LowercaseRomanDotTxtNumberingStyle : RomanTxtNumberingStyleBase
    {
        internal LowercaseRomanDotTxtNumberingStyle()
            : base(TxtLettersHelper.gLowercaseRomanNumbers, '.', NumberStyle.LowercaseRoman)
        {
        }
    }
}
