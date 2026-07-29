// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented by a descendant, applies a field result represented by different objects.
    /// </summary>
    internal abstract class FieldResultApplier
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="field"></param>
        protected FieldResultApplier(Field field)
            : this(field, true)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="applyFormat"></param>
        protected FieldResultApplier(Field field, bool applyFormat)
        {
            Field = field;
            mApplyFormat = applyFormat;
        }

        /// <summary>
        /// Performs all actions required to apply a field result: removes old result, applies new result
        /// and applies result format.
        /// </summary>
        internal void ApplyResult()
        {
            // Obtain format applier.
            IFieldResultFormatApplier formatApplier = mApplyFormat
                ? Field.Format.GetFieldResultFormatProvider().GetFormatApplier()
                : null;

            // Forcibly insert field separator here.
            Field.EnsureSeparator(true);

            // Remove the old result.
            Field.RemoveFieldResult();

            // Apply the new result.
            ApplyResultCore();

            // Apply result format.
            if (formatApplier != null)
                formatApplier.ApplyFormat(Field.GetFieldResultRange());
        }

        /// <summary>
        /// Actually applies the result.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void ApplyResultCore();

        protected Field Field { get; }

        private readonly bool mApplyFormat;
    }
}
