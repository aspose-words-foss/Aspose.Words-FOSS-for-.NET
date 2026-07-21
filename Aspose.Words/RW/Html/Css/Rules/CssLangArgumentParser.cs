// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Parses arguments of the :lang() pseudo-class.
    /// </summary>
    /// <remarks>
    /// An argument must be a valid CSS identifier, optionally surrounded by whitespace.
    /// See http://www.w3.org/TR/css3-selectors/#lang-pseudo 
    /// </remarks>
    internal class CssLangArgumentParser
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tokens">Tokens of the argument.</param>
        private CssLangArgumentParser(IList<CssToken> tokens)
        {
            Debug.Assert(tokens != null);
            mTokens = tokens;
        }

        /// <summary>
        /// Parses the tokens into a pseudo-class argument.
        /// </summary>
        /// <returns>The parsed argument or <c>null</c> in case of an error.</returns>
        internal static string Parse(IList<CssToken> tokens)
        {
            CssLangArgumentParser parser = new CssLangArgumentParser(tokens);
            return parser.Run();
        }

        /// <summary>
        /// Parses the tokens into a pseudo-class argument.
        /// </summary>
        /// <returns>The parsed argument or <c>null</c> in case of an error.</returns>
        private string Run()
        {
            mTokenIndex = 0;
            if ((mTokens == null) || (mTokenIndex >= mTokens.Count))
            {
                // Parsing error. The list of argument tokens is empty.
                return null;
            }

            // Pre-fetch the current token.
            mCurrentToken = (mTokenIndex < mTokens.Count)
                ? mTokens[mTokenIndex]
                : CssToken.Eof;

            // Optional leading whitespace.
            SkipOptionalWhitespace();

            // The argument token, which must be an identifier.
            if (mCurrentToken.Type != CssTokenType.Ident)
            {
                // Parsing error. Unexpected token.
                return null;
            }

            string result = ((CssTextToken)mCurrentToken).Text;
            if (result.Length == 0)
            {
                // Parsing error. Unexpected token value.
                return null;
            }
            Advance();

            // Optional trailing whitespace.
            SkipOptionalWhitespace();

            if (!mCurrentToken.IsEof())
            {
                // Parsing error. There are some additional tokens after the parsed argument.
                return null;
            }

            return result;
        }

        private void SkipOptionalWhitespace()
        {
            if (mCurrentToken.IsWhitespace())
            {
                Advance();
            }
        }

        /// <summary>
        /// Go to the next token of the argument.
        /// </summary>
        private void Advance()
        {
            if (mTokenIndex < mTokens.Count)
            {
                ++mTokenIndex;
                mCurrentToken = (mTokenIndex < mTokens.Count)
                    ? mTokens[mTokenIndex]
                    : CssToken.Eof;
            }
            else
            {
                Debug.Fail("Attempt to read past the end of a token list.");
            }
        }

        /// <summary>
        /// The index of the token being parsed currently.
        /// </summary>
        private int mTokenIndex;

        /// <summary>
        /// Tokens of the argument.
        /// </summary>
        private readonly IList<CssToken> mTokens;

        /// <summary>
        /// The token being parsed currently.
        /// </summary>
        private CssToken mCurrentToken;
    }
}
