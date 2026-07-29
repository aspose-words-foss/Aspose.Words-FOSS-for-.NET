// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

using System.IO;
using System.Text;

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// A parser for IE conditional expressions.
    /// </summary>
    internal class IEConditionalExpressionsParser
    {
        /// <summary>
        /// Parses a text into a conditional expression.
        /// </summary>
        /// <param name="text">A text to parse.</param>
        /// <returns>Either the parsed expression or <c>null</c> to indicate an error.</returns>
        internal ConditionalExpression Parse(string text)
        {
            Debug.Assert(text != null);
            Debug.Assert(mReader == null);

            ConditionalExpression expression;
            using (mReader = new StringReader(text))
            {
                Advance();
                expression = ParseDisjunction();

                // It is a top-level expression and it must end with an end-of-text token.
                if (mCurrentToken.TokenType != TokenType.EndOfText)
                {
                    // Error. Unexpected token.
                    expression = null;
                }
            }
            mReader = null;

            return expression;
        }

        private ConditionalExpression ParseDisjunction()
        {
            // First part of an expression.
            ConditionalExpression expression = ParseConjunction();
            if (expression == null)
            {
                // Error. Cannot parse the left part.
                return null;
            }

            // An optional sequence of additional parts separated by 'or' operators.
            while (mCurrentToken.TokenType == TokenType.Or)
            {
                Advance();
                ConditionalExpression rightPart = ParseConjunction();
                if (rightPart == null)
                {
                    // Error. Cannot parse the right part.
                    return null;
                }
                expression = new Or(expression, rightPart);
            }

            // This must be either top-level expression (which ends with an end-of-text token) or a subexpression
            // (which ends with a closing parenthesis).
            if ((mCurrentToken.TokenType != TokenType.EndOfText) &&
                (mCurrentToken.TokenType != TokenType.CloseParenthesis))
            {
                // Error. Unexpected token.
                return null;
            }

            Debug.Assert(expression != null);
            return expression;
        }

        private ConditionalExpression ParseConjunction()
        {
            // First part of an expression.
            ConditionalExpression expression = ParsePart();
            if (expression == null)
            {
                // Error. Cannot parse the first part.
                return null;
            }

            // An optional sequence of additional parts separated by 'and' operators.
            while (mCurrentToken.TokenType == TokenType.And)
            {
                Advance();
                ConditionalExpression rightPart = ParsePart();
                if (rightPart == null)
                {
                    // Error. Cannot parse one of additional parts.
                    return null;
                }
                expression = new And(expression, rightPart);
            }

            // An expression can be one of the following:
            //  - a top-level expression, which ends with an end-of-text token;
            //  - a subexpression, which ends with a closing parenthesis;
            //  - a conjunction inside a disjunction sequence, which ends with an 'or' operator.
            if ((mCurrentToken.TokenType != TokenType.EndOfText) &&
                (mCurrentToken.TokenType != TokenType.CloseParenthesis) &&
                (mCurrentToken.TokenType != TokenType.Or))
            {
                // Error. Unexpected token.
                return null;
            }

            Debug.Assert(expression != null);
            return expression;
        }

        private ConditionalExpression ParsePart()
        {
            SkipWhitespace();

            switch (mCurrentToken.TokenType)
            {
                case TokenType.OpenParenthesis:
                {
                    return ParseSubexpression();
                }
                case TokenType.Not:
                {
                    return ParseNegation();
                }
                case TokenType.Identifier:
                {
                    return ParseComparison();
                }
                default:
                {
                    // Error. Unexpected token.
                    return null;
                }
            }
        }

        private ConditionalExpression ParseSubexpression()
        {
            Debug.Assert(mCurrentToken.TokenType == TokenType.OpenParenthesis);

            Advance();
            ConditionalExpression part = ParseDisjunction();
            if ((part == null) ||
                (mCurrentToken.TokenType != TokenType.CloseParenthesis))
            {
                // Error. Cannot parse the part or found an unmatched open parenthesis.
                return null;
            }
            Advance();
            SkipWhitespace();

            Debug.Assert(part != null);
            return part;
        }

        private ConditionalExpression ParseNegation()
        {
            Advance();
            SkipWhitespace();
            ConditionalExpression negatedExpression;
            switch (mCurrentToken.TokenType)
            {
                case TokenType.OpenParenthesis:
                {
                    negatedExpression = ParseSubexpression();
                    break;
                }
                case TokenType.Identifier:
                {
                    negatedExpression = ParseComparison();
                    break;
                }
                default:
                {
                    // Error. Unexpected token.
                    negatedExpression = null;
                    break;
                }
            }
            if (negatedExpression == null)
            {
                // Error. Cannot parse the negated expression.
                return null;
            }
            return new Not(negatedExpression);
        }

        private ConditionalExpression ParseComparison()
        {
            Debug.Assert(mCurrentToken.TokenType == TokenType.Identifier);

            if ((mCurrentToken.Text == "true") || (mCurrentToken.Text == "false"))
            {
                bool value = mCurrentToken.Text == "true";
                Advance();
                SkipWhitespace();
                return new BoolConstant(value);
            }

            // Parse operation. An operation keyword is optional.
            ComparisonOperation operation;
            switch (mCurrentToken.Text)
            {
                case "gt":
                    operation = ComparisonOperation.Greater;
                    break;
                case "gte":
                    operation = ComparisonOperation.GreaterOrEqual;
                    break;
                case "lt":
                    operation = ComparisonOperation.Less;
                    break;
                case "lte":
                    operation = ComparisonOperation.LessOrEqual;
                    break;
                default:
                    operation = ComparisonOperation.Equal;
                    break;
            }
            // Equality comparison operation has no dedicated keyword.
            if (operation != ComparisonOperation.Equal)
            {
                Advance();
                SkipWhitespace();
            }

            // Parse feature. A feature is mandatory.
            if (mCurrentToken.TokenType != TokenType.Identifier)
            {
                // Error. Unexpected token.
                return null;
            }
            string feature = mCurrentToken.Text;
            Advance();

            // Parse version. A version is optional for equality comparison but it is mandatory for other operations.
            SkipWhitespace();
            VersionVector version;
            if (mCurrentToken.TokenType == TokenType.Version)
            {
                version = VersionVector.Parse(mCurrentToken.Text);
                Advance();
            }
            else
            {
                if (operation != ComparisonOperation.Equal)
                {
                    // Error. Unexpected token. Version is mandatory if an operation token is present.
                    return null;
                }
                version = null;
            }

            SkipWhitespace();

            return new Comparison(operation, feature, version);
        }

        /// <summary>
        /// Advances the parser one token forward.
        /// </summary>
        private void Advance()
        {
            mCurrentToken = ParseNextToken();
        }

        /// <summary>
        /// Skips the current token if it is a whitespace.
        /// </summary>
        private void SkipWhitespace()
        {
            if (mCurrentToken.TokenType == TokenType.Whitespace)
            {
                Advance();
            }
            Debug.Assert(mCurrentToken.TokenType != TokenType.Whitespace);
        }

        private Token ParseNextToken()
        {
            int character = mReader.Read();
            switch (character)
            {
                case -1:
                    return new Token(TokenType.EndOfText, string.Empty);
                case '(':
                    return new Token(TokenType.OpenParenthesis, "(");
                case ')':
                    return new Token(TokenType.CloseParenthesis, ")");
                case '!':
                    return new Token(TokenType.Not, "!");
                case '&':
                    return new Token(TokenType.And, "&");
                case '|':
                    return new Token(TokenType.Or, "|");
                default:
                    break;
            }
            if (StringUtil.IsWhiteSpace(character))
            {
                return new Token(TokenType.Whitespace, ReadWhitespace((char)character));
            }
            if (StringUtil.IsLetter(character))
            {
                return new Token(TokenType.Identifier, ReadIdentifier((char)character));
            }
            if (StringUtil.IsDigit(character))
            {
                return new Token(TokenType.Version, ReadVersion((char)character));
            }
            return new Token(TokenType.Error, string.Empty);
        }

        private string ReadWhitespace(char firstCharacter)
        {
            StringBuilder result = new StringBuilder();
            result.Append(firstCharacter);
            while (true)
            {
                int character = mReader.Peek();
                if (!StringUtil.IsWhiteSpace(character))
                {
                    break;
                }
                result.Append((char)character);
                mReader.Read();
            }
            return result.ToString();
        }

        private string ReadIdentifier(char firstCharacter)
        {
            StringBuilder result = new StringBuilder();
            result.Append(AsciiLowerCase(firstCharacter));
            while (true)
            {
                int character = mReader.Peek();
                if (!StringUtil.IsLetterOrDigit(character))
                {
                    break;
                }
                result.Append(AsciiLowerCase((char)character));
                mReader.Read();
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts the character to lowercase if it is an ASCII uppercase letter, otherwise leaves it unchanged.
        /// </summary>
        private static char AsciiLowerCase(char c)
        {
            const int diff = 'a' - 'A';
            return ((c >= 'A') && (c <= 'Z'))
                ? (char)(c + diff)
                : c;
        }

        private string ReadVersion(char firstCharacter)
        {
            StringBuilder result = new StringBuilder();
            result.Append(firstCharacter);
            while (true)
            {
                int character = mReader.Peek();
                if ((!StringUtil.IsLetterOrDigit(character)) && (character != '.'))
                {
                    break;
                }
                result.Append((char)character);
                mReader.Read();
            }
            return result.ToString();
        }

        private StringReader mReader;
        private Token mCurrentToken;
    }
}
