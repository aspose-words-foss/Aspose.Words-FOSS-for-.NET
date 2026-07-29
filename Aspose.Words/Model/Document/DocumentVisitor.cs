// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2004 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for custom document visitors.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>With <see cref="DocumentVisitor"/> you can define and execute custom operations
    /// that require enumeration over the document tree.</p>
    /// 
    /// <p>For example, Aspose.Words uses <see cref="DocumentVisitor"/> internally for saving <see cref="Document"/>
    /// in various formats and for other operations like finding fields or bookmarks over
    /// a fragment of a document.</p>
    /// 
    /// <p>To use <see cref="DocumentVisitor"/>:</p>
    /// <list type="number">
    /// <item>Create a class derived from <see cref="DocumentVisitor"/>.</item>
    /// <item>Override and provide implementations for some or all of the VisitXXX methods
    /// to perform some custom operations.</item>
    /// <item>Call <see cref="Node.Accept">Node.Accept</see> on the <see cref="Node"/> that
    /// you want to start the enumeration from.</item>
    /// </list>
    /// 
    /// <p><see cref="DocumentVisitor"/> provides default implementations for all of the VisitXXX methods 
    /// to make it easier to create new document visitors as only the methods required for the particular
    /// visitor need to be overridden. It is not necessary to override all of the visitor methods.</p>
    /// 
    /// <p>For more information see the Visitor design pattern.</p>
    /// 
    /// </remarks>
    public abstract class DocumentVisitor
    {
        /// <summary>
        /// Only derived classes are allowed to instantiate a document visitor.
        /// </summary>
        protected DocumentVisitor()
        {
        }

        /// <summary>
        /// Called when enumeration of the document has started.
        /// </summary>
        /// <param name="doc">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitDocumentStart(Document doc)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of the document has finished.
        /// </summary>
        /// <param name="doc">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitDocumentEnd(Document doc)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a section has started.
        /// </summary>
        /// <param name="section">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSectionStart(Section section)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a section has ended.
        /// </summary>
        /// <param name="section">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSectionEnd(Section section)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of the main text story in a section has started.
        /// </summary>
        /// <param name="body">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBodyStart(Body body)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of the main text story in a section has ended.
        /// </summary>
        /// <param name="body">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBodyEnd(Body body)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a header or footer in a section has started.
        /// </summary>
        /// <param name="headerFooter">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a header or footer in a section has ended.
        /// </summary>
        /// <param name="headerFooter">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitHeaderFooterEnd(HeaderFooter headerFooter)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a paragraph has started.
        /// </summary>
        /// <param name="paragraph">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a paragraph has ended.
        /// </summary>
        /// <param name="paragraph">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table has started.
        /// </summary>
        /// <param name="table">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitTableStart(Table table)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table has ended.
        /// </summary>
        /// <param name="table">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitTableEnd(Table table)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table row has started.
        /// </summary>
        /// <param name="row">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitRowStart(Row row)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table row has ended.
        /// </summary>
        /// <param name="row">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitRowEnd(Row row)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table cell has started.
        /// </summary>
        /// <param name="cell">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCellStart(Cell cell)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table cell has ended.
        /// </summary>
        /// <param name="cell">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCellEnd(Cell cell)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a run of text in the is encountered.
        /// </summary>
        /// <param name="run">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitRun(Run run)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field starts in the document.
        /// </summary>
        /// <remarks>
        /// <p>A field in a Word document consists of a field code and field value.</p>
        /// <p>For example, a field that displays a page number can be represented as follows:</p>
        /// <p>[FieldStart]PAGE[FieldSeparator]98[FieldEnd]</p>
        /// <p>The field separator separates field code from field value in the document. Note that some 
        /// fields have only field code and do not have field separator and field value.</p>
        /// <p>Fields can be nested.</p>
        /// </remarks>
        /// <param name="fieldStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field separator is encountered in the document.
        /// </summary>
        /// <remarks>
        /// <p>The field separator separates field code from field value in the document. Note that some 
        /// fields have only field code and do not have field separator and field value.</p>
        /// <p>For more info see <see cref="VisitFieldStart"/></p>
        /// </remarks>
        /// <param name="fieldSeparator">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field ends in the document.
        /// </summary>
        /// <remarks>
        /// <p>For more info see <see cref="VisitFieldStart"/></p>
        /// </remarks>
        /// <param name="fieldEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            return VisitorAction.Continue;
        }


        /// <summary>
        /// Informs the acceptor node if deleted nodes should be given to the visitor.
        /// </summary>
        /// <remarks>
        /// This property can be used in the Accept() method of a specific node type. 
        /// Currently, only deleted FieldChar nodes are skipped using this property for some visitors.
        /// The default implementation returns true, to retain the default behavior before introduction of this property.
        /// </remarks>
        internal virtual bool VisitsDeletedNodes
        {
            get { return true; }
        }

        /// <summary>
        /// Called when a form field is encountered in the document.
        /// </summary>
        /// <param name="formField">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFormField(FormField formField)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a start of a bookmark is encountered in the document.
        /// </summary>
        /// <param name="bookmarkStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when an end of a bookmark is encountered in the document.
        /// </summary>
        /// <param name="bookmarkEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a footnote or endnote text has started.
        /// </summary>
        /// <param name="footnote">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a footnote or endnote text has ended.
        /// </summary>
        /// <param name="footnote">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a comment text has started.
        /// </summary>
        /// <param name="comment">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCommentStart(Comment comment)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a comment text has ended.
        /// </summary>
        /// <param name="comment">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCommentEnd(Comment comment)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a start of an MoveFrom range is encountered in the document.
        /// </summary>
        /// <param name="moveFromRangeStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        internal virtual VisitorAction VisitMoveFromRangeStart(MoveRangeStart moveFromRangeStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when an end of an MoveFrom range is encountered in the document.
        /// </summary>
        /// <param name="moveFromRangeEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        internal virtual VisitorAction VisitMoveFromRangeEnd(MoveRangeEnd moveFromRangeEnd)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a start of an MoveTo range is encountered in the document.
        /// </summary>
        /// <param name="moveToRangeStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        internal virtual VisitorAction VisitMoveToRangeStart(MoveRangeStart moveToRangeStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when an end of an MoveTo range is encountered in the document.
        /// </summary>
        /// <param name="moveToRangeEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        internal virtual VisitorAction VisitMoveToRangeEnd(MoveRangeEnd moveToRangeEnd)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a start of an editable range is encountered in the document.
        /// </summary>
        /// <param name="editableRangeStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when an end of an editable range is encountered in the document.
        /// </summary>
        /// <param name="editableRangeEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a shape has started.
        /// </summary>
        /// <param name="shape">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitShapeStart(Shape shape)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a shape has ended.
        /// </summary>
        /// <param name="shape">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitShapeEnd(Shape shape)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a group shape has started.
        /// </summary>
        /// <param name="groupShape">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitGroupShapeStart(GroupShape groupShape)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a group shape has ended.
        /// </summary>
        /// <param name="groupShape">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitGroupShapeEnd(GroupShape groupShape)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a Office Math object has started.
        /// </summary>
        /// <param name="officeMath">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a Office Math object has ended.
        /// </summary>
        /// <param name="officeMath">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitOfficeMathEnd(OfficeMath officeMath)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a <see cref="Aspose.Words.SpecialChar"/> node is encountered in the document.
        /// </summary>
        /// <param name="specialChar">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        /// <remarks>
        /// This method is not be called for generic control characters (see <see cref="ControlChar"/>) that can be present in the document.
        /// </remarks>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSpecialChar(SpecialChar specialChar)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a <see cref="AbsolutePositionTab"/> node is encountered in the document.
        /// </summary>
        /// <param name="tab">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitAbsolutePositionTab(AbsolutePositionTab tab)
        {
            // by default we will treat AbsolutePositionTab as simple special char that represents regular \tab
            return VisitSpecialChar(tab);
        }

        /// <summary>
        /// Called when enumeration of a smart tag has started.
        /// </summary>
        /// <param name="smartTag">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSmartTagStart(SmartTag smartTag)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a smart tag has ended.
        /// </summary>
        /// <param name="smartTag">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSmartTagEnd(SmartTag smartTag)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a structured document tag has started.
        /// </summary>
        /// <param name="sdt">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a structured document tag has ended.
        /// </summary>
        /// <param name="sdt">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitStructuredDocumentTagEnd(StructuredDocumentTag sdt)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a glossary document has started.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="GlossaryDocument.NotVisited"]/*'/>
        /// <param name="glossary">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitGlossaryDocumentStart(GlossaryDocument glossary)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a glossary document has ended.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="GlossaryDocument.NotVisited"]/*'/>
        /// <param name="glossary">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitGlossaryDocumentEnd(GlossaryDocument glossary)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a building block has started.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="BuildingBlock.NotVisited"]/*'/>
        /// <param name="block">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBuildingBlockStart(BuildingBlock block)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a building block has ended.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="BuildingBlock.NotVisited"]/*'/>
        /// <param name="block">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitBuildingBlockEnd(BuildingBlock block)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when the start of a commented range of text is encountered.
        /// </summary>
        /// <param name="commentRangeStart">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when the end of a commented range of text is encountered.
        /// </summary>
        /// <param name="commentRangeEnd">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a sub-document is encountered.
        /// </summary>
        /// <param name="subDocument">The object that is being visited.</param>
        /// <returns>A <see cref="Aspose.Words.VisitorAction"/> value that specifies how to continue the enumeration.</returns>
        [JavaThrows(true)]
        public virtual VisitorAction VisitSubDocument(SubDocument subDocument)
        {
            return VisitorAction.Continue;
        }

        internal void Lock()
        {
            mLockCount++;
        }

        /// <summary>
        /// Called when a StructuredDocumentTagRangeStart is encountered.
        /// </summary>
        [JavaThrows(true)]
        public virtual VisitorAction VisitStructuredDocumentTagRangeStart(StructuredDocumentTagRangeStart sdtRangeStart)
        {
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a StructuredDocumentTagRangeEnd is encountered.
        /// </summary>
        [JavaThrows(true)]
        public virtual VisitorAction VisitStructuredDocumentTagRangeEnd(StructuredDocumentTagRangeEnd sdtRangeEnd)
        {
            return VisitorAction.Continue;
        }

        internal void Unlock()
        {
            Debug.Assert(mLockCount > 0, "Attempt to Unlock() at lock count 0 or less!");

            // Do not over-Unlock().
            if (mLockCount > 0)
            {
                // Over-unlocking here would cause subsequent Lock() failure.
                mLockCount--;
            }
        }

        internal bool IsLocked
        {
            get { return (mLockCount > 0); }
        }

        /// <summary>
        /// Greater than zero indicates the document content is being skipped.
        /// </summary>
        private int mLockCount;
    }
}
