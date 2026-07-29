// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

using System;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// When implemented, represents a formula constant.
    /// </summary>
    internal abstract class Constant : IExecutionItem
    {
        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            // Just return itself.
            return this;
        }

        /// <summary>
        /// Attempts to format the constant's value using numeric format. Returns a formatted string
        /// if successful, null otherwise.
        /// </summary>
        /// <param name="format">Number format.</param>
        /// <param name="field">Field context.</param>
        [JavaThrows(true)]
        internal virtual FieldFormattingResult TryFormatNumber(RichString format, Field field)
        {
            return null;
        }

        /// <summary>
        /// Attempts to format the constant's value using date/time format. Returns a formatted string
        /// if successful, null otherwise.
        /// </summary>
        internal virtual string TryFormatDateTime(string format, int eastAsianLanguageId, IFieldResultFormatter resultFormatter)
        {
            return null;
        }

        internal virtual bool TryConvertToDouble(out double value)
        {
            value = 0;
            return false;
        }

        /// <summary>
        /// Gets the value of the constant as a number.
        /// </summary>
        internal virtual double ValueDouble
        {
            get { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets the value of the constant as true or false.
        /// </summary>
        internal virtual bool ValueBoolean
        {
            get { throw new InvalidOperationException(); }
        }
        /// <summary>
        /// Gets the value of the constant as a text string.
        /// </summary>
        internal virtual string ValueString
        {
            get { throw new InvalidOperationException(); }
        }

        internal bool IsError
        {
            get { return ConstantType == ConstantType.Error; }
        }

        /// <summary>
        /// Gets the type of the constant.
        /// </summary>
        internal abstract ConstantType ConstantType { get; }
    }
}
