// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2009 by Dmitry Vorobyev

using System;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Compares two expressions. Used when evaluating the IF and COMPARE fields.
    /// Comparison expressions seem to be handled differently in these fields than those in formula fields.
    /// For example, they can include strings enclosed in double quotes.
    /// </summary>
    internal class ComparisonEvaluator
    {
        private ComparisonEvaluator(Field field, ComparisonExpressionBag expression)
        {
            mExpression = expression;
            mFieldContext = new FieldContext(field);
        }

        internal static Constant Evaluate(Field field, IComparisonExpression expression)
        {
            IComparisonExpressionEvaluator customEvaluator = field.FetchDocument().FieldOptions.ComparisonExpressionEvaluator;
            if (customEvaluator != null)
            {
                ComparisonEvaluationResult result = customEvaluator.Evaluate(
                    field,
                    new ComparisonExpression(
                        expression.LeftExpression,
                        expression.ComparisonOperator,
                        expression.RightExpression));

                if (result != null)
                {
                    return string.IsNullOrEmpty(result.ErrorMessage)
                        ? (Constant)new BooleanConstant(result.Result)
                        : new ErrorConstant(result.ErrorMessage);
                }
            }

            ComparisonEvaluator comparisonEvaluator = new ComparisonEvaluator(
                field,
                new ComparisonExpressionBag(expression));

            return comparisonEvaluator.EvaluateCore();
        }

        private Constant EvaluateCore()
        {
            if (mExpression.LeftExpression == null)
                return new ErrorConstant("Error! Missing test condition.");

            if (mExpression.ComparisonOperator == null)
                return new BooleanConstant(true);

            if (!IsOperatorValid(mExpression.ComparisonOperator))
                return new ErrorConstant("Error! Unknown op code for conditional.");

            if (mExpression.RightExpression == null)
                return new ErrorConstant("Error! Missing second part of test condition.");

            Constant rightResult = null;
            Constant leftResult = null;

            if (mExpression.IsLeftExpressionInQuotationMarks && mExpression.IsRightExpressionInQuotationMarks)
            {
                leftResult = new StringConstant(mExpression.LeftExpression);
                rightResult = new StringConstant(mExpression.RightExpression);
            }
            else if (mExpression.IsLeftExpressionInQuotationMarks)
            {
                leftResult = new StringConstant(mExpression.LeftExpression);
                rightResult = ResolveBookmark(mExpression.RightExpression);
            }
            else if (mExpression.IsRightExpressionInQuotationMarks)
            {
                leftResult = ResolveBookmark(mExpression.LeftExpression);
                if (leftResult != null)
                {
                    rightResult = new StringConstant(mExpression.RightExpression);
                }
                else
                {
                    leftResult = ExpressionEvaluator.EvaluateReferenceExpression(
                        mFieldContext,
                        mExpression.LeftExpression,
                        false);

                    rightResult = ExpressionEvaluator.EvaluateReferenceExpression(
                        mFieldContext,
                        mExpression.RightExpression);
                }
            }
            else
            {
                ReferenceExpressionResult leftResultInfo =
                    ResolveBookmarkOrEvaluateReferenceExpression(mExpression.LeftExpression);
                ReferenceExpressionResult rightResultInfo =
                    ResolveBookmarkOrEvaluateReferenceExpression(mExpression.RightExpression);

                if ((leftResultInfo == null) || !leftResultInfo.HasLeadingMinusOperator ||
                    (rightResultInfo == null) || !rightResultInfo.HasLeadingMinusOperator)
                {
                    if (leftResultInfo != null)
                        leftResult = leftResultInfo.Result;

                    if (rightResultInfo != null)
                        rightResult = rightResultInfo.Result;
                }
            }

            leftResult = ValidateResult(leftResult, mExpression.LeftExpression);
            rightResult = ValidateResult(rightResult, mExpression.RightExpression);

            bool isLeftResultStringConstant = leftResult is StringConstant;
            bool isRightResultStringConstant = rightResult is StringConstant;
            if (isLeftResultStringConstant || isRightResultStringConstant)
            {
                string leftOperand = isLeftResultStringConstant ? leftResult.ValueString : mExpression.LeftExpression;
                string rightOperand = isRightResultStringConstant ? rightResult.ValueString : mExpression.RightExpression;
                bool areWildcardsAllowed = mExpression.IsRightExpressionInQuotationMarks ||
                                           string.IsNullOrEmpty(rightOperand) ||
                                           !char.IsDigit(rightOperand[0]);
                return new BooleanConstant(CompareStrings(leftOperand, rightOperand, areWildcardsAllowed));
            }

            return new BooleanConstant(CompareDoubles(leftResult.ValueDouble, rightResult.ValueDouble));
        }

        private ReferenceExpressionResult ResolveBookmarkOrEvaluateReferenceExpression(string expression)
        {
            StringConstant bookmark = ResolveBookmark(expression);
            if (bookmark != null)
                expression = bookmark.ValueString;

            ReferenceExpressionResult result = ExpressionEvaluator.EvaluateReferenceExpressionWithInfo(
                mFieldContext,
                expression,
                true);

            if (result != null)
                return result;

            if (bookmark != null)
                return new ReferenceExpressionResult(bookmark, false);

            return null;
        }

        private static Constant ValidateResult(Constant constant, string expression)
        {
            return IsValidConstant(constant)
                ? constant
                : new StringConstant(expression);
        }

        private static bool IsValidConstant(Constant constant)
        {
            if (constant == null)
                return false;

            if (constant.IsError)
                return false;

            if (constant is BooleanConstant)
                return false;

            return true;
        }

        private StringConstant ResolveBookmark(string bookmarkName)
        {
            Bookmark bookmark = FieldUtil.GetCachedBookmark(mFieldContext.Field, bookmarkName);
            return bookmark != null
                ? new StringConstant(bookmark.GetText(true))
                : null;
        }

        private bool CompareStrings(string leftOperand, string rightOperand, bool areWildcardsAllowed)
        {
            // The comparison operator should be '=' or '<>' in order to use wildcards.
            areWildcardsAllowed = areWildcardsAllowed && (IsEqualityOperator || IsInequalityOperator);

            // OOXML states that wildcards are only allowed in the left-hand expression, but Word (as well as its help)
            // shows only the right-hand expression may have them.
            bool hasWildcards = (rightOperand.IndexOf(WordSingleWildcard) != -1) || (rightOperand.IndexOf(WordWildcard) != -1);

            if (!areWildcardsAllowed || !hasWildcards)
            {
                // WORDSNET-4640 Use the StringSort order for comparing.
                return CompareDoubles(StringUtil.CompareStringSort(leftOperand, rightOperand), 0);
            }

            // WORDSNET-5692 Replacing self-written wildcard search algorithm with Regex-based one.
            // We don't use static Regex method here as we don't want this Regex to be cached because its pattern is dynamic.
            Regex regex = new Regex(GetRegexPattern(rightOperand, leftOperand));
            return regex.IsMatch(leftOperand) == IsEqualityOperator;
        }

        /// <summary>
        /// Gets Regex pattern by word wildcard search pattern.
        /// </summary>
        /// <param name="wordPattern">word wildcard search pattern</param>
        /// <param name="leftOperand"></param>
        /// <returns>Regex pattern</returns>
        private static string GetRegexPattern(string wordPattern, string leftOperand)
        {
            StringBuilder builder = new StringBuilder(RegexStringStartDesignator);
            string workWordPattern = wordPattern;
            bool needLeaveLoop = false;
            int loopCount = 0;
            bool isFirstWildcard = true;
            int wordPatternLength = wordPattern.Length;
            int firstWildcardIndex = workWordPattern.IndexOf(WordWildcard);

            // If firstWildcard is in the center of the word (not first or last symbol)
            if ((firstWildcardIndex > 0) && (firstWildcardIndex < wordPatternLength - 1))
            {
                // If after * there is any symbol and length of the leftOperand equal length of the part wordPattern before * then add symbol ?
                // that regex always will be false
                if (leftOperand.Length == firstWildcardIndex)
                {
                    workWordPattern = workWordPattern.Replace(WordWildcard, WordSingleWildcard);
                }
                // Word ignores only one symbol after Wildcard - equal simple *
                else if (firstWildcardIndex == wordPatternLength - 2)
                {
                    workWordPattern = workWordPattern.Remove(wordPatternLength - 1);
                }
                // WildcardIndex and index of last symbol leftOperand are the same
                else if (firstWildcardIndex == leftOperand.Length - 1)
                {
                    // All symbols after Wildcard Word ignores in this case - remove these symbols for correct regex
                    workWordPattern = workWordPattern.Remove(firstWildcardIndex + 1);
                }
                // Create specific regex
                else
                {
                    // If WildcardIndex > 0 need specific logic
                    if (firstWildcardIndex > 1)
                    {
                        // Word ignores some symbols after *. Symbols count is count of symbols before *
                        // Example cvb*dfd equal cvb* or cvb*dfhgjkl equal cvb*gjkl
                        int removeCount = workWordPattern.Length - 1 - firstWildcardIndex;
                        if (removeCount > firstWildcardIndex)
                            removeCount = firstWildcardIndex;

                        workWordPattern = workWordPattern.Remove(firstWildcardIndex + 1, removeCount);
                    }
                    else
                    {
                        int leftOperandSymbolsAfterWildcardCount = leftOperand.Length - firstWildcardIndex - 1;
                        int wordPatternSymbolsAfterWildcardCount = workWordPattern.Length - firstWildcardIndex - 1;
                        // Get length of the part wordPattern after * - length of the part wordPattern after * position
                        int removeCount = wordPatternSymbolsAfterWildcardCount - leftOperandSymbolsAfterWildcardCount;

                        // First symbol after * always doesn't matter
                        if (removeCount <= 0)
                            removeCount = 1;

                        // Create strange regex
                        workWordPattern = workWordPattern.Remove(firstWildcardIndex + 1, removeCount);
                    }
                }

            }

            // Prepare Regex
            foreach (char c in workWordPattern)
            {
                switch (c)
                {
                    case WordWildcard:
                        // Second and other "*" is simple symbols (not Wildcard)
                        if (isFirstWildcard)
                        {
                            builder.Append(RegexWildcard);
                            isFirstWildcard = false;
                        }
                        else
                        {
                            builder.Append(RegexUnicodeDesignator);
                            builder.Append(FormatterPal.IntToStrX4(c));
                        }

                        // Only if wildcard at the end of string need leave loop
                        needLeaveLoop = loopCount == wordPatternLength - 1;
                        break;
                    case WordSingleWildcard:
                        builder.Append(RegexSingleWildcard);
                        break;
                    default:
                        builder.Append(RegexUnicodeDesignator);
                        builder.Append(FormatterPal.IntToStrX4(c));
                        break;
                }
                loopCount++;
                if (needLeaveLoop)
                    break;
            }

            builder.Append(RegexStringEndDesignator);
            return builder.ToString();
        }

        private bool CompareDoubles(double leftOperand, double rightOperand)
        {
            switch (mExpression.ComparisonOperator)
            {
                case "=":
                    return (leftOperand == rightOperand);
                case "<>":
                    return (leftOperand != rightOperand);
                case ">":
                    return (leftOperand > rightOperand);
                case "<":
                    return (leftOperand < rightOperand);
                case ">=":
                    return (leftOperand >= rightOperand);
                case "<=":
                    return (leftOperand <= rightOperand);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static bool IsOperatorValid(string comparisonOperator)
        {
            switch (comparisonOperator)
                {
                    case "=":
                    case "<>":
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                        return true;
                    default:
                        return false;
                }
        }

        private bool IsEqualityOperator
        {
            get { return mExpression.ComparisonOperator == "="; }
        }

        private bool IsInequalityOperator
        {
            get { return mExpression.ComparisonOperator == "<>"; }
        }

        private readonly ComparisonExpressionBag mExpression;
        private readonly FieldContext mFieldContext;

        /// <summary>
        /// Word zero or more characters wildcard.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char WordWildcard = '*';

        /// <summary>
        /// Word single character wildcard.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char WordSingleWildcard = '?';

        /// <summary>
        /// Regex zero or more characters wildcard.
        /// </summary>
        private const string RegexWildcard = ".*";

        /// <summary>
        /// Regex single character wildcard.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char RegexSingleWildcard = '.';

        /// <summary>
        /// Regex unicode character code designator.
        /// </summary>
        private const string RegexUnicodeDesignator = @"\u";

        /// <summary>
        /// Regex "must occur on string start" assertion.
        /// </summary>
        private const string RegexStringStartDesignator = @"\A";

        /// <summary>
        /// Regex "must occur on string end" assertion.
        /// </summary>
        private const string RegexStringEndDesignator = @"\z";

        private class ComparisonExpressionBag
        {
            internal ComparisonExpressionBag(IComparisonExpression arg)
            {
                FieldArgument leftArgument = arg.LeftExpressionArgument;
                if (leftArgument != null)
                {
                    LeftExpression = leftArgument.GetNormalizedText(true);
                    IsLeftExpressionInQuotationMarks = IsInQuotationMarks(
                        arg.LeftExpression,
                        arg.LeftExpressionArgument);
                }
                else
                {
                    // Left expression argument may be missed, i.e. IF = "right" "true" "false".
                    LeftExpression = arg.LeftExpression;
                }

                ComparisonOperator = arg.ComparisonOperator;

                FieldArgument rightArgument = arg.RightExpressionArgument;
                if (rightArgument != null)
                {
                    RightExpression = rightArgument.GetNormalizedText(true);
                    IsRightExpressionInQuotationMarks = IsInQuotationMarks(
                        arg.RightExpression,
                        arg.RightExpressionArgument);
                }
                else
                {
                    // Left expression argument may be missed, i.e. IF "right" "true" "false".
                    RightExpression = arg.RightExpression;
                }
            }

            private static bool IsInQuotationMarks(string expression, FieldArgument argument)
            {
                if (!WordUtil.IsInQuotationMarks(expression))
                    return false;

                if (FieldUtil.EndsWithFieldEnd(argument.Range))
                    return false;

                return true;
            }

            internal string LeftExpression { get; }
            internal bool IsLeftExpressionInQuotationMarks { get; }
            internal string ComparisonOperator { get; }
            internal string RightExpression { get; }
            internal bool IsRightExpressionInQuotationMarks { get; }

        }
    }
}
