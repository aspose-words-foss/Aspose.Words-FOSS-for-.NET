// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/15/2012 by Denis Darkin

namespace Aspose.Words
{
    /// <summary>
    /// An absolute position tab is a character which is used to advance the position on 
    /// the current line of text when displaying this WordprocessingML content.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    public class AbsolutePositionTab : SpecialChar
    {
        internal AbsolutePositionTab(DocumentBase doc, RunPr runPr) : base(doc, ControlChar.TabChar, runPr)
        {
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitAbsolutePositionTab"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><c>false</c> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitAbsolutePositionTab(this));
        }

        /// <summary>
        /// Specifies the location of the positional tab stop on the line, 
        /// as well as the alignment which shall be applied to text subsequent to this absolute position tab.
        /// </summary>
        internal AbsolutePositionTabAlignment Alignment
        {
            get { return mAlignment; }
            set { mAlignment = value; }
        }

        /// <summary>
        /// Specifies the extents which shall be used to calculate the absolute positioning of this absolute position tab.
        /// </summary>
        internal AbsolutePositionTabPositioningBase PositioningBase
        {
            get { return mPositioningBase; }
            set { mPositioningBase = value; }
        }

        /// <summary>
        /// Specifies the character which shall be used to fill in the space created by this absolute position tab.
        /// </summary>
        internal AbsolutePositionTabLeaderChar LeaderChar
        {
            get { return mLeader; }
            set { mLeader = value; }
        }

        private AbsolutePositionTabAlignment mAlignment = AbsolutePositionTabAlignment.Default;
        private AbsolutePositionTabLeaderChar mLeader = AbsolutePositionTabLeaderChar.Default;
        private AbsolutePositionTabPositioningBase mPositioningBase = AbsolutePositionTabPositioningBase.Default;
    }
}
