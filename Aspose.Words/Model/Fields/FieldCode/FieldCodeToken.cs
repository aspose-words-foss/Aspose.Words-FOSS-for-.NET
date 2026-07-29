// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies an elementary field code token like a text character or a child field's start or end.
    /// </summary>
    internal enum FieldCodeToken
    {
        /// <summary>
        /// A regular text character.
        /// </summary>
        TextChar,
        /// <summary>
        /// A node whose text cannot be read separately and which should be treated as a separate token.
        /// An example is a shape or a table.
        /// </summary>
        NonTextNode,
        /// <summary>
        /// A paragraph node.
        /// </summary>
        Paragraph,
        /// <summary>
        /// A section node.
        /// </summary>
        Section,
        /// <summary>
        /// The parser is entering a child field's code.
        /// </summary>
        ChildFieldStart,
        /// <summary>
        /// The parser is leaving a child field's code and entering a child field's result.
        /// </summary>
        ChildFieldSeparator,
        /// <summary>
        /// The parser is leaving a child field's result.
        /// </summary>
        ChildFieldEnd
    }
}
