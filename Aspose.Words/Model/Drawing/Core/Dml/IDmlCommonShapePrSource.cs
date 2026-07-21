// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/05/2015 by Andrey Noskov

using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents common shape properties for 
    /// <see cref="DmlPicture"/>, 
    /// <see cref="DmlShape"/>, 
    /// <see cref="DmlChartSpace"/>, 
    /// <see cref="DmlDiagram"/>,
    /// <see cref="DmlContentPart"/>.
    /// </summary>
    internal interface IDmlCommonShapePrSource
    {
        DmlFill Fill { get; set; }
        DmlOutline Outline { get; set; }
        DmlShapeStyle Style { get; set; }
    }
}
