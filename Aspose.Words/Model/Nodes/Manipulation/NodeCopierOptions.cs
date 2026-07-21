// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/07/2018 by Alexander Zhiltsov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Represents on/off options for the <see cref="NodeCopier"/> class.
    /// </summary>
    [Flags]
    internal enum NodeCopierOptions
    {
        /// <summary>
        /// No defined options.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates whether to replace every destination start node ancestor's properties with the corresponding
        /// source ones in case when any ancestor of the same type can be met while a source range traversal.
        /// </summary>
        UseSourceStartAncestorPr = 1,
        /// <summary>
        /// Specifies whether cross structure annotation nodes should be copied.
        /// See <see cref="NodeUtil.IsCrossStructureAnnotation(Node)"/> for details.
        /// </summary>
        SkipCrossStructureAnnotations = 2,
        /// <summary>
        /// When defined, then node need to be cloned while extracting it within the enumerated range.
        /// </summary>
        CloneNode = 4,
        /// <summary>
        /// If set, when the range has block level annotation nodes at bounds, copying gives same result as when
        /// the annotation nodes are moved to inline level.
        /// </summary>
        ProcessBoundBlockAnnotationAsInline = 8,
        /// <summary>
        /// Specifies whether the range start headers and footers should be prolongated (cloned) during reference node separation.
        /// </summary>
        ProlongRangeStartSectionHeadersFooters = 16
    }
}
