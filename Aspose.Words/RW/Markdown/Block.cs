// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2019 by Ilya Navrotskiy

using Aspose.JavaAttributes;
using Aspose.Words.Loading;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Block.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppVirtualInheritance("System.Object")]
    internal abstract class Block
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal abstract bool TryParse(string txtLine, int start);

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal abstract bool TryAppend(Block block);

        /// <summary>
        /// Closes the block.
        /// </summary>
        internal void Close()
        {
            mIsOpened = false;
        }

        /// <summary>
        /// Opens the block.
        /// </summary>
        internal void Open()
        {
            mIsOpened = true;
        }

        /// <summary>
        /// Removes the block from a parent.
        /// </summary>
        internal void Remove()
        {
            if (Parent != null)
                Parent.Remove(this);
        }

        /// <summary>
        /// Returns first parent block of a specified type.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        internal ContainerBlock GetParent(BlockType type)
        {
            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if (curParent.Type == type)
                    return curParent;

                curParent = curParent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns first parent block of a specified type with a specified opening state.
        /// </summary>
        internal ContainerBlock GetParent(BlockType type, bool isOpened)
        {
            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if ((curParent.Type == type) && (curParent.IsOpened == isOpened))
                    return curParent;

                curParent = curParent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns first parent block of a specified type.
        /// </summary>
        internal ContainerBlock GetParent(params BlockType[] types)
        {
            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if (curParent.HasType(types))
                    return curParent;

                curParent = curParent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns topmost parent block of a specified type with a specified opening state.
        /// </summary>
        internal ContainerBlock GetTopmostParent(BlockType type, bool isOpened)
        {
            ContainerBlock lastSuccessfull = null;

            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if ((curParent.Type == type) && (curParent.IsOpened == isOpened))
                    lastSuccessfull = curParent;

                curParent = curParent.Parent;
            }

            return lastSuccessfull;
        }

        /// <summary>
        /// Opens all parent blocks of a specified type.
        /// </summary>
        internal void OpenParents(BlockType type)
        {
            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if (curParent.Type == type)
                    curParent.Open();

                curParent = curParent.Parent;
            }
        }

        /// <summary>
        /// Closes all parent blocks of a specified type.
        /// </summary>
        internal void CloseParents(BlockType type)
        {
            ContainerBlock curParent = mParent;
            while ((curParent != null) && (curParent.Type == type))
            {
                curParent.Close();
                curParent = curParent.Parent;
            }
        }

        /// <summary>
        /// Inverts opening state of all parent blocks of a specified type.
        /// </summary>
        internal void InvertParents(BlockType type)
        {
            ContainerBlock curParent = mParent;
            while (curParent != null)
            {
                if (curParent.Type == type)
                    curParent.Invert();

                curParent = curParent.Parent;
            }
        }

        /// <summary>
        /// Returns true, if this block has one of the specified types.
        /// </summary>
        internal bool HasType(params BlockType[] types)
        {
            foreach (BlockType type in types)
                if (Type == type)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns previous sibling block of a specified type.
        /// </summary>
        internal Block GetPreviousSibling(params BlockType[] types)
        {
            Debug.Assert(types != null);

            int prevIndex = Index - 1;
            while (prevIndex >= 0)
            {
                Block previousSibling = Parent[prevIndex];
                foreach (BlockType type in types)
                {
                    if (previousSibling.Type == type)
                        return previousSibling;
                }

                prevIndex--;
            }

            return null;
        }

        /// <summary>
        /// Returns previous block of a specified type and opening state deep traversed over all parents.
        /// </summary>
        internal Block GetPreviousDeepTraversed(bool isOpened, params BlockType[] types)
        {
            ContainerBlock curParent = Parent;
            while (curParent != null)
            {
                int end = (curParent == Parent) ? (Index - 1) : (curParent.Count - 1);
                for (int i = end; i >= 0; i--)
                {
                    Block prevBlock = curParent[i];
                    if ((prevBlock.HasType(types)) && (prevBlock.IsOpened == isOpened))
                        return prevBlock;
                }

                curParent = curParent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void Write(MarkdownReaderContext context)
        {
            if (Text.Length == 0)
                return;

            string text = ResolveWhitespacesAndLineBreaks();
            context.WriteText(text);
        }

        /// <summary>
        /// Parses text line starting from a specified position onto the following parts:
        /// | mOpeningIndentation | mOpening | mContentLine | mClosing | mClosingIndentation |
        ///  </summary>
        protected bool Parse(string txtLine, int start)
        {
            if (txtLine == null)
                return false;

            mOpeningIndentation = GetLeftIndentation(txtLine, start);

            int openingStart = start + mOpeningIndentation.Length;
            mOpening = GetOpening(txtLine, openingStart);

            int contentStart = openingStart + mOpening.Length;

            mClosingIndentation = GetRightIndentation(txtLine, contentStart);

            int closingEnd = (txtLine.Length - 1) - mClosingIndentation.Length;
            // Every Closing MUST have a corresponding Opening.
            mClosing = (StringUtil.HasChars(mOpening) || AllowClosingWithoutOpening)
                ? GetClosing(txtLine, contentStart, closingEnd)
                : "";

            int contentEnd = closingEnd - mClosing.Length;

            // If there is no Closing, then there is no closing indentation
            // and thus whole ending of the text actually belongs to a content of the block.
            if (!StringUtil.HasChars(mClosing))
            {
                mClosingIndentation = "";
                contentEnd = txtLine.Length - 1;
            }

            mContentLine = GetContent(txtLine, contentStart, contentEnd);

            return true;
        }

        /// <summary>
        /// Gets an indentation part from a left side of a text line starting at a specified position.
        /// </summary>
        protected virtual string GetLeftIndentation(string txtLine, int start)
        {
            Debug.Assert(txtLine != null);

            int curPosition = start;
            while ((curPosition < txtLine.Length) && IsIndentationChar(txtLine[curPosition]))
                curPosition++;

            int length = curPosition - start;
            return (length > 0) ? txtLine.Substring(start, length) : "";
        }

        /// <summary>
        /// Gets an indentation part from a right side of a text line starting at the end
        /// of text up to a specified start position.
        /// </summary>
        protected virtual string GetRightIndentation(string txtLine, int start)
        {
            Debug.Assert(txtLine != null);

            if (start >= txtLine.Length)
                return "";

            int curPosition = txtLine.Length - 1;
            while ((curPosition >= start) && IsIndentationChar(txtLine[curPosition]))
                curPosition--;

            int length = (txtLine.Length - 1) - curPosition;
            return (length > 0) ? txtLine.Substring(curPosition + 1, length) : "";
        }

        /// <summary>
        /// Gets an opening part from a text line starting at a specified position.
        /// </summary>
        protected virtual string GetOpening(string txtLine, int start)
        {
            Debug.Assert(txtLine != null);

            int curPosition = start;
            int length = 0;
            while ((curPosition < txtLine.Length) && IsOpeningChar(txtLine[curPosition]))
            {
                length += MarkdownUtil.GetLength(txtLine[curPosition]);
                curPosition++;

                if (length == OpeningSearchLimit)
                    break;
            }

            return (length > 0) ? txtLine.Substring(start, curPosition - start) : "";
        }

        /// <summary>
        /// Gets a closing part from a text line moving from a specified end toward to start position.
        /// </summary>
        protected virtual string GetClosing(string txtLine, int start, int end)
        {
            Debug.Assert(txtLine != null);

            int curPosition = end;

            // It is assumed that a closing sequence of characters is the same as an opening one.
            while ((curPosition >= start) && IsOpeningChar(txtLine[curPosition]))
                curPosition--;

            // There MUST be a whitespace before a closing sequence of characters.
            // Otherwise, we cannot distinguish it from a content tail.
            if ((curPosition < 0) || !StringUtil.IsWhiteSpace(txtLine[curPosition]))
                return "";

            int length = end - curPosition;
            return (length > 0) ? txtLine.Substring(curPosition + 1, length) : "";
        }

        /// <summary>
        /// Gets a content part from a text line starting at a specified start position up to the end position.
        /// </summary>
        protected virtual string GetContent(string txtLine, int start, int end)
        {
            int length = end - start + 1;
            return (length > 0) ? txtLine.Substring(start, length) : "";
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of indentation characters.
        /// </summary>
        protected virtual bool IsIndentationChar(char c)
        {
            return ((c == ' ') || (c == '\t'));
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected virtual bool IsOpeningChar(char c)
        {
            return false;
        }

        /// <summary>
        /// Removes all parsed parts.
        /// </summary>
        /// <remarks>Use this for memory optimization with inline container blocks that are completely parsed.</remarks>
        protected void RemoveAllParts()
        {
            mOpeningIndentation = null;
            mOpening = null;
            mContentLine = null;
            mClosing = null;
            mClosingIndentation = null;
        }

        /// <summary>
        /// Inverts a block state.
        /// </summary>
        /// <remarks>IN. Made protected due to Java compiler problem.</remarks>
        protected void Invert()
        {
            mIsOpened = !mIsOpened;
        }

#if DEBUG
        public override string ToString()
        {
            string isOpened = mIsOpened ? "opened" : "closed";
            return string.Format("{0}:{1} {{Text:{2}}} {{Opening:{3}}} {{ContentLine:{4}}}",
                Type, isOpened, Text, mOpening, mContentLine);
        }
#endif

        /// <summary>
        /// Resolves whitespaces and line breaks by inserting, removing or converting them appropriately.
        /// </summary>
        private string ResolveWhitespacesAndLineBreaks()
        {
            // WORDSNET-28255 Added option to allow to specify a soft line break character.
            char softLineBreakCharacter = (Document.LoadOptions == null)
                ? MarkdownLoadOptions.DefaultSoftLineBreakCharacter
                : Document.LoadOptions.SoftLineBreakCharacter;

            return MarkdownUtil.ResolveWhitespacesAndLineBreaks(this, softLineBreakCharacter);
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal abstract BlockType Type { get; }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal abstract MarkdownBlockLevel BlockLevel { get; }

        /// <summary>
        /// Gets or sets a parent block.
        /// </summary>
        internal ContainerBlock Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether this block is a first child.
        /// </summary>
        internal bool IsFirstChild
        {
            get
            {
              return ((Parent != null) && (Parent[0] == this));
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether this block is a last child.
        /// </summary>
        internal bool IsLastChild
        {
            get { return ((Parent != null) && (Parent[Parent.Count-1] == this)); }
        }

        /// <summary>
        /// Gets block immediately preceding this block.
        /// </summary>
        internal Block PreviousSibling
        {
            get
            {
                int prevIndex = Index - 1;
                return (prevIndex < 0) ? null : Parent[prevIndex];
            }
        }

        /// <summary>
        /// Gets block immediately following this block.
        /// </summary>
        internal Block NextSibling
        {
            get
            {
                int nextIndex = Index + 1;
                return (nextIndex < Parent.Count) ? Parent[nextIndex] : null;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating the block is inside a Quote block.
        /// </summary>
        internal bool IsInQuote
        {
            get { return (GetParent(BlockType.Quote) != null); }
        }

        /// <summary>
        /// Gets a boolean value indicating the block is inside a ListItem block.
        /// </summary>
        internal bool IsInList
        {
            get { return (GetParent(BlockType.BulletListItem, BlockType.OrderedListItem) != null); }
        }

        /// <summary>
        /// Gets or sets an action that should be performed with the block.
        /// </summary>
        internal BlockAction Action
        {
            get { return mAction; }
            set { mAction = value; }
        }

        /// <summary>
        /// Gets a boolean value indicating an opening state of the block.
        /// </summary>
        internal bool IsOpened
        {
            get { return mIsOpened; }
        }

        /// <summary>
        /// Gets or sets an opening indentation part.
        /// </summary>
        internal string OpeningIndentation
        {
            get { return mOpeningIndentation; }
            set { mOpeningIndentation = value; }
        }

        /// <summary>
        /// Gets an opening part.
        /// </summary>
        internal string Opening
        {
            get { return mOpening; }
        }

        /// <summary>
        /// Gets a closing part.
        /// </summary>
        internal string Closing
        {
            get { return mClosing; }
        }

        /// <summary>
        /// Gets a content line part.
        /// </summary>
        internal string ContentLine
        {
            get { return mContentLine; }
        }

        /// <summary>
        /// Gets length of an opening part considering tabs.
        /// </summary>
        internal int OpeningLength
        {
            get { return MarkdownUtil.GetLength(mOpening); }
        }

        /// <summary>
        /// Gets length of an opening indentation part considering tabs.
        /// </summary>
        internal int OpeningIndentationLength
        {
            get { return MarkdownUtil.GetLength(mOpeningIndentation); }
        }

        /// <summary>
        /// Gets length of a content line part considering tabs.
        /// </summary>
        internal int ContentLineIndentationLength
        {
            get
            {
                int nonWhitespaceIndex = StringUtil.IndexOfNonWhitespace(mContentLine);
                if (nonWhitespaceIndex == -1)
                    return mContentLine.Length;

                return MarkdownUtil.GetLength(mContentLine.Substring(0, nonWhitespaceIndex));
            }
        }

        /// <summary>
        /// Gets total length of all block parts.
        /// </summary>
        internal virtual int Length
        {
            get
            {
                return (mOpeningIndentation.Length +
                        mOpening.Length +
                        mContentLine.Length +
                        mClosing.Length +
                        mClosingIndentation.Length);
            }
        }

        /// <summary>
        /// Gets block text.
        /// </summary>
        internal virtual string Text
        {
            get { return ContentLine; }
        }

        /// <summary>
        /// Gets block indentation.
        /// </summary>
        internal virtual int Indentation
        {
            get { return OpeningIndentationLength; }
        }

        /// <summary>
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected virtual int OpeningSearchLimit
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Gets a boolean value indicating either closing sequence is allowed without an opening one.
        /// </summary>
        protected virtual bool AllowClosingWithoutOpening
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an index of the block inside a parent block.
        /// </summary>
        protected int Index
        {
            get
            {
                if (Parent != null)
                {
                    for (int i = 0; i < Parent.Count; i++)
                        if (Parent[i] == this)
                            return i;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets parent document.
        /// </summary>
        protected MarkdownDocument Document
        {
            get
            {
                if (mDocument == null)
                    mDocument = (MarkdownDocument)GetParent(BlockType.Document);

                return mDocument;
            }
        }

        /// <summary>
        /// A maximum allowed length of indentation part of the block.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxIndentationLength = 3;

        /// <summary>
        /// An action to execute over the block.
        /// </summary>
        private BlockAction mAction;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ContainerBlock mParent;

        private MarkdownDocument mDocument;

        private bool mIsOpened = true;

        private string mOpeningIndentation = "";
        private string mOpening = "";
        private string mContentLine = "";
        private string mClosing = "";
        private string mClosingIndentation = "";
    }
}
