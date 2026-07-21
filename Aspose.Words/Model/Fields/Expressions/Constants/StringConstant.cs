// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is a text string.
    /// </summary>
    internal class StringConstant : Constant
    {
        internal StringConstant(string value)
        {
            mValue = value;
        }

        internal override FieldFormattingResult TryFormatNumber(RichString format, Field field)
        {
            DoubleConstant value = ExpressionEvaluator.EvaluateReferenceExpression(new FieldContext(field), mValue);
            return value != null
                ? value.TryFormatNumber(format, field)
                : null;
        }

        internal override string TryFormatDateTime(string format, int eastAsianLanguageId, IFieldResultFormatter resultFormatter)
        {
            DateTimeConstant value = DateTimeConstant.TryParse(mValue);
            return value != null
                ? value.TryFormatDateTime(format, eastAsianLanguageId, resultFormatter)
                : null;
        }

        internal override bool TryConvertToDouble(out double value)
        {
            DoubleConstant constant = DoubleConstant.TryParse(mValue);
            if (constant != null)
            {
                value = constant.ValueDouble;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        internal override string ValueString
        {
            get { return mValue; }
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.String; }
        }

        private readonly string mValue;
    }
}
