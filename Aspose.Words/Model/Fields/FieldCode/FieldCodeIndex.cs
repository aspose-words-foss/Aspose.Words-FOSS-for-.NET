// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Ivan Lyagin

using System.Text;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Caches the INDEX field code for reusing. See corresponding <see cref="FieldIndex"/> properties for any details.
    /// </summary>
    internal class FieldCodeIndex
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldCodeIndex(FieldIndex field)
        {
            BookmarkName = field.BookmarkName;
            NumberOfColumns = GetNumberOfColumns(field);
            SequenceSeparator = NormalizeSeparator(field.SequenceSeparator, "-");
            PageNumberSeparator = NormalizeSeparator(field.PageNumberSeparator, ", ");
            EntryType = FieldIndexAndTablesUtil.GetIndexEntryType(field.EntryType);
            PageRangeSeparator = NormalizeSeparator(field.PageRangeSeparator, "–");
            CrossReferenceSeparator = NormalizeSeparator(field.CrossReferenceSeparator, ". ");
            PageNumberListSeparator = NormalizeSeparator(field.PageNumberListSeparator, ", ");
            LetterRange = NormalizeLetterRange(field.LetterRange);
            RunSubentriesOnSameLine = field.RunSubentriesOnSameLine;
            SequenceName = field.SequenceName;
            UseYomi = field.UseYomi;
            LanguageId = field.LanguageId;

            SetHeadingData(field.Heading);
        }

        /// <summary>
        /// Gets number of columns according to MS Word limitation.
        /// </summary>
        private static int GetNumberOfColumns(FieldIndex field)
        {
            // TODO MS Word updates a INDEX field with following error if a number of columns switch has not value:
            // Error! Switch argument not specified.

            const int maxNumberOfColumns = 4;
            const int minNumberOfColumns = 0;

            int numberOfColumns = field.HasNumberOfColumnsSwitch
                ? field.NumberOfColumnsAsInt32.GetValueOrDefault(minNumberOfColumns)
                : minNumberOfColumns;

            return System.Math.Min(maxNumberOfColumns, System.Math.Max(minNumberOfColumns, numberOfColumns));
        }

        /// <summary>
        /// Normalizes separator length according to MS Word limitation. If the input value is <c>null</c>,
        /// returns the default separator.
        /// </summary>
        private static string NormalizeSeparator(string separator, string defaultSeparator)
        {
            if (separator == null)
                return defaultSeparator;

            const int maxSeparatorLength = 15;
            return (separator.Length > maxSeparatorLength) ? separator.Substring(0, maxSeparatorLength) : separator;
        }

        /// <summary>
        /// Returns a string consisting of the first and the last upper-cased letter range characters.
        /// If the input value is <c>null</c>, returns <c>null</c>. If the input range is invalid, returns an empty string.
        /// </summary>
        private static string NormalizeLetterRange(string letterRange)
        {
            if (letterRange == null)
                return null;

            letterRange = letterRange.Trim();
            if (letterRange.Length < 2)
                return string.Empty;

            // MS Word checks and uses just the first and the last letters in the argument.
            char firstLetter = char.ToUpperInvariant(letterRange[0]);
            char lastLetter = char.ToUpperInvariant(letterRange[letterRange.Length - 1]);
            if (firstLetter > lastLetter)
                return string.Empty;

            return new string(new char[] { firstLetter, lastLetter });
        }

        /// <summary>
        /// Stes the data used to generate a heading (i.e. static heading string or dynamic heading format string).
        /// </summary>
        private void SetHeadingData(string heading)
        {
            // Empty heading is not written by MS Word.
            if (!StringUtil.HasChars(heading))
                return;

            bool isALetterMet = false;
            StringBuilder formatBuilder = new StringBuilder();

            foreach (char c in heading)
            {
                switch (c)
                {
                    case 'a':
                    case 'A':
                    {
                        if (isALetterMet)
                        {
                            // Process any subsequent A-letter in a common way.
                            formatBuilder.Append(c);
                        }
                        else
                        {
                            // Replace the first A-letter with a letter placeholder.
                            formatBuilder.Append("{0}");
                            isALetterMet = true;
                        }
                        break;
                    }
                    case '{':
                    {
                        // Escape special format character.
                        formatBuilder.Append("{{");
                        break;
                    }
                    case '}':
                    {
                        // Escape special format character.
                        formatBuilder.Append("}}");
                        break;
                    }
                    default:
                    {
                        if (!isALetterMet && char.IsLetter(c))
                        {
                            // MS Word sets the format to a single space character in this case.
                            mHeading = ControlChar.Space;
                            return;
                        }

                        formatBuilder.Append(c);
                        break;
                    }
                }
            }

            // If A-letter is not met at this point then it is considered to be a static heading.
            if (isALetterMet)
            {
                mHeadingFormat = formatBuilder.ToString();
            }
            else
            {
                mHeading = heading;
            }
        }

        /// <summary>
        /// Returns a heading for the given start letter.
        /// </summary>
        internal string GetHeading(char startChar)
        {
            if (mHeading != null)
                return mHeading;

            if (mHeadingFormat != null)
                return string.Format(mHeadingFormat, startChar);

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the field code provides either static or dynamic heading.
        /// </summary>
        internal bool HasHeading
        {
            get { return (mHeading != null) || (mHeadingFormat != null); }
        }

        /// <summary>
        /// Gets the name of the bookmark that marks the portion of the document used to build the index.
        /// </summary>
        internal string BookmarkName { get; }

        /// <summary>
        /// Gets a value indicating whether the name of the bookmark that marks the portion of the document
        /// used to build the index is specified via the field code.
        /// </summary>
        internal bool HasBookmarkName
        {
            get { return BookmarkName != null; }
        }

        /// <summary>
        /// Gets the number of columns per page used when building the index.
        /// </summary>
        internal int NumberOfColumns { get; }

        /// <summary>
        /// Gets a value indicating whether the number of columns per page used when building the index
        /// is specified via the field code.
        /// </summary>
        internal bool HasNumberOfColumns
        {
            get { return NumberOfColumns > 0; }
        }

        /// <summary>
        /// Gets the character sequence that is used to separate sequence numbers and page numbers.
        /// </summary>
        internal string SequenceSeparator { get; }

        /// <summary>
        /// Gets the character sequence that is used to separate an index entry and its page number.
        /// </summary>
        internal string PageNumberSeparator { get; }

        /// <summary>
        /// Gets an index entry type used to build the index.
        /// </summary>
        internal int EntryType { get; }

        /// <summary>
        /// Gets the character sequence that is used to separate the start and end of a page range.
        /// </summary>
        internal string PageRangeSeparator { get; }

        /// <summary>
        /// Gets the character sequence that is used to separate cross references and other entries.
        /// </summary>
        internal string CrossReferenceSeparator { get; }

        /// <summary>
        /// Gets the character sequence that is used to separate two page numbers in a page number list.
        /// </summary>
        internal string PageNumberListSeparator { get; }

        /// <summary>
        /// Gets a range of letters to which limit the index. The value is trimmed.
        /// </summary>
        internal string LetterRange { get; }

        /// <summary>
        /// Gets a value indicating whether any range of letters to which limit the index
        /// is specified via the field code.
        /// </summary>
        internal bool HasLetterRange
        {
            get { return LetterRange != null; }
        }

        /// <summary>
        /// Gets a value indicating whether a valid range of letters to which limit the index
        /// is specified via the field code.
        /// </summary>
        internal bool IsLetterRangeValid
        {
            get { return StringUtil.HasChars(LetterRange); }
        }

        /// <summary>
        /// Gets whether run subentries into the same line as the main entry.
        /// </summary>
        internal bool RunSubentriesOnSameLine { get; }

        /// <summary>
        /// Gets the name of a sequence whose number is included with the page number.
        /// </summary>
        internal string SequenceName { get; }

        /// <summary>
        /// Gets a value indicating whether the name of a sequence whose number is included with the page number
        /// is specified via the field code.
        /// </summary>
        internal bool HasSequenceName
        {
            get { return SequenceName != null; }
        }

        /// <summary>
        /// Gets whether to enable the use of yomi text for index entries.
        /// </summary>
        internal bool UseYomi { get; }

        /// <summary>
        /// Gets the language ID used to generate the index.
        /// </summary>
        internal string LanguageId { get; }

        private string mHeading;
        private string mHeadingFormat;
    }
}
