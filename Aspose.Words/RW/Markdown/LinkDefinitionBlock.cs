// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2022 by Vadim Saltykov

using System;
using System.Globalization;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Link Reference Definition block.
    /// </summary>
    internal class LinkDefinitionBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            string text = TrimInPlace(txtLine);

            if (!Parse(text, start))
                return false;

            if (Opening != "" && Closing == "")
                return true;

            if ((Closing != ""))
            {
                Reference = NormalizeString(text.Substring(1, mClosingIndex - 1)).ToUpper(CultureInfo.InvariantCulture);
                if (MarkdownUtil.HasNonEscapedCharacters(Reference, 0, Reference.Length - 1, '[', ']'))
                    return false;

                int destinationIndex = mClosingIndex + ClosingDelimiter.Length;
                Destination = text.Substring(destinationIndex, text.Length - destinationIndex).TrimStart();

                if (LinkDestinationBlock.IsValid(Destination.TrimStart(), 0, Destination.Length - 1) ||
                    LinkDestinationBlock.IsValid(Destination.TrimStart() + "'", 0, Destination.Length))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            // LinkDefinitionBlock can only be appended externally.
            return false;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            // LinkDefinitionBlock is not written into a model.
        }

        /// <summary>
        /// Gets an opening part from a text line starting at a specified position.
        /// </summary>
        protected override string GetOpening(string txtLine, int start)
        {
            // Check the Opening fits in txtLine.
            if ((start + OpeningDelimiter.Length) > txtLine.Length)
                return "";

            int openingIndex = txtLine.IndexOf(
                OpeningDelimiter,
                start,
                OpeningDelimiter.Length,
                StringComparison.InvariantCulture);
            if (openingIndex != 0)
                return "";

            mOpeningIndex = openingIndex;
            mContentIndex = start + OpeningDelimiter.Length;

            return OpeningDelimiter;
        }

        /// <summary>
        /// Gets a closing part from a text line starting at a specified position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            // The Closing is not allowed without Opening.
            if (mOpeningIndex < 0)
                return "";

            mClosingIndex = txtLine.IndexOf(ClosingDelimiter, mContentIndex, StringComparison.InvariantCulture);
            if (mClosingIndex <= mContentIndex)
                mClosingIndex = -1;

            return mClosingIndex == -1 ? "" : ClosingDelimiter;
        }

        /// <summary>
        /// Gets an indentation part from a right side of a text line starting at the end
        /// of text up to a specified start position.
        /// </summary>
        protected override string GetRightIndentation(string txtLine, int start)
        {
            return "";
        }

        /// <summary>
        /// Resets parsed states.
        /// </summary>
        internal void Reset()
        {
            RemoveAllParts();
            mOpeningIndex = -1;
            mClosingIndex = -1;
            mContentIndex = -1;
            Reference = string.Empty;
            Destination = string.Empty;
        }

        /// <summary>
        /// Returns True if the given LinkDefinitionBlock is valid.
        /// </summary>
        internal bool IsValid()
        {
            return (mOpeningIndex != -1) && (mContentIndex > mOpeningIndex) && (mClosingIndex > mContentIndex) &&
                   (Reference.Length > 0) && (Destination.Length > 0) &&
                   LinkDestinationBlock.IsValid(Destination, 0, Destination.Length - 1);
        }

        /// <summary>
        /// Gets string representing a Reference part of this reference definition.
        /// </summary>
        /// <remarks>
        /// The reference is actually a ContentLine between opening [ and closing ]: delimiters.
        /// </remarks>
        internal string Reference { get; private set; }

        /// <summary>
        /// Gets string representing a Destination part of this reference definition.
        /// </summary>
        internal string Destination { get; private set; }

        /// <summary>
        /// Gets LinkDestinationBlock corresponding to current Destination string.
        /// </summary>
        internal LinkDestinationBlock LinkDestination
        {
            get
            {
                if (mLinkDestination == null)
                {
                    mLinkDestination = new LinkDestinationBlock();
                    mLinkDestination.Add(new InlineBlock(Destination));
                }

                return mLinkDestination;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either closing sequence is allowed without an opening one.
        /// </summary>
        protected override bool AllowClosingWithoutOpening
        {
            get { return true; }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.LinkDefinition; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected override int OpeningSearchLimit
        {
            get { return OpeningDelimiter.Length; }
        }

        /// <summary>
        /// Returns the string formatted in compliance with the requirements.
        /// </summary>
        /// <remarks>
        /// A line break is replaced by a space.
        /// Several consecutive spaces are replaced by one space.
        /// </remarks>
        private static string NormalizeString(string text)
        {
            while (text.Contains("\r") || text.Contains("\n"))
            {
                text = text.Replace("\r", " ");
                text = text.Replace("\n", " ");
            }

            return TrimInPlace(text);
        }

        /// <summary>
        /// Replaces multiple consecutive spaces by one space.
        /// </summary>
        private static string TrimInPlace(string text)
        {
            string result = text.Trim();

            while (result.Contains("  "))
                result = result.Replace("  ", " ");

            return result;
        }

        /// <summary>
        /// The Opening delimiter.
        /// </summary>
        private const string OpeningDelimiter = "[";

        /// <summary>
        /// The Closing delimiter.
        /// </summary>
        private const string ClosingDelimiter = "]:";

        /// <summary>
        /// The cached index of Closing delimiter.
        /// </summary>
        private int mOpeningIndex = -1;
        private int mClosingIndex = -1;

        /// <summary>
        /// The cached index of ContentLine.
        /// </summary>
        private int mContentIndex;

        private LinkDestinationBlock mLinkDestination;
    }
}
