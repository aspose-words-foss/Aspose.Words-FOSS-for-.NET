// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2017 by Alexey Butalov

using Aspose.Words.Math;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxRunPrWriter : NrxRunPrWriterBase
    {
        private DocxRunPrWriter()
        {
        }

        protected override bool IsWriteMathPr(INrxWriterContext writer)
        {
            return writer.IsDocx;
        }

        protected override void WriteMathLineBreak(MathLineBreak mathLineBreak, NrxXmlBuilder builder)
        {
            DocxMathWriter.WriteLineBreak(mathLineBreak, builder);
        }

        internal static DocxRunPrWriter Instance
        {
            get
            {
                if (gInstance == null)
                    gInstance = new DocxRunPrWriter();
                return gInstance;
            }
        }

        private static DocxRunPrWriter gInstance;
    }
}
