// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the type of a Word document node.
    /// </summary>
    /// <dev>
    /// <p>Enumerated node types are provided as a common denominator to ensure all functionality
    /// is accessible to all clients (including COM applications that might not have full access
    /// to the runtime type information).</p>
    /// </dev>
    [CppEnumEnableMetadata]
    public enum NodeType
    {
        /// <summary>
        /// Indicates all node types. Allows to select all children.
        /// </summary>
        Any,

        /// <summary>
        /// <p>A <see cref="Aspose.Words.Document"/> object that, as the root of the document tree,
        /// provides access to the entire Word document.</p>
        /// <p>A <see cref="Aspose.Words.Document"/> node can have <see cref="Aspose.Words.Section"/> nodes.</p>
        /// </summary>
        Document,

        /// <summary>
        /// <p>A <see cref="Aspose.Words.Section"/> object that corresponds to one section in a Word document.</p>
        /// <p>A <see cref="Aspose.Words.Section"/> node can have <see cref="Aspose.Words.Body"/> and <see cref="Aspose.Words.HeaderFooter"/> nodes.</p>
        /// </summary>
        Section,

        /// <summary>
        /// <p>A <see cref="Aspose.Words.Body"/> object that contains the main text of a section (main text story).</p>
        /// <p>A <see cref="Aspose.Words.Body"/> node can have <see cref="Aspose.Words.Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        Body,

        /// <summary>
        /// <p>A <see cref="Aspose.Words.HeaderFooter"/> object that contains text of a particular header or footer inside a section.</p>
        /// <p>A <see cref="Aspose.Words.HeaderFooter"/> node can have <see cref="Aspose.Words.Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        HeaderFooter,

        /// <summary>
        /// <p>A <see cref="Aspose.Words.Tables.Table"/> object that represents a table in a Word document.</p>
        /// <p>A <see cref="Aspose.Words.Tables.Table"/> node can have <see cref="Aspose.Words.Tables.Row"/> nodes.</p>
        /// </summary>
        Table,

        /// <summary>
        /// <p>A row of a table.</p>
        /// <p>A <see cref="Aspose.Words.Tables.Row"/> node can have <see cref="Aspose.Words.Tables.Cell"/> nodes.</p>
        /// </summary>
        Row,

        /// <summary>
        /// <p>A cell of a table row.</p>
        /// <p>A <see cref="Aspose.Words.Tables.Cell"/> node can have <see cref="Aspose.Words.Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        Cell,

        /// <summary>
        /// <p>A paragraph of text.</p>
        /// <p>A <see cref="Aspose.Words.Paragraph"/> node is a container for inline level elements
        /// <see cref="Aspose.Words.Run"/>,
        /// <see cref="Aspose.Words.Fields.FieldStart"/>,
        /// <see cref="Aspose.Words.Fields.FieldSeparator"/>,
        /// <see cref="Aspose.Words.Fields.FieldEnd"/>,
        /// <see cref="Aspose.Words.Fields.FormField"/>,
        /// <see cref="Aspose.Words.Drawing.Shape"/>,
        /// <see cref="Aspose.Words.Drawing.GroupShape"/>,
        /// <see cref="Aspose.Words.Notes.Footnote"/>,
        /// <see cref="Aspose.Words.Comment"/>,
        /// <see cref="Aspose.Words.SpecialChar"/>,
        /// as well as <see cref="Aspose.Words.BookmarkStart"/> and <see cref="Aspose.Words.BookmarkEnd"/>.</p>
        /// </summary>
        Paragraph,

        /// <summary>
        /// <p>A beginning of a bookmark marker.</p>
        /// </summary>
        BookmarkStart,

        /// <summary>
        /// <p>An end of a bookmark marker.</p>
        /// </summary>
        BookmarkEnd,

        /// <summary>
        /// <p>A beginning of an editable range.</p>
        /// </summary>
        EditableRangeStart,

        /// <summary>
        /// <p>An end of an editable range.</p>
        /// </summary>
        EditableRangeEnd,

        /// <summary>
        /// <p>A beginning of an MoveFrom range.</p>
        /// </summary>
        MoveFromRangeStart,

        /// <summary>
        /// <p>An end of an MoveFrom range.</p>
        /// </summary>
        MoveFromRangeEnd,

        /// <summary>
        /// <p>A beginning of an MoveTo range.</p>
        /// </summary>
        MoveToRangeStart,

        /// <summary>
        /// <p>An end of an MoveTo range.</p>
        /// </summary>
        MoveToRangeEnd,

        /// <summary>
        /// <p>A group of shapes, images, OLE objects or other group shapes.</p>
        /// <p>A <see cref="Aspose.Words.Drawing.GroupShape"/> node can contain other
        /// <see cref="Aspose.Words.Drawing.Shape"/> and <see cref="Aspose.Words.Drawing.GroupShape"/> nodes.</p>
        /// </summary>
        GroupShape,

        /// <summary>
        /// <p>A drawing object, such as an OfficeArt shape, image or an OLE object.</p>
        /// <p>A <see cref="Aspose.Words.Drawing.Shape"/> node can contain <see cref="Aspose.Words.Paragraph"/>
        /// and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        Shape,

        /// <summary>
        /// <p>A comment in a Word document.</p>
        /// <p>A <see cref="Aspose.Words.Comment"/> node can have <see cref="Aspose.Words.Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        Comment,

        /// <summary>
        /// <p>A footnote or endnote in a Word document.</p>
        /// <p>A <see cref="Aspose.Words.Notes.Footnote"/> node can have <see cref="Aspose.Words.Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> nodes.</p>
        /// </summary>
        Footnote,

        /// <summary>
        /// <p>A run of text.</p>
        /// </summary>
        Run,

        /// <summary>
        /// <p>A special character that designates the start of a Word field.</p>
        /// </summary>
        FieldStart,

        /// <summary>
        /// <p>A special character that separates the field code from the field result.</p>
        /// </summary>
        FieldSeparator,

        /// <summary>
        /// <p>A special character that designates the end of a Word field.</p>
        /// </summary>
        FieldEnd,

        /// <summary>
        /// <p>A form field.</p>
        /// </summary>
        FormField,

        /// <summary>
        /// <p>A special character that is not one of the more specific special character types.</p>
        /// </summary>
        SpecialChar,

        /// <summary>
        /// <para>A smart tag around one or more inline structures (runs, images, fields,etc.) within a paragraph</para>
        /// </summary>
        SmartTag,

        /// <summary>
        /// <para>Allows to define customer-specific information and its means of presentation.</para>
        /// </summary>
        StructuredDocumentTag,

        /// <summary>
        /// <para>A start of <b>ranged</b> structured document tag which accepts multi-sections content.</para>
        /// </summary>
        StructuredDocumentTagRangeStart,

        /// <summary>
        /// <para>A end of <b>ranged</b> structured document tag which accepts multi-sections content.</para>
        /// </summary>
        StructuredDocumentTagRangeEnd,

        /// <summary>
        /// <para>A glossary document within the main document.</para>
        /// </summary>
        GlossaryDocument,

        /// <summary>
        /// <para>A building block within a glossary document (e.g. glossary document entry).</para>
        /// </summary>
        BuildingBlock,

        /// <summary>
        /// A marker node that represents the start of a commented range.
        /// </summary>
        CommentRangeStart,

        /// <summary>
        /// A marker node that represents the end of a commented range.
        /// </summary>
        CommentRangeEnd,

        /// <summary>
        /// <para> An Office Math object. Can be equation, function, matrix or one of other mathematical objects.
        /// Can be a collection of mathematical object and also can contain some non-mathematical objects such as runs of text.</para>
        /// </summary>
        OfficeMath, 

        /// <summary>
        /// A subdocument node which is a link to another document.
        /// </summary>
        SubDocument,

        /// <summary>
        /// Reserved for internal use by Aspose.Words.
        /// </summary>
        System,

        /// <summary>
        /// Reserved for internal use by Aspose.Words.
        /// </summary>
        Null
    }
}
