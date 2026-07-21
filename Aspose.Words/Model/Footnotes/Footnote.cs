// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2004 by Roman Korchagin

using System;
using Aspose.Words.Revisions;

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Represents a container for text of a footnote or endnote.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-footnote-and-endnote/">Working with Footnote and Endnote</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>The <see cref="Footnote"/> class is used to represent both footnotes and endnotes in a Word document.</p>
    ///
    /// <p><see cref="Footnote"/> is an inline-level node and can only be a child of <see cref="Paragraph"/>.</p>
    ///
    /// <p><see cref="Footnote"/> can contain <see cref="Paragraph"/> and <see cref="Aspose.Words.Tables.Table"/> child nodes.</p>
    ///
    /// <seealso cref="FootnoteType"/>
    /// <seealso cref="DocumentBuilder.InsertFootnote(Aspose.Words.Notes.FootnoteType,string)"/>
    /// <seealso cref="FootnoteOptions"/>
    /// <seealso cref="EndnoteOptions"/>
    /// </remarks>
    public class Footnote : InlineStory, ITrackableNode 
    {
        /// <summary>
        /// Initializes an instance of the <see cref="Footnote"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Footnote"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Footnote"/> to the document use<see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the paragraph where you want the footnote inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="footnoteType">A <see cref="FootnoteType"/> value
        /// that specifies whether this is a footnote or endnote.</param>
        public Footnote(DocumentBase doc, FootnoteType footnoteType) : 
            this(doc, footnoteType, true, "", new RunPr())
        {
        }

        internal Footnote(DocumentBase doc, FootnoteType footnoteType, bool isAuto, string referenceMark, RunPr runPr) :
            base(doc, runPr)
        {
            mFootnoteType = footnoteType;
            mIsAuto = isAuto;
            mReferenceMark = referenceMark;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Footnote"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Footnote; }
        }

        /// <summary>
        /// Returns <see cref="StoryType.Footnotes"/> or <see cref="StoryType.Endnotes"/>.
        /// </summary>
        public override StoryType StoryType
        {
            get
            {
                switch (mFootnoteType)
                {
                    case FootnoteType.Footnote: return StoryType.Footnotes;
                    case FootnoteType.Endnote: return StoryType.Endnotes;
                    default: throw new InvalidOperationException("Unknown footnote type.");
                }
            }
        }

        /// <summary>
        /// Returns a value that specifies whether this is a footnote or endnote.
        /// </summary>
        public FootnoteType FootnoteType
        {
            get { return mFootnoteType; }
        }

        /// <summary>
        /// Holds a value that specifies whether this is a auto-numbered footnote or 
        /// footnote with user defined custom reference mark.
        /// </summary>
        /// <remarks>
        /// <see cref="ReferenceMark"/> initialized with empty string if <see cref="IsAuto"/> set to <c>false</c>.
        /// </remarks>
        public bool IsAuto
        {
            get { return mIsAuto; }
            set 
            {
                mIsAuto = value;
                if (!mIsAuto)
                    mReferenceMark = "";
            }
        }

        // WORDSNET-4993 Holds user defined custom reference mark for non-autonumbered footnotes.
        /// <summary>
        /// Gets/sets custom reference mark to be used for this footnote.
        /// Default value is <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms>, meaning auto-numbered footnotes are used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this property is set to <b>empty string</b><ms> (<see cref="string.Empty"/>)</ms> or <c>null</c>, then <see cref="IsAuto"/> property will automatically be set to <c>true</c>, 
        /// if set to anything else then <see cref="IsAuto"/> will be set to <c>false</c>.
        /// </para>
        /// <para>
        /// RTF-format can only store 1 symbol as custom reference mark, so upon export only the first symbol will be written others will be discard.
        /// </para>
        /// </remarks>
        public string ReferenceMark
        {
            get { return mReferenceMark; }
            set {
                // TODO WORDSNET-23410 A footnote with no reference mark and not auto. Valid in Word, rendered as nothing.
                mReferenceMark = value;
                mIsAuto = !StringUtil.HasChars(mReferenceMark);
            }
        }

        /// <summary>
        /// Gets the actual text of the reference mark displayed in the document for this footnote.
        /// </summary>
        /// <remarks>
        /// To initially populate values of this property for all reference marks of the document, or to update
        /// the values after changes in the document that might affect the reference marks, you must execute the
        /// <see cref="Document.UpdateActualReferenceMarks"/> method.
        /// Updating fields (<see cref="Document.UpdateFields"/>) may also be necessary to get the correct result.
        /// </remarks>
        public string ActualReferenceMark
        {
            get
            {
                if (!IsAuto)
                    return mReferenceMark ?? string.Empty;

                if (mIndex < 0)
                    return string.Empty; // The value is not populated yet.

                Section parentSection = (Section)GetAncestor(NodeType.Section);
                if (parentSection == null)
                    return string.Empty;

                Document document = Document as Document;
                if (document == null)
                    return string.Empty;

                NumberStyle numberStyle = (FootnoteType == FootnoteType.Footnote)
                    ? parentSection.PageSetup.FootnoteOptions.NumberStyle
                    : parentSection.PageSetup.EndnoteOptions.NumberStyle;

                return NumberConverter.NumberToString(mIndex, numberStyle, true);
            }
        }

        /// <summary>
        /// Gets or sets the actual index of this footnote on the page/section/document depending on the current
        /// footnote or endnote options.
        /// </summary>
        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls DocumentVisitor.VisitFootnoteStart, then calls Accept for all child nodes of the footnote
        /// and calls DocumentVisitor.VisitFootnoteEnd at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the footnote.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitFootnoteStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the footnote.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitFootnoteEnd(this);
        }

        internal void SetFootnoteType(FootnoteType type)
        {
            mFootnoteType = type;
        }

        EditRevision ITrackableNode.InsertRevision
        {
            get { return RunPr.InsertRevision; }
            set { RunPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return RunPr.DeleteRevision; }
            set { RunPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return RunPr.MoveFromRevision; }
            set { RunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return RunPr.MoveToRevision; }
            set { RunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            RunPr.Remove(RevisionAttr.MoveFromRevision);
            RunPr.Remove(RevisionAttr.MoveToRevision);
        }

        private FootnoteType mFootnoteType;
        private bool mIsAuto;
        private string mReferenceMark;
        private int mIndex = -1;
    }
}
