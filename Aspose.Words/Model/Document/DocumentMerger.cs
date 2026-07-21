// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2022 by Ilya Navrotskiy

using Aspose.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Properties;

namespace Aspose.Words
{
    /// <summary>
    /// Class for merging documents.
    /// </summary>
    /// <dev>
    /// This is very first version of document merge feature.
    /// According to issues, there will be many tricks and hacks while trying to preserve original documents layout.
    /// </dev>
    internal class DocumentMerger
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DocumentMerger"/> class.
        /// </summary>
        private DocumentMerger(DocumentBase dstDoc, DocumentBase srcDoc)
        {
            Debug.Assert((srcDoc != null) && (dstDoc != null));
            mNodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode, gImportFormatOptions);
        }

        /// <summary>
        /// Merges all specified documents into the <paramref name="dstDoc"/>.
        /// </summary>
        internal static void MergeDocuments(Document dstDoc, params Document[] srcDocs)
        {
            foreach (Document srcDoc in srcDocs)
            {
                DocumentMerger merger = new DocumentMerger(dstDoc, srcDoc);

                UpdateLastParagraph(dstDoc);

                // WORDSNET-26891 Apply compatibility options from the source document
                // in case of destination document is blank.
                if (IsBlank(dstDoc))
                    dstDoc.DocPr.CompatibilityOptions = srcDoc.CompatibilityOptions.Clone();

                // WORDSNET-24985 Ensure vertically anchored to paragraph TextBox,
                // will not be inside a section break paragraph.
                EnsureNoTextBoxInLastPara(dstDoc);

                merger.Execute();
            }
        }

        /// <summary>
        /// Executes merger.
        /// </summary>
        private void Execute()
        {
            // WORDSNET-28311 Word always updates `NUMPAGES` field when loads a document. This means we need
            // to replace `NUMPAGES` field with its result from the original document to preserve number of pages
            // as it was in original document.
            // Also note, to simplify the solution, we assume that `NUMPAGES` field is located inside HeadersFooters.
            // If there will be another places with `NUMPAGES`, then we will need to extend the solution.
            FieldRemoverFilter filter = new FieldRemoverFilter(FieldType.FieldNumPages);
            ReplaceFieldsWithResults(DstDoc, filter);

            // Remember last section of the original destination document.
            Section lastSection = DstDoc.LastSection;

            // The collection of forcibly unlinked from previous
            // Header and Footer types that were empty in a source document.
            HashSetGeneric<HeaderFooterType> unlinkedHeaderFooters = null;

            foreach (Section srcSection in SrcDoc.Sections)
            {
                Section importedSection = (Section)mNodeImporter.ImportNode(srcSection, true, mInfoCollector);
                DstDoc.AppendChild(importedSection);

                // For the very first section, we should unlink it from a previous section
                // and reset section break to a `NewPage`, as we want to preserve source
                // sections "as is" by design.
                if (srcSection.IsFirstSection)
                {
                    unlinkedHeaderFooters = UnlinkFromPrevious(importedSection.HeadersFooters);
                    importedSection.PageSetup.SectionStart = SectionStart.NewPage;

                    // WORDSNET-25831 Preserve original page numbering.
                    importedSection.PageSetup.RestartPageNumbering = true;
                }
                else
                {
                    Debug.Assert(unlinkedHeaderFooters != null);

                    // WORDSNET-25832 For all other (non-first) imported sections we also
                    // should consider either they were linked or not before import and
                    // either we have unlinked something forcibly for the very first section.
                    // If there is no Header (or Footer respectively) in imported section and
                    // there was forcibly unlinked first section with empty Header (Footer),
                    // then we should compensate Header (Footer) distance for such section.
                    // Otherwise, we remove Headers (Footers) from the empty unlinked collection
                    // to not consider them anymore (as they become non-empty).
                    for (int i = importedSection.HeadersFooters.Count - 1; i >= 0; i--)
                    {
                        HeaderFooter hf = importedSection.HeadersFooters[i];
                        if (hf.IsLinkedToPrevious && unlinkedHeaderFooters.Contains(hf.HeaderFooterType))
                        {
                            unlinkedHeaderFooters.Remove(hf.HeaderFooterType);
                            importedSection.HeadersFooters.RemoveAt(i);
                        }
                    }

                    // WORDSNET-28445 We can compensate Distance only when there are
                    // no content in all Headers or Footers of the section.
                    if (!HasContentInHeaders(importedSection))
                        importedSection.PageSetup.HeaderDistance = 0.0;

                    if (!HasContentInFooters(importedSection))
                        importedSection.PageSetup.FooterDistance = 0.0;
                }

                // WORDSNET-28311 Replace `NUMPAGES` fields with their results. See details above.
                ReplaceFieldsWithResults(importedSection, filter);
            }

            // WORDSNET-26046 If footnote separators in source and destination documents have
            // different formatting and source document has footnotes, but destination document
            // has no it, then we can replace destination separator formatting with the source
            // one to preserve formatting of the source document in this case.
            FootnoteSeparator dstSeparator = DstDoc.FootnoteSeparators[FootnoteSeparatorType.FootnoteSeparator];
            FootnoteSeparator srcSeparator = SrcDoc.FootnoteSeparators[FootnoteSeparatorType.FootnoteSeparator];
            // WORDSNET-28957 First check whether the source document contains FootnoteSeparator.
            bool tryResolveFootnotes = (srcSeparator != null) &&
                                       mInfoCollector.HasFootnotes &&
                                       !AreEqual(dstSeparator, srcSeparator);

            // WORDSNET-25885 If `DoNotShadeFormData` setting is different in source and destination documents,
            // then we can try the trick the same, as above for footnote separators in WORDSNET-26046.
            bool tryResolveFormFields = mInfoCollector.HasFormFields &&
                                     (DstDoc.DocPr.DoNotShadeFormData != SrcDoc.DocPr.DoNotShadeFormData);
            if (tryResolveFootnotes || tryResolveFormFields)
            {
                // Run visitor to collect data up to original
                // last section of the destination document.
                DocMergerVisitor mergerVisitor = new DocMergerVisitor(lastSection);
                DstDoc.Accept(mergerVisitor);

                if (tryResolveFootnotes && !mergerVisitor.HasFootnotes)
                {
                    // There are no footnotes in destination,
                    // so we can replace it to preserve source formatting.
                    // WORDSNET-26604 We should import, instead of just clone a node.
                    FootnoteSeparator clonedSeparator = (FootnoteSeparator)mNodeImporter.ImportNode(srcSeparator, true);
                    clonedSeparator.SetDocument(DstDoc);
                    DstDoc.FootnoteSeparators.Add(clonedSeparator);
                }

                if (tryResolveFormFields && !mergerVisitor.HasFormFields)
                {
                    // There are no form fields in destination, so we can override
                    // `DoNotShadeFormData` setting from the source document.
                    DstDoc.DocPr.DoNotShadeFormData = SrcDoc.DocPr.DoNotShadeFormData;
                }
            }

            // WORDSNET-25408 Update cloned SDTs mapped to document core
            // properties with the data from the source document.
            UpdateCorePropertiesSdts();

            // WORDSNET-25587 Added importing citations.
            mNodeImporter.ImportCitationSources();
        }

        /// <summary>
        /// Replaces fields with field results in the all headers and footers of a given document.
        /// </summary>
        private static void ReplaceFieldsWithResults(Document doc, FieldRemoverFilter filter)
        {
            foreach (Section section in doc.Sections)
                ReplaceFieldsWithResults(section, filter);
        }

        /// <summary>
        /// Replaces fields with field results in the all headers and footers of a given section.
        /// </summary>
        private static void ReplaceFieldsWithResults(Section section, FieldRemoverFilter filter)
        {
            foreach (HeaderFooter headerFooter in section.HeadersFooters)
            {
                NodeRange range = new NodeRange(headerFooter, headerFooter);
                FieldRemover.ReplaceFieldsWithResults(range, filter);
            }
        }

        /// <summary>
        /// Returns true, if specified <see cref="FootnoteSeparator"/> objects are equal.
        /// </summary>
        private static bool AreEqual(FootnoteSeparator a, FootnoteSeparator b)
        {
            if (a != null)
                return a.Equals(b);

            return (b == null);
        }

        /// <summary>
        /// Returns true, if a specified collection has any of the Headers from the <see cref="HeaderFooterType"/>.
        /// </summary>
        private static bool HasHeaders(HashSetGeneric<HeaderFooterType> headerFooterTypes)
        {
            foreach (HeaderFooterType headerFooterType in headerFooterTypes)
            {
                if ((headerFooterType == HeaderFooterType.HeaderEven) ||
                    (headerFooterType == HeaderFooterType.HeaderFirst) ||
                    (headerFooterType == HeaderFooterType.HeaderPrimary))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true, if a specified collection has any of the Footers from the <see cref="HeaderFooterType"/>.
        /// </summary>
        private static bool HasFooters(HashSetGeneric<HeaderFooterType> headerFooterTypes)
        {
            foreach (HeaderFooterType headerFooterType in headerFooterTypes)
            {
                if ((headerFooterType == HeaderFooterType.FooterEven) ||
                    (headerFooterType == HeaderFooterType.FooterFirst) ||
                    (headerFooterType == HeaderFooterType.FooterPrimary))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns collection of <see cref="HeaderFooterType"/> objects that are used in a specified <see cref="Document"/>.
        /// </summary>
        private static HashSetGeneric<HeaderFooterType> GetUsedHeaderFooterTypes(Document doc)
        {
            HashSetGeneric<HeaderFooterType> headerFooterTypes = new HashSetGeneric<HeaderFooterType>();
            foreach (Section section in doc.Sections)
            {
                HashSetGeneric<HeaderFooterType> curSectionheaderFooterTypes = GetUsedHeaderFooterTypes(section);
                foreach (HeaderFooterType curSectionheaderFooterType in curSectionheaderFooterTypes)
                    headerFooterTypes.Add(curSectionheaderFooterType);
            }

            return headerFooterTypes;
        }

        /// <summary>
        /// Returns collection of <see cref="HeaderFooterType"/> objects that are used in a specified <see cref="Document"/>.
        /// </summary>
        private static HashSetGeneric<HeaderFooterType> GetUsedHeaderFooterTypes(Section section)
        {
            HashSetGeneric<HeaderFooterType> headerFooterTypes = new HashSetGeneric<HeaderFooterType>();
            foreach (HeaderFooter hf in section.HeadersFooters)
                headerFooterTypes.Add(hf.HeaderFooterType);

            return headerFooterTypes;
        }

        /// <summary>
        /// Unlinks specified HeadersFooters from previous section.
        /// </summary>
        /// <remarks>
        /// Unlinks only existing HeadersFooters,
        /// preventing creation an extra empty HeadersFooters.
        /// </remarks>
        /// <returns> The collection of <see cref="HeaderFooterType"/> objects for which
        /// <see cref="HeaderFooter"/> were originally empty and unlinked.</returns>
        private HashSetGeneric<HeaderFooterType> UnlinkFromPrevious(HeaderFooterCollection headersFooters)
        {
            HashSetGeneric<HeaderFooterType> unlinkedHeaderFooters = new HashSetGeneric<HeaderFooterType>();

            // Collect existing types of HeadersFooters in destination document.
            HashSetGeneric<HeaderFooterType> dstHeaderFooterTypes = GetUsedHeaderFooterTypes(DstDoc);

            // If there was no HeaderFooter originally, or it was empty, then try to
            // compensate it by eliminating Header/Footer distance.
            // WORDSNET-25886 Compensate distance only when destination has header/footer and
            // merging section does not have it.
            if (!headersFooters.HasHeaders && HasHeaders(dstHeaderFooterTypes))
                headersFooters.ParentSection.PageSetup.HeaderDistance = 0.0;

            if (!headersFooters.HasFooters && HasFooters(dstHeaderFooterTypes))
                headersFooters.ParentSection.PageSetup.FooterDistance = 0.0;

            foreach (HeaderFooterType type in dstHeaderFooterTypes)
            {
                HeaderFooter headerFooter = headersFooters[type];
                bool isLinkedToPrevious = (headerFooter == null) || headerFooter.IsLinkedToPrevious;

                // There is such HeaderFooter type in destination document,
                // so unlink this type from previous forcibly.
                if (isLinkedToPrevious)
                {
                    headersFooters.LinkToPrevious(type, false);
                    unlinkedHeaderFooters.Add(type);

                    headerFooter = headersFooters[type];
                    // As we unlinked HeaderFooter, there must be at least one paragraph.
                    Debug.Assert(headerFooter.FirstParagraph != null);

                    // If there was no HeaderFooter originally, or it was empty,
                    // then try to compensate it by hiding paragraph.
                    headerFooter.FirstParagraph.ParagraphBreakRunPr[FontAttr.Size] = 0;
                    headerFooter.FirstParagraph.ParagraphBreakRunPr[FontAttr.Hidden] = AttrBoolEx.True;
                }
            }

            return unlinkedHeaderFooters;
        }

        /// <summary>
        /// Returns true, if a specified section has any content in the HeaderFooter of a specified type.
        /// </summary>
        /// <remarks>
        /// Considers also content of the previous HeaderFooter if this one is linked to it.
        /// </remarks>
        private static bool HasContentInHeaderFooter(Section section, HeaderFooterType type)
        {
            while (section != null)
            {
                HeaderFooter hf = section.HeadersFooters[type];
                section = (Section)section.PreviousSibling;

                if (hf == null || !hf.HasChildNodes)
                    continue;

                if (hf.Count > 1)
                    return true;

                return (hf.FirstParagraph != null) && hf.FirstParagraph.HasChildNodes;
            }

            return false;
        }

        /// <summary>
        /// Returns true, if a specified section has any content in any of its Headers.
        /// </summary>
        /// <remarks>
        /// Considers also content of the previous Headers, if this one is linked to it.
        /// </remarks>
        private static bool HasContentInHeaders(Section section)
        {
            return HasContentInHeaderFooter(section, HeaderFooterType.HeaderFirst) ||
                   HasContentInHeaderFooter(section, HeaderFooterType.HeaderPrimary) ||
                   HasContentInHeaderFooter(section, HeaderFooterType.HeaderEven);
        }

        /// <summary>
        /// Returns true, if a specified section has any content in any of its Footers.
        /// </summary>
        /// <remarks>
        /// Considers also content of the previous Footers, if this one is linked to it.
        /// </remarks>
        private static bool HasContentInFooters(Section section)
        {
            return HasContentInHeaderFooter(section, HeaderFooterType.FooterFirst) ||
                   HasContentInHeaderFooter(section, HeaderFooterType.FooterPrimary) ||
                   HasContentInHeaderFooter(section, HeaderFooterType.FooterEven);
        }

        /// <summary>
        /// Fixes the issue with incorrect alignment of a paragraph when it appears not at the end of the document during the merge process.
        /// </summary>
        private static void UpdateLastParagraph(Document doc)
        {
            if ((doc.LastSection == null) || (doc.LastSection.Body == null))
                return;

            Paragraph lastPara = doc.LastSection.Body.LastParagraph;
            if (lastPara == null || lastPara.IsEmptyOrContainsOnlyCrossAnnotation)
                return;

            if (lastPara.ParaPr.Alignment == ParagraphAlignment.Justify)
            {
                // A zero-size paragraph is added, so that the page layout does not have to be changed.
                // In fact, the actual height of such paragraph is 2 pixels, or 30 twips.
                Paragraph hiddenPara = new Paragraph(doc);
                hiddenPara.ParaPr.SpaceAfter = 0;
                hiddenPara.ParaPr.SpaceBefore = 0;
                hiddenPara.ParagraphBreakRunPr.Size = 0;
                hiddenPara.ParagraphBreakRunPr.SizeBi = 0;
                doc.LastSection.Body.AppendChild(hiddenPara);
            }
        }

        /// <summary>
        /// Updates cloned SDTs mapped to document core properties
        /// with the data from the source document.
        /// </summary>
        private void UpdateCorePropertiesSdts()
        {
            foreach (StructuredDocumentTag sdt in mInfoCollector.CorePropSdts)
            {
                // For the moment we process only Title sdts.
                UpdateTitleSdt(sdt);
            }
        }

        /// <summary>
        /// Updates specified SDT if it maps to document core property Title
        /// with a Title from the source document.
        /// </summary>
        private void UpdateTitleSdt(StructuredDocumentTag sdt)
        {
            if (sdt.XmlMapping.GetPropertyName() != PropertyName.Title)
                return;

            // Save current Title.
            string dstTitle = DstDoc.BuiltInDocumentProperties.Title;

            // Update with source document Title.
            DstDoc.BuiltInDocumentProperties.Title = SrcDoc.BuiltInDocumentProperties.Title;
            sdt.XmlMapping.UpdateContent();

            // Remove mapping to preserve updated content from any future changes.
            sdt.XmlMapping.Delete();

            // Restore original Title.
            DstDoc.BuiltInDocumentProperties.Title = dstTitle;
        }

        /// <summary>
        /// Ensures that there is no TextBox inside a very last paragraph of a specified document
        /// vertically anchored to this paragraph.
        /// </summary>
        private static void EnsureNoTextBoxInLastPara(Document doc)
        {
            // WORDSNET-26815 Resilience against a completely empty document.
            if (doc.LastSection == null)
                return;

            Paragraph lastPara = doc.LastSection.Body.LastParagraph;
            if (lastPara == null)
                return;

            Shape shape = (Shape)lastPara.GetChild(NodeType.Shape, 0, true);
            // If there is no TextBox, or it is not vertically anchored to paragraph,
            // then nothing to do and just exit.
            if ((shape == null) || !shape.IsTextBox || (shape.RelativeVerticalPosition != RelativeVerticalPosition.Paragraph))
                return;

            // To ensure TextBox is not inside a very last paragraph
            // just add extra paragraph after it.
            lastPara.ParentNode.AppendChild(new Paragraph(doc));
        }

        /// <summary>
        /// Returns true, if a specified document is blank.
        /// </summary>
        private static bool IsBlank(Document doc)
        {
            if (doc.FirstSection == null)
                return true;

            // For the moment consider blank only single-section documents.
            if (doc.Count > 1)
                return false;

            // For the moment consider as blank, if there are no any headers and footers.
            if (doc.FirstSection.HeadersFooters.HasHeaders || doc.FirstSection.HeadersFooters.HasFooters)
                return false;

            if (doc.FirstSection.Body == null)
                return true;

            foreach (Node node in doc.FirstSection.Body)
            {
                switch (node.NodeType)
                {
                    case NodeType.BookmarkStart:
                    case NodeType.BookmarkEnd:
                        continue;

                    case NodeType.Paragraph:
                        if (((Paragraph)node).IsEmptyOrContainsOnlyCrossAnnotation)
                            continue;
                        break;
                    default:
                        return false;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// The source document.
        /// </summary>
        private Document SrcDoc
        {
            get { return (Document)mNodeImporter.Context.SrcDoc; }
        }

        /// <summary>
        /// The destination document.
        /// </summary>
        private Document DstDoc
        {
            get { return (Document)mNodeImporter.Context.DstDoc; }
        }

        /// <summary>
        /// The instance of NodeImporter based on merging documents.
        /// </summary>
        private readonly NodeImporter mNodeImporter;

        /// <summary>
        /// The import format options being used upon merging documents.
        /// </summary>
        private static readonly ImportFormatOptions gImportFormatOptions = new ImportFormatOptions()
        {
            IsMerger = true, IgnoreHeaderFooter = false, IgnoreTextBoxes = false, KeepSourceNumbering = true
        };

        /// <summary>
        /// The import format mode.
        /// </summary>
        /// <remarks>
        /// To preserve source document 'as is' we should always use `KeepSourceFormatting` mode.
        /// </remarks>
        private const ImportFormatMode ImportFormatMode = Words.ImportFormatMode.KeepSourceFormatting;

        /// <summary>
        /// Clone listener that collects all necessary information about inserted content upon importing its particular nodes.
        /// </summary>
        private readonly DocMergerInfoCollector mInfoCollector = new DocMergerInfoCollector();

        /// <summary>
        /// Implements <see cref="IFieldRemoverFilter"/> for certain field types.
        /// </summary>
        private class FieldRemoverFilter : IFieldRemoverFilter
        {
            /// <summary>
            /// Initializes a new instance of <see cref="FieldRemoverFilter"/> class.
            /// </summary>
            public FieldRemoverFilter(params FieldType[] fieldTypes)
            {
                ArgumentUtil.CheckNotNull(fieldTypes, "fieldTypes");
                mFieldTypes = fieldTypes;
            }

            bool IFieldRemoverFilter.NeedReplaceFieldWithResult(FieldChar fieldChar)
            {
                foreach (FieldType fieldType in mFieldTypes)
                {
                    if (fieldType == fieldChar.FieldType)
                        return true;
                }

                return false;
            }

            private readonly FieldType[] mFieldTypes;
        }
    }
}
