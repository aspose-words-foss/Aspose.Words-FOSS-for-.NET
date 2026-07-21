// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a SIGN function.
    /// </summary>
    internal class SignFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            DoubleConstant result = DoubleConstant.CreateFrom(parameters[0], System.Math.Sign(parameters[0].ValueDouble));
            result.IsUsesNegativeParentheses = false;
            return result;
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
