// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2023 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Implement this interface if you want to control when certain <see cref="Revision"/> should be accepted/rejected
    /// or not by the <see cref="RevisionCollection.Accept"/>/<see cref="RevisionCollection.Reject"/> methods.
    /// </summary>
    public interface IRevisionCriteria
    {
        /// <summary>
        /// Checks whether or not specified <paramref name="revision"/> matches criteria.
        /// </summary>
        /// <param name="revision">The <see cref="Revision"/> instance to match criteria.</param>
        /// <returns><c>True</c> if the <paramref name="revision"/> matches criteria, otherwise <c>False</c>.</returns>
        /// <remarks>
        /// The method implementation should not accept/reject the revision or modify it in any way due to unexpected results.
        /// </remarks>
        [JavaThrows(true)]
        bool IsMatch(Revision revision);
    }
}
