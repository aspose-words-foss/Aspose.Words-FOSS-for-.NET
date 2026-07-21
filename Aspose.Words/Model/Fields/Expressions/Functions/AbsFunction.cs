// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an ABS function.
    /// </summary>
    internal class AbsFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            // If field format does not specified, MS Word uses number of decimal places from argument of ABS function for result.
            DoubleConstant doubleParameter = parameters[0] as DoubleConstant;
            int numberOfDecimalPlaces = doubleParameter != null ? doubleParameter.NumberOfDigitsAfterDecimalPoint : 0;
            return DoubleConstant.CreateFrom(parameters[0], System.Math.Abs(parameters[0].ValueDouble), numberOfDecimalPlaces);
        }

        protected override int ParameterCountMin
        {
            get { return 1; }
        }

        protected override int ParameterCountMax
        {
            get { return 1; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            return constantType == ConstantType.Double;
        }
    }
}
