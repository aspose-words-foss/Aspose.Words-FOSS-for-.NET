// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a version vector (a version number) of <see cref="Feature"/> supported by HTML import.
    /// </summary>
    internal abstract class VersionVector
    {
        /// <summary>
        /// Parses a string into a version vectors.
        /// </summary>
        /// <param name="text">A string to parse.</param>
        /// <returns>A parsed version vector, either supported or unsupported. The result is never <c>null</c>.</returns>
        internal static VersionVector Parse(string text)
        {
            NumericVersionVector parsedNumericVector = NumericVersionVectorParser.Parse(text);
            return (parsedNumericVector != null)
                ? (VersionVector)parsedNumericVector
                : new UnsupportedVersionVector(text);
        }

        /// <summary>
        /// Compares this version vector to other vector using the specified comparison operation.
        /// </summary>
        internal abstract bool CompareTo(VersionVector other, ComparisonOperation operation);
    }
}
