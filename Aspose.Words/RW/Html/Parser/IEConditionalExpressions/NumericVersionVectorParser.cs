// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2015 by Victor Chebotok

using System;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// A parser for supported version vectors.
    /// </summary>
    /// <remarks>
    /// Supported version vectors are either one or two non-negative 32-bit integer values separated with a single dot.
    /// For example, '9', '9.1', '0.1'.
    /// </remarks>
    internal static class NumericVersionVectorParser
    {
        /// <summary>
        /// Parses text as a numeric version vector.
        /// </summary>
        /// <returns>
        /// Returns either a parsed version vector or <c>null</c> to indicate an error.
        /// </returns>
        internal static NumericVersionVector Parse(string text)
        {
            Match match = Regex.Match(text, @"^[0]*([0-9]+)(?:\.[0]*([0-9]+))?$");
            if (match.Success)
            {
                // Regex captures 3 groups: the text itself, the major part of the version, and its minor part.
                Debug.Assert(match.Groups.Count == 3);

                int majorPart = ParseNonNegativeNumber(match.Groups[1].Value);
                if (majorPart >= 0)
                {
                    // The minor part is optional. Let's check if it is present.
                    string minorPartText = match.Groups[2].Value;
                    if (StringUtil.HasChars(minorPartText))
                    {
                        int minorPart = ParseNonNegativeNumber(minorPartText);
                        if (minorPart >= 0)
                        {
                            return new NumericVersionVector(majorPart, minorPart);
                        }
                    }
                    else
                    {
                        return new NumericVersionVector(majorPart);
                    }
                }
            }

            // Text doesn't match the regex or vector parts cannot be parsed as integers.
            return null;
        }

        /// <summary>
        /// Parses text at the current position as a non-negative 32-bit integer.
        /// </summary>
        /// <returns>
        /// Returns either a parsed integer or -1 to indicate an error.
        /// </returns>
        private static int ParseNonNegativeNumber(string text)
        {
            // Actually, the text comes from a regular expression that doesn't allow for invalid characters and the text
            // is expected to be a sequence of digits. However, we check the sign of the result and catch format exceptions
            // anyway just to be sure there are no logical errors.
            const int error = -1;
            try
            {
                int parsedNumber = FormatterPal.ParseInt(text);
                return (parsedNumber >= 0)
                    ? parsedNumber
                    : error;
            }
            catch (FormatException)
            {
                // If the regex we use to parse vectors is correct, we will never get here.
                return error;
            }
            catch (OverflowException)
            {
                // Using Regex alone, it is impossible to filter out huge numbers that don't fit into 32-bit integers
                // so we have to check for integer overflow here.
                return error;
            }
        }
    }
}
