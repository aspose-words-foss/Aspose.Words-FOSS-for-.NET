// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2009 by Dmitry Vorobyev

using System;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Stores data about expression parsing state.
    /// </summary>
    internal class ExpressionContext
    {
        internal ExpressionContext(string expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Increments the position.
        /// </summary>
        internal void NextChar()
        {
            Position++;
        }

        /// <summary>
        /// Compare the specified string with the expression string starting at the current position.
        /// If the comparison is successful, advances the position by the length of the string.
        /// </summary>
        /// <param name="s">The string to compare.</param>
        /// <returns>True if the comparison is successful.</returns>
        internal bool CompareAndAdvanceIfSuccessful(string s)
        {
            if (string.Compare(Expression, Position, s, 0, s.Length, StringComparison.Ordinal) == 0)
            {
                Position += s.Length;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void SkipWhitespace()
        {
            while (!IsEof && char.IsWhiteSpace(CurrentChar))
                NextChar();
        }

        /// <summary>
        /// Finds any of the specified characters starting from the current position.
        /// </summary>
        /// <remarks>
        /// After successful search the <see cref="Position"/> is moved to the found character.
        /// After unsuccessful search the <see cref="IsEof"/> is <c>true</c>.
        /// </remarks>
        /// <param name="searchChars">Characters to search.</param>
        /// <returns><c>True</c>, if one of search character was found; otherwise, <c>false</c>.</returns>
        internal bool FindAny(char[] searchChars)
        {
            Position = Expression.IndexOfAny(searchChars, Position);
            if (Position < 0)
            {
                Position = Expression.Length;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        internal string Expression { get; }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        internal int Position { get; private set; }

        /// <summary>
        /// Gets the current character.
        /// </summary>
        internal char CurrentChar
        {
            get { return Expression[Position]; }
        }

        /// <summary>
        /// Gets whether the end of the expression string is reached or the formula is null.
        /// </summary>
        internal bool IsEof
        {
            get { return (Expression == null) || (Position >= Expression.Length); }
        }
    }
}
