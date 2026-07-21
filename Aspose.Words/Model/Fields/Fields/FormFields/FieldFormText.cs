// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2010 by Roman Korchagin

using Aspose.Common;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the FORMTEXT field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts a text box style form field.
    /// </remarks>
    public class FieldFormText : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            FormField formField = Start.FormField;

            // WORDSNET-8618 we assume that document is not correctly formed
            if (formField == null)
                return null;

            switch (formField.TextInputType)
            {
                case TextFormFieldType.CurrentDate:
                case TextFormFieldType.CurrentTime:
                {
                    // Current date and time format is stored in the inner field code and the result is formatted
                    // when the inner field is updated so we only need to copy the value into the outer form field.
                    if (UpdateContext.FirstChild == null)
                        return BuildDefaultTextFieldUpdateAction(formField);

                    string result = UpdateContext.FirstChild.Field.Result;
                    result = ApplyMaxLength(result, formField.MaxLength);
                    return new FieldUpdateActionApplyResult(this, result, false);
                }
                case TextFormFieldType.Calculated:
                {
                    FieldFormula fieldFormula = UpdateContext.FirstChild != null
                        ? UpdateContext.FirstChild.Field as FieldFormula
                        : null;

                    string stringResult = FieldCodeCache.GetArgumentAsString(InnerFieldArgumentIndex, true, true);
                    double doubleResult = double.NaN;

                    // WORDSNET-12301 we should accept FORMULA field result directly as double.
                    if (fieldFormula != null)
                    {
                        Constant formulaResult = fieldFormula.Evaluate();

                        // WORDSNET-14703 MS Word does not pass errors from formula field.
                        if (formulaResult is ErrorConstant)
                            return BuildDefaultTextFieldUpdateAction(formField);

                        if (formulaResult is DoubleConstant)
                        {
                            doubleResult = formulaResult.ValueDouble;
                        }
                        else if (formulaResult is StringConstant)
                        {
                            stringResult = formulaResult.ValueString;
                        }
                    }

                    if (double.IsNaN(doubleResult))
                        doubleResult = FormatterPal.TryParseDoubleInvariant(stringResult);

                    if (!double.IsNaN(doubleResult))
                    {
                        FieldOptions fieldOptions = FetchDocument().FieldOptions;
                        NumberFormattingOptions formattingOptions =
                            fieldOptions.GetNumberFormattingOptions() |
                            NumberFormattingOptions.IgnoreUnmatchedDigitPlaceholder |
                            NumberFormattingOptions.IsMultiplyPercent;
                        stringResult = FormatterPal.NumberToStrMSWord(
                            doubleResult,
                            formField.TextInputFormat,
                            formattingOptions);
                    }

                    return new FieldUpdateActionApplyResult(this, stringResult, false);
                }
                case TextFormFieldType.Regular:
                case TextFormFieldType.Number:
                {
                    string result = formField.TextInputDefault;
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = ApplyMaxLength(result, formField.MaxLength);

                        return new FieldUpdateActionApplyResult(this, result, false);
                    }

                    return BuildDefaultTextFieldUpdateAction(formField);
                }
                default:
                {
                    // All other text input types are not calculated.
                    return null;
                }
            }
        }

        private NodeRange BuildDefaultTextInputResultRange(int maxLength)
        {
            string result = ApplyMaxLength(FormField.DefaultTextInputValue, maxLength);

            Paragraph para = new Paragraph(Document);

            foreach (char c in result)
                para.AppendChild(new SpecialChar(Document, c, new RunPr()));

            return new NodeRange(para.FirstChild, para.LastChild);
        }

        private FieldUpdateAction BuildDefaultTextFieldUpdateAction(FormField formField)
        {
            NodeRange resultRange = BuildDefaultTextInputResultRange(formField.MaxLength);
            return new FieldUpdateActionApplyResult(this, new NodeRangeFieldResult(resultRange));
        }

        private static string ApplyMaxLength(string result, int maxLength)
        {
            if (string.IsNullOrEmpty(result))
                return result;

            if (maxLength <= 0)
                return result;

            if (maxLength >= result.Length)
                return result;

            return result.Substring(0, maxLength);
        }

        internal override void BeforeUnlink()
        {
            FormField formField = Start.FormField;
            if (formField == null)
                return;

            BookmarkStart bookmarkStart = formField.BookmarkStart;
            if (bookmarkStart == null)
                return;

            Start.InsertPrevious(bookmarkStart);
        }

        internal override FieldResultApplier BuildResultApplier(string result)
        {
            if (result != FormField.DefaultTextInputValue)
                return base.BuildResultApplier(result);

            NodeRange resultRange = BuildDefaultTextInputResultRange(0);
            return new NodeRangeResultApplier(this, resultRange);
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int InnerFieldArgumentIndex = 0;
    }
}
