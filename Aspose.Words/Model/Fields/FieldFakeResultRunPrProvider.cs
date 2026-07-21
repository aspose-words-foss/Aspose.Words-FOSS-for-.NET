// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2013 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides fake result run properties in a common way for the majority of fields.
    /// </summary>
    internal class FieldFakeResultRunPrProvider : IFieldRunPrProvider
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldFakeResultRunPrProvider(Field field)
        {
            mField = field;
        }

        RunPr IFieldRunPrProvider.GetRunPr()
        {
            // Fake result run properties should be fully expanded since they are to be applied to runs
            // which do not belong to the model.
            return mField.Start.GetExpandedRunPr(RunPrExpandFlags.Layout);
        }

        private readonly Field mField;
    }
}
