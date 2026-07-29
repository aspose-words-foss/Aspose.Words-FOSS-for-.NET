// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssNumberToken : CssToken
    {
        internal CssNumberToken(CssTokenType tokenType, double value, bool isInteger, bool isSigned)
            : base(tokenType)
        {
            Value = value;
            IsInteger = isInteger;
            IsSigned = isSigned;
        }

        internal double Value { get; }

        internal bool IsInteger { get; }

        internal bool IsSigned { get; }
    }
}
