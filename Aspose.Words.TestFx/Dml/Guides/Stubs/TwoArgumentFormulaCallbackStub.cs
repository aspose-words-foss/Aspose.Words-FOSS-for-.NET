// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Guides;

namespace Aspose.Words.Tests.Dml
{
    public class TwoArgumentFormulaCallbackStub : IDmlTwoArgumentFormulaCallback
    {
        public double Calculate(double x, double y)
        {
            return x + y;
        }
    }
}
