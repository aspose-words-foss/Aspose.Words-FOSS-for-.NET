// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/12/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Applies a field result, whether by updating a real result or replacing the whole field with updated result.
    /// </summary>
    internal class FieldUpdateActionApplyResult : FieldUpdateAction
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, string result)
            : this(field, result, true)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, string result, bool formatResult)
            : this(
                field,
                (result != null ? new FieldResult(new StringConstant(result)) : null),
                formatResult)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, Constant result)
            : this(field, result, true)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, Constant result, bool formatResult)
            : this(
                field,
                (result != null ? new FieldResult(result) : null),
                formatResult)
        {
        }


        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, IFieldResult result)
            : this(field, result, true)
        {
        }

        internal FieldUpdateActionApplyResult(Field field, string result, ParagraphBreakCharReplacement paragraphBreakCharReplacement)
            : this(field, result)
        {
            mParagraphBreakCharReplacement = paragraphBreakCharReplacement;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldUpdateActionApplyResult(Field field, IFieldResult result, bool formatResult)
            : base(field)
        {
            mResult = result;
            mFormatResult = formatResult;
        }

        internal override void Perform()
        {
            FieldResultApplier resultApplier = GetResultApplier();
            resultApplier.ApplyResult();

            Field.UpdateContext.JoinOldResultNodes();
        }

        private FieldResultApplier GetResultApplier()
        {
            if (mResult != null)
            {
                Constant result = mResult.GetFieldResultValue();
                string resultString = result.ValueString;

                if (resultString == null)
                    return new TextResultApplier(Field, string.Empty);

                FieldFormattingResult formattingResult = new FieldFormattingResult(resultString);
                bool isFormatted = mFormatResult && Field.Format.FormatResult(result, out formattingResult);

                NodeRange resultRange = mResult.GetFieldResultRange();
                if ((resultRange == null) || isFormatted)
                {
                    // Source formatting is unavailable or overridden by a field switch, use a text representation
                    // of the calculated result.
                    return new TextResultApplier(Field, formattingResult, mParagraphBreakCharReplacement);
                }
                else
                {
                    // Copy formatted content.
                    return new NodeRangeResultApplier(Field, resultRange);
                }
            }
            else
            {
                // Set empty result.
                return new TextResultApplier(Field, string.Empty);
            }
        }

        private readonly IFieldResult mResult;
        private readonly bool mFormatResult;
        private readonly ParagraphBreakCharReplacement mParagraphBreakCharReplacement;
    }
}
