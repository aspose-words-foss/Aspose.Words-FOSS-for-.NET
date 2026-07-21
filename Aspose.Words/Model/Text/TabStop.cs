// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/05/2005 by Roman Korchagin

using Aspose.Words.Lists;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a single custom tab stop. The <see cref="TabStop"/> object is a member of the
    /// <see cref="TabStopCollection"/> collection.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Normally, a tab stop specifies a position where a tab stop exists. But because
    /// tab stops can be inherited from parent styles, it might be needed for the child object
    /// to define explicitly that there is no tab stop at a given position. To clear
    /// an inherited tab stop at a given position, create a <see cref="TabStop"/> object and set
    /// <see cref="Alignment"/> to <see cref="TabAlignment.Clear"/>.</p>
    ///
    /// <p>For more information see <see cref="TabStopCollection"/>.</p>
    ///
    /// <seealso cref="ParagraphFormat"/>
    /// <seealso cref="TabStopCollection"/>
    /// <seealso cref="Aspose.Words.Document.DefaultTabStop"/>
    /// </remarks>
    [System.Serializable]
    public sealed class TabStop
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TabStop()
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public TabStop(double position) :
            this(position, TabAlignment.Left, TabLeader.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="position">The position of the tab stop in points.</param>
        /// <param name="alignment">A <see cref="TabAlignment"/> value that
        /// specifies the alignment of text at this tab stop.</param>
        /// <param name="leader">A <see cref="TabLeader"/> value that specifies
        /// the type of the leader line displayed under the tab character.</param>
        public TabStop(double position, TabAlignment alignment, TabLeader leader) :
            this(ConvertUtilCore.PointToTwip(position), alignment, leader)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal TabStop(int positionTwips, TabAlignment alignment, TabLeader leader)
        {
            mPositionTwips = positionTwips;
            mAlignment = alignment;
            mLeader = leader;
        }

        /// <summary>
        /// Creates legacy tab stop instance.
        /// </summary>
        internal static TabStop CreateLegacyTabStop(double position, ListTrailingCharacter trailingCharacter)
        {
            TabStop tabStop = new TabStop(position, TabAlignment.List, TabLeader.None);
            tabStop.IsLegacyTab = true;

            tabStop.mTrailingCharacter = trailingCharacter;

            return tabStop;
        }

        internal TabStop Clone()
        {
            return (TabStop)MemberwiseClone();
        }

        /// <summary>
        /// Compares with the specified <see cref="TabStop"/>.
        /// </summary>
        public bool Equals(TabStop rhs)
        {
            return
                (mPositionTwips == rhs.mPositionTwips) &&
                (mAlignment == rhs.mAlignment) &&
                (mLeader == rhs.mLeader) &&
                (mToleranceTwips == rhs.mToleranceTwips) &&
                (mUndocumented40 == rhs.mUndocumented40);
        }

        /// <summary>
        /// Calculates hash code for this object.
        /// </summary>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            int hashCode = mPositionTwips.GetHashCode();
            hashCode = (hashCode * 397) ^ mAlignment.GetHashCode();
            hashCode = (hashCode * 397) ^ mLeader.GetHashCode();
            hashCode = (hashCode * 397) ^ mToleranceTwips.GetHashCode();
            hashCode = (hashCode * 397) ^ mUndocumented40.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Gets the position of the tab stop in points.
        /// </summary>
        public double Position
        {
            get { return ConvertUtilCore.TwipToPoint(mPositionTwips); }
        }

        /// <summary>
        /// Gets or sets the alignment of text at this tab stop.
        /// </summary>
        public TabAlignment Alignment
        {
            get { return mAlignment; }
            set
            {
                mAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the leader line displayed under the tab character.
        /// </summary>
        public TabLeader Leader
        {
            get { return mLeader; }
            set
            {
                mLeader = value;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this tab stop clears any existing tab stops in this position.
        /// </summary>
        public bool IsClear
        {
            get { return mAlignment == TabAlignment.Clear; }
        }

        internal int PositionTwips
        {
            get { return mPositionTwips; }
            set
            {
                mPositionTwips = value;
            }
        }

        /// <summary>
        /// Tolerance on both sides of the position to delete tab stops, twips.
        /// </summary>
        internal int ToleranceTwips
        {
            get { return mToleranceTwips; }
            set
            {
                mToleranceTwips = value;
            }
        }

        /// <summary>
        /// An undocumented flag that is sometimes set on tab stops in MS Word.
        /// </summary>
        internal bool Undocumented40
        {
            get { return mUndocumented40; }
            set
            {
                mUndocumented40 = value;
            }
        }

        /// <summary>
        /// Gets trailing character used for legacy tab stop.
        /// </summary>
        internal ListTrailingCharacter LegacyListTrailingCharacter
        {
            get
            {
                Debug.Assert(IsLegacyTab, "Should be called for legacy tab stop only.");
                return (mTrailingCharacter);
            }
        }

        /// <summary>
        /// True, if tab stop is produced by legacy list formatting.
        /// </summary>
        internal bool IsLegacyTab { get; private set; }

        private int mPositionTwips;
        private TabAlignment mAlignment;
        private TabLeader mLeader;
        /// <summary>
        /// I'd really love to remove tolerances some day.
        /// Looks like RTF and WordML do not have this feature, only DOC does.
        /// </summary>
        private int mToleranceTwips;
        private bool mUndocumented40;
        private ListTrailingCharacter mTrailingCharacter;
    }
}
