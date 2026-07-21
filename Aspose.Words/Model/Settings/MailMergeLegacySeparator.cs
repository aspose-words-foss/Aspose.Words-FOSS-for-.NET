// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2009 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies the token to separate fields or records in the mail merge data file.
    /// Due to Microsoft Word limitations can be only stored in DOC files.
    /// </summary>
    /// <seealso cref="MailMergeSettings"/>
    internal enum MailMergeLegacySeparator
    {
        None = 0x00,
        Enter = 0x02,
        Tab = 0x06,
        Comma = 0x0a,
        Dot = 0x0b,
        Exclamation = 0x0c,
        Sharp = 0x0d,
        Dollar = 0x0e,
        Percent = 0x0f,
        Ampersand = 0x10,
        ParenthesisLeft = 0x11,
        ParenthesisRight = 0x12,
        Asterisk = 0x13,
        Plus = 0x14,
        Minus = 0x15,
        SlashForward = 0x16,
        Colon = 0x17,
        Semicolon = 0x18,
        LessThan = 0x19,
        Equals = 0x1a,
        GreaterThan = 0x1b,
        QuestionMark = 0x1c,
        CommercialAt = 0x1d,
        BracketLeft = 0x1e,
        BracketRight = 0x1f,
        Power = 0x21,
        Underscore = 0x22,
        Apostrophe = 0x23,
        CurlyBracketLeft = 0x24,
        CurlyBracketRight = 0x25,
        Bar = 0x26,
        Tild = 0x27,
        FieldEnd = 0x46,
        TableCell = 0x47,
        TableRow = 0x48
    }
}
