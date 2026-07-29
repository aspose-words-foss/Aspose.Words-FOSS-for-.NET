// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2014 by Anton Savko

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    internal class MarkupLanguageShapeWriter
    {
        internal static bool IsShapeFloating(ShapeBase shapeBase)
        {
            return (shapeBase.WrapType == WrapType.None);
        }

        internal static bool IsShapeRenderable(ShapeBase shapeBase)
        {
            // Non-top level shapes will be rendered as parts of corresponding group shapes.
            return shapeBase.IsTopLevel && !shapeBase.Hidden && !shapeBase.Font.Hidden;
        }
    }
}
