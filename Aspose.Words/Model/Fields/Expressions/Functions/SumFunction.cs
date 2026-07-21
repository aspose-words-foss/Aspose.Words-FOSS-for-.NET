// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a SUM function.
    /// </summary>
    internal class SumFunction : AggregateFunction
    {
        protected override NullableDouble Apply(NullableDouble currentValue, double parameterValue)
        {
            return new NullableDouble(currentValue.GetValueOrDefault() + parameterValue);
        }
    }
}
