// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// When implemented, represents a formula function.
    /// </summary>
    internal abstract class Function : IExecutionItem
    {
        protected Function()
        {
            Parameters = new ExecutionItemCollection();
        }

        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            if ((Parameters.Count < ParameterCountMin) || (Parameters.Count > ParameterCountMax))
                return ErrorConstant.CreateSyntaxError();

            ConstantCollection evaluatedParameters = new ConstantCollection();

            foreach (IExecutionItem parameter in Parameters)
            {
                Constant evaluatedParameter = parameter.Evaluate(null);

                if ((evaluatedParameter.IsError) && !IsDefinedFunction)
                    return evaluatedParameter;

                // WORDSNET-10008 If parameters can not be parsed as double, we should return bookmark error.
                if ((evaluatedParameter.ConstantType == ConstantType.String) && !(IsParameterTypeValid(ConstantType.String)))
                    return ErrorConstant.CreateBookmarkError(evaluatedParameter.ValueString);

                evaluatedParameters.Add(evaluatedParameter);
            }

            return EvaluateCore(evaluatedParameters);
        }

        protected abstract Constant EvaluateCore(ConstantCollection parameters);

        /// <summary>
        /// Gets whether this function accepts an aggregate argument.
        /// </summary>
        internal virtual bool IsAggregate
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether this is DefinedFunction.
        /// Created to fix SonarCube issue.
        /// </summary>
        internal virtual bool IsDefinedFunction
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the minimum number of function parameters.
        /// </summary>
        protected abstract int ParameterCountMin { get; }
        /// <summary>
        /// Gets the maximum number of function parameters.
        /// </summary>
        protected abstract int ParameterCountMax { get; }

        /// <summary>
        /// Gets a collection of function parameters.
        /// </summary>
        internal ExecutionItemCollection Parameters { get; }

        /// <summary>
        /// Checks parameters for validity
        /// </summary>
        protected abstract bool IsParameterTypeValid(ConstantType constantType);
    }
}
