// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2017 by Andrey Noskov

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Specifies the possible values for the location of a single custom XML element's start and/or end
    /// tag relative to the location of an annotation tag in document order.
    /// </summary>
    internal enum DisplacedByType
    {
        /// <summary>
        /// Specifies that this annotation anchor shall be displaced
        /// by the physical representation of the next element of
        /// custom XML markup in the document.
        /// </summary>
        Next,

        /// <summary>
        /// Specifies that this annotation anchor shall be displaced
        /// by the physical representation of the previous element
        /// of custom XML markup in the document.
        /// </summary>
        Prev,

        /// <summary>
        /// Means that location is not specified.
        /// </summary>
        Unspecified,
    }
}
