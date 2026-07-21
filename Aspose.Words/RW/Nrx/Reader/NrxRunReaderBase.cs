// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/04/2010 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Charset;
using Aspose.Fonts;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fonts;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Tables;
using Aspose.Xml;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class, provides methods for reading run of text in DOCX and WML.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxRunReaderBase
    {
        protected NrxRunReaderBase(NrxStoryReaderBase storyReader,
            NrxRunPrReaderBase runPrReader,
            NrxParaReaderBase paraReader,
            NrxFldCharReaderBase fldCharReader,
            NrxFldSimpleReader fldSimpleReader,
            NrxHyperlinkReaderBase hyperlinkReader,
            INrxVmlReader vmlReader)
        {
            Debug.Assert(storyReader != null);
            Debug.Assert(runPrReader != null);
            Debug.Assert(paraReader != null);
            Debug.Assert(fldCharReader != null);
            Debug.Assert(fldSimpleReader != null);
            Debug.Assert(hyperlinkReader != null);

            mStoryReader = storyReader;
            mRunPrReader = runPrReader;
            mParaReader = paraReader;
            mFldCharReader = fldCharReader;
            mFldSimpleReader = fldSimpleReader;
            mHyperlinkReader = hyperlinkReader;
            mVmlReader = vmlReader;
        }

        /// <summary>
        /// Reads 'w:r' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        /// <param name="reader">DocxParser/WmlReader to read from. Should be positioned to element start.</param>
        internal void Read(NrxDocumentReaderBase reader)
        {
            Read(reader, null);
        }

        internal void Read(NrxDocumentReaderBase reader, RunPr runPr)
        {
            ReadCore(reader, runPr);
        }

        internal void ReadText(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string elementName = xmlReader.LocalName;

            // andrnosk: WORDSNET-4664 Get "Space" attribute either from document (WML) or from current run (DOCX).
            bool preserveSpace = reader.PreserveSpace || (xmlReader.ReadAttribute("space", "") == "preserve") || (elementName != "t");

            while (xmlReader.ReadChildWithTextValues(elementName, AnyXmlTextHandlingConsts.TextAndSignificant))
            {
                switch (xmlReader.LocalName)
                {
                    case "":
                        {
                            string t = xmlReader.Value;

                            // andrnosk: WORDSNET-5553 Simulate MS Word behavior replace \r\n chars with white space.
                            t = t.Replace(ControlChar.CrLf, ControlChar.Space);

                            // WORDSNET-7391 Trim LF at start and replace LF with Space in the rest of string.
                            t = t.TrimStart(ControlChar.LineFeedChar);

                            t = t.Replace(ControlChar.Lf, ControlChar.Space);
                            t = t.Replace(ControlChar.Cr, ControlChar.Space);

                            // andrnosk: WORDSNET-4664 If PreserveSpace option of Run is not set then remove all leading and trailing white-spaces and tabs.
                            if (!preserveSpace)
                            {
                                t = t.Trim(gWhitespaceChars);

                                // WORDSNET-3020 Word replaces tab characters with space if xml space is not preserved.
                                t = t.Replace('\t', ' ');
                            }

                            if (IsDefaultTextInputChar(t))
                                ReadSpecialChar(reader, runPr, t[0]);
                            else if (!IsJavaWoodstoxWhitespace(t))
                                xmlReader.RunTextBuilder.Append(t);
                            break;
                        }
                    case "noBreakHyphen":
                        xmlReader.RunTextBuilder.Append(ControlChar.NonBreakingHyphen);
                        break;
                    case "p":
                        // RESILIENCY 20010 - a "p" tag is found inside "r" element.
                        ResilientParaRead(reader, runPr);
                        break;
                    case "tbl":
                        // rework for RESILIENCY 3760. "tbl" tag is found inside "r" element.
                        // DD: after "p" tag processing was added per fix for 20010, I had to add table processing as well.
                        ResilientTableRead(reader);
                        break;
                    default:
                        // There can be inline elements like pict or drawing inside 't' element.
                        ReadTextLevelElement(reader, runPr);
                        break;
                }
            }
        }

        /// <summary>Resilient version of <see cref="Read(NrxDocumentReaderBase)"/> allowing to specify runPr of a parent w:r for nested w:r.</summary>
        /// <param name="reader">DocxParser/WmlReader to read from. Should be positioned to element start.</param>
        /// <param name="runPr">When non null, used for resiliency, when we deal with nested runs inside runs, etc. in these cases we need to consider properties
        /// of runs for their nested runs.</param>
        private void ReadCore(NrxDocumentReaderBase reader, RunPr runPr)
        {
            if (runPr == null)
                runPr = new RunPr();

            // Read attributes.
            NrxXmlReader xmlReader = reader.XmlReader;

            if (!reader.LoadOptions.SkipFormatting)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    // The reading below is not entirely correct because not all rsid attributes are in the model now.
                    switch (xmlReader.LocalName)
                    {
                        case "rsidDel":
                            // RK Not yet sure where to store in the model.
                            // Specifies a unique identifier used to track the editing session when
                            // the run was deleted from the main document.
                            break;
                        case "rsidR":
                            {
                                // Specifies a unique identifier used to track the editing session when
                                // the run was added to the main document.
                                int rsidR = NrxXmlUtil.TryHexToInt(xmlReader.Value);
                                if ((rsidR != int.MinValue) && (rsidR != xmlReader.RsidRDefault))
                                    runPr.SetAttr(FontAttr.RsidR, rsidR);
                                break;
                            }
                        case "rsidRPr":
                            {
                                // Specifies a unique identifier used to track the editing session when
                                // the run properties were last modified in the main document.
                                int rsidDpr = NrxXmlUtil.TryHexToInt(xmlReader.Value);
                                if (rsidDpr != int.MinValue)
                                    runPr.SetAttr(FontAttr.RsidRPr, rsidDpr);
                                break;
                            }
                        default:
                        {
                            // Ignored.
                            break;
                        }
                    }
                }
            }

            ReadRChildren(reader, runPr, false);

            FlushRun(reader, runPr);
        }

        /// <summary>
        /// Reads elements that can occur as a child of an "r" or "mc:Fallback" element.
        /// </summary>
        private void ReadRChildren(NrxDocumentReaderBase reader, RunPr runPr, bool selfRecursive)
        {
            // RK Clones or runPr are not done everywhere here. We should fix sometime.

            NrxXmlReader xmlReader = reader.XmlReader;

            // Indicates whether this run should be separated.
            bool createNewRun = false;
            bool isRunCreated = false;

            // Need to do this to find the name of the element whose contents we are reading.
            // It can be "r" or "Fallback".
            xmlReader.MoveToElement();
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                bool rReadingCompleted = false;

                switch (xmlReader.LocalName)
                {
                    case "rPr":
                    {
                        // WORDSNET-8064 If this run contains several runPr with same prefix, we should create new run for each runPr.
                        // WORDSNET-13481 MS Word does it only when new 'w:t' or 'm:t' element is occurred.
                        // WORDSNET-17225 MS Word does it for 'w:tab' too.
                        if (createNewRun)
                        {
                            FlushRun(reader, runPr);
                            runPr = new RunPr();
                            createNewRun = false;
                            isRunCreated = true;
                        }

                        mRunPrReader.Read(reader, runPr);
                        break;
                    }
                    case "rFonts":
                        mRunPrReader.ReadFonts(xmlReader, runPr);
                        break;
                    case "ruby":
                        ReadRuby(reader, runPr);
                        break;
                    case "t":
                    case "instrText":
                    case "delText":
                    case "delInstrText":
                    {
                        // It does not matter how the text is declared in WordML - 'w:t', 'w:instrText', etc.
                        // It all goes to run's text anyway.
                        // The only exception is special chars. In WordML they are written as
                        //   <w:r>
                        //     <w:t>[special char]</w:t>
                        //   </w:r>
                        // where [special char] is a special char symbol.
                        ReadText(reader, runPr);
                        createNewRun = true;
                        break;
                    }

                    case "fldChar":
                        // WORDSNET-28498 Avoid run properties to be shared across nodes.
                        FlushRun(reader, runPr);
                        runPr = runPr.Clone();
                        ReadFldChar(reader, runPr);
                        runPr = runPr.Clone();
                        break;

                    case "tab":
                        // WORDSNET-19151 MS Word ignores the 'tab' element inside the unsupported child node.
                        if (!selfRecursive)
                        {
                            xmlReader.RunTextBuilder.Append(ControlChar.Tab);
                            createNewRun = true;
                        }
                        break;
                    case "cr":
                        xmlReader.RunTextBuilder.Append(ControlChar.ParagraphBreak);
                        break;
                    case "softHyphen":
                        xmlReader.RunTextBuilder.Append(ControlChar.OptionalHyphen);
                        break;
                    case "noBreakHyphen":
                        xmlReader.RunTextBuilder.Append(ControlChar.NonBreakingHyphen);
                        break;
                    case "sym":
                        ReadSymbol(reader, runPr);
                        // WORDSNET-26245. Create new attribute collection for runs after symbolic run.
                        runPr = runPr.Clone();
                        isRunCreated = true;
                        break;
                    case "endnoteReference": // DOCX.
                    case "endnote": // WML.
                        rReadingCompleted = ReadFootnote(reader, runPr, FootnoteType.Endnote);
                        break;
                    case "footnoteReference": // DOCX.
                    case "footnote": // WML.
                        rReadingCompleted = ReadFootnote(reader, runPr, FootnoteType.Footnote);
                        break;
                    case "footnoteRef":
                    case "endnoteRef":
                        ReadSpecialChar(reader, runPr, ControlChar.FootnoteRefChar);
                        break;
                    case "separator":
                        ReadSpecialChar(reader, runPr, ControlChar.FootnoteSeparatorChar);
                        break;
                    case "continuationSeparator":
                        ReadSpecialChar(reader, runPr, ControlChar.FootnoteContinuationChar);
                        break;
                    case "commentReference": // DOCX.
                    case "annotation": // WML.
                    case "content":
                        if(!reader.IsEquationXmlReader)
                            ReadComment(reader, runPr);
                        break;
                    case "annotationRef":
                        ReadSpecialChar(reader, runPr, ControlChar.AnnotationRefChar);
                        break;
                    case "pgNum":
                        ReadLegacyPageNumber(reader, runPr);
                        break;
                    case "AlternateContent": // Word 2010 DrawingML Extensions.
                        // We do not check namespaces at all, this is not good.
                        // This one is xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006".
                        // WORDSNET-14642 Create copy of the run properties to avoid references duplication
                        // to one "Fallback" from shapes.
                        ReadAlternateContent(reader, runPr.Clone());
                        break;
                    case "ptab":
                        ReadAbsolutePositionTab(reader, runPr);
                        break;
                    case "commentRangeStart":
                        // WORDSNET-23757 Support of commentRangeStart in unexpected places.
                        FlushRun(reader, runPr);
                        DocxCommentsReader.ReadCommentRangeStart(reader);
                        break;
                    case "commentRangeEnd":
                        // WORDSNET-23757 Support of commentRangeEnd in unexpected places.
                        FlushRun(reader, runPr);
                        DocxCommentsReader.ReadCommentRangeEnd(reader);
                        break;
                    default:
                        // AN: Move all cases that occur in incorrect place ('p' or 't' element) from this switch to ReadTextLevelElement method
                        // to avoid duplication of the code. I do not wand to move whole switch to this method, because using
                        // ReadTextLevelElement we process text level elements in incorrect places. This is not documented, so we do not know
                        // which of these elements can be in incorrect place and how we should handle them. Wait while a customer report a problem.
                        if (!ReadTextLevelElement(reader, runPr))
                        {
                            // WORDSNET-18361 Read child nodes of unsupported tag and place the read content into the current run.
                            ReadRChildren(reader, runPr, true);
                        }
                        break;
                }

                if (rReadingCompleted)
                    break;
            }

            // WORDSNET-17225 Only needed when new runs were created inside this method.
            // We need to flush with the new last created runPr.
            if (isRunCreated)
                FlushRun(reader, runPr);
        }

        /// <summary>
        /// Reads "ptab" element and attributes.
        /// </summary>
        private static void ReadAbsolutePositionTab(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            AbsolutePositionTab ptab = new AbsolutePositionTab(reader.Document, runPr);
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "alignment":
                        ptab.Alignment = DocxEnum.DocxToAbsolutePositionTabAlignment(xmlReader.Value);
                        break;
                    case "leader":
                        ptab.LeaderChar = DocxEnum.DocxToAbsolutePositionTabLeaderChar(xmlReader.Value);
                        break;
                    case "relativeTo":
                        ptab.PositioningBase = DocxEnum.DocxToAbsolutePositionTabPositioningBase(xmlReader.Value);
                        break;
                    default:
                        break;
                }
            }

            reader.AddChild(ptab);
        }

        /// <summary>
        /// Adds <see cref="Run"/> to the model if non-empty run text elements were previously encountered.
        /// </summary>
        internal static void FlushRun(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string runText = xmlReader.RunTextBuilder.ToString();
            if (StringUtil.HasChars(runText) || runPr.Contains(FontAttr.Ruby))
            {
                Run run = new Run(reader.Document, runText, runPr);
                reader.AddChild(run);

                // AM. Word ignores hpsBase attribute from ruby properties and takes it from ruby run.
                // That's why we need to update it after run is placed to DOM.
                if (runPr.Contains(FontAttr.Ruby))
                    ProcessRuby(run);

                FormatRevisionUtil.ConvertToPositiveDifference(run);

                reader.DetectToggleIssue(run);

                // Reset run text builder.
                xmlReader.RunTextBuilder.Length = 0;
            }
        }

        /// <summary>
        /// If the current element is inline, then read it into the model and returns true.
        ///
        /// Reworked fix of issues: 1589, 11002, 14837, 20010, 3760, 22434.
        /// AN: There were several problems where Aspose.Words lost content because elements were in wrong place.
        /// Theoretically, any of elements that occurs in 'r' element can also occur in 't' or 'p' element,
        /// but for now I added cases only for elements that already occurred in 't' or 'p' element.
        /// To handle this, we use this method to read inline elements, if there is nothing to read just continue processing in regular way.
        /// </summary>
        internal bool ReadTextLevelElement(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            bool readElement = true;
            switch (xmlReader.LocalName)
            {
                // DD: Looking at all these resiliency cases I think someday we need to rework the design so it
                // is more generic towards fixing wrong placements of OOXML elements.
                // Consider this whenever we fix more of such cases across run or para levels.
                case "pic":
                case "pict":
                case "object":
                    // RESILIENCY 1589 A "pict" element occurs inside a "t" element and MS Word reads it well. So do we now.

                    // WORDSNET-26269 Shape and next run shared the same RunPr.
                    // But we cannot start with new RunPr if we are within AlternateContent section.
                    readElement = ReadVmlObject(reader, reader.IsInAlternateContent ? runPr : runPr.Clone());
                    break;
                case "drawing":
                    // RESILIENCY 22434 - a "drawing" tag is found inside "t" element.
                    ReadDrawingML(reader, runPr);
                    break;
                case "AlternateContent":
                    // WORDSNET-23885 "AlternateContent" inside "p" element.
                    ReadAlternateContent(reader, runPr.Clone());
                    break;
                case "br":
                    // RESILIENCY 11002 / 14837 - a "br" element occurs inside "t" element.
                    xmlReader.RunTextBuilder.Append(ReadBreak(xmlReader, runPr));
                    break;
                case "fldChar":
                    // RESILIENCY 17632  - a "fldChar" tag is found inside "p" element.
                    ReadFldChar(reader, runPr);
                    break;
                case "fldSimple":
                    // RESILIENCY 16450 - a 'fldSimple' tag is found inside 'r' element.
                    mFldSimpleReader.Read(reader, runPr);
                    break;
                case "r":
                    // WORDSNET-5056 'r' inside 'r' element.
                    // WORDSNET-13961 Separate attributes for inner run.
                    ReadCore(reader, runPr.Clone());
                    break;
                case "tbl":
                    ResilientTableRead(reader);
                    break;
                case "body":
                    ResilientBodyRead(reader);
                    break;
                case "rPr":
                    mRunPrReader.Read(reader, runPr);
                    break;
                case "p":
                    // WORDSNET-9791 there is paragraph 'p' element inside 'sdtContent' of inline level sdt.
                    // WORDSNET-10434 'p' elements inside 'r'.
                    readElement = ResilientParaReadInsideInline(reader, runPr);
                    break;
                case "oMath":
                    // WORDSNET-19522 Math equations placed as child of a run.
                    DocxMathReader.ReadOMath(reader);
                    break;
                case "hlink":
                case "hyperlink":
                    // WORDSNET-13627 processing undocumented case in spec with hyper-link inside “Run” element.
                    ResilientHyperlink(reader, runPr);
                    break;
                case "sect":
                    // WORDSNET-13416 'sect' element inside run. This element is just a hint for MS Word,
                    // so skip it and continue reading its content.
                    ReadRChildren(reader, runPr, false);
                    break;
                case "sectPr":
                    // WORDSNET-13416 We have skipped 'sect' element above and continue reading its content.
                    // There is 'sectPr' element inside it.
                    reader.ReadGlobalSectPr(false);
                    break;
                case "bookmarkStart":
                    // WORDSNET-19438 Bookmark elements are child of w:r element.
                    FlushRun(reader, runPr);
                    DocxBookmarkReader.ReadStart(reader);
                    break;
                case "bookmarkEnd":
                    FlushRun(reader, runPr);
                    DocxBookmarkReader.ReadEnd(reader);
                    break;
                case "t":
                    // WORDSNET-24761 't' element inside 'fldSimple' element.
                    ReadText(reader, runPr);
                    FlushRun(reader, runPr);
                    break;
                default:
                    readElement = ReadFormatSpecific(xmlReader.LocalName, reader, runPr);
                    break;
            }

            return readElement;
        }

        [JavaThrows(true)]
        protected abstract bool ReadFormatSpecific(string localName, NrxDocumentReaderBase reader, RunPr runPr);

        /// <summary>
        /// Make additional processing related to contained Ruby element.
        /// </summary>
        private static void ProcessRuby(Run rubyRun)
        {
            int size = (int)InlineHelper.FetchAttr(rubyRun, FontAttr.Size);
            Ruby ruby = (Ruby)rubyRun.RunPr[FontAttr.Ruby];
            ruby.BaseSize = size;

            ruby.Validate(rubyRun);
        }

        private static bool IsDefaultTextInputChar(String t)
        {
            return (t.Length == 1) && (t[0] == ControlChar.DefaultTextInputChar);
        }

        /// <summary>
        /// RK This is a workaround for the problem where in one specific case Woodstox XML parser on Java reports text,
        /// whereas .NET parser reports whitespace. It is the case of an indented element with start and end tags, but no text.
        /// [parent]
        ///     [child]
        ///     [/child]
        /// [parent]
        ///
        /// Woodstox will report that [child] has "/r/n    " text inside it. It does not seem occur too often - only two tests had this problem.
        /// I cannot find a better solution hence a workaround.
        /// </summary>
        private static bool IsJavaWoodstoxWhitespace(String t)
        {
            if ((t.Length > 0) && (t[0] == '\r'))
                return StringUtil.ContainsOnlyWhitespaces(t);
            else
                return false;
        }

        /// <summary>
        /// Process case when found hyper-link inside run.
        /// </summary>
        /// <param name="reader">Document reader.</param>
        /// <param name="runPr">Object that contains "Run" attributes.</param>
        private void ResilientHyperlink(NrxDocumentReaderBase reader, RunPr runPr)
        {
            // Add read "Run" data before hyper-link to the model.
            FlushRun(reader, runPr);
            mHyperlinkReader.Read(reader);
        }

        /// <summary>
        /// andrnosk: WORDSNET-9226 Found 'body' inside run: Finish current para, add a data from body and then
        /// create new para.
        /// </summary>
        private void ResilientBodyRead(NrxDocumentReaderBase reader)
        {
            Paragraph paragraph = (Paragraph)reader.CurContainer;
            reader.PopContainer(NodeType.Paragraph);

            // Remove empty paragraph, Word does the same in this case.
            if (paragraph.GetChildNodes(NodeType.Any, false).Count == 0)
                paragraph.Remove();

            mStoryReader.ReadChild(reader);

            reader.AddAndPushContainer(new Paragraph(reader.Document));
        }


        /// <summary>
        /// Reads table placed at inline level.
        /// </summary>
        private void ResilientTableRead(NrxDocumentReaderBase reader)
        {
            switch (reader.CurContainer.NodeType)
            {
                case NodeType.Paragraph:
                    ResilientTableIntoParaRead(reader);
                    break;
                // Table occurred into inline SDT.
                case NodeType.StructuredDocumentTag:
                    ResilientTableIntoSdtRead(reader);
                    break;
                default:
                    reader.XmlReader.IgnoreElementUnexpected(reader.IsDocx);
                    break;
            }
        }

        /// <summary>
        /// Implements reading table from inline SDT.
        /// </summary>
        private void ResilientTableIntoSdtRead(NrxDocumentReaderBase reader)
        {
            InlineMarkupResolver resolver = reader.InlineResolver;
            StructuredDocumentTag sdt = resolver.TopLevelInline;

            // Reserve position for table in the node sequence.
            int index = resolver.StartPendingInline((StructuredDocumentTag)reader.CurContainer, NodeType.Table);

            // Read table.
            ReadTable(reader);

            // Resolve it later when inline SDT will be read completely.
            resolver.EndPendingInline(sdt, (Table)reader.CurContainer.LastNonAnnotationChild, index);
        }

        /// <summary>
        /// WORDSNET-9791 Found 'p' inside 'sdtContent' of inline level sdt.
        /// The contents of the first paragraph moved straight into sdtContent, all follow-up paragraphs move the outside, like MSW.
        /// </summary>
        private void ResilientParaReadInsideInlineSdt(NrxDocumentReaderBase reader)
        {
            InlineMarkupResolver resolver = reader.InlineResolver;
            StructuredDocumentTag sdt = resolver.TopLevelInline;

            // Reserve position for paragraph in the node sequence.
            int index = resolver.StartPendingInline((StructuredDocumentTag)reader.CurContainer, NodeType.Paragraph);

            // Read the paragraph.
            mParaReader.Read(reader);

            // Read paragraph have to be resolved later.
            resolver.EndPendingInline(sdt, (Paragraph)reader.CurContainer.LastNonAnnotationChild, index);
        }

        /// <summary>
        /// Reworked fix for 3760: Found table inside para: Finish current para, add a table and then create new para.
        /// </summary>
        private void ResilientTableIntoParaRead(NrxDocumentReaderBase reader)
        {
            Paragraph paragraph = (Paragraph)reader.CurContainer;
            reader.PopContainer(NodeType.Paragraph);

            ReadTable(reader);

            // MS Word places this paragraph after the table.
            // Runs before the table are moved to its first cell. And the first paragraph of the first cell gets
            // properties of the table parent paragraph, do the same with expanding.

            Table table = (Table)reader.CurContainer.LastChild;
            Row firstRow = table.FirstRow;
            if ((firstRow == null) || (firstRow.FirstCell == null))
            {
                // Looks like MS Word removes tables without rows and cannot open a document if a row has no cells.
                table.Remove();
                reader.CurContainer = paragraph;
                return;
            }

            Paragraph firstCellParagraph = (Paragraph)firstRow.FirstCell.GetChild(NodeType.Paragraph, 0, true);
            if (firstCellParagraph != null)
            {
                firstCellParagraph.InsertAfter(paragraph.FirstChild, null, null);
                ParaPr paraPr = paragraph.ParaPr.Clone();
                firstCellParagraph.ParaPr.ExpandTo(paraPr);
                firstCellParagraph.ParaPr = paraPr;
            }

            paragraph.Remove();
            reader.AddAndPushContainer(paragraph);
        }

        [JavaThrows(true)]
        protected abstract void ReadTable(NrxDocumentReaderBase reader);

        /// <summary>
        /// Fix for defect 20010: Found a para inside para: Finish current para, add a para then create a new para.
        /// </summary>
        /// <remarks>DD: I could have made resilient read without breaking para by creating temp container, reading into it
        /// and then inserting content from temp container into the current container.
        /// However I decided to do similar to <see cref="ResilientTableRead"/> for approach consistency.</remarks>
        private void ResilientParaRead(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);

            CompositeNode curParagraph = reader.CurContainer;
            reader.CurContainer = curParagraph.ParentNode;

            bool isSectionBreak = mParaReader.Read(reader);

            Paragraph readParagraph = reader.CurContainer.LastNonAnnotationChild as Paragraph;

            // WORDSNET-11628 Need to start a new section if it was read within current paragraph.
            if (isSectionBreak)
            {
                reader.StartNewSection();
                reader.CurContainer.AppendChild(curParagraph);
            }

            if (readParagraph != null)
            {
                // WORDSNET-19303 Move all content of 'curParagraph' as a block, i.e. without changing sequence.
                Paragraph targetParagraph = (Paragraph)curParagraph.NextSiblingOfType(NodeType.Paragraph) ?? readParagraph;
                while (curParagraph.HasChildNodes)
                    targetParagraph.PrependChild(curParagraph.LastChild);

                readParagraph.InsertNext(curParagraph);

                if (curParagraph.NodeType == NodeType.Paragraph)
                {
                    // WORDSNET-24226 Copy formatting from parent container.
                    ParaPr sourceParaPr = ((Paragraph)curParagraph).ParaPr;
                    foreach (int key in sourceParaPr.GetKeys())
                    {
                        // Ignore some keys according to the Word behavior. See related Test19303.
                        if ((key == ParaAttr.Istd) || (key == ParaAttr.RsidP))
                            continue;

                        readParagraph.ParaPr.SetAttrNoOverride(key, sourceParaPr[key]);
                    }
                }
            }

            reader.CurContainer = curParagraph;
        }

        private static void ReadSpecialChar(NrxDocumentReaderBase reader, RunPr runPr, char c)
        {
            FlushRun(reader, runPr);
            reader.AddChild(new SpecialChar(reader.Document, c, runPr));
        }

        private static void ReadSymbol(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);

            RunPr symbolRunPr = runPr.Clone();

            // WORDSNET-12033 Character category does not affect symbols so remove it.
            symbolRunPr.Remove(FontAttr.CharacterCategoryHint);
            char sym = reader.XmlReader.ReadSymbol(symbolRunPr);
            symbolRunPr.SetAttr(FontAttr.Sys_Symbol, true);

            // WORDSNET-17859 Exclude this character from common SYM processing. See Test17859() for details.
            if (sym != 0xA0)
            {
                // WORDSNET-17650 Detect symbolic font by charset.
                FontInfo fontInfo = reader.Document.FontInfos[symbolRunPr.NameAscii];
                if ((fontInfo != null) && (fontInfo.Charset == CodePage.SymbolCharSet))
                    sym = FontUtil.SymbolToUnicode(sym);
            }

            // WORDSNET-14953 Append symbol to preceding node when it is footnote and formatted as footnote reference mark.
            if (IsReferenceMarkChunk(reader, symbolRunPr))
                ((Footnote)reader.CurContainer.LastNonAnnotationChild).ReferenceMark += sym;
            else
                reader.AddChild(new Run(reader.Document, sym.ToString(), symbolRunPr));
        }

        /// <summary>
        /// Returns "True" when current "sym" content must be appended as the separate run node.
        /// </summary>
        private static bool IsReferenceMarkChunk(NrxDocumentReaderBase reader, RunPr runPr)
        {
            Debug.Assert((reader != null) && (runPr != null));

            // Last appended node must be "Footnote", otherwise interpret current node as separate run.
            Node prevNode = reader.CurContainer.LastNonAnnotationChild;
            Footnote footnote = (prevNode != null) && (prevNode.NodeType == NodeType.Footnote) ? (Footnote)prevNode : null;

            // Word uses not more than 10 characters.
            if ((footnote == null) || footnote.IsAuto || (footnote.ReferenceMark.Length >= 10))
                return false;

            // «FootnoteReference» style required to treat an "inline" node as part of the "Footnote".
            Style footnoteRefStyle = reader.Document.Styles.GetBySti(StyleIdentifier.FootnoteReference, false);
            if (!runPr.Contains(FontAttr.Istd) || (footnoteRefStyle == null) || (runPr.Istd != footnoteRefStyle.Istd))
                return false;

            // It is necessary to create separate run when properties of "inline" nodes are different.
            // Otherwise formatting will be lost.
            bool runPrEquals = footnote.RunPr.Equals(runPr, RunPr.NonCollapsibleFontKeys);
            return runPrEquals;
        }

        /// <summary>
        /// Reads a footnote reference and retrieves a corresponding footnote for it.
        /// </summary>
        /// <param name="reader">Where to read the footnote reference from.</param>
        /// <param name="runPr">The run properties to apply to the footnote node.</param>
        /// <param name="footnoteType">Whether it is a footnote or endnote.</param>
        /// <returns>
        /// Returns <c>true</c> if the <paramref name="reader"/> has read the `r` element.
        /// </returns>
        private bool ReadFootnote(NrxDocumentReaderBase reader, RunPr runPr, FootnoteType footnoteType)
        {
            FlushRun(reader, runPr);

            int id = -1;
            bool suppressRef = false;

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        id = xmlReader.ValueAsInt;
                        break;
                    case "type":    // WML.
                        // This is supposed to be empty string or "normal" indicating a normal footnote or endnote.
                        break;
                    case "customMarkFollows":
                        // WORDSNET-4993 setting suppressRef if footnote with custom mark used.
                        suppressRef = xmlReader.ValueAsBool;
                        break;
                    case "suppressRef": // WML
                        suppressRef = xmlReader.ValueAsBool;
                        break;
                    default:
                        // Ignored.
                        break;
                }
            }

            Footnote footnote = GetFootnote(reader, id, footnoteType, runPr);
            if (footnote == null)
                return false;

            // WORDSNET-4993 setting proper footnote type (changed IsAuto to be writable for this).
            footnote.IsAuto = !suppressRef;
            footnote.SetRunPrInternal(runPr);

            bool rReadingCompleted = suppressRef && ReadCustomFootnoteMark(reader, runPr, footnote, id);

            reader.AddChild(footnote);

            return rReadingCompleted;
        }

        [JavaThrows(true)]
        protected abstract Footnote GetFootnote(NrxDocumentReaderBase reader, int id, FootnoteType footnoteType, RunPr runPr);

        /// <summary>
        /// WORDSNET-4993, 5323, 5324, 12842: reading reference mark:
        /// can be a single 't', 'br', 'sym'  or 'tab' right after footnote.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the <paramref name="reader"/> has read the `r` element.
        /// </returns>
        private static bool ReadCustomFootnoteMark(NrxDocumentReaderBase reader, RunPr runPr, Footnote footnote, int id)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (xmlReader.ReadChild("r"))
            {
                switch (xmlReader.LocalName)
                {
                    case "t":
                    case "delText":
                    case "instrText":
                    {
                        if (xmlReader.ReadChildWithTextValues("t", AnyXmlTextHandlingConsts.TextAndSignificant) &&
                            (xmlReader.LocalName == ""))
                        {
                            footnote.ReferenceMark = xmlReader.Value;
                        }
                        else
                        {
                            // WORDSNET-19063 Word doesn't generate error while reading custom footnote mark with empty
                            // reference mark. It is not a full solution because Word can bind reference mark to some
                            // paragraph while saving to docx.
                            reader.Warn(
                                WarningType.DataLoss,
                                WarningSource.Docx,
                                string.Format(WarningStrings.FootnoteWithEmptyReferenceMark, id));
                        }

                        break;
                    }
                    case "br":
                        footnote.ReferenceMark = ReadBreak(xmlReader, runPr).ToString();
                        break;
                    case "sym":
                        footnote.ReferenceMark = reader.XmlReader.ReadSymbol(runPr).ToString();
                        break;
                    case "tab":
                        footnote.ReferenceMark = ControlChar.Tab;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported footnote reference mark.");
                }

                return false;
            }

            // WORDSNET-16473 Word doesn't generate error while reading custom footnote mark without reference mark.
            // It is not a full solution because Word can bind reference mark to some paragraph while saving to docx.
            reader.Warn(
                WarningType.DataLoss,
                WarningSource.Docx,
                string.Format(WarningStrings.FootnoteWithoutReferenceMark, id));

            return true;
        }

        private void ReadComment(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);

            IWarningCallback warningCallback = reader.LoadOptions.WarningCallback;

            Comment comment = GetComment(reader, runPr);

            //strubaev WORDSNET-13545 Do not process nodes of comment that is already a child of another node.
            if (comment != null)
            {
                if (comment.ParentNode == null)
                {
                    comment.SetRunPrInternal(runPr);
                    reader.AddChild(comment);
                }
                else
                {
                    if (warningCallback != null)
                    {
                        warningCallback.Warning(new WarningInfo(WarningType.DataLoss, WarningSource.Docx, string.Format("Custom comment with id={0} was already added", comment.Id.ToString())));
                    }
                }
            }
        }

        [JavaThrows(true)]
        protected abstract Comment GetComment(NrxDocumentReaderBase reader, RunPr runPr);

        private bool ReadVmlObject(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);
            if (mVmlReader == null)
                return false;

            IList<ShapeBase> shapes = mVmlReader.Read(reader);
            foreach (ShapeBase shapeBase in shapes)
            {
                if (shapeBase != null)
                {
                    shapeBase.RunPr = runPr;

                    // WORDSNET-7796 Try to get image size from image data. Made for WML only. See TestJira7796 for details.
                    // WORDSNET-10477 Replaces zero width and height with default values for DOCX.
                    // WORDSNET-26342 MS Word doesn't fix shape zero size if 'position: absolute' shape style is
                    // defined. We currently store position in the WrapType attribute, and this value can be
                    // overwritten by wrap type itself. But let's ignore this case; if customers will complain,
                    // we can implement temporary storage of position in IVmlShapeReaderContext.
                    if (shapeBase.IsImage && (shapeBase.WrapType != WrapType.None))
                        ((Shape)shapeBase).FixZeroSize(reader.IsDocx);

                    reader.AddChild(shapeBase);
                }
            }

            return true;
        }

        /// <summary>
        /// Reads 17.3.3.9 drawing (DrawingML Object)
        /// </summary>
        private static void ReadDrawingML(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);

            // I just cast here because DOCX reader implement this. WML reader does not, but DrawingML does not occur in WML.
            INrxDmlReader dmlReader = (INrxDmlReader)reader;
            ShapeBase drawingML = dmlReader.ReadDrawing(runPr);
            if (drawingML != null)
            {
                // Apply any current delete/insert/move revision.
                reader.StoryRevisionStack.Apply(runPr, drawingML);

                drawingML.RunPr = runPr;

                reader.AddToDmlTextboxesMap(drawingML);
            }
        }

        private void ReadFldChar(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);
            mFldCharReader.Read(reader, runPr);
        }

        /// <summary>
        /// As per a recommendation in OOXML we convert w:pgNum into a PAGE field.
        /// </summary>
        private static void ReadLegacyPageNumber(NrxDocumentReaderBase reader, RunPr runPr)
        {
            FlushRun(reader, runPr);

            Node[] fieldNodes = MiscRwUtil.CreatePageField(reader.Document, runPr);
            for (int curNode = 0; curNode < fieldNodes.Length; curNode++)
                reader.AddChild(fieldNodes[curNode]);
        }

        internal static char ReadBreak(NrxXmlReader xmlReader, RunPr runPr)
        {
            // Set the default character to line break. That corresponds to MS Word behavior.
            char br = ControlChar.LineBreakChar;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                        br = NrxRunEnum.XmlToBreak(xmlReader.Value);
                        break;
                    case "clear":
                        runPr.SetAttr(FontAttr.LineBreakClear, NrxRunEnum.XmlToLineBreakClear(xmlReader.Value));
                        break;
                    default:
                        // Ignored.
                        break;
                }
            }

            return br;
        }

        /// <summary>
        /// [MS-ODRAWXML] 2.2.7 WordprocessingML Drawing
        /// Reads the mc:AlternateContent written by Word 2010. It can only occur inside an "r" element.
        /// </summary>
        internal void ReadAlternateContent(NrxDocumentReaderBase reader, RunPr runPr)
        {
            AlternateContent alternateContent = null;
            NrxXmlReader xmlReader = reader.XmlReader;
            bool choiceWasRead = false;

            reader.IsInAlternateContent = true;

            while (xmlReader.ReadChild("AlternateContent"))
            {
                // Choice and Fallback, are used to provide alternates for specified content.
                switch (xmlReader.LocalName)
                {
                    case "Choice":
                        alternateContent = ReadChoice(reader, runPr);
                        choiceWasRead = true;
                        break;
                    case "Fallback":
                        ReadFallback(reader, runPr, alternateContent);

                        // MS Word crashes when Fallback is specified before Choice.
                        // In this case we just read FallBack and stop the loop.
                        if (!choiceWasRead)
                            return;
                        else
                            reader.ComplianceInfo.MarkAsIsoTransitional();
                        break;
                    default:
                        xmlReader.IgnoreElement(WarningType.UnexpectedContent, WarningSource.Nrx, xmlReader.LocalName);
                        break;
                }
            }

            reader.IsInAlternateContent = false;
        }

        /// <summary>
        /// Reads the Choice element. If there is a match between required namespaces (in this case Word 2010) the appropriate Choice is read.
        /// </summary>
        private AlternateContent ReadChoice(NrxDocumentReaderBase reader, RunPr runPr)
        {
            bool choiceMatched = false;
            AlternateContent alternateContent = new AlternateContent();
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    // The Requires attribute specifies a set of space-delimited namespaces that must be understood in order to select that choice.
                    case "Requires":
                    {
                        string prefix = xmlReader.Value;
                        if (StringUtil.HasChars(prefix) &&
                            IsAllowedChoiceRequirement(xmlReader.LookupNamespace(prefix)))
                        {
                            choiceMatched = true;
                            alternateContent.Requires = prefix;
                        }
                        break;
                    }
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Nrx, xmlReader.LocalName);
                        break;
                }
            }

            if (choiceMatched)
            {
                // RK There is work here with a "temporary container". It is needed currently to work around the "DrawingML Text Box" problem e.g. WORDSNET-6710
                // In earlier OOXML documents DrawingML could not contain  MS Word content. But starting with some MS Word update, they can now have
                // DrawingML shapes with MS Word textbox content, e.g. paragraphs, tables and so on. Our model does not yet support DrawingML with MS Word content.
                // So this workaround detects this situation (that we cannot represent in the model) and uses the Fallback instead.
                //
                // I tried simplifying this code by just checking for the "wps" namespace (instead of checking for the "txbx" element, but that causes some
                // other VML shapes to be caught into Fallback use. This in turns breaks some unit tests. Since this all is a temporary solution,
                // I agree to leave this code working with temporary container for now. But the team needs to rework how to store VML in DrawingML.

                // Remember the current container and remove it from the tree, we will restore it when finish reading Choice.
                CompositeNode curContainer = reader.CurContainer;
                reader.PopContainer(curContainer.NodeType);
                curContainer.Remove();

                // Instead of the current container push a temporary container, where we will read Choice.
                CompositeNode temporaryContainer = (CompositeNode)curContainer.Clone(false);
                reader.AddAndPushContainer(temporaryContainer);

                // Read Choice to the temporary container.
                ReadRChildren(reader, runPr, false);

                // Remove the temporary container from the tree, and restore the previously remembered container.
                reader.PopContainer(temporaryContainer.NodeType);
                temporaryContainer.Remove();
                reader.AddAndPushContainer(curContainer);

                // If Choice does not contains unsupported elements, insert its content into the tree.
                while (temporaryContainer.HasChildNodes)
                    reader.CurContainer.AppendChild(temporaryContainer.FirstChild); // AppendChild removes the child so using FirstChild every time is okay.

                return alternateContent;
            }

            return null;
        }

        /// <summary>
        /// Checks that a namespace referenced by value of the Requires attribute of the alternate content choice part
        /// is a known DOCX namespace.
        /// </summary>
        internal static bool IsAllowedChoiceRequirement(string nameSpace)
        {
            // Check whether this is MS Word 2010 or 2016 namespace or well known namespace.
            // WORDSNET-12000 Requires attribute of AlternateContent equals "v" it is well known namespace
            // so choice will be matched.
            return
                DocxNamespaces.IsWord2010Namespace(nameSpace) ||
                DocxNamespaces.IsWord2016Namespace(nameSpace) ||
                DocxNamespaces.IsWellKnownNamespace(nameSpace);
        }

        /// <summary>
        /// Reads "Fallback" child elements with error suppression.
        /// </summary>
        /// <param name="reader">Provides methods for document reading.</param>
        /// <param name="runPr">Keeps run attributes values.</param>
        /// <param name="alternateContent">Container to store "AlternateContent" data.</param>
        private void ReadFallbackSafe(NrxDocumentReaderBase reader, RunPr runPr, AlternateContent alternateContent)
        {
            Debug.Assert(alternateContent != null);

            string localeName = reader.XmlReader.LocalName;
            CompositeNode prevContainer = reader.CurContainer;
            CompositeNode tmpContainer = (CompositeNode)reader.CurContainer.Clone(false);
            reader.AddAndPushContainer(tmpContainer);

            // WORDSNET-13846 Preserve "Fallback" reading from any faults.
            // "FallBack" will be written by AW while processes validation.
            try
            {
                // Read Fallback to temporary container.
                ReadRChildren(reader, runPr.Clone(), false);
            }
            catch (Exception e)
            {
                // Read broken element to the end.
                while (reader.XmlReader.ReadChild(localeName))
                    reader.XmlReader.IgnoreElementNoWarn();

                ClearTmpContainer(reader, tmpContainer, prevContainer);
                // "Fallback" part set to null in case when some errors occur.
                tmpContainer = null;

                // Register error as warning.
                string msg = string.Format(ReadFallbackErrTmpl, e.Message);
                reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, msg);
            }

            alternateContent.FallBack = tmpContainer;
            Node fallbackShape = (tmpContainer != null) ? tmpContainer.FirstChild : null;

            if (fallbackShape is ShapeBase)
                ((ShapeBase)fallbackShape).IsFallback = true;

            // Save AlternateContent to RunPr bag.
            runPr.AlternateContent = alternateContent;
            ClearTmpContainer(reader, tmpContainer, prevContainer);
        }


        /// <summary>
        /// Restores previous container and removes current temporary container.
        /// </summary>
        /// <param name="reader">Provides methods for document reading.</param>
        /// <param name="tmpCont">Temporary container to remove.</param>
        /// <param name="prevCont">Original container to restore.</param>
        private static void ClearTmpContainer(NrxDocumentReaderBase reader, CompositeNode tmpCont, CompositeNode prevCont)
        {
            if (tmpCont == null)
                return;

            // Restore and remove unneeded container.
            reader.CurContainer = prevCont;
            tmpCont.Remove();
        }

        /// <summary>
        /// Read Fallback element which can be utilized as a default when no match with Choice occurs.
        /// </summary>
        private void ReadFallback(NrxDocumentReaderBase reader, RunPr runPr, AlternateContent alternateContent)
        {
            // If Choice was matched, we should read Fallback to temporary container
            // and then use it for writing correct Ecma376_2006 document and mc:AlternateContent for Iso29500_2008_Transitional document.
            if (alternateContent != null)
                ReadFallbackSafe(reader, runPr, alternateContent);
            else
                ReadRChildren(reader, runPr, false);
        }

        /// <summary>
        /// Tries to recover 'p' elements if it unexpectedly occurred inside inline.
        /// </summary>
        private bool ResilientParaReadInsideInline(NrxDocumentReaderBase reader, RunPr runPr)
        {
            switch (reader.CurContainer.NodeType)
            {
                case NodeType.Paragraph:
                    ResilientParaRead(reader, runPr);
                    return true;
                case NodeType.StructuredDocumentTag:
                    ResilientParaReadInsideInlineSdt(reader);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Reads 'ruby' element.
        /// </summary>
        private void ReadRuby(NrxDocumentReaderBase reader, RunPr runPr)
        {
            Ruby ruby = new Ruby();
            runPr.SetAttr(FontAttr.Ruby, ruby);

            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("ruby"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rubyPr":
                        ReadRubyPr(reader, ruby);
                        break;

                    case "rt":
                        ReadRubyRun(reader, ruby.Top);
                        break;

                    case "rubyBase":
                        ReadRubyRun(reader, ruby.Base);
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'rt' or 'rubyBase' elements which are <see cref="Ruby"/> top and base runs.
        /// </summary>
        private void ReadRubyRun(NrxDocumentReaderBase reader, RubyChunkCollection rubyChunks)
        {
            AnyXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;

            Paragraph tempContainer = new Paragraph(reader.Document);

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    // AM. Not ready to read edit revisions for Ruby.
                    // Read as ordinary content.
                    case "ins":
                    case "del":
                    case "r":
                        // Switch current container to Ruby.
                        CompositeNode savedContainer = reader.CurContainer;
                        reader.CurContainer = tempContainer;
                        tempContainer.GetChildNodes(NodeType.Any, false).Clear();

                        Read(reader);

                        Run run = tempContainer.FirstRun;
                        RubyChunk chunk = new RubyChunk();
                        chunk.Text = run.Text;
                        chunk.RunPr =  run.RunPr.Clone();
                        rubyChunks.Add(chunk);

                        // After read switch it back.
                        reader.CurContainer = savedContainer;
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'rubyPr' element which specifies <see cref="Ruby"/> properties.
        /// </summary>
        private static void ReadRubyPr(NrxDocumentReaderBase reader, Ruby ruby)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            while (xmlReader.ReadChild("rubyPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rubyAlign":
                        ruby.Alignment = NrxRunEnum.XmlToRubyAlignment(xmlReader.ReadVal());
                        break;

                    case "hps":
                        ruby.TopSize = xmlReader.ReadValAsHalfPoints(complianceInfo);
                        break;

                    case "hpsRaise":
                        ruby.Distance = xmlReader.ReadValAsHalfPoints(complianceInfo);
                        break;

                    case "hpsBaseText":
                        ruby.BaseSize = xmlReader.ReadValAsHalfPoints(complianceInfo);
                        break;

                    case "lid":
                        string lid = xmlReader.ReadVal();
                        int language = reader.IsDocx
                            ? LocaleConverter.DocxTagToLocale(lid)
                            : LocaleConverter.WmlTagToLocale(lid);

                        ruby.Language = (Language)language;
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static readonly char[] gWhitespaceChars = new char[] { ControlChar.SpaceChar, ControlChar.TabChar };

        /// <summary>
        /// Message template for case when "Fallback" element cannot be loaded.
        /// </summary>
        private const string ReadFallbackErrTmpl = "Read 'Fallback' element failed. Error details: {0}";

        private readonly NrxStoryReaderBase mStoryReader;
        private readonly NrxRunPrReaderBase mRunPrReader;
        private readonly NrxParaReaderBase mParaReader;
        private readonly NrxFldCharReaderBase mFldCharReader;
        private readonly NrxFldSimpleReader mFldSimpleReader;
        private readonly NrxHyperlinkReaderBase mHyperlinkReader;
        private readonly INrxVmlReader mVmlReader;
    }
}
