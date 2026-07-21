// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/16/2015 by Alexey Noskov

using Aspose.JavaAttributes;
using Aspose.Words.Math;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    internal interface INrxMathReader
    {
        void AddAndPushMathContainer(MathObject mathObject);
        void AddAndPushMathContainer(MathObject mathObject, IMathRunPr rPr);
        [JavaThrows(true)]
        void PopMathContainer();
        IMathRunPr CreateRunPr();
        [JavaThrows(true)]
        void ReadInlineChildren();
        [JavaThrows(true)]
        void ReadCtrlPr(IMathRunPr rPr);

        NrxDocumentReaderBase DocumentReader { get; }
        NrxXmlReader XmlReader { get; }
    }
}
