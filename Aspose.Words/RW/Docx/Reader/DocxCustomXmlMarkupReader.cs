// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2008 by Roman Korchagin

using System;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class DocxCustomXmlMarkupReader
    {
        internal DocxCustomXmlMarkupReader(DocxInlineReader inlineReader, MarkupLevel level) : this(inlineReader, level, null)
        {
        }

        internal DocxCustomXmlMarkupReader(DocxInlineReader inlineReader, MarkupLevel level, TablePr objPr)
        {
            Debug.Assert(inlineReader != null);
            mInlineReader = inlineReader;
            mLevel = level;
            mObjPr = objPr;
        }

        /// <summary>
        /// Reads customXml object of any level.
        /// </summary>
        /// <returns>
        /// For all readers except <see cref="DocxRowReader"/> this parameter is not used.
        /// For <see cref="DocxRowReader"/> it returns gridBefore value.
        /// </returns>
        /// <param name="reader">DocxReader to read from. Should be positioned to element start.</param>
        internal void Read(NrxDocumentReaderBase reader)
        {
            // WORDSNET-10749 CustomXml is no longer supported.
            reader.Warn(WarningType.DataLoss, WarningSource.Docx, WarningStrings.ViolationI4I);
            
            NrxXmlReader xmlReader = reader.XmlReader;
            
            // Read elements.
            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("customXml"))
            {
                switch (xmlReader.LocalName)
                {
                    case "customXmlPr":
                        // WORDSNET-10749 We are not reading CustomXml now and should skip reading its properties too.
                        break;
                    default:
                        ReadCustomXmlContent(reader);
                        break;
                }
            }
        }

        /// <summary>
        /// Read contents of customXml.
        /// </summary>
        private void ReadCustomXmlContent(NrxDocumentReaderBase reader)
        {
            switch (mLevel)
            {
                case MarkupLevel.Inline:
                    mInlineReader.ReadChild(reader);
                    break;
                case MarkupLevel.Block:
                    DocxReaderFactory.StoryReader.ReadChild(reader);
                    break;
                case MarkupLevel.Row:
                    DocxTableReader.ReadChild(reader, mObjPr);
                    break;
                case MarkupLevel.Cell:
                    DocxRowReader.ReadChild(reader, mObjPr);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected custom XML markup level.");
            }
        }

        private readonly DocxInlineReader mInlineReader;
        private readonly MarkupLevel mLevel;

        /// <summary>
        /// The same Pr is used either for storing TablePr or rowPr.
        /// </summary>
        private readonly TablePr mObjPr;
    }
}

