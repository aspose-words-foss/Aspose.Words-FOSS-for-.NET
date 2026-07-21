// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2008 by Roman Korchagin

using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxAnnotationReader
    {
        internal DocxAnnotationReader(NrxAnnotationReader nrxAnnotationReader,
            DocxInlineReader inlineReader,
            DocxSectPrReader sectPrReader)
        {
            Debug.Assert(nrxAnnotationReader != null);
            Debug.Assert(inlineReader != null);
            Debug.Assert(sectPrReader != null);
            mNrxAnnotationReader = nrxAnnotationReader;
            mInlineReader = inlineReader;
            mSectPrReader = sectPrReader;
        }

        /// <summary>
        /// Reads an inline deletion revision.
        /// </summary>
        internal void ReadInlineDeletion(NrxDocumentReaderBase reader)
        {
            // This is just common code that reads the attributes.
            NrxAnnotation annotation = new NrxAnnotation(reader.XmlReader);

            // We create a deletion revision object and store it in the main reader.
            // Then we read the content of the revision and the main reader
            // applies the current deletion revision object to all created nodes.
            //
            // It is interesting to note that according to the DOCX spec, deletion and insertion
            // revisions can be recursive. I have not seen such and they will not work with this code.
            // I just presume they never occur.
            EditRevision deletion = new EditRevision(EditRevisionType.Deletion, annotation.Author, annotation.Date);
            reader.StoryRevisionStack.Push(deletion);

            // Content of the deletion revision seems to be mostly inline elements,
            // although according to the DOCX spec it can contain various Math elements.
            // For the time being, just recurse.
            mInlineReader.ReadChildren(reader);

            // Remove the revision object from the main reader so it is no longer applied to new nodes.
            reader.StoryRevisionStack.Pop(deletion);
        }

        /// <summary>
        /// Reds an inline insertion revision. See comments above.
        /// </summary>
        internal void ReadInlineInsertion(NrxDocumentReaderBase reader)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader.XmlReader);
            EditRevision insertion = new EditRevision(EditRevisionType.Insertion, annotation.Author, annotation.Date);
            reader.StoryRevisionStack.Push(insertion);
            mInlineReader.ReadChildren(reader);
            reader.StoryRevisionStack.Pop(insertion);
        }

        /// <summary>
        /// This is for "ins" inside "rPr", occurs in paragraph marks.
        /// </summary>
        internal static NrxAnnotation ReadRunPrInsertion(NrxXmlReader reader, RunPr runPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);
            runPr.InsertRevision = new EditRevision(EditRevisionType.Insertion, annotation.Author, annotation.Date);
            return annotation;
        }

        /// <summary>
        /// This is for "del" inside "rPr", occurs in paragraph marks.
        /// </summary>
        internal static NrxAnnotation ReadRunPrDeletion(NrxXmlReader reader, RunPr runPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);
            runPr.DeleteRevision = new EditRevision(EditRevisionType.Deletion, annotation.Author, annotation.Date);
            return annotation;
        }

        /// <summary>
        /// This is for "moveFrom" inside "rPr", occurs in paragraph marks.
        /// </summary>
        internal static NrxAnnotation ReadRunPrMoveFrom(NrxXmlReader reader, RunPr runPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);
            runPr.MoveFromRevision = new MoveRevision(MoveRevisionType.MoveFrom, annotation.Author, annotation.Date);
            return annotation;
        }

        /// <summary>
        /// This is for "moveTo" inside "rPr", occurs in paragraph marks.
        /// </summary>
        internal static NrxAnnotation ReadRunPrMoveTo(NrxXmlReader reader, RunPr runPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);
            runPr.MoveToRevision = new MoveRevision(MoveRevisionType.MoveTo, annotation.Author, annotation.Date);
            return annotation;
        }

        internal void ReadParaPrRevision(NrxDocumentReaderBase reader, ParaPr paraPr, RunPr paragraphBreakRunPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            while (xmlReader.ReadChild("pPrChange"))
            {
                switch (xmlReader.LocalName)
                {
                    case "pPr":
                        mNrxAnnotationReader.ReadParaPr(reader, paraPr, paragraphBreakRunPr, annotation);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        internal void ReadSectPrRevision(NrxDocumentReaderBase reader, SectPr sectPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            while (xmlReader.ReadChild("sectPrChange"))
            {
                switch (xmlReader.LocalName)
                {
                    case "sectPr":
                        // See comments inside ReadParaPrRevision method.
                        //
                        // But note, it looks we should not be excluding rsid attributes from collapse here.
                        // Probably because rsid section is specified directly on "sectPr", but
                        // rsid for paragraph is specified on "p", not on "pPr.
                        SectPr revPr = sectPr.Clone();
                        sectPr.Clear();

                        // WORDSNET-25040 Try to copy Word behavior and handle certain revised attributes.
                        revPr.CopyTo(sectPr, SectAttr.Orientation);
                        revPr.CopyTo(sectPr, SectAttr.PageWidth);
                        revPr.CopyTo(sectPr, SectAttr.PageHeight);
                        revPr.CopyTo(sectPr, SectAttr.SectionStart);

                        revPr.CopyTo(sectPr, SectAttr.ColumnsSpacing);

                        mSectPrReader.Read(reader, sectPr);
                        revPr.Collapse(sectPr);
                        sectPr.FormatRevision = new FormatRevision(revPr, annotation.Author, annotation.Date);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads math del(§17.13.5.13) and ins(§17.13.5.16) run revisions.
        /// </summary>
        internal void ReadMathRunPrRevision(NrxDocumentReaderBase reader, RunPr runPr, bool isDeleteMathPrRevision)
        {
            string tagName = (isDeleteMathPrRevision) ? "del" : "ins";
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = (isDeleteMathPrRevision) ? ReadRunPrDeletion(xmlReader, runPr) : ReadRunPrInsertion(xmlReader, runPr);

            // RK I don't fully understand this. Why formatting revision tag name is "del" or "ins" here?
            ReadRevisionCore(reader, xmlReader, tagName, runPr, annotation);

            // DD: That is what MS Word seems to do. See AW Office Math Implementation notes for details.
            runPr.RejectFormatRevision();
        }

        internal void ReadRunPrRevision(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            ReadRevisionCore(reader, xmlReader, "rPrChange", runPr, annotation);
        }

        /// <summary>
        /// Read previous RunPr properties (see ISO 29500 17.3.2.27 rPr (Previous Run Properties))
        /// </summary>
        private void ReadRevisionCore(
            NrxDocumentReaderBase reader,
            NrxXmlReader xmlReader,
            string tagName,
            RunPr runPr,
            NrxAnnotation annotation)
        {
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "rPr":
                        mNrxAnnotationReader.ReadRunPr(reader, runPr, annotation);
                        break;
                    // WORDSNET-10596 <m:t> element inside <w:del> occurred.
                    case "t":
                        DocxReaderFactory.RunReader.ReadText(reader, runPr);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// This is the case where number formatting was added to a paragraph.
        /// </summary>
        internal static void ReadNumberingInsertion(NrxXmlReader reader, ParaPr paraPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);

            ParagraphNumberRevision numberRevision = new ParagraphNumberRevision();
            numberRevision.Author = annotation.Author;
            numberRevision.DateTime = annotation.Date;
            numberRevision.IsInsertion = true;

            paraPr.NumberRevision = numberRevision;
        }

        /// <summary>
        /// This covers the case where number formatting was accepted or removed
        /// or where the number was simply changed.
        /// </summary>
        internal static void ReadNumberingChange(NrxXmlReader reader, ParaPr paraPr)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader);

            ParagraphNumberRevision numberRevision = new ParagraphNumberRevision();
            numberRevision.Author = annotation.Author;
            numberRevision.DateTime = annotation.Date;
            // RK Yes, this code is in WmlAnnnotationReader.
            NrxAnnotationReader.ReadOriginal(numberRevision, annotation.Original);
            numberRevision.WasNumbered = true;
            paraPr.NumberRevision = numberRevision;
        }

        internal static void ReadTablePrFormatRevision(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            ReadTablePrFormatRevision(reader, tablePr, reader.XmlReader.LocalName);
        }

        internal static void ReadRowPrFormatRevision(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            ReadTablePrFormatRevision(reader, tablePr, reader.XmlReader.LocalName);
        }

        internal static void ReadCellPrRevision(NrxDocumentReaderBase reader, CellPr cellPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            while (xmlReader.ReadChild("tcPrChange"))
            {
                switch (xmlReader.LocalName)
                {
                    case "tcPr":
                        {
                            // See comments inside ReadParaPrRevision method.
                            WordAttrCollection revPr = cellPr.Clone();
                            cellPr.Clear();
                            DocxReaderFactory.CellPrReader.Read(reader, cellPr);
                            revPr.Collapse(cellPr);
                            cellPr.FormatRevision = new FormatRevision(revPr, annotation.Author, annotation.Date);
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read deletion/insertion revisions to specified <paramref name="attrCollection"/>.
        /// </summary>
        /// <remarks>
        /// Used for reading table, row (17.13.5.12, 17.13.5.17 and cell (17.13.5.1, 17.13.5.2) revisions.
        /// </remarks>
        internal static void ReadEditRevision(NrxDocumentReaderBase reader, WordAttrCollection attrCollection, EditRevisionType revisionType)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation revision = new NrxAnnotation(xmlReader);

            switch (revisionType)
            {
                case EditRevisionType.Deletion:
                    attrCollection.DeleteRevision = new EditRevision(revisionType, revision.Author, revision.Date);
                    break;
                case EditRevisionType.Insertion:
                    attrCollection.InsertRevision = new EditRevision(revisionType, revision.Author, revision.Date);
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private static void ReadTablePrFormatRevision(NrxDocumentReaderBase reader, TablePr tablePr, string parentTag)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            NrxAnnotation annotation = new NrxAnnotation(xmlReader);

            while (xmlReader.ReadChild(parentTag))
            {
                switch (xmlReader.LocalName)
                {
                    case "tblPr":
                    case "tblPrEx":
                        ReadTablePrFormatRevisionCore(reader, tablePr, annotation, true);
                        break;
                    case "trPr":
                        ReadTablePrFormatRevisionCore(reader, tablePr, annotation, false);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Read table, row and tblPrEx (17.4.61) revisions to TablePr bag.
        /// </summary>
        private static void ReadTablePrFormatRevisionCore(NrxDocumentReaderBase reader, TablePr tablePr, NrxAnnotation annotation, bool isTblPr)
        {
            WordAttrCollection revPr =  tablePr.Clone();

            // WORDSNET-6086 andrnosk: RevPr can already have format revisions from Table, we need to preserve them.
            // That is why we need to accept current revisions and add new revisions to this collection using Collapse method (see below).
            if (revPr.HasFormatRevision)
                revPr.AcceptFormatRevision();

            // Keep all attributes except revision.
            tablePr.Remove(RevisionAttr.FormatRevision);

            // Read 'w:tblPr', 'w:tblPrEx' or 'w:trPr' element from the specified reader.
            if (isTblPr)
                // Dmatv: per WORDSNET-17857, I've changed the logic that decrements FrameLeft/FrameTop.
                // No excessive decrement will happen after reading twice into the same tablePr.
                DocxTablePrReader.ReadTblPr(reader, tablePr);
            else
                DocxRowPrReader.Read(reader, tablePr);

            revPr.Collapse(tablePr);
            tablePr.FormatRevision = new FormatRevision(revPr, annotation.Author, annotation.Date);
        }

        /// <summary>
        /// Reads 'w:moveFromRangeStart' and 'w:moveToRangeStart' node depending on isMoveFrom flag.
        /// </summary>
        internal static void ReadMoveRangeStart(NrxDocumentReaderBase reader, bool isMoveFrom)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            MoveRangeStart moveRangeStart;

            if (isMoveFrom)
                moveRangeStart = new MoveFromRangeStart(reader.Document, annotation.Id, annotation.Author, annotation.Name, annotation.Date, annotation.DisplacedBy);
            else
                moveRangeStart = new MoveToRangeStart(reader.Document, annotation.Id, annotation.Author, annotation.Name, annotation.Date, annotation.DisplacedBy);

            if (annotation.ColFirst >= 0 && annotation.ColLast >= 0)
            {
                moveRangeStart.FirstColumn = annotation.ColFirst;
                moveRangeStart.LastColumn = annotation.ColLast;
            }

            reader.AddCrossStructureAnnotation(moveRangeStart);

            MoveRevision revision = new MoveRevision(isMoveFrom ? MoveRevisionType.MoveFrom : MoveRevisionType.MoveTo,
                annotation.Author, annotation.Date);
            reader.StoryRevisionStack.Push(revision, true);
        }

        /// <summary>
        /// Reads 'w:moveFromRangeEnd' and 'w:moveToRangeEnd' node depending on isMoveFrom flag.
        /// </summary>
        internal static void ReadMoveRangeEnd(NrxDocumentReaderBase reader, bool isMoveFrom)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            NrxAnnotation annotation = new NrxAnnotation(xmlReader);
            if (!annotation.HasId)
                return;

            MoveRangeEnd moveRangeEnd;

            if (isMoveFrom)
                moveRangeEnd = new MoveFromRangeEnd(reader.Document, annotation.Id, annotation.DisplacedBy);
            else
                moveRangeEnd = new MoveToRangeEnd(reader.Document, annotation.Id, annotation.DisplacedBy);

            MoveRevisionType revisionType = isMoveFrom ? MoveRevisionType.MoveFrom : MoveRevisionType.MoveTo;
            MoveRevision moveRevision = new MoveRevision(revisionType, annotation.Author, annotation.Date);

            RevisionBase revision = reader.StoryRevisionStack.Pop(moveRevision);
            if (revision != null)
                reader.AddCrossStructureAnnotation(moveRangeEnd);
        }

        /// <summary>
        /// Reds an inline moveTo/moveFrom revisions.
        /// </summary>
        internal void ReadInlineMove(NrxDocumentReaderBase reader, bool isMoveTo)
        {
            NrxAnnotation annotation = new NrxAnnotation(reader.XmlReader);
            MoveRevisionType revType = (isMoveTo) ? MoveRevisionType.MoveTo : MoveRevisionType.MoveFrom;
            MoveRevision revision = new MoveRevision(revType, annotation.Author, annotation.Date);

            reader.StoryRevisionStack.Push(revision, false);
            mInlineReader.ReadChildren(reader);
            reader.StoryRevisionStack.Pop(revision);
        }

        /// <summary>
        /// Reads customXml edit revision from the reader into
        /// <see cref="NrxDocumentReaderBase.CurrentCustomXmlRevision"/>.
        /// </summary>
        /// <remarks>
        /// 'Start' of revision range is read here and placed to the temporary storage in
        /// <see cref="NrxDocumentReaderBase.CurrentCustomXmlRevision"/>.
        /// So, when we read SDT itself, we know if some revision was started and therefore can apply it appropriately.
        /// </remarks>
        internal static void ReadCustomXmlEditRevision(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string revisionName = xmlReader.LocalName;
            EditRevisionType revisionType = (revisionName == "customXmlDelRangeStart")
                ? EditRevisionType.Deletion
                : EditRevisionType.Insertion;

            NrxAnnotation revision = new NrxAnnotation(xmlReader);

            // Set current customXml revision in reader to indicate that subsequent SDTs will have revisions.
            reader.CurrentCustomXmlRevision = new EditRevision(revisionType, revision.Author, revision.Date);
        }

        /// <summary>
        /// Reads customXml move revision from the reader into
        /// <see cref="NrxDocumentReaderBase.CurrentCustomXmlRevision"/>.
        /// </summary>
        /// <remarks>
        /// 'Start' of revision range is read here and placed to the temporary storage in
        /// <see cref="NrxDocumentReaderBase.CurrentCustomXmlRevision"/>.
        /// So, when we read SDT itself, we know if some revision was started and therefore can apply it appropriately.
        /// </remarks>
        internal static void ReadCustomXmlMoveRevision(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string revisionName = xmlReader.LocalName;
            MoveRevisionType revisionType = (revisionName == "customXmlMoveFromRangeStart")
                ? MoveRevisionType.MoveFrom
                : MoveRevisionType.MoveTo;

            NrxAnnotation revision = new NrxAnnotation(xmlReader);

            // Set current customXml revision in reader to indicate that subsequent SDTs will have revisions.
            reader.CurrentCustomXmlRevision = new MoveRevision(revisionType, revision.Author, revision.Date);
        }

        /// <summary>
        /// Resets customXml revision in <see cref="NrxDocumentReaderBase.CurrentCustomXmlRevision"/>.
        /// </summary>
        /// <remarks>
        /// This method is called during reading of customXmlXXXRangeEnd elements.
        /// </remarks>
        internal static void ClearCustomXmlRevision(NrxDocumentReaderBase reader)
        {
            reader.CurrentCustomXmlRevision = null;
        }

        private readonly NrxAnnotationReader mNrxAnnotationReader;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxInlineReader mInlineReader;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocxSectPrReader mSectPrReader;
    }
}
