// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using Aspose.Common;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal class DmlFormulaHelper
    {
        /// <summary>
        /// If valueString is numeric then return its value otherwise query value in value provider.
        /// </summary>
        public static double GetValue(string valueString, IDmlGuideValueProvider valueProvider)
        {
            double result = FormatterPal.TryParseDoubleInvariant(valueString);
            if (!double.IsNaN(result))
                return result;

            return valueProvider.GetValue(valueString);
        }
    }
}