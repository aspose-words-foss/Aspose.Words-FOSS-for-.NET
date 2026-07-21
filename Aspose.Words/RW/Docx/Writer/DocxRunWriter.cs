// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2007 by Vladimir Averkin

using System.Collections.Generic;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes character formatting properties to WordML.
    /// </summary>
    internal class DocxRunWriter : NrxRunWriter
    {
        internal DocxRunWriter(DocxDocumentWriterBase writer)
        {
            mWriter = writer;
        }

        internal override void WriteInline(Inline inline)
        {
            string text = inline.GetText();

            if (inline.RunPr.Contains(FontAttr.Ruby))
            {
                WriteRuby(mWriter, inline);
                return;
            }

            // We are not writing runs which do not have text, even if they exist in the model.
            // That is to be consistent with the our reader, which does not read runs with no text to the model.
            if (!StringUtil.HasChars(text))
                return;

            // TestFormFieldOldFormat2.doc contains runs with \x0001 character as text.
            // MS Word writes this runs as the runs with no text.
            // I don't want to do the same because it does not make sense and will be inconsistent 
            // with our reader, which ignores runs with no text elements inside.
            // So I will just skip writing such runs.
            if (text[0] == ControlChar.PictureChar)
                return;

            // We need to check if the run text should be output as symbol character(inside 'w:sym' element).
            // Example:
            //   <w:sym w:font="Wingdings" w:char="F0A8" />
            //
            // MS Word uses a slightly different approach, it splits the run into individual characters but outputs
            // them inside 'w:t' element. And the symbol nature of the element is expressed by 'wx:sym' element
            // inside run properties 'w:rPr' element. I think it is ok to just output this chars as 'w:sym' runs.
            //
            // Generally MS Word handles symbols correctly even if they are inside 'w:t' element.
            // It is wordml2html.xslt that we are caring about here.
            //
            // RK Sometimes MS Word writes symbols as normal text, not as symbols like in ManualAdmin ms.docx.
            // It looks like it does so when the font attribute is not specified directly. I do the same here.
            if (inline.IsWriteAsSymbol)
            {
                // We should also check for cases when there are multiple symbol characters in the run.
                // As symbol characters use specific fonts they are probably never mixed with conventional characters.
                // So we don't need to use any complex algorithms to separate them from conventional characters,
                // we should just output each character in a separate run with 'w:sym' element representing run's text.
                for (int i = 0; i < text.Length; i++)
                {
                    WriteRunStart(inline);
                    DocxBuilder builder = mWriter.CurrentBuilder;

                    // andrnosk: WORDSNET-8777 Font of a symbol can be specified through the theme, 
                    // we have to check this and get correct font name.

                    // alexnosk: WORDSNET-5876 Use NameOther for symbols.
                    // Since symbolic characters are supposed to be outside ASCII range. MS Word seems to do the same.
                    string fontName = inline.RunPr.ComplexNameOther.Resolve(mWriter.Document.GetThemeInternal());

                    builder.WriteElementWithAttributes(
                        "w:sym",
                        "w:font", fontName,
                        "w:char", FormatterPal.IntToStrX4(text[i]));

                    WriteRunEnd(inline.RunPr);
                }
            }
            else
            {
                // BiDi control char should be written outside of 'w:r' elements
                // and it has its own XML element ('w:dir' or 'w:bdo').
                // So, do not write 'w:r' element for it.
                int initialRunStartedCount = mRunStartedCount;
                if (!BidiControlCharUtil.IsBidiControlCharStart(text[0]))
                    WriteRunStart(inline);

                WriteRunContent(inline);

                // Write the end only when a run was actually started. 
                if (mRunStartedCount > initialRunStartedCount)
                    WriteRunEnd(inline.RunPr);
            }
        }

        /// <summary>
        /// Writes sub nodes of the run element. If break chars are present we split up run text to simple text cuts
        /// and break chars and output them differently.
        /// </summary>
        private void WriteRunContent(Inline inline)
        {
            string text = inline.GetText();
            bool isDeleteRevision = inline.IsDeleteRevision;
            int textStart = 0;
            DocxBuilder builder = mWriter.CurrentBuilder;

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case ControlChar.FootnoteRefChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            WriteFootnoteRef();
                            break;
                        }
                    case ControlChar.FootnoteSeparatorChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            builder.WriteEmptyElement("w:separator");
                            break;
                        }
                    case ControlChar.FootnoteContinuationChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            builder.WriteEmptyElement("w:continuationSeparator");
                            break;
                        }
                    case ControlChar.AnnotationRefChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            builder.WriteEmptyElement("w:annotationRef");
                            break;
                        }
                    case ControlChar.TabChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            WriteTab();
                            break;
                        }
                    case ControlChar.ParagraphBreakChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            builder.WriteEmptyElement("w:cr");
                            break;
                        }
                    case ControlChar.PageBreakChar:
                    case ControlChar.LineBreakChar:
                    case ControlChar.ColumnBreakChar:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);
                            WriteBr(text[i], inline.RunPr.LineBreakClear);
                            break;
                        }
                    case ControlChar.NonBreakingHyphenChar:
                        {

                            // If the hyphen is not the first char in the run, then MS Word seems to break
                            // the run's text and writes 'w:softHyphen' in the new 'w:r' element.
                            // I think it is a good idea to do the same.
                            WriteTextAndStartNewRun(text, textStart, i, inline);
                            builder.WriteEmptyElement("w:noBreakHyphen");
                            break;
                        }
                    case ControlChar.OptionalHyphenChar:
                        {
                            // If soft hyphen is not the first char in the run, then MS Word seems to break
                            // the run's text and writes 'w:softHyphen' in the new 'w:r' element.
                            // I think it is a good idea to do the same.
                            WriteTextAndStartNewRun(text, textStart, i, inline);
                            builder.WriteEmptyElement("w:softHyphen");
                            break;
                        }
                    case ControlChar.LeftToRightEmbedding:
                    case ControlChar.RightToLeftEmbedding:
                    case ControlChar.LeftToRightOverride:
                    case ControlChar.RightToLeftOverride:
                        {
                            WriteText(text, textStart, i, isDeleteRevision);

                            // BiDi control chars should be written as special
                            // tags ('w:dir' or 'w:bdo') outside of any 'w:r' element.
                            // WORDSNET-18525 BiDi control char occurs at the beginning of "inline" node, so there is not an
                            // appropriate start of run.
                            if (i > 0)
                                WriteRunEnd(inline.RunPr);

                            WriteBidiControlChar(text[i], inline.ParentParagraph);

                            // Start new 'w:r' for the rest of the text,
                            // if next char is not BiDi again.
                            int nextIndex = i + 1;
                            if ((nextIndex < text.Length) && !BidiControlCharUtil.IsBidiControlCharStart(text[nextIndex]))
                                WriteRunStart(inline);

                            break;
                        }
                    case ControlChar.PopDirectionalFormatting:
                        {
                            WriteTextAndStartNewRun(text, textStart, i, inline);
                            // Write BiDi 'PDF' control char into a separate run as Unicode character.
                            WriteTextAndStartNewRun(text, i, i + 1, inline);
                            break;
                        }
                    default:
                        {
                            // Usual text.
                            // Continue to skip setting textStart.
                            continue;
                        }
                }

                textStart = i + 1;
            }

            // Write rest of the text.
            WriteText(text, textStart, text.Length, isDeleteRevision);
        }

        internal override void WriteRunStart(Inline inline)
        {
            WriteRunStart(inline.RunPr, inline, true);
        }

        /// <summary>
        /// Writes w:ins and/or w:del if needed, then writes start of w:r and writes w:rPr.
        /// </summary>
        internal void WriteRunStart(RunPr runPr, IRunAttrSource attrSource, bool isWriteRsid)
        {
            mRunStartedCount++;

            DocxBuilder builder = mWriter.CurrentBuilder;

            if (runPr.HasInsertRevision)
                builder.WriteRevisionStart(runPr.InsertRevision, mWriter.GetNextAnnotationId());
                
            if (runPr.HasDeleteRevision)
                builder.WriteRevisionStart(runPr.DeleteRevision, mWriter.GetNextAnnotationId());

            if (runPr.HasMoveFromRevision)
                builder.WriteRevisionStart(runPr.MoveFromRevision, mWriter.GetNextAnnotationId());

            if (runPr.HasMoveToRevision)
                builder.WriteRevisionStart(runPr.MoveToRevision, mWriter.GetNextAnnotationId());

            builder.StartElement(builder.IsInMath ? "m:r" : "w:r");

            if (isWriteRsid)
            {
                builder.WriteAttribute("w:rsidRPr", NrxXmlUtil.IntToHex(runPr.GetDirectAttr(FontAttr.RsidRPr)));

                if (runPr.ContainsKey(FontAttr.RsidR))
                {
                    string rsidR = NrxXmlUtil.IntToHex(runPr.GetDirectAttr(FontAttr.RsidR));

                    // Actually, we must compare with parent paragraph's rsidRDefault here.
                    // But I have not found rsidRDefault among paragraph attributes.
                    if (rsidR != builder.RsidR)
                        builder.WriteAttribute("w:rsidR", rsidR);
                }
            }

            DocxRunPrWriter.Instance.WriteForNodes(runPr, attrSource, false, mWriter);
        }

        /// <summary>
        /// Ends a w:r element (that could be wrapped into w:ins and/or w:del).
        /// </summary>
        internal override void WriteRunEnd(RunPr runPr)
        {
            if (!IsRunStarted)
                return;

            DocxBuilder builder = mWriter.CurrentBuilder;

            builder.EndElement();    //w:r

            if (runPr.HasDeleteRevision)
                builder.WriteRevisionEnd();
            
            if (runPr.HasInsertRevision)
                builder.WriteRevisionEnd();

            if (runPr.HasMoveFromRevision)
                builder.WriteRevisionEnd();

            if (runPr.HasMoveToRevision)
                builder.WriteRevisionEnd();

            mRunStartedCount--;
        }

        /// <summary>
        /// Writes a footnoteRef or endnoteRef element.
        /// </summary>
        private void WriteFootnoteRef()
        {
            string elemName = (mCurrentFootnoteType == FootnoteType.Footnote) ? "w:footnoteRef" : "w:endnoteRef";
            mWriter.CurrentBuilder.WriteEmptyElement(elemName);
        }

        /// <summary>
        /// Writes a literal text element.
        /// </summary>
        /// <param name="text">A string that contains text to write.</param>
        /// <param name="fromIndex">An index, from which text substring will be written.</param>
        /// <param name="toIndex">An index, until which text substring will be written.
        /// The character at this index is not included.</param>
        /// <param name="isDeletion">A flag indicating that destination element is a deleted text element.</param>
        private void WriteText(string text, int fromIndex, int toIndex, bool isDeletion)
        {
            if (fromIndex >= toIndex)
                return;
            
            DocxBuilder builder = mWriter.CurrentBuilder;

            string elementName;

            // Check if this text is actually a field code text.
            if (mWriter.IsInsideFieldCode)
            {
                elementName = isDeletion ? "w:delInstrText" : "w:instrText";
            }
            else
            {
                if (isDeletion && !builder.IsInMath) // MS Word does not write w:delText for deleted math runs, it writes m:t
                {
                    elementName = "w:delText";
                }
                else
                {
                    elementName = builder.IsInMath ? "m:t" : "w:t";
                }
            }

            builder.StartElement(elementName);

            if ((text[fromIndex] == ControlChar.SpaceChar) || (text[toIndex - 1] == ControlChar.SpaceChar))
                builder.WriteAttributeString("xml:space", "preserve");

            builder.WriteString(text.Substring(fromIndex, toIndex - fromIndex));

            builder.EndElement();
        }

        private void WriteTab()
        {
            // Example: 
            //   <w:tab wx:wTab="1080" wx:tlc="none" wx:cTlc="17" />

            // My guess is that wx:wTab, wx:tlc and wx:cTlc are calculated based on default tab settings
            // or on tab settings provided in ParentParagraph.ParaPr.TabStops.
            // I don't think we are able to calculate it without rendering.

            mWriter.CurrentBuilder.WriteEmptyElement("w:tab");
        }

        //<w:br w:type="text-wrapping" w:clear="all" />
        private void WriteBr(char br, LineBreakClear lineBreakClear)
        {
            string clear = null;

            // WORDSNET-28089 MS Word writes the 'clear' attribute for each break type: do the same in Aspose.Words.
            if (lineBreakClear != LineBreakClear.None)
                clear = NrxRunEnum.LineBreakClearToXml(lineBreakClear);

            if (br == ControlChar.LineBreakChar && lineBreakClear == LineBreakClear.None)
                mWriter.CurrentBuilder.WriteEmptyElement("w:br");
            else
                mWriter.CurrentBuilder.WriteElementWithAttributes(
                    "w:br",
                    "w:type", NrxRunEnum.BreakToXml(br, true),
                    "w:clear", clear);
        }

        /// <summary>
        /// Flushes text from the <paramref name="fromIndex"/> to the <paramref name="toIndex"/>,
        /// then closes current run and starts new one.
        /// </summary>
        private void WriteTextAndStartNewRun(string text, int fromIndex, int toIndex, Inline inline)
        {
            // Flush text.
            WriteText(text, fromIndex, toIndex, inline.IsDeleteRevision);

            // If we at the start of run already or at the end of text,
            // then it is not necessary to start new run.
            if ((toIndex == 0) || (toIndex >= text.Length))
                return;

            WriteRunEnd(inline.RunPr);
            WriteRunStart(inline);
        }

        /// <summary>
        /// Writes 'dir' or 'bdo' elements for RLE, LRE, RLO, LRO.
        /// Note, these elements should be written outside of 'w:r' element.
        /// </summary>
        private void WriteBidiControlChar(char bidiControlChar, Paragraph curParagraph)
        {
            string bidiControlCharType = string.Format("w:{0}", BidiControlCharUtil.ToTypeString(bidiControlChar));
            string bidiControlCharDirection = BidiControlCharUtil.ToDirectionString(bidiControlChar);

            mWriter.CurrentBuilder.WriteElementWithAttributesStart(bidiControlCharType, "w:val", bidiControlCharDirection);

            // Word closes 'w:dir' and 'w:bdo' elements at the end of paragraph,
            // so we collect it here to use at the end of the current paragraph.
            Stack<string> bidiControlChars = mWriter.GetOrCreateBiDiControlCharStack(curParagraph);
            bidiControlChars.Push(bidiControlCharType);
        }
        
        internal FootnoteType CurrentFootnoteType
        {
            get { return mCurrentFootnoteType; }
            set { mCurrentFootnoteType = value; }
        }

        private bool IsRunStarted
        {
            get { return mRunStartedCount > 0; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxDocumentWriterBase mWriter;
        private FootnoteType mCurrentFootnoteType = FootnoteType.Footnote;

        private int mRunStartedCount;
    }
}
