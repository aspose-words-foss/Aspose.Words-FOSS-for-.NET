// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2015 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// The result type for functions that handle nodes.
    /// </summary>
    internal enum HandleNodeAction
    {
        NotHandled,
        /// <summary>
        /// The reader needs to process all children elements of the current element and also needs to process the eng tag of this element.
        /// </summary>
        HandledTraverseChildren,
        /// <summary>
        /// If this value is returned, it indicates the processing of the element is finished.
        /// The reader should skip all children of this element and the processing of the element end will not be invoked.
        /// </summary>
        HandledSkipChildren
    }
}