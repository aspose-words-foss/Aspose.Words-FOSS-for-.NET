// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssDimensionToken : CssNumberToken
    {
        internal CssDimensionToken(double value, string unit, bool isInteger, bool isSigned)
            : base(CssTokenType.Dimension, value, isInteger, isSigned)
        {
            Unit = unit;
        }

        internal string Unit { get; }
    }
}
