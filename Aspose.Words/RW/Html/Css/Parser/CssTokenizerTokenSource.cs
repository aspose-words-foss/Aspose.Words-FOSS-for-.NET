// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Provides CSS tokens for parsing by taking them from a CSS tokenizer that processes a CSS text.
    /// </summary>
    internal class CssTokenizerTokenSource : CssTokenSource
    {
        internal CssTokenizerTokenSource(string css)
        {
            mTokenizer = new CssTokenizer(css);
        }

        internal override CssToken Consume()
        {
            if (!mReconsumed)
            {
                mTokenizer.MoveNext();
            }
            mReconsumed = false;
            return mTokenizer.CurrentToken;
        }

        internal override void Reconsume()
        {
            Debug.Assert(!mReconsumed);
            mReconsumed = true;
        }

        private readonly CssTokenizer mTokenizer;
        private bool mReconsumed;
    }
}
