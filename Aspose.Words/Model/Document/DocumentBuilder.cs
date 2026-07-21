// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2004 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Lists;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Factories;
using Aspose.Words.RW.Ole;
using Aspose.Words.Tables;
using Aspose.Words.Notes;
using CodePorting.Translator.Cs2Cpp;
using Aspose.Words.Settings;
using Aspose.Words.Validation;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#else
using Image = System.Drawing.Image;
#endif

namespace Aspose.Words
{
    /// <summary>
    /// Provides methods to insert text, images and other content, specify font, paragraph and section formatting.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/document-builder-overview/">Document Builder Overview</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="DocumentBuilder"/> makes the process of building a <see cref="Aspose.Words.Document"/> easier.
    /// <see cref="Aspose.Words.Document"/> is a composite object consisting of a tree of nodes and while inserting content
    /// nodes directly into the tree is possible, it requires good understanding of the tree structure.
    /// <see cref="DocumentBuilder"/> is a "facade" for the complex structure of <see cref="Aspose.Words.Document"/> and allows
    /// to insert content and formatting quickly and easily.</p>
    ///
    /// <p>Create a <see cref="DocumentBuilder"/> and associate it with a <see cref="Aspose.Words.Document"/>.</p>
    ///
    /// <p>The <see cref="DocumentBuilder"/> has an internal cursor where the text will be inserted
    /// when you call <see cref="Write"/>, <see cref="Writeln(string)"/>, <see cref="InsertBreak"/>
    /// and other methods. You can navigate the <see cref="DocumentBuilder"/> cursor to a different location
    /// in a document using various MoveToXXX methods.</p>
    ///
    /// <p>Use the <see cref="Font"/> property to specify character formatting that will apply to
    /// all text inserted from the current position in the document onwards.</p>
    ///
    /// <p>Use the <see cref="ParagraphFormat"/> property to specify paragraph formatting for the current
    /// and all paragraphs that will be inserted.</p>
    ///
    /// <p>Use the <see cref="PageSetup"/> property to specify page and section properties for the current
    /// section and all section that will be inserted.</p>
    ///
    /// <p>Use the <see cref="CellFormat"/> and <see cref="RowFormat"/> properties to specify
    /// formatting properties for table cells and rows. User the <see cref="InsertCell"/> and
    /// <see cref="EndRow"/> methods to build a table.</p>
    ///
    /// <p>Note that <see cref="Font"/>, <see cref="ParagraphFormat"/> and <see cref="PageSetup"/> properties are updated whenever
    /// you navigate to a different place in the document to reflect formatting properties available at the new location.</p>
    ///
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppForceForwardDeclaration("Aspose.Words.Fields.FieldBundle")]
    public class DocumentBuilder : IRunAttrSource, IParaAttrSource, IRowAttrSource, ICellAttrSource
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="DocumentBuilder"/> object and attaches it to a new <see cref="Aspose.Words.Document"/> object.
        /// </remarks>
        public DocumentBuilder()
        {
            Document = new Document();
            mDocumentBuilderOptions = new DocumentBuilderOptions();
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="DocumentBuilder"/> object and attaches it to a new <see cref="Aspose.Words.Document"/> object.
        /// Additional document building options can be specified.
        /// </remarks>
        public DocumentBuilder(DocumentBuilderOptions options)
        {
            Document = new Document();
            mDocumentBuilderOptions = options;
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="DocumentBuilder"/> object, attaches to the specified <see cref="Aspose.Words.Document"/> object.
        /// The cursor is positioned at the beginning of the document.
        /// </remarks>
        /// <param name="doc">The <see cref="Aspose.Words.Document"/> object to attach to.</param>
        public DocumentBuilder(Document doc)
        {
            Document = doc;
            mDocumentBuilderOptions = new DocumentBuilderOptions();
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <remarks>
        /// Creates a new <see cref="DocumentBuilder"/> object, attaches to the specified <see cref="Aspose.Words.Document"/> object.
        /// The cursor is positioned at the beginning of the document.
        /// </remarks>
        /// <param name="doc">The <see cref="Aspose.Words.Document"/> object to attach to.</param>
        /// <param name="options">Additional options for the document building process.</param>
        public DocumentBuilder(Document doc, DocumentBuilderOptions options)
        {
            Document = doc;
            mDocumentBuilderOptions = options;
        }

        /// <summary>
        /// Moves the cursor to the beginning of the document.
        /// </summary>
        public void MoveToDocumentStart()
        {
            MoveTo(0, StoryType.MainText, 0, 0);
        }

        /// <summary>
        /// Moves the cursor to the end of the document.
        /// </summary>
        public void MoveToDocumentEnd()
        {
            MoveTo(-1, StoryType.MainText, -1, -1);

            // WORDSNET-4487 We should not move inside footnotes.
            Footnote curFootnote = (Footnote)CurrentParagraph.GetAncestor(NodeType.Footnote);
            if (curFootnote != null)
                MoveTo(curFootnote.ParentParagraph);
        }

        /// <summary>
        /// Moves the cursor to the beginning of the body in a specified section.
        /// </summary>
        /// <remarks>
        /// <p>When <paramref name="sectionIndex"/> is greater than or equal to 0, it specifies an index from
        /// the beginning of the document with 0 being the first section. When <paramref name="sectionIndex"/> is less than 0,
        /// it specified an index from the end of the document with -1 being the last section.</p>
        /// <p>The cursor is moved to the first paragraph in the <see cref="Body"/> of the specified section.</p>
        /// </remarks>
        /// <param name="sectionIndex">The index of the section to move to.</param>
        public void MoveToSection(int sectionIndex)
        {
            MoveTo(sectionIndex, StoryType.MainText, 0, 0);
        }

        /// <summary>
        /// Moves the cursor to the beginning of a header or footer in the current section.
        /// </summary>
        /// <remarks>
        /// <p>After you moved the cursor into a header or footer, you can use the rest of <see cref="DocumentBuilder"/>
        /// methods to modify the contents of the header or footer.</p>
        /// <p>If you want to create headers and footers different for the first page, you need
        /// to set <see cref="Aspose.Words.PageSetup.DifferentFirstPageHeaderFooter"/>.</p>
        /// <p>If you want to create headers and footers different for even and odd pages, you need
        /// to set <see cref="Aspose.Words.PageSetup.OddAndEvenPagesHeaderFooter"/>.</p>
        /// <p>Use <see cref="MoveToSection"/> to move out of the header into the main text.</p>
        /// </remarks>
        /// <param name="headerFooterType">Specifies the header or footer to move to.</param>
        public void MoveToHeaderFooter(HeaderFooterType headerFooterType)
        {
            MoveTo(CurrentSection, WordUtil.HeaderFooterTypeToStoryType(headerFooterType), 0, 0);
        }

        private void MoveTo(int sectionIdx, StoryType storyType, int paraIdx, int charIdx)
        {
            mDoc.EnsureMinimum();

            //Select the section. Throw if not there.
            Section section = (Section)mDoc.GetChild(NodeType.Section, sectionIdx, false);
            if (section == null)
                throw new ArgumentOutOfRangeException("sectionIdx");

            MoveTo(section, storyType, paraIdx, charIdx);
        }

        private void MoveTo(Section section, StoryType storyType, int paraIdx, int charIdx)
        {
            section.EnsureMinimum();

            //Select the story, create header or footer on demand.
            Story story;
            if (storyType == StoryType.MainText)
            {
                story = section.Body;
            }
            else
            {
                HeaderFooterType headerFooterType = WordUtil.StoryTypeToHeaderFooterType(storyType);
                story = section.HeadersFooters[headerFooterType];
                if (story == null)
                    story = section.AppendChild(new HeaderFooter(mDoc, headerFooterType));

                if (story.FirstParagraph == null)
                    story.AppendChild(new Paragraph(mDoc));
            }

            MoveTo(story, paraIdx, charIdx);
        }

        private void MoveTo(Story story, int paraIdx, int charIdx)
        {
            // Select the paragraph, throw if not there.
            // RK This has to use deep navigation because it was originally published this way
            // and also because it allows to move to paragraphs inside tables. Although it is
            // not always very easy for the user to find the paragraph index property because
            // paragraphs in shapes and textboxes also participate in this.
            Paragraph para = (Paragraph)story.GetChild(NodeType.Paragraph, paraIdx, true);
            if (para == null)
                throw new ArgumentOutOfRangeException("paraIdx");

            MoveTo(para, charIdx);
        }

        internal void MoveTo(Paragraph para, int charIdx)
        {
            MoveToCharIndex(para, charIdx);
        }

        /// <overloads>Moves the cursor to the specified merge field.</overloads>
        /// <summary>
        /// Moves the cursor to a position just beyond the specified merge field and removes the merge field.
        /// </summary>
        /// <remarks>
        /// <p>Note that this method deletes the merge field from the document after moving the cursor.</p>
        /// </remarks>
        /// <param name="fieldName">The case-insensitive name of the mail merge field.</param>
        /// <returns><c>true</c> if the merge field was found and the cursor was moved; <c>false</c> otherwise.</returns>
        public bool MoveToMergeField(string fieldName)
        {
            return MoveToMergeField(fieldName, true, true);
        }

        /// <summary>
        /// Moves the merge field to the specified merge field.
        /// </summary>
        /// <param name="fieldName">The case-insensitive name of the mail merge field.</param>
        /// <param name="isAfter">When <c>true</c>, moves the cursor to be after the field end.
        /// When <c>false</c>, moves the cursor to be before the field start. </param>
        /// <param name="isDeleteField">When <c>true</c>, deletes the merge field.</param>
        /// <returns><c>true</c> if the merge field was found and the cursor was moved; <c>false</c> otherwise.</returns>
        public bool MoveToMergeField(string fieldName, bool isAfter, bool isDeleteField)
        {
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            //This is reasonably slow.
            // WORDSNET-10934 Cursor may reference to removed node.
            FieldMergeField mergeField = Cursor.IsRemoved
                ? MergeFieldFinder.FindMergeField(mDoc, fieldName)
                : MergeFieldFinder.FindMergeFieldFromNode(Cursor, fieldName);

            if (mergeField == null)
                return false;

            return MoveToField(mergeField, isAfter, isDeleteField);
        }

        /// <summary>
        /// Moves the cursor to a field in the document.
        /// </summary>
        /// <param name="field">The field to move the cursor to.</param>
        /// <param name="isAfter">When <c>true</c>, moves the cursor to be after the field end.
        /// When <c>false</c>, moves the cursor to be before the field start.</param>
        public void MoveToField(Field field, bool isAfter)
        {
            MoveToField(field, isAfter, false);
        }

        private bool MoveToField(Field field, bool isAfter, bool isDeleteField)
        {
            // WORDSNET-1489 Merge fields lose formatting.
            // Although the above call to MoveTo has already set font formatting, we are better off
            // by setting it to be like at the beginning of the field result.
            IFieldResultFormatProvider formatProvider = field.Format.GetFieldResultFormatProvider();
            Inline formatSource = formatProvider.GetSourceNode();

            Node newCursor;
            if (isDeleteField)
            {
                newCursor = field.Remove();
                if (newCursor == null)
                    return false;
            }
            else if (isAfter)
            {
                // Normally we would move to the next node after the field end, but if the field node is at
                // the end of the paragraph, have to move to the end of the paragraph (the paragraph node itself).
                newCursor = field.End.NextSibling;
                if (newCursor == null)
                    newCursor = field.End.ParentParagraph;
            }
            else
            {
                newCursor = field.Start;
            }

            MoveTo(newCursor);
            if (formatSource != null)
                SetFont(formatSource.RunPr, true);

            return true;
        }

        /// <summary>
        /// Moves the cursor to a bookmark.
        /// </summary>
        /// <remarks>
        /// <p>Moves the cursor to a position just after the start of the bookmark with the
        /// specified name.</p>
        /// <p>The comparison is not case-sensitive. If the bookmark was not found, <c>false</c> is
        /// returned and the cursor is not moved.</p>
        /// <p>Inserting new text does not replace existing text of the bookmark.</p>
        /// <p>Note that some bookmarks in the document are assigned to form fields.
        /// Moving to such a bookmark and inserting text there inserts the text into the
        /// form field code. Although this will not invalidate the form field, the inserted
        /// text will not be visible because it becomes part of the field code.</p>
        /// </remarks>
        /// <param name="bookmarkName">The name of the bookmark to move the cursor to.</param>
        /// <returns><c>true</c> if the bookmark was found; <c>false</c> otherwise.</returns>
        public bool MoveToBookmark(string bookmarkName)
        {
            return MoveToBookmark(bookmarkName, true, true);
        }

        /// <summary>
        /// Moves the cursor to a bookmark with greater precision.
        /// </summary>
        /// <remarks>
        /// <p>Moves the cursor to a position before or after the bookmark start or end.</p>
        /// <p>If desired position is not at inline level, moves to the next paragraph.</p>
        /// <p>The comparison is not case-sensitive. If the bookmark was not found, <c>false</c> is
        /// returned and the cursor is not moved.</p>
        /// </remarks>
        /// <param name="bookmarkName">The name of the bookmark to move the cursor to.</param>
        /// <param name="isStart">When <c>true</c>, moves the cursor to the beginning of the bookmark.
        /// When <c>false</c>, moves the cursor to the end of the bookmark.</param>
        /// <param name="isAfter">When <c>true</c>, moves the cursor to be after the bookmark
        /// start or end position. When <c>false</c>, moves the cursor to be before the bookmark
        /// start or end position.</param>
        /// <returns><c>true</c> if the bookmark was found; <c>false</c> otherwise.</returns>
        public bool MoveToBookmark(string bookmarkName, bool isStart, bool isAfter)
        {
            if (bookmarkName == null)
                throw new ArgumentNullException("bookmarkName");

            Node bookmarkNode;
            if (isStart)
                bookmarkNode = BookmarkFinder.FindBookmarkStart(mDoc, bookmarkName);
            else
                bookmarkNode = BookmarkFinder.FindBookmarkEnd(mDoc, bookmarkName);

            if (bookmarkNode == null)
                return false;

            BookmarkStart bookmarkStart = isStart
                ? (BookmarkStart)bookmarkNode
                : BookmarkFinder.FindBookmarkStart(Document, bookmarkName, (BookmarkEnd)bookmarkNode);

            if ((bookmarkStart != null) && bookmarkStart.IsColumn)
            {
                // Position of a column bookmark is logical and does not refer to any node, so there is no difference
                // between 'before' and 'after': the 'isAfter' parameter is ignored for column bookmarks.

                BookmarkEnd bookmarkEnd = isStart
                    ? BookmarkFinder.FindBookmarkEnd(Document, bookmarkName, bookmarkStart)
                    : (BookmarkEnd)bookmarkNode;

                if (MoveToColumnBookmark(bookmarkStart, bookmarkEnd, isStart))
                    return true;
            }

            if (bookmarkNode.NodeLevel == NodeLevel.Inline)
            {
                MoveTo(bookmarkNode.FirstNonMarkupParentNode,
                    isAfter ? bookmarkNode.NextSibling : bookmarkNode);
            }
            else
            {
                Paragraph paragraph = NodeUtil.FindNextParagraph(bookmarkNode);
                if (paragraph == null)
                    return false;
                MoveTo(paragraph, paragraph.FirstChild);
            }

            return true;
        }

        /// <summary>
        /// Moves the cursor to a column bookmark.
        /// </summary>
        /// <remarks>
        /// If <paramref name="toStart"/> is <b>true</b>, the cursor is moved to the beginning of the first cell of the
        /// bookmark area after any cross-structure annotation nodes. When it is <b>false</b>, the cursor is moved to
        /// the end of the last cell of the bookmark area.
        /// Cross-structure annotation nodes are skipped at the beginning of a cell so that written text is placed after
        /// them, since such annotation nodes should retain their position at the beginning of a cell/paragraph before
        /// text to behave correctly.
        /// </remarks>
        private bool MoveToColumnBookmark(BookmarkStart start, BookmarkEnd end, bool toStart)
        {
            Debug.Assert(start.IsColumn);

            Cell cell = toStart
                ? BookmarkUtil.GetColumnBookmarkFirstCell(start, end)
                : BookmarkUtil.GetColumnBookmarkLastCell(start, end);

            if (cell == null)
                return false;

            // If the cell contains a sub-table, move to its paragraphs, e.g. like MS Word places bookmark nodes in the
            // similar cases.
            Paragraph para = (Paragraph)cell.GetChild(NodeType.Paragraph, (toStart ? 0 : -1), true);
            if (para == null)
                return false;

            // Skip annotations at the beginning of the paragraph: see the comment in the method remarks.
            MoveTo(para, toStart ? para.FirstNonAnnotationChild : null);

            return true;
        }

        /// <summary>
        /// Moves the cursor to a paragraph in the current section.
        /// </summary>
        /// <remarks>
        /// <p>The navigation is performed inside the current story of the current section.
        /// That is, if you moved the cursor to the primary header of the first section,
        /// then <paramref name="paragraphIndex"/> specified the index of the paragraph inside that header
        /// of that section.</p>
        /// <p>When <paramref name="paragraphIndex"/> is greater than or equal to 0, it specifies an index from
        /// the beginning of the section with 0 being the first paragraph. When <paramref name="paragraphIndex"/> is less than 0,
        /// it specified an index from the end of the section with -1 being the last paragraph.</p>
        /// </remarks>
        /// <param name="paragraphIndex">The index of the paragraph to move to.</param>
        /// <param name="characterIndex">The index of the character inside the paragraph.
        /// A negative value allows you to specify a position from the end of the paragraph. Use -1 to move to the end of
        /// the paragraph.</param>
        public void MoveToParagraph(int paragraphIndex, int characterIndex)
        {
            MoveTo(CurrentStory, paragraphIndex, characterIndex);
        }

        /// <summary>
        /// Moves the cursor to a structured document tag in the current section.
        /// </summary>
        /// <remarks>
        /// <p>The navigation is performed inside the current story of the current section. That is, if you moved the
        /// cursor to the primary header of the first section, then <paramref name="structuredDocumentTagIndex"/>
        /// specified the index of the structured document tag inside that header of that section.</p>
        /// <p>When <paramref name="structuredDocumentTagIndex"/> is greater than or equal to 0, it specifies an index
        /// from the beginning of the section with 0 being the first structured document tag. When
        /// <paramref name="structuredDocumentTagIndex"/> is less than 0, it specified an index from the end of the
        /// section with -1 being the last structured document tag.</p>
        /// </remarks>
        /// <param name="structuredDocumentTagIndex">The index of the structured document tag to move to.</param>
        /// <param name="characterIndex">The index of the character inside the structured document tag.
        /// A negative value allows you to specify a position from the end of the structured document tag. Use -1 to
        /// move to the end of the structured document tag. If the structured document tag is at the block level, and
        /// you want to move the cursor to the end of its last paragraph, specify -2.</param>
        public void MoveToStructuredDocumentTag(int structuredDocumentTagIndex, int characterIndex)
        {
            StructuredDocumentTag sdt = (StructuredDocumentTag)CurrentStory.GetChild(NodeType.StructuredDocumentTag,
                structuredDocumentTagIndex, true);
            if (sdt == null)
                throw new ArgumentOutOfRangeException("structuredDocumentTagIndex");

            MoveToStructuredDocumentTag(sdt, characterIndex);
        }

        /// <summary>
        /// Moves the cursor to the structured document tag.
        /// </summary>
        /// <param name="structuredDocumentTag">The structured document tag to move to.</param>
        /// <param name="characterIndex">The index of the character inside the structured document tag.
        /// A negative value allows you to specify a position from the end of the structured document tag. Use -1 to
        /// move to the end of the structured document tag. If the structured document tag is at the block level, and
        /// you want to move the cursor to the end of its last paragraph, specify -2.</param>
        public void MoveToStructuredDocumentTag(StructuredDocumentTag structuredDocumentTag, int characterIndex)
        {
            if (structuredDocumentTag == null)
                throw new ArgumentNullException("structuredDocumentTag");

            if (characterIndex >= 0)
            {
                MoveToCharIndexForward(structuredDocumentTag, characterIndex);
            }
            else
            {
                // characterIndex == -1 means 0 characters from the end of the SDT: increase such negative
                // characterIndex by one.
                characterIndex++;

                if (characterIndex == 0)
                    MoveToStructuredDocumentTagEnd(structuredDocumentTag);
                else
                    MoveToCharIndexBackward(structuredDocumentTag, characterIndex);
            }
        }

        /// <summary>
        /// Moves the cursor to a table cell in the current section.
        /// </summary>
        /// <remarks>
        /// <p>The navigation is performed inside the current story of the current section.</p>
        /// <p>For the index parameters, when index is greater than or equal to 0, it specifies an index from
        /// the beginning with 0 being the first element. When index is less than 0, it specified an index from
        /// the end with -1 being the last element.</p>
        /// </remarks>
        /// <param name="tableIndex">The index of the table to move to.</param>
        /// <param name="rowIndex">The index of the row in the table.</param>
        /// <param name="columnIndex">The index of the column in the table.</param>
        /// <param name="characterIndex">The index of the character inside the cell.
        /// A negative value allows you to specify a position from the end of the cell. Use -1 to move to the end of
        /// the cell.</param>
        public void MoveToCell(int tableIndex, int rowIndex, int columnIndex, int characterIndex)
        {
            Row row = FetchRow(tableIndex, rowIndex);

            Cell cell = (Cell)row.GetChild(NodeType.Cell, columnIndex, false);
            if (cell == null)
                throw new ArgumentOutOfRangeException("columnIndex");

            cell.EnsureMinimum();

            MoveToCharIndex(cell, characterIndex);
        }

        /// <summary>
        /// Moves the cursor to an inline node or to the end of a paragraph.
        /// </summary>
        /// <remarks>
        /// <p>When <i>node</i> is an inline-level node, the cursor is moved to this node
        /// and further content will be inserted before that node.</p>
        /// <p>When <i>node</i> is a <see cref="Paragraph"/>, the cursor is moved to the end of the paragraph
        /// and further content will be inserted just before the paragraph break.</p>
        /// <p>When <i>node</i> is a block-level node but not a <see cref="Paragraph"/>, the cursor is moved to the end of the first paragraph into block-level node
        /// and further content will be inserted just before the paragraph break.</p>
        /// </remarks>
        /// <param name="node">The node must be a paragraph or a direct child of a paragraph.</param>
        public void MoveTo(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.Document != Document)
                throw new ArgumentException("The node belongs to a different document.");

            if (node.ParentNode == null)
                throw new ArgumentException("Parent cannot be null.");

            if ((node.NodeType == NodeType.Paragraph) || (node.NodeLevel == NodeLevel.Inline))
            {
                Cursor = node;
            }
            else if (NodeUtil.IsCrossStructureAnnotation(node))
            {
                // On moving cursor to an annotation on the block level, move it to be before first child of a next
                // paragraph. It is acceptable since position of a bookmark before a paragraph logically equals to
                // a position of a bookmark as the first child of the paragraph. Also, such behaviour allows achieving
                // the same result of moving to a bookmark as in the mode with disabled block level bookmarks as with
                // the enabled ones.
                Paragraph paragraph = NodeUtil.FindNextParagraph(node);
                if (paragraph == null)
                    throw new InvalidOperationException("There is no paragraph next to the specified node.");

                // Cursor is before first child of the paragraph.
                Cursor = paragraph.HasChildNodes ? paragraph.FirstChild : paragraph;
            }
            else if (node.IsComposite && ((node.NodeLevel == NodeLevel.Block) || (node.NodeLevel == NodeLevel.Cell)))
            {
                Node paragraph = ((CompositeNode)node).GetChild(NodeType.Paragraph, 0, true);
                if (paragraph == null)
                    throw new InvalidOperationException("The block level node doesn't contain paragraph.");

                Cursor = paragraph;
            }
            else
            {
                throw new InvalidOperationException("The node must be a block or an inline.");
            }

            mCharOffset = 0;

            if (IsAtEndOfParagraph)
            {
                AttachFontToParagraph();
            }
            else
            {
                if (!AttachFontToInline())
                    AttachFontToParagraph();
            }
        }

        /// <summary>
        /// If inline node is not null, moves to it, otherwise moves to the paragraph node.
        /// </summary>
        private void MoveTo(CompositeNode compositeNode, Node inline)
        {
            if (inline != null)
                MoveTo(inline);
            else
                MoveTo(compositeNode);
        }

        private bool AttachFontToInline()
        {
            // Try to find a node that has character formatting properties.
            // For example, if the cursor was moved to a bookmark, then the node given to us is not a run
            // and we need to loop to next siblings until we find a run that can be a source for runPr.
            Node curNode = Cursor;

            // WORDSNET-15423 Skip shapes nodes while determining properties of the run.
            if (!(curNode is Inline))
            {
                // Try to get previous Inline sibling first.
                while ((curNode != null) && !(curNode is Inline))
                    curNode = curNode.PreviousSibling;

                // WORDSNET-23920 Get formatting from a parent SDT like MS Word does.
                if ((curNode == null) && (Cursor.ParentNode.NodeType == NodeType.StructuredDocumentTag))
                {
                    SetFont(((StructuredDocumentTag)Cursor.ParentNode).ContentsRunPr, true);
                    return true;
                }
            }

            if (curNode == null)
            {
                curNode = Cursor;
                while ((curNode != null) && !(curNode is Inline))
                    curNode = curNode.NextSibling;
            }

            // If the source for the character properties was found, use it to select new font formatting.
            if (curNode != null)
            {
                Inline inlineNode = (Inline)curNode;
                // Require cloning of attrs because we don't want font of this run to be modified.
                SetFont(inlineNode.RunPr, true);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AttachFontToParagraph()
        {
            SetFont(CurrentParagraph.ParagraphBreakRunPr, true);
        }

        /// <summary>
        /// Deletes a row from a table.
        /// </summary>
        /// <remarks>
        /// <p>If the cursor is inside the row that is being deleted, the cursor is moved
        /// out to the next row or to the next paragraph after the table.</p>
        /// <p>If you delete a row from a table that contains only one row, the whole
        /// table is deleted.</p>
        /// <p>For the index parameters, when index is greater than or equal to 0, it specifies an index from
        /// the beginning with 0 being the first element. When index is less than 0, it specified an index from
        /// the end with -1 being the last element.</p>
        /// </remarks>
        /// <param name="tableIndex">The index of the table.</param>
        /// <param name="rowIndex">The index of the row in the table.</param>
        /// <returns>The row node that was just removed.</returns>
        public Row DeleteRow(int tableIndex, int rowIndex)
        {
            Row row = FetchRow(tableIndex, rowIndex);
            Table table = row.ParentTable;

            if (CurTableBuilder != null)
                throw new InvalidOperationException("Cannot delete a table row while building a table.");

            if (Cursor.IsAncestorNode(row))
            {
                //If the cursor is inside the deleted row, then need to move the cursor.
                bool isLastRow = (row == table.LastRow);
                if (!isLastRow)
                {
                    //Move the cursor to the next row.
                    MoveToCell(tableIndex, rowIndex + 1, 0, 0);
                }
                else
                {
                    //Move the cursor to just after the table.
                    Paragraph nextPara = (Paragraph)table.NextNonMarkupCompositeLimited;
                    MoveTo(nextPara, 0);
                }
            }

            //Remove the table row and also remove the table if this was the last row.
            row.Remove();
            if (table.FirstRow == null)
                table.Remove();

            return row;
        }

        /// <summary>
        /// Inserts a string into the document at the current insert position.
        /// </summary>
        /// <remarks>
        /// Current font formatting specified by the <see cref="Font"/> property is used.
        /// </remarks>
        /// <param name="text">The string to insert into the document.</param>
        public void Write(string text)
        {
            WriteCore(text, false);
        }

        /// <summary>
        /// Inserts a string and a paragraph break into the document.
        /// </summary>
        /// <remarks>
        /// Current font and paragraph formatting specified by the <see cref="Font"/> and <see cref="ParagraphFormat"/> properties are used.
        /// </remarks>
        /// <param name="text">The string to insert into the document.</param>
        public void Writeln(string text)
        {
            WriteCore(text, true);
        }

        /// <summary>
        /// Inserts a paragraph break into the document.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="InsertParagraph"/>.</p>
        /// </remarks>
        public void Writeln()
        {
            InsertParagraph();
        }

        /// <summary>
        /// Inserts a paragraph break into the document.
        /// </summary>
        /// <remarks>
        /// <p>Current paragraph formatting specified by the <see cref="ParagraphFormat"/> property is used.</p>
        /// <p>Breaks the current paragraph in two. After inserting the paragraph, the cursor is placed at the beginning of the new paragraph.</p>
        /// <p>An exception is thrown if it is not possible to insert a paragraph break at the current cursor position.</p>
        /// </remarks>
        /// <returns>The paragraph node that was just inserted. It is the same node as <see cref="CurrentParagraph"/>.</returns>
        public Paragraph InsertParagraph()
        {
            // WORDSNET-24033 Disallow inserting if the cursor is inside an inline-level SDT.
            if ((Cursor != null) &&
                (Cursor.NodeLevel == NodeLevel.Inline) &&
                (IsAtEndOfStructuredDocumentTag || (Cursor.ParentNode is StructuredDocumentTag)))
            {
                throw new InvalidOperationException("Cannot insert a node of this type at this location.");
            }

            EnsureCurrentRunIsSplitByCharOffset();

            // AW DOM represents paragraph slightly different from Word.
            // InsertParagraph means insert paragraph break character for Word (at least it acts this way) and
            // paragraph break character should be marked as inserted when track revision.
            // AW inserts new paragraph node and may insert new run node.
            // These newly created node should not be marked as inserted so revision tracking should be suspended during internal
            // node manipulations.
            Paragraph lastCurrent = CurrentParagraph;

            Paragraph newPara;
            using (new SuspendTrackRevisionsDocument(Document))
            {
                //New paragraph gets a copy of the current paragraph attributes.
                newPara = new Paragraph(mDoc, GetParaPrCopy(), GetRunPrCopy());

                if ((lastCurrent == null) && IsAtEndOfStructuredDocumentTag)
                {
                    // Expand SDT formatting to newly created paragraph.
                    if ((CurrentStructuredDocumentTag != null) && (CurrentStructuredDocumentTag.Level == MarkupLevel.Block))
                    {
                        Paragraph lastSdtPara = CurrentStructuredDocumentTag.LastChild as Paragraph;

                        if(lastSdtPara != null)
                            lastSdtPara.ParaPr.ExpandTo(newPara.ParaPr);
                    }

                    // Let's allow AppendChild to generate an exception if Cursor is an SDT of non-block level.
                    CurrentStructuredDocumentTag.AppendChild(newPara);
                }
                else
                {
                    if (CurrentParagraph.ParentNode.NodeType == NodeType.StructuredDocumentTag &&
                        CurrentParagraph.IsLastChild &&
                        !IsAtEndOfStructuredDocumentTag &&
                        IsAtEndOfParagraph)
                    {
                        // See Test25317Customer() for explanation.
                        CurrentStructuredDocumentTag.InsertNext(newPara);
                    }
                    else
                    {
                        // Add new paragraph node after (for performance reasons) the current paragraph.
                        CurrentParagraph.InsertNext(newPara);
                    }
                }

                if (IsAtEndOfParagraph || (lastCurrent == null))
                {
                    // The cursor was at the end of the old paragraph and now it needs to move to the newly appended
                    // empty paragraph.
                    // Or the cursor was at the end of a block-level SDT, let's place it to the new paragraph.
                    MoveTo(newPara);
                }
                else
                {
                    // The cursor was in the middle of the old paragraph and we inserted nodes before the cursor
                    // and appended a new paragraph.
                    // We move the nodes from the cursor to the end of the old paragraph into the new paragraph.
                    // We don't need to move the cursor itself because it is just a pointer and it remains valid.
                    newPara.InsertAfter(Cursor, null, newPara.LastChild);
                }
            }

            // Mark old paragraph as inserted. In other words it means that paragraph break character was inserted.
            if (Document.IsTrackRevisionsEnabled)
                RevisionTrackingUtil.AddInsertRevision((lastCurrent != null) ? lastCurrent : newPara, Document.EditSession);

            return CurrentParagraph;
        }

        /// <summary>
        /// Inserts a <see cref="StructuredDocumentTag" /> into the document.
        /// </summary>
        /// <returns>The <see cref="StructuredDocumentTag"/> node that was just inserted.</returns>
        /// <dev>
        /// AM. There are lots of tricky cases related to cursor positioning before SDT being inserted and these
        /// cases going to be breaking changes. I handled few, lets handle others on demand.
        /// </dev>
        public StructuredDocumentTag InsertStructuredDocumentTag(SdtType type)
        {
            switch (type)
            {
                case SdtType.PlainText:
                case SdtType.RichText:
                case SdtType.Checkbox:
                case SdtType.DropDownList:
                case SdtType.ComboBox:
                case SdtType.Picture:
                case SdtType.Date:
                    return InsertStructuredDocumentTagCore(type);

                // RepeatingSection cannot be inserted at cursor.
                case SdtType.RepeatingSection:
                case SdtType.RepeatingSectionItem:
                    throw new InvalidOperationException(
                        "Repeating section controls can only be inserted around entire paragraphs or rows.");

                // Postpone insertion of SDT types that are not present in MS Word UI.
                case SdtType.None:
                case SdtType.Bibliography:
                case SdtType.Citation:
                case SdtType.Equation:
                case SdtType.BuildingBlockGallery:
                case SdtType.DocPartObj:
                case SdtType.Group:
                case SdtType.EntityPicker:
                default:
                    throw new NotImplementedException("Structured document tag of given type cannot be inserted.");
            }
        }

        private StructuredDocumentTag InsertStructuredDocumentTagCore(SdtType type)
        {
            // Case when SDT insertion at document end while document end is inside existing SDT.
            if ((CurrentStructuredDocumentTag != null) && (IsAtEndOfStructuredDocumentTag == false) &&
                CurrentParagraph.IsLastChild && (CurrentStructuredDocumentTag.GetChildNodes(NodeType.Any, false).Count == 1))
            {
                // MS Word changes existing SDT markup level to inline and inserts new SDT at inline level as well.

                Paragraph para = CurrentParagraph;
                StructuredDocumentTag sdt = CurrentStructuredDocumentTag;
                using (new SuspendMappedCustomXmlUpdateDocument(mDoc))
                {
                    para.Remove();
                    sdt.ParentNode.InsertBefore(para, sdt);
                    sdt.Remove();
                    sdt.SetLevel(MarkupLevel.Inline);
                    while (para.HasChildNodes)
                        sdt.AppendChild(para.FirstChild);
                    para.AppendChild(sdt);
                }

                StructuredDocumentTag newSdt = new StructuredDocumentTag(mDoc, type, MarkupLevel.Inline);
                para.AppendChild(newSdt);

                MoveTo(newSdt);

                return newSdt;
            }

            // Case when SDT is inserted at end of existing SDT.
            if ((CurrentStructuredDocumentTag != null) && (IsAtEndOfParagraph == false) && IsAtEndOfStructuredDocumentTag)
            {
                // Insert block level SDT after existing SDT.

                StructuredDocumentTag newSdt = new StructuredDocumentTag(mDoc, type, MarkupLevel.Block);

                Paragraph lastPara = (Paragraph)CurrentStructuredDocumentTag.GetChild(NodeType.Paragraph, -1, true);
                if(lastPara != null)
                    lastPara.ParaPr.ExpandTo(((Paragraph)newSdt.FirstChild).ParaPr);

                mRunPr.ExpandTo(newSdt.ContentsRunPr);

                // We might get mapping update here.
                InsertNode(newSdt);

                return newSdt;
            }

            // Case when SDT is inserted at the end of empty paragraph.
            if (IsAtEndOfParagraph && CurrentParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
            {
                // Insert block level SDT that surrounds current paragraph.

                StructuredDocumentTag newSdt = new StructuredDocumentTag(mDoc, type, MarkupLevel.Block);
                newSdt.IsShowingPlaceholderText = true;
                ParaPr.ExpandTo(((Paragraph)newSdt.FirstChild).ParaPr);

                CurrentParagraph.InsertPrevious(newSdt);

                // Surround current paragraph by newly created block level SDT.
                Paragraph currParagraphBackup = CurrentParagraph;
                MoveTo((Paragraph)newSdt.FirstChild);
                currParagraphBackup.Remove();

                return newSdt;
            }
            else
            {
                // Otherwise insert inline level SDT.

                StructuredDocumentTag newSdt = new StructuredDocumentTag(mDoc, type, MarkupLevel.Inline);
                newSdt.IsShowingPlaceholderText = true;

                mRunPr.ExpandTo(newSdt.ContentsRunPr);

                // Word copies formatting to SDT content in case of CheckBox SDT.
                if (type == SdtType.Checkbox)
                    mRunPr.ExpandTo(((Run)newSdt.FirstChild).RunPr);

                InsertNode(newSdt);

                return newSdt;
            }
        }

        /// <summary>
        /// Inserts paragraph to current builder position in manner like Word does.
        /// </summary>
        /// <returns>Inserted paragraph.</returns>
        internal Paragraph InsertParagraphAsWord()
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // Detect font formatting source.
            Node curNode = Cursor;
            Paragraph curPara = CurrentParagraph;

            Run fontFormattingSource = IsAtEndOfParagraph ?
                curPara.GetLastRun() : (Run)curNode.PreviousSiblingOfType(NodeType.Run);

            if ((fontFormattingSource == null) && ReferenceEquals(curNode, curPara.FirstRun))
                fontFormattingSource = curPara.FirstRun;

            // Prepare font formatting properties for the new paragraph.
            RunPr runPr = (fontFormattingSource != null) ?
                fontFormattingSource.RunPr.Clone() : curPara.ParagraphBreakRunPr.Clone();

            Paragraph newPara = new Paragraph(mDoc, GetParaPrCopy(), runPr);
            curPara.InsertPrevious(newPara);

            // Only new paragraph should be marked with insertion revision.
            using (new SuspendTrackRevisionsDocument(mDoc))
            {
                Node lastNode = IsAtEndOfParagraph ? null : curNode;
                newPara.InsertAfter(curPara.FirstChild, lastNode, newPara.LastChild);
            }

            return newPara;
        }

        /// <summary>
        /// Inserts style separator into the document.
        /// </summary>
        /// <remarks>
        /// This method allows to apply different paragraph styles to two different parts of a text line.
        /// </remarks>
        public void InsertStyleSeparator()
        {
            StyleSeparatorInserter.InsertStyleSeparator(this);
        }

        /// <summary>
        /// Inserts a break of the specified type into the document.
        /// </summary>
        /// <remarks>
        /// Use this method to insert paragraph, page, column, section or line break into the document.
        /// </remarks>
        /// <param name="breakType">Specifies the type of the break to insert.</param>
        public void InsertBreak(BreakType breakType)
        {
            InsertBreakCore(breakType, true);
        }

        /// <summary>
        /// Inserts a break of the specified type into the document.
        /// </summary>
        internal void InsertBreakCore(BreakType breakType, bool throwOnError)
        {
            switch (breakType)
            {
                case BreakType.ParagraphBreak:
                    InsertParagraph();
                    return;
                case BreakType.PageBreak:
                {
                    if (!CheckCanSeriousBreak(throwOnError))
                        return;

                    bool inTheParaStart = IsAtStartOfParagraph && !IsAtEndOfParagraph;

                    InsertRun(ControlChar.PageBreak);

                    // WORDSNET-22834 Add paragraph after the page break depending from the compatibility options.
                    CompatibilityOptions compatOptions = Document.CompatibilityOptions;
                    if (inTheParaStart && (!compatOptions.SplitPgBreakAndParaMark || compatOptions.IsWord2013OrLaterCompatible))
                        InsertBreakCore(BreakType.ParagraphBreak, throwOnError);

                    return;
                }
                case BreakType.ColumnBreak:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertRun(ControlChar.ColumnBreak);
                    return;
                case BreakType.SectionBreakNewColumn:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertSectionCore(SectionStart.NewColumn);
                    return;
                case BreakType.SectionBreakNewPage:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertSectionCore(SectionStart.NewPage);
                    return;
                case BreakType.SectionBreakContinuous:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertSectionCore(SectionStart.Continuous);
                    return;
                case BreakType.SectionBreakEvenPage:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertSectionCore(SectionStart.EvenPage);
                    return;
                case BreakType.SectionBreakOddPage:
                    if (CheckCanSeriousBreak(throwOnError))
                        InsertSectionCore(SectionStart.OddPage);
                    return;
                case BreakType.LineBreak:
                    InsertRun(ControlChar.LineBreak);
                    return;
                default:
                    if (throwOnError)
                        throw new InvalidOperationException("Unknown break type.");
                    return;
            }
        }

        /// <summary>
        /// Inserts a TOC (table of contents) field into the document.
        /// </summary>
        /// <remarks>
        /// <p>This method inserts a TOC (table of contents) field into the document at
        /// the current position.</p>
        /// <p>A table of contents in a Word document can be built in a number of ways
        /// and formatted using a variety of options. The way the table is built and
        /// displayed by Microsoft Word is controlled by the field switches.</p>
        /// <p>The easiest way to specify the switches is to insert and configure a table of
        /// contents into a Word document using the Insert->Reference->Index and Tables menu,
        /// then switch display of field codes on to see the switches. You can press Alt+F9 in
        /// Microsoft Word to toggle display of field codes on or off.</p>
        /// <p>For example, after creating a table of contents, the following field is inserted
        /// into the document: <b>{ TOC &#92;o "1-3" &#92;h &#92;z <ms>&#92;u</ms> }</b>.
        /// You can copy <b>&#92;o "1-3" &#92;h &#92;z <ms>&#92;u</ms></b> and use it as the switches parameter.</p>
        /// <p>Note that <see cref="InsertTableOfContents"/> will only insert a TOC field, but
        /// will not actually build the table of contents. The table of contents is built by
        /// Microsoft Word when the field is updated.</p>
        /// <p>If you insert a table of contents using this method and then open the file
        /// in Microsoft Word, you will not see the table of contents because the TOC field
        /// has not yet been updated.</p>
        /// <p>In Microsoft Word, fields are not automatically updated when a document is opened,
        /// but you can update fields in a document at any time by pressing F9.</p>
        /// </remarks>
        /// <param name="switches">The TOC field switches.</param>
        public Field InsertTableOfContents(string switches)
        {
            // RK I'm using &#92; instead of \ in the above XML comment for autoporting to Java.

            if (!StringUtil.HasChars(switches))
                throw new ArgumentException("switches is required.");

            return InsertField(string.Format("TOC {0}", switches), "");
        }

        /// <overloads>Inserts a Word field into a document.</overloads>
        /// <summary>
        /// Inserts a Word field into a document and optionally updates the field result.
        /// </summary>
        /// <remarks>
        /// <para>This method inserts a field into a document.
        /// Aspose.Words can update fields of most types, but not all. For more details see the
        /// <see cref="InsertField(string,string)"/> overload.</para>
        ///
        /// <seealso cref="Field"/>
        /// </remarks>
        /// <param name="fieldType">The type of the field to append.</param>
        /// <param name="updateField">Specifies whether to update the field immediately.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(FieldType fieldType, bool updateField)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertField(fieldType, updateField, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        /// <summary>
        /// Inserts a Word field into a document and updates the field result.
        /// </summary>
        /// <remarks>
        /// <para>This method inserts a field into a document and updates the field result immediately.
        /// Aspose.Words can update fields of most types, but not all. For more details see the
        /// <see cref="InsertField(string,string)"/> overload.</para>
        ///
        /// <seealso cref="Field"/>
        /// </remarks>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(string fieldCode)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertField(fieldCode, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        /// <summary>
        /// Inserts a Word field into a document without updating the field result.
        /// </summary>
        /// <remarks>
        /// <p>Fields in Microsoft Word documents consist of a field code and a field result.
        /// The field code is like a formula and the field result is like the value that
        /// the formula produces. The field code may also contain field switches
        /// that are like additional instructions to perform a specific action.</p>
        ///
        /// <p>You can switch between displaying field codes and results in your document in
        /// Microsoft Word using the keyboard shortcut Alt+F9. Field codes appear between curly braces ( { } ).</p>
        ///
        /// <p>To create a field, you need to specify a field type, field code and a "placeholder" field value.
        /// If you are not sure about a particular field code syntax, create the field in Microsoft Word first
        /// and switch to see its field code.</p>
        ///
        /// <para>Aspose.Words can calculate field results for most of the field types, but this method
        /// does not update the field result automatically. Because the field result is not calculated automatically,
        /// you are expected to pass some string value (or even an empty string) that will be inserted into the field result.
        /// This value will remain in the field result as a placeholder until the field is updated.
        /// To update the field result you can call <see cref="Field.Update()"/> on the field object returned
        /// to you or <see cref="Aspose.Words.Document.UpdateFields"/> to update fields in the whole document.</para>
        ///
        /// <seealso cref="Field"/>
        /// </remarks>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="fieldValue">The field value to insert. Pass <c>null</c> for fields that do not have a value.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertField(string fieldCode, string fieldValue)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertField(fieldCode, fieldValue, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        /// <summary>
        /// Inserts a hyperlink into the document.
        /// </summary>
        /// <remarks>
        /// <p>Note that you need to specify font formatting for the hyperlink display text explicitly
        /// using the <see cref="DocumentBuilder.Font"/> property.</p>
        /// <p>This methods internally calls <see cref="InsertField(string)"/> to insert an MS Word HYPERLINK field
        /// into the document.</p>
        /// </remarks>
        /// <param name="displayText">Text of the link to be displayed in the document.</param>
        /// <param name="urlOrBookmark">Link destination. Can be a url or a name of a bookmark inside the document.
        /// This method always adds apostrophes at the beginning and end of the url.</param>
        /// <param name="isBookmark"><c>true</c> if the previous parameter is a name of a bookmark inside the document;
        /// <c>false</c> is the previous parameter is a URL.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        public Field InsertHyperlink(string displayText, string urlOrBookmark, bool isBookmark)
        {
            ArgumentUtil.CheckNotNull(displayText, "displayText");
            ArgumentUtil.CheckNotNull(urlOrBookmark, "hrefOrBookmark");

            FieldBundle field = StartHyperlink(urlOrBookmark, isBookmark, "", "");
            Write(displayText);
            field.End = EndHyperlink();

            return field.GetField();
        }

        /// <summary>
        /// Inserts a text form field at the current position.
        /// </summary>
        /// <remarks>
        /// <p>If you specify a name for the form field, then a bookmark is automatically created with the same name.</p>
        /// </remarks>
        /// <param name="name">The name of the form field. Can be an empty string.</param>
        /// <param name="type">Specifies the type of the text form field.</param>
        /// <param name="format">Format string used to format the value of the form field.</param>
        /// <param name="fieldValue">Text that will be shown in the field.</param>
        /// <param name="maxLength">Maximum length the user can enter into the form field. Set to zero for unlimited length.</param>
        /// <returns>The form field node that was just inserted.</returns>
        public FormField InsertTextInput(
            string name,
            TextFormFieldType type,
            string format,
            string fieldValue,
            int maxLength)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (format == null)
                throw new ArgumentNullException("format");
            if (fieldValue == null)
                throw new ArgumentNullException("fieldValue");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength");

            InsertFieldStart(FieldType.FieldFormTextInput);

            if (StringUtil.HasChars(name))
                StartBookmark(name);

            InsertFieldCode(" FORMTEXT ");

            Node fieldSeparatorNode = InsertFieldSeparator(FieldType.FieldFormTextInput);

            // Create a field result that will be displayed in the document.
            // When there is no default there will be maximum 5 circles displayed.
            // Just mimic what MS Word is doing.
            string fieldResult = (StringUtil.HasChars(fieldValue)) ? fieldValue : FormField.DefaultTextInputValue;

            Run fieldResultNode = new Run(mDoc, fieldResult, GetRunPrCopy());
            InsertNode(fieldResultNode);

            Node lastInsertedNode = InsertFieldEnd(FieldType.FieldFormTextInput, true);

            if (StringUtil.HasChars(name))
                lastInsertedNode = EndBookmark(name);

            //Create and insert the actual form field node.
            FormField formField = InsertFormFieldNode(fieldSeparatorNode);
            formField.Name = name;
            formField.TextInputType = type;
            formField.TextInputFormat = format;
            formField.Result = fieldValue;
            formField.MaxLength = maxLength;

            // WORDSNET-5287 Cursor must be right after the inserted FormField nodes.
            MoveTo((Paragraph)lastInsertedNode.FirstNonMarkupParentNode, lastInsertedNode.NextSibling);

            return formField;
        }

        /// <summary>
        /// Inserts a checkbox form field at the current position.
        /// </summary>
        /// <remarks>
        /// <p>If you specify a name for the form field, then a bookmark is automatically created with the same name.</p>
        /// </remarks>
        /// <param name="name">The name of the form field. Can be an empty string. The value longer than 20 characters will be truncated.</param>
        /// <param name="checkedValue">Checked status of the checkbox form field.</param>
        /// <param name="size">Specifies the size of the checkbox in points. Specify 0 for MS Word
        /// to calculate the size of the checkbox automatically.</param>
        /// <returns>The form field node that was just inserted.</returns>
        public FormField InsertCheckBox(string name, bool checkedValue, int size)
        {
            // Default value equals to checked value at this case.
            return InsertCheckBox(name, checkedValue, checkedValue, size);
        }

        /// <summary>
        /// Inserts a checkbox form field at the current position.
        /// </summary>
        /// <remarks>
        /// <p>If you specify a name for the form field, then a bookmark is automatically created with the same name.</p>
        /// </remarks>
        /// <param name="name">The name of the form field. Can be an empty string. The value longer than 20 characters will be truncated.</param>
        /// <param name="defaultValue">Default value of the checkbox form field.</param>
        /// <param name="checkedValue">Current checked status of the checkbox form field.</param>
        /// <param name="size">Specifies the size of the checkbox in points. Specify 0 for MS Word
        /// to calculate the size of the checkbox automatically.</param>
        /// <returns>The form field node that was just inserted.</returns>
        public FormField InsertCheckBox(string name, bool defaultValue, bool checkedValue, int size)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            InsertFieldStart(FieldType.FieldFormCheckBox);

            if (StringUtil.HasChars(name))
                StartBookmark(name);

            InsertFieldCode(" FORMCHECKBOX ");

            // There is no field separator for this field.
            Node fieldEnd = InsertFieldEnd(FieldType.FieldFormCheckBox, false);

            if (StringUtil.HasChars(name))
                EndBookmark(name);

            //Create and insert the actual form field node.
            FormField formField = InsertFormFieldNode(fieldEnd);
            formField.Name = name;
            formField.Default = defaultValue;
            formField.Checked = checkedValue;
            if (size != 0)
            {
                formField.IsCheckBoxExactSize = true;
                formField.CheckBoxSize = size;
            }
            else
            {
                formField.IsCheckBoxExactSize = false;
                formField.CheckBoxSize = 10;    //Default size MS Word uses.
            }

            return formField;
        }

        /// <summary>
        /// Inserts a combobox form field at the current position.
        /// </summary>
        /// <remarks>
        /// <p>If you specify a name for the form field, then a bookmark is automatically created with the same name.</p>
        /// </remarks>
        /// <param name="name">The name of the form field. Can be an empty string. The value longer than 20 characters will be truncated.</param>
        /// <param name="items">The items of the ComboBox. Maximum is 25 items.</param>
        /// <param name="selectedIndex">The index of the selected item in the ComboBox.</param>
        /// <returns>The form field node that was just inserted.</returns>
        public FormField InsertComboBox(string name, string[] items, int selectedIndex)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (items == null)
                throw new ArgumentNullException("items");
            if (items.Length > DropDownItemCollection.MaxItemsCount)
                throw new ArgumentException("items");
            if ((selectedIndex < 0) || (selectedIndex >= items.Length))
                throw new ArgumentOutOfRangeException("selectedIndex");

            InsertFieldStart(FieldType.FieldFormDropDown);

            if (StringUtil.HasChars(name))
                StartBookmark(name);

            InsertFieldCode(" FORMDROPDOWN ");

            // There is no field separator for this field.
            Node fieldEnd = InsertFieldEnd(FieldType.FieldFormDropDown, false);

            if (StringUtil.HasChars(name))
                EndBookmark(name);

            //Create and insert the actual form field node.
            FormField formField = InsertFormFieldNode(fieldEnd);
            formField.Name = name;
            formField.DropDownSelectedIndex = selectedIndex;
            for (int i = 0; i < items.Length; i++)
                formField.DropDownItems.Add(items[i]);

            return formField;
        }

        /// <summary>
        /// Inserts a footnote or endnote into the document.
        /// </summary>
        /// <param name="footnoteType">Specifies whether to insert a footnote or an endnote.</param>
        /// <param name="footnoteText">Specifies the text of the footnote.</param>
        /// <returns>Returns a footnote object that was just created.</returns>
        public Footnote InsertFootnote(FootnoteType footnoteType, string footnoteText)
        {
            return InsertFootnote(footnoteType, footnoteText, null);
        }

        /// <summary>
        /// Inserts a footnote or endnote into the document.
        /// </summary>
        /// <param name="footnoteType">Specifies whether to insert a footnote or an endnote.</param>
        /// <param name="footnoteText">Specifies the text of the footnote.</param>
        /// <param name="referenceMark">Specifies the custom reference mark of the footnote.</param>
        /// <returns>Returns a footnote object that was just created.</returns>
        public Footnote InsertFootnote(FootnoteType footnoteType, string footnoteText, string referenceMark)
        {
            // Create the footnote.
            Footnote footnote = new Footnote(mDoc, footnoteType, !StringUtil.HasChars(referenceMark), referenceMark, GetRunPrCopy());
            Style referenceStyle = Document.Styles.FetchBySti(FootnoteUtil.GetFootnoteReferenceStyleIdentifier(footnoteType));
            footnote.RunPr[FontAttr.Istd] = referenceStyle.Istd;
            InsertNode(footnote);

            // Create a paragraph that will contain the footnote text.
            Paragraph footnotePara = new Paragraph(mDoc);
            Style paraStyle = Document.Styles.FetchBySti(FootnoteUtil.GetFootnoteTextStyleIdentifier(footnoteType));
            footnotePara.ParaPr[ParaAttr.Istd] = paraStyle.Istd;

            footnote.Paragraphs.Add(footnotePara);

            Inline footnoteNumber = (footnote.IsAuto)
                ? (Inline)new SpecialChar(mDoc, ControlChar.FootnoteRefChar, new RunPr())
                : new Run(mDoc, footnote.ReferenceMark, new RunPr());
            footnoteNumber.RunPr[FontAttr.Istd] = referenceStyle.Istd;
            footnotePara.AppendChild(footnoteNumber);

            if (StringUtil.HasChars(footnoteText))
            {
                Node saveCursor = Cursor;
                MoveTo(footnotePara);
                Write(" ");
                Write(footnoteText);
                Cursor = saveCursor;
            }

            return footnote;
        }

        /// <overloads>Inserts an image into the document.</overloads>
        /// <summary>
        /// Inserts an image from a <ms>.NET <see cref="Image"/></ms><java><see cref="Image"/></java><cpp><see cref="Image"/></cpp>
        /// object into the document. The image is inserted inline and at 100% scale.
        /// </summary>
        /// <param name="image">The image to insert into the document.</param>
        /// <returns>The image node that was just inserted.</returns>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// <java>
        /// <p>Aspose.Words will insert the image in the PNG format and with default settings.If you want to insert a<tt>BufferedImage</tt> in
        /// another format or with other settings, you need to save the image into a byte array and use <see cref = "InsertImage(byte[])"/>.</p>
        /// </java >
        /// </remarks>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public Shape InsertImage(Image image)
        {
            return InsertImage(image, -1, -1);
        }

        /// <summary>
        /// Inserts an image from a file or URL into the document. The image is inserted inline and at 100% scale.
        /// </summary>
        /// <param name="fileName">The file with the image. Can be any valid local or remote URI.</param>
        /// <returns>The image node that was just inserted.</returns>
        /// <remarks>
        /// <p>This overload will automatically download the image before inserting into the document
        /// if you specify a remote URI.</p>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(string fileName)
        {
            return InsertImage(fileName, -1, -1);
        }

        /// <summary>
        /// Inserts an image from a stream into the document. The image is inserted inline and at 100% scale.
        /// </summary>
        /// <param name="stream">The stream that contains the image.
        /// <java> The stream will be read from the current position, so one should be careful about stream position.</java>
        /// </param>
        /// <returns>The image node that was just inserted.</returns>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        /// <javaName>com.aspose.words.Shape insertImage(java.io.InputStream stream)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first
        public Shape InsertImage([CppIOStreamWrapper(IOStreamType.IStream)] Stream stream)
        {
            return InsertImage(stream, -1, -1);
        }

        /// <summary>
        /// Inserts an image from a byte array into the document. The image is inserted inline and at 100% scale.
        /// </summary>
        /// <param name="imageBytes">The byte array that contains the image.</param>
        /// <returns>The image node that was just inserted.</returns>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(byte[] imageBytes)
        {
            return InsertImage(imageBytes, -1, -1);
        }

        /// <summary>
        /// Inserts an inline image from a <ms>.NET <see cref="Image"/></ms><java><see cref="Image"/></java><cpp><see cref="Image"/></cpp>
        /// object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="image">The image to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// <java>
        /// <p>Aspose.Words will insert the image in the PNG format and with default settings.If you want to insert a<tt>BufferedImage</tt> in
        /// another format or with other settings, you need to save the image into a byte array and use <see cref = "InsertImage(byte[])"/>.</p>
        /// </java >
        /// </remarks>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public Shape InsertImage(Image image, double width, double height)
        {
            return InsertImage(image,
                RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault, 0,
                width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an inline image from a file or URL into the document and scales it to the specified size.
        /// </summary>
        /// <param name="fileName">The file that contains the image.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(string fileName, double width, double height)
        {
            return InsertImage(fileName,
                RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault, 0,
                width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an inline image from a stream into the document and scales it to the specified size.
        /// </summary>
        /// <param name="stream">The stream that contains the image.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        /// <javaName>com.aspose.words.Shape insertImage(java.io.InputStream stream,double width,double height)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first
        public Shape InsertImage([CppIOStreamWrapper(IOStreamType.IStream)] Stream stream, double width, double height)
        {
            return InsertImage(stream,
                RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault, 0,
                width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an inline image from a byte array into the document and scales it to the specified size.
        /// </summary>
        /// <param name="imageBytes">The byte array that contains the image.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(byte[] imageBytes, double width, double height)
        {
            return InsertImage(imageBytes,
                RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault, 0,
                width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an image from a <ms>.NET <see cref="Image"/></ms><java><see cref="Image"/></java><cpp><see cref="Image"/></cpp>
        /// object at the specified position and size.
        /// </summary>
        /// <param name="image">The image to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// <java>
        /// <p>Aspose.Words will insert the image in the PNG format and with default settings.If you want to insert a<tt>BufferedImage</tt> in
        /// another format or with other settings, you need to save the image into a byte array and use <see cref = "InsertImage(byte[])"/>.</p>
        /// </java >
        /// </remarks>
#if NETSTANDARD
        [CLSCompliant(false)] // SkiaSharp.SKBitmap is not CLSCompliant.
#endif
        public Shape InsertImage(
            Image image,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (MemoryStream stream = new MemoryStream())
            {
#if NETSTANDARD || CPLUSPLUS
                BitmapPal.SaveNativeImageForWord97(image, stream);
#else
                BitmapPal.SaveNativeImageForWord97(image, stream);
#endif
                return InsertImage(stream, horzPos, left, vertPos, top, width, height, wrapType);
            }
        }

        /// <summary>
        /// Inserts an image from a file or URL at the specified position and size.
        /// </summary>
        /// <param name="fileName">The file that contains the image.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(
            string fileName,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            ArgumentUtil.CheckHasChars(fileName, "fileName");

            // andrnosk: WORDSNET-927 Allow user to handle loading external resources.
            if (Document.ResourceLoadingCallback != null)
            {
                ResourceLoadingArgs args = new ResourceLoadingArgs("", fileName, ResourceType.Image);
                switch (Document.ResourceLoadingCallback.ResourceLoading(args))
                {
                    case ResourceLoadingAction.Default:
                        break;
                    case ResourceLoadingAction.Skip:
                        return null;
                    case ResourceLoadingAction.UserProvided:
                        return InsertImage(args.GetData(), horzPos, left, vertPos, top, width, height, wrapType);
                    default:
                        Debug.Fail("Never invoked!");
                        return null;
                }
            }

            using (Stream stream = SystemPal.OpenStreamFromHref(fileName))
                return InsertImage(stream, horzPos, left, vertPos, top, width, height, wrapType);
        }

        /// <summary>
        /// Inserts an image from a stream at the specified position and size.
        /// </summary>
        /// <param name="stream">The stream that contains the image.</param>
        /// <ms cpp='true'><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/></ms>
        /// <java><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageJavaParamsEx"]/*'/></java>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        /// <javaName>com.aspose.words.Shape insertImage(java.io.InputStream stream,int horzPos,double left,int vertPos,double top,double width,double height,int wrapType)</javaName>
        // JAVA: the first public api change map will be used: Stream -> java.io.InputStream
        // WORDSJAVA-25686 - Loading from InputStream always load into memory first
        public Shape InsertImage(
            [CppIOStreamWrapper(IOStreamType.IStream)] Stream stream,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] imageBytes = StreamUtil.CopyStreamToByteArray(stream);
            return InsertImage(imageBytes, horzPos, left, vertPos, top, width, height, wrapType);
        }

        /// <summary>
        /// Inserts an image from a byte array at the specified position and size.
        /// </summary>
        /// <param name="imageBytes">The byte array that contains the image.</param>
        /// <ms cpp='true'><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/></ms>
        /// <java><include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageJavaParamsEx"]/*'/></java>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertImage(
            byte[] imageBytes,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            return InsertImage(imageBytes, horzPos, left, vertPos, top, width, height, wrapType, true);
        }

        /// <summary>
        /// Inserts an image from a byte array at the specified position and size.
        /// </summary>
        internal Shape InsertImage(
            byte[] imageBytes,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType,
            bool considerExifOrientation)
        {
            if (imageBytes == null)
                throw new ArgumentNullException("imageBytes");

            bool isGif = false;
            if (ImageUtil.IsGif(imageBytes))
            {
                // Passed GIF image must be valid. So, check that the bitmap may be created.
                using (new BitmapPal(imageBytes))
                    isGif = true;
            }

            SizeD imageSize = null; // Size calculated based on the image data.

            bool isWord2007OrHigher = (mDoc.CompatibilityOptions.MswVersion > MsWordVersionCore.Word2007) ||
                (mDoc.CompatibilityOptions.MswVersion == MsWordVersionCore.Unspecified);

            Shape shape;

            // If MS Word version is higher then Word2007 or not specified
            // insert image as DML shape, in other cases as VML shape.
            if (isWord2007OrHigher)
            {
                shape = new Shape(mDoc, ShapeMarkupLanguage.Dml);

                DmlPicture picture = new DmlPicture();
                picture.Geometry = DmlGeometryReader.GetPresetGeometry("rect");
                picture.BlipFill = new DmlBlipFill();
                picture.BlipFill.Blip.Document = mDoc;

                // Dml shape requires NvDrawingProperties.Id.
                DmlNvPrPicture nvPrPicture = new DmlNvPrPicture();

                // WORDSNET-18867 Use unique value for "cNvPr" identifier which differ from identifier of the shape.
                nvPrPicture.NvDrawingProperties = new DmlNvDrawingProperties(mDoc.GetNextShapeId(), "");
                nvPrPicture.CNvProperties = new DmlCnvPrPicture();
                picture.NonVisualPr = nvPrPicture;
                shape.SetShapeType(ShapeType.Image);
                shape.DmlNode = picture;
            }
            else
            {
                shape = new Shape(mDoc, ShapeType.Image);
            }

            // Check whether the image is SVG. In this case it can be stored in DOCX document as svgBlip extension.
            if (ImageUtil.IsSvg(imageBytes))
                throw new NotSupportedException("FOSS");

            // WORDSNET-1786 Allow inserting GIF images into the document.
            // For VML shapes still previous logic is used. I.e. GIF bytes will be converted to PNG.
            // A customer MUST NOT access ImageBytes for DML shape with GIF bytes. It causes conversion to PNG.
            if (isWord2007OrHigher && isGif)
            {
                ((DmlPicture)shape.DmlNode).BlipFill.Blip.EmbedImage = imageBytes;

                // Set image size to avoid bytes conversion while updating shape size.
                ImageSizeCore gifSize = ImageUtil.GetGifSize(imageBytes);
                imageSize = new SizeD(gifSize.WidthPoints, gifSize.HeightPoints);
            }
            else
            {
                shape.ImageData.ImageBytes = imageBytes;
            }

            shape.RunPr = GetRunPrCopy();
            shape.RelativeHorizontalPosition = horzPos;
            shape.Left = left;
            shape.RelativeVerticalPosition = vertPos;
            shape.Top = top;
            shape.WrapType = wrapType;

            // WORDSNET-15504 Mimic MSW behavior, and set AspectRatioLocked true for ShapeType.Image.
            shape.AspectRatioLocked = true;

            // We presume we are inserting a top level shape, it has to be inserted into the tree
            // for the shape size validation below to work properly.
            InsertNode(shape);

            // WORDSNET-22466 Update shape orientation according to EXIF data for DML shape.
            // Note: Word does not change shape orientation while document import. See, test for issue 20914.
            // Perhaps, when loading progress will be implemented then it will be possible to remove "importMode" flag.
            double rotationAng = 0.0d;
            if (considerExifOrientation && isWord2007OrHigher && ImageUtil.IsJpeg(imageBytes))
                rotationAng = UpdateImageOrientation(shape, imageBytes);

            // Set shape size either to the image size or to the specified size.
            shape.SetSizeSafe(width, height, imageSize, rotationAng);

            // Set angle after the size. Because effect extends are added with the rotation.
            if (rotationAng > 0)
                shape.Rotation = rotationAng;

            return shape;
        }

        private double UpdateImageOrientation(Shape shape, byte[] imageBytes)
        {
            ExifOrientation exifOrientation = ImageUtil.GetJpegOrientation(imageBytes);
            if (exifOrientation == ExifOrientation.Horizontal)
                return 0.0d;

            FlipOrientation flip = ExifOrientationUtils.GetFlipOrientation(exifOrientation);
            if (flip != FlipOrientation.None)
                shape.FlipOrientation = flip;

            double rotationAng = ExifOrientationUtils.GetRotationAngle(exifOrientation);
            // Rotation must be updated after the shape size will set (effect extend will be updated to).
            return rotationAng;
        }

        /// <summary>
        /// Inserts <see cref="HtmlOleControl" /> object into current position.
        /// </summary>
        internal Shape InsertHtmlOleControl(HtmlOleControl htmlOleControl, Stream presentation)
        {
            Debug.Assert(presentation != null, "Image presentation cannot be null.");

            Shape shape = InsertOleImage(presentation);
            shape.SetShapeType(ShapeType.OleControl);
            shape.SetShapeAttrInternal(ShapeAttr.OleObject, htmlOleControl);

            return shape;
        }

        /// <summary>
        /// Inserts <see cref="Forms2OleControl"/> object into current position.
        /// </summary>
        /// <returns><see cref="Shape"/> object that contains passed <see cref="Forms2OleControl"/></returns>
        /// <seealso cref="Shape.OleFormat"/><seealso cref="OleFormat.OleControl"/>.
        public Shape InsertForms2OleControl(Forms2OleControl forms2OleControl)
        {
            Shape shape = new Shape(mDoc, ShapeMarkupLanguage.Vml);
            shape.SetShapeType(ShapeType.OleControl);

            shape.RunPr = GetRunPrCopy();
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Default;
            shape.Left = 0;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.TextFrameDefault;
            shape.Top = 0;
            shape.WrapType = WrapType.Inline;

            // Note, it is important that the Width and Height would be greater than 0. Otherwise,
            // for example, DOC writer will not write this shape at all (see Picf.IsValid())
            shape.SetWidthSafe(forms2OleControl.Width);
            shape.SetHeightSafe(forms2OleControl.Height);

            shape.SetShapeAttrInternal(ShapeAttr.OleObject, forms2OleControl);

            InsertNode(shape);

            return shape;
        }

        /// <summary>
        /// Inserts an HTML string into the document.
        /// </summary>
        /// <param name="html">An HTML string to insert into the document.</param>
        /// <remarks>
        /// You can use this method to insert an HTML fragment or whole HTML document.
        /// </remarks>
        public void InsertHtml(string html)
        {
            InsertHtml(html, HtmlInsertOptions.None);
        }

        /// <summary>
        /// Inserts an HTML string into the document.
        /// </summary>
        /// <param name="html">An HTML string to insert into the document.</param>
        /// <param name="useBuilderFormatting">
        /// A value indicating whether formatting specified in <see cref="DocumentBuilder"/>
        /// is used as base formatting for text imported from HTML.
        /// </param>
        /// <remarks>
        /// <para>
        /// You can use this method to insert an HTML fragment or whole HTML document.
        /// </para>
        /// <para>
        /// When <paramref name="useBuilderFormatting"/> is <c>false</c>,
        /// <see cref="DocumentBuilder"/> formating is ignored and formatting of inserted text
        /// is based on default HTML formatting. As a result, the text looks as it is rendered in browsers.
        /// </para>
        /// <para>
        /// When <paramref name="useBuilderFormatting"/> is <c>true</c>,
        /// formatting of inserted text is based on <see cref="DocumentBuilder"/> formatting,
        /// and the text looks as if it were inserted with <see cref="Write"/>.
        /// </para>
        /// </remarks>
        public void InsertHtml(string html, bool useBuilderFormatting)
        {
            HtmlInsertOptions options = useBuilderFormatting
                ? HtmlInsertOptions.UseBuilderFormatting
                : HtmlInsertOptions.None;
            InsertHtml(html, options);
        }

        /// <summary>
        /// Inserts an HTML string into the document. Allows to specify additional options.
        /// </summary>
        /// <param name="html">An HTML string to insert into the document.</param>
        /// <param name="options">Options that are used when HTML string is inserted.</param>
        /// <remarks>
        /// You can use this method to insert an HTML fragment or whole HTML document.
        /// </remarks>
        public void InsertHtml(string html, HtmlInsertOptions options)
        {
            InsertHtml(html, options, null);
        }

        /// <summary>
        /// Inserts an HTML string into the document. Allows to specify a hash set of bookmark names to be used in
        /// <see cref="RW.Html.Reader.HtmlHyperlinkResolver"/> to avoid bookmark retrieval for each inserted HTML value.
        /// </summary>
        internal void InsertHtml(string html, HtmlInsertOptions options, HashSetGeneric<string> bookmarkNames)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // WORDSNET-28036 Do not strip formatting in Design mode.
            if (IsStripFormatting() && !mDocumentBuilderOptions.DesignMode)
            {
                DocumentBuilder builder = new DocumentBuilder();
                builder.InsertHtml(html);
                Write(StripControlChars(builder.Document.ToString(SaveFormat.Text)));
            }
            else
            {
                IDocumentReader htmlInserter = ReaderFactory.CreateHtmlDocumentInserter(html, options, this, bookmarkNames);
                htmlInserter.Read();
            }
        }

        /// <summary>
        /// Groups the shapes passed as a parameter into a new GroupShape node which is inserted into the current position.
        /// </summary>
        /// <param name="shapes">The list of shapes to be grouped.</param>
        /// <remarks>
        /// <p>The position and dimension of the new GroupShape will be calculated automatically.</p>
        /// <p>VML and DML shapes cannot be grouped together.</p>
        /// </remarks>
        public GroupShape InsertGroupShape(params ShapeBase[] shapes)
        {
            if (shapes.Length == 0 || (shapes[0] == null))
                throw new ArgumentException("The shapes parameter shall contain at least one shape.");

            ShapeMarkupLanguage markupLanguage = shapes[0].MarkupLanguage;
            double leftMin = shapes[0].Left;
            double leftMax = leftMin;
            double topMin = shapes[0].Top;
            double topMax = topMin;
            double width = shapes[0].Width;
            double height = shapes[0].Height;
            foreach (ShapeBase shape in shapes)
            {
                if (shape.MarkupLanguage != markupLanguage)
                    throw new ArgumentException("VML and DML shapes cannot be grouped together.");

                if (shape.Left > leftMax)
                {
                    leftMax = shape.Left;
                    width = shape.Width;
                }

                if (shape.Left < leftMin)
                    leftMin = shape.Left;

                if (shape.Top > topMax)
                {
                    topMax = shape.Top;
                    height = shape.Height;
                }

                if (shape.Top < topMin)
                    topMin = shape.Top;
            }

            return InsertGroupShape(leftMin, topMin, width + leftMax - leftMin, height + topMax - topMin, shapes);
        }

        /// <summary>
        /// Groups the shapes passed as a parameter into a new GroupShape node of the specified size which is inserted into the specified position.
        /// </summary>
        /// <param name="left">Distance in points from the origin to the left side of the group shape.</param>
        /// <param name="top">Distance in points from the origin to the top side of the group shape.</param>
        /// <param name="width">The width of the group shape in points. A negative value is not allowed.</param>
        /// <param name="height">The height of the group shape in points. A negative value is not allowed.</param>
        /// <param name="shapes">The list of shapes to be grouped.</param>
        /// <remarks>
        /// VML and DML shapes cannot be grouped together.
        /// </remarks>
        public GroupShape InsertGroupShape(double left, double top, double width, double height, params ShapeBase[] shapes)
        {
            if (shapes.Length == 0 || (shapes[0] == null))
                throw new ArgumentException("The shapes parameter shall contain at least one shape.");

            ShapeMarkupLanguage markupLanguage = shapes[0].MarkupLanguage;
            foreach (ShapeBase shape in shapes)
            {
                if (shape.MarkupLanguage != markupLanguage)
                    throw new ArgumentException("VML and DML shapes cannot be grouped together.");
                if (!shape.IsTopLevel)
                    throw new ArgumentException("Only top-level shapes can be grouped.");
            }

            GroupShape group = new GroupShape(Document, markupLanguage);
            group.Left = left;
            group.Top = top;
            group.Width = width;
            group.Height = height;

            if (markupLanguage == ShapeMarkupLanguage.Dml)
            {
                CreateDmlGroupShape(group, DmlNodeType.WordprocessingGroupShape);
                UpdateTransforms(group, null);

                group.GraphicData.SetWidthCore(width, false);
                group.GraphicData.SetHeightCore(height, false);
            }
            else
            {
                group.GraphicData.Name = string.Format("Group {0}", group.Id);
                group.SetCoordSizeSafe(new Size(ConvertUtilCore.PointToTwip(width), ConvertUtilCore.PointToTwip(height)));
            }

            foreach (ShapeBase shape in shapes)
            {
                if (shape.WrapType != WrapType.None)
                    shape.WrapType = WrapType.None;
                if (markupLanguage == ShapeMarkupLanguage.Dml)
                {
                    if (shape.NodeType == NodeType.GroupShape)
                        CreateDmlGroupShape((GroupShape)shape, DmlNodeType.GroupShape);

                    UpdateTransforms(shape, group);
                }

                group.AppendChild(shape);

                if (markupLanguage == ShapeMarkupLanguage.Dml)
                {
                    // Coordinates of child shapes should be in EMUs: copy from the transform.
                    DmlTransform transform = shape.DmlNode.Transform;
                    shape.GraphicData.SetWidthCore(transform.Width, false);
                    shape.GraphicData.SetHeightCore(transform.Height, false);
                    shape.Left = transform.X;
                    shape.Top = transform.Y;
                }
                else
                {
                    shape.GraphicData.SetWidthCore(ConvertUtilCore.PointToTwip(shape.Width), false);
                    shape.GraphicData.SetHeightCore(ConvertUtilCore.PointToTwip(shape.Height), false);
                    shape.Left = ConvertUtilCore.PointToTwip(shape.Left);
                    shape.Top = ConvertUtilCore.PointToTwip(shape.Top);
                }
            }
            InsertNode(group);

            return group;
        }

        /// <summary>
        /// Inserts an chart object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="chartType">The chart type to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertChart(ChartType chartType, double width, double height)
        {
            return InsertChart(chartType, width, height, ChartStyle.Normal);
        }

        /// <summary>
        /// Inserts an chart object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="chartType">The chart type to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <param name="chartStyle">The style of the inserted chart.</param>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertChart(ChartType chartType, double width, double height, ChartStyle chartStyle)
        {
            return InsertChart(chartType, RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault,
                0, width, height, WrapType.Inline, chartStyle);
        }


        /// <summary>
        /// Inserts an chart object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="chartType">The chart type to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertChart(
            ChartType chartType,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType)
        {
            return InsertChart(chartType, horzPos, left, vertPos, top, width, height, wrapType, ChartStyle.Normal);
        }

        /// <summary>
        /// Inserts an chart object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="chartType">The chart type to insert into the document.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <param name="chartStyle">The style of the inserted chart.</param>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertChart(
            ChartType chartType,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType,
            ChartStyle chartStyle)
        {
            IChartInserter chartInserter = ReaderFactory.CreateChartInserter();
            return chartInserter.InsertChart(chartType, chartStyle, horzPos, left, vertPos,
                top, width, height, wrapType, this);
        }

        /// <summary>
        /// Inserts an online video object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="videoUrl">The URL to the video.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// <para>Insertion of online video from the following resources is supported:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>https://www.youtube.com/</description>
        /// </item>
        /// <item>
        /// <description>https://vimeo.com/</description>
        /// </item>
        /// </list>
        /// <para>If your online video is not displaying correctly, use <see cref="InsertOnlineVideo(string, string, byte[], double, double)"/>, which accepts custom embedded html code.</para>
        /// <para>The code for embedding video can vary between providers, consult your corresponding provider of choice for details.</para>
        /// </remarks>
        public Shape InsertOnlineVideo(string videoUrl, double width, double height)
        {
            return InsertOnlineVideo(videoUrl, RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.TextFrameDefault,
                 0, width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an online video object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="videoUrl">The URL to the video.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// <para>Insertion of online video from the following resources is supported:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>https://www.youtube.com/</description>
        /// </item>
        /// <item>
        /// <description>https://vimeo.com/</description>
        /// </item>
        /// </list>
        /// <para>If your online video is not displaying correctly, use <see cref="InsertOnlineVideo(string, string, byte[], double, double)"/>, which accepts custom embedded html code.</para>
        /// <para>The code for embedding video can vary between providers, consult your corresponding provider of choice for details.</para>
        /// </remarks>
        public Shape InsertOnlineVideo(
           string videoUrl,
           RelativeHorizontalPosition horzPos,
           double left,
           RelativeVerticalPosition vertPos,
           double top,
           double width,
           double height,
           WrapType wrapType)
        {
            return new VideoInserter(this).InsertFromUrl(videoUrl, horzPos, left, vertPos, top, width, height, wrapType);
        }

        /// <summary>
        /// Inserts an online video object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="videoUrl">The URL to the video.</param>
        /// <param name="videoEmbedCode">The embed code for the video.</param>
        /// <param name="thumbnailImageBytes">The thumbnail image bytes.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParams"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertOnlineVideo(
           string videoUrl,
           string videoEmbedCode,
           byte[] thumbnailImageBytes,
           double width,
           double height)
        {
            return InsertOnlineVideo(videoUrl, videoEmbedCode, thumbnailImageBytes, RelativeHorizontalPosition.Default,
                0, RelativeVerticalPosition.TextFrameDefault, 0, width, height, WrapType.Inline);
        }

        /// <summary>
        /// Inserts an online video object into the document and scales it to the specified size.
        /// </summary>
        /// <param name="videoUrl">The URL to the video.</param>
        /// <param name="videoEmbedCode">The embed code for the video.</param>
        /// <param name="thumbnailImageBytes">The thumbnail image bytes.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageParamsEx"]/*'/>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="DocumentBuilder.InsertImageCommon"]/*'/>
        /// </remarks>
        public Shape InsertOnlineVideo(
           string videoUrl,
           string videoEmbedCode,
           byte[] thumbnailImageBytes,
           RelativeHorizontalPosition horzPos,
           double left,
           RelativeVerticalPosition vertPos,
           double top,
           double width,
           double height,
           WrapType wrapType)
        {
            return new VideoInserter(this).InsertFromVideoEmbedCode(videoUrl, videoEmbedCode, thumbnailImageBytes, horzPos,
                left, vertPos, top, width, height, wrapType);
        }

        /// <summary>
        /// Inserts a horizontal rule shape into the document.
        /// </summary>
        /// <returns>The shape that is a horizontal rule.</returns>
        public Shape InsertHorizontalRule()
        {
            Shape shape = Shape.CreateHorizontalRule(mDoc);
            InsertNode(shape);
            return shape;
        }

        /// <summary>
        /// Returns <c>true</c> if we need to strip the formatting off the source string.
        /// </summary>
        private bool IsStripFormatting()
        {
            bool stripFormatting = false;

            if (Cursor.ParentNode.NodeType == NodeType.StructuredDocumentTag)
            {
                StructuredDocumentTag tag = (StructuredDocumentTag)Cursor.ParentNode;
                switch (tag.SdtType)
                {
                    // MS Word strips formatting off the content inserted into these SDT types.
                    case SdtType.Date:
                    case SdtType.ComboBox:
                    case SdtType.PlainText:
                        stripFormatting = true;
                        break;
                    case SdtType.Picture:
                        // DD: MS Word converts inserted content into an image and then inserts image into SDT. We will skip this for now
                        // until we get customer requests.
                        break;
                    case SdtType.Checkbox:
                    case SdtType.DropDownList:
                        throw new InvalidOperationException("Can not insert text into this StructuredDocumentTag.");
                    default:
                        break;
                }
            }
            return stripFormatting;
        }

        /// <summary>
        /// Remove control chars from plain text string.
        /// </summary>
        private static string StripControlChars(string plaintext)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in plaintext)
            {
                if ((c != ControlChar.LineBreakChar) &&
                    (c != ControlChar.LineFeedChar) &&
                    (c != ControlChar.SectionBreakChar) &&
                    (c != ControlChar.ParagraphBreakChar) &&
                    (c != ControlChar.PageBreakChar))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Inserts a table cell into the document.
        /// </summary>
        /// <remarks>
        /// <p>To start a table, just call <see cref="InsertCell"/>. After this, any content you add using
        /// other methods of the <see cref="DocumentBuilder"/> class will be added to the current cell.</p>
        /// <p>To start a new cell in the same row, call <see cref="InsertCell"/> again.</p>
        /// <p>To end a table row call <see cref="EndRow"/>.</p>
        /// <p>Use the <see cref="CellFormat"/> property to specify cell formatting.</p>
        /// </remarks>
        /// <returns>The cell node that was just inserted.</returns>
        public Cell InsertCell()
        {
            EnsureCurrentRunIsSplitByCharOffset();

            //Calling StartTable is implicit for the first level table.
            if (CurTableBuilder == null)
                StartTable();

            //User never gets to call StartRow, we always do it.
            if (CurTableBuilder.TableState == TableState.InTable)
                CurTableBuilder.StartRow();

            //User never gets to call EndCell, we always do it.
            if (CurTableBuilder.TableState == TableState.InCell)
                CurTableBuilder.EndCell();

            return CurTableBuilder.StartCell();
        }

        /// <summary>
        /// Starts a table in the document.
        /// </summary>
        /// <remarks>
        /// <p>The next method to call is <see cref="InsertCell"/>.</p>
        /// <p>This method starts a nested table when called inside a cell.</p>
        /// </remarks>
        /// <returns>The table node that was just created.</returns>
        public Table StartTable()
        {
            mTableBuilders.Push(new TableBuilder(this, mDocumentBuilderOptions.ContextTableFormatting));
            return CurTableBuilder.StartTable();
        }

        /// <summary>
        /// Ends a table in the document.
        /// </summary>
        /// <remarks>
        /// <p>This method should be called only once after <see cref="EndRow"/> was called. When called,
        /// <see cref="EndTable"/> moves the cursor out of the current cell to point just after the table.</p>
        /// </remarks>
        /// <returns>The table node that was just finished.</returns>
        public Table EndTable()
        {
            if (CurTableBuilder == null)
                throw new InvalidOperationException("Cannot end a table while not building a table.");

            Table table = CurTableBuilder.EndTable();
            mTableBuilders.Pop();

            return table;
        }

        /// <summary>
        /// Ends a table row in the document.
        /// </summary>
        /// <remarks>
        /// <p>Call <see cref="EndRow"/> to end a table row. If you call <see cref="InsertCell"/> immediately
        /// after that, then the table continues on a new row.</p>
        /// <p>Use the <see cref="RowFormat"/> property to specify row formatting.</p>
        /// </remarks>
        /// <returns>The row node that was just finished.</returns>
        public Row EndRow()
        {
            if (CurTableBuilder == null)
                throw new InvalidOperationException("Cannot end a row while not building a table.");

            Row row = CurTableBuilder.EndRow();

            // WORDSNET-13772 Update row hidden attribute to get unified behavior.
            if ((mFont != null) && mFont.Hidden)
                row.TablePr.Hidden = true;

            return row;
        }

        /// <summary>
        /// Marks the current position in the document as a bookmark start.
        /// </summary>
        /// <remarks>
        /// <p>Bookmarks in a document can overlap and span any range. To create a valid bookmark you need to
        /// call both <see cref="StartBookmark"/> and <see cref="EndBookmark"/> with the same <paramref name="bookmarkName"/>
        /// parameter.</p>
        /// <p>Badly formed bookmarks or bookmarks with duplicate names will be ignored when the document is saved.</p>
        /// </remarks>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <returns>The bookmark start node that was just created.</returns>
        public BookmarkStart StartBookmark(string bookmarkName)
        {
            BookmarkStart bookmarkStart = new BookmarkStart(mDoc, bookmarkName, 0);
            InsertNode(bookmarkStart);
            return bookmarkStart;
        }

        /// <summary>
        /// Marks the current position in the document as a bookmark end.
        /// </summary>
        /// <remarks>
        /// <p>Bookmarks in a document can overlap and span any range. To create a valid bookmark you need to
        /// call both <see cref="StartBookmark"/> and <see cref="EndBookmark"/> with the same <paramref name="bookmarkName"/>
        /// parameter.</p>
        /// <p>Badly formed bookmarks or bookmarks with duplicate names will be ignored when the document is saved.</p>
        /// </remarks>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <returns>The bookmark end node that was just created.</returns>
        public BookmarkEnd EndBookmark(string bookmarkName)
        {
            BookmarkEnd bookmarkEnd = new BookmarkEnd(mDoc, bookmarkName);
            InsertNode(bookmarkEnd);
            return bookmarkEnd;
        }

        /// <summary>
        /// Marks the current position in the document as a column bookmark start. The position must be in a table cell.
        /// </summary>
        /// <remarks>
        /// <p>A column bookmark covers one or more columns in a range of rows. To create a valid bookmark you
        /// need to call both <see cref="StartColumnBookmark"/> and <see cref="EndColumnBookmark"/> with the same
        /// <paramref name="bookmarkName"/> parameter.</p>
        /// <p>Badly formed bookmarks or bookmarks with duplicate names will be ignored when the document is saved.</p>
        /// <p>The actual position of the inserted <see cref="BookmarkStart"/> node may differ from the current document
        /// builder position.</p>
        /// </remarks>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <returns>The bookmark start node that was just created.</returns>
        public BookmarkStart StartColumnBookmark(string bookmarkName)
        {
            Cell cell = CurrentCell;
            if (cell == null)
                throw new InvalidOperationException("A column bookmark can only be started in a table cell.");

            Cell firstCell = cell.ParentRow.FirstCell;
            firstCell.EnsureMinimum();

            BookmarkStart bookmarkStart = new BookmarkStart(mDoc, bookmarkName);
            firstCell.FirstParagraph.InsertAfter(bookmarkStart, null);

            // Get column index as it will be in saved document after converting HorizontalMerge to GridSpan.
            int cellIndex = cell.GetMergedColumnIndex();
            bookmarkStart.FirstColumn = cellIndex;

            return bookmarkStart;
        }

        /// <summary>
        /// Marks the current position in the document as a column bookmark end. The position must be in a table cell.
        /// </summary>
        /// <remarks>
        /// <p>A column bookmark covers one or more columns in a range of rows. To create a valid bookmark you
        /// need to call both <see cref="StartColumnBookmark"/> and <see cref="EndColumnBookmark"/> with the same
        /// <paramref name="bookmarkName"/> parameter.</p>
        /// <p>Badly formed bookmarks or bookmarks with duplicate names will be ignored when the document is saved.</p>
        /// <p>The actual position of the inserted <see cref="BookmarkEnd"/> node may differ from the current document
        /// builder position.</p>
        /// </remarks>
        /// <param name="bookmarkName">Name of the bookmark.</param>
        /// <returns>The bookmark end node that was just created.</returns>
        public BookmarkEnd EndColumnBookmark(string bookmarkName)
        {
            Cell cell = CurrentCell;
            if (cell == null)
                throw new InvalidOperationException("A column bookmark can only be ended in a table cell.");

            Bookmark bookmark = cell.ParentTable.Range.Bookmarks[bookmarkName];
            if (bookmark == null)
                throw new InvalidOperationException("The corresponding bookmark start must be in the same table.");

            // Get column index as it will be in saved document after converting HorizontalMerge to GridSpan.
            int cellIndex = cell.GetMergedColumnIndex();
            if (bookmark.BookmarkStart.FirstColumn > cellIndex)
            {
                throw new InvalidOperationException(
                    "The end column index must be greater than or equal to the start column index.");
            }

            BookmarkEnd bookmarkEnd = new BookmarkEnd(mDoc, bookmarkName);
            cell.ParentTable.InsertAfter(bookmarkEnd, cell.ParentRow);

            bookmark.BookmarkStart.LastColumn = cellIndex;

            return bookmarkEnd;
        }

        /// <summary>
        /// Marks the current position in the document as an editable range start.
        /// </summary>
        /// <remarks>
        /// <p>Editable range in a document can overlap and span any range. To create a valid editable range you need to
        /// call both <see cref="StartEditableRange"/> and <see cref="EndEditableRange()"/>
        /// or <see cref="EndEditableRange(EditableRangeStart)"/> methods.</p>
        /// <p>Badly formed editable range will be ignored when the document is saved.</p>
        /// </remarks>
        /// <returns>The editable range start node that was just created.</returns>
        public EditableRangeStart StartEditableRange()
        {
            EditableRangeStart editableRangeStart = new EditableRangeStart(mDoc);
            mLastEditableRangeId = editableRangeStart.Id;
            InsertNode(editableRangeStart);
            return editableRangeStart;
        }

        /// <summary>
        /// Marks the current position in the document as an editable range end.
        /// </summary>
        /// <remarks>
        /// <p>Editable range in a document can overlap and span any range. To create a valid editable range you need to
        /// call both <see cref="StartEditableRange"/> and <see cref="EndEditableRange()"/>
        /// or <see cref="EndEditableRange(EditableRangeStart)"/> methods.</p>
        /// <p>Badly formed editable range will be ignored when the document is saved.</p>
        /// </remarks>
        /// <returns>The editable range end node that was just created.</returns>
        public EditableRangeEnd EndEditableRange()
        {
            if (mLastEditableRangeId == Uninitialized)
                throw new InvalidOperationException("EndEditableRange can not be called before StartEditableRange.");

            EditableRangeEnd editableRangeEnd = new EditableRangeEnd(mDoc, mLastEditableRangeId);
            InsertNode(editableRangeEnd);
            return editableRangeEnd;
        }

        /// <summary>
        /// Marks the current position in the document as an editable range end.
        /// </summary>
        /// <remarks>
        /// <p>Use this overload during creating nested editable ranges.</p>
        /// <p>Editable range in a document can overlap and span any range. To create a valid editable range you need to
        /// call both <see cref="StartEditableRange"/> and <see cref="EndEditableRange()"/>
        /// or <see cref="EndEditableRange(EditableRangeStart)"/> methods.</p>
        /// <p>Badly formed editable range will be ignored when the document is saved.</p>
        /// </remarks>
        /// <param name="start">This editable range start.</param>
        /// <returns>The editable range end node that was just created.</returns>
        public EditableRangeEnd EndEditableRange(EditableRangeStart start)
        {
            EditableRangeEnd editableRangeEnd = new EditableRangeEnd(mDoc, start.Id);
            InsertNode(editableRangeEnd);
            return editableRangeEnd;
        }
        /// <summary>
        /// Inserts a document at the cursor position.
        /// </summary>
        /// <remarks>
        /// This method mimics the MS Word behavior, as if CTRL+'A' (select all content) was pressed,
        /// then CTRL+'C' (copy selected into the buffer) inside one document
        /// and then CTRL+'V' (insert content from the buffer) inside another document.
        /// </remarks>
        /// <param name="srcDoc">Source document for inserting.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <returns>First node of the inserted content.</returns>
        public Node InsertDocument(Document srcDoc, ImportFormatMode importFormatMode)
        {
            return InsertDocument(srcDoc, importFormatMode, new ImportFormatOptions());
        }

        /// <summary>
        /// Inserts a document at the cursor position.
        /// </summary>
        /// <remarks>
        /// This method mimics the MS Word behavior, as if CTRL+'A' (select all content) was pressed,
        /// then CTRL+'C' (copy selected into the buffer) inside one document
        /// and then CTRL+'V' (insert content from the buffer) inside another document.
        /// </remarks>
        /// <param name="srcDoc">Source document for inserting.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <param name="importFormatOptions">Allows to specify options that affect formatting of a result document.</param>
        /// <returns>First node of the inserted content.</returns>
        public Node InsertDocument(Document srcDoc, ImportFormatMode importFormatMode, ImportFormatOptions importFormatOptions)
        {
            ArgumentUtil.CheckNotNull(importFormatOptions, "ImportFormatOptions");
            EnsureCurrentRunIsSplitByCharOffset();
            return DocumentInserter.InsertDocument(this, srcDoc, importFormatMode, importFormatOptions);
        }

        /// <summary>
        /// Inserts a document inline at the cursor position.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method mimics the MS Word behavior, as if CTRL+'A' (select all content) was pressed,
        /// then CTRL+'C' (copy selected into the buffer) inside one document
        /// and then CTRL+'V' (insert content from the buffer) inside another document.
        /// </para>
        /// <para>As a difference from <see cref="InsertDocument(Aspose.Words.Document,Aspose.Words.ImportFormatMode,Aspose.Words.ImportFormatOptions)"/>
        /// this method moves the content of the paragraph of the destination document,
        /// before which the source document is inserted, into the last
        /// paragraph of the inserted source document. Actually, this means that
        /// paragraph break of the last inserted paragraph is removed.</para>
        /// <para>Note, if the last node of the source document is not a paragraph, then nothing will be done.</para>
        /// </remarks>
        /// <param name="srcDoc">Source document for inserting.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <param name="importFormatOptions">Allows to specify options that affect formatting of a result document.</param>
        /// <returns>First node of the inserted content.</returns>
        public Node InsertDocumentInline(Document srcDoc, ImportFormatMode importFormatMode, ImportFormatOptions importFormatOptions)
        {
            ArgumentUtil.CheckNotNull(importFormatOptions, "ImportFormatOptions");
            EnsureCurrentRunIsSplitByCharOffset();

            importFormatOptions.InlineMode = true;
            Node node = DocumentInserter.InsertDocument(this, srcDoc, importFormatMode, importFormatOptions);
            importFormatOptions.InlineMode = false;

            return node;
        }

        /// <summary>
        /// Gets or sets the <see cref="Document"/> object that this object is attached to.
        /// </summary>
        public Document Document
        {
            get
            {
                return mDoc;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value == mDoc)
                    return;

                // Detach from the current document and attach to the new document.
                mDoc = value;
                Cursor = null;

                mRunPr = new RunPr();
                mRunPrStack = null;
                mParaPrStack = null;
                mFont = null;

                mTableBuilders = new Stack<TableBuilder>();

                // RK We are moving to start here because moving to end was making document builder very slow.
                // I never had time to figure out why, but now it is too late to change since many clients rely on this.
                MoveToDocumentStart();
            }
        }

        /// <summary>
        /// Returns an object that represents current font formatting properties.
        /// </summary>
        /// <remarks>
        /// <p>Use <see cref="Font"/> to access and modify font formatting properties.</p>
        /// <p>Specify font formatting before inserting text.</p>
        /// </remarks>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(this, Document);
                return mFont;
            }
        }

        /// <summary>
        /// True if the font is formatted as bold.
        /// </summary>
        public bool Bold
        {
            get { return Font.Bold; }
            set { Font.Bold = value; }
        }

        /// <summary>
        /// True if the font is formatted as italic.
        /// </summary>
        public bool Italic
        {
            get { return Font.Italic; }
            set { Font.Italic = value; }
        }

        /// <summary>
        /// Gets/sets underline type for the current font.
        /// </summary>
        public Underline Underline
        {
            get { return Font.Underline; }
            set { Font.Underline = value; }
        }

        /// <summary>
        /// Returns an object that represents current paragraph formatting properties.
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
        /// Returns an object that represents current list formatting properties.
        /// </summary>
        public ListFormat ListFormat
        {
            get { return (CurrentParagraph != null) ? CurrentParagraph.ListFormat : null; }
        }

        /// <summary>
        /// Returns an object that represents current page setup and section properties.
        /// </summary>
        public PageSetup PageSetup
        {
            get { return CurrentSection.PageSetup; }
        }

        /// <summary>
        /// Returns an object that represents current table row formatting properties.
        /// </summary>
        public RowFormat RowFormat
        {
            get
            {
                if (mRowFormat == null)
                    mRowFormat = new RowFormat(this);
                return mRowFormat;
            }
        }

        /// <summary>
        /// Returns an object that represents current table cell formatting properties.
        /// </summary>
        public CellFormat CellFormat
        {
            get
            {
                if (mCellFormat == null)
                    mCellFormat = new CellFormat(this);
                return mCellFormat;
            }
        }

        internal void InsertBeforeCurPara(Node node)
        {
            CurrentParagraph.InsertPrevious(node);
        }

        /// <summary>
        /// Saves current character formatting onto the stack.
        /// </summary>
        /// <seealso cref="Font"/>
        /// <seealso cref="PopFont"/>
        public void PushFont()
        {
            // WORDSNET-19483 ParagraphBreakRunPr should be considered.
            RunPr paraBreakRunPr = (CurrentParagraph != null)
                ? CurrentParagraph.ParagraphBreakRunPr.Clone()
                : GetRunPrCopy();

            FontPrs.Push(new FontPr(GetRunPrCopy(), paraBreakRunPr));
        }

        /// <summary>
        /// Retrieves character formatting previously saved on the stack.
        /// </summary>
        /// <seealso cref="Font"/>
        /// <seealso cref="PushFont"/>
        public void PopFont()
        {
            if (FontPrs.Count > 0)
            {
                FontPr fontPr = FontPrs.Pop();
                //Don't request need cloning of these attrs since they are not connected to the model.
                SetFont(fontPr.RunPr, false);
                if (CurrentParagraph != null)
                    CurrentParagraph.ParagraphBreakRunPr = fontPr.ParaBreakRunPr;
            }
        }

        /// <summary>
        /// Saves current run properties to the stack.
        /// </summary>
        internal void PushRunPr()
        {
            RunPrStack.Push(GetRunPrCopy());
        }

        /// <summary>
        /// Restores current run properties from the stack.
        /// </summary>
        internal void PopRunPr()
        {
            if (RunPrStack.Count > 0)
            {
                //Don't request need cloning of these attrs since they are not connected to the model.
                SetFont(RunPrStack.Pop(), false);
            }
        }

        /// <summary>
        /// Saves current paragraph properties to the stack.
        /// </summary>
        internal void PushParaPr()
        {
            ParaPrStack.Push(GetParaPrCopy());
        }

        /// <summary>
        /// Restores current paragraph properties from the stack.
        /// </summary>
        internal void PopParaPr()
        {
            if (ParaPrStack.Count > 0)
            {
                ParaPr paraPr = ParaPrStack.Pop();
                if (CurrentParagraph != null)
                    CurrentParagraph.ParaPr = paraPr;
            }
        }

        internal FieldBundle StartHyperlink(string href, string target, string screenTip)
        {
            bool isBookmark = UriUtil.IsSubAddressOnly(href);
            string urlOrBookmark = (isBookmark) ? UriUtil.GetSubAddress(href) : href;
            return StartHyperlink(urlOrBookmark, isBookmark, target, screenTip);
        }

        /// <summary>
        /// Starts Hyperlink with the specified field code in the current position in the document.
        /// </summary>
        internal FieldBundle StartHyperlink(FieldCodeHyperlink fieldCode)
        {
            FieldStart start = InsertFieldStart(FieldType.FieldHyperlink);
            InsertFieldCode(fieldCode.ToFieldCodeString());
            FieldSeparator separator = InsertFieldSeparator(FieldType.FieldHyperlink);
            return new FieldBundle(start, separator, null);
        }

        /// <summary>
        /// Needed so you can put any formatted text into hyperlink text.
        /// </summary>
        private FieldBundle StartHyperlink(string urlOrBookmark, bool isBookmark, string target, string screenTip)
        {
            FieldStart start = InsertFieldStart(FieldType.FieldHyperlink);

            FieldCodeHyperlink fieldCode = new FieldCodeHyperlink();
            fieldCode.Target = target;
            fieldCode.ScreenTip = screenTip;
            if (isBookmark)
            {
                fieldCode.SubAddress = urlOrBookmark;
            }
            else
            {
                fieldCode.Address = UriUtil.GetAddress(urlOrBookmark);
                fieldCode.SubAddress = UriUtil.GetSubAddress(urlOrBookmark);
            }
            InsertFieldCode(fieldCode.ToFieldCodeString());

            FieldSeparator separator = InsertFieldSeparator(FieldType.FieldHyperlink);
            return new FieldBundle(start, separator, null);
        }

        /// <summary>
        /// Call after you called StartHyperlink and inserted hyperlink text.
        /// No checks are made for proper start/end, so be careful.
        /// </summary>
        internal FieldEnd EndHyperlink()
        {
            return InsertFieldEnd(FieldType.FieldHyperlink, true);
        }

        internal RunPr GetRunPrCopy()
        {
            return mRunPr.Clone();
        }

        internal ParaPr GetParaPrCopy()
        {
            return ParaPr.Clone();
        }

        internal TablePr GetTablePrCopy()
        {
            return TablePr.Clone();
        }

        internal CellPr GetCellPrCopy()
        {
            return CellPr.Clone();
        }

        /// <summary>
        /// Returns <c>true</c> if the cursor is at the beginning of the current paragraph (no text before the cursor).
        /// </summary>
        public bool IsAtStartOfParagraph
        {
            get
            {
                if (CurrentParagraph == null)
                    return false;

                // Walk all nodes from the start of the paragraph.
                // If we have something other than bookmark, it means we are not at the start of the paragraph.
                Node node = CurrentParagraph.FirstChild;
                while ((node != null) && (node != Cursor))
                {
                    if ((node.NodeType == NodeType.BookmarkStart) || (node.NodeType == NodeType.BookmarkEnd))
                        node = node.NextSibling;
                    else
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the cursor is at the end of the current paragraph.
        /// </summary>
        public bool IsAtEndOfParagraph
        {
            //The only way the cursor as the end is when it is pointing to the paragraph itself.
            get { return (Cursor.NodeType == NodeType.Paragraph); }
        }

        /// <summary>
        /// Returns <b>true</b> if the cursor is at the end of a structured document tag.
        /// </summary>
        public bool IsAtEndOfStructuredDocumentTag
        {
            get
            {
                return mIsAtEndOfStructuredDocumentTag && (Cursor.NodeType == NodeType.StructuredDocumentTag);
            }
        }

        /// <summary>
        /// This is a common implementation for Write and Writeln. It is required because even when calling
        /// Write, the text may contain paragraph breaks and it needs to be split to create more than one paragraph.
        /// </summary>
        private void WriteCore(string text, bool isWriteln)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (IsAtEndOfStructuredDocumentTag)
            {
                switch (((StructuredDocumentTag)Cursor).Level)
                {
                    case MarkupLevel.Block:
                        InsertParagraph();
                        isWriteln = false;
                        break;
                    case MarkupLevel.Inline:
                        break;
                    default:
                        throw new InvalidOperationException("Cannot insert text at this cursor position.");
                }
            }

            //Convert CrLf and Lf into Crs.
            string normalizedText = WordUtil.NormalizeToWord(text);

            //If text had any Crs insert paragraphs as appropriate.
            int paraStart = 0;
            while (paraStart <= normalizedText.Length)
            {
                int paraEnd = normalizedText.IndexOf(ControlChar.ParagraphBreakChar, paraStart);
                if (paraEnd != -1)
                {
                    int length = paraEnd - paraStart;
                    if (length > 0)
                        InsertBidiAwareRun(normalizedText.Substring(paraStart, length));

                    // WORDSNET-27645 Ensure that whole multi-line text inserted into one location.
                    EnsureAtStructuredDocumentTagEnd();

                    switch (mParagraphBreakCharReplacement)
                    {
                        case ParagraphBreakCharReplacement.Paragraph:
                            InsertParagraph();
                            break;
                        case ParagraphBreakCharReplacement.ParagraphBreakChar:
                            InsertRun(ControlChar.ParagraphBreak);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    paraStart = paraEnd + 1;
                }
                else
                {
                    //No more paragraph breaks are found.
                    int length = normalizedText.Length - paraStart;
                    if (length > 0)
                        InsertBidiAwareRun(normalizedText.Substring(paraStart, length));

                    if (isWriteln)
                        InsertParagraph();

                    break;
                }
            }
        }

        /// <summary>
        /// Inserts a node before the cursor.
        /// </summary>
        public void InsertNode(Node node)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            //This is a hangover from the previous version where we end tables implicitly
            //whenever user starts inserting text after end of a table row.
            if ((CurTableBuilder != null) && (CurTableBuilder.TableState == TableState.InTable))
                EndTable();

            if (IsAtEndOfParagraph)
                CurrentParagraph.AppendChild(node);
            else if (IsAtEndOfStructuredDocumentTag)
                CurrentStructuredDocumentTag.AppendChild(node);
            else
                Cursor.InsertPrevious(node);
        }

        /// <summary>
        /// Adding new section involves breaking current paragraph and breaking current section
        /// and moving content after cursor to the new section.
        /// </summary>
        internal void InsertSection(SectionStart sectionStart)
        {
            // Shall we also have an ability to insert a section in exception-safe manner?
            CheckCanSeriousBreak(true);

            InsertSectionCore(sectionStart);
        }

        private void InsertSectionCore(SectionStart sectionStart)
        {
            InsertParagraph();

            // When Word tracks section insertion it just mark section break character as inserted.
            // We have no section break characters in DOM. Instead we have Section node.
            // I don't fully understand what should we do here so suspend revision tracking for this operation for a while.
            using (new SuspendTrackRevisionsDocument(Document))
            {
                //New section gets a copy of the current section SEP.
                SectPr newSectPr = CurrentSection.SectPr.Clone();

                //Add new section node after current section.
                Section newSection = new Section(mDoc, newSectPr);
                newSection.PageSetup.SectionStart = sectionStart;
                newSection.AppendChild(new Body(mDoc));

                CurrentSection.InsertNext(newSection);

                //We move only main text and do not copy headers, maybe could do in the future.
                newSection.Body.InsertAfter(CurrentParagraph, null, newSection.Body.LastChild);
            }
        }

        internal FieldStart InsertFieldStart(FieldType fieldType)
        {
            EnsureCurrentRunIsSplitByCharOffset();

            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertFieldStart(fieldType, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        internal Run InsertFieldCode(string fieldCode)
        {
            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertFieldCode(fieldCode, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        internal FieldEnd InsertFieldEnd(FieldType fieldType, bool hasSeparator)
        {
            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertFieldEnd(fieldType, hasSeparator, GetRunPrCopy(), CurrentParent,
                ReferenceNode, ReferenceNode == null);
        }

        internal FieldSeparator InsertFieldSeparator(FieldType fieldType)
        {
            // FieldAppender has different behavior than e.g. the CompositeNode.Insert method. To append field
            // nodes to a composite node, the 'refNode' argument must be 'null' and isAfter must be 'true': pass
            // ReferencedNode == null as isAfter.
            return FieldAppender.InsertFieldSeparator(fieldType, GetRunPrCopy(), CurrentParent, ReferenceNode,
                ReferenceNode == null);
        }

        /// <summary>
        /// Reposition cursor to the end of SDT if current paragraph is the last child of SDT.
        /// This is needed to insert new paragraph after current paragraph,
        /// otherwise it will be inserted after current SDT.
        /// </summary>
        internal void EnsureAtStructuredDocumentTagEnd()
        {
            if(IsAtEndOfParagraph && (CurrentParagraph.ParentNode.NodeType == NodeType.StructuredDocumentTag))
                MoveToStructuredDocumentTag((StructuredDocumentTag)CurrentParagraph.ParentNode, -1);
        }

        /// <summary>
        /// Creates a form field node for the specified ffdata and inserts it before the specified node.
        /// </summary>
        private FormField InsertFormFieldNode(Node insertBefore)
        {
            FormField formField = new FormField(mDoc, new FormFieldPr(), GetRunPrCopy());

            CompositeNode parentNode = (insertBefore == null)
                ? CurrentParent
                : insertBefore.ParentNode;

            //Although the actual FormField node is just before the field separator or field end
            //I insert it last as it allows soft refreshable collection of form fields to work properly.
            parentNode.InsertBefore(formField, insertBefore);
            return formField;
        }

        /// <summary>
        /// "Serious" breaks are page, column and section breaks. They are not always allowed.
        /// Throws if such a break is not allowed.
        /// </summary>
        private bool CheckCanSeriousBreak(bool throwOnError)
        {
            bool isOutsideMainStory = CurrentStory.StoryType != StoryType.MainText;
            bool isInsideTable = CurTableBuilder != null;

            if (throwOnError)
            {
                if (isOutsideMainStory)
                    throw new InvalidOperationException("Cannot insert the requested break outside of the main story.");

                if (isInsideTable)
                    throw new InvalidOperationException("Cannot insert the requested break inside a table.");
            }

            return throwOnError || !(isOutsideMainStory || isInsideTable);
        }

        /// <summary>
        /// Searches through tables in the current story of the current section.
        /// Returns the specified row of the specified table or throws if cannot complete successfully.
        /// </summary>
        private Row FetchRow(int tableIndex, int rowIndex)
        {
            //Need recursive search for tables since they can be nested.
            NodeCollection tables = CurrentStory.GetChildNodes(NodeType.Table, true);
            Table table = (Table)tables[tableIndex];
            if (table == null)
                throw new ArgumentOutOfRangeException("tableIndex");

            Row row = (Row)table.GetChild(NodeType.Row, rowIndex, false);
            if (row == null)
                throw new ArgumentOutOfRangeException("rowIndex");

            return row;
        }

        /// <summary>
        /// Inserts run, which takes into account whether it is rtl or ltr text.
        /// </summary>
        private void InsertBidiAwareRun(string text)
        {
            if (IsNeedBidiExplicitly(text))
            {
                // We are not using BidiParagraph.BidiRuns here, because this performs compatibility text normalization
                // which replaces some special characters. As we prepare the text for output to a document and not
                // to a screen, we are using BidiParagraph.GetBidiRunsInMsWordOrder(), which allows to specify canonical normalization
                // for the text in Unicode BiDi algorithm explicitly and therefore leaves characters unchanged.
                // See http://www.unicode.org/reports/tr15/ for more information about text normalization.
                BidiParagraph bidiParagraph = new BidiParagraph(text, ParagraphFormat.Bidi ? 1 : 0, null);
                foreach (BidiRun bidiRun in bidiParagraph.GetBidiRunsInMsWordOrder(ParagraphFormat.Bidi))
                {
                    AttrBoolEx newBidi = AttrBoolEx.FromBool(bidiRun.Rtl);

                    RunPr runPr = GetRunPrCopy();
                    runPr.SetAttr(FontAttr.Bidi, newBidi);

                    // WORDSNET-13550 We have to set bidi locale to 'he-IL' on RTL runs containing numbers with any of
                    // the following number separator characters: '+', '-', and '/'. Otherwise, these separators will be treated
                    // as neutral characters in MS Word and parts of the number will be reordered.
                    if (bidiRun.RequiresHebrewLocaleBi)
                    {
                        runPr.LocaleIdBi = (int)Language.HebrewIsrael;
                    }

                    Run newRunNode = new Run(mDoc, bidiRun.Text, runPr);
                    InsertNode(newRunNode);
                    ComplexScriptRunUpdater.DoProcess(newRunNode);
                }
            }
            else
            {
                InsertRun(text);
            }
        }

        /// <summary>
        /// Returns <c>true</c>, if for run, based on this text, BiDi property has to be set explicitly.
        /// </summary>
        private bool IsNeedBidiExplicitly(string text)
        {
            bool isRtlRun = Font.Bidi;

            foreach (char c in text)
            {
                BidiCharacterType characterType = UnicodeCharacterDataResolver.GetBidiCharacterType(c);
                bool isStrongLtrCharacter = characterType == BidiCharacterType.L;
                bool isStrongRtlCharacter = (characterType == BidiCharacterType.R) || (characterType == BidiCharacterType.AL);
                if ((isRtlRun && isStrongLtrCharacter) || (!isRtlRun && isStrongRtlCharacter))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates and sets a DmlGroupShape corresponding to the specified DmlNodeType.
        /// </summary>
        private static void CreateDmlGroupShape(GroupShape group, DmlNodeType dmlNodeType)
        {
            DmlGroupShape dmlGroup = new DmlGroupShape(dmlNodeType);
            group.DmlNode = dmlGroup;
            dmlGroup.Name = string.Format("Group {0}", group.Id);

            dmlGroup.NonVisualPr = new DmlNvPrGroupShape();
            dmlGroup.NonVisualPr.CNvProperties = new DmlCnvPrGroupShape();
            dmlGroup.NonVisualPr.NvDrawingProperties = new DmlNvDrawingProperties();
        }

        /// <summary>
        /// Updates DML transformation data.
        /// </summary>
        private static void UpdateTransforms(ShapeBase shape, GroupShape parentGroup)
        {
            DmlTransform transform = shape.DmlNode.Transform;

            // If child shape is a shape of another shape group, its bounds are already in EMUs and transform is defined.
            bool areBoundsInEmus = (shape.Width == transform.Width) && (shape.Height == transform.Height);
            if (!areBoundsInEmus)
            {
                transform.Width = ConvertUtilCore.PointToEmu(shape.Width);
                transform.Height = ConvertUtilCore.PointToEmu(shape.Height);
            }

            if (parentGroup != null)
            {
                transform.X = areBoundsInEmus
                    ? shape.Left - ConvertUtilCore.PointToEmu(parentGroup.Left)
                    : ConvertUtilCore.PointToEmu(shape.Left - parentGroup.Left);
                transform.Y = areBoundsInEmus
                    ? shape.Top - ConvertUtilCore.PointToEmu(parentGroup.Top)
                    : ConvertUtilCore.PointToEmu(shape.Top - parentGroup.Top);
            }

            DmlGroupTransform grTransform = transform as DmlGroupTransform;
            if (grTransform != null)
            {
                grTransform.ChildX = 0;
                grTransform.ChildY = 0;
                grTransform.ChildWidth = transform.Width;
                grTransform.ChildHeight = transform.Height;
            }
        }

        /// <summary>
        /// Sets font attributes to the specified attributes.
        /// </summary>
        /// <param name="runPr">New font attributes to use.</param>
        /// <param name="isNeedClone">Whether to clone the attributes before using or not.</param>
        internal void SetFont(RunPr runPr, bool isNeedClone)
        {
            mRunPr = (isNeedClone) ? runPr.Clone() : runPr;
        }

        /// <summary>
        /// Clears font attributes not affecting current paragraph break run attributes.
        /// </summary>
        internal void ClearFont()
        {
            mRunPr.Clear();
        }

        /// <summary>
        /// Saves formatting from the current paragraph cell to the builder's cell formatting.
        /// </summary>
        internal void SaveCurCellFormatting()
        {
            if (CurrentCell != null)
            {
                CellPr currCellPr = CurrentCell.CellPr;
                mCellPr = currCellPr.Clone();
                // Clone() removes inherited complex attributes,
                // such as borders, so let's copy them explicitly.
                currCellPr.CopyBordersTo(mCellPr);
            }
        }

        /// <summary>
        /// Inserts image as VML shape, for future use as container for embedded OLE object.
        /// </summary>
        private Shape InsertOleImage(Stream imageStream)
        {
            byte[] imageBytes = StreamUtil.CopyStreamToByteArray(imageStream);
            return InsertOleImage(imageBytes);
        }

        /// <summary>
        /// Inserts image as VML shape, for future use as container for embedded OLE object.
        /// </summary>
        private Shape InsertOleImage(byte[] imageBytes)
        {
            if (imageBytes == null)
                throw new ArgumentNullException("imageBytes");

            Shape shape = new Shape(mDoc, ShapeMarkupLanguage.Vml);
            shape.SetShapeType(ShapeType.Image);

            shape.RunPr = GetRunPrCopy();
            shape.ImageData.ImageBytes = imageBytes;
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Default;
            shape.Left = 0;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.TextFrameDefault;
            shape.Top = 0;
            shape.WrapType = WrapType.Inline;

            // We presume we are inserting a top level shape, it has to be inserted into the tree
            // for the shape size validation below to work properly.
            InsertNode(shape);

            // Set shape size either to the image size or to the specified size.
            // If auto-setting to the image size, limit the size to max allowed shape size to avoid throwing.
            ImageSize imageSize = shape.ImageData.ImageSize;
            shape.SetSizeSafe(imageSize.WidthPoints, imageSize.HeightPoints);

            return shape;
        }

        /// <summary>
        /// Moves the cursor to the child of the composite node at the specified position in text.
        /// </summary>
        private void MoveToCharIndex(CompositeNode node, int charIdx)
        {
            if (charIdx >= 0)
                MoveToCharIndexForward(node, charIdx);
            else
                MoveToCharIndexBackward(node, charIdx);
        }

        /// <summary>
        /// Moves the cursor to the child of the composite node at the specified position from beginning of the node.
        /// </summary>
        private void MoveToCharIndexForward(CompositeNode node, int charIdx)
        {
            DocumentPosition position = DocumentPosition.CreatePositionBefore(node);
            Node currentNode = null;

            while (currentNode != node)
            {
                position.Move(null, true, true, true, false, true);
                currentNode = position.Node;
                if (position.IsStart &&
                    // Prefer position before an inline story than before the first node inside the story.
                    !(currentNode.IsComposite && (currentNode.NodeLevel == NodeLevel.Inline) && (charIdx == 0)))
                {
                    continue;
                }

                int textLength = currentNode.IsComposite
                    ? ((CompositeNode)currentNode).GetEndText().Length
                    : currentNode.GetTextLength();

                // Skip this zero-length composite node end and go to the next node, if this is not the node in which
                // the position is being set.
                if (!position.IsStart && (textLength == 0) && currentNode.IsComposite && (currentNode != node))
                    continue;

                int oldCharIdx = charIdx;
                charIdx -= textLength;
                if ((oldCharIdx == 0) || (charIdx < 0))
                {
                    if (!position.IsStart && (currentNode == node) && (node.NodeType == NodeType.StructuredDocumentTag))
                    {
                        MoveToStructuredDocumentTagEnd((StructuredDocumentTag)node);
                    }
                    else
                    {
                        MoveTo(null, currentNode);
                        mCharOffset = oldCharIdx;
                    }

                    return;
                }
            }

            throw new InvalidOperationException("The character index is too large.");
        }

        private void MoveToStructuredDocumentTagEnd(StructuredDocumentTag structuredDocumentTag)
        {
            Cursor = structuredDocumentTag;
            mIsAtEndOfStructuredDocumentTag = true;
            mCharOffset = 0;

            // Mimic MS Word in setting inserting text font.

            RunPr runPr = structuredDocumentTag.ContentsRunPr;
            switch (structuredDocumentTag.NodeLevel)
            {
                case NodeLevel.Inline:
                {
                    Node inlineLevelNode = structuredDocumentTag.LastNonAnnotationChild;
                    while ((inlineLevelNode != null) && !(inlineLevelNode is Inline))
                        inlineLevelNode = inlineLevelNode.PreviousSibling;

                    if (inlineLevelNode != null)
                        runPr = ((Inline)inlineLevelNode).RunPr;

                    break;
                }
                case NodeLevel.Block:
                {
                    Paragraph paragraph = (Paragraph)structuredDocumentTag.GetChild(NodeType.Paragraph, -1, true);
                    if (paragraph != null)
                        runPr = paragraph.ParagraphBreakRunPr;

                    break;
                }
                default:
                    // No need to set font.
                    return;
            }

            SetFont(runPr, true);
        }

        /// <summary>
        /// Moves the cursor to the child of the composite node at the specified position from ending of the node (the
        /// position is a negative number).
        /// </summary>
        private void MoveToCharIndexBackward(CompositeNode node, int charIdx)
        {
            DocumentPosition position = DocumentPosition.CreatePositionAfter(node);

            while (position.Node != node || !position.IsStart)
            {
                Node currentNode = position.Node;
                int textLength = currentNode.IsComposite
                    ? ((CompositeNode)currentNode).GetEndText().Length
                    : currentNode.GetTextLength();

                charIdx += textLength;
                if (charIdx >= 0)
                {
                    MoveTo(null, currentNode);
                    mCharOffset = charIdx;
                    return;
                }

                while (position.Move(null, false, true, true, false, true) &&
                       position.IsStart &&
                       (position.Node != node))
                {
                    // Move if the current position is at the beginning of a node other than 'node'.
                }
            }

            throw new InvalidOperationException("The character index is too small.");
        }

        /// <summary>
        /// Splits the current run at the character offset if it has been specified by the previous execution of a method
        /// that sets a position in text.
        /// </summary>
        private void EnsureCurrentRunIsSplitByCharOffset()
        {
            int offset = mCharOffset;
            if (offset == 0)
                return;

            // Reset offset even if the current node cannot be split: to not use the value on cursor change.
            mCharOffset = 0;

            Run run = CurrentNode as Run;
            if (run != null)
                run.SplitBefore(offset);
        }

        private Run InsertRun(string text)
        {
            Run newRunNode = new Run(mDoc, text, GetRunPrCopy());
            InsertNode(newRunNode);
            ComplexScriptRunUpdater.DoProcess(newRunNode);

            return newRunNode;
        }

        /// <summary>
        /// True, when current position of the document builder is inside group shape.
        /// </summary>
        internal bool IsInGroupShape
        {
            get
            {
                return !IsAtEndOfParagraph && (CurrentNode.ParentNode.NodeType == NodeType.GroupShape);
            }
        }

        internal ParagraphBreakCharReplacement ParagraphBreakCharReplacement
        {
            get { return mParagraphBreakCharReplacement; }
            set { mParagraphBreakCharReplacement = value; }
        }

        /// <summary>
        /// Gets the node that is currently selected in this DocumentBuilder.
        /// </summary>
        /// <remarks>
        /// <p><see cref="CurrentNode"/> is a cursor of <see cref="DocumentBuilder"/> and points to a <see cref="Node"/>
        /// that is a direct child of a <see cref="Paragraph"/>. Any insert operations you perform using
        /// <see cref="DocumentBuilder"/> will insert before the <see cref="CurrentNode"/>.</p>
        /// <p>When the current paragraph is empty or the cursor is positioned just
        /// before the end of a paragraph or structured document tag, <see cref="CurrentNode"/> returns <c>null</c>.</p>
        /// <seealso cref="CurrentParagraph"/>
        /// <seealso cref="CurrentStructuredDocumentTag"/>
        /// </remarks>
        public Node CurrentNode
        {
            get { return (IsAtEndOfParagraph || IsAtEndOfStructuredDocumentTag) ? null : Cursor; }
        }

        /// <summary>
        /// Gets the paragraph that is currently selected in this <see cref="DocumentBuilder"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CurrentNode"/>
        /// </remarks>
        public Paragraph CurrentParagraph
        {
            get
            {
                if (IsAtEndOfParagraph)
                    return (Paragraph)Cursor;
                else
                    return (Paragraph)Cursor.GetAncestor(NodeType.Paragraph);
            }
        }

        /// <summary>
        /// Gets the structured document tag that is currently selected in this <see cref="DocumentBuilder"/>.
        /// </summary>
        public StructuredDocumentTag CurrentStructuredDocumentTag
        {
            get
            {
                if (IsAtEndOfStructuredDocumentTag)
                    return (StructuredDocumentTag)Cursor;
                else
                    return (StructuredDocumentTag)Cursor.GetAncestor(NodeType.StructuredDocumentTag);
            }
        }

        /// <summary>
        /// Gets the story that is currently selected in this <see cref="DocumentBuilder"/>.
        /// </summary>
        public Story CurrentStory
        {
            get
            {
#if CPLUSPLUS
                return Cursor.GetAncestorOf<Story>();
#else
                return (Story)Cursor.GetAncestor(typeof(Story));
#endif
            }
        }

        /// <summary>
        /// Gets the section that is currently selected in this <see cref="DocumentBuilder"/>.
        /// </summary>
        public Section CurrentSection
        {
            get { return (Section)CurrentStory.ParentNode; }
        }

        /// <summary>
        /// Returns current table builder or <c>null</c> if not building a table.
        /// </summary>
        private TableBuilder CurTableBuilder
        {
            get { return mTableBuilders.Top(); }
        }

        /// <summary>
        /// Gets the cell that is currently selected in this DocumentBuilder.
        /// </summary>
        private Cell CurrentCell
        {
            get
            {
                if (CurrentParagraph == null)
                    return null;

                return CurrentParagraph.ParentCell;
            }
        }

        /// <summary>
        /// Gets the parent node of the cursor position.
        /// </summary>
        private CompositeNode CurrentParent
        {
            get
            {
                if (IsAtEndOfParagraph || IsAtEndOfStructuredDocumentTag)
                    return (CompositeNode)Cursor;
                else
                    return Cursor.ParentNode;
            }
        }

        /// <summary>
        /// Gets the next node of the cursor position.
        /// </summary>
        private Node ReferenceNode
        {
            get
            {
                if (IsAtEndOfParagraph || IsAtEndOfStructuredDocumentTag)
                    return null;
                else
                    return Cursor;
            }
        }

        /// <summary>
        /// Gets stack of saved run character formatting. Created on demand.
        /// </summary>
        private Stack<RunPr> RunPrStack
        {
            get
            {
                if (mRunPrStack == null)
                    mRunPrStack = new Stack<RunPr>();
                return mRunPrStack;
            }
        }

        /// <summary>
        /// Gets stack of saved paragraph attributes.
        /// </summary>
        private Stack<ParaPr> ParaPrStack
        {
            get
            {
                if (mParaPrStack == null)
                    mParaPrStack = new Stack<ParaPr>();
                return mParaPrStack;
            }
        }

        /// <summary>
        /// Gets stack of saved character formatting. Created on demand.
        /// </summary>
        private Stack<FontPr> FontPrs
        {
            get
            {
                if (mFontPrs == null)
                    mFontPrs = new Stack<FontPr>();
                return mFontPrs;
            }
        }

        #region IRunAttrSource

        /// <summary>
        /// Implements IRunAttrSource to provide access to the current run attributes in the document builder.
        ///
        /// DocumentBuilder has its own copy of run attributes, not connected to the model
        /// to allow user to set font formatting before inserting text into the model.
        ///
        /// The fact that the builder has its own disconnected copy of run attributes
        /// and provides the Font property requires the builder to perform proper
        /// run attribute resolution using character and paragraph styles.
        /// </summary>
        object IRunAttrSource.GetDirectRunAttr(int fontAttr)
        {
            return mRunPr.GetDirectAttr(fontAttr);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return mRunPr.GetDirectAttr(key, revisionsView);
        }

        /// <summary>
        /// The logic in this method is same as in Run.
        /// </summary>
        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            //Try to find the value specified in the character style or one of the based on character styles.
            object value = Font.Style.GetFontAttr(fontAttr, false);
            if (value != null)
                return value;

            //Get the value from the paragraph styles or from the default attributes.
            return ParagraphFormat.Style.GetFontAttr(fontAttr, true);
        }

        void IRunAttrSource.SetRunAttr(int fontAttr, object value)
        {
            mRunPr.SetAttr(fontAttr, value);

            if (CurrentParagraph == null)
                return;

            // andrnosk: WORDSNET-9823 Mimic MS Word behavior, apply run attrs to paragraph break when we at the end of paragraph or it is empty.
            if (IsAtEndOfParagraph || CurrentParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
                CurrentParagraph.ParagraphBreakRunPr.SetAttr(fontAttr, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            mRunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            mRunPr.Clear();

            if (CurrentParagraph == null)
                return;

            // WORDSNET-10765 MS Word clears paragraph break when cursor is at the end of paragraph or it is empty.
            if (IsAtEndOfParagraph || CurrentParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
                CurrentParagraph.ParagraphBreakRunPr.Clear();
        }

        #endregion

        #region IParaAttrSource

        object IParaAttrSource.GetDirectParaAttr(int key)
        {
            return ParaPr.GetDirectAttr(key);
        }

        object IParaAttrSource.GetDirectParaAttr(int key, RevisionsView revisionsView)
        {
            return ParaPr.GetDirectAttr(key, revisionsView);
        }

        object IParaAttrSource.FetchInheritedParaAttr(int key)
        {
            return (CurrentParagraph != null)
                ? CurrentParagraph.FetchInheritedParaAttr(key, RevisionsView.Original)
                : Document.Styles.DefaultParaPr.FetchAttr(key);
        }

        object IParaAttrSource.FetchParaAttr(int key)
        {
            IParaAttrSource paraAttrSource = this;
            object value = paraAttrSource.GetDirectParaAttr(key);
            return (value != null) ? value : paraAttrSource.FetchInheritedParaAttr(key);
        }

        void IParaAttrSource.SetParaAttr(int key, object value)
        {
            if (CurrentParagraph != null)
                ((IParaAttrSource)CurrentParagraph).SetParaAttr(key, value);
            else
                ParaPr.SetAttr(key, value);
        }

        void IParaAttrSource.RemoveParaAttr(int key)
        {
            ParaPr.Remove(key);
        }

        void IParaAttrSource.ClearParaAttrs()
        {
            ParaPr.Clear();
        }

        #endregion

        #region IRowAttrSource

        object IRowAttrSource.GetDirectRowAttr(int key)
        {
            return TablePr.GetDirectAttr(key);
        }

        object IRowAttrSource.FetchRowAttr(int key)
        {
            return TablePr.FetchAttr(key);
        }

        object IRowAttrSource.FetchInheritedRowAttr(int key)
        {
            return TablePr.FetchInheritedAttr(key);
        }

        void IRowAttrSource.SetRowAttr(int key, object value)
        {
            TablePr.SetAttr(key, value);
        }

        void IRowAttrSource.ClearRowAttrs()
        {
            ((IRowAttrSource)TablePr).ClearRowAttrs();
        }

        void IRowAttrSource.ResetToDefaultAttrs()
        {
            // WORDSNET-5727 reset to default attributes. Clearing attributes results in ugly rendering.
            TablePr.Clear();
            TablePr.CreateMSWordLooking().ExpandTo(TablePr);

            // DM 5727 comments:
            // Actually, IRowAttrSource.ResetToDefaultAttrs() was added per 5727 to support RowFormat.ClearFormatting().
            // RowFormat.ClearFormatting should revert to the default attributes values, according to the API reference.
            // However, clearing the attributes does not produce this result, as default attributes do have some values.

            // This implementation is different from Font.ClearFormatting() where clearing the attributes is sufficient.
            // This is because on clearing the font attributes, the missing attributes are resolved via styles.
            // There is not implemented for table styles and row attributes.
            // Per 5727, I considered introducing row attributes resolution via table styles too risky.
            // Besides, it would break HtmlTableReader which really needs empty attributes.
            // With style resolution, they will never be empty, because "Table Normal" style is a global default,
            // and some attributes are defined in "Table Normal".
            // But some day we'll need to do proper row attributes resolution via table styles.
            // End of 5727 comments.
        }

        #endregion

        #region ICellAttrSource

        object ICellAttrSource.GetDirectCellAttr(int key)
        {
            return CellPr.GetDirectAttr(key);
        }

        object ICellAttrSource.FetchCellAttr(int key)
        {
            return CellPr.FetchAttr(key);
        }

        object ICellAttrSource.FetchInheritedCellAttr(int key)
        {
            return CellPr.FetchInheritedAttr(key);
        }

        void ICellAttrSource.SetCellAttr(int key, object value)
        {
            // Border is complex attribute. We should explicitly set its parent because DocumentBuilder
            // can have various cellPr source depending on where is builder's Cursor at the moment.
            Border border = value as Border;
            if (border != null)
            {
                CellFormat cellFormat = (CurrentCell != null) ? CurrentCell.CellFormat : CellFormat;
                border.SetParent(cellFormat);
                border = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(border, cellFormat);
            }

            CellPr.SetAttr(key, value);
        }

        void ICellAttrSource.ClearCellAttrs()
        {
            CellPr.Clear();
        }

        #endregion

        /// <summary>
        /// Provides safe cursor position. In case of cursor node was removed, tries to get its parent as cursor.
        /// </summary>
        /// <remarks>
        /// WORDSNET-6144 This workaround tries to recover cursor position
        /// if node at cursor is removed by external procedure.
        /// In this CR node is removed by field merge operation but I think
        /// there are many possible way to do this.
        ///
        /// Better approach is to use INodeChangingCallback here to update cursor runtime but I found
        /// that it's not easy to detect that node is really removed or it just
        /// in changing process during, for example, DocumentBuilder.InsertBreakCore method.
        ///
        /// So since it is rare case and it is's not decided yet should we update DocumentBuilder
        /// with runtime DOM changes or it's better for customer to create new DocumentBuilder
        /// instance we decided to put this "workaround" first.
        /// </remarks>
        private Node Cursor
        {
            get
            {
                Node cursor;

                if ((mCursor != null) && (mCursor.ParentNode == null))
                    cursor = mCursorParentBackup;
                else
                    cursor = mCursor;

                if ((cursor != null) && (cursor.NodeLevel == NodeLevel.SectionStory))
                {
                    Paragraph paragraph = ((Story)cursor).FirstParagraph;
                    if (paragraph == null)
                        // get first paragraph from current section
                        cursor = ((Section)cursor.GetAncestor(NodeType.Section)).Body.FirstParagraph;
                    else
                        cursor = paragraph;
                }

                if (cursor == null)
                    cursor = Document.FirstSection.Body.FirstParagraph;

                mIsAtEndOfStructuredDocumentTag = mIsAtEndOfStructuredDocumentTag &&
                    (cursor != null) && (cursor.NodeType == NodeType.StructuredDocumentTag);

                return cursor;
            }
            set
            {
                mCursor = value;

                if (mCursor != null)
                {
                    mCursorParentBackup = mCursor.ParentNode;
                }

                // mIsAtEndOfStructuredDocumentTag is set only in MoveToStructuredDocumentTagEnd after setting Cursor.
                mIsAtEndOfStructuredDocumentTag = false;
            }
        }

        /// <summary>
        /// Returns current paragraph formatting depending on the current position within document.
        /// </summary>
        /// <remarks>
        /// When we are inside a paragraph this exposes formatting of the paragraph, otherwise we return formatting from
        /// <see cref="mParaPr"/>.
        /// </remarks>
        private ParaPr ParaPr
        {
            get
            {
                if (CurrentParagraph != null)
                    return CurrentParagraph.ParaPr;

                if (mParaPr == null)
                    mParaPr = new ParaPr();

                return mParaPr;
            }
        }

        /// <summary>
        /// Returns current row formatting depending on the current position within document.
        /// </summary>
        /// <remarks>
        /// When we are inside a row this exposes formatting of this row, otherwise we return formatting from <see cref="DocumentBuilder.mTablePr"/>.
        /// </remarks>
        private TablePr TablePr
        {
            get
            {
                // We are not started any row yet. Hence we can't change formatting of rows and we get it from the builder.
                if ((CurTableBuilder != null) && (CurTableBuilder.TableState == TableState.InTable))
                    return mTablePr;
                // We have started a row.
                // If we are inside of any cell it means we should work with row formatting of its table.
                if ((CurrentParagraph != null) && CurrentParagraph.IsInCell)
                    return CurrentParagraph.ParentRow.TablePr;

                return mTablePr;
            }
        }

        /// <summary>
        /// Returns current cell formatting depending on the current position within document.
        /// </summary>
        /// <remarks>
        /// When we are inside a cell this exposes formatting of this cell, otherwise we return formatting from <see cref="DocumentBuilder.mCellPr"/>.
        /// </remarks>
        private CellPr CellPr
        {
            get
            {
                // We are not started any cell yet. Hence we can't change formatting of cells and we get it from the builder.
                if ((CurTableBuilder != null) && (CurTableBuilder.TableState != TableState.InCell))
                    return mCellPr;
                // We have started a cell.
                // If we are inside of any cell it means we should work with cell formatting of its table.
                if ((CurrentParagraph != null) && CurrentParagraph.IsInCell)
                    return CurrentParagraph.ParentCell.CellPr;

                return mCellPr;
            }
        }

        /// <summary>
        /// Stores font properties.
        /// </summary>
        private class FontPr
        {
            public FontPr(RunPr runPr, RunPr paraBreakRunPr)
            {
                mRunPr = runPr;
                mParaBreakRunPr = paraBreakRunPr;
            }

            public RunPr RunPr
            {
                get
                {
                    return mRunPr;
                }
            }

            public RunPr ParaBreakRunPr
            {
                get
                {
                    return mParaBreakRunPr;
                }
            }

            private readonly RunPr mRunPr;
            private readonly RunPr mParaBreakRunPr;
        }

        private Document mDoc;

        /// <summary>
        /// Additional options for the document building process.
        /// </summary>
        private readonly DocumentBuilderOptions mDocumentBuilderOptions;

        /// <summary>
        /// The cursor position. Can be an inline node or a paragraph.
        /// When points to an inline node, inserts will be done before it.
        /// When points to a paragraph, inserts will be at the end of the paragraph.
        /// </summary>
        private Node mCursor;

        /// <summary>
        /// Parent node of node was last time cursor moved to.
        /// </summary>
        /// <remarks>
        /// See DocumentBuilder.Cursor property for details.
        /// </remarks>
        private Node mCursorParentBackup;

        /// <summary>
        /// This is our own run attributes, therefore changing font properties does not change
        /// them in the document, but changes them in this collection.
        /// </summary>
        private RunPr mRunPr;
        /// <summary>
        /// The facade object that exposes the run attributes.
        /// </summary>
        private Font mFont;

        /// <summary>
        /// The stack of saved run attributes.
        /// </summary>
        /// <remarks>Always use the getter property as it creates this on demand.</remarks>
        private Stack<RunPr> mRunPrStack;

        /// <summary>
        /// The stack of saved paragraph attributes.
        /// </summary>
        /// <remarks>Always use the getter property as it creates this on demand.</remarks>
        private Stack<ParaPr> mParaPrStack;

        /// <summary>
        /// Always use the getter property as it creates this on demand.
        /// </summary>
        private Stack<FontPr> mFontPrs;

        /// <summary>
        /// Row formatting used when we are starting a new table.
        /// See more details in <see cref="TablePr"/> property.
        /// </summary>
        private readonly TablePr mTablePr = TablePr.CreateMSWordLooking();
        /// <summary>
        /// Cell formatting used when we are starting a new table.
        /// See more details in <see cref="CellPr"/> property.
        /// </summary>
        private CellPr mCellPr = new CellPr();

        private ParaPr mParaPr;

        /// <summary>
        /// The facade objects that expose the table row and cell attributes.
        /// </summary>
        private RowFormat mRowFormat;
        private CellFormat mCellFormat;

        /// <summary>
        /// The facade object that exposes the paragraph formatting.
        /// </summary>
        private ParagraphFormat mParagraphFormat;

        private Stack<TableBuilder> mTableBuilders;
        private ParagraphBreakCharReplacement mParagraphBreakCharReplacement = ParagraphBreakCharReplacement.Paragraph;

        /// <summary>
        /// The last created EditableRange Id, used to end EditableRange which was previously started.
        /// </summary>
        private int mLastEditableRangeId = Uninitialized;

        /// <summary>
        /// Offset of the current position within the run.
        /// </summary>
        private int mCharOffset;

        private bool mIsAtEndOfStructuredDocumentTag;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int Uninitialized = -1;
    }
}
