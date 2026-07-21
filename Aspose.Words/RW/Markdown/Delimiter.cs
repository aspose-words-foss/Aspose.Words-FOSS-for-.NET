// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/04/2019 by Ilya Navrotskiy

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown delimiter of inline blocks.
    /// </summary>
    /// <remarks>
    /// The delimiters are represented with the sequence of one of the characters, like: '*', '_', '`', '~' and so on.
    /// See also the algorithm for parsing nested emphasis and links at https://spec.commonmark.org
    /// </remarks>
    internal abstract class Delimiter : ILinkedListNode
    {
        /// <summary>
        /// Creates Delimiter object from a specified DelimiterRun.
        /// </summary>
        protected Delimiter(DelimiterRun delimiterRun, int offset, int maxLength)
        {
            Debug.Assert(delimiterRun != null);

            Start = delimiterRun.Start + offset;
            End = (Start + CalculateLength(delimiterRun, offset, maxLength)) - 1;

            mText = delimiterRun.Text;
            mFlankingType = MarkdownUtil.GetFlankingType(PrevChar, NextChar);
        }

        /// <summary>
        /// Creates a corresponding markdown inline block.
        /// </summary>
        internal abstract Block ToBlock();

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be Opening for a specified delimiter.
        /// </summary>
        internal abstract bool CanBeOpeningFor(Delimiter other);

        /// <summary>
        /// Gets a delimiter that can be an Opening for this delimiter.
        /// </summary>
        internal virtual Delimiter GetPotentialOpeningBackward()
        {
            Delimiter curDelimiter = (Delimiter)PrevNode;
            while (curDelimiter != null)
            {
                if (!IsNotIncluded)
                {
                    if (curDelimiter.IsLinked)
                    {
                        // Jump over linked delimiters with the same or greater priority.
                        if ((curDelimiter.IsClosing) && (Priority <= curDelimiter.Priority))
                            curDelimiter = curDelimiter.LinkedDelimiter;
                    }
                    else
                    {
                        if (curDelimiter.CanBeOpeningFor(this))
                            return curDelimiter;
                    }
                }

                curDelimiter = (Delimiter)curDelimiter.PrevNode;
            }

            return null;
        }

        /// <summary>
        /// Gets a delimiter that can be a Closing, starting from a specified delimiter forward.
        /// </summary>
        internal Delimiter GetPotentialClosingForward()
        {
            if (CanBeClosing)
                return this;

            Delimiter curDelimiter = (Delimiter)NextNode;
            while (curDelimiter != null)
            {
                if (curDelimiter.CanBeClosing)
                    return curDelimiter;
                curDelimiter = (Delimiter)curDelimiter.NextNode;
            }

            return null;
        }

        /// <summary>
        /// Links two delimiters, so that one of them becomes Opening and another one becomes Closing.
        /// </summary>
        /// <remarks>
        /// After two delimiters are linked, their lengths become equal. So, if the delimiters do not have equal
        /// lengths, then those of them which is longer will be truncated.
        /// </remarks>
        /// <returns>
        /// Remained part of the longer delimiter, or <c>null</c>, if the delimiters have equal lengths.
        /// </returns>
        internal Delimiter Link(Delimiter other)
        {
            // There can be a situation when we try to link delimiters and one of them is linked already.
            // The case is possible for FootnoteDelimiter, for example, as it has the same closing type, as LinkTextOpening.
            // In this case we first need to unlink such delimiters.
            if (IsLinked)
            {
                Debug.Assert(LinkedDelimiter.Priority <= other.Priority);
                UnLink();
            }
            if (other.IsLinked)
            {
                Debug.Assert(other.LinkedDelimiter.Priority <= Priority);
                other.UnLink();
            }

            LinkedDelimiter = other;
            other.LinkedDelimiter = this;

            int minLength = System.Math.Min(Length, other.Length);

            Delimiter remainedDelimiter = Cut(minLength);
            if (remainedDelimiter != null)
                return remainedDelimiter;

            remainedDelimiter = other.Cut(minLength);
            if (remainedDelimiter != null)
                return remainedDelimiter;

            return null;
        }

        /// <summary>
        /// Unlinks delimiter, so that this delimiter and its linked one become nor Opening neither Closing.
        /// </summary>
        internal void UnLink()
        {
            Debug.Assert(IsLinked);

            Delimiter opening = (IsOpening) ? this : LinkedDelimiter;
            Delimiter closing = (IsOpening) ? LinkedDelimiter : this;

            if (opening.PrevNode != null)
                opening.Merge((Delimiter)opening.PrevNode);

            if (closing.NextNode != null)
                closing.Merge((Delimiter)closing.NextNode);

            opening.LinkedDelimiter = null;
            closing.LinkedDelimiter = null;
        }

        /// <summary>
        /// Returns true, if this delimiter is located before a specified delimiter in a text.
        /// </summary>
        internal bool IsBefore(Delimiter delimiter)
        {
            return (End < delimiter.Start);
        }

        /// <summary>
        /// Splits linked delimiter in accordance with maximum allowed <see cref="MaxLinkedLength"/> value.
        /// </summary>
        internal void Split(LinkedList delimiters)
        {
            if (!IsLinked)
                return;

            Delimiter opening = (IsOpening) ? this : LinkedDelimiter;
            Delimiter closing = (IsOpening) ? LinkedDelimiter : this;

            while (opening.IsLinked && (opening.Length > MaxLinkedLength))
            {
                // Split opening delimiter.
                delimiters.MoveObjectSafe(opening);
                opening = opening.Cut(MaxLinkedLength);
                delimiters.Insert(opening);

                // Split closing delimiter.
                delimiters.MoveObjectSafe(closing);
                closing = closing.Cut(MaxLinkedLength);
                delimiters.Append(closing);

                // Link remained delimiters together.
                if (opening.CanBeOpeningFor(closing))
                {
                    opening.Link(closing);
                }
                else
                {
                    if (opening.PrevNode != null)
                        opening.Merge((Delimiter)opening.PrevNode);

                    if (closing.NextNode != null)
                        closing.Merge((Delimiter)closing.NextNode);
                }

                delimiters.MoveObjectSafe(closing);
            }
        }

        /// <summary>
        /// Gets corresponding LinkText closing delimiter.
        /// </summary>
        internal Delimiter GetLinkTextClosing()
        {
            if (PrevNode == null)
                return null;

            Delimiter linkTextClosing = (Delimiter)PrevNode;
            if (linkTextClosing.Type != DelimiterType.LinkTextClosing)
                return null;

            return ((linkTextClosing.End + 1) == Start) ? linkTextClosing : null;
        }

        /// <summary>
        /// Cuts this delimiter to a specified length.
        /// </summary>
        /// <returns>Remained part of the delimiter.</returns>
        private Delimiter Cut(int newLength)
        {
            if (newLength >= Length)
                return null;

            Debug.Assert(LinkedDelimiter != null);

            Delimiter remainedDelimiter = Clone();

            if (IsOpening)
            {
                remainedDelimiter.End -= newLength;
                Start = remainedDelimiter.End + 1;
            }
            else
            {
                remainedDelimiter.Start += newLength;
                End = remainedDelimiter.Start - 1;
            }

            return remainedDelimiter;
        }

        /// <summary>
        /// Merges this delimiter with a specified one.
        /// </summary>
        private void Merge(Delimiter other)
        {
            Debug.Assert(other != null);

            // It is allowed to merge only unlinked sibling delimiters.
            if (IsLinked || other.IsLinked)
                return;

            if ((End + 1) == other.Start)
            {
                End = other.End;
                other.MarkAsRemoved();
            }

            if ((Start - 1) == other.End)
            {
                Start = other.Start;
                other.MarkAsRemoved();
            }
        }

        /// <summary>
        /// Clones the delimiter.
        /// </summary>
        private Delimiter Clone()
        {
            Delimiter lhs = (Delimiter)MemberwiseClone();
            lhs.LinkedDelimiter = null;

            // Reset index for linked list.
            lhs.MarkAsRemoved();

            return lhs;
        }

        /// <summary>
        /// Calculates length of the delimiter.
        /// </summary>
        private int CalculateLength(DelimiterRun delimiterRun, int offset, int maxLength)
        {
            Debug.Assert(offset < delimiterRun.Opening.Length);

            char delimiterChar = delimiterRun.Opening[offset];

            int minLength = System.Math.Min(delimiterRun.Opening.Length - offset, maxLength);
            Debug.Assert(minLength > 0);

            int length = 0;
            while ((length < minLength) && (delimiterRun.Opening[offset + length] == delimiterChar))
            {
                length++;
            }

            return length;
        }

#if DEBUG
        public override string ToString()
        {
            string isLinked = IsLinked ? "true" : "false";
            return string.Format("{{{0}}}[{1}:{2}], Length={3}, Flanking={4}, IsLinked={5}",
                Type, Start, End, Length, FlankingType, isLinked);
        }
#endif
        #region ILInkedListNode implementation

        public ILinkedListNode NextNode
        {
            get { return mNextNode; }
            set { mNextNode = value; }
        }

        public ILinkedListNode PrevNode
        {
            get { return mPrevNode; }
            set { mPrevNode = value; }
        }

        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        public long SecondaryIndex
        {
            get { return mSecondaryIndex; }
            set { mSecondaryIndex = value; }
        }

        public bool IsNotIncluded
        {
            get { return mIndex == int.MinValue; }
        }

        public void MarkAsRemoved()
        {
            mIndex = int.MinValue;
        }

        #endregion

        /// <summary>
        /// Gets type of the delimiter.
        /// </summary>
        internal abstract DelimiterType Type { get; }

        /// <summary>
        /// Gets a priority of the delimiter.
        /// </summary>
        /// <remarks> The delimiter with greater priority can break linked delimiters with lower priority.</remarks>
        internal abstract DelimiterPriority Priority { get; }

        /// <summary>
        /// Gets a priority to unlink inside linked delimiters.
        /// </summary>
        /// <remarks>
        /// This is a priority of a linked delimiter below which there can be no other linked delimiters inside.
        /// </remarks>
        internal abstract DelimiterPriority UnlinkPriority { get; }

        /// <summary>
        /// Gets a boolean value indicating whether the delimiter is valid.
        /// </summary>
        internal virtual bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets length of the delimiter.
        /// </summary>
        internal int Length
        {
            [CppConstMethod]
            get { return (End - Start) + 1; }
        }

        /// <summary>
        /// Gets delimiter start position in original text.
        /// </summary>
        internal int Start { get; private set; }

        /// <summary>
        /// Gets delimiter end position in original text.
        /// </summary>
        internal int End { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating whether the delimiter is linked with another delimiter,
        /// so that they are form a pair of Opening and Closing delimiters.
        /// </summary>
        internal bool IsLinked
        {
            [CppConstMethod]
            get { return (LinkedDelimiter != null); }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter forms an Opening sequence in a pair of delimiters.
        /// </summary>
        internal bool IsOpening
        {
            get
            {
                Debug.Assert(IsLinked);
                return IsBefore(LinkedDelimiter);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter forms an Closing sequence in a pair of delimiters.
        /// </summary>
        internal bool IsClosing
        {
            get
            {
                Debug.Assert(IsLinked);
                return LinkedDelimiter.IsBefore(this);
            }
        }

        /// <summary>
        /// Gets linked delimiter.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        internal Delimiter LinkedDelimiter { get; private set; }

        /// <summary>
        /// Gets a string value representing a text this delimiter belongs.
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Gets flanking type of the delimiter.
        /// </summary>
        internal FlankingType FlankingType
        {
            get { return mFlankingType; }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter can be a Closing.
        /// </summary>
        protected abstract bool CanBeClosing { get; }

        /// <summary>
        /// Gets an integer value representing a maximum allowed length of the delimiter in a linked state.
        /// </summary>
        protected abstract int MaxLinkedLength { get; }

        /// <summary>
        /// Gets a boolean value indicating the delimiter is escaped.
        /// </summary>
        /// <remarks>
        /// Note, for a moment the only InlineCode (`) delimiter can be escaped. All other escaped delimiters
        /// are excluded while building delimiter runs they are belonging.
        /// </remarks>
        protected bool IsEscaped
        {
            get { return MarkdownUtil.IsEscaped(mText, Start); }
        }

        /// <summary>
        /// Gets a boolean value indicating the delimiter is intraword.
        /// </summary>
        protected bool IsIntraword
        {
            get { return MarkdownUtil.IsIntraword(PrevChar, NextChar); }
        }

        /// <summary>
        /// Gets character immediately preceding this delimiter in a text.
        /// </summary>
        private char PrevChar
        {
            get { return (Start > 0) ? mText[Start - 1] : InlineContainerBlock.LineSeparator; }
        }

        /// <summary>
        /// Gets character immediately following this delimiter in a text.
        /// </summary>
        private char NextChar
        {
            get { return ((End + 1) < mText.Length) ? mText[End + 1] : InlineContainerBlock.LineSeparator; }
        }

        private ILinkedListNode mNextNode;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ILinkedListNode mPrevNode;
        private int mIndex = int.MinValue;
        private long mSecondaryIndex;

        private readonly string mText;
        private readonly FlankingType mFlankingType;
    }
}
