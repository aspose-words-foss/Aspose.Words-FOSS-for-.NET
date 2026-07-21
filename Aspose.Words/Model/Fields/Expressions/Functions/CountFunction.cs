// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a COUNT function.
    /// </summary>
    internal class CountFunction : AggregateFunction
    {
        protected override NullableDouble FinalizeCalculation(NullableDouble currentValue, int parameterCount)
        {
            return new NullableDouble(parameterCount);
        }

        protected override bool CountNullValues
        {
            get { return false; }
        }
    }
}
