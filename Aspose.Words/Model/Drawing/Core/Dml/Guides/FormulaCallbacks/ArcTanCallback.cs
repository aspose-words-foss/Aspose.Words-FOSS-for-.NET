// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2010 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Guides.FormulaCallbacks
{
    internal class ArcTanCallback : IDmlTwoArgumentFormulaCallback
    {
        public double Calculate(double x, double y)
        {
            double result = System.Math.Atan2(y, x);
            return ConvertUtilCore.RadiansToDmlAngles(result);
        }
    }
}