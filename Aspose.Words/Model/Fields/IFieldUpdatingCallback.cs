// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2021 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implement this interface if you want to have your own custom methods called during a field update.
    /// </summary>
    public interface IFieldUpdatingCallback
    {
        /// <summary>
        /// A user defined method that is called just before a field is updated.
        /// </summary>
        void FieldUpdating(Field field);

        /// <summary>
        /// A user defined method that is called just after a field is updated.
        /// </summary>
        void FieldUpdated(Field field);
    }
}
