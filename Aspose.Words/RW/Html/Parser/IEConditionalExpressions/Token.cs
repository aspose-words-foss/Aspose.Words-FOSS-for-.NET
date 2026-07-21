// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a token into which a conditional expression is split during parsing.
    /// </summary>
    internal class Token
    {
        internal Token(TokenType type, string text)
        {
            Debug.Assert(text != null);
            mType = type;
            mText = text;
        }

        internal TokenType TokenType
        {
            get { return mType; }
        }

        internal string Text
        {
            get { return mText; }
        }

        private readonly TokenType mType;
        private readonly string mText;
    }
}
