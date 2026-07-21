// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/12/2014 by Ilya Navrotskiy

using System;
using Aspose.Collections;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Class for inserting one document into another.
    /// </summary>
    internal class DocumentInserter
    {
        /// <summary>
        /// Initializes new instance with a specified destination document.
        /// </summary>
        internal DocumentInserter(Document dstDoc)
        {
            mDstDoc = dstDoc;
        }

        /// <summary>
        /// Initializes new instance with a specified DocumentBuilder.
        /// </summary>
        internal DocumentInserter(DocumentBuilder documentBuilder) : this(documentBuilder.Document)
        {
            mDocumentBuilder = documentBuilder;
        }

        /// <summary>
        /// Inserts document at the current position of a document builder.
        /// </summary>
        internal static Node InsertDocument(DocumentBuilder builder, Document srcDoc, ImportFormatMode importFormatMode,
            ImportFormatOptions options)
        {
            DocumentInserter documentInserter = new DocumentInserter(builder);
            return documentInserter.InsertDocument(srcDoc, importFormatMode, options);
        }

        /// <summary>
        /// Inserts document at the current position of a document builder.
        /// </summary>
        internal Node InsertDocument(Document srcDoc, ImportFormatMode importFormatMode, ImportFormatOptions options)
        {
            Debug.Assert(options != null);

            if (!CanInsertDocument(srcDoc))
                throw new InvalidOperationException("You cannot insert document with this content into this location.");

            mNodeImporter = new NodeImporter(srcDoc, DstDoc, importFormatMode, options);

            // FOSS

            // Word inserts document into inline level SDT in a very special way.
            return ((CurrentNode != null) && (CurrentNode.ParentNode.NodeLevel == NodeLevel.Inline))
                ? InsertIntoInlineLevelSdt()
                : InsertIntoBlockLevelNode();
        }

        /// <summary>
        /// Appends source document to the end of a destination document.
        /// </summary>
        internal void AppendDocument(Document srcDoc, ImportFormatMode importFormatMode, ImportFormatOptions options)
        {
            ImportFormatOptions importFormatOptions = (options != null) ? options : new ImportFormatOptions();
            bool isSmartStyleBehavior = importFormatOptions.SmartStyleBehavior;

            // Some tests (for example, Test19275, TestDefect26460, UnifiedTestImportStyleWithList) fail,
            // if SmartStyleBehavior is disabled and it is not clear which result is correct,
            // as we are not mimic Word behavior in these tests (see analysis of WORDSNET-19275).
            // So, lets for a while always append documents with this option enabled.
            importFormatOptions.SmartStyleBehavior = true;
            mNodeImporter = new NodeImporter(srcDoc, DstDoc, importFormatMode, importFormatOptions);

            mParaBefore = LastDstParagraph;

            // FOSS

            foreach (Section srcSection in SrcDoc.Sections)
            {
                Section importedSection = (Section)mNodeImporter.ImportNode(srcSection, true, mInfoCollector);

                // WORDSNET-28243 Word removes section type of the first appended section.
                // WORDSNET-28336 Implemented a new option that allows to control either to remove section type, or not.
                if (srcSection.IsFirstSection && importFormatOptions.AppendDocumentWithNewPage)
                {
                    if (importedSection.SectPr.SectionStart != SectionStart.NewPage)
                        importedSection.SectPr.Remove(SectAttr.SectionStart);
                }

                PreInsertFormatting();
                DstDoc.AppendChild(importedSection);
            }

            PostInsertFormatting();

            // Restore SmartStyleBehavior.
            if (options != null)
                options.SmartStyleBehavior = isSmartStyleBehavior;

            // WORDSNET-25587 Added importing citations.
            mNodeImporter.ImportCitationSources();
        }

        /// <summary>
        /// Returns true, if document can be inserted at the current position of a document builder.
        /// </summary>
        private bool CanInsertDocument(Document document)
        {
            if (document.Sections.Count == 1)
                return true;

            NodeType nodeType = CurrentParagraph.ParentNode.NodeType;

            return (nodeType != NodeType.Cell &&
                    nodeType != NodeType.Shape &&
                    nodeType != NodeType.HeaderFooter &&
                    nodeType != NodeType.Footnote &&
                    nodeType != NodeType.Comment);
        }

        /// <summary>
        /// Inserts nodes from a source document into a block level node of a destination document.
        /// </summary>
        /// <returns>First node of source document inserted into destination document.</returns>
        private Node InsertIntoBlockLevelNode()
        {
            Section dstSection = CurrentParagraph.ParentSection;

            // WORDSNET-14073 Word has special logic when inserting into the last empty section.
            // Note, we must calculate it here while destination document is not yet modified.
            bool isDstSectionEmptyAndLast = (dstSection.IsLastSection && IsEmpty(dstSection));

            StructuredDocumentTag parentSdt = (StructuredDocumentTag)CurrentParagraph.GetAncestor(NodeType.StructuredDocumentTag);
            bool hasPlainSdtAncestor = (parentSdt != null) &&
                                NodeUtil.HasAncestor(parentSdt, NodeType.StructuredDocumentTag) &&
                                (parentSdt.SdtType == SdtType.PlainText);

            // Split current destination paragraph to insert source document between split parts.
            mParaAfter = CurrentParagraph;
            mParaBefore = SplitCurrentParagraph();

            foreach (Section node in SrcDoc.Sections)
            {
                Section importedSection = (Section)mNodeImporter.ImportNode(node, true, mInfoCollector);
                PreInsertFormatting();

                // When inserting into the nested SDT, Word ignores section breaks and inserts all the content
                // the way as if it is entirely resides in a single section (see Test13652NestedSdt()).
                if (mInfoCollector.IsLastSection || hasPlainSdtAncestor)
                {
                    // The last inserting section should be merged with destination section, so that there will
                    // be only one single merged section produced from those two sections. The way in which
                    // the sections will be merged depends on either destination section is last and empty
                    // and on the place where the source section will be put (i.e. Body or HeaderFooter).
                    if (isDstSectionEmptyAndLast && (mParaAfter.ParentStory.StoryType == StoryType.MainText))
                    {
                        // In case we insert LAST source section into a Body of LAST EMPTY destination section,
                        // we should insert imported section and merge destination section into it.
                        ReplaceWithImportedSection(importedSection, dstSection);
                    }
                    else
                    {
                        // Otherwise, we leave destination section and merge content of imported section into it.
                        mParaAfter.InsertPrevious(importedSection.Body.FirstChild, null);
                    }
                }
                else
                {
                    // If we are here, then the source document has multiple sections.
                    // In this case all source sections, except of very last section, should be inserted 'as is'.
                    DstDoc.InsertBefore(importedSection, dstSection);

                    // If this is the first inserting section, then we should merge content before
                    // split destination paragraph into the inserted section (up to inclusive 'paraBefore').
                    if (mInfoCollector.IsFirstSection)
                    {
                        // WORDSNET-13652 If we insert document with multiple sections into SDT,
                        // then we need to convert destination SDT to ranged one.
                        if (parentSdt != null)
                            StructuredDocumentTag.ConvertOuterSdtsToRanges(parentSdt);

                        // WORDSNET-23180 Word preserves SectionBreak of the destination section when importing first section.
                        if (importedSection.PageSetup.SectionStart != dstSection.PageSetup.SectionStart)
                            importedSection.SectPr.SectionStart = dstSection.SectPr.SectionStart;

                        importedSection.Body.InsertBefore(mParaBefore.ParentNode.FirstChild, mParaBefore.NextNode,
                            importedSection.Body.FirstChild);
                    }
                }
            }

            Node firstImportedNode = mParaBefore.NextNonAnnotationSibling;
            PostInsertFormatting();

            // WORDSNET-25587 Added importing citations.
            mNodeImporter.ImportCitationSources();

            return firstImportedNode;
        }

        /// <summary>
        /// Replaces destination section with imported one.
        /// </summary>
        /// <remarks>
        /// Preserves some formatting and content from the destination section.
        /// </remarks>
        private void ReplaceWithImportedSection(Section importedSection, Section dstSection)
        {
            DstDoc.AppendChild(importedSection);

            // Some formatting should be preserved from the destination section.
            foreach (int key in gNonReplacingAttrs)
            {
                // WORDSNET-23180 We shall not change SectionBreak if there is only one section.
                if ((key == SectAttr.SectionStart) && (SrcDoc.Sections.Count > 1))
                    continue;
                dstSection.SectPr.MoveTo(importedSection.SectPr, key);
            }

            // Preserve paraAfter and paraBefore around inserted content (they are actually the paragraph
            // that we are split to insert source content).
            importedSection.Body.AppendChild(mParaAfter);
            if (mInfoCollector.IsFirstSection)
                importedSection.Body.PrependChild(mParaBefore);

            dstSection.Remove();
        }

        /// <summary>
        /// Returns true, if a specified section can be treated as empty.
        /// </summary>
        private static bool IsEmpty(Section section)
        {
            if (!IsEmpty(section.Body))
                return false;

            // Word ignores Headers and Footers of the last section of a multi-section documents.
            if (section.IsLastSection && !section.IsFirstSection)
                return true;
            foreach (HeaderFooter node in section.HeadersFooters)
            {
                if (!IsEmpty((Story)node))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true, if a specified story is empty.
        /// </summary>
        private static bool IsEmpty(Story story)
        {
            foreach (Node node in story)
            {
                if (node.NodeType == NodeType.Paragraph)
                {
                    Paragraph para = (Paragraph)node;
                    if (!para.IsEmptyOrContainsOnlyCrossAnnotation)
                        return false;

                    return (story.StoryType == StoryType.MainText) ? para.IsEndOfSection : para.IsEndOfHeaderFooter;
                }

                if (!NodeUtil.IsCrossStructureAnnotation(node))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Inserts specified document into inline level SDT.
        /// </summary>
        /// <remarks>
        /// Insertion into inline SDT has special logic in Word:
        /// 1. Only runs from the source document body are inserted, headers and footers are ignored.
        /// 2. 'Paragraph Break' control char is inserted after each SDT in last cell of row in source document.
        /// 3. 'Tab' control char is inserted before each cell except very first in source document table if previously cell hasn't SDT.
        /// </remarks>
        /// <returns>First inserted run.</returns>
        private Node InsertIntoInlineLevelSdt()
        {
            Node nodeBefore = CurrentNode.PreviousSibling;

            foreach (Section section in SrcDoc.Sections)
            {
                // Word inserts extra run before very first section if AdjustSpacing option is enabled and always inserts it before all subsequent sections.
                if (!section.IsFirstSection || Options.AdjustSentenceAndWordSpacing)
                    InsertSpaceBeforeNode(CurrentNode);

                // Word inserts nodes only from the body. Nodes from the headers and footers should not be taken into account.
                InsertNodesWithFormatting(section.Body, CurrentNode.ParentNode, CurrentNode);
            }

            if (Options.AdjustSentenceAndWordSpacing)
            {
                // Word inserts extra space after inserted content if source document ends with SDT.
                if (SrcDoc.LastSection.Body.LastNonAnnotationChild.NodeType == NodeType.StructuredDocumentTag)
                    InsertSpaceBeforeNode(CurrentNode);
            }

            return (nodeBefore == null) ? CurrentNode.ParentNode.FirstChild : nodeBefore.NextSibling;
        }

        /// <summary>
        /// Splits current paragraph onto two parts.
        /// </summary>
        /// <remarks>
        /// This method splits paragraph at the cursor position and is a wrapper for
        /// <see cref="Aspose.Words.DocumentBuilder.InsertParagraphAsWord"/> method.
        /// </remarks>
        /// <returns>Paragraph, which is the first part of the split paragraph.</returns>
        private Paragraph SplitCurrentParagraph()
        {
            // Word moves bookmarks to the left part of the split paragraph.
            while ((CurrentNode != null) && (CurrentNode.NextSibling != null))
            {
                if ((CurrentNode.NodeType != NodeType.BookmarkStart) && (CurrentNode.NodeType != NodeType.BookmarkEnd))
                    break;

                DocumentBuilder.MoveTo(CurrentNode.NextSibling);
            }

            // WORDSNET-19667 Insert paragraph in Word manner.
            return DocumentBuilder.InsertParagraphAsWord();
        }

        /// <summary>
        /// Inserts all nodes from a source container into a destination container before a referencing node, applying
        /// specified formatting to the inserting nodes. Processes also all child composite nodes recursively.
        /// See <see cref="InsertIntoInlineLevelSdt"/> for more details.
        /// </summary>
        private void InsertNodesWithFormatting(CompositeNode srcContainer, CompositeNode dstContainer, Node dstRefNode)
        {
            foreach (Node node in srcContainer)
            {
                CompositeNode composite = node as CompositeNode;

                switch (node.NodeType)
                {
                    case NodeType.Run:
                    {
                        ProcessRun((Run)node, dstContainer, dstRefNode);
                        break;
                    }
                    case NodeType.Cell:
                    {
                        // Add 'Tab' into inline SDT before cell if this cell is not first in row
                        // and previous cell is not inside SDT.
                        Cell curCell = (Cell)node;
                        if (!curCell.IsFirstCell && !NodeUtil.HasAncestor(curCell.PreviousCell, NodeType.StructuredDocumentTag))
                        {
                            // Word applies formatting from the first found run inside source document,
                            // if it does not start from SDT.
                            RunPr runPr = (!mInfoCollector.IsFirstNodeSdt && (mInfoCollector.FirstRun != null))
                                ? mInfoCollector.FirstRun.RunPr.Clone()
                                : new RunPr();
                            Run newRun = new Run(DstDoc, ControlChar.Tab, runPr);
                            dstContainer.InsertBefore(newRun, dstRefNode);
                        }
                        break;
                    }
                    case NodeType.StructuredDocumentTag:
                    {
                        StructuredDocumentTag sdt = (StructuredDocumentTag)mNodeImporter.ImportNode(node, false);
                        sdt.SetLevel(MarkupLevel.Inline);

                        dstContainer.InsertBefore(sdt, dstRefNode);

                        InsertNodesWithFormatting(composite, sdt, null);
                        continue;
                    }
                    // WORDSNET-18731 Word allows shapes inside inline level SDTs, so added the case to process them.
                    // WORDSNET-20413 The comments are also allowed inside inline level SDTs.
                    case NodeType.Shape:
                    case NodeType.GroupShape:
                    case NodeType.CommentRangeStart:
                    case NodeType.CommentRangeEnd:
                    case NodeType.Comment:
                    case NodeType.FieldStart:
                    case NodeType.FieldSeparator:
                    case NodeType.FieldEnd:
                    case NodeType.BookmarkStart:
                    case NodeType.BookmarkEnd:
                    {
                        Node importedNode = mNodeImporter.ImportNode(node, true);
                        dstContainer.InsertBefore(importedNode, dstRefNode);
                        continue;
                    }
                    default:
                        // skipped now
                        break;
                }

                if (node.IsComposite)
                    InsertNodesWithFormatting(composite, dstContainer, dstRefNode);
            }
        }

        /// <summary>
        /// Adds a run before a specified node applying an appropriate formatting.
        /// </summary>
        /// <remarks>
        /// The formatting being applied depends on importing mode and concrete location of run inside a document.
        /// To mimic Word behavior it expands style into direct attributes for KeepSourceFormatting importing mode
        /// and preserves it for other importing modes.
        /// </remarks>
        private void ProcessRun(Run run, CompositeNode dstContainer, Node dstRefNode)
        {
            RunPr formatting;
            if (mNodeImporter.Context.ImportFormatMode == ImportFormatMode.KeepSourceFormatting)
            {
                // Word expands character style into direct attributes and removes style.
                RunPr runPr = run.RunPr;
                Style srcStyle = SrcDoc.Styles.FetchByIstd(runPr.Istd, StyleIndex.DefaultParagraphFont);
                formatting = (srcStyle.Istd != StyleIndex.DefaultParagraphFont)
                    ? srcStyle.RunPr.Clone()
                    : new RunPr();

                runPr.ExpandTo(formatting);

                formatting.Remove(FontAttr.Istd);
            }
            else
            {
                formatting = ((Run)mNodeImporter.ImportNode(run, false)).RunPr;
            }

            Run newRun = new Run(DstDoc, run.Text, formatting);
            dstContainer.InsertBefore(newRun, dstRefNode);

            // Word inserts paragraph break if this run is last node in last cell of row inside SDT.
            Cell cell = (Cell)run.GetAncestor(NodeType.Cell);
            if (cell != null)
            {
                Node lastRun = cell.GetChild(NodeType.Run, -1, true);
                if ((run == lastRun) &&
                    cell.IsLastCell && NodeUtil.HasAncestor(cell, NodeType.StructuredDocumentTag))
                {
                    newRun = new Run(DstDoc, ControlChar.ParagraphBreak, formatting);
                    dstContainer.InsertBefore(newRun, dstRefNode);
                }
            }
        }

        /// <summary>
        /// Applies various formatting to a content before insertion.
        /// </summary>
        private void PreInsertFormatting()
        {
            if (Options.MergePastedLists)
                MergePastedLists();

            // By default, NodeImporter imports paragraphs as if SmartStyleBehavior option is enabled.
            // If the option is disabled, then we need to process it additionally.
            // WORDSNET-22709 The option is not applicable if there is list defined in direct attributes of some paragraph
            // with the same ListDefId as in some style of this document.
            if (!Options.SmartStyleBehavior || mInfoCollector.HasStyleListInDirectAttrs)
            {
                foreach (Paragraph para in mInfoCollector.ImportedParagraphs)
                    ProcessNoSmartStyle(para);
            }
        }

        /// <summary>
        /// Merges pasted lists.
        /// </summary>
        /// <remarks>
        /// Processes imported paragraphs by merging source and destination lists in accordance with Word behavior.
        /// </remarks>
        private void MergePastedLists()
        {
            if (Options.MergePastedLists)
            {
                if (CanMergePastedLists)
                {
                    // If a first inserting node is a table,
                    // then Word merges lists only with the very first paragraph of that table.
                    if (mInfoCollector.IsFirstNodeTable)
                    {
                        Paragraph para = (Paragraph)mInfoCollector.ImportedParagraphs[0];
                        if (para.ParentTable == mInfoCollector.FirstNode)
                            ApplyList(para, mParaBefore);
                    }
                    else
                    {
                        foreach (Paragraph para in mInfoCollector.ImportedParagraphs)
                        {
                            // If content has list, then only this list must be merged,
                            // otherwise all imported paragraphs must be merged.
                            if (!para.IsInCell && (!mInfoCollector.HasLists || para.IsListItem))
                                ApplyList(para, mParaBefore);
                        }
                    }
                }

                // When source content ends with a block level SDT and inserting not at the end of a list item,
                // Word defines new list for the rest of the list items after inserted content.
                if (mInfoCollector.IsLastSection && mInfoCollector.IsLastNodeSdt && !IsInsertAtEnd)
                    ApplyListDefCopy(mParaAfter);
            }
        }

        /// <summary>
        /// Applies various formatting to inserted content.
        /// </summary>
        private void PostInsertFormatting()
        {
            if (mParaBefore == null)
                return;

            Node firstImportedNode = mParaBefore.NextNonAnnotationSibling;
            // WORDSNET-22103 If we append document, then there is no ParaAfter.
            // In this case the last imported node is just last node of the destination document.
            Node lastImportedNode = (mParaAfter != null) ? mParaAfter.PreviousNonAnnotationSibling : LastNonAnnotationDstChild;
            if ((lastImportedNode != null) && NodeUtil.IsCrossStructureAnnotation(lastImportedNode))
                lastImportedNode = lastImportedNode.PreviousNonAnnotationSibling;

            if ((firstImportedNode == null) || (lastImportedNode == null))
                return;

            // WORDSNET-24960 Actually, the behavior in Word slightly more complex.
            // It analyses the character before and after inserted content and behaves appropriately.
            // But for a moment (till real customers requests) let's leave it as is.
            if (Options.AdjustSentenceAndWordSpacing)
            {
                // Word inserts run with space character before inserted content if it does not start with a table.
                if (firstImportedNode.NodeType != NodeType.Table)
                    AddSpaceRun(mParaBefore, false);

                // Word inserts run with space character after inserted content if it ends with SDT.
                if (lastImportedNode.NodeType == NodeType.StructuredDocumentTag)
                    AddSpaceRun(mParaAfter, true);
            }

            Paragraph firstImportedPara = (firstImportedNode.NodeType == NodeType.Paragraph)
                ? (Paragraph)firstImportedNode
                : (Paragraph)firstImportedNode.NextPreOrderOfType(firstImportedNode.Document, NodeType.Paragraph);

            // Move annotations between the merged paragraphs.
            if (firstImportedPara != null)
            {
                // Block-level.
                // WORDSNET-26905 Move bookmark nodes only.
                if ((mParaBefore.NextSibling != null) && (mParaBefore.NextSibling is IBookmarkNode) &&
                    (mParaBefore.ParentNode == firstImportedNode.ParentNode))
                {
                    // WORDSNET-23299 Interrupt the insertion when SDT range node  occur among annotations.
                    firstImportedPara.InsertBefore(mParaBefore.NextSibling, firstImportedNode, firstImportedPara.FirstChild, true);
                }
                // Inline-level.
                while ((mParaBefore.LastChild != null) && NodeUtil.IsCrossStructureAnnotation(mParaBefore.LastChild))
                    firstImportedPara.PrependChild(mParaBefore.LastChild);
            }

            // Word concatenates first nodes of srcDoc and dstDoc if they are both paragraphs.
            if (firstImportedNode.NodeType == NodeType.Paragraph)
            {
                if (mParaBefore.HasChildNodes)
                    ConcatenateParagraphs(mParaBefore, firstImportedPara);
            }
            else if ((firstImportedPara != null) && (firstImportedNode.NodeType != NodeType.Table))
            {
                // Word applies formatting to the left part of split paragraph from the first found paragraph in srcDoc
                // if first node of srcDoc is not a paragraph or a table.
                mParaBefore.ParaPr = firstImportedPara.ParaPr.Clone();
            }

            // Delete left part of split paragraph if it is empty.
            if (!mParaBefore.HasChildNodes)
                mParaBefore.Remove();

            // WORDSNET-25505 Implemented a new option to insert document as inline for new method InsertDocumentInline().
            if (Options.InlineMode && (mParaAfter != null))
            {
                Paragraph dstLastPara = mParaAfter.PreviousSibling as Paragraph;
                if (dstLastPara != null)
                {
                    ConcatenateParagraphs(dstLastPara, mParaAfter);
                    if (!dstLastPara.HasChildNodes)
                        dstLastPara.Remove();
                }
            }
        }

        /// <summary>
        /// Adds run with one space character to a specified paragraph.
        /// </summary>
        /// <param name="paragraph">Paragraph to add run with space character.</param>
        /// <param name="isAtBeginning">Specify where to insert new run.</param>
        private static void AddSpaceRun(Paragraph paragraph, bool isAtBeginning)
        {
            if (paragraph == null)
                return;

            Run refRun = isAtBeginning ? paragraph.FirstRun : paragraph.GetLastRun(true);

            if ((refRun == null) || (refRun.Text.Length == 0))
                return;

            char edgeChar;
            if (isAtBeginning)
            {
                edgeChar = refRun.Text[0];
                // Word inserts at the beginning only if first character in the paragraph is a letter.
                if (!StringUtil.IsLetter(edgeChar))
                    return;
            }
            else
            {
                edgeChar = refRun.Text[refRun.Text.Length - 1];
            }

            // Don't add space character, if it already exists.
            if (char.IsWhiteSpace(edgeChar))
                return;

            Run newRun = new Run(refRun.Document, " ", refRun.RunPr.Clone());

            if (isAtBeginning)
                paragraph.PrependChild(newRun);
            else
                paragraph.AppendChild(newRun);
        }

        /// <summary>
        /// Concatenates last paragraph before imported content with the first imported paragraph.
        /// </summary>
        /// <remarks>
        /// When Word inserts into not a very beginning of the paragraph, so that the left split part of this paragraph
        /// will contain some text, it preserves the original style of the imported paragraph and an original formatting
        /// of the both paragraphs being concatenated.
        /// </remarks>
        private void ConcatenateParagraphs(Paragraph paraBeforeImportedContent, Paragraph firstImportedPara)
        {
            // WORDSNET-19532 A concatenated paragraph should have a style name,
            // that first imported paragraph had in a source document.
            Style styleFirstImportedParaOriginal = SrcDoc.FirstSection.Body.FirstParagraph.ParagraphStyle;

            // The style that should be set to the concatenated paragraph.
            Style dstStyle = DstDoc.Styles.FindLocaleIndependentMatch(styleFirstImportedParaOriginal);

            RunPr dstStyleRunPrExp = dstStyle.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
            ParaPr dstStyleParaPrExp = dstStyle.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);

            // The imported paragraph has all properties expanded into direct attributes and collapsed
            // over the source Normal style during the importing process. But now, if we want to apply
            // a some another (not a Normal) style to the imported paragraph, then we need to recalculate
            // its attributes in accordance with the formatting of the style being applied.
            if (dstStyle.StyleIdentifier != StyleIdentifier.Normal)
            {
                foreach (Node importedNode in firstImportedPara)
                {
                    Inline importedInline = importedNode as Inline;
                    if (importedInline != null)
                    {
                        importedInline.RunPr = importedInline.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
                        importedInline.RunPr.Collapse(dstStyleRunPrExp);
                    }
                }
            }

            // Concatenate paragraphs by moving a content of the last paragraph
            // before imported content into the first imported paragraph.
            while (paraBeforeImportedContent.LastChild != null)
            {
                // If style will be changed, then we need to recalculate direct
                // attributes of paragraph content to preserve its formatting.
                if (paraBeforeImportedContent.ParagraphStyle.Name != dstStyle.Name)
                {
                    Inline inline = paraBeforeImportedContent.LastChild as Inline;
                    if (inline != null)
                    {
                        inline.RunPr = inline.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
                        inline.RunPr.Collapse(dstStyleRunPrExp);
                    }
                }

                firstImportedPara.PrependChild(paraBeforeImportedContent.LastChild);
            }

            // Recalculate formatting of the resulted paragraph in accordance
            // with a formatting of the new style being applied.
            RunPr runPrExp = firstImportedPara.GetExpandedParagraphBreakRunPr(RunPrExpandFlags.DocumentDefaults);
            runPrExp.Collapse(dstStyleRunPrExp);
            firstImportedPara.ParagraphBreakRunPr = runPrExp;

            ParaPr paraPrExp = firstImportedPara.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);
            paraPrExp.Collapse(dstStyleParaPrExp);
            firstImportedPara.ParaPr = paraPrExp;

            // At last, apply the style.
            firstImportedPara.ParaPr[ParaAttr.Istd] = dstStyle.Istd;
        }

        /// <summary>
        /// Inserts new run with space char before specified node.
        /// </summary>
        private static void InsertSpaceBeforeNode(Node node)
        {
            if ((node != null) && (node.ParentNode != null))
                node.ParentNode.InsertBefore(new Run(node.Document, " "), node);
        }

        /// <summary>
        /// Returns true if inserted paragraphs can be merged with surrounding lists.
        /// </summary>
        private bool CanMergePastedLists
        {
            get
            {
                // Exit, if we insert into paragraph that is not a list.
                if ((mParaBefore == null) || !mParaBefore.IsListItem)
                    return false;

                // When 'AdjustSentenceAndWordSpacing' option is set to true and inserted content ends with SDT,
                // Word does not merge lists.
                // Please see TestInsertDocumentOptions.TestMergeListsLastSdt() for example.
                if (Options.AdjustSentenceAndWordSpacing && mInfoCollector.IsLastNodeSdt)
                    return false;

                // Word merges lists only when inserts into the beginning of paragraph.
                // The exception to this rule is when first inserted node is a table.
                // In this case Word merges very first paragraph of that table.
                // Please see TestInsertDocumentOptions.TestMergeListsParagraph() and
                // TestInsertDocumentOptions.TestMergeListsTable() for example.
                if (!IsInsertAtStart && !mInfoCollector.IsFirstNodeTable)
                    return false;

                // At last, there are three different cases depending on lists existence inside inserting content:
                // 1) If source content has no any lists, then Word applies numbering to the all inserted paragraphs.
                if (!mInfoCollector.HasLists)
                    return true;

                // 2) If there is exactly one list inside source content and inserted content either starts or ends with this
                // list, then Word merges lists.
                if (mInfoCollector.HasSingleList)
                {
                    Paragraph para = mInfoCollector.FirstNode as Paragraph;
                    if ((para != null) && para.IsListItem)
                        return true;

                    para = mInfoCollector.LastNode as Paragraph;
                    if ((para != null) && para.IsListItem)
                        return true;
                }

                // 3) In all other cases Word does not merge lists.
                return false;
            }
        }

        /// <summary>
        /// Applies list to the paragraph from the referred <paramref name="refPara"/>.
        /// </summary>
        private static void ApplyList(Paragraph para, Paragraph refPara)
        {
            int refListId = (int)((IParaAttrSource)refPara).FetchParaAttr(ParaAttr.ListId);
            para.ParaPr[ParaAttr.ListId] = refListId;

            // Also Word resets Normal style to a ListParagraph by default.
            // See TestTableDocStartsWithPara.TestStyleInsertedParagraphs for example.
            if (para.ParagraphStyle.StyleIdentifier == StyleIdentifier.Normal)
                para.ParaPr[ParaAttr.Istd] = refPara.Document.Styles.FetchBySti(StyleIdentifier.ListParagraph).Istd;
        }

        /// <summary>
        /// Creates a copy of list definition of the specified paragraph and applies it to the all paragraphs that have same
        /// list id as specified one, starting from this paragraph and to the end of a document.
        /// </summary>
        private static void ApplyListDefCopy(Paragraph fromPara)
        {
            int listId = (int)((IParaAttrSource)fromPara).FetchParaAttr(ParaAttr.ListId);
            if (listId == 0)
                return;

            DocumentBase doc = fromPara.Document;
            List oldList = doc.Lists.GetListByListId(listId);
            List newList = doc.Lists.AddCopy(oldList);

            Paragraph curPara = fromPara;
            while (curPara != null)
            {
                int curListId = (int)curPara.ParaPr.FetchAttr(ParaAttr.ListId);

                if (curListId == oldList.ListId)
                    curPara.ParaPr.SetAttr(ParaAttr.ListId, newList.ListId);

                curPara = NodeUtil.FindNextParagraph(curPara);
            }
        }

        /// <summary>
        /// Applies formatting to a specified paragraph as if it were imported
        /// with the <see cref="ImportFormatOptions.SmartStyleBehavior"/> option disabled.
        /// </summary>
        /// <remarks>
        /// When SmartStyleBehavior option is disabled, Word expands source style of the specified
        /// paragraph into direct attributes of this paragraph, if style has numbering.
        /// The existing direct attributes of paragraph are not overridden.
        /// </remarks>
        private void ProcessNoSmartStyle(Paragraph paragraph)
        {
            Style srcStyle = GetSourceStyle(paragraph);
            if (srcStyle == null)
                return;

            ParaPr dstParaPr = paragraph.ParaPr;

            int srcStyleListId = (int)((IParaAttrSource)srcStyle).FetchParaAttr(ParaAttr.ListId);
            // If style is not numbered, then exit.
            // WORDSNET-22149 Also do not expand source style, if paragraph is explicitly not a list item to mimic Word.
            if ((srcStyleListId == 0) || dstParaPr.IsExplicitlyNotListItem)
                return;

            // Otherwise, import source style list to apply it to the destination
            // paragraph, if there is no direct list applied yet (i.e. expand listId no-override).
            if (dstParaPr.ListId == 0)
                dstParaPr.ListId = DstDoc.Lists.ImportList(mNodeImporter.Context, srcStyleListId);

            int dstListId = (int)paragraph.FetchParaAttr(ParaAttr.ListId, RevisionsView.Original);
            // This is listId, which paragraph has in the source document.
            int srcListId = mNodeImporter.Context.ImportedListIds.GetKey(dstListId);
            if (IntToIntDictionary.IsNullSubstitute(srcListId))
                srcListId = srcStyleListId;

            // This is resolved list level, which paragraph has in both: source and destination document.
            int listLevel = (int)paragraph.FetchParaAttr(ParaAttr.ListLevel, RevisionsView.Original);

            // WORDSNET-27484 Check, if there is no list level set in numbered style of the source document.
            // If so, then mimic Word and set it from the direct (resolved) list level of the paragraph.
            // Note, resolved list level value is the same in source and destination documents.
            bool hasSrcStyleListLevel = srcStyle.ParaPr.Contains(ParaAttr.ListLevel);
            if (!hasSrcStyleListLevel)
                srcStyle.ParaPr[ParaAttr.ListLevel] = listLevel;

            // Expand source style attributes.
            ParaPr srcStyleExpandedParaPr = srcStyle.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);

            // Restore original list level of the source style.
            if (!hasSrcStyleListLevel)
            {
                srcStyle.ParaPr.Remove(ParaAttr.ListLevel);
                // Also remove it from the expanded collection.
                srcStyleExpandedParaPr.Remove(ParaAttr.ListLevel);
            }

            // Now check, if resolved source listId is not the same, as the source style listId of the paragraph,
            // then it means the paragraph has direct listId attribute. Word applies its formatting too.
            if (srcListId != srcStyleListId)
            {
                List srcList = SrcDoc.Lists.GetListByListId(srcListId);
                ListLevel srcDirectListLevel = srcList.GetListLevelOverrideAware(listLevel);
                srcDirectListLevel.ParaPr.ExpandTo(srcStyleExpandedParaPr);
            }

            // Preserve direct paragraph attributes.
            dstParaPr.ExpandTo(srcStyleExpandedParaPr);

            // WORDSNET-22709 Word also collapses it over inherited attributes in destination document.
            // WORDSNET-23909 Don't collapse direct formatting in KeepDifferentStyles mode (by design).
            int[] keysToIgnore = (mNodeImporter.Context.ImportFormatMode == ImportFormatMode.KeepDifferentStyles)
                ? dstParaPr.GetKeys()
                : new[] { ParaAttr.Istd };
            ParaPr dstInheritedParaPr = paragraph.GetExpandedParaPr(ParaPrExpandFlags.NoDirectFormatting);
            srcStyleExpandedParaPr.Collapse(dstInheritedParaPr, keysToIgnore);

            paragraph.ParaPr = srcStyleExpandedParaPr;
        }

        /// <summary>
        /// Returns style of the specified imported paragraph that it had in source document.
        /// </summary>
        private Style GetSourceStyle(Paragraph importedParagraph)
        {
            Style dstStyle = importedParagraph.ParagraphStyle;

            StyleCollection srcStyles = SrcDoc.Styles;

            int istdSrcStyle = mNodeImporter.Context.ImportedIstds.TryGetValueIndirect(dstStyle.Istd);
            Style srcStyle = (istdSrcStyle != int.MinValue)
                ? srcStyles.GetByIstd(istdSrcStyle, false)
                : srcStyles.FindLocaleIndependentMatch(dstStyle);

            return srcStyle;
        }

        /// <summary>
        /// Gets current paragraph in destination document.
        /// </summary>
        private Paragraph CurrentParagraph
        {
            get { return DocumentBuilder.CurrentParagraph; }
        }

        /// <summary>
        /// Gets current node in destination document.
        /// </summary>
        private Node CurrentNode
        {
            get { return DocumentBuilder.CurrentNode; }
        }

        /// <summary>
        /// Gets a source document.
        /// </summary>
        private Document SrcDoc
        {
            get { return (Document)mNodeImporter.Context.SrcDoc; }
        }

        /// <summary>
        /// Gets a destination document.
        /// </summary>
        private Document DstDoc
        {
            get { return mDstDoc; }
        }

        /// <summary>
        /// Gets a DocumentBuilder of the destination document.
        /// </summary>
        private DocumentBuilder DocumentBuilder
        {
            get
            {
                if (mDocumentBuilder == null)
                    mDocumentBuilder = new DocumentBuilder(mDstDoc);

                return mDocumentBuilder;
            }
        }

        /// <summary>
        /// Gets an importing options.
        /// </summary>
        private ImportFormatOptions Options
        {
            get { return mNodeImporter.Context.ImportFormatOptions; }
        }

        /// <summary>
        /// Returns true, if inserting at start of destination paragraph.
        /// </summary>
        private bool IsInsertAtStart
        {
            get { return (mParaBefore == null) || (mParaAfter == null) || mParaBefore.IsEmptyOrContainsOnlyCrossAnnotation; }
        }

        /// <summary>
        /// Returns true, if inserting at end of destination paragraph.
        /// </summary>
        private bool IsInsertAtEnd
        {
            get { return (mParaAfter == null) || mParaAfter.IsEmptyOrContainsOnlyCrossAnnotation; }
        }

        /// <summary>
        /// Gets last non-annotation child of destination document.
        /// </summary>
        private Node LastNonAnnotationDstChild
        {
            get
            {
                return ((DstDoc.LastSection != null) && (DstDoc.LastSection.Body != null))
                    ? DstDoc.LastSection.Body.LastNonAnnotationChild
                    : null;
            }
        }

        /// <summary>
        /// Gets last paragraph of destination document.
        /// </summary>
        private Paragraph LastDstParagraph
        {
            get
            {
                return ((DstDoc.LastSection != null) && (DstDoc.LastSection.Body != null))
                    ? DstDoc.LastSection.Body.LastParagraph
                    : null;
            }
        }

        /// <summary>
        /// Document builder of a destination document.
        /// </summary>
        private DocumentBuilder mDocumentBuilder;

        /// <summary>
        /// Destination document.
        /// </summary>
        private readonly Document mDstDoc;

        /// <summary>
        /// The node importer to import nodes from source to destination document.
        /// </summary>
        private NodeImporter mNodeImporter;

        /// <summary>
        /// Clone listener that collects all necessary information about inserted content upon importing its particular nodes.
        /// </summary>
        private readonly ImportInfoCollector mInfoCollector = new ImportInfoCollector();

        /// <summary>
        /// Paragraph before inserted content.
        /// </summary>
        private Paragraph mParaBefore;
        /// <summary>
        /// Paragraph after inserted content.
        /// </summary>
        private Paragraph mParaAfter;

        /// <summary>
        /// The collection of section attributes that should be preserved when replacing last empty section with imported one.
        /// </summary>
        private static readonly int[] gNonReplacingAttrs = new[]
        {
            // See TestJira16127.
            SectAttr.SectionStart,

            // The following attrs (related to TextColumnCollection) actually should not be preserved.
            // But there is incorrect WORDSNET-19014, so let's leave it at that for now.
            SectAttr.ColumnsCount, SectAttr.ColumnsEvenlySpaced, SectAttr.ColumnsSpacing, SectAttr.ColumnsLineBetween, SectAttr.Columns
        };
    }
}
