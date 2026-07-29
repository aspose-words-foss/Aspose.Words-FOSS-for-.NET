// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2006 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Parses an expression to a queue of execution items.
    /// </summary>
    internal class ExpressionParser
    {
        private ExpressionParser(FieldContext fieldContext, IExpressionParserBehavior builder)
        {
            mFieldContext = fieldContext;
            mBuilder = builder;
        }

        /// <summary>
        /// Parses an expression to a queue of execution items.
        /// </summary>
        /// <returns>A queue of execution items or null if an error has been encountered.</returns>
        internal static ExecutionQueue Parse(FieldContext fieldContext, string expression, IExpressionParserBehavior builder)
        {
            ExpressionParser parser = new ExpressionParser(fieldContext, builder);
            return parser.Parse(expression);
        }

        private ExecutionQueue Parse(string exp)
        {
            List<object> expression = ParseExpression(exp);
            if (expression == null)
                return null;

            expression = mBuilder.NormalizeExpression(expression, mFieldContext);
            if (expression == null)
                return null;

            expression = JoinSequencedUnaryMinusOperators(expression);

            ExecutionQueue queue = BuildExecutionQueue(expression);
            if (queue == null)
                return null;

            return mBuilder.NormalizeExecutionQueue(queue, mFieldContext, exp);
        }

        private static List<object> JoinSequencedUnaryMinusOperators(List<object> expression)
        {
            List<object> result = new List<object>(expression.Count);

            int i = 0;
            while (i < expression.Count)
            {
                if (IsUnaryMinusOperatorAtIndex(expression, i) && IsUnaryMinusOperatorAtIndex(expression, i + 1))
                {
                    // Skip both minus operators
                    i += 2;
                    continue;
                }

                result.Add(expression[i]);
                i++;
            }

            return result;
        }

        private static bool IsUnaryMinusOperatorAtIndex(List<object> expression, int index)
        {
            object item = index < expression.Count
                ? expression[index]
                : null;

            return item is UnaryMinusOperator;
        }

        private List<object> ParseExpression(string expression)
        {
            mExpressionContext = new ExpressionContext(expression);
            List<object> result = new List<object>();
            bool isFirstOperator = true;

            while (!mExpressionContext.IsEof)
            {
                // Skip an optional equality sign at the beginning of the expression.
                if ((mExpressionContext.Position == 0) && (mExpressionContext.CurrentChar == '='))
                {
                    mExpressionContext.NextChar();

                    // WORDSNET-5301 Conditional field is "< =" instead of "<=" in expression.
                    if (mExpressionContext.IsEof)
                        return null;
                }

                mExpressionContext.SkipWhitespace();

                if (mExpressionContext.IsEof || mExpressionContext.CurrentChar == '\\')
                    break;

                Operator @operator = ParseOperator(isFirstOperator);
                if (@operator != null)
                {
                    result.Add(@operator);
                    isFirstOperator = !@operator.IsRightParenthesis;
                }
                else
                {
                    IExecutionItem operand = ParseOperand();
                    result.Add(operand);
                    isFirstOperator = false;
                }
            }

            return result;
        }

        private ExecutionQueue BuildExecutionQueue(List<object> expression)
        {
            mExecutionQueue = new ExecutionQueue();
            mDeferredExecutionQueue = new ExecutionQueue();
            mOperatorStack = new OperatorStack();
            mDeferExecutionItems = false;

            if (expression.Count > 0)
            {
                Operator @operator = expression[0] as Operator;
                if (@operator != null && expression[0] is IExecutionItem && !@operator.IsUnary)
                    return null;
            }

            foreach (object item in expression)
            {
                Operator @operator = item as Operator;
                if (@operator != null)
                {
                    if (!ProcessOperator(@operator))
                        return null;

                    continue;
                }

                IExecutionItem executionItem = item as IExecutionItem;
                if (executionItem != null)
                {
                    ProcessOperand(executionItem);
                    continue;
                }

                Debug.Assert(false);
            }

            // If execution items are deferred at this point then being parsed expression has invalid syntax,
            // because of unclosed parenthesis.
            if (mDeferExecutionItems)
                return null;

            // There is no need to call ProcessDeferredExecutionQueue forcibly because the only way
            // deferred execution queue may be not empty at this point is that operator stack is not empty.
            // But this case is handled below.
            while (mOperatorStack.Count > 0)
            {
                Operator @operator = mOperatorStack.Pop();

                // if @operator doesn't implement IExecutionItem, then being parsed expression has invalid syntax.
                // E.g. mOperatorStack can contain instances of LeftParenthesisOperator or RightParenthesisOperator
                // (both of these inheritors of Operator do not implement IExecutionItem interface) at this stage,
                // which means that the expression contains extra parentheses.
                // return null, that indicates syntax error.
                IExecutionItem executionItem = @operator as IExecutionItem;
                if (executionItem != null)
                {
                    EnqueueExecutionItem(executionItem);
                }
                else
                {
                    return null;
                }
            }

            return mExecutionQueue;
        }

        /// <summary>
        /// Parses an operator at the current expression position.
        /// </summary>
        /// <param name="isFirstOperator">True to indicate this is the first operator after an operand.</param>
        /// <returns>An <see cref="Operator"/> implementation or null if no operator has been recognized.</returns>
        private Operator ParseOperator(bool isFirstOperator)
        {
            if (mExpressionContext.CompareAndAdvanceIfSuccessful("+"))
                return new AdditionOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("-"))
            {
                if (isFirstOperator)
                    return new UnaryMinusOperator();
                else
                    return new SubtractionOperator();
            }

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("%"))
                return new UnaryPercentOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("*"))
                return new MultiplicationOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("/"))
                return new DivisionOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("^"))
                return new ExponentiationOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("="))
                return new EqualityOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful(">="))
                return new GreaterThanOrEqualToOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful(">"))
                return new GreaterThanOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("<="))
                return new LessThanOrEqualToOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("<>"))
                return new InequalityOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("<"))
                return new LessThanOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful("("))
                return new LeftParenthesisOperator();

            if (mExpressionContext.CompareAndAdvanceIfSuccessful(")"))
                return new RightParenthesisOperator();

            return null;
        }

        private bool ProcessOperator(Operator @operator)
        {
            if (@operator.IsLeftParenthesis)
            {
                // If execution items are deferred already then deferred execution items must be flushed because parentheses
                // may be interpreted as unary minus operator only when contain nothing but double constant optionally in
                // conjunction with unary non-minus operators, that is they can not contain nested parentheses in this case.
                if (mDeferExecutionItems)
                    ProcessDeferredExecutionQueue();
                else
                    mDeferExecutionItems = true;
            }

            if ((mOperatorStack.Count == 0) || (@operator.IsLeftParenthesis))
            {
                mOperatorStack.Push(@operator);
                return true;
            }

            if (@operator.IsRightParenthesis)
            {
                while (mOperatorStack.Count > 0)
                {
                    Operator stackOperator = mOperatorStack.Pop();
                    if (!stackOperator.IsLeftParenthesis)
                    {
                        IExecutionItem executionItem = stackOperator as IExecutionItem;

                        if (executionItem != null)
                            EnqueueExecutionItem(executionItem);
                        else
                            return false;
                    }
                    else
                    {
                        if (mDeferExecutionItems)
                        {
                            // Parentheses are interpreted as unary minus operator, so add it.
                            mDeferExecutionItems = false;
                            EnqueueExecutionItem(new UnaryMinusOperator(true));
                        }

                        return true;
                    }
                }

                return true;
            }

            // Parentheses can not contain not unary or unary minus operator to be interpreted as unary minus operator.
            if (mDeferExecutionItems && (!@operator.IsUnary || @operator.IsUnaryMinus))
                mDeferExecutionItems = false;

            while (mOperatorStack.Count > 0)
            {
                if (@operator.Order >= mOperatorStack.Peek().Order)
                    EnqueueExecutionItem((IExecutionItem)mOperatorStack.Pop());
                else
                    break;
            }

            mOperatorStack.Push(@operator);
            return true;
        }

        private IExecutionItem ParseOperand()
        {
            string operand = GetSubstringBeforeDelimiter();

            Function functionItem = ParseFunction(operand);
            if (functionItem != null)
            {
                // WORDSNET-13860 Whitespace between function name and opening parenthesis.
                mExpressionContext.SkipWhitespace();

                if ((!mExpressionContext.IsEof) && (mExpressionContext.CurrentChar == '('))
                    ParseFunctionParameters(functionItem);

                return functionItem;
            }

            // A numeric constant?
            IExecutionItem executionItem = ParseConstant(operand);
            if (executionItem != null)
                return executionItem;

            // Nothing worked above, so treat it as a string constant.
            return new StringConstant(operand);
        }

        private void ProcessOperand(IExecutionItem operand)
        {
            // Parentheses can not contain not double constant operand to be interpreted as unary minus operator.
            if (mDeferExecutionItems && !(operand is DoubleConstant))
                mDeferExecutionItems = false;

            EnqueueExecutionItem(operand);
        }

        private void EnqueueExecutionItem(IExecutionItem executionItem)
        {
            if (mDeferExecutionItems)
            {
                mDeferredExecutionQueue.Enqueue(executionItem);
            }
            else
            {
                ProcessDeferredExecutionQueue();
                mExecutionQueue.Enqueue(executionItem);
            }
        }

        /// <summary>
        /// Flushes all deferred execution items to execution queue.
        /// </summary>
        private void ProcessDeferredExecutionQueue()
        {
            while (mDeferredExecutionQueue.Count != 0)
            {
                mExecutionQueue.Enqueue(mDeferredExecutionQueue.Dequeue());
            }
        }

        /// <summary>
        /// Tries to parse the specified operand string to a function.
        /// </summary>
        /// <param name="operand">The operand string.</param>
        /// <returns>The parsed function or null if the operand is not a function.</returns>
        private static Function ParseFunction(string operand)
        {
            switch (operand.ToUpper())
            {
                case "ABS":
                    return new AbsFunction();
                case "AND":
                    return new AndFunction();
                case "AVERAGE":
                    return new AverageFunction();
                case "COUNT":
                    return new CountFunction();
                case "DEFINED":
                    return new DefinedFunction();
                case "FALSE":
                    return new FalseFunction();
                case "IF":
                    return new IfFunction();
                case "INT":
                    return new IntFunction();
                case "MAX":
                    return new MaxFunction();
                case "MIN":
                    return new MinFunction();
                case "MOD":
                    return new ModFunction();
                case "NOT":
                    return new NotFunction();
                case "OR":
                    return new OrFunction();
                case "PRODUCT":
                    return new ProductFunction();
                case "ROUND":
                    return new RoundFunction();
                case "SIGN":
                    return new SignFunction();
                case "SUM":
                    return new SumFunction();
                case "TRUE":
                    return new TrueFunction();
                default:
                    return null;
            }
        }

        private void ParseFunctionParameters(Function function)
        {
            bool noString = true;
            int nestingLevel = 0;

            char listSeparator = FormatterPal.GetListSeparatorCurrent();
            char numberGroupSeparator = FormatterPal.GetNumberGroupSeparatorCurrent();

            char[] allSpecialChars = new char[4];
            allSpecialChars[0] = '"';
            allSpecialChars[1] = '(';
            allSpecialChars[2] = ')';
            allSpecialChars[3] = listSeparator;

            bool checkNumbers = listSeparator == numberGroupSeparator;

            char[] quoteChar = { '"' };

            mExpressionContext.NextChar();
            int startIndex = mExpressionContext.Position;
            string numberParameter = string.Empty;
            char[] specialChars = allSpecialChars;
            while (!mExpressionContext.IsEof)
            {
                // tries to find a next special char
                if (!mExpressionContext.FindAny(specialChars))
                {
                    break;
                }

                switch (mExpressionContext.CurrentChar)
                {
                    case '"':
                        {
                            // all special chars before the next quote are ignored, so we don't need to look for them
                            specialChars = noString ? quoteChar : allSpecialChars;
                            noString = !noString;
                            break;
                        }
                    case '(':
                        {
                            nestingLevel++;
                            break;
                        }
                    case ')':
                        {
                            if (nestingLevel == 0)
                            {
                                if (checkNumbers)
                                {
                                    numberParameter = ProcessParameter(function, startIndex, numberParameter);
                                }
                                else
                                {
                                    EvaluateAndAddFunctionParameter(function, startIndex);
                                }

                                mExpressionContext.NextChar();
                                if (StringUtil.HasChars(numberParameter))
                                {
                                    EvaluateAndAddFunctionParameter(function, numberParameter);
                                }

                                return;
                            }
                            else
                            {
                                nestingLevel--;
                            }

                            break;
                        }
                    // in case of any list separator chars
                    default:
                        {
                            if (nestingLevel == 0)
                            {
                                if (checkNumbers)
                                {
                                    numberParameter = ProcessParameter(function, startIndex, numberParameter);
                                }
                                else
                                {
                                    EvaluateAndAddFunctionParameter(function, startIndex);
                                }

                                startIndex = mExpressionContext.Position + 1;
                            }
                            break;
                        }
                }

                mExpressionContext.NextChar();
            }

            if (StringUtil.HasChars(numberParameter))
            {
                EvaluateAndAddFunctionParameter(function, numberParameter);
            }

            EvaluateAndAddFunctionParameter(function, startIndex);
        }

        /// <summary>
        /// Checks whether a parsed parameter is a next part of number and process it.
        /// (if decimal group separator equals to list separator.)
        /// </summary>
        /// <param name="function">A function which parameter is belong to.</param>
        /// <param name="startIndex">A position in the being parsed expression where parameter start.</param>
        /// <param name="numberParameter">A number parameter that is parsed before.</param>
        /// <returns>A number parameter.</returns>
        private string ProcessParameter(Function function, int startIndex, string numberParameter)
        {
            string parameterExpression = mExpressionContext.Expression.Substring(startIndex, mExpressionContext.Position - startIndex);
            FunctionParameter functionParameter = new FunctionParameter(parameterExpression);
            // check whether parameter is not a number
            if (!functionParameter.IsNumber)
            {
                // WORDSNET-19391 Handle a situation when list separator == group separator and the expression continues.
                if (functionParameter.ContainsOperator && functionParameter.NumberOfDigitsAtStart == 3)
                    return ConcatenateNumberAndFunctionParameters(numberParameter, functionParameter);

                if (StringUtil.HasChars(numberParameter))
                    EvaluateAndAddFunctionParameter(function, numberParameter);

                EvaluateAndAddFunctionParameter(function, functionParameter.ParameterValue);
                return string.Empty;
            }

            // check whether parameter isn't a next number part but a start of next number.
            if (functionParameter.IsSpaceBefore || functionParameter.IsCurrencyAtStart ||
                functionParameter.IsNegative || functionParameter.LengthOfIntegerPart < 3)
            {
                if (StringUtil.HasChars(numberParameter))
                    EvaluateAndAddFunctionParameter(function, numberParameter);

                numberParameter = functionParameter.ParameterValue;
            }
            else
            {
                numberParameter = ConcatenateNumberAndFunctionParameters(numberParameter, functionParameter);
            }

            // check whether parameter is a last part of number
            if (functionParameter.IsCurrencyAtEnd || functionParameter.IsFractional || functionParameter.IsSpaceAfter)
            {
                EvaluateAndAddFunctionParameter(function, numberParameter);
                return string.Empty;
            }

            return numberParameter;
        }

        private static string ConcatenateNumberAndFunctionParameters(string numberParameter, FunctionParameter functionParameter)
        {
            string separator = (StringUtil.ContainsOnlyWhitespaces(numberParameter) ||
                                StringUtil.ContainsOnlyWhitespaces(functionParameter.ParameterValue))
                ? string.Empty
                : FormatterPal.GetNumberGroupSeparatorCurrent().ToString();

            return string.Concat(numberParameter, separator, functionParameter.ParameterValue);
        }

        private void EvaluateAndAddFunctionParameter(Function function, int startIndex)
        {
            string parameterExpression = mExpressionContext.Expression.Substring(startIndex, mExpressionContext.Position - startIndex).Trim();
            EvaluateAndAddFunctionParameter(function, parameterExpression);
        }

        private void EvaluateAndAddFunctionParameter(Function function, string parameterExpression)
        {
            // Try to parse a cell reference.
            if (function.IsAggregate)
            {
                IExecutionItem cellReference = CellReference.TryParse(mFieldContext, parameterExpression);
                if (cellReference != null)
                {
                    function.Parameters.Add(cellReference);
                    return;
                }
            }

            Constant parameter = ExpressionEvaluator.EvaluateExpression(mFieldContext, parameterExpression);
            function.Parameters.Add(parameter);
        }

        /// <summary>
        /// Tries to parse the specified operand string to a constant.
        /// </summary>
        /// <param name="operand">The operand to parse.</param>
        /// <returns>The parsed constant or null if the operand is not a constant.</returns>
        private static IExecutionItem ParseConstant(string operand)
        {
            return DoubleConstant.TryParse(operand);
        }

        /// <summary>
        /// Returns a substring of the expression string starting at the current position and ending at the index of
        /// any of the delimiters or at the end of the string if no delimiters were encountered. Advances
        /// the position as appropriate.
        /// </summary>
        /// <returns>The substring.</returns>
        private string GetSubstringBeforeDelimiter()
        {
            StringBuilder builder = new StringBuilder();
            bool whiteSpace = false;
            bool normal = false;

            while (mExpressionContext.Position < mExpressionContext.Expression.Length)
            {
                char c = mExpressionContext.CurrentChar;
                CharType charType = GetCharType(c, builder);

                bool breakLoop = false;
                switch (charType)
                {
                    case CharType.Normal:
                        if (!whiteSpace)
                        {
                            normal = true;
                            builder.Append(c);
                        }
                        else
                            breakLoop = true;
                        break;
                    case CharType.WhiteSpace:
                        if (normal)
                            whiteSpace = true;
                        break;
                    case CharType.Delimiter:
                        breakLoop = true;
                        break;
                    case CharType.Currency:
                        builder.Append(c);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (breakLoop)
                    break;

                mExpressionContext.NextChar();
            }

            return builder.ToString().Trim();
        }

        private CharType GetCharType(char c, StringBuilder builder)
        {
            if (char.IsWhiteSpace(c))
            {
                char groupSeparator = FormatterPal.GetNumberGroupSeparatorCurrent();
                if (c != groupSeparator)
                    return CharType.WhiteSpace;
            }

            if (IsDelimiter(c, builder))
                return CharType.Delimiter;

            // TODO: multiple char currency support
            string currencySymbol = FormatterPal.GetCurrencySymbolCurrent();
            if ((currencySymbol.Length == 1) && (c == currencySymbol[0]))
                return CharType.Currency;

            return CharType.Normal;
        }

        /// <summary>
        /// Checks if the specified character is a delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <param name="builder"></param>
        /// <returns>True if the character is a delimiter.</returns>
        private bool IsDelimiter(char c, StringBuilder builder)
        {
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                case '>':
                case '<':
                case '=':
                case '%':
                case '(':
                case ')':
                case '\\':
                    return true;
                default:
                    if (mBuilder.IsDelimiter(c, mExpressionContext.Expression, mExpressionContext.Position, builder) ||
                        IsCultureDependentDelimiter(c))
                    {
                        mExpressionContext.NextChar();
                        return true;
                    }

                    return false;
            }
        }

        private static bool IsCultureDependentDelimiter(char c)
        {
            if ((c != '.') && (c != ','))
                return false;

            char decimalSeparator = FormatterPal.GetDecimalSeparatorCurrent();
            char groupSeparator = FormatterPal.GetNumberGroupSeparatorCurrent();

            return (decimalSeparator != c) && (groupSeparator != c);
        }

        private ExpressionContext mExpressionContext;
        private readonly FieldContext mFieldContext;
        private readonly IExpressionParserBehavior mBuilder;
        private ExecutionQueue mExecutionQueue;
        private ExecutionQueue mDeferredExecutionQueue;
        private OperatorStack mOperatorStack;
        private bool mDeferExecutionItems;

        private enum CharType
        {
            Normal,
            WhiteSpace,
            Delimiter,
            Currency
        }
    }
}
