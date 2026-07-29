// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Drawing.Core.Dml
{
    internal interface IDmlShapePrSource
    {
        DmlTransform Transform { get; set; }
        BWMode BWMode { get; set; }
        DmlGeometry Geometry { get; set; }
        DmlFill Fill { get; set; }
        DmlOutline Outline { get; set; }
        DmlShapeStyle Style { get; set; }
        DmlShapeEffectsCollection Effects { get; set; }
        DmlScene3DProperties Scene3DProperties { get; set; }
        DmlShape3DProperties Shape3DProperties { get; set; }
        StringToObjDictionary<DmlExtension> SpPrExtensions { get; set; }
    }
}
