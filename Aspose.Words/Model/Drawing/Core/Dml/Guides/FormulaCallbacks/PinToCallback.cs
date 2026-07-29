// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks
{
    internal class PinToCallback : IDmlThreeArgumentFormulaCallback
    {
        public double Calculate(double x, double y, double z)
        {
            if (y < x)
                return x;
            else if (y > z)
                return z;
            else
                return y;
        }
    }
}