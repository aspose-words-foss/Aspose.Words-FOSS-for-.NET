// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2022 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides data for the field updating progress event.
    /// </summary>
    public sealed class FieldUpdatingProgressArgs
    {
        internal FieldUpdatingProgressArgs(int totalFieldsCount, int updatedFieldsCount, bool updateCompleted)
        {
            TotalFieldsCount = totalFieldsCount;
            UpdatedFieldsCount = updatedFieldsCount;
            UpdateCompleted = updateCompleted;
        }

        /// <summary>
        /// Gets the total fields count to be updated.
        /// </summary>
        /// <remarks>
        /// The value is not constant and may be increased during updating process.
        /// </remarks>
        public int TotalFieldsCount { get; }

        /// <summary>
        /// Gets the number of updated fields.
        /// </summary>
        public int UpdatedFieldsCount { get; }

        /// <summary>
        /// Gets a value indicating whether field updating is completed.
        /// </summary>
        public bool UpdateCompleted { get; }
    }
}
