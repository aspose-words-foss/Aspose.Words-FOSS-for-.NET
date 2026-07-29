// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System.Text;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a paragraph of text.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-paragraphs/">Working with Paragraphs</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Paragraph"/> is a block-level node and can be a child of classes derived from
    /// <see cref="Aspose.Words.Story"/> or <see cref="Aspose.Words.InlineStory"/>.</p>
    ///
    /// <p><see cref="Paragraph"/> can contain any number of inline-level nodes and bookmarks.</p>
    ///
    /// <p>The complete list of child nodes that can occur inside a paragraph consists of
    /// <see cref="BookmarkStart"/>, <see cref="BookmarkEnd"/>,
    /// <see cref="Fields.FieldStart"/>, <see cref="Fields.FieldSeparator"/>,
    /// <see cref="Fields.FieldEnd"/>, <see cref="Fields.FormField"/>,
    /// <see cref="Comment"/>, <see cref="Footnote"/>,
    /// <see cref="Run"/>, <see cref="SpecialChar"/>,
    /// <see cref="Aspose.Words.Drawing.Shape"/>, <see cref="Aspose.Words.Drawing.GroupShape"/>,
    /// <see cref="Aspose.Words.Markup.SmartTag"/>.</p>
    ///
    /// <p>A valid paragraph in Microsoft Word always ends with a paragraph break character and
    /// a minimal valid paragraph consists just of a paragraph break. The <see cref="Paragraph"/>
    /// class automatically appends the appropriate paragraph break character at the end
    /// and this character is not part of the child nodes of the <see cref="Paragraph"/>, therefore
    /// a <see cref="Paragraph"/> can be empty.</p>
    ///
    /// <p>Do not include the end of paragraph <see cref="ControlChar.ParagraphBreak"/>
    /// or end of cell <see cref="ControlChar.Cell"/> characters inside the text of
    /// the paragraph as it might make the paragraph invalid when the document is opened in Microsoft Word.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    [CppDeclareFriendClass("Aspose.Words.Fields.FieldSeqDataProvider")]
    public class Paragraph : CompositeNode,
        IParaAttrSource,
        IRunAttrSource,
        ITrackableNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Paragraph"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Paragraph"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the story where you want the paragraph inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Paragraph(DocumentBase doc) : this(doc, new ParaPr(), new RunPr())
        {
        }

        internal Paragraph(DocumentBase doc, ParaPr paraPr, RunPr runPr)
            : base(doc)
        {
            // RK I had nasty problems with importers creating paragraphs with null properties
            // and forgetting to initialize them when reading some corrupted documents.
            // Now it is disallowed to create paragraphs with null properties.
            Debug.Assert(paraPr != null);
            Debug.Assert(runPr != null);

            mParaPr = paraPr;
            mParagraphBreakRunPr = runPr;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Paragraph"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Paragraph; }
        }

        /// <summary>
        /// Retrieves the parent section-level story that can be <see cref="Body"/> or <see cref="HeaderFooter"/>.
        /// </summary>
        public Story ParentStory
        {
            get
            {
#if CPLUSPLUS
                return GetAncestorOf<Story>();
#else
                return (Story)GetAncestor(typeof(Story));
#endif
            }
        }

        /// <summary>
        /// Retrieves the parent <see cref="Section"/> of the paragraph.
        /// </summary>
        public Section ParentSection
        {
            get { return (Section)GetAncestor(NodeType.Section); }
        }

        /// <summary>
        /// True if this paragraph is an immediate child of <see cref="Cell"/>; false otherwise.
        /// </summary>
        public bool IsInCell
        {
            get { return (FirstNonMarkupParentNode is Cell); }
        }

        /// <summary>
        /// True if this paragraph is the first paragraph in a Cell.
        /// </summary>
        internal bool IsStartOfCell
        {
            get
            {
                CompositeNode parent = ParentNode;
                if (parent == null)
                    return false;

                if (NodeUtil.IsMarkupNode(parent))
                {
                    // node can be first child of Sdt/CustomXml but it does not mean that it is first child of cell, so we check differently
                    return IsInCell && IsAncestorNode(FirstNonMarkupParentNode.FirstChild);
                }

                return IsInCell && (this == parent.FirstNonMarkupCompositeDescendant);
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in a <see cref="Cell"/>; false otherwise.
        /// </summary>
        public bool IsEndOfCell
        {
            get
            {
                CompositeNode parent = FirstNonMarkupParentNode;
                return (parent is Cell) && (parent.LastNonMarkupCompositeDescendant == this);
            }
        }

        /// <summary>
        /// True if this paragraph break is a Style Separator. A style separator allows one
        /// paragraph to consist of parts that have different paragraph styles.
        /// </summary>
        public bool BreakIsStyleSeparator
        {
            get
            {
                return InlineHelper.GetBool(mParagraphBreakRunPr, FontAttr.Hidden) &&
                    InlineHelper.GetBool(mParagraphBreakRunPr, FontAttr.SpecialHidden);
            }
        }

        /// <summary>
        /// True if this paragraph is the first paragraph in the first cell of a row.
        /// </summary>
        internal bool IsStartOfRow
        {
            get { return (IsStartOfCell && ParentCell.IsFirstCell); }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in the last cell of a row.
        /// </summary>
        internal bool IsEndOfRow
        {
            get { return (IsEndOfCell && ParentCell.IsLastCell); }
        }

        internal bool IsStartOfTable
        {
            get { return (IsStartOfRow && ParentRow.IsFirstRow); }
        }

        internal bool IsEndOfTable
        {
            get { return (IsEndOfRow && ParentRow.IsLastRow); }
        }

        internal Cell ParentCell
        {
            get { return FirstNonMarkupParentNode as Cell; }
        }

        internal Row ParentRow
        {
            get
            {
                // WORDSNET-11447 IsInCell is true, but ParentCell is null when its child is cloned.
                return (ParentCell != null) ? ParentCell.ParentRow : null;
            }
        }

        internal Table ParentTable
        {
            get
            {
                // WORDSNET-11447 IsInCell is true, but ParentRow is null when its child is cloned.
                return (ParentRow != null) ? ParentRow.ParentTable : null;
            }
        }

        /// <summary>
        /// True if this paragraph is an immediate (ignoring SDTs) child of <see cref="Drawing.Shape"/>.
        /// </summary>
        internal bool IsInShape
        {
            get { return FirstNonMarkupParentNode is Shape; }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in the <see cref="Body"/> (main text story) of a <see cref="Section"/>; false otherwise.
        /// </summary>
        public bool IsEndOfSection
        {
            get
            {
                Story parentStory = ParentStory;
                return ((parentStory != null) &&
                    (parentStory.StoryType == StoryType.MainText) &&
                    (this == parentStory.LastNonMarkupCompositeDescendant));
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in the <see cref="HeaderFooter"/> (main text story) of a <see cref="Section"/>; false otherwise.
        /// </summary>
        public bool IsEndOfHeaderFooter
        {
            get
            {
                Story parentStory = ParentStory;
                return (parentStory is HeaderFooter && parentStory.LastNonMarkupCompositeDescendant == this);
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in the last section of the document.
        /// </summary>
        public bool IsEndOfDocument
        {
            get
            {
                if (!IsEndOfSection)
                    return false;

                // DV NodeType.System is used during merge. We don't need it to be considered as other nodes.
                for (Node node = Document.LastChild; node != null; node = node.PreviousSibling)
                    if (node is Section)
                        return (node == ParentSection);

                // This happens if document has no sections which shall never happen.
                return true;
            }
        }

        internal bool IsSectionBreakParagraph
        {
            get
            {
                if (NextNonAnnotationSibling != null)
                    return false;

                if (ParentNode.NodeType != NodeType.Body)
                    return false;

                if (ParentSection.IsLastChild)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Provides access to the paragraph formatting properties.
        /// </summary>
        public ParagraphFormat ParagraphFormat
        {
            get
            {
                if (mParagraphFormat == null)
                    mParagraphFormat = new ParagraphFormat(this, Document.Styles);
                return mParagraphFormat;
            }
        }

        /// <summary>
        /// Provides access to the list formatting properties of the paragraph.
        /// </summary>
        public ListFormat ListFormat
        {
            get
            {
                if (mListFormat == null)
                    mListFormat = new ListFormat(this, this, Document.Lists);
                return mListFormat;
            }
        }

        /// <summary>
        /// Provides access to the frame formatting properties.
        /// </summary>
        public FrameFormat FrameFormat
        {
            get
            {
                if (mFrameFormat == null)
                    mFrameFormat = new FrameFormat(this);
                return mFrameFormat;
            }
        }

        /// <summary>
        /// Gets a <see cref="ListLabel"/> object that provides access to list numbering value and formatting
        /// for this paragraph.
        /// </summary>
        public ListLabel ListLabel
        {
            get
            {
                if (mListLabel == null)
                    mListLabel = new ListLabel(this);
                return mListLabel;
            }
        }

        /// <summary>
        /// Provides access to the typed collection of pieces of text inside the paragraph.
        /// </summary>
        public RunCollection Runs
        {
            get
            {
                if (mRuns == null)
                    mRuns = new RunCollection(this);
                return mRuns;
            }
        }

        /// <summary>
        /// Provides access to the font formatting of the paragraph break character.
        /// </summary>
        public Font ParagraphBreakFont
        {
            // I don't expect this to be used often so it is easier to create the object everytime than to cache it.
            get { return new Font(this, Document); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsInsertRevision"]/*'/>
        public bool IsInsertRevision
        {
            get { return mParagraphBreakRunPr.HasInsertRevision; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsDeleteRevision"]/*'/>
        public bool IsDeleteRevision
        {
            get { return mParagraphBreakRunPr.HasDeleteRevision; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveFromRevision"]/*'/>
        public bool IsMoveFromRevision
        {
            get { return mParagraphBreakRunPr.HasMoveFromRevision; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveToRevision"]/*'/>
        public bool IsMoveToRevision
        {
            get { return mParagraphBreakRunPr.HasMoveToRevision; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsFormatRevision"]/*'/>
        public bool IsFormatRevision
        {
            get { return mParaPr.HasFormatRevision; }
        }

        /// <summary>
        /// Gets the paragraph style assigned to this paragraph.
        /// This sometimes is more efficient than using ParagraphFormat.Style.
        /// </summary>
        internal Style ParagraphStyle
        {
            get { return GetParagraphStyle(RevisionsView.Original); }
        }

        internal Style GetParagraphStyle(RevisionsView view)
        {
            object value = mParaPr.GetDirectAttr(ParaAttr.Istd, view);
            int istd = (value != null) ? (int)value : StyleIndex.Normal;
            return Document.Styles.FetchByIstd(istd, StyleIndex.Normal);
        }

        /// <summary>
        /// Gets the character style assigned to the paragraph break character.
        /// </summary>
        internal Style ParagraphBreakCharacterStyle
        {
            get { return Document.Styles.FetchByIstd(mParagraphBreakRunPr.Istd, StyleIndex.DefaultParagraphFont); }
        }

        internal ParaPr ParaPr
        {
            get { return mParaPr; }
            set
            {
                Debug.Assert(value != null);
                mParaPr = value;
            }
        }

        // WORDSNET-7485
        internal void CloneListLabelStringAndValueIfNeeded(Paragraph other)
        {
            if ((other.ListLabel.LabelFragments != null) && (other.ListLabel.NumberState != null))
            {
                ListLabel.SetLabelStringAndValue(other.ListLabel.LabelFragments, other.ListLabel.LabelArabicNumbers,
                    other.ListLabel.NumberState.Snapshot(), RevisionsView.Original);
            }
            if ((other.ListLabel.LabelFragmentsFinal != null) && (other.ListLabel.NumberStateFinal != null))
            {
                ListLabel.SetLabelStringAndValue(other.ListLabel.LabelFragmentsFinal, other.ListLabel.LabelArabicNumbersFinal,
                    other.ListLabel.NumberStateFinal.Snapshot(), RevisionsView.Final);
            }
        }

        internal void ResetListLabel(RevisionsView view)
        {
            if(mListLabel != null)
                mListLabel.SetLabelStringAndValue(null, null, null, view);
        }

        /// <summary>
        /// Gets or sets the font formatting of the paragraph mark.
        /// </summary>
        internal RunPr ParagraphBreakRunPr
        {
            get { return mParagraphBreakRunPr; }
            set
            {
                Debug.Assert(value != null);
                mParagraphBreakRunPr = value;
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Paragraph lhs = (Paragraph)base.Clone(isCloneChildren, cloningListener);

            ParaPr paraPr = mParaPr.Clone();
            lhs.mParaPr = paraPr;

            RunPr paragraphBreakRunPr = mParagraphBreakRunPr.Clone();
            lhs.mParagraphBreakRunPr = paragraphBreakRunPr;

            //Don't clone these because created on demand.
            lhs.mParagraphFormat = null;
            lhs.mFrameFormat = null;
            lhs.mListFormat = null;
            lhs.mListLabel = null;
            lhs.mRuns = null;

            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitParagraphStart"/>, then calls <see cref="Node.Accept"/> for all child nodes
        /// of the paragraph and calls <see cref="DocumentVisitor.VisitParagraphEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }


        /// <summary>
        /// Accepts a visitor for visiting the start of the document's paragraph.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitParagraphStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the document's paragraph.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitParagraphEnd(this);
        }

        /// <summary>
        /// Gets the text of this paragraph including the end of paragraph character.
        /// </summary>
        ///<remarks>
        ///<p>The text of all child nodes is concatenated and the end of paragraph character is appended as follows:</p>
        ///<list type="bullet">
        ///<item>If the paragraph is the last paragraph of <see cref="Body"/>, then
        ///<see cref="ControlChar.SectionBreak"/> (\x000c) is appended.</item>
        ///<item>If the paragraph is the last paragraph of <see cref="Cell"/>, then
        ///<see cref="ControlChar.Cell"/> (\x0007) is appended.</item>
        ///<item>For all other paragraphs
        ///<see cref="ControlChar.ParagraphBreak"/> (\r) is appended.</item>
        ///</list>
        ///<p>The returned string includes all control and special characters as described in <see cref="ControlChar"/>.</p>
        ///</remarks>
        public override string GetText()
        {
            // RK Although resharper says this is redundant method override, I want to keep it here
            // by design. It has customized API documentation comment that applies to paragraph's GetText.
            return base.GetText();
        }

        /// <summary>
        /// Returns fully expanded paragraph properties.
        /// </summary>
        internal ParaPr GetExpandedParaPr(ParaPrExpandFlags flags)
        {
            ParaPr dstParaPr = new ParaPr();
            ExpandParaPr(dstParaPr, flags);

            // Expand legacy tab stop if requested.
            if ((flags & ParaPrExpandFlags.ExpandLegacyTab) != 0)
            {
                TabStop legacyTabStop = LegacyListFormattingConverter.CalculateLegacyTab(this);
                if (legacyTabStop != null)
                {
                    if (!dstParaPr.ContainsKey(ParaAttr.TabStops))
                        dstParaPr.SetAttr(ParaAttr.TabStops, new TabStopCollection());

                    dstParaPr.TabStops.Add(legacyTabStop);
                }
            }

            return dstParaPr;
        }

        /// <summary>
        /// Fully expands paragraph properties of this object into one collection.
        /// </summary>
        internal void ExpandParaPr(ParaPr dstParaPr, ParaPrExpandFlags flags)
        {
            bool globalDefaults = ((flags & ParaPrExpandFlags.GlobalDefaults) != 0);
            bool documentDefaults = ((flags & ParaPrExpandFlags.DocumentDefaults) != 0);
            bool directFormatting = ((flags & ParaPrExpandFlags.NoDirectFormatting) == 0);
            // Reset flag immediately as we need to ignore direct formatting only for this entity.
            flags &= ~ParaPrExpandFlags.NoDirectFormatting;

            if (documentDefaults)
                Document.Styles.DefaultParaPr.ExpandTo(dstParaPr, globalDefaults);

            if ((flags & ParaPrExpandFlags.ExpandTableStyle) != 0)
            {
                // andrnosk: WORDSNET-6467 We need to expand table style too, because paragraphs inside table inherit table style.
                // Also AW does not need to skip direct formatting while expanding table style, otherwise direct formatting
                // for this style will be lost. See related test TestImportTableStyle.
                Cell cell = ParentCell;
                if (cell != null)
                {
                    Table table = cell.ParentTable;
                    if (table != null)
                    {
                        TableStyle tableStyle = table.Style as TableStyle;

                        if (tableStyle != null)
                        {
                            tableStyle.ExpandParaPr(cell, dstParaPr);
                        }
                    }
                }
            }

            ParaPr srcParaPr = mParaPr.GetSourceParaPr(flags);

            // Make sure layout knows that paragraph had format revision so that it can be marked in output.
            if ((flags & ParaPrExpandFlags.Revised) != 0 && mParaPr.HasFormatRevision)
                dstParaPr.FormatRevision = (FormatRevision)((IComplexAttr)mParaPr.FormatRevision).DeepCloneComplexAttr();

            // Expand style formatting
            Style paraStyle = Document.Styles.FetchByIstd(srcParaPr.Istd, StyleIndex.Normal);

            // Global defaults values and defaults values of the document already were expanded. So, AW does not need
            // to expand defaults values one more time.
            ParaPrExpandFlags paraStyleExpFlags = documentDefaults ? flags & ~ParaPrExpandFlags.DocumentDefaults : flags;
            paraStyleExpFlags = globalDefaults ? paraStyleExpFlags & ~ParaPrExpandFlags.GlobalDefaults : paraStyleExpFlags;

            paraStyle.ExpandParaPr(dstParaPr, paraStyleExpFlags);

            // Style has been expanded into dstParaPr so we have listId defined by style in it.
            // Check if direct formatting defines either different listId or different list level.
            if ((dstParaPr.ListId != srcParaPr.ListId) || (dstParaPr.ListLevel != srcParaPr.ListLevel))
            {
                // WORDSNET-11291 If srcPara has no list level defined we should inherit it from parent style.
                // Here I use the fact that we already have parent style expanded into dstParaPr and
                // don't need to calculate it once again.
                ParaPr newParaPr = srcParaPr;
                if(!srcParaPr.Contains(ParaAttr.ListLevel) && dstParaPr.Contains(ParaAttr.ListLevel))
                {
                    // We should not modify original collection so create new temporary.
                    newParaPr = srcParaPr.Clone();
                    newParaPr.SetAttr(ParaAttr.ListLevel, dstParaPr[ParaAttr.ListLevel]);
                }

                Document.Lists.ExpandDirectList(newParaPr, dstParaPr);
            }

            if ((flags & ParaPrExpandFlags.ExpandHtmlBlocks) != 0)
            {
                if (mParaPr.Contains(ParaAttr.HtmlBlockId))
                    Document.HtmlBlockCollection.Expand(mParaPr, dstParaPr, (ParentTable == null));
            }

            // Expand direct formatting
            if (directFormatting)
                srcParaPr.ExpandTo(dstParaPr);

            srcParaPr.FrameInheritanceHack(dstParaPr);

            // AM. Maybe we should use more complex logic as ListCollection.ExpandDirectList does?
            if (srcParaPr.IsExplicitlyNotListItem)
            {
                // Do not overwrite direct indent that might be set.
                if (!srcParaPr.Contains(ParaAttr.FirstLineIndent))
                    dstParaPr.FirstLineIndent = 0;

                if (!srcParaPr.Contains(ParaAttr.LeftIndent))
                    dstParaPr.LeftIndent = 0;

                // WORDSNET-21040 LeftIndentUnits and FirstLineIndentUnits should be also handled here.
                // Note, we need to remove only inherited values and
                // return zero, if global defaults were requested.
                if (!srcParaPr.Contains(ParaAttr.LeftIndentUnits))
                {
                    if (globalDefaults)
                        dstParaPr[ParaAttr.LeftIndentUnits] = 0;
                    else
                        dstParaPr.Remove(ParaAttr.LeftIndentUnits);

                }

                if (!srcParaPr.Contains(ParaAttr.FirstLineIndentUnits))
                {
                    if (globalDefaults)
                        dstParaPr[ParaAttr.FirstLineIndentUnits] = 0;
                    else
                        dstParaPr.Remove(ParaAttr.FirstLineIndentUnits);
                }
            }

            // WORDSNET-7115 It seems that Word ignores floating attributes for textboxes.
            if (IsInShape)
                dstParaPr.RemoveFloatingAttrs();

            if ((flags & ParaPrExpandFlags.RemoveClearTabStops) != 0)
                dstParaPr.RemoveClearTabStops();
        }

        /// <summary>
        /// Fully expands font properties of the paragraph break.
        /// </summary>
        internal RunPr GetExpandedParagraphBreakRunPr(RunPrExpandFlags flags)
        {
            RunPr dstRunPr = new RunPr();
            ExpandParagraphBreakRunPr(dstRunPr, flags);
            return dstRunPr;
        }

        internal void ExpandParagraphBreakRunPr(RunPr dstRunPr, RunPrExpandFlags flags)
        {
            InlineHelper.ExpandRunPr(this, dstRunPr, flags);
        }

        /// <summary>
        /// Removes direct formatting that duplicates inherited formatting.
        /// </summary>
        internal void RemoveDuplicateDirectFormatting(RevisionsView view)
        {
            // We enumerate attributes in the reverse order, because we remove duplicate attributes in place and want to make
            // sure this doesn't change indexes of other attributes that we haven't checked yet.
            for (int i = mParaPr.Count - 1; i >= 0; i--)
            {
                int key = mParaPr.GetKey(i);

                // We don't want the style or list properties of the paragraph to change in the process,
                // because resolution of inherited formatting depends these values.
                if ((key == ParaAttr.Istd) || (key == ParaAttr.ListId) || (key == ParaAttr.ListLevel))
                {
                    continue;
                }

                object value = mParaPr.GetDirectAttr(key, view);
                Debug.Assert(value != null);
                object inheritedReferenceValue = FetchInheritedParaAttr(key, view);
                if (value.Equals(inheritedReferenceValue))
                {
                    // Remove direct formatting. The inherited value will be used instead.
                    mParaPr.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in a Comment; false otherwise.
        /// </summary>
        internal bool IsEndOfComment
        {
            get
            {
                CompositeNode parent = FirstNonMarkupParentNode;
                return (parent is Comment) && (this == parent.LastNonMarkupCompositeDescendant);
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in a Footnote; false otherwise.
        /// </summary>
        internal bool IsEndOfFootnote
        {
            get
            {
                CompositeNode parent = FirstNonMarkupParentNode;
                return (parent is Footnote) && (this == parent.LastNonMarkupCompositeDescendant);
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph of an endnote/footnote separator; false otherwise.
        /// </summary>
        internal bool IsEndOfNoteSeparator
        {
            get
            {
                // All note separator types are stored under FootnoteSeparator in the model.
                // Despite the type name, this includes endnote and continuation separators as well.
                CompositeNode parent = FirstNonMarkupParentNode;
                return (parent is FootnoteSeparator) && (this == parent.LastNonMarkupCompositeDescendant);
            }
        }

        /// <summary>
        /// True if this paragraph is the last paragraph in a shape; false otherwise.
        /// </summary>
        internal bool IsEndOfTextbox
        {
            get
            {
                CompositeNode parent = FirstNonMarkupParentNode;
                return (parent is Shape && parent.LastNonMarkupCompositeDescendant == this);
            }
        }

        /// <summary>
        /// Calculates minimum and maximum line height in points and criterion whether it can change from inherited.
        /// Maximum is basically the biggest font size used in the paragraph. Maybe some other attributes should be analyzed.
        /// Note that like in MS Word, the font size of the paragraph mark is only taken into account
        /// when the paragraph is empty.
        /// Changing criterion is needed to properly output attributes with relative size (percentage).
        /// I don't like this patch. It would be better to calculate this all if only needed.
        /// This code doesn't consider list label if any. Maybe we need it too.
        /// </summary>
        internal void GetMinMaxFontSize(out double minFontSize, out double maxFontSize, out bool sizeCanChange)
        {
            minFontSize = int.MaxValue;
            maxFontSize = 0;
            sizeCanChange = false;
            for (Node node = FirstNonMarkupDescendant; node != null; node = node.NextNonMarkupNodeLimited)
            {
                // I should do this using interfaces. Looking at base classes is not really good.
                if (node is Inline)
                    ConsiderInMinMaxFontSize((Inline)node, ref minFontSize, ref maxFontSize, ref sizeCanChange);
            }

            // If the paragraph is empty, then the paragraph mark itself dictates the font size.
            // It also can have non-default size.
            if (maxFontSize == 0)
            {
                // Passing this as IRunAttrSource means paragraph break properties.
                // I don't like this. Maybe it's better to make more explicit.
                ConsiderInMinMaxFontSize(this, ref minFontSize, ref maxFontSize, ref sizeCanChange);
            }
        }

        /// <summary>
        /// Considers one IRunAttrSource in the min/max font size and changing criterion.
        /// </summary>
        private static void ConsiderInMinMaxFontSize(IRunAttrSource attrSource, ref double minFontSize, ref double maxFontSize, ref bool sizeCanChange)
        {
            object directSize = attrSource.GetDirectRunAttr(FontAttr.Size);
            double curSize = ConvertUtilCore.HalfPointToPoint((int)((directSize != null) ? directSize : attrSource.FetchInheritedRunAttr(FontAttr.Size)));

            minFontSize = System.Math.Min(curSize, minFontSize);
            maxFontSize = System.Math.Max(curSize, maxFontSize);

            if (!sizeCanChange)
            {
                sizeCanChange = (directSize != null);
                if (!sizeCanChange)
                {
                    object istdObj = attrSource.GetDirectRunAttr(FontAttr.Istd);
                    sizeCanChange = (istdObj != null) && ((int)istdObj != StyleIndex.DefaultParagraphFont);
                }
            }
        }

        /// <summary>
        /// Returns the appropriate end of paragraph character.
        /// </summary>
        internal override string GetEndText()
        {
            if (IsEndOfCell)
                return ControlChar.Cell;
            else if (IsEndOfSection)
                return ControlChar.SectionBreak;
            else
                return ControlChar.ParagraphBreak;
        }

        /// <summary>
        /// Allows to insert inline-level elements.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            // WORDSNET-22783, 23418. Not allowing textboxes and line shapes to be child to textboxes.
            Shape childShape = newChild as Shape;
            if ((childShape != null) &&
                (childShape.IsTextBox || (childShape.ShapeType == ShapeType.Line)))
            {
                Shape parentShape = (Shape)GetAncestor(NodeType.Shape);
                if ((parentShape != null) && parentShape.IsTextBox)
                    return false;
            }

            return NodeUtil.IsInlineLevelNode(newChild);
        }

        #region IParaAttrSource

        object IParaAttrSource.GetDirectParaAttr(int key)
        {
            return mParaPr.GetDirectAttr(key);
        }

        object IParaAttrSource.GetDirectParaAttr(int key, RevisionsView revisionsView)
        {
            return mParaPr.GetDirectAttr(key, revisionsView);
        }

        object IParaAttrSource.FetchInheritedParaAttr(int key)
        {
            return FetchInheritedParaAttr(key, RevisionsView.Original);
        }

        object IParaAttrSource.FetchParaAttr(int key)
        {
            return FetchParaAttr(key, RevisionsView.Original);
        }

        void IParaAttrSource.SetParaAttr(int key, object value)
        {
            mParaPr.SetAttr(key, value);

            switch (key)
            {
                case ParaAttr.SpaceBeforeUnits:
                case ParaAttr.SpaceAfterUnits:
                case ParaAttr.LeftIndentUnits:
                case ParaAttr.RightIndentUnits:
                case ParaAttr.FirstLineIndentUnits:
                    // Setting unit spacing/indent updates normal spacing/indent accordingly.
                    UpdateFromUnits();
                    break;

                default:
                    break;
            }
        }

        void IParaAttrSource.RemoveParaAttr(int key)
        {
            mParaPr.Remove(key);
        }

        void IParaAttrSource.ClearParaAttrs()
        {
            mParaPr.Clear();
        }

        #endregion

        internal object FetchInheritedParaAttr(int key, RevisionsView view)
        {
            object value;

            // 1. RK When fetching inherited value we need to look at the list formatting first.
            ListLevel listLevel = Document.Lists.GetDirectListLevel(ParaPr, view);
            if (listLevel != null)
            {
                value = listLevel.ParaPr.GetDirectAttr(key);
                if (value != null)
                    return value;
            }
            else
            {
                object listId = mParaPr.GetDirectAttr(ParaAttr.ListId, view);

                // WORDSNET-18137 (related to 1545) - Microsoft Word seems to be resetting to zero indents for a paragraph
                // that has bulleting explicitly turned off, but formatted with a style that has bullets.)
                // WORDSNET-21040 Note, the reason of such behavior is actually not related to bullets,
                // but rather just relates to the fact that listId explicitly set to zero.
                // This is a special case in Word described in 17.9.18 numId
                // (Numbering Definition Instance Reference) of ISO29500-1.
                if (((listId != null) && ((int)listId == 0)) && ParaPr.IsNoListAttr(key))
                    return 0;
            }

            // 2. Look at the paragraph styles but skip document-wide or global defaults.
            value = GetParagraphStyle(view).GetParaAttr(key, view);
            if (value != null)
                return value;

            // 3. Return either from table styles or defaults.
            if (IsTableStyleAffected)
            {
                TableStyle tableStyle = (TableStyle) ParentTable.Style;
                value = tableStyle.GetParaAttr(key, ParentCell);
                if (value != null)
                    return value;
            }

            return Document.Styles.DefaultParaPr.FetchAttr(key);
        }

        /// <summary>
        /// Indicates that paragraph formatting can be affected by table styles.
        /// </summary>
        private bool IsTableStyleAffected
        {
            get
            {
                Cell cell = ParentCell;
                if (cell == null)
                    return false;

                Row row = cell.ParentRow;
                if (row == null)
                    return false;

                Table table = row.ParentTable;
                if (table == null)
                    return false;

                TableStyle tableStyle = Document.Styles.GetByIstd(ParentTable.Istd, false) as TableStyle;

                if (tableStyle == null)
                    return false;

                // TableNormal style has predefined attribute set and has no paragraph properties.
                if (tableStyle.StyleIdentifier == StyleIdentifier.TableNormal)
                    return false;

                return true;
            }
        }

        internal object FetchParaAttr(int key, RevisionsView view)
        {
            object value = mParaPr.GetDirectAttr(key, view);
            return (value != null) ? value : FetchInheritedParaAttr(key, view);
        }

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return ((IRunAttrSource)this).GetDirectRunAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mParagraphBreakRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            //TODO 2 Same code as in Inline, maybe can refactor.

            //Try to find the value specified in the character style or one of the based on character styles.
            object value = ParagraphBreakCharacterStyle.GetFontAttr(key, false);
            if (value != null)
                return value;

            //Get the value from the paragraph styles or from the default attributes.
            return ParagraphStyle.GetFontAttr(key, true);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            mParagraphBreakRunPr.SetAttr(key, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mParagraphBreakRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mParagraphBreakRunPr.Clear();
        }

        #endregion

        EditRevision ITrackableNode.InsertRevision
        {
            get { return ParagraphBreakRunPr.InsertRevision; }
            set { ParagraphBreakRunPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return ParagraphBreakRunPr.DeleteRevision; }
            set { ParagraphBreakRunPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return ParagraphBreakRunPr.MoveFromRevision; }
            set { ParagraphBreakRunPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return ParagraphBreakRunPr.MoveToRevision; }
            set { ParagraphBreakRunPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            ParagraphBreakRunPr.Remove(RevisionAttr.MoveFromRevision);
            ParagraphBreakRunPr.Remove(RevisionAttr.MoveToRevision);
        }

        /// <summary>
        /// Conforming paragraphs in MS Word have same left and right indent and a few other
        /// attributes that cause MS Word to draw borders in a special way.
        /// MS Word does not draw the Top and Bottom borders between the conforming paragraphs,
        /// but draws the Between border between them.
        /// </summary>
        internal bool IsConformingWithNext()
        {
            Node next = NextNonMarkupCompositeLimited;
            if (!(next is Paragraph))
                return false;

            Paragraph nextParagraph = (Paragraph)next;
            ParagraphFormat pf = ParagraphFormat;
            ParagraphFormat pfNext = nextParagraph.ParagraphFormat;

            if (pfNext.PageBreakBefore)
                return false;

            if (nextParagraph.ParaPr.HtmlBlockId != ParaPr.HtmlBlockId)
                return false;

            // We ignore (convert to zero) positive first line indent since it does not affect conformity
            // of the paragraphs. But we add negative first line indent (hanging) since MS Word treats
            // it like it affects left indent and therefore affects conformity.
            double thisLeft = pf.LeftIndent + System.Math.Min(pf.FirstLineIndent, 0);
            double nextLeft = pfNext.LeftIndent + System.Math.Min(pfNext.FirstLineIndent, 0);
            if (thisLeft != nextLeft)
                return false;

            if (pf.RightIndent != pfNext.RightIndent)
                return false;

            if (!pf.Borders[BorderType.Left].Equals(pfNext.Borders[BorderType.Left]))
                return false;

            if (!pf.Borders[BorderType.Right].Equals(pfNext.Borders[BorderType.Right]))
                return false;

            if (!pf.Borders[BorderType.Top].Equals(pfNext.Borders[BorderType.Top]))
                return false;

            if (!pf.Borders[BorderType.Bottom].Equals(pfNext.Borders[BorderType.Bottom]))
                return false;

            if (!IsSameFloatingPositioning(nextParagraph))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if this paragraph has same absolute positioning with the specified paragraph.
        /// </summary>
        internal bool IsSameFloatingPositioning(Paragraph rhs)
        {
            // In MS Word I hope text frame positioning attributes cannot be inherited from the style,
            // so it is cheaper to compare attributes in ParaPr directly, without looking at ParagraphFormat.
            return mParaPr.IsSameFloatingPositioning(rhs.ParaPr);
        }

        /// <summary>
        /// Gets the first run.
        /// </summary>
        internal Run FirstRun
        {
            get { return (Run)GetChild(NodeType.Run, 0, false); }
        }

        /// <summary>
        /// Gets the last run in the paragraph. The returned run can be empty.
        /// </summary>
        internal Run GetLastRun()
        {
            return GetLastRun(false);
        }

        /// <summary>
        /// Gets the last run in the paragraph optionally skipping empty ones.
        /// </summary>
        internal Run GetLastRun(bool skipEmpty)
        {
            Run lastRun = null;
            for (Node node = FirstNonMarkupDescendant; node != null; node = node.NextNonMarkupNodeLimited)
            {
                if ((node.NodeType == NodeType.Run) && (!skipEmpty || !string.IsNullOrEmpty(node.GetText())))
                    lastRun = (Run)node;
            }

            return lastRun;
        }

        internal bool IsInMainTextStory
        {
            get
            {
                Story parentStory = ParentStory;
                return ((parentStory != null) && (parentStory.StoryType == StoryType.MainText));
            }
        }

        /// <summary>
        /// True when the paragraph is an item in a bulleted or numbered list in original revision.
        /// </summary>
        public bool IsListItem
        {
            get { return IsListItemOriginal; }
        }

        /// <summary>
        /// Returns array of all tab stops applied to this paragraph, including applied indirectly by styles or lists.
        /// </summary>
        public TabStop[] GetEffectiveTabStops()
        {
            ParaPr expandedParaPr = GetExpandedParaPr(ParaPrExpandFlags.Normal);

            int tabStopCount = expandedParaPr.Contains(ParaAttr.TabStops) ? expandedParaPr.TabStops.Count : 0;

            TabStop[] tabStops = new TabStop[tabStopCount];

            for (int i = 0; i < tabStopCount; i++)
                tabStops[i] = expandedParaPr.TabStops[i].Clone();

            return tabStops;
        }

        /// <summary>
        /// Joins runs with the same formatting in the paragraph.
        /// </summary>
        /// <returns>Number of joins performed. When <b>N</b> adjacent runs are being joined they count as <b>N - 1</b> joins.</returns>
        public int JoinRunsWithSameFormatting()
        {
            return JoinRunsWithSameFormatting(new StringBuilder());
        }

        /// <summary>
        /// Returns true if the paragraph has list label in original revision.
        /// </summary>
        /// <remarks>
        /// A paragraph can be list item and have no list label.
        /// </remarks>
        internal bool HasListLabel
        {
            get { return IsListItemOriginal && ListLabel.HasChars; }
        }

        /// <summary>
        /// Returns true if the paragraph has list label in final revision.
        /// </summary>
        /// <remarks>
        /// A paragraph can be list item and have no list label.
        /// </remarks>
        internal bool HasListLabelFinal
        {
            get { return IsListItemFinal && ListLabel.HasCharsFinal; }
        }

        /// <summary>
        /// Returns true if the paragraph has insert, delete, formatting, or list numbering revision.
        /// </summary>
        internal bool HasRevisions
        {
            get { return (ParagraphBreakRunPr.HasRevisions || ParaPr.HasRevisions || ParaPr.HasNumberRevision); }
        }

        /// <summary>
        /// Returns true if the paragraph contains text other than whitespaces or any other nodes.
        /// </summary>
        internal bool HasNonWhitespaceChildren
        {
            get
            {
                for (Node childNode = FirstChild; childNode != null; childNode = childNode.NextSibling)
                {
                    if (NodeUtil.IsNonWhitespace(childNode))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks that paragraph is empty or contains only bookmark and comment range nodes.
        /// </summary>
        internal bool IsEmptyOrContainsOnlyCrossAnnotation
        {
            get
            {
                foreach (Node node in GetChildNodes(NodeType.Any, false))
                    if (!NodeUtil.IsCrossStructureAnnotation(node))
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Joins runs with same formatting in the paragraph.
        /// StringBuilder is passed to avoid its recreation since we process many paragraphs.
        /// Someday we can make override without parameters public and retain this one internal.
        /// </summary>
        /// <param name="sb">Utility StringBuilder</param>
        /// <returns>Number of joins performed</returns>
        internal int JoinRunsWithSameFormatting(StringBuilder sb)
        {
            return JoinRunsSpecificParent(this, sb);
        }

        private static int JoinRunsSpecificParent(CompositeNode parent, StringBuilder sb)
        {
            // Here we cannot use Runs collection because we should join only adjacent runs.
            // What is done in the loop:
            //   Remember the first found run and try joining with the very next.
            //   On success remove the first of them from the parent. Otherwise remember the second
            //   as candidate for joining with next.
            //   If current node is not a run then forget what we remembered.
            //   There are many cases with multiple runs in a sequence. So collect text parts in StringBuilder.

            Debug.Assert((sb != null) && (sb.Length == 0));

            int joinCount = 0;
            Run prevRun = null;

            const RunPrExpandFlags expandFlags = RunPrExpandFlags.DocumentDefaults | RunPrExpandFlags.GlobalDefaults;
            RunPr prevRunPrExp = null;

            for (Node curNode = parent.FirstChild; curNode != null; curNode = curNode.NextSibling)
            {
                if (curNode.NodeType == NodeType.Run)
                {
                    Run curRun = (Run)curNode;

                    if (prevRun != null)
                    {
                        // Found two adjacent runs. Let's compare their completely expanded properties.
                        // During comparison some attributes should be ignored. See Run.KeysToIgnoreInComparisonOnJoin.
                        // Then collect texts in StringBuilder if properties are equal.
                        // Empty StringBuilder means that it is first join in the sequence.
                        RunPr curRunPrExp = curRun.GetExpandedRunPr(expandFlags);
                        if (prevRunPrExp == null)
                            prevRunPrExp = prevRun.GetExpandedRunPr(expandFlags);

                        if (RunPr.IsSameFormatting(curRunPrExp, prevRunPrExp))
                        {
                            if (sb.Length == 0)
                                sb.Append(prevRun.Text);
                            sb.Append(curRun.Text);

                            ++joinCount;
                            parent.RemoveChild(prevRun);
                        }
                        else
                        {
                            FlushJoinedRunsText(prevRun, sb);
                            prevRunPrExp = null;
                        }
                    }

                    prevRun = curRun;
                }
                else
                {
                    // Flush if any collected and forget last candidate since the sequence is finished.
                    FlushJoinedRunsText(prevRun, sb);
                    prevRun = null;
                    prevRunPrExp = null;
                }

                // WORDSNET-20358 Join runs within StructuredDocumentTag.
                if (curNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)curNode;
                    JoinRunsSpecificParent(sdt, sb);
                }
            }

            // Reached the end of paragraph. Flush if any collected.
            FlushJoinedRunsText(prevRun, sb);
            Debug.Assert(sb.Length == 0);

            return joinCount;
        }

        /// <summary>
        /// Flushes collected text into the given run and reset StringBuilder.
        /// </summary>
        private static void FlushJoinedRunsText(Run run, StringBuilder sb)
        {
            if ((run != null) && (sb.Length != 0))
            {
                run.Text = sb.ToString();
                sb.Length = 0;
            }
        }

        /// <overloads>Appends a Word field to this paragraph.</overloads>
        /// <summary>
        /// Appends a field to this paragraph.
        /// </summary>
        /// <param name="fieldType">The type of the field to append.</param>
        /// <param name="updateField">Specifies whether to update the field immediately.</param>
        /// <returns>A <see cref="Field"/> object that represents the appended field.</returns>
        public Field AppendField(FieldType fieldType, bool updateField)
        {
            return InsertField(fieldType, updateField, null, true);
        }

        /// <summary>
        /// Appends a field to this paragraph.
        /// </summary>
        /// <param name="fieldCode">The field code to append (without curly braces).</param>
        /// <returns>A <see cref="Field"/> object that represents the appended field.</returns>
        public Field AppendField(string fieldCode)
        {
            return InsertField(fieldCode, null, true);
        }

        /// <summary>
        /// Appends a field to this paragraph.
        /// </summary>
        /// <param name="fieldCode">The field code to append (without curly braces).</param>
        /// <param name="fieldValue">The field value to append. Pass <c>null</c> for fields that do not have a value.</param>
        /// <returns>A <see cref="Field"/> object that represents the appended field.</returns>
        public Field AppendField(string fieldCode, string fieldValue)
        {
            return InsertField(fieldCode, fieldValue, null, true);
        }

        /// <overloads>Inserts a Word field into this paragraph.</overloads>
        /// <summary>
        /// Inserts a field into this paragraph.
        /// </summary>
        /// <param name="fieldType">The type of the field to insert.</param>
        /// <param name="updateField">Specifies whether to update the field immediately.</param>
        /// <param name="refNode">Reference node inside this paragraph (if <paramref name="refNode"/> is <c>null</c>, then appends to the end of the paragraph).</param>
        /// <param name="isAfter">Whether to insert the field after or before reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(FieldType fieldType, bool updateField, Node refNode, bool isAfter)
        {
            return FieldAppender.InsertField(fieldType, updateField, GetRunPr(refNode), this, refNode, isAfter);
        }

        /// <summary>
        /// Inserts a field into this paragraph.
        /// </summary>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="refNode">Reference node inside this paragraph (if <paramref name="refNode"/> is <c>null</c>, then appends to the end of the paragraph).</param>
        /// <param name="isAfter">Whether to insert the field after or before reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(string fieldCode, Node refNode, bool isAfter)
        {
            return FieldAppender.InsertField(fieldCode, GetRunPr(refNode), this, refNode, isAfter);
        }

        /// <summary>
        /// Inserts a field into this paragraph.
        /// </summary>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="fieldValue">The field value to insert. Pass <c>null</c> for fields that do not have a value.</param>
        /// <param name="refNode">Reference node inside this paragraph (if <paramref name="refNode"/> is <c>null</c>, then appends to the end of the paragraph).</param>
        /// <param name="isAfter">Whether to insert the field after or before reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(string fieldCode, string fieldValue, Node refNode, bool isAfter)
        {
            return FieldAppender.InsertField(fieldCode, fieldValue, GetRunPr(refNode), this, refNode, isAfter);
        }

        internal bool IsListItemOriginal
        {
            get { return (int)FetchParaAttr(ParaAttr.ListId, RevisionsView.Original) != 0; }
        }

        internal bool IsListItemFinal
        {
            get { return (int)FetchParaAttr(ParaAttr.ListId, RevisionsView.Final) != 0; }
        }

        /// <summary>
        /// Returns original list level applied to the current paragraph.
        /// </summary>
        internal ListLevel ListLevel
        {
            get { return GetListLevel(false); }
        }

        /// <summary>
        /// Returns either original or final list applied to the current paragraph.
        /// </summary>
        internal List GetList(bool isRevised)
        {
            RevisionsView view = isRevised ? RevisionsView.Final : RevisionsView.Original;

            int listId = (int)FetchParaAttr(ParaAttr.ListId, view);
            if (listId == 0)
                return null;

            List list = Document.Lists.FetchListByListId(listId);

            return list;
        }

        /// <summary>
        /// Returns either original or final list level applied to the current paragraph.
        /// </summary>
        internal ListLevel GetListLevel(bool isRevised)
        {
            RevisionsView view = isRevised ? RevisionsView.Final : RevisionsView.Original;

            int listId = (int)FetchParaAttr(ParaAttr.ListId, view);
            if (listId == 0)
                return null;

            List list = Document.Lists.FetchListByListId(listId);

            int listLevel = (int)FetchParaAttr(ParaAttr.ListLevel, view);

            ListLevel level = list.GetListLevelOverrideAware(listLevel);

            return level;
        }

#if DEBUG && !CPLUSPLUS
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "{0} {1}",
                base.ToString(),
                StringUtil.Ellipsisize(GetText().TrimEnd(ControlChar.ParagraphBreakChar), 50));
        }
#endif

        /// <summary>
        /// <para>
        /// Gets integer value representing nesting level of BlockQuote
        /// when the paragraph belongs to some <see cref="HtmlBlock"/>.
        /// </para>
        /// <para>
        /// A <c>zero</c> value means the paragraph is not in BlockQuote.
        /// </para>
        /// </summary>
        internal int HtmlBlockQuoteLevel
        {
            get
            {
                HtmlBlock htmlBlock = Document.HtmlBlockCollection.GetHtmlBlockById(ParaPr.HtmlBlockId);
                int level = 0;
                while (htmlBlock != null)
                {
                    if (htmlBlock.HtmlBlockType == HtmlBlockType.BlockQuote)
                        level++;

                    htmlBlock = Document.HtmlBlockCollection.GetHtmlBlockById(htmlBlock.ParentId);
                }

                return level;
            }
        }

        /// <summary>
        /// Specifies an identifier for a paragraph that is unique within the document part.
        /// </summary>
        /// <remarks>
        /// See [MS-DOCX] 2.4.1 paraId.
        ///
        /// AM. I don't want to put this into attribute collection since this property is not inheritable.
        /// We already put rsid related attributes to collection and now have to write some additional code,
        /// for example AttrCollection.IsIgnoredOnCollapse() method. I want to avoid it.
        /// </remarks>
        internal int ParaId { get; set; }

        /// <summary>
        /// Specifies version identifier for the paragraph.
        /// </summary>
        /// <remarks>
        /// See [MS-DOCX] 2.4.2 textId
        /// </remarks>
        internal int TextId { get; set; }

        private RunPr GetRunPr(Node refNode)
        {
            if (refNode == null)
            {
                if (Runs.Count > 0)
                    return Runs[Runs.Count - 1].RunPr;
            }
            else
            {
                if (refNode is Run && refNode.ParentNode == this)
                    return ((Run)refNode).RunPr;
            }

            return ParagraphBreakRunPr;
        }

        /// <summary>
        /// Updates normal spacings/indents from unit attribute values.
        /// </summary>
        [JavaThrows(false)]
        private void UpdateFromUnits()
        {
            // Do not update if paragraph outside document section.
            if ((Document == null) || (ParentSection == null) || (Document.NodeType != NodeType.Document))
                return;

            UnitConverter converter = new UnitConverter();
            converter.VisitDocumentStart((Document)Document);
            // WORDSNET-26551 Update from units depending on story type.
            if (ParentStory.NodeType == NodeType.Body)
                converter.VisitBodyStart((Body)ParentStory);
            else
                converter.VisitHeaderFooterStart((HeaderFooter)ParentStory);
            converter.VisitParagraphStart(this);
        }

        // These need to be cloned.
        private ParaPr mParaPr;
        /// <summary>
        /// The formatting of the paragraph mark.
        /// </summary>
        private RunPr mParagraphBreakRunPr;

        // These should not be cloned.
        private ParagraphFormat mParagraphFormat;
        private FrameFormat mFrameFormat;
        private ListFormat mListFormat;
        private ListLabel mListLabel;
        private RunCollection mRuns;
    }
}
