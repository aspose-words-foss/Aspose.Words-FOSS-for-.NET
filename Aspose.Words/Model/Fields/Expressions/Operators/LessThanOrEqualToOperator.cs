// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a less than or equal to operator.
    /// </summary>
    internal class LessThanOrEqualToOperator : BinaryOperator
    {
        protected override Constant EvaluateCore(DoubleConstant operand1, DoubleConstant operand2)
        {
            return new BooleanConstant(operand1.ValueDouble <= operand2.ValueDouble);
        }

        internal override int Order
        {
            get { return 6; }
        }
    }
}
