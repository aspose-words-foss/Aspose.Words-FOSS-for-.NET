// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System;
using System.Text;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the INDEX field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Builds an index using the index entries specified by XE fields, and inserts that index at this place in the document.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppForceForwardDeclaration("Aspose.Common.NullableInt32")]
    public class FieldIndex : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            // Ensure that field code is valid.
            FieldIndexValidateResult validateResult = Validate(Updater);
            if (validateResult.IsError)
                return new FieldUpdateActionInsertErrorMessage(this, validateResult.ErrorMessage);

            Updater.ReflowLayout();

            // An INDEX field result influences on the layout of XE fields located after it in the document.
            //
            // MS Word calculates page numbers for such XE fields before an INDEX field update and uses them during the update.
            // By this reason page numbers in INDEX field results after the first (when there is no result) and any subsequent
            // update (when there is an old result) may be different. Let's mimic this behavior by page numbers' calculating
            // before the old field result removal.

            FieldCodeIndex fieldCode = validateResult.FieldCode;

            FieldSeqDataProvider fieldSeqDataProvider = fieldCode.HasSequenceName
                ? Updater.DataProviders.Ensure(new FieldSeqDataProvider(Updater))
                : null;

            // Build an index entry/subentry tree based on the whole document or its part contained within the specified
            // bookmark if any.
            IndexEntry rootEntry;
            if (fieldCode.HasBookmarkName)
            {
                Bookmark bookmark = validateResult.Bookmark;

                // Do not use Bookmark.GetNodeRange() since MS Word processes table column bookmarks as common ones here.
                NodeRange bookmarkRange = new NodeRange(bookmark.BookmarkStart, false, bookmark.BookmarkEnd, false);
                rootEntry = IndexEntry.GetRootEntry(fieldCode, fieldSeqDataProvider, bookmarkRange);
            }
            else
            {
                rootEntry = IndexEntry.GetRootEntry(fieldCode, fieldSeqDataProvider, Document);
            }

            if (!rootEntry.HasSubentries)
                return new FieldUpdateActionInsertErrorMessage(this, "No index entries found.");

            // An INDEX field result is quite complex, so let's build it right away.
            using (UpdateContext.RemoveOldResultSafe())
            {
                DocumentBuilder builder = FieldIndexAndTablesUtil.GetDocumentBuilder(this);
                FieldIndexAndTablesUtil.ConvertOuterInlineSdtToBlock(this);

                SectPr initialColumnsSectPr = null;

                if (fieldCode.HasNumberOfColumns)
                {
                    // If a number of columns is provided then remember the columns settings (count, spacing, widths) for the current section
                    // and insert a section break with the provided number of columns. Note, that it should be done regardless of
                    // whether the provided number of columns is equal to the current one or not. MS Word behavior.

                    SectPr indexColumnsSectPr = new SectPr();
                    indexColumnsSectPr.ColumnsCount = fieldCode.NumberOfColumns;
                    indexColumnsSectPr.ColumnsSpacing = 720; // MS Word uses this value here.

                    initialColumnsSectPr = InsertSection(builder, indexColumnsSectPr);
                }

                // Write the entry/subentry tree.
                mWriteContext = new FieldIndexWriteContext(fieldCode, builder);
                WriteSubentries(null, rootEntry, IndexEntry.MinSubentryLevel);
                mWriteContext = null;

                if (fieldCode.HasNumberOfColumns)
                {
                    // Insert a section break with the old columns settings.
                    builder.MoveTo(End);
                    InsertSection(builder, initialColumnsSectPr);
                }
            }

            // Everything is done, simply return empty action.
            return new FieldUpdateActionDoNothing(this);
        }

        /// <summary>
        /// Checks whether the field code provides valid arguments for the field to be updated.
        /// </summary>
        /// <param name="updater">
        /// A <see cref="FieldUpdater"/> instance.
        /// It is passed as an argument as the method can be used while updating of another field (typically SEQ).
        /// </param>
        /// <returns>
        /// True if all of the field code arguments are valid.
        /// </returns>
        private FieldIndexValidateResult Validate(FieldUpdater updater)
        {
            string errorMessage = null;
            FieldCodeIndex fieldCode = null;
            Bookmark bookmark = null;

            if (!HasBodyAsStoryAncestor(Start))
            {
                // An INDEX field can not be contained outside a document body (even in shapes within a body).
                errorMessage = "Error! Index not allowed in footnote, endnote, header, footer, or comment.";
            }
            else
            {
                // Cache FieldIndex properties as they are used multiple times during the update.
                fieldCode = new FieldCodeIndex(this);

                if (fieldCode.HasLetterRange && !fieldCode.IsLetterRangeValid)
                {
                    errorMessage = "Error! Not a valid range of characters.";
                }
                else if (fieldCode.HasBookmarkName)
                {
                    // Check whether a bookmark with the specified name exists in a document body.
                    // Note, that even if the bookmark exists in another story, it still can not be used.
                    // SPEED Get a bookmark from a cache.
                    bookmark = updater.GetCachedBookmark(fieldCode.BookmarkName);
                    if ((bookmark != null) && !HasBodyAsStoryAncestor(bookmark.BookmarkStart))
                        bookmark = null;

                    if (bookmark == null)
                        errorMessage = Bookmark.ErrorBookmarkNotDefined;
                }
            }

            return new FieldIndexValidateResult(errorMessage, fieldCode, bookmark);
        }

        /// <summary>
        /// A shortcut method.
        /// </summary>
        private static bool HasBodyAsStoryAncestor(Node node)
        {
            return node.GetStoryAncestor(NodeType.Body) != null;
        }

        /// <summary>
        /// Insert a continuous section break with the specified columns settings.
        /// </summary>
        private static SectPr InsertSection(DocumentBuilder builder, SectPr columnsSectPr)
        {
            StructuredDocumentTag.ConvertOuterSdtsToRanges(builder.CurrentNode);

            SectPr oldSectPr = new SectPr();
            builder.CurrentSection.SectPr.MirrorTo(
                oldSectPr,
                SectAttr.Columns,
                SectAttr.ColumnsCount,
                SectAttr.ColumnsSpacing,
                SectAttr.ColumnsEvenlySpaced);

            builder.InsertSection(SectionStart.Continuous);

            SectPr newSectPr = builder.CurrentSection.SectPr;
            columnsSectPr.MirrorTo(
                newSectPr,
                SectAttr.Columns,
                SectAttr.ColumnsCount,
                SectAttr.ColumnsSpacing,
                SectAttr.ColumnsEvenlySpaced);
            columnsSectPr.ExpandToInclusive(newSectPr, SectAttr.Columns); // Deep clone columns (complex attr).

            return oldSectPr;
        }

        /// <summary>
        /// Writes subentries for the given index entry.
        /// </summary>
        private void WriteSubentries(Node dummyRefNode, IndexEntry entry, int subentryLevel)
        {
            if (!entry.HasSubentries)
                return;

            bool isMinSubentryLevel = subentryLevel == IndexEntry.MinSubentryLevel;
            bool needWriteHeading = isMinSubentryLevel && mWriteContext.FieldCode.HasHeading;
            StyleIdentifier styleIdentifier = GetStyleIdentifier(subentryLevel);
            char lastStartChar = char.MinValue;

            for (int index = 0; index < entry.SubentryCount; index++)
            {
                string subentryText = entry.GetSubentryText(index);
                int startCharIndex = StringUtil.IndexOfNonWhitespace(subentryText);

                // Do not write an empty subentry. But write its subentries though. MS Word behavior.
                if (startCharIndex == -1)
                {
                    // Ensure that a subentry of the min level is not empty which should be guaranteed
                    // by processing of valid entries only (see FieldXE.HasValidText).
                    Debug.Assert(!isMinSubentryLevel);

                    WriteSubentries(dummyRefNode, entry.GetSubentry(index), subentryLevel + 1);
                    continue;
                }

                if (needWriteHeading)
                {
                    char startChar = char.ToUpperInvariant(subentryText[startCharIndex]);

                    // Write a heading only if the start character is changed.
                    if (lastStartChar != startChar)
                    {
                        WriteHeading(startChar);
                        lastStartChar = startChar;
                    }
                }

                WriteSubentry(dummyRefNode, entry, subentryLevel, index, styleIdentifier);
            }
        }

        /// <summary>
        /// Gets an index style identifier for the given subentry level.
        /// </summary>
        private static StyleIdentifier GetStyleIdentifier(int subentryLevel)
        {
            switch (subentryLevel)
            {
                case 1:
                    return StyleIdentifier.Index1;
                case 2:
                    return StyleIdentifier.Index2;
                case 3:
                    return StyleIdentifier.Index3;
                case 4:
                    return StyleIdentifier.Index4;
                case 5:
                    return StyleIdentifier.Index5;
                case 6:
                    return StyleIdentifier.Index6;
                case 7:
                    return StyleIdentifier.Index7;
                case 8:
                    return StyleIdentifier.Index8;
                case 9:
                    return StyleIdentifier.Index9;
                default:
                    throw new ArgumentOutOfRangeException("subentryLevel");
            }
        }

        /// <summary>
        /// Writes a heading for the given start character.
        /// </summary>
        private void WriteHeading(char startChar)
        {
            FieldIndexAndTablesUtil.SetUpEntryParagraph(this, mWriteContext.DocumentBuilder, StyleIdentifier.IndexHeading);
            FieldIndexAndTablesUtil.EnsureEntryParagraphTabStop(mWriteContext.DocumentBuilder, null);

            Paragraph currentParagraph = mWriteContext.DocumentBuilder.CurrentParagraph;
            FieldIndexAndTablesUtil.SetFontForTabStopOrParagraphBreak(mWriteContext.DocumentBuilder.Document, currentParagraph);
            currentParagraph.ParaPr.KeepWithNext = true;

            string heading = mWriteContext.FieldCode.GetHeading(startChar);
            FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, heading);
        }

        /// <summary>
        /// Writes a subentry of the given entry with the specified index.
        /// </summary>
        private void WriteSubentry(
            Node dummyRefNode,
            IndexEntry entry,
            int subentryLevel,
            int subentryIndex,
            StyleIdentifier styleIdentifier)
        {
            // Initialize a dummy (i.e. temporary) reference node together with containing paragraph if it was not
            // initialized yet. This can happen in the following cases:
            //   - subentries are written on separate paragraphs (i.e. \r switch is not used).
            //   - subentries are written on the same paragraph (i.e. \r switch is used) but we have encountered
            //     a subentry with the min level (i.e. an entry).
            if (dummyRefNode == null)
            {
                FieldIndexAndTablesUtil.SetUpEntryParagraph(this, mWriteContext.DocumentBuilder, styleIdentifier);
                FieldIndexAndTablesUtil.EnsureEntryParagraphTabStop(mWriteContext.DocumentBuilder, null);

                dummyRefNode = FieldIndexAndTablesUtil.CreateAndInsertDummyRefNode(mWriteContext.DocumentBuilder);
                mWriteContext.DocumentBuilder.MoveTo(dummyRefNode);
            }

            IndexEntry subentry = entry.GetSubentry(subentryIndex);

            if (mWriteContext.FieldCode.RunSubentriesOnSameLine)
            {
                // Precede a subentry with a non-min level by non-overridable separator.
                if ((subentryIndex == 0) && (subentryLevel == IndexEntry.MinSubentryLevel + 1) && !entry.HasPageNumberInfos)
                {
                    FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, ": ");
                }
                else if (subentryLevel != IndexEntry.MinSubentryLevel)
                {
                    FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, "; ");
                }
            }

            Node precedingNode = dummyRefNode.PreviousSibling;

            CopySubentryNodes(subentry.NodeRange, dummyRefNode, styleIdentifier);

            Node startNode = (precedingNode != null) ? precedingNode.NextSibling : dummyRefNode.ParentNode.FirstChild;
            Node endNode = dummyRefNode.PreviousSibling;

            // Subentry nodes should be trimmed. MS Word behavior.
            TrimSubentryNodes(startNode, endNode, true);
            TrimSubentryNodes(endNode, startNode, false);

            WritePageNumbers(dummyRefNode, subentry, styleIdentifier);

            // Remove a dummy reference node if subentries are written on separate paragraphs (i.e. \r switch is not used)
            // and set it to null, it not to be used for subentries of this subentry.
            if (!mWriteContext.FieldCode.RunSubentriesOnSameLine)
            {
                dummyRefNode.Remove();
                dummyRefNode = null;
            }

            WriteSubentries(dummyRefNode, subentry, subentryLevel + 1);

            // If subentries are written on the same paragraph (i.e. \r switch is used) we should remove a dummy refererence
            // node only at the min level i.e. when all of the subentries are written.
            if ((subentryLevel == IndexEntry.MinSubentryLevel) && mWriteContext.FieldCode.RunSubentriesOnSameLine)
                dummyRefNode.Remove();
        }

        /// <summary>
        /// Copies subentry text nodes performing common modifications of their contents and font attributes.
        /// </summary>
        private void CopySubentryNodes(NodeRange range, Node refNode, StyleIdentifier styleIdentifier)
        {
            CompositeModifier compositeModifier = new CompositeModifier(
                new FieldXETextArgumentDecoderNodeModifier(range),
                new IndexEntryAttributeModifier(styleIdentifier, Document.Styles));

            if (range.Document != Document)
            {
                compositeModifier.AddModifier(
                    new ExternalDocumentModifier(
                        range.Document,
                        Document,
                        ImportFormatMode.UseDestinationStyles));
            }

            LinearNodeCopier.Copy(range, compositeModifier, refNode);
        }

        /// <summary>
        /// Performs trimming (i.e. removal of whitespaces) of subentry text nodes moving forward or backward.
        /// </summary>
        private static void TrimSubentryNodes(Node startNode, Node endNode, bool isForward)
        {
            Node node = startNode;
            while (node.NodeType == NodeType.Run)
            {
                Run run = (Run)node;

                // The node can be removed below so advance to the next one here.
                node = node.GetNearestSibling(isForward);

                // All of the whitespaces are replaced with spaces by FieldXETextArgumentDecoder
                // so check for spaces only as it works faster.
                if (StringUtil.ContainsOnlySpaces(run.Text))
                {
                    run.Remove();
                    if (run == endNode)
                        break;
                }
                else
                {
                    run.Text = isForward ? run.Text.TrimStart() : run.Text.TrimEnd();
                    break;
                }
            }
        }

        /// <summary>
        /// Writes page numbers for the given subentry.
        /// </summary>
        private void WritePageNumbers(Node refNode, IndexEntry subentry, StyleIdentifier styleIdentifier)
        {
            if (!subentry.HasPageNumberInfos)
                return;

            mWriteContext.StartWritePageNumbers();

            int lastFirstPageNumber = -1;
            int lastFirstSequenceValue = -1;

            foreach (IndexEntryPageNumberInfo pageNumberInfo in subentry.PageNumberInfos)
            {
                // Page number replacements should be written after all of the direct page numbers.
                if (pageNumberInfo.RefFieldCode.HasPageNumberReplacement)
                    continue;

                int firstSequenceValue = 0;
                int lastSequenceValue = 0;

                // Get a sequence value(s) for the page number if any sequence is used to build page numbers.
                if (mWriteContext.FieldCode.HasSequenceName)
                {
                    if (pageNumberInfo.HasPageRange)
                    {
                        firstSequenceValue = pageNumberInfo.FirstSequenceNumber;
                        lastSequenceValue = pageNumberInfo.LastSequenceNumber;
                    }
                    else
                    {
                        firstSequenceValue = pageNumberInfo.FirstSequenceNumber;
                    }
                }

                // MS Word merges sequential identical single page numbers. Let's call this a merged sequence.
                // Note, that such a sequence can contain a single item.
                //
                // A merged sequence can be interrupted by a valid or invalid page number range (a page number range
                // is considered to be invalid if corresponding bookmark is missing). The first page number of an interrupting
                // valid page range can be merged either if it satisfies the condition of single page numbers.
                //
                // A valid page range which has identical the first and the last page numbers is considered to be a single
                // page number (i.e. it does not break a merged sequence). IndexEntryPageNumberInfo.HasSinglePageRange
                // indicates whether this is the case.
                //
                // Only the first item of a merged sequence is written using corresponding formatting.
                //
                // As we do not know whether a merged sequence is going to be interrupted by a valid page number range,
                // we should write a merged sequence only at the moment of its interruption (not necessarily by a valid page
                // number range). However, page number ranges should be written immediately as they do not form any sequences.
                // That's the explanation of a cumbersome code below.
                if ((pageNumberInfo.FirstPageNumber != lastFirstPageNumber) || (firstSequenceValue != lastFirstSequenceValue))
                {
                    // A merged sequence is broken, so write it if any.
                    WriteSinglePageNumber(lastFirstSequenceValue);

                    lastFirstPageNumber = pageNumberInfo.FirstPageNumber;
                    lastFirstSequenceValue = firstSequenceValue;
                }

                if (pageNumberInfo.RefFieldCode.HasPageRangeBookmarkName && !pageNumberInfo.HasSinglePageRange)
                {
                    if (pageNumberInfo.HasPageRange)
                    {
                        // Write a valid page number range considering possible merged sequence presence.
                        WritePageNumberRange(pageNumberInfo, firstSequenceValue, lastSequenceValue);
                    }
                    else
                    {
                        // A merged sequence is broken, so write it if any.
                        WriteSinglePageNumber(firstSequenceValue);

                        // Write an invalid page number range.
                        WritePageRangeBookmarkError(pageNumberInfo, firstSequenceValue);
                    }
                }
                else if (!mWriteContext.HasCurrentSinglePageNumberInfo)
                {
                    // Remember the first item of a merged sequence.
                    mWriteContext.CurrentSinglePageNumberInfo = pageNumberInfo;
                }
            }

            // Write the last merged sequence if any.
            WriteSinglePageNumber(lastFirstSequenceValue);

            // And now it's time to write page number replacements.
            WritePageNumberReplacements(refNode, subentry, styleIdentifier);
        }

        /// <summary>
        /// Writes a single page number contained within the current write context if any.
        /// </summary>
        private void WriteSinglePageNumber(int sequenceValue)
        {
            if (!mWriteContext.HasCurrentSinglePageNumberInfo)
                return;

            StringBuilder pageNumberBuilder = GetPageNumberBuilder();
            IndexEntryPageNumberInfo pageNumberInfo = mWriteContext.CurrentSinglePageNumberInfo;

            BuildPageNumberBase(pageNumberBuilder, null, pageNumberInfo, pageNumberInfo.FirstPageNumber, sequenceValue);
            EndWritePageNumber(pageNumberBuilder, pageNumberInfo);
        }

        /// <summary>
        /// Writes a valid page number range. Considers a single page number contained within the current write context if any.
        /// </summary>
        private void WritePageNumberRange(IndexEntryPageNumberInfo pageNumberInfo, int firstSequenceValue, int lastSequenceValue)
        {
            IndexEntryPageNumberInfo firstPageNumberInfo = mWriteContext.HasCurrentSinglePageNumberInfo
                ? mWriteContext.CurrentSinglePageNumberInfo
                : pageNumberInfo;

            StringBuilder pageNumberBuilder = GetPageNumberBuilder();

            BuildPageNumberBase(
                pageNumberBuilder,
                null,
                firstPageNumberInfo,
                firstPageNumberInfo.FirstPageNumber,
                firstSequenceValue);

            BuildPageNumberPart(
                pageNumberBuilder,
                mWriteContext.FieldCode.PageRangeSeparator,
                firstPageNumberInfo.RefFieldCode,
                null);

            BuildPageNumberBase(
                pageNumberBuilder,
                null,
                pageNumberInfo,
                pageNumberInfo.LastPageNumber,
                lastSequenceValue);

            EndWritePageNumber(pageNumberBuilder, pageNumberInfo);
        }

        /// <summary>
        /// Writes an invalid (i.e. with missing corresponding bookmark) page number range.
        /// </summary>
        private void WritePageRangeBookmarkError(IndexEntryPageNumberInfo pageNumberInfo, int sequenceValue)
        {
            StringBuilder pageNumberBuilder = GetPageNumberBuilder();
            WritePageNumberPart(pageNumberBuilder, null);

            // Save current font properties.
            RunPr runPr = mWriteContext.DocumentBuilder.GetRunPrCopy();

            // Set bold formatting for the error message.
            IRunAttrSource runAttrSource = mWriteContext.DocumentBuilder;
            runAttrSource.SetRunAttr(FontAttr.Bold, AttrBoolEx.True);
            if (mWriteContext.DocumentBuilder.Document.FieldOptions.IsBidiTextSupportedOnUpdate)
                runAttrSource.SetRunAttr(FontAttr.BoldBi, AttrBoolEx.True);

            FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, "Error! Not a valid bookmark in entry on page ");

            // Restore font properties.
            mWriteContext.DocumentBuilder.SetFont(runPr, false);

            BuildPageNumberBase(
                pageNumberBuilder,
                null,
                pageNumberInfo,
                pageNumberInfo.FirstPageNumber,
                sequenceValue);

            EndWritePageNumber(pageNumberBuilder, pageNumberInfo);
        }

        /// <summary>
        /// Initializes a <see cref="StringBuilder"/> instance used to build page numbers.
        /// Appends page number or page number list separator to it according to the current write context.
        /// </summary>
        private StringBuilder GetPageNumberBuilder()
        {
            string separator = mWriteContext.UsePageNumberListSeparator
                ? mWriteContext.FieldCode.PageNumberListSeparator
                : mWriteContext.FieldCode.PageNumberSeparator;

            return new StringBuilder(separator);
        }

        /// <summary>
        /// Finalizes writing of a page number (i.e. flushes the buffer and changes the state of the current write context).
        /// </summary>
        private void EndWritePageNumber(StringBuilder pageNumberBuilder, IndexEntryPageNumberInfo pageNumberInfo)
        {
            WritePageNumberPart(pageNumberBuilder, pageNumberInfo.RefFieldCode);
            mWriteContext.EndWritePageNumber();
        }

        /// <summary>
        /// Appends a page number base (i.e. the number itself) to the specified page number builder.
        /// </summary>
        private void BuildPageNumberBase(
            StringBuilder pageNumberBuilder,
            FieldCodeXE lastFieldCodeXE,
            IndexEntryPageNumberInfo pageNumberInfo,
            int pageNumber,
            int sequenceValue)
        {
            string formattedPageNumber = pageNumberInfo.RefField.FormatPageNumber(pageNumber);
            string part = mWriteContext.FieldCode.HasSequenceName ? sequenceValue.ToString() : formattedPageNumber;

            BuildPageNumberPart(pageNumberBuilder, part, lastFieldCodeXE, pageNumberInfo.RefFieldCode);

            if (mWriteContext.FieldCode.HasSequenceName)
            {
                // These parts use the same text formatting, so append them right away.
                pageNumberBuilder.Append(mWriteContext.FieldCode.SequenceSeparator);
                pageNumberBuilder.Append(formattedPageNumber);
            }
        }

        /// <summary>
        /// A core method to build any page number part.
        /// Flushes the buffer (i.e. contents of the given page number builder) if text formatting has changed.
        /// </summary>
        private void BuildPageNumberPart(
            StringBuilder pageNumberBuilder,
            string part,
            FieldCodeXE lastFieldCodeXE,
            FieldCodeXE currentFieldCodeXE)
        {
            if ((NeedInvertBold(currentFieldCodeXE) != NeedInvertBold(lastFieldCodeXE)) ||
                (NeedInvertItalic(currentFieldCodeXE) != NeedInvertItalic(lastFieldCodeXE)))
            {
                WritePageNumberPart(pageNumberBuilder, lastFieldCodeXE);
            }

            pageNumberBuilder.Append(part);
        }

        /// <summary>
        /// Writes a page number part using text formatting corresponding to the specified XE field code.
        /// </summary>
        private void WritePageNumberPart(StringBuilder pageNumberBuilder, FieldCodeXE fieldCodeXE)
        {
            if (pageNumberBuilder.Length == 0)
                return;

            // Flush the buffer.
            string pageNumberPart = pageNumberBuilder.ToString();
            pageNumberBuilder.Length = 0;

            bool invertBold = NeedInvertBold(fieldCodeXE);
            bool invertItalic = NeedInvertItalic(fieldCodeXE);

            // If there are no font attrs to invert, write the part right away.
            if (!invertBold && !invertItalic)
            {
                FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, pageNumberPart);
                return;
            }

            // Save current font attrs.
            RunPr runPr = mWriteContext.DocumentBuilder.GetRunPrCopy();

            // Invert font attrs where needed.
            bool isBidiTextSupported = mWriteContext.DocumentBuilder.Document.FieldOptions.IsBidiTextSupportedOnUpdate;
            if (invertBold)
                InvertAttrBoolEx(mWriteContext.DocumentBuilder, FontAttr.Bold);
            if (invertItalic)
                InvertAttrBoolEx(mWriteContext.DocumentBuilder, FontAttr.Italic);
            if (invertBold && isBidiTextSupported)
                InvertAttrBoolEx(mWriteContext.DocumentBuilder, FontAttr.BoldBi);
            if (invertItalic && isBidiTextSupported)
                InvertAttrBoolEx(mWriteContext.DocumentBuilder, FontAttr.ItalicBi);

            FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, pageNumberPart);

            // Restore old font attrs.
            mWriteContext.DocumentBuilder.SetFont(runPr, false);
        }

        private static bool NeedInvertBold(FieldCodeXE fieldCodeXE)
        {
            return (fieldCodeXE != null) && fieldCodeXE.IsBold;
        }

        private static bool NeedInvertItalic(FieldCodeXE fieldCodeXE)
        {
            return (fieldCodeXE != null) && fieldCodeXE.IsItalic;
        }

        /// <summary>
        /// Inverts <see cref="AttrBoolEx"/> value for a font attr specified by its key for the given
        /// <see cref="IRunAttrSource"/> instance.
        /// </summary>
        private static void InvertAttrBoolEx(IRunAttrSource runAttrSource, int key)
        {
            AttrBoolEx value = (AttrBoolEx)InlineHelper.FetchAttr(runAttrSource, key);
            runAttrSource.SetRunAttr(key, value.Invert());
        }

        /// <summary>
        /// Writes page number replacements for the given subentry if any.
        /// </summary>
        private void WritePageNumberReplacements(Node refNode, IndexEntry subentry, StyleIdentifier styleIdentifier)
        {
            if (!subentry.HasPageNumberReplacements)
                return;

            foreach (IndexEntryPageNumberInfo pageNumberInfo in subentry.PageNumberInfos)
            {
                if (pageNumberInfo.RefFieldCode.HasPageNumberReplacement)
                    WritePageNumberReplacement(refNode, pageNumberInfo.RefFieldCode, styleIdentifier);
            }
        }

        /// <summary>
        /// Writes a single page number replacement.
        /// </summary>
        private void WritePageNumberReplacement(Node refNode, FieldCodeXE fieldCodeXE, StyleIdentifier styleIdentifier)
        {
            string separator;
            if (mWriteContext.UsePageNumberListSeparator)
            {
                separator = mWriteContext.FieldCode.PageNumberListSeparator;
            }
            else if (HasPageNumberSeparator)
            {
                // MS Word uses page number separator here if it is overridden.
                separator = mWriteContext.FieldCode.PageNumberSeparator;
            }
            else
            {
                // MS Word uses cross reference separator only if page number separator is not overridden.
                separator = mWriteContext.FieldCode.CrossReferenceSeparator;
            }

            FieldTextHelper.WriteTextBidiAware(this, mWriteContext.DocumentBuilder, separator);
            CopySubentryNodes(fieldCodeXE.PageNumberReplacementRange, refNode, styleIdentifier);
            mWriteContext.EndWritePageNumber();
        }

        internal override IBidiParagraphLevelOverride GetBidiParagraphLevelOverride()
        {
            // Experiments with MS Word show that SectPr.Bidi influences on this.
            Section parentSection = (Section)Start.GetAncestor(NodeType.Section);
            return ConstantBidiParagraphLevelOverride.GetInstance(parentSection.SectPr.Bidi);
        }

        /// <summary>
        /// Returns a value indicating whether the field's code contains any invalid arguments.
        /// </summary>
        /// <param name="updater">
        /// A <see cref="FieldUpdater"/> instance.
        /// It is passed as an argument as the method can be used while updating of another field (typically SEQ).
        /// </param>
        /// <returns>
        /// True if any of the field code arguments is not valid.
        /// </returns>
        internal bool IsError(FieldUpdater updater)
        {
            FieldIndexValidateResult validateResult = Validate(updater);
            return validateResult.IsError;
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case RunSubentriesOnSameLineSwitch:
                case UseYomiSwitch:
                    return FieldSwitchType.Flag;
                case BookmarkNameSwitch:
                case NumberOfColumnsSwitch:
                case SequenceSeparatorSwitch:
                case PageNumberSeparatorSwitch:
                case EntryTypeSwitch:
                case PageRangeSeparatorSwitch:
                case HeadingSwitch:
                case CrossReferenceSeparatorSwitch:
                case PageNumberListSeparatorSwitch:
                case LetterRangeSwitch:
                case SequenceNameSwitch:
                case LanguageIdSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark that marks the portion of the document used to build the index.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(BookmarkNameSwitch); }
            set { FieldCodeCache.SetSwitch(BookmarkNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the number of columns per page used when building the index.
        /// </summary>
        public string NumberOfColumns //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(NumberOfColumnsSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(NumberOfColumnsSwitch, value); }
        }

        internal bool HasNumberOfColumnsSwitch
        {
            get { return FieldCodeCache.HasSwitch(NumberOfColumnsSwitch); }
        }

        internal NullableInt32 NumberOfColumnsAsInt32
        {
            get { return FieldCodeCache.GetSwitchArgumentAsInt32(NumberOfColumnsSwitch); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate sequence numbers and page numbers.
        /// </summary>
        public string SequenceSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SequenceSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(SequenceSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate an index entry and its page number.
        /// </summary>
        public string PageNumberSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a page number separator is overridden through the field's code.
        /// </summary>
        public bool HasPageNumberSeparator
        {
            get { return FieldCodeCache.HasSwitch(PageNumberSeparatorSwitch); }
        }

        /// <summary>
        /// Gets or sets an index entry type used to build the index.
        /// </summary>
        public string EntryType
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryTypeSwitch); }
            set { FieldCodeCache.SetSwitch(EntryTypeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate the start and end of a page range.
        /// </summary>
        public string PageRangeSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageRangeSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(PageRangeSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a heading that appears at the start of each set of entries for any given letter.
        /// </summary>
        public string Heading
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(HeadingSwitch); }
            set { FieldCodeCache.SetSwitch(HeadingSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate cross references and other entries.
        /// </summary>
        public string CrossReferenceSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(CrossReferenceSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(CrossReferenceSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate two page numbers in a page number list.
        /// </summary>
        public string PageNumberListSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberListSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberListSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a range of letters to which limit the index.
        /// </summary>
        public string LetterRange
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LetterRangeSwitch); }
            set { FieldCodeCache.SetSwitch(LetterRangeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether run subentries into the same line as the main entry.
        /// </summary>
        public bool RunSubentriesOnSameLine
        {
            get { return FieldCodeCache.HasSwitch(RunSubentriesOnSameLineSwitch); }
            set { FieldCodeCache.SetSwitch(RunSubentriesOnSameLineSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of a sequence whose number is included with the page number.
        /// </summary>
        public string SequenceName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SequenceNameSwitch); }
            set { FieldCodeCache.SetSwitch(SequenceNameSwitch, value); }
        }

        /// <summary>
        /// Gets a value indicating whether a sequence should be used while the field's result building.
        /// </summary>
        public bool HasSequenceName
        {
            get { return FieldCodeCache.HasSwitch(SequenceNameSwitch); }
        }

        /// <summary>
        /// Gets or sets whether to enable the use of yomi text for index entries.
        /// </summary>
        public bool UseYomi
        {
            get { return FieldCodeCache.HasSwitch(UseYomiSwitch); }
            set { FieldCodeCache.SetSwitch(UseYomiSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the language ID used to generate the index.
        /// </summary>
        public string LanguageId
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LanguageIdSwitch); }
            set { FieldCodeCache.SetSwitch(LanguageIdSwitch, value); }
        }

        private FieldIndexWriteContext mWriteContext;

        private const string BookmarkNameSwitch = "\\b";
        private const string NumberOfColumnsSwitch = "\\c";
        private const string SequenceSeparatorSwitch = "\\d";
        private const string PageNumberSeparatorSwitch = "\\e";
        private const string EntryTypeSwitch = "\\f";
        private const string PageRangeSeparatorSwitch = "\\g";
        private const string HeadingSwitch = "\\h";
        private const string CrossReferenceSeparatorSwitch = "\\k";
        private const string PageNumberListSeparatorSwitch = "\\l";
        private const string LetterRangeSwitch = "\\p";
        private const string RunSubentriesOnSameLineSwitch = "\\r";
        private const string SequenceNameSwitch = "\\s";
        private const string UseYomiSwitch = "\\y";
        private const string LanguageIdSwitch = "\\z";

        private class LinearNodeCopier
        {
            private LinearNodeCopier(NodeRange range, INodeModifier compositeModifier, Node refNode)
            {
                DocumentBase document = refNode.Document;

                Run run = new Run(document);
                Paragraph paragraph = new Paragraph(document);
                Body body = new Body(document);
                Section section = new Section(document);

                section.AppendChild(body);
                body.AppendChild(paragraph);
                paragraph.AppendChild(run);

                mDocument = document;
                mVirtualDomRefNode = run;
                mRefNode = refNode;
                mRange = range;
                mCompositeModifier = compositeModifier;
            }

            public static void Copy(NodeRange range, INodeModifier compositeModifier, Node refNode)
            {
                LinearNodeCopier copier = new LinearNodeCopier(range, compositeModifier, refNode);
                copier.Copy();
            }

            private void Copy()
            {
                NodeRange result = NodeCopier.CopyWithoutFields(
                    mRange,
                    mVirtualDomRefNode,
                    mCompositeModifier,
                    null,
                    true,
                    NodeCopierOptions.UseSourceStartAncestorPr | NodeCopierOptions.SkipCrossStructureAnnotations | NodeCopierOptions.CloneNode);

                if (result == null)
                    return;

                foreach (Node node in result)
                {
                    if (NeedSkipNode(node))
                        continue;

                    switch (node.NodeType)
                    {
                        case NodeType.Paragraph:
                            VisitParagraph((Paragraph)node);
                            break;

                        case NodeType.Row:
                            VisitTableRow((Row)node);
                            break;

                        default:
                            if (node is IInline)
                                VisitInline(node);

                            break;
                    }
                }
            }

            private void VisitParagraph(Paragraph paragraph)
            {
                if (mLastTableRow != null && paragraph.ParentRow != mLastTableRow)
                    AppendTableRowSeparator();

                AppendParagraphSeparator();

                mParagraphSeparator = new Run(mDocument, Separator, paragraph.ParagraphBreakRunPr);
            }

            private void VisitTableRow(Row row)
            {
                AppendParagraphSeparator();
                AppendTableRowSeparator();

                mTableRowSeparator = new Run(mDocument, Separator);
                mLastTableRow = row;
            }

            private void VisitInline(Node node)
            {
                mLastInline = node;
                AppendResultNode(node.Clone(true));
            }

            private bool NeedSkipNode(Node node)
            {
                if (mLastInline == null)
                    return false;

                if (!node.IsAncestorNode(mLastInline))
                    return false;

                return true;
            }

            private void AppendParagraphSeparator()
            {
                if (mParagraphSeparator == null)
                    return;

                AppendResultNode(mParagraphSeparator);

                mParagraphSeparator = null;
            }

            private void AppendTableRowSeparator()
            {
                if (mTableRowSeparator == null)
                    return;

                AppendResultNode(mTableRowSeparator);

                mLastTableRow = null;
                mTableRowSeparator = null;
            }

            private void AppendResultNode(Node node)
            {
                mRefNode.InsertPrevious(node);
            }

            private Run mParagraphSeparator;
            private Run mTableRowSeparator;
            private Row mLastTableRow;
            private Node mLastInline;

            private readonly Node mVirtualDomRefNode;
            private readonly NodeRange mRange;
            private readonly INodeModifier mCompositeModifier;
            private readonly Node mRefNode;
            private readonly DocumentBase mDocument;

            private const string Separator = " ";
        }
    }
}
