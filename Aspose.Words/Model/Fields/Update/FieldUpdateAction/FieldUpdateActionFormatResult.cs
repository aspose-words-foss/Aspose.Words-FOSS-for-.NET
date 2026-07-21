// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2010 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// This class is used to format field result only. It is used in fields whose result is formed directly in
    /// the UpdateCore method.
    /// </summary>
    internal class FieldUpdateActionFormatResult : FieldUpdateAction
    {
        internal FieldUpdateActionFormatResult(Field field)
            : base(field)
        {
            mFormatApplier = Field.Format.GetFieldResultFormatProvider().GetFormatApplier();
        }

        internal override void Perform()
        {
            // First try to format result as a number, date etc.
            // If unsuccessful, only apply general formatting.
            FieldFormattingResult formattingResult;
            StringConstant result = new StringConstant(Field.Result);

            if (Field.Format.FormatResult(result, out formattingResult))
            {
                TextResultApplier resultApplier = new TextResultApplier(Field, formattingResult);
                resultApplier.ApplyResult();
            }
            else
            {
                // Apply result format.
                if (mFormatApplier != null)
                    mFormatApplier.ApplyFormat(Field.GetFieldResultRange());
            }

            Field.UpdateContext.JoinOldResultNodes();
        }

        private readonly IFieldResultFormatApplier mFormatApplier;
    }
}
