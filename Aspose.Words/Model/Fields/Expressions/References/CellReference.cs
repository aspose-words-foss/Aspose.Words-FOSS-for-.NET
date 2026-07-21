// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/01/2010 by Dmitry Vorobyev

using System.Text;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a reference to a table cell.
    /// </summary>
    internal class CellReference : IExecutionItem
    {
        private CellReference(ICellRange range, FieldContext context, bool isAggregateFunctionParameter)
        {
            mRange = range;
            mContext = context;
            mIsAggregateFunctionParameter = isAggregateFunctionParameter;
        }

        internal static IExecutionItem TryParse(FieldContext context, string referenceName)
        {
            IExecutionItem reference = TryParseAsOneDimensionRange(context, referenceName);
            return reference != null
                ? reference
                : TryParseAsRectangularRange(context, referenceName, true);
        }

        /// <summary>
        /// Attempts to parse a cell reference represented by a one dimension range (ABOVE, BELOW, LEFT or RIGHT).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="referenceName"></param>
        /// <returns></returns>
        private static IExecutionItem TryParseAsOneDimensionRange(FieldContext context, string referenceName)
        {
            ICellRange range;

            Constant result = OneDimensionCellRange.TryParse(context, referenceName, out range);
            if (result != null)
                return result;

            return range != null
                ? new CellReference(range, context, false)
                : null;
        }

        /// <summary>
        /// Attempts to parse a cell reference represented by a rectangular range (A1, A1:B2, C:C, 3:3 etc).
        /// </summary>
        internal static IExecutionItem TryParseAsRectangularRange(
            FieldContext context,
            string referenceName,
            bool isAggregateFunctionParameter)
        {
            ICellRange range;

            Constant result = RectangularCellRange.TryParse(context, referenceName, isAggregateFunctionParameter, out range);
            if (result != null)
                return result;

            return range != null
                ? new CellReference(range, context, isAggregateFunctionParameter)
                : null;
        }

        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            return mRange.IsOneCell
                ? EvaluateSingleCell()
                : EvaluateMultipleCells();
        }

        private Constant EvaluateSingleCell()
        {
            if (mRange.Start.Cell != null)
            {
                DoubleConstant cellValue = GetCellValue(mRange.Start.Cell, true).Value;
                if (cellValue != null)
                    return cellValue;

                return mIsAggregateFunctionParameter
                    ? (Constant)NullConstant.Instance
                    : new DoubleConstant(0d);
            }

            if (mIsAggregateFunctionParameter)
                return NullCellReference.Instance;

            return new ErrorConstant(string.Format("!{0} Is Not In Table", mRange.Start));
        }


        private Constant EvaluateMultipleCells()
        {
            EvaluationState state = new EvaluationState();
            AggregateConstant result = new AggregateConstant();

            foreach (Cell cell in mRange)
            {
                if (!TryEvaluateCellInRange(cell, result, state))
                    break;
            }

            return result;
        }

        private bool TryEvaluateCellInRange(Cell cell, AggregateConstant result, EvaluationState state)
        {
            CellValue cellValue = GetCellValue(cell, !state.IsParsedConstantReached);
            state.IsParsedConstantReached = state.IsParsedConstantReached || cellValue.IsParsedConstant;

            if (cellValue.Value != null)
            {
                if (!state.IsStarted)
                {
                    result.Values.Add(cellValue.Value);

                    state.IsStarted = true;
                    state.StartsWithEndNonDigits = cellValue.Value.HasEndNonDigits;
                    state.PreviousValueHasAllDigits = !state.StartsWithEndNonDigits;

                    return true;
                }

                if (cellValue.Value.HasEndNonDigits)
                {
                    if (state.PreviousValueHasAllDigits)
                        return false;

                    result.Values.Add(cellValue.Value);
                }
                else
                {
                    result.Values.Add(cellValue.Value);

                    state.PreviousValueHasAllDigits = true;
                }
            }
            else if (!mRange.IsRectangular)
            {
                if (!state.IsStarted || state.StartsWithEndNonDigits)
                    result.Values.Add(mRange.EmptyCellValue);
                else
                    return false;
            }

            return true;
        }

        private CellValue GetCellValue(Cell cell, bool evaluateCellText)
        {
            if (!cell.HasChildNodes)
                return CellValue.Empty;

            StringBuilder builder = new StringBuilder();
            NodeCollection cellParagraphs = cell.GetChildNodes(NodeType.Paragraph, true);
            foreach (Paragraph paragraph in cellParagraphs)
            {
                if (!paragraph.HasChildNodes)
                    continue;

                string paragraphText = NodeTextCollector.GetText(paragraph.FirstChild, true, paragraph.LastChild, true, true);
                builder.Append(paragraphText).Append(' ');
            }

            string cellText = builder.ToString();

            DoubleConstant cellValue = DoubleConstant.TryParse(cellText);
            if (cellValue != null)
                return new CellValue(cellValue, true);

            if (evaluateCellText || mRange.AlwaysEvaluateCellText)
                cellValue = ExpressionEvaluator.EvaluateReferenceExpression(mContext, cellText);

            return new CellValue(cellValue, false);
        }

        private readonly ICellRange mRange;
        private readonly FieldContext mContext;
        private readonly bool mIsAggregateFunctionParameter;

        private class CellValue
        {
            internal static readonly CellValue Empty = new CellValue(null, false);

            internal CellValue(DoubleConstant value, bool isParsedConstant)
            {
                Value = value;
                IsParsedConstant = isParsedConstant;
            }

            internal DoubleConstant Value { get; }

            internal bool IsParsedConstant { get; }
        }

        private class EvaluationState
        {
            internal bool IsStarted { get; set; }
            internal bool StartsWithEndNonDigits { get; set; }
            internal bool PreviousValueHasAllDigits { get; set; }
            internal bool IsParsedConstantReached { get; set; }
        }
    }
}
