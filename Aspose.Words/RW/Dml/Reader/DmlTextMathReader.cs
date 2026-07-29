// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/16/2015 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlTextMathReader : DmlReaderBase, INrxMathReader
    {
        private DmlTextMathReader(NrxDocumentReaderBase reader)
        {
            mReader = reader;
        }

        internal static DmlTextMath Read(NrxDocumentReaderBase reader)
        {
            DmlTextMathReader mathReader = new DmlTextMathReader(reader);
            mathReader.ReadCore();
            // Return empty MathObjectOMathPara if 'm' tag is empty.
            DmlTextMath rootElement = (mathReader.mRootMath == null)
                ? new DmlTextMath(new MathObjectOMathPara())
                : mathReader.mRootMath;

            rootElement.IsRootElement = true;

            return rootElement;
        }

        private void ReadCore()
        {
            while (XmlReader.ReadChild("m"))
            {
                switch (XmlReader.LocalName)
                {
                    case "oMath":
                        NrxMathReaderUtil.ReadOMath(this);
                        break;
                    case "oMathPara":
                        NrxMathReaderUtil.ReadOMathPara(this);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }


        public void AddAndPushMathContainer(MathObject mathObject)
        {
            AddAndPushMathContainer(mathObject, new DmlRunProperties());
        }

        public void AddAndPushMathContainer(MathObject mathObject, IMathRunPr rPr)
        {
            DmlTextMath math = new DmlTextMath(mathObject, (DmlRunProperties)rPr);

            if (CurrentMath == null)
                mRootMath = math;
            else
                CurrentMath.AddElement(math);

            mMathStack.Push(math);
        }

        public void PopMathContainer()
        {
            mMathStack.Pop();
        }

        public IMathRunPr CreateRunPr()
        {
            return new DmlRunProperties();
        }

        public void ReadInlineChildren()
        {
            DmlParagraphTextElementBase inline = DmlTextShapeReader.ReadInline((DocxDocumentReaderBase)DocumentReader);
            if (inline != null)
                CurrentMath.AddElement(inline);
        }

        public void ReadCtrlPr(IMathRunPr rPr)
        {
            while (XmlReader.ReadChild("ctrlPr"))
            {
                switch (XmlReader.LocalName)
                {
                    case "rPr":
                        DmlTextShapeReader.ReadRunProperties((DmlRunProperties)rPr, (DocxDocumentReaderBase)DocumentReader);
                        break;
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
        }

        public NrxDocumentReaderBase DocumentReader
        {
            get { return mReader; }
        }

        public NrxXmlReader XmlReader
        {
            get { return mReader.XmlReader; }
        }

        private DmlTextMath CurrentMath
        {
            get { return mMathStack.Top(); }
        }

        private readonly NrxDocumentReaderBase mReader;
        private readonly Stack<DmlTextMath> mMathStack = new Stack<DmlTextMath>();
        private DmlTextMath mRootMath;
    }
}
