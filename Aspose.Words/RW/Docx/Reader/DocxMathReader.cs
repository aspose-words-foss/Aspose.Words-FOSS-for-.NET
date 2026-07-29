// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2011 by Denis Darkin

using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads m:oMath and m:oMathPara objects and all possible children into a math tree.
    /// </summary>
    internal class DocxMathReader : NrxMathReaderBase
    {
        internal DocxMathReader(NrxDocumentReaderBase reader,
            DocxRunPrReader runPrReader,
            DocxInlineReader inlineReader,
            DocxAnnotationReader annotationReader)
            : base(reader, runPrReader, inlineReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        protected override bool ReadFormatSpecificAttribute(string attrName, IMathRunPr rPr)
        {
            switch (attrName)
            {
                case "del":
                    mAnnotationReader.ReadMathRunPrRevision(DocumentReader, (RunPr)rPr, true);
                    return true;
                case "ins":
                    mAnnotationReader.ReadMathRunPrRevision(DocumentReader, (RunPr)rPr, false);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Read root-level math object m:oMathPara
        /// </summary>
        internal static void ReadOMathPara(NrxDocumentReaderBase reader)
        {
            DocxMathReader mathReader = DocxReaderFactory.CreateMathReader(reader);
            NrxMathReaderUtil.ReadOMathPara(mathReader);
        }
        
        /// <summary>
        /// Read root-level math object m:oMath
        /// </summary>
        internal static void ReadOMath(NrxDocumentReaderBase reader)
        {
            DocxMathReader mathReader = DocxReaderFactory.CreateMathReader(reader);
            NrxMathReaderUtil.ReadOMath(mathReader);
        }

        private readonly DocxAnnotationReader mAnnotationReader;
    }
}