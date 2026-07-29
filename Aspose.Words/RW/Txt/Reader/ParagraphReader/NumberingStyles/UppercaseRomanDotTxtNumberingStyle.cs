// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2021 by Alexey Maslov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Uppercase Roman style numbering like I. II. III.
    /// </summary>
    internal class UppercaseRomanDotTxtNumberingStyle : RomanTxtNumberingStyleBase
    {
        internal UppercaseRomanDotTxtNumberingStyle()
            : base(TxtLettersHelper.gUppercaseRomanNumbers, '.', NumberStyle.UppercaseRoman)
        {
        }
    }
}
