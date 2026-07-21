// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using Aspose.Fonts;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Revisions;
using Aspose.Words.Themes;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for inline-level nodes that can have character formatting associated with them, but cannot have child nodes of their own.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/logical-levels-of-nodes-in-a-document/">Logical Levels of Nodes in a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A class derived from <see cref="Inline"/> can be a child of <see cref="Paragraph"/>.</p>
    /// </remarks>
    public abstract class Inline : Node, IInline, ITrackableNode
    {
        internal Inline(DocumentBase doc, RunPr runPr) : base(doc)
        {
            if (runPr == null)
                throw new ArgumentNullException("runPr");

            mRunPr = runPr;
        }

        /// <summary>
        /// Retrieves the parent <see cref="Paragraph"/> of this node.
        /// </summary>
        public Paragraph ParentParagraph
        {
            get { return (Paragraph)GetAncestor(NodeType.Paragraph); }
        }

        /// <summary>
        /// Provides access to the font formatting of this object.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(this, Document);
                return mFont;
            }
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

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="IsFormatRevision"]/*'/>
        public bool IsFormatRevision
        {
            get { return InlineHelper.IsFormatRevision(this); }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the node accepts the given visitor.
        /// </summary>
        /// <remarks>
        /// This is used to skip the nodes marked for deletion.
        /// </remarks>
        /// <param name="visitor"></param>
        /// <returns></returns>
        internal bool IsVisitorAcceptable(DocumentVisitor visitor)
        {
            return !IsDeleteRevision || visitor.VisitsDeletedNodes;
        }

        /// <summary>
        /// Gets a boolean value indicating if the node is hidden or deleted.
        /// </summary>
        /// <remarks>
        /// It's a shortcut as these two checks often go together.
        /// </remarks>
        /// <returns></returns>
        internal bool IsHiddenOrDeleted
        {
            get { return IsDeleteRevision || Font.Hidden;}
        }

        /// <summary>
        /// Gets the character style assigned to this inline object.
        /// This sometimes is more efficient than using Font.Style.
        /// </summary>
        internal Style CharacterStyle
        {
            get { return InlineHelper.GetCharacterStyle(this); }
        }

        /// <summary>
        /// If you set, do so with care.
        /// </summary>
        internal RunPr RunPr
        {
            get { return mRunPr; }
            set
            {
                mRunPr = value;
            }
        }

        RunPr IInline.RunPr_IInline
        {
            get { return mRunPr; }
            set { mRunPr = value; }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Inline lhs = (Inline)base.Clone(isCloneChildren, cloningListener);
            RunPr runPr = mRunPr.Clone();
            lhs.mRunPr = runPr;
            lhs.mFont = null;   //Don't clone this because created on demand.
            return lhs;
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

        internal void ExpandRunPr(RunPr dstRunPr, RunPrExpandFlags flags)
        {
            InlineHelper.ExpandRunPr(this, dstRunPr, flags);
        }

        /// <summary>
        /// Indicates that inline should be written as Symbol.
        /// </summary>
        internal bool IsWriteAsSymbol
        {
            get
            {
                string text = GetText();

                Theme theme = Document.NodeType == NodeType.Document
                    ? ((Document)Document).GetThemeInternal()
                    : null;

                string fontNameOther = RunPr.ComplexNameOther.IsThemeFont
                    ? RunPr.ComplexNameOther.Resolve(theme)
                    : RunPr.NameOther;

                // WORDSNET-22241 Saving the Checkbox SDT with the symbolic font.
                if (ParentNode != null)
                {
                    // WORDSNET-27559 Do not write character as symbol inside PlainText SDT.
                    StructuredDocumentTag parentTag = ParentNode as StructuredDocumentTag;
                    if ((parentTag != null) && (parentTag.SdtType == SdtType.PlainText))
                        return false;

                    StructuredDocumentTag ancestorTag = ParentNode.ParentNode as StructuredDocumentTag;
                    if ((ancestorTag != null) && (ancestorTag.SdtType == SdtType.Checkbox) && FontUtil.IsSymbolic(fontNameOther))
                        return true;
                }

                // WORDSNET-17859 This character excluded from common SYM processing and should be written as symbol.
                // WORDSNET-19401 The font shouldn't be the theme font.
                if ((text.Length > 0) && (text[0] == 0xA0) && FontUtil.IsSymbolic(fontNameOther))
                    return true;

                // WORDSNET-23237 Write SYM if font is symbolic only.
                if (!Run.IsSymbolicCharacter(text) || !FontUtil.IsSymbolic(fontNameOther))
                    return false;

                if (RunPr.Contains(FontAttr.CharacterCategoryHint))
                    return false;

                if (!RunPr.Contains(FontAttr.NameOther))
                    return false;

                if (!object.Equals(RunPr.ComplexNameAscii, RunPr.ComplexNameOther))
                    return false;

                return true;
            }
        }

        #region IRunAttrSource

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return mRunPr.GetDirectAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mRunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            return InlineHelper.FetchInheritedAttr(this, fontAttr);
        }

        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            // WORDSNET-26171 Translate decoration attribute into MathStyle.
            if ((key == FontAttr.Italic) && (GetAncestor(NodeType.OfficeMath) != null))
            {
                // AM. Actually we need more complex logic here, we should consider currently applied decoration,
                // but seems this feature is rarely used so lets start with very minimum solution.
                mRunPr.SetAttr(FontAttr.MathStyle, ((AttrBoolEx)value).ToBool() ? MathStyle.Italic : MathStyle.Plain);
            }
            else
                mRunPr.SetAttr(key, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mRunPr.Clear();
        }

        #endregion

        private RunPr mRunPr;
        private Font mFont;

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
    }
}
