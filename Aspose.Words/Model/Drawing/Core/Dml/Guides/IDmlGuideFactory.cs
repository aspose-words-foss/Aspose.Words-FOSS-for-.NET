// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/11/2010 by Alexey Titov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Guides
{
    internal interface IDmlGuideFactory
    {
        DmlGuide CreateGuide(string formula, string name, bool isPreset);

        /// <summary>
        /// Initializes common guides.
        /// </summary>
        /// <remarks>
        /// Described in 5.1.12.56 ST_ShapeType (Preset Shape Types)
        /// </remarks>
        IList<DmlGuide> CreateCommonGuides();
    }
}