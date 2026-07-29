// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2021 by Mikhail Nepreteamov

using Aspose.Collections.Generic;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown FootnoteReference block.
    /// </summary>
    internal class FootnoteReferenceBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            Debug.Assert(IsOpened && (block != null));

            Add(block);
            return true;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            Debug.Assert(context != null);

            // If there is no an appropriate FootnoteDefinition, then write it as just a simple inline text.
            if (DefinitionBlock == null)
            {
                context.Builder.Write(Text);
            }
            else
            {
                context.Open(this);
                foreach (Block definitionBlockChild in DefinitionBlock)
                    definitionBlockChild.Write(context);
                context.Close(this);
            }
        }

        /// <summary>
        /// Returns true if the specified delimiters can constitute a valid Footnote reference.
        /// </summary>
        internal static bool IsValid(FootnoteOpeningDelimiter opening, Delimiter closing)
        {
            Debug.Assert((opening != null) && (closing != null) && opening.IsBefore(closing));

            return ((opening.LinkTextOpening != null) && IsValid(opening.Text, opening.End + 1, closing.Start - 1));
        }

        /// <summary>
        /// Returns true if the specified text within the specified bounds can contain a valid Footnote reference.
        /// </summary>
        internal static bool IsValid(string text, int startIndex, int endIndex)
        {
            const int minReferenceLength = 1;

            Debug.Assert((text != null) && (startIndex >= 0) && (startIndex < text.Length) &&
                (endIndex >= 0) && (endIndex < text.Length));

            if ((endIndex - startIndex + 1) < minReferenceLength)
                return false;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (char.IsWhiteSpace(text[i]) || ArrayUtil.FindCharInArray(gUnallowableSpecialChars, text[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Resolves this FootnoteReference by binding it with the first occurred in a specified collection
        /// of FootnoteDefinitions with the same Reference.
        /// </summary>
        internal void Resolve(HashSetGeneric<FootnoteDefinitionBlock> footnoteDefinitionBlocks)
        {
            Debug.Assert(footnoteDefinitionBlocks != null);

            // The different markdown editors process definitions with duplicated references in a different ways.
            // As we are attracting preferably to GitLab, we take the first Definition we encounter.
            foreach (FootnoteDefinitionBlock definitionBlock in footnoteDefinitionBlocks)
            {
                if (SetDefinition(definitionBlock))
                    break;
            }
        }

        /// <summary>
        /// Sets FootnoteDefinitionBlock to this object if the References are equal.
        /// </summary>
        private bool SetDefinition(FootnoteDefinitionBlock definitionBlock)
        {
            Debug.Assert(definitionBlock != null);

            if (definitionBlock.Reference != Reference)
                return false;

            DefinitionBlock = definitionBlock;
            return true;
        }

        /// <summary>
        /// Gets text of the block.
        /// </summary>
        internal override string Text
        {
            get { return string.Format("{0}{1}{2}", OpeningDelimiter, Reference, ClosingDelimiter); }
        }

        /// <summary>
        /// A type of the block.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.FootnoteReference; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Gets string representing a Reference part of this FootnoteReferenceBlock.
        /// </summary>
        /// <remarks>This is a text between opening [^ and closing ]: delimiters.</remarks>
        private string Reference
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return base.Text; }
        }

        /// <summary>
        /// Gets a corresponding FootnoteDefinition block.
        /// </summary>
        private FootnoteDefinitionBlock DefinitionBlock { get; set; }

        /// <summary>
        /// Unallowable special characters.
        /// </summary>
        private static readonly char[] gUnallowableSpecialChars = { '~', '`', '!', ':', '&', '*', '_', '<', '\\', '[',
            ']', '{', '}' };

        /// <summary>
        /// The Opening delimiter.
        /// </summary>
        internal const string OpeningDelimiter = "[^";

        /// <summary>
        /// The Closing delimiter.
        /// </summary>
        internal const string ClosingDelimiter = "]";
    }
}
