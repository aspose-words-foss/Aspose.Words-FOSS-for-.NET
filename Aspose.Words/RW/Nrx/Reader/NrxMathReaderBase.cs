// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2017 by Alexey Butalov

using Aspose.JavaAttributes;
using Aspose.Words.Math;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class, helps to read m:oMath and m:oMathPara objects and all possible children into a math tree.
    /// </summary>
    internal abstract class NrxMathReaderBase : INrxMathReader
    {
        protected NrxMathReaderBase(NrxDocumentReaderBase reader,
            NrxRunPrReaderBase runPrReader,
            NrxInlineReaderBase inlineReader)
        {
            Debug.Assert(reader != null);
            Debug.Assert(runPrReader != null);
            Debug.Assert(inlineReader != null);
            mReader = reader;
            mRunPrReader = runPrReader;
            mInlineReader = inlineReader;
        }

        public void AddAndPushMathContainer(MathObject mathObject)
        {
            DocumentReader.AddAndPushContainer(new OfficeMath(DocumentReader.Document, mathObject));
        }

        public void AddAndPushMathContainer(MathObject mathObject, IMathRunPr rPr)
        {
            RunPr runPr = rPr as RunPr;
            if (runPr != null)
            {
                OfficeMath node = new OfficeMath(DocumentReader.Document, mathObject, runPr);
                DocumentReader.AddAndPushContainer(node);
                DocumentReader.StoryRevisionStack.Apply(runPr, node);
            }
        }

        public void PopMathContainer()
        {
            DocumentReader.PopContainer(NodeType.OfficeMath);
        }

        public IMathRunPr CreateRunPr()
        {
            return new RunPr();
        }

        public void ReadInlineChildren()
        {
            mInlineReader.ReadChild(DocumentReader);
        }

        public void ReadCtrlPr(IMathRunPr rPr)
        {
            while (XmlReader.ReadChild("ctrlPr"))
            {
                switch (XmlReader.LocalName)
                {
                    case "rPr":
                        mRunPrReader.Read(DocumentReader, (RunPr)rPr);
                        break;
                    case "annotation":
                    case "content":
                       // WORDSNET-12224 Ignore 'annotation' elements while read EquationXML.
                        break;
                    default:
                        if (!ReadFormatSpecificAttribute(XmlReader.LocalName, rPr))
                            XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        [JavaThrows(true)]
        protected abstract bool ReadFormatSpecificAttribute(string attrName, IMathRunPr rPr);

        public NrxDocumentReaderBase DocumentReader
        {
            get { return mReader; }
        }

        public NrxXmlReader XmlReader
        {
            get { return mReader.XmlReader; }
        }

        private readonly NrxDocumentReaderBase mReader;
        private readonly NrxRunPrReaderBase mRunPrReader;
        private readonly NrxInlineReaderBase mInlineReader;
    }
}
