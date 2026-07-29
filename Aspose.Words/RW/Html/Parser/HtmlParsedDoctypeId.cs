// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/05/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Utility class containing results of parsing ID string of a DOCTYPE token.
    /// </summary>
    internal class HtmlParsedDoctypeId
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="value">Parsed ID string. Can be <c>null</c>.</param>
        /// <param name="correct">Value indicating whether any parsing errors occurred (<c>true</c> - no errors).</param>
        internal HtmlParsedDoctypeId(string value, bool correct)
        {
            mValue = value;
            mCorrect = correct;
        }

        /// <summary>
        /// Gets the parsed ID string. Can be <c>null</c>.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Gets a value indicating whether any parsing errors occurred (<c>true</c> - no errors).
        /// </summary>
        internal bool Correct
        {
            get { return mCorrect; }
        }

        private readonly string mValue;

        private readonly bool mCorrect;
    }
}
