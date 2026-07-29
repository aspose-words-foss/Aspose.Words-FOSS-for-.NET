// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for inline-level nodes that can contain paragraphs and tables.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/logical-levels-of-nodes-in-a-document/">Logical Levels of Nodes in a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="InlineStory"/> is a container for block-level nodes <see cref="Paragraph"/> and <see cref="Table"/>.</p>
    ///
    /// <p>The classes that derive from <see cref="InlineStory"/> are inline-level nodes that can contain
    /// their own text (paragraphs and tables). For example, a <see cref="Comment"/> node contains text of a comment
    /// and a <see cref="Aspose.Words.Notes.Footnote"/> contains text of a footnote.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public abstract class InlineStory : CompositeNode, IInline, IStory
    {
        internal InlineStory(DocumentBase doc, RunPr runPr) : base(doc)
        {
            if (runPr == null)
                throw new ArgumentNullException("runPr");

            RunPr = runPr;
        }

        /// <summary>
        /// Returns the type of the story.
        /// </summary>
        public abstract StoryType StoryType { get; }

        /// <summary>
        /// Retrieves the parent <see cref="Paragraph"/> of this node.
        /// </summary>
        public Paragraph ParentParagraph
        {
            get { return (Paragraph)GetAncestor(NodeType.Paragraph); }
        }

        /// <summary>
        /// Gets the first paragraph in the story.
        /// </summary>
        public Paragraph FirstParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, 0, false); }
        }

        /// <summary>
        /// Gets the last paragraph in the story.
        /// </summary>
        public Paragraph LastParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, -1, false); }
        }

        /// <summary>
        /// Gets a collection of paragraphs that are immediate children of the story.
        /// </summary>
        public ParagraphCollection Paragraphs
        {
            get { return mParagraphs ?? (mParagraphs = new ParagraphCollection(this)); }
        }

        /// <summary>
        /// Gets a collection of tables that are immediate children of the story.
        /// </summary>
        public TableCollection Tables
        {
            get { return mTables ?? (mTables = new TableCollection(this)); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsInsertRevision"]/*'/>
        public bool IsInsertRevision
        {
            get { return InlineHelper.IsInsertRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsDeleteRevision"]/*'/>
        public bool IsDeleteRevision
        {
            get { return InlineHelper.IsDeleteRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveFromRevision"]/*'/>
        public bool IsMoveFromRevision
        {
            get { return InlineHelper.IsMoveFromRevision(this); }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsMoveToRevision"]/*'/>
        public bool IsMoveToRevision
        {
            get { return InlineHelper.IsMoveToRevision(this); }
        }

        /// <summary>
        /// Provides access to the font formatting of the anchor character of this object.
        /// </summary>
        public Font Font
        {
            get { return mFont ?? (mFont = new Font(this, Document)); }
        }

        internal RunPr RunPr { get; set; }

        RunPr IInline.RunPr_IInline
        {
            get { return RunPr; }
            set { RunPr = value; }
        }

        /// <summary>
        /// Added by VA for DOCX import.
        /// </summary>
        internal void SetRunPrInternal(RunPr runPr)
        {
            RunPr = runPr;
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            InlineStory lhs = (InlineStory)base.Clone(isCloneChildren, cloningListener);
            lhs.RunPr = RunPr.Clone();
            lhs.mFont = null;
            lhs.mParagraphs = null;
            lhs.mTables = null;
            return lhs;
        }

        /// <summary>
        /// If the last child is not a paragraph, creates and appends one empty paragraph.
        /// </summary>
        public void EnsureMinimum()
        {
            WordUtil.EnsureNonEmptyStory(this);
        }

        /// <summary>
        /// Allows to insert block level nodes.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsBlockLevelNode(newChild);
        }

        /// <summary>
        /// Creates a fully expanded version of the RunPr object.
        /// </summary>
        internal RunPr GetExpandedRunPr(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        Paragraph IInline.ParentParagraph_IInline
        {
            get { return this.ParentParagraph; }
        }

        DocumentBase IInline.Document_IInline
        {
            get { return this.Document; }
        }

        /// <summary>
        /// Creates a fully expanded version of the RunPr object.
        /// </summary>
        RunPr IInline.GetExpandedRunPr_IInline(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return RunPr.GetDirectAttr(key);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return RunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            return InlineHelper.FetchInheritedAttr(this, fontAttr);
        }

        void IRunAttrSource.SetRunAttr(int fontAttr, object value)
        {
            RunPr.SetAttr(fontAttr, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            RunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            RunPr.Clear();
        }

        #endregion

        private Font mFont;
        private ParagraphCollection mParagraphs;
        private TableCollection mTables;
    }
}
