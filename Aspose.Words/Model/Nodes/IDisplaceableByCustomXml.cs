// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/06/2018 by Alexander Zhiltsov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Common interface for nodes which position can be displaced by custom XML (structured document tag).
    /// </summary>
    internal interface IDisplaceableByCustomXml
    {
        /// <summary>
        /// Specifies that placement of the annotation node is directly linked with the location of the physical
        /// presentation of a custom XML element in the document.
        /// </summary>
        DisplacedByType DisplacedByCustomXml { get; set; }
    }
}
