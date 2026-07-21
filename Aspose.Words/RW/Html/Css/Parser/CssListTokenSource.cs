// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2019 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Provides CSS tokens for parsing from a list.
    /// </summary>
    internal class CssListTokenSource : CssTokenSource
    {
        internal CssListTokenSource(IList<CssToken> tokens)
        {
            mTokens = tokens;
            mPosition = -1;
        }

        internal override CssToken Consume()
        {
            if (mPosition < mTokens.Count)
            {
                ++mPosition;
            }
            CssToken result = (mPosition < mTokens.Count)
                ? mTokens[mPosition]
                : CssToken.Eof;
            return result;
        }

        internal override void Reconsume()
        {
            if (mPosition >= 0)
            {
                --mPosition;
            }
        }

        private readonly IList<CssToken> mTokens;
        private int mPosition;
    }
}
