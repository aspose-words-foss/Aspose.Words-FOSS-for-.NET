// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/01/2015 by Andrey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    internal class DmlOutlinePropertiesDefaults : DmlHierarchicalPropertyBag
    {
        private DmlOutlinePropertiesDefaults()
        {
            SetProperty((int)DmlOutlinePropertiesIds.LineEndingCapType, EndCap.Flat);
            SetProperty((int)DmlOutlinePropertiesIds.LineJoinStyle, JoinStyle.Round);
            SetProperty((int)DmlOutlinePropertiesIds.WidthInEmus, 0.0);
            SetProperty((int)DmlOutlinePropertiesIds.CompoundLineType, ShapeLineStyle.Single);
            SetProperty((int)DmlOutlinePropertiesIds.StrokeAlignment, false);
            SetProperty((int)DmlOutlinePropertiesIds.Dash, new DmlPresetDash());
            SetProperty((int)DmlOutlinePropertiesIds.LineMiterLimit, 0.0);
            SetProperty((int)DmlOutlinePropertiesIds.Fill, new DmlNoFill());
            SetProperty((int)DmlOutlinePropertiesIds.HeadLineEndStyle, new DmlHeadLineEndStyle());
            SetProperty((int)DmlOutlinePropertiesIds.TailLineEndStyle, new DmlTailLineEndStyle());
        }

        internal static DmlOutlinePropertiesDefaults Instance
        {
            get { return gInstance; }
        }

        private static readonly DmlOutlinePropertiesDefaults gInstance = new DmlOutlinePropertiesDefaults();
    }
}
