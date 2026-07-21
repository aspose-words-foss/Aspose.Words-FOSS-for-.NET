// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssHashToken : CssTextToken
    {
        internal CssHashToken(string text, bool isIdentifier)
            : base(CssTokenType.Hash, text)
        {
            IsIdentifier = isIdentifier;
        }

        internal bool IsIdentifier { get; }
    }
}
