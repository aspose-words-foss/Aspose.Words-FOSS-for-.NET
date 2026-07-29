// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssDelimToken : CssToken
    {
        internal CssDelimToken(char value)
            : base(CssTokenType.Delim)
        {
            Value = value;
        }

        internal char Value { get; }
    }
}
