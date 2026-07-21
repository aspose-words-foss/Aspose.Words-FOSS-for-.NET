// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/07/2006 by Roman Korchagin

using System;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies how a group shape should be edited in Microsoft Word.
    /// </summary>
    [CppEnumEnableMetadata]
    internal enum EditAs
    {
        /// <summary>
        /// The group shape is a normal group of shapes.
        /// Not in DOC files, just our own value.
        /// </summary>
        Group = -1,
        /// <summary>
        /// The group shape is a canvas.
        /// </summary>
        Canvas = 0,
        /// <summary>
        /// The group shape is an organizational chart.
        /// </summary>
        OrganizationalChart = 1,
        /// <summary>
        /// The group shape is a Radial diagram.
        /// </summary>
        Radial = 2,
        /// <summary>
        /// The group shape is a Cycle diagram.
        /// </summary>
        Cycle = 3,
        /// <summary>
        /// The group shape is stacked pyramid diagram.
        /// </summary>
        Pyramid = 4,
        /// <summary>
        /// The group shape is a Venn diagram.
        /// </summary>
        Venn = 5,
        /// <summary>
        /// The group shape is a target diagram.
        /// </summary>
        Target = 6
    }
}
