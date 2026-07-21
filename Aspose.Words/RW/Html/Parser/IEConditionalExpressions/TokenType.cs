// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Types of tokens into which conditional expressions are split during parsing.
    /// </summary>
    internal enum TokenType
    {
        EndOfText,
        Error,
        Whitespace,
        OpenParenthesis,
        CloseParenthesis,
        Not,
        And,
        Or,
        Identifier,
        Version
    }
}
