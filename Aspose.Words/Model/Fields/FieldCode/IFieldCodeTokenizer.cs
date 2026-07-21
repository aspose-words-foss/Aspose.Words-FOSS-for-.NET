// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/12/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, represents a tokenizer that enumerates over field code tokens.
    /// </summary>
    internal interface IFieldCodeTokenizer
    {
        /// <summary>
        /// Proceeds to the next field code token, such as a text character, child field's start or end, etc.
        /// </summary>
        /// <returns>True if successful, false if passed the last field code token.</returns>
        bool MoveToNextToken();

        /// <summary>
        /// Returns a document position that corresponds to the current position of the tokenizer.
        /// </summary>
        /// <returns>A document position that corresponds to the current position of the tokenizer.</returns>
        DocumentPosition GetCurrentPosition();

        /// <summary>
        /// Gets the current field code token.
        /// </summary>
        FieldCodeToken CurrentToken { get; }

        /// <summary>
        /// Gets whether the current position is at the end of the current token.
        /// </summary>
        bool IsEndOfToken { get; }

        /// <summary>
        /// Gets the current field char.
        /// </summary>
        /// <remarks>
        /// Only makes sense if <see cref="CurrentToken"/> is <see cref="FieldCodeToken.TextChar"/>.
        /// </remarks>
        char CurrentChar { get; }

        /// <summary>
        /// Gets the current field node.
        /// </summary>
        /// <remarks>
        /// It should not be used if <see cref="CurrentToken"/> is <see cref="FieldCodeToken.TextChar"/>.
        /// </remarks>
        Node CurrentNode { get; }

        /// <summary>
        /// A boolean value indicating if the tokenizer reached the end of data;
        /// </summary>
        bool IsEof { get; }

        /// <summary>
        /// Locale ID specified via formatting properties of the current Inline node.
        /// </summary>
        /// <remarks>
        /// If the current node is not defined or is not Inline, <see cref="RunPr.ProcessOrUserDefaultLanguageId"/> is returned.
        /// </remarks>
        int CurrentLocaleId { get; }

        /// <summary>
        /// Locale ID of the East Asian language specified via formatting properties of the current Inline node.
        /// </summary>
        /// <remarks>
        /// If the current node is not defined or is not Inline, <see cref="RunPr.ProcessOrUserDefaultLanguageId"/> is returned.
        /// </remarks>
        int CurrentLocaleIdFarEast { get; }

        /// <summary>
        /// Locale ID of the RTL specified via formatting properties of the current Inline node.
        /// </summary>
        /// <remarks>
        /// If the current node is not defined or is not Inline, <see cref="RunPr.ProcessOrUserDefaultLanguageId"/> is returned.
        /// </remarks>
        int CurrentLocaleIdBi { get; }

        /// <summary>
        /// A boolean value indicating if RTL is specified via formatting properties of the current Inline node.
        /// </summary>
        bool CurrentBidi { get; }

        /// <summary>
        /// A font name specified via formatting properties of the current Inline node.
        /// </summary>
        /// <remarks>
        /// If the current node is not defined or is not Inline, <c>null</c> is returned.
        /// </remarks>
        string CurrentFontName { get; }
    }
}
