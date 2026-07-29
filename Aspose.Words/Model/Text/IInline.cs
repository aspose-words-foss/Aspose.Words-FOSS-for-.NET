// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Implemented by nodes that represent inline content
    /// (nodes that occur inside paragraphs and basically exclude bookmarks).
    /// </summary>
    internal interface IInline : IRunAttrSource
    {
        /// <summary>
        /// Gets the immediate parent paragraph.
        /// Can be null if the node is not inserted into the document.
        /// </summary>
        Paragraph ParentParagraph_IInline { get; }

        /// <summary>
        /// Gets the owner document. Cannot be null.
        /// </summary>
        DocumentBase Document_IInline { get; }

        /// <summary>
        /// These methods have such ugly names to avoid name clashes during autoporting to Java.
        /// </summary>
        RunPr GetExpandedRunPr_IInline(RunPrExpandFlags flags);

        /// <summary>
        /// These methods have such ugly names to avoid name clashes during autoporting to Java.
        /// </summary>
        RunPr RunPr_IInline { get; set; }
    }
}
