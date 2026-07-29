// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a DEFINED function.
    /// </summary>
    internal class DefinedFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            return new BooleanConstant(!parameters[0].IsError);
        }

        protected override int ParameterCountMin
        {
            get { return 1; }
        }

        protected override int ParameterCountMax
        {
            get { return 1; }
        }

        internal override bool IsDefinedFunction
        {
            get { return true; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            return true;
        }
    }
}
