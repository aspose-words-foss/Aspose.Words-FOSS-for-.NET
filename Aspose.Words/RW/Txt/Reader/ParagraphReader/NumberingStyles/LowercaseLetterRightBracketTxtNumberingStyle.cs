// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Butalov

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Lowercase latin letter style like a) b) c)
    /// </summary>
    internal class LowercaseLetterRightBracketTxtNumberingStyle : LetterTxtNumberingStyleBase
    {
        internal LowercaseLetterRightBracketTxtNumberingStyle()
            : base(TxtLettersHelper.gLowercaseLatinLetters, ')', NumberStyle.LowercaseLetter)
        {
        }
    }
}