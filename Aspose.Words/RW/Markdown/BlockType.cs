// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Enumerates all possible types of markdown blocks.
    /// </summary>
    internal enum BlockType : byte
    {
        Document,

        Inline,
        BoldInline,
        ItalicInline,
        InlineCode,
        Strikethrough,
        Underline,
        Autolink,
        ImageDescription,
        LinkText,
        LinkDestination,
        FootnoteReference,

        BlankLine,
        EndOfLine,

        Paragraph,
        HorizontalRule,
        AtxHeading,
        SetextHeading,
        Quote,
        Cell,
        Row,
        Table,
        FootnoteDefinition,
	    LinkDefinition,

        IndentedCode,
        FencedCode,

        BulletListItem,
        OrderedListItem,

        DelimiterRun,

        HtmlTag,
    }
}
