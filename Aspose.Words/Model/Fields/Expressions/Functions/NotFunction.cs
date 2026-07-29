// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a NOT function.
    /// </summary>
    internal class NotFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            return new BooleanConstant(!parameters[0].ValueBoolean);
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
            return constantType == ConstantType.Boolean;
        }
    }
}
