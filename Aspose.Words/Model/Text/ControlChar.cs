// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Control characters often encountered in documents.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-control-characters/">Working With Control Characters</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Provides both char and string versions of the same constants. For example:
    /// string <see cref="LineBreak"/> and char <see cref="LineBreakChar"/> have the same value.
    /// </remarks>
    [CppConstexpr]
    public static class ControlChar
    {
        /// <summary>
        /// Placeholder for an inline picture: (char)1.
        /// At the moment occurs in the model in the FormField node. Ideally should not occur in the model.
        /// </summary>
        internal const char PictureChar = (char)1;

        /// <summary>
        /// Footnote reference character: (char)2.
        /// Occurs in the model inside text of a footnote node. Represents a placeholder where
        /// MS Word will display the footnote number.
        /// </summary>
        internal const char FootnoteRefChar = (char)2;
        /// <summary>
        /// Occurs in the model in <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.
        /// </summary>
        internal const char FootnoteSeparatorChar = (char)3;
        /// <summary>
        /// Occurs in the model in <see cref="Aspose.Words.Notes.FootnoteSeparator"/>.
        /// </summary>
        internal const char FootnoteContinuationChar = (char)4;
        /// <summary>
        /// Annotation (comment) reference character: (char)5.
        ///
        /// Represents a placeholder where MS Word will display the author name and comment number.
        /// Normally occurs as the first character, but can be moved to anywhere in the comment text.
        /// If this character is omitted, MS Word will display the author name at the beginning of the comment text.
        ///
        /// In OOXML this is not a character, but a w:annotationRef element. Maybe we should convert
        /// it into an element in the model as well? But for consistency we will need to create similar
        /// classes for footnotes and endnotes.
        /// </summary>
        internal const char AnnotationRefChar = (char)5;
        /// <summary>
        /// End of a table cell or end of a table row character: (char)7 or "\a".
        /// </summary>
        public const char CellChar = (char)7;
        /// <summary>
        /// Tab character: (char)9 or "\t".
        /// </summary>
        public const char TabChar = (char)9;
        /// <summary>
        /// Line feed character: (char)10 or "\n".
        /// </summary>
        public const char LineFeedChar = (char)10;
        /// <summary>
        /// Line break character: (char)11 or "\v".
        /// </summary>
        public const char LineBreakChar = (char)11;
        /// <summary>
        /// Page break character: (char)12 or "\f".
        /// </summary>
        public const char PageBreakChar = (char)12;
        /// <summary>
        /// End of section character: (char)12 or "\f".
        /// </summary>
        public const char SectionBreakChar = (char)12;
        /// <summary>
        /// End of paragraph character: (char)13 or "\r".
        /// </summary>
        public const char ParagraphBreakChar = (char)13;
        /// <summary>
        /// End of column character: (char)14.
        /// </summary>
        public const char ColumnBreakChar = (char)14;
        /// <summary>
        /// Start of MS Word field character: (char)19.
        /// </summary>
        public const char FieldStartChar = (char)19;
        /// <summary>
        /// Field separator character separates field code from field value. Optional in some fields. Value: (char)20.
        /// </summary>
        public const char FieldSeparatorChar = (char)20;
        /// <summary>
        /// End of MS Word field character: (char)21.
        /// </summary>
        public const char FieldEndChar = (char)21;
        /// <summary>
        /// Non-breaking Hyphen in Microsoft Word is (char)30.
        /// </summary>
        /// <remarks>
        /// <p>Non-breaking Hyphen in Microsoft Word does not correspond to the
        /// Unicode character U+2011 non-breaking hyphen but instead represents
        /// internal information that tells Microsoft Word to display a hyphen and not to break a line.</p>
        /// <p>Useful info: http://www.cs.tut.fi/~jkorpela/dashes.html#linebreaks.</p>
        /// </remarks>
        public const char NonBreakingHyphenChar = (char)30;
        /// <summary>
        /// Optional Hyphen in Microsoft Word is (char)31.
        /// </summary>
        /// <remarks>
        /// <p>Optional Hyphen in Microsoft Word does not correspond to the Unicode character U+00AD soft hyphen.
        /// Instead, it inserts internal information that tells Word about a possible hyphenation point.</p>
        /// </remarks>
        public const char OptionalHyphenChar = (char)31;
        /// <summary>
        /// Space character: (char)32.
        /// </summary>
        public const char SpaceChar = (char)32;

        /// <summary>
        /// The start and end characters of structured document tag (in Model it's custom xml).
        /// </summary>
        internal const char SdtStartChar = (char)0x3C;
        internal const char SdtEndChar = (char)0x3E;

        /// <summary>
        /// \ - backslash.
        /// </summary>
        internal const char BackslashChar = '\\';
        /// <summary>
        /// Non-breaking space character: (char)160.
        /// </summary>
        public const char NonBreakingSpaceChar = '\x00a0';
        /// <summary>
        /// Unicode Yen currency character.
        /// </summary>
        internal const char YenChar = '\x00a5';
        /// <summary>
        /// Soft hyphen character http://www.fileformat.info/info/unicode/char/00ad/index.htm
        /// </summary>
        internal const char UnicodeOptionalHyphenChar = '\x00ad';

        /// <summary>
        /// This is the "o" character used as a default value in text input form fields.
        /// </summary>
        public const char DefaultTextInputChar = '\x2002';

        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2002/index.htm
        /// </summary>
        internal const char EnSpaceChar = '\x2002';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2003/index.htm
        /// </summary>
        internal const char EmSpaceChar = '\x2003';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2005/index.htm
        /// </summary>
        internal const char FourPerEmSpaceChar = '\x2005';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/3000/index.htm
        /// </summary>
        internal const char IdeographicSpaceChar = '\x3000';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/200e/index.htm
        /// </summary>
        internal const char LeftToRightMarkChar = '\x200e';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/200f/index.htm
        /// </summary>
        internal const char RightToLeftMarkChar = '\x200f';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/202a/index.htm
        /// </summary>
        internal const char LeftToRightEmbedding = '\x202A';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/202b/index.htm
        /// </summary>
        internal const char RightToLeftEmbedding = '\x202B';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/202d/index.htm
        /// </summary>
        internal const char LeftToRightOverride = '\x202D';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/202e/index.htm
        /// </summary>
        internal const char RightToLeftOverride = '\x202E';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/202c/index.htm
        /// </summary>
        internal const char PopDirectionalFormatting = '\x202C';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2066/index.htm
        /// </summary>
        internal const char LeftToRightIsolate = '\x2066';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2067/index.htm
        /// </summary>
        internal const char RightToLeftIsolate = '\x2067';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2068/index.htm
        /// </summary>
        internal const char FirstStrongIsolate = '\x2068';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2069/index.htm
        /// </summary>
        internal const char PopDirectionalIsolate = '\x2069';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/061c/index.htm
        /// </summary>
        internal const char ArabicLetterMark = '\x061C';
        /// <summary>
        /// See <see cref="NonBreakingHyphenChar"/>.
        /// </summary>
        internal const char UnicodeNonBreakingHyphenChar = '\x2011';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2013/index.htm
        /// </summary>
        internal const char EnDashChar = '\x2013';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2014/index.htm
        /// </summary>
        internal const char EmDashChar = '\x2014';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/200c/index.htm
        /// a.k.a No-Width Optional Break
        /// </summary>
        internal const char ZeroWidthNonJoinerChar = '\x200c';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/200d/index.htm
        /// a.k.a No-Width Non Break
        /// </summary>
        internal const char ZeroWidthJoinerChar = '\x200d';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/200b/index.htm
        /// </summary>
        internal const char ZeroWidthSpaceChar = '\x200b';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/feff/index.htm
        /// Not sure if MS Word uses this.
        /// </summary>
        internal const char ZeroWithNoBreakSpace = '\xfeff';
        /// <summary>
        /// http://www.fileformat.info/info/unicode/char/2028/index.htm
        /// Unicode character 'LINE SEPARATOR'.
        /// </summary>
        internal const char UnicodeLineSeparator = '\x2028';

        internal const char NoWidthOptionalBreakChar = ZeroWidthNonJoinerChar;
        internal const char NoWidthNonBreakChar = ZeroWidthJoinerChar;

        /// <summary>
        /// Placeholder for an inline picture: "\x0001".
        /// </summary>
        internal static readonly string Picture = PictureChar.ToString();
        /// <summary>
        /// Footnote reference character: "\x0002".
        /// </summary>
        internal static readonly string FootnoteRef = FootnoteRefChar.ToString();
        /// <summary>
        /// Annotation (comment) reference character: "\x0005".
        /// </summary>
        internal static readonly string AnnotationRef = AnnotationRefChar.ToString();
        /// <summary>
        /// End of a table cell or end of a table row character: "\x0007" or "\a".
        /// </summary>
        public static readonly string Cell = CellChar.ToString();
        /// <summary>
        /// Tab character: "\x0009" or "\t".
        /// </summary>
        public static readonly string Tab = TabChar.ToString();
        /// <summary>
        /// Line feed character: "\x000a" or "\n". Same as <see cref="LineFeed"/>.
        /// </summary>
        public static readonly string Lf = LineFeedChar.ToString();
        /// <summary>
        /// Line feed character: "\x000a" or "\n". Same as <see cref="Lf"/>.
        /// </summary>
        public static readonly string LineFeed = LineFeedChar.ToString();
        /// <summary>
        /// Line break character: "\x000b" or "\v".
        /// </summary>
        public static readonly string LineBreak = LineBreakChar.ToString();
        /// <summary>
        /// Page break character: "\x000c" or "\f". Note it has the same value as <see cref="SectionBreak"/>.
        /// </summary>
        public static readonly string PageBreak = PageBreakChar.ToString();
        /// <summary>
        /// End of section character: "\x000c" or "\f". Note it has the same value as <see cref="PageBreak"/>.
        /// </summary>
        public static readonly string SectionBreak = SectionBreakChar.ToString();
        /// <summary>
        /// Carriage return character: "\x000d" or "\r". Same as <see cref="ParagraphBreak"/>.
        /// </summary>
        public static readonly string Cr = ParagraphBreakChar.ToString();
        /// <summary>
        /// End of paragraph character: "\x000d" or "\r". Same as <see cref="Cr"/>
        /// </summary>
        public static readonly string ParagraphBreak = ParagraphBreakChar.ToString();
        /// <summary>
        /// End of column character: "\x000e".
        /// </summary>
        public static readonly string ColumnBreak = ColumnBreakChar.ToString();
        /// <summary>
        /// Carriage return followed by line feed character: "\x000d\x000a" or "\r\n".
        /// Not used as such in Microsoft Word documents, but commonly used in text files for paragraph breaks.
        /// </summary>
        public static readonly string CrLf = Cr + Lf;

        internal static readonly string FieldStart = FieldStartChar.ToString();

        internal static readonly string FieldSeparator = FieldSeparatorChar.ToString();

        internal static readonly string FieldEnd = FieldEndChar.ToString();

        internal static readonly string SdtStart = SdtStartChar.ToString();
        internal static readonly string SdtEnd = SdtEndChar.ToString();

        internal static readonly string Space = SpaceChar.ToString();

        /// <summary>
        /// Non-breaking space character: "\x00a0".
        /// </summary>
        public static readonly string NonBreakingSpace = NonBreakingSpaceChar.ToString();

        /// <summary>
        /// Special char used to designate non-breaking hyphen. Can occur in run's text.
        /// </summary>
        internal static readonly string NonBreakingHyphen = NonBreakingHyphenChar.ToString();
        /// <summary>
        /// Special char used to designate optional hyphen. Can occur in run's text.
        /// </summary>
        internal static readonly string OptionalHyphen = OptionalHyphenChar.ToString();

        internal static readonly string UnicodeNonBreakingHyphen = UnicodeNonBreakingHyphenChar.ToString();
        internal static readonly string UnicodeOptionalHyphen = UnicodeOptionalHyphenChar.ToString();

        internal static readonly string Backslash = BackslashChar.ToString();
        internal static readonly string Yen = YenChar.ToString();

        internal static readonly string DefaultTextInput = DefaultTextInputChar.ToString();
    }
}
