// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks
{
    internal class SquareRootCallback : IDmlOneArgumentFormulaCallback
    {
        public double Calculate(double x)
        {
            // According to MS-OI29500: Microsoft Office Implementation Information for ISO/IEC-29500 Standard Compliance
            return System.Math.Sqrt(System.Math.Abs(x));
        }
    }
}