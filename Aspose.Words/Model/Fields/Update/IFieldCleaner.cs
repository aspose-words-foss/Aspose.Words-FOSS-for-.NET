// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/06/2016 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Performs cleanup actions during fields update process.
    /// </summary>
    internal interface IFieldCleaner
    {
        /// <summary>
        /// Removes empty paragraph according to <see cref="FieldUpdateCleanupActions.RemoveContainingParagraphIfEmpty"/>.
        /// </summary>
        /// <returns>Returns a value indicating if the paragraph was actually removed.</returns>
        bool RemoveEmptyParagraph(Paragraph paragraph);

        /// <summary>
        /// Removes field code according to <see cref="FieldUpdateCleanupActions.RemoveContainingFieldCode"/>.
        /// </summary>
        /// <returns>Returns a value indicating if the field code was actually removed.</returns>
        [JavaThrows(true)]
        bool RemoveFieldCode(FieldUpdateContext context);

        /// <summary>
        /// Performs postponed removals.
        /// </summary>
        [JavaThrows(true)]
        void FinalizeCleanup();
    }
}
