// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks
{
    internal class SineCallback : IDmlTwoArgumentFormulaCallback
    {
        public double Calculate(double x, double y)
        {
            y = ConvertUtilCore.DmlAnglesToRadians(y);
            return x*System.Math.Sin(y);
        }
    }
}