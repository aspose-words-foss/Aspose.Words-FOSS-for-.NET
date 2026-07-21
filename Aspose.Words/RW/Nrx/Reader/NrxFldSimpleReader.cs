// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/02/2011 by Alexey Noskov

using Aspose.Words.Fields;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Class reads fldSimple from WML or DOCX document.
    /// It is created as a result of refactoring WmlFldSimpleReader and DocxFldSimpleReader.
    /// </summary>
    internal class NrxFldSimpleReader
    {
        internal NrxFldSimpleReader(NrxInlineReaderBase inlineReader)
        {
            Debug.Assert(inlineReader != null);
            mInlineReader = inlineReader;
        }

        /// <summary>
        /// Reads an fldSimple element.
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DocumentBase doc = reader.Document;

            string fieldCode = xmlReader.ReadAttribute("instr", "");
            string[] fieldCodeWords = fieldCode.TrimStart(' ').Split(' ');
            FieldType fieldType = FieldUtil.GetFieldType(fieldCodeWords[0]);
            FieldStart fieldStart = new FieldStart(doc, new RunPr(), fieldType);
            reader.AddChild(fieldStart);
            reader.AddChild(new Run(doc, fieldCode));
            reader.AddChild(new FieldSeparator(doc, new RunPr(), fieldType));

            fieldStart.IsLocked = NrxXmlReader.XmlToBool(xmlReader.ReadAttribute("fldLock", "off"));

            mInlineReader.ReadChildren(reader, runPr);

            // We need to set field end formatting to be the same as field value formatting
            // so that the merged data could be formatted correctly after merging.
            // (Currently, merged text takes formatting from merge field end).
            IInline lastInline = reader.CurContainer.LastChild as IInline;
            RunPr fieldEndRunPr;

            // WORDSNET-5397 andrnosk: Do not take formatting from lastInline if there is delete revision.
            if (lastInline != null && !lastInline.RunPr_IInline.HasDeleteRevision)
                fieldEndRunPr = lastInline.RunPr_IInline.Clone();
            else
                fieldEndRunPr = new RunPr();

            reader.AddChild(new FieldEnd(doc, fieldEndRunPr, fieldType, true));
        }

        private readonly NrxInlineReaderBase mInlineReader;
    }
}
