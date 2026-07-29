// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks
{
    internal class AbsoluteValueCallback : IDmlOneArgumentFormulaCallback
    {
        public double Calculate(double x)
        {
            if (x < 0)
                return -1.0*x;
            else
                return x;
        }
    }
}