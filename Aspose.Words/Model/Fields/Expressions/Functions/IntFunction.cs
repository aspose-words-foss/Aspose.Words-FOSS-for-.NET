// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an INT function.
    /// </summary>
    internal class IntFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            return DoubleConstant.CreateFrom(parameters[0], (int)parameters[0].ValueDouble);
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
