// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2005 by Roman Korchagin

using System;
using System.Text.RegularExpressions;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the IF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Compares the values designated by the expressions <see cref="LeftExpression"/> and <see cref="RightExpression"/>
    /// in comparison using the operator designated by <see cref="ComparisonOperator"/>.</p>
    /// <p>A field in the following format will be used as a mail merge source: { IF 0 = 0 "{PatientsNameFML}" "" \* MERGEFORMAT }</p>
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class FieldIf : Field, IMergeFieldSurrogate
    {
        internal override FieldUpdateAction UpdateCore()
        {
            EnsureComparisonResult();

            FieldUpdateAction action = !mComparisonResult.IsError
                ? (FieldUpdateAction)new FieldUpdateActionApplyResult(this, ResultArgument)
                : new FieldUpdateActionInsertErrorMessage(this, mComparisonResult.ValueString);

            InvalidateComparisonResult(); // Comparison result should not be used on next update if any.

            return action;
        }

        internal override FieldUpdateStrategy GetChildFieldsUpdateStrategyInArgument(IFieldArgument argument)
        {
            switch (UpdateContext.ChildUpdateStage)
            {
                case FieldChildUpdateStage.Permanent:
                    return (argument != TrueResultArgument) && (argument != FalseResultArgument)
                        ? FieldUpdateStrategy.Update
                        : FieldUpdateStrategy.Defer;

                case FieldChildUpdateStage.Conditional:
                    if ((argument != TrueResultArgument) && (argument != FalseResultArgument))
                        return FieldUpdateStrategy.Defer;

                    EnsureComparisonResult();

                    return !mComparisonResult.IsError && (argument == ResultArgument)
                        ? FieldUpdateStrategy.Update
                        : FieldUpdateStrategy.Reject;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal override void NotifyChildFieldUpdated(IFieldArgument argument)
        {
            FieldIfComparisonExpression comparisonExpression = GetComparisonExpression();
            if ((argument == comparisonExpression.LeftExpressionArgument) || (argument == comparisonExpression.RightExpressionArgument))
                InvalidateComparisonResult();

            base.NotifyChildFieldUpdated(argument);
        }

        internal override void ParseFieldCode()
        {
            // WORDSNET-12193 Ensures separator before parse field code.
            EnsureSeparator(true);

            base.ParseFieldCode();
        }

        private void EnsureComparisonResult()
        {
            if (mComparisonResult != null)
                return;

            mComparisonResult = EvaluateComparisonResult();
        }

        private void InvalidateComparisonResult()
        {
            mComparisonResult = null;
        }

        private Constant EvaluateComparisonResult()
        {
            IComparisonExpression comparison = GetComparisonExpression();
            return ComparisonEvaluator.Evaluate(this, comparison);
        }

        private FieldIfComparisonExpression GetComparisonExpression()
        {
            return new FieldIfComparisonExpression(FieldCodeCache);
        }

        string IMergeFieldSurrogate.GetMergeFieldName()
        {
            string fieldName = TrueResultArgument.GetNormalizedText();
            return WordUtil.TrimDoubleQuotes(fieldName).TrimStart('{').TrimEnd('}');
        }

        bool IMergeFieldSurrogate.CanWorkAsMergeField()
        {
            return false;
        }

        bool IMergeFieldSurrogate.IsMergeValueRequired()
        {
            return false;
        }

        internal override bool SupportsConditionalUpdate
        {
            get { return !IsInHeaderFooter; }
        }

        private FieldArgument ResultArgument
        {
            get
            {
                Debug.Assert(!mComparisonResult.IsError);

                return mComparisonResult.ValueBoolean ? TrueResultArgument : FalseResultArgument;
            }
        }

        internal FieldArgument TrueResultArgument
        {
            get
            {
                FieldIfComparisonExpression comparisonExpression = GetComparisonExpression();
                return comparisonExpression.TrueResultArgument;
            }
        }

        internal FieldArgument FalseResultArgument
        {
            get
            {
                FieldIfComparisonExpression comparisonExpression = GetComparisonExpression();
                return comparisonExpression.FalseResultArgument;
            }
        }

        /// <summary>
        /// Gets or sets the left part of the comparison expression.
        /// </summary>
        public string LeftExpression
        {
            get { return FieldCodeCache.GetArgumentAsString(LeftExpressionArgumentIndex, false); }
            set { FieldCodeCache.SetArgument(LeftExpressionArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        public string ComparisonOperator
        {
            get { return FieldCodeCache.GetArgumentAsString(ComparisonOperatorArgumentIndex); }
            set { FieldCodeCache.SetArgument(ComparisonOperatorArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the right part of the comparison expression.
        /// </summary>
        public string RightExpression
        {
            get { return FieldCodeCache.GetArgumentAsString(RightExpressionArgumentIndex, false); }
            set { FieldCodeCache.SetArgument(RightExpressionArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the text displayed if the comparison expression is true.
        /// </summary>
        public string TrueText
        {
            get { return FieldCodeCache.GetArgumentAsString(TrueTextArgumentIndex); }
            set { FieldCodeCache.SetArgument(TrueTextArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the text displayed if the comparison expression is <c>false</c>.
        /// </summary>
        public string FalseText
        {
            get { return FieldCodeCache.GetArgumentAsString(FalseTextArgumentIndex); }
            set { FieldCodeCache.SetArgument(FalseTextArgumentIndex, value); }
        }

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <returns>
        /// A <see cref="FieldIfComparisonResult"/> value that represents the result of the condition evaluation.
        /// </returns>
        public FieldIfComparisonResult EvaluateCondition()
        {
            Constant result = EvaluateComparisonResult();
            if (result.IsError)
                return FieldIfComparisonResult.Error;
            return result.ValueBoolean
                ? FieldIfComparisonResult.True
                : FieldIfComparisonResult.False;
        }

        internal override INodeModifier GetResultModifier()
        {
            return new FieldIfHyperlinkFormattingModifier(FetchDocument());
        }

        /// <summary>
        /// Represents result of comparison.
        /// </summary>
        private Constant mComparisonResult;
        /// <summary>
        /// Index of true result argument at field code.
        /// </summary>
        private const int TrueResultArgumentIndex = 3;
        /// <summary>
        /// Index of false result argument at field code.
        /// </summary>
        private const int FalseResultArgumentIndex = 4;

        private const int LeftExpressionArgumentIndex = 0;
        private const int ComparisonOperatorArgumentIndex = 1;
        private const int RightExpressionArgumentIndex = 2;
        private const int TrueTextArgumentIndex = 3;
        private const int FalseTextArgumentIndex = 4;

        private class FieldIfComparisonExpression : IComparisonExpression
        {
            internal FieldIfComparisonExpression(FieldCode fieldCode)
            {
                FieldIfArgumentsBundle defaultArguments = new FieldIfArgumentsBundle(
                    fieldCode.GetArgument(LeftExpressionArgumentIndex),
                    fieldCode.GetArgumentAsString(LeftExpressionArgumentIndex, false),
                    fieldCode.GetArgumentAsString(ComparisonOperatorArgumentIndex, false),
                    fieldCode.GetArgument(RightExpressionArgumentIndex),
                    fieldCode.GetArgumentAsString(RightExpressionArgumentIndex, false),
                    fieldCode.GetArgument(TrueResultArgumentIndex),
                    fieldCode.GetArgument(FalseResultArgumentIndex));

                mArguments = TryMissedLeftExpression(fieldCode, defaultArguments)
                          ?? TryMissedOperatorAndRightExpression(fieldCode, defaultArguments)
                          ?? defaultArguments;
            }

            public FieldArgument LeftExpressionArgument
            {
                get { return mArguments.LeftExpressionArgument; }
            }

            public string LeftExpression
            {
                get { return mArguments.LeftExpression; }
            }

            public string ComparisonOperator
            {
                get { return mArguments.ComparisonOperator; }
            }

            public FieldArgument RightExpressionArgument
            {
                get { return mArguments.RightExpressionArgument; }
            }

            public string RightExpression
            {
                get { return mArguments.RightExpression; }
            }

            internal FieldArgument TrueResultArgument
            {
                get { return mArguments.TrueResultArgument; }
            }

            internal FieldArgument FalseResultArgument
            {
                get { return mArguments.FalseResultArgument; }
            }

            /// <summary>
            /// FieldIf with missed left expression (IF = "Right" "True Text (optional)" "False Text (optional)") update to (IF "" = "" "Right" "Right")
            /// </summary>
            private static FieldIfArgumentsBundle TryMissedLeftExpression(FieldCode fieldCode, FieldIfArgumentsBundle defaultArguments)
            {
                if (fieldCode.Arguments.Count < 2)
                    return null;

                if (ComparisonEvaluator.IsOperatorValid(defaultArguments.ComparisonOperator))
                    return null;

                if (!ComparisonEvaluator.IsOperatorValid(defaultArguments.LeftExpression))
                    return null;

                return new FieldIfArgumentsBundle(
                    null,
                    string.Empty,
                    "=",
                    null,
                    string.Empty,
                    fieldCode.GetArgument(1),
                    fieldCode.GetArgument(1));
            }

            /// <summary>
            /// FieldIf with missed operator and right expression (IF "Right" "True Text" "False Text (optional)") update to (IF "" = "" "Right" "Right")
            /// </summary>
            private static FieldIfArgumentsBundle TryMissedOperatorAndRightExpression(FieldCode fieldCode, FieldIfArgumentsBundle defaultArguments)
            {
                if (fieldCode.Arguments.Count < 2)
                    return null;

                if (ComparisonEvaluator.IsOperatorValid(defaultArguments.ComparisonOperator))
                    return null;

                if (gMatchNumber.IsMatch(defaultArguments.LeftExpression))
                    return null;

                return new FieldIfArgumentsBundle(
                    null,
                    string.Empty,
                    "=",
                    null,
                    string.Empty,
                    fieldCode.GetArgument(1),
                    fieldCode.GetArgument(1));
            }

            private readonly FieldIfArgumentsBundle mArguments;

            private static readonly Regex gMatchNumber = new Regex(@"^[0-9]+$", RegexOptions.Compiled);

            private class FieldIfArgumentsBundle
            {
                internal FieldIfArgumentsBundle(
                    FieldArgument leftExpressionArgument,
                    string leftExpression,
                    string comparisonOperator,
                    FieldArgument rightExpressionArgument,
                    string rightExpression,
                    FieldArgument trueResultArgument,
                    FieldArgument falseResultArgument)
                {
                    LeftExpression = leftExpression;
                    ComparisonOperator = comparisonOperator;
                    RightExpression = rightExpression;
                    TrueResultArgument = trueResultArgument;
                    FalseResultArgument = falseResultArgument;
                    LeftExpressionArgument = leftExpressionArgument;
                    RightExpressionArgument = rightExpressionArgument;
                }

                internal FieldArgument LeftExpressionArgument { get; }

                internal string LeftExpression { get; }

                internal string ComparisonOperator { get; }

                internal FieldArgument RightExpressionArgument { get; }

                internal string RightExpression { get; }

                internal FieldArgument TrueResultArgument { get; }

                internal FieldArgument FalseResultArgument { get; }
            }
        }
    }
}
