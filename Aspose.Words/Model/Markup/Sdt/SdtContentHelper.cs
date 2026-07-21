// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/17/2011 by Denis Darkin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Helps to insert content inside Sdt node. Covers two major cases:
    /// -1. Creation and insertion of default content upon creation.
    ///   MS Word will not show sdt if default content is not specified, so AW has to populate default content upon Sdt creation.
    /// -2. Insertion of bound content from custom xml parts. Databinding use case is not yet supported, but will be using this class facilities, when databinding supported.
    /// </summary>
    internal static class SdtContentHelper
    {
        /// <summary>
        /// Replaces content of Sdt with placeholder.
        /// </summary>
        internal static void ReplaceContentWithPlaceholder(StructuredDocumentTag sdt)
        {
            // WORDSNET-9881 Quick fix to remove regression introduced by WORDSNET-9337
            if (sdt.Level != MarkupLevel.Inline)
                return;

            // WORDSNET-12052 MS Word does not replace sdt content if 'showingPlcHdr' element set to true.
            if ((sdt.Placeholder != null) && !sdt.IsShowingPlaceholderText)
            {
                Body placeholderBody = (Body)sdt.Placeholder.GetChild(NodeType.Body, 0, true);

                sdt.RemoveAllChildren();

                foreach (Node node in placeholderBody.GetChildNodes(NodeType.Any, false))
                    InsertContent(sdt, node, true);

                // Sdt content has been changed, it is necessary to set IsShowingPlaceholderText flag.
                sdt.IsShowingPlaceholderText = true;
            }
        }

        /// <summary>
        /// - Inserts default content from placeholders into sdt,
        /// or
        /// - Builds default content and inserts it into Sdt node if sdt has no applicable placeholders.
        /// </summary>
        internal static void InsertDefaultContent(StructuredDocumentTag sdt, bool safe)
        {
            if (sdt.Placeholder != null)
            {
                InsertPlaceholderContent(sdt);
            }
            else
            {
                // picture does not have a placeholder, but has default dml content.
                switch (sdt.SdtType)
                {
                    case SdtType.Picture:
                        InsertPictureDefaultContent(sdt);
                        break;
                    case SdtType.Checkbox:
                        UpdateCheckboxContent(sdt);
                        break;
                    case SdtType.RepeatingSection:
                    case SdtType.RepeatingSectionItem:
                        // No default content for repeating section and item.
                        break;
                    case SdtType.Citation:
                        // No default content for citation.
                        break;
                    case SdtType.PlainText:
                        InsertContent(sdt, new Run(sdt.Document, DefaultContentIfNoPlaceholder), true);
                        break;
                    default:
                        if (!safe)
                            throw new InvalidOperationException("Unknown sdt type. Please report exception.");
                        break;
                }
            }

            // Sdt content has been changed, it is necessary to set IsShowingPlaceholderText flag.
            if (!sdt.IsShowingPlaceholderText && (sdt.Placeholder != null))
                sdt.IsShowingPlaceholderText = true;
        }

        /// <summary>
        /// Inserts content inside sdt. Verifies that Markup level of sdt is compatible with level of content being inserted.
        /// Performs some minor resiliencies.
        /// </summary>
        internal static Node InsertContent(StructuredDocumentTag sdt, Node content, bool clearChildNodes)
        {
            Debug.Assert((sdt != null) && (content != null));

            if (content.NodeType == NodeType.Document)
                return InsertDocument(sdt, (Document)content);

            Node referenceNode = null;

            // andrnosk: WORDSNET-7009 We need to apply SDT RunPr to SDT content to preserve formatting.
            if ((content.NodeType == NodeType.Run) &&
                (sdt.SdtType != SdtType.Checkbox)) // DD: Checkboxes do not seem to have RunPr in their SDT props.
            {
                ((Run)content).RunPr = sdt.ContentsRunPr.Clone();
            }

            // andrnosk: WORDSNET-7273 We have to remove child nodes the same type as content node only,
            // to preserve parent nodes formatting.
            if (clearChildNodes)
            {
                NodeCollection childNodes = sdt.GetChildNodes(content.NodeType, true);

                // WORDSNET-21562 We need to insert updated sdt content to the proper position. If referenceNode is null
                // it will be added to the end of the sdt child nodes collection (default behavior).
                if (childNodes.Count != 0)
                    referenceNode = childNodes[childNodes.Count - 1].NextSibling;

                childNodes.Clear();
            }

            Node newNode;
            switch (sdt.Level)
            {
                case MarkupLevel.Inline:
                    newNode = InsertInlineContent(sdt, content, referenceNode);
                    break;
                case MarkupLevel.Block:
                    newNode = sdt.Document.ImportNode(content, true);
                    InsertBlockContent(sdt, newNode);
                    break;
                case MarkupLevel.Cell:
                case MarkupLevel.Row:
                    newNode = sdt.Document.ImportNode(content, true);
                    InsertCellRowContent(sdt, newNode);
                    break;
                default:
                    throw new InvalidOperationException("Unknown markup level.");
            }

            if (sdt.IsShowingPlaceholderText && (sdt.Placeholder == null))
                sdt.IsShowingPlaceholderText = false;

            return newNode;
        }

        /// <summary>
        /// Cleans existing Checkbox content and inserts new one based on the value of Checked property.
        /// </summary>
        internal static void UpdateCheckboxContent(StructuredDocumentTag sdt)
        {
            SdtCheckBox checkBox = (SdtCheckBox)sdt.ControlProperties;

            if (!StringUtil.HasChars(checkBox.CheckedStateInfo.FontName))
                checkBox.CheckedStateInfo = SdtCheckBoxStateInfo.DefaultCheckedStateInfo;

            if (!StringUtil.HasChars(checkBox.UncheckedStateInfo.FontName))
                checkBox.UncheckedStateInfo = SdtCheckBoxStateInfo.DefaultUncheckedStateInfo;

            Run curRun = (Run) sdt.GetChild(NodeType.Run, 0, true);
            SdtCheckBoxStateInfo sdtCheckBoxStateInfo =
                (checkBox.Checked) ? checkBox.CheckedStateInfo : checkBox.UncheckedStateInfo;
            string characterText = ((char)sdtCheckBoxStateInfo.CharacterCode).ToString();
            string fontName = sdtCheckBoxStateInfo.FontName;

            if (curRun != null)
            {
                curRun.Text = characterText;
                curRun.RunPr.Name = fontName;
            }
            else
            {
                Run r = new Run(sdt.Document, characterText);
                RunPr rPr = r.RunPr;
                rPr.Name = fontName;
                InsertContent(sdt, r, true);
            }
        }

        /// <summary>
        /// Replaces content of CustomXML/Core properties if SDT is data bound.
        /// </summary>
        internal static bool ReplaceDataBoundContent(StructuredDocumentTag sdt, Regex pattern, string replacement)
        {
            string xmlValue = sdt.XmlMapping.GetValue();

            if (StringUtil.HasChars(xmlValue))
            {
                string result = pattern.Replace(xmlValue, replacement);
                if (result != xmlValue)
                {
                    sdt.XmlMapping.SetValue(result);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns Sdt display value according to the current sdt datetime display format.
        /// </summary>
        internal static string GetSdtDateDisplayValue(StructuredDocumentTag sdt, string datetime, XmlMappingContext ctx)
        {
            string fieldUpdateCultureName = LocaleConverter.LocaleToDocxTag(sdt.DateDisplayLocale);
            bool needSetLocation = StringUtil.HasChars(fieldUpdateCultureName);

            try
            {
                if (needSetLocation)
                {
                    SystemPal.SaveCulture();
                    SystemPal.SetCulture(fieldUpdateCultureName);
                }

                string dateDisplayFormat = ConvertFormatString(sdt.DateDisplayFormat);

                // Use value as date only if it's in strict Xml dateTime format otherwise render it as is.
                DateTime dateTime = FormatterPal.XmlToDateTimeExact(datetime);

                return dateTime == DateTime.MinValue
                        ? datetime
                        : TimeZoneInfo.ConvertTime(dateTime, ctx.CustomTimeZone).ToString(dateDisplayFormat);
            }
            finally
            {
                if (needSetLocation)
                    SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Clears contents of the specified structured document tag and displays a placeholder if it is defined.
        /// </summary>
        internal static void Clear(StructuredDocumentTag sdt)
        {
            // Removing contents of an SDT may cause that, for example, it will not be removed after accepting
            // revisions. Do nothing at this case.
            if (RevisionUtil.HasRevisions(sdt))
                return;

            SdtType sdtType = sdt.SdtType;

            // Do nothing for some SDT types if there is no placeholder.
            if (!IsCleanable(sdt))
                return;

            // Clear XML node only for simple type SDTs.
            if (!sdt.XmlMapping.IsEmpty && !sdt.IsRepeatingSection && !sdt.IsRepeatingSectionItem)
            {
                try
                {
                    sdt.XmlMapping.SetValue(null);
                }
                catch
                {
                    // Catch exceptions since mapping may be incorrect.
                }
            }

            if (sdtType == SdtType.ComboBox || sdtType == SdtType.DropDownList)
            {
                ((SdtDropDownListBase)sdt.ControlProperties).ListItems.SelectedValue = null;
            }

            switch (sdt.Level)
            {
                case MarkupLevel.Row:
                {
                    // Placeholder of a row-level SDT works only on a single-column rows. If there are several cells,
                    // MS Word raises an error on editing their contents.
                    NodeCollection cells = sdt.GetChildNodes(NodeType.Cell, true);
                    if (cells.Count == 1)
                        ClearCell((Cell)cells[0], sdt); // placeholder is inserted into a cell
                    else
                        return; // skip setting IsShowingPlaceholderText

                    break;
                }
                case MarkupLevel.Cell:
                {
                    Cell cell = (Cell)sdt.GetChild(NodeType.Cell, 0, true);
                    if (cell != null)
                        ClearCell(cell, sdt);

                    break;
                }
                default:
                {
                    Paragraph formatSourceParagraph = GetLastParagraph(sdt);

                    sdt.RemoveAllChildren();
                    InsertDefaultContent(sdt, true);

                    if (sdt.Count == 0)
                    {
                        InsertContentForEmptySdt(
                            sdt,
                            sdt.ContentsRunPr,
                            sdt.Level == MarkupLevel.Block,
                            sdt.EndCharacterRunPr);
                    }

                    if (formatSourceParagraph != null)
                    {
                        Paragraph paragraph = GetLastParagraph(sdt);
                        if (paragraph != null)
                        {
                            paragraph.ParaPr = formatSourceParagraph.ParaPr.Clone();
                            paragraph.ParagraphBreakRunPr = formatSourceParagraph.ParagraphBreakRunPr.Clone();
                        }
                    }

                    break;
                }
            }

            sdt.IsShowingPlaceholderText = true;
        }

        private static bool IsCleanable(StructuredDocumentTag sdt)
        {
            if (sdt.Placeholder != null)
                return true;

            if (sdt.IsRepeatingSection)
                return false;

            if (sdt.IsRepeatingSectionItem)
                return false;

            if (sdt.Level == MarkupLevel.Row)
                return false;

            switch (sdt.SdtType)
            {
                case SdtType.Checkbox:
                case SdtType.Citation:
                case SdtType.Group:
                case SdtType.EntityPicker:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Inserts five spaces as contents of the specified node. MS Word inserts such contents on clearing an SDT
        /// if it has no placeholder.
        /// </summary>
        internal static void InsertContentForEmptySdt(CompositeNode parentNode, RunPr runPr, bool addParagraph,
            RunPr breakRunPr)
        {
            CompositeNode runParent = parentNode;
            if (addParagraph)
            {
                runParent = new Paragraph(parentNode.Document);
                ((Paragraph)runParent).ParagraphBreakRunPr = breakRunPr.Clone();
                parentNode.AppendChild(runParent);
            }

            // If a structured document tag has no placeholder, MS Word inserts five spaces on clearing its contents
            // to be able to select the SDT in the document.
            Run run = new Run(parentNode.Document, DefaultContentIfNoPlaceholder);
            run.RunPr = runPr.Clone();
            runParent.AppendChild(run);
        }

        internal static void SetDisplayText(StructuredDocumentTag sdt, string value)
        {
            Run r = new Run(sdt.FetchDocument(), ResolveLineBreaks(value));
            InsertContent(sdt, r, true);
        }

        /// <summary>
        /// Returns true, if a specified paragraph can be removed from bound document while updating SDT.
        /// </summary>
        /// <remarks>
        /// When updating SDT from bound document we mimic Word behavior and remove last empty paragraph
        /// that also meets some other requirements.
        /// </remarks>
        internal static bool CanRemove(Paragraph paragraph)
        {
            return CanOverrideAttrs(paragraph) && paragraph.IsEmptyOrContainsOnlyCrossAnnotation;
        }

        /// <summary>
        /// Returns true, if attributes of a specified paragraph can be overriden with attributes of reference paragraph
        /// while updating SDT with bound document.
        /// </summary>
        /// <remarks>
        /// When updating SDT from bound document we mimic Word behavior by overriding attributes of last paragraph
        /// in bound document with attributes of reference node before which the bound document is inserted.
        /// </remarks>
        private static bool CanOverrideAttrs(Paragraph paragraph)
        {
            if (paragraph == null)
                return false;

            if (paragraph.IsListItemOriginal || paragraph.IsListItemFinal)
                return false;

            return true;
        }

        /// <summary>
        /// Inserts document inside sdt.
        /// </summary>
        private static Node InsertDocument(StructuredDocumentTag sdt, Document doc)
        {
            Debug.Assert((sdt != null) && (doc != null));

            if (sdt.Level == MarkupLevel.Row)
                return InsertDocumentAtRowLevel(sdt, doc);

            Paragraph lastPara = doc.LastSection.Body.LastParagraph;
            bool canOverrideAttrs = CanOverrideAttrs(lastPara);

            // AM. Some of the below comments may be not relevant due to logic changes.
            // WORDSNET-21636 Word deletes the last paragraph if it's empty, so do we.
            // WORDSNET-23213 Word treats numbered paragraphs as non-empty.
            // WORDSNET-23284 Word also doesn't remove an empty paragraph when it has some built-in style.

            // WORDSNET-22815 Microsoft has changed XmlMapping logic and now last paragraph is not deleted except
            // the case with empty document. We need to insert placeholder, that's why paragraph should be removed.
            if (IsDocumentEmpty(doc))
                lastPara.Remove();

            // WORDSNET-25442 Move out SDT in case of updating inline SDT from multiline document.
            bool movedOut = false;
            if ((sdt.Level == MarkupLevel.Inline) && (sdt.SdtType == SdtType.RichText) && IsMultilineDocument(doc))
            {
                sdt.ConvertToBlock();
                movedOut = true;
            }
            else
            {
                // WORDSNET-26027 Preserve last SDT paragraph if SDT is the last child of story.
                // There is some inconsistency in Word behavior, see related tests.
#if CPLUSPLUS
                Story parentStory = sdt.GetAncestorOf<Story>();
#else
                Story parentStory = (Story)sdt.GetAncestor(typeof(Story));
#endif
                Paragraph lastSdtPara = sdt.LastChild as Paragraph;
                if ((lastSdtPara != null) && (sdt == parentStory.LastChild))
                {
                    parentStory.InsertAfter(lastSdtPara, sdt);
                    lastSdtPara.RemoveAllChildren();
                }
            }

            // We need node inside SDT to insert document before.
            Node refNode = CreateReferenceNode(sdt);

            // If referenceNode is null, nothing to do.
            if (refNode == null)
                return null;

            DocumentBuilder builder = new DocumentBuilder(sdt.FetchDocument());
            builder.MoveTo(refNode);

            ImportFormatOptions importOptions = new ImportFormatOptions
            {
                // WORDSNET-23889 Use "UseExistingLists" to avoid creating new lists each time an SDT is updated.
                UseExistingLists = true,
                RenameDuplicateBookmarks = false
            };

            // WORDSNET-27335 Update target document annotation id counter before XML mapping update.
            builder.Document.UpdateAnnotationId();

            Node newNode = builder.InsertDocument(doc, ImportFormatMode.UseDestinationStyles, importOptions);

            // WORDSNET-23284 Word overrides attributes of last paragraph only when this paragraph is not deleted,
            // is not numbered and its style is not built-in.
            if (IsCellOrBlockLevel(sdt.Level) && canOverrideAttrs && !lastPara.IsRemoved)
            {
                // The SDT content now consists of 2 parts: the child nodes from the SDT xml and the referenceNode.
                // If the SDT is at the cell or block level, then the referenceNode is a Paragraph.
                // We need to set the referenceNode attributes to the last paragraph of the inserted nodes.
                Paragraph para = refNode.PreviousSiblingOfType(NodeType.Paragraph) as Paragraph;
                if (para != null)
                {
                    Paragraph refPara = (Paragraph)refNode;

                    // WORDSNET-21636 Copy formatting only if there is at least one property. In other words, we do should
                    // not reset formatting. Setting empty ParaPr actually is the same as remove all formatting.
                    // WORDSNET-23284 Actually, there seems more complex behavior in Word. Something similar to
                    // importing document with KeepSourceFormatting mode. But at the moment, we just copy attributes.
                    if (movedOut)
                        CopyProperties(refPara, para);

                    // WORDSNET-21436 Copying ParaId and TextId.
                    CopyIdentifiers(refPara, para);
                }
            }

            // WORDSNET-22091 Keep the referenceNode if the previous node is a table.
            if (!(refNode.PrevNode is Table))
            {
                // WORDSNET-23284 Word has special logic when removing reference node in a cell-level SDT.
                if (sdt.Level == MarkupLevel.Cell)
                {
                    if (!lastPara.IsRemoved && canOverrideAttrs)
                        refNode.Remove();
                }
                else
                {
                    if (IsOnlyParagraphInSection(refNode))
                    {
                        // WORDSNET-23370 Keep reference paragraph to ensure that section has at least one.
                        //
                        // AM. There is total mess in which paragraph we delete or keep,
                        // it could be reference or last paragraph in document being inserted,
                        // sometimes we copy attributes which means we actually remove wrong paragraph, etc.
                        //
                        // It feels this logic should be reconsidered and simplified.
                    }
                    else
                        refNode.Remove();
                }
            }

            return newNode;
        }

        /// <summary>
        /// Updates row-level SDT from mapped document.
        /// </summary>
        private static Row InsertDocumentAtRowLevel(StructuredDocumentTag sdt, Document doc)
        {
            sdt.RemoveAllChildren();
            Row row = doc.GetChild(NodeType.Row, 0, true) as Row;

            if (row != null)
            {
                Row newRow = (Row)row.Clone(true);
                newRow.SetDocument(sdt.Document);

                sdt.AppendChild(newRow);

                // Copy following bookmarks.
                Node node = row.NextSibling;
                while(node is IBookmarkNode)
                {
                    Node newBookmarkNode = node.Clone(true);
                    newBookmarkNode.SetDocument(sdt.Document);

                    sdt.AppendChild(newBookmarkNode);

                    node = node.NextSibling;
                }
            }

            return row;
        }

        /// <summary>
        /// Checks if document has more than one line.
        /// </summary>
        private static bool IsMultilineDocument(Document doc)
        {
            return (doc.FirstSection.Body.Paragraphs.Count > 1) || (doc.FirstSection.Body.Tables.Count > 0);
        }

        /// <summary>
        /// Checks that given document is empty.
        /// </summary>
        private static bool IsDocumentEmpty(Document doc)
        {
            if (doc.Sections.Count > 1)
                return false;

            if (doc.FirstSection.Body.Paragraphs.Count > 1)
                return false;

            if (doc.FirstSection.Body.Tables.Count > 0)
                return false;

            if (doc.FirstSection.Body.FirstParagraph.Count > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks that given node is only paragraph in section.
        /// </summary>
        private static bool IsOnlyParagraphInSection(Node refNode)
        {
            if (refNode.NodeType != NodeType.Paragraph)
                return false;

            if (refNode.ParentNode.NodeType != NodeType.Body)
                return false;

            Body body = (Body)refNode.ParentNode;
            return body.ParentSection.GetChildNodes(NodeType.Paragraph, true).Count == 1;
        }

        /// <summary>
        /// It seems MS Word replaces all line breaks with soft line break (\v). This method does the same.
        /// </summary>
        private static string ResolveLineBreaks(string input)
        {
            return input
                .Replace(ControlChar.CrLf, ControlChar.LineBreak)
                .Replace(ControlChar.Cr, ControlChar.LineBreak)
                .Replace(ControlChar.Lf, ControlChar.LineBreak);
        }

        /// <summary>
        /// Returns the last paragraph of the specified structured document tag.
        /// </summary>
        private static Paragraph GetLastParagraph(StructuredDocumentTag sdt)
        {
            // NodeCollection skips SDTs, let's find in a cycle.
            Node paragraph = sdt.LastChild;
            while (paragraph != null && (paragraph.NodeType != NodeType.Paragraph))
                paragraph = paragraph.PreviousSibling;
            return (Paragraph)paragraph;
        }

        /// <summary>
        /// Clears contents of the specified cell of the structured document tag and inserts a placeholder or
        /// default contents.
        /// </summary>
        private static void ClearCell(Cell cell, StructuredDocumentTag sdt)
        {
            // Keep paragraph formatting.
            Paragraph formatSourceParagraph = (Paragraph)cell.GetChild(NodeType.Paragraph, -1, false);

            cell.RemoveAllChildren();
            if (sdt.Placeholder != null)
                cell.AppendChild(sdt.Document.ImportNode(sdt.Placeholder.FirstSection.Body.FirstParagraph, true));
            else
                InsertContentForEmptySdt(cell, sdt.ContentsRunPr, true, sdt.EndCharacterRunPr);

            if (formatSourceParagraph != null)
            {
                Paragraph paragraph = (Paragraph)cell.GetChild(NodeType.Paragraph, 0, false);
                if (paragraph != null)
                {
                    paragraph.ParaPr = formatSourceParagraph.ParaPr.Clone();
                    paragraph.ParagraphBreakRunPr = formatSourceParagraph.ParagraphBreakRunPr.Clone();
                }
            }
        }

        /// <summary>
        /// Creates and finds out correct place for the reference node depending on SDT MarkupLevel.
        /// </summary>
        private static Node CreateReferenceNode(StructuredDocumentTag sdt)
        {
            // For Block, Inline and Cell remove all child nodes and replace them with a new node.
            switch (sdt.Level)
            {
                case MarkupLevel.Block:
                    return ReplaceChildrenWithParagraph(sdt);
                case MarkupLevel.Inline:
                    return ReplaceChildrenWithNode(sdt, new Run(sdt.Document));
                case MarkupLevel.Cell:
                {
                    // WORDSNET-21138 We need to remove all child nodes from the Cell
                    // but keep the properties of the last paragraph (if it exists).
                    return ReplaceChildrenWithParagraph((CompositeNode)sdt.GetChild(NodeType.Cell, 0, false));
                }
                // After WORDSNET-9813 will be fixed we can came back to implement this case.
                case MarkupLevel.Row:
                default:
                    throw new InvalidOperationException("Unknown markup level.");
            }
        }

        /// <summary>
        /// Creates a reference node, replacing the ancestor's child nodes with an empty paragraph.
        /// If the last paragraph of the replaced child nodes has got properties,
        /// stores them in the inserted paragraph.
        /// </summary>
        private static Node ReplaceChildrenWithParagraph(CompositeNode ancestor)
        {
            if (ancestor == null)
                return null;

            Paragraph para = ancestor.LastNonAnnotationChild as Paragraph;
            if (para != null)
                para.GetChildNodes(NodeType.Any, false).Clear();
            else
                para = new Paragraph(ancestor.Document);

            return ReplaceChildrenWithNode(ancestor, para);
        }

        /// <summary>
        /// Replaces child nodes of the ancestor with specified node.
        /// </summary>
        private static Node ReplaceChildrenWithNode(CompositeNode ancestor, Node newChild)
        {
            Debug.Assert((ancestor != null) && (newChild != null));

            ancestor.GetChildNodes(NodeType.Any, false).Clear();
            return ancestor.AppendChild(newChild);
        }

        /// <summary>
        /// Inserts content from placeholder building block into sdtContent.
        /// </summary>
        private static void InsertPlaceholderContent(StructuredDocumentTag sdt)
        {
            Debug.Assert((sdt != null) && (sdt.Placeholder != null));

            bool isRowLevel = (sdt.Level == MarkupLevel.Row);

            // WORDSNET-21607 We need to use the same logic as when inserting a document.
            Node refNode = isRowLevel ? null : CreateReferenceNode(sdt);

            foreach (Paragraph para in sdt.Placeholder.FirstSection.Body.Paragraphs)
            {
                bool clearChildNodes = isRowLevel && para.IsFirstChild;

                if (para.IsLastChild)
                {
                    // Prevent modification of the original paragraph.
                    Paragraph paraClone = (Paragraph)para.Clone(true);
                    paraClone.ParaPr.Clear();

                    Node newNode = InsertContent(sdt, paraClone, clearChildNodes);

                    // Copy the 'refNode' properties and identifiers to the last paragraph.
                    // WORDSNET-24116 Apply formatting AFTER content has been imported.
                    if (IsCellOrBlockLevel(sdt.Level) && (refNode != null))
                    {
                        Paragraph refPara = (Paragraph)refNode;
                        Paragraph newPara = (Paragraph)newNode;
                        CopyProperties(refPara, newPara);
                        CopyIdentifiers(refPara, newPara);
                    }
                }
                else
                {
                    InsertContent(sdt, para, clearChildNodes);
                }
            }

            if (refNode != null)
                refNode.Remove();
        }

        /// <summary>
        /// Inserts default content for <see cref="SdtType.Picture"/> inside sdtContent.
        /// </summary>
        private static void InsertPictureDefaultContent(StructuredDocumentTag sdt)
        {
            Paragraph container = new Paragraph(sdt.Document);
            container.AppendChild(GetPlaceholderDrawingFromInternalResource(sdt.Document));
            InsertContent(sdt, container, true);
        }

        /// <summary>
        /// This method is using non-imported nodes contrary to <see cref="InsertBlockContent"/> and others,
        /// because here we first need to figure out which part of the content to import.
        /// Then the required part is selected (run or paragraph) and then imported and inserted as usual.
        /// </summary>
        private static Node InsertInlineContent(StructuredDocumentTag sdt, Node content, Node referenceNode)
        {
            Node newNode = null;

            if (NodeUtil.IsBlockLevelNode(content) && content.IsComposite)
            {
                CompositeNode node = (CompositeNode)content;

                // WORDSNET-11759 Fixed situation when Placeholder contains paragraph but doesn't contain run with content.
                if (node.FirstChild != null)
                {
                    newNode = sdt.Document.ImportNode(node.FirstChild, false);

                    // By default all placeholders contain para as wrappers, so we take the first run if sdt is inline.
                    sdt.AppendChild(newNode);
                }
            }
            else if (NodeUtil.IsInlineLevelNode(content))
            {
                newNode = sdt.Document.ImportNode(content, false);

                sdt.InsertBefore(newNode, referenceNode);
            }
            else
            {
                throw new InvalidOperationException("Cannot insert such node at Inline level.");
            }

            return newNode;
        }

        private static void InsertBlockContent(StructuredDocumentTag sdt, Node content)
        {
            if (NodeUtil.IsBlockLevelNode(content))
            {
                sdt.AppendChild(content);
            }
            else if (NodeUtil.IsInlineLevelNode(content))
            {
                InsertInlineNode(content, sdt);
            }
            else
            {
                throw new InvalidOperationException("Cannot insert such node at Block level.");
            }
        }

        /// <summary>
        /// Handles insertion of cell/row - level content into sdt. Does wrapping of block elements if needed.
        /// Wrapping allows insertion of block content from placeholder building block into row/cell sdt.
        /// </summary>
        private static void InsertCellRowContent(StructuredDocumentTag sdt, Node content)
        {
            bool isRowLevel = (sdt.Level == MarkupLevel.Row);
            bool isCellLevel = (sdt.Level == MarkupLevel.Cell);

            if ((isRowLevel && NodeUtil.IsRowLevelNode(content)) || (isCellLevel && NodeUtil.IsCellLevelNode(content)))
            {
                sdt.AppendChild(content);
            }
            else if (NodeUtil.IsBlockLevelNode(content))
            {
                CompositeNode wrapper;

                if (isCellLevel)
                {
                    if (sdt.HasChildNodes && (sdt.FirstChild.NodeType == NodeType.Cell))
                    {
                        // Use existing cell, do not create wrapper.
                        wrapper = (CompositeNode)sdt.FirstChild;
                    }
                    else
                    {
                        wrapper = new Cell(sdt.Document);
                        sdt.AppendChild(wrapper);
                    }
                }
                else
                {
                    wrapper = new Row(sdt.Document);
                    sdt.AppendChild(wrapper);
                }

                if (isRowLevel && !NodeUtil.IsCellLevelNode(content)) // wrap content into cell so we can put it inside row.
                {
                    Cell innerWrapper = new Cell(sdt.Document);
                    wrapper.AppendChild(innerWrapper);
                    wrapper = innerWrapper;
                }

                wrapper.AppendChild(content);
            }
            else if (NodeUtil.IsInlineLevelNode(content))
            {
                InsertInlineNode(content, sdt);
            }
            else
            {
                throw new InvalidOperationException("Can not insert node at this level.");
            }
        }

        /// <summary>
        /// Inserts inline node into the paragraph if it exists. If not, creates new paragraph and inserts node into it.
        /// </summary>
        private static void InsertInlineNode(Node content, StructuredDocumentTag sdt)
        {
            // Only inline content nodes is expected.
            if (NodeUtil.IsBlockLevelNode(content) && !NodeUtil.IsInlineLevelNode(content))
                throw new ArgumentException("Only inline content nodes is expected.");

            Paragraph paragraph = (Paragraph)sdt.GetChild(NodeType.Paragraph, 0, true);
            if (paragraph == null)
            {
                paragraph = new Paragraph(sdt.Document);
                sdt.AppendChild(paragraph);
            }

            // WORDSNET-26045 Try to append content BiDi aware.
            if (!AppendRunBiDiAware(paragraph, content as Run))
                paragraph.AppendChild(content);
        }

        /// <summary>
        /// Appends a specified run to a specified paragraph BiDi aware.
        /// </summary>
        /// <remarks>
        /// Splits passed run using <see cref="BidiAlgorithm.Apply(string,bool)"/> and then
        /// splits by <see cref="CharacterClassifier.IsRtlScript"/> attribute to mimic Word behavior.
        /// </remarks>
        /// <returns>True, if appended successfully.</returns>
        private static bool AppendRunBiDiAware(Paragraph paragraph, Run run)
        {
            if (run == null)
                return false;

            BidiRun[] bidiRuns = BidiAlgorithm.Apply(run.Text, paragraph.ParaPr.Bidi);
            foreach (BidiRun bidiRun in bidiRuns)
            {
                Run bidiAwareRun;
                // Word also splits text in SDT by rtl script,
                // if there are more than one BiDi run and it is RTL.
                if ((bidiRuns.Length > 1) && bidiRun.Rtl)
                {
                    List<BidiRun> subRuns = SplitByRtlScript(bidiRun.Text);
                    foreach (BidiRun subRun in subRuns)
                    {
                        bidiAwareRun = new Run(run.Document, subRun.Text, run.RunPr.Clone());
                        // Set BiDi property only if it has been changed to avoid writing of extra BiDi property.
                        if (bidiAwareRun.RunPr.Bidi.ToBool() != subRun.Rtl)
                            bidiAwareRun.RunPr.Bidi = AttrBoolEx.FromBool(subRun.Rtl);
                        paragraph.AppendChild(bidiAwareRun);
                    }
                }
                else
                {
                    bidiAwareRun = new Run(run.Document, bidiRun.Text, run.RunPr.Clone());
                    // Set BiDi property only if it has been changed to avoid writing of extra BiDi property.
                    if (bidiAwareRun.RunPr.Bidi.ToBool() != bidiRun.Rtl)
                        bidiAwareRun.RunPr.Bidi = AttrBoolEx.FromBool(bidiRun.Rtl);
                    paragraph.AppendChild(bidiAwareRun);
                }
            }

            return true;
        }

        /// <summary>
        /// Splits text within a specified Run by <see cref="CharacterClassifier.IsRtlScript"/> attribute.
        /// </summary>
        private static List<BidiRun> SplitByRtlScript(string text)
        {
            List<BidiRun> bidiRuns = new List<BidiRun>();

            if (text.Length == 0)
                return bidiRuns;

            BidiRun bidiRun;
            bool isRtl = CharacterClassifier.IsRtlScript(text[0]);
            int start = 0;
            for (int i = 1; i < text.Length; i++)
            {
                if (CharacterClassifier.IsRtlScript(text[i]) == isRtl)
                    continue;

                // Found text with different rtl script.
                bidiRun = new BidiRun(text.Substring(start, i - start), isRtl ? 1 : 0);
                bidiRuns.Add(bidiRun);

                isRtl = !isRtl;
                start = i;
            }

            // Proceed with very last piece of Run text.
            bidiRun = new BidiRun(text.Substring(start, text.Length - start), isRtl ? 1 : 0);
            bidiRuns.Add(bidiRun);

            return bidiRuns;
        }

        /// <summary>
        /// Extract DrawingML placeholder for image Sdt from internally stored docx.
        /// This is done due to two limitations:
        /// 1) MS Word only expects DrawingML inside SdtContent of <see cref="SdtType.Picture"/>.
        /// 2) We only have ctors in DrawingML that allow reading from XmlDocument. So we have
        /// to read something pre-created rather than create new DrawingML on the fly.
        ///
        /// There are no limitations now.
        /// </summary>
        [JavaConvertCheckedExceptions]
        private static Shape GetPlaceholderDrawingFromInternalResource(DocumentBase dstDocument)
        {
            Node dml;

            const string resourceName = "Aspose.Words.Resources.SdtImageDefaultContent.docx";
            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                // alexnosk WORDSNET-19906 Specify LoadFormat.Docx explicitly to skip file format detection to minimize document creation time.
                // Since we are sure SdtImageDefaultContent.docx is not encrypted DOCX file, it is safe to specify it explicitly.
                LoadOptions loadOptions = new LoadOptions();
                loadOptions.LoadFormat = LoadFormat.Docx;
                Document doc = new Document(stream, loadOptions, false);
                dml = MemoryManagement.ExtendLifetime(doc.GetChild(NodeType.Shape, 0, true), doc);
            }

            return (Shape)dstDocument.ImportNode(dml, true);
        }

        /// <summary>
        /// Copy properties from the specified Paragraph to another.
        /// </summary>
        private static void CopyProperties(Paragraph paraFrom, Paragraph paraTo)
        {
            Debug.Assert((paraFrom != null) && (paraTo != null));
            paraTo.ParaPr = paraFrom.ParaPr.Clone();
        }

        /// <summary>
        /// Copy ParaId and TextId from the specified Paragraph to another.
        /// </summary>
        private static void CopyIdentifiers(Paragraph paraFrom, Paragraph paraTo)
        {
            Debug.Assert((paraFrom != null) && (paraTo != null));
            paraTo.ParaId = paraFrom.ParaId;
            paraTo.TextId = paraFrom.TextId;
        }

        private static bool IsCellOrBlockLevel(MarkupLevel markupLevel)
        {
            return (markupLevel == MarkupLevel.Cell) || (markupLevel == MarkupLevel.Block);
        }

        /// <summary>
        /// Converts MS Word date display mask to C# DateTime format string.
        /// See ISO 29500 §17.5.2.8 dateFormat (Date Display Mask) for details.
        /// </summary>
        private static string ConvertFormatString(string format)
        {
            // Note date is converted to local time as Word does.
            // WORDSNET-10270 The "tt" custom format specifier represents the entire AM/PM designator.
            // Also, replace it case-insensitive.
            string result = Regex.Replace(format, "AM/PM", "tt", RegexOptions.IgnoreCase);

            // WORDSNET-25629 Spec allows a case insensitive year format.
            result = result.Replace("YY", "yy");

            return result;
        }

        internal const string DefaultContentIfNoPlaceholder = "     ";
    }
}
